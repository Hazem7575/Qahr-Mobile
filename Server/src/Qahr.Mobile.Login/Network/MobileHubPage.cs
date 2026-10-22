using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qahr.Mobile.Login
{
    /// <summary>
    /// QahrMobile: the page behind the phone's "WebHub" button. The game server answers the phone's CMsgInnerWebPage
    /// (B5SystemAcks.cs) with the link of Data\mobile_webpages.txt page 2 = GET /qahr/hub?uid=&lt;character&gt; here.
    /// Content: MySQL table web_hub (one card per row: title, body, image, link, link_text, sort, active), created and
    /// filled with a welcome card on first use; edit the table, the page reads it on every open. Same look and art as the
    /// recharge page (MobileShopPage).
    /// </summary>
    internal static class MobileHubPage
    {
        const string Table = "web_hub";
        static int _ready;

        internal static bool Matches(string requestLine)
        {
            return requestLine.StartsWith("GET /qahr/hub", StringComparison.Ordinal);
        }

        internal static async Task Handle(NetworkStream s, string requestLine, CancellationToken ct)
        {
            string html;
            try { html = Render(); }
            catch (Exception e)
            {
                Console.WriteLine("[HubPage] " + e.Message);
                html = MobileShopPage.Page("<div class='empty'>Please try again later.</div>");
            }
            await MobileShopPage.Send(s, "text/html; charset=utf-8", Encoding.UTF8.GetBytes(html), ct);
        }

        static void EnsureTable()
        {
            if (Interlocked.CompareExchange(ref _ready, 1, 0) != 0) return;
            try
            {
                using (var c = Database.DataHolder.MySqlConnection)
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + Table + " (" +
                            "id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, title VARCHAR(120) NOT NULL, body VARCHAR(2000) NOT NULL DEFAULT '', " +
                            "image VARCHAR(255) NOT NULL DEFAULT '', link VARCHAR(255) NOT NULL DEFAULT '', link_text VARCHAR(60) NOT NULL DEFAULT '', " +
                            "sort INT NOT NULL DEFAULT 0, active TINYINT NOT NULL DEFAULT 1, created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP) CHARSET=utf8mb4";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT COUNT(*) FROM " + Table;
                        if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                        {
                            cmd.CommandText = "INSERT INTO " + Table + " (title,body,image,sort) VALUES " +
                                "('Welcome to Qahr Online','News, events and updates of the server appear here.','pack_780057.png',1)";
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

        static string Render()
        {
            EnsureTable();
            var sb = new StringBuilder("<div class='hub'><div class='hubttl'>Qahr Online</div>");
            int n = 0;
            using (var c = Database.DataHolder.MySqlConnection)
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT title,body,image,link,link_text FROM " + Table + " WHERE active=1 ORDER BY sort,id DESC";
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            n++;
                            string title = r.GetString(0), body = r.GetString(1), image = r.GetString(2), link = r.GetString(3), linkText = r.GetString(4);
                            sb.Append("<div class='news'>");
                            if (image.Length > 0)
                                sb.Append("<div class='nimg' style=\"background-image:url('").Append(MobileShopPage.Esc(MobileShopPage.ImageUrl(image))).Append("')\"></div>");
                            sb.Append("<div class='ntxt'><b>").Append(MobileShopPage.Esc(title)).Append("</b><p>")
                              .Append(MobileShopPage.Esc(body).Replace("\n", "<br>")).Append("</p>");
                            if (link.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || link.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                                sb.Append("<a class='go' href='").Append(MobileShopPage.Esc(link)).Append("'><span>")
                                  .Append(MobileShopPage.Esc(linkText.Length > 0 ? linkText : "Open")).Append("</span></a>");
                            sb.Append("</div></div>");
                        }
                }
            }
            if (n == 0) sb.Append("<div class='news'><div class='ntxt'><p>No news yet.</p></div></div>");
            sb.Append("</div><style>")
              .Append(".hub{width:100%;max-width:980px;max-height:100%;overflow:auto;background:#1e2024e6;border:1px solid #5d5f63;padding:12px 16px;box-shadow:0 6px 24px #000a}")
              .Append(".hubttl{color:var(--gold);font-size:22px;text-align:center;margin:2px 0 12px;letter-spacing:1px}")
              .Append(".news{display:flex;gap:14px;background:linear-gradient(180deg,#3b3d42,#2a2c30);border:1px solid #6a6c70;padding:10px;margin-bottom:10px}")
              .Append(".nimg{flex:0 0 140px;height:96px;background:center/contain no-repeat}")
              .Append(".ntxt{flex:1}.ntxt b{color:#f2f0ea;font-size:17px}.ntxt p{margin:6px 0;color:var(--dim)}")
              .Append(".go{display:inline-block;position:relative;height:40px;min-width:150px;text-decoration:none;background:url('/qahr/shopart/bar_m.png') repeat-x;background-size:auto 100%}")
              .Append(".go span{position:relative;z-index:1;display:block;text-align:center;line-height:40px;color:var(--ink);font-weight:600;padding:0 40px}")
              .Append("body{align-items:flex-start}</style>");
            return MobileShopPage.Page(sb.ToString());
        }
    }
}
