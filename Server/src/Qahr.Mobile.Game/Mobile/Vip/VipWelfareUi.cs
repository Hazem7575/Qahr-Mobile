using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: VIP levels in the client's own Welfare window, tab "Credit Rebate" (CDlgTopUpPrivilege, gui 4013 inside
    /// Welfare 646): one row per VIP level with its badge, reward items, progress "points/needed" and Claim / claimed stamp.
    /// No client change:
    ///  - Lua (sent once per session, re-applied before every use because the client reloads ini/topupprivilege.lua):
    ///    shows the tab (Fund_GetActClose 3), points it at the activity of Data\mobile_vip_levels.txt ("window" line) and
    ///    gives each row the VIP badge (fuli_tqdjNpic) and the reward items of that level.
    ///  - MsgOperatingActInfo 2004 type 0 (login): the activity + "quiet" ids (flag 2) so the client's query fits in one
    ///    packet; type 1: {activity, claimed levels, VIP points}. The row thresholds come from the client's
    ///    ini/operating_prize.dat for that activity, so the level points in the data file must be the same numbers.
    ///  - The client asks with MsgOperatingAct 2421 type 0 when the tab opens and claims with type 2 (B6OperatingAct routes
    ///    both here); every answer is type 1 + a redraw of 4013 (PostCmd 0xcf0 = refresh without clearing).
    /// Opened from the VIP menu (VipPoints) "VIP rewards".
    /// </summary>
    public static class VipWelfareUi
    {
        const ushort WelfareDialog = 646, PrivilegeDialog = 4013, RefreshCmd = 0xcf0;
        const uint Begin = 2001010000, End = 4212312359;// YYMMDDHHMM: always running

        sealed class State { public bool Defined; }
        static readonly ConditionalWeakTable<Client.GameClient, State> states = new ConditionalWeakTable<Client.GameClient, State>();

        public static uint ActId { get { return VipPoints.WindowActivity; } }

        /// <summary>Login (mobile): defines the Lua, shows the tab and sends the activity + the player's VIP points.</summary>
        public static void OnLogin(Client.GameClient c)
        {
            if (!MobileClient.Is(c) || c.Player == null || ActId == 0) return;
            if (!Define(c)) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                c.Send(ActivityList(stream));
                c.Send(UserInfo(c, stream));
            }
        }

        /// <summary>Opens Welfare on the VIP tab.</summary>
        public static bool Open(Client.GameClient c)
        {
            if (!MobileClient.Is(c) || c.Player == null || ActId == 0 || !Define(c)) return false;
            LuaUi.LuaPush.Send(c, "QahrVipW_Open|" + WelfareDialog + "," + (PrivilegeTab * 100));
            Refresh(c);
            return true;
        }

        const int PrivilegeTab = 10;// CDlgPerfectActivity::SetCategory(10) = Credit Rebate; OpenWindow(646, X) uses X / 100

        /// <summary>New points or a claim: send the state and redraw the tab (harmless when it is closed).</summary>
        public static void Refresh(Client.GameClient c)
        {
            State st;
            if (c == null || c.Player == null || ActId == 0 || !states.TryGetValue(c, out st) || !st.Defined) return;
            using (var rec = new ServerSockets.RecycledPacket())
                c.Send(UserInfo(c, rec.GetStream()));
            LuaUi.LuaPush.Send(c, "QahrVipW_Redraw|" + PrivilegeDialog + "," + RefreshCmd);
        }

        /// <summary>B6OperatingAct: the client's query (type 0) lists our activity.</summary>
        public static void OnQuery(Client.GameClient c)
        {
            Refresh(c);
        }

        /// <summary>B6OperatingAct: Claim (type 2) on our activity = the next level in order.</summary>
        public static void OnClaim(Client.GameClient c)
        {
            var levels = VipPoints.Levels;
            int claimed = ClaimedInOrder(c);
            if (claimed < levels.Count)
                VipPoints.Claim(c, levels[claimed]);
            Refresh(c);
        }

        /// <summary>The client claims in order: the number of levels claimed from the first one without a gap.</summary>
        static int ClaimedInOrder(Client.GameClient c)
        {
            int n = 0;
            foreach (var lv in VipPoints.Levels)
            {
                if (!VipPoints.IsClaimed(c.Player.UID, lv.Vip)) break;
                n++;
            }
            return n;
        }

        // ------------------------------------------------------------------ packets (MsgOperatingActInfo 2004)
        static ServerSockets.Packet ActivityList(ServerSockets.Packet stream)
        {
            var quiet = VipPoints.QuietActivities;
            stream.InitWriter();
            stream.Write((ushort)0);// type 0 = activity list
            stream.Write((ushort)(1 + quiet.Count));
            WriteActivity(stream, ActId, 1);
            foreach (var id in quiet)
                WriteActivity(stream, id, 2);// flag 2 = left out of the client's query
            stream.Finalize(Game.GamePackets.MsgOperatingActInfo);
            return stream;
        }

        static void WriteActivity(ServerSockets.Packet stream, uint id, ushort flag)
        {
            stream.Write(id);
            stream.Write(flag);
            stream.Write(Begin);
            stream.Write(End);
        }

        static ServerSockets.Packet UserInfo(Client.GameClient c, ServerSockets.Packet stream)
        {
            long points = VipPoints.Points(c.Player.UID);
            stream.InitWriter();
            stream.Write((ushort)1);// type 1 = the player's progress
            stream.Write((ushort)1);
            stream.Write(ActId);
            stream.Write((ushort)ClaimedInOrder(c));
            stream.Write((ushort)0);
            stream.Write((uint)Math.Min(points, uint.MaxValue));
            stream.Finalize(Game.GamePackets.MsgOperatingActInfo);
            return stream;
        }

        // ------------------------------------------------------------------ Lua
        static bool Define(Client.GameClient c)
        {
            var st = states.GetValue(c, _ => new State());
            if (!st.Defined)
            {
                if (!LuaUi.LuaPush.SendScript(c, "QahrVipW", LuaSource)) return false;
                st.Defined = true;
            }
            return LuaUi.LuaPush.Send(c, "QahrVipW_Set|" + LuaUi.LuaPush.SafeArg(Rows()));
        }

        /// <summary>{activity, {{vip, {{id, count, bound}...}}...}} in the order of the window rows.</summary>
        static string Rows()
        {
            var sb = new StringBuilder("{").Append(ActId).Append(",{");
            var levels = VipPoints.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('{').Append(levels[i].Vip).Append(",{");
                var items = levels[i].Items.Take(5).ToList();// the row has 5 item slots
                for (int k = 0; k < items.Count; k++)
                {
                    if (k > 0) sb.Append(',');
                    sb.Append('{').Append(items[k].Id).Append(',').Append(items[k].Count).Append(',').Append(items[k].Bound ? 1 : 0).Append('}');
                }
                sb.Append("}}");
            }
            return sb.Append("}}").ToString();
        }

        // No "local" and no '|' (LuaPush.SendScript). Every hook only acts while QahrVipW_Act is set.
        const string LuaSource = @"QahrVipW_Act = 0
QahrVipW_Rows = {}
QahrVipW_Mine = QahrVipW_Mine or {}
function QahrVipW_Hook(name, make)
	if _G[name] ~= nil and _G[name] ~= QahrVipW_Mine[name] then
		QahrVipW_Mine[name] = make(_G[name])
		_G[name] = QahrVipW_Mine[name]
	end
end
function QahrVipW_Item(lv, i)
	QahrVipW_R = QahrVipW_Rows[lv]
	return QahrVipW_R and QahrVipW_R[2][i]
end
function QahrVipW_Apply()
	QahrVipW_Hook('Fund_GetActClose', function(o) return function(a, ...) if a == 3 and QahrVipW_Act > 0 then return false end return o(a, ...) end end)
	QahrVipW_Hook('TopupPrivilege_GetItemPosition', function(o) return function(k, k2) if k == 'id' and QahrVipW_Act > 0 then return QahrVipW_Act end return o(k, k2) end end)
	QahrVipW_Hook('TopupPrivilege_GetMaxTopNum', function(o) return function() if QahrVipW_Act > 0 then return #QahrVipW_Rows end return o() end end)
	QahrVipW_Hook('TopupPrivilege_GetSketchInfo', function(o) return function(lv, k)
		if QahrVipW_Act > 0 and QahrVipW_Rows[lv] then
			if k == 'image' then return 'fuli_tqdj' .. QahrVipW_Rows[lv][1] .. 'pic' end
			return 'VIP ' .. QahrVipW_Rows[lv][1]
		end
		return o(lv, k) end end)
	QahrVipW_Hook('TopupPrivilege_GetPrivilegeRewardCount', function(o) return function(lv)
		if QahrVipW_Act > 0 then return QahrVipW_Rows[lv] and #QahrVipW_Rows[lv][2] or 0 end
		return o(lv) end end)
	QahrVipW_Hook('TopupPrivilege_GetPrivilegeReward', function(o) return function(lv, i, k)
		if QahrVipW_Act > 0 then
			QahrVipW_I = QahrVipW_Item(lv, i)
			if QahrVipW_I == nil then return 0 end
			if k == 'id' then return QahrVipW_I[1] end
			if k == 'num' then return QahrVipW_I[2] end
			if k == 'monoly' then return QahrVipW_I[3] end
			return 0
		end
		return o(lv, i, k) end end)
end
function QahrVipW_Set(s)
	QahrVipW_D = StrToTable(s)
	if QahrVipW_D == nil then return end
	QahrVipW_Act = QahrVipW_D[1] or 0
	QahrVipW_Rows = QahrVipW_D[2] or {}
	QahrVipW_Apply()
end
function QahrVipW_Open(s)
	QahrVipW_Apply()
	LuaUIMgr_OpenWindow(s)
	QahrVipW_Apply()
end
function QahrVipW_Redraw(s)
	QahrVipW_Apply()
	QahrVipW_A = StrToTable('{' .. s .. '}')
	pcall(function() g_objLuaUIMgrCop:PostCmd(QahrVipW_A[2], QahrVipW_A[1]) end)
end
";
    }
}
