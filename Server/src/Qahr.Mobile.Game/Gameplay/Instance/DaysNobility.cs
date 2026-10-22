using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Role.Instance
{
    public class DaysNobility
    {
        private Player Owner;
        private bool Active = false;
        public bool IsActive { get { return this.Active; } }
        private uint id;
        public uint Identifier { get { return this.id; } private set { this.id = value; } }
        private ulong lastdonation;
        public ulong LastNobilityDonation { get { return this.lastdonation; } private set { this.lastdonation = value; } }
        private Nobility.NobilityRank paidrnk;
        public Nobility.NobilityRank PaidRank { get { return this.paidrnk; } private set { this.paidrnk = value; } }
        private long periodtick;
        public DateTime PeriodTime { get { return DateTime.FromBinary(this.periodtick); } private set { this.periodtick = value.ToBinary(); } }
        public DaysNobility(Player client)
        {
            this.Owner = client;
            this.id = client.UID;
        }
        public void Paid(int days, Nobility.NobilityRank rnk)
        {
            this.paidrnk = rnk;
            this.periodtick = DateTime.Now.AddDays(days).ToBinary();
            this.lastdonation = this.Owner.Nobility.Donation;
            Pool.NobilityRanking.RemoveAndUpdateTheRest(this.id);
            this.Active = true;
            switch (rnk)
            {
                case Qahr.Mobile.Role.Instance.Nobility.NobilityRank.King:
                    {
                        this.Owner.Nobility.Donation = 0;
                        this.Owner.Nobility.Position = 0;
                        this.Owner.NobilityRank = this.Owner.Nobility.Rank;
                        break;
                    }
                case Qahr.Mobile.Role.Instance.Nobility.NobilityRank.Prince:
                    {
                        this.Owner.Nobility.Donation = 0;
                        this.Owner.Nobility.Position = 6;
                        this.Owner.NobilityRank = this.Owner.Nobility.Rank;
                        break;
                    }
                case Qahr.Mobile.Role.Instance.Nobility.NobilityRank.Duke:
                    {
                        this.Owner.Nobility.Donation = 0;
                        this.Owner.Nobility.Position = 15;
                        this.Owner.NobilityRank = this.Owner.Nobility.Rank;
                        break;
                    }
                default:
                    {
                        this.Active = false;
                        break;
                    }
            }
        }
        public void SetData(uint ID = 0, ulong lastDS = 0, byte rank = 0, long tick = 0, bool act = false)
        {
            if (ID != 0)
            {
                this.id = ID;
            }
            if (lastDS != 0)
            {
                this.lastdonation = lastDS;
            }
            if (rank != 0)
            {
                this.paidrnk = (Nobility.NobilityRank)rank;
            }
            if (tick != 0)
            {
                this.periodtick = tick;
            }
            if (act != false)
            {
                this.Active = act;
            }
        }
        public void SetDataAnyWay(uint ID = 0, ulong lastDS = 0, byte rank = 0, long tick = 0, bool act = false)
        {
            this.id = ID;
            this.lastdonation = lastDS;
            this.paidrnk = (Nobility.NobilityRank)rank;
            this.periodtick = tick;
            this.Active = act;
        }
    }
}
