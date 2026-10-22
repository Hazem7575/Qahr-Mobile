using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role
{
    public enum MapObjectType : byte
    {
        Player = 0,
        Monster = 1,
        SobNpc = 2,
        StaticRole = 3,
        Item = 4,
        Npc = 5,
        //PokerTable = 6,
        Count = 7

    }
    public enum MapObjectTypet
    {
        SobNpc,
        Npc,
        Item,
        Monster,
        FloorSpell,
        Player,
        Nothing,
        InvalidCast,
        StaticEntity
    }
}
