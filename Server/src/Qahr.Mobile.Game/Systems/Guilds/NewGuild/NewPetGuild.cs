using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Role;
using Qahr.Mobile.Role.Instance;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class NewPetGuild
    {
        public static uint UID = 700010;
        public static void Death(GameClient user, ServerSockets.Packet stream)
        {
            foreach (MonsterRole listPet in user.Player.ListPets)
            {
                listPet.GMap.View.LeaveMap<IMapObj>((IMapObj)listPet);
                user.Map.SetMonsterOnTile(listPet.X, listPet.Y, false);
                ActionQuery pQuery = new ActionQuery()
                {
                    ObjId = listPet.UID,
                    Type = 135
                };
                user.Player.View.SendView(stream.ActionCreate(pQuery), true);
            }
            user.Player.ListPets.Clear();
            user.Player.RemoveFlag(MsgUpdate.Flags.LotusDemon);
            user.Player.RemoveFlag(MsgUpdate.Flags.SierraBeast);
        }

        public static void AddPet(GameClient user, uint id, uint Flag, ushort X, ushort Y, ServerSockets.Packet stream)
        {
            MonsterFamily monsterFamily;
            if (!Pool.MonsterFamilies.TryGetValue(1U, out monsterFamily))
                return;
            MonsterFamily Famili = monsterFamily.Copy();
            Famili.SpawnX = X;
            Famili.SpawnY = Y;
            Famili.MaxSpawnX = X;
            Famili.MaxSpawnY = Y;
            Famili.MapID = user.Player.Map;
            Famili.SpawnCount = (byte)1;
            MonsterRole monsterRole = user.Map.MonstersColletion.Add(Famili, true, user.Player.DynamicID, true);
            if (monsterRole == null)
                return;
            monsterRole.UID = NewPetGuild.UID;
            ++NewPetGuild.UID;
            monsterRole.Family.ID = id;
            monsterRole.FloorStampTimer = DateTime.Now.AddSeconds(7.0);
            monsterRole.Family.SpellId = 13190U;
            monsterRole.Family.AttackSpeed = 1000;
            monsterRole.Family.Settings = MonsterSettings.Lottus;
            monsterRole.PetFlag = Flag;
            monsterRole.Boss = (byte)0;
            monsterRole.Level = (byte)140;
            monsterRole.Name = "";
            monsterRole.Map = 1038U;
            if (id >= 6639U && id <= 6658U)
            {
                monsterRole.Mesh = 138U;
                monsterRole.HitPoints = 10U;
                monsterRole.Family.MaxHealth = 10;
            }
            if (id >= 6659U && id <= 6678U)
            {
                monsterRole.Mesh = 137U;
                monsterRole.HitPoints = 10U;
                monsterRole.Family.MaxHealth = 10;
            }
            if (id >= 6679U && id <= 6698U)
            {
                monsterRole.Mesh = 848U;
                monsterRole.HitPoints = 10U;
                monsterRole.Family.MaxHealth = 10;
            }
            monsterRole.StampFloorSecounds = 10000;
            monsterRole.FloorStampTimer = DateTime.Now.AddSeconds(1.0);
            monsterRole.OwnerFloor = user;
            user.Map.View.EnterMap<IMapObj>((IMapObj)monsterRole);
            user.Send(stream.CreatePetInfo(monsterRole.UID, id, monsterRole.PetFlag, monsterRole.Mesh, (ushort)14, monsterRole.X, monsterRole.Y, monsterRole.Name));
            monsterRole.Send(monsterRole.GetArray(stream, false));
            ActionQuery pQuery = new ActionQuery()
            {
                ObjId = monsterRole.UID,
                Type = 134,
                PositionX = (uint)monsterRole.X,
                PositionY = (uint)monsterRole.Y
            };
            user.Player.ListPets.Add(monsterRole);
            user.Player.View.SendView(stream.ActionCreate(pQuery), true);
            user.Map.GetRandCoord(ref X, ref Y, 5);
        }
    }
}
