using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgNewRole
    {

        public static object SynName = new object();


        public static void GetNewRoleInfo(this ServerSockets.Packet msg, out string name, out ushort Body, out uint Class)
        {
            msg.ReadBytes(36);
            name = msg.ReadCString(32);
            msg.ReadBytes(64);

            Body = msg.ReadUInt16();
            Class = msg.ReadUInt16();

        }

        [PacketAttribute(Game.GamePackets.MsgRegister)]
        public unsafe static void CreateCharacter(Client.GameClient client, ServerSockets.Packet stream)
        {
            if ((client.ClientFlag & Client.ServerFlag.CreateCharacter) == Client.ServerFlag.CreateCharacter)
            {
                Client.GameClient INGame;
                if (Pool.GamePoll.TryGetValue(client.ConnectionUID, out INGame))
                {
                    Database.ServerDatabase.LoginQueue.Enqueue("[Login_Dub] " + client.Player.Name + " " + client.ConnectionUID + " >> NewClient On Game IP : [ " + client.Socket.RemoteIp + " ] On Game MacAddress [ " + client.OnLogin.MacAddress + " ]" + "  [The Clock] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    Console.WriteLine("[Login_Dub] " + client.Player.Name + " " + client.ConnectionUID + " >> NewClient");
                    client.Socket.Disconnect();
                    INGame.Socket.Disconnect();
                    return;
                }
                client.ClientFlag &= ~Client.ServerFlag.AcceptLogin;


                string CharacterName; ushort Body; uint Class;

                stream.GetNewRoleInfo(out CharacterName, out Body, out Class);

                byte attackType = 0;
                switch (Class)
                {
                    case 0:
                    case 1: Class = 10000; break;
                    case 2:
                    case 3: Class = 1000; break;
                    case 4:
                    case 5: Class = 4000; break;
                    case 6:
                    case 7: Class = 2000; break;
                    case 8:
                    case 9: Class = 5000; break;
                    case 10:
                    case 11: Class = 6000; break;
                    case 12:
                    case 13: Class = 7000; break;
                    case 14:
                    case 15: Class = 8000; break;

                    case 16:
                    case 17:
                        {
                            attackType = 0;
                            Class = 16000;
                            break;
                        }
                    case 18:
                    case 19:
                        {
                            attackType = 1;
                            Class = 16000;
                            break;
                        }
                    case 20:
                    case 21: Class = 9000; break;

                    case 22:
                    case 23: Class = 3000; break;
                }


                if (!ExitBody(Body))
                {
#if Arabic
                     client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Body, 
                    Y", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#else
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Body, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));

#endif
                    return;
                }
                if (!ExitClass(Class))
                {
#if Arabic
                       client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Class, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#else
                    client.Send(new MsgServer.MsgMessage("AHAHAH! WRONG Class, NICE TRY", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#endif

                    return;
                }

                CharacterName = CharacterName.Replace("\0", "");
                if (BaseFunc.NameStrCheck(CharacterName))
                {
                    if (!Pool.NameUsed.Contains(CharacterName.GetHashCode()))
                    {
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacter;

                        lock (Pool.NameUsed)
                            Pool.NameUsed.Add(CharacterName.GetHashCode());

                        client.Player.Name = CharacterName;
                        client.Player.Class = Class;
                        client.Player.Body = Body;


                        client.Player.VipLevel = 1;
                        client.Player.Level = 1;
                        client.Player.Map = 1002;
                        // QahrMobile: Twin City square (the mobile's WindPlain has sea at the PC's 270,429)
                        client.Player.X = 430;
                        client.Player.Y = 378;
                        client.Player.Money = 10000;


                        Database.DataCore.LoadClient(client.Player);
                        client.Beasts = new Role.Instance.Beasts(client);
                        client.HairfaceStorage = new Role.Instance.HairfaceStorage(client);
                        client.HundredWeapons = new Role.Instance.HundredWeapons(client);
                        client.MyNinja = new Role.Instance.Ninja(client);
                        client.MyArchives = new Role.Instance.Archives(client);
                        client.Player.UID = client.ConnectionUID;
                        client.Player.DominoCode = Pool.DominoCounter.Next;
                        if (attackType == 1)
                            client.Player.MainFlag |= Role.Player.MainFlagType.OnMeleeAttack;

                        Database.DataCore.AtributeStatus.GetStatus(client.Player);

                        if (Body == 1008)
                            client.Player.Face = (ushort)Pool.GetRandom.Next(1, 50);
                        else
                            client.Player.Face = (ushort)Pool.GetRandom.Next(201, 250);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.ID == client.Player.Face && i.Type == MsgHairfaceStorage.Type.Avatar).FirstOrDefault(), false);
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Avatar).FirstOrDefault());

                        client.Player.Hair = 27;
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.ID == client.Player.Hair % 1000 && i.Type == MsgHairfaceStorage.Type.Hairstyle).FirstOrDefault(), false);

                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.TexasTable).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.CardBack).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.TexasBet).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Level).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Map).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Dealer).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Carpet).FirstOrDefault(), false);
                        client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Frame).FirstOrDefault(), false);

                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.TexasTable).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.CardBack).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.TexasBet).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Level).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Map).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Dealer).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Carpet).FirstOrDefault());
                        client.HairfaceStorage.Equip(Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Frame).FirstOrDefault());

                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            client.Equipment.Add(stream, 152005, Role.Flags.ConquerItem.Ring);
                            client.Equipment.Add(stream, 421301, Role.Flags.ConquerItem.RightWeapon);
                        }

                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.Equipment.Add(stream, 150003, Role.Flags.ConquerItem.Ring);
                            client.Equipment.Add(stream, 500006, Role.Flags.ConquerItem.RightWeapon);
                        }
                        else
                        {
                            client.Equipment.Add(stream, 150003, Role.Flags.ConquerItem.Ring);
                            if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 611301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 612301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 420301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 420301, Role.Flags.ConquerItem.LeftWeapon);
                            }

                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 617301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 617301, Role.Flags.ConquerItem.LeftWeapon);
                                client.Equipment.Add(stream, 138313, Role.Flags.ConquerItem.Armor);
                            }
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 610301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 610301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 601301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 601301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            {

                                client.Equipment.Add(stream, 101319, Role.Flags.ConquerItem.Armor);
                                client.Equipment.Add(stream, 626301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 626301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                            {

                                client.Equipment.Add(stream, 102313, Role.Flags.ConquerItem.Armor);
                                client.Equipment.Add(stream, 681301, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 680301, Role.Flags.ConquerItem.LeftWeapon);
                            }
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 561301, Role.Flags.ConquerItem.RightWeapon);
                            }
                            else if (Database.AtributesStatus.IsDune(client.Player.Class))
                            {
                                client.Equipment.Add(stream, 608009, Role.Flags.ConquerItem.RightWeapon);
                                client.Equipment.Add(stream, 608009, Role.Flags.ConquerItem.LeftWeapon);
                                client.Equipment.Add(stream, 171009, Role.Flags.ConquerItem.Head);
                                client.Equipment.Add(stream, 103009, Role.Flags.ConquerItem.Armor);
                            }
                            else
                                client.Equipment.Add(stream, 410301, Role.Flags.ConquerItem.RightWeapon);
                        }
                        if (!Database.AtributesStatus.IsLee(client.Player.Class) && !Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            client.Equipment.Add(stream, 132009, Role.Flags.ConquerItem.Armor);

                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);

                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                        }
                        else if (Database.AtributesStatus.IsDune(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.MoonwardLeap))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.MoonwardLeap);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.SwallowDive))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.SwallowDive);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.SwallowDive2))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.SwallowDive2);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.TempestStrike))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.TempestStrike);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.CliffCrusher))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.CliffCrusher);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.WandererNormalATK))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.WandererNormalATK);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.FinalStand))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.FinalStand);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.LonelyBattle))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.LonelyBattle);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.SheathParry))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.SheathParry);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.FleetingShadow))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.FleetingShadow);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellIDDune.SkyStep))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellIDDune.SkyStep);
                            }
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                        }
                        client.Inventory.Add(stream, 300000, 1, 6, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // Steed (B)
                        client.Inventory.Add(stream, 203009, 1, 6, 1, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // Crop (B)
                        client.Inventory.Add(stream, 202009, 1, 6, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true); // Tower (B)
                        client.Inventory.Add(stream, 201009, 1, 6, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true); // Fan (B)
                        client.Inventory.Add(stream, 204009, 1, 6, 0, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true); // Wing (B)
                        client.Inventory.Add(stream, 727266, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // Stuff (B)

                        client.Inventory.Add(stream, 721259, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // CelestialStone
                        client.Inventory.Add(stream, 723701, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true); // ExemptionToken

                        // send each currency with its own value (was Money for all four)
                        client.Player.SendUpdate(stream, client.Player.BoundConquerPoints, MsgServer.MsgUpdate.DataType.BoundConquerPoints);
                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgServer.MsgUpdate.DataType.ConquerPoints);
                        client.Send(new MsgServer.MsgMessage("ANSWER_OK", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
                        client.Player.ExpireVipback = true;
                        // client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                        client.Player.UpdateVip(stream);
                        client.Status.MaxHitpoints = client.CalculateHitPoint();
                        client.Player.HitPoints = (int)client.Status.MaxHitpoints;


                        client.ClientFlag |= Client.ServerFlag.CreateCharacterSucces;
                    }
                    else
                    {
#if Arabic
                          client.Send(new MsgServer.MsgMessage("The name is in use! try other name", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#else
                        client.Send(new MsgServer.MsgMessage("The name is in use! try other name", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#endif

                    }
                }
                else
                {
#if Arabic
                     client.Send(new MsgServer.MsgMessage("Invalid characters name!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#else
                    client.Send(new MsgServer.MsgMessage("Invalid characters name!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.PopUP).GetArray(stream));
#endif

                }
            }
        }

        public static bool ExitBody(ushort _body)
        {
            return (_body == 1008 || _body == 2007);
        }

        public static bool ExitClass(uint cls)
        {
            return (cls == 1000 || cls == 2000 || cls == 3000 || cls == 4000
                || cls == 5000 || cls == 6000 || cls == 7000 || cls == 10000 || cls == 8000 || cls == 16000 || cls == 9000);
        }
    }
}
