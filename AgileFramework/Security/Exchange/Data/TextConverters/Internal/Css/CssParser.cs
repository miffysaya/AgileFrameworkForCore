using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal class CssParser : IDisposable
    {
        internal const int MaxCssLength = 524288;

        private ConverterInput input;

        private bool endOfFile;

        private CssParseMode parseMode;

        private bool isInvalid;

        private char[] parseBuffer;

        private int parseStart;

        private int parseCurrent;

        private int parseEnd;

        private int ruleDepth;

        protected CssTokenBuilder tokenBuilder;

        private CssToken token;

        private static readonly string[] SafeTermFunctions = new string[]
        {
            "rgb",
            "counter"
        };

        private static readonly string[] SafePseudoFunctions = new string[]
        {
            "lang"
        };

        public CssToken Token
        {
            get
            {
                return token;
            }
        }

        public CssParser(ConverterInput input, int maxRuns, bool testBoundaryConditions)
        {
            this.input = input;
            tokenBuilder = new CssTokenBuilder(null, 256, 256, maxRuns, testBoundaryConditions);
            token = tokenBuilder.Token;
        }

        void IDisposable.Dispose()
        {
            if (input != null)
            {
                ((IDisposable)input).Dispose();
            }
            input = null;
            parseBuffer = null;
            token = null;
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            endOfFile = false;
            parseBuffer = null;
            parseStart = 0;
            parseCurrent = 0;
            parseEnd = 0;
            ruleDepth = 0;
        }

        public void SetParseMode(CssParseMode parseMode)
        {
            this.parseMode = parseMode;
        }

        public CssTokenId Parse()
        {
            if (endOfFile)
            {
                return CssTokenId.EndOfFile;
            }
            tokenBuilder.Reset();
            char[] array = parseBuffer;
            int num = parseCurrent;
            int num2 = parseEnd;
            if (num >= num2)
            {
                input.ReadMore(ref parseBuffer, ref parseStart, ref parseCurrent, ref parseEnd);
                if (parseEnd == 0)
                {
                    return CssTokenId.EndOfFile;
                }
                tokenBuilder.BufferChanged(parseBuffer, parseStart);
                array = parseBuffer;
                num = parseCurrent;
                num2 = parseEnd;
            }
            char ch = array[num];
            CharClass charClass = ParseSupport.GetCharClass(ch);
            int num3 = num;
            if (parseMode == CssParseMode.StyleTag)
            {
                ScanStyleSheet(ch, ref charClass, ref num);
                if (num3 >= num)
                {
                    tokenBuilder.Reset();
                    return CssTokenId.EndOfFile;
                }
                if (tokenBuilder.Incomplete)
                {
                    tokenBuilder.EndRuleSet();
                }
            }
            else
            {
                ScanDeclarations(ch, ref charClass, ref num);
                if (num < num2)
                {
                    endOfFile = true;
                    tokenBuilder.Reset();
                    return CssTokenId.EndOfFile;
                }
                if (tokenBuilder.Incomplete)
                {
                    tokenBuilder.EndDeclarations();
                }
            }
            endOfFile = (num == num2);
            parseCurrent = num;
            return token.TokenId;
        }

        private char ScanStyleSheet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            while (true)
            {
                int num2 = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    break;
                }
                if (IsNameStartCharacter(ch, charClass, parseCurrent) || ch == '*' || ch == '.' || ch == ':' || ch == '#' || ch == '[')
                {
                    ch = ScanRuleSet(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    if (!isInvalid)
                    {
                        return ch;
                    }
                }
                else if (ch == '@')
                {
                    ch = ScanAtRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    if (!isInvalid)
                    {
                        return ch;
                    }
                }
                else if (ch == '/' && parseCurrent < num && array[parseCurrent + 1] == '*')
                {
                    ch = ScanComment(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (ch == '<')
                {
                    ch = ScanCdo(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (ch == '-')
                {
                    ch = ScanCdc(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else
                {
                    isInvalid = true;
                }
                if (isInvalid)
                {
                    isInvalid = false;
                    tokenBuilder.Reset();
                    ch = SkipToNextRule(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                if (num2 >= parseCurrent)
                {
                    return ch;
                }
            }
            return ch;
        }

        private char ScanCdo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            parseCurrent++;
            if (parseCurrent + 3 >= parseEnd)
            {
                parseCurrent = parseEnd;
                return ch;
            }
            if (parseBuffer[parseCurrent++] != '!' || parseBuffer[parseCurrent++] != '-' || parseBuffer[parseCurrent++] != '-')
            {
                return SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private char ScanCdc(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            parseCurrent++;
            if (parseCurrent + 2 >= parseEnd)
            {
                parseCurrent = parseEnd;
                return ch;
            }
            if (parseBuffer[parseCurrent++] != '-' || parseBuffer[parseCurrent++] != '>')
            {
                return SkipToNextRule(ch, ref charClass, ref parseCurrent);
            }
            ch = parseBuffer[parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private char ScanAtRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            int num2 = parseCurrent;
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                isInvalid = true;
                return ch;
            }
            tokenBuilder.StartRuleSet(num2, CssTokenId.AtRule);
            if (!tokenBuilder.CanAddSelector())
            {
                parseCurrent = num;
                return ch;
            }
            tokenBuilder.StartSelectorName();
            PrepareAndAddRun(CssRunKind.AtRuleName, num2, ref parseCurrent);
            if (parseCurrent == num)
            {
                return ch;
            }
            int nameLength;
            ch = ScanName(CssRunKind.AtRuleName, ch, ref charClass, ref parseCurrent, out nameLength);
            tokenBuilder.EndSelectorName(nameLength);
            if (parseCurrent == num)
            {
                return ch;
            }
            if (IsNameEqual("page", num2 + 1, parseCurrent - num2 - 1))
            {
                ch = ScanPageSelector(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            else if (!IsNameEqual("font-face", num2 + 1, parseCurrent - num2 - 1))
            {
                isInvalid = true;
                return ch;
            }
            tokenBuilder.EndSimpleSelector();
            ch = ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == num)
            {
                return ch;
            }
            return ch;
        }

        private char ScanPageSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            if (IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                tokenBuilder.EndSimpleSelector();
                tokenBuilder.StartSelectorName();
                int nameLength;
                ch = ScanName(CssRunKind.PageIdent, ch, ref charClass, ref parseCurrent, out nameLength);
                tokenBuilder.EndSelectorName(nameLength);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                tokenBuilder.SetSelectorCombinator(CssSelectorCombinator.Descendant, false);
            }
            if (ch == ':')
            {
                ch = parseBuffer[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                PrepareAndAddRun(CssRunKind.PagePseudoStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
                if (!IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                    return ch;
                }
                tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
                int num;
                ch = ScanName(CssRunKind.PagePseudo, ch, ref charClass, ref parseCurrent, out num);
                tokenBuilder.EndSelectorClass();
                if (parseCurrent == parseEnd)
                {
                    return ch;
                }
            }
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            return ch;
        }

        private char ScanRuleSet(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            tokenBuilder.StartRuleSet(parseCurrent, CssTokenId.RuleSet);
            ch = ScanSelectors(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd || isInvalid)
            {
                return ch;
            }
            ch = ScanDeclarationBlock(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            return ch;
        }

        private char ScanDeclarationBlock(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            if (ch != '{')
            {
                isInvalid = true;
                return ch;
            }
            ruleDepth++;
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            ch = ScanDeclarations(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            if (ch != '}')
            {
                isInvalid = true;
                return ch;
            }
            ruleDepth--;
            ch = parseBuffer[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == parseEnd)
            {
                return ch;
            }
            return ch;
        }

        private char ScanSelectors(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            int i = parseCurrent;
            ch = ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
            if (parseCurrent == num || isInvalid)
            {
                return ch;
            }
            while (i < parseCurrent)
            {
                CssSelectorCombinator combinator = CssSelectorCombinator.None;
                bool flag = false;
                bool flag2 = false;
                i = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (i < parseCurrent)
                {
                    flag = true;
                    combinator = CssSelectorCombinator.Descendant;
                }
                if (ch == '+' || ch == '>' || ch == ',')
                {
                    combinator = ((ch == '+') ? CssSelectorCombinator.Adjacent : ((ch == '>') ? CssSelectorCombinator.Child : CssSelectorCombinator.None));
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    PrepareAndAddRun(CssRunKind.SelectorCombinatorOrComma, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    flag2 = true;
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (i == parseCurrent)
                {
                    break;
                }
                i = parseCurrent;
                ch = ScanSimpleSelector(ch, ref charClass, ref parseCurrent);
                if (i == parseCurrent)
                {
                    if (flag2)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorCombinatorOrComma);
                    }
                    if (flag)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                        break;
                    }
                    break;
                }
                else
                {
                    if (isInvalid)
                    {
                        return ch;
                    }
                    tokenBuilder.SetSelectorCombinator(combinator, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
            }
            return ch;
        }

        private char ScanSimpleSelector(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            if (ch == '.' || ch == ':' || ch == '#' || ch == '[')
            {
                if (!tokenBuilder.CanAddSelector())
                {
                    parseCurrent = num;
                    return ch;
                }
                tokenBuilder.BuildUniversalSelector();
            }
            else
            {
                if (!IsNameStartCharacter(ch, charClass, parseCurrent) && ch != '*')
                {
                    return ch;
                }
                if (!tokenBuilder.CanAddSelector())
                {
                    parseCurrent = num;
                    return ch;
                }
                tokenBuilder.StartSelectorName();
                int nameLength;
                if (ch == '*')
                {
                    nameLength = 1;
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    PrepareAndAddRun(CssRunKind.SelectorName, parseCurrent - 1, ref parseCurrent);
                }
                else
                {
                    ch = ScanName(CssRunKind.SelectorName, ch, ref charClass, ref parseCurrent, out nameLength);
                }
                tokenBuilder.EndSelectorName(nameLength);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            ch = ScanSelectorSuffix(ch, ref charClass, ref parseCurrent);
            tokenBuilder.EndSimpleSelector();
            return ch;
        }

        private char ScanSelectorSuffix(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            if (ch == '[')
            {
                tokenBuilder.StartSelectorClass(CssSelectorClassType.Attrib);
                ch = ScanSelectorAttrib(ch, ref charClass, ref parseCurrent);
                tokenBuilder.EndSelectorClass();
                return ch;
            }
            int num = parseEnd;
            char[] array = parseBuffer;
            if (ch != ':')
            {
                if (ch == '.' || ch == '#')
                {
                    bool flag = ch == '.';
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (IsNameCharacter(ch, charClass, parseCurrent) && (!flag || IsNameStartCharacter(ch, charClass, parseCurrent)))
                    {
                        PrepareAndAddRun(flag ? CssRunKind.SelectorClassStart : CssRunKind.SelectorHashStart, parseCurrent - 1, ref parseCurrent);
                        if (parseCurrent == num)
                        {
                            return ch;
                        }
                        tokenBuilder.StartSelectorClass(flag ? CssSelectorClassType.Regular : CssSelectorClassType.Hash);
                        int num2;
                        ch = ScanName(flag ? CssRunKind.SelectorClass : CssRunKind.SelectorHash, ch, ref charClass, ref parseCurrent, out num2);
                        tokenBuilder.EndSelectorClass();
                        if (parseCurrent == num)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        PrepareAndAddInvalidRun(CssRunKind.FunctionStart, ref parseCurrent);
                    }
                }
                return ch;
            }
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            PrepareAndAddRun(CssRunKind.SelectorPseudoStart, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == num)
            {
                return ch;
            }
            tokenBuilder.StartSelectorClass(CssSelectorClassType.Pseudo);
            ch = ScanSelectorPseudo(ch, ref charClass, ref parseCurrent);
            tokenBuilder.EndSelectorClass();
            return ch;
        }

        private char ScanSelectorPseudo(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                tokenBuilder.InvalidateLastValidRun(CssRunKind.SelectorPseudoStart);
                return ch;
            }
            int start = parseCurrent;
            int num2;
            ch = ScanName(CssRunKind.SelectorPseudo, ch, ref charClass, ref parseCurrent, out num2);
            if (parseCurrent == num)
            {
                return ch;
            }
            if (ch == '(')
            {
                if (!IsSafeIdentifier(SafePseudoFunctions, start, parseCurrent))
                {
                    return ch;
                }
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == num)
                {
                    return ch;
                }
                PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (!IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    isInvalid = true;
                    return ch;
                }
                ch = ScanName(CssRunKind.SelectorPseudoArg, ch, ref charClass, ref parseCurrent, out num2);
                if (parseCurrent == num)
                {
                    return ch;
                }
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (ch != ')')
                {
                    isInvalid = true;
                    return ch;
                }
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            return ch;
        }

        private char ScanSelectorAttrib(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            PrepareAndAddRun(CssRunKind.SelectorAttribStart, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == num)
            {
                return ch;
            }
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == num)
            {
                return ch;
            }
            if (!IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                isInvalid = true;
                return ch;
            }
            int num2;
            ch = ScanName(CssRunKind.SelectorAttribName, ch, ref charClass, ref parseCurrent, out num2);
            if (parseCurrent == num)
            {
                return ch;
            }
            ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
            if (parseCurrent == num)
            {
                return ch;
            }
            int num3 = parseCurrent;
            if (ch == '=')
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                PrepareAndAddRun(CssRunKind.SelectorAttribEquals, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            else if ((ch == '~' || ch == '|') && array[parseCurrent + 1] == '=')
            {
                parseCurrent += 2;
                PrepareAndAddRun((ch == '~') ? CssRunKind.SelectorAttribIncludes : CssRunKind.SelectorAttribDashmatch, parseCurrent - 2, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                ch = array[parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            if (num3 < parseCurrent)
            {
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    num3 = parseCurrent;
                    ch = ScanName(CssRunKind.SelectorAttribIdentifier, ch, ref charClass, ref parseCurrent, out num2);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (ch == '"' || ch == '\'')
                {
                    num3 = parseCurrent;
                    ch = ScanString(ch, ref charClass, ref parseCurrent, false);
                    PrepareAndAddRun(CssRunKind.SelectorAttribString, num3, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            if (ch != ']')
            {
                isInvalid = true;
                return ch;
            }
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            PrepareAndAddRun(CssRunKind.SelectorAttribEnd, parseCurrent - 1, ref parseCurrent);
            if (parseCurrent == num)
            {
                return ch;
            }
            return ch;
        }

        private char ScanDeclarations(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            tokenBuilder.StartDeclarations(parseCurrent);
            while (true)
            {
                int num2 = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    break;
                }
                if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    if (!tokenBuilder.CanAddProperty())
                    {
                        goto Block_3;
                    }
                    tokenBuilder.StartPropertyName();
                    int nameLength;
                    ch = ScanName(CssRunKind.PropertyName, ch, ref charClass, ref parseCurrent, out nameLength);
                    tokenBuilder.EndPropertyName(nameLength);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    if (ch != ':')
                    {
                        goto Block_6;
                    }
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    PrepareAndAddRun(CssRunKind.PropertyColon, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    tokenBuilder.StartPropertyValue();
                    ch = ScanPropertyValue(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    tokenBuilder.EndPropertyValue();
                    tokenBuilder.EndProperty();
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                if (ch != ';')
                {
                    goto Block_11;
                }
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                PrepareAndAddRun(CssRunKind.Delimiter, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (num2 >= parseCurrent)
                {
                    return ch;
                }
            }
            return ch;
            Block_3:
            parseCurrent = num;
            return ch;
            Block_6:
            tokenBuilder.MarkPropertyAsDeleted();
            return ch;
            Block_11:
            tokenBuilder.EndDeclarations();
            return ch;
        }

        private char ScanPropertyValue(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            ch = ScanExpr(ch, ref charClass, ref parseCurrent, 0);
            if (parseCurrent == num)
            {
                return ch;
            }
            if (ch == '!')
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == num)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
                PrepareAndAddRun(CssRunKind.ImportantStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                if (parseCurrent == num)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
                if (!IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
                int num2 = parseCurrent;
                int num3;
                ch = ScanName(CssRunKind.Important, ch, ref charClass, ref parseCurrent, out num3);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (!IsNameEqual("important", num2, parseCurrent - num2))
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
            }
            return ch;
        }

        private char ScanExpr(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            int i = parseCurrent;
            ch = ScanTerm(ch, ref charClass, ref parseCurrent, level);
            if (parseCurrent == num)
            {
                return ch;
            }
            while (i < parseCurrent)
            {
                bool flag = false;
                bool flag2 = false;
                i = parseCurrent;
                ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, false);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (i < parseCurrent)
                {
                    flag = true;
                }
                if (ch == '/' || ch == ',')
                {
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    PrepareAndAddRun(CssRunKind.Operator, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    flag2 = true;
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (i == parseCurrent)
                {
                    break;
                }
                i = parseCurrent;
                ch = ScanTerm(ch, ref charClass, ref parseCurrent, level);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (i == parseCurrent)
                {
                    if (flag2)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Operator);
                    }
                    if (flag)
                    {
                        tokenBuilder.InvalidateLastValidRun(CssRunKind.Space);
                        break;
                    }
                    break;
                }
            }
            return ch;
        }

        private char ScanTerm(char ch, ref CharClass charClass, ref int parseCurrent, int level)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            bool flag = false;
            if (ch == '-' || ch == '+')
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                if (parseCurrent == num)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                    return ch;
                }
                PrepareAndAddRun(CssRunKind.UnaryOperator, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                flag = true;
            }
            if (ParseSupport.NumericCharacter(charClass) || ch == '.')
            {
                ch = ScanNumeric(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (ch == '.')
                {
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    PrepareAndAddRun(CssRunKind.Dot, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    int num2 = parseCurrent;
                    ch = ScanNumeric(ch, ref charClass, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                    if (num2 == parseCurrent)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                    }
                }
                if (ch == '%')
                {
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    PrepareAndAddRun(CssRunKind.Percent, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (IsNameStartCharacter(ch, charClass, parseCurrent))
                {
                    int num3;
                    ch = ScanName(CssRunKind.Metrics, ch, ref charClass, ref parseCurrent, out num3);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
            }
            else if (IsNameStartCharacter(ch, charClass, parseCurrent))
            {
                int num2 = parseCurrent;
                int num4;
                ch = ScanName(CssRunKind.TermIdentifier, ch, ref charClass, ref parseCurrent, out num4);
                if (parseCurrent == num)
                {
                    return ch;
                }
                int start = parseCurrent;
                if (ch == '+' && num2 + 1 == parseCurrent && (array[num2] == 'u' || array[num2] == 'U'))
                {
                    ch = ScanUnicodeRange(ch, ref charClass, ref parseCurrent);
                    PrepareAndAddRun(CssRunKind.UnicodeRange, start, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (ch == '(')
                {
                    bool flag2 = false;
                    if (!IsSafeIdentifier(SafeTermFunctions, num2, parseCurrent))
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        if (IsNameEqual("url", num2, parseCurrent - num2))
                        {
                            flag2 = true;
                        }
                    }
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    if (parseCurrent == num)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }
                    PrepareAndAddRun(CssRunKind.FunctionStart, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }
                    ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                    if (parseCurrent == num)
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                        return ch;
                    }
                    if (flag2)
                    {
                        if (ch == '"' || ch == '\'')
                        {
                            num2 = parseCurrent;
                            ch = ScanString(ch, ref charClass, ref parseCurrent, true);
                            PrepareAndAddRun(CssRunKind.String, num2, ref parseCurrent);
                            if (parseCurrent == num)
                            {
                                return ch;
                            }
                        }
                        else
                        {
                            num2 = parseCurrent;
                            ch = ScanUrl(ch, ref charClass, ref parseCurrent);
                            if (parseCurrent == num)
                            {
                                return ch;
                            }
                        }
                        ch = ScanWhitespace(ch, ref charClass, ref parseCurrent, true);
                        if (parseCurrent == num)
                        {
                            return ch;
                        }
                    }
                    else
                    {
                        if (++level > 16)
                        {
                            tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                        ch = ScanExpr(ch, ref charClass, ref parseCurrent, level);
                        if (parseCurrent == num)
                        {
                            tokenBuilder.MarkPropertyAsDeleted();
                            return ch;
                        }
                    }
                    if (ch != ')')
                    {
                        tokenBuilder.MarkPropertyAsDeleted();
                    }
                    ch = array[++parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    PrepareAndAddRun(CssRunKind.FunctionEnd, parseCurrent - 1, ref parseCurrent);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else if (flag)
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (flag)
            {
                tokenBuilder.MarkPropertyAsDeleted();
            }
            else if (ch == '#')
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
                PrepareAndAddRun(CssRunKind.HexColorStart, parseCurrent - 1, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
                if (IsNameCharacter(ch, charClass, parseCurrent))
                {
                    int num5;
                    ch = ScanName(CssRunKind.HexColor, ch, ref charClass, ref parseCurrent, out num5);
                    if (parseCurrent == num)
                    {
                        return ch;
                    }
                }
                else
                {
                    tokenBuilder.MarkPropertyAsDeleted();
                }
            }
            else if (ch == '"' || ch == '\'')
            {
                int num2 = parseCurrent;
                ch = ScanString(ch, ref charClass, ref parseCurrent, true);
                PrepareAndAddRun(CssRunKind.String, num2, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            return ch;
        }

        private char ScanNumeric(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int start = parseCurrent;
            char[] array = parseBuffer;
            while (ParseSupport.NumericCharacter(charClass))
            {
                ch = array[++parseCurrent];
                charClass = ParseSupport.GetCharClass(ch);
            }
            PrepareAndAddRun(CssRunKind.Numeric, start, ref parseCurrent);
            return ch;
        }

        private char ScanString(char ch, ref CharClass charClass, ref int parseCurrent, bool inProperty)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            char c = ch;
            char c2 = '\0';
            char c3 = '\0';
            while (true)
            {
                ch = array[++parseCurrent];
                if (parseCurrent == num)
                {
                    break;
                }
                if (CssToken.AttemptUnescape(array, num, ref ch, ref parseCurrent))
                {
                    if (parseCurrent == num)
                    {
                        goto Block_4;
                    }
                    c2 = '\0';
                    c3 = '\0';
                }
                else
                {
                    if (ch == c || (ch == '\n' && c2 == '\r' && c3 != '\\') || (((ch == '\n' && c2 != '\r') || ch == '\r' || ch == '\f') && c2 != '\\'))
                    {
                        goto IL_B0;
                    }
                    c3 = c2;
                    c2 = ch;
                }
            }
            if (inProperty)
            {
                tokenBuilder.MarkPropertyAsDeleted();
            }
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
            Block_4:
            if (inProperty)
            {
                tokenBuilder.MarkPropertyAsDeleted();
            }
            charClass = ParseSupport.GetCharClass(array[parseCurrent]);
            return array[parseCurrent];
            IL_B0:
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private char ScanName(CssRunKind runKind, char ch, ref CharClass charClass, ref int parseCurrent, out int nameLength)
        {
            nameLength = 0;
            int num;
            while (true)
            {
                num = parseCurrent;
                while (IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                {
                    nameLength++;
                    if (parseCurrent == parseEnd)
                    {
                        break;
                    }
                    ch = parseBuffer[++parseCurrent];
                }
                if (parseCurrent != num)
                {
                    PrepareAndAddRun(runKind, num, ref parseCurrent);
                }
                if (parseCurrent == parseEnd)
                {
                    goto IL_F6;
                }
                num = parseCurrent;
                if (ch != '\\')
                {
                    goto IL_F6;
                }
                if (!CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                {
                    break;
                }
                parseCurrent++;
                if (!IsNameCharacterNoEscape(ch, ParseSupport.GetCharClass(ch)))
                {
                    goto Block_6;
                }
                nameLength++;
                PrepareAndAddLiteralRun(runKind, num, ref parseCurrent, (int)ch);
                if (parseCurrent == parseEnd)
                {
                    goto IL_F6;
                }
                ch = parseBuffer[parseCurrent];
            }
            ch = parseBuffer[++parseCurrent];
            PrepareAndAddInvalidRun(runKind, ref parseCurrent);
            goto IL_F6;
            Block_6:
            if (parseCurrent != num)
            {
                PrepareAndAddLiteralRun(runKind, num, ref parseCurrent, (int)ch);
            }
            nameLength = 0;
            IL_F6:
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private char ScanUrl(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            while (true)
            {
                int num = parseCurrent;
                while (IsUrlCharacter(ch, ParseSupport.GetCharClass(ch), parseCurrent) && parseCurrent != parseEnd)
                {
                    ch = parseBuffer[++parseCurrent];
                }
                if (parseCurrent != num)
                {
                    PrepareAndAddRun(CssRunKind.Url, num, ref parseCurrent);
                }
                if (parseCurrent == parseEnd)
                {
                    goto IL_BA;
                }
                num = parseCurrent;
                if (ch != '\\')
                {
                    goto IL_BA;
                }
                if (!CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
                {
                    break;
                }
                parseCurrent++;
                PrepareAndAddLiteralRun(CssRunKind.Url, num, ref parseCurrent, (int)ch);
                if (parseCurrent == parseEnd)
                {
                    goto IL_BA;
                }
                ch = parseBuffer[parseCurrent];
            }
            ch = parseBuffer[++parseCurrent];
            PrepareAndAddInvalidRun(CssRunKind.Url, ref parseCurrent);
            IL_BA:
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private char ScanUnicodeRange(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            int num = parseCurrent + 1;
            int i = num;
            bool flag = true;
            char c;
            while (i < num + 6)
            {
                c = array[i];
                if ('?' == c)
                {
                    flag = false;
                    for (i++; i < num + 6; i++)
                    {
                        if ('?' != array[i])
                        {
                            break;
                        }
                    }
                    break;
                }
                if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(c)))
                {
                    if (i == num)
                    {
                        return ch;
                    }
                    break;
                }
                else
                {
                    i++;
                }
            }
            c = array[i];
            if ('-' == c && flag)
            {
                i++;
                num = i;
                while (i < num + 6)
                {
                    c = array[i];
                    if (!ParseSupport.HexCharacter(ParseSupport.GetCharClass(c)))
                    {
                        if (i == num)
                        {
                            return ch;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            c = array[i];
            charClass = ParseSupport.GetCharClass(c);
            parseCurrent = i;
            return c;
        }

        private char ScanWhitespace(char ch, ref CharClass charClass, ref int parseCurrent, bool ignorable)
        {
            char[] array = parseBuffer;
            int num = parseEnd;
            IL_7F:
            while (ParseSupport.WhitespaceCharacter(charClass) || ch == '/')
            {
                if (ch != '/')
                {
                    int start = parseCurrent;
                    while (++parseCurrent != num)
                    {
                        ch = array[parseCurrent];
                        charClass = ParseSupport.GetCharClass(ch);
                        if (!ParseSupport.WhitespaceCharacter(charClass))
                        {
                            if (tokenBuilder.IsStarted)
                            {
                                PrepareAndAddRun(ignorable ? CssRunKind.Invalid : CssRunKind.Space, start, ref parseCurrent);
                                goto IL_7F;
                            }
                            goto IL_7F;
                        }
                    }
                    return ch;
                }
                if (parseCurrent >= num || array[parseCurrent + 1] != '*')
                {
                    break;
                }
                ch = ScanComment(ch, ref charClass, ref parseCurrent);
                if (parseCurrent == num)
                {
                    return ch;
                }
            }
            return ch;
        }

        private char ScanComment(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            char[] array = parseBuffer;
            int num = parseEnd;
            int start = parseCurrent;
            ch = array[++parseCurrent];
            while (++parseCurrent != num)
            {
                if (array[parseCurrent] == '*' && parseCurrent + 1 != num && array[parseCurrent + 1] == '/')
                {
                    parseCurrent++;
                    if (++parseCurrent == num)
                    {
                        return ch;
                    }
                    if (tokenBuilder.IsStarted)
                    {
                        PrepareAndAddRun(CssRunKind.Invalid, start, ref parseCurrent);
                    }
                    ch = array[parseCurrent];
                    charClass = ParseSupport.GetCharClass(ch);
                    return ch;
                }
            }
            return ch;
        }

        private void PrepareAndAddRun(CssRunKind runKind, int start, ref int parseCurrent)
        {
            if (!tokenBuilder.PrepareAndAddRun(runKind, start, parseCurrent))
            {
                parseCurrent = parseEnd;
            }
        }

        private void PrepareAndAddInvalidRun(CssRunKind runKind, ref int parseCurrent)
        {
            if (!tokenBuilder.PrepareAndAddInvalidRun(runKind, parseCurrent))
            {
                parseCurrent = parseEnd;
            }
        }

        private void PrepareAndAddLiteralRun(CssRunKind runKind, int start, ref int parseCurrent, int value)
        {
            if (!tokenBuilder.PrepareAndAddLiteralRun(runKind, start, parseCurrent, value))
            {
                parseCurrent = parseEnd;
            }
        }

        private char SkipToNextRule(char ch, ref CharClass charClass, ref int parseCurrent)
        {
            int num = parseEnd;
            char[] array = parseBuffer;
            while (true)
            {
                if (ch == '"' || ch == '\'')
                {
                    ch = ScanString(ch, ref charClass, ref parseCurrent, false);
                    if (parseCurrent == num)
                    {
                        break;
                    }
                }
                else
                {
                    if (ch == '{')
                    {
                        ruleDepth++;
                    }
                    else if (ch == '}')
                    {
                        if (ruleDepth > 0)
                        {
                            ruleDepth--;
                        }
                        if (ruleDepth == 0)
                        {
                            goto Block_6;
                        }
                    }
                    else if (ch == ';' && ruleDepth == 0)
                    {
                        goto Block_8;
                    }
                    if (++parseCurrent == num)
                    {
                        return ch;
                    }
                    ch = array[parseCurrent];
                }
            }
            return ch;
            Block_6:
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
            Block_8:
            ch = array[++parseCurrent];
            charClass = ParseSupport.GetCharClass(ch);
            return ch;
        }

        private bool IsSafeIdentifier(string[] table, int start, int end)
        {
            int length = end - start;
            for (int i = 0; i < table.Length; i++)
            {
                if (IsNameEqual(table[i], start, length))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsNameEqual(string name, int start, int length)
        {
            return name.Equals(new string(parseBuffer, start, length), StringComparison.OrdinalIgnoreCase);
        }

        private bool IsNameCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            return IsNameStartCharacter(ch, charClass, parseCurrent) || ParseSupport.NumericCharacter(charClass) || ch == '-';
        }

        private bool IsNameStartCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            if (IsNameStartCharacterNoEscape(ch, charClass))
            {
                return true;
            }
            if (CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent))
            {
                charClass = ParseSupport.GetCharClass(ch);
                return IsNameStartCharacterNoEscape(ch, charClass);
            }
            return false;
        }

        private static bool IsNameCharacterNoEscape(char ch, CharClass charClass)
        {
            return IsNameStartCharacterNoEscape(ch, charClass) || ParseSupport.NumericCharacter(charClass) || ch == '-';
        }

        private static bool IsNameStartCharacterNoEscape(char ch, CharClass charClass)
        {
            return ParseSupport.AlphaCharacter(charClass) || ch == '_' || ch > '\u007f';
        }

        private bool IsUrlCharacter(char ch, CharClass charClass, int parseCurrent)
        {
            return IsUrlCharacterNoEscape(ch, charClass) || IsEscape(ch, parseCurrent);
        }

        private static bool IsUrlCharacterNoEscape(char ch, CharClass charClass)
        {
            return (ch >= '*' && ch != '\u007f') || (ch >= '#' && ch <= '&') || ch == '!';
        }

        private bool IsEscape(char ch, int parseCurrent)
        {
            return CssToken.AttemptUnescape(parseBuffer, parseEnd, ref ch, ref parseCurrent);
        }
    }
}
