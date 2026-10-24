using System;
﻿// ============================================================================
//  AdminApi - [FestTest 2026-10-15] DEVELOPMENT-ONLY test driver for festival scripts (locked in production mode).
//  Lets a local test tool press what a player would press, through the same server hooks the packets use:
//    op "festtest" {"action":"use","uid":N,"id":ITEMTYPE}        right-click an item in the bag (FoolsKit.Kit.OnUse)
//    op "festtest" {"action":"dialog","uid":N,"option":K}        answer the virtual festival dialog the player has open
//    op "festtest" {"action":"npc","uid":N,"npc":NPCID}          click a festival NPC standing in the player`s screen
//  Each call is logged to adminapi_log.txt. Nothing here bypasses the scripts` own checks (items, space, NPC range).
// ============================================================================
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.FoolsKit;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        private static Dictionary<string, object> FestTestOp(AdminArgs a)
        {
            if (!ServerMode.IsDevelopment) throw new AdminError("festtest is a development tool (server is in production mode)");
            var c = FindOnline(a);
            if (c == null) throw new AdminError("player is not online");
            string action = (a.Str("action") ?? "").Trim().ToLowerInvariant();
            bool handled = false; string what;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                if (action == "use")
                {
                    uint id = (uint)a.Long("id", 0);
                    Game.MsgServer.MsgGameItem item = null;
                    foreach (var x in c.Inventory.ClientItems.Values) if (x.ITEM_ID == id) { item = x; break; }
                    if (item == null) throw new AdminError("item " + id + " is not in the bag");
                    handled = Kit.OnUse(c, item, s);
                    what = "use " + id;
                }
                else if (action == "dialog")
                {
                    uint last;
                    if (!Kit.LastDialog.TryGetValue(c.Player.UID, out last)) throw new AdminError("no festival dialog open");
                    byte opt = (byte)a.Long("option", 255);
                    handled = Kit.OnDialog(c, s, last, opt, a.Str("input") ?? "");
                    what = "dialog " + last + " option " + opt;
                }
                else if (action == "npc")
                {
                    uint npc = (uint)a.Long("npc", 0);
                    handled = Kit.OnDialog(c, s, npc, 0, "");
                    what = "npc " + npc;
                }
                else if (action == "hairface")
                {
                    // test setup: {"mode":"remove"|"add","type":9|10,"hid":N} on the player`s HairfaceStorage
                    var t = (Game.MsgServer.MsgHairfaceStorage.Type)a.Long("type", 9); uint hid = (uint)a.Long("hid", 0);
                    string mode = (a.Str("mode") ?? "").ToLowerInvariant();
                    if (mode == "remove") handled = c.HairfaceStorage.Remove(hid, t);
                    else if (mode == "add") handled = PuppetTheater.PuppetEvents.GrantHairface(c, t, hid, 0, "festtest") != "none";
                    else if (mode == "has") handled = c.HairfaceStorage.Exists(hid, t);
                    if (mode == "remove") try { Database.ServerDatabase.SaveHairfaceStorage(c); } catch (Exception err) { MyConsole.SaveException(err); }
                    what = "hairface " + mode + " " + (int)t + ":" + hid;
                }
                else if (action == "realnpc")
                {
                    // [DreamTheater 2026-10-15] the exact client path of an NPC click / dialog answer (MsgNpc -> Procesor queue):
                    // option 0 = click the NPC (must stand in the player`s screen), option N = answer the NPC dialog open now.
                    uint npc = (uint)a.Long("npc", 0);
                    byte opt = (byte)a.Long("option", 0);
                    if (opt != 0 || npc == 0) npc = (uint)c.ActiveNpc;
                    if (opt == 255) throw new AdminError("option 255 closes the dialog on the client only");
                    c.ActiveNpc = npc;
                    Game.MsgNpc.Procesor.ExecuteNpc.Enqueue(new Game.MsgNpc.Procesor.InvokerClient(c, s, npc, 0, opt, a.Str("input") ?? ""));
                    handled = true;
                    what = "realnpc " + npc + " option " + opt;
                }
                else if (action == "kill")
                {
                    // [DreamTheater 2026-10-15] test only: the killing blow of MY Attack\ReceiveAttack\Monster.cs on the monsters in
                    // the player`s screen (same map + private copy), credited to the player (quest kill counters, drops, death).
                    int max = (int)a.Long("count", 1); string name = a.Str("name") ?? "";
                    int n = 0;
                    foreach (var o in c.Map.View.Roles(Role.MapObjectType.Monster, c.Player.X, c.Player.Y).ToList())
                    {
                        if (n >= max) break;
                        var m = o as Game.MsgMonster.MonsterRole;
                        if (m == null || !m.Alive || m.DynamicID != c.Player.DynamicID) continue;
                        if (name.Length > 0 && !string.Equals(m.Name, name, System.StringComparison.OrdinalIgnoreCase)) continue;
                        if (Role.Core.GetDistance(c.Player.X, c.Player.Y, m.X, m.Y) > 18) continue;
                        c.Map.SetMonsterOnTile(m.X, m.Y, false);
                        m.Dead(s, c, c.Player.UID, c.Map);
                        n++;
                    }
                    handled = n > 0;
                    what = "kill " + n + " " + name;
                }
                else if (action == "realuse")
                {
                    // [DreamTheater 2026-10-15] the exact client path of a right-click (MsgItemUsuagePacket.UseItem, all its hooks)
                    uint id = (uint)a.Long("id", 0);
                    Game.MsgServer.MsgGameItem item = null;
                    foreach (var x in c.Inventory.ClientItems.Values) if (x.ITEM_ID == id) { item = x; break; }
                    if (item == null) throw new AdminError("item " + id + " is not in the bag");
                    Game.MsgServer.MsgItemUsuagePacket.UseItem(item, c, s);
                    handled = true;
                    what = "realuse " + id;
                }
                else if (action == "dtday")
                {
                    what = "dtday " + DreamTheater.DreamTheaterModule.TestShiftDays((int)a.Long("days", 0));
                    handled = true;
                }
                else if (action == "dtpoints")
                {
                    what = "dtpoints " + DreamTheater.DreamTheaterModule.TestSetPoints(c, a.Long("points", 0));
                    handled = true;
                }
                else if (action == "rmtitle")
                {
                    // test cleanup: removes one special title (id) the test gave the player
                    var t = (Game.MsgServer.MsgTitleStorage.TitleType)a.Long("title", 0);
                    handled = c.Player.SpecialTitles.Contains(t);
                    if (handled) c.Player.RemoveSpecialTitle(t, s);
                    what = "rmtitle " + t;
                }
                else if (action == "dtsettle")
                {
                    // [DreamTheater 2026-10-15] test only: settle the Dream Theater local ranking now (freeze + ranking mails)
                    what = "dtsettle " + DreamTheater.DreamTheaterModule.TestSettle();
                    handled = true;
                }
                else throw new AdminError("'action' must be use, realuse, dialog, npc, realnpc, kill, dtday, dtsettle or hairface");
            }
            Log("festtest", Who(c), what + " => " + (handled ? "handled" : "not handled"), true);
            var r = Ok(what + (handled ? " handled" : " not handled"));
            r["handled"] = handled;
            return r;
        }
    }
}
