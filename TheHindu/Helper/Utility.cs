using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace TheHindu.Helper
{
    static public class Utility
    {
        public static bool EqualNoCase(this string value, string content)
        {
            return value != null && value.Equals(content, StringComparison.OrdinalIgnoreCase);
        }

        public static string Truncate(this String str, int length, bool ellipsis = false)
        {
            if (String.IsNullOrEmpty(str)) return str ?? String.Empty;
            str = str.Trim();
            if (str.Length <= length) return str ?? String.Empty;
            return ellipsis ? str.Substring(0, length) + "..." : str.Substring(0, length);
        }

        public static string DecodeHtml(string htmltext)
        {
            htmltext = htmltext.Replace("<p>", "").Replace("</p>", "\r\n\r\n");

            var decoded = String.Empty;
            if (htmltext.IndexOf('<') > -1 || htmltext.IndexOf('>') > -1 || htmltext.IndexOf('&') > -1)
            {
                try
                {
                    var document = new HtmlDocument();

                    var decode = document.CreateElement("div");
                    htmltext = htmltext.Replace(".<", ". <").Replace("?<", "? <").Replace("!<", "! <").Replace("&#039;", "'");
                    decode.InnerHtml = htmltext;

                    var allElements = decode.Descendants().ToArray();
                    for (var n = allElements.Length - 1; n >= 0; n--)
                    {
                        if (allElements[n].NodeType == HtmlNodeType.Comment || allElements[n].Name.EqualNoCase("style") || allElements[n].Name.EqualNoCase("script"))
                        {
                            allElements[n].Remove();
                        }
                    }
                    decoded = WebUtility.HtmlDecode(decode.InnerText);
                }
                catch (Exception exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine("App.xaml.cs:" + exception);
                    }
                }
            }
            else
            {
                decoded = htmltext;
            }
            return decoded;
        }
    }
}