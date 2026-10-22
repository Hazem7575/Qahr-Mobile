using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MobileStudio.Core;

/// <summary>The GameServer AdminApi (Admin\AdminApi, 127.0.0.1 only): one JSON line per request, like tools\admin.py.
/// The secret is read from Data\adminapi.txt for each call and never shown, logged or kept in the settings.</summary>
public static class AdminClient
{
    public sealed class Reply
    {
        public bool Ok;
        public string Message = "";
        public JsonObject Json;
        public string Error;
    }

    public static bool Configured => File.Exists(Svc.Settings.AdminApiTxt);

    public static int Port
    {
        get
        {
            var c = ReadCfg();
            return c != null && c.TryGetValue("port", out var p) && int.TryParse(p, out int n) ? n : 5899;
        }
    }

    private static Dictionary<string, string> ReadCfg()
    {
        try
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string raw in File.ReadAllLines(Svc.Settings.AdminApiTxt))
            {
                string l = raw.Trim();
                if (l.Length == 0 || l.StartsWith("#") || !l.Contains('=')) continue;
                int i = l.IndexOf('=');
                d[l[..i].Trim()] = l[(i + 1)..].Trim();
            }
            return d;
        }
        catch { return null; }
    }

    /// <summary>Calls one op. Args are plain values (string / number / bool).</summary>
    public static async Task<Reply> CallAsync(string op, IDictionary<string, object> args = null, int timeoutMs = 15000)
    {
        var cfg = ReadCfg();
        if (cfg == null || !cfg.ContainsKey("secret"))
            return new Reply { Error = ScriptHazem.Kit.Loc.Get("api.noConfig") };
        int port = cfg.TryGetValue("port", out var ps) && int.TryParse(ps, out int pn) ? pn : 5899;
        var req = new JsonObject();
        if (args != null)
            foreach (var kv in args)
                req[kv.Key] = kv.Value switch
                {
                    null => null,
                    int i => i,
                    long l => l,
                    bool b => b,
                    double d => d,
                    _ => JsonValue.Create(kv.Value.ToString()),
                };
        req["secret"] = cfg["secret"];
        req["op"] = op;
        try
        {
            using var c = new TcpClient();
            using var cts = new CancellationTokenSource(timeoutMs);
            await c.ConnectAsync("127.0.0.1", port, cts.Token);
            var s = c.GetStream();
            byte[] b = Encoding.UTF8.GetBytes(req.ToJsonString() + "\n");
            await s.WriteAsync(b, cts.Token);
            var ms = new MemoryStream();
            var buf = new byte[65536];
            while (true)
            {
                int n = await s.ReadAsync(buf, cts.Token);
                if (n <= 0) break;
                ms.Write(buf, 0, n);
                if (buf[n - 1] == (byte)'\n') break;
            }
            var node = JsonNode.Parse(Encoding.UTF8.GetString(ms.ToArray())) as JsonObject;
            var r = new Reply { Json = node };
            r.Ok = node?["ok"]?.GetValue<bool>() == true;
            r.Message = node?["message"]?.ToString() ?? node?["error"]?.ToString() ?? "";
            if (!r.Ok) r.Error = node?["error"]?.ToString() ?? r.Message;
            return r;
        }
        catch (OperationCanceledException) { return new Reply { Error = ScriptHazem.Kit.Loc.Get("api.timeout") }; }
        catch (SocketException) { return new Reply { Error = ScriptHazem.Kit.Loc.Get("api.down") }; }
        catch (Exception ex) { return new Reply { Error = ex.Message }; }
    }

    public static Task<Reply> CallAsync(string op, params (string k, object v)[] args) =>
        CallAsync(op, args.ToDictionary(a => a.k, a => a.v));

    public static int Int(JsonNode n, string key) { try { return n?[key]?.GetValue<int>() ?? 0; } catch { try { return (int)(n?[key]?.GetValue<long>() ?? 0); } catch { return 0; } } }
    public static long Long(JsonNode n, string key) { try { return n?[key]?.GetValue<long>() ?? 0; } catch { try { return (long)(n?[key]?.GetValue<double>() ?? 0); } catch { return 0; } } }
    public static string Str(JsonNode n, string key) => n?[key]?.ToString() ?? "";
    public static bool Bool(JsonNode n, string key) { try { return n?[key]?.GetValue<bool>() ?? false; } catch { return false; } }
}
