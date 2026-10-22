using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetBroadcast(
            this ServerSockets.Packet stream,
            out MsgBroadcast.BroadTyp Action,
            out uint dwParam,
            out string[] Strings)
        {
            Action = (MsgBroadcast.BroadTyp)stream.ReadUInt32(); // 4
            dwParam = stream.ReadUInt32();                       // 8
            stream.ReadUInt32();                                 // 12
            Strings = stream.ReadStringList();                   // 16
        }

        public static unsafe ServerSockets.Packet BroadcastCreate(
            this ServerSockets.Packet stream,
            MsgBroadcast.BroadTyp Action,
            uint dwParam,
            string[] Strings)
        {
            stream.InitWriter();

            stream.Write((uint)Action);
            stream.Write(dwParam);
            stream.Write(0);

            if (Strings != null)
                stream.Write(Strings);

            stream.Finalize(GamePackets.MsgPigeon);

            return stream;
        }
    }


    public unsafe struct MsgBroadcast
    {

        private static readonly string[] BlockedBroadcastWords =
{
    "fuck",
    "fucking",
    "motherfucker",
    "shit",
    "bitch",
    "asshole",
    "dick",
    "pussy",
    "porn",
    "porno",
    "pornography",
    "sex",
    "sexual",
    "xxx",
    "nude",
    "naked",
     "omk",
      "omak",
       "abok",

    "كس",
    "كسك",
    "كسم",
    "كس امك",
    "كسمك",
    "قحبة",
    "قحبه",
    "شرموط",
    "شرموطة",
    "شرموطه",
    "متناك",
    "منيوك",
    "نيك",
    "انيك",
    "زبر",
    "طيز",
    "خول",
    "خرا",
    "عرص",
    "وسخ",
    "اباحي",
    "إباحي",
    "اباحية",
    "إباحية",
    "احا",
    "احو",
    "أحو",
      "كس",
    "كسك",
    "كسم",
    "كسمك",
    "كس امك",
    "كسم امك",
    "كس اختك",
    "كسم اختك",
    "كس ابوك",
    "كسم ابوك",
    "نيك",
    "نيكه",
    "نيكك",
    "نيك امك",
    "نيك اختك",
    "انيك",
    "تنيك",
    "منيك",
    "منيكة",
    "منيوك",
    "متناك",
    "متناكة",
    "قحبة",
    "قحبه",
    "قحاب",
    "قحبات",
    "شرموط",
    "شرموطة",
    "شرموطه",
    "شراميط",
    "خول",
    "خولات",
    "خولك",
    "عرص",
    "عراص",
    "معرص",
    "معرصين",
    "طيز",
    "طيزك",
    "طيزك",
    "مؤخرة",
    "زبر",
    "زب",
    "زبه",
    "زبرك",
    "خرا",
    "خراء",
    "خريان",
    "وسخ",
    "وسخة",
    "وسخين",
    "احا",
    "أحا",
    "احو",
    "أحو",
    "يا ابن الاحبة",
    "ابن الاحبة",
    "ابن الكلب",
    "ابن القحبة",
    "ابن الشرموطة",
    "اباحي",
    "إباحي",
    "اباحية",
    "إباحية",
    "جنس",
    "جنسي",
    "جنسية",
    "ممارسة جنسية",
    "عاري",
    "عارية",
    "عريان",
    "عريانة",
    "متحرش",
    "سيرفر ميت",
    "سيرفر تعبان",
    "سيرفر عرص",
    "سيرفر متناك",
    "سيرفر شرموط",
    "سيرفر",
    "تحرش",
    "تعبان",
    "ميت",
    "أحا",
     "الشارموطة",
      "الشارموطهه",
    "الشرموطة"
};

        private static string NormalizeBroadcastText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.ToLowerInvariant();

            text = text
                .Replace(" ", "")
                .Replace("\t", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("-", "")
                .Replace("_", "")
                .Replace("*", "")
                .Replace("#", "")
                .Replace("@", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("^", "")
                .Replace("&", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace("|", "");
            text = text
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ة", "ه");

            return text;
        }

        private static bool ContainsBlockedBroadcastWord(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string normalized = NormalizeBroadcastText(text);

            foreach (string word in BlockedBroadcastWords)
            {
                string normalizedWord = NormalizeBroadcastText(word);

                if (string.IsNullOrEmpty(normalizedWord))
                    continue;

                if (normalized.Contains(normalizedWord))
                    return true;
            }

            return false;
        }
        private static readonly ConcurrentDictionary<uint, DateTime> BroadcastCooldown
            = new ConcurrentDictionary<uint, DateTime>();

        private const int BroadcastWaitMinutes = 3;

        public enum BroadTyp : byte
        {
            ReleaseSoonMessages = 1,
            MyMessages = 2,
            BroadcastMessage = 3,
            Urgen15CPs = 4,
            Urgen5CPs = 5
        }

        [PacketAttribute(GamePackets.MsgPigeon)]
        private static void Handler(
            Client.GameClient user,
            ServerSockets.Packet stream)
        {
            try
            {
                BroadTyp Action;
                uint dwParam;
                string[] Strings;

                stream.GetBroadcast(
                    out Action,
                    out dwParam,
                    out Strings);

                switch (Action)
                {
                    #region Urgen5CPs

                    case BroadTyp.Urgen5CPs:
                        {

                            if (user.IsVendor)
                                return;

                            if (user.InTrade)
                                return;

                            for (int c = 0;
                                 c < MsgTournaments.MsgBroadcast.Broadcasts.Count;
                                 c++)
                            {
                                var broadcast =
                                    MsgTournaments.MsgBroadcast.Broadcasts[c];

                                if (broadcast.ID == dwParam)
                                {
                                    if (c != 0)
                                    {
                                        if (user.InTrade)
                                            break;

                                        if (Program.TreadeOrShop.Contains(user.Player.Map))
                                        {
                                            user.SendSysMesage(
                                                "Sorry, You cannot use Broadcast here.");
                                            return;
                                        }

                                        if (user.Player.ConquerPoints >= 5)
                                        {
                                            broadcast.SpentCPs += 5;

                                            user.Player.ConquerPoints -= 5;

                                            if (MsgTournaments.MsgBroadcast.Broadcasts[c - 1].SpentCPs
                                                <= broadcast.SpentCPs)
                                            {
                                                MsgTournaments.MsgBroadcast.Broadcasts[c] =
                                                    MsgTournaments.MsgBroadcast.Broadcasts[c - 1];

                                                MsgTournaments.MsgBroadcast.Broadcasts[c - 1] =
                                                    broadcast;
                                            }
                                            else
                                            {
                                                MsgTournaments.MsgBroadcast.Broadcasts[c] =
                                                    broadcast;
                                            }
                                        }
                                    }

                                    break;
                                }
                            }

                            break;
                        }

                    #endregion

                    #region Urgen15CPs

                    case BroadTyp.Urgen15CPs:
                        {

                            if (user.IsVendor)
                                return;

                            if (user.InTrade)
                                return;

                            for (int c = 0;
                                 c < MsgTournaments.MsgBroadcast.Broadcasts.Count;
                                 c++)
                            {
                                var broadcast =
                                    MsgTournaments.MsgBroadcast.Broadcasts[c];

                                if (broadcast.ID == dwParam)
                                {
                                    if (c != 0)
                                    {
                                        if (user.InTrade)
                                            break;

                                        if (Program.TreadeOrShop.Contains(user.Player.Map))
                                        {
                                            user.SendSysMesage(
                                                "Sorry, You cannot use Broadcast here.");
                                            return;
                                        }

                                        if (user.Player.ConquerPoints >= 15)
                                        {
                                            broadcast.SpentCPs += 15;

                                            user.Player.ConquerPoints -= 15;

                                            for (int b = c - 1; b > 0; b--)
                                            {
                                                MsgTournaments.MsgBroadcast.Broadcasts[b] =
                                                    MsgTournaments.MsgBroadcast.Broadcasts[b - 1];
                                            }

                                            MsgTournaments.MsgBroadcast.Broadcasts[0] =
                                                broadcast;
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    #endregion

                    #region ReleaseSoonMessages

                    case BroadTyp.ReleaseSoonMessages:
                        {

                            if (user.IsVendor)
                                return;

                            if (user.InTrade)
                                return;

                            if (Program.TreadeOrShop.Contains(user.Player.Map))
                            {
                                user.SendSysMesage(
                                    "Sorry, You cannot use Broadcast here.");
                                return;
                            }

                            const int max = 10;

                            int offset = (int)(dwParam * max);

                            int count = Math.Min(
                                max,
                                Math.Max(
                                    0,
                                    MsgTournaments.MsgBroadcast.Broadcasts.Count
                                    - offset));

                            ushort total = 0;

                            for (int x = 0; x < count; x++)
                            {
                                if (MsgTournaments.MsgBroadcast.Broadcasts.Count
                                    > offset + x)
                                {
                                    if (x % 5 == 0 || x == 0)
                                    {
                                        uint add = (uint)count;

                                        if (count > 5)
                                        {
                                            if (x > 4)
                                                add = (uint)(count - x);
                                            else
                                                add = 5;
                                        }

                                        if (x != 0)
                                            user.Send(stream.FinalizeBroadcastlist());

                                        stream.BroadcastlistCreate(
                                            dwParam,
                                            total,
                                            (ushort)add);

                                        total++;
                                    }

                                    var element =
                                        MsgTournaments.MsgBroadcast.Broadcasts[
                                            offset + x];

                                    stream.AddItemBroadcastlist(
                                        element,
                                        (uint)(offset + x));
                                }
                            }

                            user.Send(stream.FinalizeBroadcastlist());

                            break;
                        }

                    #endregion

                    #region MyMessages

                    case BroadTyp.MyMessages:
                        {

                            if (user.IsVendor)
                                return;

                            if (user.InTrade)
                                return;

                            if (Program.TreadeOrShop.Contains(user.Player.Map))
                            {
                                user.SendSysMesage(
                                    "Sorry, You cannot use Broadcast here.");
                                return;
                            }

                            var MyMessages =
                                MsgTournaments.MsgBroadcast.Broadcasts
                                .Where(p => p.EntityID == user.Player.UID)
                                .ToArray();

                            const int max = 10;

                            int offset = (int)(dwParam * max);

                            int count = Math.Min(
                                max,
                                Math.Max(
                                    0,
                                    MyMessages.Length - offset));

                            ushort total = 0;

                            for (int x = 0; x < count; x++)
                            {
                                if (MyMessages.Length > offset + x)
                                {
                                    if (x % 5 == 0 || x == 0)
                                    {
                                        uint add = (uint)count;

                                        if (count > 5)
                                        {
                                            if (x > 4)
                                                add = (uint)(count - x);
                                            else
                                                add = 5;
                                        }

                                        if (x != 0)
                                            user.Send(stream.FinalizeBroadcastlist());

                                        stream.BroadcastlistCreate(
                                            dwParam,
                                            total,
                                            (ushort)add);

                                        total++;
                                    }

                                    var element =
                                        MyMessages[offset + x];

                                    stream.AddItemBroadcastlist(
                                        element,
                                        (uint)(offset + x));
                                }
                            }

                            user.Send(stream.FinalizeBroadcastlist());

                            break;
                        }

                    #endregion

                    #region BroadcastMessage

                    case BroadTyp.BroadcastMessage:
                        {
                            /*
                             * Basic restrictions
                             */


                            if (user.IsVendor)
                                return;

                            if (user.InTrade)
                                return;

                            /*
                             * Don't allow Broadcast in Trade/Shop maps.
                             */

                            if (Program.TreadeOrShop.Contains(user.Player.Map))
                            {
                                user.SendSysMesage(
                                    "Sorry, You cannot use Broadcast here.");
                                return;
                            }

                            /*
                             * Maximum broadcast list.
                             */

                            if (MsgTournaments.MsgBroadcast.Broadcasts.Count
                                == MsgTournaments.MsgBroadcast.MaxBroadcasts)
                            {
#if Arabic
                            user.SendSysMesage(
                                "You cannot send any broadcasts for now. The limit has been reached. Wait until a broadcast is chopped down.");
#else
                                user.SendSysMesage(
                                    "You cannot send any broadcasts for now. The limit has been reached. Wait until a broadcast is chopped down.");
#endif
                                break;
                            }

                            /*
                             * Validate message.
                             */

                            if (Strings == null)
                                break;

                            if (Strings.Length == 0)
                                return;

                            if (Strings[0] == null)
                                break;

                            if (string.IsNullOrWhiteSpace(Strings[0]))
                                return;

                            // Block bad / sexual / offensive words BEFORE taking CP
                            if (ContainsBlockedBroadcastWord(Strings[0]))
                            {
                                user.CreateBoxDialog(
                                    "You cannot use offensive or inappropriate words in Broadcast.");

                                return;
                            }

                            /*
                             * Check Chat Ban BEFORE taking CP.
                             */

                            if (user.Player.IsStillBanned)
                            {
                                if (user.Player.PermenantBannedChat)
                                {
                                    user.SendSysMesage(
                                        "Sorry, you still banned from chatting Permenatly.",
                                        Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.System,
                                        Qahr.Mobile.Game.MsgServer.MsgMessage.MsgColor.white);
                                }
                                else
                                {
                                    user.SendSysMesage(
                                        "Sorry, you still banned from chatting till "
                                        + user.Player.BannedChatStamp.ToString(),
                                        Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.System,
                                        Qahr.Mobile.Game.MsgServer.MsgMessage.MsgColor.white);
                                }

                                return;
                            }

                            /*
                             * Map 5040 restriction.
                             */

                            if (user.Player.Map == 5040)
                            {
                                user.SendSysMesage(
                                    "Sorry, you still banned from chatting Permenatly.",
                                    Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.System,
                                    Qahr.Mobile.Game.MsgServer.MsgMessage.MsgColor.white);

                                return;
                            }

                            /*
                             * 15 MINUTE BROADCAST COOLDOWN
                             */

                            DateTime lastBroadcast;

                            if (BroadcastCooldown.TryGetValue(
                                user.Player.UID,
                                out lastBroadcast))
                            {
                                TimeSpan remaining =
                                    lastBroadcast
                                    .AddMinutes(BroadcastWaitMinutes)
                                    - DateTime.Now;

                                if (remaining.TotalSeconds > 0)
                                {
                                    user.CreateBoxDialog(
                                        "You must wait "
                                        + Math.Ceiling(remaining.TotalMinutes)
                                        + " minutes before sending another broadcast.");

                                    break;
                                }
                            }

                            /*
                             * Need 5 CP.
                             */

                            if (user.Player.ConquerPoints < 5)
                            {
                                user.CreateBoxDialog(
                                    "You do not have enough Conquer Points. You need 5 CP to send a broadcast.");

                                break;
                            }

                            /*
                             * Save CP before/after for logging.
                             */

                            uint OldCPs = user.Player.ConquerPoints;

                            /*
                             * Take CP.
                             */

                            user.Player.ConquerPoints -= 5;

                            uint NewCPs = user.Player.ConquerPoints;

                            /*
                             * Set cooldown ONLY after successful payment.
                             */

                            BroadcastCooldown[user.Player.UID] =
                                DateTime.Now;

                            /*
                             * Limit message length.
                             */

                            if (Strings[0].Length > 80)
                                Strings[0] = Strings[0].Remove(80);

                            /*
                             * Create Broadcast object.
                             */

                            MsgTournaments.MsgBroadcast.BroadcastStr broadcast =
                                new MsgTournaments.MsgBroadcast.BroadcastStr();

                            broadcast.EntityID =
                                user.Player.UID;

                            broadcast.EntityName =
                                user.Player.Name;

                            if (user.Player.InUnion)
                            {
                                broadcast.UnionRank =
                                    (uint)Role.Instance.Union.Member.GetRank(
                                        user.Player.UnionMemeber.Rank);
                            }

                            broadcast.ID =
                                MsgTournaments.MsgBroadcast.BroadcastCounter.Next;

                            broadcast.Message =
                                Strings[0];

                            /*
                             * Process linked Items.
                             */

                            string[] lines =
                                broadcast.Message.Split(
                                    new string[] { "[" },
                                    StringSplitOptions.RemoveEmptyEntries);

                            for (int x = 0; x < lines.Length; x++)
                            {
                                string str = lines[x];

                                if (str.Contains("Item "))
                                {
                                    string[] line =
                                        str.Split(' ');

                                    if (line != null && line.Length > 2)
                                    {
                                        uint UID = 0;

                                        if (uint.TryParse(
                                            line[2],
                                            out UID))
                                        {
                                            MsgGameItem msg_item;

                                            if (user.TryGetItem(
                                                UID,
                                                out msg_item))
                                            {
                                                Pool.ChatItems.Add(
                                                    msg_item.UID,
                                                    msg_item);
                                            }
                                        }
                                    }
                                }
                            }

                            /*
                             * Log Broadcast
                             */

                            try
                            {
                                Database.ServerDatabase.LoginQueue.Enqueue(
                                    "[Broadcast Message] "
                                    + "Player : [ "
                                    + user.Player.Name
                                    + " ] "

                                    + "UID : [ "
                                    + user.Player.UID
                                    + " ] "

                                    + "Message : [ "
                                    + broadcast.Message
                                    + " ] "

                                    + "CP Before : [ "
                                    + OldCPs
                                    + " ] "

                                    + "CP Cost : [ 5 ] "

                                    + "CP After : [ "
                                    + NewCPs
                                    + " ] "

                                    + "Date : [ "
                                    + DateTime.Now.ToString(
                                        "yyyy-MM-dd HH:mm:ss")
                                    + " ] "

                                    + "Map : [ "
                                    + user.Player.Map
                                    + " ] "

                                    + "X : [ "
                                    + user.Player.X
                                    + " ] "

                                    + "Y : [ "
                                    + user.Player.Y
                                    + " ] "

                                    + "IP : [ "
                                    + user.Socket.RemoteIp
                                    + " ]"
                                );
                            }
                            catch
                            {
                                // Don't break Broadcast if logging fails.
                            }

                            /*
                             * First broadcast becomes CurrentBroadcast.
                             */

                            if (MsgTournaments.MsgBroadcast.Broadcasts.Count == 0)
                            {
                                if (MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID == 1)
                                {
                                    user.Player.LastBroadCast =
                                        DateTime.Now;

                                    MsgTournaments.MsgBroadcast.CurrentBroadcast =
                                        broadcast;

                                    MsgTournaments.MsgBroadcast.LastBroadcast =
                                        DateTime.Now;

                                    /*
                                     * Send Global Message
                                     */

                                    if (user.Player.InUnion)
                                    {
                                        Server.SendGlobalPacket(
                                            new MsgServer.MsgMessage(
                                                Strings[0],
                                                "ALLUSERS",
                                                user.Player.Name,
                                                MsgServer.MsgMessage.MsgColor.red,
                                                MsgServer.MsgMessage.ChatMode.BroadcastMessage)
                                            .GetArray(
                                                stream,
                                                (uint)Role.Instance.Union.Member.GetRank(
                                                    user.Player.UnionMemeber.Rank)));
                                    }
                                    else
                                    {
                                        Server.SendGlobalPacket(
                                            new MsgServer.MsgMessage(
                                                Strings[0],
                                                "ALLUSERS",
                                                user.Player.Name,
                                                MsgServer.MsgMessage.MsgColor.red,
                                                MsgServer.MsgMessage.ChatMode.BroadcastMessage)
                                            .GetArray(stream));
                                    }

                                    /*
                                     * Send response to player.
                                     */

                                    user.Send(
                                        stream.BroadcastCreate(
                                            BroadTyp.BroadcastMessage,
                                            dwParam,
                                            Strings));

                                    break;
                                }
                            }

                            /*
                             * Save broadcast to list.
                             */

                            MsgTournaments.MsgBroadcast.Broadcasts.Add(
                                broadcast);

                            dwParam =
                                (uint)MsgTournaments.MsgBroadcast.Broadcasts.Count;

                            /*
                             * Send response.
                             */

                            user.Send(
                                stream.BroadcastCreate(
                                    BroadTyp.BroadcastMessage,
                                    dwParam,
                                    Strings));

                            /*
                             * Save additional local log.
                             */

                            try
                            {
                                string folderN =
                                    DateTime.Now.Year
                                    + "-"
                                    + DateTime.Now.Month;

                                string Path =
                                    "Cache\\Logs\\broadcasts\\";

                                string NewPath =
                                    System.IO.Path.Combine(
                                        Path,
                                        folderN);

                                if (!Directory.Exists(NewPath))
                                {
                                    Directory.CreateDirectory(NewPath);
                                }

                                string FilePath =
                                    System.IO.Path.Combine(
                                        NewPath,
                                        DateTime.Now.Day + ".txt");

                                if (!File.Exists(FilePath))
                                {
                                    using (
                                        FileStream fs =
                                            File.Create(FilePath))
                                    {
                                        fs.Close();
                                    }
                                }

                                using (
                                    StreamWriter file =
                                        new StreamWriter(
                                            FilePath,
                                            true))
                                {
                                    file.WriteLine(
                                        "UserName[Acc] |"
                                        + broadcast.EntityName
                                        + "| UIDAccount |"
                                        + broadcast.EntityID
                                        + " | MESSAGE: |"
                                        + broadcast.Message
                                        + " [The Clock] "
                                        + DateTime.Now.Hour
                                        + ":"
                                        + DateTime.Now.Minute
                                        + ":"
                                        + DateTime.Now.Second);

                                    file.Close();
                                }
                            }
                            catch
                            {
                                // Don't break Broadcast if file logging fails.
                            }

                            break;
                        }

                    #endregion
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
        }
    }
}