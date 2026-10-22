using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class FlowersTable
    {
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                Role.Instance.Flowers flower = new Role.Instance.Flowers(UID, Name);
                string FlowerArray = ini.ReadBigString("Character", "Flowers", "None");
                Database.DBActions.ReadLine reader = new DBActions.ReadLine(FlowerArray, '/');
                flower.FreeFlowers = reader.Read((uint)0);
                foreach (var flow in flower)
                {
                    flow.Amount = reader.Read((uint)0);
                    flow.Amount2day = reader.Read((uint)0);
                    flow.Head = reader.Read((uint)0);
                    flow.Mesh = reader.Read((uint)0);
                    flow.HairStyle = reader.Read((uint)0);
                    flow.Garment = reader.Read((uint)0);
                    flow.LeftWeapon = reader.Read((uint)0);
                    flow.LefttWeaponAccessory = reader.Read((uint)0);
                    flow.RightWeapon = reader.Read((uint)0);
                    flow.RightWeaponAccessory = reader.Read((uint)0);
                    flow.MountArmor = reader.Read((uint)0);
                    flow.Armor = reader.Read((uint)0);
                    flow.Wing = reader.Read((uint)0);
                    flow.WingPlus = reader.Read((uint)0);
                    flow.Title = reader.Read((uint)0);
                    flow.Flag = reader.Read((uint)0);
                    flow.FrameID = reader.Read((uint)0);
                    flow.GuildName = reader.Read("");
                    flow.FlowerFree = reader.Read((uint)0);
                }
                if (flower.Sum((x) => x.Amount) != 0)
                {
                    if (!Role.Instance.Flowers.ClientPoll.ContainsKey(UID))
                        Role.Instance.Flowers.ClientPoll.TryAdd(UID, flower);
                    if (Role.Core.IsBoy(Body))
                    {
                        foreach (var flow in flower)
                            Pool.BoysFlowersRanking.UpdateRank(flow, flow.Type);
                    }
                    else if (Role.Core.IsGirl(Body))
                    {
                        foreach (var flow in flower)
                            Pool.GirlsFlowersRanking.UpdateRank(flow, flow.Type);
                    }
                }


            }
            // [perf] كان GC.Collect() هنا: GC كامل بيوقّف كل الثريدات (~21ms على heap 150MB). الـ runtime بيجمع لوحده.
        }
    }
}
