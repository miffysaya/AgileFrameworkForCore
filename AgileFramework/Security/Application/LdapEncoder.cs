using System;
using System.Collections;
using System.Text;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Provides LDAP Encoding methods.
    /// </summary>
    internal static class LdapEncoder
    {
        /// <summary>
        /// The values to output for each character when filter encoding.
        /// </summary>
        private static Lazy<char[][]> filterCharacterValuesLazy = new Lazy<char[][]>(new Func<char[][]>(InitialiseFilterSafeList));

        /// <summary>
        /// The values to output for each character when DN encoding.
        /// </summary>
        private static Lazy<char[][]> distinguishedNameCharacterValuesLazy = new Lazy<char[][]>(new Func<char[][]>(InitialiseDistinguishedNameSafeList));

        /// <summary>
        /// Encodes the input string for use in LDAP filters.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>An encoded version of the input string suitable for use in LDAP filters.</returns>
        internal static string FilterEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            char[][] value = filterCharacterValuesLazy.Value;
            byte[] bytes = Encoding.UTF8.GetBytes(input.ToCharArray());
            char[] array = new char[bytes.Length * 3];
            int length = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (value[(int)b] != null)
                {
                    char[] array2 = value[(int)b];
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
        /// Encodes the input string for use in LDAP DNs.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <param name="useInitialCharacterRules">Value indicating whether the special case rules for encoding of spaces and octothorpes at the start of a string are used.</param>
        /// <param name="useFinalCharacterRule">Value indicating whether the special case for encoding of final character spaces is used.</param>
        /// <returns>An encoded version of the input string suitable for use in LDAP DNs.</returns>
        internal static string DistinguishedNameEncode(string input, bool useInitialCharacterRules, bool useFinalCharacterRule)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            char[][] value = distinguishedNameCharacterValuesLazy.Value;
            byte[] bytes = Encoding.UTF8.GetBytes(input.ToCharArray());
            char[] array = new char[bytes.Length * 3];
            int length = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (i == 0 && b == 32 && useInitialCharacterRules)
                {
                    array[length++] = '\\';
                    array[length++] = ' ';
                }
                else if (i == 0 && b == 35 && useInitialCharacterRules)
                {
                    array[length++] = '\\';
                    array[length++] = '#';
                }
                else if (i == bytes.Length - 1 && b == 32 && useFinalCharacterRule)
                {
                    array[length++] = '\\';
                    array[length++] = ' ';
                }
                else if (value[(int)b] != null)
                {
                    char[] array2 = value[(int)b];
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
        /// Initializes the LDAP filter safe list.
        /// </summary>
        /// <returns>The LDAP filter safe list.</returns>
        private static char[][] InitialiseFilterSafeList()
        {
            char[][] result = SafeList.Generate(255, new SafeList.GenerateSafeValue(SafeList.SlashThenHexValueGenerator));
            SafeList.PunchSafeList(ref result, FilterEncodingSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for LDAP filter encoding.
        /// </summary>
        /// <returns>The safe characters for LDAP filter encoding.</returns>
        /// <remarks>See http://tools.ietf.org/html/rfc4515/</remarks>
        private static IEnumerable FilterEncodingSafeList()
        {
            for (int i = 32; i <= 126; i++)
            {
                if (i != 40 && i != 41 && i != 42 && i != 47 && i != 92)
                {
                    yield return i;
                }
            }
            yield break;
        }

        /// <summary>
        /// Initializes the LDAP DN safe lists.
        /// </summary>
        /// <returns>The DN safe list.</returns>
        private static char[][] InitialiseDistinguishedNameSafeList()
        {
            char[][] result = SafeList.Generate(255, new SafeList.GenerateSafeValue(SafeList.HashThenHexValueGenerator));
            SafeList.PunchSafeList(ref result, DistinguishedNameSafeList());
            EscapeDistinguisedNameCharacter(ref result, ',');
            EscapeDistinguisedNameCharacter(ref result, '+');
            EscapeDistinguisedNameCharacter(ref result, '"');
            EscapeDistinguisedNameCharacter(ref result, '\\');
            EscapeDistinguisedNameCharacter(ref result, '<');
            EscapeDistinguisedNameCharacter(ref result, '>');
            EscapeDistinguisedNameCharacter(ref result, ';');
            return result;
        }

        /// <summary>
        /// Provides the safe characters for LDAP filter encoding.
        /// </summary>
        /// <returns>The safe characters for LDAP filter encoding.</returns>
        /// <remarks>See http://www.ietf.org/rfc/rfc2253.txt </remarks>
        private static IEnumerable DistinguishedNameSafeList()
        {
            for (int i = 32; i <= 126; i++)
            {
                if (i != 44 && i != 43 && i != 34 && i != 92 && i != 60 && i != 62 && i != 38 && i != 33 && i != 124 && i != 61 && i != 45 && i != 39 && i != 59)
                {
                    yield return i;
                }
            }
            yield break;
        }

        /// <summary>
        /// Escapes a special DN character.
        /// </summary>
        /// <param name="safeList">The safe list to escape the character within.</param>
        /// <param name="c">The character to escape.</param>
        private static void EscapeDistinguisedNameCharacter(ref char[][] safeList, char c)
        {
            safeList[(int)c] = new char[]
            {
                '\\',
                c
            };
        }
    }
}
