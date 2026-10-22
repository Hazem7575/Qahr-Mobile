using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
// The Class Edit By [ Mohy ElDin - محي الدين ]
namespace Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanUseSpell
    {
        public unsafe static bool Verified(InteractQuery Attack, Client.GameClient client, Dictionary<ushort, Database.MagicType.Magic> DBSpells
            , out MsgSpell ClientSpell, out  Database.MagicType.Magic Spell)
        {
            try
            {
                foreach (var sub in Pool.WeaponSpells)
                    if (sub.Value.Contains(Attack.SpellID))
                    {
                        if (client.Player.RandomSpell != Attack.SpellID)
                        {
                            ClientSpell = default(MsgSpell);
                            Spell = default(Database.MagicType.Magic);
                            return false;
                        }
                        client.Player.RandomSpell = 0;
                        break;
                    }
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                {
                    if (DBSpells.TryGetValue(ClientSpell.Level, out Spell))
                    {
                        if (Spell.Type == Database.MagicType.MagicSort.DirectAttack || Spell.Type == Database.MagicType.MagicSort.Attack)
                        {
                            if (!client.IsInSpellRange(Attack.OpponentUID, (byte)Spell.Range))
                            {
                                ClientSpell = default(MsgSpell);
                                Spell = default(Database.MagicType.Magic);
                                return false;
                            }
                        }

                        uint IncreaseSpellStamina = 0;
                        if (client.Player.ContainFlag(MsgUpdate.Flags.ScurvyBomb))
                            IncreaseSpellStamina += 6;
                        //if (client.Player.ContainFlag(MsgUpdate.Flags.DragonPierceStamina))
                        //{
                        //    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPierce))
                        //    {
                        //        Database.MagicType.Magic DragonPierces = Pool.Magic[(ushort)Role.Flags.SpellID.DragonPierce][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.DragonPierce].Level];
                        //        IncreaseSpellStamina += (uint)DragonPierces.Damage3;
                        //    }
                        //}
                        if (ClientSpell.UseSpellSoul > 0)
                            client.OnSoulSpell = ClientSpell.ID;
                        else
                            client.OnSoulSpell = 0;
                        #region BenefitShower
                        MsgSpell user_spell = null;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Enums.SpellID.BenefitShower))
                        {
                            if (!client.Player.ContainFlag((MsgUpdate.Flags)Role.Enums.Flag.BenefitShower))
                            {
                                if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Enums.SpellID.BenefitShower, out user_spell))
                                {
                                    var Spells = Pool.Magic[user_spell.ID][user_spell.Level];
                                    int rate = Spells.Rate;
                                    if (client.Player.ContainFlag((MsgUpdate.Flags)Role.Enums.Flag.DivineArrival))
                                        rate += (int)(Spells.Damage / 10);
                                    if (MsgYuanshen.NewRate(rate))
                                    {
                                        var InteractQuery = new InteractQuery();
                                        InteractQuery.UID = client.Player.UID;
                                        InteractQuery.SpellID = user_spell.ID;
                                        InteractQuery.SpellLevel = user_spell.Level;
                                        InteractQuery.X = 0;
                                        InteractQuery.Y = 0;
                                        InteractQuery.OpponentUID = 0;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            MsgServer.MsgAttackPacket.ProcescMagic(client, stream, InteractQuery, true);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        if (client.Player.Map != 1039)
                        {
                            if (Spell.UseStamina + IncreaseSpellStamina > client.Player.Stamina)
                                return false;
                            else
                            {
                                #region Redcurse
                                if (client.MyArchives.isOpen(Role.Instance.Archives.TypeID.Redcurse) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PowerDash))
                                {
                                    Database.MagicType.Magic PowerDash = Pool.Magic[(ushort)Role.Flags.SpellID.PowerDash][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.PowerDash].Level];
                                    IncreaseSpellStamina -= (uint)PowerDash.Damage2;
                                }
                                #endregion
                                if ((ushort)(Spell.UseStamina + IncreaseSpellStamina) > 0)
                                {
                                    client.Player.Stamina -= (ushort)(Spell.UseStamina + IncreaseSpellStamina);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                                    }
                                }
                            }
                            int useMana = Spell.UseMana;
                            byte itemLevel = 0;
                            if (client.Rune.IsEquipped("MPMaster", ref itemLevel))
                            {
                                double percent = itemLevel * 10;
                                if (itemLevel == 2) percent = 15;
                                else if (itemLevel == 3) percent = 25;
                                else if (itemLevel == 7) percent = 75;
                                else if (itemLevel == 8) percent = 85;
                                useMana -= (int)((double)useMana * percent / 100d);
                            }

                            if (useMana > client.Player.Mana)
                                return false;
                            else
                            {
                                if (useMana > 0)
                                {
                                    client.Player.Mana -= (ushort)useMana;
                                }
                            }
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Absolution))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Absolution);


                        if (Spell.IsSpellWithColdTime && Spell.ID != (ushort)Role.Flags.SpellID.HeavensWrath && !client.Player.ContainFlag(MsgUpdate.Flags.CannonBarrage) && !client.Player.ContainFlag(MsgUpdate.Flags.BlackbeardsRage))
                        {
                            DateTime now = DateTime.Now;
                            if (ClientSpell.ColdTime > now)
                            {
                                if (!ClientSpell.IsSpellWithColdTime)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                ushort[] Spellsss = new ushort[] {25590, 303, 304, 305, 400, 401, 402, 26040, 26050, 26060, 26100, 26110, 26120, 26130, 26170, 25610, 25620, 25640, 25650, 25670, 25680, 25690, 25700, 25710, 25720, 25780, 25900, 25910, 12830, 12840, 12860, 12890, 12870, 12930, 12940, 12950, 12960, 12970, 12980, 12990, 13000, 13020, 13030, 13040, 13050, 13060, 13070, 13080, 13090, 13190, 12280, 14440, 14470, 14530, 15350, 15380, 15410, 15530, 15560, 15590, 15470, 10800, 10700, 10800, 14220, 14380, 15620, 20300, 20290, 22830, 20250, 20240, 20230, 20280, 20270, 14500, 20320, 20390, 20350, 22850, 1120, 20370, 15440, 14190, 15500, 12680, 10400, 10425, 10430, 14160, 6004, 6003, 6002, 6001, 11190, 11180, 11170, 11160, 11130, 11120, 11060, 11050, 11040, 11030, 11070, 14710, 14680, 21540, 21720, 21490, 21510, 21520, 21530, 21550, 21560, 21570, 21580, 21590, 21600, 21690, 21610, 21620, 21630, 21640, 21650, 21660, 21670, 21680, 21700, 21710, 21730, 21740, 21750, 21760, 21770, 21780, 21790, 21800, 21810, 21820, 21920, 21930, 22040, 22050, 22060, 22070, 22080, 22090, 22100, 22110, 22020, 22030, 12550, 25170, 25230, 25250, 25130, 16520, 16530, 16540, 16550, 16400, 16410, 16310, 16320, 16440, 16450, 16460, 16470, 16500, 16510, 16420, 16430, 16330, 16340, 16480, 16490, 16360, 16370, 16380, 16350, 16390 };
                                if (Spellsss.Contains(Spell.ID))
                                {
                                    #region (Ninja)
                                    Role.Instance.Ninja.Item item;
                                    if (Spell.ID == (ushort)Role.Flags.SpellID.WildFireball)
                                    {
                                        if (client.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.WildSigilRapid, out item))
                                            ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - (int)(Role.Instance.Ninja.CoolDownChange[item.Level] * 1000));
                                        else ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                                    }
                                    else if (Spell.ID == (ushort)Role.Flags.SpellID.FlameofDestruction)
                                    {
                                        if (client.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.FlameSigilScorch, out item))
                                            ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - (int)(Role.Instance.Ninja.CoolDownChange[item.Level] * 1000));
                                        else ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                                    }
                                    else if (Spell.ID == (ushort)Role.Flags.SpellID.DustDetachment)
                                    {
                                        if (client.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.DustSigilExtinction, out item))
                                            ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - (int)(Role.Instance.Ninja.CoolDownChange[item.Level] * 1000));
                                        else ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                                    }
                                    else if (Spell.ID == (ushort)Role.Flags.SpellID.SickleWind)
                                    {
                                        if (client.MyNinja.TryGetValueEquip(Role.Instance.Ninja.ItemType.SickleSigilGallop, out item))
                                            ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - (int)(Role.Instance.Ninja.CoolDownChange[item.Level] * 1000));
                                        else ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                                    }
                                    else ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);

                                    #endregion
                                }
                                else if (client.Player.ContainFlag(MsgUpdate.Flags.WaveBreak))
                                {
                                    if (Spell.ID == 22040 || Spell.ID == 22050 || Spell.ID == 22060)
                                    {
                                        ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - 1000);
                                    }
                                }
                                else if (Spell.ID == (ushort)Role.Flags.SpellID.CrackingSwipe)
                                {
                                    if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuanniDominance))
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            var AttackPaket = new InteractQuery()
                                            {
                                                X = client.Player.X,
                                                Y = client.Player.Y,
                                                AtkType = (ushort)MsgAttackPacket.AttackID.Magic,
                                                UID = client.Player.UID,
                                                SpellID = (ushort)Role.Flags.SpellID.DragonRoar,
                                            };
                                            MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);
                                            Game.MsgServer.MsgSpell ClientSpells;
                                            if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.SuanniDominance, out ClientSpells))
                                            {
                                                var Magic = Pool.Magic[ClientSpells.ID][ClientSpells.Level];
                                                if (Spell.ID == (ushort)Role.Flags.SpellID.SuanniRoar)
                                                {
                                                    ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime - Magic.Duration * 1000);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ClientSpell.IsSpellWithColdTime = true;
                                    ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                                }
                            }
                        }
                        return true;
                    }
                }
                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
                return false;
            }
            return false;
        }
    }
}