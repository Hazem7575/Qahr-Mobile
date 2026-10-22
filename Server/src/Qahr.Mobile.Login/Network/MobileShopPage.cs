using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: the in-game recharge page. The phone's Mall "Credit" tab calls ChannelPayment; our client patch
    /// (java QahrPay) opens a full-screen WebView on
    ///   GET /qahr/shop?product=&lt;id of ini\iap.ini&gt;&amp;price=&lt;cents&gt;&amp;count=&lt;n&gt;&amp;role=&lt;uid&gt;&amp;server=&lt;id&gt;&amp;account=&lt;acc&gt;
    /// on the MobileRegister listener. It shows only the tapped product: art, details, payment methods, price.
    /// MySQL tables (created and filled on first use, edit them freely):
    ///   recharge_packs   product_id, title, price, currency, image, cps, bonus, item_id, item_count, sort, active, details
    ///   recharge_methods id, name, details, sort, active
    /// Art: web\shop\*.png|jpg next to the exe (GET /qahr/shopart/&lt;file&gt;), cut from the phone's Mall; a product
    /// shows web\shop\pack_&lt;product_id&gt;.png when it exists. Payment itself is not wired yet.
    /// </summary>
    internal static class MobileShopPage
    {
        const string Packs = "recharge_packs", Methods = "recharge_methods";
        static int _ready;

        internal static bool Matches(string requestLine)
        {
            return requestLine.StartsWith("GET /qahr/shop", StringComparison.Ordinal);
        }

        internal static async Task Handle(NetworkStream s, string requestLine, CancellationToken ct)
        {
            string target = requestLine.Split(' ')[1];
            if (target.StartsWith("/qahr/shopart/", StringComparison.Ordinal))
            {
                await Art(s, target.Substring(14), ct);
                return;
            }
            var q = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int qm = target.IndexOf('?');
            if (qm >= 0)
                foreach (string kv in target.Substring(qm + 1).Split('&'))
                {
                    int eq = kv.IndexOf('=');
                    if (eq > 0) q[WebUtility.UrlDecode(kv.Substring(0, eq))] = WebUtility.UrlDecode(kv.Substring(eq + 1));
                }
            string html;
            try { html = Render(q); }
            catch (Exception e)
            {
                Console.WriteLine("[ShopPage] " + e.Message);
                html = Page("<div class='empty'>The shop is not available right now. Please try again later.</div>");
            }
            await Send(s, "text/html; charset=utf-8", Encoding.UTF8.GetBytes(html), ct);
        }

        static async Task Send(NetworkStream s, string type, byte[] body, CancellationToken ct, string status = "200 OK", string cache = "no-store")
        {
            byte[] head = Encoding.ASCII.GetBytes("HTTP/1.1 " + status + "\r\nContent-Type: " + type + "\r\nCache-Control: " + cache + "\r\nContent-Length: "
                + body.Length + "\r\nConnection: close\r\n\r\n");
            await s.WriteAsync(head, 0, head.Length, ct);
            await s.WriteAsync(body, 0, body.Length, ct);
        }

        // ------------------------------------------------------------------------------------------------ art
        static string ArtDir { get { return Path.Combine(AppContext.BaseDirectory, "web", "shop"); } }

        static async Task Art(NetworkStream s, string name, CancellationToken ct)
        {
            int qm = name.IndexOf('?');
            if (qm >= 0) name = name.Substring(0, qm);
            string ext = Path.GetExtension(name).ToLowerInvariant();
            bool ok = name.Length > 0 && name.Length < 80 && name.IndexOfAny(new[] { '/', '\\', ':' }) < 0 && !name.Contains("..")
                      && (ext == ".png" || ext == ".jpg") && File.Exists(Path.Combine(ArtDir, name));
            if (!ok) { await Send(s, "text/plain", Encoding.ASCII.GetBytes("not found"), ct, "404 Not Found"); return; }
            await Send(s, ext == ".png" ? "image/png" : "image/jpeg", File.ReadAllBytes(Path.Combine(ArtDir, name)), ct, "200 OK", "max-age=86400");
        }

        static bool HasArt(string name) { return File.Exists(Path.Combine(ArtDir, name)); }

        // ------------------------------------------------------------------------------------------------ data
        sealed class Pack { public string Product, Title, Details, Currency, Image; public decimal Price; public int Cps, Bonus; public uint ItemId, ItemCount; }
        sealed class Method { public int Id; public string Name, Details; }

        static void EnsureTables()
        {
            if (Interlocked.CompareExchange(ref _ready, 1, 0) != 0) return;
            try
            {
                using (var c = Database.DataHolder.MySqlConnection)
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + Packs + " (" +
                            "product_id VARCHAR(64) NOT NULL PRIMARY KEY, title VARCHAR(64) NOT NULL, " +
                            "price DECIMAL(12,2) NOT NULL DEFAULT 0, currency VARCHAR(16) NOT NULL DEFAULT '$', image VARCHAR(255) NOT NULL DEFAULT '', " +
                            "cps INT NOT NULL DEFAULT 0, bonus INT NOT NULL DEFAULT 0, item_id INT UNSIGNED NOT NULL DEFAULT 0, item_count INT UNSIGNED NOT NULL DEFAULT 0, " +
                            "sort INT NOT NULL DEFAULT 0, active TINYINT NOT NULL DEFAULT 1, details VARCHAR(1000) NOT NULL DEFAULT '') CHARSET=utf8mb4";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT COUNT(*) FROM " + Packs;
                        if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                        {
                            // the 17 products of the phone's ini\iap.ini (id = what ChannelPayment sends)
                            cmd.CommandText = "INSERT INTO " + Packs + " (product_id,title,cps,price,image,sort) VALUES " +
                                "('7830013','60 CPs',60,0.99,'pack_7830013.png',1),('780022','320 CPs',320,4.99,'pack_780022.png',2),('780000','530 CPs',530,7.99,'pack_780000.png',3)," +
                                "('780001','1075 CPs',1075,15.99,'pack_780001.png',4),('780023','2050 CPs',2050,29.99,'pack_780023.png',5),('780057','4200 CPs',4200,59.99,'pack_780057.png',6)," +
                                "('782111','Basic Monthly Card',0,14.99,'',7),('782117','Deluxe Monthly Card',0,59.99,'',8),('782123','Exclusive Monthly Card',0,59.99,'',9)," +
                                "('782138','Advanced Weekly Card',0,7.99,'',10),('782139','Supreme Weekly Card',0,15.99,'',11),('782140','Bundle',0,19.99,'',12)," +
                                "('782141','Bundle - Advanced Weekly Card',0,3.99,'',13),('782142','Bundle - Supreme Weekly Card',0,11.99,'',14)," +
                                "('782158','Supreme Contract',0,39.99,'',15),('780135','$29.99 Return Token',0,29.99,'',16),('780147','$29.99 Awakening Token',0,29.99,'',17)";
                            cmd.ExecuteNonQuery();
                        }
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + Methods + " (" +
                            "id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, name VARCHAR(64) NOT NULL, details VARCHAR(200) NOT NULL DEFAULT '', " +
                            "sort INT NOT NULL DEFAULT 0, active TINYINT NOT NULL DEFAULT 1) CHARSET=utf8mb4";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT COUNT(*) FROM " + Methods;
                        if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                        {
                            cmd.CommandText = "INSERT INTO " + Methods + " (name,details,sort) VALUES " +
                                "('Vodafone Cash','Mobile wallet',1),('InstaPay','Bank transfer',2),('Credit / Debit Card','Visa, Mastercard',3)";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {
                _ready = 0;
                throw;
            }
        }

        static Pack FindPack(string product)
        {
            using (var c = Database.DataHolder.MySqlConnection)
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT product_id,title,price,currency,image,cps,bonus,item_id,item_count,details FROM " + Packs + " WHERE product_id=@p AND active=1";
                    cmd.Parameters.AddWithValue("@p", product);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read())
                            return new Pack
                            {
                                Product = r.GetString(0), Title = r.GetString(1), Price = r.GetDecimal(2), Currency = r.GetString(3), Image = r.GetString(4),
                                Cps = r.GetInt32(5), Bonus = r.GetInt32(6), ItemId = r.GetUInt32(7), ItemCount = r.GetUInt32(8), Details = r.GetString(9)
                            };
                }
            }
            return null;
        }

        static List<Method> LoadMethods()
        {
            var list = new List<Method>();
            using (var c = Database.DataHolder.MySqlConnection)
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT id,name,details FROM " + Methods + " WHERE active=1 ORDER BY sort,id";
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            list.Add(new Method { Id = r.GetInt32(0), Name = r.GetString(1), Details = r.GetString(2) });
                }
            }
            return list;
        }

        // ------------------------------------------------------------------------------------------------ page
        static string Render(Dictionary<string, string> q)
        {
            EnsureTables();
            string product = q.TryGetValue("product", out var p) ? p : "";
            Pack pack = product.Length > 0 && product.Length <= 64 ? FindPack(product) : null;
            if (pack == null)
                return Page("<div class='empty'>This item is not on sale right now.</div>");

            string art = ImageUrl(pack.Image);
            string details = pack.Details.Length > 0 ? Esc(pack.Details)
                : pack.Cps > 0 ? "Adds <b>" + pack.Cps.ToString("N0") + " CPs</b> to your character right after the payment is confirmed."
                : "Delivered to your character right after the payment is confirmed.";
            string role = q.TryGetValue("role", out var r0) ? r0 : "", acc = q.TryGetValue("account", out var a0) ? a0 : "";

            var sb = new StringBuilder();
            sb.Append("<div class='wrap'>")
              // left: the product the way the Mall draws it
              .Append("<div class='card'><div class='ttl'>").Append(Esc(pack.Title)).Append("</div>")
              .Append("<div class='pic' style=\"background-image:url('").Append(Esc(art)).Append("')\"></div>")
              .Append("<div class='bar'><span>").Append(Money(pack)).Append("</span></div></div>")
              // right: details, payment methods, pay
              .Append("<div class='side'><div class='sec'>Item details</div><div class='box'>")
              .Append("<div class='row'><i>Item</i><b>").Append(Esc(pack.Title)).Append("</b></div>")
              .Append("<div class='row'><i>Item No.</i><b>#").Append(Esc(pack.Product)).Append("</b></div>");
            if (pack.Cps > 0)
            {
                sb.Append("<div class='row'><i>You get</i><b class='cp'><img src='/qahr/shopart/cp.png'>").Append(pack.Cps.ToString("N0"));
                if (pack.Bonus > 0) sb.Append(" <em>+").Append(pack.Bonus.ToString("N0")).Append("</em>");
                sb.Append("</b></div>");
            }
            if (pack.ItemId > 0)
                sb.Append("<div class='row'><i>Item reward</i><b>").Append(Math.Max(1u, pack.ItemCount)).Append(" &times; #").Append(pack.ItemId).Append("</b></div>");
            if (role.Length > 0)
                sb.Append("<div class='row'><i>Character</i><b>").Append(Esc(role)).Append(acc.Length > 0 ? " &middot; " + Esc(acc) : "").Append("</b></div>");
            sb.Append("<p class='note'>").Append(details).Append("</p></div>")
              .Append("<div class='sec'>Payment method</div><div class='methods'>");
            foreach (var m in LoadMethods())
                sb.Append("<label class='m'><input type='radio' name='m' value='").Append(m.Id).Append("'><span><b>").Append(Esc(m.Name))
                  .Append("</b><small>").Append(Esc(m.Details)).Append("</small></span></label>");
            sb.Append("</div><button class='pay' id='pay' disabled><span>Pay ").Append(Money(pack)).Append("</span></button>")
              .Append("<div class='msg' id='msg'></div></div></div>")
              .Append("<script>var b=document.getElementById('pay'),g=document.getElementById('msg');")
              .Append("document.querySelectorAll('input[name=m]').forEach(function(i){i.onchange=function(){b.disabled=false;g.textContent='';};});")
              .Append("b.onclick=function(){g.textContent='Online payment is being set up. Please check back soon.';};</script>");
            return Page(sb.ToString());
        }

        // a one-character symbol goes in front ($9.99), a code or word after (150.00 EGP, 150.00 ج.م)
        static string Money(Pack p)
        {
            string amount = p.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            string cur = (p.Currency ?? "").Trim();
            if (cur.Length == 0) return Esc(amount);
            return Esc(cur.Length == 1 ? cur + amount : amount + " " + cur);
        }

        // image column: a full http(s) link, a file of web\shop (pack_780000.png), or empty = the big chest
        static string ImageUrl(string image)
        {
            image = (image ?? "").Trim();
            if (image.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || image.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return image;
            if (image.Length > 0 && image.IndexOfAny(new[] { '/', '\\', ':' }) < 0 && HasArt(image))
                return "/qahr/shopart/" + image;
            return "/qahr/shopart/pack_780057.png";
        }

        static string Esc(string s) { return WebUtility.HtmlEncode(s ?? ""); }

        // the phone Mall: blurred shelf background, grey card with a title strip, gold price bar with corner ornaments
        static string Page(string content)
        {
            return "<!doctype html><html><head><meta charset='utf-8'>" +
                "<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1,user-scalable=no'><title>Credit</title><style>" +
                ":root{--gold:#d9c48a;--gold2:#c2ad73;--ink:#2b2418;--txt:#e9e4d8;--dim:#9a958c;--card1:#5b5d62;--card2:#2e3034;--line:#8a8270}" +
                "*{box-sizing:border-box;-webkit-tap-highlight-color:transparent}" +
                "html,body{margin:0;height:100%}" +
                "body{background:#15171b url('/qahr/shopart/bg.jpg') center/cover fixed;color:var(--txt);" +
                "font:15px/1.35 'Noto Sans','Roboto','Segoe UI',sans-serif;display:flex;align-items:center;justify-content:center;padding:14px}" +
                ".wrap{display:flex;gap:18px;width:100%;max-width:980px;align-items:stretch}" +
                ".card{flex:0 0 42%;display:flex;flex-direction:column;background:linear-gradient(180deg,var(--card1),var(--card2) 85%);" +
                "border:1px solid #777b80;box-shadow:0 6px 24px #000a}" +
                ".ttl{text-align:center;font-size:22px;padding:10px 0 8px;color:#f2f0ea;border-bottom:1px solid #ffffff22;text-shadow:0 1px 2px #000}" +
                ".pic{flex:1;min-height:200px;background:center/contain no-repeat}" +
                ".bar{position:relative;height:56px;background:url('/qahr/shopart/bar_m.png') repeat-x;background-size:auto 100%}" +
                ".bar:before,.bar:after{content:'';position:absolute;top:0;width:50px;height:100%;background:url('/qahr/shopart/bar_l.png') 0 0/100% 100%}" +
                ".bar:before{left:0}.bar:after{right:0;background-image:url('/qahr/shopart/bar_r.png')}" +
                ".bar span{position:relative;z-index:1;display:block;text-align:center;line-height:56px;font-size:26px;color:var(--ink);font-weight:600}" +
                ".side{flex:1;display:flex;flex-direction:column;background:#1e2024e6;border:1px solid #5d5f63;padding:14px 16px;box-shadow:0 6px 24px #000a}" +
                ".sec{color:var(--gold);font-size:14px;letter-spacing:1px;text-transform:uppercase;margin:2px 0 8px;display:flex;align-items:center;gap:8px}" +
                ".sec:after{content:'';flex:1;height:1px;background:linear-gradient(90deg,var(--line),transparent)}" +
                ".box{background:#00000040;border:1px solid #ffffff14;padding:8px 12px;margin-bottom:12px}" +
                ".row{display:flex;justify-content:space-between;padding:4px 0;border-bottom:1px dashed #ffffff12}.row:last-of-type{border:0}" +
                ".row i{font-style:normal;color:var(--dim)}.row b{font-weight:600}.cp{display:flex;align-items:center;gap:4px}.cp img{height:18px}" +
                ".cp em{font-style:normal;color:#8fdc7a}.note{margin:6px 0 2px;color:var(--dim);font-size:13px}" +
                ".methods{display:grid;grid-template-columns:repeat(auto-fit,minmax(150px,1fr));gap:8px;margin-bottom:12px}" +
                ".m input{display:none}.m span{display:block;padding:9px 12px;border:1px solid #6a6c70;background:linear-gradient(#3b3d42,#2a2c30);cursor:pointer}" +
                ".m b{display:block}.m small{color:var(--dim)}.m input:checked+span{border-color:var(--gold);box-shadow:inset 0 0 0 1px var(--gold),0 0 10px #d9c48a40}" +
                ".pay{margin-top:auto;height:56px;border:0;padding:0;cursor:pointer;position:relative;background:url('/qahr/shopart/bar_m.png') repeat-x;background-size:auto 100%}" +
                ".pay:before,.pay:after{content:'';position:absolute;top:0;width:50px;height:100%;background:url('/qahr/shopart/bar_l.png') 0 0/100% 100%}" +
                ".pay:before{left:0}.pay:after{right:0;background-image:url('/qahr/shopart/bar_r.png')}" +
                ".pay span{position:relative;z-index:1;font:600 22px 'Noto Sans','Roboto',sans-serif;color:var(--ink)}.pay:disabled{filter:grayscale(.8) brightness(.7)}" +
                ".msg{min-height:20px;margin-top:8px;text-align:center;color:var(--gold);font-size:13px}" +
                ".empty{background:#1e2024e6;border:1px solid #5d5f63;padding:30px 40px;font-size:18px}" +
                "@media (max-width:640px){.wrap{flex-direction:column}.card{flex:none}.pic{min-height:180px}}" +
                ".x{position:fixed;top:10px;right:12px;width:46px;height:46px;background:url('/qahr/shopart/close.png') center/contain no-repeat;z-index:9}" +
                "</style></head><body><a class='x' href='#' onclick=\"try{Qahr.close()}catch(e){history.back()}return false\"></a>" + content + "</body></html>";
        }
    }
}
