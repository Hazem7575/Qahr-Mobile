using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Qahr.Mobile.WindowsAPI
{
    /// <summary>
    /// Raw struct I/O over Kernel32 ReadFile/WriteFile.
    ///
    /// IMPORTANT (.NET 6+ / .NET 10): every call to FileStream.SafeFileHandle re-seeks the OS file
    /// pointer back to the FileStream's own cached position. Since this class never reads/writes
    /// through the FileStream itself, that cached position stays 0, so asking for the handle before
    /// every operation made every Read start at offset 0 and every Write overwrite offset 0 (wiping
    /// player items/spells/mail on the first save). The handle is therefore taken ONCE right after
    /// opening and cached in <see cref="hFile"/>; pFile.SafeFileHandle must never be touched again.
    /// Behaviour is identical on .NET Framework 4.8.
    /// </summary>
    public unsafe class BinaryFile
    {
        private FileStream pFile;
        private SafeFileHandle hFile;
        private byte[] mem;      // [DB-only] in-memory (blob) mode: Read serves bytes from here instead of a file handle
        private int memPos;
        private System.IO.MemoryStream memWrite;  // [DB-only] in-memory WRITE mode: Write appends here (fileless save)

        public BinaryFile(string FileName, FileMode Mode)
        {
            Open(FileName, Mode);
        }
        public BinaryFile()
        {
            pFile = null;
            hFile = null;
        }

        public int Position
        {
            get
            {
                if (!Success)
                    throw new IOException("File isn't open/failed to open previously");
                if (mem != null)
                    return memPos;
                if (memWrite != null)
                    return (int)memWrite.Position;
                return Kernel32.SetFilePointer(hFile, 0, null, SeekOrigin.Current);
            }
            set
            {
                if (!Success)
                    throw new IOException("File isn't open/failed to open previously");
                if (mem != null)
                {
                    memPos = value;
                    return;
                }
                if (memWrite != null)
                {
                    memWrite.Position = value;
                    return;
                }
                int distance = value - Position;
                Kernel32.SetFilePointer(hFile, distance, null, SeekOrigin.Current);
            }
        }
        public bool Success { get { return (pFile != null && hFile != null) || mem != null || memWrite != null; } }

        /// <summary>[DB-only 2026-10-17] Open from an in-memory blob (exact .bin bytes) instead of a file.
        /// Reads serve from the byte[]; identical parse to a real file. Read-only (no Write in this mode).</summary>
        public bool OpenBytes(byte[] data)
        {
            Close();
            if (data == null) return false;
            mem = data;
            memPos = 0;
            return true;
        }

        /// <summary>[DB-only 2026-10-17] Open an in-memory WRITE buffer (fileless save). Writes append here;
        /// call GetWrittenBytes() after the writer runs to get the exact bytes (identical to the .bin file).</summary>
        public bool OpenMemoryWrite()
        {
            Close();
            memWrite = new System.IO.MemoryStream();
            return true;
        }

        /// <summary>The bytes written in memory-write mode, or null if not in that mode.</summary>
        public byte[] GetWrittenBytes()
        {
            return memWrite != null ? memWrite.ToArray() : null;
        }

        /// <summary>[DB-only 2026-10-17] Optional hook: given a .bin path, returns the exact bytes from the DB
        /// (player_blobs) or null. Set once at startup by PlayerBinaryStorage. When set, reads prefer the DB
        /// blob and fall back to the file, so every binary load is fileless without touching the call sites.</summary>
        public static Func<string, byte[]> BlobResolver;

        public bool Open(string FileName, FileMode Mode)
        {
            // Never leak a previously opened file if Open is called twice on the same instance.
            Close();
            // Reads: prefer the DB blob (exact bytes) when a resolver is registered; fall back to the file.
            if (Mode == FileMode.Open && BlobResolver != null)
            {
                try { byte[] b = BlobResolver(FileName); if (b != null && b.Length > 0 && OpenBytes(b)) return true; }
                catch { }
            }
            try
            {
                pFile = new FileStream(FileName, Mode);
                // Take the handle exactly once (see class remarks). The stream position is 0 here,
                // so the one-time re-seek done by the getter on .NET 6+ is harmless.
                hFile = pFile.SafeFileHandle;
            }
            catch (Exception ex)
            {
                if (pFile != null)
                {
                    try { pFile.Close(); }
                    catch { }
                }
                pFile = null;
                hFile = null;
                // A simply-missing file (e.g. a player with no house yet -> Houses\<uid>.bin does not exist)
                // is normal and must stay quiet. Only log REAL I/O problems (sharing violation, access
                // denied, path errors) - those used to make every later save fail with no trace at all.
                if (!(ex is System.IO.FileNotFoundException) && !(ex is System.IO.DirectoryNotFoundException))
                {
                    try { MyConsole.WriteLine("[BinaryFile] Failed to open '" + FileName + "' (" + Mode + "): " + ex.Message, ConsoleColor.Yellow); }
                    catch { }
                }
            }
            return Success;
        }
        public void Close()
        {
            // Idempotent: safe to call from a finally block even if Open failed or Close ran already.
            // The FileStream owns the handle, so closing the stream releases the handle exactly once.
            mem = null;
            memPos = 0;
            if (memWrite != null) { try { memWrite.Dispose(); } catch { } memWrite = null; }
            FileStream stream = pFile;
            pFile = null;
            hFile = null;
            if (stream != null)
            {
                try { stream.Close(); }
                catch { }
            }
        }
        public bool Reopen(string FileName, FileMode Mode)
        {
            Close();
            return Open(FileName, Mode);
        }
        public bool Read(void* Buffer, int Count, int Size)
        {
            return Read(Buffer, Count * Size);
        }
        /// <summary>Returns false if the read failed or fewer than AmountOfBytes bytes were available (EOF / truncated file).</summary>
        public bool Read(void* Buffer, int AmountOfBytes)
        {
            if (AmountOfBytes < 0)
                throw new ArgumentException("AmountOfBytes");
            if (!Success)
                throw new IOException("File isn't open/failed to open previously");
            if (AmountOfBytes == 0)
                return true;
            if (mem != null)
            {
                if (memPos < 0 || memPos + AmountOfBytes > mem.Length) return false; // EOF / truncated (same as a short file read)
                System.Runtime.InteropServices.Marshal.Copy(mem, memPos, (IntPtr)Buffer, AmountOfBytes);
                memPos += AmountOfBytes;
                return true;
            }
            int read = 0;
            int ok = Kernel32.ReadFile(hFile, (byte*)Buffer, AmountOfBytes, &read, IntPtr.Zero);
            return ok != 0 && read == AmountOfBytes;
        }
        public bool Write(void* Buffer, int Count, int Size)
        {
            return Write(Buffer, Count * Size);
        }
        /// <summary>Returns false if the write failed or fewer than AmountOfBytes bytes were written.</summary>
        public bool Write(void* Buffer, int AmountOfBytes)
        {
            if (AmountOfBytes < 0)
                throw new ArgumentException("AmountOfBytes");
            if (!Success)
                throw new IOException("File isn't open/failed to open previously");
            if (AmountOfBytes == 0)
                return true;
            if (memWrite != null)
            {
                byte[] tmp = new byte[AmountOfBytes];
                System.Runtime.InteropServices.Marshal.Copy((IntPtr)Buffer, tmp, 0, AmountOfBytes);
                memWrite.Write(tmp, 0, AmountOfBytes);
                return true;
            }
            if (mem != null)
                throw new IOException("in-memory BinaryFile is read-only");
            int written = 0;
            int ok = Kernel32.WriteFile(hFile, (byte*)Buffer, AmountOfBytes, &written, IntPtr.Zero);
            return ok != 0 && written == AmountOfBytes;
        }
    }
}
