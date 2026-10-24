using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Game;

namespace Qahr.Mobile.Role
{
    public class RoleView
    {
        public const int ViewThreshold = 18;
        public Extensions.Time32 Monster_BuffersCallbackStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.AI_Buffers);
        public Extensions.Time32 Monster_GuardsCallbackStamp = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.AI_Guards);
        public Extensions.Time32 Monster_AliveMonstersCallback = Extensions.Time32.Now.AddMilliseconds(ThreadPlayer.AI_Monsters);

        public Game.MsgMonster.ActionHandler MobActions = new Game.MsgMonster.ActionHandler();


        public System.SafeDictionary<uint, IMapObj>[] src;
        public Client.GameClient Owner;
        public Role.Player GetPlayer()
        {
            return Owner.Player;
        }
        public RoleView(Client.GameClient _client)
        {
            Owner = _client;
            src = new System.SafeDictionary<uint, IMapObj>[(byte)MapObjectType.Count];
            for (byte x = 0; x < (byte)MapObjectType.Count; x++)
                src[x] = new System.SafeDictionary<uint, IMapObj>();
        }

        public void CheckUpMonsters(Extensions.Time32 clock)
        {
            if (Program.ExitRequested)
                return;
            if (clock > Monster_BuffersCallbackStamp)
            {
                Game.MsgMonster.PoolProcesses.BuffersCallback(Owner);
                Monster_BuffersCallbackStamp.Value = clock.Value + ThreadPlayer.AI_Buffers;
            }
            else if (clock > Monster_GuardsCallbackStamp)
            {
                Game.MsgMonster.PoolProcesses.GuardsCallback(Owner);
                Monster_GuardsCallbackStamp.Value = clock.Value + ThreadPlayer.AI_Guards;
            }
            else if (clock > Monster_AliveMonstersCallback)
            {
                Game.MsgMonster.PoolProcesses.AliveMonstersCallback(Owner);
                Monster_AliveMonstersCallback.Value = clock.Value + ThreadPlayer.AI_Monsters;
            }
        }


        public unsafe void ReSendView(ServerSockets.Packet stream)
        {
            SendView(Owner.Player.GetArray(stream, false), false);
        }
        public bool SameLocation(MapObjectType typ, out Role.IMapObj obj)
        {
            foreach (var client in Roles(typ))
            {
                if (client.X == GetPlayer().X && client.Y == GetPlayer().Y)
                {
                    obj = client;
                    return true;
                }
            }
            obj = null;
            return false;
        }
        public HashSet<uint> SigilNinja = new HashSet<uint> { 2540, 2530, 2560, 2570, 4380, 4390, 18456 };
        public HashSet<uint> SigilPirate = new HashSet<uint> { 4130, 4150, 4160, 4170, 4180, 4190, 4200, 4210 };
        public unsafe void SendView(byte[] msg, bool me)
        {

            if (me)
                Owner.Send(msg);
            foreach (IMapObj obj in Roles(MapObjectType.Player))
            {
                (obj as Role.Player).Owner.Send(msg);
            }
        }
        public unsafe void SendView(ServerSockets.Packet msg, bool me)
        {
            byte[] buffer = new byte[msg.Size];
            fixed (byte* ptr = buffer)
            {
                msg.memcpy(ptr, msg.Memory, msg.Size);
            }
            ushort id = BitConverter.ToUInt16(buffer, 2);
            uint idfloor = BitConverter.ToUInt32(buffer, 8);
            bool shouldSendToOwner = ShouldSendToOwner(id, idfloor, me);
            if (shouldSendToOwner)
            {
                Owner.Send(msg);
            }

            foreach (IMapObj obj in Roles(MapObjectType.Player))
            {
                var client = obj as Player;
                if (client != null && ShouldSendToClient(id, idfloor, client))
                {

                    client.Send(msg);
                }
            }
        }
        private bool ShouldSendToOwner(ushort id, uint idfloor, bool me)
        {
            if (!me || id != 2440)
            {
                return true;
            }

            if (Owner.Player.HideFloor)
            {
                return false;
            }

            if (SigilNinja.Contains(idfloor) && !Owner.Player.SigilNinja)
            {
                return true;
            }

            if (SigilPirate.Contains(idfloor) && !Owner.Player.SigilPirate)
            {
                return true;
            }

            return !SigilNinja.Contains(idfloor) && !SigilPirate.Contains(idfloor);
        }

        private bool ShouldSendToClient(ushort id, uint idfloor, Player client)
        {
            if (id != 2440 || client.HideFloor)
            {
                return true;
            }

            if (SigilNinja.Contains(idfloor) && !client.SigilNinja)
            {
                return true;
            }

            if (SigilPirate.Contains(idfloor) && !client.SigilPirate)
            {
                return true;
            }

            return !SigilNinja.Contains(idfloor) && !SigilPirate.Contains(idfloor);
        }


        private bool ShouldSendToClient(ushort idPacket, uint idTrap, Player client, HashSet<uint> sigilNinja, HashSet<uint> sigilPirate)
        {
            return idPacket == 2440 &&
                   (sigilNinja.Contains(idTrap) && client.SigilNinja ||
                    sigilPirate.Contains(idTrap) && client.SigilPirate);
        }


        //public unsafe HashSet<uint> SigilNinja = new HashSet<uint> { 2540, 2530, 2560, 2570, 4380, 4390, 18456 };
        //public unsafe HashSet<uint> SigilPirate = new HashSet<uint> { 4130, 4150, 4160, 4170, 4180, 4190, 4200,4210 };
       
        //public unsafe void SendView(ServerSockets.Packet msg, bool me)
        //{
        //    byte[] buffer = new byte[msg.Size];
        //    fixed (byte* ptr = buffer)
        //    {
        //        msg.memcpy(ptr, msg.Memory, msg.Size);
        //    }
        //    ushort id = BitConverter.ToUInt16(buffer, 2);
        //    uint idfloor = BitConverter.ToUInt32(buffer, 8);
        //    if (me)
        //    {
        //        if (id == 2440)
        //        {
        //            if (!Owner.Player.HideFloor)
        //            {
        //                if (SigilNinja.Contains(idfloor) || SigilPirate.Contains(idfloor))
        //                {
        //                    if (SigilNinja.Contains(idfloor))
        //                    {
        //                        if (!Owner.Player.SigilNinja )
        //                        {
        //                            Owner.Send(msg);
        //                        }
        //                    }
        //                    if (SigilPirate.Contains(idfloor))
        //                    {
        //                        if (!Owner.Player.SigilPirate)
        //                        {
        //                            Owner.Send(msg);
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    Owner.Send(msg);
        //                }
        //            }
        //        }
        //        else 
        //        {
        //            Owner.Send(msg);
        //        }
        //    }
        //    foreach (IMapObj obj in Roles(MapObjectType.Player))
        //    {
        //        var Client = obj as Player;
        //        if (id == 2440)
        //        {
        //            if (!Client.HideFloor)
        //            {
        //                if (SigilNinja.Contains(idfloor))
        //                {
        //                    if (Client != null)
        //                    {

        //                        if (!Client.SigilNinja)
        //                        {
        //                            obj.Send(msg);
        //                        }
        //                    }
        //                }
        //                if (SigilPirate.Contains(idfloor))
        //                {
        //                    if (Client != null)
        //                    {
        //                        if (!Client.SigilPirate)
        //                        {
        //                            obj.Send(msg);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    obj.Send(msg);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            obj.Send(msg);
        //        }
        //    }
        //}

    

        public IEnumerable<IMapObj> Roles(MapObjectType typ, Predicate<bool> P = null)
        {
            if (Owner.Map != null)
            {
                if (P != null)
                    return Owner.Map.View.Roles(typ, Owner.Player.X, Owner.Player.Y, p => CanSee(p) && P(p.Alive));
                else
                    return Owner.Map.View.Roles(typ, Owner.Player.X, Owner.Player.Y, p => CanSee(p));
            }
            else
                return new IMapObj[0];
        }
        public IEnumerable<IMapObj> AttackableRoles()
        {
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.Monster, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.Player, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
            foreach (var rule in Owner.Map.View.Roles(MapObjectType.SobNpc, Owner.Player.X, Owner.Player.Y, x => CanSee(x)))
                yield return rule;
        }
        public bool TryGetValue(uint UID, out IMapObj obj, MapObjectType typ)
        {
            if (Owner.Map != null)
                return Owner.Map.View.TryGetObject<IMapObj>(UID, typ, Owner.Player.X, Owner.Player.Y, out obj);
            obj = null;
            return false;
        }
        public bool CanSee(IMapObj obj)
        {
            try
            {
                if (obj == null)
                    return false;
                if (obj.Map != Owner.Player.Map)
                    return false;
                if (!obj.AllowDynamic)
                    if (obj.DynamicID != Owner.Player.DynamicID)
                        return false;
                if (obj.UID == Owner.Player.UID)
                    return false;
                if (obj.Map == 12431 && !global::Qahr.Mobile.DreamTheater.DreamTheaterModule.TheaterNpcVisible(Owner, obj)) return false;// [DreamTheater 2026-10-15] per-player theater NPCs (recruited actors, cleaning spots, corners)

                return Core.GetDistance(obj.X, obj.Y, Owner.Player.X, Owner.Player.Y) <= ViewThreshold;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
        public bool Contains(IMapObj obj)
        {
            if (obj.UID == Owner.Player.UID)
                return true;
            if (Owner.Map != null)
                return Owner.Map.View.Contain(obj.UID, Owner.Player.X, Owner.Player.Y);
            return false;
        }
        public bool ContainMobInScreen(string name)
        {
            bool contain = false;
            foreach (var obj in Roles(MapObjectType.Monster))
            {
                var monster = obj as Game.MsgMonster.MonsterRole;
                if (monster.Name == name)
                {
                    contain = true;
                    break;
                }
            }
            return contain;
        }
     
      public unsafe bool CanAdd(IMapObj obj, bool Force, ServerSockets.Packet stream)
{
    try
    {
        if (!CanSee(obj))
            return false;
        if (Owner.Player.InView(obj.X, obj.Y, ViewThreshold) || Force)
        {
            try
            {
                if (obj.ObjType == MapObjectType.Monster)
                {

                    if (!obj.Alive)
                    {
                        var monster = obj as Game.MsgMonster.MonsterRole;
                        if (monster.CanRespawn(Owner.Map) && !Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(monster.Family.ID))
                            monster.Respawn(false);
                    }

                    else
                        Owner.Send(obj.GetArray(stream, false));
                }
                else if (obj.ObjType == MapObjectType.Player)
                {

                    if (Owner.Player.Map == 700)
                    {
                        var pClient = (obj as Role.Player).Owner;
                        if (Owner.InQualifier() && pClient.IsWatching())
                            return true;
                        if (pClient.IsWatching())
                            return true;
                    }
                    var apClient = (obj as Role.Player).Owner;
                    if (Owner.Player.Invisible == false && apClient.Player.Invisible == true)
                        return true;
                    if (apClient.Player.Invisible)
                        return true;

                    Owner.Send(obj.GetArray(stream, false));
                  
                    //var clones = Owner.Player.MyClones.ToArray();
                    foreach (var clone in Owner.Player.MyClones.GetValues())
                    {
                        clone.Send((obj as Role.Player).Owner, stream);
                    }
                    if (Force && Owner.IsWatching() == false && Owner.Player.Invisible == false)
                    {
                       
                        //var clones2 = (obj as Role.Player).MyClones.ToArray();
                        foreach (var clone in Owner.Player.MyClones.GetValues())
                        {
                            clone.Send(Owner, stream);
                        }
                    }
                }
                else if (obj.ObjType == MapObjectType.Item)
                {
                    if (Owner.Player.HideFloor)
                        return false;

                    if (!obj.Alive)
                    {
                        var item = obj as Game.MsgFloorItem.MsgItem;
                        item.SendAll(stream, Game.MsgFloorItem.MsgDropID.Remove);
                        Owner.Map.View.LeaveMap<IMapObj>(obj);
                    }
                    else
                    {
                        if (obj.Map == 4006)
                        {
                            var item = obj as Game.MsgFloorItem.MsgItem;
                            if (item.Typ == Game.MsgFloorItem.MsgItem.ItemType.Effect && item.MsgFloor.m_ID == 1037)
                            {
                                if (Owner.Player.JoinTowerOfMysteryLayer == 7)
                                    Owner.Send(obj.GetArray(stream, false));
                            }
                            else if (item.MsgFloor.m_ID == 810)
                                Owner.Send(obj.GetArray(stream, false));
                        }
                        else
                            Owner.Send(obj.GetArray(stream, false));
                    }
                }
                else if (obj.ObjType == MapObjectType.Npc)
                {
                    if (Phone.MobileQuestMarkers.Hide(Owner, obj.UID))
                        return false;// QahrMobile: invisible "~" quest markers only for players on that step (Mobile\Quests\MobileQuestMarkers.cs)
                    if (obj.Map == 1015 && obj.UID == (uint)Game.MsgNpc.NpcID.LittleBen)
                    {
                        if (Owner.Player.QuestGUI.CheckQuest(6129, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            return false;
                    }
                    if (obj.Map == 4009)
                    {
                        if (obj.UID == 19139)
                        {
                            Owner.Send(stream.NpcCreate(obj as Npc, (ushort)(40350 + Math.Min(8, (int)Owner.Player.JoinTowerOfMysteryLayer) * 10)));
                            return true;
                        }
                    }
                    if (obj.Map == 4000 || obj.Map == 4003 || obj.Map == 4006 || obj.Map == 4008)
                    {
                        if (Owner.Player.TOM_StartChallenge == false)
                            Owner.Send(stream.NpcCreate(obj as Npc, (ushort)(40450 + Math.Min(9, (int)Owner.Player.JoinTowerOfMysteryLayer + 1) * 10)));
                        else if (Owner.Player.TOM_FinishChallenge)
                            Owner.Send(stream.NpcCreate(obj as Npc, 40150));
                    }
                    else
                    {
                        if (Owner.OnLogin.Language == "En" || obj.Map == 1038)
                            Owner.Send(obj.GetArray(stream, false));
                        else
                            Owner.Send(obj.GetArray(stream, true));

                    }
                }
                else if (obj.ObjType == MapObjectType.SobNpc || obj.ObjType == MapObjectType.StaticRole
                    )
                        Owner.Send(obj.GetArray(stream, true));
            }
            catch (Exception e) { MyConsole.WriteException(e); }
            return true;
        }
        else
            return false;
    }
    catch (Exception e)
    {
        MyConsole.WriteException(e);
        return false;
    }
}
      
        public bool InColorScrenn = false;
        public unsafe void CheckScrennColors(ServerSockets.Packet stream)
        {


            if (InColorScrenn)
            {
                InColorScrenn = false;
                var action = new ActionQuery()
                {
                    ObjId = Owner.Player.UID,
                    Type = ActionType.SetMapColor,
                    dwParam = 0,
                    PositionX = Owner.Player.X,
                    PositionY = Owner.Player.Y
                };
                Owner.Send(stream.ActionCreate(action));
            }
        }
        public unsafe void Role(bool clear = false, ServerSockets.Packet msg = null)
        {
            try
            {
                if (Owner.Player == null || Owner.Map == null)
                    return;
                if (clear)
                {
                    Owner.Player.Px = 0;
                    Owner.Player.Py = 0;
                }

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    CheckScrennColors(stream);
                    try
                    {
                        if (Database.HouseTable.InHouse(Owner.Player.Map))
                        {
                            if (Owner.Player.UID == Owner.Player.DynamicID)
                            {
                                if (Owner.MyHouse != null)
                                {
                                    foreach (var npc in Owner.MyHouse.Furnitures.Values)
                                        npc.Send(stream);
                                }
                            }
                            else
                            {
                                Qahr.Mobile.Role.Instance.House House;
                                if (Qahr.Mobile.Role.Instance.House.HousePoll.TryGetValue(Owner.Player.DynamicID, out House))
                                {
                                    try
                                    {
                                        foreach (var npc in House.Furnitures.Values)
                                        {
                                            try
                                            {
                                                npc.Send(stream);
                                            }
                                            catch (Exception e)
                                            {
                                                MyConsole.WriteException(e);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MyConsole.WriteException(e);
                                    }
                                }
                            }
                        }
                        if (Owner.Player.Map == 3852)
                        {
                            foreach (var Roullet in Database.Roulettes.RoulettesPoll.Values)
                            {
                                try
                                {
                                    if (Owner.Player.InView(Roullet.SpawnPacket.X, Roullet.SpawnPacket.Y, 16))
                                    {
                                        Owner.Send(stream.RouletteTableCreate(Roullet.SpawnPacket.UID, Roullet.SpawnPacket.TableNumber
                                            , Roullet.SpawnPacket.MoneyType, Roullet.SpawnPacket.X, Roullet.SpawnPacket.Y, Roullet.SpawnPacket.Mesh, Roullet.SpawnPacket.PlayersCount));
                                    }
                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                }

                            }
                        }

                        foreach (var m_client in Owner.Map.View.Roles(MapObjectType.Player, Owner.Player.X, Owner.Player.Y, null))// && (p as Role.Player).View.CanAdd(Owner.Player,clear, stream)))
                        {
                            if (m_client == null)
                                continue;
                            try
                            {

                                if (CanAdd(m_client, clear, stream) && (m_client as Role.Player).View.CanAdd(Owner.Player, true, stream))
                                {

                                    var client = (m_client as Role.Player).Owner;
                                    try
                                    {
                                        if (client.Socket != null)
                                        {
                                            if (client.Socket.Alive == false)
                                            {
                                                Owner.Map.Denquer(client);
                                                ActionQuery action;

                                                action = new ActionQuery()
                                                {
                                                    ObjId = client.Player.UID,
                                                    Type = ActionType.RemoveEntity
                                                };
                                                SendView(stream.ActionCreate(action), false);
                                                continue;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MyConsole.SaveException(e);
                                    }
                                    if (msg != null)
                                        client.Send(msg);

                                    client.Player.SendScrennXPSkill(Owner.Player);
                                    Owner.Player.SendScrennXPSkill(client.Player);



                                    if (Owner.Player.ArrowBladesPower > 0)
                                    Owner.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades, 0, (uint)Owner.Player.ArrowBladesPower, 0, MsgUpdate.DataType.AppendIcon, true);
                                    if (client.Player.ArrowBladesPower > 0)
                                    client.Player.SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.ActiveArrowBlades, 0, (uint)client.Player.ArrowBladesPower, 0, MsgUpdate.DataType.AppendIcon, true);
                                    if (client.Player.PengchengMilesCount > 0)
                                        client.Player.SendUpdate(stream, (MsgUpdate.Flags)client.Player.UID, 8, (uint)client.Player.PengchengMilesCount, (uint)3, (MsgUpdate.DataType)143, true);
                                    if (Owner.Player.PengchengMilesCount > 0)
                                        Owner.Player.SendUpdate(stream, (MsgUpdate.Flags)Owner.Player.UID, 8, (uint)Owner.Player.PengchengMilesCount, (uint)3, (MsgUpdate.DataType)143, true);


                                    client.Player.SendPowerTaoist(Owner, stream);
                                    Owner.Player.SendPowerTaoist(client, stream);

                                    if (client.Player.BlackSpot)
                                    {
                                        Owner.Send(stream.BlackspotCreate(true, client.Player.UID));
                                    }
                                    if (Owner.Player.BlackSpot)
                                    {
                                        client.Send(stream.BlackspotCreate(true, Owner.Player.UID));
                                    }
                                    if (client.Player.OnInteractionEffect)
                                    {
                                        client.Player.InteractionEffect.X = client.Player.X;
                                        client.Player.InteractionEffect.Y = client.Player.Y;


                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);

                                        Owner.Send(stream.InteractionCreate(action));

                                    }
                                    if (Owner.Player.OnInteractionEffect)
                                    {
                                        Owner.Player.InteractionEffect.X = Owner.Player.X;
                                        Owner.Player.InteractionEffect.Y = Owner.Player.Y;


                                        InteractQuery action = InteractQuery.ShallowCopy(Owner.Player.InteractionEffect);
                                        client.Send(stream.InteractionCreate(action));

                                    }
                                    if (Owner.IsVendor)
                                    {
                                        if (Owner.MyVendor.HalkMeesaje != null)
                                            client.Send(Owner.MyVendor.HalkMeesaje.GetArray(stream));
                                    }
                                    if (client.IsVendor)
                                    {
                                        if (client.MyVendor.HalkMeesaje != null)
                                            Owner.Send(client.MyVendor.HalkMeesaje.GetArray(stream));
                                    }
                                    if (client.MyBooth != null)
                                    {
                                        if (client.MyBooth.HawkMessage != null)
                                            Owner.Send(client.MyBooth.HawkMessage.GetArray(stream));

                                    }
                                    if (Owner.MyBooth != null)
                                    {
                                        if (Owner.MyBooth.HawkMessage != null)
                                            client.Send(Owner.MyBooth.HawkMessage.GetArray(stream));
                                    }
                                    if (Owner.Player.OnFairy != false)
                                    {
                                        client.Send(stream.TransformFairyCreate(Owner.Player.FairySpawn.Mode, Owner.Player.FairySpawn.FairyType, Owner.Player.FairySpawn.UID));
                                    }
                                    if (client.Player.OnFairy != false)
                                    {
                                        Owner.Send(stream.TransformFairyCreate(client.Player.FairySpawn.Mode, client.Player.FairySpawn.FairyType, client.Player.FairySpawn.UID));
                                    }


                                }
                            }
                            catch (Exception e)
                            {
                                MyConsole.WriteException(e);
                            }
                        }
                        foreach (var npc in Owner.Map.View.Roles(MapObjectType.Npc, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {

                        }

                        foreach (var mob in Owner.Map.View.Roles(MapObjectType.Monster, Owner.Player.X, Owner.Player.Y
                            , p => CanAdd(p, clear, stream)))
                        {
                            if (mob == null)
                                continue;
                            var Monster = mob as Game.MsgMonster.MonsterRole;
                            if (Monster.HitPoints > ushort.MaxValue || Monster.Boss == 1)
                            {
                                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, Monster.UID, 1);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Monster.Family.MaxHealth);
                                stream = Upd.GetArray(stream);
                                Owner.Send(stream);
                                stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, Monster.HitPoints);
                                stream = Upd.GetArray(stream);
                                Owner.Send(stream);
                            }
                            if (Monster.BlackSpot)
                            {
                                Owner.Send(stream.BlackspotCreate(true, Monster.UID));
                            }
                        }


                        foreach (var item in Owner.Map.View.Roles(MapObjectType.Item, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            var Trap = item as Game.MsgFloorItem.MsgItem;
                            if ((SigilNinja.Contains(Trap.MsgFloor.m_ID) || SigilPirate.Contains(Trap.MsgFloor.m_ID))&& !Owner.Player.HideFloor)
                            {
                                if (Owner.Player.SigilNinja == true)
                                    Owner.Send(item.GetArray(stream, false));
                                if (Owner.Player.SigilPirate == true)
                                    Owner.Send(item.GetArray(stream, false));
                            }
                            else
                            {
                                Owner.Send(item.GetArray(stream, false));
                            }

                        }
                       // foreach (var item in Owner.Map.View.Roles(MapObjectType.PokerTable, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {

                        }


                        foreach (var subnpc in Owner.Map.View.Roles(MapObjectType.SobNpc, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {
                            if (subnpc == null)
                                continue;
                            var SobMobNpcs = subnpc as Role.SobNpc;
                            if (SobMobNpcs.BitVector.ArrayFlags.Count != 0)
                            {
                                Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, subnpc.UID, 1);
                                stream = upd.Append(stream, MsgUpdate.DataType.StatusFlag, SobMobNpcs.BitVector.bits);
                                stream = upd.GetArray(stream);
                                Owner.Send(stream);
                            }

                        }
                        foreach (var staticrole in Owner.Map.View.Roles(MapObjectType.StaticRole, Owner.Player.X, Owner.Player.Y, p => CanAdd(p, clear, stream)))
                        {

                        }
                        //if (Owner.Player.Map == 3053 || Owner.Player.Map == 1860)
                        //{
                        //    foreach (var T in Poker.PokerLoad.Tables.Values)
                        //    {
                        //        if (T.MapId == Owner.Player.Map)
                        //        {
                        //            if (T.OnScreen == null)
                        //            {
                        //                T.OnScreen = new ConcurrentDictionary<uint, uint>();
                        //            }
                        //            if (Owner.Player.InView(T.X, T.Y, 18) && !T.OnScreen.ContainsKey(Owner.Player.UID))
                        //            {
                        //                var Packets = Poker.MsgShowHandTable.CreateShowHandTable(T);
                        //                Owner.Send(Packets);
                        //                T.OnScreen.TryAdd(Owner.Player.UID, Owner.Player.UID);
                        //            }
                        //            else if (T.OnScreen.ContainsKey(Owner.Player.UID))
                        //            {
                        //                uint x = 0;
                        //                T.OnScreen.TryRemove(Owner.Player.UID, out x);
                        //            }
                        //        }
                        //    }
                        //}
                    
                        if (Owner.Player.Map == 11257)
                        {
                            if (Owner.Player.EnergyPoints >= 100)
                            {
                                if (Owner.Map.ContainItem(284) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 284;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, Owner.Player.Map
                                                   , 0, false, Owner.Map, 60 * 60 * 24 * 30);

                                    if (Owner.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }

                            }
                            if (Owner.Player.EnergyPoints >= 150)
                            {
                                if (Owner.Map.ContainItem(285) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 285;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, Owner.Player.Map
                                                   , 0, false, Owner.Map, 60 * 60 * 24 * 30);

                                    if (Owner.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }

                            }
                            if (Owner.Player.EnergyPoints >= 200)
                            {
                                if (Owner.Map.ContainItem(286) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 286;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, Owner.Player.Map
                                                   , 0, false, Owner.Map, 60 * 60 * 24 * 30);

                                    if (Owner.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                            }

                            if (Owner.Player.EnergyPoints >= 300)
                            {
                                if (Owner.Map.ContainItem(287) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 287;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, Owner.Player.Map
                                                   , 0, false, Owner.Map, 60 * 60 * 24 * 30);

                                    if (Owner.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                            }
                            if (Owner.Player.EnergyPoints >= 400)
                            {
                                Owner.Player.SendString(stream, MsgStringPacket.StringID.Effect, 200013, true, "gssswq_xunhuan5");
                                if (Owner.Map.ContainItem(2924) == 0)
                                {
                                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
                                    item.Color = (Role.Flags.Color)2;
                                    item.ITEM_ID = 2924;
                                    Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, 56, 56, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, Owner.Player.Map
                                                   , 0, false, Owner.Map, 60 * 60 * 24 * 30);

                                    if (Owner.Map.EnqueueItem(DropItem))
                                        DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Effect);
                                }
                            }
                        }
                       
                    }

                    catch (Exception e)
                    {
                        MyConsole.WriteException(e);
                    }
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }
        public unsafe void Clear(ServerSockets.Packet stream)
        {
            Owner.Player.Px = 0;
            Owner.Player.Py = 0;

            ActionQuery action;

            action = new ActionQuery()
            {
                ObjId = Owner.Player.UID,
                Type = ActionType.RemoveEntity
            };
            SendView(stream.ActionCreate(action), false);


            foreach (var clone in Owner.Player.MyClones.GetValues())
            {
                clone.RemoveThat(Owner);
            }

            Owner.Player.MyClones.Clear();
            if (Owner.Map != null)
                Owner.Map.Denquer(Owner);
        }


    }
}