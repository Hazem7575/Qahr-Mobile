using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Client;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class AgateEx
    {
        public static void GetAgate(this ServerSockets.Packet stream, out AgateEx.AgateActions action, out uint UID, out sbyte Map)
        {
            action = (AgateEx.AgateActions)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Map = stream.ReadInt8();
        }

        public enum AgateActions
        {
            Record = 1,
            Recall = 3,
            Repair = 4,
        }
    }
    public class MsgMemoryAgate
    {
        public static readonly List<uint> revnomap = new List<uint>() { (uint)1, (uint)2, (uint)3, 2060U, 12602U, 12275U, 9855U, 12546U, 13249U, 12602U, 13140U, 12546U, 12800U, 11331U, 12799U, 11996U, 12665U, 12601U, 12816U, 12099U, 7009U, 2060U, 1950U, 1005U, 7005U, 7006U, 7008U, 6000U, 6004U, 6001U, 6002U, 6003U, 1844U, 7001U, 1801U, 1508U, 1518U, 7777U, 8877U, 3333U, 1090U, 1225U, 1860U, 700U, 3073U, 2064U, 1038U };
        public static List<uint> BlockedMaps = new List<uint>(){2071U,1005U,1036U,2056U};

       [PacketAttribute(GamePackets.MsgSuperFlag)]
        public static void MemoryAgateHandler(GameClient client, ServerSockets.Packet stream)
        {
            if (Pool.Constants.SkillsNotAvailableHere.Contains(client.Player.Map))
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }

            if (client.Player.Map == 5061 || client.Player.Map == 3030 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }

            if (client.Player.Map == 11 || client.Player.Map == 3030 || client.Player.Map == 3030 || client.Player.Map == 7 || client.Player.Map == 5 || client.Player.Map == 15 || client.Player.Map == 14 || client.Player.Map == 12 || client.Player.Map == 8 || client.Player.Map == 20 || client.Player.Map == 1730)
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }

            if (client.Player.Map == 12667 || client.Player.Map == 3030 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 12602 || client.Player.Map == 3071 || client.Player.Map == 10741 || client.Player.Map == 2071 || client.Player.Map == 3030 || client.Player.Map == 10133)
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }

            if (client.Player.Map == 10134 || client.Player.Map == 3030 || client.Player.Map == 3030 || client.Player.Map == 12099 || client.Player.Map == 3071 || client.Player.Map == 10133 || client.Player.Map == 22348 || client.Player.Map == 12344 || client.Player.Map == 10070 || client.Player.Map == 10504 || client.Player.Map == 12344 || client.Player.Map == 6521 || client.Player.Map == 95152)
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }
            if (client.Player.Map == 12546 || client.Player.Map == 3030 || client.Player.Map == 12602 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 13249 || client.Player.Map == 12602 || client.Player.Map == 13140 || client.Player.Map == 12546 || client.Player.Map == 12800 || client.Player.Map == 11331 || client.Player.Map == 12799 || client.Player.Map == 11996 || client.Player.Map == 12665 || client.Player.Map == 12601 || client.Player.Map == 12816 || client.Player.Map == 12099)
            {
                client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                return;
            }
            AgateEx.AgateActions action;
            uint UID;
            sbyte Map;
            stream.GetAgate(out action, out UID, out Map);
            switch (action)
            {
                case AgateEx.AgateActions.Record:
                    {
                        if (Pool.Constants.SkillsNotAvailableHere.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12546 || client.Player.Map == 12602 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 13249 || client.Player.Map == 12602 || client.Player.Map == 13140 || client.Player.Map == 12546 || client.Player.Map == 12800 || client.Player.Map == 11331 || client.Player.Map == 12799 || client.Player.Map == 11996 || client.Player.Map == 12665 || client.Player.Map == 12601 || client.Player.Map == 12816 || client.Player.Map == 12099)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 5061 || client.Player.Map == 3030 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 11 || client.Player.Map == 3030 || client.Player.Map == 7 || client.Player.Map == 5 || client.Player.Map == 15 || client.Player.Map == 14 || client.Player.Map == 12 || client.Player.Map == 8 || client.Player.Map == 20 || client.Player.Map == 1730)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12667 || client.Player.Map == 3030 || client.Player.Map == 12275 || client.Player.Map == 12602 || client.Player.Map == 10741 || client.Player.Map == 2071 || client.Player.Map == 3030 || client.Player.Map == 10133 || client.Player.Map == 3071)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 10134 || client.Player.Map == 3030 || client.Player.Map == 12099 || client.Player.Map == 10133 || client.Player.Map == 22348 || client.Player.Map == 3071 || client.Player.Map == 12344 || client.Player.Map == 10070 || client.Player.Map == 10504 || client.Player.Map == 12344 || client.Player.Map == 6521 || client.Player.Map == 95152)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }
                        if (!client.Player.Alive || client.Player.DeadState || client.Player.DynamicID > 0U)
                            break;
                        MsgGameItem msgItemInfo1 = null;
                        if (MsgMemoryAgate.revnomap.Contains(client.Player.Map) || client.Player.Map == 1210U || client.Player.Map == 1700U || client.Player.Map == 9855U || !client.Inventory.TryGetItem(UID, out msgItemInfo1))
                            break;
                        if (msgItemInfo1.Agate_map.ContainsKey((uint)Map))
                        {
                            if (MsgMemoryAgate.BlockedMaps.Contains(client.Player.Map))
                                break;
                            Dictionary<uint, string> agateMap = msgItemInfo1.Agate_map;
                            int num1 = (int)Map;
                            string[] strArray = new string[5]
            {
              client.Player.Map.ToString(),
              "~",
              null,
              null,
              null
            };
                            ushort num2 = client.Player.X;
                            strArray[2] = num2.ToString();
                            strArray[3] = "~";
                            num2 = client.Player.Y;
                            strArray[4] = num2.ToString();
                            string str = string.Concat(strArray);
                            agateMap[(uint)num1] = str;
                            msgItemInfo1.SendAgate(client);
                            break;
                        }
                        if ((int)Map > msgItemInfo1.Agate_map.Count)
                        {
                            msgItemInfo1.Agate_map.Add((uint)(byte)msgItemInfo1.Agate_map.Count, client.Player.Map.ToString() + "~" + client.Player.X.ToString() + "~" + client.Player.Y.ToString());
                            msgItemInfo1.SendAgate(client);
                            break;
                        }
                        if (msgItemInfo1.Agate_map.ContainsKey((uint)Map) || MsgMemoryAgate.BlockedMaps.Contains(client.Player.Map))
                            break;
                        Dictionary<uint, string> agateMap1 = msgItemInfo1.Agate_map;
                        int num3 = (int)Map;
                        string[] strArray1 = new string[5]
          {
            client.Player.Map.ToString(),
            "~",
            null,
            null,
            null
          };
                        ushort num4 = client.Player.X;
                        strArray1[2] = num4.ToString();
                        strArray1[3] = "~";
                        num4 = client.Player.Y;
                        strArray1[4] = num4.ToString();
                        string str1 = string.Concat(strArray1);
                        agateMap1.Add((uint)num3, str1);
                        msgItemInfo1.SendAgate(client);
                        break;
                    }
                case AgateEx.AgateActions.Recall:
                    {
                        if (Pool.Constants.SkillsNotAvailableHere.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12546 || client.Player.Map == 12602 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 13249 || client.Player.Map == 12602 || client.Player.Map == 13140 || client.Player.Map == 12546 || client.Player.Map == 12800 || client.Player.Map == 11331 || client.Player.Map == 12799 || client.Player.Map == 11996 || client.Player.Map == 12665 || client.Player.Map == 12601 || client.Player.Map == 12816 || client.Player.Map == 12099)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 5061 || client.Player.Map == 3030 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 11 || client.Player.Map == 3030 || client.Player.Map == 7 || client.Player.Map == 5 || client.Player.Map == 15 || client.Player.Map == 14 || client.Player.Map == 12 || client.Player.Map == 8 || client.Player.Map == 20 || client.Player.Map == 1730)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12667 || client.Player.Map == 3030 || client.Player.Map == 12275 || client.Player.Map == 12602 || client.Player.Map == 10741 || client.Player.Map == 2071 || client.Player.Map == 3030 || client.Player.Map == 10133 || client.Player.Map == 3071)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 10134 || client.Player.Map == 3030 || client.Player.Map == 12099 || client.Player.Map == 10133 || client.Player.Map == 3071 || client.Player.Map == 22348 || client.Player.Map == 12344 || client.Player.Map == 10070 || client.Player.Map == 10504 || client.Player.Map == 12344 || client.Player.Map == 6521 || client.Player.Map == 95152)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }
                        if (!client.Player.Alive || client.Player.DeadState || client.Player.DynamicID > 0U)
                            break;
                        MsgGameItem msgItemInfo2 = (MsgGameItem)null;
                        if (MsgMemoryAgate.revnomap.Contains(client.Player.Map) || client.Player.Map == 1210U || client.Player.Map == 1700U || client.Player.Map == 9855U || !client.Inventory.TryGetItem(UID, out msgItemInfo2) || !msgItemInfo2.Agate_map.ContainsKey((uint)Map) || ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[0].ToString()) == (ushort)1038 || ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[0].ToString()) == (ushort)6001)
                            break;
                        ushort num5 = ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[0].ToString());
                        if (MsgMemoryAgate.BlockedMaps.Contains((uint)num5))
                            break;
                        client.Teleport(ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[1].ToString()), ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[2].ToString()), (uint)ushort.Parse(msgItemInfo2.Agate_map[(uint)Map].Split('~')[0].ToString()));
                        --msgItemInfo2.Durability;
                        msgItemInfo2.SendAgate(client);
                        break;
                    }
                case AgateEx.AgateActions.Repair:
                    {
                        if (Pool.Constants.SkillsNotAvailableHere.Contains(client.Player.Map))
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12546 || client.Player.Map == 12602 || client.Player.Map == 12275 || client.Player.Map == 3030 || client.Player.Map == 13249 || client.Player.Map == 12602 || client.Player.Map == 13140 || client.Player.Map == 12546 || client.Player.Map == 12800 || client.Player.Map == 11331 || client.Player.Map == 12799 || client.Player.Map == 11996 || client.Player.Map == 12665 || client.Player.Map == 12601 || client.Player.Map == 12816 || client.Player.Map == 12099)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 5061 || client.Player.Map == 3030 || client.Player.Map == 5062 || client.Player.Map == 5063 || client.Player.Map == 5064 || client.Player.Map == 5065 || client.Player.Map == 5066 || client.Player.Map == 5051 || client.Player.Map == 5052 || client.Player.Map == 5053 || client.Player.Map == 5054 || client.Player.Map == 5055 || client.Player.Map == 5056 || client.Player.Map == 5057 || client.Player.Map == 5058)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 11 || client.Player.Map == 3030 || client.Player.Map == 7 || client.Player.Map == 5 || client.Player.Map == 15 || client.Player.Map == 14 || client.Player.Map == 12 || client.Player.Map == 8 || client.Player.Map == 20 || client.Player.Map == 1730)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 12667 || client.Player.Map == 3030 || client.Player.Map == 12275 || client.Player.Map == 12602 || client.Player.Map == 10741 || client.Player.Map == 2071 || client.Player.Map == 3030 || client.Player.Map == 10133 || client.Player.Map == 3071)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }

                        if (client.Player.Map == 10134 || client.Player.Map == 3030 || client.Player.Map == 12099 || client.Player.Map == 10133 || client.Player.Map == 22348 || client.Player.Map == 12344 || client.Player.Map == 3071 || client.Player.Map == 10070 || client.Player.Map == 10504 || client.Player.Map == 12344 || client.Player.Map == 6521 || client.Player.Map == 95152)
                        {
                            client.SendSysMesage("Sorry, You Cannot Use Memory Agate In This Map.");
                            return;
                        }
                        if (client.Player.DeadState || !client.Player.Alive)
                            break;
                        MsgGameItem msgItemInfo3 = (MsgGameItem)null;
                        if (!client.Inventory.TryGetItem(UID, out msgItemInfo3))
                            break;
                        uint num6 = ((uint)msgItemInfo3.MaximDurability - (uint)msgItemInfo3.Durability) / 2U;
                        if (num6 == 0U)
                            num6 = 1U;
                        if (client.Player.ConquerPoints > num6)
                        {
                            client.Player.ConquerPoints -= (uint)num6;
                            msgItemInfo3.Durability = msgItemInfo3.MaximDurability;
                            msgItemInfo3.SendAgate(client);
                        }
                        break;
                    }
            }
        }

    }
}
