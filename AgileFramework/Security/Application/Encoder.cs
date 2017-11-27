using System;
using System.Globalization;
using System.Text;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Performs encoding of input strings to provide protection against
    /// Cross-Site Scripting (XSS) attacks and LDAP injection attacks in 
    /// various contexts.
    /// </summary>
    /// <remarks>
    /// This encoding library uses the Principle of Inclusions, 
    /// sometimes referred to as "safe-listing" to provide protection 
    /// against injection attacks.  With safe-listing protection, 
    /// algorithms look for valid inputs and automatically treat 
    /// everything outside that set as a potential attack.  This library 
    /// can be used as a defense in depth approach with other mitigation 
    /// techniques. It is suitable for applications with high security 
    /// requirements.
    /// </remarks>
    public static class Encoder
    {
        /// <summary>
        /// Empty string for Visual Basic Script context
        /// </summary>
        private const string VbScriptEmptyString = "\"\"";

        /// <summary>
        /// Empty string for Java Script context
        /// </summary>
        private const string JavaScriptEmptyString = "''";

        /// <summary>
        /// Initializes character Html encoding array
        /// </summary>
        private static readonly char[][] SafeListCodes = Encoder.InitializeSafeList();

        /// <summary>
        /// Encodes input strings for use as a value  in Lightweight Directory Access Protocol (LDAP) filter queries.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use as a value in LDAP filter queries.</returns>
        /// <remarks>
        /// This method encodes all but known safe characters defined in the safe list.
        /// <newpara />
        /// RFC 4515 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in \XX format where XX is the hex representation of the character.
        /// <newpara />
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>Parens R Us (for all your parenthetical needs)</term><description>Parens R Us \28for all your parenthetical needs\29</description></item>
        /// <item><term>*</term><description>\2A</description></item>
        /// <item><term>C:\MyFile</term><description>C:\5CMyFile</description></item>
        /// <item><term>NULLNULLNULLEOT (binary)</term><description>\00\00\00\04</description></item>
        /// <item><term>Lučić</term><description>Lu\C4\8Di\C4\87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapFilterEncode(string input)
        {
            return LdapEncoder.FilterEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use as a value in Lightweight Directory Access Protocol (LDAP) DNs.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use as a value in LDAP DNs.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara />
        /// RFC 2253 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in #XX format where XX is the hex representation of the character or a 
        /// specific \ escape format.
        /// <newpara />
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description>\ hello</description></item>
        /// <item><term>hello </term><description>hello \ </description></item>
        /// <item><term>#hello</term><description>\#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapDistinguishedNameEncode(string input)
        {
            return Encoder.LdapDistinguishedNameEncode(input, true, true);
        }

        /// <summary>
        /// Encodes input strings for use as a value in Lightweight Directory Access Protocol (LDAP) DNs.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="useInitialCharacterRules">Value indicating whether the special case rules for encoding of spaces and octothorpes at the start of a string are used.</param>
        /// <param name="useFinalCharacterRule">Value indicating whether the special case for encoding of final character spaces is used.</param>
        /// <returns>Encoded string for use as a value in LDAP DNs.</returns>\
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara />
        /// RFC 2253 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in #XX format where XX is the hex representation of the character or a 
        /// specific \ escape format.
        /// <newpara />
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description>\ hello</description></item>
        /// <item><term>hello </term><description>hello\ </description></item>
        /// <item><term>#hello</term><description>\#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// If useInitialCharacterRules is set to false then escaping of the initial space or octothorpe characters is not performed;
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description> hello</description></item>
        /// <item><term>hello </term><description>hello\ </description></item>
        /// <item><term>#hello</term><description>#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// If useFinalCharacterRule is set to false then escaping of a space at the end of a string is not performed;
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description> hello</description></item>
        /// <item><term>hello </term><description>hello </description></item>
        /// <item><term>#hello</term><description>#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapDistinguishedNameEncode(string input, bool useInitialCharacterRules, bool useFinalCharacterRule)
        {
            return LdapEncoder.DistinguishedNameEncode(input, useInitialCharacterRules, useFinalCharacterRule);
        }

        /// <summary>
        /// Encodes input strings to be used as a value in Lightweight Directory Access Protocol (LDAP) search queries.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use in LDAP search queries.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara />
        /// RFC 4515 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in \XX format where XX is the hex representation of the character.
        /// <newpara />
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>Parens R Us (for all your parenthetical needs)</term><description>Parens R Us \28for all your parenthetical needs\29</description></item>
        /// <item><term>*</term><description>\2A</description></item>
        /// <item><term>C:\MyFile</term><description>C:\5CMyFile</description></item>
        /// <item><term>NULLNULLNULLEOT (binary)</term><description>\00\00\00\04</description></item>
        /// <item><term>Lučić</term><description>Lu\C4\8Di\C4\87</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.LdapFilterEncode() instead.")]
        public static string LdapEncode(string input)
        {
            return Encoder.LdapFilterEncode(input);
        }

        /// <summary>
        /// Encodes the specified string for use in Cascading Style Sheet (CSS) attributes. The return value from this function is expected to be
        /// used in building an attribute string. CSS string attributes should be quoted values.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use in CSS element values.</returns>
        /// <remarks>This method encodes all characters except those that are in the safe list.
        /// <newpara />
        /// The following table lists the default safe characters.
        /// <list type="table">
        /// <listheader><term>Unicode Code Chart</term><term>Characters(s)</term><description>Description</description></listheader>
        /// <item><term>C0 Controls and Basic Latin</term><term>A-Z</term><description>Uppercase alphabetic letters</description>&gt;</item>
        /// <item><term>C0 Controls and Basic Latin</term><term>a-z</term><description>Lowercase alphabetic letters</description>&gt;</item>
        /// <item><term>C0 Controls and Basic Latin</term><term>0-9</term><description>Numbers</description>&gt;</item>
        /// </list>
        /// <newpara />
        /// The CSS character escape sequence consists of a backslash character (\) followed by up to six hexadecimal digits that represent a character code from the ISO 10646 standard. 
        /// (The ISO 10646 standard is effectively equivalent to Unicode.) Any character other than a hexadecimal digit terminates the escape sequence. If a character that follows the 
        /// escape sequence is also a valid hexadecimal digit, it must either include six digits in the escape sequence or use a whitespace character to terminate the escape sequence. 
        /// For example, \000020 denotes a space.
        /// </remarks>
        public static string CssEncode(string input)
        {
            return CssEncoder.Encode(input);
        }

        /// <summary>
        /// Encodes input strings for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in HTML.
        /// </returns>
        /// <remarks>
        /// All characters not safe listed are encoded to their Unicode decimal value, using &amp;#DECIMAL; notation.
        /// The default safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>'</term><description>Apostrophe</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="M:Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts,Microsoft.Security.Application.LowerMidCodeCharts,Microsoft.Security.Application.MidCodeCharts,Microsoft.Security.Application.UpperMidCodeCharts,Microsoft.Security.Application.UpperCodeCharts)" />.
        /// <newpara />
        /// Example inputs and their related encoded outputs:
        /// <list type="table">
        /// <item><term>&lt;script&gt;alert('XSS Attack!');&lt;/script&gt;</term><description>&amp;lt;script&amp;gt;alert('XSS Attack!');&amp;lt;/script&amp;gt;</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// <item><term>"Anti-Cross Site Scripting Library"</term><description>&amp;quote;Anti-Cross Site Scripting Library&amp;quote;</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlEncode(string input)
        {
            return Encoder.HtmlEncode(input, false);
        }

        /// <summary>
        /// Encodes input strings for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="useNamedEntities">Value indicating if the HTML 4.0 named entities should be used.</param>
        /// <returns>
        /// Encoded string for use in HTML.
        /// </returns>
        /// <remarks>
        /// All characters not safe listed are encoded to their Unicode decimal value, using &amp;#DECIMAL; notation.
        /// If you choose to use named entities then if a character is an HTML4.0 named entity the named entity will be used.
        /// The default safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>'</term><description>Apostrophe</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="M:Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts,Microsoft.Security.Application.LowerMidCodeCharts,Microsoft.Security.Application.MidCodeCharts,Microsoft.Security.Application.UpperMidCodeCharts,Microsoft.Security.Application.UpperCodeCharts)" />.
        /// <newpara />
        /// Example inputs and their related encoded outputs:
        /// <list type="table">
        /// <item><term>&lt;script&gt;alert('XSS Attack!');&lt;/script&gt;</term><description>&amp;lt;script&amp;gt;alert('XSS Attack!');&amp;lt;/script&amp;gt;</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// <item><term>"Anti-Cross Site Scripting Library"</term><description>&amp;quote;Anti-Cross Site Scripting Library&amp;quote;</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlEncode(string input, bool useNamedEntities)
        {
            return UnicodeCharacterEncoder.HtmlEncode(input, useNamedEntities);
        }

        /// <summary>
        /// Encodes an input string for use in an HTML attribute.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>The input string encoded for use in an HTML attribute.</returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using  &amp;#DECIMAL; notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="M:Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts,Microsoft.Security.Application.LowerMidCodeCharts,Microsoft.Security.Application.MidCodeCharts,Microsoft.Security.Application.UpperMidCodeCharts,Microsoft.Security.Application.UpperCodeCharts)" />.
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;#39;XSS&amp;#32;Attack!&amp;#39;);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&amp;#32;Site&amp;#32;Scripting&amp;#32;Library</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlAttributeEncode(string input)
        {
            return UnicodeCharacterEncoder.HtmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX 
        /// and %DOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert%28%27XSS%20Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        public static string UrlEncode(string input)
        {
            return Encoder.UrlEncode(input, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX 
        /// and %DOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert%28%27XSS+Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlFormUrlEncode(string input)
        {
            return Encoder.HtmlFormUrlEncode(input, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="codePage">Codepage number of the input.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        public static string UrlEncode(string input, int codePage)
        {
            return Encoder.UrlEncode(input, Encoding.GetEncoding(codePage));
        }

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="codePage">Codepage number of the input.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlFormUrlEncode(string input, int codePage)
        {
            return Encoder.HtmlFormUrlEncode(input, Encoding.GetEncoding(codePage));
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="inputEncoding">Input encoding type.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// If the inputEncoding is null then UTF-8 is assumed by default.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        public static string UrlEncode(string input, Encoding inputEncoding)
        {
            if (inputEncoding == null)
            {
                inputEncoding = Encoding.UTF8;
            }
            return HtmlParameterEncoder.QueryStringParameterEncode(input, inputEncoding);
        }

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="inputEncoding">Input encoding type.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// If the inputEncoding is null then UTF-8 is assumed by default.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        public static string HtmlFormUrlEncode(string input, Encoding inputEncoding)
        {
            if (inputEncoding == null)
            {
                inputEncoding = Encoding.UTF8;
            }
            return HtmlParameterEncoder.FormStringParameterEncode(input, inputEncoding);
        }

        /// <summary>
        /// URL-encodes the path section of a URL string and returns the encoded string.
        /// </summary>
        /// <param name="input">The text to URL path encode</param>
        /// <returns>The URL path encoded text.</returns>
        public static string UrlPathEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            string str;
            string s;
            string str2;
            if (!UriUtil.TrySplitUriForPathEncode(input, out str, out s, out str2))
            {
                str = null;
                UriUtil.ExtractQueryAndFragment(input, out s, out str2);
            }
            return str + HtmlParameterEncoder.UrlPathEncode(s, Encoding.UTF8) + str2;
        }

        /// <summary>
        /// Encodes input strings for use in XML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in XML.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters. Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="M:Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts,Microsoft.Security.Application.LowerMidCodeCharts,Microsoft.Security.Application.MidCodeCharts,Microsoft.Security.Application.UpperMidCodeCharts,Microsoft.Security.Application.UpperCodeCharts)" />.
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;apos;XSS Attack!&amp;apos;);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// </list>
        /// </remarks>
        public static string XmlEncode(string input)
        {
            return UnicodeCharacterEncoder.XmlEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in XML attributes.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in XML attributes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="M:Microsoft.Security.Application.UnicodeCharacterEncoder.MarkAsSafe(Microsoft.Security.Application.LowerCodeCharts,Microsoft.Security.Application.LowerMidCodeCharts,Microsoft.Security.Application.MidCodeCharts,Microsoft.Security.Application.UpperMidCodeCharts,Microsoft.Security.Application.UpperCodeCharts)" />.
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;apos;XSS Attack!&amp;apos);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&amp;#32;Site&amp;#32;Scripting&amp;#32;Library</description></item>
        /// </list>
        /// </remarks>
        public static string XmlAttributeEncode(string input)
        {
            return UnicodeCharacterEncoder.XmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in JavaScript.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        public static string JavaScriptEncode(string input)
        {
            return Encoder.JavaScriptEncode(input, true);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="emitQuotes">value indicating whether or not to emit quotes. true = emit quote. false = no quote.</param>
        /// <returns>
        /// Encoded string for use in JavaScript and does not return the output with en quotes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        public static string JavaScriptEncode(string input, bool emitQuotes)
        {
            if (!string.IsNullOrEmpty(input))
            {
                int length = 0;
                int length2 = input.Length;
                char[] array = new char[length2 * 8];
                if (emitQuotes)
                {
                    array[length++] = '\'';
                }
                for (int i = 0; i < length2; i++)
                {
                    int num = (int)input[i];
                    char c = input[i];
                    if (Encoder.SafeListCodes[num] != null || num == 92 || (num >= 123 && num <= 127))
                    {
                        if (num >= 127)
                        {
                            array[length++] = '\\';
                            array[length++] = 'u';
                            int num2 = (int)c;
                            string text = num2.ToString("x", CultureInfo.InvariantCulture).PadLeft(4, '0');
                            array[length++] = text[0];
                            array[length++] = text[1];
                            array[length++] = text[2];
                            array[length++] = text[3];
                        }
                        else
                        {
                            array[length++] = '\\';
                            array[length++] = 'x';
                            int num3 = (int)c;
                            string text2 = num3.ToString("x", CultureInfo.InvariantCulture).PadLeft(2, '0');
                            array[length++] = text2[0];
                            array[length++] = text2[1];
                        }
                    }
                    else
                    {
                        array[length++] = input[i];
                    }
                }
                if (emitQuotes)
                {
                    array[length++] = '\'';
                }
                return new string(array, 0, length);
            }
            if (!emitQuotes)
            {
                return string.Empty;
            }
            return "''";
        }

        /// <summary>
        /// Encodes input strings for use in Visual Basic Script.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in Visual Basic Script.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are 
        /// encoded using &amp;chrw(DECIMAL) notation.
        /// <newpara />
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// <newpara />
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>"alert"&amp;chrw(40)&amp;chrw(39)&amp;"XSS Attack"&amp;chrw(33)&amp;chrw(39)&amp;chrw(41)&amp;chrw(59)</description></item>
        /// <item><term>user@contoso.com</term><description>"user"&amp;chrw(64)&amp;"contoso.com"</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>"Anti-Cross Site Scripting Library"</description></item>
        /// </list></remarks>
        public static string VisualBasicScriptEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "\"\"";
            }
            int num = 0;
            int length = input.Length;
            char[] array = new char[length * 12];
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                int num2 = (int)input[i];
                char c = input[i];
                if (Encoder.SafeListCodes[num2] != null)
                {
                    if (flag)
                    {
                        array[num++] = '"';
                        flag = false;
                    }
                    string text = "&chrw(" + (uint)c + ")";
                    string text2 = text;
                    for (int j = 0; j < text2.Length; j++)
                    {
                        char c2 = text2[j];
                        array[num++] = c2;
                    }
                }
                else
                {
                    if (!flag)
                    {
                        array[num++] = '&';
                        array[num++] = '"';
                        flag = true;
                    }
                    array[num++] = input[i];
                }
            }
            if (flag)
            {
                array[num++] = '"';
            }
            if (array.Length > 0 && array[0] == '&')
            {
                return new string(array, 1, num - 1);
            }
            return new string(array, 0, num);
        }

        /// <summary>
        /// Initializes the safe list.
        /// </summary>
        /// <returns>A two dimensional character array containing characters and their encoded values.</returns>
        private static char[][] InitializeSafeList()
        {
            char[][] array = new char[65536][];
            for (int i = 0; i < array.Length; i++)
            {
                if ((i >= 97 && i <= 122) || (i >= 65 && i <= 90) || (i >= 48 && i <= 57) || (i == 32 || i == 46 || i == 44 || i == 45 || i == 95 || (i >= 256 && i <= 591)) || (i >= 880 && i <= 2047) || (i >= 2304 && i <= 6319) || (i >= 6400 && i <= 6687) || (i >= 6912 && i <= 7039) || (i >= 7680 && i <= 8191) || (i >= 11264 && i <= 11743) || (i >= 12352 && i <= 12591) || (i >= 12688 && i <= 12735) || (i >= 12784 && i <= 12799) || (i >= 19968 && i <= 40899) || (i >= 40960 && i <= 42191) || (i >= 42784 && i <= 43055) || (i >= 43072 && i <= 43135) || (i >= 44032 && i <= 55215))
                {
                    array[i] = null;
                }
                else
                {
                    string text = i.ToString(CultureInfo.InvariantCulture);
                    int length = text.Length;
                    char[] array2 = new char[length];
                    for (int j = 0; j < length; j++)
                    {
                        array2[j] = text[j];
                    }
                    array[i] = array2;
                }
            }
            return array;
        }
    }
}
