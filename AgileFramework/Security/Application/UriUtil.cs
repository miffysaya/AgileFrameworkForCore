using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Contains helpers for URI parsing 
    /// </summary>
    internal static class UriUtil
    {
        /// <summary>
        /// Query Fragment separators.
        /// </summary>
        private static readonly char[] QueryFragmentSeparators = new char[]
        {
            '?',
            '#'
        };

        /// <summary>
        /// Extracts the query string and fragment from the input path by splitting on the separator characters.
        /// Doesn't perform any validation as to whether the input represents a valid URL.
        /// Concatenating the pieces back together will form the original input string.
        /// </summary>
        /// <param name="input">The URL to split.</param>
        /// <param name="path">The path portion of <paramref name="input" />.</param>
        /// <param name="queryAndFragment">The query and fragment of <paramref name="input" />.</param>
        internal static void ExtractQueryAndFragment(string input, out string path, out string queryAndFragment)
        {
            int num = input.IndexOfAny(QueryFragmentSeparators);
            if (num != -1)
            {
                path = input.Substring(0, num);
                queryAndFragment = input.Substring(num);
                return;
            }
            path = input;
            queryAndFragment = null;
        }

        /// <summary>
        /// Returns a value indicating whether the schemes used in <paramref name="url" /> is generally considered safe for the purposes of redirects or other places where URLs are rendered to the page.
        /// </summary>
        /// <param name="url">The URL to parse</param>
        /// <returns>true if the scheme is considered safe, otherwise false.</returns>
        internal static bool IsSafeScheme(string url)
        {
            return url.IndexOf(":", StringComparison.Ordinal) == -1 || url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("file:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("news:", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Attempts to split a URI into its constituent pieces.
        /// Even if this method returns true, one or more of the out parameters might contain a null or empty string, e.g. if there is no query / fragment.
        /// Concatenating the pieces back together will form the original input string.
        /// </summary>
        /// <param name="input">The input URI to split.</param>
        /// <param name="schemeAndAuthority">The scheme and authority used in the <paramref name="input" /> uri.</param>
        /// <param name="path">The path contained in the <paramref name="input" /> uri.</param>
        /// <param name="queryAndFragment">The query and fragment contained in the <paramref name="input" /> uri.</param>
        /// <returns>true if the URI could be split, otherwise false.</returns>
        internal static bool TrySplitUriForPathEncode(string input, out string schemeAndAuthority, out string path, out string queryAndFragment)
        {
            string text;
            ExtractQueryAndFragment(input, out text, out queryAndFragment);
            Uri uri;
            if (IsSafeScheme(text) && Uri.TryCreate(text, UriKind.Absolute, out uri))
            {
                string authority = uri.Authority;
                if (!string.IsNullOrEmpty(authority))
                {
                    int num = text.IndexOf(authority, StringComparison.Ordinal);
                    if (num != -1)
                    {
                        int num2 = num + authority.Length;
                        schemeAndAuthority = text.Substring(0, num2);
                        path = text.Substring(num2);
                        return true;
                    }
                }
            }
            schemeAndAuthority = null;
            path = null;
            queryAndFragment = null;
            return false;
        }
    }
}
