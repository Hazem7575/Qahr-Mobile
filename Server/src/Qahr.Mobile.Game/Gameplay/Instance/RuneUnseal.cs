using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public static class RuneUnseal
    {
        public const uint PermanentFlag = 4;
        private const MsgResonanceRune.ActionID SyncAction = MsgResonanceRune.ActionID.unk1;
        private static readonly DateTime Epoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public sealed class MaterialOption
        {
            public byte Index;
            public uint FailPoints;
            public readonly Dictionary<uint, uint> Items = new Dictionary<uint, uint>();
        }
        public static readonly Dictionary<uint, uint[]> IdealComponents = new Dictionary<uint, uint[]>()
        {
            { 4031709, new uint[] { 4035109, 4034709 } }, // Indestructible Balance = NatureShield + UniversalShield
            { 4031809, new uint[] { 4032909, 4038509 } }, // Shadow Flow = Overwhelm + Seethrough
            { 4037909, new uint[] { 4032309, 4036009 } }, // Supreme Armor = Wonder + Circle of Life
            { 4038009, new uint[] { 4035509, 4035909 } }, // Tough Steel = Conqueror`s Blade + Fearless
            { 4039009, new uint[] { 4031109, 4030609 } }, // XP Devourer = XPBooster + XPKiller
            { 4039109, new uint[] { 4034509, 4035409 } }, // Unbreakable = MiracleTouch + Solidness
            { 4039509, new uint[] { 4032709, 4034209 } }, // Cosmic Balance = Sharpness + Nature`s Chant
            { 4070309, new uint[] { 4034809, 4030109 } }, // Tortoise Predator = TortoiseSmasher + TortoiseBreaker
            { 4070609, new uint[] { 4037709, 4038309 } }, // Invincible Resolve = Unswerving + Iron Bone
            { 4072109, new uint[] { 4071009, 4071109 } }, // Deathhold = Area Occupier + Equalizer
            { 4072609, new uint[] { 4030909, 4031509 } }, // Battle Reaper = BossKiller + Breakdown
            { 4072709, new uint[] { 4035009, 4037409 } }, // Peakcarver = StrikeBooster + Wild Cleave
            { 4072809, new uint[] { 4035809, 4035709 } }, // Honed Soul = Tempered Glaive + Whetted Blade
            { 4073309, new uint[] { 4071509, 4072009 } }, // Soulrazor = Velocity Bane + Soul Annihilation
            { 4073409, new uint[] { 4072909, 4032009 } }, // Sanctified Pierce = Damage Pierce + Sanctity
        };
        public static double SuccessRate = 1; // Rate
        public static uint TemporaryThreshold = 8400;
        public static uint DecayPerMinute = 200;
        public static Dictionary<byte, MaterialOption> Options = CreateDefaultOptions();
        private static Dictionary<byte, MaterialOption> CreateDefaultOptions()
        {
            // Client ini/luacfg/RuneSys.dat, table PoFengMeterial. The client refuses to send the request unless the
            // player owns exactly these, so the server must ask for the same items.
            // MaterialSel1 is option 1 (tip: failure adds 710), MaterialSel2 is option 2 (tip: failure adds 840).
            var options = new Dictionary<byte, MaterialOption>();
            var first = new MaterialOption() { Index = 1, FailPoints = 710 };
            first.Items[3324226] = 3;   // Cosmos Hammer
            first.Items[4050001] = 500; // Yellow Rune Essence
            options[1] = first;
            var second = new MaterialOption() { Index = 2, FailPoints = 840 };
            second.Items[3324226] = 2;   // Cosmos Hammer
            second.Items[4060001] = 700; // Universal Rune Essence
            options[2] = second;
            return options;
        }

        /// rune_unseal.txt (optional):
        ///   SuccessRate=1
        ///   TemporaryThreshold=8400
        ///   DecayPerMinute=200
        ///   Option1=710|3324226:3|4050001:500

        public static void Load()
        {
            Options = CreateDefaultOptions();
            string path = Program.ServerConfig.DbLocation + "rune_unseal.txt";
            if (!File.Exists(path))
                return;
            var loaded = new Dictionary<byte, MaterialOption>();
            foreach (string raw in File.ReadAllLines(path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//"))
                    continue;
                int separator = line.IndexOf('=');
                if (separator <= 0)
                    continue;
                string key = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim();
                try
                {
                    if (key.Equals("SuccessRate", StringComparison.OrdinalIgnoreCase))
                        SuccessRate = double.Parse(value, CultureInfo.InvariantCulture);
                    else if (key.Equals("TemporaryThreshold", StringComparison.OrdinalIgnoreCase))
                        TemporaryThreshold = uint.Parse(value);
                    else if (key.Equals("DecayPerMinute", StringComparison.OrdinalIgnoreCase))
                        DecayPerMinute = uint.Parse(value);
                    else if (key.StartsWith("Option", StringComparison.OrdinalIgnoreCase))
                    {
                        var option = new MaterialOption() { Index = byte.Parse(key.Substring(6)) };
                        string[] parts = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        option.FailPoints = uint.Parse(parts[0].Trim());
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] pair = parts[i].Split(':');
                            option.Items[uint.Parse(pair[0].Trim())] = uint.Parse(pair[1].Trim());
                        }
                        loaded[option.Index] = option;
                    }
                }
                catch (Exception e)
                {
                    Log("Bad line in rune_unseal.txt: " + raw + " (" + e.Message + ")");
                }
            }
            if (loaded.Count > 0)
                Options = loaded;
        }
        public static bool IsIdealRune(uint itemId)
        {
            return IdealComponents.ContainsKey(itemId);
        }

        private static uint GetSlot(MsgGameItem item, int index)
        {
            if (item.RelicAttributes == null || item.RelicAttributes.Length <= index)
                return 0;
            return item.RelicAttributes[index].Data;
        }
        private static void SetSlot(MsgGameItem item, int index, uint value)
        {
            if (item.RelicAttributes == null || item.RelicAttributes.Length < 5)
            {
                var slots = new RelicAttribute[5];
                if (item.RelicAttributes != null)
                    Array.Copy(item.RelicAttributes, slots, item.RelicAttributes.Length);
                item.RelicAttributes = slots;
            }
            item.RelicAttributes[index].Data = value;
        }
        public static uint GetPoints(MsgGameItem item) { return GetSlot(item, 0); }
        public static uint GetFlags(MsgGameItem item) { return GetSlot(item, 1); }

        public static bool IsPermanent(MsgGameItem item)
        {
            return (GetFlags(item) & PermanentFlag) != 0;
        }
        public static bool IsUnsealed(MsgGameItem item)
        {
            return item != null && IsIdealRune(item.ITEM_ID) && (IsPermanent(item) || GetPoints(item) >= TemporaryThreshold);
        }
        public static bool HasComponents(Client.GameClient client, uint idealItemId)
        {
            uint[] parts;
            if (client == null || client.Rune == null || !IdealComponents.TryGetValue(idealItemId, out parts))
                return false;
            bool first = false, second = false;
            foreach (var owned in client.Rune.Objects)
            {
                if (owned == null)
                    continue;
                if (owned.ITEM_ID == parts[0])
                    first = true;
                else if (owned.ITEM_ID == parts[1])
                    second = true;
            }
            return first && second;
        }
        public static bool IsActive(Client.GameClient client, MsgGameItem item)
        {
            return IsUnsealed(item) && HasComponents(client, item.ITEM_ID);
        }
        private static uint NowMinute()
        {
            return (uint)(DateTime.UtcNow - Epoch).TotalMinutes;
        }
        private static string ItemName(uint itemId)
        {
            Database.ItemType.DBItem db;
            return Pool.ItemsBase.TryGetValue(itemId, out db) ? db.Name : itemId.ToString();
        }
        public static void HandleRequest(Client.GameClient client, ServerSockets.Packet stream, MsgResonanceRune.MsgResonanceProto request)
        {
            if (client == null || client.Player == null || client.Rune == null || request == null)
                return;

            if (client.IsConnectedInterServer())
            {
                client.SendSysMesage("Function unavailable in cross-server state.");
                return;
            }

            Log(client.Player.Name + " request: rune UID " + request.UID + ", option " + request.Unk4
                + ", client stacks: " + DescribeStacks(client, request.Values));

            MsgGameItem rune = FindIdealRune(client, (uint)request.UID);
            if (rune == null)
            {
                Log(client.Player.Name + " rejected: Ideal Rune UID " + request.UID + " is not in the Rune warehouse.");
                client.CreateBoxDialog("Select Ideal Runes to unseal.");
                return;
            }

            MaterialOption option;
            if (!Options.TryGetValue((byte)request.Unk4, out option))
            {
                Log(client.Player.Name + " rejected: unknown material option " + request.Unk4 + ".");
                client.CreateBoxDialog("Materials cannot be empty.");
                return;
            }

            bool wasUnsealed = IsUnsealed(rune);
            try
            {
                lock (client.Rune)
                    TryUnseal(client, stream, rune, option, request);
            }
            finally
            {
                client.Rune.InvalidateIdealCache();
                if (IsUnsealed(rune) != wasUnsealed)
                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
        }
        private static void TryUnseal(Client.GameClient client, ServerSockets.Packet stream, MsgGameItem rune, MaterialOption option,
            MsgResonanceRune.MsgResonanceProto request)
        {
            ApplyDecay(client, rune, stream);

            if (IsPermanent(rune))
            {
                Log(client.Player.Name + " rejected: " + ItemName(rune.ITEM_ID) + " is already permanently unsealed.");
                client.CreateBoxDialog("The corresponding Ideal Rune is in an Unsealing state and takes effect without being equipped.");
                return;
            }

            Dictionary<uint, uint> plan;
            string missing;
            if (!TryPlanMaterials(client, option, request.Values, out plan, out missing))
            {
                Log(client.Player.Name + " rejected: option " + option.Index + " missing " + missing + ".");
                client.CreateBoxDialog("You don`t have enough materials: " + missing + ".");
                return;
            }
            foreach (var use in plan)
                client.Inventory.RemoveStackItem(use.Key, (ushort)use.Value, stream);

            if (Roll(SuccessRate))
            {
                Log(client.Player.Name + " permanently unsealed " + ItemName(rune.ITEM_ID) + " (UID " + rune.UID + ").");
                SetSlot(rune, 1, GetFlags(rune) | PermanentFlag);
                SendSync(client, stream, rune);
                client.SendSysMesage("Unseal Ideal Rune successfully. After Unsealing, it takes effect without being equipped.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                Server.SendGlobalPacket(new MsgMessage("Congratulations, to [ " + client.Player.Name
                    + " ] for permanently unsealing the Ideal Rune " + ItemName(rune.ITEM_ID) + ", reaching the pinnacle of achievement!",
                    MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            }
            else
            {
                uint points = GetPoints(rune);
                if (points == 0)
                    SetSlot(rune, 2, NowMinute());
                SetSlot(rune, 0, (uint)Math.Min(uint.MaxValue, (ulong)points + option.FailPoints));
                Log(client.Player.Name + " failed on " + ItemName(rune.ITEM_ID) + ": points " + points + " -> " + GetPoints(rune) + ".");
                SendSync(client, stream, rune);
            }
        }
        private static void Log(string text)
        {
            // MyConsole.WriteLine("[RuneUnseal] " + text);
            try
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(folder);
                File.AppendAllText(Path.Combine(folder, "rune_unseal.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text + Environment.NewLine);
            }
            catch
            {
            }
        }
        private static MsgGameItem FindIdealRune(Client.GameClient client, uint key)
        {
            MsgGameItem item;
            if (key != 0 && client.Rune.TryGetItem(key, out item) && IsIdealRune(item.ITEM_ID))
                return item;
            if (IsIdealRune(key) && client.Rune.TryGetItem2(key, out item))
                return item;
            return null;
        }
        private static bool TryPlanMaterials(Client.GameClient client, MaterialOption option, List<MsgResonanceRune.RuneValue> picked,
            out Dictionary<uint, uint> plan, out string missing)
        {
            plan = new Dictionary<uint, uint>();
            var remaining = new Dictionary<uint, uint>(option.Items);
            if (picked != null)
                foreach (var stack in picked)
                    if (stack != null)
                        Take(client, stack.Key, stack.Value, remaining, plan);
            foreach (var itemId in remaining.Keys.ToArray())
                foreach (var stack in client.Inventory.ClientItems.Values)
                    if (stack.ITEM_ID == itemId)
                        Take(client, stack.UID, uint.MaxValue, remaining, plan);

            missing = string.Join(", ", remaining.Where(r => r.Value > 0).Select(r => ItemName(r.Key) + " x" + r.Value).ToArray());
            return missing.Length == 0;
        }
        private static void Take(Client.GameClient client, uint uid, uint wanted, Dictionary<uint, uint> remaining, Dictionary<uint, uint> plan)
        {
            MsgGameItem stack;
            uint need;
            if (!client.Inventory.TryGetItem(uid, out stack) || !remaining.TryGetValue(stack.ITEM_ID, out need) || need == 0)
                return;
            uint used;
            plan.TryGetValue(uid, out used);
            uint available = Math.Max((uint)1, (uint)stack.StackSize);
            if (available <= used)
                return;
            uint take = Math.Min(Math.Min(need, wanted), available - used);
            if (take == 0)
                return;
            remaining[stack.ITEM_ID] = need - take;
            plan[uid] = used + take;
        }

        private static string DescribeStacks(Client.GameClient client, List<MsgResonanceRune.RuneValue> picked)
        {
            if (picked == null || picked.Count == 0)
                return "nothing";
            return string.Join(", ", picked.Where(p => p != null).Select(p =>
            {
                MsgGameItem stack;
                return client.Inventory.TryGetItem(p.Key, out stack) ? stack.ITEM_ID + " x" + p.Value : "UID " + p.Key + " x" + p.Value;
            }).ToArray());
        }
        private static bool Roll(double percent)
        {
            if (percent <= 0)
                return false;
            if (percent >= 100)
                return true;
            return Pool.GetRandom.Next(0, 1000000) < (int)(percent * 10000);
        }
        public static void SendSync(Client.GameClient client, ServerSockets.Packet stream, MsgGameItem rune)
        {
            var packet = new MsgResonanceRune.MsgResonanceProto();
            packet.Type = SyncAction;
            packet.UID = GetFlags(rune);
            packet.Values = new List<MsgResonanceRune.RuneValue>();
            packet.Values.Add(new MsgResonanceRune.RuneValue() { Key = rune.UID, Value = GetPoints(rune) });
            client.Send(stream.CreateRuneResonance(packet));

            rune.Mode = Role.Flags.ItemMode.Update;
            rune.Send(client, stream);
        }
        public static void Tick(Client.GameClient client)
        {
            if (client == null || client.Rune == null || client.Player == null)
                return;
            int now = Environment.TickCount;
            if (client.Rune.UnsealTickStamp != 0 && unchecked(now - client.Rune.UnsealTickStamp) < 5000)
                return;
            client.Rune.UnsealTickStamp = now;

            bool stateChanged = false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                lock (client.Rune)
                {
                    foreach (var item in client.Rune.Objects)
                        if (item != null && IsIdealRune(item.ITEM_ID))
                            stateChanged |= ApplyDecay(client, item, stream);
                }
            }
            if (stateChanged)
            {
                client.Rune.InvalidateIdealCache();
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
            }
        }
        private static bool ApplyDecay(Client.GameClient client, MsgGameItem item, ServerSockets.Packet stream)
        {
            uint points = GetPoints(item);
            if (points == 0 || IsPermanent(item))
                return false;

            uint nowMinute = NowMinute();
            uint last = GetSlot(item, 2);
            if (last == 0 || last > nowMinute)
            {
                SetSlot(item, 2, nowMinute);
                return false;
            }
            uint elapsed = nowMinute - last;
            if (elapsed == 0)
                return false;

            ulong loss = (ulong)elapsed * DecayPerMinute;
            uint newPoints = loss >= points ? 0 : (uint)(points - loss);
            SetSlot(item, 0, newPoints);
            SetSlot(item, 2, nowMinute);
            SendSync(client, stream, item);

            bool expired = points >= TemporaryThreshold && newPoints < TemporaryThreshold;
            if (expired)
                client.SendSysMesage(ItemName(item.ITEM_ID) + "`s Unsealing status has expired!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            return expired;
        }
    }
}
