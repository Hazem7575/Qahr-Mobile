//using Qahr.Mobile.Game.MsgFloorItem;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Qahr.Mobile.Game.MsgServer.AttackHandler
//{
//    public class BladeTempest
//    {
//        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
//        {
//            Database.MagicType.Magic DBSpell;
//            MsgSpell ClientSpell;
//            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
//            {
//                switch (ClientSpell.ID)
//                {
//                    case (ushort)Role.Flags.SpellID.BladeTempest:
//                        {
//                            bool Tidy = false;
//                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
//                                   , 0, Attack.X, Attack.Y, ClientSpell.ID
//                                   , ClientSpell.Level, ClientSpell.UseSpellSoul);
//                            byte dist = (Tidy ? (byte)5 : (byte)10);
//                            List<Algoritms.InLineAlgorithm.coords> coord = Algoritms.MoveCoords.CheckBladeTeampsCoords(user.Player.X, user.Player.Y, Attack.X
//                                , Attack.Y, user.Map, dist);
//                            if (user.Rune.IsEquipped("TideTrap") && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TideTrap))
//                            {
//                                MsgSpell.bomb = 1;
//                                Tidy = true;

//                            }
//                            Attack.X = user.Player.X;
//                            Attack.Y = user.Player.Y;
//                            for (int i = 0; i < coord.Count; i++)
//                            {
//                                if (user.Map.ValidLocation((ushort)coord[i].X, (ushort)coord[i].Y)
//                                    && CheckAttack.CheckFloors.CheckGuildWar(user, coord[coord.Count - 1].X, coord[coord.Count - 1].Y))
//                                {
//                                    Attack.X = (ushort)coord[i].X;
//                                    Attack.Y = (ushort)coord[i].Y;
//                                }
//                                else
//                                {
//                                    break;
//                                }
//                            }
//                            MsgSpell.X = Attack.X;
//                            MsgSpell.Y = Attack.Y;

//                            uint Experience = 0;
//                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
//                            {
//                                bool hit = false;
//                                for (int j = 0; j < coord.Count; j++)
//                                    if (Calculate.Base.GetDDistance(target.X, target.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
//                                        hit = true;
//                                if (hit)
//                                {
//                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
//                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
//                                    {
//                                        MsgSpellAnimation.SpellObj AnimationObj;
//                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
//                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
//                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
//                                        MsgSpell.Targets.Enqueue(AnimationObj);


//                                        if (target.Alive)
//                                        {
//                                            MsgSpell blackspotSpell;
//                                            Database.MagicType.Magic blackspotDBSpell;
//                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out blackspotSpell))
//                                            {
//                                                Dictionary<ushort, Database.MagicType.Magic> bDBSpells;
//                                                if (Pool.Magic.TryGetValue((ushort)Role.Flags.SpellID.Blackspot, out bDBSpells))
//                                                {
//                                                    if (bDBSpells.TryGetValue(blackspotSpell.Level, out blackspotDBSpell))
//                                                    {
//                                                        if (Role.Core.Rate(blackspotDBSpell.Rate))
//                                                        {
//                                                            attacked.BlackSpot = true;
//                                                            attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)blackspotDBSpell.Duration);

//                                                            user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
//                                                        }
//                                                    }
//                                                }
//                                            }

//                                        }
//                                    }
//                                }
//                            }
//                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
//                            {
//                                bool hit = false;
//                                for (int j = 0; j < coord.Count; j++)
//                                    if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
//                                        hit = true;
//                                if (hit)
//                                {
//                                    var attacked = targer as Role.Player;

//                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
//                                    {

//                                        MsgSpellAnimation.SpellObj AnimationObj;
//                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
//                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
//                                        MsgSpell.Targets.Enqueue(AnimationObj);
//                                        if (attacked.Alive)
//                                        {
//                                            if (Role.Core.Rate(35))
//                                            {
//                                                attacked.BlackSpot = true;
//                                                attacked.Stamp_BlackSpot = DateTime.Now.AddSeconds((int)DBSpell.Duration);
//                                                user.Player.View.SendView(stream.BlackspotCreate(true, attacked.UID), true);
//                                            }
//                                        }

//                                    }
//                                }

//                            }
//                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
//                            {
//                                bool hit = false;
//                                for (int j = 0; j < coord.Count; j++)
//                                    if (Calculate.Base.GetDDistance(targer.X, targer.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 1.5)
//                                        hit = true;
//                                if (hit)
//                                {
//                                    var attacked = targer as Role.SobNpc;
//                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
//                                    {
//                                        MsgSpellAnimation.SpellObj AnimationObj;
//                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
//                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
//                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
//                                        MsgSpell.Targets.Enqueue(AnimationObj);

//                                    }
//                                }
//                            }
//                            Updates.IncreaseExperience.Up(stream, user, Experience);
//                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
//                            MsgSpell.SetStream(stream);
//                            MsgSpell.SendRole(user);
//                            MsgSpell.Send(user);
//                            if (!Tidy)
//                            {
//                                user.Map.View.MoveTo<Role.IMapObj>(user.Player, MsgSpell.X, MsgSpell.Y);
//                                user.Player.X = MsgSpell.X;
//                                user.Player.Y = MsgSpell.Y;
//                            }
//                            else
//                            {
//                                if (!user.Player.FloorSpells.ContainsKey((ushort)Role.Flags.SpellID.TideTrap))
//                                    user.Player.FloorSpells.TryAdd((ushort)Role.Flags.SpellID.TideTrap, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, Attack.X, Attack.Y, 0, Pool.Magic[(ushort)Role.Flags.SpellID.TideTrap][0], user.Map));
//                                var FloorItem = new Role.FloorSpell(Game.MsgFloorItem.MsgItemPacket.TideTrap, Attack.X, (ushort)Attack.Y, 14, DBSpell, 4000);
//                                FloorItem.FloorPacket.ItemOwnerUID = user.Player.UID;
//                                FloorItem.FloorPacket.OwnerX = user.Player.X;
//                                FloorItem.FloorPacket.OwnerY = user.Player.Y;
//                                FloorItem.FloorPacket.Name = "trap";
//                                FloorItem.FloorPacket.MapID = user.Player.Map;
//                                user.Player.FloorSpells[(ushort)Role.Flags.SpellID.TideTrap].AddItem(FloorItem);
//                                user.Player.View.SendView(stream.ItemPacketCreate(FloorItem.FloorPacket), true);
//                            }

//                            break;
//                        }
//                }
//            }
//        }

//    }
//}
