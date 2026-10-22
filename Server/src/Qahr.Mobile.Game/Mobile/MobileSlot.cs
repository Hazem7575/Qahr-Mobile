namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// Slot machine result (MsgSlotResult 2045 -> mobile CMsgSlotResult 10007, the same struct).
    /// CMsgSlotResult::Process action 1 (stop) passes u64 8 and npc 16 to CSlotMachineMgr::SSetSettleInfo(npc, money),
    /// which stores u64 8 as the "bonus to depot" (GetBonus2Depot): CDlgSlotMachine::SettleBackFromServer shows the box
    /// "The Silver you are unable to carry has been deposited in your warehouse" when it is above 0, and takes it off the
    /// win counted into "My Silver". The "Win" itself the mobile works out from the reels (see MobileSlotOdds), so u64 8
    /// stays 0: this server pays every win into the bag.
    /// (The reel symbols are the same numbers on both sides: 0..6, 6 = dragon pearl, see
    /// CSlotMachineMgr::GetDragonPearl; a reel of 0 is drawn by the mobile as a random blank unit.)
    /// </summary>
    public static class MobileSlot
    {
        public static ServerSockets.Packet StopCreate(ServerSockets.Packet stream, Role.Instance.SlotMachine machine, uint npcId, ulong reward)
        {
            stream.InitWriter();
            stream.Write((byte)Game.MsgServer.MsgMachine.SlotMachineSubType.StopSpin);
            stream.Write((byte)machine.Wheels[0]);
            stream.Write((byte)machine.Wheels[1]);
            stream.Write((byte)machine.Wheels[2]);
            stream.Write((ulong)0);// bonus deposited in the warehouse (none)
            stream.Write(npcId);
            stream.Write((uint)0);
            stream.Finalize(Game.GamePackets.MsgSlotResult);
            return stream;
        }
    }
}
