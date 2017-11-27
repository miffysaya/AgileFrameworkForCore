using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal class CssTokenBuilder : TokenBuilder
    {
        protected const byte BuildStateEndedCss = 6;

        protected const byte BuildStatePropertyListStarted = 20;

        protected const byte BuildStateBeforeSelector = 23;

        protected const byte BuildStateSelectorName = 24;

        protected const byte BuildStateEndSelectorName = 25;

        protected const byte BuildStateSelectorClass = 26;

        protected const byte BuildStateEndSelectorClass = 27;

        protected const byte BuildStateBeforeProperty = 43;

        protected const byte BuildStatePropertyName = 44;

        protected const byte BuildStateEndPropertyName = 45;

        protected const byte BuildStatePropertyValue = 46;

        protected const byte BuildStateEndPropertyValue = 47;

        protected CssToken cssToken;

        protected int maxProperties;

        protected int maxSelectors;

        public new CssToken Token
        {
            get
            {
                return cssToken;
            }
        }

        public bool Incomplete
        {
            get
            {
                return state >= 10 && state != 10;
            }
        }

        public CssTokenBuilder(char[] buffer, int maxProperties, int maxSelectors, int maxRuns, bool testBoundaryConditions) : base(new CssToken(), buffer, maxRuns, testBoundaryConditions)
        {
            cssToken = (CssToken)base.Token;
            int num = 16;
            int num2 = 16;
            if (!testBoundaryConditions)
            {
                this.maxProperties = maxProperties;
                this.maxSelectors = maxSelectors;
            }
            else
            {
                num = 1;
                num2 = 1;
                this.maxProperties = 5;
                this.maxSelectors = 5;
            }
            cssToken.propertyList = new CssToken.PropertyEntry[num];
            cssToken.selectorList = new CssToken.SelectorEntry[num2];
        }

        public override void Reset()
        {
            if (state >= 6)
            {
                cssToken.Reset();
            }
            base.Reset();
        }

        public void StartRuleSet(int baseOffset, CssTokenId id)
        {
            state = 23;
            cssToken.tokenId = (TokenId)id;
            cssToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        public void EndRuleSet()
        {
            if (state >= 43)
            {
                EndDeclarations();
            }
            tokenValid = true;
            state = 6;
            token.wholePosition.Rewind(token.whole);
        }

        public void BuildUniversalSelector()
        {
            StartSelectorName();
            EndSelectorName(0);
        }

        public bool CanAddSelector()
        {
            return cssToken.selectorTail - cssToken.selectorHead < maxSelectors;
        }

        public void StartSelectorName()
        {
            if (cssToken.selectorTail == cssToken.selectorList.Length)
            {
                int num;
                if (maxSelectors / 2 > cssToken.selectorList.Length)
                {
                    num = cssToken.selectorList.Length * 2;
                }
                else
                {
                    num = maxSelectors;
                }
                CssToken.SelectorEntry[] array = new CssToken.SelectorEntry[num];
                Array.Copy(cssToken.selectorList, 0, array, 0, cssToken.selectorTail);
                cssToken.selectorList = array;
            }
            cssToken.selectorList[cssToken.selectorTail].nameId = HtmlNameIndex.Unknown;
            cssToken.selectorList[cssToken.selectorTail].name.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.selectorList[cssToken.selectorTail].className.Reset();
            state = 24;
        }

        public void EndSelectorName(int nameLength)
        {
            cssToken.selectorList[cssToken.selectorTail].name.tail = cssToken.whole.tail;
            cssToken.selectorList[cssToken.selectorTail].nameId = LookupTagName(nameLength, cssToken.selectorList[cssToken.selectorTail].name);
            state = 25;
        }

        public void StartSelectorClass(CssSelectorClassType classType)
        {
            cssToken.selectorList[cssToken.selectorTail].className.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.selectorList[cssToken.selectorTail].classType = classType;
            state = 26;
        }

        public void EndSelectorClass()
        {
            cssToken.selectorList[cssToken.selectorTail].className.tail = cssToken.whole.tail;
            state = 27;
        }

        public void SetSelectorCombinator(CssSelectorCombinator combinator, bool previous)
        {
            int num = cssToken.selectorTail;
            if (previous)
            {
                num--;
            }
            cssToken.selectorList[num].combinator = combinator;
        }

        public void EndSimpleSelector()
        {
            cssToken.selectorTail++;
        }

        public void StartDeclarations(int baseOffset)
        {
            state = 43;
            if (cssToken.tokenId == TokenId.None)
            {
                cssToken.tokenId = (TokenId)5;
            }
            cssToken.partMajor = CssToken.PropertyListPartMajor.Begin;
            cssToken.partMinor = CssToken.PropertyListPartMinor.Empty;
            cssToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        public bool CanAddProperty()
        {
            return cssToken.propertyTail - cssToken.propertyHead < maxProperties;
        }

        public void StartPropertyName()
        {
            if (cssToken.propertyTail == cssToken.propertyList.Length)
            {
                int num;
                if (maxProperties / 2 > cssToken.propertyList.Length)
                {
                    num = cssToken.propertyList.Length * 2;
                }
                else
                {
                    num = maxProperties;
                }
                CssToken.PropertyEntry[] array = new CssToken.PropertyEntry[num];
                Array.Copy(cssToken.propertyList, 0, array, 0, cssToken.propertyTail);
                cssToken.propertyList = array;
            }
            if (cssToken.partMinor == CssToken.PropertyListPartMinor.Empty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.BeginProperty;
            }
            cssToken.propertyList[cssToken.propertyTail].nameId = CssNameIndex.Unknown;
            cssToken.propertyList[cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.Begin;
            cssToken.propertyList[cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.BeginName;
            cssToken.propertyList[cssToken.propertyTail].quoteChar = 0;
            cssToken.propertyList[cssToken.propertyTail].name.Initialize(cssToken.whole.tail, tailOffset);
            cssToken.propertyList[cssToken.propertyTail].value.Reset();
            state = 44;
        }

        public void EndPropertyName(int nameLength)
        {
            cssToken.propertyList[cssToken.propertyTail].name.tail = cssToken.whole.tail;
            CssToken.PropertyEntry[] expr_50_cp_0 = cssToken.propertyList;
            int expr_50_cp_1 = cssToken.propertyTail;
            expr_50_cp_0[expr_50_cp_1].partMinor = (expr_50_cp_0[expr_50_cp_1].partMinor | CssToken.PropertyPartMinor.EndName);
            if (cssToken.propertyList[cssToken.propertyTail].IsPropertyBegin)
            {
                cssToken.propertyList[cssToken.propertyTail].nameId = LookupName(nameLength, cssToken.propertyList[cssToken.propertyTail].name);
            }
            state = 45;
        }

        public void StartPropertyValue()
        {
            cssToken.propertyList[cssToken.propertyTail].value.Initialize(cssToken.whole.tail, tailOffset);
            CssToken.PropertyEntry[] expr_56_cp_0 = cssToken.propertyList;
            int expr_56_cp_1 = cssToken.propertyTail;
            expr_56_cp_0[expr_56_cp_1].partMinor = (expr_56_cp_0[expr_56_cp_1].partMinor | CssToken.PropertyPartMinor.BeginValue);
            state = 46;
        }

        public void EndPropertyValue()
        {
            cssToken.propertyList[cssToken.propertyTail].value.tail = cssToken.whole.tail;
            CssToken.PropertyEntry[] expr_50_cp_0 = cssToken.propertyList;
            int expr_50_cp_1 = cssToken.propertyTail;
            expr_50_cp_0[expr_50_cp_1].partMinor = (expr_50_cp_0[expr_50_cp_1].partMinor | CssToken.PropertyPartMinor.EndValue);
            state = 47;
        }

        public void EndProperty()
        {
            CssToken.PropertyEntry[] expr_1B_cp_0 = cssToken.propertyList;
            int expr_1B_cp_1 = cssToken.propertyTail;
            expr_1B_cp_0[expr_1B_cp_1].partMajor = (expr_1B_cp_0[expr_1B_cp_1].partMajor | CssToken.PropertyPartMajor.End);
            cssToken.propertyTail++;
            if (cssToken.propertyTail < cssToken.propertyList.Length)
            {
                cssToken.propertyList[cssToken.propertyTail].partMajor = CssToken.PropertyPartMajor.None;
                cssToken.propertyList[cssToken.propertyTail].partMinor = CssToken.PropertyPartMinor.Empty;
            }
            if (cssToken.partMinor == CssToken.PropertyListPartMinor.BeginProperty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.Properties;
            }
            else if (cssToken.partMinor == CssToken.PropertyListPartMinor.ContinueProperty)
            {
                cssToken.partMinor = CssToken.PropertyListPartMinor.EndProperty;
            }
            else
            {
                CssToken expr_DD = cssToken;
                expr_DD.partMinor |= CssToken.PropertyListPartMinor.Properties;
            }
            state = 43;
        }

        public void EndDeclarations()
        {
            if (state != 20)
            {
                if (state == 44)
                {
                    cssToken.propertyList[cssToken.propertyTail].name.tail = cssToken.whole.tail;
                }
                else if (state == 46)
                {
                    cssToken.propertyList[cssToken.propertyTail].value.tail = cssToken.whole.tail;
                }
            }
            if (state == 44)
            {
                EndPropertyName(0);
            }
            else if (state == 46)
            {
                EndPropertyValue();
            }
            if (state == 45 || state == 47)
            {
                EndProperty();
            }
            state = 43;
            CssToken expr_D8 = cssToken;
            expr_D8.partMajor |= CssToken.PropertyListPartMajor.End;
            tokenValid = true;
        }

        public bool PrepareAndAddRun(CssRunKind cssRunKind, int start, int end)
        {
            if (end != start)
            {
                if (!base.PrepareToAddMoreRuns(1))
                {
                    return false;
                }
                base.AddRun((cssRunKind == CssRunKind.Invalid) ? RunType.Invalid : ((RunType)2147483648u), (cssRunKind == CssRunKind.Space) ? RunTextType.Space : RunTextType.NonSpace, (uint)cssRunKind, start, end, 0);
            }
            return true;
        }

        public bool PrepareAndAddInvalidRun(CssRunKind cssRunKind, int end)
        {
            if (!base.PrepareToAddMoreRuns(1))
            {
                return false;
            }
            base.AddInvalidRun(end, (uint)cssRunKind);
            return true;
        }

        public bool PrepareAndAddLiteralRun(CssRunKind cssRunKind, int start, int end, int value)
        {
            if (end != start)
            {
                if (!base.PrepareToAddMoreRuns(1))
                {
                    return false;
                }
                base.AddRun((RunType)3221225472u, RunTextType.NonSpace, (uint)cssRunKind, start, end, value);
            }
            return true;
        }

        public void InvalidateLastValidRun(CssRunKind kind)
        {
            int num = token.whole.tail;
            Token.RunEntry runEntry;
            while (true)
            {
                num--;
                runEntry = token.runList[num];
                if (runEntry.Type != RunType.Invalid)
                {
                    break;
                }
                if (num <= 0)
                {
                    return;
                }
            }
            if (kind == (CssRunKind)runEntry.Kind)
            {
                token.runList[num].Initialize(RunType.Invalid, runEntry.TextType, runEntry.Kind, runEntry.Length, runEntry.Value);
                return;
            }
        }

        public void MarkPropertyAsDeleted()
        {
            cssToken.propertyList[cssToken.propertyTail].IsPropertyDeleted = true;
        }

        public CssNameIndex LookupName(int nameLength, Token.Fragment fragment)
        {
            if (nameLength > 26)
            {
                return CssNameIndex.Unknown;
            }
            short num = (short)((ulong)(token.CalculateHashLowerCase(fragment) ^ 2) % 329uL);
            int num2 = (int)CssData.nameHashTable[(int)num];
            if (num2 > 0)
            {
                while (true)
                {
                    string name = CssData.names[num2].name;
                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(token.buffer[fragment.headOffset]) && (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1)))
                            {
                                break;
                            }
                        }
                        else if (token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            goto Block_6;
                        }
                    }
                    num2++;
                    if (CssData.names[num2].hash != num)
                    {
                        return CssNameIndex.Unknown;
                    }
                }
                return (CssNameIndex)num2;
                Block_6:
                return (CssNameIndex)num2;
            }
            return CssNameIndex.Unknown;
        }

        public HtmlNameIndex LookupTagName(int nameLength, Token.Fragment fragment)
        {
            if (nameLength > 14)
            {
                return HtmlNameIndex.Unknown;
            }
            short num = (short)((ulong)(token.CalculateHashLowerCase(fragment) ^ 221) % 601uL);
            int num2 = (int)HtmlNameData.nameHashTable[(int)num];
            if (num2 > 0)
            {
                while (true)
                {
                    string name = HtmlNameData.names[num2].name;
                    if (name.Length == nameLength)
                    {
                        if (fragment.tail == fragment.head + 1)
                        {
                            if (name[0] == ParseSupport.ToLowerCase(token.buffer[fragment.headOffset]) && (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(fragment.headOffset + 1, name, 1)))
                            {
                                break;
                            }
                        }
                        else if (token.CaseInsensitiveCompareEqual(ref fragment, name))
                        {
                            goto Block_6;
                        }
                    }
                    num2++;
                    if (HtmlNameData.names[num2].hash != num)
                    {
                        return HtmlNameIndex.Unknown;
                    }
                }
                return (HtmlNameIndex)num2;
                Block_6:
                return (HtmlNameIndex)num2;
            }
            return HtmlNameIndex.Unknown;
        }
    }
}
