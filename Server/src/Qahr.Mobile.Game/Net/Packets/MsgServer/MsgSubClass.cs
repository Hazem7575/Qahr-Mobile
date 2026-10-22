using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetSubClass(this ServerSockets.Packet stream, out MsgSubClass.Action mode, out Database.DBLevExp.Sort clas)
        {
            //uint timer = stream.ReadUInt32();
            mode = (MsgSubClass.Action)stream.ReadUInt16();
            clas = (Database.DBLevExp.Sort)stream.ReadInt8();
        }

        public static unsafe ServerSockets.Packet SubClassCreate(this ServerSockets.Packet stream, MsgSubClass.Action mode, Database.DBLevExp.Sort clas, byte Level, ushort StudyReceive)
        {
            stream.InitWriter();

            //stream.Write(DateTime.Now.Value());

            stream.Write((ushort)mode);//4
            stream.Write((byte)clas);//8
            stream.Write(Level);

            stream.ZeroFill(6);//unknow
            stream.Write(StudyReceive);

            stream.ZeroFill(10);

            stream.Finalize(GamePackets.MsgSubPro);
            return stream;
        }

        public static unsafe void GetSubClass(this ServerSockets.Packet stream, out MsgSubClass.Action mode, out MsgSubClass.SubClases[] list)
        {
            // uint timer = stream.ReadUInt32();
            mode = (MsgSubClass.Action)stream.ReadUInt16();
            stream.SeekForward(16);
            int size = stream.ReadInt32();
            list = new MsgSubClass.SubClases[size];
            for (int x = 0; x < size; x++)
            {
                list[x] = new MsgSubClass.SubClases();
                list[x].ID = (Database.DBLevExp.Sort)stream.ReadUInt8();
                list[x].Phrase = stream.ReadUInt8();
                list[x].Level = stream.ReadUInt8();

            }
        }
        public static unsafe ServerSockets.Packet SubClassCreate(this ServerSockets.Packet stream, MsgSubClass.Action mode, ushort study, ushort StudyReceive, MsgSubClass.SubClases[] list = null)
        {
            stream.InitWriter();

            // stream.Write(DateTime.Now.Value());

            stream.Write((ushort)mode);
            stream.Write(study);

            stream.ZeroFill(6);//unknow
            stream.Write(StudyReceive);

            stream.ZeroFill(6);
            if (list != null)
            {
                stream.Write(list.Length);

                for (int x = 0; x < list.Length; x++)
                {
                    stream.Write((byte)list[x].ID);
                    stream.Write(list[x].Phrase);
                    stream.Write(list[x].Level);
                }
            }
            else
            {
                stream.ZeroFill(4);
            }


            stream.Finalize(GamePackets.MsgSubPro);
            return stream;
        }
    }

    public unsafe struct MsgSubClass
    {
        public static uint[] ItemsPromote = new uint[] { 0, 721259, 721261, 711188, 723087, 1088001, 711679, 0, 0, 723903 };
        public static byte[] ItemsCount = new byte[] { 0, 5, 10, 1, 20, 10, 1, 0, 0, 40 };

        public class SubClases
        {
            public Database.DBLevExp.Sort ID;
            public byte Phrase;
            public byte Level;

            public static SubClases Create(Database.DBLevExp.Sort _id)
            {
                SubClases item = new SubClases();
                item.ID = _id;
                item.Level = 1;
                item.Phrase = 1;
                return item;
            }
            public static SubClases CreateFixed(Database.DBLevExp.Sort _id)
            {
                SubClases item = new SubClases();
                item.ID = _id;
                item.Level = 9;
                item.Phrase = 9;
                return item;
            }
        }
        public enum Action : ushort
        {
            SwitchSubClass = 0,
            ActivateSubClass = 1,
            Upgrade = 2,
            SendUpdate = 3,
            LearnSubClass = 4,
            MartialPromoted = 5,
            Open = 6,
            ShowGUI = 7,
            Animation = 8,
            Join = 9,
            Pro = 10
        }

        [PacketAttribute(GamePackets.MsgSubPro)]
        private static void SubClass(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgSubClass.Action Mode;
            Database.DBLevExp.Sort Class;

            stream.GetSubClass(out Mode, out Class);


            switch (Mode)
            {
                case Action.Open:
                    {
                        user.Player.SubClass.ShowGui(user, stream);
                        break;
                    }
                case Action.Pro:
                    {
                        SubClases item;
                        if (user.Player.SubClass.src.TryGetValue(Class, out item))
                        {
                            if (item.Phrase >= 9)
                                break;
                            if (item.Phrase < item.Level)
                            {
                                if (Pool.SubClassInfo.AllowUpdate(user, Class, (byte)(item.Phrase + 1)))
                                {
                                    item.Phrase++;
                                    user.Player.SubClass.src[Class] = item;

                                    user.Player.SubClass.SendPromoted(user, item, stream);
                                    user.Equipment.QueryEquipment(user.Equipment.Alternante);
                                }
                            }
                        }
                        break;
                    }
                case Action.Upgrade:
                    {
                        SubClases item;
                        if (user.Player.SubClass.src.TryGetValue(Class, out item))
                        {
                            if (item.Level >= 9)
                                break;

                            ushort NeedStudyPoints = (ushort)Pool.LevelInfo[Class][item.Level].Experience;
                            if (user.Player.SubClass.StudyPoints >= NeedStudyPoints)
                            {
                                user.Player.SubClass.RemoveStudy(user, NeedStudyPoints, stream);
                                item.Level++;
                                user.Player.SubClass.src[Class] = item;

                                user.Send(stream.SubClassCreate(Action.SendUpdate, item.ID, 0, 0));
                            }
                        }
                        break;
                    }
                case Action.Join:
                    {
                        if ((byte)Class > 9)
                            break;

                        if (Pool.SubClassInfo.AllowUpdate(user, Class, 0))
                        {
                            uint ItemID = ItemsPromote[(byte)Class];
                            byte amount = ItemsCount[(byte)Class];
                            if (user.Inventory.Contain(ItemID, amount))
                            {
                                if (user.Player.SubClass.SendLearn(user, SubClases.Create(Class), stream))
                                {
                                    user.Inventory.Remove(ItemID, amount, stream);
                                }
                            }
                        }
                        break;
                    }
                case Action.SwitchSubClass:
                    {
                        if (user.Player.SubClass.src.ContainsKey(Class))
                        {

                            user.Send(stream.SubClassCreate(Action.ActivateSubClass, Class, user.Player.SubClass.src[Class].Level, 0));


                            user.Player.SubClass.ActiveSubclass = Class;
                            user.Player.SubClass.CreateSpawn(user);
                        }
                        else if (Class == Database.DBLevExp.Sort.User)
                        {
                            user.Send(stream.SubClassCreate(Action.ActivateSubClass, Database.DBLevExp.Sort.User, 0, 0));

                            user.Player.SubClass.ActiveSubclass = Class;
                            user.Player.SubClass.CreateSpawn(user);
                        }
                        break;
                    }
            }
        }
    }
}
