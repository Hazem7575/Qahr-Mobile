using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        /// <summary>
        /// "nearby": the monsters on the player's map within "radius" tiles (default 40) as the server sees them,
        /// nearest first (at most "max", default 30). For checking what a client should be shown (QahrMobile tests).
        /// </summary>
        private static Dictionary<string, object> Nearby(AdminArgs a)
        {
            if (!ServerMode.IsDevelopment) throw new AdminError("nearby is a development tool (server is in production mode)");// QahrMobile: test helper, development mode only
            var c = FindOnline(a);
            int radius = (int)a.Long("radius", 40), max = (int)a.Long("max", 30);
            var list = new List<object>();
            int total = 0;
            foreach (var o in c.Map.View.GetAllMapRoles(Role.MapObjectType.Monster).ToArray()
                .Select(o => new { o, d = Role.Core.GetDistance(c.Player.X, c.Player.Y, o.X, o.Y) })
                .Where(e => e.d <= radius)
                .OrderBy(e => e.d))
            {
                total++;
                if (list.Count >= max) continue;
                var m = o.o as Game.MsgMonster.MonsterRole;
                list.Add(new Dictionary<string, object>
                {
                    { "uid", o.o.UID }, { "name", m != null ? m.Name : "?" }, { "x", (int)o.o.X }, { "y", (int)o.o.Y },
                    { "distance", (int)o.d }, { "alive", m != null && m.Alive }, { "type", m != null ? (object)m.Family.ID : 0 },
                    { "dynamicId", o.o.DynamicID }
                });
            }
            var r = Ok(total + " monsters within " + radius + " tiles of " + c.Player.Name + " (" + c.Player.Map + " " + c.Player.X + "," + c.Player.Y + ")");
            r["monsters"] = list;
            return r;
        }

        /// <summary>
        /// "createguild" name=player guild=GuildName: makes the player the leader of a new guild, the same way the
        /// guild NPC does (Role.Instance.Guild ctor) without its level / money / nobility checks. For tests.
        /// </summary>
        private static Dictionary<string, object> CreateGuild(AdminArgs a)
        {
            if (!ServerMode.IsDevelopment) throw new AdminError("createguild is a development tool (server is in production mode)");// QahrMobile: test helper, development mode only
            var c = FindOnline(a);
            string guild = (a.Str("guild") ?? "").Trim();
            if (guild.Length == 0) throw new AdminError("guild= is required");
            if (c.Player.MyGuild != null) throw new AdminError(c.Player.Name + " is already in guild " + c.Player.MyGuild.GuildName);
            if (!Role.Instance.Guild.AllowToCreate(guild)) throw new AdminError("the name " + guild + " is taken or not allowed");
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                new Role.Instance.Guild(c, guild, stream);
            }
            return Ok(c.Player.Name + " now leads guild " + guild);
        }
    }
}
