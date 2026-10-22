using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgNpc
{
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID>;

    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet stream, byte Option, string Input, uint id);



    public class Procesor
    {
        public static ExecuteNpcInvoker ExecuteNpc = new ExecuteNpcInvoker();
        public unsafe class InvokerClient
        {
            public Client.GameClient client;
            public byte InteractType;
            public byte option;
            public string input;
            public uint npcid;
            public InvokerClient(Client.GameClient Client, ServerSockets.Packet Server_Replay, uint _npcid, byte _InteractType, byte _option, string _input)
            {
                client = Client;

                option = _option;
                InteractType = _InteractType;
                input = _input;
                npcid = _npcid;
            }
        }

        public static ActionInvoker invoker = new ActionInvoker(NpcAttribute.Translator);

        [PacketAttribute(GamePackets.MsgNpc)]
        private unsafe static void NpcServerReplay(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor || !user.Socket.Alive)
                return;

            uint npcid;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;
            //action ==6 place!
            stream.GetNpcRequest(out npcid, out option, out type, out Action, out input);
            if (npcid != 12 && user.Player.OnAutoHunt)
                return;
            if (Action == MsgNpc.NpcServerReplay.Mode.PlaceFurniture)
            {
                if (!user.Inventory.HaveSpace(1))
                {
                    user.CreateBoxDialog("Please make 1 more space your inventory.");
                    return;
                }
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    var npc = Database.NpcServer.GetNpcFromMesh(furniture.Mesh);
                    if (npc != null)
                    {
                        user.Inventory.Add(stream, npc.ItemID);
                        user.MyHouse.Furnitures.TryRemove(npcid, out furniture);
                        Database.ItemType.DBItem item;
                        if (Pool.ItemsBase.TryGetValue(npc.ItemID, out item))
                            user.SendSysMesage("You got a " + item.Name + "!", MsgMessage.ChatMode.System);
                        var action = new ActionQuery()
                        {
                            ObjId = npcid,
                            Type = ActionType.RemoveEntity
                        };
                        user.Send(stream.ActionCreate(action));
                    }
                }
                return;
            }
            if (Action == MsgNpc.NpcServerReplay.Mode.Statue)
            {
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    user.MyHouse.Furnitures.TryRemove(npcid, out furniture);

                    var action = new ActionQuery()
                    {
                        ObjId = npcid,
                        Type = ActionType.RemoveEntity
                    };
                    user.Send(stream.ActionCreate(action));
                }
                return;
            }
            if (option == 255)
                return;
            if (UserAgreementDB.TryHandlePending(user, stream, (byte)option)) return; // [AgreementFix 2026-10-15]
            user.ActiveNpc = (uint)npcid;
            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        [PacketAttribute(GamePackets.MsgTaskDialog)]
        private unsafe static void NpcServerRequest(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor)
                return;

            uint npcid;
            ushort Mesh;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;

            stream.NpcDialog(out npcid, out Mesh, out option, out type, out Action, out input);

            if (!user.Socket.Alive && type != (byte)NpcReply.InteractTypes.MessageBox)
                return;

            if (type == (byte)NpcReply.InteractTypes.MessageBox)
            {
                if (user.Player.StartMessageBox > DateTime.Now)
                {
                    if (option == 0 && user.Player.MessageOK != null)
                        user.Player.MessageOK.Invoke(user);
                    else if (user.Player.MessageCancel != null)
                        user.Player.MessageCancel.Invoke(user);
                }
                user.Player.MessageOK = null;
                user.Player.MessageCancel = null;
                return;
            }
            if (type == 102)
            {
                if (user.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || user.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
                {
                    if (user.Player.MyGuild != null)
                    {
                        user.Player.MyGuild.Quit(input, true, stream);
                        return;
                    }
                }
            }
            if (option == 255 || option == 0 || user.InTrade)
                return;
            if (UserAgreementDB.TryHandlePending(user, stream, (byte)option)) return; // [AgreementFix 2026-10-15]
            npcid = (uint)user.ActiveNpc;

            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        public static readonly List<uint> MaintinaceNpc = new List<uint>() { /*23746, 23492 [MaintNpc 2026-10-15] scripted in Base\MaintNpc*/ 23736, 19350, 22034, 95390, 27814, 27815, 29726, 29725, 29251, 35874, /*30767 [MaintNpc 2026-10-15]*/ 28402, 30708, 28928, 35683, 28824, 21901, 20539, 25168, 21321, 23586, 20442, 18742, 29728, 22614, 22626, 30947, 25760 };
        static readonly bool _fragmentWheelOpen = global::Qahr.Mobile.FragmentWheel.FragmentWheelModule.ReleaseMaintenance(MaintinaceNpc); // [FragmentWheel 2026-10-14] 27814 Daisy / 27815 FragmentWheel scripted in Base\FragmentWheel
        public class ExecuteNpcInvoker : ConcurrentSmartThreadQueue<InvokerClient>
        {
            public ExecuteNpcInvoker()
                : base(3)
            {
                Start(10);
            }
            public void TryEnqueue(InvokerClient action)
            {
                Enqueue(action);
            }

            protected unsafe override void OnDequeue(InvokerClient action, int time)
            {
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (global::Qahr.Mobile.Phone.MobileQuestStoryNpc.OnFarClick(action.client, action.npcid, action.option)) return;// QahrMobile: main-story finish NPC clicked from far away -> walk there
                        if (global::Qahr.Mobile.Phone.MobileVipMenu.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// QahrMobile: the "@vip" dialog (MobileVipMenu.cs)
                        if (global::Qahr.Mobile.QixiKit.Hooks.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// [QixiRomance2022 2026-10-15] Qixi group NPCs (Weaver Girl / Orphee ...) + Silk/Blessing item dialogs

                        if (!action.client.Player.OnMyOwnServer)
                        {
                            if (action.npcid == (ushort)NpcID.KingdomMissionEnvoy
                                || action.npcid == (ushort)NpcID.RealmEnvoy)
                            {
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            }
                            else if (action.client.Player.Map == 3935 || action.client.Player.Map == 10137 || action.client.Player.Map == Game.MsgTournaments.MsgEliteGroup.WaitingAreaID)
                            {
                                Game.MsgNpc.Npc _obj;
                                if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                                {
                                    if (action.client.ProjectManager || action.client.Player.Name.Contains("[GM]"))
                                        action.client.SendSysMesage("ID NPC ( " + action.npcid + " ) X ( " + _obj.X + " ) Y ( " + _obj.Y + " )", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                    if (action.npcid >= (ushort)NpcID.Crystal1 && action.npcid <= (ushort)NpcID.Crystal5)
                                    {
                                        NpcHandler.KingDoomCrystals(action.client, stream, action.option, action.input, action.npcid);
                                        return;
                                    }
                                    //if (action.npcid > 30000 && action.npcid < 30010)
                                    //{
                                    //    NpcHandler.RealmNPC(action.client, stream, action.option, action.input, action.npcid);
                                    //    return;
                                    //}
                                    Tuple<NpcAttribute, ProcessAction> processFolded;
                                    if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                        processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else
                                {
                                    Role.IMapObj inpc;
                                    if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                                    {
                                        var npc = inpc as Role.SobNpc;
                                        Tuple<NpcAttribute, ProcessAction> processFolded;
                                        if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                            processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);

                                    }
                                }
                            }
                            return;
                        }
                        if (action.InteractType == (byte)NpcReply.InteractTypes.MessageBox)
                        {
                            if (action.client.Player.StartMessageBox > DateTime.Now)
                            {
                                if (action.option == 255 && action.client.Player.MessageOK != null)
                                    action.client.Player.MessageOK.Invoke(action.client);
                                else if (action.client.Player.MessageCancel != null)
                                    action.client.Player.MessageCancel.Invoke(action.client);
                            }
                            action.client.Player.MessageOK = null;
                            action.client.Player.MessageCancel = null;
                            return;
                        }
                        if (action.client.Player.IsGameMaster() || action.client.Player.Name.Contains("[GM]"))
                            action.client.SendSysMesage("ID NPC ( " + action.npcid + " )", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                        //if (action.npcid == (uint)NpcID.GoldBoxWar || action.npcid == (uint)NpcID.ScorePlayer)
                        //{
                        //    MsgGoldBoxWarNpc.GoldBoxWar(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        //if (action.npcid == (uint)NpcID.FlagsWar)
                        //{
                        //    MsgFlagsWarNpc.FlagsWar(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        //if (action.npcid == (uint)NpcID.TreasureThiefEvent)
                        //{
                        //    MsgTreasureThiefNpc.TreasureThief(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        //if (action.npcid == (uint)NpcID.CPCastleEvent)
                        //{
                        //    MsgCPCastleNpc.CPCastle(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}

                        if (global::Qahr.Mobile.FoolsKit.Kit.OnDialog(action.client, stream, action.npcid, action.option, action.input)) return;// [AprilFools2023 2026-10-15] April Fools dialogs (virtual ids 8892090-8892099) + Lyon 8892001
                        if ((uint)action.npcid == 3124)//house WH
                        {
                            if (action.client.MyHouse != null && action.client.Player.DynamicID == action.client.Player.UID)
                            {
                                ActionQuery query = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = action.client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.Warehouse,
                                    PositionX = action.client.Player.X,
                                    PositionY = action.client.Player.Y
                                };
                                action.client.Send(stream.ActionCreate(query));

                                return;
                            }
                            else
                            {
#if Arabic
                                 action.client.SendSysMesage("I'm sorry but you dont own this house !");
#else
                                action.client.SendSysMesage("I'm sorry but you dont own this house !");
#endif

                            }
                        }
                        // [QuestIntegration] a maintenance NPC that begins/finishes engine quests (e.g. 23492 UniversalBankManager,
                        // ER-12) goes on to the engine dialog below; its hand-written script stays closed (see after OnNpc).
                        if (MaintinaceNpc.Contains(action.npcid) && !global::Qahr.Mobile.QuestEngine.QuestEngineCore.HasEngineQuests(action.npcid))
                        {
                            action.client.CreateBoxDialog("Under maintenance, come back later");
                            return;
                        }
                        else if (action.npcid == 27950)
                        {
                            ActionQuery info = new ActionQuery()
                            {
                                ObjId = action.client.Player.UID,
                                dwParam = 3653,
                                Timestamp = 250365859,
                                NpcID = 27950,
                                Type = ActionType.OpenDialog,
                                Fascing = 7,
                                PositionX = action.client.Player.X,
                                PositionY = action.client.Player.Y,
                            };
                            action.client.Send(stream.ActionCreate(info));
                            return;
                        }
                        else if (action.npcid == 23561)
                        {
                            ActionQuery info = new ActionQuery()
                            {
                                ObjId = action.client.Player.UID,
                                dwParam = 3653,
                                Timestamp = 250365859,
                                NpcID = 23561,
                                Type = ActionType.OpenDialog,
                                Fascing = 7,
                                PositionX = action.client.Player.X,
                                PositionY = action.client.Player.Y,
                            };
                            action.client.Send(stream.ActionCreate(info));
                            return;
                        }
                        else if (action.npcid == 22372)
                        {
                            ActionQuery info = new ActionQuery()
                            {
                                ObjId = action.client.Player.UID,
                                dwParam = 3824,
                                Timestamp = 250365859,
                                NpcID = 22372,
                                Type = ActionType.OpenDialog,
                                Fascing = 7,
                                PositionX = action.client.Player.X,
                                PositionY = action.client.Player.Y,
                            };
                            action.client.Send(stream.ActionCreate(info));
                            return;
                        }
                        else if (action.npcid == 23127)
                        {
                            ActionQuery info = new ActionQuery()
                            {
                                ObjId = action.client.Player.UID,
                                dwParam = 3653,
                                Timestamp = 250365859,
                                NpcID = 23127,
                                Type = ActionType.OpenDialog,
                                Fascing = 7,
                                PositionX = action.client.Player.X,
                                PositionY = action.client.Player.Y,
                            };
                            action.client.Send(stream.ActionCreate(info));
                            return;
                        }
                        if (action.client.Player.Map == 1038)//Guild War
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }

                        }
                        if (action.client.Player.Map == 1357)//Guild War
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }

                        }
                        if (action.npcid >= 7832 && action.npcid <= 7840)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.CheckDesertGuardian(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8546 && action.npcid <= 8550)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.SoldierBird(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8551 && action.npcid <= 8555)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.BandittiFlowers(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid == (uint)NpcID.SelectSacredRefineryPack || action.npcid == (uint)NpcID.SelectP7WeaponSoulPack
                            || action.npcid == (uint)NpcID.SelectP7EquipmentSoulPack
                            || action.npcid == (uint)NpcID.Steed1
                            || action.npcid == (uint)NpcID.Steed3
                            || action.npcid == (uint)NpcID.Steed6 || action.npcid == (uint)NpcID.DailyItem1
                            || action.npcid == (uint)NpcID.DailyEliteSpiritBead
                            || action.npcid == (uint)NpcID.GoldPrizeToken
                            || action.npcid == (uint)NpcID.DailyNormalSpiritBead
                            || action.npcid == (uint)NpcID.DailyRefinedSpiritBead
                            || action.npcid == (uint)NpcID.DailyUniqueSpiritBead
                            || action.npcid == (uint)NpcID.DailySuperSpiritBead
                            || action.npcid == (uint)NpcID.Level43UniqueRingPack
                            || action.npcid == (uint)NpcID.NobleSteedPack
                            || action.npcid == (uint)NpcID.DazzlingDiamondBox
                            || action.npcid == (uint)NpcID.RareSteedPack6
                            || action.npcid == (uint)NpcID.TempestSecretLetter
                            || action.npcid == (uint)NpcID.SashFragment_Realm
                            || action.npcid == (uint)NpcID.GarmentPacket
                            || action.npcid == (uint)NpcID.GarmentPacket2
                            || action.npcid == (uint)NpcID.MountPacket
                            || action.npcid == (uint)NpcID.MountPacket2
                                     || action.npcid == (uint)NpcID.AccesoryPacket
                              || action.npcid == (uint)NpcID.AccesoryPacket2
                               || action.npcid == (uint)NpcID.MountPacket3
                            || action.npcid == (uint)NpcID.GoldPrizeToken
                            || action.npcid == (uint)NpcID.BlackFridayGarmentPack
                            || action.npcid == (uint)NpcID.BlackFridayMountPack || action.npcid == (uint)NpcID.BlackFridayAccesory
                            || action.npcid == (uint)NpcID.Steed1Pack
                            || action.npcid == (uint)NpcID.Steed3Pack
                              || action.npcid == (uint)NpcID.ChargeChi
                              || action.npcid == (uint)NpcID.Relic
                              || action.npcid == (uint)NpcID.ChangeToken
                                || action.npcid == (uint)NpcID.ChargeJiang
                              || action.npcid == (uint)NpcID.HeavenDemonBox
                              || action.npcid == (uint)NpcID.ChaosDemonBox
                               || action.npcid == (uint)NpcID.TaiChiDemonBox
                              || action.npcid == (uint)NpcID.SacredDemonBox
                              || action.npcid == (uint)NpcID.AuroraDemonBox
                              || action.npcid == (uint)NpcID.DemonBox
                              || action.npcid == (uint)NpcID.AncientDemonBox
                            || action.npcid == (uint)NpcID.FloodDemonBox
                           || action.npcid == (uint)NpcID.MrMirror2

                             || action.npcid == (uint)NpcID.EpicArcher
                                               || action.npcid == (uint)NpcID.LuxuryRelicChest
                                                     || action.npcid == (uint)NpcID.RuneStarStonePack
                                                                    || action.npcid == (uint)NpcID.RareSteedPack
                                                                                        || action.npcid == (uint)NpcID.RareSteedPack8
                                   || action.npcid == (uint)NpcID.LilyCard
                                    || action.npcid == (uint)NpcID.PowerBoosterPack
                                     || action.npcid == (uint)NpcID.MysteryFruit
                                      || action.npcid == (uint)NpcID.ThunderGloryGem
                             || action.npcid == (uint)NpcID.DelicateFoxEars
                                  || action.npcid == (uint)NpcID.LoveHat
                             || action.npcid == (uint)NpcID.TomatoUmbrella
                             || action.npcid == (uint)NpcID.HotTurkey
                             || action.npcid == (uint)NpcID.OceanSecret
                             || action.npcid == (uint)NpcID.PhoenixLegend
                             || action.npcid == (uint)NpcID.GraspofLove
                             || action.npcid == (uint)NpcID.EpicPirate

                             || action.npcid == (uint)NpcID.OneStarTreasureMap
                              || action.npcid == (uint)NpcID.TwoStarTreasureMap
                               || action.npcid == (uint)NpcID.ThreeStarTreasureMap
                                || action.npcid == (uint)NpcID.FourStarTreasureMap
                                || action.npcid == (uint)NpcID.EpicNinja
                                   || action.npcid == (uint)3004464
                              || action.npcid == (uint)NpcID.EpicMonk
                               || action.npcid == (uint)NpcID.EpicTrojan
                            || action.npcid == (uint)NpcID.SuperHeadgearPack || action.npcid == (uint)NpcID.RingPack
                            || action.npcid == (uint)NpcID.ClothingPack || action.npcid == (uint)NpcID.PowerBook
                            || action.npcid == (uint)NpcID.Level50UniqueWeaponPack
                            || action.npcid == (uint)NpcID.Level52UniqueHeadgearPack
                            || action.npcid == (uint)NpcID.Level55EliteWeaponPack
                            || action.npcid == (uint)NpcID.Level67EliteHeadgearPack
                            || action.npcid == (uint)NpcID.L60UniqueGearPack)
                        {
                            if (Pool.ItemsBase.ContainsKey(action.npcid))
                            {
                                if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                                {
                                    action.client.SendSysMesage("You Don't have this item in Inventory");
                                    return;

                                }
                            }
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314674)
                        {
                            NpcHandler.ItemArena(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314675)
                        {
                            NpcHandler.ItemArenaTwo(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        #region New Item Lottery
                        else if (action.npcid == 3325755)
                        {
                            NpcHandler.L1MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3325993)
                        {
                            NpcHandler.L2MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3325994)
                        {
                            NpcHandler.L3MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3325995)
                        {
                            NpcHandler.L4MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3326004 || action.npcid == 3335932)
                        {
                            NpcHandler.L5MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3326005 || action.npcid == 3335933)
                        {
                            NpcHandler.L6MythsoulPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329302)
                        {
                            NpcHandler.ParasolScrap(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329303)
                        {
                            NpcHandler.FancyParasol(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329389)
                        {
                            NpcHandler.PrimaryAwakening(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329390)
                        {
                            NpcHandler.MediumAwakening(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329391)
                        {
                            NpcHandler.AdvancedAwakening(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3329392)
                        {
                            NpcHandler.UltimateAwakening(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3324521)
                        {
                            NpcHandler.TiangongKaiwu(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3319466)
                        {
                            NpcHandler.RuneSystem(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3307212)
                        {
                            NpcHandler.YellowRuneSelectionPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3306560 || action.npcid == 3306562)
                        {
                            NpcHandler.RedRuneSelectionPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3311899)
                        {
                            NpcHandler.RedRuneSlayer(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3311906)
                        {
                            NpcHandler.YellowRuneSlayer(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3311900)
                        {
                            NpcHandler.BlueRuneSlayer(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3306885)
                        {
                            NpcHandler.RelicCrystal(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316725)
                        {
                            NpcHandler.FireFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316752)
                        {
                            NpcHandler.BigPermanentSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316753)
                        {
                            NpcHandler.PeachGearSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316754)
                        {
                            NpcHandler.SimpleStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316755)
                        {
                            NpcHandler.ClearSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316756)
                        {
                            NpcHandler.SimpleSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316757)
                        {
                            NpcHandler.BloodlineSigilSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        //else if (action.npcid == 3316736)
                        //{
                        //    NpcHandler.RareYellowRuneBox(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        else if (action.npcid == 3316748)
                        {
                            NpcHandler.ChasteStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316749)
                        {
                            NpcHandler.AdvancedJadeSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316750)
                        {
                            NpcHandler.ForgingStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331666)
                        {
                            NpcHandler.MysteryStarStonePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316751)
                        {
                            NpcHandler.PureSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316741)
                        {
                            NpcHandler.P3FateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331658)
                        {
                            NpcHandler.P3BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316735)
                        {
                            NpcHandler.DragonBallScrollSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316740)
                        {
                            NpcHandler.JadeStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316739)
                        {
                            NpcHandler.FightingSoulSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316738)
                        {
                            NpcHandler.FateSoulSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316737)
                        {
                            NpcHandler.RareBlueRuneBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316746)
                        {
                            NpcHandler.GreatRichSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316732)
                        {
                            NpcHandler.P4FateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331656)
                        {
                            NpcHandler.P5BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314242)
                        {
                            NpcHandler.RedRuneBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314248)
                        {
                            NpcHandler.YellowRunePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314249)
                        {
                            MsgServer.MsgGameItem item;
                            if (action.client.Inventory.TryGetItem(action.client.ActiveNpcBound, out item))
                            {
                                NpcHandler.BlueRunePack(action.client, stream, action.option, item.ITEM_ID.ToString(), action.client.ActiveNpcBound);
                            }
                            else
                            {
                                NpcHandler.BlueRunePack(action.client, stream, action.option, action.input, action.npcid);
                            }
                            return;
                        }
                        else if (action.npcid == 3331659)
                        {
                            NpcHandler.WaterSigilOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331660)
                        {
                            NpcHandler.FireSigilOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316716)
                        {
                            NpcHandler.DragonBallSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316703)
                        {
                            NpcHandler.FateRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316702)
                        {
                            NpcHandler.FateWealthSelectionBox2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316701)
                        {
                            NpcHandler.FateStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316700)
                        {
                            NpcHandler.RuneStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316699)
                        {
                            NpcHandler.FateScrapSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331653)
                        {
                            NpcHandler.P8BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316698)
                        {
                            NpcHandler.P8PeachSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316709)
                        {
                            NpcHandler.FatePotencySelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316708)
                        {
                            NpcHandler.ValueStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316707)
                        {
                            NpcHandler.FateWealthSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316706)
                        {
                            NpcHandler.RelicSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316705)
                        {
                            NpcHandler.RuneSigilSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331654)
                        {
                            NpcHandler.P7BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316704)
                        {
                            NpcHandler.P7FateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316719)
                        {
                            NpcHandler.HonorStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316718)
                        {
                            NpcHandler.YellowRuneSelectionBox2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316717)
                        {
                            NpcHandler.Class5FortuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331661)
                        {
                            NpcHandler.WindSigilOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316715)
                        {
                            NpcHandler.TortoiseGemSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316714)
                        {
                            NpcHandler.ThunderGemSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316713)
                        {
                            NpcHandler.GloryGemSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316720)
                        {
                            NpcHandler.FateSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331664)
                        {
                            NpcHandler.ArcaneRunePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316712)
                        {
                            NpcHandler.ArcaneRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331669)
                        {
                            NpcHandler.LegendarySpiritStonePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316711)
                        {
                            NpcHandler.SoldierSoulSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331655)
                        {
                            NpcHandler.P6BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316710)
                        {
                            NpcHandler.P6FateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314244)
                        {
                            NpcHandler.BlueRuneBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3311747)
                        {
                            NpcHandler.OptionalRareBlueRunePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316731)
                        {
                            NpcHandler.BlueRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316730)
                        {
                            NpcHandler.YellowRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316727)
                        {
                            NpcHandler.FateEraserSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316729)
                        {
                            NpcHandler.RedRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316728)
                        {
                            NpcHandler.FateSigilSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316726)
                        {
                            NpcHandler.WindFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316724)
                        {
                            NpcHandler.WaterFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316723)
                        {
                            NpcHandler.PureStoneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316722)
                        {
                            NpcHandler.RuneFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316721)
                        {
                            NpcHandler.P5FateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331663)
                        {
                            NpcHandler.LightningSigilOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316734)
                        {
                            NpcHandler.LightningFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331662)
                        {
                            NpcHandler.EarthSigilOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316733)
                        {
                            NpcHandler.EarthFateSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331657)
                        {
                            NpcHandler.P4BlissPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316742)
                        {
                            NpcHandler.HonorSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316743)
                        {
                            NpcHandler.PermanentOrchidSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3331665)
                        {
                            NpcHandler.PermanentOrchidPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316744)
                        {
                            NpcHandler.ValueSteedSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316745)
                        {
                            NpcHandler.RareBlueRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3316747)
                        {
                            NpcHandler.RareYellowRuneSelectionBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        #endregion
                        else if (action.npcid >= 3330028 && action.npcid <= 3330062)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.SigilPacks(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }

                        else if (action.npcid == 3321817)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }                            
                        }
                        else if (action.npcid == 3321818)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }                           
                        }
                        else if (action.npcid == 3321819)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }                           
                        }
                        else if (action.npcid == 3321820)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }                           
                        }

                        else if (action.npcid == 3306560 || action.npcid == 3306562 || action.npcid == 3314242)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.RedRuneSelectionPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314245)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.RareYellowRuneBoxS(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314246)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.RareBlueRuneBoxS(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3314243)
                        {
                            NpcHandler.YellowRuneBox(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        //else if (action.npcid == 3316793)
                        //{
                        //    NpcHandler.PeachJade(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        else if (action.npcid == 3311746)
                        {
                            NpcHandler.OptionalRareYellowRunePack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        else if (action.npcid == 3307212 || action.npcid == 3314248)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.YellowRuneSelectionPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }

                        else if (action.npcid == 3306561)
                        {
                            NpcHandler.BlueRuneSelectionPack2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }

                        else if (action.npcid == 3306563 || action.npcid == 3314244)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.BlueRuneSelectionPack(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        //else if (action.npcid == 3307142)
                        //{
                        //    if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1,1))
                        //    {
                        //        action.client.SendSysMesage("You Don't have this item in Inventory");
                        //        return;

                        //    }
                        //    else
                        //        //NpcHandler.RelicSpirit(action.client, stream, action.option, action.input, action.npcid);
                        //    return;
                        //}
                        else if (action.npcid == 3306885)
                        {
                            if (!action.client.Inventory.Contain((uint)action.npcid, 1) && !action.client.Inventory.Contain((uint)action.npcid, 1, 1))
                            {
                                action.client.SendSysMesage("You Don't have this item in Inventory");
                                return;

                            }
                            else
                                NpcHandler.RelicCrystal(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }


                        if (global::Qahr.Mobile.SummerWhales.SummerWhalesWish.OnItemDialog(action.client, stream, action.npcid, action.option, action.input)) return;// [SummerWhales 2026-10-15] Ambergris / Cool Surfboard / Whale Spirit item dialogs (npcid = item id)
                        if (global::Qahr.Mobile.NewYear2023.SpringHooks.OnItemDialog(action.client, stream, action.npcid, action.option, action.input)) return;// [NewYear2023 2026-10-15] Spring Festival group choose-one boxes (npcid = item id)
                        if (global::Qahr.Mobile.CastleMystery.CastleStartle.OnItemDialog(action.client, stream, action.npcid, action.option, action.input)) return;// [CastleMystery 2026-10-15] Valentine group item dialogs: Castle Key / key boxes / Romantic boxes / Dream Note (npcid = item id)
                        if (global::Qahr.Mobile.DreamTheater.DreamTheaterModule.OnItemDialog(action.client, stream, action.npcid, action.option, action.input)) return;// [DreamTheater 2026-10-15] Love Velvet 3347321 item dialog (npcid = item id)
                        Game.MsgNpc.Npc obj;
                        // [InstanceNpc] DynamicID-aware: an NPC inside a private copy answers only the players in that copy
                        if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, action.client.Player.DynamicID, out obj))
                        {
                            if (action.client.ProjectManager || action.client.Player.Name.Contains("[GM]"))
                                action.client.SendSysMesage("ID NPC ( " + action.npcid + " ) X ( " + obj.X + " ) Y ( " + obj.Y + " )", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            if (action.client.Player.Map == 1026 || action.client.Player.Map == 1027 || action.client.Player.Map == 1028)
                            {
                                uint[] Items = new uint[] { 3324985, 3324993 };
                                uint ItemID = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                                if (Role.Core.Rate(50))
                                    action.client.Inventory.Add(stream, ItemID);
                                var Map = Pool.ServerMaps[action.client.Player.Map];
                                action.client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, "accession2");
                                Map.RemoveNpc(obj, stream);
                                action.client.Teleport(410, 354, 1002);
                            }
                            if (action.client.Player.Map == 3030)
                            {
                                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TreasureThief)
                                {
                                    var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTreasureThief;
                                    tournament.Reward(action.client, obj, stream);
                                    return;
                                }
                            }
                            if (action.client.Player.Map == 3826 || action.client.Player.Map == 3827
                                || action.client.Player.Map == 3828 || action.client.Player.Map == 3829)
                            {
                                if (action.npcid >= 10584 && action.npcid <= 10588)
                                    NpcHandler.Epic2MonkMisery(action.client, stream, action.option, action.input, action.npcid);
                                if (action.npcid >= 10589 && action.npcid <= 10598)
                                    NpcHandler.ArraysEye(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }
                            //if (action.client.Player.Map == 1511)
                            //{
                            //    NpcHandler.Furnitures(action.client, stream, action.option, action.input, action.npcid);
                            //    return;
                            //}
                            // [QuestEngine] begin/finish NPCs of engine quests (and "talk to X" steps): generic accept /
                            // progress / finish dialog with a "Something else." option back to the hand-written script.
                            // Hand-coded MissionIds are never shown here. The NPC is in view (checked just above).
                            // [Festivals] NPCs of a running festival: engine quests first, then its exchanges / greeting.
                            if (global::Qahr.Mobile.HalloweenSenluo2023.HsKit.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// [HalloweenSenluo2023 2026-10-15] Macabre Realm NPCs (Count Dracula, zone/hall copies)
                            if (global::Qahr.Mobile.SummerWhales.SummerWhalesUi.OnNpc(action.client, stream, action.npcid, action.option, action.input))
                                return;// [SummerWhales 2026-10-14] Scallop NPC gives the official Scallop 3336088 (quest 5113)
                            if (global::Qahr.Mobile.DreamTheater.DreamTheaterModule.OnNpc(action.client, stream, action.npcid, action.option, action.input))
                                return;// [DreamTheater] theater management + click mini-games (Base\DreamTheater)
                            if (global::Qahr.Mobile.ChildrensDay2025.DreamParkNpcs.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// [ChildrensDay2025 2026-10-15] Suyi / Prince Puff stores, Flora + Starlight Blessings (8866001-8866013)
                            if (global::Qahr.Mobile.NewYear2023.SpringHooks.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// [NewYear2023 2026-10-15] Spring Festival group daily NPCs (prayer statues / firework tubes 8840013-8840025, ...)
                            if (global::Qahr.Mobile.CastleMystery.CastleStartle.OnNpc(action.client, stream, action.npcid, action.option, action.input)) return;// [CastleMystery 2026-10-15] Suyi / Lord Anthony in Twin City (8871011, 8871013)
                            if (global::Qahr.Mobile.Festivals.FestivalManager.OnNpc(action.client, stream, action.npcid, action.option, action.input))
                                return;
                            if (global::Qahr.Mobile.QuestEngine.QuestEngineCore.OnNpc(action.client, stream, action.npcid, action.option, action.input))
                                return;
                            if (NpcHandler.MobileVideoNpc(action.client, stream, action.npcid, action.option, action.input)) return;// QahrMobile: NPCs from the official mobile video (9600001-9600016, 5600 in TC, 9072 in Market), see NpcHandler.MobileVideo.cs
                            if (NpcHandler.MobileCityNpc(action.client, stream, action.npcid, action.option, action.input)) return;// QahrMobile: city content (ExtremePKManager, slot machines), see NpcHandler.MobileCity.cs
                            if (NpcHandler.MobileQuestNpc(action.client, stream, action.npcid, action.option, action.input)) return;// QahrMobile: mobile quest NPC fallback dialog, see NpcHandler.MobileQuests.cs
                            if (MaintinaceNpc.Contains(action.npcid))// [QuestIntegration] nothing for this player from the engine
                            {
                                action.client.CreateBoxDialog("Under maintenance, come back later");
                                return;
                            }
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            else
                            {

                                if (action.npcid == (uint)NpcID.GuildConductor1 || action.npcid == (uint)NpcID.GuildConductor2
                                        || action.npcid == (uint)NpcID.GuildConductor3 || action.npcid == (uint)NpcID.GuildConductor4)
                                    NpcHandler.GuildConductorsProces(action.client, stream, action.option, action.input, action.npcid);
                                else if (((int)action.npcid >= 10031 && (int)action.npcid <= 10041 || (int)action.npcid == 10043) && action.client.Player.DynamicID == 0)
                                {
                                    NpcHandler.SpaceMarks(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.TeleGuild1 || action.npcid == (uint)NpcID.TeleGuild2
                                   || action.npcid == (uint)NpcID.TeleGuild3 || action.npcid == (uint)NpcID.TeleGuild4)
                                {
                                    NpcHandler.GuildCondTeleBack(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.WHTwin || action.npcid == (uint)NpcID.wHPheonix
                                   || action.npcid == (uint)NpcID.WHMarket || action.npcid == (uint)NpcID.WHBird
                                   || action.npcid == (uint)NpcID.WHDesert || action.npcid == (uint)NpcID.WHApe
                                   || action.npcid == (uint)NpcID.WHPoker || action.npcid == (uint)NpcID.WHPokerTwin)
                                {
                                    NpcHandler.Warehause(action.client, stream, action.option, action.input, action.npcid);
                                }


                                else if (((int)action.npcid >= 22598 && (int)action.npcid <= 22605) || ((int)action.npcid >= 21973 && (int)action.npcid <= 21976))
                                {
                                    NpcHandler.RareDeitylandChests(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (((int)action.npcid >= 22607 && (int)action.npcid <= 22609) || ((int)action.npcid == 21974))
                                {
                                    NpcHandler.EpicDeitylandChests(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 21972 || (int)action.npcid == 22577)
                                {
                                    NpcHandler.VictoryChest(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 22560 || (int)action.npcid == 22561)
                                {
                                    NpcHandler.DeityAltar(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid >= 20861 && (int)action.npcid <= 20865)
                                {
                                    NpcHandler.DeitylandPortals(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 21875 || (int)action.npcid == 22575)
                                {
                                    NpcHandler.HauKan(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 22568 || (int)action.npcid == 22578)
                                {
                                    NpcHandler.Nezha(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 22569 || (int)action.npcid == 22579)
                                {
                                    NpcHandler.SaintTaiyi(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 20825 || (int)action.npcid == 22574)
                                {
                                    NpcHandler.DeityCobbWind(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if ((int)action.npcid == 20824 || (int)action.npcid == 22573)
                                {
                                    NpcHandler.YuKoon(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else
                                {
                                    if (action.client.ProjectManager)
                                        MyConsole.WriteLine("Didn't find Npc -> " + action.npcid);
                                }
                            }
                        }
                        else if (action.npcid == 12)
                        {
                            if (action.client.Player.VipLevel > 0)
                            {
                                if (action.client.Player.HitPoints == 0) { action.client.SendSysMesage("You are dead.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true); return; };
                                ActionQuery query = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = action.client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.VIPWarehouse,
                                    PositionX = action.client.Player.X,
                                    PositionY = action.client.Player.Y
                                };
                                action.client.Send(stream.ActionCreate(query));



                            }
                        }
                        else
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);

                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
        }
    }
}
