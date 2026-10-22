using System.Threading.Generic;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Game.MsgServer.AttackHandler;
using Qahr.Mobile.Game.MsgTournaments;
using Qahr.Mobile.Client;
using System.IO;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.Game.MsgServer;
using System;
using System.Linq;
using System.Collections.Generic;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
using Qahr.Mobile.Game.MsgServer.AttackHandler.CheckAttack;
using Qahr.Mobile.Database;
using System.Runtime.InteropServices.ComTypes;

using System.Windows.Forms;

namespace Qahr.Mobile.Client
{
    public class PoolProcesses
    {
        
        public static unsafe void CharactersCallback(Client.GameClient client)
        {
            
            if (Program.ExitRequested || !client.FullLoading || client.Fake || !client.Player.CompleteLogin)
                return;

            try
            {
                DateTime Now = DateTime.Now;
                global::Qahr.Mobile.QuestEngine.QuestEngineCore.OnTick(client);// [QuestEngine] Visit objectives (player at the place)
                global::Qahr.Mobile.FoolsKit.Kit.OnTick(client);// [AprilFools2023 2026-10-15] April Fools group: private maze copies / encounters (Base\AprilFools2023\FoolsKit.cs)


                #region OnlineStamp
                if (Now >= client.Player.OnlineStamp.AddMinutes(2))
                {
                    client.Player.OnlineMinutes += 1;
                    client.Player.OnlineStamp = DateTime.Now;
                    // client.SendSysMesage("You Got 1 Online Point Keep Your Account Open and You Will Get More.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                }
                #endregion




                #region [CP Castle]
                if (Game.MsgTournaments.MsgSchedules.CpsCastle.Process == Game.MsgTournaments.ProcesType.Dead)
                {
                    if (client.Player.Name.Contains("[GM]"))
                        return;
                   
                    if (client.Player.Map == 10133)
                    {
                        client.Teleport(410, 354, 1002);
                    }
                }
                #endregion 


                #region Flags War                
                if (Game.MsgTournaments.MsgSchedules.FlagsWar.Process == Game.MsgTournaments.ProcesType.Dead)
                {
                    if (client.Player.Name.Contains("[GM]"))
                        return;

                    if (client.Player.Map == 2071)
                    {
                        client.Teleport(410, 354, 1002);
                    }
                }
                #endregion
  

                #region [Gold Box]
                if (Game.MsgTournaments.MsgSchedules.GoldBoxWar.Process == Game.MsgTournaments.ProcesType.Dead)
                {

                    if (client.Player.Map == 10741)
                    {
                        client.Teleport(410, 354, 1002);
                    }
                }
                #endregion

                #region [SquidwardOctopus]
                if (Game.MsgTournaments.MsgSchedules.SquidwardOctopus.Process == Game.MsgTournaments.ProcesType.Dead)
                {
                    if (client.Player.Map == 3071)
                    {
                        client.Teleport(410, 354, 1002);
                    }
                }
                #endregion  


                #region [Treasure Thief]
                if (Game.MsgTournaments.MsgSchedules.TreasureThief.Process == Game.MsgTournaments.ProcesType.Dead)
                {
                    if (client.Player.Map == 3030)
                    {
                        client.Teleport(410, 354, 1002);
                    }
                }
                #endregion  


                #region Attribute Points Limit (MAX 903 SAFE)

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    int total =
                        client.Player.Strength +
                        client.Player.Agility +
                        client.Player.Vitality +
                        client.Player.Spirit +
                        client.Player.Atributes;

                    if (total > 903)
                    {
                        int extra = total - 903;

                        int newAttr = client.Player.Atributes - extra;

                        if (newAttr < 0)
                            newAttr = 0;

                        client.Player.Atributes = (ushort)newAttr;

                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                    }
                }

                #endregion


                #region Atribute Points
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    int limitPoints = client.Player.Agility + client.Player.Strength + client.Player.Vitality + client.Player.Spirit + client.Player.Atributes;
                    if (limitPoints > 903)
                    {
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.Atributes = 901;
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                    }
                }
                #endregion

                #region Jiang Hu
                if (client.Player.MyJiangHu != null)
                {
                    client.Player.MyJiangHu.CheckStatus(client);
                }
                #endregion                
                #region Intensify Archer
                if (client.Player.InUseIntensify)
                {
                    if (Now > client.Player.IntensifyStamp.AddSeconds(5))
                    {
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Focused))
                        {
                            client.Player.InUseIntensify = false;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                          , client.Player.UID, 0, 0, client.Player.FocusClientSpell.ID
                          , client.Player.FocusClientSpell.Level, client.Player.FocusClientSpell.UseSpellSoul);


                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var stream = recycledPacket.GetStream();
                                {
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(client);
                                }
                            }
                            client.Player.AddFlag(MsgUpdate.Flags.Focused, 60, true);
                        }
                    }
                }
                #endregion
                #region Online Training Map
                if (client.Player.Map == 601)
                {
                    if (!client.Map.ValidLocation(client.Player.X, client.Player.Y))
                    {
                        client.Teleport(64, 56, 601);
                    }
                }
                #endregion
                #region Map 10250 No Auto hunt
                if (client.Player.Map == 10250)
                {
                    if (client.Player.OnAutoHunt)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(stream.AutoHuntCreate(3, 0));
                            client.Player.OnAutoHunt = false;
                            client.CreateBoxDialog("You cannot use Auto Hunt in here..");
                        }
                    }
                }
                #endregion
                #region Active Pick
                if (client.Player.ActivePick)
                {
                    if (Now > client.Player.PickStamp)
                    {
                        client.Player.ActivePick = false;

                        if (client.Player.MonkMiseryTransforming == 1)
                        {
                            client.Player.MonkMiseryTransforming = 0;
                            client.Teleport(client.Player.X, client.Player.Y, 3831);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Server.AddMapMonster(stream, client.Map, 7484, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID);

                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1830, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.Player.Money >= 99999)
                                {
                                    client.Player.Money -= 99999;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    if (Role.Core.Rate(60))
                                    {
                                        client.Player.Money += 100;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                        client.Inventory.Add(stream, 721878);
                                        client.SendSysMesage("You received 100 Silver!");
                                        client.Player.QuestGUI.FinishQuest(1830);
                                        client.SendSysMesage("Shark is satisfied with your bid and sold the Victory Portrait to you.");
                                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.Shark;
                                        Game.MsgNpc.NpcHandler.Shark(client, stream, 4, "", 0);
                                    }
                                    else
                                    {
                                        client.CreateDialog(stream, "Too low! Higher!", "I~see.");
                                    }
                                }
                                else
                                {
                                    client.CreateDialog(stream, "Sorry, but you don`t have enough Silver.", "I~see.");
                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower1 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower6
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower2 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower5
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower3 || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower4
                                || client.ActiveNpc == (uint)Game.MsgNpc.NpcID.LavaFlower7)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(3008747, 0, 1, stream);
                                        client.SendSysMesage("You received LavaFlower!", MsgMessage.ChatMode.System);
                                        if (client.Inventory.Contain(3008747, 10))
                                            client.CreateBoxDialog("You`ve collected 10 Lava Flowers. Go and try to extract the Fire Force.");

                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");

                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc >= (uint)Game.MsgNpc.NpcID.WhiteHerb1 && client.ActiveNpc <= (uint)Game.MsgNpc.NpcID.WhiteHerb6)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(3008741, 0, 1, stream);
                                        client.SendSysMesage("You received WhiteHerb!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");

                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(1653, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc >= 8551 && client.ActiveNpc <= 8555)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.AddItemWitchStack(711478, 0, 1, stream);
                                        client.SendSysMesage("You~received~a~Rainbow~Flower!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");


                                    if (client.OnRemoveNpc != null)
                                    {
                                        client.OnRemoveNpc.Respawn = DateTime.Now.AddSeconds(10);
                                        client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                                        client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);
                                    }
                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(6131, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720995, 1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    ActionQuery action = new ActionQuery()
                                    {
                                        ObjId = client.Player.UID,
                                        Type = ActionType.ClikerON,
                                        Fascing = 7,
                                        PositionX = client.Player.X,
                                        PositionY = client.Player.Y,
                                        dwParam = 0x0c

                                    };
                                    client.Send(stream.ActionCreate(action));
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.SaltedFish)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711479);
                                        client.SendSysMesage("You received a pack of Salted Fish!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(1640, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.SaltedFish)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711472);
                                        client.SendSysMesage("You receive the Salted Fish!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.FishingNet)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        client.Inventory.Add(stream, 711473);
                                        client.SendSysMesage("You received a Fishing Net!", MsgMessage.ChatMode.System);
                                    }
                                    else
                                        client.CreateBoxDialog("Please make 1 more space in your inventory.");
                                }

                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1594, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.WhiteChrysanthemum)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711441);
                                    client.SendSysMesage("You've got a White Chrysanthemum!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.Jasmine)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711442);
                                    client.SendSysMesage("You've got a Jasmine!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.Lily)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711440);
                                    client.SendSysMesage("You've got a Lily!", MsgMessage.ChatMode.System);
                                }
                            }
                            else if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.WillowLeaf)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 711443);
                                    client.SendSysMesage("You've got a Willow Leaf!", MsgMessage.ChatMode.System);
                                }
                            }


                        }
                        if (client.Player.QuestGUI.CheckQuest(1469, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.st1TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.nd2TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                            if (client.ActiveNpc == (ushort)Game.MsgNpc.NpcID.rd3TreeSeed)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Inventory.Add(stream, 720971);
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1469, 0, 0, 1);
                                    if (client.Player.QuestGUI.CheckObjectives(1469, 1, 1, 1))
                                        client.CreateBoxDialog("You`ve~collected~enough~seeds.~Go~report~to~Wan~Ying,~right~away.");
                                    else
                                        client.CreateBoxDialog("You`ve~received~a~seed.");
                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1330, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
                                switch (client.Player.QuestCaptureType)
                                {

                                    case 1:
                                        {
#if Arabic
                                             client.SendSysMesage("You captured a Thunder Ape.", MsgMessage.ChatMode.System);
#else
                                            client.SendSysMesage("You captured a Thunder Ape.", MsgMessage.ChatMode.System);
#endif

                                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1330, 1);
                                        }
                                        break;
                                    case 2:
                                        {
#if Arabic
                                            client.SendSysMesage("You captured a Thunder Ape L58.", MsgMessage.ChatMode.System);
#else
                                            client.SendSysMesage("You captured a Thunder Ape L58.", MsgMessage.ChatMode.System);
#endif

                                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1330, 0, 1);
                                        }
                                        break;

                                }
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(1317, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.CarpenterJack, client.Player.Class, 1317);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Inventory.AddItemWitchStack(711356, 0, 1, stream);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1317, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
#if Arabic
                                client.SendSysMesage("You received 1 Chiff Flower.", MsgMessage.ChatMode.System);
#else
                                client.SendSysMesage("You received 1 Chiff Flower.", MsgMessage.ChatMode.System);
#endif

                            }
                            if (client.Player.QuestGUI.CheckObjectives(1317, 20))
                            {
#if Arabic
                                     client.Player.QuestGUI.SendAutoPatcher("You have collected enough CliffFowers. Send it to Carpenter Jack.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                           
#else
                                client.Player.QuestGUI.SendAutoPatcher("You have collected enough CliffFowers. Send it to Carpenter Jack.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif
                            }
                        }

                        else if (client.Player.QuestGUI.CheckQuest(1011, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Inventory.Contain(711239, 5))
                                {
                                    var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.XuLiang, client.Player.Class, 1011);
#if Arabic
                                    client.Player.QuestGUI.SendAutoPatcher("You`ve~picked~5~Peach~Blossoms!~Now~give~them~to~Xu~Liang.", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
#else
                                    client.Player.QuestGUI.SendAutoPatcher("You`ve~picked~5~Peach~Blossoms!~Now~give~them~to~Xu~Liang.", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
#endif

                                }
                                else
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, 1011, 1);
                                        client.Inventory.AddItemWitchStack(711239, 0, 1, stream);
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "allcure5");
#if Arabic
                                            client.SendSysMesage("You picked a Peach Blossom from the Peach Tree!", MsgMessage.ChatMode.System);
#else
                                        client.SendSysMesage("You picked a Peach Blossom from the Peach Tree!", MsgMessage.ChatMode.System);
#endif

                                    }

                                }
                            }
                            else
                            {
#if Arabic
                                client.SendSysMesage("Please make 1 more space in your inventory.", MsgMessage.ChatMode.System);
#else
                                client.SendSysMesage("Please make 1 more space in your inventory.", MsgMessage.ChatMode.System);
#endif

                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(6049, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, 6049, 1, 1);

                                if (client.OnRemoveNpc != null)
                                {
                                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                    packet.ID = MsgStringPacket.StringID.Effect;
                                    packet.UID = client.OnRemoveNpc.UID;
                                    packet.Strings = new string[1] { "M_Fire1" };
                                    client.Player.View.SendView(stream.StringPacketCreate(packet), true);


                                    client.OnRemoveNpc.Respawn = DateTime.Now.AddSeconds(10);
                                    client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                                    client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);
                                    //add effect here

                                    Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
#if Arabic
                                      dialog.AddText("What? You said the Desert Guardian sent you here to find us? Well, I had to play dead to keep the bandits from seeing me. I will avenge my comrades, one day!")
                                    .AddText("~I`ll go back and report this to Desert Guardian! Thanks for coming to find us. I thought we would never be seen again.");
                                    dialog.AddOption("No~Problem.", 255);
                                    dialog.AddAvatar(101).FinalizeDialog();
#else
                                    dialog.AddText("What? You said the Desert Guardian sent you here to find us? Well, I had to play dead to keep the bandits from seeing me. I will avenge my comrades, one day!")
                                  .AddText("~I`ll go back and report this to Desert Guardian! Thanks for coming to find us. I thought we would never be seen again.");
                                    dialog.AddOption("No~Problem.", 255);
                                    dialog.AddAvatar(101).FinalizeDialog();
#endif

                                }

                                if (client.Player.QuestGUI.CheckObjectives(6049, 8))
                                {

                                    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.DesertGuardian, client.Player.Class, 6049);
#if Arabic
          client.Player.QuestGUI.SendAutoPatcher("You~are~too~far~away~from~the~Soldier`s~Remains!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                           
#else
                                    client.Player.QuestGUI.SendAutoPatcher("You~are~too~far~away~from~the~Soldier`s~Remains!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

#endif
                                    client.Player.QuestGUI.SendAutoPatcher("You~are~too~far~away~from~the~Soldier`s~Remains!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                                }
                                else
                                {
#if Arabic
                                       client.CreateBoxDialog("This soldier has died. Release his soul!");
#else
                                    client.CreateBoxDialog("This soldier has died. Release his soul!");
#endif
                                }

                                //client.CreateBoxDialog("You~are~too~far~away~from~the~Soldier`s~Remains!");
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(6014, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(client.Player.DailyMagnoliaItemId, 1))
                            {


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    client.Map.AddMagnolia(stream, client.Player.DailyMagnoliaItemId);
                                    Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                    packet.ID = MsgStringPacket.StringID.Effect;
                                    packet.UID = client.Map.Magnolia.UID;
                                    packet.Strings = new string[1] { "accession1" };
                                    client.Player.View.SendView(stream.StringPacketCreate(packet), true);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                    client.Player.QuestGUI.FinishQuest(6014);
                                    client.Inventory.Remove(client.Player.DailyMagnoliaItemId, 1, stream);
                                    switch (client.Player.DailyMagnoliaItemId)
                                    {
                                        case 729306:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 10, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
#if Arabic
                                                 client.CreateBoxDialog("Congratulations!~You~received~60 minutes of EXP, 10 Study Points and 1 Chi Token.!");
#else
                                                client.CreateBoxDialog("Congratulations!~You~received~60 minutes of EXP, 10 Study Points and 1 Chi Token.!");
#endif

                                                break;
                                            }
                                        case 729307:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 20, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(900, true, Role.Flags.ExperienceEffect.angelwing);
#if Arabic
                                                  client.CreateBoxDialog("Congratulations!~You~received~90 minutes of EXP, 20 Study Points, 1 Chi Token.!");
#else
                                                client.CreateBoxDialog("Congratulations!~You~received~90 minutes of EXP, 20 Study Points, 1 Chi Token.!");
#endif

                                                break;
                                            }
                                        case 729308:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 50, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(1200, true, Role.Flags.ExperienceEffect.angelwing);
#if Arabic
                                                   client.CreateBoxDialog("Congratulations!~You~received~120 minutes of EXP, 50 Study Points, 1 Chi Token!");
#else
                                                client.CreateBoxDialog("Congratulations!~You~received~120 minutes of EXP, 50 Study Points, 1 Chi Token!");
#endif

                                                break;
                                            }
                                        case 729309:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(1800, true, Role.Flags.ExperienceEffect.angelwing);
#if Arabic
                                                 client.CreateBoxDialog("Congratulations!~You~received~180 minutes of EXP, 100 Study Points, 1 Chi Token.!");
#else
                                                client.CreateBoxDialog("Congratulations!~You~received~180 minutes of EXP, 100 Study Points, 1 Chi Token.!");
#endif

                                                break;
                                            }
                                        case 7293010:
                                            {
                                                client.Player.SubClass.AddStudyPoints(client, 300, stream);
                                                client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                                                client.GainExpBall(3000, true, Role.Flags.ExperienceEffect.angelwing);
#if Arabic
                                                  client.CreateBoxDialog("Congratulations!~You~received~300 minutes of EXP, 300 Study Points, 1 Chi Token.!");
#else
                                                client.CreateBoxDialog("Congratulations!~You~received~300 minutes of EXP, 300 Study Points, 1 Chi Token.!");
#endif

                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.RemovePick(stream);
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Taoist Power
                client.Player.UpdateTaoistPower(Now);
                #endregion
                #region Two Hand Safety Checks
                if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                {
                    if (client.Equipment.LeftWeapon != 0)
                    {
                        if (Database.ItemType.IsHossu(client.Equipment.LeftWeapon) == false)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream);
                                    client.Equipment.LeftWeapon = 0;
                                }
                            }
                        }
                    }
                }
                else if (Database.ItemType.IsTwoHand(client.Equipment.RightWeapon))
                {
                    if (client.Equipment.LeftWeapon != 0 && Database.ItemType.IsShield(client.Equipment.LeftWeapon) == false)
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.Equipment.Remove(Role.Flags.ConquerItem.LeftWeapon, stream) == false)
                                    client.Equipment.Remove(Role.Flags.ConquerItem.AleternanteLeftWeapon, stream);
                                client.Equipment.LeftWeapon = 0;
                            }
                        }
                    }
                }
                #endregion
                #region Invalid X | Y
                if (client.Player.X == 0 || client.Player.Y == 0)
                {
                    client.Teleport(410, 354, 1002);
                }
                #endregion                

                if (client.Player.Map == 1005)
                {
                    if (!client.Player.Alive)
                    {
                        if (Now >= client.Player.DeadStamp.AddSeconds(3))
                        {
                            ushort x = 0; ushort y = 0;
                            client.Map.GetRandCoord(ref x, ref y);
                            client.Teleport(x, y, 1005, 0);
                        }
                    }
                }
                if (client.Player.Map == 10445 || client.Player.Map == 10446 || client.Player.Map == 10447 || client.Player.Map == 10448 || client.Player.Map == 10449 || client.Player.Map == 10450 || client.Player.Map == 10451 || client.Player.Map == 10452 || client.Player.Map == 10453 || client.Player.Map == 10454 || client.Player.Map == 10455)
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 66, 90) <= 2)
                    {
                        client.Teleport(57, 79, 1004);
                    }

                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void FloorCallback(Client.GameClient client)
        {
            if (Program.ExitRequested)
                return;
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Fake)
                    return;
                DateTime Now = DateTime.Now;



                #region ManiacDance
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.ManiacDance))
                {

                    if (Now > client.Player.ManiacDanceStamp)
                    {
                        client.Player.ManiacDanceStamp = DateTime.Now.AddMilliseconds(1000);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var ClientSpell = client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.ManiacDance];
                            var DBSpell = Pool.Magic[(ushort)Role.Flags.SpellID.ManiacDance][0];
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                client.Player.UID
                                  , 0, client.Player.X, client.Player.Y, ClientSpell.ID
                                  , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience = 0;

                            foreach (Role.IMapObj target in client.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                Game.MsgMonster.MonsterRole attacked = target as Game.MsgMonster.MonsterRole;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, attacked, DBSpell, out AnimationObj);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, attacked.X, attacked.Y) <= 5)
                                {
                                    if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, attacked, DBSpell, out AnimationObj);
                                        Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(client);
                        }
                    }
                }
                #endregion

                if (client.Player.FloorSpells.Count != 0)
                {
                    foreach (var ID in client.Player.FloorSpells)
                    {

                        switch (ID.Key)
                        {
                            #region SuperSpeedGun
                            case (ushort)Role.Flags.SpellID.SuperSpeedGun:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                client.Player.SuperSpeedGun++;
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                //foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                //{
                                                //    var monster = obj as Game.MsgMonster.MonsterRole;
                                                //    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                //    {
                                                //        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                //        {
                                                //            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                //            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                //            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                //            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                //            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                //        }
                                                //    }
                                                //}
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, false, client.Player.SuperSpeedGun);                                                   
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Effect = MsgAttackPacket.AttackEffect.None;
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion



                           


                            #region StarChainFire
                            case (ushort)Role.Flags.SpellID.StarChainFire:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (!client.Player.Alive)
                                            {
                                                while (RemoveSpells.Count > 0)
                                                    spellclient.RemoveItem(RemoveSpells.Dequeue());
                                                Role.FloorSpell.ClientFloorSpells FloorSpell;
                                                client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                break;
                                            }
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 120 / 100; // تعديل الدمج 
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            if (!target.ContainFlag(MsgUpdate.Flags.StarChainFire))
                                                            {
                                                                target.AddFlag(MsgUpdate.Flags.StarChainFire, (int)spell.DBSkill.Duration, true, 2);
                                                                target.SendUpdate(stream, MsgUpdate.Flags.StarChainFire, (uint)spell.DBSkill.Duration, (uint)spell.DBSkill.Damage, (uint)spell.DBSkill.Damage, MsgUpdate.DataType.ArchiveSkill, true);

                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region DragonRising
                            case (ushort)Role.Flags.SpellID.DragonRising:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 40 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);
                                                if (client.Player.Stamina < MaxStamina)
                                                {
                                                    client.Player.Stamina += 5;
                                                    client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region SubstitutionAttack
                            case (ushort)Role.Flags.SpellID.SubstitutionAttack:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = spell.FloorPacket.m_X;
                                                spellclient.SpellPacket.Y = spell.FloorPacket.m_Y;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = (uint)spell.DBSkill.GDamage;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region FireArrow
                            case (ushort)Role.Flags.SpellID.FireArrow:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region MysticalMelody
                            case (ushort)Role.Flags.SpellID.MysticalMelody:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);

                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);

                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Magic.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region SpaceLeap
                            case (ushort)Role.Flags.SpellID.SpaceLeap:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();


                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;

                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region ShadowOfChaser
                            case (ushort)Role.Flags.SpellID.ShadowofChaser:
                                {


                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);


                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);



                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);



                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Hit = 1;

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region HorrorOfStomper
                            case (ushort)Role.Flags.SpellID.HorrorofStomper:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = spell.FloorPacket.OwnerX;
                                                spellclient.SpellPacket.Y = spell.FloorPacket.OwnerY;
                                                spellclient.SpellPacket.SpellLevel = spell.DBSkill.Level;


                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    for (int i = 0; i < 2; i++)
                                                    {
                                                        var monster = obj as Game.MsgMonster.MonsterRole;
                                                        byte myAngle = (byte)spell.FloorPacket.Angle;

                                                        if (myAngle > 3)
                                                            myAngle -= 2;
                                                        else
                                                            myAngle += 2;
                                                        if (i != 0)
                                                        {
                                                            if (myAngle > 3)
                                                                myAngle -= 4;
                                                            else
                                                                myAngle += 4;
                                                        }
                                                        uint xxxx = spell.FloorPacket.m_X;
                                                        uint yyyy = spell.FloorPacket.m_Y;
                                                        client.Map.Pushback(ref xxxx, ref yyyy, (Role.Flags.ConquerAngle)myAngle, 7);
                                                        if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) < 9)
                                                        {
                                                            Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms.Fan sector = new Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms.Fan(spell.FloorPacket.m_X, spell.FloorPacket.m_Y, (ushort)xxxx, (ushort)yyyy, 2, 40);
                                                            if (sector.IsInFan(obj.X, obj.Y))
                                                            {
                                                                if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                                {
                                                                    Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                    Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                                    Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                                    spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                                }
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    for (int i = 0; i < 2; i++)
                                                    {
                                                        byte myAngle = (byte)spell.FloorPacket.Angle;

                                                        if (myAngle > 3)
                                                            myAngle -= 2;
                                                        else
                                                            myAngle += 2;
                                                        if (i != 0)
                                                        {
                                                            if (myAngle > 3)
                                                                myAngle -= 4;
                                                            else
                                                                myAngle += 4;
                                                        }
                                                        uint xxxx = spell.FloorPacket.m_X;
                                                        uint yyyy = spell.FloorPacket.m_Y;
                                                        client.Map.Pushback(ref xxxx, ref yyyy, (Role.Flags.ConquerAngle)myAngle, 7);
                                                        var target = obj as Role.SobNpc;

                                                        Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms.Fan sector = new Qahr.Mobile.Game.MsgServer.AttackHandler.Algoritms.Fan(spell.FloorPacket.m_X, spell.FloorPacket.m_Y, (ushort)xxxx, (ushort)yyyy, 2, 40);
                                                        if (sector.IsInFan(obj.X, obj.Y))
                                                        {
                                                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                            {
                                                                //     return;
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                                Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                                AnimationObj.Hit = 1;//??
                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                {

                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                }


                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region PeaceOfStomper
                            case (ushort)Role.Flags.SpellID.PeaceofStomper:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                spellclient.UID = spell.FloorPacket.ItemOwnerUID;
                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            //    return;
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Hit = 1;//??
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);


                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));


                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);

                                    }
                                    break;
                                }
                            #endregion
                            #region SeaBurial
                            case (ushort)Role.Flags.SpellID.SeaBurial:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (!client.Player.Alive)
                                            {
                                                while (RemoveSpells.Count > 0)
                                                    spellclient.RemoveItem(RemoveSpells.Dequeue());
                                                Role.FloorSpell.ClientFloorSpells FloorSpell;
                                                client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                break;
                                            }
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                uint xX = spell.FloorPacket.m_X, yY = spell.FloorPacket.m_Y;
                                                client.Map.Pushback(ref xX, ref  yY, Role.Core.GetAngle(spell.FloorPacket.m_X, spell.FloorPacket.m_Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY), 18);


                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = (ushort)xX;
                                                spellclient.SpellPacket.Y = (ushort)yY;
                                                spellclient.SpellPacket.bomb = 1;
                                                Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm Line = new Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm(spell.FloorPacket.m_X, spell.FloorPacket.OwnerX, spell.FloorPacket.m_Y, spell.FloorPacket.OwnerY, client.Map, 18, 0);

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            //AnimationObj.Damage = AnimationObj.Damage * 60 / 100;
                                                            // AnimationObj.Damage = AnimationObj.Damage * 95 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);
                                                spell.FloorPacket.Timer = 0;
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region TideTrap
                            case (ushort)Role.Flags.SpellID.TideTrap:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();


                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);
                                                //if (Attack)
                                                spellclient.X = spell.FloorPacket.OwnerX;
                                                spellclient.Y = spell.FloorPacket.OwnerY;

                                                spellclient.CreateMsgSpell(0);

                                                ushort X = spell.FloorPacket.m_X, Y = spell.FloorPacket.m_Y;
                                                var coord = Game.MsgServer.AttackHandler.Algoritms.MoveCoords.CheckBladeTeampsCoords(spellclient.X, spellclient.Y, X
                                                , Y, null, 12);

                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    bool hit = false;
                                                    for (int j = 0; j < coord.Count; j++)
                                                        if (Game.MsgServer.AttackHandler.Calculate.Base.GetDDistance(obj.X, obj.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                                            hit = true;
                                                    if (hit)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    bool hit = false;
                                                    for (int j = 0; j < coord.Count; j++)
                                                        if (Game.MsgServer.AttackHandler.Calculate.Base.GetDDistance(obj.X, obj.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                                            hit = true;
                                                    if (hit)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);

                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    bool hit = false;
                                                    for (int j = 0; j < coord.Count; j++)
                                                        if (Game.MsgServer.AttackHandler.Calculate.Base.GetDDistance(obj.X, obj.Y, (ushort)coord[j].X, (ushort)coord[j].Y) <= 2)
                                                            hit = true;
                                                    if (hit)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Hit = 1;//??

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region RageofWar
                            case (ushort)Role.Flags.SpellID.RageofWar:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 150 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);


                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region WrathOfTheEmperor & InfernalEcho
                            case (ushort)Role.Flags.SpellID.WrathoftheEmperor:
                            case (ushort)Role.Flags.SpellID.InfernalEcho:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;
                                                RemoveSpells.Enqueue(spell);

                                                spellclient.X = spell.FloorPacket.m_X;
                                                spellclient.Y = spell.FloorPacket.m_Y;

                                                spellclient.CreateMsgSpell(0);


                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= (int)(ID.Key == (ushort)Role.Flags.SpellID.WrathoftheEmperor ? 2 : 3))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);



                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Hit = 1;

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                     p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }

                                    break;
                                }
                            #endregion
                            #region TwilightDance
                            case (ushort)Role.Flags.SpellID.TwilightDance:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                uint Experience = 0;

                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(client.Player.UID);



                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, true);
                                                            AnimationObj.Damage = AnimationObj.Damage * 95 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);


                                                            Experience += Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);


                                                            AnimationObj.Hit = 1;

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                                ActionQuery action = new ActionQuery()
                                                {
                                                    ObjId = spell.FloorPacket.ItemOwnerUID,
                                                    TargetPositionY = spell.FloorPacket.m_Y,
                                                    TargetPositionX = spell.FloorPacket.m_X,
                                                    PositionX = spell.FloorPacket.OwnerX,
                                                    PositionY = spell.FloorPacket.OwnerY,
                                                    Type = ActionType.RemoveTrap
                                                };

                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y,
                                                 p => Role.Core.GetDistance(p.X, p.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 18))
                                                {
                                                    if (user.DynamicID == client.Player.DynamicID)
                                                    {
                                                        user.Send(stream.ActionCreate(action));

                                                        user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                    }
                                                }

                                                Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, client, Experience);

                                            }

                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region WaterShockwave
                            case (ushort)Role.Flags.SpellID.WaterShockwave:
                            case (ushort)Role.Flags.SpellID.WaterShockwavePassive:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm Line = new Game.MsgServer.AttackHandler.Algoritms.InLineAlgorithm(spell.FloorPacket.m_X, spell.FloorPacket.OwnerX, spell.FloorPacket.m_Y, spell.FloorPacket.OwnerY, client.Map, 15, 0);
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 2;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                spellclient.SpellPacket.X = (ushort)(spell.FloorPacket.OwnerX);
                                                spellclient.SpellPacket.Y = (ushort)(spell.FloorPacket.OwnerY);
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Hit = 1;
                                                            AnimationObj.MoveX = monster.X;
                                                            AnimationObj.MoveY = monster.Y;
                                                            Pool.ServerMaps[monster.Map].Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, client.Player.Angle, 2);

                                                            client.Map.View.MoveTo<Role.IMapObj>(monster, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                                            monster.X = (ushort)AnimationObj.MoveX;
                                                            monster.Y = (ushort)AnimationObj.MoveY;
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);

                                                            AnimationObj.Hit = 1;
                                                            AnimationObj.MoveX = target.X;
                                                            AnimationObj.MoveY = target.Y;
                                                            target.Owner.Map.Pushback(ref AnimationObj.MoveX, ref AnimationObj.MoveY, client.Player.Angle, 2);

                                                            if (!Game.MsgServer.AttackHandler.CheckAttack.CheckFloors.CheckGuildWar(target.Owner, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY))
                                                            {
                                                                continue;
                                                            }

                                                            client.Map.View.MoveTo<Role.IMapObj>(target, (ushort)AnimationObj.MoveX, (ushort)AnimationObj.MoveY);
                                                            target.X = (ushort)AnimationObj.MoveX;
                                                            target.Y = (ushort)AnimationObj.MoveY;
                                                            target.View.Role(false, null);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Line.InLine(obj.X, obj.Y, 2))
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            AnimationObj.MoveX = obj.X;
                                                            AnimationObj.MoveY = obj.Y;
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);

                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            uint Damage = (uint)spell.DBSkill.Damage2;
                                                            AnimationObj.Damage = (uint)Damage;
                                                            Ninja.Item item;
                                                            if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.DragonSigilBillow, out item))
                                                            {
                                                                AnimationObj.Damage += (uint)item.DBItem.Damage;
                                                            }
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);

                                                            AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman;

                                                            Ninja.Item item;
                                                            if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.DragonSigilBillow, out item))
                                                            {
                                                                AnimationObj.Damage += (uint)item.DBItem.Damage;
                                                            }
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            uint Damage = (uint)spell.DBSkill.Damage2;
                                                            AnimationObj.Damage = (uint)Damage;
                                                            Ninja.Item item;
                                                            if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.DragonSigilBillow, out item))
                                                            {
                                                                AnimationObj.Damage += (uint)item.DBItem.Damage;
                                                            }
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region WildFireball
                            case (ushort)Role.Flags.SpellID.WildFireball:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {


                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;

                                                            Ninja.Item item;
                                                            #region WildSigilBurning
                                                            if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.WildSigilBurning, out item))
                                                            {
                                                                if (Role.Core.Rate(30))
                                                                {
                                                                    target.AddSpellFlag(MsgUpdate.Flags.WildSigilBurning, (int)5, true, 1);
                                                                }

                                                            }
                                                            #endregion
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);

                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region WildFireballPassive
                            case (ushort)Role.Flags.SpellID.WildFireballPassive:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.OwnerX, spell.FloorPacket.OwnerY) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {

                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region FlameofDestruction
                            case (ushort)Role.Flags.SpellID.FlameofDestruction:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            if (monster.Family.ID != 2700 && monster.Family.ID != 2699 && monster.Family.ID != 3636 && monster.Family.ID != 7562)
                                                            {
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                                uint Damage = (uint)spell.DBSkill.Damage2;
                                                                AnimationObj.Damage = (uint)Damage;


                                                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            if (AnimationObj.Damage >= spell.DBSkill.DamageOnHuman)
                                                                AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman;
                                                            else
                                                            {
                                                                if (spell.DBSkill.DamageOnHuman / 2 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman / 2;
                                                                if (spell.DBSkill.DamageOnHuman / 4 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman / 4;
                                                            }
                                                            #region FlameSigilRapid
                                                            Ninja.Item item;
                                                            if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.FlameSigilRapid, out item))
                                                            {
                                                                if (target.Alive)
                                                                {
                                                                    target.FlameOfDestructionDg = (uint)item.DBItem.Power;
                                                                    if (target.AddFlag(MsgUpdate.Flags.Flame_Sigil_Rage, (int)item.DBItem.Damage, true, 1))
                                                                        target.SendUpdate(stream, MsgUpdate.Flags.Flame_Sigil_Rage, item.DBItem.Damage, (uint)target.FlameOfDestructionDg, item.Level, MsgUpdate.DataType.FlameofDestruction);
                                                                }

                                                            }
                                                            #endregion

                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);


                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            uint Damage = (uint)spell.DBSkill.Damage2;
                                                            if (AnimationObj.Damage >= Damage)
                                                                AnimationObj.Damage = (uint)Damage;
                                                            else
                                                            {
                                                                if (Damage / 2 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)Damage / 2;
                                                                if (Damage / 4 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)Damage / 4;
                                                            }


                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }

                                    }

                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region FlameofDestructionPassive
                            case (ushort)Role.Flags.SpellID.FlameofDestructionPassive:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            if (monster.Family.ID != 2700 && monster.Family.ID != 2699 && monster.Family.ID != 3636 && monster.Family.ID != 7562)
                                                            {
                                                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                                Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                                uint Damage = (uint)spell.DBSkill.Damage2;
                                                                AnimationObj.Damage = (uint)Damage;


                                                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            if (AnimationObj.Damage >= spell.DBSkill.DamageOnHuman)
                                                                AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman;
                                                            else
                                                            {
                                                                if (spell.DBSkill.DamageOnHuman / 2 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman / 2;
                                                                if (spell.DBSkill.DamageOnHuman / 4 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)spell.DBSkill.DamageOnHuman / 4;
                                                            }


                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);


                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            uint Damage = (uint)spell.DBSkill.Damage2;
                                                            if (AnimationObj.Damage >= Damage)
                                                                AnimationObj.Damage = (uint)Damage;
                                                            else
                                                            {
                                                                if (Damage / 2 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)Damage / 2;
                                                                if (Damage / 4 > AnimationObj.Damage)
                                                                    AnimationObj.Damage = (uint)Damage / 4;
                                                            }


                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }

                                    }

                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region Quelling Robe
                            case (ushort)Role.Flags.SpellID.QuellingRobe: // New Skill Archive Monk Edit By Mohy ElDin ...
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 120 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);


                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region VajraRing
                            case (ushort)Role.Flags.SpellID.VajraRing: // New Skill Archive Monk Edit By Mohy ElDin ...
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);


                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {

                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion 
                                                  


                            #region FastBlader
                            case (ushort)Role.Flags.SpellID.FastBlader:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 80 / 100; // تعديل الدمج 
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion



                            #region ScrenSword
                            case (ushort)Role.Flags.SpellID.ScrenSword:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 80 / 100; // تعديل الدمج 
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region ClapBomb
                            case (ushort)Role.Flags.SpellID.ClapBomb:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion



                            #region DustDetachment
                            case (ushort)Role.Flags.SpellID.DustDetachment:

                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            if (target.Alive)
                                                            {
                                                                Ninja.Item item;
                                                                if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.DustSigilStunn, out item))
                                                                {
                                                                    if (client.Player.BattlePower > target.BattlePower)
                                                                    {
                                                                        if (Role.Core.Rate(8))
                                                                        {
                                                                            target.AddFlag(MsgUpdate.Flags.Dizzy, 3, true);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }

                            case (ushort)Role.Flags.SpellID.DustDetachmentPassive:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 105 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            if (target.Alive)
                                                            {
                                                                Ninja.Item item;
                                                                if (client.MyNinja.TryGetValueEquip(Ninja.ItemType.DustSigilStunn, out item))
                                                                {
                                                                    if (client.Player.BattlePower > target.BattlePower)
                                                                    {
                                                                        if (Role.Core.Rate(8))
                                                                        {
                                                                            target.AddFlag(MsgUpdate.Flags.Dizzy, 3, true);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region SickleWind
                            case (ushort)Role.Flags.SpellID.SickleWind:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            if (target.ContainFlag(MsgUpdate.Flags.Fly))
                                                            {
                                                                target.RemoveFlag(MsgUpdate.Flags.Infinity);
                                                                target.RemoveFlag(MsgUpdate.Flags.Fly);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            case (ushort)Role.Flags.SpellID.SickleWindPassive:
                                {
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 105 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                            if (target.ContainFlag(MsgUpdate.Flags.Fly))
                                                            {
                                                                target.RemoveFlag(MsgUpdate.Flags.Infinity);
                                                                target.RemoveFlag(MsgUpdate.Flags.Fly);
                                                            }
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 5)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, spellclient.LevelHu);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region SandMist
                            case (ushort)Role.Flags.SpellIDPirate.SandMist:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 3)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region StarVolCano
                            case (ushort)Role.Flags.SpellIDPirate.StarVolCano:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region StarVolcanoPassive
                            case (ushort)Role.Flags.SpellIDPirate.StarVolcanoPassive:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region LavaSea
                            case (ushort)Role.Flags.SpellIDPirate.LavaSea:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 90 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region LavaSeaPassive
                            case (ushort)Role.Flags.SpellIDPirate.LavaSeaPassive:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.bomb = 1;
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 50 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= 4)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region IceAge
                            case (ushort)Role.Flags.SpellIDPirate.IceAge:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (!client.Player.Alive)
                                            {
                                                while (RemoveSpells.Count > 0)
                                                    spellclient.RemoveItem(RemoveSpells.Dequeue());
                                                Role.FloorSpell.ClientFloorSpells FloorSpell;
                                                client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                break;
                                            }
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    PirateSkills.ExecuteSkills(client, (ushort)Role.Flags.SpellIDPirate.ColdBloodline, client.Player.UID, 0, client.Player.X, client.Player.Y);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region IceAgePassive
                            case (ushort)Role.Flags.SpellIDPirate.IceAgePassive:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (!client.Player.Alive)
                                            {
                                                while (RemoveSpells.Count > 0)
                                                    spellclient.RemoveItem(RemoveSpells.Dequeue());
                                                Role.FloorSpell.ClientFloorSpells FloorSpell;
                                                client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                                break;
                                            }
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                foreach (var user in spellclient.GMap.View.Roles(Role.MapObjectType.Player, spellclient.X, spellclient.Y, p => Role.Core.GetDistance(p.X, p.Y, spellclient.X, spellclient.Y) <= 18))
                                                    user.Send(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    PirateSkills.ExecuteSkills(client, (ushort)Role.Flags.SpellIDPirate.ColdBloodline, client.Player.UID, 0, client.Player.X, client.Player.Y);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region PheasantBeak
                            case (ushort)Role.Flags.SpellIDPirate.PheasantBeak:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;
                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                #region ColdBloodline
                                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    PirateSkills.ExecuteSkills(client, (ushort)Role.Flags.SpellIDPirate.ColdBloodline, client.Player.UID, client.Player.UID, client.Player.X, client.Player.Y);
                                                #endregion
                                            }

                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region PheasantBeakPassive
                            case (ushort)Role.Flags.SpellIDPirate.PheasantBeakPassive:
                                {
                                    if (!client.Player.Alive)
                                        return;
                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {
                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                #region ColdBloodline
                                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    PirateSkills.ExecuteSkills(client, (ushort)Role.Flags.SpellIDPirate.ColdBloodline, client.Player.UID, client.Player.UID, client.Player.X, client.Player.Y);
                                                #endregion
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region LonelyBattle
                            case (ushort)Role.Flags.SpellIDDune.LonelyBattle:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip: monsters = power-30000): AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region CliffCrusher
                            case (ushort)Role.Flags.SpellIDDune.CliffCrusher:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip: monsters = power-30000): AnimationObj.Damage = AnimationObj.Damage * 100 / 500; // تعديل الدمج
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, false, 1); // [CombatAudit 2026-10-14] delayed 2nd hit uses the official 2nd-hit field (CliffCrusher)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion
                            #region FinalStand
                            case (ushort)Role.Flags.SpellIDDune.FinalStand:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            // [CombatAudit 2026-10-14] removed custom nerf (official Dune tooltip: monsters = power-30000): AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj, false, 1); // [CombatAudit 2026-10-14] delayed 2nd hit uses the official 2nd-hit field (FinalStand)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region SwallowDive
                            case (ushort)Role.Flags.SpellIDDune.SwallowDive:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region MoonwardLeap
                            case (ushort)Role.Flags.SpellIDDune.MoonwardLeap:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region TempestStrike
                            case (ushort)Role.Flags.SpellIDDune.TempestStrike:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region SwallowDive2
                            case (ushort)Role.Flags.SpellIDDune.SwallowDive2:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region WandererNormalATK
                            case (ushort)Role.Flags.SpellIDDune.WandererNormalATK:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region SheathParry
                            case (ushort)Role.Flags.SpellIDDune.SheathParry:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region FleetingShadow
                            case (ushort)Role.Flags.SpellIDDune.FleetingShadow:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region SkyStep
                            case (ushort)Role.Flags.SpellIDDune.SkyStep:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region HeroicHeart
                            case (ushort)Role.Flags.SpellIDDune.HeroicHeart:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region TideReversal
                            case (ushort)Role.Flags.SpellIDDune.TideReversal:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region DualStrike
                            case (ushort)Role.Flags.SpellID.DualStrike:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region InnerSight
                            case (ushort)Role.Flags.SpellID.InnerSight:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region PulseLock
                            case (ushort)Role.Flags.SpellID.PulseLock:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region MightyBlaze
                            case (ushort)Role.Flags.SpellID.MightyBlaze:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region MirrorStrike
                            case (ushort)Role.Flags.SpellID.MirrorStrike:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region DreadSlash
                            case (ushort)Role.Flags.SpellID.DreadSlash:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region RemoteHit
                            case (ushort)Role.Flags.SpellID.RemoteHit:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region Unruffled
                            case (ushort)Role.Flags.SpellID.Unruffled:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region ArchShadow
                            case (ushort)Role.Flags.SpellID.ArchShadow:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region DragonHeart
                            case (ushort)Role.Flags.SpellID.DragonHeart:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region Inchstrength
                            case (ushort)Role.Flags.SpellID.Inchstrength:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion

                            #region BeastShield
                            case (ushort)Role.Flags.SpellID.BeastShield:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region Hunter
                            case (ushort)Role.Flags.SpellID.Hunter:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 20 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region BladeTempest
                            case (ushort)Role.Flags.SpellID.BladeTempest:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                {
                                                    var monster = obj as Game.MsgMonster.MonsterRole;
                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 60 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion



                            #region ChaoticDanceAttack
                            case (ushort)Role.Flags.SpellID.ChaoticDanceAttack:
                                {

                                    var spellclient = ID.Value;
                                    Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var spells = spellclient.Spells.ToArray();
                                        foreach (var spell in spells)
                                        {

                                            if (spellclient.CheckInvocke(Now, spell))
                                            {
                                                RemoveSpells.Enqueue(spell);
                                                spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                                                spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                                            }
                                            else if (spellclient.CheckElseInvocke(Now, spell))
                                            {
                                                spellclient.CreateMsgSpell(0);
                                                spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                                                //foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                                                //{
                                                //    var monster = obj as Game.MsgMonster.MonsterRole;
                                                //    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                //    {
                                                //        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                                                //        {
                                                //            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                //            Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                                                //            Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                                                //            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                //        }
                                                //    }
                                                //}
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                                                {
                                                    var target = obj as Role.Player;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            AnimationObj.Damage = AnimationObj.Damage * 40 / 100; // Mohe Done (Attack)
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                                                {
                                                    var target = obj as Role.SobNpc;

                                                    if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                                                    {
                                                        if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                                                        {
                                                            Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                                            Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                                                            Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                                                            spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                                                        }
                                                    }
                                                }
                                                spellclient.SendView(stream, client);
                                            }
                                        }
                                    }
                                    while (RemoveSpells.Count > 0)
                                    {
                                        spellclient.RemoveItem(RemoveSpells.Dequeue());
                                    }

                                    if (spellclient.Spells.Count == 0)
                                    {
                                        Role.FloorSpell.ClientFloorSpells FloorSpell;
                                        client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                                    }
                                    break;
                                }
                            #endregion


                            #region PulseLock
                            //case (ushort)Role.Flags.SpellID.TripleBlasts:
                            //    {

                            //        var spellclient = ID.Value;
                            //        Queue<Role.FloorSpell> RemoveSpells = new Queue<Role.FloorSpell>();
                            //        using (var rec = new ServerSockets.RecycledPacket())
                            //        {
                            //            var stream = rec.GetStream();
                            //            var spells = spellclient.Spells.ToArray();
                            //            foreach (var spell in spells)
                            //            {

                            //                if (spellclient.CheckInvocke(Now, spell))
                            //                {
                            //                    RemoveSpells.Enqueue(spell);
                            //                    spell.FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.RemoveEffect;

                            //                    spell.FloorPacket.SendScreen(stream.ItemPacketCreate(spell.FloorPacket));
                            //                }
                            //                else if (spellclient.CheckElseInvocke(Now, spell))
                            //                {
                            //                    spellclient.CreateMsgSpell(0);
                            //                    spellclient.SpellPacket.UID = spell.FloorPacket.m_UID;
                            //                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
                            //                    {
                            //                        var monster = obj as Game.MsgMonster.MonsterRole;
                            //                        if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                            //                        {
                            //                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, monster, spell.DBSkill))
                            //                            {
                            //                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                            //                                Game.MsgServer.AttackHandler.Calculate.Physical.OnMonster(client.Player, monster, spell.DBSkill, out AnimationObj);
                            //                                AnimationObj.Damage = AnimationObj.Damage * 100 / 500;
                            //                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, client, monster);
                            //                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                            //                            }
                            //                        }
                            //                    }
                            //                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                            //                    {
                            //                        var target = obj as Role.Player;

                            //                        if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                            //                        {
                            //                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, target, spell.DBSkill))
                            //                            {
                            //                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                            //                                Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(client.Player, target, spell.DBSkill, out AnimationObj);
                            //                                AnimationObj.Damage = AnimationObj.Damage * 60 / 100;
                            //                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, target);
                            //                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                            //                            }
                            //                        }
                            //                    }
                            //                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.SobNpc))
                            //                    {
                            //                        var target = obj as Role.SobNpc;

                            //                        if (Role.Core.GetDistance(obj.X, obj.Y, spell.FloorPacket.m_X, spell.FloorPacket.m_Y) <= spell.DBSkill.MaxTargets)
                            //                        {
                            //                            if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, target, spell.DBSkill))
                            //                            {
                            //                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                            //                                Game.MsgServer.AttackHandler.Calculate.Physical.OnNpcs(client.Player, target, spell.DBSkill, out AnimationObj);
                            //                                Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, client, target);
                            //                                spellclient.SpellPacket.Targets.Enqueue(AnimationObj);
                            //                            }
                            //                        }
                            //                    }
                            //                    spellclient.SendView(stream, client);
                            //                }
                            //            }
                            //        }
                            //        while (RemoveSpells.Count > 0)
                            //        {
                            //            spellclient.RemoveItem(RemoveSpells.Dequeue());
                            //        }

                            //        if (spellclient.Spells.Count == 0)
                            //        {
                            //            Role.FloorSpell.ClientFloorSpells FloorSpell;
                            //            client.Player.FloorSpells.TryRemove(spellclient.DBSkill.ID, out FloorSpell);
                            //        }
                            //        break;
                            //    }
                            #endregion


                        }
                    }
                }
                #region Check Items
                foreach (var item in client.Player.View.Roles(Role.MapObjectType.Item))
                {
                    if (!item.Alive)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var PItem = item as Game.MsgFloorItem.MsgItem;
                            if (PItem.ItemBase != null) PItem.ItemBase.Color = 0;
                            PItem.SendAll(stream, item.IsTrap() ? Game.MsgFloorItem.MsgDropID.RemoveEffect : Game.MsgFloorItem.MsgDropID.Remove);
                            client.Map.View.LeaveMap<Role.IMapObj>(item);
                            continue;
                        }
                    }
                    if (item.IsTrap())
                    {
                        var FloorItem = item as Game.MsgFloorItem.MsgItem;
                        if (FloorItem.ItemBase == null)
                            continue;
                        switch (FloorItem.ItemBase.ITEM_ID)
                        {
                            case Game.MsgFloorItem.MsgItemPacket.NormalDaggerStorm:
                            case Game.MsgFloorItem.MsgItemPacket.SoulOneDaggerStorm:
                            case Game.MsgFloorItem.MsgItemPacket.SoulTwoDaggerStorm:
                                {
                                    if (DateTime.Now >= FloorItem.AttackStamp.AddMilliseconds(900))
                                    {
                                        FloorItem.AttackStamp = DateTime.Now;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(FloorItem.OwnerEffert.Player.UID, 0, FloorItem.X, FloorItem.Y, FloorItem.DBSkill.ID, FloorItem.DBSkill.Level, FloorItem.OwnerEffert.MySpells.ClientSpells[FloorItem.DBSkill.ID].UseSpellSoul);
                                            foreach (var _monster in FloorItem.GMap.View.Roles(Role.MapObjectType.Monster, FloorItem.X, FloorItem.Y, p => Role.Core.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                            {
                                                var monster = _monster as Game.MsgMonster.MonsterRole;
                                                if (CanAttackMonster.Verified(FloorItem.OwnerEffert, monster, FloorItem.DBSkill))
                                                {
                                                    MsgSpellAnimation.SpellObj AnimationObj;
                                                    Game.MsgServer.AttackHandler.Calculate.Range.OnMonster(FloorItem.OwnerEffert.Player, monster, FloorItem.DBSkill, out AnimationObj);
                                                    Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, FloorItem.OwnerEffert, monster);
                                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                                }
                                            }
                                            foreach (var player in FloorItem.GMap.View.Roles(Role.MapObjectType.Player, FloorItem.X, FloorItem.Y, p => Role.Core.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                            {
                                                if (player.UID != FloorItem.OwnerEffert.Player.UID)
                                                {
                                                    var atacked = player as Role.Player;
                                                    if (CanAttackPlayer.Verified(FloorItem.OwnerEffert, atacked, FloorItem.DBSkill))
                                                    {
                                                        MsgSpellAnimation.SpellObj AnimationObj;
                                                        Game.MsgServer.AttackHandler.Calculate.Range.OnPlayer(FloorItem.OwnerEffert.Player, atacked, FloorItem.DBSkill, out AnimationObj);
                                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, FloorItem.OwnerEffert, atacked);
                                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                                    }
                                                }
                                            }
                                            foreach (var player in FloorItem.GMap.View.Roles(Role.MapObjectType.SobNpc, FloorItem.X, FloorItem.Y, p => Role.Core.GetDistance(p.X, p.Y, FloorItem.MsgFloor.m_X, FloorItem.MsgFloor.m_Y) <= 3))
                                            {
                                                if (player.UID != FloorItem.OwnerEffert.Player.UID)
                                                {
                                                    var atacked = player as Role.SobNpc;
                                                    if (CanAttackNpc.Verified(FloorItem.OwnerEffert, atacked, FloorItem.DBSkill))
                                                    {
                                                        MsgSpellAnimation.SpellObj AnimationObj;
                                                        Game.MsgServer.AttackHandler.Calculate.Range.OnNpcs(FloorItem.OwnerEffert.Player, atacked, FloorItem.DBSkill, out AnimationObj);
                                                        Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, AnimationObj, FloorItem.OwnerEffert, atacked);
                                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                                    }
                                                }
                                            }
                                            MsgSpell.SetStream(stream);
                                            MsgSpell.Send(client);
                                        }
                                    }
                                    break;
                                }
                            case MsgItemPacket.FloraWard:
                                {
                                    if (DateTime.Now >= FloorItem.AttackStamp.AddSeconds((int)900))
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            foreach (var user in FloorItem.GMap.View.Roles(Role.MapObjectType.Player, FloorItem.X, FloorItem.Y, p => Role.Core.GetDistance(p.X, p.Y, FloorItem.X, FloorItem.Y) <= 10))
                                            {
                                                var player = user as Role.Player;
                                                var MyTeam = FloorItem.OwnerEffert;
                                                bool IsEnemy = true;
                                                if (MyTeam != null)
                                                {
                                                    if (player.Owner.Team.UID == MyTeam.Team.UID
                                                            || MyTeam.Player.GuildID == player.GuildID
                                                            || MyTeam.Player.ClanUID == player.ClanUID
                                                            || MyTeam.Player.Associate.Contain(Role.Instance.Associate.Friends, player.UID)
                                                            || MyTeam.Player.MyClan.Ally.ContainsKey(player.ClanUID)
                                                            || MyTeam.Player.MyGuild.Ally.ContainsKey(player.GuildID))
                                                    {
                                                        player.AddSpellFlag(MsgUpdate.Flags.FloraWard, 5, true);
                                                        player.Owner.ExtraStatus.TryAdd(RoleStatus.StatuTyp.FloraWard, new RoleStatus((uint)FloorItem.DBSkill.Damage2, 5));
                                                        player.Owner.UpdateStamina(stream, (uint)FloorItem.DBSkill.Damage2);
                                                        IsEnemy = false;
                                                    }
                                                }
                                                else if (player.UID == FloorItem.OwnerEffert.Player.UID)
                                                {
                                                    player.AddSpellFlag(MsgUpdate.Flags.FloraWard, 5, true);
                                                    player.Owner.ExtraStatus.TryAdd(RoleStatus.StatuTyp.FloraWard, new RoleStatus((uint)FloorItem.DBSkill.Damage2, 5));
                                                    player.Owner.UpdateStamina(stream, (uint)FloorItem.DBSkill.Damage2);
                                                    IsEnemy = false;
                                                }
                                                if (IsEnemy)
                                                {
                                                    if (player.ContainFlag(MsgUpdate.Flags.DivineEmptiness))
                                                    {
                                                        player.RemoveFlag(MsgUpdate.Flags.DivineEmptiness);
                                                        player.Owner.Equipment.QueryEquipment(player.Owner.Equipment.Alternante);
                                                    }
                                                }
                                            }
                                        }
                                        FloorItem.AttackStamp = DateTime.Now;
                                    }
                                    break;
                                }

                        }

                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }

        }

        public static unsafe void AutoAttackCallback(Client.GameClient client)
        {
            if (Program.ExitRequested)
                return;
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Fake)
                    return;
                if (client.OnAutoAttack && (client.Player.Alive || client.Player.ContainFlag(MsgUpdate.Flags.NeptuneCurse)) && DateTime.Now >= client.Player.AttackStamp.AddMilliseconds(client.Equipment.AttackSpeed(true)))
                {
                    if (client.AutoAttack == null)
                    {

                    }
                    InteractQuery actions = new InteractQuery();
                    if (client.AutoAttack != null)
                    {
                        actions = InteractQuery.ShallowCopy(client.AutoAttack);
                    }
                    GameClient Client;
                    if (Pool.GamePoll.TryGetValue(actions.OpponentUID, out Client))
                    {
                        short Dis = Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate.Base.GetDistance(client.Player.X, client.Player.Y, Client.Player.X, Client.Player.Y);
                        if (Dis > 2)
                        {
                            client.OnAutoAttack = false;
                            return;
                        }
                    }
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dizzy))
                    {
                        client.OnAutoAttack = false;
                        return;
                    }

                    if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                    {
                        InteractQuery action = new InteractQuery();
                        action = InteractQuery.ShallowCopy(client.AutoAttack);
                        client.Player.RandomSpell = action.SpellID;
                        MsgAttackPacket.Process(client, action);
                        client.OnAutoAttack = true;
                        return;
                    }
                    if (client.Player.Owner.Rune.IsEquipped("DeadlySight"))
                    {
                        InteractQuery action = new InteractQuery();
                        action = InteractQuery.ShallowCopy(client.AutoAttack);
                        client.Player.RandomSpell = action.SpellID;
                        MsgAttackPacket.Process(client, action);
                    }
                    if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                    {
                        InteractQuery action = new InteractQuery();
                        action = InteractQuery.ShallowCopy(client.AutoAttack);
                        client.Player.RandomSpell = action.SpellID;
                        MsgAttackPacket.Process(client, action);
                    }
                    if (DateTime.Now >= client.Player.AttackStamp.AddMilliseconds(client.Equipment.AttackSpeed(true)))
                    {
                        InteractQuery action = new InteractQuery();
                        action = InteractQuery.ShallowCopy(client.AutoAttack);
                        client.Player.RandomSpell = action.SpellID;
                        MsgAttackPacket.Process(client, action);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void SaveMeleAttack(Client.GameClient client)
        {
            if (Program.ExitRequested)
                return;
            try
            {
                if (client.SaveMele == null || client == null || !client.FullLoading || client.Player == null || client.Fake)
                {
                    return;
                }
                client.onSaveMele = false;
                InteractQuery action = new InteractQuery();
                action = InteractQuery.ShallowCopy(client.SaveMele.AutoAttack);
                client.SaveMele.nSaveMele = true;
                MsgAttackPacket.Process(client.SaveMele, action);
                client.SaveMele.nSaveMele = false;
                client.SaveMele = null;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void BuffersCallback(Client.GameClient client)
        {

            if (Program.ExitRequested)
                return;
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Fake)
                    return;

                DateTime Now = DateTime.Now;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Dead) && client.Player.HitPoints != 0)
                {
                    if (client.Player.HitPoints != 0)
                        client.Player.IsDead(client.Player);
                 
                }
                MsgYuanshen.UpdateEnergy(client, client.Player.EonspiritLevel);

                Game.MsgServer.AttackHandler.DuneSkills.UpdateEnergy(client);

                #region ChaoticDanceRestoringHP
                if (DateTime.Now >= client.Player.RestoringHPStamp.AddMilliseconds(1))
                {
                    if (client.Player.Alive && client.Player.ActiveChaoticDance == true)
                    {
                        client.Player.HitPoints += (int)client.Player.RestoringHP;
                        if (client.Player.HitPoints > client.Status.MaxHitpoints)
                            client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    }
                }
                if (DateTime.Now >= client.Player.RestoringStaminaStamp.AddMilliseconds(700))
                {
                    if (client.Player.Alive && client.Player.ActiveChaoticDance == true)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 2 : 100);
                            client.UpdateStamina(stream, (uint)MaxStamina);
                        }
                    }
                }
                #endregion


                #region SuperSpeedGun
                if (DateTime.Now >= client.Player.RestoringHPStamp.AddMilliseconds(1))
                {
                    if (client.Player.Alive && client.Player.ActiveSuperSpeedGun == true)
                    {
                        client.Player.HitPoints += (int)client.Player.RestoringHP;
                        if (client.Player.HitPoints > client.Status.MaxHitpoints)
                            client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    }
                }
                if (DateTime.Now >= client.Player.RestoringStaminaStamp.AddMilliseconds(700))
                {
                    if (client.Player.Alive && client.Player.ActiveSuperSpeedGun == true)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 2 : 100);
                            client.UpdateStamina(stream, (uint)MaxStamina);
                        }
                    }
                }
                #endregion

                #region Remove Rider
                if (client.Player.Map == 22382 ||
                    client.Player.Map == 22389 ||
                    client.Player.Map == 22385 ||
                    client.Player.Map == 22388 ||
                    client.Player.Map == 22384 ||
                    client.Player.Map == 22383 ||
                    client.Player.Map == 22380 ||
                    client.Player.Map == 22381 ||
                    client.Player.Map == 26700 ||
                    client.Player.Map == 26701 ||
                    client.Player.Map == 26702 ||
                    client.Player.Map == 22386 ||
                    client.Player.Map == 22387 ||

                    client.Player.Map == 5661 ||
                    client.Player.Map == 1487 ||
                    client.Player.Map == 1486 ||
                    client.Player.Map == 1483 ||
                    client.Player.Map == 1484 ||
                    client.Player.Map == 1485 ||
                    client.Player.Map == 6526 ||
                    client.Player.Map == 2515 ||
                    client.Player.Map == 12099 ||
                    client.Player.Map == 9855 ||
                    client.Player.Map == 3820 ||
                    client.Player.Map == 1508 ||

                     client.Player.Map == 1518 ||
                      client.Player.Map == 50016 ||
                       client.Player.Map == 50100 ||
                        client.Player.Map == 50101 ||
                         client.Player.Map == 50102 ||
                          client.Player.Map == 50103 ||
                           client.Player.Map == 50018 ||
                           client.Player.Map == 50019 ||
                           client.Player.Map == 50020 ||
                           client.Player.Map == 50021 ||
                           client.Player.Map == 50104 ||
                            client.Player.Map == 50105 ||
                            client.Player.Map == 50017 ||

                              client.Player.Map == 22340 ||
                                client.Player.Map == 22341 ||
                                  client.Player.Map == 6522 ||


                    client.Player.Map == 5061 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058 || client.Player.Map == 1005 || client.Player.Map == 26392 || client.Player.Map == 3053 || client.Player.Map == 9855 || client.Player.Map == 1004)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    }
                }
                #endregion
                #region Monk Bug Rune
                if (client.Player.ContainFlag(MsgUpdate.Flags.IronShield) && client.Player.ContainFlag(MsgUpdate.Flags.IronGuard))
                {
                    client.Socket.Disconnect();
                }
                #endregion
                //#region Monk Bug
                //if (!client.Player.ContainFlag(MsgUpdate.Flags.TyrantAura))
                //{
                //    client.Player.AddAura(client.Player.UseAura, null, 0);
                //}
                //#endregion
                #region Room Flag
                if (client.Player.Map == 5061 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride) || client.Player.ContainFlag(MsgUpdate.Flags.AzureShield))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        client.Player.RemoveFlag(MsgUpdate.Flags.AzureShield);
                    }
                }
                if (client.Player.Map == 1138 || client.Player.Map == 10133 || client.Player.Map == 3071 || client.Player.Map == 10134 || client.Player.Map == 10741 || client.Player.Map == 3030)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                    }
                }
                #endregion

                #region Room Flag WindWalker
                if (client.Player.Map == 5051 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.HealingSnow);
                    }
                }
                if (client.Player.Map == 5051 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                {
                    if (client.Player.ContainFlag(MsgUpdate.Flags.FreezingPelter))
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.FreezingPelter);
                    }
                }
                #endregion

                #region PK Points
                if (client.Player.PKPoints > 0)
                {
                    if (Now > client.Player.PkPointsStamp.AddMinutes(6))
                    {
                        client.Player.PKPoints -= 1;
                        client.Player.PkPointsStamp = DateTime.Now;
                    }
                }
                #endregion
                #region XP List ( تعديل سرعة شريط الأكسبي انو يملا )
                if (Now >= client.Player.XPListStamp.AddSeconds(4) && client.Player.Alive)
                {
                    client.Player.XPListStamp = Now.AddSeconds(4);
                    if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.XPList))
                    {
                        client.Player.XPCount++;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.SendUpdate(stream, client.Player.XPCount, MsgUpdate.DataType.XPCircle);
                            if (client.Player.XPCount >= 100)
                            {
                                client.Player.XPCount = 0;
                                client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.XPList, 20, true);
                                client.Player.SendUpdate(stream, 1, MsgUpdate.DataType.XPList);

                                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "xp" });
                            }
                        }
                    }
                }
                #endregion
                #region Undying Will
                if (client.Player.Alive && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingWill))
                {
                    if (Time32.Now >= client.Player.UndyingWillStamp.AddSeconds(5))
                    {
                        client.Player.UndyingWillStamp = Time32.Now;
                        MsgSpell ClientSpell;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.UndyingWill, out ClientSpell))
                        {
                            if (client.Player.HitPoints < client.Status.MaxHitpoints || client.Player.Mana < client.Status.MaxMana)
                            {
                                var DBSpell = Pool.Magic[ClientSpell.ID][ClientSpell.Level];
                                client.Player.HitPoints += (int)(DBSpell.Damage * client.Status.MaxHitpoints / 100);
                                client.Player.Mana += (ushort)(DBSpell.DamageOnMonster * client.Status.MaxMana / 100);
                                if (client.Player.HitPoints > client.Status.MaxHitpoints)
                                    client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                                if (client.Player.Mana > client.Status.MaxMana)
                                    client.Player.Mana = (ushort)client.Status.MaxMana;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, (MsgStringPacket.StringID)30, true, "hxdf_hf");

                                    ushort firstlevel = ClientSpell.Level;
                                    if (ClientSpell.Level < Pool.Magic[ClientSpell.ID].Count - 1)
                                    {
                                        if (client.Player.Level >= DBSpell.NeedLevel)
                                        {
                                            ClientSpell.Experience += (int)(client.Status.MaxHitpoints / 100 * Program.ServerConfig.ExpRateSpell);
                                            if (ClientSpell.Experience > DBSpell.Experience)
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
                                                    client.SendSysMesage("You increased the spell level!", MsgMessage.ChatMode.TopLeftSystem, MsgMessage.MsgColor.red, false);
                                            }
                                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                                            client.Send(stream.SpellCreate(ClientSpell));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Stamina & Vigor
                if (client.Player.Alive && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Fly))
                {
                    byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);
                    if (client.Equipment.UseMonkEpicWeapon)
                    {
                        MsgSpell user_spell = null;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.GraceofHeaven, out user_spell))
                        {
                            Database.MagicType.Magic DBSpell = Pool.Magic[user_spell.ID][user_spell.Level];
                            MaxStamina += (byte)DBSpell.Damage;
                        }
                    }
                    if (client.Player.Stamina < MaxStamina && !client.Player.ContainFlag(MsgUpdate.Flags.Duel) && !client.Player.ContainFlag(MsgUpdate.Flags.FrostArrows) && Time32.Now >= client.Player.DuelEndStamp.AddSeconds(20))
                    {
                        if (DateTime.Now >= client.Player.StaminaStamp.AddSeconds(1) || client.Player.ContainFlag(MsgUpdate.Flags.Rampage))
                        {
                            #region Rampage
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.Rampage) && Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                MsgSpell user_spell = null;
                                if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Rampage, out user_spell) && client.Rune.IsEquipped("Rampage"))
                                {
                                    Database.MagicType.Magic DBSpell = Pool.Magic[user_spell.ID][user_spell.Level];
                                    if (Time32.Now >= client.Player.RampageStamp.AddMilliseconds(DBSpell.ColdTime))
                                    {
                                        if (client.Player.Stamina <= 10)
                                        {
                                            client.Player.RampageStamp = Time32.Now;
                                            client.Player.AddFlag(MsgUpdate.Flags.Rampage, (int)DBSpell.Duration, true);
                                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = recycledPacket.GetStream();
                                                {
                                                    MsgUpdate upd = new MsgUpdate(stream, client.Player.UID, 1);
                                                    upd.Append(stream, MsgUpdate.DataType.FineRain, (uint)MsgUpdate.Flags.Rampage, (uint)DBSpell.Duration, client.Status.MaxHitpoints, client.Status.MaxHitpoints);
                                                    client.Send(upd.GetArray(stream));

                                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                                                , client.Player.UID, 0, 0, DBSpell.ID
                                                , DBSpell.Level, 0);
                                                    MsgSpell.SetStream(stream);
                                                    MsgSpell.Send(client);
                                                }
                                            }
                                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                        }
                                    }
                                }
                            }
                            #endregion
                            client.Player.StaminaStamp = DateTime.Now;
                            ushort addstamin = 0;
                            if (Now > client.Player.FanRecoverStamin.AddSeconds(8))
                            {
                                if (client.Player.OnXPSkill() == MsgUpdate.Flags.DragonFlow)
                                    addstamin += 20;
                            }
                            if (client.Player.Action == Role.Flags.ConquerAction.Sit)
                                addstamin += 10;
                            else
                                addstamin += 2;
                            if (client.Player.OnXPSkill() == MsgUpdate.Flags.Omnipotence)
                                addstamin *= 2;

                            if (client.Player.ContainFlag(MsgUpdate.Flags.WindWalkerFan))
                            {
                                if (Now > client.Player.FanRecoverStamin.AddSeconds(5))
                                {
                                    addstamin += 10;
                                    client.Player.FanRecoverStamin = DateTime.Now;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        if (client.Player.Stamina + addstamin < MaxStamina)
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "TSM_SXJ_HPhf");
                                    }
                                }

                            }
                            if (client.Player.ContainFlag(MsgUpdate.Flags.SparkShield))
                            {
                                if (Time32.Now >= client.Player.LightningShieldStamp.AddSeconds(8))
                                {
                                    addstamin += 20;
                                    client.Player.LightningShieldStamp = Time32.Now;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        if (client.Player.Stamina + addstamin < MaxStamina)
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "thor_HPrec");
                                    }
                                }
                            }

                            client.Player.Stamina = (ushort)Math.Min((int)(client.Player.Stamina + addstamin), MaxStamina);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                            if (client.Player.ContainFlag(MsgUpdate.Flags.LightningShield))
                            {
                                if (Time32.Now >= client.Player.LightningShieldStamp.AddSeconds(8))
                                {
                                    addstamin += 20;
                                    client.Player.LightningShieldStamp = Time32.Now;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        if (client.Player.Stamina + addstamin < MaxStamina)
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "thor_HPrec");
                                    }
                                }
                            }

                            client.Player.Stamina = (ushort)Math.Min((int)(client.Player.Stamina + addstamin), MaxStamina);
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                        }
                    }

                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Ride))
                    {
                        if (client.Player.CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags.Ride, Now))
                        {
                            if (client.Vigor < client.Status.MaxVigor)
                            {
                                client.Vigor = (ushort)Math.Min(client.Vigor + 2, client.Status.MaxVigor);

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Send(stream.ServerInfoCreate(client.Vigor));
                                }
                            }
                        }

                    }
                }
                #endregion


                #region Healer
                if (client.Player.Alive && !client.Player.ContainFlag(MsgUpdate.Flags.HealingSnow))
                {
                    byte itemLevel = 0;
                    ushort points = 0;
                    if (client.Rune.IsEquipped("Healer", ref itemLevel))
                    {
                        if (Time32.Now >= client.Player.HealerStamp.AddSeconds(5))
                        {
                            client.Player.HealerStamp = Time32.Now;
                            if (client.Player.HitPoints < client.Status.MaxHitpoints || client.Player.Mana < client.Status.MaxMana)
                            {
                                switch (itemLevel)
                                {
                                    case 1: points = 1000; break;
                                    case 2: points = 3000; break;
                                    case 3: points = 5000; break;
                                    case 4: points = 7000; break;
                                    case 5: points = 9000; break;
                                    case 6: points = 11000; break;
                                    case 7: points = 14000; break;
                                    case 8: points = 17000; break;
                                    case 9: points = 20000; break;
                                }
                                client.Player.HitPoints += points;
                                client.Player.Mana += points;
                                if (client.Player.HitPoints > client.Status.MaxHitpoints)
                                    client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                                if (client.Player.Mana > client.Status.MaxMana)
                                    client.Player.Mana = (ushort)client.Status.MaxMana;
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendString(stream, (MsgStringPacket.StringID)30, true, "hxdf_hf");
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Defense Potion
                if (client.Player.OnDefensePotion)
                {
                    if (Now >= client.Player.OnDefensePotionStamp)
                    {
                        client.Player.OnDefensePotion = false;
                    }
                }
                #endregion

                #region Attack Potion
                if (client.Player.OnAttackPotion)
                {
                    if (Now >= client.Player.OnAttackPotionStamp)
                    {
                        client.Player.OnAttackPotion = false;
                    }
                }
                #endregion

                #region Heaven Blessing
                if (client.Player.HeavenBlessing > 0)
                {
                    if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                    {
                        if (Now >= client.Player.HeavenBlessTime)
                        {
                            client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing);
                            client.Player.HeavenBlessing = 0;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, 0, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing);
                                client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Remove, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);

                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, 100);
                                client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                            }
                        }
                        if (client.Player.Map != 601 && client.Player.Map != 1039)
                        {
                            if (Now >= client.Player.ReceivePointsOnlineTraining)
                            {
                                client.Player.ReceivePointsOnlineTraining = Now.AddMinutes(1);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.IncreasePoints, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);//+10
                                }
                            }
                            if (Now >= client.Player.OnlineTrainingTime)
                            {
                                client.Player.OnlineTrainingPoints += 100000;
                                client.Player.OnlineTrainingTime = Now.AddMinutes(10);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.ReceiveExperience, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Enlighten
                if (client.Player.EnlightenReceive > 0)
                {
                    if (Now >= client.Player.EnlightenTime.AddMinutes(20))
                    {
                        client.Player.EnlightenTime = DateTime.Now;
                        client.Player.EnlightenReceive -= 1;
                    }
                }
                #endregion

                #region Double Exp Time
                if (client.Player.DExpTime > 0)
                {
                    client.Player.DExpTime -= 1;
                    if (client.Player.DExpTime == 0)
                        client.Player.RateExp = 1;
                }
                #endregion

                #region Exp Protection
                if (client.Player.ExpProtection > 0)
                    client.Player.ExpProtection -= 1;
                #endregion

                #region Expire VIP
                if (Now >= client.Player.ExpireVip)
                {
                    if (client.Player.VipLevel == 0)
                        client.Player.VipLevel = 1;
                    if (client.Player.VipLevel > 1)
                    {
                        client.Player.VipLevel = 1;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);

                            client.Player.UpdateVip(stream);
                        }
                    }
                }
                #endregion

                #region Ghost flag
                if (!client.Player.Alive && client.Player.CompleteLogin)
                {
                    if (DateTime.Now > client.Player.GhostStamp)
                    {
                        if (!client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
                        {
                            client.Player.AddFlag(Game.MsgServer.MsgUpdate.Flags.Ghost, Role.StatusFlagsBigVector32.PermanentFlag, true);
                            if (client.Player.Body % 10 == 5)
                                client.Player.TransformationID = 99;
                            else
                                client.Player.TransformationID = 98;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(stream.MapStatusCreate(client.Player.Map, client.Map.ID, client.Map.TypeStatus));
                            }
                        }
                    }
                }
                #endregion

                #region Activeness
                if (Now > client.Player.LoginTimer.AddHours(1))
                {
                    client.Player.LoginTimer = DateTime.Now;
                    client.Activeness.IncreaseTask(3);
                    client.Activeness.IncreaseTask(15);
                    client.Activeness.IncreaseTask(27);

                }
                #endregion

                #region Blackspot
                if (client.Player.BlackSpot)
                {
                    if (Now >= client.Player.Stamp_BlackSpot)
                    {
                        client.Player.BlackSpot = false;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            client.Player.View.SendView(stream.BlackspotCreate(false, client.Player.UID), true);
                        }
                    }
                }
                #endregion

                #region EagleEye Countdown
                if (client.Player.EagleEyeCountDown)
                {
                    if (Now >= client.Player.EagleEyeStamp.AddSeconds(20))
                    {
                        client.Player.EagleEyeCountDown = false;
                    }
                }
                #endregion

                #region Overwhelm
                if (client.Rune.IsEquipped("Overwhelm") || client.Rune.IsEquipped("Shadow Flow "))
                    client.Player.AddFlag(MsgUpdate.Flags.Overwhelm, Role.StatusFlagsBigVector32.PermanentFlag, false);
                else client.Player.RemoveFlag(MsgUpdate.Flags.Overwhelm);
                #endregion

                #region Crackstar
                if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CrackStarNegative) && client.Player.Alive)
                {
                    foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                    {
                        if (obj.UID == client.Player.UID) continue;
                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, obj.X, obj.Y) <= 3)
                        {
                            var Target = obj as Role.Player;
                            if (Target == null) continue;
                            if (Target.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CrackStar))
                            {
                                if (Game.MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(Target.Owner, client.Player, null))
                                {
                                    client.Player.CrackStarNegativeDealer = Target;
                                    client.Player.AddFlag(MsgUpdate.Flags.CrackStarNegative, 5, true);


                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, 0, 0, 0, 10010, 0, 0);
                                    Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                    Game.MsgServer.AttackHandler.Calculate.Physical.OnPlayer(Target, client.Player, null, out AnimationObj);
                                    Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, Target.Owner, client.Player);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(client);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Undying~Imprinting
                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingImprinting))
                {
                    uint limit = (uint)(client.Rune.IsEquipped("FightingWill") ? 120 : 60);
                    if (client.Player.ThunderStrikerUndyingImprinting < limit)
                    {
                        client.Player.ThunderStrikerUndyingImprinting++;
                        if (client.Player.ThunderStrikerUndyingImprinting % 60 == 0 && client.Player.ThunderStrikerUndyingImprinting > 0)
                        {
                            client.Player.Stamina += 20;
                            if (client.Player.Stamina > 100 + (client.Player.HeavenBlessing > 0 ? 50 : 0))
                                client.Player.Stamina = (byte)(100 + (client.Player.HeavenBlessing > 0 ? 50 : 0));
                        }
                    }
                    else if (client.Player.ThunderStrikerUndyingImprinting > limit)
                        client.Player.ThunderStrikerUndyingImprinting = limit;
                }
                #endregion

                #region PengchengMiles
                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KunpengTrek)
                    && client.Player.PengchengMilesCount < 3 &&
                    Now > client.Player.PengchengMilesStamp.AddSeconds(8))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.PengchengMilesStamp = DateTime.Now;
                        client.Player.UpdatePengchengMiles(stream, +1);
                    }
                }
                #endregion

                #region Flags
                foreach (var flag in client.Player.BitVector.GetFlags())
                {
                    if (flag.Expire(Now))
                    {
                        switch ((uint)flag.Key)
                        {
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Rampage:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.RiseofTaoism:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.IronGuard:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FineRain1:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FineRain2:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.BloodTide:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FireArrow:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.IceArrow:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.PoisonArrow:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WildDash:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.HolyProtection:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FlameShield:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.DivineEmptiness:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FlowKnack:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.PhoenixBlessing:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.DeadwoodCurse:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.CrackMantra1:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.CrackMantra2:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WildPhoenix:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.SolidBulwark:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Diabolize:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Vast:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Barrier:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Shell:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Storm:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Thrash:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.ThunderPirate:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Torrent:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Tide:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Splash:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Sailing:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Sense:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.ColdBloodline:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Dark:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.VenomMyth:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Solid:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WildSigilBurning:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.SlashSeal:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WhirlSigilAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.MudWall:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.DefenseDecreasing:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.ActiveWeapon:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.KunpengHeart:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.HawksAmbition:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WindElementEffect:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.RisingPhoenix:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.SoulReap:
                            case (uint)(Game.MsgServer.MsgUpdate.Flags)420:
                            case (uint)MsgUpdate.Flags.StarChainFire:
                                {
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)MsgUpdate.Flags.ChaoticDance:
                                {
                                    client.Player.ActiveChaoticDance = false;
                                    client.Player.IncreasingHP = 0;
                                    client.Player.IncreaseasStaminaPercent = 0;
                                    client.Player.RestoringHP = 0;
                                    client.Player.RestoringHPStamp = System.DateTime.Now;
                                    client.Player.RestoringStaminaStamp = System.DateTime.Now;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)MsgUpdate.Flags.SuperSpeedGun:
                                {
                                    client.Player.ActiveSuperSpeedGun = false;
                                    client.Player.IncreasingHP = 0;
                                    client.Player.IncreaseasStaminaPercent = 0;
                                    client.Player.RestoringHP = 0;
                                    client.Player.RestoringHPStamp = System.DateTime.Now;
                                    client.Player.RestoringStaminaStamp = System.DateTime.Now;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)(MsgUpdate.Flags)491:
                                {
                                    client.Player.AreaOccupier = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)(MsgUpdate.Flags)488:
                                {
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)MsgUpdate.Flags.HurricaneSweep:
                            case (uint)MsgUpdate.Flags.SkySoarer:
                                {
                                    client.Player.AreaOccupier = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)MsgUpdate.Flags.GrowFromHurt:
                                {
                                    client.Player.GrowFromHurtHitpoints = 0;
                                    client.Player.GrowFromHurtStamp = System.Time32.Now;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.ShieldofTruth:
                                {
                                    client.Player.HitRateFantasy = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WaveBreak:
                                {
                                    client.Player.WaveBreaKLife = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.Commanding:
                                {
                                    client.Player.SuanniCommandCount = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.PrisonSigilFutility:
                                {
                                    client.Player.FutilityDg = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WeepStorm:
                                {
                                    client.Player.WeepStronCantidad = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                            case (uint)Game.MsgServer.MsgUpdate.Flags.StarChainWater:
                                {
                                    client.Player.StarChainCount = 0;
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }

                            case (uint)Game.MsgServer.MsgUpdate.Flags.TyrantAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FendAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.MetalAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WoodAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WaterAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.FireAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.EartAura:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.NobleSpirit:
                            case (uint)Game.MsgServer.MsgUpdate.Flags.WackeSpirit:
                                {
                                    client.Player.AddAura(client.Player.UseAura, null, 0);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    break;
                                }
                        }
                    }
                    if (flag.Expire(Now))
                    {
                        #region HellVortex
                        if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.HellVortex)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                InteractQuery action = new InteractQuery()
                                {
                                    UID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.HellVortex,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Magic
                                };
                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(action), action);
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        }
                        #endregion


                        #region Duel
                        if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Duel)
                        {
                            #region ClearXP
                            if (client.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                                client.Player.RemoveFlag(client.Player.OnXPSkill());
                            client.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            client.Player.XPCount = 0;
                            #endregion
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                        #endregion
                        #region CrackStarNegative
                        if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.CrackStarNegative)
                        {
                            if (client.Player.CrackStarNegativeDealer != null)
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, 0, 0, 0, 10010, 0, 0);
                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj() { Damage = (uint)((double)client.Player.CrackStarNegativeDealer.HitPoints * 20d / 100d), UID = client.Player.UID, Hit = 1 };
                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client.Player.CrackStarNegativeDealer.Owner, client.Player);
                                MsgSpell.Targets.Enqueue(AnimationObj);

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(client);
                                }
                                client.Player.CrackStarNegativeDealer = null;
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                        #endregion
                        #region ThunderRampage
                        else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ThunderRampage)
                        {
                            if (client.Player.ThunderStrikerUndyingImprinting >= 60)
                            {
                                client.Player.ThunderStrikerUndyingImprinting -= 60;
                                client.Player.Stamina += 20;
                                if (client.Player.Stamina > 100 + (client.Player.HeavenBlessing > 0 ? 50 : 0))
                                    client.Player.Stamina = (byte)(100 + (client.Player.HeavenBlessing > 0 ? 50 : 0));
                                #region ChainedStorm(Rune Skill)
                                if (client.Rune.IsEquipped("ChainedStorm") && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WindstormBattleaxe))
                                {
                                    var spell = Pool.Magic[(ushort)Role.Flags.SpellID.WindstormBattleaxe][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.WindstormBattleaxe].Level];
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID
                                           , 0, client.Player.X, client.Player.Y, spell.ID
                                           , spell.Level, 0);
                                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = recycledPacket.GetStream();
                                        {
                                            MsgSpell.SetStream(stream);
                                            MsgSpell.Send(client);
                                        }
                                    }
                                    client.Player.AddSpellFlag(MsgUpdate.Flags.AttackUp, (int)(spell.Duration + 30), true);
                                }
                                #endregion
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                        #endregion
                        #region LightningShieldActivated
                        else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.LightningShieldActivated)
                        {
                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var stream = recycledPacket.GetStream();
                                {
                                    client.Player.SendUpdate(stream, MsgUpdate.Flags.LightningShieldActivated, 0, 0, 0, MsgUpdate.DataType.AzureShield);
                                    client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                                }
                            }
                        }
                        else
                        {

                            if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Superman || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cyclone
                                 || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.SuperCyclone)
                            {
                                Role.KOBoard.KOBoardRanking.AddItem(new Role.KOBoard.Entry() { UID = client.Player.UID, Name = client.Player.Name, Points = client.Player.KillCounter }, true);
                            }
                            client.Player.RemoveFlag((Game.MsgServer.MsgUpdate.Flags)flag.Key);
                        }
                        #endregion
                    }
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.HellVortex)
                    {
                        if (flag.CheckElseInvockes(Now))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                InteractQuery action = new InteractQuery()
                                {
                                    UID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.HellVortex,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Magic
                                };
                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(action), action);
                            }
                        }
                    }
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.SoulReap)
                    {
                        if (flag.CheckElseInvocke(Now))
                        {
                            byte rate = 0;
                            byte Solidness = 0;
                            if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                            {

                                switch (Solidness)
                                {
                                    case 1: rate = 10; break;
                                    case 2: rate = 20; break;
                                    case 3: rate = 30; break;
                                    case 4: rate = 40; break;
                                    case 5: rate = 50; break;
                                    case 6: rate = 60; break;
                                    case 7: rate = 70; break;
                                    case 8: rate = 85; break;
                                    case 9: rate = 100; break;
                                }

                            }
                            if (!Role.Core.Rate(rate))
                            {
                                int Damage = (int)client.Player.HitPoints / 2;
                                if (Damage > 0)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, client.Player.UID, 0, 0, 10700, 1, 0, 1);
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                                        AnimationObj.Damage = (uint)Damage;
                                        AnimationObj.Hit = 1;
                                        AnimationObj.UID = (uint)client.Player.UID;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, client.Player);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                        }
                    }
                    #region Duel
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Duel)
                    {
                        #region ClearXP
                        if (client.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                            client.Player.RemoveFlag(client.Player.OnXPSkill());
                        client.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        client.Player.XPCount = 0;
                        #endregion
                    }
                    #endregion
                    #region ScarofEarth
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ScarofEarth)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.ScarofEarthl != null && client.Player.AttackerScarofEarthl != null)
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    var DBSpell = client.Player.ScarofEarthl;
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(
                                        client.Player.UID
                                          , 0, client.Player.X, client.Player.Y, DBSpell.ID
                                          , DBSpell.Level, 0, 1);

                                    MsgSpellAnimation.SpellObj AnimationObj = new MsgSpellAnimation.SpellObj()
                                    {
                                        UID = client.Player.UID,
                                        Damage = (uint)DBSpell.Damage2,
                                        Hit = 1
                                    };

                                    Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client.Player.AttackerScarofEarthl, client.Player);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    MsgSpell.Send(client);
                                }
                            }
                        }
                    }
                    #endregion
                    #region DrainingHP
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.DrainingHP)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (Time32.Now >= client.Player.CrescentShadow.AddSeconds(8))
                            {
                                client.Player.RemoveFlag(MsgUpdate.Flags.DrainingHP);
                            }
                            else
                            {
                                int damage = (int)Math.Min(client.Player.MaxDecreaseHP, client.Player.HitPoints / 2);
                                if (client.Player.HitPoints == 1)
                                {
                                    damage = 0;
                                    goto jump;
                                }
                                client.Player.HitPoints = Math.Max(1, (client.Player.HitPoints - damage));
                                client.Player.SendUpdateHP();

                            jump:
                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = damage,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.View.SendView(stream.InteractionCreate(action), true);
                                }
                            }
                        }
                    }
                    #endregion
                    #region RisingPhoenix
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.RisingPhoenix)
                    {
                        if (flag.CheckInvoke(Now) && client.Player.Alive)
                        {
                            int Damage = 5000;

                            byte rate = 0;
                            byte Solidness = 0;
                            if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                            {

                                switch (Solidness)
                                {
                                    case 1: rate = 10; break;
                                    case 2: rate = 20; break;
                                    case 3: rate = 30; break;
                                    case 4: rate = 40; break;
                                    case 5: rate = 50; break;
                                    case 6: rate = 60; break;
                                    case 7: rate = 70; break;
                                    case 8: rate = 85; break;
                                    case 9: rate = 100; break;
                                }

                            }
                            if (!Role.Core.Rate(rate))
                            {
                                client.Player.HitPoints = Math.Max(1, client.Player.HitPoints - Damage);
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    InteractQuery action = new InteractQuery()
                                    {
                                        Damage = Damage,
                                        AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                        X = client.Player.X,
                                        Y = client.Player.Y,
                                        OpponentUID = client.Player.UID
                                    };
                                    client.Player.View.SendView(stream.InteractionCreate(action), true);
                                }
                            }
                        }
                    }
                    #endregion
                    #region WildSigilBurning
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.WildSigilBurning)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            byte rate = 0;
                            byte Solidness = 0;
                            if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                            {

                                switch (Solidness)
                                {
                                    case 1: rate = 10; break;
                                    case 2: rate = 20; break;
                                    case 3: rate = 30; break;
                                    case 4: rate = 40; break;
                                    case 5: rate = 50; break;
                                    case 6: rate = 60; break;
                                    case 7: rate = 70; break;
                                    case 8: rate = 85; break;
                                    case 9: rate = 100; break;
                                }

                            }
                            if (!Role.Core.Rate(rate))
                            {
                                int damage = (int)Math.Min(10000, client.Player.HitPoints / 2);
                                if (client.Player.HitPoints == 1)
                                {
                                    damage = 0;
                                    goto jump;
                                }
                                client.Player.HitPoints = Math.Max(1, (client.Player.HitPoints - damage));
                                client.Player.SendUpdateHP();

                            jump:
                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = damage,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.View.SendView(stream.InteractionCreate(action), true);
                                }
                            }
                        }
                    }
                    #endregion
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.WhirlSigilAura)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.Alive)
                            {
                                int damage = 0;
                                client.Player.WhirlSigilDg++;
                                if (client.Player.WhirlSigilDg == 1)
                                {
                                    damage = 20000;
                                }

                                if (client.Player.WhirlSigilDg == 2)
                                {
                                    damage = 24000;
                                }
                                byte Solidness = 0;
                                int Val_Solidness = 0;
                                if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                                {
                                    switch (Solidness)
                                    {
                                        case 1: Val_Solidness = 10; break;
                                        case 2: Val_Solidness = 20; break;
                                        case 3: Val_Solidness = 30; break;
                                        case 4: Val_Solidness = 40; break;
                                        case 5: Val_Solidness = 50; break;
                                        case 6: Val_Solidness = 60; break;
                                        case 7: Val_Solidness = 70; break;
                                        case 8: Val_Solidness = 85; break;
                                        case 9: Val_Solidness = 100; break;
                                    }
                                    if (Role.Core.Rate(Val_Solidness))
                                    {
                                        damage = 0;
                                    }
                                }
                                if (client.Player.HitPoints == 1)
                                {
                                    damage = 0;
                                    goto jump;
                                }
                                client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - damage));
                            jump:
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    InteractQuery action = new InteractQuery()
                                    {
                                        Damage = damage,
                                        AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                        X = client.Player.X,
                                        Y = client.Player.Y,
                                        OpponentUID = client.Player.UID
                                    };
                                    client.Player.View.SendView(stream.InteractionCreate(action), true);
                                }
                            }
                        }
                    }

                    #region  DragonFlow
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.DragonFlow)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            byte MaxStamina = (byte)(client.Player.HeavenBlessing > 0 ? 150 : 100);

                            if (client.Player.Stamina < MaxStamina)
                            {
                                client.Player.Stamina += 20;
                                client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, MaxStamina); using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Player.SendUpdate(stream, client.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                }
                            }
                        }
                    }
                    #endregion
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.HealingSnow)
                    {
                        if (flag.CheckInvoke(Now) && client.Player.Alive)
                        {
                            if (client.Player.HitPoints < client.Status.MaxHitpoints || client.Player.Mana < client.Status.MaxMana)
                            {
                                MsgSpell spell;
                                if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.HealingSnow, out spell))
                                {
                                    var arrayspells = Pool.Magic[(ushort)Role.Flags.SpellID.HealingSnow];
                                    var DbSpell = arrayspells[(ushort)Math.Min((int)spell.Level, arrayspells.Count - 1)];

                                    client.Player.HitPoints = (int)Math.Min(client.Status.MaxHitpoints, (int)(client.Player.HitPoints + DbSpell.Damage2));
                                    client.Player.Mana = (ushort)Math.Min(client.Status.MaxMana, (int)(client.Player.Mana + DbSpell.Damage3));
                                    client.Player.SendUpdateHP();
                                    client.Player.XPCount += 1;
                                }
                            }
                        }
                    }
                    #region Bleed
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Bleed)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.HitPoints < client.Player.BleedDamage)
                            {
                                client.Player.BleedDamage = 0;
                                goto jump;
                            }
                            client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - client.Player.BleedDamage));
                        jump:

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = client.Player.BleedDamage,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                client.Player.View.SendView(stream.InteractionCreate(action), true);
                            }
                        }
                    }
                    #endregion
                    #region Poisoned
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Poisoned)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            int damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.CalculatePoisonDamage((uint)client.Player.HitPoints, client.Player.PoisonLevel);

                            if (client.Player.HitPoints == 1)
                            {
                                damage = 0;
                                goto jump;
                            }
                            damage -= (int)((damage * Math.Min(client.Status.Detoxication, 90)) / 100);
                            client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - damage));

                        jump:

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    Damage = damage,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    OpponentUID = client.Player.UID
                                };
                                client.Player.View.SendView(stream.InteractionCreate(action), true);
                            }

                        }
                    }
                    #endregion
                    #region VenomMyth
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.VenomMyth)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.Alive)
                            {
                                byte Solidness = 0;
                                int Val_Solidness = 0;
                                if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                                {
                                    switch (Solidness)
                                    {
                                        case 1: Val_Solidness = 10; break;
                                        case 2: Val_Solidness = 20; break;
                                        case 3: Val_Solidness = 30; break;
                                        case 4: Val_Solidness = 40; break;
                                        case 5: Val_Solidness = 50; break;
                                        case 6: Val_Solidness = 60; break;
                                        case 7: Val_Solidness = 70; break;
                                        case 8: Val_Solidness = 85; break;
                                        case 9: Val_Solidness = 100; break;
                                    }
                                }
                                if (!Role.Core.Rate(Val_Solidness))
                                {
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, client.Player.UID, 0, 0, 20000, 1, 0, 1);
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                                    AnimationObj.Damage = (uint)client.Player.VenomDamage;
                                    AnimationObj.UID = (uint)client.Player.UID;
                                    Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, client.Player);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                    using (var recVen = new ServerSockets.RecycledPacket())
                                    {
                                        MsgSpell.SetStream(recVen.GetStream());
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.SlashSeal)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.Alive)
                            {
                                int damage = (int)client.Status.MaxHitpoints * 30 / 100;
                                if (client.Player.HitPoints == 1)
                                {
                                    damage = 0;
                                    goto jump;
                                }
                                byte Solidness = 0;
                                int Val_Solidness = 0;
                                if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                                {
                                    switch (Solidness)
                                    {
                                        case 1: Val_Solidness = 10; break;
                                        case 2: Val_Solidness = 20; break;
                                        case 3: Val_Solidness = 30; break;
                                        case 4: Val_Solidness = 40; break;
                                        case 5: Val_Solidness = 50; break;
                                        case 6: Val_Solidness = 60; break;
                                        case 7: Val_Solidness = 70; break;
                                        case 8: Val_Solidness = 85; break;
                                        case 9: Val_Solidness = 100; break;
                                    }
                                    if (Role.Core.Rate(Val_Solidness))
                                    {
                                        damage = 0;
                                    }
                                }
                                client.Player.HitPoints = Math.Max(1, (int)(client.Player.HitPoints - damage));
                            jump:
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    InteractQuery action = new InteractQuery()
                                    {
                                        Damage = damage,
                                        AtkType = (ushort)MsgAttackPacket.AttackID.Physical,
                                        X = client.Player.X,
                                        Y = client.Player.Y,
                                        OpponentUID = client.Player.UID
                                    };
                                    client.Player.View.SendView(stream.InteractionCreate(action), true);
                                }
                            }
                        }
                    }

                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Flame_Sigil_Rage)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (!client.Player.ContainFlag(MsgUpdate.Flags.Dead))
                            {
                                uint Damage = client.Player.FlameOfDestructionDg;
                                byte Solidness = 0;
                                uint[] r = { 0, 10, 20, 30, 40, 50, 60, 70, 85, 100 };
                                if (client.Rune.IsEquipped("Solidness", ref Solidness) || client.Rune.IsEquipped("Unbreakable", ref Solidness))
                                    if (Role.MyMath.Success(r[Solidness]))
                                        Damage = 0;
                                if (Damage > 0)
                                {
                                    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, client.Player.UID, 0, 0, 16540, 1, 0, 1);
                                    Game.MsgServer.AttackHandler.CheckAttack.CheckGemEffects.CheckRespouseDamage(client);
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        AnimationObj.Damage = (uint)Damage;
                                        AnimationObj.Effect = MsgAttackPacket.AttackEffect.FireCurse;
                                        AnimationObj.Hit = 1;
                                        AnimationObj.UID = client.Player.UID;
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, client.Player);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        MsgSpell.SetStream(stream);
                                        MsgSpell.Send(client);
                                    }
                                }
                            }
                        }
                    }

                    #region Insouciance
                    if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Insouciance)
                    {
                        if (flag.CheckElseInvocke(Now))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Insouciance))
                                {
                                    Database.MagicType.Magic Insouciance = Pool.Magic[(ushort)Role.Flags.SpellID.Insouciance][client.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.Insouciance].Level];
                                    client.UpdateStamina(stream, (uint)Insouciance.GDamage);
                                }

                            }
                        }
                    }
                    #endregion
                    #region ShurikenVortex
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.ShurikenVortex)
                    {
                        if (flag.CheckInvoke(Now))
                        {

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                InteractQuery action = new InteractQuery()
                                {
                                    UID = client.Player.UID,
                                    X = client.Player.X,
                                    Y = client.Player.Y,
                                    SpellID = (ushort)Role.Flags.SpellID.ShurikenEffect,
                                    AtkType = (ushort)MsgAttackPacket.AttackID.Magic
                                };

                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(action), action);
                            }
                        }
                    }
                    #endregion
                    #region RedName&&BlackName
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.RedName || flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.BlackName)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.PKPoints > 0)
                                client.Player.PKPoints -= 1;

                            client.Player.PkPointsStamp = DateTime.Now;
                        }
                    }
                    #endregion
                    #region Cursed
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Cursed)
                    {
                        if (flag.CheckInvoke(Now))
                        {
                            if (client.Player.CursedTimer > 0)
                                client.Player.CursedTimer -= 1;
                        }

                    }
                    #endregion
                    #region RuneFlags
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.TidalWave)
                    {
                        if (client.Player.XPCount < 100)
                            client.Player.XPCount++;
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.Quench)
                    {
                        if (client.Player.XPCount > 0)
                            client.Player.XPCount--;
                    }
                    else if (flag.Key == (int)Game.MsgServer.MsgUpdate.Flags.FineRain1)
                    {
                        if (client.Player.FineRainPower > 0 && client.Player.Alive)
                        {
                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var stream = recycledPacket.GetStream();

                                MsgUpdate upd = new MsgUpdate(stream, client.Player.UID, 1);
                                upd.Append(
                                    stream,
                                    MsgUpdate.DataType.FineRain,
                                    (uint)MsgUpdate.Flags.FineRain1,
                                    (uint)(Time32.Now.AllSeconds() - flag.Timer.AddSeconds(flag.Secounds).AllSeconds()),
                                    client.Player.FineRainPower,
                                    client.Player.FineRainPower);

                                client.Send(upd.GetArray(stream));
                            }
                            if (client.Player.FineRainPower == 0)
                            {
                                client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.FineRain1);
                            }
                        }
                    }
                }
                    #endregion
                #endregion
                #region Transform
                if (client.Player.OnTransform)
                {
                    if (client.Player.TransformInfo != null)
                    {
                        if (client.Player.TransformInfo.CheckUp(Now))
                            client.Player.TransformInfo = null;
                    }
                }
                #endregion

                #region Praying
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                {
                    if (client.Player.BlessTime < 7200000 - 30000)
                    {
                        if (Now > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            bool have = false;
                            foreach (var ownerpraying in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, ownerpraying.X, ownerpraying.Y) <= 2)
                                {
                                    var target = ownerpraying as Role.Player;
                                    if (target.ContainFlag(MsgUpdate.Flags.CastPray))
                                    {
                                        have = true;
                                        break;
                                    }
                                }
                            }
                            if (!have)
                                client.Player.RemoveFlag(MsgUpdate.Flags.Praying);
                            client.Player.CastPrayStamp = DateTime.Now;
                            client.Player.BlessTime += 30000;
                        }
                    }
                    else
                        client.Player.BlessTime = 3100000;
                }
                #endregion

                #region Castpray
                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray))
                {
                    if (client.Player.BlessTime < 7200000 - 60000)
                    {
                        if (Now > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            client.Player.CastPrayStamp = DateTime.Now;
                            client.Player.BlessTime += 60000;
                        }
                    }
                    else
                        client.Player.BlessTime = 7200000;
                    if (Now > client.Player.CastPrayActionsStamp.AddSeconds(5))
                    {
                        client.Player.CastPrayActionsStamp = DateTime.Now;
                        foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                        {
                            if (Role.Core.GetDistance(client.Player.X, client.Player.Y, obj.X, obj.Y) <= 1)
                            {
                                var Target = obj as Role.Player;
                                if (Target.Reborn < 2)
                                {
                                    if (!Target.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                                    {
                                        Target.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, Role.StatusFlagsBigVector32.PermanentFlag, true);

                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            ActionQuery action = new ActionQuery()
                                            {
                                                ObjId = client.Player.UID,
                                                dwParam = (uint)client.Player.Action,
                                                Timestamp = obj.UID
                                            };
                                            client.Player.View.SendView(stream.ActionCreate(action), true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (client.Player.BlessTime > 0)
                {
                    if (!client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.CastPray) && !client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Praying))
                    {

                        if (Now > client.Player.CastPrayStamp.AddSeconds(2))
                        {
                            if (client.Player.BlessTime > 2000)
                                client.Player.BlessTime -= 2000;
                            else
                                client.Player.BlessTime = 0;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Player.SendUpdate(stream, client.Player.BlessTime, Game.MsgServer.MsgUpdate.DataType.LuckyTimeTimer);
                            }
                            client.Player.CastPrayStamp = DateTime.Now;
                        }
                    }
                }
                #endregion

                #region Team invite
                if (client.Team != null)
                {
                    if (client.Team.AutoInvite == true && client.Player.Map != 1036 && client.Team.CkeckToAdd())
                    {
                        if (Now > client.Team.InviteTimer.AddSeconds(10))
                        {
                            client.Team.InviteTimer = Now;
                            foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                if (!client.Team.SendInvitation.Contains(obj.UID))
                                {
                                    client.Team.SendInvitation.Add(obj.UID);

                                    if ((obj as Role.Player).Owner.Team == null)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();

                                            obj.Send(stream.RelationCreate(client.Player, obj as Role.Player));

                                            stream.TeamCreate(MsgTeam.TeamTypes.InviteRequest, client.Player.UID);
                                            obj.Send(stream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (client.Team.TeamLider(client))
                    {
                        if (Now > client.Team.UpdateLeaderLocationStamp.AddSeconds(4))
                        {
                            client.Team.UpdateLeaderLocationStamp = Now;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                ActionQuery action = new ActionQuery()
                                {
                                    ObjId = client.Player.UID,
                                    dwParam = 1015,
                                    Type = ActionType.LocationTeamLieder,
                                    PositionX = client.Team.Leader.Player.X,
                                    PositionY = client.Team.Leader.Player.Y
                                };
                                client.Team.SendTeam(stream.ActionCreate(action), client.Player.UID, client.Player.Map);
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
        public static unsafe void CheckItemsTime(Client.GameClient client)
        {
            if (Program.ExitRequested)
                return;
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || client.Fake)
                    return;
                DateTime Now = DateTime.Now;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var title in client.Player.TitlesWithTime)
                    {
                        TimeSpan timeSpan = title.DateStamp - DateTime.Now;
                        if (DateTime.Now > title.DateStamp)
                        {
                            client.Player.RemoveSpecialTitleTime(title.titleID, stream);
                            var name = Enum.GetName(typeof(MsgTitleStorage.AllTitle), title.titleID);
                            if (name != null)
                                client.SendSysMesage(string.Format("Your Title [{0}] Has Expired", name), MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                    }
                    foreach (var item in client.AllMyTimeItems())
                    {
                        TimeSpan timeSpan = item.TimeStamp - DateTime.Now;
                        if (timeSpan.TotalSeconds < 0)
                            continue;
                        if (timeSpan.TotalSeconds > 0)
                        {
                            if (DateTime.Now < item.TimeStamp)
                                continue;
                        }


                        if (client.Inventory.ClientItems.ContainsKey(item.UID))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.SendSysMesage(Pool.ItemsBase[item.ITEM_ID].Name + " Time Expired.");
                            continue;
                        }
                        if (client.Equipment.ClientItems.ContainsKey(item.UID))
                        {
                            Role.Flags.ConquerItem position = (Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID);
                            client.Equipment.Remove(position, stream);
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            client.SendSysMesage(Pool.ItemsBase[item.ITEM_ID].Name + " Time Expired.");
                            continue;
                        }
                        if (client.Collection.items.ContainsKey(item.ITEM_ID))
                        {
                            client.Collection.items.Remove(item.ITEM_ID);

                            MsgCollectionStorage.UpdateSkillCollection(client, true);
                            var Info = new MsgCollectionStorage.ProtoStructure { Action = MsgCollectionStorage.Action.TakeOutExpire, ID = item.ITEM_ID };
                            client.Send(stream.CreatCollection(Info));
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            client.SendSysMesage(Pool.ItemsBase[item.ITEM_ID].Name + " Time Expired.");
                            continue;
                        }
                        MsgGameItem Item;
                        if (client.Rune.TryGetItem(item.UID, out Item))
                        {
                            client.Rune.Unequip(Item, true);
                            var Info = new MsgRuneStorage.MsgRuneStorageProto();
                            Info.Type = 1;
                            Info.ItemUID = Item.UID;
                            item.Position = (ushort)Role.Flags.ConquerItem.RuneBag;
                            Info.HPAdd = client.Rune.HPClient;
                            client.Send(stream.CreateRuneStorage(Info));
                            MsgGameItem Items;
                            if (client.Rune.TryGetItem(item.UID, out  Items) && item.Position == (ushort)Role.Flags.ConquerItem.RuneBag)
                            {
                                if (client.Rune.Remove(item.UID, false))
                                {
                                    client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Rune, item.UID, 1, 0, 0, 0, 0, 0));
                                }
                            }
                            Database.RuneRank.Update(client);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        }
                        if (client.MyWardrobe.ContainUID(item.UID))
                        {
                            MsgGameItem items2;
                            if (client.MyWardrobe.RemoveItem(item.UID, out items2))
                            {
                                if (item.IsEquip)
                                {
                                    client.Equipment.Remove((Role.Flags.ConquerItem)Database.ItemType.ItemPosition(item.ITEM_ID), stream);
                                    Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                    store.ActionID = (uint)MsgCoatStorage.Types.TakeOff;
                                    store.ID = items2.UID;
                                    store.dwpram2 = items2.ITEM_ID;
                                    client.Send(stream.CreateCoatStorage(store));
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                                client.Inventory.Update(items2, Role.Instance.AddMode.REMOVE, stream);
                                Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                astore.ActionID = (uint)MsgCoatStorage.Types.Retrive;
                                astore.ID = items2.UID;
                                astore.dwpram2 = items2.ITEM_ID;
                                client.Send(stream.CreateCoatStorage(astore));
                                client.Send(stream.CreateCoatStorage(astore));
                                client.SendSysMesage(Pool.ItemsBase[item.ITEM_ID].Name + " Time Expired.");
                            }
                        }

                        foreach (var Wh in client.Warehouse.ClientItems)
                        {
                            foreach (var item2 in Wh.Value.Values)
                            {
                                if (item2.UID == item.UID)
                                {
                                    client.Warehouse.RemoveItem(item2.UID, Wh.Key, stream);
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    client.SendSysMesage(Pool.ItemsBase[item.ITEM_ID].Name + " Time Expired.");
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
    }
}

