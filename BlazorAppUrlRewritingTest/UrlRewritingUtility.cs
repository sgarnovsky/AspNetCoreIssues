using Microsoft.AspNetCore.Rewrite;
using System.Text.RegularExpressions;

namespace BlazorAppUrlRewritingTest
{
    public static class UrlRewritingUtility
    {
        private static Regex StaticFilesCachedPathRegex = new Regex(@"^(/cache/\d{6}_\d{7})(/.+)$",
          RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static void RewriteCachedStaticFilePathRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            var path = request.Path.Value;
            if (path is null || path.Length <= 21 || !path.Contains("/cache/", StringComparison.CurrentCultureIgnoreCase))
                return;

            var m = StaticFilesCachedPathRegex.Match(path);

            if (m.Success)
            {
                context.Result = RuleResult.SkipRemainingRules;
                request.Path = m.Groups[2].Value;
            }
        }
    }
}
