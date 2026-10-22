using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.WindowsAPI
{
    public unsafe class IniFile
    {
        public string FileName;
        public IniFile(string _FileName, bool overridep = false)
        {
            if (!overridep)
                FileName = Program.ServerConfig.DbLocation + _FileName;
            else
                FileName = _FileName;
        }
        public IniFile()
        {
            FileName = null;
        }
        public const int
            Int32_Size = 15,
            Int16_Size = 9,
            Int8_Size = 6,
            Bool_Size = 6,
            Double_Size = 20,
            Int64_Size = 22,
            Float_Size = 10;

        public static Func<string, int> ToInt32 = new Func<string, int>(int.Parse);
        public static Func<string, uint> ToUInt32 = new Func<string, uint>(uint.Parse);
        public static Func<string, short> ToInt16 = new Func<string, short>(short.Parse);
        public static Func<string, ushort> ToUInt16 = new Func<string, ushort>(ushort.Parse);
        public static Func<string, sbyte> ToInt8 = new Func<string, sbyte>(sbyte.Parse);
        public static Func<string, byte> ToUInt8 = new Func<string, byte>(byte.Parse);
        public static Func<string, bool> ToBool = new Func<string, bool>(bool.Parse);
        public static Func<string, double> ToDouble = new Func<string, double>(double.Parse);
        public static Func<string, long> ToInt64 = new Func<string, long>(long.Parse);
        public static Func<string, ulong> ToUInt64 = new Func<string, ulong>(ulong.Parse);
        public static Func<string, float> ToFloat = new Func<string, float>(float.Parse);

        // ===== [Storage] read overlay =====
        // مصدر بديل لقراءة ملف معيّن (مثلًا صف اللاعب من MySQL وقت LoadCharacter). لو مفيش overlay للملف ده
        // (العادي)، القراءة ماشية في نفس المسار القديم بالظبط. VerifyOnly = القيمة بتيجي من الملف زي الأول، والـ
        // overlay بيتقارن بيها (بعد التحويل للنوع) وأي اختلاف بيتبلّغ.
        public interface IReadOverlay
        {
            bool TryGet(string section, string key, out string value);
            void OnMismatch(string section, string key, string fileValue, string overlayValue);
            // the file was written for this key while the overlay is active -> stop serving it (the file is newer)
            void OnWrite(string section, string key);
        }
        private sealed class OverlayEntry { public IReadOverlay Src; public bool VerifyOnly; }
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, OverlayEntry> overlays =
            new System.Collections.Concurrent.ConcurrentDictionary<string, OverlayEntry>(StringComparer.OrdinalIgnoreCase);
        private static int overlayCount;

        private static string OverlayKey(string fileName)
        {
            try { return System.IO.Path.GetFullPath(fileName); } catch { return fileName; }
        }
        public static void SetOverlay(string fileName, IReadOverlay src, bool verifyOnly)
        {
            if (fileName == null || src == null) return;
            if (overlays.TryAdd(OverlayKey(fileName), new OverlayEntry { Src = src, VerifyOnly = verifyOnly }))
                System.Threading.Interlocked.Increment(ref overlayCount);
            else
                overlays[OverlayKey(fileName)] = new OverlayEntry { Src = src, VerifyOnly = verifyOnly };
        }
        public static void ClearOverlay(string fileName)
        {
            OverlayEntry e;
            if (fileName != null && overlays.TryRemove(OverlayKey(fileName), out e))
                System.Threading.Interlocked.Decrement(ref overlayCount);
        }
        private OverlayEntry GetOverlay()
        {
            if (System.Threading.Volatile.Read(ref overlayCount) == 0 || FileName == null) return null;
            OverlayEntry e;
            return overlays.TryGetValue(OverlayKey(FileName), out e) ? e : null;
        }
        // GetPrivateProfileString بيرجّع أقصى Size-1 حرف
        private static string Clip(string v, int size)
        {
            if (v == null) return "";
            return size > 0 && v.Length > size - 1 ? v.Substring(0, Math.Max(0, size - 1)) : v;
        }
        private T ParseOverlay<T>(string v, T Default, Func<string, T> callback)
        {
            try
            {
                // القاعدة بتخزّن bool كـ 1/0 (Coerce)، والملف True/False
                if ((object)callback == (object)ToBool)
                {
                    if (v == "1") return (T)(object)true;
                    if (v == "0") return (T)(object)false;
                }
                return callback.Invoke(v);
            }
            catch { return Default; }
        }

        private string RawRead(string Section, string Key, string Default, int Size)
        {
            string cached;
            if (IniCache.TryRead(FileName, Section, Key, Default, Size, out cached))
                return cached;
            char* lpBuffer = stackalloc char[Size];
            Kernel32.GetPrivateProfileStringW(Section, Key, Default, lpBuffer, Size, FileName);
            return new string(lpBuffer).Trim('\0');
        }

        public string ReadString(string Section, string Key, string Default, int Size)
        {
            OverlayEntry ov = GetOverlay();
            string dbv;
            if (ov != null && ov.Src.TryGet(Section, Key, out dbv))
            {
                dbv = Clip(dbv, Size);
                if (!ov.VerifyOnly) return dbv;
                string fv = RawRead(Section, Key, Default, Size);
                if (!string.Equals(fv, dbv, StringComparison.Ordinal))
                    ov.Src.OnMismatch(Section, Key, fv, dbv);
                return fv;
            }
            return RawRead(Section, Key, Default, Size);
        }
        public string ReadString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255);
        }
        public string ReadBigString(string Section, string Key, string Default)
        {
            return ReadString(Section, Key, Default, 255 * 18);
        }
        public void ReadString(string Section, string Key, void* Default, void* Buffer, int Size)
        {
            Kernel32.GetPrivateProfileStringA(Section, Key, Default, (sbyte*)Buffer, Size, FileName);
        }

        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback)
        {
            return ReadValue<T>(Section, Key, Default, callback, 255);
        }
        public T ReadValue<T>(string Section, string Key, T Default, Func<string, T> callback, int BufferSize)
        {
            OverlayEntry ov = GetOverlay();
            string dbv;
            if (ov != null && ov.Src.TryGet(Section, Key, out dbv))
            {
                T dbT = ParseOverlay(Clip(dbv, BufferSize), Default, callback);
                if (!ov.VerifyOnly) return dbT;
                T fileT;
                try { fileT = callback.Invoke(RawRead(Section, Key, Default.ToString(), BufferSize)); }
                catch { fileT = Default; }
                if (!EqualityComparer<T>.Default.Equals(fileT, dbT))
                    ov.Src.OnMismatch(Section, Key, Convert.ToString(fileT, System.Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(dbT, System.Globalization.CultureInfo.InvariantCulture) + " (db raw '" + dbv + "')");
                return fileT;
            }
            try
            {
                return callback.Invoke(RawRead(Section, Key, Default.ToString(), BufferSize));
            }
            catch
            {
                return Default;
            }
        }

        public int ReadInt32(string Section, string Key, int Default)
        {
            return ReadValue<int>(Section, Key, Default, ToInt32, Int32_Size);
        }
        public ulong ReadUInt64(string Section, string Key, ulong Default)
        {
            return ReadValue<ulong>(Section, Key, Default, ToUInt64, Int64_Size);
        }
        public long ReadInt64(string Section, string Key, long Default)
        {
            return ReadValue<long>(Section, Key, Default, ToInt64, Int64_Size);
        }
        public double ReadDouble(string Section, string Key, double Default)
        {
            return ReadValue<double>(Section, Key, Default, ToDouble, Double_Size);
        }
        public uint ReadUInt32(string Section, string Key, uint Default)
        {
            return ReadValue<uint>(Section, Key, Default, ToUInt32, Int32_Size);
        }
        public short ReadInt16(string Section, string Key, short Default)
        {
            return ReadValue<short>(Section, Key, Default, ToInt16, Int16_Size);
        }
        public ushort ReadUInt16(string Section, string Key, ushort Default)
        {
            return ReadValue<ushort>(Section, Key, Default, ToUInt16, Int16_Size);
        }
        public sbyte ReadSByte(string Section, string Key, sbyte Default)
        {
            return ReadValue<sbyte>(Section, Key, Default, ToInt8, Int8_Size);
        }
        public byte ReadByte(string Section, string Key, byte Default)
        {
            return ReadValue<byte>(Section, Key, Default, ToUInt8, Int8_Size);
        }
        public bool ReadBool(string Section, string Key, bool Default)
        {
            return ReadValue<bool>(Section, Key, Default, ToBool, Bool_Size);
        }
        public float ReadFloat(string Section, string Key, float Default)
        {
            return ReadValue<float>(Section, Key, Default, ToFloat, Float_Size);
        }

        /// <summary>
        /// Central guard against INI injection. WritePrivateProfileString writes the value verbatim, so a
        /// value containing CR/LF (or a NUL) could inject extra key=value lines or close the section and
        /// forge others (e.g. ConquerPoints, Money, Level). Player-controlled strings reach here through
        /// name/description/signature, so every control character is stripped before it is written.
        /// </summary>
        private static string SanitizeIni(string value)
        {
            if (value == null)
                return null;
            if (value.IndexOf('\r') < 0 && value.IndexOf('\n') < 0 && value.IndexOf('\0') < 0)
                return value;
            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (c == '\r' || c == '\n' || c == '\0')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        public void WriteString(string Section, string Key, string Value)
        {
            if (Value == null)
            {
                Value = "0";
            }
            WriteRaw(SanitizeIni(Section), SanitizeIni(Key), SanitizeIni(Value));
        }
        public void Write<T>(string Section, string Key, T Value)
        {
            WriteRaw(SanitizeIni(Section), SanitizeIni(Key), SanitizeIni(Value.ToString()));
        }

        /// <summary>Every WriteString/Write&lt;T&gt; ends here, already sanitized.</summary>
        private void WriteRaw(string section, string key, string value)
        {
            OverlayEntry ov = GetOverlay();
            if (ov != null) ov.Src.OnWrite(section, key);
            // null section/key/value = "delete" in the profile API; never batched, done right away.
            if (batch != null && section != null && key != null && value != null)
            {
                batch.Add(section, key, value);
                return;
            }
            Kernel32.WritePrivateProfileString(section, key, value, FileName);
            IniCache.Invalidate(FileName);
        }

        #region Atomic batch (SaveClient)
        // WritePrivateProfileString rewrites the WHOLE file on every call: ~270 rewrites per player save,
        // and a crash in the middle leaves a half-old/half-new file. In batch mode the writes of THIS
        // instance are collected in memory and Commit() applies them all at once: re-read the file as it
        // is NOW (other instances may have written keys meanwhile), patch only these keys, write a .tmp
        // in the same folder and swap it in with File.Replace. Switch: ServerConfig.ini [Storage] AtomicIni.

        /// <summary>
        /// Encoding the profile API uses for a non-Unicode .ini: Kernel32.WritePrivateProfileString is
        /// CharSet.Unicode (= ...W) and the W API converts to the system ANSI code page (CP_ACP) unless the
        /// file starts with a UTF-16LE BOM. GetACP() is exactly that code page (the SYSTEM locale - not
        /// CultureInfo.CurrentCulture.TextInfo.ANSICodePage, which follows the user's format culture and can
        /// differ). On .NET Framework this equals Encoding.Default. On .NET Core/.NET 10 Encoding.Default is
        /// UTF-8 (wrong here!) and GetEncoding(1252/1256/...) needs
        /// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance) once at startup.
        /// </summary>
        public static Encoding FileEncoding
        {
            get
            {
                if (fileEncoding == null)
                {
                    Encoding e;
                    try { e = Encoding.GetEncoding((int)GetACP()); }
                    catch { e = Encoding.Default; }// .NET Framework: Encoding.Default == CP_ACP
                    fileEncoding = e;
                }
                return fileEncoding;
            }
        }
        private static Encoding fileEncoding;
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetACP();

        private static int atomicSetting = -1;
        /// <summary>ServerConfig.ini [Storage] AtomicIni (default 1), read once.</summary>
        public static bool AtomicEnabled
        {
            get
            {
                if (atomicSetting < 0)
                {
                    int v = 1;
                    try { v = SharedConfig.Int("Storage", "AtomicIni", 1); } catch { }
                    atomicSetting = v != 0 ? 1 : 0;
                }
                return atomicSetting == 1;
            }
        }

        private sealed class PendingWrites
        {
            // section -> (key -> value), insertion order kept (only matters for brand-new keys/sections)
            public readonly List<string> Sections = new List<string>();
            public readonly Dictionary<string, List<KeyValuePair<string, string>>> Keys =
                new Dictionary<string, List<KeyValuePair<string, string>>>(StringComparer.OrdinalIgnoreCase);
            public int Count;
            public void Add(string section, string key, string value)
            {
                string s = section.Trim(), k = key.Trim();
                List<KeyValuePair<string, string>> list;
                if (!Keys.TryGetValue(s, out list))
                {
                    list = new List<KeyValuePair<string, string>>();
                    Keys.Add(s, list);
                    Sections.Add(s);
                }
                for (int i = 0; i < list.Count; i++)
                    if (string.Equals(list[i].Key, k, StringComparison.OrdinalIgnoreCase))
                    {
                        list[i] = new KeyValuePair<string, string>(list[i].Key, value);
                        return;
                    }
                list.Add(new KeyValuePair<string, string>(k, value));
                Count++;
            }
        }
        private PendingWrites batch;

        public bool InBatch { get { return batch != null; } }

        /// <summary>Start collecting writes of this instance (no-op when AtomicIni=0).</summary>
        public void BeginBatch()
        {
            if (batch == null && FileName != null && AtomicEnabled)
                batch = new PendingWrites();
        }

        /// <summary>Apply the collected writes in one atomic file swap. Safe to call twice; never throws.</summary>
        public void Commit()
        {
            PendingWrites pending = batch;
            batch = null;
            if (pending == null || pending.Count == 0)
                return;
            try
            {
                string path = IniCache.FullPath(FileName);
                lock (CommitLocks[(StringComparer.OrdinalIgnoreCase.GetHashCode(path) & 0x7fffffff) % CommitLocks.Length])
                {
                    CommitToDisk(path, pending);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[IniFile] atomic commit failed for " + FileName + " (" + e.Message + ") - writing key by key", ConsoleColor.Yellow);
                try
                {
                    foreach (string s in pending.Sections)
                        foreach (var kv in pending.Keys[s])
                            Kernel32.WritePrivateProfileString(s, kv.Key, kv.Value, FileName);
                }
                catch (Exception e2) { MyConsole.WriteLine("[IniFile] fallback write failed: " + e2.Message, ConsoleColor.Red); }
            }
            IniCache.Invalidate(FileName);
        }

        private static readonly object[] CommitLocks = CreateLocks(64);
        private static object[] CreateLocks(int n)
        {
            object[] a = new object[n];
            for (int i = 0; i < n; i++) a[i] = new object();
            return a;
        }

        /// <summary>One line of the file: Text = decoded line without CR/LF; Raw = original bytes (incl. terminator) or null if (re)built.</summary>
        private sealed class IniLine
        {
            public string Text;
            public byte[] Raw;
        }

        private static void CommitToDisk(string path, PendingWrites pending)
        {
            byte[] bytes = System.IO.File.Exists(path) ? System.IO.File.ReadAllBytes(path) : new byte[0];
            bool unicode = bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE;
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                throw new NotSupportedException("big-endian UTF-16 ini");
            Encoding enc = unicode ? Encoding.Unicode : FileEncoding;

            // ---- split into lines, keeping the untouched ones byte-for-byte ----
            var lines = new List<IniLine>();
            if (unicode)
            {
                string text = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
                int pos = 0;
                while (pos < text.Length)
                {
                    int end = text.IndexOf('\n', pos);
                    string seg = end < 0 ? text.Substring(pos) : text.Substring(pos, end + 1 - pos);
                    pos = end < 0 ? text.Length : end + 1;
                    lines.Add(new IniLine { Text = seg.TrimEnd('\r', '\n'), Raw = Encoding.Unicode.GetBytes(seg) });
                }
            }
            else
            {
                // 0x0A is never a DBCS trail byte, so splitting the raw bytes is safe for every ANSI code page.
                int pos = 0;
                while (pos < bytes.Length)
                {
                    int end = Array.IndexOf<byte>(bytes, 0x0A, pos);
                    int stop = end < 0 ? bytes.Length : end + 1;
                    byte[] raw = new byte[stop - pos];
                    Buffer.BlockCopy(bytes, pos, raw, 0, raw.Length);
                    pos = stop;
                    lines.Add(new IniLine { Text = enc.GetString(raw).TrimEnd('\r', '\n'), Raw = raw });
                }
            }

            // ---- apply each pending section ----
            foreach (string section in pending.Sections)
            {
                var keys = pending.Keys[section];
                int header = -1;
                for (int i = 0; i < lines.Count; i++)
                {
                    string name;
                    if (IsSection(lines[i].Text, out name) && string.Equals(name, section, StringComparison.OrdinalIgnoreCase))
                    { header = i; break; }// like the API: the first matching section wins
                }
                if (header < 0)
                {
                    EnsureTerminated(lines, enc, unicode);
                    lines.Add(new IniLine { Text = "[" + section + "]" });
                    foreach (var kv in keys)
                        lines.Add(new IniLine { Text = kv.Key + "=" + kv.Value });
                    continue;
                }
                int sectionEnd = lines.Count;// first line after the section body
                for (int i = header + 1; i < lines.Count; i++)
                {
                    string dummy;
                    if (IsSection(lines[i].Text, out dummy)) { sectionEnd = i; break; }
                }
                var added = new List<IniLine>();
                foreach (var kv in keys)
                {
                    int found = -1;
                    for (int i = header + 1; i < sectionEnd; i++)
                    {
                        string t = lines[i].Text;
                        if (t.Trim().Length == 0)
                            continue;
                        int eq = t.IndexOf('=');
                        string k = (eq < 0 ? t : t.Substring(0, eq)).Trim();
                        if (string.Equals(k, kv.Key, StringComparison.OrdinalIgnoreCase))
                        { found = i; break; }
                    }
                    if (found >= 0)
                    {
                        string t = lines[found].Text;
                        int eq = t.IndexOf('=');
                        string keyText = (eq < 0 ? t : t.Substring(0, eq)).Trim();// keep the file's own spelling
                        lines[found] = new IniLine { Text = keyText + "=" + kv.Value };
                    }
                    else
                        added.Add(new IniLine { Text = kv.Key + "=" + kv.Value });
                }
                if (added.Count > 0)
                {
                    int insertAt = sectionEnd;// after the last non-blank line of the section
                    while (insertAt - 1 > header && lines[insertAt - 1].Text.Trim().Length == 0)
                        insertAt--;
                    IniLine prev = lines[insertAt - 1];
                    if (prev.Raw != null && !EndsWithNewLine(prev.Raw, unicode))
                        prev.Raw = null;// rebuilt with CRLF
                    lines.InsertRange(insertAt, added);
                }
            }

            // ---- write .tmp next to the file, then swap ----
            var ms = new System.IO.MemoryStream(bytes.Length + 1024);
            if (unicode) { ms.WriteByte(0xFF); ms.WriteByte(0xFE); }
            byte[] crlf = enc.GetBytes("\r\n");
            for (int i = 0; i < lines.Count; i++)
            {
                IniLine l = lines[i];
                if (l.Raw != null)
                    ms.Write(l.Raw, 0, l.Raw.Length);
                else
                {
                    byte[] b = enc.GetBytes(l.Text);
                    ms.Write(b, 0, b.Length);
                    ms.Write(crlf, 0, crlf.Length);
                }
            }
            string tmp = path + "." + System.Threading.Thread.CurrentThread.ManagedThreadId + ".tmp";
            using (var fs = new System.IO.FileStream(tmp, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                ms.WriteTo(fs);
                fs.Flush(true);
            }
            try
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Replace(tmp, path, null, true);
                else
                    System.IO.File.Move(tmp, path);
            }
            catch
            {
                try { System.IO.File.Delete(tmp); } catch { }
                throw;
            }
        }

        private static bool IsSection(string text, out string name)
        {
            name = null;
            string t = text.TrimStart();
            if (t.Length == 0 || t[0] != '[')
                return false;
            int close = t.IndexOf(']');
            name = (close < 0 ? t.Substring(1) : t.Substring(1, close - 1)).Trim();
            return true;
        }

        private static bool EndsWithNewLine(byte[] raw, bool unicode)
        {
            if (unicode)
                return raw.Length >= 2 && raw[raw.Length - 2] == 0x0A && raw[raw.Length - 1] == 0;
            return raw.Length >= 1 && raw[raw.Length - 1] == 0x0A;
        }

        private static void EnsureTerminated(List<IniLine> lines, Encoding enc, bool unicode)
        {
            if (lines.Count == 0)
                return;
            IniLine last = lines[lines.Count - 1];
            if (last.Raw != null && !EndsWithNewLine(last.Raw, unicode))
                last.Raw = null;
        }
        #endregion
        public void WriteStruct(string Section, string Key, void* lpStruct, int Size)
        {
            Kernel32.WritePrivateProfileStructW(Section, Key, lpStruct, Size, FileName);
            IniCache.Invalidate(FileName);
        }

        public string[] GetSectionNames(int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionNamesW(lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSection(string Section, int BufferSize)
        {
            char* lpBuffer = stackalloc char[BufferSize];
            int Size = Kernel32.GetPrivateProfileSectionW(Section, lpBuffer, BufferSize, FileName);
            if (Size == 0)
                return new string[0];
            return new string(lpBuffer, 0, Size - 1).Split('\0');
        }
        public string[] GetSectionNames()
        {
            return GetSectionNames(4096);
        }
        public string[] GetSection(string Section)
        {
            return GetSection(Section, 4096);
        }
        public bool SectionExists(string Section)
        {
            char* temp = stackalloc char[Section.Length + 1];
            int r = Kernel32.GetPrivateProfileSectionW(Section, temp, Section.Length + 1, FileName);
            return (temp[0] != 0);
        }
        public bool KeyExists(string Section, string Key)
        {
            const char not_used = (char)0x007F;
            const string not_used_s = "\007F";

            char* temp = stackalloc char[2];
            uint r = Kernel32.GetPrivateProfileStringW(Section, Key, not_used_s, temp, 2, FileName);
            return (r == 0 && temp[0] == 0) || (r > 0 && temp[0] != not_used);
        }


    }
    public static class IniCache
    {
        sealed class Entry
        {
            public long Length;
            public DateTime Time;
            public Dictionary<string, Dictionary<string, string>> Sections;
            public long CheckedAt;// [FastBoot 2026-10-15] trustClock ms of the last size/time check (0 = never)
        }

        // [FastBoot 2026-10-15] Boot only (Server.Initialize, before any socket is open): a file whose size + write time
        // were checked less than TrustMs ago is not stat'ed again (the boot reads ~40k keys, each one did a FileInfo
        // stat). IniFile writers still Invalidate() the entry, so the server's own writes are seen at once; only an
        // outside edit made in the same 100 ms is picked up on the next check instead. Off = the old check on every read.
        static volatile bool trustRecent;
        const long TrustMs = 100;
        static readonly System.Diagnostics.Stopwatch trustClock = System.Diagnostics.Stopwatch.StartNew();// [FastBoot 2026-10-15] net48-safe (no TickCount64)
        public static bool TrustRecent { get { return trustRecent; } set { trustRecent = value; } }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Entry> files =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        static volatile bool active;
        static readonly string windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static int Hits, Parsed, Fallbacks;

        public static bool Active { get { return active; } }

        public static void Begin()
        {
            files.Clear();
            Hits = Parsed = Fallbacks = 0;
            active = true;
        }

        public static void End()
        {
            trustRecent = false;// [FastBoot 2026-10-15]
            active = false;
            files.Clear();
        }

        public static void Invalidate(string fileName)
        {
            if (!active || fileName == null)
                return;
            Entry removed;
            try { files.TryRemove(FullPath(fileName), out removed); }
            catch { files.Clear(); }
        }

        internal static string FullPath(string fileName)
        {
            // Like the profile API: a bare file name lives in the Windows folder, anything with a
            // folder part is resolved from the current directory.
            if (fileName.IndexOf('\\') < 0 && fileName.IndexOf('/') < 0)
                return System.IO.Path.Combine(windowsDir, fileName);
            return System.IO.Path.GetFullPath(fileName);
        }

        /// <summary>False = not handled (the caller asks Win32).</summary>
        public static bool TryRead(string fileName, string section, string key, string def, int size, out string result)
        {
            result = null;
            if (!active || fileName == null || section == null || key == null || size <= 0)
                return false;
            try
            {
                string path = FullPath(fileName);
                Dictionary<string, Dictionary<string, string>> sections = null;
                Entry recent;// [FastBoot 2026-10-15] boot: reuse an entry checked < TrustMs ago without a new stat
                long now = trustRecent ? trustClock.ElapsedMilliseconds : 0;
                if (trustRecent && files.TryGetValue(path, out recent) && now - System.Threading.Volatile.Read(ref recent.CheckedAt) < TrustMs)
                    sections = recent.Sections;
                else
                {
                var info = new System.IO.FileInfo(path);
                if (info.Exists)
                {
                    Entry entry;
                    if (!files.TryGetValue(path, out entry) || entry.Length != info.Length || entry.Time != info.LastWriteTimeUtc)
                    {
                        entry = new Entry { Length = info.Length, Time = info.LastWriteTimeUtc, Sections = Parse(System.IO.File.ReadAllBytes(path)) };
                        if (trustRecent) entry.CheckedAt = now;
                        files[path] = entry;
                        System.Threading.Interlocked.Increment(ref Parsed);
                    }
                    else if (trustRecent)
                        System.Threading.Volatile.Write(ref entry.CheckedAt, now);
                    sections = entry.Sections;
                }
                }
                string value = null;
                Dictionary<string, string> keys;
                if (sections != null && sections.TryGetValue(section.Trim(), out keys))
                    keys.TryGetValue(key.Trim(), out value);
                if (value == null)
                    value = def == null ? "" : def.TrimEnd(' ');
                if (value.Length > size - 1)
                    value = value.Substring(0, size - 1);
                int nul = value.IndexOf('\0');
                if (nul >= 0)
                    value = value.Substring(0, nul);
                result = value.Trim('\0');
                System.Threading.Interlocked.Increment(ref Hits);
                return true;
            }
            catch
            {
                System.Threading.Interlocked.Increment(ref Fallbacks);
                return false;
            }
        }

        static Dictionary<string, Dictionary<string, string>> Parse(byte[] bytes)
        {
            string text;
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                text = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            else
                text = IniFile.FileEncoding.GetString(bytes); // [net10] was Encoding.Default (= CP_ACP on 4.8, UTF-8 on .NET 10)
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> current = null;
            int pos = 0;
            while (pos < text.Length)
            {
                int end = text.IndexOf('\n', pos);
                if (end < 0)
                    end = text.Length;
                string line = text.Substring(pos, end - pos).Trim();
                pos = end + 1;
                if (line.Length == 0)
                    continue;
                if (line[0] == '[')
                {
                    int close = line.IndexOf(']');
                    string name = (close < 0 ? line.Substring(1) : line.Substring(1, close - 1)).Trim();
                    if (result.ContainsKey(name))
                        current = null;// a repeated section is never reached by the API
                    else
                    {
                        current = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        result.Add(name, current);
                    }
                    continue;
                }
                if (current == null)
                    continue;
                int eq = line.IndexOf('=');
                string k = eq < 0 ? line : line.Substring(0, eq).Trim();
                if (current.ContainsKey(k))
                    continue;
                string v = eq < 0 ? "" : line.Substring(eq + 1).Trim();
                if (v.Length >= 2 && (v[0] == '"' || v[0] == '\'') && v[v.Length - 1] == v[0])
                    v = v.Substring(1, v.Length - 2);
                current.Add(k, v);
            }
            return result;
        }
    }
}
