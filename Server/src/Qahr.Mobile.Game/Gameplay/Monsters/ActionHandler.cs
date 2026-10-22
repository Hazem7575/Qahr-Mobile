using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.ServerSockets;
using Qahr.Mobile.Role.Instance;

namespace Qahr.Mobile.Game.MsgMonster
{
    public class ActionHandler
    {
        public System.MSRandom Random = new System.MSRandom();
        public Random SystemRandom = new Random();

        public bool Rate(int value)
        {
            return value > Random.Next() % 100;
        }

        public void ExecuteAction(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster.Family.ID == 3100 || monster.Family.ID == 3103 || monster.Family.ID == 40121 || monster.Family.ID == 40122 || monster.Family.ID == 40123 || monster.Family.ID == 40124)
                return;
            if (global::Qahr.Mobile.Phone.MobilePassiveAggro.Skip(monster, client)) { monster.State = MobStatus.Idle; return; } // QahrMobile: passive monster (attack_user=0) only fights a player who hit it, see MobilePassiveAggro.cs
            switch (monster.State)
            {
                case MobStatus.Idle:
                    {
                        monster.Target = null;
                        CheckTarget(client, monster); break;
                    }
                case MobStatus.SearchTarget: SearchTarget(client, monster); break;
                case MobStatus.Attacking: AttackingTarget(client, monster); break;
            }
        }
        public unsafe void CheckGuardPosition(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster.Alive)
            {
                if (monster.X != monster.RespawnX || monster.Y != monster.RespawnY)
                {
                    if (DateTime.Now > monster.MoveStamp.AddMilliseconds(300))
                    {
                        monster.MoveStamp = DateTime.Now;

                        Role.Flags.ConquerAngle dir = GetAngle(monster.X, monster.Y, monster.RespawnX, monster.RespawnY);
                        ushort WalkX = monster.X; ushort WalkY = monster.Y;
                        IncXY(dir, ref  WalkX, ref WalkY);

                        client.Owner.Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                        monster.X = WalkX;
                        monster.Y = WalkY;

                        WalkQuery walk = new WalkQuery()
                        {
                            Direction = (byte)dir,
                            Running = 1,
                            UID = monster.UID,
                            MapID = monster.Map
                        };
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            monster.Send(stream.MovementCreate(walk));
                        }
                    }
                }
            }
        }
        public unsafe bool GuardAttackPlayer(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return false;

            #region Nirvana Rune New Edit
            client.Owner.Player.deathtime = 20;
            byte itemLevels = 0;
            if (client.Owner.Rune.IsEquipped("Nirvana", ref itemLevels))
            {
                if (itemLevels == 9) client.Owner.Player.deathtime -= 15;
                else if (itemLevels == 8) client.Owner.Player.deathtime -= 12;
                else client.Owner.Player.deathtime -= (byte)(4 + itemLevels);
            }
            // The revive countdown only means something to a dead player (as in Player.Dead); this runs every tick for
            // every guard in view, and the mobile client shows the countdown as a red death tint on a living player.
            if (client.ContainFlag(MsgServer.MsgUpdate.Flags.Dead) || client.HitPoints == 0)
            {
                using (var recss = new ServerSockets.RecycledPacket())
                {
                    var streamss = recss.GetStream();
                    client.Send(streamss.NirvanaRune(client.Owner.Player.deathtime));
                }
            }

            #endregion


            if (client.ContainFlag(MsgServer.MsgUpdate.Flags.FlashingName) && client.Alive)
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange)
                {
                    if (!CheckRespouseDamage(client, monster))
                    {
                        uint Damage = MagicAttack(client.Owner, monster);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                , 0, client.X, client.Y, (ushort)monster.Family.SpellId, 0, 0);

                            SpellPacket.Targets.Enqueue(
                                new MsgServer.MsgSpellAnimation.SpellObj(client.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None)
                            );

                            SpellPacket.SetStream(stream);
                            SpellPacket.Send(monster);
                        }

                        // كانت متناداة مرتين ورا بعض → الضرر بيتطبّق ضعف الرقم اللي بيظهر للاعب
                        CheckForOponnentDead(client, Damage, monster);

                        return true;
                    }
                }
            }

            return false;
        }

        public unsafe bool GuardAttackMonster(Role.GameMap map, Game.MsgMonster.MonsterRole attacked, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return false;
            if (attacked.Alive)
            {
                short distance = MonsterView.GetDistance(attacked.X, attacked.Y, monster.X, monster.Y);
                if (distance < monster.Family.AttackRange)
                {
                    uint Damage = MagicAttack(attacked, monster);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                            , 0, attacked.X, attacked.Y, (ushort)1002, 0, 0);
                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(attacked.UID, Damage));
                        SpellPacket.SetStream(stream);
                        SpellPacket.Send(monster);

                        if (Damage >= attacked.HitPoints)
                        {
                            map.SetMonsterOnTile(attacked.X, attacked.Y, false);
                            attacked.Dead(stream, null, monster.UID, map);
                        }
                        else
                            attacked.HitPoints -= Damage;

                    }
                    return true;
                }
            }
            return false;
        }
        public bool ExtraBoss(Game.MsgMonster.MonsterRole monster)
        {
            return monster.Family.MaxHealth > 300000 && monster.Family.MaxHealth < 7000000;
        }
        public unsafe void AttackingTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!client.AllowAttack())
                return;
            if (!monster.Alive)
                return;
            short distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);

            if (monster.Boss == 1 && monster.HitPoints >= 2000000)
                monster.Family.AttackRange = 18;
            if (distance > monster.Family.AttackRange || monster.Target == null || !monster.Target.Alive || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                || !monster.Target.Owner.Socket.Alive)
            {

                monster.State = MobStatus.SearchTarget;
            }
            else
            {
                if (DateTime.Now > monster.AttackSpeed.AddMilliseconds(monster.Family.AttackSpeed))
                {
                    monster.AttackSpeed = DateTime.Now;
                    if (ExtraBoss(monster))
                    {
                        if (!CheckRespouseDamage(client, monster))
                        {
                            ushort SpellID = 9999;
                            if (monster.Level < 80)
                                SpellID = 9998;
                            else if (monster.Level < 60)
                                SpellID = 9966;

                            uint Damage = MagicAttack(monster.Target.Owner, monster);

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                    , 0, monster.Target.X, monster.Target.Y, (ushort)SpellID, 0, 0);
                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                SpellPacket.SetStream(stream);
                                SpellPacket.Send(monster);
                            }

                            CheckForOponnentDead(monster.Target, Damage, monster);
                        }
                        return;
                    }
                    if (monster.Boss == 0 && monster.Family.SpellId == 0)
                    {
                        if (!CheckRespouseDamage(client, monster))
                        {
                            uint Damage = PhysicalAttack(monster.Target.Owner, monster, true);
                            Damage = CheckDodge(monster.Target.Owner.Status.Dodge) ? Damage : 0;
                            if (Damage >= 1)
                            {
                                MsgServer.AttackHandler.CheckAttack.CheckItems.RespouseDurability(monster.Target.Owner);
                            }
                            InteractQuery action = new InteractQuery()
                            {
                                AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                X = monster.Target.X,
                                Y = monster.Target.Y,
                                UID = monster.UID,
                                OpponentUID = monster.Target.UID,
                                Damage = (int)Damage
                            };
                            if (monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5))
                                action.Effect = (uint)MsgAttackPacket.AttackEffect.TailedBeast;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                monster.Send(stream.InteractionCreate(action));
                            }

                            CheckForOponnentDead(monster.Target, Damage, monster);
                        }
                    }
                    else if (monster.Family.SpellId != 0 && monster.Boss == 0)
                    {
                        if (!CheckRespouseDamage(client, monster))
                        {
                            uint Damage = MagicAttack(monster.Target.Owner, monster);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                    , 0, monster.Target.X, monster.Target.Y, (ushort)monster.Family.SpellId, 0, 0);
                                SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                SpellPacket.SetStream(stream);
                                SpellPacket.Send(monster);
                            }

                            CheckForOponnentDead(monster.Target, Damage, monster);
                        }
                    }
                    else if (monster.Boss != 0)
                    {
                        if (monster.ContainFlag(MsgUpdate.Flags.Dizzy))
                            return;
                        if (!monster.Target.Alive || Role.Core.GetDistance(monster.X, monster.Y, monster.Target.X, monster.Target.Y) > 18)
                        {
                            monster.State = MobStatus.SearchTarget;
                            return;
                        }
                        switch (monster.Family.ID)
                        {
                            case 999996641:
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                        MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                         , 0, monster.Target.X, monster.Target.Y, 13190, 0, 0);
                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                        SpellPacket.SetStream(stream);
                                        SpellPacket.Send(monster);


                                        CheckForOponnentDead(monster.Target, Damage, monster);
                                    }
                                    break;
                                }
                            case 40946:
                            case 40936:
                            case 4212:
                            case 5355:
                            case 3977:
                                {
                                    List<ushort> Spells = new List<ushort>() { 8003, 11311, 1002 }; //, 11311, 1002, 11309, 11310
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    switch (Spells[rand])
                                    {
                                        case 1002:
                                        case 8003:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                break;
                                            }

                                        case 11311:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.ResistEarth));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                break;
                                            }
                                        case 11309:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 7)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 11310:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);


                                                            if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                                player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 7985:
                            case 40938:
                            case 40948:
                            case 4220:
                            case 40940:
                            case 4171://snow bamshee
                            case 32070://snow bamshee
                            case 3976:
                            case 3978:
                            case 3970:
                                {
                                    List<ushort> Spells = new List<ushort>() { 30013, 30014, 10372, 30013 };
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    switch (Spells[rand])
                                    {
                                        case 30010:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 14)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30011:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 8)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30012:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(10) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 30014:
                                        case 30013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                    CheckForOponnentDead(monster.Target, Damage, monster);
                                                }
                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                {
                                                    if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                        if (!Role.Core.Rate(55))
                                                        {
                                                            monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                }
                                                break;
                                            }
                                        case 10372:
                                            {
                                                uint Damage = 80000;
                                                monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage));
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                    , 0, monster.X, monster.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.UID, Damage, monster.Target.Owner.Beasts.Activated && Role.Core.Rate(monster.Target.Owner.Beasts.Level / 5) ? MsgServer.MsgAttackPacket.AttackEffect.TailedBeast : MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }

                            case 2751://chaos Guard
                                {
                                    List<ushort> Spells = new List<ushort>() { 7013, 7017, 10362, 10531, 11322 };
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    switch (Spells[rand])
                                    {
                                        case 11322:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);

                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                    if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                        if (!Role.Core.Rate(55))
                                                        {
                                                            monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                break;
                                            }
                                        case 10531:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 10362:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 10)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7017:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 18)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 20101:
                            case 40937:
                            case 40947:
                            case 40939:
                            case 40949:

                            case 20060://terato Dragon
                            case 3971:
                                {
                                    List<ushort> Spells = new List<ushort>() { 7013, 7014, 7015, 7016, 7017 };
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    switch (Spells[rand])
                                    {
                                        case 7017:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();

                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                       , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 8)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 7014:
                                        case 7013:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.Target.X, monster.Target.Y, player.X, player.Y) <= 6)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);

                                                }
                                                break;
                                            }
                                        case 7015:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);

                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                    if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                        if (!Role.Core.Rate(55))
                                                        {
                                                            monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                        }
                                                break;
                                            }
                                        case 7016:
                                            {
                                                uint Damage = 80000;
                                                monster.HitPoints = (uint)Math.Min(monster.Family.MaxHealth, (int)(monster.HitPoints + Damage));
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                    , 0, monster.X, monster.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);


                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case 40935:
                            case 40945:
                            case 20055://lava beast
                                {
                                    List<ushort> Spells = new List<ushort>() { 10000, 10001, 10003 };
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    switch (Spells[rand])
                                    {
                                        case 10003:
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                      , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);

                                                    foreach (var targent in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                                    {
                                                        if (!targent.Alive)
                                                            continue;
                                                        var player = targent as Role.Player;
                                                        if (Role.Core.GetDistance(monster.X, monster.Y, player.X, player.Y) <= 3)
                                                        {
                                                            uint Damage = PhysicalAttack(player.Owner, monster);
                                                            SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(player.UID, Damage
                                                                , MsgServer.MsgAttackPacket.AttackEffect.None));
                                                            CheckForOponnentDead(player, Damage, monster);

                                                            if (Rate(5) && !player.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                                                if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                                    if (!Role.Core.Rate(55))
                                                                    {
                                                                        player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                                                    }
                                                        }
                                                    }
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                break;
                                            }
                                        case 10001:
                                            {

                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                     , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);

                                                if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Dizzy))
                                                {
                                                    if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                                        if (!Role.Core.Rate(55))
                                                        {
                                                            monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 6, true);
                                                        }
                                                }
                                                break;
                                            }
                                        default:
                                            {
                                                uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                              , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                                    SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                                    SpellPacket.SetStream(stream);
                                                    SpellPacket.Send(monster);
                                                }
                                                CheckForOponnentDead(monster.Target, Damage, monster);
                                                break;
                                            }

                                    }
                                    break;
                                }
                            case 66432:
                            case 40934:
                            case 40944:
                            case 6643://SwordMaster
                                {
                                    List<ushort> Spells = new List<ushort>() { 10500, 10502, 10503, 10504, 10506 };
                                    ushort rand = (byte)Pool.GetRandom.Next(0, Spells.Count);
                                    uint Damage = PhysicalAttack(monster.Target.Owner, monster);
                                    if (rand == 0)
                                        Damage = (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, 130, 100);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgServer.MsgSpellAnimation SpellPacket = new MsgServer.MsgSpellAnimation(monster.UID
                                                                      , 0, monster.Target.X, monster.Target.Y, (ushort)Spells[rand], 0, 0);
                                        SpellPacket.Targets.Enqueue(new MsgServer.MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgServer.MsgAttackPacket.AttackEffect.None));
                                        SpellPacket.SetStream(stream);
                                        SpellPacket.Send(monster);
                                    }
                                    CheckForOponnentDead(monster.Target, Damage, monster);

                                    if (Rate(5))
                                    {
                                        monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Frightened, 6, true);
                                    }
                                    else if (Rate(5) && !monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Frightened))
                                        if (monster.Target.Owner.Rune.IsEquipped("Acalanatha"))
                                            if (!Role.Core.Rate(55))
                                            {
                                                monster.Target.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 3, true);
                                            }


                                    break;
                                }

                        }

                    }
                    #region CounterPunch
                    if (monster.Boss == 0 && Rate(15) && monster.Target != null && monster.Target.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CounterPunch))
                    {
                        var mySpell = Pool.Magic[(ushort)Role.Flags.SpellID.CounterPunch][monster.Target.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.CounterPunch].Level];
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(monster.UID
                                      , monster.Target.UID, 0, 0, mySpell.ID
                                      , mySpell.Level, 0);

                            MsgSpellAnimation.SpellObj AnimationObj;
                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(monster.Target.Owner.Player, monster, mySpell, out AnimationObj, 8);
                            MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, monster.Target.Owner, monster);
                            MsgSpell.Targets.Enqueue(AnimationObj);


                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(monster);
                        }
                    }
                    #endregion

                }
            }
        }
        public unsafe void SearchTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (monster == null)
                return;
            if (!monster.Alive)
                return;
            try
            {
                if (monster.Target != null)
                {

                    if (monster.Target.Owner.Fake || !monster.Target.Alive || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                        || !monster.Target.Owner.Socket.Alive)
                    {
                        monster.State = MobStatus.Idle;
                        return;
                    }
                }
                if (monster.Target == null)
                {
                    monster.State = MobStatus.Idle;
                    return;
                }
                if (monster.Family == null)
                    return;
                short distance = MonsterView.GetDistance(monster.Target.X, monster.Target.Y, monster.X, monster.Y);
                if (distance <= monster.Family.AttackRange || (monster.Boss == 1 && monster.HitPoints > 2000000 && distance <= 18))
                {
                    monster.State = MobStatus.Attacking;
                }
                else
                {
                    monster.State = MobStatus.Idle;
                }
                if (distance <= monster.Family.ViewRange && monster.Target != null && monster.Target.Alive)
                {

                    try
                    {
                        if (DateTime.Now > monster.MoveStamp.AddMilliseconds(monster.Family.MoveSpeed))
                        {
                            monster.MoveStamp = DateTime.Now;

                            bool Walk = Random.Next() % 100 < 70;
                            if (Walk)
                            {
                                Role.Flags.ConquerAngle dir = GetAngle(monster.X, monster.Y, monster.Target.X, monster.Target.Y);
                                ushort WalkX = monster.X; ushort WalkY = monster.Y;
                                IncXY(dir, ref WalkX, ref WalkY);

                                var Map = monster.Target.Owner.Map;
                                if (Map != null)
                                {
                                    if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                    {
                                        Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                        Map.SetMonsterOnTile(WalkX, WalkY, true);

                                        Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                                        monster.X = WalkX; monster.Y = WalkY;


                                        WalkQuery action = new WalkQuery()
                                        {
                                            Direction = (byte)dir,
                                            Running = 1,
                                            UID = monster.UID,
                                            MapID = monster.Map
                                        };
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            monster.Send(stream.MovementCreate(action));
                                        }
                                    }
                                    else
                                    {
                                        dir = (Role.Flags.ConquerAngle)(Random.Next() % 8);
                                        WalkX = monster.X; WalkY = monster.Y;
                                        IncXY(dir, ref WalkX, ref WalkY);

                                        if (Map.ValidLocation(WalkX, WalkY) && !Map.MonsterOnTile(WalkX, WalkY))
                                        {
                                            Map.SetMonsterOnTile(monster.X, monster.Y, false);
                                            Map.SetMonsterOnTile(WalkX, WalkY, true);

                                            Map.View.MoveTo<Role.IMapObj>(monster, WalkX, WalkY);

                                            monster.X = WalkX; monster.Y = WalkY;


                                            WalkQuery action = new WalkQuery()
                                            {
                                                Direction = (byte)dir,
                                                Running = 1,
                                                UID = monster.UID,
                                                MapID = monster.Map
                                            };
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                monster.Send(stream.MovementCreate(action));

                                                monster.UpdateMonsterView(monster.Target.View, stream);
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch { monster.State = MobStatus.Idle; }
                }
                else
                    monster.State = MobStatus.Idle;
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public void CheckTarget(Role.Player client, Game.MsgMonster.MonsterRole monster)
        {
            if (!monster.Alive)
                return;
            if (monster.Target == null && !client.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (distance <= monster.Family.ViewRange && client.Alive)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (monster.Target != null)
                        {
                            if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                            {
                                monster.State = MobStatus.SearchTarget;
                            }
                            else
                            {
                                foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                                {
                                    var obj = OtherTarget as Role.Player;
                                    if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                    {
                                        monster.Target = obj;
                                        monster.State = MobStatus.SearchTarget;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                short distance = MonsterView.GetDistance(client.X, client.Y, monster.X, monster.Y);
                if (monster.Target == null)
                {
                    var targ = monster.View.GetTarget(client.Owner.Map, Role.MapObjectType.Player);
                    if (targ != null)
                    {
                        monster.Target = targ as Role.Player;
                        if (!monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly) || monster.Boss == 1)
                        {
                            monster.State = MobStatus.SearchTarget;
                        }
                        else
                        {
                            foreach (var OtherTarget in monster.View.Roles(client.Owner.Map, Role.MapObjectType.Player))
                            {
                                var obj = OtherTarget as Role.Player;
                                if (!obj.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                {
                                    monster.Target = obj;
                                    monster.State = MobStatus.SearchTarget;
                                }
                            }
                        }
                    }
                }
                if (monster.Target != null && (distance > monster.Family.ViewRange || monster.Target.ContainFlag(MsgServer.MsgUpdate.Flags.Fly)
                    || monster.Target.Owner.Socket == null || !monster.Target.Owner.Socket.Alive))
                {
                    monster.Target = null;
                }
                else if (monster.Target == null || monster.Target.Alive)
                    monster.State = MobStatus.SearchTarget;
            }
        }
        public bool CheckDodge(uint Dodge)
        {
            bool allow = true;
            uint Noumber = (uint)Random.Next() % 150;
            if (Noumber > 60)
                Noumber = (uint)Random.Next() % 150;
            if (Noumber < Dodge)
                allow = false;
            return allow;
        }

        public void CheckForOponnentDead(Role.Player Player, uint Damage, MonsterRole monster)
        {
            if (Player.Alive == false)
                return;

            if (Player.ActivePick)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Player.RemovePick(stream);
                }
            }
            if (!Player.Owner.Socket.Alive)
                return;
            if (Damage >= Player.HitPoints) // حل مشكلة الاميل الى مش بيموت من الجارد ومن الوحوش Guard-Monsters
            {

                ushort X = Player.X;
                ushort Y = Player.Y;
                Player.Dead(null, X, Y, monster.UID);
                if (Player.OnAutoHunt)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (Player.MainFlag == Role.Player.MainFlagType.ClaimGift || Player.MainFlag == Role.Player.MainFlagType.CanClaim)
                        {
                            Player.Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 5, 0, Player.AutoHuntExp, monster.Name));
                            if (Player.AutoHuntExp > 0)
                            {
                                Player.Owner.IncreaseAutoExperience(stream, Player.AutoHuntExp);
                                Player.AutoHuntExp = 0;
                            }
                        }
                        else
                        {
                            Player.Owner.Send(MsgAutoHunt.AutoHuntCreate(stream, 6, 0, Player.AutoHuntExp, monster.Name));
                            Player.AutoHuntExp = 0;
                        }
                    }
                }
            }
            else
            {
                if (Player.Action == Role.Flags.ConquerAction.Sit)
                {
                    if (Player.Stamina >= 20)
                        Player.Stamina -= 20;
                    else
                        Player.Stamina = 0;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Player.SendUpdate(stream, Player.Stamina, MsgServer.MsgUpdate.DataType.Stamina);
                    }


                    Player.Action = Role.Flags.ConquerAction.None;
                }

                Player.HitPoints -= (int)Damage;
            }
        }
        public unsafe bool CheckRespouseDamage(Role.Player player, MonsterRole Monster)
        {

            MsgYuanshen.Magics(player.Owner, Monster, 1);
            MsgSpell user_spell = null;
            #region SkyRocket
            if (player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengRocket))
            {
                if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.KunpengRocket, out user_spell))
                {
                    Database.MagicType.Magic SkyRocket = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (Role.Core.Rate(SkyRocket.Rate))
                    {
                        var InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = user_spell.ID;
                        InteractQuery.SpellLevel = user_spell.Level;
                        InteractQuery.X = player.X;
                        InteractQuery.Y = player.Y;
                        InteractQuery.UID = player.UID;
                        InteractQuery.OpponentUID = Monster.UID;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgAttackPacket.ProcescMagic(player.Owner, stream, InteractQuery, true);
                        }
                    }
                }
            }
            #endregion
            #region Hunter
            if (Role.Core.Rate(10))
            {
                if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Hunter, out user_spell))
                {
                    InteractQuery InteractQuery = new InteractQuery();
                    InteractQuery.SpellID = user_spell.ID;
                    InteractQuery.SpellLevel = user_spell.Level;
                    InteractQuery.X = Monster.X;
                    InteractQuery.Y = Monster.Y;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        MsgAttackPacket.ProcescMagic(player.Owner, stream, InteractQuery, true);
                    }
                }
            }
            #endregion
            #region HeavensWonder
            if (player.ContainFlag(MsgUpdate.Flags.SparkShield) && player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.SparkShield, out user_spell))
            {
                Database.MagicType.Magic SparkShield = Pool.Magic[user_spell.ID][user_spell.Level];
                if (Role.Core.Rate(SparkShield.DamageOnHuman))
                {
                    player.SparkShieldDamage = (uint)SparkShield.Damage2;
                    player.SparkShieldLevel = SparkShield.Level;
                    player.SparkShieldTime = (int)SparkShield.DamageOnMonster;
                    player.SparkShieldStamp = DateTime.Now;
                    player.AddSpellFlag(MsgUpdate.Flags.SparkShieldActivated, (int)(int)SparkShield.DamageOnMonster, true);
                }
            }
            #endregion
            #region HeavensWonder
            if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HeavensWonder, out user_spell))
            {
                if (Database.AtributesStatus.IsNinja(player.Class))
                {
                    Database.MagicType.Magic HeavensWonder = Pool.Magic[user_spell.ID][user_spell.Level];
                    if (Role.Core.Rate(HeavensWonder.Rate / 100))
                    {
                        var InteractQuery = new InteractQuery();
                        InteractQuery.SpellID = user_spell.ID;
                        InteractQuery.SpellLevel = user_spell.Level;
                        InteractQuery.X = player.X;
                        InteractQuery.Y = player.Y;
                        InteractQuery.OpponentUID = player.UID;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgAttackPacket.ProcescMagic(player.Owner, stream, InteractQuery, true);
                        }
                    }
                }
            }
            #endregion
            #region HellVortex
            Role.Instance.Ninja.Item item;
            if (player.Owner.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.HellVortex, out item))
            {
                if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HellVortex, out user_spell))
                {
                    if (Database.AtributesStatus.IsNinja(player.Class))
                    {
                        Database.MagicType.Magic HellVortex = Pool.Magic[user_spell.ID][user_spell.Level];
                        if (Role.Core.Rate(HellVortex.Rate / 100))
                        {
                            var InteractQuery = new InteractQuery();
                            InteractQuery.SpellID = user_spell.ID;
                            InteractQuery.SpellLevel = user_spell.Level;
                            InteractQuery.X = player.X;
                            InteractQuery.Y = player.Y;
                            InteractQuery.OpponentUID = player.UID;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                MsgAttackPacket.ProcescMagic(player.Owner, stream, InteractQuery, true);
                            }
                            return true;
                        }
                    }
                }
            }
            #endregion
            {
                if (player.Owner.MyArchives.isOpen(Archives.TypeID.Redcurse) && Role.Core.Rate(10))
                {
                    MsgSpell Spell;
                    if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ArmorofImmunity, out Spell))
                    {
                        InteractQuery Attack = new InteractQuery();
                        Attack.SpellID = Spell.ID;
                        Attack.SpellLevel = Spell.Level;
                        Attack.X = (ushort)player.X;
                        Attack.Y = (ushort)player.Y;
                        Attack.OpponentUID = Monster.UID;
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            ServerSockets.Packet stream = recycledPacket.GetStream();
                            MsgAttackPacket.ProcescMagic(player.Owner, stream, Attack, true);
                        }
                        return true;
                    }
                }

                if (player.EagleEyeCountDown)
                {
                    MsgSpell adrenalineSpell;
                    Database.MagicType.Magic adrenalineDBSpell;
                    if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.AdrenalineRush, out adrenalineSpell))
                    {
                        Dictionary<ushort, Database.MagicType.Magic> aDBSpells;
                        if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.AdrenalineRush, out aDBSpells))
                        {
                            if (aDBSpells.TryGetValue(adrenalineSpell.Level, out adrenalineDBSpell))
                            {
                                if (Role.Core.Rate(adrenalineDBSpell.Rate))
                                {
                                    MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(player.UID
                                          , player.UID, 0, 0, adrenalineSpell.ID
                                          , adrenalineSpell.Level, adrenalineSpell.UseSpellSoul);
                                    RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = player.UID, Damage = adrenalineSpell.ID });
                                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = recycledPacket.GetStream();
                                        RemoveCloudDown.SetStream(streamm);
                                    }
                                    RemoveCloudDown.JustMe(player.Owner);
                                    player.EagleEyeCountDown = false;

                                    MsgMagicColdTime.MagicColdTime packet = new MsgMagicColdTime.MagicColdTime();
                                    packet.ZeroEagleEye();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        player.Send(stream.MagicColdTimeCreate(packet));
                                    }
                                }
                            }
                        }
                    }
                }
                if (player.ContainFlag(MsgUpdate.Flags.RevengeTail))
                {
                    if (player.RevengeTailChange > 0)
                    {
                        MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out ClientSpell))
                        {
                            Database.MagicType.Magic DBSpell;
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.RevengeTail, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {

                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(player.UID
                                 , 0, player.X, player.Y, ClientSpell.ID
                                 , ClientSpell.Level, ClientSpell.UseSpellSoul);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgSpell.bomb = 1;
                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(player.Owner, Monster, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                            {
                                                UID = player.UID,
                                                Hit = 1
                                            };
                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, player.Owner, Monster);
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(player.Owner);
                                    }
                                    player.RevengeTailChange -= 1;
                                    if (player.RevengeTailChange == 0)
                                        player.RemoveFlag(MsgUpdate.Flags.RevengeTail);
                                }

                            }
                        }
                    }
                }
                if (player.ContainFlag(MsgUpdate.Flags.Backfire))
                {
                    player.RemoveFlag(MsgUpdate.Flags.Backfire);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (player.ContainFlag(MsgServer.MsgUpdate.Flags.ShurikenVortex))
                            return false;
                        Database.MagicType.Magic DBSpell;
                        Game.MsgServer.MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out ClientSpell))
                        {
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.Backfire, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {
                                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj(Monster.UID, 0, MsgAttackPacket.AttackEffect.None);
                                    DmgObj.Damage = (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(player.HitPoints, 55, 100);

                                    //update spell
                                    if (ClientSpell.Level < DBSpells.Count - 1)
                                    {
                                        ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                        if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                        {
                                            ClientSpell.Level++;
                                            ClientSpell.Experience = 0;
                                        }
                                        player.Send(stream.SpellCreate(ClientSpell));
                                    }

                                    InteractQuery action = new InteractQuery()
                                    {

                                        Damage = (int)DmgObj.Damage,
                                        AtkType = (ushort)MsgAttackPacket.AttackID.BackFire,
                                        X = player.X,
                                        Y = player.Y,
                                        OpponentUID = Monster.UID,
                                        UID = player.UID,
                                        Effect = (uint)DmgObj.Effect
                                    };

                                    Monster.Send(stream.InteractionCreate(action));


                                    Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            if (Rate(30))
            {
                if (player.ContainReflect)
                {
                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj();
                    DmgObj.Damage = PhysicalAttack(player.Owner, Monster, true);
                    DmgObj.Damage /= 60;
                    if (DmgObj.Damage == 0)
                        DmgObj.Damage = 1;

                    InteractQuery action = new InteractQuery()
                    {
                        Damage = (int)DmgObj.Damage,
                        ResponseDamage = DmgObj.Damage,
                        AtkType = (ushort)MsgAttackPacket.AttackID.Reflect,
                        X = player.X,
                        Y = player.Y,
                        OpponentUID = Monster.UID,
                        UID = player.UID,
                        Effect = (uint)DmgObj.Effect
                    };
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Monster.Send(stream.InteractionCreate(action));

                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                    }
                    return true;


                }

                if (player.ActivateCounterKill)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (player.ContainFlag(MsgServer.MsgUpdate.Flags.ShurikenVortex))
                            return false;
                        Database.MagicType.Magic DBSpell;
                        Game.MsgServer.MsgSpell ClientSpell;
                        if (player.Owner.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out ClientSpell))
                        {
                            Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                            if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.CounterKill, out DBSpells))
                            {
                                if (DBSpells.TryGetValue(ClientSpell.Level, out DBSpell))
                                {
                                    Game.MsgServer.MsgSpellAnimation.SpellObj DmgObj = new Game.MsgServer.MsgSpellAnimation.SpellObj();
                                    Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(player, Monster, DBSpell, out DmgObj);

                                    //update spell
                                    if (ClientSpell.Level < DBSpells.Count - 1)
                                    {
                                        ClientSpell.Experience += (int)(DmgObj.Damage * Program.ServerConfig.ExpRateSpell);
                                        if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                        {
                                            ClientSpell.Level++;
                                            ClientSpell.Experience = 0;
                                        }
                                        player.Send(stream.SpellCreate(ClientSpell));
                                    }

                                    InteractQuery action = new InteractQuery()
                                    {
                                        ResponseDamage = DmgObj.Damage,
                                        AtkType = (ushort)MsgAttackPacket.AttackID.Scapegoat,
                                        X = player.X,
                                        Y = player.Y,
                                        OpponentUID = Monster.UID,
                                        UID = player.UID,
                                        Effect = (uint)DmgObj.Effect
                                    };

                                    Monster.Send(stream.InteractionCreate(action));


                                    Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, DmgObj, player.Owner, Monster);
                                }
                            }
                        }
                    }
                    return true;
                }
            }

            player.ThundercloudSight = Monster.UID;
            return false;
        }
        public uint MagicAttack(MonsterRole attacked, MonsterRole monster)
        {
            uint power = (uint)monster.Family.MaxAttack;
            if (power > attacked.Family.Defense)
                power -= attacked.Family.Defense;
            else power = 1;
            return power;
        }
        public uint MagicAttack(Client.GameClient client, MonsterRole monster)
        {
            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;            
            if (monster.Name == "AncientDragon") return 0;
            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = (uint)monster.Family.MaxAttack;
            power = DecreaseBless(power, client.Status.ItemBless);

            if (power > client.Status.MagicDefence)
                power -= client.Status.MagicDefence;
            else power = 1;
            if (power > client.Status.MagicDamageDecrease)
                power -= client.Status.MagicDamageDecrease;
            else power = 1;
            if (client.Player.ContainFlag(MsgUpdate.Flags.HawksAmbition))
            {
                power -= (uint)(power * client.Player.HawksAmbitionPower / 100);
                client.Player.RemoveFlag(MsgUpdate.Flags.HawksAmbition);
            }
            return power;
        }
        public uint PhysicalAttack(Client.GameClient client, MonsterRole monster, bool melee = false)
        {
            if (!client.Socket.Alive)
                return 0;
            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return 1;           
            if (monster.Name == "AncientDragon") return 0;


            MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);

            uint power = 0;
            if (monster.Family.MaxAttack > monster.Family.MinAttack)
                power = (uint)SystemRandom.Next(monster.Family.MinAttack, monster.Family.MaxAttack);
            else
                power = (uint)SystemRandom.Next(monster.Family.MaxAttack, monster.Family.MinAttack);



            power += (uint)(power * Program.ServerConfig.PhysicalDamage / 10000);

            power = DecreaseBless(power, client.Status.ItemBless);

            if (power > client.AjustDefense)
                power -= client.AjustDefense;
            else
                power = 1;

            if (client.Player.ContainFlag(MsgUpdate.Flags.IronShield))
            {
                if (client.Player.IronShieldDamage >= power)
                {
                    client.Player.IronShieldDamage -= power;
                    power = 0;
                }
                else
                {
                    power -= client.Player.IronShieldDamage;
                    client.Player.IronShieldDamage = 0;
                }
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.IronShield))
            {
                if (client.Player.IronShieldDamage >= power)
                {
                    client.Player.IronShieldDamage -= power;
                    power = 0;
                }
                else
                {
                    power -= client.Player.IronShieldDamage;
                    client.Player.IronShieldDamage = 0;
                }
            }
            if (client.MyNinja.Valid())
            {
                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HellVortex) && !client.Player.ContainFlag(MsgUpdate.Flags.HellVortex))
                {
                    var Spell = Pool.Magic[(ushort)Role.Flags.SpellID.HellVortex][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.HellVortex].Level];
                    if (Role.Core.Rate(Spell.Damage2))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                                   , monster.UID, client.Player.X, client.Player.Y, Spell.ID
                                   , Spell.Level, 0);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(client);
                            if (client.Player.AddFlag(MsgUpdate.Flags.HellVortex, (int)(int)Spell.Duration, true))
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.HellVortex, (uint)Spell.Duration, 0, 0, MsgUpdate.DataType.ArchiveSkill);
                        }
                    }
                }
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.HawksAmbition))
            {
                power -= (uint)(power * client.Player.HawksAmbitionPower / 100);
                client.Player.RemoveFlag(MsgUpdate.Flags.HawksAmbition);
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.LightningShield) && !client.Player.ContainFlag(MsgUpdate.Flags.LightningShieldActivated) && client.Player.Owner.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LightningShield))
            {
                if (Role.Core.Rate(20))
                {
                    var lightningSpell = Pool.Magic[(ushort)Role.Flags.SpellID.LightningShield][client.Player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.LightningShield].Level];
                    client.Player.LightningShieldLeft = (uint)(client.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage) ? (20000 + (lightningSpell.Level + 1 * 1000)) : 20000);
                    #region FlashShield(Rune Skill)
                    byte itemLevel = 0;
                    if (client.Player.Owner.Rune.IsEquipped("FlashShield", ref itemLevel))
                    {
                        uint addition = 5000;
                        switch (itemLevel)
                        {
                            case 2: addition = 6000; break;
                            case 3: addition = 7000; break;
                            case 4: addition = 7500; break;
                            case 5: addition = 8000; break;
                            case 6: addition = 8500; break;
                            case 7: addition = 9000; break;
                            case 8: addition = 9500; break;
                            case 9: addition = 10000; break;
                            case 10: addition = 11000; break;
                            case 11: addition = 11500; break;
                            case 12: addition = 12000; break;
                            case 13: addition = 12500; break;
                            case 14: addition = 13000; break;
                            case 15: addition = 13500; break;
                            case 16: addition = 14000; break;
                            case 17: addition = 14500; break;
                            case 18: addition = 15000; break;
                            case 19: addition = 15500; break;
                            case 20: addition = 16000; break;
                            case 21: addition = 16500; break;
                            case 22: addition = 17000; break;
                            case 23: addition = 17500; break;
                            case 24: addition = 18000; break;
                            case 25: addition = 18500; break;
                            case 26: addition = 19000; break;
                            case 27: addition = 20000; break;
                        }
                        client.Player.LightningShieldLeft += addition;
                    }
                    #endregion
                    client.Player.AddFlag(MsgUpdate.Flags.LightningShieldActivated, 10, true);
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        client.Player.SendUpdate(streamm, MsgUpdate.Flags.LightningShieldActivated, 10, client.Player.LightningShieldLeft, lightningSpell.Level, MsgUpdate.DataType.AzureShield);
                    }
                }
            }
            if (client.Player.ContainFlag(MsgUpdate.Flags.LightningShieldActivated))
            {
                if (client.Player.LightningShieldLeft > 0)
                {
                    if (power >= client.Player.LightningShieldLeft)
                    {
                        power -= client.Player.LightningShieldLeft;
                        client.Player.LightningShieldLeft = 0;
                    }
                    else
                    {
                        client.Player.LightningShieldLeft -= power;
                        power = 0;
                    }
                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                    {
                        var streamm = recycledPacket.GetStream();
                        client.Player.SendUpdate(streamm, MsgUpdate.Flags.LightningShieldActivated, (uint)((client.Player.BitVector.ArrayFlags[(int)MsgUpdate.Flags.LightningShieldActivated].Timer.AddSeconds(10) - DateTime.Now).TotalSeconds), client.Player.LightningShieldLeft, client.Player.Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.LightningShield].Level, MsgUpdate.DataType.AzureShield);
                    }
                }
            }
            return power;
        }
        public uint DecreaseBless(uint Damage, uint bless)
        {
            uint power = Damage;
            power = (power * bless) / 100;
            power = Damage - power;
            return power;
        }
        public Role.Flags.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Role.Flags.ConquerAngle)(byte)((int)Dir % 8);
        }
        public static void IncXY(Role.Flags.ConquerAngle Facing, ref ushort x, ref ushort y)
        {
            sbyte xi, yi;
            xi = yi = 0;
            switch (Facing)
            {
                case Role.Flags.ConquerAngle.North: xi = -1; yi = -1; break;
                case Role.Flags.ConquerAngle.South: xi = 1; yi = 1; break;
                case Role.Flags.ConquerAngle.East: xi = 1; yi = -1; break;
                case Role.Flags.ConquerAngle.West: xi = -1; yi = 1; break;
                case Role.Flags.ConquerAngle.NorthWest: xi = -1; break;
                case Role.Flags.ConquerAngle.SouthWest: yi = 1; break;
                case Role.Flags.ConquerAngle.NorthEast: yi = -1; break;
                case Role.Flags.ConquerAngle.SouthEast: xi = 1; break;
            }
            x = (ushort)(x + xi);
            y = (ushort)(y + yi);
        }
    }
}
