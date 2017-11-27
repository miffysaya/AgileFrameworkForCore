using System;
using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class Token
    {
        public struct RunEnumerator
        {
            private Token token;

            public TokenRun Current
            {
                get
                {
                    return new TokenRun(token);
                }
            }

            public bool IsValidPosition
            {
                get
                {
                    return token.wholePosition.run >= token.whole.head && token.wholePosition.run < token.whole.tail;
                }
            }

            public int CurrentIndex
            {
                get
                {
                    return token.wholePosition.run;
                }
            }

            public int CurrentOffset
            {
                get
                {
                    return token.wholePosition.runOffset;
                }
            }

            internal RunEnumerator(Token token)
            {
                this.token = token;
            }

            public bool MoveNext()
            {
                return token.MoveToNextRun(ref token.whole, ref token.wholePosition, false);
            }

            public bool MoveNext(bool skipInvalid)
            {
                return token.MoveToNextRun(ref token.whole, ref token.wholePosition, skipInvalid);
            }

            public RunEnumerator GetEnumerator()
            {
                return this;
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct TextReader
        {
            private Token token;

            public int Length
            {
                get
                {
                    return token.GetLength(ref token.whole);
                }
            }

            internal TextReader(Token token)
            {
                this.token = token;
            }

            public void Rewind()
            {
                token.wholePosition.Rewind(token.whole);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(ref token.whole, sink);
            }

            public void StripLeadingWhitespace()
            {
                token.StripLeadingWhitespace(ref token.whole);
                Rewind();
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        internal struct RunEntry
        {
            internal const int MaxRunLength = 134217727;

            internal const int MaxRunValue = 16777215;

            private uint lengthAndType;

            private uint valueAndKind;

            public RunType Type
            {
                get
                {
                    return (RunType)(lengthAndType & 3221225472u);
                }
            }

            public RunTextType TextType
            {
                get
                {
                    return (RunTextType)(lengthAndType & 939524096u);
                }
            }

            public int Length
            {
                get
                {
                    return (int)(lengthAndType & 16777215u);
                }
                set
                {
                    lengthAndType = (uint)(value | (int)(lengthAndType & 4278190080u));
                }
            }

            public uint Kind
            {
                get
                {
                    return valueAndKind & 4278190080u;
                }
            }

            public uint MajorKindPlusStartFlag
            {
                get
                {
                    return valueAndKind & 4227858432u;
                }
            }

            public uint MajorKind
            {
                get
                {
                    return valueAndKind & 2080374784u;
                }
            }

            public int Value
            {
                get
                {
                    return (int)(valueAndKind & 16777215u);
                }
            }

            public void Initialize(RunType type, RunTextType textType, uint kind, int length, int value)
            {
                lengthAndType = (uint)(length | (int)type | (int)textType);
                valueAndKind = (uint)(value | (int)kind);
            }

            public void InitializeSentinel()
            {
                valueAndKind = 2147483648u;
            }

            public override string ToString()
            {
                return string.Concat(new object[]
                {
                    Type.ToString(),
                    " - ",
                    TextType.ToString(),
                    " - ",
                    ((Kind & 2147483647u) >> 26).ToString(),
                    "/",
                    (Kind >> 24 & 3u).ToString(),
                    " (",
                    Length,
                    ") = ",
                    Value.ToString("X6")
                });
            }
        }

        internal struct LexicalUnit
        {
            public int head;

            public int headOffset;

            public void Reset()
            {
                head = -1;
                headOffset = 0;
            }

            public void Initialize(int run, int offset)
            {
                head = run;
                headOffset = offset;
            }

            public override string ToString()
            {
                return head.ToString("X") + " / " + headOffset.ToString("X");
            }
        }

        internal struct Fragment
        {
            public int head;

            public int tail;

            public int headOffset;

            public bool IsEmpty
            {
                get
                {
                    return head == tail;
                }
            }

            public void Reset()
            {
                head = (tail = (headOffset = 0));
            }

            public void Initialize(int run, int offset)
            {
                tail = run;
                head = run;
                headOffset = offset;
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    head.ToString("X"),
                    " - ",
                    tail.ToString("X"),
                    " / ",
                    headOffset.ToString("X")
                });
            }
        }

        internal struct FragmentPosition
        {
            public int run;

            public int runOffset;

            public int runDeltaOffset;

            public void Reset()
            {
                run = -2;
                runOffset = 0;
                runDeltaOffset = 0;
            }

            public void Rewind(LexicalUnit unit)
            {
                run = unit.head - 1;
                runOffset = unit.headOffset;
                runDeltaOffset = 0;
            }

            public void Rewind(Fragment fragment)
            {
                run = fragment.head - 1;
                runOffset = fragment.headOffset;
                runDeltaOffset = 0;
            }

            public bool SameAs(FragmentPosition pos2)
            {
                return run == pos2.run && runOffset == pos2.runOffset && runDeltaOffset == pos2.runDeltaOffset;
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    run.ToString("X"),
                    " / ",
                    runOffset.ToString("X"),
                    " + ",
                    runDeltaOffset.ToString("X")
                });
            }
        }

        private class LowerCaseCompareSink : ITextSink
        {
            private bool definitelyNotEqual;

            private int strIndex;

            private string str;

            public bool IsEqual
            {
                get
                {
                    return !definitelyNotEqual && strIndex == str.Length;
                }
            }

            public bool IsEnough
            {
                get
                {
                    return definitelyNotEqual;
                }
            }

            public void Reset(string str)
            {
                this.str = str;
                strIndex = 0;
                definitelyNotEqual = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                int num = offset + count;
                while (offset < num)
                {
                    if (strIndex == 0)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            offset++;
                            continue;
                        }
                    }
                    else if (strIndex == str.Length)
                    {
                        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset])))
                        {
                            offset++;
                            continue;
                        }
                        definitelyNotEqual = true;
                        return;
                    }
                    if (ParseSupport.ToLowerCase(buffer[offset]) != str[strIndex])
                    {
                        definitelyNotEqual = true;
                        return;
                    }
                    offset++;
                    strIndex++;
                }
            }

            public void Write(int ucs32Char)
            {
                if (LiteralLength(ucs32Char) != 1)
                {
                    definitelyNotEqual = true;
                    return;
                }
                if (strIndex == 0)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        return;
                    }
                }
                else if (strIndex == str.Length)
                {
                    if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)ucs32Char)))
                    {
                        return;
                    }
                    definitelyNotEqual = true;
                    return;
                }
                if (str[strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    definitelyNotEqual = true;
                    return;
                }
                strIndex++;
            }
        }

        private class LowerCaseSubstringSearchSink : ITextSink
        {
            private bool found;

            private int strIndex;

            private string str;

            public bool IsFound
            {
                get
                {
                    return found;
                }
            }

            public bool IsEnough
            {
                get
                {
                    return found;
                }
            }

            public void Reset(string str)
            {
                this.str = str;
                strIndex = 0;
                found = false;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                int num = offset + count;
                while (offset < num && strIndex < str.Length)
                {
                    if (ParseSupport.ToLowerCase(buffer[offset]) == str[strIndex])
                    {
                        strIndex++;
                    }
                    else
                    {
                        strIndex = 0;
                    }
                    offset++;
                }
                if (strIndex == str.Length)
                {
                    found = true;
                }
            }

            public void Write(int ucs32Char)
            {
                if (LiteralLength(ucs32Char) != 1 || str[strIndex] != ParseSupport.ToLowerCase((char)ucs32Char))
                {
                    strIndex = 0;
                    return;
                }
                strIndex++;
                if (strIndex == str.Length)
                {
                    found = true;
                }
            }
        }

        protected internal TokenId tokenId;

        protected internal int argument;

        protected internal char[] buffer;

        protected internal RunEntry[] runList;

        protected internal Fragment whole;

        protected internal FragmentPosition wholePosition;

        private LowerCaseCompareSink compareSink;

        private LowerCaseSubstringSearchSink searchSink;

        private StringBuildSink stringBuildSink;

        public TokenId TokenId
        {
            get
            {
                return tokenId;
            }
        }

        public int Argument
        {
            get
            {
                return argument;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return whole.tail == whole.head;
            }
        }

        public RunEnumerator Runs
        {
            get
            {
                return new RunEnumerator(this);
            }
        }

        public TextReader Text
        {
            get
            {
                return new TextReader(this);
            }
        }

        public bool IsWhitespaceOnly
        {
            get
            {
                return IsWhitespaceOnlyImp(ref whole);
            }
        }

        public Token()
        {
            Reset();
        }

        protected internal bool IsWhitespaceOnlyImp(ref Fragment fragment)
        {
            bool result = true;
            for (int num = fragment.head; num != fragment.tail; num++)
            {
                if (runList[num].Type >= (RunType)2147483648u && runList[num].TextType > RunTextType.UnusualWhitespace)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        internal static int LiteralLength(int literal)
        {
            if (literal <= 65535)
            {
                return 1;
            }
            return 2;
        }

        internal static char LiteralFirstChar(int literal)
        {
            if (literal <= 65535)
            {
                return (char)literal;
            }
            return ParseSupport.HighSurrogateCharFromUcs4(literal);
        }

        internal static char LiteralLastChar(int literal)
        {
            if (literal <= 65535)
            {
                return (char)literal;
            }
            return ParseSupport.LowSurrogateCharFromUcs4(literal);
        }

        protected internal int Read(LexicalUnit unit, ref FragmentPosition position, char[] buffer, int offset, int count)
        {
            int num = offset;
            if (unit.head != -1)
            {
                uint majorKind = runList[unit.head].MajorKind;
                int num2 = position.run;
                if (num2 == unit.head - 1)
                {
                    num2 = (position.run = unit.head);
                }
                RunEntry runEntry = runList[num2];
                if (num2 == unit.head || runEntry.MajorKindPlusStartFlag == majorKind)
                {
                    int num3 = position.runOffset;
                    int num4 = position.runDeltaOffset;
                    int num6;
                    while (true)
                    {
                        if (runEntry.Type == (RunType)3221225472u)
                        {
                            int num5 = LiteralLength(runEntry.Value);
                            if (num4 != num5)
                            {
                                if (num5 == 1)
                                {
                                    buffer[offset++] = (char)runEntry.Value;
                                    count--;
                                }
                                else if (num4 != 0)
                                {
                                    buffer[offset++] = LiteralLastChar(runEntry.Value);
                                    count--;
                                }
                                else
                                {
                                    buffer[offset++] = LiteralFirstChar(runEntry.Value);
                                    count--;
                                    if (count == 0)
                                    {
                                        break;
                                    }
                                    buffer[offset++] = LiteralLastChar(runEntry.Value);
                                    count--;
                                }
                            }
                        }
                        else if (runEntry.Type == (RunType)2147483648u)
                        {
                            num6 = Math.Min(count, runEntry.Length - num4);
                            Buffer.BlockCopy(this.buffer, (num3 + num4) * 2, buffer, offset * 2, num6 * 2);
                            offset += num6;
                            count -= num6;
                            if (num4 + num6 != runEntry.Length)
                            {
                                goto Block_10;
                            }
                        }
                        num3 += runEntry.Length;
                        num4 = 0;
                        runEntry = runList[++num2];
                        if (runEntry.MajorKindPlusStartFlag != majorKind || count == 0)
                        {
                            goto IL_1CF;
                        }
                    }
                    num4 = 1;
                    goto IL_1CF;
                    Block_10:
                    num4 += num6;
                    IL_1CF:
                    position.run = num2;
                    position.runOffset = num3;
                    position.runDeltaOffset = num4;
                }
            }
            return offset - num;
        }

        protected internal virtual void Rewind()
        {
            wholePosition.Rewind(whole);
        }

        protected internal int GetLength(ref Fragment fragment)
        {
            int num = fragment.head;
            int num2 = 0;
            if (num != fragment.tail)
            {
                do
                {
                    RunEntry runEntry = runList[num];
                    if (runEntry.Type == (RunType)2147483648u)
                    {
                        num2 += runEntry.Length;
                    }
                    else if (runEntry.Type == (RunType)3221225472u)
                    {
                        num2 += LiteralLength(runEntry.Value);
                    }
                }
                while (++num != fragment.tail);
            }
            return num2;
        }

        protected internal int GetLength(LexicalUnit unit)
        {
            int num = unit.head;
            int num2 = 0;
            if (num != -1)
            {
                RunEntry runEntry = runList[num];
                uint majorKind = runEntry.MajorKind;
                do
                {
                    if (runEntry.Type == (RunType)2147483648u)
                    {
                        num2 += runEntry.Length;
                    }
                    else if (runEntry.Type == (RunType)3221225472u)
                    {
                        num2 += LiteralLength(runEntry.Value);
                    }
                    runEntry = runList[++num];
                }
                while (runEntry.MajorKindPlusStartFlag == majorKind);
            }
            return num2;
        }

        protected internal bool IsFragmentEmpty(ref Fragment fragment)
        {
            int num = fragment.head;
            if (num != fragment.tail)
            {
                while (true)
                {
                    RunEntry runEntry = runList[num];
                    if (runEntry.Type == (RunType)2147483648u || runEntry.Type == (RunType)3221225472u)
                    {
                        break;
                    }
                    if (++num == fragment.tail)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        protected internal bool IsFragmentEmpty(LexicalUnit unit)
        {
            int num = unit.head;
            if (num != -1)
            {
                RunEntry runEntry = runList[num];
                uint majorKind = runEntry.MajorKind;
                while (runEntry.Type != (RunType)2147483648u && runEntry.Type != (RunType)3221225472u)
                {
                    runEntry = runList[++num];
                    if (runEntry.MajorKindPlusStartFlag != majorKind)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        protected internal bool IsContiguous(ref Fragment fragment)
        {
            return fragment.head + 1 == fragment.tail && runList[fragment.head].Type == (RunType)2147483648u;
        }

        protected internal bool IsContiguous(LexicalUnit unit)
        {
            return runList[unit.head].Type == (RunType)2147483648u && runList[unit.head].MajorKind != runList[unit.head + 1].MajorKindPlusStartFlag;
        }

        protected internal int CalculateHashLowerCase(Fragment fragment)
        {
            int num = fragment.head;
            if (num == fragment.tail)
            {
                return HashCode.CalculateEmptyHash();
            }
            int num2 = fragment.headOffset;
            if (num + 1 == fragment.tail && runList[num].Type == (RunType)2147483648u)
            {
                return HashCode.CalculateLowerCase(buffer, num2, runList[num].Length);
            }
            HashCode hashCode = new HashCode(true);
            do
            {
                RunEntry runEntry = runList[num];
                if (runEntry.Type == (RunType)2147483648u)
                {
                    hashCode.AdvanceLowerCase(buffer, num2, runEntry.Length);
                }
                else if (runEntry.Type == (RunType)3221225472u)
                {
                    hashCode.AdvanceLowerCase(runEntry.Value);
                }
                num2 += runEntry.Length;
            }
            while (++num != fragment.tail);
            return hashCode.FinalizeHash();
        }

        protected internal int CalculateHashLowerCase(LexicalUnit unit)
        {
            int num = unit.head;
            if (num == -1)
            {
                return HashCode.CalculateEmptyHash();
            }
            int num2 = unit.headOffset;
            RunEntry runEntry = runList[num];
            uint majorKind = runEntry.MajorKind;
            if (runEntry.Type == (RunType)2147483648u && majorKind != runList[num + 1].MajorKindPlusStartFlag)
            {
                return HashCode.CalculateLowerCase(buffer, num2, runEntry.Length);
            }
            HashCode hashCode = new HashCode(true);
            do
            {
                if (runEntry.Type == (RunType)2147483648u)
                {
                    hashCode.AdvanceLowerCase(buffer, num2, runEntry.Length);
                }
                else if (runEntry.Type == (RunType)3221225472u)
                {
                    hashCode.AdvanceLowerCase(runEntry.Value);
                }
                num2 += runEntry.Length;
                runEntry = runList[++num];
            }
            while (runEntry.MajorKindPlusStartFlag == majorKind);
            return hashCode.FinalizeHash();
        }

        protected internal void WriteOriginalTo(ref Fragment fragment, ITextSink sink)
        {
            int num = fragment.head;
            if (num != fragment.tail)
            {
                int num2 = fragment.headOffset;
                do
                {
                    RunEntry runEntry = runList[num];
                    if (runEntry.Type == (RunType)2147483648u || runEntry.Type == (RunType)3221225472u)
                    {
                        sink.Write(buffer, num2, runEntry.Length);
                    }
                    num2 += runEntry.Length;
                }
                while (++num != fragment.tail && !sink.IsEnough);
            }
        }

        protected internal void WriteTo(ref Fragment fragment, ITextSink sink)
        {
            int num = fragment.head;
            if (num != fragment.tail)
            {
                int num2 = fragment.headOffset;
                do
                {
                    RunEntry runEntry = runList[num];
                    if (runEntry.Type == (RunType)2147483648u)
                    {
                        sink.Write(buffer, num2, runEntry.Length);
                    }
                    else if (runEntry.Type == (RunType)3221225472u)
                    {
                        sink.Write(runEntry.Value);
                    }
                    num2 += runEntry.Length;
                }
                while (++num != fragment.tail && !sink.IsEnough);
            }
        }

        protected internal void WriteTo(LexicalUnit unit, ITextSink sink)
        {
            int num = unit.head;
            if (num != -1)
            {
                int num2 = unit.headOffset;
                RunEntry runEntry = runList[num];
                uint majorKind = runEntry.MajorKind;
                do
                {
                    if (runEntry.Type == (RunType)2147483648u)
                    {
                        sink.Write(buffer, num2, runEntry.Length);
                    }
                    else if (runEntry.Type == (RunType)3221225472u)
                    {
                        sink.Write(runEntry.Value);
                    }
                    num2 += runEntry.Length;
                    runEntry = runList[++num];
                }
                while (runEntry.MajorKindPlusStartFlag == majorKind && !sink.IsEnough);
            }
        }

        protected internal string GetString(ref Fragment fragment, int maxLength)
        {
            if (fragment.head == fragment.tail)
            {
                return string.Empty;
            }
            if (IsContiguous(ref fragment))
            {
                return new string(buffer, fragment.headOffset, GetLength(ref fragment));
            }
            if (IsFragmentEmpty(ref fragment))
            {
                return string.Empty;
            }
            if (stringBuildSink == null)
            {
                stringBuildSink = new StringBuildSink();
            }
            stringBuildSink.Reset(maxLength);
            this.WriteTo(ref fragment, stringBuildSink);
            return stringBuildSink.ToString();
        }

        protected internal string GetString(LexicalUnit unit, int maxLength)
        {
            if (IsFragmentEmpty(unit))
            {
                return string.Empty;
            }
            if (IsContiguous(unit))
            {
                return new string(buffer, unit.headOffset, GetLength(unit));
            }
            if (stringBuildSink == null)
            {
                stringBuildSink = new StringBuildSink();
            }
            stringBuildSink.Reset(maxLength);
            this.WriteTo(unit, stringBuildSink);
            return stringBuildSink.ToString();
        }

        protected internal bool CaseInsensitiveCompareEqual(ref Fragment fragment, string str)
        {
            if (compareSink == null)
            {
                compareSink = new Token.LowerCaseCompareSink();
            }
            compareSink.Reset(str);
            WriteTo(ref fragment, compareSink);
            return compareSink.IsEqual;
        }

        protected internal bool CaseInsensitiveCompareEqual(LexicalUnit unit, string str)
        {
            if (compareSink == null)
            {
                compareSink = new LowerCaseCompareSink();
            }
            compareSink.Reset(str);
            WriteTo(unit, compareSink);
            return compareSink.IsEqual;
        }

        protected internal virtual bool CaseInsensitiveCompareRunEqual(int runOffset, string str, int strOffset)
        {
            int i = strOffset;
            while (i < str.Length)
            {
                if (ParseSupport.ToLowerCase(buffer[runOffset++]) != str[i++])
                {
                    return false;
                }
            }
            return true;
        }

        protected internal bool CaseInsensitiveContainsSubstring(ref Fragment fragment, string str)
        {
            if (searchSink == null)
            {
                searchSink = new LowerCaseSubstringSearchSink();
            }
            searchSink.Reset(str);
            WriteTo(ref fragment, searchSink);
            return searchSink.IsFound;
        }

        protected internal void StripLeadingWhitespace(ref Fragment fragment)
        {
            int num = fragment.head;
            if (num != fragment.tail)
            {
                int num2 = fragment.headOffset;
                if (runList[num].Type < (RunType)2147483648u)
                {
                    SkipNonTextRuns(ref num, ref num2, fragment.tail);
                }
                if (num == fragment.tail)
                {
                    return;
                }
                int i;
                do
                {
                    if (runList[num].Type == (RunType)3221225472u)
                    {
                        if (runList[num].Value > 65535)
                        {
                            break;
                        }
                        CharClass charClass = ParseSupport.GetCharClass((char)runList[num].Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }
                    }
                    else
                    {
                        for (i = num2; i < num2 + runList[num].Length; i++)
                        {
                            CharClass charClass = ParseSupport.GetCharClass(buffer[i]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }
                        }
                        if (i < num2 + runList[num].Length)
                        {
                            goto Block_8;
                        }
                    }
                    num2 += runList[num].Length;
                    num++;
                    if (num != fragment.tail && runList[num].Type < (RunType)2147483648u)
                    {
                        SkipNonTextRuns(ref num, ref num2, fragment.tail);
                    }
                }
                while (num != fragment.tail);
                goto IL_162;
                Block_8:
                RunEntry[] expr_FA_cp_0 = runList;
                int expr_FA_cp_1 = num;
                expr_FA_cp_0[expr_FA_cp_1].Length = expr_FA_cp_0[expr_FA_cp_1].Length - (i - num2);
                num2 = i;
                IL_162:
                fragment.head = num;
                fragment.headOffset = num2;
            }
        }

        protected internal bool SkipLeadingWhitespace(LexicalUnit unit, ref FragmentPosition position)
        {
            int num = unit.head;
            if (num != -1)
            {
                int num2 = unit.headOffset;
                RunEntry runEntry = runList[num];
                uint majorKind = runEntry.MajorKind;
                int runDeltaOffset = 0;
                int i;
                do
                {
                    if (runEntry.Type == (RunType)3221225472u)
                    {
                        if (runEntry.Value > 65535)
                        {
                            break;
                        }
                        CharClass charClass = ParseSupport.GetCharClass((char)runEntry.Value);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            break;
                        }
                    }
                    else if (runEntry.Type == (RunType)2147483648u)
                    {
                        for (i = num2; i < num2 + runEntry.Length; i++)
                        {
                            CharClass charClass = ParseSupport.GetCharClass(buffer[i]);
                            if (!ParseSupport.WhitespaceCharacter(charClass))
                            {
                                break;
                            }
                        }
                        if (i < num2 + runEntry.Length)
                        {
                            goto Block_7;
                        }
                    }
                    num2 += runEntry.Length;
                    runEntry = runList[++num];
                }
                while (runEntry.MajorKindPlusStartFlag == majorKind);
                goto IL_EF;
                Block_7:
                runDeltaOffset = i - num2;
                IL_EF:
                position.run = num;
                position.runOffset = num2;
                position.runDeltaOffset = runDeltaOffset;
                if (num == unit.head || runEntry.MajorKindPlusStartFlag == majorKind)
                {
                    return true;
                }
            }
            return false;
        }

        protected internal bool MoveToNextRun(ref Fragment fragment, ref FragmentPosition position, bool skipInvalid)
        {
            int num = position.run;
            if (num != fragment.tail)
            {
                if (num >= fragment.head)
                {
                    position.runOffset += runList[num].Length;
                    position.runDeltaOffset = 0;
                }
                num++;
                if (skipInvalid)
                {
                    while (num != fragment.tail && runList[num].Type == RunType.Invalid)
                    {
                        position.runOffset += runList[num].Length;
                        num++;
                    }
                }
                position.run = num;
                return num != fragment.tail;
            }
            return false;
        }

        internal void SkipNonTextRuns(ref int run, ref int runOffset, int tail)
        {
            do
            {
                runOffset += runList[run].Length;
                run++;
            }
            while (run != tail && runList[run].Type < (RunType)2147483648u);
        }

        internal void Reset()
        {
            tokenId = TokenId.None;
            argument = 0;
            whole.Reset();
            wholePosition.Reset();
        }
    }
}
