// ============================================================================
//  Easy GM commands - short, simple chat commands for staff (typed in the game chat).
//
//  Every command works on yourself; add a player name at the end to do it to someone else.
//    @help                      this list
//    @cps 1000 [name]           add CPs                 @money 1000000 [name]   add silver
//    @level 140 [name]          set level               @reborn 2 [name]        set reborn (0-2)
//    @item 1088000 [count] [name]  give an item         @find dragon            search item ids by name
//    @heal [name]               revive / full HP+MP     @vip 6 [days] [name]    VIP level (0 = remove)
//    @skill 1045 [level] [name] learn a skill           @clearbag [name]        empty the bag
//    @vippts 100 [name]         add VIP points (Mobile\Vip\VipPoints.cs)
//    @go twin | market | hall   go to a place (Data\places.txt)   @go 1002 451 365   go to map x y
//    @goto name                 go to a player          @bring name             bring a player to you
//    @kick name                 kick a player           @say text               message to everyone
//    @online                    who is online           @save                   save everyone
//
//  Runs through the same AdminApi ops as admin.py / the dashboard (same checks, same log file).
//  Wrong arguments fall through to the old ChatCommands block, so its own commands keep working.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        private static readonly string[] EasyHelp =
        {
            "@cps 1000 | @money 1000000 | @level 140 | @reborn 2",
            "@item 1088000 [count] | @find dragon | @heal | @vip 6 [days] | @vippts 100",
            "@skill 1045 [level] | @clearbag",
            "@go twin/market/hall | @go 1002 451 365 | @goto name | @bring name",
            "@kick name | @say text | @online | @save",
            "Add a player name at the end to do it to someone else.",
        };

        /// <summary>Called first from MsgMessage.ChatCommands. True = handled.</summary>
        public static bool EasyCommand(Client.GameClient c, string text)
        {
            if (string.IsNullOrEmpty(text) || text[0] != '@' || !CanUseGmCommands(c)) return false;
            var p = text.Substring(1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length == 0) return false;
            string cmd = p[0].ToLowerInvariant();
            var args = p.Skip(1).ToList();

            Dictionary<string, object> r;
            try
            {
                r = EasyRun(c, cmd, args);
            }
            catch (AdminError e)
            {
                r = Fail(e.Message);
            }
            catch (FormatException)
            {
                return false; // not our syntax: let the old command with the same name try
            }
            if (r == null) return false;

            object v;
            bool ok = r.TryGetValue("ok", out v) && v is bool && (bool)v;
            string reply = ok ? (r.TryGetValue("message", out v) && v != null ? v.ToString() : "done")
                              : "Error: " + (r.TryGetValue("error", out v) ? v : "failed");
            EasyReply(c, reply);
            return true;
        }

        private static Dictionary<string, object> EasyRun(Client.GameClient c, string cmd, List<string> args)
        {
            switch (cmd)
            {
                case "help":
                    foreach (var line in EasyHelp) EasyReply(c, line);
                    return Ok("That's all the easy commands.");
                case "cps": return Dispatch("cps", EasyArgs(c, args, 1, "amount", "mode=add"));
                case "money": return Dispatch("money", EasyArgs(c, args, 1, "amount", "mode=add"));
                case "level": return Dispatch("setlevel", EasyArgs(c, args, 1, "level"));
                case "reborn": return Dispatch("reborn", EasyArgs(c, args, 1, "count"));
                case "vip":
                    {
                        // @vip = the VIP window (as for every player); @vip level [days] [name] = set a timed VIP
                        if (args.Count == 0) return null;
                        int numbers = args.Count >= 2 && IsNumber(args[1]) ? 2 : 1;
                        return Dispatch("vip", EasyArgs(c, args, numbers, "level", "days"));
                    }
                case "vippts":
                    {
                        // @vippts 100 [name] = add VIP points (raises the permanent VIP level like a VIP card)
                        var a = EasyArgs(c, args, 1, "amount");
                        int amount = int.Parse(a.Raw["amount"].ToString());
                        var who = FindOnline(a);
                        if (who == null) throw new AdminError("player is not online");
                        long total = Phone.VipPoints.AddPoints(who, amount, "gm " + c.Player.Name);
                        return Ok(who.Player.Name + " has " + total + " VIP points (VIP " + Phone.VipPoints.LevelFor(total) + ").");
                    }
                case "heal": return Dispatch("revive", EasyArgs(c, args, 0));
                case "clearbag": return Dispatch("clearinventory", EasyArgs(c, args, 0));
                case "save": return Dispatch("save", EasyArgs(c, args, 0, "all=1"));
                case "online": return EasyOnline();
                case "find": return EasyFind(args);
                case "item":
                    {
                        // @item id [count] [name]
                        if (args.Count == 0) throw new AdminError("write: @item 1088000 [count] [name]");
                        int numbers = args.Count >= 2 && IsNumber(args[1]) ? 2 : 1;
                        return Dispatch("giveitem", EasyArgs(c, args, numbers, "id", "amount"));
                    }
                case "skill":
                    {
                        int numbers = args.Count >= 2 && IsNumber(args[1]) ? 2 : 1;
                        var a = EasyArgs(c, args, numbers, "skill", "level");
                        if (!a.Has("level")) a.Raw["level"] = "0";
                        return Dispatch("learnskill", a);
                    }
                case "go":
                    {
                        Place place;
                        if (args.Count >= 1 && Places.TryGet(args[0], out place))
                            return Dispatch("teleport", EasyArgs(c, new List<string> { place.Map.ToString(), place.X.ToString(), place.Y.ToString() }, 3, "map", "x", "y"));
                        if (args.Count < 3) throw new AdminError("write: @go " + string.Join(" / ", Places.Names) + ", or @go map x y");
                        return Dispatch("teleport", EasyArgs(c, args, 3, "map", "x", "y"));
                    }
                case "goto":
                case "bring":
                    {
                        if (args.Count != 1) throw new AdminError("write: @" + cmd + " playername");
                        var other = FindOnline(new AdminArgs(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "name", args[0] } }));
                        if (other == null) throw new AdminError(args[0] + " is not online");
                        var mover = cmd == "goto" ? c : other;
                        var target = cmd == "goto" ? other : c;
                        return Dispatch("bring", new AdminArgs(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                            { { "uid", mover.Player.UID.ToString() }, { "toUid", target.Player.UID.ToString() } }));
                    }
                case "kick":
                    if (args.Count != 1) throw new AdminError("write: @kick playername");
                    return Dispatch("kick", new AdminArgs(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        { { "name", args[0] }, { "reason", "kicked by " + c.Player.Name } }));
                case "say":
                    if (args.Count == 0) throw new AdminError("write: @say your message");
                    return Dispatch("broadcast", new AdminArgs(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        { { "text", string.Join(" ", args) }, { "mode", "center" } }));
                case "lua":
                    // UI testing: a raw client Lua call, e.g. @lua Fgui_CumulativeAward_Open|{['Code']=200,...}
                    if (args.Count == 0) throw new AdminError("write: @lua Function|arguments");
                    LuaUi.LuaPush.Send(c, string.Join(" ", args));
                    return Ok("Lua call sent.");
            }
            return null; // not an easy command
        }

        /// <summary>The first <paramref name="numbers"/> words fill <paramref name="keys"/> in order (they must be
        /// numbers); one more word = the target player name, otherwise the GM. "key=value" keys are fixed values.</summary>
        private static AdminArgs EasyArgs(Client.GameClient c, List<string> args, int numbers, params string[] keys)
        {
            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var names = keys.Where(k => !k.Contains("=")).ToList();
            foreach (var k in keys.Where(k => k.Contains("=")))
                d[k.Substring(0, k.IndexOf('='))] = k.Substring(k.IndexOf('=') + 1);
            if (args.Count < numbers) throw new AdminError("missing number - type @help");
            for (int i = 0; i < numbers; i++)
            {
                if (!IsNumber(args[i])) throw new FormatException();
                d[names[i]] = args[i];
            }
            if (args.Count > numbers + 1) throw new FormatException();
            if (args.Count == numbers + 1) d["name"] = args[numbers];
            else d["uid"] = c.Player.UID.ToString();
            return new AdminArgs(d);
        }

        private static bool IsNumber(string s)
        {
            long n;
            return long.TryParse(s, out n);
        }

        private static Dictionary<string, object> EasyOnline()
        {
            var names = OnlineClients().Select(x => x.Player.Name).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
            return Ok(names.Count + " online: " + string.Join(", ", names.Take(30)) + (names.Count > 30 ? " ..." : ""));
        }

        private static Dictionary<string, object> EasyFind(List<string> args)
        {
            string q = string.Join(" ", args).Trim();
            if (q.Length < 2) throw new AdminError("write: @find part of the item name");
            if (Pool.ItemsBase == null) throw new AdminError("items are not loaded");
            var hits = Pool.ItemsBase.Values
                .Where(i => i != null && i.Name != null && i.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(i => i.Name.Length).ThenBy(i => i.ID)
                .Take(8)
                .Select(i => i.ID + " " + i.Name)
                .ToList();
            if (hits.Count == 0) throw new AdminError("no item name contains '" + q + "'");
            return Ok(string.Join(" | ", hits));
        }

        private static void EasyReply(Client.GameClient c, string text)
        {
            c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
        }
    }
}
