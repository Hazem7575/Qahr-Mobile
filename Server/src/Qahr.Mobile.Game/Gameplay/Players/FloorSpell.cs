using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role
{
    public class FloorSpell
    {
        public DateTime Stamp = new DateTime();
        public DateTime CheckMohy3 = new DateTime();
        public DateTime ElseStamp = new DateTime();
        public Game.MsgFloorItem.MsgItemPacket FloorPacket;
        public int ElseMillisecondsStamp;
        public int Stamps;
        public Database.MagicType.Magic DBSkill;

        public FloorSpell(uint ID, ushort X, ushort Y, byte color
      , Database.MagicType.Magic _DBSkill, int MillisecondsStamp
      , int _ElseMillisecondsStamp = 0)
        {

            if (Game.MsgFloorItem.MsgItem.UIDS.Count < 900092)
                Game.MsgFloorItem.MsgItem.UIDS.Set(900092);

            FloorPacket = Game.MsgFloorItem.MsgItemPacket.Create();
            FloorPacket.m_UID = Game.MsgFloorItem.MsgItem.UIDS.Next;

            FloorPacket.m_ID = ID;
            FloorPacket.m_X = X;
            FloorPacket.m_Y = Y;
            FloorPacket.m_Color = color;
            FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.Effect;
            Stamps = MillisecondsStamp;
            ElseMillisecondsStamp = _ElseMillisecondsStamp;
            Stamp = DateTime.Now.AddMilliseconds(MillisecondsStamp);
            DBSkill = _DBSkill;

        }

        public unsafe class ClientFloorSpells
        {
            public List<FloorSpell> Spells = new List<FloorSpell>();
            public Game.MsgServer.MsgSpellAnimation SpellPacket;
            public Database.MagicType.Magic DBSkill;
            public ushort X;
            public ushort Y;
            public byte LevelHu;
            public uint UID;
            private object SyncRoot;
            public GameMap GMap;
            public ushort AttackerX;
            public ushort AttackerY;
            public ClientFloorSpells(uint _UID, ushort _X, ushort _Y, byte _LevelHu, Database.MagicType.Magic _DBSkill, GameMap _GMap, ushort xAttackerX = 0, ushort xAttackerY = 0)
            {
                GMap = _GMap;
                DBSkill = _DBSkill;
                SyncRoot = new object();
                X = _X;
                Y = _Y;
                LevelHu = _LevelHu;
                UID = _UID;
                AttackerX = xAttackerX;
                AttackerY = xAttackerY;
            }
            public void CreateMsgSpell(uint Target)
            {
                if (DBSkill.ID == (ushort)Role.Flags.SpellID.TwilightDance)
                {
                    SpellPacket = new Game.MsgServer.MsgSpellAnimation(1000000, Target, X, Y, DBSkill.ID, DBSkill.Level, LevelHu);
                }
                else
                {
                    SpellPacket = new Game.MsgServer.MsgSpellAnimation(UID, Target, X, Y, DBSkill.ID, DBSkill.Level, LevelHu);
                }
            }
        
            public bool CheckInvocke(DateTime Now, FloorSpell spell)
            {
                if (Now > spell.Stamp)
                {
                    return true;
                }
                return false;
            }
            public bool CheckElseInvocke(DateTime Now, FloorSpell spell)
            {
                if (Now > spell.ElseStamp)
                {
                    spell.ElseStamp = DateTime.Now;
                    spell.ElseStamp = DateTime.Now.AddMilliseconds(spell.ElseMillisecondsStamp);
                    return true;
                }
                
                return false;
            }
            public bool CheckMohy1(DateTime Now, FloorSpell spell)
            {
                if (Now > spell.CheckMohy3)
                {
                    return true;
                }
                return false;
            }
            public bool CheckMohy2(DateTime Now, FloorSpell spell)
            {
                if (Now > spell.ElseStamp)
                {
                    spell.ElseStamp = DateTime.Now;
                    spell.ElseStamp = DateTime.Now.AddSeconds(spell.ElseMillisecondsStamp);
                    return true;
                }

                return false;
            }
            public void RemoveItem(FloorSpell item)
            {
                lock (SyncRoot)
                {
                    Spells.Remove(item);
                }
            }
            public void SendView(ServerSockets.Packet stream, Client.GameClient client)
            {
                
                SpellPacket.SetStream(stream);

                SpellPacket.Send(client);

            }

            public void AddItem(FloorSpell item)
            {
                lock (SyncRoot)
                {
                    item.FloorPacket.MapID = GMap.ID;
                    Spells.Add(item);
                }
            }
        }
    }
}
