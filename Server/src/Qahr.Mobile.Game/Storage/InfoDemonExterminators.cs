using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class InfoDemonExterminators
    {
        public static Dictionary<uint, List<MonsterInfo>> CityQuests = new Dictionary<uint, List<MonsterInfo>>();

        public class MonsterInfo
        {
            public byte Level = 0;
            public uint ID = 0;
            public string Name = "";
        }


        public static void Create()
        {

            CityQuests.Add(1002, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =1, Name = "Pheasant" },
                new MonsterInfo(){ ID =2, Name = "Turtledove" },
                new MonsterInfo(){ ID =3, Name = "Robin" },
                new MonsterInfo(){ ID =4, Name = "Apparition" },
                new MonsterInfo(){ ID =5, Name = "Poltergeist" }
            });
            CityQuests.Add(1011, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =6, Name = "WingedSnake" },
                new MonsterInfo(){ ID =7, Name = "Bandit" },
                new MonsterInfo(){ ID =8, Name = "Ratling" },
                new MonsterInfo(){ ID = 113, Name = "FireSpirit" }//9
            });
            CityQuests.Add(1020, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =10, Name = "Macaque" },
                new MonsterInfo(){ ID =11, Name = "GiantApe" },
                new MonsterInfo(){ ID =12, Name = "ThunderApe" },
                new MonsterInfo(){ ID =13, Name = "Snakeman" }
            });
            CityQuests.Add(1000, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =14, Name = "SandMonster" },
                new MonsterInfo(){ ID =15, Name = "HillMonster" },
                new MonsterInfo(){ ID =16, Name = "RockMonster" },
                new MonsterInfo(){ ID =17, Name = "BladeGhost" }
            });
            CityQuests.Add(1015, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =18, Name = "Birdman" },
                new MonsterInfo(){ ID =19, Name = "HawKing" },
                new MonsterInfo(){ ID =55, Name = "BanditL97" }
            });
            CityQuests.Add(1001, new List<MonsterInfo>()
            {
                new MonsterInfo(){ ID =20, Name = "TombBat" },
                new MonsterInfo(){ ID =56, Name = "BloodyBat" },
                new MonsterInfo(){ ID =57, Name = "BullMonster" },
                new MonsterInfo(){ ID =58, Name = "RedDevilL117" }
            });

           

            foreach (var map in CityQuests.Values)
            {
                foreach (var mob in map)
                {
                    if (Pool.MonsterFamilies.ContainsKey(mob.ID))
                        mob.Level = (byte)(Pool.MonsterFamilies[mob.ID].Level);
                }
            }

        }
        public static ushort GetNextMap(byte level)
        {
            return (ushort)1020;
        }
       

        internal static ushort[] StageKills = new ushort[] { 600, 1200, 1500, 1800 };

        internal static ushort[][] QuestTyp = new[]
       {  
           new ushort[] {1/*Pheasants*/,8/*Ratlings*/,14/*Sandmonsters*/ ,55/*97 Bandits*/},
           new ushort[] {2/*TurtleDoves*/,7/*Bandits*/,13/*Snakemen*/,18/*Birdmen*/},
           new ushort[] {4/*Aparitions*/,11/*Giant Apes*/,17/*Blade Ghosts*/,58/*Red Devils*/},
           new ushort[] {5/*Poltergeists*/,12/*Thunder Apes*/,19/*Hawkings*/,57/*BullMonsters*/},
           new ushort[] {6/*Winged Snakes*/,9/*Fire Spirits*/,15/*Hill Monsters*/,20/*Tomb Bats*/},
           new ushort[] {3/*Robins*/,10/*Monkeys and Macaques*/,16/*Rock Monsters*/,56/*Bloody Bats*/}
        };

        internal static string[][] QuestName = new[]
        {
           new string[] {"Pheasants","AgentSpy","Ratlings","Sandmonsters" ,"97 Bandits"},
           new string[] {"TurtleDoves","Bandits","Snakemen","Birdmen"},
           new string[] {"Aparitions","Giant Apes","Blade Ghosts","Red Devils"},
           new string[] {"Poltergeists","Thunder Apes","Hawkings","BullMonsters"},
           new string[] {"Winged Snakes","Fire Spirits","Hill Monsters","Tomb Bats"},
           new string[] {"Robins","Monkeys and Macaques","Rock Monsters","Bloody Bats"}
        };

       

    }
}
