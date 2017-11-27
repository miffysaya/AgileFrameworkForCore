using AgileFramework.Security.Application.TextConverters.HTML;
using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlTokenBuilder : TokenBuilder
    {
        protected const byte BuildStateEndedHtml = 6;

        protected const byte BuildStateTagStarted = 20;

        protected const byte BuildStateTagText = 21;

        protected const byte BuildStateTagName = 22;

        protected const byte BuildStateTagBeforeAttr = 23;

        protected const byte BuildStateTagAttrName = 24;

        protected const byte BuildStateTagEndAttrName = 25;

        protected const byte BuildStateTagAttrValue = 26;

        protected const byte BuildStateTagEndAttrValue = 27;

        protected HtmlToken htmlToken;

        protected int maxAttrs;

        protected int numCarryOverRuns;

        protected int carryOverRunsHeadOffset;

        protected int carryOverRunsLength;

        public new HtmlToken Token
        {
            get
            {
                return htmlToken;
            }
        }

        public bool IncompleteTag
        {
            get
            {
                return state >= 10 && state != 10;
            }
        }

        public HtmlTokenBuilder(char[] buffer, int maxRuns, int maxAttrs, bool testBoundaryConditions) : base(new HtmlToken(), buffer, maxRuns, testBoundaryConditions)
        {
            htmlToken = (HtmlToken)base.Token;
            int num = 8;
            if (maxAttrs != 0)
            {
                if (!testBoundaryConditions)
                {
                    this.maxAttrs = maxAttrs;
                }
                else
                {
                    num = 1;
                    this.maxAttrs = 5;
                }
                htmlToken.attributeList = new HtmlToken.AttributeEntry[num];
            }
            htmlToken.nameIndex = HtmlNameIndex._NOTANAME;
        }

        public override void Reset()
        {
            if (state >= 6)
            {
                htmlToken.Reset();
                numCarryOverRuns = 0;
            }
            base.Reset();
        }

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId);
        }

        public HtmlTokenId MakeEmptyToken(HtmlTokenId tokenId, int argument)
        {
            return (HtmlTokenId)base.MakeEmptyToken((TokenId)tokenId, argument);
        }

        public void StartTag(HtmlNameIndex nameIndex, int baseOffset)
        {
            state = 20;
            htmlToken.tokenId = (TokenId)4;
            htmlToken.partMajor = HtmlToken.TagPartMajor.Begin;
            htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
            htmlToken.nameIndex = nameIndex;
            htmlToken.tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;
            htmlToken.whole.headOffset = baseOffset;
            tailOffset = baseOffset;
        }

        public void AbortConditional(bool comment)
        {
            htmlToken.nameIndex = (comment ? HtmlNameIndex._COMMENT : HtmlNameIndex._BANG);
        }

        public void SetEndTag()
        {
            HtmlToken expr_06 = htmlToken;
            expr_06.flags |= HtmlToken.TagFlags.EndTag;
        }

        public void SetEmptyScope()
        {
            HtmlToken expr_06 = htmlToken;
            expr_06.flags |= HtmlToken.TagFlags.EmptyScope;
        }

        public void StartTagText()
        {
            state = 21;
            htmlToken.unstructured.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.unstructuredPosition.Rewind(htmlToken.unstructured);
        }

        public void EndTagText()
        {
            if (htmlToken.unstructured.head == htmlToken.whole.tail)
            {
                AddNullRun(HtmlRunKind.TagText);
            }
            state = 20;
        }

        public void StartTagName()
        {
            state = 22;
            HtmlToken expr_0E = htmlToken;
            expr_0E.partMinor |= HtmlToken.TagPartMinor.BeginName;
            htmlToken.name.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.localName.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.namePosition.Rewind(htmlToken.name);
        }

        public void EndTagNamePrefix()
        {
            htmlToken.localName.Initialize(htmlToken.whole.tail, tailOffset);
        }

        public void EndTagName(int nameLength)
        {
            if (htmlToken.localName.head == htmlToken.whole.tail)
            {
                AddNullRun(HtmlRunKind.Name);
                if (htmlToken.localName.head == htmlToken.name.head)
                {
                    HtmlToken expr_55 = htmlToken;
                    expr_55.flags |= HtmlToken.TagFlags.EmptyTagName;
                }
            }
            HtmlToken expr_69 = htmlToken;
            expr_69.partMinor |= HtmlToken.TagPartMinor.EndName;
            if (htmlToken.IsTagBegin)
            {
                AddSentinelRun();
                htmlToken.nameIndex = LookupName(nameLength, htmlToken.name);
                htmlToken.tagIndex = (htmlToken.originalTagIndex = HtmlNameData.names[(int)htmlToken.nameIndex].tagIndex);
            }
            state = 23;
        }

        public bool CanAddAttribute()
        {
            return htmlToken.attributeTail < maxAttrs;
        }

        public void StartAttribute()
        {
            if (htmlToken.attributeTail == htmlToken.attributeList.Length)
            {
                int num;
                if (maxAttrs / 2 > htmlToken.attributeList.Length)
                {
                    num = htmlToken.attributeList.Length * 2;
                }
                else
                {
                    num = maxAttrs;
                }
                HtmlToken.AttributeEntry[] array = new HtmlToken.AttributeEntry[num];
                Array.Copy(htmlToken.attributeList, 0, array, 0, htmlToken.attributeTail);
                htmlToken.attributeList = array;
            }
            if (htmlToken.partMinor == HtmlToken.TagPartMinor.Empty)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.BeginAttribute;
            }
            htmlToken.attributeList[htmlToken.attributeTail].nameIndex = HtmlNameIndex.Unknown;
            htmlToken.attributeList[htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.Begin;
            htmlToken.attributeList[htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.BeginName;
            htmlToken.attributeList[htmlToken.attributeTail].quoteChar = 0;
            htmlToken.attributeList[htmlToken.attributeTail].name.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.attributeList[htmlToken.attributeTail].localName.Initialize(htmlToken.whole.tail, tailOffset);
            htmlToken.attributeList[htmlToken.attributeTail].value.Reset();
            state = 24;
        }

        public void EndAttributeNamePrefix()
        {
            htmlToken.attributeList[htmlToken.attributeTail].localName.Initialize(htmlToken.whole.tail, tailOffset);
        }

        public void EndAttributeName(int nameLength)
        {
            HtmlToken.AttributeEntry[] expr_1B_cp_0 = htmlToken.attributeList;
            int expr_1B_cp_1 = htmlToken.attributeTail;
            expr_1B_cp_0[expr_1B_cp_1].partMinor = (expr_1B_cp_0[expr_1B_cp_1].partMinor | HtmlToken.AttrPartMinor.EndName);
            if (htmlToken.attributeList[htmlToken.attributeTail].localName.head == htmlToken.whole.tail)
            {
                AddNullRun(HtmlRunKind.Name);
                if (htmlToken.attributeList[htmlToken.attributeTail].localName.head == htmlToken.attributeList[htmlToken.attributeTail].name.head)
                {
                    HtmlToken.AttributeEntry[] expr_D5_cp_0 = htmlToken.attributeList;
                    int expr_D5_cp_1 = htmlToken.attributeTail;
                    expr_D5_cp_0[expr_D5_cp_1].partMajor = (expr_D5_cp_0[expr_D5_cp_1].partMajor | HtmlToken.AttrPartMajor.EmptyName);
                }
            }
            if (htmlToken.attributeList[htmlToken.attributeTail].IsAttrBegin)
            {
                AddSentinelRun();
                htmlToken.attributeList[htmlToken.attributeTail].nameIndex = LookupName(nameLength, htmlToken.attributeList[htmlToken.attributeTail].name);
            }
            state = 25;
        }

        public void StartValue()
        {
            htmlToken.attributeList[htmlToken.attributeTail].value.Initialize(htmlToken.whole.tail, tailOffset);
            HtmlToken.AttributeEntry[] expr_56_cp_0 = htmlToken.attributeList;
            int expr_56_cp_1 = htmlToken.attributeTail;
            expr_56_cp_0[expr_56_cp_1].partMinor = (expr_56_cp_0[expr_56_cp_1].partMinor | HtmlToken.AttrPartMinor.BeginValue);
            state = 26;
        }

        public void SetValueQuote(char ch)
        {
            htmlToken.attributeList[htmlToken.attributeTail].IsAttrValueQuoted = true;
            htmlToken.attributeList[htmlToken.attributeTail].quoteChar = (byte)ch;
        }

        public void EndValue()
        {
            if (htmlToken.attributeList[htmlToken.attributeTail].value.head == htmlToken.whole.tail)
            {
                AddNullRun(HtmlRunKind.AttrValue);
            }
            HtmlToken.AttributeEntry[] expr_5D_cp_0 = htmlToken.attributeList;
            int expr_5D_cp_1 = htmlToken.attributeTail;
            expr_5D_cp_0[expr_5D_cp_1].partMinor = (expr_5D_cp_0[expr_5D_cp_1].partMinor | HtmlToken.AttrPartMinor.EndValue);
            state = 27;
        }

        public void EndAttribute()
        {
            HtmlToken.AttributeEntry[] expr_1B_cp_0 = htmlToken.attributeList;
            int expr_1B_cp_1 = htmlToken.attributeTail;
            expr_1B_cp_0[expr_1B_cp_1].partMajor = (expr_1B_cp_0[expr_1B_cp_1].partMajor | HtmlToken.AttrPartMajor.End);
            htmlToken.attributeTail++;
            if (htmlToken.attributeTail < htmlToken.attributeList.Length)
            {
                htmlToken.attributeList[htmlToken.attributeTail].partMajor = HtmlToken.AttrPartMajor.None;
                htmlToken.attributeList[htmlToken.attributeTail].partMinor = HtmlToken.AttrPartMinor.Empty;
            }
            if (htmlToken.partMinor == HtmlToken.TagPartMinor.BeginAttribute)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.Attributes;
            }
            else if (htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueAttribute)
            {
                htmlToken.partMinor = HtmlToken.TagPartMinor.EndAttribute;
            }
            else
            {
                HtmlToken expr_DE = htmlToken;
                expr_DE.partMinor |= HtmlToken.TagPartMinor.Attributes;
            }
            state = 23;
        }

        public void EndTag(bool complete)
        {
            if (complete)
            {
                if (state != 23)
                {
                    if (state == 21)
                    {
                        EndTagText();
                    }
                    else if (state == 22)
                    {
                        EndTagName(0);
                    }
                    else
                    {
                        if (state == 24)
                        {
                            EndAttributeName(0);
                        }
                        else if (state == 26)
                        {
                            EndValue();
                        }
                        if (state == 25 || state == 27)
                        {
                            EndAttribute();
                        }
                    }
                }
                AddSentinelRun();
                state = 6;
                HtmlToken expr_85 = htmlToken;
                expr_85.partMajor |= HtmlToken.TagPartMajor.End;
            }
            else if (state >= 24)
            {
                if (htmlToken.attributeTail != 0 || htmlToken.name.head != -1 || htmlToken.attributeList[htmlToken.attributeTail].name.head > 0)
                {
                    AddSentinelRun();
                    numCarryOverRuns = htmlToken.whole.tail - htmlToken.attributeList[htmlToken.attributeTail].name.head;
                    carryOverRunsHeadOffset = htmlToken.attributeList[htmlToken.attributeTail].name.headOffset;
                    carryOverRunsLength = tailOffset - carryOverRunsHeadOffset;
                    HtmlToken expr_17B_cp_0 = htmlToken;
                    expr_17B_cp_0.whole.tail = expr_17B_cp_0.whole.tail - numCarryOverRuns;
                }
                else
                {
                    if (state == 24)
                    {
                        if (htmlToken.attributeList[htmlToken.attributeTail].name.head == htmlToken.whole.tail)
                        {
                            AddNullRun(HtmlRunKind.Name);
                        }
                    }
                    else if (state == 26 && htmlToken.attributeList[htmlToken.attributeTail].value.head == htmlToken.whole.tail)
                    {
                        AddNullRun(HtmlRunKind.AttrValue);
                    }
                    AddSentinelRun();
                    htmlToken.attributeTail++;
                }
            }
            else
            {
                if (state == 22)
                {
                    if (htmlToken.name.head == htmlToken.whole.tail)
                    {
                        AddNullRun(HtmlRunKind.Name);
                    }
                }
                else if (state == 21 && htmlToken.unstructured.head == htmlToken.whole.tail)
                {
                    AddNullRun(HtmlRunKind.TagText);
                }
                AddSentinelRun();
            }
            tokenValid = true;
        }

        public int RewindTag()
        {
            if (state >= 24)
            {
                if (htmlToken.attributeTail == 0 || htmlToken.attributeList[htmlToken.attributeTail - 1].IsAttrEnd)
                {
                    int tail = htmlToken.whole.tail;
                    Array.Copy(htmlToken.runList, tail, htmlToken.runList, 0, numCarryOverRuns);
                    htmlToken.whole.head = 0;
                    htmlToken.whole.headOffset = carryOverRunsHeadOffset;
                    htmlToken.whole.tail = numCarryOverRuns;
                    numCarryOverRuns = 0;
                    htmlToken.attributeList[0] = htmlToken.attributeList[htmlToken.attributeTail];
                    htmlToken.partMinor = (HtmlToken.TagPartMinor)htmlToken.attributeList[0].MajorPart;
                    if (htmlToken.attributeList[0].name.head != -1)
                    {
                        HtmlToken.AttributeEntry[] expr_144_cp_0_cp_0 = htmlToken.attributeList;
                        int expr_144_cp_0_cp_1 = 0;
                        expr_144_cp_0_cp_0[expr_144_cp_0_cp_1].name.head = expr_144_cp_0_cp_0[expr_144_cp_0_cp_1].name.head - tail;
                    }
                    if (htmlToken.attributeList[0].localName.head != -1)
                    {
                        HtmlToken.AttributeEntry[] expr_185_cp_0_cp_0 = htmlToken.attributeList;
                        int expr_185_cp_0_cp_1 = 0;
                        expr_185_cp_0_cp_0[expr_185_cp_0_cp_1].localName.head = expr_185_cp_0_cp_0[expr_185_cp_0_cp_1].localName.head - tail;
                    }
                    if (htmlToken.attributeList[0].value.head != -1)
                    {
                        HtmlToken.AttributeEntry[] expr_1C9_cp_0_cp_0 = htmlToken.attributeList;
                        int expr_1C9_cp_0_cp_1 = 0;
                        expr_1C9_cp_0_cp_0[expr_1C9_cp_0_cp_1].value.head = expr_1C9_cp_0_cp_0[expr_1C9_cp_0_cp_1].value.head - tail;
                    }
                }
                else
                {
                    htmlToken.whole.Initialize(0, tailOffset);
                    htmlToken.attributeList[0].nameIndex = htmlToken.attributeList[htmlToken.attributeTail - 1].nameIndex;
                    htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.Continue;
                    HtmlToken.AttrPartMinor partMinor = htmlToken.attributeList[htmlToken.attributeTail - 1].partMinor;
                    if (partMinor == HtmlToken.AttrPartMinor.BeginName || partMinor == HtmlToken.AttrPartMinor.ContinueName)
                    {
                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueName;
                    }
                    else if (partMinor == HtmlToken.AttrPartMinor.EndNameWithBeginValue || partMinor == HtmlToken.AttrPartMinor.CompleteNameWithBeginValue || partMinor == HtmlToken.AttrPartMinor.BeginValue || partMinor == HtmlToken.AttrPartMinor.ContinueValue)
                    {
                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.ContinueValue;
                    }
                    else
                    {
                        htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                    }
                    htmlToken.attributeList[0].IsAttrDeleted = false;
                    htmlToken.attributeList[0].IsAttrValueQuoted = htmlToken.attributeList[htmlToken.attributeTail - 1].IsAttrValueQuoted;
                    htmlToken.attributeList[0].quoteChar = htmlToken.attributeList[htmlToken.attributeTail - 1].quoteChar;
                    if (state == 24)
                    {
                        htmlToken.attributeList[0].name.Initialize(0, tailOffset);
                        htmlToken.attributeList[0].localName.Initialize(0, tailOffset);
                    }
                    else
                    {
                        htmlToken.attributeList[0].name.Reset();
                        htmlToken.attributeList[0].localName.Reset();
                    }
                    if (state == 26)
                    {
                        htmlToken.attributeList[0].value.Initialize(0, tailOffset);
                    }
                    else
                    {
                        htmlToken.attributeList[0].value.Reset();
                    }
                    htmlToken.partMinor = (HtmlToken.TagPartMinor)htmlToken.attributeList[0].MajorPart;
                }
            }
            else
            {
                htmlToken.whole.Initialize(0, tailOffset);
                if (htmlToken.partMinor == HtmlToken.TagPartMinor.BeginName || htmlToken.partMinor == HtmlToken.TagPartMinor.ContinueName)
                {
                    htmlToken.partMinor = HtmlToken.TagPartMinor.ContinueName;
                }
                else
                {
                    htmlToken.partMinor = HtmlToken.TagPartMinor.Empty;
                }
                if (htmlToken.attributeList != null)
                {
                    htmlToken.attributeList[0].partMajor = HtmlToken.AttrPartMajor.None;
                    htmlToken.attributeList[0].partMinor = HtmlToken.AttrPartMinor.Empty;
                }
            }
            if (state == 21)
            {
                htmlToken.unstructured.Initialize(0, tailOffset);
            }
            else
            {
                htmlToken.unstructured.Reset();
            }
            if (state == 22)
            {
                htmlToken.name.Initialize(0, tailOffset);
                htmlToken.localName.Initialize(0, tailOffset);
            }
            else
            {
                htmlToken.name.Reset();
                htmlToken.localName.Reset();
            }
            htmlToken.attributeTail = 0;
            htmlToken.currentAttribute = -1;
            htmlToken.partMajor = HtmlToken.TagPartMajor.Continue;
            tokenValid = false;
            return htmlToken.whole.headOffset;
        }

        public HtmlNameIndex LookupName(int nameLength, Token.LexicalUnit unit)
        {
            if (nameLength != 0 && nameLength <= 14)
            {
                short num = (short)((ulong)(token.CalculateHashLowerCase(unit) ^ 221) % 601uL);
                int num2 = (int)HtmlNameData.nameHashTable[(int)num];
                if (num2 > 0)
                {
                    while (true)
                    {
                        string name = HtmlNameData.names[num2].name;
                        if (name.Length == nameLength)
                        {
                            if (token.IsContiguous(unit))
                            {
                                if (name[0] == ParseSupport.ToLowerCase(token.buffer[unit.headOffset]) && (nameLength == 1 || token.CaseInsensitiveCompareRunEqual(unit.headOffset + 1, name, 1)))
                                {
                                    break;
                                }
                            }
                            else if (token.CaseInsensitiveCompareEqual(unit, name))
                            {
                                goto Block_7;
                            }
                        }
                        num2++;
                        if (HtmlNameData.names[num2].hash != num)
                        {
                            return HtmlNameIndex.Unknown;
                        }
                    }
                    return (HtmlNameIndex)num2;
                    Block_7:
                    return (HtmlNameIndex)num2;
                }
            }
            return HtmlNameIndex.Unknown;
        }

        public bool PrepareToAddMoreRuns(int numRuns, int start, HtmlRunKind skippedRunKind)
        {
            return base.PrepareToAddMoreRuns(numRuns, start, (uint)skippedRunKind);
        }

        public void AddNullRun(HtmlRunKind kind)
        {
            base.AddNullRun((uint)kind);
        }

        public void AddRun(RunTextType textType, HtmlRunKind kind, int start, int end)
        {
            base.AddRun((RunType)2147483648u, textType, (uint)kind, start, end, 0);
        }

        public void AddLiteralRun(RunTextType textType, HtmlRunKind kind, int start, int end, int literal)
        {
            base.AddRun((RunType)3221225472u, textType, (uint)kind, start, end, literal);
        }

        protected override void Rebase(int deltaOffset)
        {
            HtmlToken expr_0B_cp_0 = htmlToken;
            expr_0B_cp_0.unstructured.headOffset = expr_0B_cp_0.unstructured.headOffset + deltaOffset;
            HtmlToken expr_23_cp_0 = htmlToken;
            expr_23_cp_0.unstructuredPosition.runOffset = expr_23_cp_0.unstructuredPosition.runOffset + deltaOffset;
            HtmlToken expr_3B_cp_0 = htmlToken;
            expr_3B_cp_0.name.headOffset = expr_3B_cp_0.name.headOffset + deltaOffset;
            HtmlToken expr_53_cp_0 = htmlToken;
            expr_53_cp_0.localName.headOffset = expr_53_cp_0.localName.headOffset + deltaOffset;
            HtmlToken expr_6B_cp_0 = htmlToken;
            expr_6B_cp_0.namePosition.runOffset = expr_6B_cp_0.namePosition.runOffset + deltaOffset;
            for (int i = 0; i < htmlToken.attributeTail; i++)
            {
                HtmlToken.AttributeEntry[] expr_92_cp_0_cp_0 = htmlToken.attributeList;
                int expr_92_cp_0_cp_1 = i;
                expr_92_cp_0_cp_0[expr_92_cp_0_cp_1].name.headOffset = expr_92_cp_0_cp_0[expr_92_cp_0_cp_1].name.headOffset + deltaOffset;
                HtmlToken.AttributeEntry[] expr_B5_cp_0_cp_0 = htmlToken.attributeList;
                int expr_B5_cp_0_cp_1 = i;
                expr_B5_cp_0_cp_0[expr_B5_cp_0_cp_1].localName.headOffset = expr_B5_cp_0_cp_0[expr_B5_cp_0_cp_1].localName.headOffset + deltaOffset;
                HtmlToken.AttributeEntry[] expr_D8_cp_0_cp_0 = htmlToken.attributeList;
                int expr_D8_cp_0_cp_1 = i;
                expr_D8_cp_0_cp_0[expr_D8_cp_0_cp_1].value.headOffset = expr_D8_cp_0_cp_0[expr_D8_cp_0_cp_1].value.headOffset + deltaOffset;
            }
            if (state >= 24)
            {
                HtmlToken.AttributeEntry[] expr_124_cp_0_cp_0 = htmlToken.attributeList;
                int expr_124_cp_0_cp_1 = htmlToken.attributeTail;
                expr_124_cp_0_cp_0[expr_124_cp_0_cp_1].name.headOffset = expr_124_cp_0_cp_0[expr_124_cp_0_cp_1].name.headOffset + deltaOffset;
                HtmlToken.AttributeEntry[] expr_151_cp_0_cp_0 = htmlToken.attributeList;
                int expr_151_cp_0_cp_1 = htmlToken.attributeTail;
                expr_151_cp_0_cp_0[expr_151_cp_0_cp_1].localName.headOffset = expr_151_cp_0_cp_0[expr_151_cp_0_cp_1].localName.headOffset + deltaOffset;
                HtmlToken.AttributeEntry[] expr_17E_cp_0_cp_0 = htmlToken.attributeList;
                int expr_17E_cp_0_cp_1 = htmlToken.attributeTail;
                expr_17E_cp_0_cp_0[expr_17E_cp_0_cp_1].value.headOffset = expr_17E_cp_0_cp_0[expr_17E_cp_0_cp_1].value.headOffset + deltaOffset;
            }
            HtmlToken expr_196_cp_0 = htmlToken;
            expr_196_cp_0.attrNamePosition.runOffset = expr_196_cp_0.attrNamePosition.runOffset + deltaOffset;
            HtmlToken expr_1AE_cp_0 = htmlToken;
            expr_1AE_cp_0.attrValuePosition.runOffset = expr_1AE_cp_0.attrValuePosition.runOffset + deltaOffset;
            carryOverRunsHeadOffset += deltaOffset;
            base.Rebase(deltaOffset);
        }
    }
}
