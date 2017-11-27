using AgileFramework.Security.Application.TextConverters.HTML;
using AgileFramework.Security.Exchange.Data.Globalization;
using System;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        protected enum ParseState : byte
        {
            Text,
            TagStart,
            TagNamePrefix,
            TagName,
            TagWsp,
            AttrNameStart,
            AttrNamePrefix,
            AttrName,
            AttrWsp,
            AttrValueWsp,
            AttrValue,
            EmptyTagEnd,
            TagEnd,
            TagSkip,
            CommentStart,
            Comment,
            Conditional,
            CommentConditional,
            Bang,
            Dtd,
            Asp
        }

        private const int ParseThresholdMax = 16;

        private ConverterInput input;

        private bool endOfFile;

        private bool literalTags;

        private HtmlNameIndex literalTagNameId;

        private bool literalEntities;

        private bool plaintext;

        //private bool parseConditionals;

        private ParseState parseState;

        private char[] parseBuffer;

        private int parseStart;

        private int parseCurrent;

        private int parseEnd;

        private int parseThreshold = 1;

        private bool slowParse = true;

        private char scanQuote;

        private char valueQuote;

        private CharClass lastCharClass;

        private int nameLength;

        private HtmlTokenBuilder tokenBuilder;

        private HtmlToken token;

        private IRestartable restartConsumer;

        private bool detectEncodingFromMetaTag;

        private short[] hashValuesTable;

        private bool rightMeta;

        private Encoding newEncoding;

        public HtmlToken Token
        {
            get
            {
                return token;
            }
        }

        public HtmlParser(ConverterInput input, bool detectEncodingFromMetaTag, bool preformatedText, int maxRuns, int maxAttrs, bool testBoundaryConditions)
        {
            this.input = input;
            this.detectEncodingFromMetaTag = detectEncodingFromMetaTag;
            input.SetRestartConsumer(this);
            tokenBuilder = new HtmlTokenBuilder(null, maxRuns, maxAttrs, testBoundaryConditions);
            token = tokenBuilder.Token;
            plaintext = preformatedText;
            literalEntities = preformatedText;
        }

        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        private void Reinitialize()
        {
            endOfFile = false;
            literalTags = false;
            literalTagNameId = HtmlNameIndex._NOTANAME;
            literalEntities = false;
            plaintext = false;
            parseState = ParseState.Text;
            parseBuffer = null;
            parseStart = 0;
            parseCurrent = 0;
            parseEnd = 0;
            parseThreshold = 1;
            slowParse = true;
            scanQuote = '\0';
            valueQuote = '\0';
            lastCharClass = CharClass.Invalid;
            nameLength = 0;
            tokenBuilder.Reset();
            tokenBuilder.MakeEmptyToken(HtmlTokenId.Restart);
        }

        bool IRestartable.CanRestart()
        {
            return restartConsumer != null && restartConsumer.CanRestart();
        }

        void IRestartable.Restart()
        {
            if (restartConsumer != null)
            {
                restartConsumer.Restart();
            }
            Reinitialize();
        }

        void IRestartable.DisableRestart()
        {
            if (restartConsumer != null)
            {
                restartConsumer.DisableRestart();
                restartConsumer = null;
            }
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            ((IReusable)input).Initialize(newSourceOrDestination);
            Reinitialize();
            input.SetRestartConsumer(this);
        }

        public void Initialize(string fragment, bool preformatedText)
        {
            (input as ConverterBufferInput).Initialize(fragment);
            Reinitialize();
            plaintext = preformatedText;
            literalEntities = preformatedText;
        }

        void IDisposable.Dispose()
        {
            if (input != null)
            {
                ((IDisposable)input).Dispose();
            }
            input = null;
            restartConsumer = null;
            parseBuffer = null;
            token = null;
            tokenBuilder = null;
            hashValuesTable = null;
            GC.SuppressFinalize(this);
        }

        public HtmlTokenId Parse()
        {
            if (slowParse)
            {
                return ParseSlow();
            }
            if (tokenBuilder.Valid)
            {
                input.ReportProcessed(parseCurrent - parseStart);
                parseStart = parseCurrent;
                tokenBuilder.Reset();
            }
            char[] array = parseBuffer;
            int num = parseCurrent;
            int num2 = num;
            bool flag = false;
            char c = array[++num];
            if (c == '/')
            {
                flag = true;
                c = array[++num];
            }
            CharClass charClass = ParseSupport.GetCharClass(c);
            if (ParseSupport.AlphaCharacter(charClass))
            {
                tokenBuilder.StartTag(HtmlNameIndex.Unknown, num2);
                if (flag)
                {
                    tokenBuilder.SetEndTag();
                }
                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num2, num);
                tokenBuilder.StartTagName();
                int num3 = 0;
                num2 = num;
                parseState = ParseState.TagNamePrefix;
                do
                {
                    c = array[++num];
                    charClass = ParseSupport.GetCharClass(c);
                }
                while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));
                if (c == ':')
                {
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
                    tokenBuilder.EndTagNamePrefix();
                    num3 = num + 1 - num2;
                    num2 = num + 1;
                    do
                    {
                        c = array[++num];
                        charClass = ParseSupport.GetCharClass(c);
                    }
                    while (ParseSupport.HtmlSimpleTagNameCharacter(charClass));
                    parseState = ParseState.TagName;
                }
                if (num != num2)
                {
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
                    num3 += num - num2;
                }
                if (ParseSupport.HtmlEndTagNameCharacter(charClass))
                {
                    tokenBuilder.EndTagName(num3);
                    while (true)
                    {
                        IL_19A:
                        if (ParseSupport.WhitespaceCharacter(charClass))
                        {
                            num2 = num;
                            do
                            {
                                c = array[++num];
                                charClass = ParseSupport.GetCharClass(c);
                            }
                            while (ParseSupport.WhitespaceCharacter(charClass));
                            tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
                        }
                        while (c != '>' && (c != '/' || array[num + 1] != '>'))
                        {
                            parseState = ParseState.TagWsp;
                            if (!ParseSupport.HtmlSimpleAttrNameCharacter(charClass) || !tokenBuilder.CanAddAttribute() || !tokenBuilder.PrepareToAddMoreRuns(11))
                            {
                                goto IL_5D4;
                            }
                            tokenBuilder.StartAttribute();
                            num3 = 0;
                            num2 = num;
                            parseState = ParseState.AttrNamePrefix;
                            do
                            {
                                c = array[++num];
                                charClass = ParseSupport.GetCharClass(c);
                            }
                            while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));
                            if (c == ':')
                            {
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
                                tokenBuilder.EndAttributeNamePrefix();
                                num3 = num + 1 - num2;
                                num2 = num + 1;
                                do
                                {
                                    c = array[++num];
                                    charClass = ParseSupport.GetCharClass(c);
                                }
                                while (ParseSupport.HtmlSimpleAttrNameCharacter(charClass));
                                parseState = ParseState.AttrName;
                            }
                            if (num != num2)
                            {
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num2, num);
                                num3 += num - num2;
                            }
                            if (!ParseSupport.HtmlEndAttrNameCharacter(charClass))
                            {
                                goto IL_5D4;
                            }
                            tokenBuilder.EndAttributeName(num3);
                            if (ParseSupport.WhitespaceCharacter(charClass))
                            {
                                num2 = num;
                                do
                                {
                                    c = array[++num];
                                    charClass = ParseSupport.GetCharClass(c);
                                }
                                while (ParseSupport.WhitespaceCharacter(charClass));
                                tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
                                parseState = ParseState.AttrWsp;
                                if (ParseSupport.InvalidUnicodeCharacter(charClass))
                                {
                                    goto IL_5D4;
                                }
                            }
                            if (c != '=')
                            {
                                tokenBuilder.EndAttribute();
                            }
                            else
                            {
                                tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num, num + 1);
                                c = array[++num];
                                charClass = ParseSupport.GetCharClass(c);
                                if (ParseSupport.WhitespaceCharacter(charClass))
                                {
                                    num2 = num;
                                    do
                                    {
                                        c = array[++num];
                                        charClass = ParseSupport.GetCharClass(c);
                                    }
                                    while (ParseSupport.WhitespaceCharacter(charClass));
                                    tokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num2, num);
                                    parseState = ParseState.AttrValueWsp;
                                    if (ParseSupport.InvalidUnicodeCharacter(charClass))
                                    {
                                        goto IL_5D4;
                                    }
                                }
                                if (ParseSupport.QuoteCharacter(charClass))
                                {
                                    valueQuote = c;
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num, num + 1);
                                    tokenBuilder.StartValue();
                                    tokenBuilder.SetValueQuote(valueQuote);
                                    c = array[++num];
                                    charClass = ParseSupport.GetCharClass(c);
                                    if (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass))
                                    {
                                        num2 = num;
                                        do
                                        {
                                            c = array[++num];
                                            charClass = ParseSupport.GetCharClass(c);
                                        }
                                        while (ParseSupport.HtmlSimpleAttrQuotedValueCharacter(charClass));
                                        tokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num2, num);
                                    }
                                    if (c != valueQuote)
                                    {
                                        goto Block_33;
                                    }
                                    valueQuote = '\0';
                                    tokenBuilder.EndValue();
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num, num + 1);
                                    c = array[++num];
                                    charClass = ParseSupport.GetCharClass(c);
                                    tokenBuilder.EndAttribute();
                                    goto IL_19A;
                                }
                                else
                                {
                                    if (!ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass))
                                    {
                                        goto IL_5CC;
                                    }
                                    tokenBuilder.StartValue();
                                    num2 = num;
                                    do
                                    {
                                        c = array[++num];
                                        charClass = ParseSupport.GetCharClass(c);
                                    }
                                    while (ParseSupport.HtmlSimpleAttrUnquotedValueCharacter(charClass));
                                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrValue, num2, num);
                                    parseState = ParseState.AttrValue;
                                    if (ParseSupport.HtmlEndAttrUnquotedValueCharacter(charClass))
                                    {
                                        tokenBuilder.EndValue();
                                        tokenBuilder.EndAttribute();
                                        goto IL_19A;
                                    }
                                    goto IL_5D4;
                                }
                            }
                        }
                        break;
                    }
                    num2 = num;
                    if (c == '/')
                    {
                        num++;
                        tokenBuilder.SetEmptyScope();
                    }
                    num++;
                    tokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num2, num);
                    tokenBuilder.EndTag(true);
                    if (array[num] == '<')
                    {
                        parseState = ParseState.TagStart;
                    }
                    else
                    {
                        parseState = ParseState.Text;
                        slowParse = true;
                    }
                    parseCurrent = num;
                    HandleSpecialTag();
                    return token.TokenId;
                    Block_33:
                    scanQuote = valueQuote;
                    parseState = ParseState.AttrValue;
                    goto IL_5D4;
                    IL_5CC:
                    parseState = ParseState.AttrValueWsp;
                }
                IL_5D4:
                parseCurrent = num;
                lastCharClass = ParseSupport.GetCharClass(array[num - 1]);
                nameLength = num3;
            }
            slowParse = true;
            return ParseSlow();
        }

        public HtmlTokenId ParseSlow()
        {
            if (tokenBuilder.Valid)
            {
                if (tokenBuilder.IncompleteTag)
                {
                    int num = tokenBuilder.RewindTag();
                    input.ReportProcessed(num - parseStart);
                    parseStart = num;
                }
                else
                {
                    input.ReportProcessed(parseCurrent - parseStart);
                    parseStart = parseCurrent;
                    tokenBuilder.Reset();
                }
            }
            ConverterDecodingInput converterDecodingInput;
            while (true)
            {
                bool flag = false;
                if (parseCurrent + parseThreshold > parseEnd)
                {
                    if (!endOfFile)
                    {
                        if (!input.ReadMore(ref parseBuffer, ref parseStart, ref parseCurrent, ref parseEnd))
                        {
                            break;
                        }
                        tokenBuilder.BufferChanged(parseBuffer, parseStart);
                        converterDecodingInput = (input as ConverterDecodingInput);
                        if (converterDecodingInput != null && converterDecodingInput.EncodingChanged)
                        {
                            goto Block_7;
                        }
                        if (input.EndOfFile)
                        {
                            endOfFile = true;
                        }
                        if (!endOfFile && parseEnd - parseStart < input.MaxTokenSize)
                        {
                            continue;
                        }
                    }
                    flag = true;
                }
                char ch = parseBuffer[parseCurrent];
                CharClass charClass = ParseSupport.GetCharClass(ch);
                if (ParseSupport.InvalidUnicodeCharacter(charClass) || parseThreshold > 1)
                {
                    bool flag2 = SkipInvalidCharacters(ref ch, ref charClass, ref parseCurrent);
                    if (token.IsEmpty)
                    {
                        input.ReportProcessed(parseCurrent - parseStart);
                        parseStart = parseCurrent;
                        if (tokenBuilder.IncompleteTag)
                        {
                            tokenBuilder.BufferChanged(parseBuffer, parseStart);
                        }
                    }
                    if (!flag2)
                    {
                        if (!flag)
                        {
                            continue;
                        }
                        if (parseCurrent == parseEnd && !tokenBuilder.IsStarted && endOfFile)
                        {
                            goto IL_226;
                        }
                    }
                    parseThreshold = 1;
                }
                if (ParseStateMachine(ch, charClass, flag))
                {
                    goto Block_17;
                }
            }
            return HtmlTokenId.None;
            Block_7:
            converterDecodingInput.EncodingChanged = false;
            return tokenBuilder.MakeEmptyToken(HtmlTokenId.EncodingChange, converterDecodingInput.Encoding.CodePage);
            Block_17:
            return token.TokenId;
            IL_226:
            return tokenBuilder.MakeEmptyToken(HtmlTokenId.EndOfFile);
        }

        public bool ParseStateMachine(char ch, CharClass charClass, bool forceFlushToken)
        {
            HtmlTokenBuilder htmlTokenBuilder = tokenBuilder;
            char[] array = parseBuffer;
            int num = parseCurrent;
            int num2 = parseEnd;
            int num3 = num;
            switch (parseState)
            {
                case ParseState.Text:
                    if (ch == '<' && !plaintext)
                    {
                        parseState = ParseState.TagStart;
                        goto IL_E4;
                    }
                    break;
                case ParseState.TagStart:
                    goto IL_E4;
                case ParseState.TagNamePrefix:
                    goto IL_28A;
                case ParseState.TagName:
                    goto IL_358;
                case ParseState.TagWsp:
                    goto IL_431;
                case ParseState.AttrNameStart:
                    goto IL_4A5;
                case ParseState.AttrNamePrefix:
                    goto IL_4D7;
                case ParseState.AttrName:
                    goto IL_588;
                case ParseState.AttrWsp:
                    goto IL_60A;
                case ParseState.AttrValueWsp:
                    goto IL_6C8;
                case ParseState.AttrValue:
                    goto IL_795;
                case ParseState.EmptyTagEnd:
                    goto IL_85B;
                case ParseState.TagEnd:
                    goto IL_8EB;
                case ParseState.TagSkip:
                    goto IL_979;
                case ParseState.CommentStart:
                    goto IL_ABE;
                case ParseState.Comment:
                case ParseState.Conditional:
                case ParseState.CommentConditional:
                case ParseState.Bang:
                case ParseState.Dtd:
                case ParseState.Asp:
                    goto IL_D73;
                default:
                    parseCurrent = num;
                    throw new TextConvertersException("internal error: invalid parse state");
            }
            IL_A0:
            htmlTokenBuilder.StartText(num3);
            ParseText(ch, charClass, ref num);
            if (token.IsEmpty && !forceFlushToken)
            {
                htmlTokenBuilder.Reset();
                slowParse = true;
                goto IL_F2D;
            }
            htmlTokenBuilder.EndText();
            parseCurrent = num;
            return true;
            IL_E4:
            char c = array[num + 1];
            CharClass charClass2 = ParseSupport.GetCharClass(c);
            bool flag = false;
            if (c == '/')
            {
                c = array[num + 2];
                charClass2 = ParseSupport.GetCharClass(c);
                if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!endOfFile || num + 2 < num2))
                {
                    parseThreshold = 3;
                    goto IL_F2D;
                }
                num++;
                flag = true;
            }
            else if (!ParseSupport.AlphaCharacter(charClass2) || literalTags)
            {
                if (c == '!')
                {
                    parseState = ParseState.CommentStart;
                    goto IL_ABE;
                }
                if (c == '?' && !literalTags)
                {
                    num += 2;
                    htmlTokenBuilder.StartTag(HtmlNameIndex._DTD, num3);
                    htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                    htmlTokenBuilder.StartTagText();
                    lastCharClass = charClass2;
                    ch = array[num];
                    charClass = ParseSupport.GetCharClass(ch);
                    num3 = num;
                    parseState = ParseState.Dtd;
                    goto IL_D73;
                }
                if (c == '%')
                {
                    num += 2;
                    htmlTokenBuilder.StartTag(HtmlNameIndex._ASP, num3);
                    htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                    htmlTokenBuilder.StartTagText();
                    ch = array[num];
                    charClass = ParseSupport.GetCharClass(ch);
                    num3 = num;
                    parseState = ParseState.Asp;
                    goto IL_D73;
                }
                if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!endOfFile || num + 1 < num2))
                {
                    parseThreshold = 2;
                    goto IL_F2D;
                }
                parseState = ParseState.Text;
                goto IL_A0;
            }
            num++;
            lastCharClass = charClass;
            ch = c;
            charClass = charClass2;
            htmlTokenBuilder.StartTag(HtmlNameIndex.Unknown, num3);
            if (flag)
            {
                htmlTokenBuilder.SetEndTag();
            }
            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
            nameLength = 0;
            htmlTokenBuilder.StartTagName();
            num3 = num;
            parseState = ParseState.TagNamePrefix;
            IL_28A:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.Name))
            {
                goto IL_A53;
            }
            ch = ScanTagName(ch, ref charClass, ref num, CharClass.HtmlTagNamePrefix);
            if (num != num3)
            {
                nameLength += num - num3;
                if (literalTags && (nameLength > 14 || ch == '<'))
                {
                    goto IL_A80;
                }
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            if (ch != ':')
            {
                goto IL_3D1;
            }
            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
            nameLength++;
            tokenBuilder.EndTagNamePrefix();
            ch = array[++num];
            charClass = ParseSupport.GetCharClass(ch);
            num3 = num;
            parseState = ParseState.TagName;
            IL_358:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.Name))
            {
                goto IL_A53;
            }
            ch = ScanTagName(ch, ref charClass, ref num, CharClass.HtmlTagName);
            if (num != num3)
            {
                nameLength += num - num3;
                if (literalTags && (nameLength > 14 || ch == '<'))
                {
                    goto IL_A80;
                }
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            IL_3D1:
            htmlTokenBuilder.EndTagName(nameLength);
            if (literalTags && token.NameIndex != literalTagNameId)
            {
                goto IL_A80;
            }
            num3 = num;
            if (ch == '>')
            {
                parseState = ParseState.TagEnd;
                goto IL_8EB;
            }
            if (ch == '/')
            {
                parseState = ParseState.EmptyTagEnd;
                goto IL_85B;
            }
            lastCharClass = charClass;
            parseState = ParseState.TagWsp;
            IL_431:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagWhitespace))
            {
                goto IL_A53;
            }
            ch = ScanWhitespace(ch, ref charClass, ref num);
            if (num != num3)
            {
                htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            num3 = num;
            if (ch == '>')
            {
                parseState = ParseState.TagEnd;
                goto IL_8EB;
            }
            if (ch == '/')
            {
                parseState = ParseState.EmptyTagEnd;
                goto IL_85B;
            }
            parseState = ParseState.AttrNameStart;
            IL_4A5:
            if (!htmlTokenBuilder.CanAddAttribute() || !htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Name))
            {
                goto IL_A53;
            }
            nameLength = 0;
            htmlTokenBuilder.StartAttribute();
            parseState = ParseState.AttrNamePrefix;
            IL_4D7:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Name))
            {
                goto IL_A53;
            }
            ch = ScanAttrName(ch, ref charClass, ref num, CharClass.HtmlAttrNamePrefix);
            if (num != num3)
            {
                nameLength += num - num3;
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            if (ch != ':')
            {
                goto IL_5E4;
            }
            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.NamePrefixDelimiter, num, num + 1);
            nameLength++;
            tokenBuilder.EndAttributeNamePrefix();
            ch = array[++num];
            charClass = ParseSupport.GetCharClass(ch);
            num3 = num;
            parseState = ParseState.AttrName;
            IL_588:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.Name))
            {
                goto IL_A53;
            }
            ch = ScanAttrName(ch, ref charClass, ref num, CharClass.HtmlAttrName);
            if (num != num3)
            {
                nameLength += num - num3;
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.Name, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            IL_5E4:
            htmlTokenBuilder.EndAttributeName(nameLength);
            num3 = num;
            if (ch != '=')
            {
                lastCharClass = charClass;
                parseState = ParseState.AttrWsp;
                goto IL_60A;
            }
            IL_60A:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagWhitespace))
            {
                goto IL_A53;
            }
            ch = ScanWhitespace(ch, ref charClass, ref num);
            if (num != num3)
            {
                htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
            }
            if (ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                goto IL_9F3;
            }
            num3 = num;
            if (ch == '=')
            {
                goto IL_68E;
            }
            htmlTokenBuilder.EndAttribute();
            if (ch == '>')
            {
                parseState = ParseState.TagEnd;
                goto IL_8EB;
            }
            if (ch == '/')
            {
                parseState = ParseState.EmptyTagEnd;
                goto IL_85B;
            }
            parseState = ParseState.AttrNameStart;
            goto IL_4A5;
            IL_68E:
            lastCharClass = charClass;
            ch = array[++num];
            charClass = ParseSupport.GetCharClass(ch);
            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrEqual, num3, num);
            num3 = num;
            parseState = ParseState.AttrValueWsp;
            goto IL_6C8;
            IL_6C8:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.TagWhitespace))
            {
                goto IL_A53;
            }
            ch = ScanWhitespace(ch, ref charClass, ref num);
            if (num != num3)
            {
                htmlTokenBuilder.AddRun(RunTextType.Space, HtmlRunKind.TagWhitespace, num3, num);
            }
            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                num3 = num;
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0')
                    {
                        scanQuote = ch;
                    }
                    valueQuote = ch;
                    lastCharClass = charClass;
                    ch = array[++num];
                    charClass = ParseSupport.GetCharClass(ch);
                    htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num);
                    num3 = num;
                }
                htmlTokenBuilder.StartValue();
                if (valueQuote != '\0')
                {
                    htmlTokenBuilder.SetValueQuote(valueQuote);
                }
                parseState = ParseState.AttrValue;
                goto IL_795;
            }
            goto IL_9F3;
            IL_795:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.AttrValue) || !ParseAttributeText(ch, charClass, ref num))
            {
                goto IL_A53;
            }
            ch = array[num];
            charClass = ParseSupport.GetCharClass(ch);
            if (ParseSupport.InvalidUnicodeCharacter(charClass) || parseThreshold > 1)
            {
                goto IL_9F3;
            }
            htmlTokenBuilder.EndValue();
            num3 = num;
            if (ch == valueQuote)
            {
                lastCharClass = charClass;
                ch = array[++num];
                charClass = ParseSupport.GetCharClass(ch);
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.AttrQuote, num3, num);
                valueQuote = '\0';
                num3 = num;
            }
            htmlTokenBuilder.EndAttribute();
            if (ch == '>')
            {
                parseState = ParseState.TagEnd;
                goto IL_8EB;
            }
            if (ch != '/')
            {
                parseState = ParseState.TagWsp;
                goto IL_431;
            }
            parseState = ParseState.EmptyTagEnd;
            IL_85B:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagWhitespace))
            {
                goto IL_A53;
            }
            c = array[num + 1];
            charClass2 = ParseSupport.GetCharClass(c);
            if (c == '>')
            {
                htmlTokenBuilder.SetEmptyScope();
                num++;
                lastCharClass = charClass;
                ch = c;
                charClass = charClass2;
                parseState = ParseState.TagEnd;
            }
            else
            {
                if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!endOfFile || num + 1 < num2))
                {
                    parseThreshold = 2;
                    goto IL_9F3;
                }
                lastCharClass = charClass;
                num++;
                ch = c;
                charClass = charClass2;
                num3 = num;
                parseState = ParseState.TagWsp;
                goto IL_431;
            }
            IL_8EB:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagSuffix))
            {
                goto IL_A53;
            }
            lastCharClass = charClass;
            num++;
            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num3, num);
            if (scanQuote == '\0')
            {
                htmlTokenBuilder.EndTag(true);
                if (array[num] == '<')
                {
                    parseState = ParseState.TagStart;
                    slowParse = false;
                }
                else
                {
                    parseState = ParseState.Text;
                }
                parseCurrent = num;
                HandleSpecialTag();
                return true;
            }
            num3 = num;
            ch = array[num];
            charClass = ParseSupport.GetCharClass(ch);
            parseState = ParseState.TagSkip;
            IL_979:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(1, num3, HtmlRunKind.TagText))
            {
                goto IL_A53;
            }
            ch = ScanSkipTag(ch, ref charClass, ref num);
            if (num != num3)
            {
                htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
            }
            if (!ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                num++;
                htmlTokenBuilder.EndTag(true);
                if (array[num] == '<')
                {
                    parseState = ParseState.TagStart;
                    slowParse = false;
                }
                else
                {
                    parseState = ParseState.Text;
                }
                parseCurrent = num;
                HandleSpecialTag();
                return true;
            }
            IL_9F3:
            if (!forceFlushToken || num + parseThreshold < num2)
            {
                goto IL_F2D;
            }
            if (endOfFile)
            {
                if (num < num2)
                {
                    if (ScanForInternalInvalidCharacters(num))
                    {
                        goto IL_F2D;
                    }
                    num = num2;
                }
                if (!token.IsTagBegin)
                {
                    htmlTokenBuilder.EndTag(true);
                    parseCurrent = num;
                    HandleSpecialTag();
                    parseState = ParseState.Text;
                    return true;
                }
                goto IL_A80;
            }
            IL_A53:
            if (!literalTags || token.NameIndex != HtmlNameIndex.Unknown)
            {
                htmlTokenBuilder.EndTag(false);
                parseCurrent = num;
                HandleSpecialTag();
                return true;
            }
            IL_A80:
            num = parseStart;
            scanQuote = (valueQuote = '\0');
            htmlTokenBuilder.Reset();
            num3 = num;
            ch = array[num];
            charClass = ParseSupport.GetCharClass(ch);
            parseState = ParseState.Text;
            goto IL_A0;
            IL_ABE:
            int num4 = 2;
            c = array[num + num4];
            if (c == '-')
            {
                num4++;
                c = array[num + num4];
                if (c == '-')
                {
                    num4++;
                    c = array[num + num4];
                    /*
                    if (c == '[' && parseConditionals)
                    {
                        num += 5;
                        htmlTokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, num3);
                        htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                        htmlTokenBuilder.StartTagText();
                        ch = array[num];
                        charClass = ParseSupport.GetCharClass(ch);
                        num3 = num;
                        parseState = ParseState.CommentConditional;
                        goto IL_D73;
                    }
                    */
                    if (c == '>')
                    {
                        num += 5;
                        htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
                        htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num - 1);
                        htmlTokenBuilder.StartTagText();
                        htmlTokenBuilder.EndTagText();
                        htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num - 1, num);
                        htmlTokenBuilder.EndTag(true);
                        parseState = ParseState.Text;
                        parseCurrent = num;
                        return true;
                    }
                    if (c == '-')
                    {
                        num4++;
                        c = array[num + num4];
                        if (c == '>')
                        {
                            num += 6;
                            htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
                            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num - 2);
                            htmlTokenBuilder.StartTagText();
                            htmlTokenBuilder.EndTagText();
                            htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num - 2, num);
                            htmlTokenBuilder.EndTag(true);
                            parseState = ParseState.Text;
                            parseCurrent = num;
                            return true;
                        }
                    }
                    charClass2 = ParseSupport.GetCharClass(c);
                    if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
                    {
                        num += 4;
                        htmlTokenBuilder.StartTag(HtmlNameIndex._COMMENT, num3);
                        htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                        htmlTokenBuilder.StartTagText();
                        ch = array[num];
                        charClass = ParseSupport.GetCharClass(ch);
                        num3 = num;
                        parseState = ParseState.Comment;
                        goto IL_D73;
                    }
                }
            }
            /*
            else if (c == '[' && parseConditionals)
            {
                num += 3;
                htmlTokenBuilder.StartTag(HtmlNameIndex._CONDITIONAL, num3);
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                htmlTokenBuilder.StartTagText();
                ch = array[num];
                charClass = ParseSupport.GetCharClass(ch);
                num3 = num;
                parseState = ParseState.Conditional;
                goto IL_D73;
            }
            */
            charClass2 = ParseSupport.GetCharClass(c);
            if (ParseSupport.InvalidUnicodeCharacter(charClass2))
            {
                if (!endOfFile || num + num4 < num2)
                {
                    parseThreshold = num4 + 1;
                    goto IL_F2D;
                }
                parseState = ParseState.Text;
                goto IL_A0;
            }
            else
            {
                if (literalTags)
                {
                    parseState = ParseState.Text;
                    goto IL_A0;
                }
                num += 2;
                htmlTokenBuilder.StartTag(HtmlNameIndex._BANG, num3);
                htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagPrefix, num3, num);
                htmlTokenBuilder.StartTagText();
                lastCharClass = ParseSupport.GetCharClass('!');
                ch = array[num];
                charClass = ParseSupport.GetCharClass(ch);
                num3 = num;
                parseState = ParseState.Bang;
            }
            IL_D73:
            if (!htmlTokenBuilder.PrepareToAddMoreRuns(2, num3, HtmlRunKind.TagText))
            {
                goto IL_A53;
            }
            while (!ParseSupport.InvalidUnicodeCharacter(charClass))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                }
                else if (ParseSupport.HtmlSuffixCharacter(charClass))
                {
                    int num5;
                    int num6;
                    bool flag2;
                    if (!CheckSuffix(num, ch, out num5, out num6, out flag2))
                    {
                        num += num5;
                        parseThreshold = num6 + 1;
                        break;
                    }
                    if (!flag2)
                    {
                        num += num5;
                        lastCharClass = charClass;
                        ch = array[num];
                        charClass = ParseSupport.GetCharClass(ch);
                        continue;
                    }
                    scanQuote = '\0';
                    num += num5;
                    if (num != num3)
                    {
                        htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
                    }
                    htmlTokenBuilder.EndTagText();
                    if (num6 != 0)
                    {
                        num3 = num;
                        num += num6;
                        htmlTokenBuilder.AddRun(RunTextType.NonSpace, HtmlRunKind.TagSuffix, num3, num);
                    }
                    htmlTokenBuilder.EndTag(true);
                    parseState = ParseState.Text;
                    parseCurrent = num;
                    return true;
                }
                lastCharClass = charClass;
                ch = array[++num];
                charClass = ParseSupport.GetCharClass(ch);
            }
            if (num != num3)
            {
                htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num3, num);
                if (!htmlTokenBuilder.PrepareToAddMoreRuns(2))
                {
                    goto IL_A53;
                }
            }
            if (forceFlushToken && num + parseThreshold > num2)
            {
                if (endOfFile && num < num2)
                {
                    htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.TagText, num, num2);
                    num = num2;
                }
                htmlTokenBuilder.EndTag(endOfFile);
                parseCurrent = num;
                return true;
            }
            IL_F2D:
            parseCurrent = num;
            return false;
        }

        private static void ProcessNumericEntityValue(int entityValue, out int literal)
        {
            if (entityValue < 65536)
            {
                if (128 <= entityValue && entityValue <= 159)
                {
                    literal = ParseSupport.Latin1MappingInUnicodeControlArea(entityValue);
                    return;
                }
                if (ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass((char)entityValue)))
                {
                    literal = 63;
                    return;
                }
                literal = entityValue;
                return;
            }
            else
            {
                if (entityValue < 1114112)
                {
                    literal = entityValue;
                    return;
                }
                literal = 63;
                return;
            }
        }

        private static bool FindEntityByHashName(short hash, char[] buffer, int nameOffset, int nameLength, out int entityValue)
        {
            entityValue = 0;
            bool result = false;
            HtmlEntityIndex htmlEntityIndex = HtmlNameData.entityHashTable[(int)hash];
            if (htmlEntityIndex > (HtmlEntityIndex)0)
            {
                while (true)
                {
                    if (HtmlNameData.entities[(int)htmlEntityIndex].name.Length == nameLength)
                    {
                        int num = 0;
                        while (num < nameLength && HtmlNameData.entities[(int)htmlEntityIndex].name[num] == buffer[nameOffset + num])
                        {
                            num++;
                        }
                        if (num == nameLength)
                        {
                            break;
                        }
                    }
                    htmlEntityIndex += 1;
                    if (HtmlNameData.entities[(int)htmlEntityIndex].hash != hash)
                    {
                        return result;
                    }
                }
                entityValue = (int)HtmlNameData.entities[(int)htmlEntityIndex].value;
                result = true;
            }
            return result;
        }

        private bool SkipInvalidCharacters(ref char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseCurrent;
            int num2 = parseEnd;
            while (ParseSupport.InvalidUnicodeCharacter(charClass) && num < num2)
            {
                ch = parseBuffer[++num];
                charClass = ParseSupport.GetCharClass(ch);
            }
            if (parseThreshold > 1 && num + 1 < num2)
            {
                int num3 = num + 1;
                int num4 = num3;
                while (num4 < num2 && num3 < num + parseThreshold)
                {
                    char c = parseBuffer[num4];
                    CharClass charClass2 = ParseSupport.GetCharClass(c);
                    if (!ParseSupport.InvalidUnicodeCharacter(charClass2))
                    {
                        if (num4 != num3)
                        {
                            parseBuffer[num3] = c;
                            parseBuffer[num4] = '\0';
                        }
                        num3++;
                    }
                    num4++;
                }
                if (num4 == num2)
                {
                    num2 = (parseEnd = input.RemoveGap(num3, num2));
                }
            }
            parseCurrent = num;
            return num + parseThreshold <= num2;
        }

        private char ScanTagName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            char[] array = parseBuffer;
            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                }
                else if (ch == '<' && literalTags)
                {
                    break;
                }
                lastCharClass = charClass;
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanAttrName(char ch, ref CharClass charClass, ref int parseCurrent, CharClass acceptCharClassSet)
        {
            char[] array = parseBuffer;
            while (ParseSupport.IsCharClassOneOf(charClass, acceptCharClassSet))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                    if (ch != '`')
                    {
                        array[parseCurrent] = '?';
                    }
                }
                lastCharClass = charClass;
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            while (ParseSupport.WhitespaceCharacter(charClass))
            {
                lastCharClass = charClass;
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanText(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            while (ParseSupport.HtmlTextCharacter(charClass))
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanAttrValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            while (ParseSupport.HtmlAttrValueCharacter(charClass))
            {
                lastCharClass = charClass;
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private char ScanSkipTag(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            while (!ParseSupport.InvalidUnicodeCharacter(charClass) && (ch != '>' || scanQuote != '\0'))
            {
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                }
                lastCharClass = charClass;
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return ch;
        }

        private bool ScanForInternalInvalidCharacters(int parseCurrent)
        {
            char[] array = parseBuffer;
            char ch;
            do
            {
                ch = array[parseCurrent++];
            }
            while (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(ch)));
            parseCurrent--;
            return parseCurrent < parseEnd;
        }

        private void ParseText(char ch, CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            HtmlTokenBuilder htmlTokenBuilder = tokenBuilder;
            int num2 = parseCurrent;
            int num3 = num2;
            while (true)
            {
                ch = ScanText(ch, ref charClass, ref parseCurrent);
                if (ParseSupport.WhitespaceCharacter(charClass))
                {
                    if (parseCurrent != num3)
                    {
                        htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
                        num3 = parseCurrent;
                    }
                    if (ch == ' ')
                    {
                        char c = array[parseCurrent + 1];
                        CharClass charClass2 = ParseSupport.GetCharClass(c);
                        if (!ParseSupport.WhitespaceCharacter(charClass2))
                        {
                            ch = c;
                            charClass = charClass2;
                            parseCurrent++;
                            htmlTokenBuilder.AddTextRun(RunTextType.Space, num3, parseCurrent);
                            num3 = parseCurrent;
                            goto IL_338;
                        }
                    }
                    ParseWhitespace(ch, charClass, ref parseCurrent);
                    if (parseThreshold > 1)
                    {
                        break;
                    }
                    ch = array[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    goto IL_334;
                }
                else if (ch == '<')
                {
                    if (!plaintext && num2 != parseCurrent)
                    {
                        goto IL_E4;
                    }
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                }
                else if (ch == '&')
                {
                    if (literalEntities)
                    {
                        ch = array[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                    else
                    {
                        int num4;
                        int num5;
                        if (!DecodeEntity(parseCurrent, false, out num4, out num5))
                        {
                            goto IL_286;
                        }
                        if (num5 != 1)
                        {
                            if (parseCurrent != num3)
                            {
                                htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
                            }
                            if (num4 <= 65535 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass((char)num4)))
                            {
                                char c2 = (char)num4;
                                switch (c2)
                                {
                                    case '\t':
                                        htmlTokenBuilder.AddLiteralTextRun(RunTextType.Tabulation, parseCurrent, parseCurrent + num5, num4);
                                        goto IL_26C;
                                    case '\n':
                                        htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + num5, num4);
                                        goto IL_26C;
                                    case '\v':
                                    case '\f':
                                        break;
                                    case '\r':
                                        htmlTokenBuilder.AddLiteralTextRun(RunTextType.NewLine, parseCurrent, parseCurrent + num5, num4);
                                        goto IL_26C;
                                    default:
                                        if (c2 == ' ')
                                        {
                                            htmlTokenBuilder.AddLiteralTextRun(RunTextType.Space, parseCurrent, parseCurrent + num5, num4);
                                            goto IL_26C;
                                        }
                                        break;
                                }
                                htmlTokenBuilder.AddLiteralTextRun(RunTextType.UnusualWhitespace, parseCurrent, parseCurrent + num5, num4);
                            }
                            else if (num4 == 160)
                            {
                                htmlTokenBuilder.AddLiteralTextRun(RunTextType.Nbsp, parseCurrent, parseCurrent + num5, num4);
                            }
                            else
                            {
                                htmlTokenBuilder.AddLiteralTextRun(RunTextType.NonSpace, parseCurrent, parseCurrent + num5, num4);
                            }
                            IL_26C:
                            parseCurrent += num5;
                            ch = array[parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                            goto IL_334;
                        }
                        ch = array[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                    }
                }
                else
                {
                    if (ParseSupport.NbspCharacter(charClass))
                    {
                        if (parseCurrent != num3)
                        {
                            htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
                        }
                        num3 = parseCurrent;
                        do
                        {
                            ch = array[++parseCurrent];
                            charClass = ParseSupport.GetCharClass(ch);
                        }
                        while (ParseSupport.NbspCharacter(charClass));
                        htmlTokenBuilder.AddTextRun(RunTextType.Nbsp, num3, parseCurrent);
                        goto IL_334;
                    }
                    if (parseCurrent != num3)
                    {
                        htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
                    }
                    if (parseCurrent >= num)
                    {
                        return;
                    }
                    while (true)
                    {
                        ch = array[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                        if (!ParseSupport.InvalidUnicodeCharacter(charClass) || parseCurrent >= num)
                        {
                            goto IL_334;
                        }
                    }
                }
                IL_338:
                if (!htmlTokenBuilder.PrepareToAddMoreRuns(3, num3, HtmlRunKind.Text))
                {
                    return;
                }
                continue;
                IL_334:
                num3 = parseCurrent;
                goto IL_338;
            }
            return;
            IL_E4:
            if (parseCurrent != num3)
            {
                htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
            }
            parseState = ParseState.TagStart;
            slowParse = literalTags;
            return;
            IL_286:
            if (parseCurrent != num3)
            {
                htmlTokenBuilder.AddTextRun(RunTextType.NonSpace, num3, parseCurrent);
            }
            parseThreshold = 10;
        }

        private bool ParseAttributeText(char ch, CharClass charClass, ref int parseCurrent)
        {
            int num = parseCurrent;
            char[] array = parseBuffer;
            HtmlTokenBuilder htmlTokenBuilder = tokenBuilder;
            while (true)
            {
                ch = ScanAttrValue(ch, ref charClass, ref parseCurrent);
                if (ParseSupport.QuoteCharacter(charClass))
                {
                    if (ch == scanQuote)
                    {
                        scanQuote = '\0';
                    }
                    else if (scanQuote == '\0' && ParseSupport.HtmlScanQuoteSensitiveCharacter(lastCharClass))
                    {
                        scanQuote = ch;
                    }
                    lastCharClass = charClass;
                    if (ch == valueQuote)
                    {
                        goto IL_151;
                    }
                    parseCurrent++;
                }
                else if (ch == '&')
                {
                    lastCharClass = charClass;
                    int literal;
                    int num2;
                    if (!DecodeEntity(parseCurrent, true, out literal, out num2))
                    {
                        goto IL_EA;
                    }
                    if (num2 == 1)
                    {
                        ch = array[++parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                        continue;
                    }
                    if (parseCurrent != num)
                    {
                        htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num, parseCurrent);
                    }
                    htmlTokenBuilder.AddLiteralRun(RunTextType.Unknown, HtmlRunKind.AttrValue, parseCurrent, parseCurrent + num2, literal);
                    parseCurrent += num2;
                    if (!htmlTokenBuilder.PrepareToAddMoreRuns(2))
                    {
                        break;
                    }
                    num = parseCurrent;
                }
                else if (ch == '>')
                {
                    lastCharClass = charClass;
                    if (valueQuote == '\0')
                    {
                        goto IL_151;
                    }
                    if (scanQuote == '\0')
                    {
                        goto Block_13;
                    }
                    parseCurrent++;
                }
                else
                {
                    if (!ParseSupport.WhitespaceCharacter(charClass))
                    {
                        goto IL_151;
                    }
                    lastCharClass = charClass;
                    if (valueQuote == '\0')
                    {
                        goto IL_151;
                    }
                    parseCurrent++;
                }
                ch = array[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            return false;
            IL_EA:
            parseThreshold = 10;
            goto IL_151;
            Block_13:
            valueQuote = '\0';
            IL_151:
            if (parseCurrent != num)
            {
                htmlTokenBuilder.AddRun(RunTextType.Unknown, HtmlRunKind.AttrValue, num, parseCurrent);
            }
            return true;
        }

        private void ParseWhitespace(char ch, CharClass charClass, ref int parseCurrent)
        {
            int start = parseCurrent;
            char[] array = parseBuffer;
            HtmlTokenBuilder htmlTokenBuilder = tokenBuilder;
            do
            {
                char c = ch;
                switch (c)
                {
                    case '\t':
                        do
                        {
                            ch = array[++parseCurrent];
                        }
                        while (ch == '\t');
                        htmlTokenBuilder.AddTextRun(RunTextType.Tabulation, start, parseCurrent);
                        goto IL_12F;
                    case '\n':
                        ch = array[++parseCurrent];
                        htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
                        goto IL_12F;
                    case '\v':
                    case '\f':
                        break;
                    case '\r':
                        if (array[parseCurrent + 1] != '\n')
                        {
                            CharClass charClass2 = ParseSupport.GetCharClass(array[parseCurrent + 1]);
                            if (ParseSupport.InvalidUnicodeCharacter(charClass2) && (!endOfFile || parseCurrent + 1 < parseEnd))
                            {
                                parseThreshold = 2;
                                goto IL_12F;
                            }
                        }
                        else
                        {
                            parseCurrent++;
                        }
                        ch = array[++parseCurrent];
                        htmlTokenBuilder.AddTextRun(RunTextType.NewLine, start, parseCurrent);
                        goto IL_12F;
                    default:
                        if (c == ' ')
                        {
                            do
                            {
                                ch = array[++parseCurrent];
                            }
                            while (ch == ' ');
                            htmlTokenBuilder.AddTextRun(RunTextType.Space, start, parseCurrent);
                            goto IL_12F;
                        }
                        break;
                }
                do
                {
                    ch = array[++parseCurrent];
                }
                while (ch == '\f' || ch == '\v');
                htmlTokenBuilder.AddTextRun(RunTextType.UnusualWhitespace, start, parseCurrent);
                IL_12F:
                charClass = ParseSupport.GetCharClass(ch);
                start = parseCurrent;
            }
            while (ParseSupport.WhitespaceCharacter(charClass) && htmlTokenBuilder.PrepareToAddMoreRuns(1) && parseThreshold == 1);
        }

        private bool CheckSuffix(int parseCurrent, char ch, out int addToTextCnt, out int tagSuffixCnt, out bool endScan)
        {
            addToTextCnt = 1;
            tagSuffixCnt = 0;
            endScan = false;
            char c;
            switch (parseState)
            {
                case ParseState.Comment:
                    break;
                case ParseState.Conditional:
                case ParseState.CommentConditional:
                    if (ch == '>')
                    {
                        parseState = ((parseState == ParseState.CommentConditional) ? ParseState.Comment : ParseState.Bang);
                        tokenBuilder.AbortConditional(parseState == ParseState.Comment);
                        return CheckSuffix(parseCurrent, ch, out addToTextCnt, out tagSuffixCnt, out endScan);
                    }
                    if (ch != '-' || parseState != ParseState.CommentConditional)
                    {
                        if (ch != ']')
                        {
                            return true;
                        }
                        c = parseBuffer[parseCurrent + 1];
                        if (c == '>')
                        {
                            addToTextCnt = 0;
                            tagSuffixCnt = 2;
                            endScan = true;
                            return true;
                        }
                        int num = 1;
                        if (c == '-')
                        {
                            num++;
                            c = parseBuffer[parseCurrent + 2];
                            if (c == '-')
                            {
                                num++;
                                c = parseBuffer[parseCurrent + 3];
                                if (c == '>')
                                {
                                    addToTextCnt = 0;
                                    tagSuffixCnt = 4;
                                    endScan = true;
                                    return true;
                                }
                            }
                        }
                        if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
                        {
                            addToTextCnt = num;
                            return true;
                        }
                        addToTextCnt = 0;
                        tagSuffixCnt = num;
                        return false;
                    }
                    break;
                case ParseState.Bang:
                case ParseState.Dtd:
                    if (ch == '>' && scanQuote == '\0')
                    {
                        addToTextCnt = 0;
                        tagSuffixCnt = 1;
                        endScan = true;
                    }
                    return true;
                case ParseState.Asp:
                    if (ch != '%')
                    {
                        return true;
                    }
                    c = parseBuffer[parseCurrent + 1];
                    if (c == '>')
                    {
                        addToTextCnt = 0;
                        tagSuffixCnt = 2;
                        endScan = true;
                        return true;
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
                    {
                        return true;
                    }
                    addToTextCnt = 0;
                    tagSuffixCnt = 1;
                    return false;
                default:
                    return true;
            }
            if (ch != '-')
            {
                return true;
            }
            int num2 = parseCurrent;
            do
            {
                c = parseBuffer[++num2];
            }
            while (c == '-');
            if (c == '>' && num2 - parseCurrent >= 2)
            {
                if (parseState == ParseState.CommentConditional)
                {
                    parseState = ParseState.Comment;
                    tokenBuilder.AbortConditional(true);
                }
                addToTextCnt = num2 - parseCurrent - 2;
                tagSuffixCnt = 3;
                endScan = true;
                return true;
            }
            if (!ParseSupport.InvalidUnicodeCharacter(ParseSupport.GetCharClass(c)))
            {
                addToTextCnt = num2 - parseCurrent;
                return true;
            }
            addToTextCnt = ((num2 - parseCurrent > 2) ? (num2 - parseCurrent - 2) : 0);
            tagSuffixCnt = num2 - parseCurrent - addToTextCnt;
            return false;
        }

        private bool DecodeEntity(int parseCurrent, bool inAttribute, out int literal, out int consume)
        {
            char[] array = parseBuffer;
            int num = parseCurrent + 1;
            int num2 = num;
            int num3 = 0;
            int num4 = 0;
            char c = array[num2];
            CharClass charClass = ParseSupport.GetCharClass(c);
            if (c == '#')
            {
                c = array[++num2];
                charClass = ParseSupport.GetCharClass(c);
                if (c == 'x' || c == 'X')
                {
                    c = array[++num2];
                    charClass = ParseSupport.GetCharClass(c);
                    while (ParseSupport.HexCharacter(charClass))
                    {
                        num3++;
                        num4 = (num4 << 4) + ParseSupport.CharToHex(c);
                        c = array[++num2];
                        charClass = ParseSupport.GetCharClass(c);
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (endOfFile && num2 >= parseEnd) || num3 > 6)
                    {
                        if ((inAttribute || c == ';') && num4 != 0 && num3 <= 6)
                        {
                            HtmlParser.ProcessNumericEntityValue(num4, out literal);
                            consume = num2 - parseCurrent;
                            if (c == ';')
                            {
                                consume++;
                            }
                            return true;
                        }
                        literal = 0;
                        consume = 1;
                        return true;
                    }
                }
                else
                {
                    while (ParseSupport.NumericCharacter(charClass))
                    {
                        num3++;
                        num4 = num4 * 10 + ParseSupport.CharToDecimal(c);
                        num2++;
                        c = array[num2];
                        charClass = ParseSupport.GetCharClass(c);
                    }
                    if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (endOfFile && num2 >= parseEnd) || num3 > 7)
                    {
                        if (num4 != 0 && num3 <= 7)
                        {
                            HtmlParser.ProcessNumericEntityValue(num4, out literal);
                            consume = num2 - parseCurrent;
                            if (c == ';')
                            {
                                consume++;
                            }
                            return true;
                        }
                        literal = 0;
                        consume = 1;
                        return true;
                    }
                }
            }
            else
            {
                short[] array2 = hashValuesTable;
                if (array2 == null)
                {
                    array2 = (hashValuesTable = new short[8]);
                }
                HashCode hashCode = new HashCode(true);
                while (ParseSupport.HtmlEntityCharacter(charClass) && num3 < 8)
                {
                    short num5 = (short)((ulong)(hashCode.AdvanceAndFinalizeHash(c) ^ 230) % 705uL);
                    array2[num3++] = num5;
                    num2++;
                    c = array[num2];
                    charClass = ParseSupport.GetCharClass(c);
                }
                if (!ParseSupport.InvalidUnicodeCharacter(charClass) || (endOfFile && num2 >= parseEnd))
                {
                    if (num3 > 1)
                    {
                        int num6;
                        if (HtmlParser.FindEntityByHashName(array2[num3 - 1], array, num, num3, out num6) && (c == ';' || num6 <= 255))
                        {
                            num4 = num6;
                        }
                        else if (!inAttribute)
                        {
                            for (int i = num3 - 2; i >= 0; i--)
                            {
                                if (HtmlParser.FindEntityByHashName(array2[i], array, num, i + 1, out num6) && num6 <= 255)
                                {
                                    num4 = num6;
                                    num3 = i + 1;
                                    break;
                                }
                            }
                        }
                        if (num4 != 0)
                        {
                            literal = num4;
                            consume = num3 + 1;
                            if (array[num + num3] == ';')
                            {
                                consume++;
                            }
                            return true;
                        }
                    }
                    literal = 0;
                    consume = 1;
                    return true;
                }
            }
            literal = 0;
            consume = 0;
            return false;
        }

        private void HandleSpecialTag()
        {
            if (HtmlNameData.names[(int)token.NameIndex].literalTag)
            {
                literalTags = !token.IsEndTag;
                literalTagNameId = (literalTags ? token.NameIndex : HtmlNameIndex.Unknown);
                if (HtmlNameData.names[(int)token.NameIndex].literalEnt)
                {
                    literalEntities = (!token.IsEndTag && !token.IsEmptyScope);
                }
                slowParse = (slowParse || literalTags);
            }
            HtmlNameIndex nameIndex = token.NameIndex;
            if (nameIndex != HtmlNameIndex.Meta)
            {
                if (nameIndex != HtmlNameIndex.PlainText)
                {
                    return;
                }
                if (!token.IsEndTag)
                {
                    plaintext = true;
                    literalEntities = true;
                    if (token.IsTagEnd)
                    {
                        parseState = ParseState.Text;
                    }
                }
            }
            else if (input is ConverterDecodingInput && detectEncodingFromMetaTag && ((IRestartable)this).CanRestart())
            {
                if (token.IsTagBegin)
                {
                    rightMeta = false;
                    newEncoding = null;
                }
                token.Attributes.Rewind();
                int num = -1;
                bool lookForWordCharset = false;
                HtmlToken.AttributeEnumerator enumerator = token.Attributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    HtmlAttribute current = enumerator.Current;
                    if (current.NameIndex == HtmlNameIndex.HttpEquiv)
                    {
                        if (!current.Value.CaseInsensitiveCompareEqual("content-type") && !current.Value.CaseInsensitiveCompareEqual("charset"))
                        {
                            break;
                        }
                        rightMeta = true;
                        if (num != -1)
                        {
                            break;
                        }
                    }
                    else if (current.NameIndex == HtmlNameIndex.Content)
                    {
                        num = current.Index;
                        lookForWordCharset = true;
                        if (rightMeta)
                        {
                            break;
                        }
                    }
                    else if (current.NameIndex == HtmlNameIndex.Charset)
                    {
                        num = current.Index;
                        lookForWordCharset = false;
                        rightMeta = true;
                        break;
                    }
                }
                if (num != -1)
                {
                    string @string = token.Attributes[num].Value.GetString(100);
                    string text = HtmlParser.CharsetFromString(@string, lookForWordCharset);
                    if (text != null)
                    {
                        Charset.TryGetEncoding(text, out newEncoding);
                    }
                }
                if (rightMeta && newEncoding != null)
                {
                    (input as ConverterDecodingInput).RestartWithNewEncoding(newEncoding);
                }
                token.Attributes.Rewind();
                return;
            }
        }

        private static string CharsetFromString(string arg, bool lookForWordCharset)
        {
            for (int i = 0; i < arg.Length; i++)
            {
                while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                {
                    i++;
                }
                if (i == arg.Length)
                {
                    break;
                }
                if (!lookForWordCharset || (arg.Length - i >= 7 && string.Equals(arg.Substring(i, 7), "charset", StringComparison.OrdinalIgnoreCase)))
                {
                    if (lookForWordCharset)
                    {
                        i = arg.IndexOf('=', i + 7);
                        if (i < 0)
                        {
                            break;
                        }
                        i++;
                        while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
                        {
                            i++;
                        }
                        if (i == arg.Length)
                        {
                            break;
                        }
                    }
                    int num = i;
                    while (num < arg.Length && arg[num] != ';' && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[num])))
                    {
                        num++;
                    }
                    return arg.Substring(i, num - i);
                }
                i = arg.IndexOf(';', i);
                if (i < 0)
                {
                    break;
                }
            }
            return null;
        }
    }
}
