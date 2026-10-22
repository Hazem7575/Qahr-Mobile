// ============================================================================
//  Template: a self-contained CUSTOM event module (folder-per-feature).
//
//  Everything that belongs to this feature lives in THIS folder:
//     Modules\Custom\ExampleEvent\
//        ExampleEventModule.cs   <- the module (lifecycle + schedule)
//        <YourNpc>.cs            <- its NPC handler(s)   ([NpcAttribute])
//        <YourPacket>.cs         <- its packet handler(s) ([PacketAttribute])
//        ...                      (data files go under  <server>\Modules\ExampleEvent\ via DataPath)
//
//  The module APPEARS and DISAPPEARS by itself on the schedule below - the
//  manager calls OnActivate when the window opens and OnDeactivate when it
//  closes (re-checked every ~30s). Set Schedule => null to be always on.
// ============================================================================
using Qahr.Mobile.Modules;

namespace Qahr.Mobile.Modules.Custom.ExampleEvent
{
    public sealed class ExampleEventModule : ModuleBase
    {
        public override string Name { get { return "ExampleEvent"; } }
        public override string Version { get { return "1.0"; } }

        // Template is off by default; flip to true and it will run on the schedule below.
        public override bool Enabled { get { return false; } }

        // ----- WHEN this module is live -----------------------------------
        // every day, 20:00 -> 22:00. Swap for any of:
        //   ModuleSchedule.Weekly(DayOfWeek.Friday, "21:00", "23:00")
        //   ModuleSchedule.Weekly(new[]{ DayOfWeek.Saturday, DayOfWeek.Sunday }, "18:00", "23:00")
        //   ModuleSchedule.StartOfMonth()                         // the 1st of every month
        //   ModuleSchedule.MonthlyDays(new[]{1,15}, "18:00","23:59")
        //   ModuleSchedule.Yearly(12, 24, "00:00", "23:59")       // 24 Dec every year
        //   ModuleSchedule.Range(new System.DateTime(2026,12,24,20,0,0), new System.DateTime(2026,12,26,23,0,0))
        //   null                                                  // always on
        public override ModuleSchedule Schedule
        {
            get { return ModuleSchedule.Daily("20:00", "22:00"); }
        }

        public override void OnLoad()
        {
            // Called once at startup (code loaded). Set up data here - do NOT start the event yet.
            Log("loaded (template). Data folder: " + DataPath);
        }

        public override void OnActivate()
        {
            // The schedule window just OPENED - start the event, spawn NPCs/bosses, announce, etc.
            Log("event STARTED (schedule window opened).");
        }

        public override void OnDeactivate()
        {
            // The schedule window just CLOSED - end the event, remove spawns, give rewards, clean up.
            Log("event ENDED (schedule window closed).");
        }

        public override void OnLogin(Client.GameClient client)
        {
            // Only called while the event is active. Keep it fast.
            // client.Player is available here.
        }
    }
}
