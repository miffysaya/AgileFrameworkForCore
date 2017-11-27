using System;
using System.Collections;
using System.Text;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Provides CSS Encoding methods.
    /// </summary>
    internal static class CssEncoder
    {
        /// <summary>
        /// The values to output for each character.
        /// </summary>
        private static Lazy<char[][]> characterValuesLazy = new Lazy<char[][]>(new Func<char[][]>(InitialiseSafeList));

        /// <summary>
        /// Encodes according to the CSS encoding rules.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        internal static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            char[][] value = characterValuesLazy.Value;
            StringBuilder outputStringBuilder = EncoderUtil.GetOutputStringBuilder(input.Length, 7);
            Utf16StringReader utf16StringReader = new Utf16StringReader(input);
            while (true)
            {
                int num = utf16StringReader.ReadNextScalarValue();
                if (num < 0)
                {
                    break;
                }
                if (num >= value.Length)
                {
                    char[] value2 = SafeList.SlashThenSixDigitHexValueGenerator(num);
                    outputStringBuilder.Append(value2);
                }
                else if (value[num] != null)
                {
                    char[] value3 = value[num];
                    outputStringBuilder.Append(value3);
                }
                else
                {
                    outputStringBuilder.Append((char)num);
                }
            }
            return outputStringBuilder.ToString();
        }

        /// <summary>
        /// Initializes the CSS safe list.
        /// </summary>
        /// <returns>
        /// The CSS safe list.
        /// </returns>
        private static char[][] InitialiseSafeList()
        {
            char[][] result = SafeList.Generate(255, new SafeList.GenerateSafeValue(SafeList.SlashThenSixDigitHexValueGenerator));
            SafeList.PunchSafeList(ref result, CssSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for CS encoding.
        /// </summary>
        /// <returns>The safe characters for CSS encoding.</returns>
        /// <remarks>See http://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet </remarks>
        private static IEnumerable CssSafeList()
        {
            for (int i = 48; i <= 57; i++)
            {
                yield return i;
            }
            for (int j = 65; j <= 90; j++)
            {
                yield return j;
            }
            for (int k = 97; k <= 122; k++)
            {
                yield return k;
            }
            yield break;
        }
    }
}
