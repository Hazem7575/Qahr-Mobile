namespace Qahr.Mobile.DBFunctionality
{
    using System;
    using System.Text.RegularExpressions;

    public unsafe static class MySqlExtensions
    {
        public static string MySqlEscape(this string usString)
        {
            if (usString == null) return null;
            // SQL Encoding for MySQL Recommended here:
            // http://au.php.net/manual/en/function.mysql-real-escape-string.php
            // it escapes \r, \n, \x00, \x1a, backslash, single quotes, and double quotes.
            // Without this every player-supplied string reaching the query builder is injectable.
            return Regex.Replace(usString, @"[\r\n\x00\x1a\\'""]", @"\$0");
        }
    }
}
