using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Qahr.Mobile.WindowsAPI
{
    public unsafe class Kernel32
    {






        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileSection(string lpAppName, sbyte* lpReturnedString, int nSize, string lpFileName);



        [DllImport("kernel32.dll")]
        public static extern unsafe int SetFilePointer(SafeFileHandle handle, int lo, int* hi, SeekOrigin origin);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe int WriteFile(SafeFileHandle handle, byte* bytes, int numBytesToWrite, int* numBytesWritten, IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe int ReadFile(SafeFileHandle handle, byte* bytes, int numBytesToRead, int* numBytesRead, IntPtr lpOverlapped);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetPrivateProfileIntW(string Section, string Key, int Default, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetPrivateProfileStringW(string Section, string Key, string Default, char* ReturnedString, int Size, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern uint GetPrivateProfileStringA(string Section, string Key, void* Default, sbyte* ReturnedString, int Size, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPrivateProfileStructW(string Section, string Key, void* lpStruct, int StructSize, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileSectionNamesW(char* ReturnBuffer, int Size, string FileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileSectionW(string Section, char* ReturnBuffer, int Size, string FileName);


        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

     

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileStructW(string Section, string Key, void* lpStruct, int StructSize, string FileName);


      


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTitle(string lpConsoleTitle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadConsole(IntPtr hConsoleInput, [Out] StringBuilder lpBuffer, uint nNumberOfCharsToRead, out uint lpNumberOfCharsRead, IntPtr lpReserved);
        [DllImport("kernel32.dll")]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);
        [DllImport("kernel32.dll")]
        public static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);

        [DllImport("kernel32.dll")]
        public static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);


        [DllImport("kernel32")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwProcessId);


        public static unsafe void* malloc(Int32 size)
        {
            void* ptr = Marshal.AllocHGlobal(size).ToPointer();
            return ptr;
        }
        public static unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            

            Int32 count = size / sizeof(Int32);
            for (Int32 i = 0; i < count; i++)
                *(((Int32*)dest) + i) = *(((Int32*)src) + i);

            Int32 pos = size - (size % sizeof(Int32));
            for (Int32 i = 0; i < size % sizeof(Int32); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
        public static unsafe void free(void* ptr)
        {
            if (ptr != null)
                Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}