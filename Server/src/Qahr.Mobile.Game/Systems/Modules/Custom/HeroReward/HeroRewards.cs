using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role.Instance
{
    public class HeroRewards
    {
        public class Item
        {

            public uint UID;

            public uint UnKnow;

            public byte Claim;
        }

        private Client.GameClient Owner;
        public System.SafeDictionary<uint, Item> ArrayGoals = new System.SafeDictionary<uint, Item>();

        public HeroRewards(Client.GameClient _owner)
        {
            Owner = _owner;
        }

        public void AddGoal(uint ID)
        {
            if (!ArrayGoals.ContainsKey(ID))
            {
                ArrayGoals.Add(ID, new Item { UID = ID });
            }
        }

        public bool ContainGoal(uint ID)
        {
            return ArrayGoals.ContainsKey(ID);
        }

        public bool FinishStage(uint ID)
        {
            int count = ArrayGoals.Where(p => p.Key / 100 == ID).Count();
            if (Pool.TableHeroRewards.ContainsKey(ID))
                return count == Pool.TableHeroRewards[ID].CountGoals;
            return
                false;
        }
        public bool ComplateFullStage(uint ID)
        {

            int count = ArrayGoals.Where(p => p.Key / 100 == ID && p.Value.Claim > 0 || p.Key == ID && p.Value.Claim > 0).Count();
            if (Pool.TableHeroRewards.ContainsKey(ID))
                return count == Pool.TableHeroRewards[ID].CountGoals + 1;
            return
                false;
        }

        public override string ToString()
        {
            Database.DBActions.WriteLine writeline = new Database.DBActions.WriteLine('/');
            writeline.Add(ArrayGoals.Count);
            foreach (var item in ArrayGoals.Values)
            {
                writeline.Add(item.UID).Add(item.Claim);
            }
            return writeline.Close();
        }
        public void Load(string line)
        {
            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(line, '/');
            int count = reader.Read((int)0);
            for (int x = 0; x < count; x++)
            {
                Item item = new Item();
                item.UID = reader.Read((uint)0);
                item.Claim = reader.Read((byte)0);

                if (!ArrayGoals.ContainsKey(item.UID))
                    ArrayGoals.Add(item.UID, item);
            }
        }
    

    }
}
