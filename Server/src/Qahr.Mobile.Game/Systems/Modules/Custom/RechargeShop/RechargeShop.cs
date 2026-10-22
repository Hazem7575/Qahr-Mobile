using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Database
{
    public static class RechargeShop
    {


        public static ConcurrentDictionary<uint, Role.Player.RechargeType> RechargeAccounts = new ConcurrentDictionary<uint, Role.Player.RechargeType>();


        public static void UpdateRecharge(Client.GameClient user)
        {
            if (RechargeAccounts.ContainsKey(user.Player.UID))
            {
                RechargeAccounts[user.Player.UID] = user.Player.RechargeProgress;
            }
            else
                RechargeAccounts.TryAdd(user.Player.UID, user.Player.RechargeProgress);
        }

        // يرجّع تقدّم الشحن المحفوظ للاعب عند الدخول (من غيره كان بيرجع None وياخد المكافأة تاني)
        public static void RestoreRecharge(Client.GameClient user)
        {
            if (user == null || user.Player == null)
                return;
            Role.Player.RechargeType val;
            if (RechargeAccounts.TryGetValue(user.Player.UID, out val))
                user.Player.RechargeProgress |= val;
        }

        public static void Save()
        {
            try
            {
                using (Database.DBActions.Write _wr = new Database.DBActions.Write("RechargeShop.txt"))
                {

                    var dictionary = RechargeAccounts.ToArray();
                    foreach (var item in dictionary)
                        _wr.Add(item.Key + "/" + (ulong)item.Value);// بنكتب الرقم مش اسم الـ enum
                    _wr.Execute(DBActions.Mode.Open);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[RechargeShop.Save ERROR] " + e);
            }

        }

        public static void Load()
        {
            try
            {
                using (Database.DBActions.Read r = new Database.DBActions.Read("RechargeShop.txt"))
                {
                    if (r.Reader())
                    {
                        int count = r.Count;
                        for (uint x = 0; x < count; x++)
                        {
                            Database.DBActions.ReadLine readerline = new DBActions.ReadLine(r.ReadString(""), '/');
                            uint key = readerline.Read((uint)0);
                            // يقبل الرقم (الجديد) أو اسم الـ enum (الملفات القديمة)
                            string raw = readerline.Read("");
                            ulong num;
                            Role.Player.RechargeType val;
                            if (ulong.TryParse(raw, out num))
                                val = (Role.Player.RechargeType)num;
                            else if (!Enum.TryParse(raw, true, out val))
                                val = Role.Player.RechargeType.None;
                            if (!RechargeAccounts.ContainsKey(key))
                                RechargeAccounts.TryAdd(key, val);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[RechargeShop.Load ERROR] " + e);
            }
        }

    }
}