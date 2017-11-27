namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal static class ParseSupport
    {
        private static readonly char[] latin1MappingInUnicodeControlArea = new char[]
        {
            '€',
            '\u0081',
            '‚',
            'ƒ',
            '„',
            '…',
            '†',
            '‡',
            'ˆ',
            '‰',
            'Š',
            '‹',
            'Œ',
            '\u008d',
            'Ž',
            '\u008f',
            '\u0090',
            '‘',
            '’',
            '“',
            '”',
            '•',
            '–',
            '—',
            '˜',
            '™',
            'š',
            '›',
            'œ',
            '\u009d',
            'ž',
            'Ÿ'
        };

        private static readonly byte[] charToHexTable = new byte[]
        {
            255,
            10,
            11,
            12,
            13,
            14,
            15,
            255,
            255,
            255,
            255,
            255,
            255,
            255,
            255,
            255,
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            255,
            255,
            255,
            255,
            255,
            255
        };

        private static readonly CharClass[] lowCharClass = new CharClass[]
        {
            CharClass.RtfInteresting,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Whitespace,
            CharClass.Whitespace,
            CharClass.Whitespace | CharClass.RtfInteresting,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Control,
            CharClass.Whitespace,
            CharClass.NotInterestingText,
            CharClass.DoubleQuote,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText | CharClass.HtmlSuffix,
            CharClass.Ampersand,
            CharClass.SingleQuote,
            CharClass.Parentheses,
            CharClass.Parentheses,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.Comma,
            CharClass.NotInterestingText | CharClass.HtmlSuffix,
            CharClass.NotInterestingText,
            CharClass.Solidus,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Numeric,
            CharClass.Colon,
            CharClass.NotInterestingText,
            CharClass.LessThan,
            CharClass.Equals,
            CharClass.GreaterThan | CharClass.HtmlSuffix,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.SquareBrackets,
            CharClass.Backslash | CharClass.RtfInteresting,
            CharClass.SquareBrackets | CharClass.HtmlSuffix,
            CharClass.Circumflex,
            CharClass.NotInterestingText,
            CharClass.GraveAccent,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            (CharClass)2147483656u,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.Alpha,
            CharClass.CurlyBrackets | CharClass.RtfInteresting,
            CharClass.VerticalLine,
            CharClass.CurlyBrackets | CharClass.RtfInteresting,
            CharClass.Tilde,
            CharClass.Control,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.Nbsp,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText,
            CharClass.NotInterestingText
        };

        public static int CharToDecimal(char ch)
        {
            return (int)(ch - '0');
        }

        public static int CharToHex(char ch)
        {
            return (int)ParseSupport.charToHexTable[(int)(ch & '\u001f')];
        }

        public static char HighSurrogateCharFromUcs4(int ich)
        {
            return (char)(55296 + (ich - 65536 >> 10));
        }

        public static char LowSurrogateCharFromUcs4(int ich)
        {
            return (char)(56320 + (ich & 1023));
        }

        public static bool IsCharClassOneOf(CharClass charClass, CharClass charClassSet)
        {
            return (charClass & charClassSet) != CharClass.Invalid;
        }

        public static bool InvalidUnicodeCharacter(CharClass charClass)
        {
            return (charClass & CharClass.UniqueMask) == CharClass.Invalid;
        }

        public static bool HtmlTextCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlNonWhitespaceText) != CharClass.Invalid;
        }

        public static bool WhitespaceCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Whitespace) != CharClass.Invalid;
        }

        public static bool WhitespaceCharacter(char ch)
        {
            return (ParseSupport.GetCharClass(ch) & CharClass.Whitespace) != CharClass.Invalid;
        }

        public static bool NbspCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Nbsp) != CharClass.Invalid;
        }

        public static bool AlphaCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Alpha) != CharClass.Invalid;
        }

        public static bool QuoteCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Quote) != CharClass.Invalid;
        }

        public static bool HtmlAttrValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrValue) != CharClass.Invalid;
        }

        public static bool HtmlScanQuoteSensitiveCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlScanQuoteSensitive) != CharClass.Invalid;
        }

        public static bool HtmlSimpleTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleTagName) != CharClass.Invalid;
        }

        public static bool HtmlEndTagNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndTagName) != CharClass.Invalid;
        }

        public static bool HtmlSimpleAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrName) != CharClass.Invalid;
        }

        public static bool HtmlEndAttrNameCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrName) != CharClass.Invalid;
        }

        public static bool HtmlSimpleAttrQuotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSimpleAttrQuotedValue) != CharClass.Invalid;
        }

        public static bool HtmlSimpleAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlAttrValue) != CharClass.Invalid;
        }

        public static bool HtmlEndAttrUnquotedValueCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEndAttrUnquotedValue) != CharClass.Invalid;
        }

        public static bool NumericCharacter(CharClass charClass)
        {
            return (charClass & CharClass.Numeric) != CharClass.Invalid;
        }

        public static bool HexCharacter(CharClass charClass)
        {
            return (charClass & (CharClass)2147483664u) != CharClass.Invalid;
        }

        public static bool HtmlEntityCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlEntity) != CharClass.Invalid;
        }

        public static bool HtmlSuffixCharacter(CharClass charClass)
        {
            return (charClass & CharClass.HtmlSuffix) != CharClass.Invalid;
        }

        public static CharClass GetCharClass(char ch)
        {
            if (ch > 'ÿ')
            {
                return ParseSupport.GetHighCharClass(ch);
            }
            return ParseSupport.lowCharClass[(int)ch];
        }

        public static CharClass GetHighCharClass(char ch)
        {
            if (ch < '﷐')
            {
                return CharClass.NotInterestingText;
            }
            if (('￹' <= ch && ch <= '�') || ('ﷰ' <= ch && ch <= '￯'))
            {
                return CharClass.NotInterestingText;
            }
            return CharClass.Invalid;
        }

        public static bool IsUpperCase(char ch)
        {
            return ch - 'A' <= '\u0019';
        }

        public static char ToLowerCase(char ch)
        {
            if (!ParseSupport.IsUpperCase(ch))
            {
                return ch;
            }
            return (char)(ch + ' ');
        }

        public static int Latin1MappingInUnicodeControlArea(int value)
        {
            return (int)ParseSupport.latin1MappingInUnicodeControlArea[value - 128];
        }

        public static bool TwoFarEastNonHanguelChars(char ch1, char ch2)
        {
            return ch1 >= '\u3000' && ch2 >= '\u3000' && !ParseSupport.HanguelRange(ch1) && !ParseSupport.HanguelRange(ch2);
        }

        public static bool FarEastNonHanguelChar(char ch)
        {
            return ch >= '\u3000' && !ParseSupport.HanguelRange(ch);
        }

        private static bool HanguelRange(char ch)
        {
            return ('㄰' <= ch && ch <= '㆏') || ('가' <= ch && ch <= '힣') || ('ﾡ' <= ch && ch <= 'ￜ');
        }
    }
}
