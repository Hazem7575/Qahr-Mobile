using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Database
{
    public class ClientSpells
    {
        public struct DBSpell
        {
            public ushort ID;
            public ushort Level;
            public int Experience;
            public byte PreviousLevel;
            public byte SoulLevel;
            public byte UseJiangSpell;

            public DBSpell GetDBSpell(Game.MsgServer.MsgSpell spell)
            {
                ID = spell.ID;
                Level = spell.Level;
                Experience = spell.Experience;
                PreviousLevel = spell.PreviousLevel;
                SoulLevel = spell.SoulLevel;
                UseJiangSpell = spell.UseSpellSoul;
                return this;
            }
            public Game.MsgServer.MsgSpell GetClientSpell()
            {
                Game.MsgServer.MsgSpell spell = new Game.MsgServer.MsgSpell();
                spell.ID = ID;
                spell.Level = Level;
                spell.Experience = Experience;
                spell.PreviousLevel = PreviousLevel;
                spell.SoulLevel = SoulLevel;
                spell.UseSpellSoul = UseJiangSpell;
                return spell;
            }
        }
    }
}
