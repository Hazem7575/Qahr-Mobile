// ============================================================================
//  AdminApi - [Festivals] ops (Events\Framework\FestivalManager.cs).
//
//  op "festivals"        -> {"ok":true,"festivals":[{key,name_ar,name_en,valid,enabled,running,override,schedule,due,
//                             nextStart,nextEnd,npcs,monsterTypes,spawns,quests,exchanges,started?,run?,liveNpcs?,
//                             liveMonsters?,errors[],warnings[]}]}
//  op "festival"         -> {"key":"SampleLantern","action":"on"|"off"|"auto"|"info"}  (on/off = manual override, kept in
//                             Data\Festivals\_state.json; auto = back to the schedule). Applied live.
//  op "reloadfestivals"  -> re-reads Data\Festivals\*\ (a running festival whose folder changed is restarted).
// ============================================================================
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        private static Dictionary<string, object> FestivalsList()
        {
            var d = Ok();
            d["festivals"] = global::Qahr.Mobile.Festivals.FestivalManager.ListJson();
            return d;
        }

        private static Dictionary<string, object> FestivalOp(AdminArgs a)
        {
            string key = a.Str("key");
            string action = (a.Str("action") ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(key)) throw new AdminError("'key' is required");
            if (action != "on" && action != "off" && action != "auto" && action != "info") throw new AdminError("'action' must be on, off, auto or info");
            if (global::Qahr.Mobile.Festivals.FestivalManager.Find(key) == null) throw new AdminError("unknown festival '" + key + "'");
            var lines = global::Qahr.Mobile.Festivals.FestivalManager.Command(new[] { action, key }, "AdminApi");
            var f = global::Qahr.Mobile.Festivals.FestivalManager.Find(key);
            var d = Ok(lines.Count > 0 ? lines[lines.Count - 1] : "done");
            d["lines"] = lines.Cast<object>().ToList();
            d["running"] = f != null && global::Qahr.Mobile.Festivals.FestivalManager.IsRunning(f.Key);
            if (action != "info") Log("festival", key, action + ": " + (lines.Count > 0 ? lines[lines.Count - 1] : ""), true);
            return d;
        }

        private static Dictionary<string, object> FestivalsReload()
        {
            var lines = global::Qahr.Mobile.Festivals.FestivalManager.Command(new[] { "reload" }, "AdminApi");
            var d = Ok(lines.FirstOrDefault(l => l.StartsWith("Festivals reloaded")) ?? "reloaded");
            d["lines"] = lines.Cast<object>().ToList();
            d["festivals"] = global::Qahr.Mobile.Festivals.FestivalManager.ListJson();
            Log("reloadfestivals", "-", d["message"].ToString(), true);
            return d;
        }
    }
}
