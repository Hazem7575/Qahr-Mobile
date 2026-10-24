using System.Collections.Generic;
using Qahr.Mobile.EventData;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: the "Download &amp; Get Rewards" window of the mobile client (engine window, items from
    /// ini\luacfg\info.lua DownloadPrize). Its "Claim Reward" button sends the raw call "&lt;/F&gt;UpFeedBack_DownloadReward"
    /// (CMsgMBAction 10046). Items: Data\mobile_download_reward.txt. One claim per character (EventStore).
    /// Hook: LuaUi\LuaPush.HandleCallback (raw calls).
    /// </summary>
    public static class MobileDownloadReward
    {
        public const string Func = "UpFeedBack_DownloadReward";
        const string Ev = "mdownload", ClaimKey = "reward";

        static readonly DataFile<List<RewardItem>> Items = new DataFile<List<RewardItem>>("mobile_download_reward.txt", "DownloadReward", Parse);

        static List<RewardItem> Parse(string[] lines)
        {
            var items = new List<RewardItem>();
            foreach (var line in DataFile.Clean(lines))
            {
                var p = DataFile.Words(line);
                if (p.Length > 1 && p[0].Equals("items", System.StringComparison.OrdinalIgnoreCase))
                    items.AddRange(Rewards.Parse(p, 1));
            }
            return items;
        }

        /// <summary>True = the call was the download reward (answered).</summary>
        public static bool OnRawCall(Client.GameClient c, string func)
        {
            if (func != Func || !MobileClient.Is(c) || c.Player == null)
                return false;
            var items = Items.Value;
            if (EventStore.IsClaimed(c.Player.UID, Ev, ClaimKey))
            {
                PlayerMsg.Tell(c, "You have already claimed the download reward.");
                return true;
            }
            if (items.Count == 0 || !Rewards.CanGive(c, items, "DownloadReward", "Nothing was claimed."))
                return true;
            if (!EventStore.TryClaimOnce(c.Player.UID, Ev, ClaimKey))
            {
                PlayerMsg.Tell(c, "The reward could not be claimed right now. Please try again later.");
                return true;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var given = Rewards.Give(c, rec.GetStream(), items, "DownloadReward");
                PlayerMsg.Tell(c, "Download reward: you received " + string.Join(", ", given) + ".");
                MyConsole.WriteLine("[DownloadReward] " + c.Player.Name + " (" + c.Player.UID + ") claimed: " + string.Join(", ", given));
            }
            return true;
        }
    }
}
