using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
    public class Nobility
    {
        public class NobilityRanking
        {

            public ulong KnightDonation = 30000, BaronDonation = 10000, EarlDonation = 20000;
            public ulong DukeDonation;
            public ulong PrinceDonation;
            public ulong KingDonation;

            private Dictionary<uint, Nobility> ClientPoll;

            public NobilityRanking()
            {
                ClientPoll = new Dictionary<uint, Nobility>();
            }
            public bool TryGetValue(uint UID, out Nobility user)
            {
                return ClientPoll.TryGetValue(UID, out user);
            }
            public Nobility[] GetArray()
            {
                return ClientPoll.Values.ToArray();
            }
            public void RemoveAndUpdateTheRest(uint UID)
            {
                if (this.ClientPoll.ContainsKey(UID))
                {
                    this.ClientPoll.Remove(UID);
                }
                foreach (Nobility Nobil in GetArray())
                {
                    UpdateRank(Nobil);
                }
            }
            public void UpdateRank(Nobility user)
            {
                if (ClientPoll.Count < 50)
                    CreateRank(user);
                else
                {
                    var array = ClientPoll.Values.ToArray();
                    if (array[49].Donation < user.Donation)
                        CreateRank(user);
                    else if (ClientPoll.ContainsKey(user.UID))
                        CreateRank(user);
                }
            }
            internal void CreateRank(Nobility user)
            {
                lock (ClientPoll)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!ClientPoll.ContainsKey(user.UID))
                            ClientPoll.Add(user.UID, user);
                        var Array = ClientPoll.Values.Where(p => p.Donation > EarlDonation).ToArray();
                        var Order = Array.OrderByDescending(p => p.Donation).ToArray();
                        ClientPoll.Clear();

                        for (int x = 0; x < Order.Length; x++)
                        {
                            var nobilityclient = Order[x];
                            int OldPosition = nobilityclient.Position;
                            var OldRank = nobilityclient.Rank;
                            if (x < 50)
                            {
                                nobilityclient.Position = x;
                                ClientPoll.Add(nobilityclient.UID, nobilityclient);
                                if (x == 2)
                                    KingDonation = nobilityclient.Donation;
                                else if (x == 14)
                                    PrinceDonation = nobilityclient.Donation;
                                else if (x == 49)
                                    DukeDonation = nobilityclient.Donation;
                            }
                            else
                                nobilityclient.Position = -1;

                            if (OldPosition != nobilityclient.Position)
                            {

                                UpdateClientIcon(stream, nobilityclient);
                                if (Pool.GamePoll.ContainsKey(nobilityclient.UID))
                                    Pool.GamePoll[nobilityclient.UID].Equipment.QueryEquipment(Pool.GamePoll[nobilityclient.UID].Equipment.Alternante);
                                if (nobilityclient.Rank > OldRank)
                                {
                                    ushort Genderx = 0;
                                    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                                    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                                    {
                                        ini.FileName = fname;

                                        ushort Body = ini.ReadUInt16("Character", "Body", 1002);
                                        uint UID = ini.ReadUInt32("Character", "UID", 0);

                                        if (UID == nobilityclient.UID)
                                        {

                                            Genderx = Body;
                                        }

                                    }

                                    #region New Update In Donations Edit By Mohy ElDin ...

                                    string nobcall = "";
                                    if (Genderx == 1008)
                                    { nobcall = nobilityclient.Rank.ToString(); }
                                    else
                                    {
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.King)
                                            nobcall = "Queen"; //Girl
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.Prince)
                                            nobcall = "Princess"; //Girl
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.Duke)
                                            nobcall = "Duchess"; //Girl
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.Knight)
                                            nobcall = "Lady";   //Girl
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.Baron)
                                            nobcall = "Baroness";  //Girl
                                        if (nobilityclient.Rank == Role.Instance.Nobility.NobilityRank.Earl)
                                            nobcall = "Countess"; //Girl
                                    }

                                    #endregion
#if Arabic
                                           Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(nobilityclient.Name + " has been promoted to " + nobilityclient.Rank.ToString() + ".", "ALL", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                      
#else
                                    Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(nobilityclient.Name + " has been promoted to " + nobcall + ".", "ALL", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#endif
                                }
                                //Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulation! " + nobilityclient.Name + " donated to " + nobilityclient.Rank.ToString() + " in Nobility Rank.", "ALL", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                        }
                    }

                }
            }
            internal unsafe void UpdateClientIcon(ServerSockets.Packet stream, Nobility user)
            {
                Client.GameClient client;
                if (Pool.GamePoll.TryGetValue(user.UID, out client))
                {
                    client.Player.NobilityRank = user.Rank;
                    client.Send(stream.NobilityIconCreate(user));
                }
            }
        }
        public enum NobilityRank : byte
        {
            Serf = 0,
            Knight = 1,
            Baron = 3,
            Earl = 5,
            Duke = 7,
            Prince = 9,
            King = 12
        }


        public uint UID;
        public string Name;
        public int Position = -1;
        public ulong Donation;
        public uint Mesh;
        public byte Gender;
        public NobilityRank Rank
        {
            get
            {
#if Encore
                   if (Position >= 0 && Position < 3)
                    return NobilityRank.King;
                   else if (Position >= 3 && Position < 15)
                       return NobilityRank.Prince;
#else
                if (Position >= 0 && Position < 3)
                    return NobilityRank.King;
                else if (Position >= 3 && Position < 15)
                    return NobilityRank.Prince;
#endif
                else if (Position >= 15 && Position < 50)
                    return NobilityRank.Duke;

                if (Donation >= 20000)
                    return NobilityRank.Earl;
                else if (Donation >= 10000)
                    return NobilityRank.Baron;
                else if (Donation >= 30000)
                    return NobilityRank.Knight;

                return NobilityRank.Serf;
            }
        }
        public Nobility(uint _uid, string _name, ulong _donation, uint _mesh, byte _gender)
        {
            UID = _uid;
            Name = _name;
            Donation = _donation;
            Mesh = _mesh;
            Gender = _gender;
        }
        public Nobility(Client.GameClient user)
        {
            UID = user.Player.UID;
            Name = user.Player.Name;
            Mesh = user.Player.Mesh;
            Gender = user.Player.GetGender;
        }
    }
}
