using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: the VIP window of the mobile client (docs\VIP_UI.md).
    /// The PC VIP windows of the 1.1.2.3 library cannot be used: CDlgVip / CDlgVipEx / CDlgVipRepairAll / CDlgVipDepot have
    /// no command, Lua binding or OpenDialog id that shows them, and the two that can be shown (CDlgVipTransport with
    /// PostCmd 0xc89/0xc8a, reachable from the server through MsgAction type 116 OpenCustom dwParam = cmd) draw nothing,
    /// because the data\interface\vip\* pictures are not in the mobile resources. So the window is a Lua dialog
    /// ("DlgQahrVip", the same engine widgets as the official luagui dialogs) whose code is sent by the server the first
    /// time it is opened in a session (StrToTable chunks, see LuaSource), and whose content is filled by the server:
    ///   QahrVip_Show|{title,lev,H={4 header lines},T={tab texts},ts=selected tab,P=page title,L={11 lines},O={8 options},B={4 buttons}}
    /// Clicks come back as "</F>ClientManager_Callback</N>19992</S>QahrVip_Cmd</N>T3|O1|B2" (hook: LuaPush.HandleCallback).
    /// Opened by the main screen icon (newmenu.dat QahrHud icon "vip", Lua: Public_CallServerFunc(19992,"QahrVip_Cmd","</N>open"))
    /// and by the chat command "@vip". Text: Data\mobile_vip.txt (re-read when saved). The services reuse MobileVipMenu
    /// (city teleport self/team, repair all, remote warehouse; VIP 2+ not expired).
    /// </summary>
    public static class MobileVipUi
    {
        public const uint LuaMsg = 19992;

        sealed class State { public bool Defined; public string Page = "main"; public int Tab; }
        static readonly ConditionalWeakTable<Client.GameClient, State> states = new ConditionalWeakTable<Client.GameClient, State>();

        // ------------------------------------------------------------------ Data\mobile_vip.txt
        static readonly object sync = new object();
        static DateTime stamp = DateTime.MinValue;
        static List<string> how = new List<string>();
        static SortedDictionary<int, List<string>> levels = new SortedDictionary<int, List<string>>();

        public static string FilePath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_vip.txt"); }
        }

        static void Refresh()
        {
            try
            {
                string p = FilePath;
                DateTime t = File.Exists(p) ? File.GetLastWriteTimeUtc(p) : DateTime.MinValue;
                lock (sync)
                {
                    if (t == stamp)
                        return;
                    stamp = t;
                    var h = new List<string>();
                    var lv = new SortedDictionary<int, List<string>>();
                    List<string> cur = null;
                    if (t != DateTime.MinValue)
                    {
                        foreach (var raw in File.ReadAllLines(p))
                        {
                            string line = raw.Trim();
                            if (line.Length == 0 || line[0] == '#')
                                continue;
                            if (line[0] == '[' && line.EndsWith("]"))
                            {
                                string sec = line.Substring(1, line.Length - 2).Trim().ToLowerInvariant();
                                int n;
                                if (sec == "how")
                                    cur = h;
                                else if (sec.StartsWith("level") && int.TryParse(sec.Substring(5).Trim(), out n) && n >= 1 && n <= 9)
                                    lv[n] = cur = new List<string>();
                                else
                                    cur = null;
                                continue;
                            }
                            if (cur != null)
                                cur.Add(line);
                        }
                    }
                    if (lv.Count == 0)
                        for (int i = 1; i <= 6; i++)
                            lv[i] = new List<string> { "City teleport, team teleport, repair all and the remote warehouse (VIP 2+)." };
                    how = h;
                    levels = lv;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] mobile_vip.txt: " + e.Message, ConsoleColor.Red);
            }
        }

        // ------------------------------------------------------------------ open / answer
        /// <summary>"@vip" and the main screen icon. resend = send the Lua code again (the icon says "open0" when the
        /// client has no QahrVip_Show yet, "open1" when it has; "@vip" always sends it).</summary>
        public static bool Open(Client.GameClient client, bool resend = true)
        {
            if (client == null || client.Player == null)
                return false;
            var st = states.GetValue(client, _ => new State());
            if (resend)
                st.Defined = false;
            st.Page = "main";
            st.Tab = Math.Max(1, Math.Min((int)client.Player.VipLevel, MaxLevel()));
            if (!MobileVipMenu.HasVip(client))
                st.Tab = 0;// the "Get VIP" tab
            return Show(client, st);
        }

        /// <summary>LuaPush.HandleCallback (msg 19992). True = answered.</summary>
        public static bool OnLua(Client.GameClient client, string func, string[] args)
        {
            if (client == null || client.Player == null || func != "QahrVip_Cmd")
                return false;
            string k = args != null && args.Length > 0 ? (args[0] ?? "").Trim() : "";
            try
            {
                if (k == "open" || k == "open0" || k == "open1")
                    return Open(client, k != "open1");
                State st;
                if (!states.TryGetValue(client, out st) || !st.Defined || k.Length < 2)
                    return Open(client);
                int n;
                if (!int.TryParse(k.Substring(1), out n))
                    return true;
                switch (k[0])
                {
                    case 'T': OnTab(client, st, n); break;
                    case 'B': OnBottom(client, st, n); break;
                    case 'O': OnOption(client, st, n); break;
                }
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
            }
            return true;
        }

        static int MaxLevel()
        {
            Refresh();
            lock (sync)
            {
                int m = 0;
                foreach (var k in levels.Keys)
                    m = Math.Max(m, k);
                return Math.Min(m, 6);// 7 tab slots: the levels + "Get VIP"
            }
        }

        static void OnTab(Client.GameClient client, State st, int n)
        {
            int max = MaxLevel();
            st.Page = "main";
            st.Tab = n >= 1 && n <= max ? n : 0;
            Show(client, st);
        }

        static void OnBottom(Client.GameClient client, State st, int n)
        {
            if (st.Page != "main")
            {
                st.Page = "main";// B1 = Back
                Show(client, st);
                return;
            }
            if (!MobileVipMenu.HasVip(client))
            {
                client.SendSysMesage("This service needs VIP " + MobileVipMenu.MinLevelValue + " or higher (not expired).",
                    Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                st.Tab = 0;
                Show(client, st);
                return;
            }
            switch (n)
            {
                case 1: st.Page = "tele"; Show(client, st); break;
                case 2: st.Page = "team"; Show(client, st); break;
                case 3: Run(client, 3); break;
                case 4: st.Page = "wh"; Show(client, st); break;
            }
        }

        static void OnOption(Client.GameClient client, State st, int n)
        {
            if (!MobileVipMenu.HasVip(client))
                return;
            int i = n - 1;
            if (st.Page == "tele" && i >= 0 && i < MobileVipMenu.CityNames.Length)
            {
                Hide(client);
                Run(client, (byte)(10 + i));
            }
            else if (st.Page == "team" && i >= 0 && i < MobileVipMenu.CityNames.Length)
            {
                if (client.Team == null)
                {
                    client.SendSysMesage("You are not in a team.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                    return;
                }
                Hide(client);
                Run(client, (byte)(20 + i));
            }
            else if (st.Page == "wh" && i >= 0 && i < MobileVipMenu.WarehouseNames.Length)
            {
                Hide(client);
                Run(client, (byte)(30 + i));
            }
        }

        static void Run(Client.GameClient client, byte option)
        {
            using (var rec = new ServerSockets.RecycledPacket())
                MobileVipMenu.Service(client, rec.GetStream(), option);
        }

        public static bool Hide(Client.GameClient client)
        {
            return LuaUi.LuaPush.Send(client, "QahrVip_Hide|0");
        }

        // ------------------------------------------------------------------ the window content
        static bool Show(Client.GameClient client, State st)
        {
            Refresh();
            if (!st.Defined)
            {
                foreach (var chunk in DefineCalls())
                    if (!LuaUi.LuaPush.Send(client, chunk))
                        return false;
                st.Defined = true;
            }
            var p = client.Player;
            bool vip = MobileVipMenu.HasVip(client);
            var H = new List<string>();
            if (vip)
            {
                H.Add("My VIP level: " + p.VipLevel);
                H.Add("Valid until: " + p.ExpireVip.ToString("yyyy-MM-dd HH:mm"));
                var left = p.ExpireVip - DateTime.Now;
                H.Add(left.TotalDays >= 1 ? "Time left: " + (int)left.TotalDays + " day(s) " + left.Hours + " h" : "Time left: " + left.Hours + " h " + left.Minutes + " min");
                H.Add("<color=0xff02f402>VIP services are active.</color>");
            }
            else
            {
                H.Add(p.VipLevel > 0 ? "My VIP level: " + p.VipLevel + " (free level)" : "You are not a VIP yet.");
                H.Add("VIP services need VIP " + MobileVipMenu.MinLevelValue + "+.");
                H.Add("See \"Get VIP\" for how to get it.");
            }
            int max = MaxLevel();
            var T = new List<string>();
            for (int i = 1; i <= max; i++)
                T.Add("VIP " + i);
            T.Add("Get VIP");
            int ts = st.Tab >= 1 && st.Tab <= max ? st.Tab : max + 1;

            string P;
            var L = new List<string>();
            var O = new List<string>();
            var B = new List<string>();
            switch (st.Page)
            {
                case "tele":
                case "team":
                    P = st.Page == "tele" ? "City Teleport: choose a city" : "Team Teleport: your teammates are asked to follow";
                    O.AddRange(MobileVipMenu.CityNames);
                    B.Add("Back");
                    break;
                case "wh":
                    P = "Remote Warehouse: choose a warehouse";
                    foreach (var w in MobileVipMenu.WarehouseNames)
                        O.Add(w + " WH");
                    B.Add("Back");
                    break;
                default:
                    lock (sync)
                    {
                        List<string> lines;
                        if (ts <= max && levels.TryGetValue(ts, out lines))
                        {
                            P = "VIP " + ts + " privileges" + (vip && p.VipLevel >= ts ? "  <color=0xff02f402>(yours)</color>" : "");
                            L.AddRange(lines);
                        }
                        else
                        {
                            P = "How to get VIP";
                            L.AddRange(how.Count > 0 ? how : new List<string> { "Ask a GM how to get VIP on this server." });
                        }
                    }
                    B.Add("City Teleport");
                    B.Add("Team Teleport");
                    B.Add("Repair All");
                    B.Add("Warehouse");
                    break;
            }
            var sb = new StringBuilder("{title=").Append(LuaUi.LuaPush.Q("VIP Privileges"))
                .Append(",lev=").Append(vip ? Math.Min((int)p.VipLevel, 9) : 0)
                .Append(",ts=").Append(st.Page == "main" ? ts : 0)
                .Append(",P=").Append(LuaUi.LuaPush.Q(P));
            Table(sb, "H", H, 4);
            Table(sb, "T", T, 7);
            Table(sb, "L", L, 11);
            Table(sb, "O", O, 8);
            Table(sb, "B", B, 4);
            sb.Append('}');
            return LuaUi.LuaPush.Send(client, "QahrVip_Show|" + Safe(sb.ToString()));
        }

        static void Table(StringBuilder sb, string name, List<string> items, int max)
        {
            sb.Append(',').Append(name).Append("={");
            for (int i = 0; i < items.Count && i < max; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(LuaUi.LuaPush.Q(items[i]));
            }
            sb.Append('}');
        }

        /// <summary>The client's StrToTable deletes every "local" in the text and the call is split at '|'.</summary>
        static string Safe(string s)
        {
            return s.Replace("local", "Local").Replace("|", "/");
        }

        // ------------------------------------------------------------------ the Lua dialog (sent once per session)
        const int ChunkSize = 600;// bigger Lua pushes (about 1400 bytes) never reached the client

        static List<string> DefineCalls()
        {
            var calls = new List<string>();
            string src = LuaSource.Replace("\r", "");
            for (int i = 0; i < src.Length; i += ChunkSize)
            {
                string part = src.Substring(i, Math.Min(ChunkSize, src.Length - i));
                calls.Add("StrToTable|(function(s) QahrVip_Src = " + (i == 0 ? "" : "(QahrVip_Src or '') .. ") + "s end)([==[" + part + "]==])");
            }
            calls.Add("StrToTable|(function() QahrVip_Ok = pcall(load(QahrVip_Src)) QahrVip_Src = nil end)()");
            return calls;
        }

        // No "local" (StrToTable removes it) and no '|' anywhere in this text.
        const string LuaSource = @"QahrVip_Msg = 19992
function QahrVip_Img(t, n, x, y, w, h, a)
	t[#t + 1] = { uitype = UI.MyImage, name = n, x = x, y = y, w = w, h = h, base = 5, anisection = a }
end
function QahrVip_Txt(t, n, x, y, w, h, fs, col, pt)
	t[#t + 1] = { uitype = UI.ColorStatic, name = n, x = x, y = y, w = w, h = h, base = 5, fontsize = fs, postype = pt or 0, fontcolor = col, text = """" }
end
function QahrVip_Btn(t, n, x, y, w, h, a, fs, col)
	t[#t + 1] = { uitype = UI.Button, name = n, x = x, y = y, w = w, h = h, base = 5, anisection = a, text = """", clickevent = """",
		fontsize = fs, textoffsety = 5, postype = 1, normalcolor = col, clickcolor = col, disablecolor = col }
end
function QahrVip_Layout()
	QahrVip_T = { x = 0, y = 0, w = 1334, h = 750, base = 5, bgshowtype = 0, anisection = """", maskcolor = ""0xb3000000"",
		dockpos = T_POS_TYPE.POSTYPE_NORAML }
	QahrVip_Img(QahrVip_T, ""bg"", 131, 40, 1072, 670, ""msqhj_wkpic"")
	QahrVip_Txt(QahrVip_T, ""title"", 196, 52, 500, 30, 22, ""0xfff7e6c2"", 0)
	QahrVip_Btn(QahrVip_T, ""close"", 1140, 36, 64, 64, ""tongyong_daguanbibtn"", 20, ""0xffffffff"")
	QahrVip_Img(QahrVip_T, ""vippic"", 180, 112, 72, 36, ""MainUI_VipPic"")
	QahrVip_Img(QahrVip_T, ""vipnum"", 254, 112, 28, 36, ""MainUI_Vip0Pic"")
	for i = 1, 4 do
		QahrVip_Txt(QahrVip_T, ""H"" .. i, 176, 152 + (i - 1) * 30, 300, 28, 18, ""0xfff2e6c4"", 0)
	end
	for i = 1, 7 do
		QahrVip_Btn(QahrVip_T, ""T"" .. i, 186, 280 + (i - 1) * 56, 156, 48, ""tongyong_tsybtn"", 19, ""0xff2c2826"")
	end
	QahrVip_Txt(QahrVip_T, ""ptitle"", 520, 108, 640, 30, 22, ""0xfffced9c"", 0)
	for i = 1, 11 do
		QahrVip_Txt(QahrVip_T, ""L"" .. i, 520, 146 + (i - 1) * 40, 640, 36, 18, ""0xfff2e6c4"", 0)
	end
	for i = 1, 8 do
		QahrVip_Btn(QahrVip_T, ""O"" .. i, 520 + ((i - 1) % 2) * 320, 150 + math.floor((i - 1) / 2) * 70, 300, 52, ""tongyong_tsybtn"", 20, ""0xff2c2826"")
	end
	for i = 1, 4 do
		QahrVip_Btn(QahrVip_T, ""B"" .. i, 506 + (i - 1) * 162, 628, 156, 50, ""tongyong_tyydbtn"", 19, ""0xff000000"")
	end
	return QahrVip_T
end
QahrVip_Names = { ""close"" }
for i = 1, 7 do QahrVip_Names[#QahrVip_Names + 1] = ""T"" .. i end
for i = 1, 8 do QahrVip_Names[#QahrVip_Names + 1] = ""O"" .. i end
for i = 1, 4 do QahrVip_Names[#QahrVip_Names + 1] = ""B"" .. i end
if not DlgQahrVip then
	T_ModuleName[""DlgQahrVip""] = true
	DlgQahrVip = DialogClass(""DlgQahrVip"")
	package.loaded[""DlgQahrVip""] = DlgQahrVip
end
function DlgQahrVip:LoadData()
	self.data = QahrVip_Layout()
end
function DlgQahrVip:Init()
	for _, n in ipairs(QahrVip_Names) do
		if self.widgets[n] then
			self.widgets[n]:RegisterEvent(CLICKTYPE.CLICKTYPE_NORMAL, ""QahrVip_Click"", n)
		end
	end
	QahrVip_Apply()
end
function QahrVip_Set(w, s)
	if w == nil then return end
	if s == nil or s == """" then
		w:ShowWindow(0)
	else
		w:ShowWindow(5)
		w:SetWindowText(s)
	end
end
function QahrVip_Apply()
	if DlgQahrVip == nil or DlgQahrVip.widgets == nil or QahrVip_D == nil then return end
	QahrVip_Wd = DlgQahrVip.widgets
	QahrVip_Set(QahrVip_Wd[""title""], QahrVip_D.title or ""VIP"")
	if QahrVip_Wd[""vipnum""] then
		QahrVip_Wd[""vipnum""]:SetAniSection(""MainUI_Vip"" .. (QahrVip_D.lev or 0) .. ""Pic"")
	end
	for i = 1, 4 do QahrVip_Set(QahrVip_Wd[""H"" .. i], (QahrVip_D.H or {})[i]) end
	for i = 1, 7 do
		QahrVip_Set(QahrVip_Wd[""T"" .. i], (QahrVip_D.T or {})[i])
		if QahrVip_Wd[""T"" .. i] then
			QahrVip_Wd[""T"" .. i]:SetAniSection(i == QahrVip_D.ts and ""tongyong_tyydbtn"" or ""tongyong_tsybtn"")
		end
	end
	QahrVip_Set(QahrVip_Wd[""ptitle""], QahrVip_D.P)
	for i = 1, 11 do QahrVip_Set(QahrVip_Wd[""L"" .. i], (QahrVip_D.L or {})[i]) end
	for i = 1, 8 do QahrVip_Set(QahrVip_Wd[""O"" .. i], (QahrVip_D.O or {})[i]) end
	for i = 1, 4 do QahrVip_Set(QahrVip_Wd[""B"" .. i], (QahrVip_D.B or {})[i]) end
end
function QahrVip_Show(s)
	QahrVip_D = StrToTable(s)
	if type(QahrVip_D) ~= ""table"" then return end
	if DlgQahrVip.shown and DlgQahrVip.widgets and DlgQahrVip.widgets[""bg""] then
		QahrVip_Apply()
	else
		LuaUIMgr_ShowDialog(""DlgQahrVip"")
	end
end
function QahrVip_Close()
	if DlgQahrVip and DlgQahrVip.shown then
		DlgQahrVip.shown = false
		LuaUIMgr_HideDialog(""DlgQahrVip"")
	end
end
function QahrVip_Hide(s)
	QahrVip_Close()
end
function QahrVip_Click(n)
	if n == ""close"" then
		QahrVip_Close()
		return
	end
	Public_CallServerFunc(QahrVip_Msg, ""QahrVip_Cmd"", ""</N>"" .. tostring(n))
end
";
    }
}
