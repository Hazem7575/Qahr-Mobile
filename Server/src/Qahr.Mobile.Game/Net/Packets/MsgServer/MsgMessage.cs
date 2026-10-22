using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.Game.MsgNpc;
using System.IO;
using ProtoBuf;
using Qahr.Mobile.Game.MsgTournaments;
using Qahr.Mobile.Role.Instance;
using Qahr.Mobile.Role;
using System.Xml.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Qahr.Mobile.Game.MsgServer
{
    public class MsgMessage
    {
        public MsgTalkProto proto;
        [ProtoContract]
        public class MsgTalkProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Color;
            [ProtoMember(2, IsRequired = true)]
            public uint ChatType;
            [ProtoMember(3)]
            public bool Run;
            [ProtoMember(4, IsRequired = true)]
            public uint MessageUID2;
            [ProtoMember(5, IsRequired = true)]
            public uint ServerID;
            [ProtoMember(6, IsRequired = true)]
            public uint Mesh;
            [ProtoMember(7, IsRequired = true)]
            public uint UnionType;
            [ProtoMember(8, IsRequired = true)]
            public uint Count;
            [ProtoMember(9)]
            public uint Winning;
            [ProtoMember(10, IsRequired = true)]
            public uint New2;

            [ProtoMember(13, IsRequired = true)]
            public uint UIDTarget;//3570583

            [ProtoMember(14, IsRequired = true)]
            public string[] Content;

            [ProtoMember(16, IsRequired = true)]
            public long unk;//0

            [ProtoMember(17, IsRequired = true)]
            public byte unk2;//1

            [ProtoMember(18, IsRequired = true)]
            public long unk3;//0

            [ProtoMember(19, IsRequired = true)]
            public long BetLevel;//11

            [ProtoMember(20, IsRequired = true)]
            public ushort FrameID;//25

            // [ProtoFix 2026-10-14] client CMsgTalkPB has no #21 (goes 20 -> 22): not serialized any more; field kept.
            public long Member21;//1 // was [ProtoMember(21, IsRequired = true)] // [ProtoFix 2026-10-14]
        }
        public enum MsgColor : uint
        {
            black = 0x000000,
            blue = 0x0000ff,
            orange = 0xffa500,

            white = 0xffffff,
            whitesmoke = 0xf5f5f5,
            yellow = 0xffff00,
            yellowgreen = 0x9acd32,
            violet = 0xee82ee,
            purple = 0x800080,
            red = 0xff0000,
            pink = 0xffc0cb,
            lightyellow = 0xffffe0,
            cyan = 0x00ffff,
            blueviolet = 0x8a2be2,
            antiquewhite = 0xfaebd7,
        }
        public enum ChatMode : uint
        {
            Talk = 2000,
            Whisper = 2001,
            Team = 2003,
            Guild = 2004,
            Melting = 2007,
            TopLeftSystem = 2005,
            Clan = 2006,
            System = 2000,
            Friend = 2009,
            Center = 2011,
            TopLeft = 2012,
            Service = 2014,
            Tip = 2015,
            CrossServerIcon = 2016,
            Ally = 2025,
            WebSite = 2105,
            World = 2021,
            Qualifier = 2022,
            Study = 2024,
            JianHu = 2026,
            InnerPower = 2027,
            PopUP = 2100,
            Dialog = 2101,
            CrosTheServer2 = 2400,
            SlideCrosTheServer = 2401,
            CrosTheServer = 2402,
            FirstRightCorner = 2108,
            ContinueRightCorner = 2109,
            SystemWhisper = 2110,
            GuildAnnouncement = 2111,
            Winning = 2032,
            Agate = 2115,
            BroadcastMessage = 2500,
            Monster = 2600,
            SlideFromRight = 100000,
            HawkMessage = 2104,
            SlideFromRightRedVib = 1000000,
            WhiteVibrate = 10000000
        }

        public string _From;
        public string _To;
        public ChatMode ChatType;
        public uint Color;
        public string __Message;
        public string ServerName = string.Empty;

        public uint Mesh;
        public uint New1;
        public uint New2;
        public uint MessageUID1 = 0;
        public uint MessageUID2 = 0;
        public bool Run;
        public uint winning;
        public uint UID;
        public ushort FrameID;
        public MsgMessage(string _Message, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning, uint d)
        {
            this.Mesh = 0;
            this.__Message = "STR_Texas_Money@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning, uint d, bool cps = true)
        {
            this.Mesh = 0;
            this.__Message = "STR_Texas_EMoney@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning, bool Money = true)
        {
            this.Mesh = 0;
            this.__Message = "STR_Newslot_Money@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning, bool Bandit = true, bool CPs = true)
        {
            this.Mesh = 0;
            this.__Message = "STR_Newslot_EMoney@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage(string _Message, string __To, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = "SYSTEM";
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }

        public MsgMessage(string _Message, string __To, string __From, MsgColor _Color, ChatMode _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = (uint)_Color;
            this.ChatType = _ChatType;
        }

        public MsgMessage(MsgColor color, ChatMode chat, uint uid1, uint uid2, uint new1, uint new2, string from, string to, string message)
        {
            this.Mesh = 0;
            this.Color = (uint)color;
            this.ChatType = chat;
            this.MessageUID1 = uid1;
            this.MessageUID2 = uid2;
            this._From = from;
            this._To = to;
            this.__Message = message;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning)
        {
            this.Mesh = 0;
            this.__Message = "STR_Dragon_Soul@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }

        public MsgMessage(string Name, string title, uint winning, uint d)
        {
            this.Mesh = 0;
            this.__Message = "STR_YUANSHEN_LOTTERY_BROCAST_YUANSHEN_1@@";
            this.__Message += Name + "@@";
            this.__Message += title + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage(string ServerName, string Name, string Code, uint winning, uint d, string Test)
        {
            this.Mesh = 0;
            this.__Message = Test + "@@";
            this.__Message += ServerName + "@@";
            this.__Message += Name + "@@";
            this.__Message += Code + "@@";
            this._To = "ALLUSERS";
            this._From = "SYSTEM";
            this.Color = (uint)System.Drawing.Color.FromArgb(16777215).ToArgb();
            this.Run = true;
            this.winning = winning;
            this.ChatType = ChatMode.Winning;
        }
        public MsgMessage()
        {
            this.Mesh = 0;
        }
        public unsafe void Deserialize(ServerSockets.Packet stream)
        {
            proto = new MsgTalkProto();
            proto = stream.ProtoBufferDeserialize<MsgTalkProto>(proto);
            Color = proto.Color;
            ChatType = (ChatMode)proto.ChatType;
            MessageUID2 = proto.MessageUID2;
            New2 = proto.New2;
            Mesh = proto.Mesh;
            FrameID = proto.FrameID;
            if (proto.Content.Length > 0)
                _From = proto.Content[0];
            if (proto.Content.Length > 1)
                _To = proto.Content[1];
            if (proto.Content.Length > 3)
                __Message = proto.Content[3];
            if (proto.Content.Length > 6)
                ServerName = proto.Content[6];
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, uint Rank = 0)
        {
            stream.InitWriter();
            proto = new MsgTalkProto()
            {
                Color = this.Color,
                ChatType = (uint)this.ChatType,
                MessageUID2 = MessageUID2,
                Mesh = Mesh,
                UnionType = Rank,
                Count = 1,
                New2 = New2,
                Run = this.Run,
                Winning = this.winning,
                FrameID = FrameID,
                UIDTarget = UID,
                Member21 = 1,
            };
            if (ServerName != "")
            {
                uint id = 0;
                foreach (var s in Database.GroupServerList.GroupServers.Values)
                {
                    if (s.Name == ServerName)
                    {
                        proto.ServerID = id;
                        break;
                    }
                }
            }
            proto.Content = new string[7] { _From, _To, string.Empty, __Message, string.Empty, string.Empty, ServerName };
            stream.ProtoBufferSerialize(proto);
            stream.Finalize(GamePackets.MsgTalk);
            return stream;

        }

        [PacketAttribute(GamePackets.MsgTalk)]
        public unsafe static void MsgHandler(Client.GameClient client, ServerSockets.Packet packet)
        {
            string str3;
            int num3;
            MsgMessage msg = new MsgMessage();
            msg.Deserialize(packet);
            // اسم المرسِل لازم يكون اسم اللاعب الحقيقي (كان ينفع يتزوّر SYSTEM أو اسم GM)
            msg._From = client.Player.Name;
            if (client.Player.IsStillBanned)
            {
                if (client.Player.PermenantBannedChat)
                {
                    client.SendSysMesage("Sorry, you still banned from chatting Permenatly.", ChatMode.System, MsgColor.white);
                }
                else
                {
                    client.SendSysMesage("Sorry, you still banned from chatting till " + client.Player.BannedChatStamp.ToString(), ChatMode.System, MsgColor.white);
                }
                return;
            }
            if (ChatCommands(client, msg))
                return;

            if (msg.__Message.StartsWith("@"))
                return;

            try
            {
                string[] lines = msg.__Message.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);

                for (int x = 0; x < lines.Length; x++)
                {
                    string str = lines[x];
                    if (str.Contains("Item "))
                    {
                        string[] line = str.Split(' ');
                        if (line != null && line.Length > 2)
                        {
                            uint UID = 0;
                            if (uint.TryParse(line[2], out UID))
                            {
                                MsgGameItem msg_item;
                                if (client.TryGetItem(UID, out msg_item))
                                {
                                    Pool.ChatItems.Add(msg_item.UID, msg_item);
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
            if (!client.Player.Name.Contains("[GM]"))
            {
                if ((msg.__Message.Contains("http:") || msg.__Message.Contains("www.")) || msg.__Message.Contains(".com"))
                {
                    string[] strArray3 = msg.__Message.Split(new string[] { " " }, StringSplitOptions.None);
                    num3 = 0;
                    while (num3 < strArray3.Length)
                    {
                        string oldValue = strArray3[num3];
                        if ((((oldValue.EndsWith(".com") || oldValue.EndsWith(".net")) || oldValue.StartsWith("www.")) || oldValue.Contains(".com")) && !((oldValue.Contains("Lucky") || oldValue.Contains("mediafire")) || oldValue.Contains("gulfup")))
                        {
                            msg.__Message = msg.__Message.Replace(oldValue, "*****.com");
                        }
                        num3++;
                    }
                    client.CreateBoxDialog("You Can't Put Sites Here .");
                }
            }



            msg.Mesh = client.Player.Mesh;

            switch (msg.ChatType)
            {
                case ChatMode.CrosTheServer:
                    {
                        if (client.Inventory.Contain(3002218, 1))
                        {
                            packet.Seek(packet.Size - 8);
                            packet.Finalize(Game.GamePackets.MsgTalk);
                            if (client.Player.InUnion)
                                MsgInterServer.StaticConnexion.Send(packet);
                            else
                                MsgInterServer.StaticConnexion.Send(packet);

                            client.Inventory.Remove(3002218, 1, packet);
                        }
                        break;
                    }
                case ChatMode.Ally:
                    {
                        if (client.Player.MyGuild != null)
                        {
                            foreach (var guild in client.Player.MyGuild.Ally.Values)
                                guild.SendPacket(msg.GetArray(packet));
                        }
                        break;
                    }
                case ChatMode.HawkMessage:
                    {
                        if (client.IsVendor)
                        {
                            client.MyVendor.HalkMeesaje = msg;

                            client.Player.View.SendView(msg.GetArray(packet), true);
                        }
                        break;
                    }
                case ChatMode.Team:
                    {
                        if (client.Team != null)
                            client.Team.SendTeam(msg.GetArray(packet), client.Player.UID);
                        break;
                    }
                case MsgMessage.ChatMode.Talk:
                    {
                        client.Player.View.SendView(msg.GetArray(packet), false);
                        break;
                    }
                case MsgMessage.ChatMode.World:
                    {
                        if (DateTime.Now > client.Player.LastWorldMessaj.AddSeconds(15))
                        {
                            client.Player.LastWorldMessaj = DateTime.Now;
                            foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
                            {
                                var user = kv.Value;
                                if (user.Player.UID != client.Player.UID)
                                {
                                    if (user.Player.InUnion)
                                        user.Send(msg.GetArray(packet, (uint)Role.Instance.Union.Member.GetRank(user.Player.UnionMemeber.Rank)));
                                    else
                                        user.Send(msg.GetArray(packet));
                                }
                            }

                        }
                        break;
                    }
                case ChatMode.Whisper:
                    {
                        bool send = false;
                        foreach (var kv in Pool.GamePoll) // [perf] live lock-free enumeration (no .Values snapshot)
                        {
                            var user = kv.Value;
                            if (user.Player.Name == msg._To)
                            {
                                msg.Mesh = client.Player.Mesh;
                                msg.UID = client.Player.UID;
                                user.Send(msg.GetArray(packet));
                                send = true;
                                break;
                            }
                        }
                        if (!send)
                        {
#if Arabic
                                client.SendSysMesage("The player is not online.", ChatMode.System, MsgColor.white);
#else
                            client.SendSysMesage("The player is not online.", ChatMode.System, MsgColor.white);
#endif

                        }
                        break;
                    }
                case ChatMode.Guild:
                    {
                        if (client.Player.MyGuild != null)
                            client.Player.MyGuild.SendPacket(msg.GetArray(packet));
                        break;
                    }
                case ChatMode.Friend:
                    {
                        System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Associate.Member> friends;
                        if (client.Player.Associate.Associat.TryGetValue(Role.Instance.Associate.Friends, out friends))
                        {
                            foreach (var user in Pool.GamePoll.Values)
                            {
                                if (friends.ContainsKey(user.Player.UID))
                                    user.Send(msg.GetArray(packet));
                            }
                        }
                        break;
                    }
                case ChatMode.Clan:
                    {

                        if (client.Player.MyClan != null)
                            client.Player.MyClan.Send(msg.GetArray(packet));
                        break;
                    }
            }
        }

        public static uint TestGui = 0;
        public static object commandsFile = new object();
        public static unsafe bool NpcCommands(Client.GameClient client, MsgMessage msg)
        {
            // Security: this method hands out max runes/chi/jiang/ninja/archive. It must never be a
            // backdoor for normal players, so require real GM authorization before doing anything.
            if (client == null || !client.IsGm)
                return false;
            #region CommandsNpc
            msg.__Message = msg.__Message.Replace("#73", "").Replace("#60", "").Replace("#61", "").Replace("#62", "").Replace("#63", "").Replace("#64", "").Replace("#65", "").Replace("#66", "").Replace("#67", "").Replace("#68", "");
            if (msg.__Message.StartsWith("!") || msg.__Message.StartsWith("@"))
            {
                string Message = msg.__Message.Substring(1);
                string[] data = Message.Split(' ');
                switch (data[0])
                {
                    case "myhthss":
                        {
                            foreach (var item in Database.MythTable.MythSoulExpList.Values)
                            {

                                if (item.ItemType / 100000 == 407 && item.ItemType % 10 == 6)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        uint _itemid = client.Inventory.AddInbox(stream, item.ItemType);
                                        PrizeInfo mailList2 = new PrizeInfo(client, "Gift", "STR_SYSTEM_NAME@@", "STR_SYSTEM_NAME@@", 2592000, 0, 0, _itemid, 0, 1);
                                    }
                                }
                            }
                            break;
                        }
                    case "redrunes":
                        {
                            foreach (var item in Pool.ItemsBase.Values)
                            {
                                if (Database.ItemType.isRedRune(item.ID))
                                {
                                    if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                        using (var rec = new ServerSockets.RecycledPacket())
                                            client.Inventory.Add(rec.GetStream(), item.ID);
                                }
                            }
                            break;

                        }


                    case "jiangpower":
                        {
                            // jiangpower 1 1 6
                            var Stage = byte.Parse(data[1]);
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)byte.Parse(data[2]);
                            var level = byte.Parse(data[3]);

                            if (client.Player.MyJiangHu.ArrayStages.Length >= Stage)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(Stage - 1)];
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                        client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                        client.Player.MyJiangHu.Buckupstar.Stage = Stage;
                                        client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(Stage);
                                        client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                        client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                        client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                        client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                        client.Player.MyJiangHu.ApplayNewStar(client);
                                    }
                                    if (client.Player.MyJiangHu != null)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.Player.MyJiangHu.SendStatus(stream, client, client);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "ninja_sigil":
                        {
                            try
                            {
                                foreach (var i in NinjaFile.gouyu_type.Values)
                                {
                                    if (i.Level == 9)
                                    {
                                        client.MyNinja.AddItem(i.ItemID, 9, 0, 0, 0);
                                    }
                                }
                            }
                            catch (Exception ex) { Console.WriteLine(ex); }
                            break;
                        }
                    case "open_ninja":
                        {
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock_2);
                            client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death_2);
                            break;
                        }
                    case "chipower":
                        {
                            //  ;  dragon stage power
                            //   chipower 1 0 1
                            if (data.Length < 3)
                                break;
                            var ChiPowerType = (MsgChiInfo.ChiPowerType)byte.Parse(data[1]);
                            var Stage = byte.Parse(data[2]);
                            var Power = (MsgChiInfo.ChiAttribute)byte.Parse(data[3]);
                            var ChiPower = client.Player.MyChi.SingleOrDefault(p => p.Type == (Game.MsgServer.MsgChiInfo.ChiPowerType)(ChiPowerType));
                            if (!ChiPower.UnLocked)
                            {
                                ChiPower.UnLocked = true;
                                ChiPower.UID = client.Player.UID;
                                ChiPower.Name = client.Player.Name;
                            }
                            if (ChiPower != null)
                            {
                                var Value = Role.Instance.Chi.ChiMaxValues(Power);
                                ChiPower.Attributes[Stage].Type = Power;
                                ChiPower.Attributes[Stage].Value = (ushort)Value;
                                Pool.ChiRanking.Upadte(Pool.ChiRanking.Phoenix, ChiPower);
                                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade);
                            }
                            break;
                        }

                    #region Ninja




                    case "get_ninja_items":
                        {
                            for (int i = 3330028; i <= 3330062; i++)
                            {
                                if (client.Inventory.HaveSpace(34))
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.Add(rec.GetStream(), (uint)i, 0, 1);
                            }
                            break;
                        }
                    case "ninja_mode":
                        {
                            client.MyNinja.SageMode(uint.Parse(data[1]));
                            break;
                        }
                    #endregion

                    case "maxarchives":
                        {
                            if (client.HundredWeapons.Valid())
                            {
                                client.HundredWeapons.Unlocked = true;
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Blade, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Sword, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hook, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Whip, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Club, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hammer, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Axe, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Dagger, 9, false);
                                client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Scepter, 9, false);
                                foreach (var weapon in client.HundredWeapons.Objects.Values)
                                {
                                    var atts = weapon.Attributes.ToArray();
                                    foreach (var att in atts)
                                        weapon.Attributes[att.Key] = weapon.DBInfo.Attributes[att.Key];
                                }
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Send(stream.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                }
                                client.HundredWeapons.LoadPowerFoucs(client);
                            }
                            break;
                        }



                    case "maxblue":
                        {
                            foreach (var item in Pool.ItemsBase.Values)
                            {
                                if (Database.ItemType.isBlueRune(item.ID))
                                {
                                    if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                        using (var rec = new ServerSockets.RecycledPacket())
                                            client.Inventory.Add(rec.GetStream(), item.ID);
                                }
                            }
                            break;
                        }
                    case "maxyellow":
                        {
                            foreach (var item in Pool.ItemsBase.Values)
                            {
                                if (Database.ItemType.isYellowRune(item.ID))
                                {
                                    if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                        using (var rec = new ServerSockets.RecycledPacket())
                                            client.Inventory.AddReturnedItem(rec.GetStream(), item.ID);
                                }
                            }
                            break;
                        }
                }
            }
            return false;
            #endregion
        }

        public static unsafe bool ChatCommands(Client.GameClient client, MsgMessage msg)
        {
            string logss = "Chat Players [2]" + msg._From + " to " + msg._To + " " + msg.__Message + "";
            Database.ServerDatabase.LoginQueue.Enqueue(logss);
            ServerLogs.AddChatLog(msg, client);

            msg.__Message = msg.__Message.Replace("#70", "").Replace("#71", "").Replace("#72", "").Replace("#73", "").Replace("#74", "").Replace("#75", "").Replace("&245&", "").Replace("&246&", "").Replace("&247&", "").Replace("&248&", "").Replace("&249&", "").Replace("&250&", "");
            if (global::Qahr.Mobile.Phone.MobileEvents.ChatCommand(client, msg.__Message))
                return true;// QahrMobile: @join / @events (mobile event invitations)
            if (global::Qahr.Mobile.GuildAuction.GuildAuctionModule.ChatCommand(client, msg.__Message))
                return true;// QahrMobile: @gauction (guild auction, GM only), Features\GuildAuction
            if (global::Qahr.Mobile.Phone.MobileDaily.ChatCommand(client, msg.__Message))
                return true;// QahrMobile: @mdaily (mobile daily activity test, GM), Networking\Mobile\MobileDaily.cs
            if (msg.__Message.StartsWith("@"))
            {

                string Message = msg.__Message.Substring(1);
                string[] data = Message.Split(' ');
            }
            #region GMCommands
            // Authorize GM commands by the authoritative account State (from the encrypted login token),
            // keeping the legacy name check only as a fallback. Destructive, server-wide commands below
            // (restartall*, ban/banip/kick, interip, exit) are additionally gated on client.IsOwner so a
            // forged [GM] name can never trigger them.
            if (client.IsStaffAccount || client.Player.Name.Contains("[GM]"))
            {
                msg.__Message = msg.__Message.Replace("#70", "").Replace("#71", "").Replace("#72", "").Replace("#73", "").Replace("#74", "").Replace("#75", "").Replace("&245&", "").Replace("&246&", "").Replace("&247&", "").Replace("&248&", "").Replace("&249&", "").Replace("&250&", "");
                if (msg.__Message.StartsWith("@"))
                {
                    string Message = msg.__Message.Substring(1);
                    string[] data = Message.Split(' ');
                    try
                    {
                        switch (data[0])
                        {
                            case "jiangpower":
                                {
                                    // jiangpower 1 1 6
                                    var Stage = byte.Parse(data[1]);
                                    var type = (Role.Instance.JiangHu.Stage.AtributesType)byte.Parse(data[2]);
                                    var level = byte.Parse(data[3]);

                                    if (client.Player.MyJiangHu.ArrayStages.Length >= Stage)
                                    {
                                        var stage = client.Player.MyJiangHu.ArrayStages[(Stage - 1)];
                                        for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                                client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                                client.Player.MyJiangHu.Buckupstar.Stage = Stage;
                                                client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(Stage);
                                                client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                                client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                                client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                                client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                                client.Player.MyJiangHu.ApplayNewStar(client);
                                            }
                                            if (client.Player.MyJiangHu != null)
                                            {
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                {
                                                    var stream = rec.GetStream();
                                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "bll":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.UpdateArrowBlades(stream, 1);
                                    }
                                    break;
                                }
                            case "Heroo":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        stream.Seek(stream.Size - 8);
                                        stream.Finalize(Game.GamePackets.MsgTalk);
                                        MsgInterServer.StaticConnexion.Send(stream);
                                    }
                                    break;
                                }

                            #region Ninja
                            case "ninja_sigil":
                                {
                                    try
                                    {
                                        foreach (var i in NinjaFile.gouyu_type.Values)
                                        {
                                            if (i.Level == 9)
                                            {
                                                client.MyNinja.AddItem(i.ItemID, 9, 0, 0, 0);
                                            }
                                        }
                                    }
                                    catch (Exception ex) { Console.WriteLine(ex); }
                                    break;
                                }
                            case "open_ninja":
                                {
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock_2);
                                    client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death_2);
                                    break;

                                }
                            case "get_ninja_items":
                                {
                                    for (int i = 3330028; i <= 3330062; i++)
                                    {

                                        using (var rec = new ServerSockets.RecycledPacket())
                                            client.Inventory.Add(rec.GetStream(), (uint)i, 0, 1);
                                    }
                                    break;
                                }
                            case "ninja_mode":
                                {
                                    client.MyNinja.SageMode(uint.Parse(data[1]));
                                    break;
                                }
                            #endregion
                            case "chipower":
                                {
                                    //  ;  dragon stage power
                                    //   chipower 1 0 1
                                    if (data.Length < 3)
                                        break;
                                    var ChiPowerType = (MsgChiInfo.ChiPowerType)byte.Parse(data[1]);
                                    var Stage = byte.Parse(data[2]);
                                    var Power = (MsgChiInfo.ChiAttribute)byte.Parse(data[3]);
                                    var ChiPower = client.Player.MyChi.SingleOrDefault(p => p.Type == (Game.MsgServer.MsgChiInfo.ChiPowerType)(ChiPowerType));
                                    if (!ChiPower.UnLocked)
                                    {
                                        ChiPower.UnLocked = true;
                                        ChiPower.UID = client.Player.UID;
                                        ChiPower.Name = client.Player.Name;
                                    }
                                    if (ChiPower != null)
                                    {
                                        if (ChiPower.ContainAtribut(Power) == false)
                                        {
                                            var Value = Role.Instance.Chi.ChiMaxValues(Power);
                                            ChiPower.Attributes[Stage].Type = Power;
                                            ChiPower.Attributes[Stage].Value = (ushort)Value;
                                            Pool.ChiRanking.Upadte(Pool.ChiRanking.Phoenix, ChiPower);
                                            Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Send);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, MsgChiInfo.Action.Upgrade);
                                        }
                                    }
                                    break;
                                }
                            case "ClearA":
                                {
                                    client.MyArchives.Close();
                                    client.MyArchives.Close();
                                    client.Player.Class = 4051;

                                    break;
                                }

                            case "StartCPCastle":
                                {
                                    Game.MsgTournaments.MsgSchedules.CpsCastle.Start();
                                    break;
                                }
                            case "StartOctopus":
                                {
                                    Game.MsgTournaments.MsgSchedules.SquidwardOctopus.Start();
                                    break;
                                }
                            case "StartGoldBox":
                                {
                                    Game.MsgTournaments.MsgSchedules.GoldBoxWar.Start();
                                    break;
                                }
                            case "EndedGoldBox":
                                {
                                    Game.MsgTournaments.MsgSchedules.GoldBoxWar.Finish();
                                    break;
                                }
                            case "EndedCPCastle":
                                {
                                    Game.MsgTournaments.MsgSchedules.CpsCastle.Finish();
                                    break;
                                }

                            case "StartEliteGuildWar":
                                {
                                    Game.MsgTournaments.MsgSchedules.EliteGuildWar.Start();
                                    break;
                                }

                            case "EndedEliteGuildWar":
                                {
                                    Game.MsgTournaments.MsgSchedules.EliteGuildWar.End();
                                    break;
                                }

                            case "StartFlagsWar":
                                {
                                    Game.MsgTournaments.MsgSchedules.FlagsWar.Start();
                                    break;
                                }

                            case "FinishFlagsWar":
                                {
                                    Game.MsgTournaments.MsgSchedules.FlagsWar.Finish();
                                    break;
                                }

                            case "ererg":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        if (client.Team.TeamLider(client))
                                        {
                                            client.Send(stream.TeamEliteSetTeamName(GamePackets.MsgDominateTeamPopPkName, MsgTeamEliteSetTeamName.State.Apprend, client.Team.UID, client.Team.TeamName));
                                        }
                                    }
                                    break;
                                }

                            case "allgarment":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        foreach (var u in Pool.GamePoll.Values)
                                        {
                                            if (u.Player.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                                            { target = u; break; }
                                        }
                                    }

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        int count = 0;
                                        foreach (var dbItem in Pool.ItemsBase.Values)
                                        {
                                            if (Database.ItemType.ItemPosition(dbItem.ID) == (ushort)Role.Flags.ConquerItem.Garment)
                                            {
                                                if (!target.MyWardrobe.Contain(dbItem.ID))
                                                {
                                                    var newItem = new MsgGameItem()
                                                    {
                                                        UID = Pool.ITEM_Counter.Next,
                                                        ITEM_ID = dbItem.ID,
                                                        MaximDurability = dbItem.Durability,
                                                        Durability = dbItem.Durability,
                                                        Mode = Role.Flags.ItemMode.AddItem
                                                    };
                                                    target.MyWardrobe.AddItem(newItem);
                                                    count++;
                                                }
                                            }
                                        }
                                        target.MyWardrobe.SendToClient(stream);
                                        client.SendSysMesage(string.Format("[WARDROBE] Unlocked {0} new garments for {1}.", count, target.Player.Name), ChatMode.System, MsgColor.red);
                                    }
                                    return true;
                                }
                            case "allmedals":
                                {
                                    Client.GameClient target = client;

                                    if (data.Length > 1)
                                    {
                                        string name = data[1];

                                        foreach (var u in Pool.GamePoll.Values)
                                        {
                                            if (u.Player.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                                            {
                                                target = u;
                                                break;
                                            }
                                        }
                                    }

                                    int count = 0;

                                    for (uint i = 1; i <= 11; i++)
                                    {
                                        if (!target.MedalStorage.Medals.ContainsKey(i))
                                        {
                                            var medal = new MedalStorage.Medal()
                                            {
                                                UserId = target.Player.UID,
                                                Type = i,
                                                Activate = false,
                                                DateTime = ulong.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
                                                DelTime = (uint)(MedalStorage.Timestamp() + (365 * 24 * 60 * 60))
                                            };

                                            target.MedalStorage.Medals.TryAdd(i, medal);

                                            target.Send(
                                                target.MedalStorage.GetMsgMedals(
                                                    MsgMedalStorageType.Create,
                                                    target.Player.UID,
                                                    medal
                                                )
                                            );

                                            count++;
                                        }
                                    }

                                    target.MedalStorage.Update();

                                    client.SendSysMesage(
                                        $"[MEDALS] Unlocked {count} medals for {target.Player.Name}.", // [GmSec 2026-10-14] was missing $
                                        ChatMode.System,
                                        MsgColor.red
                                    );

                                    return true;
                                }
                            case "allmount":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        foreach (var u in Pool.GamePoll.Values)
                                        {
                                            if (u.Player.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                                            { target = u; break; }
                                        }
                                    }

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        int count = 0;
                                        foreach (var dbItem in Pool.ItemsBase.Values)
                                        {
                                            if (Database.ItemType.ItemPosition(dbItem.ID) == (ushort)Role.Flags.ConquerItem.SteedMount)
                                            {
                                                if (!target.MyWardrobe.Contain(dbItem.ID))
                                                {
                                                    var newItem = new MsgGameItem()
                                                    {
                                                        UID = Pool.ITEM_Counter.Next,
                                                        ITEM_ID = dbItem.ID,
                                                        MaximDurability = dbItem.Durability,
                                                        Durability = dbItem.Durability,
                                                        Mode = Role.Flags.ItemMode.AddItem
                                                    };
                                                    target.MyWardrobe.AddItem(newItem);
                                                    count++;
                                                }
                                            }
                                        }
                                        target.MyWardrobe.SendToClient(stream);
                                        client.SendSysMesage(string.Format("[WARDROBE] Unlocked {0} new mount armors for {1}.", count, target.Player.Name), ChatMode.System, MsgColor.red);
                                    }
                                    return true;
                                }


                            case "allcoll":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    foreach (var dbItem in Pool.ItemsBase.Values)
                                    {
                                        uint id = dbItem.ID;
                                        bool isCollection = (id == 4320002 || (id >= 4300000 && id <= 4300004) ||
                                                           (id >= 4310000 && id <= 4310002) || (id >= 4320000 && id <= 4320001) ||
                                                           (id >= 4330000 && id <= 4330004) || (id >= 4331000 && id <= 4332000) ||
                                                           id == 4330005 || id == 4300005);

                                        if (isCollection)
                                        {
                                            if (!target.Collection.items.ContainsKey(id))
                                            {
                                                target.Collection.items.TryAdd(id, new Role.Instance.CollectionStorge.item()
                                                {
                                                    ID = id,
                                                    Actived = 1,
                                                    Progress = 100,
                                                    Use = 0
                                                });
                                            }
                                            else
                                            {
                                                target.Collection.items[id].Actived = 1;
                                                target.Collection.items[id].Progress = 100;
                                            }
                                        }
                                    }
                                    target.Collection.Load();
                                    client.SendSysMesage(string.Format("Unlocked all Special Wardrobe (Collection) items for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }

                            case "allavatar":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    foreach (var hf in Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Avatar))
                                    {
                                        if (!target.HairfaceStorage.Exists(hf.ID, hf.Type))
                                            target.HairfaceStorage.Add(hf, false);
                                    }
                                    client.SendSysMesage(string.Format("Unlocked all Avatars for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }
                            case "allhair":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    foreach (var hf in Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Hairstyle))
                                    {
                                        if (!target.HairfaceStorage.Exists(hf.ID, hf.Type))
                                            target.HairfaceStorage.Add(hf, false);
                                    }
                                    client.SendSysMesage(string.Format("Unlocked all Hair Styles for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }
                            case "allframe":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    foreach (var hf in Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Level))
                                    {
                                        if (!target.HairfaceStorage.Exists(hf.ID, hf.Type))
                                            target.HairfaceStorage.Add(hf, false);
                                    }
                                    client.SendSysMesage(string.Format("Unlocked all Chat Frames for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }
                            case "allprofile":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    var profileTypes = new[] { 
                                        MsgHairfaceStorage.Type.TexasTable, MsgHairfaceStorage.Type.CardBack, 
                                        MsgHairfaceStorage.Type.TexasBet, MsgHairfaceStorage.Type.Level, 
                                        MsgHairfaceStorage.Type.Map, MsgHairfaceStorage.Type.Dealer, 
                                        MsgHairfaceStorage.Type.Carpet, MsgHairfaceStorage.Type.Emoji, 
                                        MsgHairfaceStorage.Type.Head 
                                    };

                                    foreach (var hf in Database.HairfaceStorageTable.Hairfaces.Where(i => profileTypes.Contains(i.Type)))
                                    {
                                        if (!target.HairfaceStorage.Exists(hf.ID, hf.Type))
                                            target.HairfaceStorage.Add(hf, false);
                                    }
                                    client.SendSysMesage(string.Format("Unlocked all Profile/Card items for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }
                            case "allms":
                            case "allmythsoul":
                                {
                                    Client.GameClient target = client;
                                    if (data.Length > 1)
                                    {
                                        string name = data[1];
                                        target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.ToLower() == name.ToLower());
                                    }
                                    if (target == null) return true;

                                    string[] whitelist = { "Free Soul", "Protection", "Wellspring", "Edge", "Elan", "Venom", "Hayaku", "Blood Thirst", "Ethereal", "Solid", "Sweep", "Demolition", "Meditation" };

                                    // Robust detection: scan items that start with 307-308 (Soul IDs) AND contain a whitelisted wearable name
                                    var maxSouls = Pool.ItemsBase.Values
                                        .Where(i => (i.ID >= 3070001 && i.ID <= 3089999) && whitelist.Any(w => i.Name.ToLower().Contains(w.ToLower())))
                                        .GroupBy(i => i.ID / 10)
                                        .Select(g => g.OrderByDescending(i => i.ID % 10).First());

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        foreach (var dbItem in maxSouls)
                                        {
                                            if (!target.MythSoulBag.Values.Any(i => i.ITEM_ID / 10 == dbItem.ID / 10))
                                            {
                                                MsgGameItem soul = new MsgGameItem();
                                                soul.ITEM_ID = dbItem.ID;
                                                soul.UID = Pool.ITEM_Counter.Next;
                                                soul.Mode = Role.Flags.ItemMode.AddItem;
                                                soul.Position = (ushort)Role.Flags.ConquerItem.MythSoulBag;
                                                if (target.MythSoulBag.TryAdd(soul.UID, soul))
                                                {
                                                    soul.Send(target, stream);
                                                }
                                            }
                                        }
                                    }
                                    client.SendSysMesage(string.Format("Maxed out all ELITE MythSoul bag items for {0}.", target.Player.Name), ChatMode.System, MsgColor.red);
                                    return true;
                                }

                            case "myth":
                                {
                                    foreach (var item in Pool.ItemsBase.Values)
                                    {
                                        byte Level = (byte)(item.ID % 10);

                                        if (item.ID / 100000 == 407 && item.ID % 10 == byte.Parse(data[1]) && (item.ID % 1000) - Level == byte.Parse(data[2]))
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();

                                                uint _itemid = client.Inventory.AddInbox(stream, item.ID);
                                                PrizeInfo mailList2 = new PrizeInfo(client, "Gift", "STR_SYSTEM_NAME@@", "STR_SYSTEM_NAME@@", 2592000, 0, 0, _itemid, 0, 1);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "myhthss":
                                {
                                    foreach (var item in Database.MythTable.MythSoulExpList.Values)
                                    {

                                        if (item.ItemType / 100000 == 407 && item.ItemType % 10 == 6)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                uint _itemid = client.Inventory.AddInbox(stream, item.ItemType);
                                                PrizeInfo mailList2 = new PrizeInfo(client, "Gift", "STR_SYSTEM_NAME@@", "STR_SYSTEM_NAME@@", 2592000, 0, 0, _itemid, 0, 1);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "TestMail":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        //uint _itemid = 2100075;
                                        //for (byte i = 1; i < 10; i++)
                                        //{
                                        //    new PrizeInfo(client, "Boss Reward", "Prize Rank", "This Prize For Rank 1", 30 * 24 * 60 * 60, 0, 0, 0, 1, 1);
                                        //}
                                        client.Send(new MsgMessage(client.Player.Name, " Kill the NemesisTyrant Get 1000 CPs ", 3, 0).GetArray(stream));
                                    }
                                    break;
                                }
                            case "Hair":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 1000)) + byte.Parse(data[1]));

                                        client.Player.SendUpdate(stream, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);
                                        client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                                    }
                                    break;
                                }

                            case "killcz":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var info = new Game.MsgServer.MsgBattleEffectiveness.ProtoStructure();
                                        info.Member1 = client.Player.UID;
                                        info.Member2 = 216;
                                        info.Member3 = 0;
                                        client.Send(stream.CreateBattleEffectiveness(info));
                                    }

                                    break;
                                }
                            case "a":
                                {

                                    client.MyArchives.AddItem((Archives.TypeID)uint.Parse(data[1]), uint.Parse(data[2]), 0, 0, 0);
                                    break;
                                }
                            case "A702":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.FrozenNut, 1, 0, 1, 0);
                                    break;
                                }
                            case "A703":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.LavaNut, 1, 0, 1, 0);
                                    break;
                                }
                            case "Lee":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Dragon, 1, 0, 1, 0);
                                    break;
                                }
                            case "Lee1":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Kunpeng, 1, 0, 1, 0);
                                    break;
                                }
                            case "Lee2":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Suanni, 1, 0, 1, 0);
                                    break;
                                }
                            case "a1":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddFlag(MsgUpdate.Flags.ActiveWeapon, 30, true);
                                        uint Type = (uint)(client.HundredWeapons.TryGetItem(5).WeaponSubtype) * 10000 + 510;
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveWeapon, 0, 0, Type, MsgUpdate.DataType.ArchiveSkill, true);
                                    }
                                    break;
                                }
                            case "a2":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SupremeLeadershipCount = 5;

                                        uint Power;
                                        int Sec;
                                        Game.MsgServer.AttackHandler.SupremeLeadership.GetUpdateAmount(client.Player.SupremeLeadershipCount, out Power, out Sec);
                                        client.Player.AddSpellFlag(MsgUpdate.Flags.SupremeLeadership, (int)Sec, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.SupremeLeadership, (uint)Sec, Power, (uint)client.Player.SupremeLeadershipCount, MsgUpdate.DataType.ArchiveSkill, true);
                                    }
                                    break;
                                }

                            case "Heart":
                                {

                                    client.MyArchives.AddItem((Archives.TypeID)byte.Parse(data[1]), 30, 0, 0, 0);
                                    break;
                                }
                            case "compatArcher1":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.StoneCracker, 1, 0, 0, 0);
                                    break;
                                }
                            case "FoxConquer":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        #region Create ini
                                        WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

                                        foreach (string fmap in System.IO.Directory.GetDirectories(Program.ServerConfig.DbLocation + "\\Test\\"))
                                        {
                                            foreach (string fmobtype in System.IO.Directory.GetDirectories(fmap))
                                            {
                                                foreach (string ffile in System.IO.Directory.GetFiles(fmobtype))
                                                {
                                                    ini.FileName = ffile;
                                                    var Item = Qahr.Mobile.Database.MythTable.MythSoulExpList.Values.Where(p => p.ItemType % 10 == 6).ToArray();
                                                    for (int i = 0; i <= Item.Length; i++)
                                                    {
                                                        ini.WriteString("cq_generator", "Item" + i.ToString() + "=", "" + Item[i].ItemType.ToString() + "@@500@@0@@0@0@0@0@@1");
                                                    }

                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                }
                            case "OnlinePointsss":
                                {
                                    var target = Pool.GamePoll.Values.FirstOrDefault(i => i.Player.Name == data[1]);
                                    if (target != null)
                                    {
                                        target.Player.OnlineMinutes += uint.Parse(data[2]);
                                    }
                                    break;
                                }
                            case "GodzillaPointsss":
                                {
                                    var target = Pool.GamePoll.Values.FirstOrDefault(i => i.Player.Name == data[1]);
                                    if (target != null)
                                    {
                                        target.Player.GodzillaPoints += uint.Parse(data[2]);
                                    }
                                    break;
                                }
                            case "startcastle":
                                {
                                    MsgTournaments.MsgSchedules.CpsCastle.Start();
                                    client.SendSysMesage("CP Castle event has been started manually.", ChatMode.System, MsgColor.red);
                                    return true;
                                }

                            case "dmg":
                                {
                                    client.GodDmgMode = !client.GodDmgMode;
                                    if (client.GodDmgMode)
                                    {
                                        client.Status.MaxHitpoints = 1000000000;
                                        client.Player.HitPoints = 1000000000;
                                        client.Player.ExtraBattlePower = 1000000;
                                    }
                                    else
                                    {
                                        client.Player.ExtraBattlePower = 0;
                                        client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, (long)client.Player.HitPoints, MsgUpdate.DataType.Hitpoints);
                                        client.Player.SendUpdate(stream, (long)client.Status.MaxHitpoints, MsgUpdate.DataType.MaxHitpoints);
                                        client.Player.SendUpdate(stream, (long)client.Player.ExtraBattlePower, MsgUpdate.DataType.ExtraBattlePower);
                                        client.Player.SendUpdate(stream, (long)client.Player.Money, MsgUpdate.DataType.Money);
                                        client.Player.SendUpdate(stream, (long)client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                    }
                                    string state = client.GodDmgMode ? "Enabled" : "Disabled";
                                    client.SendSysMesage(string.Format("[GOD MODE] {0} successfully! You are now UNSTOPPABLE!", state), ChatMode.System, MsgColor.red);
                                    return true;
                                }
                            case "compatArcher2":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.ColdMoon, 1, 0, 0, 0);
                                    break;
                                }
                            case "compatArcher3":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.ThornCutter, 1, 0, 0, 0);
                                    break;
                                }
                            case "compatFire1":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Vicissitude, 10, 0, 0, 0);
                                    break;
                                }
                            case "compatFire2":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.HighestGood, 1, 0, 0, 0);
                                    break;
                                }
                            case "compatFire3":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Evolution, 1, 0, 0, 0);
                                    break;
                                }
                            case "compatFire4":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.Thrill, 1, 0, 0, 0);
                                    break;
                                }
                            case "compatFire5":
                                {

                                    client.MyArchives.AddItem(Archives.TypeID.BirthDeath, 1, 0, 0, 0);
                                    break;
                                }
                            case "test222":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            dwParam = 3264,
                                            Type = ActionType.OpenCustom,
                                            Timestamp = (uint)Time32.Now.AllSeconds(),
                                            ObjId = client.Player.UID
                                        };
                                        client.Send(stream.ActionCreate(action));

                                    }
                                    break;
                                }
                            case "melt":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(new MsgMessage("STR_ID_tMelter[BroadCast][Result]@@" + client.Player.Name + "@@<BB@@>@@<STR_ID_tLuaRes[10026]@@<STR_ID_tLuaRes[10025]@@<STR_ITEM_TYPE_3310998@@>@@<STR_ID_tRewardTemplate[Gift]@@>@@>@@<STR_ID_tRewardTemplate[Number]@@>@@2@@>@@", MsgColor.white, ChatMode.Melting).GetArray(stream));
                                    }
                                    break;
                                }
                            case "Cyan":
                                {
                                    client.Player.CyanJadeRing += 100000;
                                    break;
                                }
                            case "UnChatBanned":
                                {
                                    string Name = string.Empty;
                                    uint UID = 0;
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    Name = data[1];
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        string RName = ini.ReadString("Character", "Name", "None");
                                        if (RName.GetHashCode() == Name.GetHashCode())
                                        {
                                            UID = ini.ReadUInt32("Character", "UID", 0);
                                            break;
                                        }

                                    }
                                    Client.GameClient clienttoban = null;
                                    if (Pool.GamePoll.TryGetValue(UID, out clienttoban))
                                    {
                                        clienttoban.Player.BannedChatStamp = DateTime.Now;
                                        clienttoban.Player.IsBannedChat = false;
                                        clienttoban.Player.PermenantBannedChat = false;
                                        MyConsole.WriteLine("Player In GamePool UnBanned Chat.");
                                    }
                                    else
                                    {
                                        WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                                        write.Write<bool>("Character", "IsBannedChat", false);
                                        write.Write<long>("Character", "BannedChatStamp", DateTime.Now.ToBinary());
                                        write.Write<bool>("Character", "PermenantBannedChat", false);
                                        global::Qahr.Mobile.Database.Storage.PlayerStorage.OnIniChanged(UID);
                                        MyConsole.WriteLine("Player In Database UnBanned Chat.");
                                    }
                                    break;
                                }
                            case "ChatBanned":
                                {
                                    string Name = string.Empty;
                                    DateTime Time = DateTime.Now;
                                    bool Permenant = false;
                                    if (data.Length < 2)
                                        break;
                                    Name = data[1];
                                    uint UID = 0;
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        string RName = ini.ReadString("Character", "Name", "None");
                                        if (RName.GetHashCode() == Name.GetHashCode())
                                        {
                                            UID = ini.ReadUInt32("Character", "UID", 0);
                                            break;
                                        }

                                    }
                                    try
                                    {
                                        int add = int.Parse(data[2]);
                                        Time = DateTime.Now.AddMinutes(add);
                                    }
                                    catch
                                    {
                                        if (data[2] == "Permemnat")
                                        {
                                            Permenant = true;
                                        }
                                    }
                                    Client.GameClient clienttoban = null;
                                    if (Pool.GamePoll.TryGetValue(UID, out clienttoban))
                                    {
                                        if (!Permenant)
                                        {
                                            clienttoban.Player.BannedChatStamp = Time;
                                        }
                                        else
                                        {
                                            clienttoban.Player.PermenantBannedChat = Permenant;
                                        }
                                        clienttoban.Player.IsBannedChat = true;
                                    }
                                    else
                                    {
                                        WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
                                        write.Write<bool>("Character", "IsBannedChat", true);
                                        if (!Permenant)
                                        {
                                            write.Write<long>("Character", "BannedChatStamp", Time.ToBinary());
                                        }
                                        else
                                        {
                                            write.Write<bool>("Character", "PermenantBannedChat", Permenant);
                                        }
                                        global::Qahr.Mobile.Database.Storage.PlayerStorage.OnIniChanged(UID);
                                    }
                                    break;


                                }

                            case "cunion":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MyUnion = Role.Instance.Union.Create(stream, client, data.Length > 0 ? data[1] : "Unknown");
                                        client.Player.MyUnion.AddGuild(stream, client.Player.MyGuild);
                                    }
                                    break;

                                }
                            case "addguiitem":
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var apacket = rec.GetStream();
                                        client.Inventory.AddReturnedItem(apacket, uint.Parse(data[1]), byte.Parse(data[2]));
                                    }
                                    break;


                                }
                            case "RemoveDonate":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Pool.NobilityRanking.RemoveAndUpdateTheRest(uint.Parse(data[1]));
                                    }
                                    break;
                                }
                            // [HotReload] @reloadnpcs / @reloadportals / @reloadshops / @reloaditems / @reloadmonsters /
                            // @reloadmapcells <map> / @reloadall - owner only; runs off this socket thread and replies here.
                            case "reloadnpcs":
                            case "reloadportals":
                            case "reloadshops":
                            case "reloaditems":
                            case "reloadmonsters":
                            case "reloadmapcells":
                            case "reloadquests":// [QuestEngine]
                            case "reloaddrops":// [FileDrops] drops.txt
                            case "reloadatkspeed":// [AttackSpeed] attackspeed.txt
                            case "atkspeed":
                            case "questreload":
                            case "reloadall":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    if (data[0] == "questreload") data[0] = "reloadquests";
                                    if (data[0] == "atkspeed") data[0] = "reloadatkspeed";
                                    client.SendSysMesage("[HotReload] " + data[0] + " started...");
                                    Qahr.Mobile.HotReload.HotReloadManager.RunForPlayer(client, data[0], data.Length > 1 ? data[1] : null);
                                    break;
                                }
                            case "questinfo":// [QuestEngine] @questinfo <MissionId>: parsed objectives, prize, engine status
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    global::Qahr.Mobile.QuestEngine.QuestEngineCore.CommandQuestInfo(client, data.Length > 1 ? data[1] : "");
                                    break;
                                }
                            case "festival":// [Festivals] @festival list | info <key> | on <key> | off <key> | auto <key> | reload
                            case "festivals":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    global::Qahr.Mobile.Festivals.FestivalManager.CommandForPlayer(client, data.Skip(1).ToArray());
                                    break;
                                }
                            case "tool":
                            case "tools":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    if (data.Length > 1)
                                    {
                                        client.SendSysMesage(Qahr.Mobile.ServerTools.ToolLauncher.Open(Qahr.Mobile.ServerTools.ToolLauncher.Find(data[1])));
                                        break;
                                    }
                                    client.ActiveNpc = (uint)Qahr.Mobile.Game.MsgNpc.NpcID.ToolsMenu;
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        Qahr.Mobile.Game.MsgNpc.NpcHandler.ToolsMenuDialog(client, rec.GetStream(), 0, "", (uint)Qahr.Mobile.Game.MsgNpc.NpcID.ToolsMenu);
                                    break;
                                }
                            case "bc":
                                {
                                    if (data.Length < 2) break; // [GmSec 2026-10-14]
                                    string bcText = string.Join(" ", data, 1, data.Length - 1); // [GmSec 2026-10-14] whole message, not only the first word
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(bcText, "ALLUSERS", MsgColor.red, ChatMode.BroadcastMessage).GetArray(stream));
                                    }
                                    break;
                                }
                            case "kingdom":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MyUnion.UpdateToKingdom(stream);
                                    }
                                    break;
                                }
                            case "munion":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MyUnion.UpdateToUnion(stream);
                                    }
                                    break;
                                }
                            case "union":
                                {


                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.LeagueOptCreate((MsgLeagueOpt.ActionID)ushort.Parse(data[1]), 10, 100, 1, ""));
                                    }
                                    break;
                                }
                            case "hide":
                                {
                                    client.Player.Invisible = true;
                                    break;
                                }
                            case "unhide":
                                {
                                    client.Player.Invisible = false;
                                    break;
                                }

                            case "battlepoints":
                                {
                                    client.Player.BattleFieldPoints = ushort.Parse(data[1]);
                                    break;
                                }
                            case "tour":
                                {
                                    // only registered tournaments (indexing an unknown type threw KeyNotFoundException)
                                    ushort tourId;
                                    Game.MsgTournaments.ITournament tour;
                                    if (data.Length > 1 && ushort.TryParse(data[1], out tourId)
                                        && Game.MsgTournaments.MsgSchedules.Tournaments.TryGetValue((MsgTournaments.TournamentType)tourId, out tour))
                                    {
                                        Game.MsgTournaments.MsgSchedules.CurrentTournament = tour;
                                        tour.Open();
                                    }
                                    else
                                        client.SendSysMesage("This tournament is not available.");
                                    break;
                                }
                            case "hit":
                                {
                                    client.Player.HitPoints = ushort.Parse(data[1]);
                                    client.Player.SendUpdateHP();
                                    break;





                                }
                            case "statue":
                                {

                                    Role.Statue.CreateStatue(client, client.Player.X, client.Player.Y, (int)client.Player.Action, 0, true);
                                    break;
                                }
                            case "reward":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Player.DailySignUpDays |= (1ul << byte.Parse(data[1]));

                                    }
                                    break;
                                }
                            case "crac":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Player.AddSpellFlag(MsgUpdate.Flags.FireArrow, 200, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.FireArrow, 200, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                                        client.Player.AddSpellFlag(MsgUpdate.Flags.PoisonArrow, 200, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.PoisonArrow, 200, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                                        client.Player.AddSpellFlag(MsgUpdate.Flags.WindArrow, 200, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.WindArrow, 200, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                                        client.Player.AddSpellFlag(MsgUpdate.Flags.IceArrow, 200, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.IceArrow, 200, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                                        client.Player.AddSpellFlag(MsgUpdate.Flags.ThunderArrow, 200, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.ThunderArrow, 200, 0, 0, MsgUpdate.DataType.ArchiveSkill);

                                    }
                                    break;
                                }
                            case "teffect":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.MsgRefineEffectCreate(new MsgRefineEffect.RefineEffectProto()
                                        {
                                            Effect = (MsgRefineEffect.RefineEffects)uint.Parse(data[1]),
                                            Id = client.Player.UID,
                                            dwParam = (uint)client.Player.UID//(int)uint.Parse(data[2])

                                        }));
                                    }
                                    break;
                                }
                            case "name":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            Type = 258,
                                            Timestamp = 569129523,
                                            Strings = new string[1] { "Flop_Func_ShowBtn|{['ShowPoint']=100,['IsShowBtn']=100}" },

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }
                                    break;
                                }
                            case "New3":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery info = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            dwParam = 382,
                                            Timestamp = 246992597,
                                            NpcID = 23561,
                                            Type = ActionType.OpenDialog,
                                            Fascing = 7,
                                            PositionX = client.Player.X,
                                            PositionY = client.Player.Y,
                                        };
                                        client.Send(stream.ActionCreate(info));
                                    }
                                    break;
                                }

                            case "Quest":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Send(stream.MsgQuestDataCreate(65536, 35034, client.Player.UID));
                                    }
                                    break;
                                }
                            case "Test2":
                                {
                                    //using (var rec = new ServerSockets.RecycledPacket())
                                    //{
                                    //    var stream = rec.GetStream();
                                    //    //client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.EffectReborn, client.Player.UID, 255, 0, 0, 0, 0));

                                    //    //client.Send(stream.CreateDialogItem(8, 272, 52504, 49610, 987, "STR_ID_tPirateEpicTask[21427][ExchangeBottle][ImmediaUse]@@"));
                                    //}
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        packet.InitWriter();

                                        packet.Write((ushort)8);
                                        packet.Write((byte)16);
                                        packet.Write((byte)1);
                                        packet.Write((byte)24);
                                        packet.Write((byte)205);
                                        packet.Write((byte)202);
                                        packet.Write((byte)193);
                                        packet.Write((byte)219);
                                        packet.Write((byte)3);
                                        packet.Write((byte)34);
                                        packet.Write((byte)59);
                                        packet.Write("STR_ID_tPirateEpicTask[21427][ExchangeBottle][ImmediaUse]@@", 58);


                                        packet.Finalize(2181);
                                        client.Send(packet);
                                    }
                                    break;
                                }

                            case "Testhh":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Inventory.AddAnimaLock(packet, 4200015, Role.Core.CreateTimer(DateTime.Now.AddDays(5)), 2);
                                    }
                                    break;
                                }

                            case "TestP":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = rec.GetStream();
                                        client.Send(
                                             streamm.CreateHairface(
                                             new MsgHairfaceStorage.MsgHairfaceStorageProto()
                                             {
                                                 Type = MsgHairfaceStorage.Actions.Equip,
                                                 Hair = new MsgHairfaceStorage.Operation() { ID = 1, dwParam = uint.Parse(data[1]) }
                                             }));
                                    }
                                    break;
                                }
                            case "TestPi":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = rec.GetStream();
                                        client.Send(new byte[] { 0x4B, 0x00, 0x85, 0x08, 0x08, 0x00, 0x10, 0x01, 0x18, 0xCD, 0xCA, 0xC1, 0xDB, 0x03, 0x22, 0x3B });
                                    }
                                    break;
                                }
                            case "Prom":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            Type = ActionType.OpenDialog,
                                            ObjId = client.Player.UID,
                                            dwParam = 975,
                                            Fascing = 7,
                                            PositionX = client.Player.X,
                                            PositionY = client.Player.Y,

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }
                                    break;
                                }
                            case "New":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            Timestamp = 294735873,
                                            Type = 258,
                                            Strings = new string[1] { "</F>UiBackpackLetter_OpenUi</N>" },

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }
                                    break;
                                }
                            case "incquest":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ushort.Parse(data[1]), ushort.Parse(data[2]), ushort.Parse(data[3]), ushort.Parse(data[4]));
                                    }
                                    break;
                                }
                            case "accquest":
                                {

                                    Database.QuestInfo.DBQuest gmQuest;// [QuestFix 2026-10-13] unknown id no longer throws
                                    if (Database.QuestInfo.AllQuests.TryGetValue(uint.Parse(data[1]), out gmQuest))
                                        client.Player.QuestGUI.Accept(gmQuest, 0);
                                    break;
                                }
                            case "remquest":
                                {
                                    client.Player.QuestGUI.AcceptedQuests.Remove(uint.Parse(data[1]));
                                    client.Player.QuestGUI.src.Remove(uint.Parse(data[1]));
                                    break;
                                }
                            case "finishquest":
                                {
                                    client.Player.QuestGUI.FinishQuest(uint.Parse(data[1]));
                                    break;
                                }
                            case "rr":
                                {
                                    client.Player.TCCaptainTimes = 0;
                                    break;
                                }
                            case "trans":
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();


                                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                                        client.Player.TransformInfo.CreateTransform(stream, 817, ushort.Parse(data[1]), (int)ushort.MaxValue - 1, 8213);
                                    }


                                    break;
                                }
                            case "pick":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddPick(stream, "GameServer[EU]", 5);
                                    }
                                    break;

                                }
                            case "Party":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var map = Pool.ServerMaps[client.Player.Map];
                                        string ItemName = data[1];
                                        uint Amount = 0;
                                        uint SpecialID = 0;
                                        switch (data[1])
                                        {
                                            case "cp":
                                                {
                                                    SpecialID = 729911;
                                                    #region Cps
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            if (SpecialID == 729911)
                                                            {
                                                                Amount = 200;
                                                            }
                                                            if (SpecialID == 729912)
                                                            {
                                                                Amount = 400;
                                                            }
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Cps, Amount, 0, 1002, client.Player.UID, false, map);

                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            case "SoulScrap1":
                                                {
                                                    SpecialID = 3004243;
                                                    #region Soul
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, false, map);

                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            case "SoulScrap2":
                                                {
                                                    SpecialID = 3004244;
                                                    #region Soul
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, false, map);

                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            case "db":
                                                {
                                                    SpecialID = 1088000;
                                                    #region Db
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, true, map);

                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            case "top":
                                                {
                                                    SpecialID = 723723;
                                                    #region top
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, 500000000, client.Player.DynamicID, client.Player.Map, client.Player.UID, true, map);
                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            case "sc":
                                                {
                                                    SpecialID = 720028;
                                                    #region top
                                                    for (int x = 0; x < int.Parse(data[2]); x++)
                                                    {
                                                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                                        DataItem.ITEM_ID = SpecialID;
                                                        Database.ItemType.DBItem DBItem;
                                                        if (Pool.ItemsBase.TryGetValue(SpecialID, out DBItem))
                                                        {
                                                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                                        }
                                                        DataItem.Color = Role.Flags.Color.Red;
                                                        ushort xx = (ushort)Program.GetRandom.Next(client.Player.X - 20, client.Player.X + 20);
                                                        ushort yy = (ushort)Program.GetRandom.Next(client.Player.Y - 20, client.Player.Y + 20);
                                                        if (client.Map.AddGroundItem(ref xx, ref yy, 3))
                                                        {
                                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, true, map);

                                                            if (client.Map.EnqueueItem(DropItem))
                                                            {
                                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            //case "Test":
                            //    {
                            //        using (var rec = new ServerSockets.RecycledPacket())
                            //        {
                            //            var stream = rec.GetStream();
                            //            ActionQuery action = new ActionQuery()
                            //            {
                            //                ObjId = client.Player.UID,
                            //                Type = 456, //456
                            //                Fascing = 0,
                            //                PositionX = client.Player.X,
                            //                PositionY = client.Player.Y,
                            //                dwParam3 = ulong.Parse(data[1]),

                            //            };
                            //            client.Send(stream.ActionCreate(action));
                            //        }
                            //        break;
                            //    }
                            case "Archer":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            dwParam3 = ushort.Parse(data[1]),//158 //186
                                            Type = 456, //137
                                            PositionX = client.Player.X,
                                            PositionY = client.Player.Y,

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }
                                    break;
                                }
                            case "Look":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        ActionQuery action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            dwParam3 = 2700,
                                            dwParam = 3332581,
                                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),
                                            Type = 462,
                                            PositionX = 1,
                                            PositionY = 1,

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }
                                    break;
                                }
                            case "Color":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                        effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                        effect.DropType = MsgDropID.Earth;
                                        effect.m_X = client.Player.X;
                                        effect.m_Y = client.Player.Y;
                                        client.Send(stream.ItemPacketCreate(effect));
                                    }

                                    break;
                                }
                            case "addnpc":
                                {


                                    Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                                    np.UID = (uint)Pool.GetRandom.Next(10000, 100000);
                                    np.NpcType = (Role.Flags.NpcType)byte.Parse(data[1]);
                                    np.Mesh = ushort.Parse(data[2]);
                                    np.Map = client.Player.Map;
                                    np.X = client.Player.X;
                                    np.Y = client.Player.Y;

                                    client.Map.AddNpc(np);
                                    break;
                                }


                            case "Testss":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddSpellFlag(MsgUpdate.Flags.ActiveWeapon, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.ActiveWeapon, 3, 3, (uint)4200000, MsgUpdate.DataType.ArchiveSkill, true);
                                    }
                                    break;
                                }
                            case "body":
                                {
                                    client.Player.Body = ushort.Parse(data[1]);
                                    break;

                                }
                            case "itemeffect":
                                {
                                    MsgGameItem item;
                                    if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out item))
                                    {
                                        item.Effect = (Role.Flags.ItemEffect)ushort.Parse(data[1]);
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            item.Send(client, stream);
                                        }
                                    }
                                    break;

                                }
                            case "itemm2":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.AddItemWitchStack(ID, 0, 10000, rec.GetStream(), false);

                                    break;
                                }
                            case "dura":
                                {
                                    MsgServer.MsgGameItem GameItem;
                                    if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)byte.Parse(data[1]), out GameItem))
                                    {
                                        GameItem.Durability = GameItem.MaximDurability = ushort.Parse(data[2]);

                                        GameItem.Mode = Role.Flags.ItemMode.Update;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            GameItem.Send(client, stream);
                                        }
                                    }
                                    break;
                                }
                            case "activitypoints":
                                {
                                    client.Activeness.ActivityPoints = uint.Parse(data[1]);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        // client.Activeness.UpdateTasksList(stream);
                                        client.Activeness.UpdateActivityPoints(stream);

                                    }
                                    break;
                                }
                            case "testct":
                                {
                                    uint Count = ushort.Parse(data[1]);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        stream.CaptureTheFlagRankingsCreate((MsgCaptureTheFlagRankings.ActionID)9, 0, 2, Count, 4, 5);
                                        for (int x = 0; x < Count; x++)
                                        {

                                            stream.AddItemCaptureTheFlagRankings(1, 1, "basta" + x.ToString(), (uint)(1));
                                        }
                                        stream.CaptureTheFlagRankingsFinalize();
                                        client.Send(stream);
                                    }

                                    break;
                                }
                            case "realbp":
                                {
                                    client.SendSysMesage("You real BatterPower is = " + client.Player.RealBattlePower + "");
                                    break;
                                }
                            case "bp":
                                {
                                    client.SendSysMesage("You BatterPower is = " + client.Player.BattlePower + "");
                                    break;
                                }
                            case "superman":
                                {
                                    client.Player.Vitality += 500;
                                    client.Player.Strength += 500;
                                    client.Player.Spirit += 500;
                                    client.Player.Agility += 500;

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);

                                    }
                                    break;
                                }
                            case "restartdonations":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                        nobility = 0;
                                        ini.Write<ulong>("Character", "DonationNobility", nobility);
                                    }

                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.Nobility.Donation = 0;

                                            user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                            Pool.NobilityRanking.UpdateRank(user.Player.Nobility);
                                        }
                                    }



                                    break;
                                }
                            case "restartcps":
                            {
                                if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                const uint ID = 3315881;

                                // إلغاء العنصر من جميع اللاعبين غير المتصلين (ملفات الـ INI)
                                WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                {
                                    ini.FileName = fname;
                                    // نفترض أن عدد العنصر محفوظ في القسم "Item" بالمفتاح ID
                                    ini.Write<ulong>("Item", ID.ToString(), 0);
                                }

                                // إلغاء العنصر من اللاعبين المتصلين
                                foreach (var pl in Pool.GamePoll.Values)
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        pl.Inventory.Remove(ID, int.MaxValue, rec.GetStream());
                                    }
                                }

                                client.SendSysMesage($"تم مسح العنصر {ID} من جميع اللاعبين (متصلين وغير متصلين)."); // [GmSec 2026-10-14] was missing $
                                break;
                            }
                            case "freeze":
                            {
                                // Usage: @freeze <playerName>
                                if (data.Length < 2)
                                {
                                    client.SendSysMesage("Usage: @freeze <playerName>");
                                    break;
                                }
                                string targetName = data[1];
                                int duration = 86400; // 1 day in seconds
                                var target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
                                if (target != null)
                                {
                                    target.Player.AddFlag(MsgUpdate.Flags.Freeze, duration, true);
                                    client.SendSysMesage($"Player {targetName} frozen for 1 day."); // [GmSec 2026-10-14] was missing $
                                }
                                else
                                {
                                    client.SendSysMesage($"Player '{targetName}' not found or offline."); // [GmSec 2026-10-14] was missing $
                                }
                                break;
                            }
                            case "unfreeze":
                            {
                                // Usage: @unfreeze <playerName>
                                if (data.Length < 2)
                                {
                                    client.SendSysMesage("Usage: @unfreeze <playerName>");
                                    break;
                                }
                                string targetName = data[1];
                                var target = Pool.GamePoll.Values.FirstOrDefault(p => p.Player.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
                                if (target != null)
                                {
                                    target.Player.RemoveFlag(MsgUpdate.Flags.Freeze);
                                    client.SendSysMesage($"Player {targetName} unfrozen."); // [GmSec 2026-10-14] was missing $
                                }
                                else
                                {
                                    client.SendSysMesage($"Player '{targetName}' not found or offline."); // [GmSec 2026-10-14] was missing $
                                }
                                break;
                            }
                            case "restartallcps":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ulong ConquerPoints = ini.ReadUInt64("Character", "ConquerPoints", 0);
                                        ConquerPoints = 0;
                                        ini.Write<ulong>("Character", "ConquerPoints", ConquerPoints);
                                    }

                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.ConquerPoints = 0;
                                            user.Player.SendUpdate(stream, user.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                                        }
                                    }



                                    break;
                                }
                            case "restartallmoney":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ulong Money = ini.ReadUInt64("Character", "Money", 0);
                                        Money = 0;
                                        ini.Write<ulong>("Character", "Money", Money);
                                    }

                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.Money = 0;
                                            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                        }
                                    }



                                    break;
                                }
                            case "restartallcpsbank":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ulong CpsBank = ini.ReadUInt64("Character", "CpsBank", 0);
                                        CpsBank = 0;
                                        ini.Write<ulong>("Character", "CpsBank", CpsBank);
                                    }

                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.CpsBank = 0;

                                        }
                                    }



                                    break;
                                }
                            case "restartallMoneyBank":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ulong WHMoney = ini.ReadUInt64("Character", "WHMoney", 0);
                                        WHMoney = 0;
                                        ini.Write<ulong>("Character", "WHMoney", WHMoney);
                                    }

                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            user.Player.WHMoney = 0;
                                            user.Player.SendUpdate(stream, user.Player.WHMoney, MsgUpdate.DataType.WHMoney);
                                        }
                                    }



                                    break;
                                }
                            case "resetstats":
                                {
                                    client.Player.Vitality = 0;
                                    client.Player.Strength = 0;
                                    client.Player.Spirit = 0;
                                    client.Player.Agility = 0;

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);

                                    }
                                    break;
                                }

                            case "bestf":
                                {
                                    Game.MsgTournaments.ITournament bestFight;
                                    if (MsgTournaments.MsgSchedules.Tournaments.TryGetValue(MsgTournaments.TournamentType.BestFight, out bestFight))
                                        bestFight.Open();
                                    else
                                        client.SendSysMesage("This tournament is not available.");
                                    break;
                                }
                            case "StartClassPKWar": // شفرة تشغيل الكلاس بيكي وار
                                {
                                    Game.MsgTournaments.MsgSchedules.ClassPkWar.Start();
                                    break;
                                }
                            case "StartSkillTeamPK": // شفرة تشغيل الاسكيل تيم
                                {
                                    Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Start();
                                    break;
                                }
                            case "StartTeamPK": // شفرة تشغيل التيم بيكي
                                {
                                    Game.MsgTournaments.MsgSchedules.TeamPkTournament.Start();
                                    break;
                                }
                            case "StartElitePK": // شفرة تشغيل الاليت بيكي
                                {
                                    Game.MsgTournaments.MsgSchedules.ElitePkTournament.Start();
                                    break;
                                }
                            case "StartCaptureTheFlag": // شفرة تشغيل الكبشر فلاج
                                {
                                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Start();
                                    break;
                                }
                            case "FinishCaptureTheFlag": // شفرة انهاء الكبشر فلاج
                                {
                                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.CheckFinish();
                                    break;
                                }
                            case "StartSuperGuildWar": // شفرة تشغيل السوبر جيلد وار
                                {
                                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                                    break;
                                }
                            case "FinishSuperGuildWar": // شفرة انهاء السوبر جيلد وار
                                {
                                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                                    Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                                    break;
                                }
                            case "StartGuildWar": // شفرة تشغيل الجيلد وار
                                {
                                    Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Alive;
                                    Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                                    break;
                                }
                            case "FinishGuildWar": // شفرة انهاء الجيلد وار
                                {
                                    Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                                    Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                                    break;
                                }
                            case "battlefieldon":
                                {
                                    MsgTournaments.MsgSchedules.Tournaments[MsgTournaments.TournamentType.BattleField].Open();
                                    break;
                                }
                            case "flags":
                                {
                                    client.Player.ClearFlags();
                                    break;
                                }
                            case "SendUpdate":
                                {
                                    using (var rect = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rect.GetStream();
                                        client.Player.View.SendView(client.Player.GetArray(stream, false), true);
                                    }

                                    break;
                                }
                            case "addmem":
                                {
                                    for (int x = 0; x < ushort.Parse(data[1]); x++)
                                    {
                                        client.Player.MyGuild.Members.TryAdd((uint)(client.Player.UID + x + 1000)
                                            , new Role.Instance.Guild.Member() { Name = "test " + x.ToString() + " ", Class = 15, Level = (byte)x, IsOnline = true, UID = (uint)(client.Player.UID + x + 1000) });
                                    }
                                    break;
                                }

                            case "party":
                                {

                                    break;
                                }

                            case "test1":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.StatusFlag);
                                    }
                                    break;
                                }
                            case "testxxx":
                                {
                                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.lianhuaran04, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                    break;
                                }
                            #region jail
                            case "givebot":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (data[1].ToLower() == "me" ? user == client : user.Player.Name.ToLower() == data[1].ToLower()) // [GmSec 2026-10-14] "me" = the GM who typed it
                                        {

                                            switch (data[2])
                                            {
                                                case "botjail":
                                                    {
                                                        user.Teleport(299, 279, 1002); // [GmSec 2026-10-14] was client (the GM)
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                            #endregion
                                }
                            case "give":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (data[1].ToLower() == "me" ? user == client : user.Player.Name.ToLower() == data[1].ToLower()) // [GmSec 2026-10-14] "me" = the GM who typed it
                                        {

                                            switch (data[2])
                                            {
                                                case "givenobil":
                                                    {
                                                        if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                                        try
                                                        {
                                                            ///Duke = 7,
                                                            ///Prince = 9,
                                                            ///King = 12
                                                            ///@give [ClientName] givenobil [Days] [NobleValue]
                                                            user.Player.PayNobilitySystem.Paid(int.Parse(data[3]), (Role.Instance.Nobility.NobilityRank)byte.Parse(data[4]));
                                                            user.Socket.Disconnect();
                                                        }
                                                        catch
                                                        {
                                                            MyConsole.WriteLine("Syntax Error In Paid Nobility System.", ConsoleColor.DarkRed);
                                                        }
                                                        break;
                                                    }
                                                case "test2":
                                                    {
                                                        var map = Pool.ServerMaps[99101];
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            Server.AddMapMonster(stream, map, 3970, client.Player.X, client.Player.Y, 18, 18, 1);

                                                        }

                                                        break;
                                                    }

                                                case "clear":
                                                    {
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                            user.Inventory.Clear(rec.GetStream()); // [GmSec 2026-10-14] was client (cleared the GM)
                                                        break;
                                                    }
                                                case "innerpotency":
                                                    {
                                                        int amount = 0;
                                                        if (int.TryParse(data[3], out amount))
                                                        {
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Player.InnerPower.AddPotency(stream, user, amount);
                                                                user.CreateBoxDialog("You receive " + amount + " InnerPower Potency.");
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "level":
                                                    {
                                                        byte amount = 0;
                                                        if (byte.TryParse(data[3], out amount))
                                                        {
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.UpdateLevel(stream, amount, true);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "money":
                                                    {
                                                        user.Player.Money += ulong.Parse(data[3]);
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                                        }
                                                        break;
                                                    }

                                                case "cps":
                                                    {
                                                        user.Player.ConquerPoints += uint.Parse(data[3]);

                                                        break;
                                                    }

                                                case "archive":
                                                    {
                                                        if (user.HundredWeapons.Valid())
                                                        {
                                                            user.HundredWeapons.Unlocked = true;
                                                            user.HundredWeapons.AddWeapon((Database.MagicType.WeaponsType)ushort.Parse(data[3]), byte.Parse(data[4]), false);
                                                            if (byte.Parse(data[4]) == 9)
                                                            {
                                                                foreach (var weapon in user.HundredWeapons.Objects.Values)
                                                                {
                                                                    var atts = weapon.Attributes.ToArray();
                                                                    foreach (var att in atts)
                                                                        weapon.Attributes[att.Key] = weapon.DBInfo.Attributes[att.Key] * 2;
                                                                }
                                                            }
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Send(stream.CreateHundredWeaponsInfo(user, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "maxarchives":
                                                    {
                                                        if (user.HundredWeapons.Valid())
                                                        {
                                                            user.HundredWeapons.Unlocked = true;
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Blade, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Sword, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hook, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Whip, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Club, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hammer, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Axe, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Dagger, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Scepter, 9, false);
                                                            foreach (var weapon in user.HundredWeapons.Objects.Values)
                                                            {
                                                                var atts = weapon.Attributes.ToArray();
                                                                foreach (var att in atts)
                                                                    weapon.Attributes[att.Key] = weapon.DBInfo.Attributes[att.Key] * 2;
                                                            }
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Send(stream.CreateHundredWeaponsInfo(user, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "reborns":
                                                    {
                                                        user.Player.Reborn = byte.Parse(data[3]);

                                                        break;
                                                    }

                                                case "item":
                                                    {
                                                        uint ID = 0;
                                                        if (!uint.TryParse(data[3], out ID))
                                                        {
                                                            client.SendSysMesage("Invlid item ID !");
                                                            break;
                                                        }
                                                        byte plus = 0;
                                                        if (!byte.TryParse(data[4], out plus))
                                                        {
                                                            client.SendSysMesage("Invlid item plus !");
                                                            break;
                                                        }
                                                        byte bless = 0;
                                                        if (!byte.TryParse(data[5], out bless))
                                                        {
                                                            client.SendSysMesage("Invlid item Enchant !");
                                                            break;
                                                        }
                                                        byte enchant = 0;
                                                        if (!byte.TryParse(data[6], out enchant))
                                                        {
                                                            client.SendSysMesage("Invlid item Enchant !");
                                                            break;
                                                        }
                                                        byte sockone = 0;
                                                        if (!byte.TryParse(data[7], out sockone))
                                                        {
                                                            client.SendSysMesage("Invlid item Socket One !");
                                                            break;
                                                        }
                                                        byte socktwo = 0;
                                                        if (!byte.TryParse(data[8], out socktwo))
                                                        {
                                                            client.SendSysMesage("Invlid item Socket Two !");
                                                            break;
                                                        }
                                                        byte count = 1;
                                                        if (data.Length > 9)
                                                        {
                                                            if (!byte.TryParse(data[9], out count))
                                                            {
                                                                client.SendSysMesage("Invlid item count !");
                                                                break;
                                                            }
                                                        }
                                                        byte Effect = 0;
                                                        if (data.Length > 10)
                                                        {
                                                            if (!byte.TryParse(data[10], out Effect))
                                                            {
                                                                client.SendSysMesage("Invlid Effect Type !");
                                                                break;
                                                            }
                                                        }
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                            user.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect);

                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            #region Nobility
                            case "give1":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (data[1].ToLower() == "me" ? user == client : user.Player.Name.ToLower() == data[1].ToLower()) // [GmSec 2026-10-14] "me" = the GM who typed it
                                        {

                                            switch (data[2])
                                            {

                                                case "NobleDonate50BB":
                                                    {
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            user.Player.Nobility.Donation += 50000000000;
                                                            user.Send(stream.NobilityIconCreate(user.Player.Nobility));
                                                            Pool.NobilityRanking.UpdateRank(user.Player.Nobility);
                                                        }
                                                        break;
                                                    }
                                                case "vip":
                                                    {
                                                        byte level = byte.Parse(data[3]);
                                                        int time = int.Parse(data[4]);
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            if (DateTime.Now > client.Player.ExpireVip)
                                                                user.Player.ExpireVip = DateTime.Now.AddDays(time);
                                                            else
                                                                user.Player.ExpireVip = client.Player.ExpireVip.AddDays(time);

                                                            user.Player.VipLevel = level;

                                                            user.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                                            user.Player.UpdateVip(stream);

                                                            user.CreateBoxDialog("You`ve received vip" + level + " (" + time + " Day).");
                                                            client.Player.MessageBox(data[1] + " Has added vip.", null, null);

                                                        }

                                                        break;
                                                    }
                                                case "innerpotency":
                                                    {
                                                        int amount = 0;
                                                        if (int.TryParse(data[3], out amount))
                                                        {
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Player.InnerPower.AddPotency(stream, user, amount);
                                                                user.CreateBoxDialog("You receive " + amount + " InnerPower Potency.");
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "level":
                                                    {
                                                        byte amount = 0;
                                                        if (byte.TryParse(data[3], out amount))
                                                        {
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.UpdateLevel(stream, amount, true);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "money":
                                                    {
                                                        user.Player.Money += ulong.Parse(data[3]);
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                        {
                                                            var stream = rec.GetStream();
                                                            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                                        }
                                                        break;
                                                    }
                                                case "cps":
                                                    {
                                                        user.Player.ConquerPoints += uint.Parse(data[3]);

                                                        break;

                                                    }

                                                #region Ninja
                                                case "ninja_sigil":
                                                    {
                                                        try
                                                        {
                                                            foreach (var i in NinjaFile.gouyu_type.Values)
                                                            {
                                                                if (i.Level == 9)
                                                                {
                                                                    client.MyNinja.AddItem(i.ItemID, 1, 0, 0, 0);
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex) { Console.WriteLine(ex); }
                                                        break;
                                                    }
                                                case "open_ninja":
                                                    {
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock_2);
                                                        client.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death_2);
                                                        break;

                                                    }
                                                case "get_ninja_items":
                                                    {
                                                        for (int i = 3330028; i <= 3330062; i++)
                                                        {
                                                            if (client.Inventory.HaveSpace(34))
                                                                using (var rec = new ServerSockets.RecycledPacket())
                                                                    client.Inventory.Add(rec.GetStream(), (uint)i, 0, 1);
                                                        }
                                                        break;
                                                    }
                                                case "ninja_mode":
                                                    {
                                                        client.MyNinja.SageMode(uint.Parse(data[1]));
                                                        break;
                                                    }
                                                #endregion
                                                #region Trojan
                                                case "archive":
                                                    {
                                                        if (user.HundredWeapons.Valid())
                                                        {
                                                            user.HundredWeapons.Unlocked = true;
                                                            user.HundredWeapons.AddWeapon((Database.MagicType.WeaponsType)ushort.Parse(data[3]), byte.Parse(data[4]), false);
                                                            if (byte.Parse(data[4]) == 9)
                                                            {

                                                            }
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Send(stream.CreateHundredWeaponsInfo(user, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                                            }
                                                        }
                                                        break;
                                                    }
                                                case "maxarchives":
                                                    {
                                                        if (user.HundredWeapons.Valid())
                                                        {
                                                            user.HundredWeapons.Unlocked = true;
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Blade, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Sword, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hook, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Whip, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Club, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hammer, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Axe, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Dagger, 9, false);
                                                            user.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Scepter, 9, false);
                                                            foreach (var weapon in user.HundredWeapons.Objects.Values)
                                                            {
                                                                var atts = weapon.Attributes.ToArray();
                                                                foreach (var att in atts)
                                                                    weapon.Attributes[att.Key] += weapon.DBInfo.Attributes[att.Key] * 20;
                                                            }
                                                            using (var rec = new ServerSockets.RecycledPacket())
                                                            {
                                                                var stream = rec.GetStream();
                                                                user.Send(stream.CreateHundredWeaponsInfo(user, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                                            }
                                                            user.HundredWeapons.LoadPowerFoucs(user);
                                                        }
                                                        break;
                                                #endregion
                                                    }
                                                case "reborns":
                                                    {
                                                        user.Player.Reborn = byte.Parse(data[3]);

                                                        break;
                                                    }

                                                case "item":
                                                    {
                                                        uint ID = 0;
                                                        if (!uint.TryParse(data[3], out ID))
                                                        {
                                                            client.SendSysMesage("Invlid item ID !");
                                                            break;
                                                        }
                                                        byte plus = 0;
                                                        if (!byte.TryParse(data[4], out plus))
                                                        {
                                                            client.SendSysMesage("Invlid item plus !");
                                                            break;
                                                        }
                                                        byte bless = 0;
                                                        if (!byte.TryParse(data[5], out bless))
                                                        {
                                                            client.SendSysMesage("Invlid item Enchant !");
                                                            break;
                                                        }
                                                        byte enchant = 0;
                                                        if (!byte.TryParse(data[6], out enchant))
                                                        {
                                                            client.SendSysMesage("Invlid item Enchant !");
                                                            break;
                                                        }
                                                        byte sockone = 0;
                                                        if (!byte.TryParse(data[7], out sockone))
                                                        {
                                                            client.SendSysMesage("Invlid item Socket One !");
                                                            break;
                                                        }
                                                        byte socktwo = 0;
                                                        if (!byte.TryParse(data[8], out socktwo))
                                                        {
                                                            client.SendSysMesage("Invlid item Socket Two !");
                                                            break;
                                                        }
                                                        byte count = 1;
                                                        if (data.Length > 9)
                                                        {
                                                            if (!byte.TryParse(data[9], out count))
                                                            {
                                                                client.SendSysMesage("Invlid item count !");
                                                                break;
                                                            }
                                                        }
                                                        byte Effect = 0;
                                                        if (data.Length > 10)
                                                        {
                                                            if (!byte.TryParse(data[10], out Effect))
                                                            {
                                                                client.SendSysMesage("Invlid Effect Type !");
                                                                break;
                                                            }
                                                        }
                                                        using (var rec = new ServerSockets.RecycledPacket())
                                                            user.Inventory.Add(rec.GetStream(), ID, count, plus, bless, enchant, (Role.Flags.Gem)sockone, (Role.Flags.Gem)socktwo, false, (Role.Flags.ItemEffect)Effect);

                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "unbanstr":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                    Database.SystemBannedAccount.RemoveBan(data[1]);
                                    break;
                                }
                            case "unbanuid":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; } // [GmSec 2026-10-14] owner only
                                    Database.SystemBannedAccount.RemoveBan(uint.Parse(data[1]));
                                    break;
                                }
                            case "ban":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {

                                            Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, uint.Parse(data[2]));
                                            user.SendSysMesage("You Account was Banned by [PM]/[GM].", ChatMode.System, MsgColor.white);
                                            user.Socket.Disconnect();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "banip":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {
                                            Database.SystemBanned.AddBan(user.Socket.RemoteIp, uint.Parse(data[2]));
                                            user.SendSysMesage("You Ip Address was Banned by [PM]/[GM].", ChatMode.System, MsgColor.white);
                                            user.Socket.Disconnect();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "con":
                                {
                                    client.Status.Counteraction += 1000;
                                    break;


                                }
                            case "ArenaPoints":
                                {
                                    client.ArenaStatistic.Info.ArenaPoints += 200;
                                    break;


                                }
                            case "exploits":
                                {
                                    client.Player.KingDomExploits = uint.Parse(data[1]);
                                    break;
                                }
                            case "kick":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {
                                            user.Socket.Disconnect();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "revive":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.Revive(rec.GetStream());

                                    break;
                                }
                            case "rev":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.Revive(rec.GetStream());

                                    break;
                                }
                            case "online":
                                {
                                    client.SendSysMesage("Online Players : " + Pool.GamePoll.Count + " ", ChatMode.System);
                                    client.SendSysMesage("Online Players : " + Pool.GamePoll.Count + " ");
                                    break;
                                }
                            case "teeee":
                                {
                                    client.Player.MyJiangHu.GetReward(client
                                        );
                                    break;
                                }
                            case "vip":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {

                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddDays(30);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddDays(30);

                                            user.Player.VipLevel = (byte)uint.Parse(data[2]);
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                user.Player.SendUpdate(stream, user.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                                user.Player.UpdateVip(stream);
                                            }
                                            user.CreateBoxDialog("You`ve received vip6 (30 days) . Thank for you donation.");

                                            break;
                                        }
                                    }
                                    break;
                                }
                            case "vip1h":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower() == data[1].ToLower())
                                        {

                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddHours(1);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddHours(1);

                                            user.Player.VipLevel = (byte)uint.Parse(data[2]);
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var stream = rec.GetStream();
                                                user.Player.SendUpdate(stream, user.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                                user.Player.UpdateVip(stream);
                                            }
                                            user.CreateBoxDialog("You`ve received vip6 (1 Hours) . Thank for you donation.");

                                            break;
                                        }
                                    }
                                    break;
                                }

                            case "info":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        foreach (var user in Pool.GamePoll.Values)
                                        {
                                            if (user.Player.Name.ToLower() == data[1].ToLower())
                                            {

                                                client.Send(new MsgMessage("[Info" + user.Player.Name + "]", MsgColor.yellow, ChatMode.FirstRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("UID = " + user.Player.UID + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("IP = hidden " /* [GmSec 2026-10-14] */, MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("PassWord[Bank] = hidden " /* [GmSec 2026-10-14] */, MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("DonationMoney = " + user.Player.DonatePoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("RewardPoints = " + user.Player.RewardPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("ConquerPoints = " + user.Player.ConquerPoints + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("Money = " + user.Player.Money + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("Map = " + user.Player.Map + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("X = " + user.Player.X + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("Y = " + user.Player.Y + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("BattlePower = " + user.Player.BattlePower + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));


                                                client.Send(new MsgMessage("ClassExperience = " + user.Player.ClassExperience + " ", MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));
                                                client.Send(new MsgMessage("PinCodeAnima = hidden " /* [GmSec 2026-10-14] */, MsgColor.yellow, ChatMode.ContinueRightCorner).GetArray(stream));

                                                break;
                                            }
                                        }
                                    }
                                    break;

                                }
                            case "trace":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user.Player.Name.ToLower().Contains(data[1].ToLower()))
                                        {
                                            client.Teleport(user.Player.X, user.Player.Y, user.Player.Map, user.Player.DynamicID);
                                            break;
                                        }
                                    }

                                    break;
                                }
                            case "scroll":
                                {
                                    switch (data[1].ToLower())
                                    {
                                        case "tc": client.Teleport(410, 354, 1002); break;
                                        case "pc": client.Teleport(195, 260, 1011); break;
                                        case "ac":
                                        case "am": client.Teleport(566, 563, 1020); break;
                                        case "dc": client.Teleport(500, 645, 1000); break;
                                        case "bi": client.Teleport(723, 573, 1015); break;
                                        case "pka": client.Teleport(050, 050, 1005); break;
                                        case "ma": client.Teleport(211, 196, 1036); break;
                                        case "ja": client.Teleport(100, 100, 6000); break;
                                    }
                                    break;
                                }
                            case "Test11":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Send(new MsgMessage(Program.ServerConfig.ServerName, client.Player.Name, "<" + 100000000 + "@@>@@FF03FA01 </F>NpcPosition_PathFind</N>6297@@", 3, 0, data[1]).GetArray(stream));
                                    }
                                    break;
                                }
                            case "bring":
                                {
                                    foreach (var user in Pool.GamePoll.Values)
                                    {
                                        if (user == null) continue;
                                        if (user.Player.Name.ToLower() == data[1].ToLower() || data[1].ToLower() == "all")
                                        {

                                            if (data[1].ToLower() == "all")
                                            {
                                                if (user.Fake)
                                                    continue;
                                                user.Teleport(
                                                    (ushort)Pool.GetRandom.Next(client.Player.X - 5, client.Player.X + 5),
                                                    (ushort)Pool.GetRandom.Next(client.Player.Y - 5, client.Player.Y + 5), client.Player.Map);
                                            }
                                            else
                                            {
                                                user.Teleport(client.Player.X, client.Player.Y, client.Player.Map);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "arenapoints":
                                {
                                    client.ArenaHonorPoints = uint.Parse(data[1]);
                                    break;
                                }
                            case "Dodge":
                                {
                                    client.Status.Dodge = uint.Parse(data[1]);
                                    break;
                                }

                            case "steedpoints":
                                {
                                    client.Player.RacePoints = uint.Parse(data[1]);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.RacePoints, MsgUpdate.DataType.RaceShopPoints);
                                    }
                                    break;
                                }
                            case "staticrole":
                                {
                                    var staticrole = new Role.StaticRole(client.Player.X, client.Player.Y);
                                    staticrole.Map = client.Player.Map;

                                    client.Map.AddStaticRole(staticrole);
                                    break;
                                }
                            case "BotCreat":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        Client.GameClient pclient = new Client.GameClient(null);
                                        pclient.Fake = true;


                                        pclient.Player = new Role.Player(pclient);
                                        pclient.Inventory = new Role.Instance.Inventory(pclient);
                                        pclient.Equipment = new Role.Instance.Equip(pclient);
                                        pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                                        pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                                        pclient.MyVendor = new Role.Instance.Vendor(pclient);
                                        pclient.MySpells = new Role.Instance.Spell(pclient);
                                        pclient.Achievement = new Database.AchievementCollection();

                                        pclient.Player.UID = Pool.ClientCounter.Next;
                                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                                        pclient.Player.HitPoints = int.MaxValue;
                                        pclient.Status.MaxHitpoints = uint.MaxValue;



                                        pclient.Status = new Game.MsgServer.MsgStatus();
                                        pclient.Player.Name = "Bot [" + Pool.ClientCounter.Next.ToString() + "]";
                                        pclient.Player.Body = client.Player.Body;
                                        pclient.Player.InnerPower = new Role.Instance.InnerPower(pclient.Player.Name, pclient.Player.UID);
                                        pclient.HundredWeapons = new Role.Instance.HundredWeapons(pclient);
                                        pclient.Player.SubClass = new Role.Instance.SubClass();
                                        pclient.Player.MyUnion = new Role.Instance.Union();
                                        pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
                                        pclient.Player.MyJiangHu = new Role.Instance.JiangHu(pclient.Player.UID);
                                        pclient.Player.Associate = new Role.Instance.Associate.MyAsociats(pclient.Player.UID);

                                        pclient.Rune = new Role.Instance.Rune(pclient);
                                        pclient.Beasts = new Role.Instance.Beasts(pclient);
                                        pclient.Player.MyClan = new Role.Instance.Clan();
                                        pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante, true);
                                        pclient.Player.X = client.Player.X;
                                        pclient.Player.FirstClass = 4051;
                                        pclient.Player.Class = 4051;
                                        pclient.Player.ExtraBattlePower = 522;
                                        pclient.Player.VipLevel = 6;
                                        pclient.Player.Y = client.Player.Y;
                                        pclient.Player.Map = client.Player.Map;
                                        pclient.Player.Level = 140;
                                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                                        pclient.Player.Face = 153;
                                        pclient.Player.Action = Flags.ConquerAction.None;
                                        pclient.Player.Angle = Role.Flags.ConquerAngle.SouthWest;
                                        pclient.Player.Hair = 774;
                                        pclient.Player.GarmentId = 181305;
                                        pclient.Player.RightWeaponId = 616439;
                                        pclient.Player.HitPoints = 65000;
                                        pclient.Player.CompleteLogin = true;
                                        pclient.FullLoading = true;
                                        pclient.Player.LeftWeaponId = 616439;
                                        pclient.Player.NiniaP0 = 9;
                                        pclient.Player.AddSpellFlag(MsgUpdate.Flags.SageMode, (int)Role.StatusFlagsBigVector32.PermanentFlag, false);
                                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                                        {
                                            var streamm = recycledPacket.GetStream();
                                            pclient.Player.SendUpdate(streamm, MsgUpdate.Flags.SageMode, (uint)Role.StatusFlagsBigVector32.PermanentFlag, 0, 0, MsgUpdate.DataType.ArchiveSkill, true);
                                        }
                                        if (pclient.Player.MyGuild == null && pclient.Player.MyGuildMember == null)
                                        {
                                            Guild Guild;
                                            if (Guild.GuildPoll.TryGetValue(client.Player.GuildID, out Guild))
                                            {
                                                Guild.AddPlayer(pclient.Player, stream);
                                            }
                                        }
                                        pclient.Map = client.Map;
                                        pclient.Map.Enquer(pclient);
                                        client.Send(pclient.Player.GetArray(stream, false));

                                        Pool.GamePoll.TryAdd(pclient.Player.UID, pclient);

                                    }
                                    break;
                                }


                            case "facke":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        for (int i = 0; i < ushort.Parse(data[1]); i++)
                                        {
                                            Client.GameClient pclient = new Client.GameClient(null);
                                            pclient.Fake = true;

                                            pclient.Player = new Role.Player(pclient);
                                            pclient.Inventory = new Role.Instance.Inventory(pclient);
                                            pclient.Equipment = new Role.Instance.Equip(pclient);
                                            pclient.Warehouse = new Role.Instance.Warehouse(pclient);
                                            pclient.MyProfs = new Role.Instance.Proficiency(pclient);
                                            pclient.MySpells = new Role.Instance.Spell(pclient);
                                            pclient.Achievement = new Database.AchievementCollection();
                                            pclient.Status = new MsgStatus();
                                            pclient.Player.Name = "[GM]GameServer[PM]";
                                            pclient.Player.Body = 1008;
                                            pclient.Player.UID = uint.MaxValue;
                                            pclient.Player.HitPoints = ushort.MaxValue;
                                            pclient.Status.MaxHitpoints = ushort.MaxValue;
                                            pclient.Player.InnerPower = new Role.Instance.InnerPower(pclient.Player.Name, pclient.Player.UID);
                                            pclient.HundredWeapons = new Role.Instance.HundredWeapons(pclient);


                                            pclient.Player.SubClass = new Role.Instance.SubClass();
                                            pclient.Player.MyUnion = new Role.Instance.Union();
                                            pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
                                            pclient.Player.MyJiangHu = new Role.Instance.JiangHu(pclient.Player.UID);
                                            pclient.Rune = new Role.Instance.Rune(pclient);
                                            pclient.Player.MyClan = new Role.Instance.Clan();
                                            pclient.Player.X = (ushort)405;
                                            pclient.Player.Y = (ushort)347;
                                            pclient.Player.Map = 1002;
                                            pclient.Player.Level = 255;
                                            pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
                                            pclient.Player.Face = 153;
                                            pclient.Player.Action = Role.Flags.ConquerAction.Sit;
                                            pclient.Player.Angle = Role.Flags.ConquerAngle.SouthWest;
                                            pclient.Player.Hair = 774;
                                            pclient.Player.GarmentId = 193625;
                                            pclient.Player.LeftWeaponAccessoryId = 360047;
                                            pclient.Player.RightWeaponAccessoryId = 360047;
                                            client.Send(pclient.Player.GetArray(stream, false));

                                            pclient.Map = Pool.ServerMaps[1002];
                                            pclient.Map.Enquer(pclient);
                                            Pool.GamePoll.TryAdd(pclient.Player.UID, pclient);
                                        }
                                    }
                                    break;
                                }
                            case "dis":
                                {
                                    Game.MsgTournaments.MsgSchedules.DisCity.Open();
                                    break;
                                }
                            case "Title":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                    }
                                    break;
                                }
                            case "cas":
                                {
                                    client.Player.CastlePoint = uint.Parse(data[1]);
                                    break;
                                }
                            case "Chi99":
                                {
                                    foreach (var user in client.Player.MyChi)
                                    {
                                        user.Exp = 6063000;
                                    }
                                    Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                    Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Send);
                                    break;
                                }
                            case "onlineminutes":
                                {
                                    client.Player.OnlineMinutes = uint.Parse(data[1]) * 60;
                                    break;
                                }
                            case "classexp":
                                {
                                    client.Player.ClassExperience = uint.Parse(data[1]);
                                    break;

                                }
                            case "haire":
                                {
                                    client.Player.Hair = ushort.Parse(data[1]); using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                                    }
                                    break;
                                }
                            case "mapstat":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.MapStatusCreate(client.Map.ID, client.Map.ID, (ulong)(1U << int.Parse(data[1]))));
                                    }
                                    break;
                                }
                            case "pkp":
                                {
                                    client.Player.PKPoints = ushort.Parse(data[1]);
                                    break;
                                }
                            case "ctf":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        for (int x = 0; x < 30; x++)
                                        {
                                            stream.CaptureTheFlagUpdateCreate((MsgCaptureTheFlagUpdate.Mode)x, 3, 1);

                                            stream.AddX2LocationCaptureTheFlagUpdate(326, 447);
                                            stream.CaptureTheFlagUpdateFinalize();
                                            client.Send(stream);
                                        }
                                    }
                                    break;
                                }
                            case "leaguepoints":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Send(stream.CreateGoldLeaguePoint(new MsgGoldLeaguePoint.GoldLeaguePoint()
                                        {
                                            Points = uint.Parse(data[1]),
                                            HistoryPoints = uint.Parse(data[2])
                                        }));
                                    }
                                    break;
                                }
                            case "searchguard":
                                {
                                    foreach (var mob in client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster))
                                    {
                                        if (mob.X == client.Player.X && mob.Y == client.Player.Y)
                                        {
                                            client.SendSysMesage("Location Spawn --> " + (mob as Game.MsgMonster.MonsterRole).LocationSpawn, ChatMode.System, MsgColor.red);
                                        }
                                    }
                                    break;
                                }
                            case "st":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MyDontion = uint.Parse(data[1]);
                                        client.Player.SendUpdate(stream, (long)client.Player.MyDontion, MsgUpdate.DataType.MyDontion);
                                    }
                                    break;
                                }
                            case "SS":
                                {
                                    client.Player.Stage = true;
                                    client.Player.EnergyPointsMystic = 580;
                                    break;
                                }
                            case "B0":
                                {
                                    client.Player.EnergyPoints = 0;
                                    client.Player.EnergyPointsTalent = 0;
                                    client.Player.EnergyPointsAntiwar = 0;
                                    client.Player.EnergyPointsAntifatalism = 0;
                                    client.Player.EnergyPointsMystic = 0;
                                    client.Player.stage = 0;

                                    client.Player.Laststage = 0;
                                    break;
                                }
                            case "ee":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Attack35r");
                                    }
                                    break;
                                }
                            case "robopt":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.CreateLeagueRobOpt(new MsgLeagueRobOpt.RobOpt()
                                        {
                                            Type = uint.Parse(data[1])
                                             ,
                                            Unknown2 = uint.Parse(data[2])
                                             ,
                                            // [ProtoFix 2026-10-14] #3 is now a repeated entry {f1 uint, f2 string}
                                            ID = new List<MsgLeagueRobOpt.RobOptServer> { new MsgLeagueRobOpt.RobOptServer { ID = uint.Parse(data[3]), Name = data[4] } }
                                             ,
                                            Name = data[4],
                                        }));
                                    }

                                    break;
                                }
                            case "gui":
                                {
                                    TestGui = ushort.Parse(data[1]);
                                    break;
                                }
                            case "sound":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Sound, false, "sound/wind.wav", "1");

                                    }
                                    break;
                                }
                            case "sound2":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Sound, false, "sound/fc2.wav", "1");

                                    }
                                    break;

                                }
                            case "ef":
                                {
                                    Game.MsgServer.MsgMovement.eeffect = int.Parse(data[1]);
                                    break;
                                }
                            case "teleback":
                                {
                                    client.TeleportCallBack();
                                    break;
                                }
                            case "map":
                                {
                                    client.SendSysMesage("MapID = " + client.Player.Map, ChatMode.System);
                                    break;
                                }
                            case "studypoints":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.SubClass.AddStudyPoints(client, ushort.Parse(data[1]), rec.GetStream());
                                    break;
                                }
                            case "expball":
                                {
                                    client.GainExpBall(double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture), true, Role.Flags.ExperienceEffect.angelwing);
                                    break;
                                }
                            case "epicr":
                                {
                                    using (var recCmd = new ServerSockets.RecycledPacket()) client.Inventory.Relice(recCmd.GetStream(), false, true, uint.Parse(data[1]), uint.Parse(data[2]), uint.Parse(data[3]), uint.Parse(data[4]), uint.Parse(data[5]));
                                    break;
                                }
                            case "string_effect":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendString(stream, MsgStringPacket.StringID.LocationEffect, true, data[1]);
                                    }
                                    break;
                                }
                            case "string_effect3":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        {
                                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                            packet.ID = (MsgStringPacket.StringID)9;
                                            //packet.X = 345;
                                            //packet.Y = 471;   

                                            //packet.X = 348;
                                            //packet.Y = 481;

                                            packet.X = client.Player.X;
                                            packet.Y = client.Player.Y;
                                            packet.Strings = new string[1] { data[1] }; ;
                                            client.Send(stream.StringPacketCreate(packet));
                                        }
                                    }
                                    break;
                                }
                            case "string_effect2":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, data[1]);
                                    }
                                    break;

                                }
                            case "xp":
                                {
                                    client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                                    using (var recCmd = new ServerSockets.RecycledPacket()) client.Player.SendUpdate(recCmd.GetStream(), 1, Game.MsgServer.MsgUpdate.DataType.XPList);
                                    break;
                                }
                            case "a22":
                                {

                                    using (var recCmd = new ServerSockets.RecycledPacket()) client.Player.SendUpdate(recCmd.GetStream(), 0, Role.StatusFlagsBigVector32.PermanentFlag, uint.Parse(data[1]), (uint)uint.Parse(data[1]), (MsgUpdate.DataType)144, true);
                                    break;
                                }

                            case "Ret":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddFlag((MsgUpdate.Flags)uint.Parse(data[1]), (int)500, true);
                                        client.Player.SendUpdate(stream, (MsgUpdate.Flags)uint.Parse(data[1]), (uint)500, (uint)5, (uint)5, MsgUpdate.DataType.ArchiveSkill);
                                    }
                                    break;
                                }
                            case "newnew":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddFlag(MsgUpdate.Flags.FineRain2, (int)35, true);
                                        client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.FineRain2, 35
                 , 124000, 0, Game.MsgServer.MsgUpdate.DataType.FineRain, true);
                                    }
                                    break;
                                }
                            case "don":
                                {
                                    for (int x = 0; x < 255; x++)
                                    {

                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(client.Player.UID, client.Player.UID, 0, 0, 16540, 1, 0, 1);
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            AnimationObj = new MsgSpellAnimation.SpellObj(client.Player.UID, 0, MsgAttackPacket.AttackEffect.None);
                                            AnimationObj.Damage = (uint)0;
                                            AnimationObj.Effect = (MsgAttackPacket.AttackEffect)x;
                                            AnimationObj.Hit = 1;
                                            AnimationObj.UID = (uint)client.Player.UID;
                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                            MsgSpell.SetStream(stream);
                                            MsgSpell.Send(client);
                                        }
                                    }

                                    break;
                                }
                            case "addflag2":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.AddFlag(MsgUpdate.Flags.NoXp, 10, true);
                                        client.Player.SendUpdate(stream, MsgUpdate.Flags.NoXp, 10, 1, 0, MsgUpdate.DataType.ArchiveSkill, false);
                                    }
                                    break;
                                }
                            case "remflag":
                                {
                                    client.Player.RemoveFlag((MsgUpdate.Flags)int.Parse(data[1]));
                                    break;
                                }
                            case "level":
                                {
                                    byte amount = 0;
                                    if (byte.TryParse(data[1], out amount))
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.UpdateLevel(stream, amount, true);
                                        }
                                    }
                                    break;


                                }
                            case "money":
                                {
                                    ulong amount = 0;
                                    if (ulong.TryParse(data[1], out amount))
                                    {
                                        client.Player.Money = amount;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                        }
                                    }
                                    break;
                                }
                            case "testbp":
                                {
                                    using (var recCmd = new ServerSockets.RecycledPacket()) client.Player.SendUpdate(recCmd.GetStream(), 50, (Game.MsgServer.MsgUpdate.DataType)uint.Parse(data[1]));
                                    break;
                                }
                            case "dominocoins":
                                {
                                    client.Player.DominoCoins = long.Parse(data[1]);
                                    break;
                                }
                            case "dominocode":
                                {
                                    client.Player.DominoCode = ulong.Parse(data[1]);
                                    break;
                                }

                            case "maxarchives":
                                {
                                    if (client.HundredWeapons.Valid())
                                    {
                                        client.HundredWeapons.Unlocked = true;
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Blade, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Sword, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hook, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Whip, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Club, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Hammer, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Axe, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Dagger, 9, false);
                                        client.HundredWeapons.AddWeapon(Database.MagicType.WeaponsType.Scepter, 9, false);
                                        foreach (var weapon in client.HundredWeapons.Objects.Values)
                                        {
                                            var atts = weapon.Attributes.ToArray();
                                            foreach (var att in atts)
                                                weapon.Attributes[att.Key] = weapon.DBInfo.Attributes[att.Key] * 2;
                                        }
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            client.Send(stream.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                                        }
                                    }
                                    break;
                                }
                            case "addarchive":
                                {
                                    if (client.HundredWeapons.Unlocked)
                                        client.HundredWeapons.AddWeapon((Database.MagicType.WeaponsType)ushort.Parse(data[1]), byte.Parse(data[2]), true);
                                    break;
                                }
                            case "cps":
                                {
                                    long amount = 0;
                                    if (long.TryParse(data[1], out amount))
                                    {
                                        client.Player.ConquerPoints = (uint)amount;

                                    }
                                    break;
                                }
                            case "maxblue":
                                {
                                    foreach (var item in Pool.ItemsBase.Values)
                                    {
                                        if (Database.ItemType.isBlueRune(item.ID))
                                        {
                                            if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                    client.Inventory.Add(rec.GetStream(), item.ID);
                                        }
                                    }
                                    break;
                                }
                            case "maxyellow":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var maxyellow = Pool.ItemsBase.Values.Where(p => p.ID / 10000 == 403 && p.ID / 100 > 40313 && p.ID % 10 == 1).ToArray();
                                        var rand = (ushort)(Pool.GetRandom.Next() % 1000);
                                        int Random = Pool.GetRandom.Next(0, maxyellow.Length);
                                        client.Inventory.Add(stream, maxyellow[Random].ID);
                                    }
                                    break;
                                }
                            case "maxrunes":
                                {
                                    foreach (var item in Pool.ItemsBase.Values)
                                    {
                                        if (Database.ItemType.isRune(item.ID))
                                        {
                                            if (Database.ItemType.EquipPassJobReq(item, client) && Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                    client.Inventory.AddReturnedItem(rec.GetStream(), item.ID);
                                        }
                                    }
                                    break;
                                }
                            case "Test22":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Database.BotJail.JoinJail(client);
                                    }
                                    break;
                                }
                            case "redrunes":
                                {
                                    foreach (var item in Pool.ItemsBase.Values)
                                    {
                                        if (Database.ItemType.isRedRune(item.ID))
                                        {
                                            if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                                using (var rec = new ServerSockets.RecycledPacket())
                                                    client.Inventory.Add(rec.GetStream(), item.ID);
                                        }
                                    }
                                    break;


                                }
                            case "inbox":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        uint _itemid = client.Inventory.AddInbox(stream, 2100075);
                                        PrizeInfo mailList2 = new PrizeInfo(client, "Gift", "DevilWar", "DevilWarPrize", 2592000, 0, 0, _itemid, 0, 1);

                                    }

                                    break;

                                }



                            case "RewardPoints":
                                {
                                    var target = Pool.GamePoll.Values.FirstOrDefault(i => i.Player.Name == data[1]);
                                    if (target != null)
                                    {
                                        target.Player.RewardPoints += uint.Parse(data[2]);
                                    }
                                    break;
                                }



                            case "donatepoints":
                                {
                                    var target = Pool.GamePoll.Values.FirstOrDefault(i => i.Player.Name == data[1]);
                                    if (target != null)
                                    {
                                        target.Player.DonatePoints += uint.Parse(data[2]);
                                    }
                                    break;
                                }
                            case "remspell":
                                {
                                    ushort ID = 0;
                                    if (!ushort.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invlid spell ID !");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.MySpells.Remove(ID, rec.GetStream());
                                    break;
                                }
                            case "spell":
                                {
                                    ushort ID = 0;
                                    if (!ushort.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invlid spell ID !");
                                        break;
                                    }
                                    byte level = 0;
                                    if (!byte.TryParse(data[2], out level))
                                    {
                                        client.SendSysMesage("Invlid spell Level ! ");
                                        break;
                                    }
                                    byte levelHu = 0;
                                    if (data.Length >= 3)
                                    {
                                        if (!byte.TryParse(data[3], out levelHu))
                                        {
                                            client.SendSysMesage("Invlid spell Level Souls ! ");
                                            break;
                                        }
                                    }
                                    int Experience = 0;
                                    if (!int.TryParse(data[4], out Experience))
                                    {
                                        client.SendSysMesage("Invlid spell Experience ! ");
                                        break;
                                    }

                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.MySpells.Add(rec.GetStream(), ID, level, levelHu, 0, Experience);
                                    break;
                                }

                            case "removespell":
                                {
                                    ushort ID = 0;
                                    if (!ushort.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invlid spell ID !");
                                        break;
                                    }

                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.MySpells.Remove(ID, rec.GetStream());
                                    break;
                                }
                            case "AllSkill":
                                {
                                    foreach (var Skill in client.MySpells.ClientSpells.Values)
                                    {
                                        MyConsole.WriteLine(Skill.ID.ToString());

                                    }
                                    break;
                                }
                            case "removespellall":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        foreach (var Spell in client.MySpells.ClientSpells.Values)
                                        {

                                            client.MySpells.Remove(Spell.ID, rec.GetStream());
                                        }


                                    break;
                                }
                            case "clearspells":
                                {
                                    var spells = client.MySpells.ClientSpells.Keys.ToArray();

                                    foreach (var spell in spells)
                                        using (var rec = new ServerSockets.RecycledPacket())
                                            client.MySpells.Remove(spell, rec.GetStream());
                                    break;
                                }
                            case "prof":
                                {
                                    ushort ID = 0;
                                    if (!ushort.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invlid prof ID !");
                                        break;
                                    }
                                    byte level = 0;
                                    if (!byte.TryParse(data[2], out level))
                                    {
                                        client.SendSysMesage("Invlid prof Level ! ");
                                        break;
                                    }
                                    uint Experience = 0;
                                    if (!uint.TryParse(data[3], out Experience))
                                    {
                                        client.SendSysMesage("Invlid prof Experience ! ");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.MyProfs.Add(rec.GetStream(), ID, level, Experience);
                                    break;
                                }
                            case "clear":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.Clear(rec.GetStream());
                                    break;
                                }
                            case "Ha2":
                                {
                                    var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Emoji).ToArray();

                                    foreach (var AddAllFrame in AllFrame)
                                    {
                                        client.HairfaceStorage.Add(AddAllFrame, true);
                                    }
                                    break;
                                }
                            case "newew":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                                        packet.ID = MsgStringPacket.StringID.Effect;
                                        packet.UID = client.Player.UID;
                                        packet.Strings = new string[1] { "moveback" };
                                        client.Player.View.SendView(stream.StringPacketCreate(packet), false);
                                    }
                                    break;
                                }
                            case "kill":
                                {
                                    client.Status.MagicAttack += 100000000;
                                    client.Status.MinAttack += 100000000;
                                    client.Status.MaxAttack += 100000000;
                                    client.Status.Counteraction += 500;
                                    break;
                                }
                            case "Mob":
                                {
                                    var map = Pool.ServerMaps[client.Player.Map];
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        Server.AddMapMonster(stream, map, uint.Parse(data[1]), client.Player.X, client.Player.Y, 1, 1, 1);

                                    }

                                    break;

                                }
                            case "addtime":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Inventory.AddTime(stream, uint.Parse(data[1]), 0, 0, 0, false, uint.Parse(data[2]) * 24 * 60 * 60);//1*24*60*60
                                    }
                                    break;
                                }

                            case "tele":
                                {

                                    client.TerainMask = 0;
                                    uint mapid = 0;
                                    if (!uint.TryParse(data[1], out mapid))
                                    {
                                        client.SendSysMesage("Invlid Map ID !");
                                        break;
                                    }
                                    ushort X = 0;
                                    if (!ushort.TryParse(data[2], out X))
                                    {
                                        client.SendSysMesage("Invlid X !");
                                        break;
                                    }
                                    ushort Y = 0;
                                    if (!ushort.TryParse(data[3], out Y))
                                    {
                                        client.SendSysMesage("Invlid Y !");
                                        break;
                                    }
                                    uint DinamicID = 0;
                                    if (!uint.TryParse(data[4], out DinamicID))
                                    {
                                        client.SendSysMesage("Invlid DinamicID !");
                                        break;
                                    }
                                    client.Teleport(X, Y, mapid, DinamicID);

                                    break;
                                }
                            case "effectfloor":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        {
                                            MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                                            item.Color = (Role.Flags.Color)2;
                                            item.ITEM_ID = uint.Parse(data[1]);//1182;
                                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, client.Player.X, client.Player.Y, MsgFloorItem.MsgItem.ItemType.Effect, 0, client.Player.UID, client.Player.Map
                                                   , 0, false, client.Map, 4);

                                            if (client.Map.EnqueueItem(DropItem))
                                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);
                                        }
                                    }
                                    break;

                                }
                            case "removegarment":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.RemoveSpecialGarment(rec.GetStream());
                                    break;
                                }
                            case "addgarment":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.AddSpecialGarment(rec.GetStream(), uint.Parse(data[1]));
                                    break;
                                }
                            case "remgarment":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Player.RemoveSpecialGarment(rec.GetStream());
                                    break;
                                }
                            case "itemm":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.AddItemWitchStack(ID, 0, 1, rec.GetStream(), false);

                                    break;
                                }
                            case "cleararchives":
                                {
                                    client.HundredWeapons.Unlocked = false;
                                    client.HundredWeapons.Objects.Clear();
                                    client.Socket.Disconnect();
                                    break;
                                }

                            case "iTEMM":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.AddItemWitchStack(ID, 0, 10, rec.GetStream(), false);

                                    break;

                                }
                            case "GoldBrick":
                                {
                                    client.Player.MyUnion.GoldBrick = uint.Parse(data[1]);
                                    break;
                                }
                            case "Treasury":
                                {
                                    client.Player.MyUnion.Treasury = uint.Parse(data[1]);
                                    break;
                                }
                            case "itemadd":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 1, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            
                            
                            case "itemadd10":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 10, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "itemadd100":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 100, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "itemadd1000":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 1000, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "itemadd5000":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 5000, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "itemadd10000":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 10000, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "ddd":
                                {
                                    client.Player.RemoveFlag(MsgUpdate.Flags.TopSuperGuildWarFiveStars);
                                    break;
                                }
                            case "itemadd2":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }
                                    byte count = 1;
                                    if (data.Length > 2)
                                    {
                                        if (!byte.TryParse(data[2], out count))
                                        {
                                            client.SendSysMesage("Invlid item count !");
                                            break;
                                        }
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 1, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            //case "count":
                            //    {
                            //        uint ID = 0;
                            //        if (!uint.TryParse(data[1], out ID))
                            //        {
                            //            client.SendSysMesage("Invalid item ID !");
                            //            break;
                            //        }

                            //        using (var rec = new ServerSockets.RecycledPacket())
                            //        {
                            //            client.Inventory.AddItemWitchStack(ID, 0, uint.Parse(data[2]), rec.GetStream(), false);


                            //        }
                            //        break;
                            //    }
                            case "count10k":
                                {
                                    uint ID = 0;
                                    if (!uint.TryParse(data[1], out ID))
                                    {
                                        client.SendSysMesage("Invalid item ID !");
                                        break;
                                    }

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        client.Inventory.AddItemWitchStack(ID, 0, 10000, rec.GetStream(), false);


                                    }
                                    break;
                                }
                            case "ClearWh":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> wh_items;
                                        if (client.Warehouse.ClientItems.TryGetValue(ushort.MaxValue, out wh_items))
                                        {
                                            foreach (var item in wh_items.Values)
                                            {
                                                client.Warehouse.ClientItems.Remove(ushort.MaxValue);
                                                item.Send(client, stream);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "updatestarsitem":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        foreach (var item in client.Inventory.ClientItems.Values)
                                        {
                                            if (item.ITEM_ID == uint.Parse(data[1]))
                                            {
                                                item.PerfectionLevel = uint.Parse(data[2]);
                                                item.Mode = Role.Flags.ItemMode.Update;
                                                item.Send(client, rec.GetStream());
                                                break;
                                            }

                                        }
                                    }
                                    break;
                                }
                            case "bcps":
                                {
                                    client.Player.BoundConquerPoints = int.Parse(data[1]);
                                    break;
                                }
                            case "pkwar":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        var action = new ActionQuery()
                                        {
                                            ObjId = client.Player.UID,
                                            Type = ActionType.SetMapColor,
                                            dwParam = 6210062,
                                            PositionX = client.Player.X,
                                            PositionY = client.Player.Y

                                        };
                                        client.Send(stream.ActionCreate(action));
                                    }


                                    break;
                                }
                            case "lotus":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Server.AddFloor(stream, client.Map, Game.MsgFloorItem.MsgItemPacket.AuroraLotus, client.Player.X, client.Player.Y, 1, Pool.Magic[12370][1], client, client.Player.GuildID, client.Player.UID, 0, "AuroraLotus", true);
                                    }
                                    break;
                                }
                            case "additemstack":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Inventory.AddItemWitchStack(uint.Parse(data[1]), 0, ushort.Parse(data[2]), stream, true);
                                    }
                                    break;
                                }
                            case "remitemstack":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Inventory.RemoveStackItem(uint.Parse(data[1]), ushort.Parse(data[2]), stream);
                                    }
                                    break;
                                }

                            case "activetrap":
                                {

                                    {
                                        uint ID = uint.Parse(data[1]);
                                        Game.MsgFloorItem.MsgItemPacket FloorPacket = Game.MsgFloorItem.MsgItemPacket.Create();
                                        FloorPacket.m_UID = Game.MsgFloorItem.MsgItem.UIDS.Count;
                                        FloorPacket.m_ID = ID;
                                        FloorPacket.m_X = client.Player.X;
                                        FloorPacket.m_Y = client.Player.Y;

                                        FloorPacket.ItemOwnerUID = client.Player.UID;


                                        FloorPacket.m_Color = (byte)0;
                                        FloorPacket.m_Color2 = (byte)14;
                                        FloorPacket.FlowerType = 3;
                                        FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var packet = rec.GetStream();
                                            client.Send(packet.ItemPacketCreate(FloorPacket));
                                        }
                                    }
                                    break;
                                }
                            #region innerpower
                            case "innerpotency":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Player.InnerPower.AddPotency(packet, client, int.Parse(data[1]));
                                    }
                                    break;
                                }
                            case "inneritems":
                                {
                                    foreach (var stage in Database.InnerPowerTable.Stages)
                                    {
                                        foreach (var gong in stage.NeiGongAtributes)
                                        {
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var packet = rec.GetStream();
                                                client.Inventory.Add(packet, gong.ItemID);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case "inner":
                                {
                                    // [Unanswered B7 2026-10-14] removed: this debug command sent a legacy binary struct
                                    // as packet 2612, but 2612 is CMsgTeamRecruit (protobuf) on the 7957 client, so the
                                    // client got a malformed message. 2612 is now answered in MY System\Unanswered\B7.
                                    break;
                            #endregion


                                }


                            case "bodynpcs":
                                {
                                    Game.MsgServer.MsgMovement.Bodyyyy = uint.Parse(data[1]);
                                    break;
                                }
                            case "addd":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        stream.ElitePkRankingCreate((MsgElitePkRanking.RankType)3, 2, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, client.Player.UID);
                                        stream.ElitePkRankingFinalize();
                                        client.Send(stream);
                                    }
                                    break;
                                }

                            case "teelite":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Send(stream.MsgInterServerIdentifier(0, client.Player.UID, uint.Parse(data[1])));
                                        client.Player.SendString(stream, MsgStringPacket.StringID.ServerName, false, data[2]);
                                    }
                                    break;
                                }
                            case "transfer":
                                {
                                    client.Player.InitializeTransfer(ushort.Parse(data[1]));
                                    break;
                                }
                            case "inter":
                                {

                                    MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
                                    break;
                                }

                            case "hp":
                                {
                                    client.Player.HitPoints = int.Parse(data[1]);
                                    client.Player.SendUpdateHP();
                                    break;
                                }

                            case "championpoints":
                                {
                                    client.Player.AddChampionPoints(uint.Parse(data[1]));
                                    break;
                                }

                            case "hair":
                                {
                                    client.Player.Hair = (ushort)((client.Player.Hair - (client.Player.Hair % 100)) + ushort.Parse(data[1]));
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Player.SendUpdate(packet, client.Player.Hair, MsgServer.MsgUpdate.DataType.HairStyle);

                                    }
                                    break;
                                }
                            case "tgui":
                                {

                                    var action = new ActionQuery()
                                    {
                                        ObjId = client.Player.UID,
                                        Type = 443,
                                        dwParam = uint.Parse(data[1])

                                    };
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Send(packet.ActionCreate(action));

                                    }
                                    break;
                                }
                            case "TestNew":
                                {

                                    break;
                                }

                            case "interip":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    MsgInterServer.PipeClient.Connect(client, data[1], ushort.Parse(data[2]));
                                    break;
                                }
                            case "dcinter":
                                {

                                    client.Socket.Disconnect();
                                    break;
                                }
                            case "addtitle":
                                {
                                    // Usage: @addtitle <titleId>
                                    if (data.Length < 2)
                                    {
                                        client.SendSysMesage("Usage: @addtitle <titleId>");
                                        break;
                                    }
                                    ushort titleId;
                                    if (!ushort.TryParse(data[1], out titleId))
                                    {
                                        client.SendSysMesage("Invalid title ID.");
                                        break;
                                    }
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Player.AddSpecialTitle((MsgTitleStorage.TitleType)titleId, packet);
                                        client.Send(packet);
                                    }
                                    client.SendSysMesage($"Added special title {titleId} to player {client.Player.Name}."); // [GmSec 2026-10-14] was missing $
                                    break;
                                }
                            case "addtitleTime":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        var TimeTitle = new Role.Player.WardRobeTitle();
                                        TimeTitle.titleID = 4001;
                                        TimeTitle.DateStamp = DateTime.Now.AddSeconds((120));
                                        TimeSpan timeSpan = TimeTitle.DateStamp - DateTime.Now;
                                        TimeTitle.TotalSeconds = (uint)timeSpan.TotalSeconds;
                                        client.Player.TitleWithTime.Add((uint)4001, TimeTitle);
                                        client.Player.AddTimeTitle((uint)4001, packet);
                                    }

                                    break;
                                }
                            case "createunion":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        client.Player.MyUnion = Role.Instance.Union.Create(packet, client, "NightMareConquer");
                                        client.Player.MyUnion.AddGuild(packet, client.Player.MyGuild);

                                    }
                                    break;
                                }
                            case "title":
                                {
                                    foreach (var atitle in Database.TitleStorage.Titles.Values)
                                    {
                                        if (atitle.ID == 2004)
                                        {
                                            MsgTitleStorage.TitleStorage title = new MsgTitleStorage.TitleStorage();
                                            title.ActionID = (MsgTitleStorage.Action)uint.Parse(data[1]);
                                            title.dwparam1 = 100;
                                            title.dwparam2 = atitle.ID;
                                            title.dwparam3 = atitle.SubID;
                                            title.Title = new MsgTitleStorage.Title();
                                            title.Title.ID = atitle.ID;
                                            title.Title.SubId = atitle.SubID;
                                            title.Title.dwparam1 = 100;
                                            using (var rec = new ServerSockets.RecycledPacket())
                                            {
                                                var packet = rec.GetStream();
                                                packet.CreateTitleStorage(title);
                                                client.Send(packet);

                                            }
                                        }
                                    }
                                    break;
                                }

                            case "floor":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();

                                        uint ID = (uint)uint.Parse(data[1]);
                                        var Item = new Game.MsgFloorItem.MsgItem(null, client.Player.X, client.Player.Y, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, client.Player.UID, client.Player.Map, 0, false, Pool.ServerMaps[client.Player.Map], 60 * 60 * 100000);
                                        Item.MsgFloor.m_ID = ID;
                                        Item.MsgFloor.m_Color = 2;
                                        Item.MsgFloor.Name = "STR_TRAP_ID_" + ID + "@@";
                                        Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                                        Item.GMap.cells[client.Player.X, client.Player.Y] |= MapFlagType.Item;
                                        Item.GMap.View.EnterMap<Role.IMapObj>(Item);

                                        Item.SendAll(packet, MsgDropID.Effect);


                                    }
                                    break;
                                }
                            case "wea":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var packet = rec.GetStream();
                                        packet.WeatherCreate((MsgWeather.WeatherType)ushort.Parse(data[1]), uint.Parse(data[2]), uint.Parse(data[3]), (uint)uint.Parse(data[4]), uint.Parse(data[5]));
                                        client.Send(packet);
                                    }
                                    break;
                                }
                            case "activefairi":
                                {
                                    //unsafe
                                    //{
                                    //    if (client.Player.testtttttttttt != 0)
                                    //    {
                                    //        MsgTransformFairy afair = MsgTransformFairy.Create();
                                    //        afair.Mode = MsgTransformFairy.Action.Dezactive;
                                    //        afair.FairyType = client.Player.testtttttttttt;
                                    //        afair.UID = client.Player.UID;


                                    //        using (var rec = new ServerSockets.RecycledPacket())
                                    //        {
                                    //            var packet = rec.GetStream();
                                    //            packet.TransformFairyCreate(MsgTransformFairy.Action.Dezactive, client.Player.testtttttttttt, client.Player.UID);
                                    //            client.Send(packet);
                                    //        }
                                    //    }


                                    //    using (var rec = new ServerSockets.RecycledPacket())
                                    //    {
                                    //        var packet = rec.GetStream();
                                    //        packet.TransformFairyCreate(MsgTransformFairy.Action.Active, uint.Parse(data[1]), client.Player.UID);
                                    //        client.Send(packet);
                                    //    }



                                    //    client.Player.testtttttttttt = uint.Parse(data[1]);
                                    //}
                                    break;
                                }

                            case "ets":
                                {
                                    client.Player.StageEpicTrojanQuest = byte.Parse(data[1]);
                                    break;
                                }

                            case "gift":
                                {
                                    client.Player.MainFlag = 0;
                                    break;
                                }
                            case "exit":
                                {
                                    if (!client.IsOwner) { client.SendSysMesage("Insufficient privileges for this command."); break; }
                                    Program.ProcessConsoleEvent(0);
                                    break;

                                }
                            case "switch":
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SwitchWingWalkerAttack(stream);
                                    }
                                    break;
                                }
                            case "mainflag":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.MainFlag = (Role.Player.MainFlagType)uint.Parse(data[1]);
                                        client.Player.SendUpdate(stream, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag, false);

                                    }
                                    break;
                                }
                            case "bossrank":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgServer.MsgBossHarmRanking Rank = new MsgServer.MsgBossHarmRanking();
                                        Rank.Type = uint.Parse(data[1]); // [ProtoFix 2026-10-14] MsgBossHarmRanking.Type is uint now
                                        Rank.MonsterID = uint.Parse(data[2]);
                                        Rank.Hunters = new MsgServer.MsgBossHarmRankingEntry[1];
                                        Rank.Hunters[0].HunterName = client.Player.Name;
                                        Rank.Hunters[0].HunterUID = client.Player.UID;
                                        Rank.Hunters[0].Rank = uint.Parse(data[3]);
                                        Rank.Hunters[0].ServerID = Database.GroupServerList.MyServerInfo.ID;
                                        Rank.Hunters[0].HunterScore = uint.Parse(data[4]);
                                        client.Send(stream.CreateBossHarmRankList(Rank));
                                    }
                                    break;



                                }
                            case "checkinfo":
                                {
                                    string Class = "";
                                    foreach (var pClient in Pool.GamePoll.Values)
                                    {
                                        if (pClient.Player.Name.ToLower() == data[1].ToLower())
                                        {
                                            if (Database.AtributesStatus.IsTrojan(pClient.Player.Class))
                                                Class = "Trojan";
                                            else if (Database.AtributesStatus.IsArcher(pClient.Player.Class))
                                                Class = "Archer";
                                            else if (Database.AtributesStatus.IsWarrior(pClient.Player.Class))
                                                Class = "Warrior";
                                            else if (Database.AtributesStatus.IsNinja(pClient.Player.Class))
                                                Class = "Ninja";
                                            else if (Database.AtributesStatus.IsMonk(pClient.Player.Class))
                                                Class = "Monk";
                                            else if (Database.AtributesStatus.IsPirate(pClient.Player.Class))
                                                Class = "Pirate";
                                            else if (Database.AtributesStatus.IsLee(pClient.Player.Class))
                                                Class = "Dragon-Warrior";
                                            else if (Database.AtributesStatus.IsThunderStriker(pClient.Player.Class))
                                                Class = "Thunderstriker";
                                            else if (Database.AtributesStatus.IsFire(pClient.Player.Class))
                                                Class = "Fire";
                                            else if (Database.AtributesStatus.IsWater(pClient.Player.Class))
                                                Class = "Water";
                                            else if (Database.AtributesStatus.IsWindWalker(pClient.Player.Class))
                                                Class = "Windwalker";
                                            else Class = "Taoist";

                                            client.SendSysMesage("[" + pClient.Player.Name + "]", ChatMode.FirstRightCorner, MsgColor.red);
                                            client.SendSysMesage("2nd Password: hidden" /* [GmSec 2026-10-14] */, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("Class: " + Class, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("Reborn: " + pClient.Player.Reborn, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("Level: " + pClient.Player.Level, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("CPs: " + pClient.Player.ConquerPoints, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("Money: " + pClient.Player.Money, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("BP: " + pClient.Player.BattlePower, ChatMode.ContinueRightCorner, MsgColor.red);
                                            client.SendSysMesage("Location: [" + pClient.Map.Name + ":" + pClient.Player.X + ", " + pClient.Player.Y + "]", ChatMode.ContinueRightCorner, MsgColor.red);
                                            break;
                                        }
                                    }
                                    break;

                                }
                            case "goldb":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        for (int x = 0; x < 30; x++)
                                            client.Send(stream.HandBrickInfoCreate((MsgHandBrickInfo.BrickInfo)x, 1000, 1000));
                                    }

                                    break;
                                }
                            case "testupd":
                                {


                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();



                                        for (int i = 100; i < 110; i++)
                                            for (int x = 50; x < 220; x++)
                                            {

                                                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, client.Player.UID, 1);
                                                stream = packet.Append(stream, (MsgUpdate.DataType)i, (uint)x, 30, 300, 300);
                                                stream = packet.GetArray(stream);

                                                client.Player.View.SendView(stream, true);

                                            }

                                    }
                                    break;

                                }
                            case "exp":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();

                                        client.Player.SendUpdate(stream, uint.Parse(data[1]), MsgUpdate.DataType.Experience, false);
                                    }
                                    break;
                                }
                            case "cd":
                                {
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        MsgMagicColdTime.MagicColdTime item = new MsgMagicColdTime.MagicColdTime();
                                        item.Spells = new MsgMagicColdTime.Spell[1];
                                        item.Spells[0] = new MsgMagicColdTime.Spell();
                                        item.Spells[0].SpellID = ushort.Parse(data[1]);
                                        item.Spells[0].Time = uint.Parse(data[2]); // [ProtoFix 2026-10-14] Spell.Time is uint now

                                        client.Send(stream.MagicColdTimeCreate(item));
                                    }
                                    break;


                                }
                            case "tttt":
                                {
                                    MsgTournaments.MsgSchedules.SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
                                    break;
                                }

                            case "reborn":
                                {
                                    client.Player.Reborn = byte.Parse(data[1]);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Reborn, MsgUpdate.DataType.Reborn);
                                    }
                                    break;
                                }
                            case "tes1":
                                {
                                    Database.ProfessionTable.TitleBenefit attr = new Database.ProfessionTable.TitleBenefit();
                                    attr.Class = Convert.ToByte(61500);
                                    attr.Rank = Convert.ToByte(70500);
                                    attr.AttributeValue = new Database.ProfessionTable.AttributeValue[4];
                                    for (int i = 0; i < attr.AttributeValue.Length; i++)
                                    {
                                        attr.AttributeValue[i] = new Database.ProfessionTable.AttributeValue();
                                        attr.AttributeValue[i].Type = (Game.MsgServer.MsgChiInfo.ChiAttribute)(Convert.ToUInt32(120500) / 10000);
                                        attr.AttributeValue[i].Value = Convert.ToUInt32(1) % 10000;
                                    }
                                    Database.ProfessionTable.TitleBenefits.Add(Convert.ToUInt32(1), attr);
                                    break;
                                }
                            case "class":
                                {
                                    client.Player.Class = uint.Parse(data[1]);
                                    break;
                                }
                        }
                        return true;
                    }
                    catch { MyConsole.WriteLine("[Error] Unexpected data while typing GM chat command => " + data[0] + " .", ConsoleColor.White); }
                }
            }

                            #endregion
            #endregion


            return false;
        }
    }
}