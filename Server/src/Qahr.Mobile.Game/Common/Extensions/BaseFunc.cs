using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Qahr.Mobile
{
    public static class BaseFunc
    {
        // C signatures: void srand(unsigned int seed); int rand(void);  (0..32767)
        // The old 'Int64 rand()' read EDX:EAX on x86 (garbage high dword from EDX) and RAX on x64.
        // The old 'srand(UInt64)' only ever used the low 32 bits of the seed on x86 (cdecl), same as now.
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void srand(uint seed);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int rand();
        public static byte AnimaUpgradeRate(uint AnimaID, bool forging = false)
        {
            switch (AnimaID % 100)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return 35;
                case 10:
                case 11:
                case 12:
                    return 50;

                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    return 100;

                case 19:
                    return 0;
            }

            return (byte)(forging ? 58 : 55);
        }
        public static int RandGet(int nMax, bool bRealRand)
        {
            if (nMax <= 0)
                nMax = 1;

            if (bRealRand)
                srand(unchecked((uint)(ulong)DateTime.Now.Value()));
            long val = rand();
            return (int)(val % nMax);
        }
        public static byte SteedSpeed(byte plus)
        {
            return _SteedSpeed[(int)System.Math.Min(plus, (byte)12)];
        }
        private static byte[] _SteedSpeed = new byte[]
		{
			0,
			5,
			10,
			15,
			20,
			30,
			40,
			50,
			65,
			85,
			90,
			95,
			100
		};
        //////////////////////////////////////////////////////////////////////
        public static double RandomRateGet(double dRange)
        {
            double pi = 3.1415926;

            int nRandom = RandGet(999, true) + 1;
            double a = Math.Sin(nRandom * pi / 1000);
            double b;
            if (nRandom >= 90)
                b = (1.0 + dRange) - Math.Sqrt(Math.Sqrt(a)) * dRange;
            else
                b = (1.0 - dRange) + Math.Sqrt(Math.Sqrt(a)) * dRange;

            return b;
        }
        
        public static uint ProficiencyLevelExperience(byte Level)
        {
            return proficiencyLevelExperience[Math.Min(Level, (byte)20)];
        }

        private static uint[] proficiencyLevelExperience = new uint[21]
        {
            0, 1200, 68000, 250000, 640000, 1600000, 4000000, 10000000, 22000000, 40000000, 90000000, 95000000, 142500000,
            213750000, 320625000, 480937500, 721406250, 1082109375, 1623164062, 2100000000, 3200000000
        };
        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name == null)
                return false;
            if (name == "")
                return false;
            // SECURITY: validate EVERY character against a whitelist (the old regex anchored with '$'
            // only checked the LAST character, so spaces, CR/LF, tabs and control characters passed
            // inside the name and could inject INI lines / forge fields). Anything outside the
            // whitelist (letters, digits, Arabic range, and the few symbols the server allows) is
            // rejected, which also blocks CR/LF/space/control/brackets/quotes/slashes.
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                bool ok = (c >= 'A' && c <= 'Z')
                          || (c >= 'a' && c <= 'z')
                          || (c >= '0' && c <= '9')
                          || (c >= 'ء' && c <= 'ي')
                          || c == '*' || c == '~' || c == '.' || c == '&' || c == '$';
                if (!ok)
                    return false;
            }

            if (name.Contains('/'))
                return false;
            if (name.Contains(@"\"))
                return false;
            if (name.Contains(@"'"))
                return false;

            // Banned words, matched case-insensitively (e.g. "gm", "Gm" must be rejected too).
            string lower = name.ToLowerInvariant();
            // (also covers longer words that contain them: gmpm, tqgm, speedhack, cheatengine, guard1 ...)
            string[] banned = { "gm", "pm", "tq", "system", "none", "clicker", "aimbot", "hack", "engine", "cheat",
                "speed", "moksha", "guard", "kosamk" };
            for (int i = 0; i < banned.Length; i++)
                if (lower.Contains(banned[i]))
                    return false;

            if (name.Length > 16 && ExceptedSize)
                return false;
            for (int x = 0; x < name.Length; x++)
                if (name[x] == 25)
                    return false;
            return true;
        }
        public static string ReverseString(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = "";
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }
       
        public static uint RandFromGivingNums(uint[] nums)
        {
            if (nums == null || nums.Length == 0) return 0;
            return nums[Pool.GetRandom.Next(0, nums.Length)];
        }
    }
}
