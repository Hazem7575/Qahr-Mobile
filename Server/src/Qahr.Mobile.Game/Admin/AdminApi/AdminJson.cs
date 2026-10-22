using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Qahr.Mobile.AdminApi
{
    /// <summary>
    /// Tiny dependency-free JSON reader/writer for the local admin API (PlayerStudio).
    /// Objects -> Dictionary&lt;string,object&gt; (case-insensitive keys), arrays -> List&lt;object&gt;,
    /// numbers -> JsonNumber (raw text, so 64-bit money values never lose precision), strings, bool, null.
    /// </summary>
    public sealed class JsonNumber
    {
        public readonly string Raw;
        public JsonNumber(string raw) { Raw = raw; }
        public override string ToString() { return Raw; }
    }

    public static class AdminJson
    {
        // ---------------------------------------------------------------- parse
        public static object Parse(string text)
        {
            int i = 0;
            object v = ParseValue(text, ref i, 0);
            SkipWs(text, ref i);
            if (i != text.Length) throw new FormatException("trailing characters at " + i);
            return v;
        }

        private static void SkipWs(string s, ref int i)
        {
            while (i < s.Length && (s[i] == ' ' || s[i] == '\t' || s[i] == '\r' || s[i] == '\n')) i++;
        }

        private static object ParseValue(string s, ref int i, int depth)
        {
            if (depth > 32) throw new FormatException("JSON nested too deep");
            SkipWs(s, ref i);
            if (i >= s.Length) throw new FormatException("unexpected end of JSON");
            char c = s[i];
            if (c == '{')
            {
                var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                i++;
                SkipWs(s, ref i);
                if (i < s.Length && s[i] == '}') { i++; return d; }
                while (true)
                {
                    SkipWs(s, ref i);
                    if (i >= s.Length || s[i] != '"') throw new FormatException("expected property name at " + i);
                    string key = ParseString(s, ref i);
                    SkipWs(s, ref i);
                    if (i >= s.Length || s[i] != ':') throw new FormatException("expected ':' at " + i);
                    i++;
                    d[key] = ParseValue(s, ref i, depth + 1);
                    SkipWs(s, ref i);
                    if (i < s.Length && s[i] == ',') { i++; continue; }
                    if (i < s.Length && s[i] == '}') { i++; return d; }
                    throw new FormatException("expected ',' or '}' at " + i);
                }
            }
            if (c == '[')
            {
                var list = new List<object>();
                i++;
                SkipWs(s, ref i);
                if (i < s.Length && s[i] == ']') { i++; return list; }
                while (true)
                {
                    list.Add(ParseValue(s, ref i, depth + 1));
                    SkipWs(s, ref i);
                    if (i < s.Length && s[i] == ',') { i++; continue; }
                    if (i < s.Length && s[i] == ']') { i++; return list; }
                    throw new FormatException("expected ',' or ']' at " + i);
                }
            }
            if (c == '"') return ParseString(s, ref i);
            if (string.CompareOrdinal(s, i, "true", 0, 4) == 0) { i += 4; return true; }
            if (string.CompareOrdinal(s, i, "false", 0, 5) == 0) { i += 5; return false; }
            if (string.CompareOrdinal(s, i, "null", 0, 4) == 0) { i += 4; return null; }
            int start = i;
            while (i < s.Length && ("+-0123456789.eE".IndexOf(s[i]) >= 0)) i++;
            if (i == start) throw new FormatException("unexpected character '" + c + "' at " + i);
            return new JsonNumber(s.Substring(start, i - start));
        }

        private static string ParseString(string s, ref int i)
        {
            i++; // opening quote
            var sb = new StringBuilder();
            while (i < s.Length)
            {
                char c = s[i++];
                if (c == '"') return sb.ToString();
                if (c != '\\') { sb.Append(c); continue; }
                if (i >= s.Length) break;
                char e = s[i++];
                switch (e)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u':
                        if (i + 4 > s.Length) throw new FormatException("bad \\u escape");
                        sb.Append((char)int.Parse(s.Substring(i, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                        i += 4;
                        break;
                    default: throw new FormatException("bad escape \\" + e);
                }
            }
            throw new FormatException("unterminated string");
        }

        // ---------------------------------------------------------------- write
        public static string Write(object v)
        {
            var sb = new StringBuilder(256);
            WriteValue(sb, v, 0);
            return sb.ToString();
        }

        private static void WriteValue(StringBuilder sb, object v, int depth)
        {
            if (depth > 32) { sb.Append("null"); return; }
            if (v == null) { sb.Append("null"); return; }
            if (v is string) { Quote(sb, (string)v); return; }
            if (v is bool) { sb.Append((bool)v ? "true" : "false"); return; }
            if (v is JsonNumber) { sb.Append(((JsonNumber)v).Raw); return; }
            if (v is Enum) { sb.Append(Convert.ToInt64(v).ToString(CultureInfo.InvariantCulture)); return; }
            if (v is double || v is float || v is decimal)
            {
                double d = Convert.ToDouble(v, CultureInfo.InvariantCulture);
                if (double.IsNaN(d) || double.IsInfinity(d)) sb.Append("0");
                else sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
                return;
            }
            if (v is IConvertible && !(v is char) && !(v is DateTime))
            {
                sb.Append(Convert.ToString(v, CultureInfo.InvariantCulture));
                return;
            }
            if (v is DateTime) { Quote(sb, ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)); return; }
            var dict = v as IDictionary<string, object>;
            if (dict != null)
            {
                sb.Append('{');
                bool first = true;
                foreach (var kv in dict)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    Quote(sb, kv.Key);
                    sb.Append(':');
                    WriteValue(sb, kv.Value, depth + 1);
                }
                sb.Append('}');
                return;
            }
            var en = v as IEnumerable;
            if (en != null)
            {
                sb.Append('[');
                bool first = true;
                foreach (var x in en)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    WriteValue(sb, x, depth + 1);
                }
                sb.Append(']');
                return;
            }
            Quote(sb, v.ToString());
        }

        private static void Quote(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }
    }

    /// <summary>Typed access to a parsed request object.</summary>
    public sealed class AdminArgs
    {
        private readonly Dictionary<string, object> _d;
        public AdminArgs(Dictionary<string, object> d) { _d = d ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); }
        public Dictionary<string, object> Raw { get { return _d; } }

        public bool Has(string key) { object v; return _d.TryGetValue(key, out v) && v != null; }

        public string Str(string key, string def = null)
        {
            object v;
            if (!_d.TryGetValue(key, out v) || v == null) return def;
            return v is string ? (string)v : v.ToString();
        }

        public long Long(string key, long def = 0)
        {
            string s = Str(key);
            if (s == null) return def;
            long r;
            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r)) return r;
            double d;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) return (long)d;
            throw new AdminError("'" + key + "' must be a number");
        }

        public ulong ULong(string key, ulong def = 0)
        {
            string s = Str(key);
            if (s == null) return def;
            ulong r;
            if (ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r)) return r;
            throw new AdminError("'" + key + "' must be a non-negative whole number");
        }

        public bool Bool(string key, bool def = false)
        {
            object v;
            if (!_d.TryGetValue(key, out v) || v == null) return def;
            if (v is bool) return (bool)v;
            string s = v.ToString().Trim().ToLowerInvariant();
            return s == "1" || s == "true" || s == "yes";
        }

        public long Need(string key, long min, long max)
        {
            if (!Has(key)) throw new AdminError("missing '" + key + "'");
            long v = Long(key);
            if (v < min || v > max) throw new AdminError("'" + key + "' must be between " + min + " and " + max);
            return v;
        }

        public List<object> List(string key)
        {
            object v;
            return _d.TryGetValue(key, out v) ? v as List<object> : null;
        }
    }

    /// <summary>An error that is reported back to the caller as {"ok":false,"error":...}.</summary>
    public sealed class AdminError : Exception
    {
        public AdminError(string message) : base(message) { }
    }
}
