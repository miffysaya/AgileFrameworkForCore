using System;
using System.Collections;
using System.Text;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Provides Html Parameter Encoding methods.
    /// </summary>
    internal static class HtmlParameterEncoder
    {
        /// <summary>
        /// The value to use when encoding a space for query strings.
        /// </summary>
        private static readonly char[] QueryStringSpace = "%20".ToCharArray();

        /// <summary>
        /// The value to use when encoding a space for form data.
        /// </summary>
        private static readonly char[] FormStringSpace = "+".ToCharArray();

        /// <summary>
        /// The values to output for each character during parameter encoding.
        /// </summary>
        private static Lazy<char[][]> characterValuesLazy = new Lazy<char[][]>(new Func<char[][]>(InitialiseSafeList));

        /// <summary>
        /// The path character safe list.
        /// </summary>
        private static Lazy<char[][]> pathCharacterValuesLazy = new Lazy<char[][]>(new Func<char[][]>(InitialisePathSafeList));

        /// <summary>
        /// Encodes a string for query string encoding and returns the encoded string.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <returns>The URL-encoded text.</returns>
        /// <remarks>URL encoding ensures that all browsers will correctly transmit text in URL strings. 
        /// Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
        /// As a result, these characters must be encoded in &lt;a&gt; tags or in query strings where the strings can be re-sent by a browser 
        /// in a request string.</remarks>
        /// <exception cref="T:System.ArgumentNullException">Thrown if the encoding is null.</exception>
        internal static string QueryStringParameterEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.QueryString);
        }

        /// <summary>
        /// Encodes a string for form URL encoding and returns the encoded string.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <returns>The URL-encoded text.</returns>
        /// <remarks>URL encoding ensures that all browsers will correctly transmit text in URL strings. 
        /// Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
        /// As a result, these characters must be encoded in &lt;a&gt; tags or in query strings where the strings can be re-sent by a browser 
        /// in a request string.</remarks>
        /// <exception cref="T:System.ArgumentNullException">Thrown if the encoding is null.</exception>
        internal static string FormStringParameterEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.HtmlForm);
        }

        /// <summary>
        /// Encodes a string as a URL
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encoding">The encoding context to use.</param>
        /// <returns>The encoded string.</returns>
        internal static string UrlPathEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.QueryString, pathCharacterValuesLazy);
        }

        /// <summary>
        /// Encodes a string for Query String or Form Data encoding.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <returns>The encoded text.</returns>
        private static string FormQueryEncode(string s, Encoding encoding, EncodingType encodingType)
        {
            return FormQueryEncode(s, encoding, encodingType, characterValuesLazy);
        }

        /// <summary>
        /// Encodes a string for Query String or Form Data encoding.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <param name="characterValues">A lazy loaded safelist to use.</param>
        /// <returns>The encoded text.</returns>
        private static string FormQueryEncode(string s, Encoding encoding, EncodingType encodingType, Lazy<char[][]> characterValues)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            byte[] bytes = encoding.GetBytes(s.ToCharArray());
            char[] array = new char[bytes.Length * 3];
            int length = 0;
            char[][] value = characterValues.Value;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (b == 0 || b == 32 || (int)b > value.Length || value[(int)b] != null)
                {
                    char[] array2;
                    if (b == 32)
                    {
                        switch (encodingType)
                        {
                            case EncodingType.QueryString:
                                array2 = QueryStringSpace;
                                break;
                            case EncodingType.HtmlForm:
                                array2 = FormStringSpace;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("encodingType");
                        }
                    }
                    else
                    {
                        array2 = value[(int)b];
                    }
                    for (int j = 0; j < array2.Length; j++)
                    {
                        array[length++] = array2[j];
                    }
                }
                else
                {
                    array[length++] = (char)b;
                }
            }
            return new string(array, 0, length);
        }

        /// <summary>
        /// Initializes the HTML safe list.
        /// </summary>
        /// <returns>Creates the safelist</returns>
        private static char[][] InitialiseSafeList()
        {
            char[][] result = SafeList.Generate(255, new SafeList.GenerateSafeValue(SafeList.PercentThenHexValueGenerator));
            SafeList.PunchSafeList(ref result, UrlParameterSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for URL parameter encoding.
        /// </summary>
        /// <returns>The safe characters for URL parameter encoding.</returns>
        private static IEnumerable UrlParameterSafeList()
        {
            yield return 45;
            yield return 46;
            for (int i = 48; i <= 57; i++)
            {
                yield return i;
            }
            for (int j = 65; j <= 90; j++)
            {
                yield return j;
            }
            yield return 95;
            for (int k = 97; k <= 122; k++)
            {
                yield return k;
            }
            yield return 126;
            yield break;
        }

        /// <summary>
        /// Initializes the Url Path safe list.
        /// </summary>
        /// <returns>A list of characters and their encoded values for URL encoding.</returns>
        private static char[][] InitialisePathSafeList()
        {
            char[][] result = SafeList.Generate(255, new SafeList.GenerateSafeValue(SafeList.PercentThenHexValueGenerator));
            SafeList.PunchSafeList(ref result, UrlPathSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for URL path encoding.
        /// </summary>
        /// <returns>The safe characters for URL path encoding.</returns>
        private static IEnumerable UrlPathSafeList()
        {
            foreach (object current in UrlParameterSafeList())
            {
                yield return current;
            }
            yield return 35;
            yield return 37;
            yield return 47;
            yield return 92;
            yield return 40;
            yield return 41;
            yield break;
        }
    }
}
