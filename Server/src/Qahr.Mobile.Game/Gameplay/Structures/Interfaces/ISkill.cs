namespace Qahr.Mobile.Structures.Interfaces
{
    using System;

    public interface ISkill
    {
        UInt32 Experience { get; set; }
        UInt16 ID { get; set; }
        Byte Level { get; set; }
        Byte PreviousLevel { get; set; }
      
        Byte LevelHu { get; set; }
        bool Available { get; set; }
      
        DateTime LastUseTime { get; set; }
    }
}
