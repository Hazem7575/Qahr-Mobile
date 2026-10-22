using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MobileStudio.Core;

/// <summary>A small Markdown → FlowDocument renderer for the analysis reports (headings, lists, tables, code,
/// **bold**, `code`). Theme brushes only; the reports are English, so the document is left-to-right.</summary>
public static class MarkdownDoc
{
    public static FlowDocument Render(string md, int maxLines = 6000)
    {
        var app = Application.Current;
        Brush text = (Brush)app.FindResource("TextBrush"), subtle = (Brush)app.FindResource("SubtleBrush"),
              line = (Brush)app.FindResource("LineBrush"), panel2 = (Brush)app.FindResource("Panel2Brush"), gold = (Brush)app.FindResource("GoldText");
        var mono = (FontFamily)app.FindResource("MonoFont");
        var doc = new FlowDocument
        {
            FontFamily = (FontFamily)app.FindResource("UiFont"), FontSize = 13, Foreground = text, PagePadding = new Thickness(18, 14, 18, 18),
            FlowDirection = FlowDirection.LeftToRight, TextAlignment = TextAlignment.Left, LineHeight = 20,
        };
        var lines = md.Replace("\r", "").Split('\n');
        if (lines.Length > maxLines) lines = lines.Take(maxLines).Append("…").ToArray();
        List listBlock = null;
        bool inCode = false;
        Paragraph code = null;
        for (int i = 0; i < lines.Length; i++)
        {
            string l = lines[i];
            if (l.StartsWith("```"))
            {
                if (!inCode) { code = new Paragraph { FontFamily = mono, FontSize = 12, Background = panel2, Padding = new Thickness(10, 8, 10, 8) }; doc.Blocks.Add(code); }
                inCode = !inCode;
                continue;
            }
            if (inCode) { if (code.Inlines.Count > 0) code.Inlines.Add(new LineBreak()); code.Inlines.Add(new Run(l)); continue; }
            if (l.TrimStart().StartsWith("|") && i + 1 < lines.Length && Regex.IsMatch(lines[i + 1], @"^\s*\|[\s:|-]+\|\s*$"))
            {
                var t = new Table { CellSpacing = 0, BorderBrush = line, BorderThickness = new Thickness(1), Margin = new Thickness(0, 6, 0, 10) };
                var head = Cells(l);
                foreach (var _ in head) t.Columns.Add(new TableColumn());
                var group = new TableRowGroup();
                group.Rows.Add(Row(head, true));
                i += 2;
                while (i < lines.Length && lines[i].TrimStart().StartsWith("|")) group.Rows.Add(Row(Cells(lines[i++]), false));
                i--;
                t.RowGroups.Add(group);
                doc.Blocks.Add(t);
                listBlock = null;
                continue;
            }
            var h = Regex.Match(l, @"^(#{1,4})\s+(.*)$");
            if (h.Success)
            {
                int lv = h.Groups[1].Length;
                var p = new Paragraph { FontSize = lv == 1 ? 20 : lv == 2 ? 16 : 14, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, lv == 1 ? 0 : 14, 0, 6) };
                Inline(p, h.Groups[2].Value);
                doc.Blocks.Add(p);
                listBlock = null;
                continue;
            }
            var b = Regex.Match(l, @"^(\s*)[-*]\s+(.*)$");
            if (b.Success)
            {
                if (listBlock == null) { listBlock = new List { MarkerStyle = TextMarkerStyle.Disc, Margin = new Thickness(0, 2, 0, 6), Padding = new Thickness(18, 0, 0, 0) }; doc.Blocks.Add(listBlock); }
                var p = new Paragraph { Margin = new Thickness(0, 1, 0, 1) };
                Inline(p, b.Groups[2].Value);
                listBlock.ListItems.Add(new ListItem(p));
                continue;
            }
            if (string.IsNullOrWhiteSpace(l)) { listBlock = null; continue; }
            if (l.StartsWith("  ") && listBlock != null && listBlock.ListItems.LastListItem?.Blocks.LastBlock is Paragraph lp)
            {
                lp.Inlines.Add(new LineBreak());
                lp.Inlines.Add(new Run(l.Trim()) { Foreground = subtle, FontSize = 12 });
                continue;
            }
            var para = new Paragraph { Margin = new Thickness(0, 2, 0, 6) };
            Inline(para, l);
            doc.Blocks.Add(para);
            listBlock = null;
        }
        return doc;

        TableRow Row(List<string> cells, bool header)
        {
            var r = new TableRow();
            if (header) r.Background = panel2;
            foreach (var c in cells)
            {
                var p = new Paragraph { Margin = new Thickness(0), FontSize = 12 };
                if (header) p.FontWeight = FontWeights.SemiBold;
                Inline(p, c);
                r.Cells.Add(new TableCell(p) { BorderBrush = line, BorderThickness = new Thickness(0, 0, 1, 1), Padding = new Thickness(8, 4, 8, 4) });
            }
            return r;
        }

        void Inline(Paragraph p, string s)
        {
            foreach (Match m in Regex.Matches(s, @"(\*\*[^*]+\*\*|`[^`]+`|[^*`]+|[*`])"))
            {
                string v = m.Value;
                if (v.StartsWith("**") && v.EndsWith("**") && v.Length > 4) p.Inlines.Add(new Run(v[2..^2]) { FontWeight = FontWeights.SemiBold, Foreground = gold });
                else if (v.StartsWith("`") && v.EndsWith("`") && v.Length > 2) p.Inlines.Add(new Run(v[1..^1]) { FontFamily = mono, FontSize = 12, Background = panel2 });
                else p.Inlines.Add(new Run(v));
            }
        }
    }

    private static List<string> Cells(string row)
    {
        string t = row.Trim();
        if (t.StartsWith("|")) t = t[1..];
        if (t.EndsWith("|")) t = t[..^1];
        return t.Split('|').Select(c => c.Trim()).ToList();
    }
}
