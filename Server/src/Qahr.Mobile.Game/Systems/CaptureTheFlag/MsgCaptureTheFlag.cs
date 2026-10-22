using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgCaptureTheFlag
    {
        public const ushort MapID = 2057,
            AliveTournamentMinutes = 59,
            X2CastleMinutes = 15,
            UpScoreBoardSecounds = 6;

        public class Basse
        {
            public Role.SobNpc Npc;
            public System.SafeDictionary<uint, WarScrore> Scores = new System.SafeDictionary<uint, WarScrore>();
            public uint CapturerID = 0;
            public bool IsX2 = false;

            public class WarScrore
            {
                public uint GuildID;
                public string Name;
                public uint Score;
            }

            internal void UpdateScore(Role.Player client, uint Damage)
            {
                if (client.MyGuild == null)
                    return;
                if (!Scores.ContainsKey(client.GuildID))
                {
                    Scores.Add(client.GuildID, new WarScrore()
                    {
                        GuildID = client.MyGuild.Info.GuildID,
                        Name = client.MyGuild.GuildName,
                        Score = Damage
                    });
                }
                else
                {
                    Scores[client.MyGuild.Info.GuildID].Score += Damage;
                }
            }
        }

        public Role.GameMap Map;
        public uint X2Castle = 0;


        public ProcesType Proces;
        public System.SafeDictionary<uint, Basse> Bases;
        public DateTime UpdateStampScore = new DateTime();
        public DateTime SendX2LoctionStamp = new DateTime();
        public DateTime TournamentStamp = new DateTime();

        public MsgCaptureTheFlag()
        {
            Proces = ProcesType.Dead;
            Bases = new System.SafeDictionary<uint, Basse>();
            Program.FreePkMap.Add(MapID);
        }

        public System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild> RegistredGuilds = new System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild>();
        public void Start()
        {
            if (Proces == ProcesType.Dead)
            {
                TournamentStamp = DateTime.Now;

                Bases.Clear();
                CreateBases();
                foreach (var guild in Role.Instance.Guild.GuildPoll.Values)
                {
                    guild.ClaimCtfReward = 0;
                    guild.CTF_Exploits = 0;
                    guild.CTF_Rank = 0;

                    foreach (var user in guild.Members.Values)
                    {
                        user.CTF_Exploits = 0;
                        user.RewardConquerPoints = 0;
                        user.RewardMoney = 0;
                        user.CTF_Claimed = 0;
                    }

                }
                RegistredGuilds = new System.Collections.Concurrent.ConcurrentDictionary<uint, Role.Instance.Guild>();
                GenerateX2Castle();

                Proces = ProcesType.Alive;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var user in Pool.GamePoll.Values)
                        user.Player.MessageBox("", new Action<Client.GameClient>(p =>
                        {
                            p.Teleport(320, 324, 1002);
                        }), null
                        , 60, MsgServer.MsgStaticMessage.Messages.CapturetheFlag);
                }
            }
        }
        public void CheckFinish()
        {
            if (Proces == ProcesType.Alive)// && DateTime.Now > TournamentStamp.AddMinutes(AliveTournamentMinutes))
            {
                Proces = ProcesType.Dead;

                var array = Role.Instance.Guild.GuildPoll.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                var ranks = array.OrderByDescending(p => p.CTF_Exploits).ToArray();
                for (int x = 0; x < Math.Min(9, ranks.Length); x++)
                    ranks[x].CTF_Rank = (byte)(x + 1);

                foreach (var guild in Role.Instance.Guild.GuildPoll.Values)
                {

                    if (RegistredGuilds.ContainsKey(guild.Info.GuildID))
                    {
                        var array_members = guild.Members.Values.Where(p => p.CTF_Exploits != 0).ToArray();
                        var Ranks_members = array_members.OrderByDescending(p => p.CTF_Exploits).ToArray();
                        for (int x = 0; x < Ranks_members.Length; x++)
                        {
                            var rank = CalculateMemberRewardCTF((uint)(x + 1), guild);
                            Ranks_members[x].RewardConquerPoints = rank[0];
                            Ranks_members[x].RewardMoney = rank[1];
                        }
                    }
                    guild.CTF_Next_ConquerPoints = 0;
                    guild.CTF_Next_Money = 0;
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Server.SendGlobalPacket(new MsgMessage("Capture The Flag has Finished.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));

                }

                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        user.Teleport(410, 354, 1002);
                }
            }
        }
        private uint[] CalculateMemberRewardCTF(uint Rank, Role.Instance.Guild guild)
        {
            uint[] rew = new uint[2];
            rew[0] = (guild.CTF_Next_ConquerPoints / (Rank + 1));
            rew[1] = (guild.CTF_Next_Money / (Rank + 1));
            return rew;
        }
        public void GenerateX2Castle()
        {
            int random = Pool.GetRandom.Next(0, Bases.Count);
            var basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = false;
            X2Castle = (uint)random;
            UpdateMapX2Location();
        }
        public void UpdateMapX2Location()
        {
            var basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = true;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    {
                        user.Send(stream);
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Proces == ProcesType.Alive)
            {
                if (!RegistredGuilds.ContainsKey(user.Player.GuildID))
                {
                    RegistredGuilds.TryAdd(user.Player.GuildID, user.Player.MyGuild);
                }


                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0, 0);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);


                var basse = Bases.Values.ToArray()[X2Castle];
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
                global::Qahr.Mobile.BattlePass.BattlePassManager.AddProgress(user, 6);// [BattlePass] task type 6 "Join in Capture the Flag"

                user.Teleport(478, 373, MapID);
                return true;
            }
            return false;
        }
        public void CreateBases()
        {
            Map = Pool.ServerMaps[MapID];

            foreach (var npc in Map.View.GetAllMapRoles(Role.MapObjectType.SobNpc))
                Bases.Add(npc.UID, new Basse() { Npc = npc as Role.SobNpc });

            SpawnFlags();
        }
        public void CheckUpX2()
        {
            if (Proces == ProcesType.Alive)
            {
                if (DateTime.Now > SendX2LoctionStamp.AddMinutes(X2CastleMinutes))
                {
                    GenerateX2Castle();

                    SendX2LoctionStamp = DateTime.Now;
                }
            }
        }

        public void SpawnFlags()
        {
            for (int i = 40 - Map.View.GetAllMapRolesCount(Role.MapObjectType.Npc, x => x.UID == (uint)(x.X * 1000 + x.Y)); i > 0; i--)
            {
                ushort x = 0; ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                if (!InMainCastle(x, y))
                {
                    var npc = Game.MsgNpc.Npc.Create();
                    npc.UID = (uint)(x * 1000 + y);
                    npc.Mesh = 8910;
                    npc.Map = MapID;
                    npc.X = x;
                    npc.Y = y;
                    npc.NpcType = Role.Flags.NpcType.Flag;

                    // Not Map.AddNpc: that marks the 5x5 cells around the NPC as blocked, and the movement checks then
                    // pull back every step next to a flag, so no flag could ever be picked up.
                    if (!Map.View.MapContain(Role.MapObjectType.Npc, npc.UID))
                    {
                        Map.View.EnterMap<Role.IMapObj>(npc);
                        using (var rec = new ServerSockets.RecycledPacket())
                            npc.Send(rec.GetStream());
                    }
                }
            }
        }
        public void UpdateMapScore()
        {
            if (Proces == ProcesType.Alive)
            {
                if (DateTime.Now > UpdateStampScore.AddSeconds(UpScoreBoardSecounds))
                {
                    SendUpdateBoardScore();
                    UpdateStampScore = DateTime.Now;
                }
            }
        }
        public void SendUpdateBoardScore()
        {

            var array = RegistredGuilds.Values.Where(p => p.CTF_Exploits != 0).ToArray();
            var rank = array.OrderByDescending(p => p.CTF_Exploits).ToArray();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0, 0);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        user.Send(stream);
                }



                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreUpdate, 2, (uint)Math.Min(rank.Length, 3));

                for (uint x = 0; x < rank.Length; x++)
                {
                    if (x == 3)
                        break;
                    var element = rank[x];

                    stream.AddItemCaptureTheFlagUpdate(x, element.CTF_Exploits, element.GuildName, 0);
                }

                stream.CaptureTheFlagUpdateFinalize();
                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        user.Send(stream);
                }

                //send base score.
                foreach (var user in Pool.GamePoll.Values)
                {
                    if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    {
                        Basse flag_base;
                        if (TryGetBase(user, out flag_base))
                        {
                            var array_scorebasse = flag_base.Scores.Values.OrderByDescending(p => p.Score).ToArray();
                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreBase, 0, (uint)Math.Min(array_scorebasse.Length, 3));
                            for (uint x = 0; x < array_scorebasse.Length; x++)
                            {
                                if (x == 3)
                                    break;
                                var element = array_scorebasse[x];
                                stream.AddItemCaptureTheFlagUpdate(x, element.Score, element.Name, 0);
                            }
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);
                        }
                    }
                }
            }

        }
        public bool TryGetBase(Client.GameClient user, out Basse bas)
        {
            if (user.Player.Map == MapID && user.Player.DynamicID == 0)
            {
                foreach (var flag_base in Bases.Values)
                {
                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, flag_base.Npc.X, flag_base.Npc.Y) <= 11)
                    {
                        bas = flag_base;
                        return true;
                    }
                }
            }
            bas = null;
            return false;
        }
        public void UpdateFlagScore(Role.Player client, Role.SobNpc Attacked, uint Damage, ServerSockets.Packet stream)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (client.MyGuild == null)
                return;
            Basse Bas;
            if (Bases.TryGetValue(Attacked.UID, out Bas))
            {
                Bas.UpdateScore(client, Damage);

                if (Bas.Npc.HitPoints == 0)
                {

                    var array = Bas.Scores.Values.OrderByDescending(p => p.Score).ToArray();
                    var GuildWinner = array.First();

                    Bas.CapturerID = GuildWinner.GuildID;

                    Bas.Scores.Clear();
                    Bas.Npc.HitPoints = Bas.Npc.MaxHitPoints;
                    Bas.Npc.Name = GuildWinner.Name;


                    foreach (var user in Pool.GamePoll.Values)
                    {
                        if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                        {
                            if (Role.Core.GetDistance(user.Player.X, user.Player.Y, Bas.Npc.X, Bas.Npc.Y) <= 9)
                            {
                                MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Bas.Npc.UID, 2);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Bas.Npc.Mesh);
                                stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Bas.Npc.HitPoints);
                                stream = upd.GetArray(stream);
                                user.Send(stream);
                                user.Send(Bas.Npc.GetArray(stream, false));
                            }
                        }
                    }
                    stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.OccupiedBase, (byte)(Bas.Npc.UID % 10), 0);
                    stream.CaptureTheFlagUpdateFinalize();
                    SendMapPacket(stream);
                }
            }
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in Pool.GamePoll.Values)
            {
                if (user.Player.Map == MapID && user.Player.DynamicID == 0)
                    user.Send(stream);
            }
        }
        public bool Attackable(Role.Player user)
        {
            return !InMainCastle(user.X, user.Y);
        }
        public bool InMainCastle(ushort X, ushort Y)
        {
            return Role.Core.GetDistance(X, Y, 482, 367) < 32;
        }
        public void PlantTheFlag(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (user.Player.Map == MapID)
            {
                if (user.Player.MyGuild == null)
                    return;

                if (user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag))
                {
                    Basse flag_base;
                    if (TryGetBase(user, out flag_base))
                    {
                        if (flag_base.CapturerID == user.Player.GuildID)
                        {
                            user.Player.RemoveFlag(MsgUpdate.Flags.CTF_Flag);

                            uint exploits = (uint)(user.Player.Level / 2);

                            if (flag_base.IsX2)
                                exploits *= 2;

                            user.Player.MyGuild.CTF_Exploits += exploits;
                            user.Player.MyGuildMember.CTF_Exploits += exploits;

                            stream.CaptureTheFlagRankingsCreate(MsgCaptureTheFlagRankings.ActionID.UpdatePoint, 1, (uint)0, (uint)0, user.Player.MyGuild != null ? user.Player.MyGuildMember.CTF_Exploits : 0, 0);
                            stream.CaptureTheFlagRankingsFinalize();
                            user.Send(stream);

                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 0, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);

                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);

                            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.RemoveFlagEffect, user.Player.UID);
                            stream.CaptureTheFlagUpdateFinalize();
                            user.Send(stream);
                        }
                    }
                }
            }
        }
        public void Location()
        {
            foreach (var user in Pool.GamePoll.Values)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    if (user != null && Bases != null)
                    {
                        var stream = rec.GetStream();
                        var Array = Bases.Values.Where(p => p.Npc.Name == user.Player.MyGuild.GuildName).ToArray();
                        stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.Location, user.Player.UID, (uint)Array.Length);
                        for (int x = 0; x < Array.Length; x++)
                        {
                            var element = Array[x];
                            stream.AddX2LocationCaptureTheFlagUpdate(element.Npc.X, element.Npc.Y);// esto es lo que te falla jajaj
                        }
                        user.Send(stream.CaptureTheFlagUpdateFinalize());
                    }
                }
            }
        }
        public void ChechMoveFlag(Client.GameClient user)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (user.Player.Map == MapID)
            {
                if (user.Player.MyGuild == null)
                    return;
                if (!user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag))
                {
                    foreach (var flag in user.Map.View.Roles(Role.MapObjectType.Npc, user.Player.X, user.Player.Y))
                    {
                        // only the spawned flags (UID = X*1000+Y), never the castle/exit NPCs
                        if (flag.UID == (uint)(flag.X * 1000 + flag.Y)
                            && Role.Core.GetDistance(user.Player.X, user.Player.Y, flag.X, flag.Y) < 2)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();


                                user.Player.MyGuild.CTF_Exploits += 3;
                                user.Player.MyGuildMember.CTF_Exploits += 3;

                                stream.CaptureTheFlagRankingsCreate(MsgCaptureTheFlagRankings.ActionID.UpdatePoint, 1, (uint)0, (uint)0, user.Player.MyGuild != null ? user.Player.MyGuildMember.CTF_Exploits : 0, 0);
                                stream.CaptureTheFlagRankingsFinalize();
                                user.Send(stream);

                                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.CTF_Flag, 60, true);

                                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 60, user.Player.UID);
                                stream.CaptureTheFlagUpdateFinalize();
                                user.Send(stream);



                                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                                stream.CaptureTheFlagUpdateFinalize();
                                user.Send(stream);


                                user.Map.View.LeaveMap<Role.IMapObj>(flag);

                                ActionQuery action;

                                action = new ActionQuery()
                                {
                                    ObjId = flag.UID,
                                    Type = ActionType.RemoveEntity
                                };
                                unsafe
                                {
                                    user.Player.View.SendView(stream.ActionCreate(action), true);
                                }

                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
