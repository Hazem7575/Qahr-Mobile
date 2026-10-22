===========================================================================
  Modules\Custom\  -  home of ALL custom (non-official-Conquer) content
===========================================================================

Any feature that is NOT part of official Conquer Online goes here. Each
feature is a SELF-CONTAINED FOLDER (like Laravel): everything for that
feature - the module, its NPCs, its packets, its data - lives together in
one place, so a feature can be added, disabled or removed as a single unit.

    Modules\Custom\
       ExampleEvent\                 <- one folder per feature
          ExampleEventModule.cs         the module (lifecycle + schedule)
          <YourNpc>.cs                  its NPC handler(s)   [NpcAttribute]
          <YourPacket>.cs               its packet handler(s) [PacketAttribute]
       MyOtherFeature\
          ...

Per-feature data (saves, config) goes under  <server>\Modules\<Name>\  -
use the module's  DataPath  property.

---------------------------------------------------------------------------
How to add a custom feature
---------------------------------------------------------------------------
1. Make a new folder under Custom\ and copy ExampleEvent\ExampleEventModule.cs
   into it; rename the class, the namespace and the Name property.
2. Put the feature's logic in the hooks:
      OnLoad       - once at startup (code loaded) - set up, don't start yet
      OnActivate   - the schedule window OPENED  -> start the event
      OnDeactivate - the schedule window CLOSED  -> end it, clean up, rewards
      OnSave       - every periodic world save (only while active)
      OnLogin      - a player logged in (only while active)
      OnLogout     - a player left (only while active)
      OnShutdown   - server shutting down
3. NPCs and packets stay exactly as the rest of the source: a static method
   with [NpcAttribute] / [PacketAttribute]. Just keep those files in the
   feature folder so everything lives together.
4. Add every new .cs file to GameServer.csproj (<Compile Include="..."/>).
5. Set Enabled => false to keep the code but not run it.

---------------------------------------------------------------------------
Scheduling - a module that appears / disappears by itself
---------------------------------------------------------------------------
A module returns a ModuleSchedule from its  Schedule  property. The manager
checks it every ~30s and calls OnActivate when the window opens, OnDeactivate
when it closes. While inactive the module gets NO hooks. Schedule => null is
always-on.

   ModuleSchedule.Daily("20:00", "22:00")                       every day 8-10pm
   ModuleSchedule.Weekly(DayOfWeek.Friday, "21:00", "23:00")    every Friday
   ModuleSchedule.Weekly(new[]{DayOfWeek.Sat,DayOfWeek.Sun}, "18:00","23:00")
   ModuleSchedule.StartOfMonth()                                the 1st, all day
   ModuleSchedule.MonthlyDays(new[]{1,15}, "18:00","23:59")     1st + 15th
   ModuleSchedule.Yearly(12, 24, "00:00", "23:59")              24 Dec every year
   ModuleSchedule.Range(new DateTime(2026,12,24,20,0,0),
                        new DateTime(2026,12,26,23,0,0))         one-off window
   .StartingOn(date) / .EndingOn(date) / .InMonths(6,7,8)       extra bounds
   null                                                         always on

---------------------------------------------------------------------------
Rules for hook code (hooks run on game threads)
---------------------------------------------------------------------------
* Keep hooks FAST. Never block on files / MySQL / network inside a hook.
* Assume concurrent calls: guard shared state with locks or Concurrent*.
* An exception inside a hook is caught + logged and never reaches the game.
* Combat and movement are intentionally NOT hooked, so a module can never
  slow those hot paths down.
