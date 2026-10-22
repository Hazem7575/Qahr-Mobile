using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: the activity-button bar of the mobile client (luaui\dialog\dlgactreceive, Dialog 1258).
    /// The PC Lua calls of LuaPush (FguiActivityMenu_OpenData / _UpdateList / _UpdateSigle,
    /// DlgActivityMenuMainBtn_OpenBtn) do not exist in the mobile 1.1.2.3 scripts, so the mobile showed no event bar.
    /// The mobile flow (luaui\function\g_function.lua, publicfiles\otherconf\actconfig / actmenuconfig / acttimemenuconfig):
    ///   login:   DlgActreceiveUI_Open|{['Code']=200,['Data']={['CurTime']=unix}}      -> the bar opens and asks
    ///   10107    ButtonStorage_Refresh  -> DlgActreceiveUI_OpenUIBindData|{['Code']=200,['Data']={{['Id']='1362',['Red']=0},..}}
    ///            (ActConfig ids: 1362 = "Daily" group, 1525 = "Limited" group, 1168 welfare, ...)
    ///   10107    ButtonStorage_DailyButton   -> DlgActbtnMenuUI_OpenData|{... ActMenuConfig ids ...}   (group 1362)
    ///   10107    ButtonStorage_LimitedButton -> DlgTimebtnMenuUI_OpenData|{... ActTimeMenuConfig ids ...} (group 1525)
    /// Limited group = the running festivals whose PC top-bar Fgui id is also a mobile ActTimeMenuConfig button
    /// (10459 Lunar Voyage, 150124 Sports Meet 2024, 150361 Top Master Campaign) + Data\mobile_events.txt
    /// limited_buttons; Daily group = daily_buttons (empty: none of the mobile daily windows has a server side yet).
    /// A group without buttons is not shown. PC clients keep LuaPush unchanged.
    /// </summary>
    public static class MobileEventsMenu
    {
        public const uint MsgActreceive = 10107;// tMsg_Info["Actreceive"]
        public const string DailyGroup = "1362", LimitedGroup = "1525";

        /// <summary>BtnIds of the mobile acttimemenuconfig.lua (1.1.2.3).</summary>
        static readonly HashSet<string> MobileTimeMenuIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "1479","1345","1507","1284","1200","1205","1218","1237","1307","1317","1256","1197","1349","1367","1159","1370",
            "1356","1255","1266","1343","1449","1474","1350","1378","1252","1499","150124","1514","10459","10503","10552",
            "150147","150294","150293","150299","10726","150283","1463","150361","10855","120010","10707","10891","10900",
        };

        static List<string> ConfigList(string key)
        {
            var l = new List<string>();
            try
            {
                if (!File.Exists(MobileEvents.Path))
                    return l;
                foreach (var raw in File.ReadAllLines(MobileEvents.Path))
                {
                    string line = raw;
                    int c = line.IndexOf('#');
                    if (c >= 0) line = line.Substring(0, c);
                    int eq = line.IndexOf('=');
                    if (eq <= 0 || !string.Equals(line.Substring(0, eq).Trim(), key, StringComparison.OrdinalIgnoreCase))
                        continue;
                    foreach (var p in line.Substring(eq + 1).Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        uint id;
                        if (uint.TryParse(p, out id) && !l.Contains(p)) l.Add(p);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] " + MobileEvents.Path + ": " + e.Message, ConsoleColor.Red);
            }
            return l;
        }

        public static List<string> LimitedButtons()
        {
            var l = ConfigList("limited_buttons");
            try
            {
                foreach (var d in Festivals.FestivalManager.Defs.ToArray())
                {
                    if (d == null || !Festivals.FestivalManager.IsRunning(d.Key))
                        continue;
                    string k = Festivals.FestivalIcons.Key(d.IconShow);
                    if (k != null && k.StartsWith("fgui:", StringComparison.Ordinal))
                    {
                        string id = k.Substring(5);
                        if (MobileTimeMenuIds.Contains(id) && !l.Contains(id)) l.Add(id);
                    }
                }
                foreach (var id in MidAutumn2024.LunarVoyage.TopBarIds())
                    if (MobileTimeMenuIds.Contains(id) && !l.Contains(id)) l.Add(id);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return l;
        }

        public static List<string> DailyButtons()
        {
            return ConfigList("daily_buttons");
        }

        static string Data(IEnumerable<string> ids, bool effect)
        {
            var sb = new StringBuilder("{['Code']=200,['Data']={");
            bool first = true;
            foreach (var id in ids)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append("{['Id']='").Append(id).Append("',['Red']=0").Append(effect ? ",['Effect']=0" : "").Append('}');
            }
            return sb.Append("}}").ToString();
        }

        /// <summary>LuaPush.OnLogin: true = mobile, the bar was opened (the PC top-bar calls are skipped).</summary>
        public static bool OnLogin(Client.GameClient client)
        {
            if (!MobileEvents.IsMobile(client))
                return false;
            LuaUi.LuaPush.Send(client, "DlgActreceiveUI_Open|{['Code']=200,['Data']={['CurTime']=" + DateTimeOffset.Now.ToUnixTimeSeconds() + "}}");
            return true;
        }

        /// <summary>10107 ButtonStorage_Refresh of a mobile client: the groups that have buttons. True = answered.</summary>
        public static bool Refresh(Client.GameClient client)
        {
            if (!MobileEvents.IsMobile(client))
                return false;
            var groups = new List<string>();
            if (DailyButtons().Count > 0) groups.Add(DailyGroup);
            if (LimitedButtons().Count > 0) groups.Add(LimitedGroup);
            LuaUi.LuaPush.Send(client, "DlgActreceiveUI_OpenUIBindData|" + Data(groups, false), MsgActreceive);
            return true;
        }

        /// <summary>LuaPush.HandleCallback, msg 10107 of a mobile client. True = answered.</summary>
        public static bool OnCallback(Client.GameClient client, string func)
        {
            if (!MobileEvents.IsMobile(client))
                return false;
            switch (func)
            {
                case "ButtonStorage_Refresh":
                    return Refresh(client);
                case "ButtonStorage_DailyButton":
                    LuaUi.LuaPush.Send(client, "DlgActbtnMenuUI_OpenData|" + Data(DailyButtons(), true), MsgActreceive);
                    return true;
                case "ButtonStorage_LimitedButton":
                    LuaUi.LuaPush.Send(client, "DlgTimebtnMenuUI_OpenData|" + Data(LimitedButtons(), true), MsgActreceive);
                    return true;
            }
            return false;
        }
    }
}
