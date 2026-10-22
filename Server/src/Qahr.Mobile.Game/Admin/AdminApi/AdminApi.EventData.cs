// ============================================================================
//  AdminApi - [EventData 2026-10-15] ops on the unified event/festival store (Base\EventData\EventStore.cs, MySQL).
//
//  op "eventdata" {"action":"get","uid":N[,"event":"Key"]}
//      -> {"ok":true,"uid":N,"available":true,"events":{"Key":{"c":{},"f":[],"cl":{},"t":{},"s":{}}}}
//  op "eventdata" {"action":"set","uid":N,"event":"Key","key":"c.name|f.name|cl.name|t.name|s.name","value":"..."}
//      value "" = delete. Written to MySQL before the reply; logged to adminapi_log.txt with the old value.
// ============================================================================
using System.Collections.Generic;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        private static Dictionary<string, object> EventDataOp(AdminArgs a)
        {
            string action = (a.Str("action") ?? "get").Trim().ToLowerInvariant();
            long uidL = a.Long("uid", -1);
            if (uidL < 0 || uidL > uint.MaxValue) throw new AdminError("'uid' is required (0 = server-wide sections)");
            uint uid = (uint)uidL;
            string ev = (a.Str("event") ?? "").Trim();
            if (action == "get")
            {
                var d = Ok();
                foreach (var kv in global::Qahr.Mobile.EventData.EventStore.AdminDump(uid, ev)) d[kv.Key] = kv.Value;
                return d;
            }
            if (action != "set") throw new AdminError("'action' must be get or set");
            string key = (a.Str("key") ?? "").Trim();
            string value = a.Has("value") ? a.Str("value") ?? "" : "";
            if (ev.Length == 0) throw new AdminError("'event' is required");
            string old;
            try { old = global::Qahr.Mobile.EventData.EventStore.AdminSet(uid, ev, key, value); }
            catch (System.ArgumentException e) { Log("eventdata", "uid=" + uid, "set " + ev + " " + key + " REFUSED: " + e.Message, false); throw new AdminError(e.Message); }
            catch (global::Qahr.Mobile.EventData.EventStore.EditFailed e) { Log("eventdata", "uid=" + uid, "set " + ev + " " + key + " FAILED: " + e.Message, false); throw new AdminError("MySQL write failed: " + e.Message); }
            Log("eventdata", "uid=" + uid, "set " + ev + " " + key + " '" + old + "' -> '" + value + "'", true);
            var r = Ok("saved");
            r["old"] = old;
            foreach (var kv in global::Qahr.Mobile.EventData.EventStore.AdminDump(uid, ev)) r[kv.Key] = kv.Value;
            return r;
        }
    }
}
