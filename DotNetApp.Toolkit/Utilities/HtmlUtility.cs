using System.Net;
using System.Text.RegularExpressions;

namespace DotNetApp.Toolkit.Utilities
{
    public static class HtmlUtility
    {
        #region Constants

        private const string RegexHtmlTagPattern = @"</?(?(?=script|link|title)notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";

        #endregion

        #region Fields

        private static readonly Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        #endregion

        #region Methods

        public static string StripTags(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        public static string SanitizeHtml(string html)
        {
            return HttpUtility.HtmlDecode(Regex.Replace(html, RegexHtmlTagPattern, ""));
        }

        #endregion
    }
}
