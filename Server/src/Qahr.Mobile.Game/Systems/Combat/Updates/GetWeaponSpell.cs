using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Updates
{
    public class GetWeaponSpell
    {
        public static void CheckExtraEffects(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.Equipment.RingEffect != Role.Flags.ItemEffect.None)
            {
                if (Calculate.Base.Rate(20))
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Stigma))
                    {
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                   , 0, client.Player.X, client.Player.Y, (ushort)Role.Flags.SpellID.Stigma
                   , 4, 0);
                        client.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, 20, true);
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(client);
                    }
                }
            }
            if (client.Equipment.NecklaceEffect != Role.Flags.ItemEffect.None)
            {
                if (Calculate.Base.Rate(20))
                {
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Shield))
                    {
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                   , 0, client.Player.X, client.Player.Y, (ushort)Role.Flags.SpellID.Shield
                   , 4, 0);
                        client.Player.AddSpellFlag(MsgUpdate.Flags.Shield, 15, true);
                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(client);
                    }
                }
            }
        }
        public unsafe static bool CheckMelee(InteractQuery Attack, ServerSockets.Packet stream, Client.GameClient user, Role.IMapObj _target)
        {
            if (Attack.SpellID != 0 && user.MySpells.ClientSpells.ContainsKey(Attack.SpellID))
            {
                MsgGameItem RightWeapon;
                MsgGameItem LeftWeapon;

                bool hasRightKnife = false;
                bool hasLeftKnife = false;

                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
                {
                    if (Database.ItemType.IsKnife(RightWeapon.ITEM_ID))
                    {
                        hasRightKnife = true;
                    }
                }

                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
                {
                    if (Database.ItemType.IsKnife(LeftWeapon.ITEM_ID))
                    {
                        hasLeftKnife = true;
                    }
                }
                if ((hasRightKnife && !hasLeftKnife) || (!hasRightKnife && hasLeftKnife))
                {
                    user.SendSysMesage("You Must Wear Knives In Both Hands To Attack .");
                    return false;
                }
                {
                    InteractQuery AttackPaket = new InteractQuery() { X = _target.X, Y = _target.Y, AtkType = (ushort)MsgAttackPacket.AttackID.Magic, OpponentUID = Attack.OpponentUID, UID = (!user.OnAutoAttack ? Attack.UID : 0) };
                    switch ((Role.Flags.SpellID)Attack.SpellID)
                    {
                        case Role.Flags.SpellID.Sector:
                        case Role.Flags.SpellID.Rectangle:
                        case Role.Flags.SpellID.Circle:

                        case Role.Flags.SpellID.LeftHook:
                        case Role.Flags.SpellID.RightHook:
                        case Role.Flags.SpellID.StraightFist:
                        case Role.Flags.SpellID.WrathoftheEmperor:
                        case Role.Flags.SpellID.FatalSpin:

                        case Role.Flags.SpellID.AirStrike:
                        case Role.Flags.SpellID.EarthSweep:
                        case Role.Flags.SpellID.Kick:

                        case Role.Flags.SpellID.UpSweep:
                        case Role.Flags.SpellID.DownSweep:
                        case Role.Flags.SpellID.Strike:

                        case Role.Flags.SpellID.NormalAttack1:
                        case Role.Flags.SpellID.NormalAttack2:
                        case Role.Flags.SpellID.NormalAttack3:

                        case Role.Flags.SpellID.LeftChop:
                        case Role.Flags.SpellID.RightChop:
                            {

                                List<Role.Flags.SpellID> doSpells = new List<Role.Flags.SpellID>();
                                #region Anger & Horror & Peace OfStomper
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Circle || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Rectangle || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Sector)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.AngerofStomper, out _clientspell))
                                    {

                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.AngerofStomper];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate + 20))
                                        {
                                            doSpells.Add(Role.Flags.SpellID.AngerofStomper);
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HorrorofStomper, out _clientspell))
                                                doSpells.Add(Role.Flags.SpellID.HorrorofStomper);
                                            if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.PeaceofStomper, out _clientspell))
                                                doSpells.Add(Role.Flags.SpellID.PeaceofStomper);
                                        }
                                    }
                                }
                                #endregion
                                #region ScarOfEarth
                                else if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.LeftHook || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.RightHook || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.StraightFist)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ScarofEarth, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.ScarofEarth];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.ScarofEarth);
                                    }
                                }
                                #endregion
                                #region DragonPunch&&DragonRising
                                else if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.LeftHook || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.RightHook || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DragonPunch)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DragonRising, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.DragonRising];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.DragonRising);
                                    }
                                    else
                                    {
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.DragonPunch, out _clientspell) && !user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRising))
                                        {
                                            var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.DragonPunch];
                                            var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                            if (Role.Core.Rate(_DBSpell.Rate))
                                                doSpells.Add(Role.Flags.SpellID.DragonPunch);
                                        }
                                    }
                                }
                                #endregion
                                #region Triple Attack
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.UpSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DownSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Strike)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.TripleAttack, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.TripleAttack];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.TripleAttack);
                                    }
                                }
                                #endregion
                                #region VajraPalm Attack
                                if (Role.Core.Rate(65))
                                {
                                    if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.UpSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DownSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Strike)
                                    {
                                        MsgSpell _clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.VajraPalm, out _clientspell))
                                        {
                                            var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.VajraPalm];
                                            var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                            if (Role.Core.Rate(_DBSpell.Rate))
                                                doSpells.Add(Role.Flags.SpellID.VajraPalm);
                                        }
                                    }
                                }
                                #endregion
                                #region FlowerTouchATK Attack
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.UpSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DownSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Strike)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlowerTouchATK, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.FlowerTouchATK];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.FlowerTouchATK);
                                    }
                                }
                                #endregion
                                #region FlowerTouchATK Attack
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.UpSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DownSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Strike)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FlowerTouch, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.FlowerTouch];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.FlowerTouch);
                                    }
                                }
                                #endregion
                                #region VioletBowl Attack
                                if (Role.Core.Rate(65))
                                {
                                    if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.UpSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.DownSweep || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.Strike)
                                    {
                                        MsgSpell _clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.VioletBowl, out _clientspell))
                                        {
                                            var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.VioletBowl];
                                            var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                            if (Role.Core.Rate(_DBSpell.Rate))
                                                doSpells.Add(Role.Flags.SpellID.VioletBowl);
                                        }
                                    }
                                }
                                #endregion
                                #region Windstorm
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.LeftChop || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.RightChop)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Windstorm, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.Windstorm];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate + 20))
                                            doSpells.Add(Role.Flags.SpellID.Windstorm);
                                    }
                                }
                                #endregion
                                #region Triple Attack
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.LeftChop || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.RightChop)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Windstorm, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.Windstorm];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                            doSpells.Add(Role.Flags.SpellID.Windstorm);
                                    }
                                }
                                #endregion
                                #region WindstormBattleaxe
                                if ((Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.NormalAttack1 || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.NormalAttack2 || (Role.Flags.SpellID)Attack.SpellID == Role.Flags.SpellID.NormalAttack3)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.WindstormBattleaxe, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.WindstormBattleaxe];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate((user.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage) ? 50 : 42)))//5 => (_DBSpell.Damage2 / 10)
                                            doSpells.Add(Role.Flags.SpellID.WindstormBattleaxe);
                                    }
                                }
                                #endregion
                                AttackPaket.OpponentUID = _target.UID;

                                foreach (var spell in doSpells)
                                {
                                    AttackPaket.SpellID = (ushort)spell;
                                    user.Player.RandomSpell = AttackPaket.SpellID;
                                    MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket, true);
                                }
                                return doSpells.Count > 0;
                            }
                    }
                    switch ((Role.Flags.SpellIDDune)Attack.SpellID)
                    {
                        case Role.Flags.SpellIDDune.TempestStrike:
                        case Role.Flags.SpellIDDune.WandererNormalATK:
                            {
                                List<Role.Flags.SpellIDDune> doSpells = new List<Role.Flags.SpellIDDune>();
                                #region TempestStrike & WandererNormalATK
                                if ((Role.Flags.SpellIDDune)Attack.SpellID == Role.Flags.SpellIDDune.TempestStrike || (Role.Flags.SpellIDDune)Attack.SpellID == Role.Flags.SpellIDDune.WandererNormalATK)
                                {
                                    MsgSpell _clientspell;
                                    if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellIDDune.TempestStrike, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellIDDune.TempestStrike];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                        {
                                            doSpells.Add(Role.Flags.SpellIDDune.TempestStrike);
                                        }
                                    }
                                    else if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellIDDune.WandererNormalATK, out _clientspell))
                                    {
                                        var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellIDDune.WandererNormalATK];
                                        var _DBSpell = arrayspells[(ushort)Math.Min(arrayspells.Count - 1, (int)_clientspell.Level)];
                                        if (Role.Core.Rate(_DBSpell.Rate))
                                        {
                                            doSpells.Add(Role.Flags.SpellIDDune.WandererNormalATK);
                                        }
                                    }
                                }
                                #endregion
                                AttackPaket.OpponentUID = _target.UID;
                                foreach (var spell in doSpells)
                                {
                                    AttackPaket.SpellID = (ushort)spell;
                                    user.Player.RandomSpell = AttackPaket.SpellID;
                                    MsgServer.MsgAttackPacket.ProcescMagic(user, stream, AttackPaket, true);
                                }
                                return doSpells.Count > 0;

                            }
                    }
                }
            }
            return false;
        }
        public unsafe static bool Check(InteractQuery Attack, ServerSockets.Packet stream, Client.GameClient client, Role.IMapObj Target)
        {
            int rate = 120;
            if (Database.AtributesStatus.IsPirate(client.Player.Class))
            {
                rate = 100;
            }
            #region FatalStrike
            if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
            {
                if (Target.ObjType == Role.MapObjectType.Monster)
                    client.Shift(Target.X, Target.Y, stream);
            }
            #endregion

            #region Deterrence&&Might
            byte rate2 = 0;
            byte Deterrence = 0;
            byte Might = 0;

            if (Target.ObjType == Role.MapObjectType.Player)
            {
                if ((Target as Role.Player).Owner.Rune.IsEquipped("Deterrence", ref Deterrence))
                {
                    switch (Deterrence)
                    {
                        case 1: rate2 = 1; break;
                        case 2: rate2 = 2; break;
                        case 3: rate2 = 3; break;
                        case 4: rate2 = 4; break;
                        case 5: rate2 = 5; break;
                        case 6: rate2 = 6; break;
                        case 7: rate2 = 7; break;
                        case 8: rate2 = 8; break;
                        case 9: rate2 = 10; break;
                    }
                    rate -= rate2;
                }
                if ((Target as Role.Player).Owner.Rune.IsEquipped("Might", ref Might))
                {
                    switch (Deterrence)
                    {
                        case 1: rate2 = 1; break;
                        case 2: rate2 = 2; break;
                        case 3: rate2 = 3; break;
                        case 4: rate2 = 4; break;
                        case 5: rate2 = 5; break;
                        case 6: rate2 = 6; break;
                        case 7: rate2 = 7; break;
                        case 8: rate2 = 8; break;
                        case 9: rate2 = 10; break;
                    }
                    rate += rate2;
                }
            }
            #endregion

            rate += (byte)client.Player.SongofPhoenixPower;
            #region FantasyKnack
            if (client.Player.ContainFlag(MsgUpdate.Flags.DivingDragon))
            {
                Database.MagicType.Magic DragonSlash = Pool.Magic[(ushort)Role.Flags.SpellID.DragonSlash][(ushort)client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonSlash].Level];
                rate += (byte)DragonSlash.Damage2;
            }

            if (Target.ObjType == Role.MapObjectType.Player)
            {
                if ((Target as Role.Player).Owner.Player.ContainFlag(MsgUpdate.Flags.FantasyKnack))
                {
                    Database.MagicType.Magic FantasyKnack = Pool.Magic[(ushort)Role.Flags.SpellID.FantasyKnack][(ushort)(Target as Role.Player).Owner.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.FantasyKnack].Level];
                    rate -= FantasyKnack.DamageOnHuman;
                }

            }
            #endregion

            if (Role.Core.Rate(rate))
            {

                #region ActiveWeaponHundredWeapons
                if (client.HundredWeapons.Unlocked && client.HundredWeapons.Valid())
                {
                    ushort nextSpell = client.HundredWeapons.GetNextSpell();
                    if (Database.ItemType.IsTrojanEpicWeapon(client.Equipment.RightWeapon)
                        || Database.ItemType.IsScepter(client.Equipment.RightWeapon)
                        || Database.ItemType.IsClub(client.Equipment.RightWeapon)
                        || Database.ItemType.IsTrojanEpicWeapon(client.Equipment.LeftWeapon)
                        || Database.ItemType.IsScepter(client.Equipment.LeftWeapon)
                        || Database.ItemType.IsClub(client.Equipment.LeftWeapon))
                    {
                        if (nextSpell > 0)
                        {
                            InteractQuery AttackPacket = new InteractQuery();
                            AttackPacket.OpponentUID = Attack.OpponentUID;
                            AttackPacket.UID = Attack.UID;
                            AttackPacket.X = Target.X;
                            AttackPacket.Y = Target.Y;
                            AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                            AttackPacket.OpponentUID = Target.UID;

                            AttackPacket.SpellID = nextSpell;
                            client.Player.RandomSpell = AttackPacket.SpellID;
                            MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                            if (client.HundredWeapons.TODO_Spells.Count == 0 && client.Player.ContainFlag(MsgUpdate.Flags.ActiveWeapon))
                                client.Player.RemoveFlag(MsgUpdate.Flags.ActiveWeapon);
                            return true;
                        }

                        else if (client.HundredWeapons.TODO_Spells.Count == 0 && !client.Player.ContainFlag(MsgUpdate.Flags.ActiveWeapon))
                        {
                            if (Role.Core.Rate(5))//Unequipped Ones
                            {
                                var weapons = client.HundredWeapons.Objects.Values.Where(i => i.BitVector.bits[0] == 0).ToArray();
                                if (weapons.Length > 0)
                                    client.HundredWeapons.TODO_Spells = new List<ushort>() { weapons[Program.GetRandom.Next(0, weapons.Length)].DBSpell.ID };
                            }
                            if (client.HundredWeapons.StageActivated(3) && Role.Core.Rate((byte)client.HundredWeapons.GetWeaponRate(3).FirstOrDefault().Key + 10) && client.HundredWeapons.CanTrigger(3))
                            {
                                client.HundredWeapons.TODO_Spells = new List<ushort>()
                           {
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(5)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(6)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(7)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(8)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(9)).DBSpell.ID
                           };
                                client.Player.AddFlag(MsgUpdate.Flags.ActiveWeapon, 30, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveWeapon, 0, 0, ((uint)client.HundredWeapons.TryGetItem(5).WeaponSubtype) * 10000 + 510, MsgUpdate.DataType.ArchiveSkill, true);

                                client.HundredWeapons.Trigger(3);
                            }
                            else if (client.HundredWeapons.StageActivated(2) && Role.Core.Rate((byte)client.HundredWeapons.GetWeaponRate(2).FirstOrDefault().Key) && client.HundredWeapons.CanTrigger(2))
                            {
                                client.HundredWeapons.TODO_Spells = new List<ushort>()
                           {
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(2)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(3)).DBSpell.ID,
                                client.HundredWeapons.Objects.Values.FirstOrDefault(i => i.BitVector.Contain(4)).DBSpell.ID
                           };
                                client.Player.AddFlag(MsgUpdate.Flags.ActiveWeapon, 20, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveWeapon, 0, 0, ((uint)client.HundredWeapons.TryGetItem(2).WeaponSubtype) * 10000 + 205, MsgUpdate.DataType.ArchiveSkill, true);
                                client.HundredWeapons.Trigger(2);
                            }
                            else if (client.HundredWeapons.StageActivated(1) && Role.Core.Rate((byte)client.HundredWeapons.GetWeaponRate(1).FirstOrDefault().Key) && client.HundredWeapons.CanTrigger(4))
                            {
                                client.HundredWeapons.TODO_Spells = new List<ushort>() { client.HundredWeapons.TryGetItem(1).DBSpell.ID };
                                client.Player.AddFlag(MsgUpdate.Flags.ActiveWeapon, 10, true);
                                client.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveWeapon, 0, 0, ((uint)client.HundredWeapons.TryGetItem(1).WeaponSubtype) * 10000, MsgUpdate.DataType.ArchiveSkill, true);
                            }
                            if (client.HundredWeapons.TODO_Spells.Count == 0 && client.Player.ContainFlag(MsgUpdate.Flags.ActiveWeapon))
                            {
                                client.Player.RemoveFlag(MsgUpdate.Flags.ActiveWeapon);
                            }
                        }
                    }
                }
                #endregion
                if (Database.ItemType.IsScepter(client.Equipment.LeftWeapon))
                {
                    if (Role.Core.Rate(50))
                    {
                        var AttackPacket = new InteractQuery();
                        AttackPacket.OpponentUID = Attack.OpponentUID;
                        AttackPacket.UID = Attack.UID;
                        AttackPacket.X = Target.X;
                        AttackPacket.Y = Target.Y;
                        AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                        AttackPacket.OpponentUID = Target.UID;
                        AttackPacket.SpellID = (ushort)Role.Flags.SpellID.Celestial;
                        client.Player.RandomSpell = AttackPacket.SpellID;
                        MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                        return true;
                    }
                }
                if (AttackHandler.Calculate.Base.Rate(10) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloomofDeath))
                {
                    InteractQuery AttackPacket = new InteractQuery();
                    AttackPacket.OpponentUID = Attack.OpponentUID;
                    AttackPacket.UID = Attack.UID;
                    AttackPacket.X = Target.X;
                    AttackPacket.Y = Target.Y;
                    AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                    AttackPacket.OpponentUID = Target.UID;

                    AttackPacket.SpellID = (ushort)Role.Flags.SpellID.BloomofDeath;
                    client.Player.RandomSpell = AttackPacket.SpellID;
                    MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                    return true;
                }
                #region IsNinja(Mele)
                if (Database.AtributesStatus.IsNinja(client.Player.Class) && !Database.ItemType.IsScepter(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsScepter(client.Equipment.LeftWeapon) && !Database.ItemType.IsKnife(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsKnife(client.Equipment.LeftWeapon) && !Database.ItemType.IsBow(client.Equipment.RightWeapon))
                {
                    if (Database.ItemType.IsNinjaEpicWeapon(client.Equipment.RightWeapon)
                        || Database.ItemType.IsKatana(client.Equipment.RightWeapon)
                        || Database.ItemType.IsScythe(client.Equipment.RightWeapon))
                    {
                        ushort SpellIDn = 0;
                        if (!Role.Core.Rate(10))
                        {
                            SpellIDn = client.MyNinja.GetSkill();
                            if (SpellIDn != 0)
                            {
                                InteractQuery AttackPaket = new InteractQuery();
                                AttackPaket.OpponentUID = Target.UID;
                                AttackPaket.UID = Attack.UID;
                                AttackPaket.X = Target.X;
                                AttackPaket.Y = Target.Y;
                                AttackPaket.SpellID = (ushort)SpellIDn;
                                client.Player.RandomSpell = AttackPaket.SpellID;
                                MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                                MsgServer.MsgAttackPacket.CreateAutoAtack(AttackPaket, client, true);
                                return true;
                            }
                        }

                    }
                }
                #endregion
                #region MyArchives(Skills)
                ushort SpellId = 0;
                SpellId = client.MyArchives.GetSkill(Target);
                if (SpellId != 0)
                {
                    InteractQuery AttackPaket = new InteractQuery();
                    AttackPaket.OpponentUID = Attack.OpponentUID;
                    AttackPaket.UID = Attack.UID;
                    AttackPaket.X = Target.X;
                    AttackPaket.Y = Target.Y;
                    AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                    AttackPaket.OpponentUID = Target.UID;
                    AttackPaket.SpellID = SpellId;
                    client.Player.RandomSpell = AttackPaket.SpellID;
                    MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);

                    return true;
                }
                if (AtributesStatus.IsMonk(client.Player.Class) && !Database.ItemType.IsKnife(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsKnife(client.Equipment.LeftWeapon) && !Database.ItemType.IsBow(client.Equipment.RightWeapon))              
                {
                    ushort SpellMonk = 0;
                    SpellMonk = client.MyArchives.GetSkillMonk(Target);
                    if (SpellMonk != 0)
                    {
                        InteractQuery AttackPaket = new InteractQuery();
                        AttackPaket.OpponentUID = Attack.OpponentUID;
                        AttackPaket.UID = Attack.UID;
                        AttackPaket.X = Target.X;
                        AttackPaket.Y = Target.Y;
                        AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                        AttackPaket.OpponentUID = Target.UID;
                        AttackPaket.SpellID = SpellMonk;
                        client.Player.RandomSpell = AttackPaket.SpellID;
                        MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                    }
                }
                if (AtributesStatus.IsLee(client.Player.Class) && !Database.ItemType.IsScepter(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsScepter(client.Equipment.LeftWeapon) && !Database.ItemType.IsKnife(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsKnife(client.Equipment.LeftWeapon) && !Database.ItemType.IsBow(client.Equipment.RightWeapon))
                {
                    ushort SpellLee = 0;
                    SpellLee = client.MyArchives.GetSkillLee(Target);
                    if (SpellLee != 0)
                    {
                        InteractQuery AttackPaket = new InteractQuery();
                        AttackPaket.OpponentUID = Attack.OpponentUID;
                        AttackPaket.UID = Attack.UID;
                        AttackPaket.X = Target.X;
                        AttackPaket.Y = Target.Y;
                        AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                        AttackPaket.OpponentUID = Target.UID;
                        AttackPaket.SpellID = SpellLee;
                        client.Player.RandomSpell = AttackPaket.SpellID;
                        MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                        return true;

                    }
                }
                if (AtributesStatus.IsPirate(client.Player.Class) && !Database.ItemType.IsScepter(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsScepter(client.Equipment.LeftWeapon) && !Database.ItemType.IsKnife(client.Equipment.RightWeapon)
                    && !Database.ItemType.IsKnife(client.Equipment.LeftWeapon) && !Database.ItemType.IsBow(client.Equipment.RightWeapon))             
                {
                    ushort[] Spell = new ushort[] { 21670, 21680, 21690, 21700, 22070, 22080, 22090, 22100, 22110 };
                    for (int xx = 0; xx < Spell.Length; xx++)
                    {
                        ushort p = Spell[Program.GetRandom.Next(0, Spell.Length)];
                        if (PirateSkills.ExecuteSkills(client, p, client.Player.UID, Target.UID, Target.X, Target.Y))
                            return true;
                    }
                }
                #endregion
            }

            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && client.Player.MainFlag != Role.Player.MainFlagType.OnMeleeAttack)
                return false;


            if (Role.Core.Rate(rate) && !client.Player.ContainFlag(MsgUpdate.Flags.HeavensWrath))
            {
                if (client.Equipment.RightWeapon != 0)
                {
                    if (client.Equipment.RightWeaponEffect != Role.Flags.ItemEffect.None && Target.ObjType != Role.MapObjectType.SobNpc)
                    {
                        if (Calculate.Base.Rate(20) && !client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                        {
                            client.Player.AttackStamp = new DateTime();

                            InteractQuery AttackPaket = new InteractQuery();

                            AttackPaket.OpponentUID = Attack.OpponentUID;
                            AttackPaket.UID = Attack.UID;
                            AttackPaket.X = Target.X;
                            AttackPaket.Y = Target.Y;

                            if (client.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison)
                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.Poison;
                            else if (client.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.MP)
                                AttackPaket.SpellID = (ushort)Role.Flags.SpellID.EffectMP;

                            client.Player.RandomSpell = AttackPaket.SpellID;

                            AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;

                            MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket);

                            return true;
                        }
                    }
                    uint rightWeapon = client.Equipment.RightWeapon;
                    ushort wep1subyte = (ushort)(rightWeapon / 1000), wep2subyte = 0;
                    bool doWep1Spell = false, doWep2Spell = false;
                    ushort wep1spellid = 0, wep2spellid = 0;
                    if (wep1subyte == 421)
                        wep1subyte--;
                    List<ushort> WeaponSpells;
                    if (Pool.WeaponSpells.ContainsKey(wep1subyte))
                    {
                        WeaponSpells = Pool.WeaponSpells[wep1subyte].ToList();
                        if (!Database.ItemType.IsTrojanEpicWeapon(client.Equipment.LeftWeapon) || !Database.ItemType.IsTrojanEpicWeapon(client.Equipment.RightWeapon))
                            WeaponSpells.Remove((ushort)Role.Flags.SpellID.MortalStrike);
                        if (WeaponSpells.Count == 0)
                            wep1spellid = WeaponSpells[0];
                        else

                            wep1spellid = WeaponSpells[Pool.GetRandom.Next(0, WeaponSpells.Count)];
                    }
                    if (Database.ItemType.IsComprehensionRune(client.Rune.EquippedRedRune) && client.Rune.EquippedRedRune.SocketProgress != 0)
                        wep1spellid = (ushort)client.Rune.EquippedRedRune.SocketProgress;


                    if (client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                    {
                        MsgSpell spell;
                        Database.MagicType.Magic DBSpell;
                        Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                        if (client.MySpells.ClientSpells.TryGetValue(wep1spellid, out spell) && Pool.Magic.TryGetValue(wep1spellid, out DBSpells) && DBSpells.TryGetValue(spell.Level, out DBSpell))
                        {
                            if (DBSpell.Rate > 0 && DBSpell.Rate <= 100)
                            {
                                doWep1Spell = Calculate.Base.Rate(DBSpell.Rate);
                                if (doWep1Spell)
                                {
                                    doWep1Spell = false;
                                }

                            }
                        }
                    }
                    if (!doWep1Spell)
                    {
                        if (client.Equipment.LeftWeapon != 0)
                        {
                            uint leftWeapon = client.Equipment.LeftWeapon;
                            wep2subyte = (ushort)(leftWeapon / 1000);
                            if (Pool.WeaponSpells.ContainsKey(wep2subyte))
                                wep2spellid = Pool.WeaponSpells[wep2subyte][Pool.GetRandom.Next(0, Pool.WeaponSpells[wep2subyte].Count)];
                            if (wep2spellid == (ushort)Role.Flags.SpellID.Blackspot && Target.ObjType == Role.MapObjectType.SobNpc)
                                wep2spellid = 0;
                            if (Database.ItemType.IsComprehensionRune(client.Rune.EquippedRedRune) && client.Rune.EquippedRedRune.SocketProgress != 0)
                                wep2spellid = (ushort)client.Rune.EquippedRedRune.SocketProgress;
                            if (client.MySpells.ClientSpells.ContainsKey(wep2spellid))
                            {
                                MsgSpell spell;
                                Database.MagicType.Magic DBSpell;
                                Dictionary<ushort, Database.MagicType.Magic> DBSpells;
                                if (client.MySpells.ClientSpells.TryGetValue(wep2spellid, out spell) && Pool.Magic.TryGetValue(wep2spellid, out DBSpells) && DBSpells.TryGetValue(spell.Level, out DBSpell))
                                {
                                    if (DBSpell.Rate > 0 && DBSpell.Rate <= 100)
                                        doWep2Spell = Calculate.Base.Rate(DBSpell.Rate);
                                }
                            }
                            if (doWep2Spell)
                            {
                                if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                {
                                    if (Target.ObjType == Role.MapObjectType.Monster)
                                        client.Shift(Target.X, Target.Y, stream);
                                }
                                MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, client);
                                InteractQuery AttackPaket = new InteractQuery();
                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                AttackPaket.UID = Attack.UID;
                                AttackPaket.X = Target.X;
                                AttackPaket.Y = Target.Y;
                                AttackPaket.SpellID = wep2spellid;
                                AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                                client.Player.RandomSpell = wep2spellid;

                                if (!CheckMelee(AttackPaket, stream, client, Target))
                                    MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                                #region CruelShade
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CruelShade) && Target.ObjType != Role.MapObjectType.SobNpc)
                                {
                                    Database.MagicType.Magic CruelShade = Pool.Magic[(ushort)Role.Flags.SpellID.CruelShade][(ushort)client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.CruelShade].Level];
                                    if (Calculate.Base.Rate(CruelShade.Rate))
                                    {
                                        InteractQuery AttackPacket = new InteractQuery();
                                        AttackPacket.OpponentUID = Attack.OpponentUID;
                                        AttackPacket.UID = Attack.UID;
                                        AttackPacket.X = Target.X;
                                        AttackPacket.Y = Target.Y;
                                        AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                                        AttackPacket.OpponentUID = Target.UID;
                                        AttackPacket.SpellID = (ushort)Role.Flags.SpellID.CruelShade;
                                        client.Player.RandomSpell = AttackPacket.SpellID;
                                        MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                                    }
                                }
                                #endregion
                                return true;
                            }
                            else
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                                    return false;
                                if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                                {
                                    if (Target.ObjType == Role.MapObjectType.Monster)
                                        client.Shift(Target.X, Target.Y, stream);
                                }

                                MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, client);
                                InteractQuery AttackPaket = new InteractQuery();
                                AttackPaket.OpponentUID = Attack.OpponentUID;
                                AttackPaket.UID = Attack.UID;
                                AttackPaket.X = Target.X;
                                AttackPaket.Y = Target.Y;
                                AttackPaket.SpellID = wep1spellid;
                                client.Player.RandomSpell = wep1spellid;
                                AttackPaket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;

                                if (!CheckMelee(AttackPaket, stream, client, Target))
                                    MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                                #region CruelShade
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CruelShade) && Target.ObjType != Role.MapObjectType.SobNpc)
                                {
                                    Database.MagicType.Magic CruelShade = Pool.Magic[(ushort)Role.Flags.SpellID.CruelShade][(ushort)client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.CruelShade].Level];
                                    if (Calculate.Base.Rate(CruelShade.Rate))
                                    {
                                        InteractQuery AttackPacket = new InteractQuery();
                                        AttackPacket.OpponentUID = Attack.OpponentUID;
                                        AttackPacket.UID = Attack.UID;
                                        AttackPacket.X = Target.X;
                                        AttackPacket.Y = Target.Y;
                                        AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                                        AttackPacket.OpponentUID = Target.UID;
                                        AttackPacket.SpellID = (ushort)Role.Flags.SpellID.CruelShade;
                                        client.Player.RandomSpell = AttackPacket.SpellID;
                                        MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                                    }
                                }
                                #endregion
                                return true;
                            }
                        }
                        else
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey(wep1spellid))
                                return false;
                            if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                            {
                                if (Target.ObjType == Role.MapObjectType.Monster)
                                    client.Shift(Target.X, Target.Y, stream);
                            }

                            MsgServer.MsgAttackPacket.CreateAutoAtack(Attack, client);
                            InteractQuery AttackPakets = new InteractQuery();
                            AttackPakets.OpponentUID = Attack.OpponentUID;
                            AttackPakets.UID = Attack.UID;
                            AttackPakets.X = Target.X;
                            AttackPakets.Y = Target.Y;
                            AttackPakets.SpellID = wep1spellid;
                            client.Player.RandomSpell = wep1spellid;
                            AttackPakets.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;

                            if (!CheckMelee(AttackPakets, stream, client, Target))
                                MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPakets, true);
                            #region CruelShade
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CruelShade) && Target.ObjType != Role.MapObjectType.SobNpc)
                            {
                                Database.MagicType.Magic CruelShade = Pool.Magic[(ushort)Role.Flags.SpellID.CruelShade][(ushort)client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.CruelShade].Level];
                                if (Calculate.Base.Rate(CruelShade.Rate))
                                {
                                    InteractQuery AttackPacket = new InteractQuery();
                                    AttackPacket.OpponentUID = Attack.OpponentUID;
                                    AttackPacket.UID = Attack.UID;
                                    AttackPacket.X = Target.X;
                                    AttackPacket.Y = Target.Y;
                                    AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                                    AttackPacket.OpponentUID = Target.UID;
                                    AttackPacket.SpellID = (ushort)Role.Flags.SpellID.CruelShade;
                                    client.Player.RandomSpell = AttackPacket.SpellID;
                                    MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                                }
                            }
                            #endregion
                            return true;
                        }

                    }
                }
                if (Calculate.Base.Rate(40) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MonsterHunter))
                {
                    InteractQuery AttackPacket = new InteractQuery();
                    AttackPacket.OpponentUID = Attack.OpponentUID;
                    AttackPacket.UID = Attack.UID;
                    AttackPacket.X = Target.X;
                    AttackPacket.Y = Target.Y;
                    AttackPacket.AtkType = (ushort)MsgAttackPacket.AttackID.Magic;
                    AttackPacket.OpponentUID = Target.UID;

                    AttackPacket.SpellID = (ushort)Role.Flags.SpellID.MonsterHunter;
                    client.Player.RandomSpell = AttackPacket.SpellID;
                    MsgServer.MsgAttackPacket.ProcescMagic(client, stream, AttackPacket, true);
                    return true;
                }
            }
            return false;
        }
    }
}