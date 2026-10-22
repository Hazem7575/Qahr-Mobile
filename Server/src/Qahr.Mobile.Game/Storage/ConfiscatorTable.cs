using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;

namespace Qahr.Mobile.Database
{
    public class ConfiscatorTable
    {
        public ConcurrentDictionary<uint, Role.Instance.Confiscator> PollContainers = new ConcurrentDictionary<uint, Role.Instance.Confiscator>();


        public void QueueObj(uint UID, Role.Instance.Confiscator conainter)
        {
            if (!PollContainers.ContainsKey(UID))
                PollContainers.TryAdd(UID, conainter);
        }
        private int GetRedeemItemsCount()
        {
            return PollContainers.Values.Sum(p => p.RedeemContainer.Count);
        }
        private int GetClaimItemsCount()
        {
            return PollContainers.Values.Sum(p => p.ClaimContainer.Count);
        }
        internal unsafe void DeleteAllOf(uint UID) 
        {
            Role.Instance.Confiscator Owner;
            if (PollContainers.TryRemove(UID, out Owner))
            {
                foreach (var Container in PollContainers.Values)
                {
                    foreach (var item in Container.RedeemContainer.Values)
                    {
                        Game.MsgServer.MsgDetainedItem Item;
                        if (item.GainerUID == UID || item.OwnerUID == UID)
                            Container.RedeemContainer.TryRemove(item.UID, out Item);
                    }
                    foreach (var item in Container.ClaimContainer.Values)
                    {
                        Game.MsgServer.MsgDetainedItem Item;
                        if (item.GainerUID == UID || item.OwnerUID == UID)
                            Container.ClaimContainer.TryRemove(item.UID, out Item); // كان بيمسح من RedeemContainer بالغلط
                    }
                }
            }
        }
        // قفل عشان الحفظ الفوري (ثريد اللعب) والحفظ الدوري مايكتبوش في نفس الوقت
        private static readonly object SaveLock = new object();

        // حفظ فوري بعد أي استرداد/استلام/حجز (يقفل ثغرة التكرار لو السيرفر وقع قبل الحفظ الدوري)
        internal void SaveNow()
        {
            try { Save(); }
            catch (Exception e) { MyConsole.WriteLine("[Confiscator SaveNow ERROR] " + e.Message, ConsoleColor.Yellow); }
        }

        // كتابة ذرّية: ملف .tmp وبعدين Replace فوق الأصلي، والعدد من نفس الـ snapshot
        private unsafe void WriteContainerAtomic(string finalPath, Game.MsgServer.MsgDetainedItem[] items)
        {
            string tmp = finalPath + ".tmp";
            bool ok = false;
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(tmp, FileMode.Create))
                return;
            try
            {
                int ItemCount = items.Length;
                binary.Write(&ItemCount, sizeof(int));
                for (int i = 0; i < items.Length; i++)
                {
                    Game.MsgServer.MsgDetainedItem packet = items[i];
                    binary.Write(&packet, sizeof(Game.MsgServer.MsgDetainedItem));
                }
                ok = true;
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Confiscator Save ERROR] " + finalPath + " : " + e.Message, ConsoleColor.Yellow);
            }
            finally
            {
                binary.Close();
            }
            try
            {
                if (!ok)
                {
                    if (File.Exists(tmp)) File.Delete(tmp);
                    return;
                }
                if (File.Exists(finalPath))
                {
                    try { File.Replace(tmp, finalPath, null); }
                    catch
                    {
                        File.Delete(finalPath);
                        File.Move(tmp, finalPath);
                    }
                }
                else
                    File.Move(tmp, finalPath);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Confiscator Replace ERROR] " + finalPath + " : " + e.Message, ConsoleColor.Yellow);
            }
        }
        internal unsafe void Save()
        {
            lock (SaveLock)
            {
                // snapshot (ToArray) قبل الحساب والكتابة
                var containers = PollContainers.Values.ToArray();
                var redeem = new List<Game.MsgServer.MsgDetainedItem>();
                var claim = new List<Game.MsgServer.MsgDetainedItem>();
                foreach (var Container in containers)
                {
                    redeem.AddRange(Container.RedeemContainer.Values.ToArray());
                    claim.AddRange(Container.ClaimContainer.Values.ToArray());
                }
                WriteContainerAtomic(Program.ServerConfig.DbLocation + "\\RedeemContainer.bin", redeem.ToArray());
                WriteContainerAtomic(Program.ServerConfig.DbLocation + "\\ClaimContainer.bin", claim.ToArray());
            }
        }

        internal unsafe void Load()
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\RedeemContainer.bin", FileMode.Open))
            {
                Game.MsgServer.MsgDetainedItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    binary.Read(&Item, sizeof(Game.MsgServer.MsgDetainedItem));
                    if (Item.UID >= Role.Instance.Confiscator.CounterUID.Count)
                        Role.Instance.Confiscator.CounterUID.Set(Item.UID + 1);

                    if (!PollContainers.ContainsKey(Item.GainerUID))
                        PollContainers.TryAdd(Item.GainerUID, new Role.Instance.Confiscator());
                    if (!PollContainers.ContainsKey(Item.OwnerUID))
                        PollContainers.TryAdd(Item.OwnerUID, new Role.Instance.Confiscator());

                    Item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(Item.Date).Ticks).Days);
                    if (Item.Action == Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps)
                    {
                        //Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        continue;
                    }
                    if (Item.DaysLeft > 7)
                    {
                        // Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        continue;
                    }
                    else
                    {
                        PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                        PollContainers[Item.OwnerUID].RedeemContainer.TryAdd(Item.UID, Item);
                    }
                }
                binary.Close();
            }
            binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\ClaimContainer.bin", FileMode.Open))
            {
                Game.MsgServer.MsgDetainedItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    binary.Read(&Item, sizeof(Game.MsgServer.MsgDetainedItem));

                    if (Item.UID >= Role.Instance.Confiscator.CounterUID.Count)
                        Role.Instance.Confiscator.CounterUID.Set(Item.UID + 1);

                    if (!PollContainers.ContainsKey(Item.GainerUID))
                        PollContainers.TryAdd(Item.GainerUID, new Role.Instance.Confiscator());
                    if (!PollContainers.ContainsKey(Item.OwnerUID))
                        PollContainers.TryAdd(Item.OwnerUID, new Role.Instance.Confiscator());

                    Item.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(Item.Date).Ticks).Days);
                    /* if (Item.DaysLeft > 7)
                      {
                          Item.Action = Game.MsgServer.MsgDetainedItem.ContainerType.RewardCps;
                      }*/

                    PollContainers[Item.GainerUID].ClaimContainer.TryAdd(Item.UID, Item);
                }
                binary.Close();
            }
        }
    }
}
