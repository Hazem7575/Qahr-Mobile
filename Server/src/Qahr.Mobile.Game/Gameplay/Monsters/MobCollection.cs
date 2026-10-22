using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgMonster
{
    public class MobCollection
    {
        public const byte Multiple = 3;

        public string LocationSpawn = "";

        public object SyncRoot = new object();
        public static Extensions.Counter GenerateUid = new Extensions.Counter(400000);

        //  public Extensions.SafeDictionary<uint, MonsterRole> Roles = new Extensions.SafeDictionary<uint, MonsterRole>();

        public Role.GameMap DMap = null;

        private uint DmapID = 0;
        public MobCollection(uint Map)
        {
            DmapID = Map;
            if (Pool.ServerMaps != null)
            {
                if (Pool.ServerMaps.TryGetValue(Map, out DMap))
                    DMap.MonstersColletion = this;
            }
        }
        public bool ReadMap()
        {
            if (Pool.ServerMaps != null)
            {
                if (Pool.ServerMaps.TryGetValue(DmapID, out DMap))
                    DMap.MonstersColletion = this;
            }

            return DMap != null;
        }
        public MonsterRole Add(MonsterFamily Famili, bool RemoveOnDead = false, uint dinamicid = 0, bool justone = false)
        {
            if (DMap == null)
                ReadMap();

            MonsterRole monsterr = null;

            int Multi = 1;

            switch (DmapID)
            {
                case 1762: // Frozen Grotto
                case 1927: // Frozen Grotto 2
                    Multi = 5;
                    break;

                case 1015: // Bird Island
                    Multi = 3;
                    break;

                case 3071: // SquidwardOctopus
                    Multi = 1;
                    break;              

                case 1000: // Desert City
                    Multi = 5;
                    break;

                case 1002: // Twin City
                    Multi = 3;
                    break;

                case 12288: // Hunting Map (1)
                    Multi = 2;
                    break;

                case 9991: // Hunting Map (2)
                    Multi = 2;
                    break;

                case 9992: // Hunting Map (3)
                    Multi = 2;
                    break;

                case 9993: // Hunting Map (4)
                    Multi = 2;
                    break;

                case 9990: // Hunting Map (5)
                    Multi = 2;
                    break;

                case 10133: // Shadow Castle
                    Multi = 2;
                    break;
            }


            int count;


            // لو طلبت وحش واحد من الكود نفسه لا تضاعفه
            if (justone)
            {
                count = 1;
            }

            // Boss لا يزيد
            else if (Famili.Boss > 0)
            {
                count = 1;
            }

            // Guards لا تزيد
            else if (Famili.Name == "Guard1" || Famili.Name == "Guard2")
            {
                count = 1;
            }

            // وحوش محددة لا تزيد
            else if (
                Famili.ID == 40947 ||
                Famili.ID == 20070 ||
                Famili.ID == 20160 ||
                Famili.ID == 40124 ||
                Famili.ID == 7696 ||
                Famili.ID == 2699 ||
                Famili.ID == 4220 ||
                Famili.ID == 4171 ||
                Famili.ID == 4212 ||
                Famili.ID == 3970 ||
                Famili.ID == 3978 ||
                Famili.ID == 3976 ||
                Famili.ID == 3977 ||
                Famili.ID == 3971 ||
                Famili.ID == 3636 ||
                Famili.ID == 2700)
            {
                count = 1;
            }

            else if (DmapID == 3071)
            {
                count = 7;
            }

            else if (DmapID == 1020)
            {
                count = 10;
            }

            else
            {
                count = Math.Max(1, (int)Famili.SpawnCount) * Multi;
            }



            for (int x = 0; x < count; x++)
            {

                MonsterRole Mob = new MonsterRole(
                    Famili.Copy(),
                    GenerateUid.Next,
                    LocationSpawn,
                    DMap);


                // مهم عشان الوحش يموت ويتشال طبيعي
                Mob.RemoveOnDead = RemoveOnDead;


                ushort _x = 0;
                ushort _y = 0;


                TryObtainSpawnXY(Famili, out _x, out _y);


                if (!DMap.ValidLocation(_x, _y) || DMap.MonsterOnTile(_x, _y))
                    continue;


                DMap.SetMonsterOnTile(_x, _y, true);


                Mob.X = _x;
                Mob.Y = _y;

                Mob.RespawnX = _x;
                Mob.RespawnY = _y;

                Mob.Map = DMap.ID;

                Mob.DynamicID = dinamicid;


                monsterr = Mob;


                DMap.View.EnterMap<MonsterRole>(Mob);
            }


            return monsterr;
        }

        /// <summary>
        /// Attemps to obtain a point where the monster can be re-spawned.
        /// </summary>
        /// <param name="X">The x-coordinate point.</param>
        /// <param name="Y">The y-coordinate point.</param>
        public void TryObtainSpawnXY(MonsterFamily Monster, out ushort X, out ushort Y)
        {

            X = (ushort)Program.GetRandom.Next(Monster.SpawnX, Monster.MaxSpawnX);
            Y = (ushort)Program.GetRandom.Next(Monster.SpawnY, Monster.MaxSpawnY);

            for (byte i = 0; i < 10; i++)
            {
                if (DMap == null)
                    break;
                if (DMap.ValidLocation(X, Y) && !DMap.MonsterOnTile(X, Y))
                    break;

                X = (ushort)Program.GetRandom.Next(Monster.SpawnX, Monster.MaxSpawnX);
                Y = (ushort)Program.GetRandom.Next(Monster.SpawnY, Monster.MaxSpawnY);
            }
        }
    }
}
