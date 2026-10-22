// ============================================================================
//  ModuleSchedule - makes a module appear / disappear automatically on a
//  pre-registered schedule. A module returns one of these from its Schedule
//  property; the ModuleManager checks it every ~30s and raises OnActivate /
//  OnDeactivate when the window opens / closes.
//
//  Schedule == null  => the module is ALWAYS active.
//
//  Examples (inside a module):
//     // every day, 20:00 - 22:00
//     public override ModuleSchedule Schedule => ModuleSchedule.Daily("20:00", "22:00");
//     // every Friday, 21:00 - 23:00
//     public override ModuleSchedule Schedule => ModuleSchedule.Weekly(DayOfWeek.Friday, "21:00", "23:00");
//     // Mon + Wed + Fri, 19:00 - 20:00
//     public override ModuleSchedule Schedule => ModuleSchedule.Weekly(new[]{DayOfWeek.Monday,DayOfWeek.Wednesday,DayOfWeek.Friday}, "19:00", "20:00");
//     // the 1st of every month, all day
//     public override ModuleSchedule Schedule => ModuleSchedule.StartOfMonth();
//     // the 1st + 15th of every month, 18:00 - 23:59
//     public override ModuleSchedule Schedule => ModuleSchedule.MonthlyDays(new[]{1,15}, "18:00", "23:59");
//     // once a year, 24 Dec .. 26 Dec (whole days)
//     public override ModuleSchedule Schedule => ModuleSchedule.Yearly(12, 24, "00:00", "23:59").UntilDay(12,26);
//     // a one-off date range (e.g. a launch event)
//     public override ModuleSchedule Schedule => ModuleSchedule.Range(new DateTime(2026,12,24,20,0,0), new DateTime(2026,12,26,23,0,0));
// ============================================================================
using System;

namespace Qahr.Mobile.Modules
{
    public sealed class ModuleSchedule
    {
        public enum Freq { Always, Daily, Weekly, MonthlyByDay, Yearly, DateRangeOnly }

        /// <summary>Overall validity window (null = unbounded).</summary>
        public DateTime? From;
        public DateTime? To;
        public Freq Frequency = Freq.Always;
        /// <summary>Weekly: which weekdays the module is active.</summary>
        public DayOfWeek[] Days;
        /// <summary>MonthlyByDay / Yearly: which day numbers (1..31).</summary>
        public int[] DaysOfMonth;
        /// <summary>Optional month filter (1..12). null = every month.</summary>
        public int[] Months;
        /// <summary>Active-from time each active day.</summary>
        public TimeSpan StartOfDay = TimeSpan.Zero;
        /// <summary>Active-until time each active day (may be before StartOfDay to cross midnight).</summary>
        public TimeSpan EndOfDay = new TimeSpan(23, 59, 59);

        public bool IsActiveNow() { return IsActive(DateTime.Now); }

        public bool IsActive(DateTime now)
        {
            if (From.HasValue && now < From.Value) return false;
            if (To.HasValue && now > To.Value) return false;
            if (Months != null && Months.Length > 0 && Array.IndexOf(Months, now.Month) < 0) return false;

            bool dayMatches;
            switch (Frequency)
            {
                case Freq.Always:
                case Freq.DateRangeOnly:
                case Freq.Daily:
                    dayMatches = true; break;
                case Freq.Weekly:
                    dayMatches = Days != null && Array.IndexOf(Days, now.DayOfWeek) >= 0; break;
                case Freq.MonthlyByDay:
                case Freq.Yearly:
                    dayMatches = DaysOfMonth != null && Array.IndexOf(DaysOfMonth, now.Day) >= 0; break;
                default:
                    dayMatches = true; break;
            }
            if (!dayMatches) return false;

            if (Frequency == Freq.DateRangeOnly) return true; // bounded only by From..To

            TimeSpan t = now.TimeOfDay;
            if (EndOfDay >= StartOfDay) return t >= StartOfDay && t <= EndOfDay;
            return t >= StartOfDay || t <= EndOfDay; // window crosses midnight
        }

        // --------------------------------------------------------- factories
        public static ModuleSchedule Always()
        {
            return new ModuleSchedule { Frequency = Freq.Always };
        }
        public static ModuleSchedule Daily(string start, string end)
        {
            return new ModuleSchedule { Frequency = Freq.Daily, StartOfDay = T(start), EndOfDay = T(end) };
        }
        public static ModuleSchedule Weekly(DayOfWeek day, string start, string end)
        {
            return Weekly(new[] { day }, start, end);
        }
        public static ModuleSchedule Weekly(DayOfWeek[] days, string start, string end)
        {
            return new ModuleSchedule { Frequency = Freq.Weekly, Days = days, StartOfDay = T(start), EndOfDay = T(end) };
        }
        public static ModuleSchedule MonthlyDays(int[] daysOfMonth, string start, string end)
        {
            return new ModuleSchedule { Frequency = Freq.MonthlyByDay, DaysOfMonth = daysOfMonth, StartOfDay = T(start), EndOfDay = T(end) };
        }
        public static ModuleSchedule StartOfMonth()
        {
            return MonthlyDays(new[] { 1 }, "00:00", "23:59");
        }
        public static ModuleSchedule StartOfMonth(string start, string end)
        {
            return MonthlyDays(new[] { 1 }, start, end);
        }
        public static ModuleSchedule Yearly(int month, int day, string start, string end)
        {
            return new ModuleSchedule { Frequency = Freq.Yearly, Months = new[] { month }, DaysOfMonth = new[] { day }, StartOfDay = T(start), EndOfDay = T(end) };
        }
        public static ModuleSchedule Range(DateTime from, DateTime to)
        {
            return new ModuleSchedule { Frequency = Freq.DateRangeOnly, From = from, To = to };
        }

        // --------------------------------------------------------- fluent bounds
        /// <summary>Only run on/after this date.</summary>
        public ModuleSchedule StartingOn(DateTime from) { From = from; return this; }
        /// <summary>Stop running after this date.</summary>
        public ModuleSchedule EndingOn(DateTime to) { To = to; return this; }
        /// <summary>Restrict to these months (1..12).</summary>
        public ModuleSchedule InMonths(params int[] months) { Months = months; return this; }

        private static TimeSpan T(string hhmm)
        {
            TimeSpan ts;
            if (TimeSpan.TryParse(hhmm, out ts)) return ts;
            return TimeSpan.Zero;
        }
    }
}
