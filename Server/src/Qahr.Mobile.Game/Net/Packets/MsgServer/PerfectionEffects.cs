//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;

//namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
//{

//    public class PerfectionEffects
//    {
//        public static void CheckEffects(Client.GameClient Attacker, Role.Player Target)
//        {
//            if (Attacker.PerfectionStatus.CalmWind > 0 && AttackHandler.Calculate.Base.Rate(Attacker.PerfectionStatus.CalmWind))
//                CalmWind(Attacker);

//            if (Attacker.PrestigeLevel > Target.Owner.PrestigeLevel && Attacker.PerfectionStatus.DivineGuard > 0)
//                if (AttackHandler.Calculate.Base.Rate(Attacker.PerfectionStatus.DivineGuard) && Attacker.Player.LastDivineGuard < DateTime.Now)
//                    DivineGuard(Attacker);

//            if (Attacker.PerfectionStatus.KillingFlash > 0 && AttackHandler.Calculate.Base.Rate(Attacker.PerfectionStatus.KillingFlash))
//                KillingFlash(Attacker);

//            if (Attacker.PerfectionStatus.DrainingTouch > 0 && AttackHandler.Calculate.Base.Rate(Attacker.PerfectionStatus.DrainingTouch))
//                DrainingTouch(Attacker);

//            if (Target.Owner.PerfectionStatus.MirrorOfSin > 0 && AttackHandler.Calculate.Base.Rate(Target.Owner.PerfectionStatus.MirrorOfSin))
//                MirrorOfSin(Target);

//            if (Attacker.PerfectionStatus.LightOfStamina > 0 && Attacker.PrestigeLevel > Target.Owner.PrestigeLevel && AttackHandler.Calculate.Base.Rate(Target.Owner.PerfectionStatus.LightOfStamina))
//                LightOfStamina(Attacker);

//            if (Target.Owner.PerfectionStatus.BloodSpawn > 0 && AttackHandler.Calculate.Base.Rate(Target.Owner.PerfectionStatus.BloodSpawn))
//                BloodSpawn(Target);
//        }
//        public static unsafe bool ToxinEraser(Role.Player Target)
//        {
//            if (Target.Owner.PerfectionStatus.ToxinEraser > 0 && AttackHandler.Calculate.Base.Rate(Target.Owner.PerfectionStatus.ToxinEraser))
//            {
//                using (var rec = new ServerSockets.RecycledPacket())
//                {
//                    var stream = rec.GetStream();
//                    Target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                    {
//                        Effect = MsgRefineEffect.RefineEffects.ToxinEraserLevel,
//                        Id = Target.UID,
//                        dwParam = Target.UID
//                    }), true);
//                }
//                return true;
//            }
//            return false;
//        }
//        public static unsafe void StraightLife(Role.Player Player, bool SendScreen)
//        {
//            using (var stream = new ServerSockets.RecycledPacket().GetStream())
//            {
//                Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.StraightLife,
//                    Id = Player.UID,
//                    dwParam = Player.UID
//                }), true);

//                byte Conqueror = 0;
//                int Val_Conqueror = 0;
//                if (Player.Owner.Rune.IsEquipped("Conqueror`sBlade", ref Conqueror) || Player.Owner.Rune.IsEquipped("Tough Steel", ref Conqueror))
//                {// test rune ac msg
//                    //Player.Owner.SendSysMesage("Rune Conqueror`sBlade Activated: StraightLife effect", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

//                    switch (Conqueror)
//                    {
//                        case 1: Val_Conqueror = 20; break;
//                        case 2: Val_Conqueror = 25; break;
//                        case 3: Val_Conqueror = 30; break;
//                        case 4: Val_Conqueror = 35; break;
//                        case 5: Val_Conqueror = 40; break;
//                        case 6: Val_Conqueror = 50; break;
//                        case 7: Val_Conqueror = 60; break;
//                        case 8: Val_Conqueror = 70; break;
//                        case 9: Val_Conqueror = 80; break;
//                    }
//                }
//                if (!Role.Core.Rate(Val_Conqueror))
//                {

//                    Player.Revive(stream);
//                }
//            }
//        }
//        private static unsafe void BloodSpawn(Role.Player Target)
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                Target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.BloodSpawn,
//                    Id = Target.UID,
//                    dwParam = Target.UID
//                }), true);
//                bool update = false;
//                if (Target.HitPoints < Target.Owner.Status.MaxHitpoints)
//                {
//                    update = true;
//                    Target.HitPoints = (int)Target.Owner.Status.MaxHitpoints;
//                }
//                if (Target.Mana < Target.Owner.Status.MaxMana)
//                {
//                    update = true;
//                    Target.Mana = (ushort)Target.Owner.Status.MaxMana;
//                }
//                if (update)
//                {
//                    Target.SendUpdateHP();
//                    Target.SendUpdate(stream, Target.Mana, MsgUpdate.DataType.Mana, false);
//                }
//            }
//        }
//        private static unsafe void LightOfStamina(Client.GameClient Attacker)
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                if (Attacker.Player.Stamina < 100)
//                {
//                    Attacker.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                    {
//                        Effect = MsgRefineEffect.RefineEffects.LightOfStamina,
//                        Id = Attacker.Player.UID,
//                        dwParam = Attacker.Player.UID
//                    }), true);
//                    Attacker.Player.Stamina = 100;
//                    Attacker.Player.SendUpdate(stream, Attacker.Player.Stamina, MsgUpdate.DataType.Stamina);
//                }
//            }
//        }
//        private static unsafe void MirrorOfSin(Role.Player Target)
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                Target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.MirrorOfSin,
//                    Id = Target.UID,
//                    dwParam = Target.UID
//                }), true);
//                if (Target.OnXPSkill() == MsgUpdate.Flags.Normal && !Target.ContainFlag(MsgUpdate.Flags.XPList))
//                {
//                    Target.AddFlag(MsgUpdate.Flags.XPList, 20, true);
//                    Target.SendUpdate(stream, 1, MsgServer.MsgUpdate.DataType.XPList);
//                }
//            }
//        }
//        private static unsafe void DrainingTouch(Client.GameClient client)
//        {

//        }
//        private static unsafe void KillingFlash(Client.GameClient client)
//        {

//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.KillingFlash,
//                    Id = client.Player.UID,
//                    dwParam = client.Player.UID
//                }), true);

//                if (!client.Player.ContainFlag(MsgUpdate.Flags.XPList) && client.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
//                    if (Role.Core.Rate(10))
//                    {
//                        client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
//                        client.Player.SendUpdate(stream, 1, MsgServer.MsgUpdate.DataType.XPList);
//                    }
//            }
//        }
//        private static unsafe void DivineGuard(Client.GameClient client)
//        {
//            client.Player.LastDivineGuard = DateTime.Now.AddMinutes(10);

//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.DivineGuard,
//                    Id = client.Player.UID,
//                    dwParam = client.Player.UID
//                }), true);

//                if (!client.Player.ContainFlag(MsgUpdate.Flags.DivineGuard))
//                {
//                    if (Role.Core.Rate(5))
//                    {
//                        client.Player.AddFlag(MsgUpdate.Flags.DivineGuard, 15, true);
//                        client.Player.SendUpdate(stream, MsgUpdate.Flags.DivineGuard, 15, 0, 0, MsgUpdate.DataType.AzureShield);
//                    }
//                }
//            }
//        }
//        private static unsafe void CalmWind(Client.GameClient client)
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                client.Player.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                {
//                    Effect = MsgRefineEffect.RefineEffects.CalmWind,
//                    Id = client.Player.UID,
//                    dwParam = client.Player.UID
//                }), true);
//            }
//        }

//        public static unsafe void Lucky_Lock_Absolute(Role.Player player, Role.Player target, ref MsgSpellAnimation.SpellObj SpellObj, ref bool update)
//        {
//            if (Role.Core.Rate(30))
//            {
//                if (Role.Core.Rate(player.Owner.PerfectionStatus.LuckyStrike + (int)player.Owner.Status.LuckyStrike))
//                {
//                    if (Role.Core.Rate(target.Owner.PerfectionStatus.StrikeLock))
//                    {
//                        if (!(Role.Core.Rate(player.Owner.PerfectionStatus.AbsoluteLuck) && player.Owner.PrestigeLevel < target.Owner.PrestigeLevel))
//                        {
//                            using (var rec = new ServerSockets.RecycledPacket())
//                            {
//                                var stream = rec.GetStream();
//                                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                                {
//                                    Effect = MsgRefineEffect.RefineEffects.StrikeLockLevel,
//                                    Id = player.UID
//                                }), true);
//                            }
//                        }
//                        else
//                        {
//                            using (var rec = new ServerSockets.RecycledPacket())
//                            {
//                                var stream = rec.GetStream();
//                                target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                                {
//                                    Effect = MsgRefineEffect.RefineEffects.AbsoluteLuck,
//                                    Id = player.UID
//                                }), true);
//                            }
//                            SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
//                            SpellObj.Damage = (uint)Calculate.Base.MulDiv((int)SpellObj.Damage, 200, 100);
//                            update = true;
//                        }
//                    }
//                    else
//                    {
//                        using (var rec = new ServerSockets.RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                            {
//                                Effect = MsgRefineEffect.RefineEffects.LuckyStrike,
//                                Id = player.UID
//                            }), true);
//                        }
//                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.LuckyStrike;
//                        SpellObj.Damage = (uint)Calculate.Base.MulDiv((int)SpellObj.Damage, 200, 100);
//                        update = true;
//                    }
//                }
//            }
//        }

//        public static unsafe void InvisibleArrow(Role.Player player, Role.Player target, ref MsgSpellAnimation.SpellObj SpellObj)
//        {
//            if (player.Owner.PerfectionStatus.InvisibleArrow > 0)
//            {
//                if (Role.Core.Rate(player.Owner.PerfectionStatus.InvisibleArrow))
//                {
//                    if (player.Owner.Status.Penetration > 0)

//                        using (var rec = new ServerSockets.RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            target.View.SendView(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
//                            {
//                                Effect = MsgRefineEffect.RefineEffects.InvisbleArrow,
//                                Id = player.UID,
//                                dwParam = player.UID
//                            }), true);
//                        }
//                }
//            }
//        }
//    }
//}
