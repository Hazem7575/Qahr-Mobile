using System;
using System.Collections.Generic;

namespace Qahr.Mobile.Phone
{
    /// <summary>One item of a reward: "id", "id:count" or "id:count:b" (b = bound) in the Data\mobile_*.txt files.</summary>
    public sealed class RewardItem
    {
        public uint Id;
        public ushort Count = 1;
        public bool Bound;
    }

    /// <summary>
    /// QahrMobile: the shared way to check and pay item rewards (Welfare, daily tasks, main story, bounties).
    /// Always CanGive first (item known + bag room), then Give, so a reward is never half paid.
    /// </summary>
    public static class Rewards
    {
        /// <summary>Bag size of the mobile client; Inventory.HaveSpace takes at most this many slots.</summary>
        public const int BagSize = 40;

        /// <summary>Parses the item tokens of a data line from index <paramref name="from"/>.</summary>
        public static List<RewardItem> Parse(string[] tokens, int from)
        {
            var items = new List<RewardItem>();
            for (int i = from; i < tokens.Length; i++)
            {
                var f = tokens[i].Split(':');
                items.Add(new RewardItem
                {
                    Id = uint.Parse(f[0]),
                    Count = f.Length > 1 ? ushort.Parse(f[1]) : (ushort)1,
                    Bound = f.Length > 2 && f[2].StartsWith("b", StringComparison.OrdinalIgnoreCase),
                });
            }
            return items;
        }

        /// <summary>Bag slots <paramref name="count"/> of an item take as new stacks (0 for an unknown item).</summary>
        public static int Slots(uint itemId, int count)
        {
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(itemId, out db)) return 0;
            return db.StackSize > 1 ? (count + db.StackSize - 1) / db.StackSize : count;
        }

        public static int Slots(IEnumerable<RewardItem> items)
        {
            int slots = 0;
            foreach (var i in items) slots += Slots(i.Id, i.Count);
            return slots;
        }

        public static bool HasRoom(Client.GameClient c, int slots)
        {
            return slots <= 0 || c.Inventory.HaveSpace((byte)Math.Min(BagSize, slots));
        }

        /// <summary>
        /// Every item exists on the server and the bag has room. Otherwise tells the player why, followed by
        /// <paramref name="nothingTaken"/> (e.g. "Nothing was charged."), and returns false.
        /// </summary>
        public static bool CanGive(Client.GameClient c, List<RewardItem> items, string tag, string nothingTaken)
        {
            foreach (var i in items)
            {
                if (Pool.ItemsBase.ContainsKey(i.Id)) continue;
                PlayerMsg.Tell(c, "This reward is not available yet (item " + i.Id + " is missing on the server). " + nothingTaken);
                MyConsole.WriteLine("[" + tag + "] item " + i.Id + " is not in itemtype.txt", ConsoleColor.Red);
                return false;
            }
            int slots = Slots(items);
            if (!HasRoom(c, slots))
            {
                PlayerMsg.Tell(c, "Please make " + slots + " free space(s) in your inventory first. " + nothingTaken);
                return false;
            }
            return true;
        }

        /// <summary>Adds the items to the bag; returns "Name xN" of what was given (for the reward message).</summary>
        public static List<string> Give(Client.GameClient c, ServerSockets.Packet stream, List<RewardItem> items, string tag)
        {
            var given = new List<string>();
            foreach (var i in items)
            {
                try
                {
                    if (c.Inventory.AddItemWitchStack(i.Id, 0, i.Count, stream, i.Bound))
                        given.Add(Pool.ItemsBase[i.Id].Name + (i.Count > 1 ? " x" + i.Count : ""));
                    else
                        MyConsole.WriteLine("[" + tag + "] could not give " + i.Id + " x" + i.Count + " to " + c.Player.Name, ConsoleColor.Red);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[" + tag + "] give " + i.Id + " to " + c.Player.Name + ": " + e.Message, ConsoleColor.Red);
                }
            }
            return given;
        }
    }

    /// <summary>QahrMobile: a yellow system line in the player's chat (replies, reward notices).</summary>
    public static class PlayerMsg
    {
        public static void Tell(Client.GameClient c, string text)
        {
            if (c == null || c.Player == null) return;
            c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
        }
    }
}
