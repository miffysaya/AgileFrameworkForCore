using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal class CssToken : Token
    {
        public enum PropertyListPartMajor : byte
        {
            None,
            Begin = 3,
            Continue = 2,
            End = 6,
            Complete
        }

        public enum PropertyListPartMinor : byte
        {
            Empty,
            BeginProperty = 24,
            ContinueProperty = 16,
            EndProperty = 48,
            EndPropertyWithOtherProperties = 176,
            PropertyPartMask = 56,
            Properties = 128
        }

        public enum PropertyPartMajor : byte
        {
            None,
            Begin = 3,
            Continue = 2,
            End = 6,
            Complete,
            ValueQuoted = 64,
            Deleted = 128,
            MaskOffFlags = 7
        }

        public enum PropertyPartMinor : byte
        {
            Empty,
            BeginName = 3,
            ContinueName = 2,
            EndName = 6,
            EndNameWithBeginValue = 30,
            EndNameWithCompleteValue = 62,
            CompleteName = 7,
            CompleteNameWithBeginValue = 31,
            CompleteNameWithCompleteValue = 63,
            BeginValue = 24,
            ContinueValue = 16,
            EndValue = 48,
            CompleteValue = 56
        }

        public struct PropertyEnumerator
        {
            private CssToken token;

            public int ValidCount
            {
                get
                {
                    int num = 0;
                    for (int i = token.propertyHead; i < token.propertyTail; i++)
                    {
                        if (!token.propertyList[i].IsPropertyDeleted)
                        {
                            num++;
                        }
                    }
                    return num;
                }
            }

            public CssProperty Current
            {
                get
                {
                    return new CssProperty(token);
                }
            }

            internal PropertyEnumerator(CssToken token)
            {
                this.token = token;
            }

            public bool MoveNext()
            {
                if (token.currentProperty != token.propertyTail)
                {
                    token.currentProperty++;
                    if (token.currentProperty != token.propertyTail)
                    {
                        token.propertyNamePosition.Rewind(token.propertyList[token.currentProperty].name);
                        token.propertyValuePosition.Rewind(token.propertyList[token.currentProperty].value);
                    }
                }
                return token.currentProperty != token.propertyTail;
            }

            public void Rewind()
            {
                token.currentProperty = token.propertyHead - 1;
            }

            public PropertyEnumerator GetEnumerator()
            {
                return this;
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct PropertyNameTextReader
        {
            private CssToken token;

            internal PropertyNameTextReader(CssToken token)
            {
                this.token = token;
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteOriginalTo(ref token.propertyList[token.currentProperty].name, sink);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct PropertyValueTextReader
        {
            private CssToken token;

            internal PropertyValueTextReader(CssToken token)
            {
                this.token = token;
            }

            public void WriteEscapedOriginalTo(ITextSink sink)
            {
                token.WriteEscapedOriginalTo(ref token.propertyList[token.currentProperty].value, sink);
            }

            public bool CaseInsensitiveContainsSubstring(string str)
            {
                return token.CaseInsensitiveContainsSubstring(ref token.propertyList[token.currentProperty].value, str);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        protected internal struct PropertyEntry
        {
            public CssNameIndex nameId;

            public byte quoteChar;

            public PropertyPartMajor partMajor;

            public PropertyPartMinor partMinor;

            public Token.Fragment name;

            public Token.Fragment value;

            public bool IsPropertyBegin
            {
                get
                {
                    return (byte)(partMajor & PropertyPartMajor.Begin) == 3;
                }
            }

            public bool IsPropertyNameEnd
            {
                get
                {
                    return (byte)(partMinor & PropertyPartMinor.EndName) == 6;
                }
            }

            public bool IsPropertyDeleted
            {
                get
                {
                    return (byte)(partMajor & PropertyPartMajor.Deleted) == 128;
                }
                set
                {
                    partMajor = (value ? (partMajor | PropertyPartMajor.Deleted) : (partMajor & (PropertyPartMajor)127));
                }
            }
        }

        public struct SelectorEnumerator
        {
            private CssToken token;

            public int ValidCount
            {
                get
                {
                    int num = 0;
                    for (int i = token.selectorHead; i < token.selectorTail; i++)
                    {
                        if (!token.selectorList[i].IsSelectorDeleted)
                        {
                            num++;
                        }
                    }
                    return num;
                }
            }

            public CssSelector Current
            {
                get
                {
                    return new CssSelector(token);
                }
            }

            internal SelectorEnumerator(CssToken token)
            {
                this.token = token;
            }

            public bool MoveNext()
            {
                if (token.currentSelector != token.selectorTail)
                {
                    token.currentSelector++;
                    if (token.currentSelector != token.selectorTail)
                    {
                        token.selectorNamePosition.Rewind(token.selectorList[token.currentSelector].name);
                        token.selectorClassPosition.Rewind(token.selectorList[token.currentSelector].className);
                    }
                }
                return token.currentSelector != token.selectorTail;
            }

            public void Rewind()
            {
                token.currentSelector = token.selectorHead - 1;
            }

            public SelectorEnumerator GetEnumerator()
            {
                return this;
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct SelectorNameTextReader
        {
            private CssToken token;

            internal SelectorNameTextReader(CssToken token)
            {
                this.token = token;
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteOriginalTo(ref token.selectorList[token.currentSelector].name, sink);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct SelectorClassTextReader
        {
            private CssToken token;

            internal SelectorClassTextReader(CssToken token)
            {
                this.token = token;
            }

            public void WriteOriginalTo(ITextSink sink)
            {
                token.WriteEscapedOriginalTo(ref token.selectorList[token.currentSelector].className, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(ref token.selectorList[token.currentSelector].className, maxSize);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        protected internal struct SelectorEntry
        {
            public HtmlNameIndex nameId;

            //public bool deleted;

            public Token.Fragment name;

            public Token.Fragment className;

            public CssSelectorClassType classType;

            public CssSelectorCombinator combinator;

            public bool IsSelectorDeleted
            {
                get
                {
                    return false;
                }
            }
        }

        protected internal PropertyListPartMajor partMajor;

        protected internal PropertyListPartMinor partMinor;

        protected internal PropertyEntry[] propertyList;

        protected internal int propertyHead;

        protected internal int propertyTail;

        protected internal int currentProperty;

        protected internal FragmentPosition propertyNamePosition;

        protected internal FragmentPosition propertyValuePosition;

        protected internal SelectorEntry[] selectorList;

        protected internal int selectorHead;

        protected internal int selectorTail;

        protected internal int currentSelector;

        protected internal FragmentPosition selectorNamePosition;

        protected internal FragmentPosition selectorClassPosition;

        public new CssTokenId TokenId
        {
            get
            {
                return (CssTokenId)base.TokenId;
            }
        }

        public bool IsPropertyListBegin
        {
            get
            {
                return (byte)(partMajor & PropertyListPartMajor.Begin) == 3;
            }
        }

        public bool IsPropertyListEnd
        {
            get
            {
                return (byte)(partMajor & PropertyListPartMajor.End) == 6;
            }
        }

        public PropertyEnumerator Properties
        {
            get
            {
                return new PropertyEnumerator(this);
            }
        }

        public SelectorEnumerator Selectors
        {
            get
            {
                return new SelectorEnumerator(this);
            }
        }

        public CssToken()
        {
            Reset();
        }

        internal new void Reset()
        {
            partMajor = PropertyListPartMajor.None;
            partMinor = PropertyListPartMinor.Empty;
            propertyHead = (propertyTail = 0);
            currentProperty = -1;
            selectorHead = (selectorTail = 0);
            currentSelector = -1;
        }

        protected internal void WriteEscapedOriginalTo(ref Token.Fragment fragment, ITextSink sink)
        {
            int num = fragment.head;
            if (num != fragment.tail)
            {
                int num2 = fragment.headOffset;
                do
                {
                    Token.RunEntry runEntry = runList[num];
                    if (runEntry.Type == (RunType)2147483648u || runEntry.Type == (RunType)3221225472u)
                    {
                        EscapeAndWriteBuffer(buffer, num2, runEntry.Length, sink);
                    }
                    num2 += runEntry.Length;
                }
                while (++num != fragment.tail && !sink.IsEnough);
            }
        }

        private void EscapeAndWriteBuffer(char[] buffer, int offset, int length, ITextSink sink)
        {
            int num = offset;
            int i = offset;
            while (i < offset + length)
            {
                char c = buffer[i];
                if (c == '>' || c == '<')
                {
                    if (i - num > 0)
                    {
                        sink.Write(buffer, num, i - num);
                    }
                    uint num2 = (uint)c;
                    char[] array = new char[]
                    {
                        '\\',
                        '\0',
                        '\0',
                        ' '
                    };
                    for (int j = 2; j > 0; j--)
                    {
                        uint num3 = num2 & 15u;
                        array[j] = (char)((ulong)num3 + (ulong)((num3 < 10u) ? 48L : 55L));
                        num2 >>= 4;
                    }
                    sink.Write(array, 0, 4);
                    i = (num = i + 1);
                }
                else
                {
                    AttemptUnescape(buffer, offset + length, ref c, ref i);
                    i++;
                }
            }
            sink.Write(buffer, num, length - (num - offset));
        }

        internal static bool AttemptUnescape(char[] parseBuffer, int parseEnd, ref char ch, ref int parseCurrent)
        {
            if (ch != '\\' || parseCurrent == parseEnd)
            {
                return false;
            }
            ch = parseBuffer[++parseCurrent];
            CharClass charClass = ParseSupport.GetCharClass(ch);
            int num = parseCurrent + 6;
            num = ((num < parseEnd) ? num : parseEnd);
            if (ParseSupport.HexCharacter(charClass))
            {
                int num2 = 0;
                do
                {
                    num2 <<= 4;
                    num2 |= ParseSupport.CharToHex(ch);
                    if (parseCurrent == num)
                    {
                        goto IL_C3;
                    }
                    ch = parseBuffer[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                while (ParseSupport.HexCharacter(charClass));
                if (ch == '\r' && parseCurrent != parseEnd)
                {
                    ch = parseBuffer[++parseCurrent];
                    if (ch == '\n')
                    {
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else
                    {
                        parseCurrent--;
                    }
                }
                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n' && ch != '\f')
                {
                    parseCurrent--;
                }
                IL_C3:
                ch = (char)num2;
                return true;
            }
            if (ch >= ' ' && ch != '\u007f')
            {
                return true;
            }
            parseCurrent--;
            ch = '\\';
            return false;
        }
    }
}
