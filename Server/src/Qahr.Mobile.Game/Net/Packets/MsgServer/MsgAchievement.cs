using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class Achievement
    {

        public static unsafe void GetAchievement(this ServerSockets.Packet stream, out ClientAchievement.TypeAchievement action, out uint UID, out uint Flag)
        {
            action = (ClientAchievement.TypeAchievement)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            stream.ReadUInt32();
            Flag = stream.ReadUInt32();
            
        }

        public static unsafe ServerSockets.Packet AchievementCreate(this ServerSockets.Packet stream, ClientAchievement.TypeAchievement action, uint UID, uint[] Flags)
        {
            stream.InitWriter();

            
            stream.Write((uint)action);
            stream.Write(UID);
            stream.Write(0);
            stream.Write(Flags.Length);

            for (int x = 0; x < Flags.Length; x++)
            {
                stream.Write(Flags[x]);
            }

            stream.Finalize(GamePackets.MsgAchievement);
            return stream;
        }
        public static unsafe ServerSockets.Packet AchievementCreate(this ServerSockets.Packet stream, ClientAchievement.TypeAchievement action, uint UID, uint Flag)
        {
            stream.InitWriter();
            stream.Write((uint)action);
            stream.Write(UID);
            stream.Write(0);
            stream.Write(Flag);

            stream.Write((uint)0);

            stream.Finalize(GamePackets.MsgAchievement);
            return stream;
        }
    }
    public class ClientAchievement
    {
        public const int File_Size = 16;

        public enum TypeAchievement : uint
        {
            BigHash = 0,
            CheakView = 1,
            SendQueue = 2
        }



        [PacketAttribute(GamePackets.MsgAchievement)]
        public static unsafe void AchievementHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
            TypeAchievement action;
            uint UID;
            uint Flag;

            stream.GetAchievement(out action, out UID, out Flag);


            switch (action)
            {
                case TypeAchievement.CheakView:
                    {
                        Role.IMapObj obj = null;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player target = obj as Role.Player;
                            if (target != null)
                            {

                                client.Send(stream.AchievementCreate(TypeAchievement.CheakView, target.UID, target.Achievement.MyFlags()));
                            }

                        }
                        break;
                    }
                case TypeAchievement.SendQueue:
                    {

                      //  client.Player.Achievement.CreateFlaID((int)Flag, client, stream);
                        break;
                    }
            }
        }


        private System.BitVector32 BitVector32;

        public int Score()
        {
            return BitVector32.Count();
        }
        public ClientAchievement(uint[] Db_value, uint UID)
        {
            BitVector32 = new System.BitVector32(32 * File_Size);

            for (int x = 0; x < Db_value.Length; x++)
                BitVector32.bits[x] = Db_value[x];
        }
        public unsafe void CreateFlaID(int id, Client.GameClient client, ServerSockets.Packet stream)
        {
            AddFlag((int)((id / 100) % 100 - 1) * 32 + (int)(id % 100 - 1), client, stream);
        }

        public unsafe void AddFlag(int flag, Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!BitVector32.Contain(flag))
            {
                BitVector32.Add(flag);

                ShowScreen(flag, client, stream);
                Send(client, stream);
            }
        }
        public unsafe void Send(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(stream.AchievementCreate(TypeAchievement.BigHash, client.Player.UID, client.Player.Achievement.MyFlags()));
        }
        private unsafe void ShowScreen(int flag, Client.GameClient client, ServerSockets.Packet stream)
        {
            uint FLAG_ID = (uint)(10100 + (uint)(100 * (byte)(flag / 32)) + (byte)(flag % 32) + 1);

            client.Send(stream.AchievementCreate(TypeAchievement.SendQueue, client.Player.UID, FLAG_ID));
#if Arabic
            string Mesajje = "" + client.Player.Name + " received [Achievement " + FLAG_ID + "]! ";
#else
            string Mesajje = "" + client.Player.Name + " received [Achievement " + FLAG_ID + "]! ";
#endif
            if (client.Player.MyGuild != null)
                client.Player.MyGuild.SendMessajGuild(Mesajje);
            else
            {
                Game.MsgServer.MsgMessage mesaj = new Game.MsgServer.MsgMessage(Mesajje, client.Player.Name, MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System);
                client.Send(mesaj.GetArray(stream));
            }
        }


        public uint[] MyFlags()
        {
            uint[] Flags = new uint[File_Size];
            for (byte x = 0; x < BitVector32.bits.Length; x++)
                Flags[x] = BitVector32.bits[x];

            return Flags;
        }
        public void Save(Database.AchievementCollection achiev)
        {
            for (int x = 0; x < File_Size; x++)
                achiev.Value[x] = this.BitVector32.bits[x];
        }
    }
}


