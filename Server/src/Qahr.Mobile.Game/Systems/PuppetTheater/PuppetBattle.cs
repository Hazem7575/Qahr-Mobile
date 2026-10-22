// ============================================================================
//  [PuppetTheater 2026-10-15] Boss battles + Puppet Finale = the official in-window puppet battle (video 01:40, 07:01):
//  client window FuiPetDaily (fairygui/window/fuipetdaily), fed segment by segment with
//  DlgPetDailyBattle_BindData|{Code=200,Data={Index,LoopEnd,Round,Btn,ServerParam,Info={{Name={player,666 Papet},PetAttr,Bar1},
//  {Name={enemy}...}},Battle={Action 1|2,SkillIndex,SkillLev,Damage={n}}}} (proxy InsertServerData opens it on LoopEnd=1).
//  Skip / end -> msg 10482 SkillCalculate_FightEnd</S><ServerParam> (answered here when the param is ours).
//  The fight is resolved on the server BEFORE it is shown (result, rating and rewards are final; the client only plays it).
//  Enemy names STR_FUI_PETDAILY_BATTLE_ENEMYNAME_35-44 (area bosses, Spider Queen first) and 52 (Evil Marionette): official.
//  Stats / damage formula / looks marked DERIVED in config.json (_battle).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.FoolsKit;
using FL = Qahr.Mobile.Festivals.FestivalLoader;

namespace Qahr.Mobile.PuppetTheater
{
    public static class PuppetBattle
    {
        public const uint EndMsg = 10482;   // tMsg_Info["Fui24Halloween"] (the window's FightEnd callback)
        public sealed class Result { public bool Won; public int Rounds; public long Dealt; public long EnemyLeft; }

        static int I(Dictionary<string, object> d, string k, int def) { var v = FL.Val(d, k); return v == null ? def : (int)Math.Min(int.MaxValue, FL.U(v)); }
        static Dictionary<string, object> Pb { get { return FL.Obj(FL.Val(PT.Cfg.Raw, "puppetBattle")) ?? new Dictionary<string, object>(); } }

        /// <summary>Resolves the fight and sends it to the client's battle window. enemyHp = HP shown (and fought).</summary>
        public static Result Fight(Client.GameClient c, int enemyName, int enemyLook, long enemyHp, long enemyMax, int enemyAtk, string param)
        {
            var pb = Pb; uint uid = c.Player.UID;
            int rounds = Math.Max(1, I(pb, "rounds", 10));
            long hpMax = I(pb, "hp", 3000) + (long)PuppetHome.Level(uid, 1) * I(pb, "hpPerPapetLv", 200);
            long atk = I(pb, "atk", 220) + (long)(PuppetHome.Level(uid, 2) + PuppetHome.Level(uid, 3)) * I(pb, "atkPerSoulLv", 20);
            long def = (long)(PuppetHome.Level(uid, 4) + PuppetHome.Level(uid, 5)) * I(pb, "defPerSoulLv", 6);
            long hp = hpMax, ehp = Math.Max(1, enemyHp);
            var res = new Result();
            var packets = new List<string>();
            string pname = (c.Player.Name ?? "").Replace("\\", "").Replace("'", "");
            int look = I(pb, "look", 246);
            int[] mySkills = { 17, 1, 2, 4, 10, 12 };
            for (int r = 1; r <= rounds && hp > 0 && ehp > 0; r++)
            {
                res.Rounds = r;
                long d1 = Math.Max(1, (long)(atk * (Kit.Next(90, 110) / 100.0)));
                d1 = Math.Min(d1, ehp); ehp -= d1; res.Dealt += d1;
                packets.Add(Seg(packets.Count + 1, r, pname, look, hp, hpMax, enemyName, enemyLook, ehp, enemyMax, 1, mySkills[Kit.Next(0, mySkills.Length - 1)], d1, param));
                if (ehp <= 0) break;
                long d2 = Math.Max(1, (long)(enemyAtk * (Kit.Next(90, 110) / 100.0)) - def);
                d2 = Math.Min(d2, hp); hp -= d2;
                packets.Add(Seg(packets.Count + 1, r, pname, look, hp, hpMax, enemyName, enemyLook, ehp, enemyMax, 2, 16, d2, param));
            }
            res.Won = ehp <= 0; res.EnemyLeft = ehp;
            Last[uid] = DateTime.Now;
            var send = packets.Select((x, i) => x.Replace("['LoopEnd']=X", "['LoopEnd']=" + (i == packets.Count - 1 ? 1 : 0))).ToList();
            InBattle[uid] = DateTime.Now;
            // Client race (fuipetdailymediator onExit -> Cmd_FuiPetDaily_ClearData): the window we closed after the previous fight
            // clears the proxy data when its exit finishes. A new batch inserted before that is wiped after segment 1 and the
            // playback freezes. So a batch never goes out sooner than CloseGapMs after our last close.
            DateTime closed; int wait = 0;
            if (LastClose.TryGetValue(uid, out closed)) wait = (int)Math.Max(0, CloseGapMs - (DateTime.Now - closed).TotalMilliseconds);
            if (wait <= 0 || SendOverride != null) { foreach (var x in send) Kit.Lua(c, x, PT.LuaMsg); }
            else System.Threading.Tasks.Task.Delay(wait).ContinueWith(_ => { try { if (c.Player != null) foreach (var x in send) Kit.Lua(c, x, PT.LuaMsg); } catch { } });
            return res;
        }
        // DamageType2 (1 attack / 2 crit / 3 heal / 4 damage) picks the float animation; without it the client's
        // UpdateSkillFloat finds no transition, never calls Cmd_FuiPetDaily_Bind(next) and the playback freezes on segment 1.
        // DamageType1 (1 physical / 2 magic) picks the hit effect.
        // PetAttr = "look,2(look)/1(mount),size" — fuipetdailymediator: com_model scale = size / 150. Official (owner screenshot,
        // stage 1 vs Spider Queen): Papet fills the left half = 300 (2x). enemyScale 300 DERIVED (the client scales both sides the same way).
        static int Scale(string k) { int v = I(Pb, k, 300); return v < 30 || v > 900 ? 300 : v; }
        static string Seg(int index, int round, string pname, int look, long hp, long hpMax, int enemy, int elook, long ehp, long emax, int action, int skill, long dmg, string param)
        {
            return new StringBuilder("DlgPetDailyBattle_BindData|{['Code']=200,['Data']={['Index']=").Append(index).Append(",['LoopEnd']=X,['Round']=").Append(round)
                .Append(",['Btn']=1,['ServerParam']='").Append(param).Append("',['Info']={{['Name']={'").Append(pname).Append("',666},['PetAttr']='").Append(look).Append(",2,").Append(Scale("scale")).Append("',['Bar1']={")
                .Append(Math.Max(0, hp)).Append(',').Append(Math.Max(1, hpMax)).Append("}},{['Name']={").Append(enemy).Append("},['PetAttr']='").Append(elook).Append(",2,").Append(Scale("enemyScale")).Append("',['Bar1']={")
                .Append(Math.Max(0, ehp)).Append(',').Append(Math.Max(1, emax)).Append("}}},['Battle']={['Action']=").Append(action).Append(",['SkillIndex']=").Append(skill)
                .Append(",['SkillLev']=1,['DamageType1']=1,['DamageType2']=1,['Damage']={").Append(dmg).Append("}}}}").ToString();
        }

        /// <summary>LuaPush msg 10482: the battle window's end / skip / leave callbacks for OUR battles (param PT...). True = ours.</summary>
        /// <summary>Official loss tip (Server_Key tSkillCalculate[NewTips][5]): "Sorry, you failed the challenge. Go to enhance your puppet?"</summary>
        public const string LossBox = "Fui_Common_MsgBox_Open|{['Code']=200,['Data']={10768,{'',{}},{'Sorry, you failed the challenge. Go to enhance your puppet?',{}},{'',{}},{'',{}},{'',{}},{'',{}},{'',{}},-1,{'',{}},{'Halloween2025Npc_GoPuppetHome',{}},{'Halloween2025Kua_OpenMain</N>0',{}},{'',{}}}}";
        /// <summary>Loss window: the client`s common message box (fairygui/window/fuimsgbox, Fui_Common_MsgBox_Open) with the official text of
        /// Server_Key text tSkillCalculate[NewTips][5]; Confirm -> Puppet Home (Halloween2025Npc_GoPuppetHome), Cancel -> main window.</summary>
        public static void ShowLoss(Client.GameClient c) { Kit.Lua(c, LossBox, PT.LuaMsg); }
        /// <summary>uid -> the win to show after the battle window closes: rating + the item attrs granted.</summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Tuple<string, List<string>>> Won = new System.Collections.Concurrent.ConcurrentDictionary<uint, Tuple<string, List<string>>>();
        /// <summary>Official "You received" popup (luaui dialog DlgPublicReward, Dialog_Info 1348 "Click to go on."):
        /// DlgPublicReward_OpenView {ShowTip = Server_Key tSkillCalculate[NewTips][10] "Congratulations on your victory! Boss Rating: %s",
        /// tCloseFunc = back to the Strings of Terror window} + DlgPublicReward_UpdateView {RewardList, IsFinish}.</summary>
        public static void ShowWin(Client.GameClient c, string rating, List<string> items)
        {
            string tip = rating != null && rating.StartsWith("STR_ID_", StringComparison.Ordinal) ? rating : "STR_ID_tSkillCalculate[NewTips][10]@@" + rating + "@@";
            Kit.Lua(c, "DlgPublicReward_OpenView|{['Code']=200,['Data']={['ShowTip']='" + tip + "',['tCloseFunc']={['Id']=" + PT.LuaMsg + ",['Func']='Halloween2025Kua_OpenMain',['Param']='</N>0'}}}", PT.LuaMsg);
            Kit.Lua(c, "DlgPublicReward_UpdateView|{['Code']=200,['Data']={['RewardList']={" + string.Join(",", (items ?? new List<string>()).Select(x => "'" + x + "'")) + "},['IsFinish']=1}}", PT.LuaMsg);
        }
        public const int CloseGapMs = 3500;
        /// <summary>Test seam: send at once (no delayed batch).</summary>
        public static Action<string> SendOverride;
        /// <summary>uid -> when we last closed the battle window / when a battle was sent and is still playing (until its FightEnd).</summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime> LastClose = new System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime>();
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime> InBattle = new System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime>();
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, bool> Lost = new System.Collections.Concurrent.ConcurrentDictionary<uint, bool>();
        /// <summary>A battle is still playing in the client (no FightEnd yet, 90 s max): a second challenge is ignored.</summary>
        public static bool Busy(uint uid) { DateTime d; return InBattle.TryGetValue(uid, out d) && (DateTime.Now - d).TotalSeconds < 90; }
        /// <summary>uid -> when the player`s last puppet battle (ours) was sent.</summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime> Last = new System.Collections.Concurrent.ConcurrentDictionary<uint, DateTime>();
        /// <summary>Client flow (fuipetdailyproxy FightEnd): after the last segment / Skip it sends SkillCalculate_FightEnd</S>param,
        /// switches c_main to 0, RESETS its data and waits for the server; further Skip clicks then come with an EMPTY param.
        /// The server must close the window (DlgPetDailyBattle_Close = Cmd_FuiPetDaily_Close). We close it and go back to the
        /// Strings of Terror window. Empty params count as ours while our battle is the player`s last one (10 minutes).</summary>
        public static bool OnEnd(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || func == null || !func.StartsWith("SkillCalculate_", StringComparison.Ordinal)) return false;
            // the client appends its param as "</S>param" (Public_CallServerFunc(msg, func, "</S>" .. sServerParam)), and LuaPush splits
            // only on </N>: so func arrives as "SkillCalculate_FightEnd</S>PTB2" (live PacketLog 14:53:19)
            string extra = "";
            int cut = func.IndexOf("</S>", StringComparison.Ordinal);
            if (cut >= 0) { extra = func.Substring(cut + 4); func = func.Substring(0, cut); }
            if (func != "SkillCalculate_FightEnd" && func != "SkillCalculate_FightLeave" && func != "SkillCalculate_FightLeave105") return false;
            string all = (extra + string.Join("|", args ?? new string[0])).Trim();
            DateTime when;
            bool recent = Last.TryGetValue(c.Player.UID, out when) && (DateTime.Now - when).TotalMinutes < 10;
            if (all.Length > 0 && !all.Contains("PTB")) return false;          // another system`s battle (Halloween2024)
            if (all.Length == 0 && !recent) return false;
            if (Kit.Throttled(c.Player.UID, "ptfightend", 500)) return true;
            Kit.Lua(c, "DlgPetDailyBattle_Close|", PT.LuaMsg);                // the result was applied when the fight was resolved
            LastClose[c.Player.UID] = DateTime.Now;
            DateTime ib; InBattle.TryRemove(c.Player.UID, out ib);
            bool lostFlag;
            if (all.Length > 0 && Lost.TryRemove(c.Player.UID, out lostFlag)) { ShowLoss(c); return true; }
            Tuple<string, List<string>> win;
            if (all.Length > 0 && Won.TryRemove(c.Player.UID, out win)) { ShowWin(c, win.Item1, win.Item2); return true; }
            if (all.Length > 0 && PT.Running) PuppetTheaterUi.SendMain(c);
            return true;
        }

        // ------------------------------------------------------------------ area boss (Challenge boss)
        public static string Boss(Client.GameClient c, int L)
        {
            var l = PuppetMaze.Lvl(L); uint uid = c.Player.UID;
            if (l == null || !PuppetMaze.Cleared(uid, L)) return "locked";
            if (Busy(uid) || Kit.Throttled(uid, "ptfight", 3000)) return "busy";
            var lv = FL.Obj(FL.Val(LevelRaw(L), "battle"));
            if (lv == null) return "noboss";
            int hp = I(lv, "hp", 2000);
            var r = Fight(c, I(lv, "enemy", 35), I(lv, "look", 951), hp, hp, I(lv, "atk", 250), "PTB" + L);
            PT.Log(c, "BOSS", "L" + L + (r.Won ? " won in " + r.Rounds + " rounds" : " lost"));
            if (!r.Won) { Lost[uid] = true; return "lost"; }   // official tip shown after the battle window closes (ShowLoss)
            int rate = r.Rounds <= 3 ? 3 : 1;   // official: S within 3 rounds (client text), video 4 rounds = B; A unused (DERIVED)
            if (rate > PT.Get(uid, "r" + L)) PT.St.Set(uid, "r" + L, rate);
            string extra = "";
            var granted = new List<string>();
            var cfg = PT.Cfg;
            if (PT.Get(uid, "fb" + L) == 0 && cfg.BossFirst.Count > 0)   // [2026-10-15] key "fb": the in-window battle pays the official first-clear Octave Bell once (old "bf" was the retired boss-copy Soulthread reward)
            {
                var rw = cfg.BossFirst.Select(x => new Reward(x[0], x[1], true)).ToArray();
                using (var rec = new ServerSockets.RecycledPacket())
                    if (PT.Space(c, rw)) { PT.St.Set(uid, "fb" + L, 1); extra = " You obtained " + PT.Give(c, rec.GetStream(), "boss first L" + L, rw) + "."; granted.AddRange(rw.Select(x => x.Item + " 0 " + x.Count)); }
                    else extra = " Make room in your inventory and win again for the first-clear reward.";
            }
            string letter = rate == 3 ? "S" : rate == 2 ? "A" : "B";
            PT.Msg(c, "Congratulations on your victory! Boss Rating: " + letter + extra);
            Won[uid] = Tuple.Create(letter, granted);
            return "won " + r.Rounds;
        }
        static Dictionary<string, object> LevelRaw(int L)
        {
            foreach (var o in FL.Arr(FL.Val(PT.Cfg.Raw, "levels")) ?? new List<object>()) { var e = FL.Obj(o); if (e != null && FL.U(FL.Val(e, "level")) == L) return e; }
            return null;
        }

        // ------------------------------------------------------------------ Puppet Finale (Evil Marionette, server pool 250000)
        public static string Finale(Client.GameClient c)
        {
            var cfg = PT.Cfg; uint uid = c.Player.UID;
            PuppetFinale.DayPublic();
            if (PuppetHome.TotalLevel(uid) < cfg.FinaleMinPuppet) { PT.Msg(c, PT.T("Guai[MsgBox][3]")); return "puppet"; }
            if (PT.St.Get(uid, "finDay") == PT.Today) { PT.Msg(c, PT.T("Guai[MsgBox][2]")); return "done"; }
            if (Busy(uid) || Kit.Throttled(uid, "ptfight", 3000)) return "busy";
            var f = FL.Obj(FL.Val(cfg.Raw, "finale")) ?? new Dictionary<string, object>();
            long pool; bool cleanup;
            lock (PT.St)
            {
                pool = PT.St.GetLong(0, "fHp", cfg.FinalePool);
                if (pool > cfg.FinalePool) { pool = cfg.FinalePool; PT.St.Set(0, "fHp", pool); }   // pool size lowered in config (250000, video)
                cleanup = pool <= 0;
            }
            PT.St.Set(uid, "finDay", PT.Today);   // one challenge a day (official), used by the fight
            int atk = I(f, "atk", 900);
            Result r = cleanup ? Fight(c, 52, I(f, "look", 327), 20000, 20000, atk, "PTBF")
                               : Fight(c, 52, I(f, "look", 327), pool, cfg.FinalePool, atk, "PTBF");
            var rw = new List<Reward>();
            long dmg = 0;
            if (!cleanup)
            {
                lock (PT.St)
                {
                    long now = PT.St.GetLong(0, "fHp", cfg.FinalePool);
                    dmg = Math.Min(r.Dealt, Math.Max(0, now));
                    PT.St.Set(0, "fHp", Math.Max(0, now - dmg));
                    PT.St.Set(uid, "fDmg" + PT.Today, PT.St.GetLong(uid, "fDmg" + PT.Today, 0) + dmg);
                    if (dmg > PT.St.GetLong(0, "fTopDmg", 0))
                    {
                        string nm = new string((c.Player.Name ?? "").Where(ch => ch != ';' && ch != '=' && ch != '\t' && ch != ',').ToArray());
                        PT.St.Set(0, Store.KV("fTop", uid), Store.KV("fTopDmg", dmg), Store.KV("fTopName", nm));
                    }
                }
            }
            if (PT.Get(uid, "finFirst") == 0 && cfg.FinaleFirst > 0) rw.Add(new Reward(PT.Soulthread, (uint)cfg.FinaleFirst));
            else if (cleanup && r.Won && cfg.FinaleCleanup > 0) rw.Add(new Reward(PT.Soulthread, (uint)cfg.FinaleCleanup));
            string got = "";
            if (rw.Count > 0)
                using (var rec = new ServerSockets.RecycledPacket())
                    if (PT.Space(c, rw.ToArray())) { PT.St.Set(uid, "finFirst", 1); got = " You obtained " + PT.Give(c, rec.GetStream(), "finale", rw.ToArray()) + "."; }
                    else rw.Clear();
            // official (video 08:20): the result popup "You completed the expedition, dealt %s damage, and gained 2 Soulthreads."
            // nothing gained (a later day`s damage run): no popup, the window just returns (OnEnd -> main)
            if (rw.Count > 0 || (cleanup && r.Won))
            Won[uid] = Tuple.Create(cleanup ? (r.Won ? "STR_ID_tSkillCalculate[NewTips][12]" : "STR_ID_tSkillCalculate[NewTips][9]")
                                  : rw.Count > 0 ? "STR_ID_tSkillCalculate[NewTips][13]@@" + dmg + "@@" : "STR_ID_tSkillCalculate[NewTips][9]",
                                    rw.Select(x => x.Item + " 0 " + x.Count).ToList());
            PT.Log(c, "FINALE", (cleanup ? "cleanup " : "") + "dealt " + (cleanup ? r.Dealt : dmg) + (r.Won ? " won" : ""));
            PT.Msg(c, (cleanup ? "You fought the Nightmare Puppets." : PT.Fmt("You completed the expedition and dealt %s damage to the Evil Marionette.", dmg)) + got);
            return cleanup ? "cleanup" : "finale " + dmg;
        }
    }
}
