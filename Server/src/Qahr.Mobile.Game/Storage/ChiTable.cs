using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class ChiTable
    {
        
        public static string PowersToString(Role.Instance.Chi.ChiAttribute[] Powers)
        {
            if (Powers == null)
                return "";
            string str = (int)Powers[0].Type + "-" + Powers[0].Value + "|" + (int)Powers[1].Type + "-" + Powers[1].Value + "|" + (int)Powers[2].Type + "-" + Powers[2].Value + "|" + (int)Powers[3].Type + "-" + Powers[3].Value;
            return str;
        }
        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;
                uint UID = ini.ReadUInt32("Character", "UID", 0);
                Role.Instance.Chi playerchi = new Role.Instance.Chi(UID);
                string Name = ini.ReadString("Character", "Name", "None");
                playerchi.Name = Name;
                playerchi.ChiPoints = ini.ReadInt32("Character", "ChiPoints", 0);
                playerchi.Dragon.Load(ini.ReadString("Character", "Dragon", ""), UID, Name);
                playerchi.Phoenix.Load(ini.ReadString("Character", "Pheonix", ""), UID, Name);
                playerchi.Turtle.Load(ini.ReadString("Character", "Turtle", ""), UID, Name);
                playerchi.Tiger.Load(ini.ReadString("Character", "Tiger", ""), UID, Name);
                #region Timer
                playerchi.DragonTime = ini.ReadInt64("Character", "DragonTime", 0);
                if (playerchi.DragonTime != 0)
                {
                    playerchi.DragonTime = 0;
                    playerchi.DragonPowers = null;
                }
                playerchi.PhoenixTime = ini.ReadInt64("Character", "PhoenixTime", 0);
                if (playerchi.PhoenixTime != 0)
                {
                    playerchi.PhoenixTime = 0;
                    playerchi.PhoenixPowers = null;
                }
                playerchi.TurtleTime = ini.ReadInt64("Character", "TurtleTime", 0);
                if (playerchi.TurtleTime != 0)
                {
                    playerchi.TurtleTime = 0;
                    playerchi.TurtlePowers = null;

                }
                playerchi.TigerTime = ini.ReadInt64("Character", "TigerTime", 0);
                if (playerchi.TigerTime != 0)
                {
                    playerchi.TigerTime = 0;
                    playerchi.TigerPowers = null;

                }
                #endregion
                if (playerchi.Dragon.UnLocked)
                {
                    Role.Instance.Chi.ChiPool.TryAdd(playerchi.UID, playerchi);
                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, playerchi.Dragon);
                }
                if (playerchi.Phoenix.UnLocked)
                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Phoenix, playerchi.Phoenix);
                if (playerchi.Tiger.UnLocked)
                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Tiger, playerchi.Tiger);
                if (playerchi.Turtle.UnLocked)
                    Pool.ChiRanking.Upadte(Pool.ChiRanking.Turtle, playerchi.Turtle);
            }

        }
    }
}
