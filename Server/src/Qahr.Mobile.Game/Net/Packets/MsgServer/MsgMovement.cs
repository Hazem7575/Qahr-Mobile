using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Qahr.Mobile.Game.MsgFloorItem;
using System.IO;
using Qahr.Mobile.ServerSockets;
using ProtoBuf;
using Qahr.Mobile.Game.MsgMonster;
using Qahr.Mobile.Role;
namespace Qahr.Mobile.Game.MsgServer
{
    [ProtoContract]
    public class WalkQuery
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Direction;
        [ProtoMember(2, IsRequired = true)]
        public uint UID;
        [ProtoMember(3, IsRequired = true)]
        public uint Running;
        [ProtoMember(4, IsRequired = true)]
        public uint TimeStamp;
        [ProtoMember(5)]
        public uint MapID;
    }
    public static unsafe class MsgMovement
    {
        public const uint Walk = 0, Run = 1, Steed = 9;


        public static sbyte[] DeltaMountX = new sbyte[24] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] DeltaMountY = new sbyte[24] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };

        
        public static unsafe void GetWalk(this ServerSockets.Packet stream, out WalkQuery pQuery)
        {
          pQuery = new WalkQuery();
          pQuery=  stream.ProtoBufferDeserialize<WalkQuery>(pQuery);
        }

        public static unsafe ServerSockets.Packet MovementCreate(this ServerSockets.Packet stream, WalkQuery pQuery)
        {
            stream.InitWriter();
            pQuery.TimeStamp = (uint)DateTime.Now.Value();
            stream.ProtoBufferSerialize(pQuery);

            stream.Finalize(GamePackets.MsgWalk);

            return stream;
        }

        public static uint Bodyyyy = 0;
        public static uint UIDDDD = 1000000;
        public static int eeffect = 1;
        public static int LastClientStamp = 0;
        [PacketAttribute(GamePackets.MsgWalk)]
        public unsafe static void Movement(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.IsVendorr)
                return;
            if (client.Player.Away == 1)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                }
            }
            Role.Flags.ConquerAngle dir;

            WalkQuery walkPacket;

            packet.GetWalk(out walkPacket);
            walkPacket.UID = client.Player.UID;

            if (client.Player.ReviveProtection)
            {
                client.Player.ReviveProtection = false;
                client.Player.ProtectAttack(0);
            }
            
            if (client.Player.ContainFlag(MsgUpdate.Flags.WeepStorm))
                client.Player.RemoveFlag(MsgUpdate.Flags.WeepStorm);
            if (client.Player.ContainFlag((MsgUpdate.Flags)491))
                client.Player.RemoveFlag((MsgUpdate.Flags)491);
            // [Audit H4] Walk speed-hack rate check now applies to ALL movement
            // (previously only ran while the Fly/HeavensWrath flag was set, leaving
            // normal ground movement unchecked). Thresholds are deliberately left
            // unchanged to avoid false-positives for legit/laggy players.
            if (client.Player.LastWalk > 0 && walkPacket.Running != MsgMovement.Steed && client.Player.TransformationID != 207 && client.Player.TransformationID != 267 && client.Player.OnXPSkill() == Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                int a1 = (Time32.Now - client.Player.lastWalkTime).AllMilliseconds();
                uint a2 = walkPacket.TimeStamp - client.Player.LastWalk;
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                {
                    if (Time32.Now < client.Player.lastWalkTime.AddMilliseconds(120) && a2 - a1 <= 1000)
                    {
                        client.Player.CountSpeedHack++;
                    }
                    else if (client.Player.CountSpeedHack > 0) client.Player.CountSpeedHack--;
                }
                else
                {
                    if (Time32.Now < client.Player.lastWalkTime.AddMilliseconds(140) && a2 - a1 <= 1000)
                    {
                        client.Player.CountSpeedHack++;
                    }
                    else if (client.Player.CountSpeedHack > 0) client.Player.CountSpeedHack--;
                }
            }
            else if (client.Player.CountSpeedHack > 0) client.Player.CountSpeedHack--;
            if (client.Player.CountSpeedHack >= 8)
            {
                // [Audit H4] snap the player back to the server-known position on
                // repeated violation instead of silently dropping a single packet.
                client.Pullback();
                return;
            }
            client.Player.lastWalkTime = Time32.Now;
            client.Player.LastWalk = walkPacket.TimeStamp;


            client.Player.Action = Role.Flags.ConquerAction.None;

            client.OnAutoAttack = false;
            client.Player.RemoveBuffersMovements(packet);

            client.Player.Protect = DateTime.Now;

            if (client.Player.InUseIntensify)
            {
                client.Player.InUseIntensify = false;
                using (var recycledPacket = new ServerSockets.RecycledPacket())
                {
                    var streamm = recycledPacket.GetStream();
                    client.Send(streamm.ActionCreate(new ActionQuery() { ObjId = client.Player.UID, TargetUID = client.Player.FocusClientSpell.ID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), Type = 103 }));
                }
            }
            client.Player.RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Focused);




            if (walkPacket.Running == MsgMovement.Steed)
            {

                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 24);

                int newX = client.Player.X + DeltaMountX[(byte)dir];
                int newY = client.Player.Y + DeltaMountY[(byte)dir];
#if TEST
                MyConsole.WriteLine("Steed walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + newX + " Y " + newY + "");
#endif

                if (client.Map == null)
                {
                    client.Teleport(357, 303, 1002);
                    return;
                }

                // [Audit H2] a two-tile steed step is only legal while actually riding.
                if (!client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                {
                    client.Pullback();
                    return;
                }

                // [Audit H2] validate the TARGET tile (bounds + walkable) from the server position.
                if (newX < 0 || newY < 0
                    || (client.Map.bounds.Width > 0 && !client.Map.ValidLocation((ushort)newX, (ushort)newY)))
                {
                    client.Pullback();
                    return;
                }

                if (client.Player.Map == 1038)
                {
                    // [Audit H2] test the NEW tile, not the old position, so mounted players
                    // cannot walk through the Guild War gates.
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, (ushort)newX, (ushort)newY))
                    {

                        client.SendSysMesage("Illegal jumping over the gates detected.");


                        client.Pullback();
                        return;
                    }
                }

                // [Audit H3] server-side step-rate limit (lenient, counter based).
                MsgLoader.SpeedProtect.MovementProdect(client);

                client.Player.View.SendView(packet.MovementCreate(walkPacket), true);

                client.Map.View.MoveTo<Role.IMapObj>(client.Player, newX, newY);
                client.Player.X = (ushort)newX;
                client.Player.Y = (ushort)newY;

                client.Player.CanUseSectorSkill = true;

                client.Player.Action = Role.Flags.ConquerAction.None;
                client.Player.View.Role(false, packet.MovementCreate(walkPacket));

                if (client.Vigor >= 2)
                    client.Vigor -= 2;
                else
                    client.Vigor = 0;

                client.Send(packet.ServerInfoCreate(client.Vigor));


            }
            else
            {
                ushort walkX = client.Player.X, walkY = client.Player.Y;
                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 8);
                Role.Core.IncXY(dir, ref walkX, ref walkY);

#if TEST
                MyConsole.WriteLine("walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + walkX + " Y " + walkY + "");
#endif
                if (client.Map == null)
                {
                    client.Teleport(357, 303, 1002);
                    return;
                }

                // [Audit H2] validate the TARGET tile (bounds + walkable) so players cannot
                // walk through walls / into blocked tiles. Guarded with bounds>0 so a map
                // with no loaded cell data never locks players in place (fail-open).
                if (client.Map.bounds.Width > 0 && !client.Map.ValidLocation(walkX, walkY))
                {
                    client.Pullback();
                    return;
                }

                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, walkX, walkY))
                    {
                        client.SendSysMesage("Illegal jumping over the gates detected.");
                        client.Pullback();
                        return;
                    }

                }

                // [Audit H3] server-side step-rate limit (lenient, counter based).
                MsgLoader.SpeedProtect.MovementProdect(client);


                if (client.Player.ObjInteraction != null)
                {
                    if (client.Player.ObjInteraction.Player.X == client.Player.X && client.Player.ObjInteraction.Player.Y == client.Player.Y)
                    {

                        InterActionWalk query = new InterActionWalk()
                        {
                            Mode = MsgInterAction.Action.Walk,
                            UID = client.Player.UID,
                            OponentUID = client.Player.ObjInteraction.Player.UID,
                            DirectionOne = (byte)dir
                        };

                        client.Player.View.SendView(packet.InterActionWalk(&query), true);
                        client.Player.Action = Role.Flags.ConquerAction.InteractionHold;
                        client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                        client.Player.X = walkX;
                        client.Player.Y = walkY;
                        client.Player.Angle = dir;

                        client.Player.View.Role(false, packet.InterActionWalk(&query));

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, walkX, walkY);
                        client.Player.ObjInteraction.Player.X = walkX;
                        client.Player.ObjInteraction.Player.Y = walkY;
                        client.Player.ObjInteraction.Player.Angle = dir;

                        client.Player.ObjInteraction.Player.View.Role();
                        return;
                    }
                }
                client.Player.View.SendView(packet.MovementCreate(walkPacket), true);
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                client.Player.X = walkX;
                client.Player.Y = walkY;
                client.Player.Angle = dir;

                client.Player.CanUseSectorSkill = true;

                client.Player.View.Role(false, packet.MovementCreate(walkPacket));
            }
            if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);
            //if (Game.MsgTournaments.MsgSchedules.GuildWar != null)
            //    Game.MsgTournaments.MsgSchedules.GuildWar.ChechMove(client);
            if (MsgTournaments.MsgSchedules.SteedRace.IsOn)
            {
                if (MsgTournaments.MsgSteedRace.MAPID == client.Player.Map)
                    MsgTournaments.MsgSchedules.SteedRace.CheckForRaceItems(client);
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(packet);

            #region ShadowClones
            if (client.Player.MyClones.Count > 0)
            {
                foreach (var clone in client.Player.MyClones.GetValues())
                {
                    clone.Owner.Player.X = client.Player.X;
                    clone.Owner.Player.Y = client.Player.Y;
                    clone.Owner.Player.Angle = client.Player.Angle;
                }
            }
            #endregion
            if (client.Player.ListPets.Count > 0&& client.Player.ListPets!= null)
            {
                foreach (var listPet in client.Player.ListPets)
                {
                    if (!listPet.Alive)
                        NewPetGuild.Death(client, packet);
                    listPet.GMap.View.MoveTo<IMapObj>((IMapObj)listPet, (int)client.Player.X, (int)client.Player.Y);
                    listPet.X = client.Player.X;
                    listPet.Y = client.Player.Y;
                }
            }
            if (client.Player.Map == 10428 && Role.Core.GetDistance(client.Player.X, client.Player.Y, 61, 66) <= 2)
            {
                client.Player.MessageBox("Are you sure you want to leave the alien world and return to Twin City?", p => { p.Teleport(351, 422, 1002); p.SendSysMesage("With a flash of light, you were teleported back to Twin City."); });
            }
            #region Stage1
            if (client.Player.Map == 11223)
            {
                #region Portal1
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 138, 164) <= 4)
                {
                    if (client.Player.Portal == 1)
                    {
                        client.Player.MessageBox("STR_ID_tArchersTask[Msg][Jump][1]@@", new Action<Client.GameClient>(p =>
                        {
                            client.Teleport(66, 141, 11223, client.Player.UID);
                        }), null);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var map = Pool.ServerMaps[11223];
                            if (!map.ContainMobID(4226, client.Player.UID))
                            {
                                Server.AddMapMonster(stream, map, 4226, 63, 140, 18, 18, 1, client.Player.UID, true);
                            }

                        }
                    }

                }
                #endregion
                #region Portal2
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 161, 191) <= 4)
                {
                    if (client.Player.Portal == 2)
                    {
                        client.Player.MessageBox("STR_ID_tArchersTask[Msg][Jump][2]@@", new Action<Client.GameClient>(p =>
                        {
                            client.Teleport(157, 244, 11223, client.Player.UID);
                        }), null);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var map = Pool.ServerMaps[11223];
                            if (!map.ContainMobID(4227, client.Player.UID))
                            {
                                Server.AddMapMonster(stream, map, 4227, 156, 243, 18, 18, 1, client.Player.UID, true);
                            }

                        }
                    }
                }
                #endregion
                #region Portal3
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 188, 153) <= 4)
                {
                    if (client.Player.Portal == 3)
                    {
                        client.Player.MessageBox("STR_ID_tArchersTask[Msg][Jump][3]@@", new Action<Client.GameClient>(p =>
                        {
                            client.Teleport(254, 147, 11223, client.Player.UID);
                        }), null);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var map = Pool.ServerMaps[11223];
                            if (!map.ContainMobID(4228, client.Player.UID))
                            {
                                Server.AddMapMonster(stream, map, 4228, 254, 147, 18, 18, 1, client.Player.UID, true);
                            }

                        }
                    }
                }
                #endregion
                #region Portal4
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, 145, 132) <= 4)
                {
                    if (client.Player.Portal == 4)
                    {
                        client.Player.MessageBox("STR_ID_tArchersTask[Msg][Jump][4]@@", new Action<Client.GameClient>(p =>
                        {
                            client.Teleport(96, 81, 11223, client.Player.UID);
                        }), null);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            var map = Pool.ServerMaps[11223];
                            if (!map.ContainMobID(4229, client.Player.UID))
                            {
                                Server.AddMapMonster(stream, map, 4229, 96, 81, 18, 18, 1, client.Player.UID, true);
                            }

                        }
                    }
                }
                #endregion

            }
            #endregion

        }
    }
}