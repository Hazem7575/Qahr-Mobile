using Qahr.Mobile.Database.DBActions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Qahr.Mobile.Role.Instance
{
    public class GuildSkill
    {
        public class Skill
        {
            public uint ID;
            public uint Level;
        }
        public ConcurrentDictionary<uint, GuildSkill.Skill> Skills = new ConcurrentDictionary<uint, GuildSkill.Skill>();
        private unsafe Client.GameClient gameClient;

        public unsafe GuildSkill(Client.GameClient gameClient)
        {
            this.gameClient = gameClient;
        }

        public override string ToString()
        {
            WriteLine writeLine = new WriteLine('/');
            writeLine.Add(this.Skills.Values.Count);
            foreach (GuildSkill.Skill skill in Skills.Values)
            {
                writeLine.Add(skill.ID);
                writeLine.Add(skill.Level);
            }
            return writeLine.Close();
        }

        public void Load(string Line)
        {
            ReadLine readLine = new ReadLine(Line, '/');
            int num = readLine.Read(0);
            for (int index = 0; index < num; ++index)
            {
                GuildSkill.Skill skill = new GuildSkill.Skill();
                skill.ID = readLine.Read((uint)0);
                skill.Level = readLine.Read((uint)0);
                this.Skills.Add(skill.ID, skill);
            }
        }
    }
}
