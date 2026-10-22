using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role.Instance
{
    public class DemonExterminator
    {
        public uint ItemUID = 0;
        public byte FinishToday = 0;
        public ushort HuntKills = 0;
        public uint QuestTyp = 0;
        public bool OnQuest = false;

        private Game.MsgServer.MsgGameItem Jar;
        public virtual void UppdateJar(Client.GameClient user, uint MonsterID)
        {
            if (ItemUID != 0)
            {
                //check jar
                if (Jar == null)
                {
                    user.Inventory.TryGetItem(ItemUID, out Jar);
                }
                if (Jar != null)
                {
                    if(Jar.UID != ItemUID)
                        user.Inventory.TryGetItem(ItemUID, out Jar);
                }
                //----------
                if (Jar != null)
                {


                    if (CheckUpKill(Jar.MaximDurability, MonsterID))
                        //Jar.MaximDurability == MonsterID)
                    {
                        HuntKills++;
                    }
                }
                else
                    ItemUID = 0;
            }
        }
        public bool CheckUpKill(ushort itemtype, uint monstertype)
        {
            switch (itemtype)
            {
                case 58:
                    {
                        if (monstertype == 8219 || monstertype == 8319 || monstertype == 8119 || monstertype == 83)
                            return true;
                        break;
                    }
                case 57:
                    {
                        if (monstertype == 8218 || monstertype == 8318 || monstertype == 8118 || monstertype == 82)
                            return true;
                        break;
                    }
                case 56:
                    {
                        if (monstertype == 8217 || monstertype == 8317 || monstertype == 8117 || monstertype == 81)
                            return true;
                        break;
                    }
                case 55:
                    {
                        if (monstertype == 84 || monstertype == 77 || monstertype == 79)
                            return true;
                        break;
                    }
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    {

                        if (monstertype == 72 + (itemtype - 13) || monstertype == 8208 + (itemtype - 13) || monstertype == 8308 + (itemtype - 13) || monstertype == 8108 + (itemtype - 13))
                            return true;
                        break;
                    }
                case 13:
                    {
                        if (monstertype == 72 || monstertype == 8208 || monstertype == 8308 || monstertype == 8108)
                            return true;
                        break;
                    }
                case 12:
                    {
                        if (monstertype == 71 || monstertype == 8207 || monstertype == 8307 || monstertype == 8107)
                            return true;
                        break;
                    }
                case 11:
                    {
                        if (monstertype == 70 || monstertype == 8206 || monstertype == 8306 || monstertype == 8106)
                            return true;
                        break;
                    }
                case 10:
                    {
                        if (monstertype == 69 || monstertype == 8205 || monstertype == 8305 || monstertype == 8105)
                            return true;
                        break;
                    }
                case 8:
                    {
                        if (monstertype == 67 || monstertype == 8203 || monstertype == 8103 || monstertype == 8303) 
                            return true;

                        break;
                    }
                case 7:
                    {
                        if (monstertype == 8102 || monstertype == 66)
                            return true;

                        break;
                    }
                case 6:
                    {
                        if (monstertype == 65 || monstertype == 8101 || monstertype == 8301 || monstertype == 8201)
                            return true;
                        break;
                    }
                case 5:
              
                    {
                        if (monstertype == 64 || monstertype == 25)
                            return true;
                        break;
                    }

            }
            return itemtype == monstertype;
        }
        public bool CreateDemonExterminator(Client.GameClient user,ServerSockets.Packet stream,  ushort AmountKills, ushort _QuestTyp)
        {
            QuestTyp = _QuestTyp;
            Game.MsgServer.MsgGameItem Jar;
            if (user.Inventory.TryGetItem(ItemUID, out Jar))
            {
                HuntKills = 0;
                Jar.Durability = 0;
                Jar.MaximDurability = (ushort)QuestTyp;
                Jar.Mode = Flags.ItemMode.Update;
                Jar.Send(user, stream);
                return true;
            }
            else
            {
                if (user.Inventory.HaveSpace(1))
                {
                    user.Inventory.Add(stream, 750000);
                    if (user.Inventory.TryGetItem(ItemUID, out Jar))
                    {
                        Jar.Durability = AmountKills;
                        Jar.MaximDurability = (ushort)QuestTyp;
                        Jar.Mode = Flags.ItemMode.Update;
                        Jar.Send(user, stream);
                        HuntKills = 0;
                    }
                }
                else
                {
#if Arabic
                     user.SendSysMesage("Please make 1 space in your bag.");
#else
                    user.SendSysMesage("Please make 1 space in your bag.");
#endif

                }
            }
            return false;
        }
        public byte GetDemonExterminatorStage(Game.MsgServer.MsgGameItem Jar)
        {
            if (Jar != null)
            {
                if (Jar.Durability == 600)
                    return 0;
                else if (Jar.Durability == 1200)
                    return 1;
                else if (Jar.Durability == 1500)
                    return 2;
                else if (Jar.Durability == 1800)
                    return 3;

                return byte.MaxValue;
            }
            return byte.MaxValue;
        }

        public string SpecialReward(Client.GameClient user, ServerSockets.Packet stream)
        {
            user.Player.TCCaptainTimes++;
            if (user.Player.TCCaptainTimes == 3)
            {
                user.Teleport(212, 136, 10723, user.Player.UID);
            }
            user.Map.AddMapMonster(stream, 8307, 212, 136, 1, 1, 1, user.Player.DynamicID,true);

            return "";
               
        }

        public override string ToString()
        {
            Database.DBActions.WriteLine Writer = new Database.DBActions.WriteLine('/');
            Writer.Add(ItemUID).Add(FinishToday).Add(HuntKills).Add(QuestTyp);
            return Writer.Close();
        }
        public void ReadLine(string Line)
        {
            Database.DBActions.ReadLine dbline = new Database.DBActions.ReadLine(Line, '/');
            ItemUID = dbline.Read((uint)0);
            FinishToday = dbline.Read((byte)0);
            HuntKills = dbline.Read((ushort)0);
            QuestTyp = dbline.Read((uint)0);
        }
    }
}
