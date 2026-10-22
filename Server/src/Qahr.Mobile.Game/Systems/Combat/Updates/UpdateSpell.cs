using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler.Updates
{
    public class UpdateSpell
    {
        public unsafe static void CheckUpdate(ServerSockets.Packet stream, Client.GameClient client, InteractQuery Attack, uint Damage, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (Damage == 0)
                return;
            if (DBSpells != null)
            {
                MsgSpell ClientSpell;
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                {
                    ushort firstlevel = ClientSpell.Level;
                    if (ClientSpell.Level < DBSpells.Count - 1)
                    {
                        if (!DBSpells[ClientSpell.Level].isRuneSkill)
                        {
                            if (client.Player.Level >= DBSpells[ClientSpell.Level].NeedLevel)
                            {
                                ClientSpell.Experience += (int)(Damage * Program.ServerConfig.ExpRateSpell);
                                if (ClientSpell.Experience > DBSpells[ClientSpell.Level].Experience)
                                {
                                    ClientSpell.Level++;
                                    ClientSpell.Experience = 0;
                                }
                                if (ClientSpell.PreviousLevel != 0 && ClientSpell.PreviousLevel >= ClientSpell.Level / 2)
                                {
                                    ClientSpell.Level = ClientSpell.PreviousLevel;
                                }
                                try
                                {
                                    if (ClientSpell.Level > firstlevel)
                                        client.SendSysMesage("You increased the spell level!", MsgMessage.ChatMode.TopLeftSystem, MsgMessage.MsgColor.red);
                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                                client.Send(stream.SpellCreate(ClientSpell));
                            }
                        }
                    }
                }
            }
            if (Attack.AtkType == (ushort)MsgAttackPacket.AttackID.Physical || Attack.AtkType == (ushort)MsgAttackPacket.AttackID.Archer || Attack.AtkType == (ushort)MsgAttackPacket.AttackID.Magic)
            {
                uint ProfRightWeapon = client.Equipment.RightWeapon / 1000;
                uint PorfLeftWeapon = client.Equipment.LeftWeapon / 1000;
                if (ProfRightWeapon != 0)
                    client.MyProfs.CheckUpdate(ProfRightWeapon, Damage * Program.ServerConfig.ExpRateProf, stream);
                if (PorfLeftWeapon != 0)
                    client.MyProfs.CheckUpdate(PorfLeftWeapon, Damage * Program.ServerConfig.ExpRateProf, stream);
            }
        }
    }
}
