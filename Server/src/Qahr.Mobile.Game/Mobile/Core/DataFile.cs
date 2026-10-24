using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: a file in Data\ that is parsed once and parsed again when it changes on disk, so data edits
    /// apply without a restart. The file date is checked at most every <see cref="CheckSeconds"/> seconds.
    /// A broken file is logged and the last good value is kept; a missing file gives <c>parse(empty)</c>.
    /// <code>static readonly DataFile&lt;Config&gt; File = new DataFile&lt;Config&gt;("mobile_x.txt", "X", Config.Parse);
    /// var cfg = File.Value;</code>
    /// </summary>
    public sealed class DataFile<T> where T : class
    {
        public const int CheckSeconds = 2;

        readonly string _name, _tag;
        readonly Func<string[], T> _parse;
        readonly Action<T> _loaded;
        readonly object _sync = new object();
        T _value;
        DateTime _stamp = DateTime.MinValue, _nextCheck = DateTime.MinValue;

        /// <param name="name">file name inside Data\</param>
        /// <param name="tag">console tag for errors, e.g. "Hud"</param>
        /// <param name="parse">raw lines -> value (DataFile.Clean strips comments)</param>
        /// <param name="loaded">optional, runs after every successful (re)load, outside the lock (e.g. a check report)</param>
        public DataFile(string name, string tag, Func<string[], T> parse, Action<T> loaded = null)
        {
            _name = name;
            _tag = tag;
            _parse = parse;
            _loaded = loaded;
        }

        public string FullPath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", _name); }
        }

        public T Value
        {
            get
            {
                Refresh();
                return _value;
            }
        }

        void Refresh()
        {
            DateTime now = DateTime.UtcNow;
            if (_value != null && now < _nextCheck)
                return;
            T loaded = null;
            lock (_sync)
            {
                if (_value != null && now < _nextCheck)
                    return;
                _nextCheck = now.AddSeconds(CheckSeconds);
                string path = FullPath;
                DateTime stamp = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                if (_value != null && stamp == _stamp)
                    return;
                try
                {
                    _value = loaded = _parse(stamp == DateTime.MinValue ? new string[0] : File.ReadAllLines(path));
                    _stamp = stamp;
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[" + _tag + "] " + _name + " could not be read, keeping the previous values: " + e.Message, ConsoleColor.Red);
                    if (_value == null)
                        _value = _parse(new string[0]);
                    _stamp = stamp;
                }
            }
            if (loaded != null && _loaded != null)
            {
                try { _loaded(loaded); }
                catch (Exception e) { MyConsole.WriteLine("[" + _tag + "] after loading " + _name + ": " + e.Message, ConsoleColor.Red); }
            }
        }
    }

    public static class DataFile
    {
        /// <summary>The lines without "# comments", trimmed, empty lines skipped.</summary>
        public static IEnumerable<string> Clean(string[] lines)
        {
            foreach (var raw in lines)
            {
                int hash = raw.IndexOf('#');
                string line = (hash >= 0 ? raw.Substring(0, hash) : raw).Trim();
                if (line.Length > 0)
                    yield return line;
            }
        }

        /// <summary>Splits a clean line on spaces and tabs.</summary>
        public static string[] Words(string line)
        {
            return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
