using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgMonster
{
    [Flags]
    public enum MonsterSettings : uint
    {
        None = 0x00,
        /// <summary>
        /// Indicates that this monster will move around
        /// </summary>
        Moves = 0x01,
        /// <summary>
        /// Indicates that this monster should revive players around it who're dead.
        /// </summary>
        RevivesSurroundings = 0x02,
        /// <summary>
        /// Indicates that this monster should attack it's target.
        /// </summary>
        Aggressive = 0x04,
        /// <summary>
        /// Indicates this monster has an owner, and should follow it, and attack with it.
        /// </summary>
        HasPlayerOwner = 0x08,
        /// <summary>
        /// Indicates this monster will drop items (and money) when it dies.
        /// </summary>
        DropItemsOnDeath = 0x10,
        /// <summary>
        /// Indicates that this mosnter should be revived after it dies (shortly).
        /// </summary>
        Respawns = 0x20,
        /// <summary>
        /// Indicates that nobody should beable to hurt this monster (hit ones).
        /// </summary>
        Invincible = 0x40,
        /// <summary>
        /// Indicates this monster should automatically attack anyone flashing (bluename).
        /// </summary>
        KillFlashers = 0x80,
        /// <summary>
        /// Indicates this monster is a king.
        /// </summary>
        King = Standard | 0x100,
        /// <summary>
        /// Indicates this mosnter is an aide
        /// </summary>
        Aide = Standard | 0x200,
        /// <summary>
        /// Indicates this monster is a messenger
        /// </summary>
        Messenger = Standard | 0x400,

        Lottus = 2048,

        Standard = Moves | Aggressive | DropItemsOnDeath | Respawns,
        Reviver = RevivesSurroundings | Invincible | Respawns,
        Guard = Aggressive | KillFlashers | Respawns
    }
}
