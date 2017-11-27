using AgileFramework.Security.Application.TextConverters.HTML;
using AgileFramework.Security.Exchange.Data.Globalization;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Format;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Text;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlToTextConverter : IProducerConsumer, IRestartable, IReusable, IDisposable
    {
        private struct NormalizerContext
        {
            public char lastCh;

            public bool oneNL;

            public bool hasSpace;

            public bool eatSpace;
        }

        private bool convertFragment;

        private IHtmlParser parser;

        private bool endOfFile;

        private TextOutput output;

        private HtmlToken token;

        private bool treatNbspAsBreakable;

        private bool outputImageLinks = true;

        private bool outputAnchorLinks = true;

        protected bool normalizedInput;

        private NormalizerContext normalizerContext;

        private TextMapping textMapping;

        private bool lineStarted;

        private bool wideGap;

        private bool nextParagraphCloseWideGap = true;

        private bool afterFirstParagraph;

        private bool ignoreNextP;

        private int listLevel;

        private int listIndex;

        private bool listOrdered;

        private bool insideComment;

        private bool insidePre;

        private bool insideAnchor;

        private ScratchBuffer urlScratch;

        private int imageHeightPixels;

        private int imageWidthPixels;

        private ScratchBuffer imageAltText;

        private ScratchBuffer scratch;

        private Injection injection;

        private UrlCompareSink urlCompareSink;

        public HtmlToTextConverter(IHtmlParser parser, TextOutput output, Injection injection, bool convertFragment, bool preformattedText, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
        {
            normalizedInput = (parser is HtmlNormalizingParser);
            treatNbspAsBreakable = testTreatNbspAsBreakable;
            this.convertFragment = convertFragment;
            this.parser = parser;
            this.parser.SetRestartConsumer(this);
            if (!convertFragment)
            {
                this.injection = injection;
                if (this.injection != null && this.injection.HaveHead)
                {
                    this.injection.Inject(true, this.output);
                    return;
                }
            }
            else
            {
                insidePre = preformattedText;
            }
        }

        private void Reinitialize()
        {
            endOfFile = false;
            normalizerContext.hasSpace = false;
            normalizerContext.eatSpace = false;
            normalizerContext.oneNL = false;
            normalizerContext.lastCh = '\0';
            lineStarted = false;
            wideGap = false;
            nextParagraphCloseWideGap = true;
            afterFirstParagraph = false;
            ignoreNextP = false;
            insideComment = false;
            insidePre = false;
            insideAnchor = false;
            if (urlCompareSink != null)
            {
                urlCompareSink.Reset();
            }
            listLevel = 0;
            listIndex = 0;
            listOrdered = false;
            if (!convertFragment && injection != null)
            {
                injection.Reset();
                if (injection.HaveHead)
                {
                    injection.Inject(true, output);
                }
            }
            textMapping = TextMapping.Unicode;
        }

        public void Run()
        {
            if (!endOfFile)
            {
                HtmlTokenId htmlTokenId = parser.Parse();
                if (htmlTokenId != HtmlTokenId.None)
                {
                    Process(htmlTokenId);
                }
            }
        }

        public bool Flush()
        {
            if (!endOfFile)
            {
                Run();
            }
            return endOfFile;
        }

        private void Process(HtmlTokenId tokenId)
        {
            token = parser.Token;
            switch (tokenId)
            {
                case HtmlTokenId.EndOfFile:
                    if (lineStarted)
                    {
                        output.OutputNewLine();
                        lineStarted = false;
                    }
                    if (!convertFragment)
                    {
                        if (injection != null && injection.HaveHead)
                        {
                            if (wideGap)
                            {
                                output.OutputNewLine();
                                wideGap = false;
                            }
                            injection.Inject(false, output);
                        }
                        output.CloseDocument();
                        output.Flush();
                    }
                    endOfFile = true;
                    break;
                case HtmlTokenId.Text:
                    if (!insideComment)
                    {
                        if (insideAnchor && urlCompareSink.IsActive)
                        {
                            token.Text.WriteTo(urlCompareSink);
                        }
                        if (insidePre)
                        {
                            ProcessPreformatedText();
                            return;
                        }
                        if (normalizedInput)
                        {
                            ProcessText();
                            return;
                        }
                        NormalizeProcessText();
                        return;
                    }
                    break;
                case HtmlTokenId.EncodingChange:
                    if (output.OutputCodePageSameAsInput)
                    {
                        int argument = token.Argument;
                        output.OutputEncoding = Charset.GetEncoding(argument);
                        return;
                    }
                    break;
                case HtmlTokenId.Tag:
                    {
                        if (token.TagIndex <= HtmlTagIndex.Unknown)
                        {
                            return;
                        }
                        HtmlDtd.TagDefinition tagDefinition = GetTagDefinition(token.TagIndex);
                        if (normalizedInput)
                        {
                            if (!token.IsEndTag)
                            {
                                if (token.IsTagBegin)
                                {
                                    PushElement(tagDefinition);
                                }
                                ProcessStartTagAttributes(tagDefinition);
                                return;
                            }
                            if (token.IsTagBegin)
                            {
                                PopElement(tagDefinition);
                                return;
                            }
                        }
                        else
                        {
                            if (!token.IsEndTag)
                            {
                                if (token.IsTagBegin)
                                {
                                    LFillTagB(tagDefinition);
                                    PushElement(tagDefinition);
                                    RFillTagB(tagDefinition);
                                }
                                ProcessStartTagAttributes(tagDefinition);
                                return;
                            }
                            if (token.IsTagBegin)
                            {
                                LFillTagE(tagDefinition);
                                PopElement(tagDefinition);
                                RFillTagE(tagDefinition);
                                return;
                            }
                        }
                        break;
                    }
                case HtmlTokenId.Restart:
                case HtmlTokenId.OverlappedClose:
                case HtmlTokenId.OverlappedReopen:
                    break;
                default:
                    return;
            }
        }

        private void PushElement(HtmlDtd.TagDefinition tagDef)
        {
            HtmlTagIndex tagIndex = tagDef.tagIndex;
            if (tagIndex <= HtmlTagIndex.Listing)
            {
                if (tagIndex <= HtmlTagIndex.DT)
                {
                    if (tagIndex != HtmlTagIndex.A)
                    {
                        if (tagIndex == HtmlTagIndex.BR)
                        {
                            goto IL_193;
                        }
                        switch (tagIndex)
                        {
                            case HtmlTagIndex.Comment:
                                break;
                            case HtmlTagIndex.DD:
                                if (lineStarted)
                                {
                                    EndLine();
                                    goto IL_2FF;
                                }
                                goto IL_2FF;
                            case HtmlTagIndex.Del:
                            case HtmlTagIndex.Dfn:
                            case HtmlTagIndex.Div:
                                goto IL_2F0;
                            case HtmlTagIndex.Dir:
                                goto IL_1BC;
                            case HtmlTagIndex.DL:
                                EndParagraph(true);
                                goto IL_2FF;
                            case HtmlTagIndex.DT:
                                if (lineStarted)
                                {
                                    EndLine();
                                    goto IL_2FF;
                                }
                                goto IL_2FF;
                            default:
                                goto IL_2F0;
                        }
                    }
                    else
                    {
                        if (insideAnchor)
                        {
                            EndAnchor();
                            goto IL_2FF;
                        }
                        goto IL_2FF;
                    }
                }
                else if (tagIndex <= HtmlTagIndex.HR)
                {
                    if (tagIndex == HtmlTagIndex.Font)
                    {
                        goto IL_2FF;
                    }
                    if (tagIndex != HtmlTagIndex.HR)
                    {
                        goto IL_2F0;
                    }
                    EndParagraph(false);
                    OutputText("________________________________");
                    EndParagraph(false);
                    goto IL_2FF;
                }
                else
                {
                    switch (tagIndex)
                    {
                        case HtmlTagIndex.Image:
                        case HtmlTagIndex.Img:
                            goto IL_2FF;
                        default:
                            switch (tagIndex)
                            {
                                case HtmlTagIndex.LI:
                                    {
                                        EndParagraph(false);
                                        OutputText("  ");
                                        for (int i = 0; i < listLevel - 1; i++)
                                        {
                                            OutputText("   ");
                                        }
                                        if (listLevel > 1 || !listOrdered)
                                        {
                                            OutputText("*");
                                            output.OutputSpace(3);
                                            goto IL_2FF;
                                        }
                                        string text = listIndex.ToString();
                                        OutputText(text);
                                        OutputText(".");
                                        output.OutputSpace((text.Length == 1) ? 2 : 1);
                                        listIndex++;
                                        goto IL_2FF;
                                    }
                                case HtmlTagIndex.Link:
                                    goto IL_2F0;
                                case HtmlTagIndex.Listing:
                                    goto IL_2E0;
                                default:
                                    goto IL_2F0;
                            }
                            //break;
                    }
                }
            }
            else if (tagIndex <= HtmlTagIndex.Style)
            {
                if (tagIndex <= HtmlTagIndex.Script)
                {
                    switch (tagIndex)
                    {
                        case HtmlTagIndex.Menu:
                        case HtmlTagIndex.OL:
                            goto IL_1BC;
                        case HtmlTagIndex.Meta:
                        case HtmlTagIndex.NextId:
                        case HtmlTagIndex.NoBR:
                        case HtmlTagIndex.NoScript:
                        case HtmlTagIndex.Object:
                        case HtmlTagIndex.OptGroup:
                        case HtmlTagIndex.Param:
                            goto IL_2F0;
                        case HtmlTagIndex.NoEmbed:
                        case HtmlTagIndex.NoFrames:
                            break;
                        case HtmlTagIndex.Option:
                            goto IL_193;
                        case HtmlTagIndex.P:
                            if (!ignoreNextP)
                            {
                                EndParagraph(true);
                            }
                            nextParagraphCloseWideGap = true;
                            goto IL_2FF;
                        case HtmlTagIndex.PlainText:
                        case HtmlTagIndex.Pre:
                            goto IL_2E0;
                        default:
                            if (tagIndex != HtmlTagIndex.Script)
                            {
                                goto IL_2F0;
                            }
                            break;
                    }
                }
                else
                {
                    if (tagIndex == HtmlTagIndex.Span)
                    {
                        goto IL_2FF;
                    }
                    if (tagIndex != HtmlTagIndex.Style)
                    {
                        goto IL_2F0;
                    }
                }
            }
            else if (tagIndex <= HtmlTagIndex.Title)
            {
                if (tagIndex != HtmlTagIndex.TD)
                {
                    switch (tagIndex)
                    {
                        case HtmlTagIndex.TH:
                            break;
                        case HtmlTagIndex.Thead:
                            goto IL_2F0;
                        case HtmlTagIndex.Title:
                            goto IL_13A;
                        default:
                            goto IL_2F0;
                    }
                }
                if (lineStarted)
                {
                    output.OutputTabulation(1);
                    goto IL_2FF;
                }
                goto IL_2FF;
            }
            else
            {
                if (tagIndex == HtmlTagIndex.UL)
                {
                    goto IL_1BC;
                }
                if (tagIndex != HtmlTagIndex.Xmp)
                {
                    goto IL_2F0;
                }
                goto IL_2E0;
            }
            IL_13A:
            insideComment = true;
            goto IL_2FF;
            IL_193:
            EndLine();
            goto IL_2FF;
            IL_1BC:
            EndParagraph(listLevel == 0);
            if (listLevel < 10)
            {
                listLevel++;
                if (listLevel == 1)
                {
                    listIndex = 1;
                    listOrdered = (token.TagIndex == HtmlTagIndex.OL);
                }
            }
            nextParagraphCloseWideGap = false;
            goto IL_2FF;
            IL_2E0:
            EndParagraph(true);
            insidePre = true;
            goto IL_2FF;
            IL_2F0:
            if (tagDef.blockElement)
            {
                EndParagraph(false);
            }
            IL_2FF:
            ignoreNextP = false;
            if (tagDef.tagIndex == HtmlTagIndex.LI)
            {
                ignoreNextP = true;
            }
        }

        private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
        {
            HtmlTagIndex tagIndex = tagDef.tagIndex;
            if (tagIndex <= HtmlTagIndex.Font)
            {
                if (tagIndex != HtmlTagIndex.A)
                {
                    if (tagIndex != HtmlTagIndex.Font)
                    {
                        return;
                    }
                    HtmlToken.AttributeEnumerator enumerator = token.Attributes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        HtmlAttribute current = enumerator.Current;
                        if (current.NameIndex == HtmlNameIndex.Face)
                        {
                            scratch.Reset();
                            scratch.AppendHtmlAttributeValue(current, 4096);
                            RecognizeInterestingFontName recognizeInterestingFontName = default(RecognizeInterestingFontName);
                            int num = 0;
                            while (num < scratch.Length && !recognizeInterestingFontName.IsRejected)
                            {
                                recognizeInterestingFontName.AddCharacter(scratch.Buffer[num]);
                                num++;
                            }
                            textMapping = recognizeInterestingFontName.TextMapping;
                            return;
                        }
                    }
                    return;
                }
                else if (outputAnchorLinks)
                {
                    HtmlToken.AttributeEnumerator enumerator2 = token.Attributes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        HtmlAttribute current2 = enumerator2.Current;
                        if (current2.NameIndex == HtmlNameIndex.Href)
                        {
                            if (current2.IsAttrBegin)
                            {
                                urlScratch.Reset();
                            }
                            urlScratch.AppendHtmlAttributeValue(current2, 4096);
                            break;
                        }
                    }
                    if (token.IsTagEnd)
                    {
                        BufferString bufferString = urlScratch.BufferString;
                        bufferString.TrimWhitespace();
                        if (bufferString.Length != 0 && bufferString[0] != '#' && bufferString[0] != '?' && bufferString[0] != ';')
                        {
                            if (!lineStarted)
                            {
                                StartParagraphOrLine();
                            }
                            string text = bufferString.ToString();
                            if (text.IndexOf(' ') != -1)
                            {
                                text = text.Replace(" ", "%20");
                            }
                            output.AnchorUrl = text;
                            insideAnchor = true;
                            if (urlCompareSink == null)
                            {
                                urlCompareSink = new UrlCompareSink();
                            }
                            urlCompareSink.Initialize(text);
                        }
                        urlScratch.Reset();
                        return;
                    }
                }
            }
            else
            {
                switch (tagIndex)
                {
                    case HtmlTagIndex.Image:
                    case HtmlTagIndex.Img:
                        if (outputImageLinks)
                        {
                            HtmlToken.AttributeEnumerator enumerator3 = token.Attributes.GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                HtmlAttribute current3 = enumerator3.Current;
                                if (current3.NameIndex == HtmlNameIndex.Src)
                                {
                                    if (current3.IsAttrBegin)
                                    {
                                        urlScratch.Reset();
                                    }
                                    urlScratch.AppendHtmlAttributeValue(current3, 4096);
                                }
                                else if (current3.NameIndex == HtmlNameIndex.Alt)
                                {
                                    if (current3.IsAttrBegin)
                                    {
                                        imageAltText.Reset();
                                    }
                                    imageAltText.AppendHtmlAttributeValue(current3, 4096);
                                }
                                else if (current3.NameIndex == HtmlNameIndex.Height)
                                {
                                    if (!current3.Value.IsEmpty)
                                    {
                                        PropertyValue propertyValue;
                                        if (current3.Value.IsContiguous)
                                        {
                                            propertyValue = HtmlSupport.ParseNumber(current3.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        else
                                        {
                                            scratch.Reset();
                                            scratch.AppendHtmlAttributeValue(current3, 4096);
                                            propertyValue = HtmlSupport.ParseNumber(scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                        }
                                        if (propertyValue.IsAbsRelLength)
                                        {
                                            imageHeightPixels = propertyValue.PixelsInteger;
                                            if (imageHeightPixels == 0)
                                            {
                                                imageHeightPixels = 1;
                                            }
                                        }
                                    }
                                }
                                else if (current3.NameIndex == HtmlNameIndex.Width && !current3.Value.IsEmpty)
                                {
                                    PropertyValue propertyValue2;
                                    if (current3.Value.IsContiguous)
                                    {
                                        propertyValue2 = HtmlSupport.ParseNumber(current3.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                                    }
                                    else
                                    {
                                        scratch.Reset();
                                        scratch.AppendHtmlAttributeValue(current3, 4096);
                                        propertyValue2 = HtmlSupport.ParseNumber(scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                                    }
                                    if (propertyValue2.IsAbsRelLength)
                                    {
                                        imageWidthPixels = propertyValue2.PixelsInteger;
                                        if (imageWidthPixels == 0)
                                        {
                                            imageWidthPixels = 1;
                                        }
                                    }
                                }
                            }
                            if (token.IsTagEnd)
                            {
                                string imageUrl = null;
                                string text2 = null;
                                BufferString bufferString2 = imageAltText.BufferString;
                                bufferString2.TrimWhitespace();
                                if (bufferString2.Length != 0)
                                {
                                    text2 = bufferString2.ToString();
                                }
                                if (text2 == null || output.ImageRenderingCallbackDefined)
                                {
                                    BufferString bufferString3 = urlScratch.BufferString;
                                    bufferString3.TrimWhitespace();
                                    if (bufferString3.Length != 0)
                                    {
                                        imageUrl = bufferString3.ToString();
                                    }
                                }
                                if (!lineStarted)
                                {
                                    StartParagraphOrLine();
                                }
                                output.OutputImage(imageUrl, text2, imageWidthPixels, imageHeightPixels);
                                urlScratch.Reset();
                                imageAltText.Reset();
                                imageHeightPixels = 0;
                                imageWidthPixels = 0;
                                return;
                            }
                        }
                        break;
                    default:
                        if (tagIndex != HtmlTagIndex.P)
                        {
                            if (tagIndex != HtmlTagIndex.Span)
                            {
                                return;
                            }
                            HtmlToken.AttributeEnumerator enumerator4 = token.Attributes.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                HtmlAttribute current4 = enumerator4.Current;
                                if (current4.NameIndex == HtmlNameIndex.Style)
                                {
                                    scratch.Reset();
                                    scratch.AppendHtmlAttributeValue(current4, 4096);
                                    RecognizeInterestingFontNameInInlineStyle recognizeInterestingFontNameInInlineStyle = default(RecognizeInterestingFontNameInInlineStyle);
                                    int num2 = 0;
                                    while (num2 < scratch.Length && !recognizeInterestingFontNameInInlineStyle.IsFinished)
                                    {
                                        recognizeInterestingFontNameInInlineStyle.AddCharacter(scratch.Buffer[num2]);
                                        num2++;
                                    }
                                    textMapping = recognizeInterestingFontNameInInlineStyle.TextMapping;
                                    return;
                                }
                            }
                        }
                        else if (token.Attributes.Find(HtmlNameIndex.Class))
                        {
                            HtmlAttribute current5 = token.Attributes.Current;
                            if (current5.Value.CaseInsensitiveCompareEqual("msonormal"))
                            {
                                wideGap = false;
                                nextParagraphCloseWideGap = false;
                                return;
                            }
                        }
                        break;
                }
            }
        }

        private void PopElement(HtmlDtd.TagDefinition tagDef)
        {
            HtmlTagIndex tagIndex = tagDef.tagIndex;
            if (tagIndex <= HtmlTagIndex.Listing)
            {
                if (tagIndex <= HtmlTagIndex.DT)
                {
                    if (tagIndex <= HtmlTagIndex.BR)
                    {
                        if (tagIndex != HtmlTagIndex.A)
                        {
                            if (tagIndex != HtmlTagIndex.BR)
                            {
                                goto IL_1D6;
                            }
                            goto IL_173;
                        }
                        else
                        {
                            if (insideAnchor)
                            {
                                EndAnchor();
                                goto IL_1E5;
                            }
                            goto IL_1E5;
                        }
                    }
                    else
                    {
                        switch (tagIndex)
                        {
                            case HtmlTagIndex.Comment:
                                break;
                            case HtmlTagIndex.DD:
                                goto IL_1E5;
                            case HtmlTagIndex.Del:
                            case HtmlTagIndex.Dfn:
                                goto IL_1D6;
                            case HtmlTagIndex.Dir:
                                goto IL_196;
                            default:
                                if (tagIndex != HtmlTagIndex.DT)
                                {
                                    goto IL_1D6;
                                }
                                goto IL_1E5;
                        }
                    }
                }
                else if (tagIndex <= HtmlTagIndex.HR)
                {
                    if (tagIndex == HtmlTagIndex.Font)
                    {
                        goto IL_1CD;
                    }
                    if (tagIndex != HtmlTagIndex.HR)
                    {
                        goto IL_1D6;
                    }
                    EndParagraph(false);
                    OutputText("________________________________");
                    EndParagraph(false);
                    goto IL_1E5;
                }
                else
                {
                    switch (tagIndex)
                    {
                        case HtmlTagIndex.Image:
                        case HtmlTagIndex.Img:
                            goto IL_1E5;
                        default:
                            if (tagIndex != HtmlTagIndex.Listing)
                            {
                                goto IL_1D6;
                            }
                            goto IL_1BD;
                    }
                }
            }
            else if (tagIndex <= HtmlTagIndex.Style)
            {
                if (tagIndex <= HtmlTagIndex.Script)
                {
                    switch (tagIndex)
                    {
                        case HtmlTagIndex.Menu:
                        case HtmlTagIndex.OL:
                            goto IL_196;
                        case HtmlTagIndex.Meta:
                        case HtmlTagIndex.NextId:
                        case HtmlTagIndex.NoBR:
                        case HtmlTagIndex.NoScript:
                        case HtmlTagIndex.Object:
                        case HtmlTagIndex.OptGroup:
                        case HtmlTagIndex.Param:
                            goto IL_1D6;
                        case HtmlTagIndex.NoEmbed:
                        case HtmlTagIndex.NoFrames:
                            break;
                        case HtmlTagIndex.Option:
                            goto IL_173;
                        case HtmlTagIndex.P:
                            EndParagraph(nextParagraphCloseWideGap);
                            nextParagraphCloseWideGap = true;
                            goto IL_1E5;
                        case HtmlTagIndex.PlainText:
                        case HtmlTagIndex.Pre:
                            goto IL_1BD;
                        default:
                            if (tagIndex != HtmlTagIndex.Script)
                            {
                                goto IL_1D6;
                            }
                            break;
                    }
                }
                else
                {
                    if (tagIndex == HtmlTagIndex.Span)
                    {
                        goto IL_1CD;
                    }
                    if (tagIndex != HtmlTagIndex.Style)
                    {
                        goto IL_1D6;
                    }
                }
            }
            else
            {
                if (tagIndex <= HtmlTagIndex.Title)
                {
                    if (tagIndex != HtmlTagIndex.TD)
                    {
                        switch (tagIndex)
                        {
                            case HtmlTagIndex.TH:
                                break;
                            case HtmlTagIndex.Thead:
                                goto IL_1D6;
                            case HtmlTagIndex.Title:
                                goto IL_130;
                            default:
                                goto IL_1D6;
                        }
                    }
                    lineStarted = true;
                    goto IL_1E5;
                }
                if (tagIndex == HtmlTagIndex.UL)
                {
                    goto IL_196;
                }
                if (tagIndex != HtmlTagIndex.Xmp)
                {
                    goto IL_1D6;
                }
                goto IL_1BD;
            }
            IL_130:
            insideComment = false;
            goto IL_1E5;
            IL_173:
            EndLine();
            goto IL_1E5;
            IL_196:
            if (listLevel != 0)
            {
                listLevel--;
            }
            EndParagraph(listLevel == 0);
            goto IL_1E5;
            IL_1BD:
            EndParagraph(true);
            insidePre = false;
            goto IL_1E5;
            IL_1CD:
            textMapping = TextMapping.Unicode;
            goto IL_1E5;
            IL_1D6:
            if (tagDef.blockElement)
            {
                EndParagraph(false);
            }
            IL_1E5:
            ignoreNextP = false;
        }

        private void ProcessText()
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }
            Token.RunEnumerator enumerator = token.Runs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TokenRun current = enumerator.Current;
                if (current.IsTextRun)
                {
                    if (current.IsAnyWhitespace)
                    {
                        output.OutputSpace(1);
                    }
                    else if (current.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            output.OutputSpace(current.Length);
                        }
                        else
                        {
                            output.OutputNbsp(current.Length);
                        }
                    }
                    else if (current.IsLiteral)
                    {
                        output.OutputNonspace(current.Literal, textMapping);
                    }
                    else
                    {
                        output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, textMapping);
                    }
                }
            }
        }

        private void ProcessPreformatedText()
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }
            Token.RunEnumerator enumerator = token.Runs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TokenRun current = enumerator.Current;
                if (current.IsTextRun)
                {
                    if (current.IsAnyWhitespace)
                    {
                        RunTextType textType = current.TextType;
                        if (textType != RunTextType.Space)
                        {
                            if (textType == RunTextType.NewLine)
                            {
                                output.OutputNewLine();
                                continue;
                            }
                            if (textType == RunTextType.Tabulation)
                            {
                                output.OutputTabulation(current.Length);
                                continue;
                            }
                        }
                        if (treatNbspAsBreakable)
                        {
                            output.OutputSpace(current.Length);
                        }
                        else
                        {
                            output.OutputNbsp(current.Length);
                        }
                    }
                    else if (current.TextType == RunTextType.Nbsp)
                    {
                        if (treatNbspAsBreakable)
                        {
                            output.OutputSpace(current.Length);
                        }
                        else
                        {
                            output.OutputNbsp(current.Length);
                        }
                    }
                    else if (current.IsLiteral)
                    {
                        output.OutputNonspace(current.Literal, textMapping);
                    }
                    else
                    {
                        output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, textMapping);
                    }
                }
            }
        }

        private void NormalizeProcessText()
        {
            Token.RunEnumerator runs = token.Runs;
            runs.MoveNext(true);
            while (runs.IsValidPosition)
            {
                TokenRun current = runs.Current;
                if (current.IsAnyWhitespace)
                {
                    int num = 0;
                    TokenRun current3;
                    do
                    {
                        int arg_48_0 = num;
                        TokenRun current2 = runs.Current;
                        num = arg_48_0 + ((current2.TextType == RunTextType.NewLine) ? 1 : 2);
                        if (!runs.MoveNext(true))
                        {
                            break;
                        }
                        current3 = runs.Current;
                    }
                    while (current3.TextType <= RunTextType.UnusualWhitespace);
                    NormalizeAddSpace(num == 1);
                }
                else if (current.TextType == RunTextType.Nbsp)
                {
                    NormalizeAddNbsp(current.Length);
                    runs.MoveNext(true);
                }
                else
                {
                    NormalizeAddNonspace(current);
                    runs.MoveNext(true);
                }
            }
        }

        private void NormalizeAddNonspace(TokenRun run)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }
            if (normalizerContext.hasSpace)
            {
                normalizerContext.hasSpace = false;
                if (normalizerContext.lastCh == '\0' || !normalizerContext.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(normalizerContext.lastCh, run.FirstChar))
                {
                    output.OutputSpace(1);
                }
            }
            if (run.IsLiteral)
            {
                output.OutputNonspace(run.Literal, textMapping);
            }
            else
            {
                output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, textMapping);
            }
            normalizerContext.eatSpace = false;
            normalizerContext.lastCh = run.LastChar;
            normalizerContext.oneNL = false;
        }

        private void NormalizeAddNbsp(int count)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }
            if (normalizerContext.hasSpace)
            {
                normalizerContext.hasSpace = false;
                output.OutputSpace(1);
            }
            if (treatNbspAsBreakable)
            {
                output.OutputSpace(count);
            }
            else
            {
                output.OutputNbsp(count);
            }
            normalizerContext.eatSpace = false;
            normalizerContext.lastCh = '\u00a0';
            normalizerContext.oneNL = false;
        }

        private void NormalizeAddSpace(bool oneNL)
        {
            if (!normalizerContext.eatSpace && afterFirstParagraph)
            {
                normalizerContext.hasSpace = true;
            }
            if (normalizerContext.lastCh != '\0')
            {
                if (oneNL && !normalizerContext.oneNL)
                {
                    normalizerContext.oneNL = true;
                    return;
                }
                normalizerContext.lastCh = '\0';
            }
        }

        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                LFill(tagDef.fill.LB);
            }
        }

        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                RFill(tagDef.fill.RB);
            }
        }

        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                LFill(tagDef.fill.LE);
            }
        }

        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (!insidePre)
            {
                RFill(tagDef.fill.RE);
            }
        }

        private void LFill(HtmlDtd.FillCode codeLeft)
        {
            normalizerContext.lastCh = '\0';
            if (normalizerContext.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    if (!lineStarted)
                    {
                        StartParagraphOrLine();
                    }
                    output.OutputSpace(1);
                    normalizerContext.eatSpace = true;
                }
                normalizerContext.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }
        }

        private void RFill(HtmlDtd.FillCode code)
        {
            if (code == HtmlDtd.FillCode.EAT)
            {
                normalizerContext.hasSpace = false;
                normalizerContext.eatSpace = true;
                return;
            }
            if (code == HtmlDtd.FillCode.PUT)
            {
                normalizerContext.eatSpace = false;
            }
        }

        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            if (tagIndex == HtmlTagIndex._NULL)
            {
                return null;
            }
            return HtmlDtd.tags[(int)tagIndex];
        }

        private void EndAnchor()
        {
            if (!urlCompareSink.IsMatch)
            {
                if (!lineStarted)
                {
                    StartParagraphOrLine();
                }
                output.CloseAnchor();
            }
            else
            {
                output.CancelAnchor();
            }
            insideAnchor = false;
            urlCompareSink.Reset();
        }

        private void OutputText(string text)
        {
            if (!lineStarted)
            {
                StartParagraphOrLine();
            }
            output.OutputNonspace(text, textMapping);
        }

        private void StartParagraphOrLine()
        {
            if (wideGap)
            {
                if (afterFirstParagraph)
                {
                    output.OutputNewLine();
                }
                wideGap = false;
            }
            lineStarted = true;
            afterFirstParagraph = true;
        }

        private void EndLine()
        {
            output.OutputNewLine();
            lineStarted = false;
            wideGap = false;
        }

        private void EndParagraph(bool wideGap)
        {
            if (insideAnchor)
            {
                EndAnchor();
            }
            if (lineStarted)
            {
                output.OutputNewLine();
                lineStarted = false;
            }
            this.wideGap = (this.wideGap || wideGap);
        }

        void IDisposable.Dispose()
        {
            if (parser != null)
            {
                ((IDisposable)parser).Dispose();
            }
            if (!convertFragment && output != null && output != null)
            {
                ((IDisposable)output).Dispose();
            }
            if (token != null && token is IDisposable)
            {
                ((IDisposable)token).Dispose();
            }
            parser = null;
            output = null;
            token = null;
            GC.SuppressFinalize(this);
        }

        bool IRestartable.CanRestart()
        {
            return convertFragment || ((IRestartable)output).CanRestart();
        }

        void IRestartable.Restart()
        {
            if (!convertFragment)
            {
                ((IRestartable)output).Restart();
            }
            Reinitialize();
        }

        void IRestartable.DisableRestart()
        {
            if (!convertFragment)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            ((IReusable)parser).Initialize(newSourceOrDestination);
            ((IReusable)output).Initialize(newSourceOrDestination);
            Reinitialize();
            parser.SetRestartConsumer(this);
        }

        public void Initialize(string fragment, bool preformatedText)
        {
            if (normalizedInput)
            {
                ((HtmlNormalizingParser)parser).Initialize(fragment, preformatedText);
            }
            else
            {
                ((HtmlParser)parser).Initialize(fragment, preformatedText);
            }
            if (!convertFragment)
            {
                ((IReusable)output).Initialize(null);
            }
            Reinitialize();
        }
    }
}
