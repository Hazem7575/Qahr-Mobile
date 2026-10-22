// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System
{
    public class OSClipboard
    {
        public const uint GMEM_DDESHARE = 8192U;
        public const uint GMEM_MOVEABLE = 2U;

        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();

        [DllImport("Kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr src, int size);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        public static string GetText()
        {
            OSClipboard.OpenClipboard(IntPtr.Zero);
            string str = Marshal.PtrToStringUni(OSClipboard.GetClipboardData(13U));
            OSClipboard.CloseClipboard();
            return str;
        }

        public static bool SetText(string text)
        {
            if (!OSClipboard.OpenClipboard(IntPtr.Zero))
                return false;
            OSClipboard.EmptyClipboard();
            IntPtr hMem = OSClipboard.GlobalAlloc(8194U, (UIntPtr)((ulong)(2 * (text.Length + 1))));
            IntPtr destination = OSClipboard.GlobalLock(hMem);
            if (text.Length > 0)
                Marshal.Copy(text.ToCharArray(), 0, destination, text.Length);
            OSClipboard.GlobalUnlock(hMem);
            OSClipboard.SetClipboardData(13U, hMem);
            OSClipboard.CloseClipboard();
            return true;
        }
    }
}