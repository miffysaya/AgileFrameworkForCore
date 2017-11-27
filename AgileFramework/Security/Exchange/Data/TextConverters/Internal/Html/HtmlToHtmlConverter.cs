using AgileFramework.Security.Application.TextConverters.HTML;
using AgileFramework.Security.Exchange.Data.Globalization;
using AgileFramework.Security.Exchange.Data.Internal;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlToHtmlConverter : IProducerConsumer, IRestartable, IDisposable
    {
        internal enum CopyPendingState : byte
        {
            NotPending,
            TagCopyPending,
            TagContentCopyPending,
            TagNameCopyPending,
            AttributeCopyPending,
            AttributeNameCopyPending,
            AttributeValueCopyPending
        }

        [Flags]
        private enum AvailableTagParts : byte
        {
            None = 0,
            TagBegin = 1,
            TagEnd = 2,
            TagName = 4,
            Attributes = 8,
            UnstructuredContent = 16
        }

        private enum AttributeIndirectKind
        {
            PassThrough,
            EmptyValue,
            FilteredStyle,
            Virtual,
            VirtualFilteredStyle,
            NameOnlyFragment
        }

        private struct AttributeIndirectEntry
        {
            public AttributeIndirectKind kind;

            public short index;
        }

        private struct AttributeVirtualEntry
        {
            public short index;

            public int offset;

            public int length;

            public int position;
        }

        private struct EndTagActionEntry
        {
            public int tagLevel;

            public bool drop;

            public bool callback;
        }

        private enum CheckUrlResult
        {
            Inconclusive,
            Unsafe,
            Safe,
            LocalHyperlink
        }

        internal class VirtualScratchSink : ITextSinkEx, ITextSink
        {
            private HtmlToHtmlConverter converter;

            private int maxLength;

            public bool IsEnough
            {
                get
                {
                    return converter.attributeVirtualScratch.Length >= maxLength;
                }
            }

            public VirtualScratchSink(HtmlToHtmlConverter converter, int maxLength)
            {
                this.converter = converter;
                this.maxLength = maxLength;
            }

            public void Write(char[] buffer, int offset, int count)
            {
                converter.attributeVirtualScratch.Append(buffer, offset, count, maxLength);
            }

            public void Write(int ucs32Char)
            {
                if (Token.LiteralLength(ucs32Char) == 1)
                {
                    converter.attributeVirtualScratch.Append((char)ucs32Char, maxLength);
                    return;
                }
                converter.attributeVirtualScratch.Append(Token.LiteralFirstChar(ucs32Char), maxLength);
                if (!IsEnough)
                {
                    converter.attributeVirtualScratch.Append(Token.LiteralLastChar(ucs32Char), maxLength);
                }
            }

            public void Write(string value)
            {
                converter.attributeVirtualScratch.Append(value, maxLength);
            }

            public void WriteNewLine()
            {
                converter.attributeVirtualScratch.Append('\r', maxLength);
                if (!IsEnough)
                {
                    converter.attributeVirtualScratch.Append('\n', maxLength);
                }
            }
        }

        private bool convertFragment;

        private bool outputFragment;

        private bool filterForFragment;

        private bool filterHtml;

        private bool truncateForCallback;

        private int smallCssBlockThreshold;

        private bool preserveDisplayNoneStyle;

        private bool hasTailInjection;

        private IHtmlParser parser;

        private bool endOfFile;

        private bool normalizedInput;

        internal HtmlWriter writer;

        private HtmlTagCallback callback;

        private HtmlToHtmlTagContext callbackContext;

        internal HtmlToken token;

        private bool headDivUnterminated;

        private int currentLevel;

        private int currentLevelDelta;

        private bool insideCSS;

        private int dropLevel = 2147483647;

        private EndTagActionEntry[] endTagActionStack;

        private int endTagActionStackTop;

        private bool tagDropped;

        private bool justTruncated;

        private bool tagCallbackRequested;

        private bool attributeTriggeredCallback;

        private bool endTagCallbackRequested;

        private bool ignoreAttrCallback;

        private bool styleIsCSS;

        private HtmlFilterData.FilterAction attrContinuationAction;

        private CopyPendingState copyPendingState;

        private HtmlTagIndex tagIndex;

        private int attributeCount;

        private int attributeSkipCount;

        private bool attributeIndirect;

        private AttributeIndirectEntry[] attributeIndirectIndex;

        private AttributeVirtualEntry[] attributeVirtualList;

        private int attributeVirtualCount;

        private ScratchBuffer attributeVirtualScratch;

        private ScratchBuffer attributeActionScratch;

        private bool attributeLeadingSpaces;

        private bool metaInjected;

        private bool insideHtml;

        private bool insideHead;

        //private bool insideBody;

        private bool tagHasFilteredStyleAttribute;

        private CssParser cssParser;

        private ConverterBufferInput cssParserInput;

        private VirtualScratchSink virtualScratchSink;

        private IProgressMonitor progressMonitor;

        private static readonly string NamePrefix = "x_";

        private static object lockObject = new object();

        private static bool textConvertersConfigured;

        private static Dictionary<string, string> safeUrlDictionary;

        private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);

        private CopyPendingState CopyPendingStateFlag
        {
            get
            {
                return copyPendingState;
            }
            set
            {
                writer.SetCopyPending(value != CopyPendingState.NotPending);
                copyPendingState = value;
            }
        }

        public HtmlToHtmlConverter(IHtmlParser parser, HtmlWriter writer, bool convertFragment, bool outputFragment, bool filterHtml, HtmlTagCallback callback, bool truncateForCallback, bool hasTailInjection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, int smallCssBlockThreshold, bool preserveDisplayNoneStyle, IProgressMonitor progressMonitor)
        {
            this.writer = writer;
            normalizedInput = (parser is HtmlNormalizingParser);
            this.progressMonitor = progressMonitor;
            this.convertFragment = convertFragment;
            this.outputFragment = outputFragment;
            filterForFragment = (outputFragment || convertFragment);
            this.filterHtml = (filterHtml || filterForFragment);
            this.callback = callback;
            this.parser = parser;
            if (!convertFragment)
            {
                this.parser.SetRestartConsumer(this);
            }
            this.truncateForCallback = truncateForCallback;
            this.hasTailInjection = hasTailInjection;
            this.smallCssBlockThreshold = smallCssBlockThreshold;
            this.preserveDisplayNoneStyle = preserveDisplayNoneStyle;
        }

        void IDisposable.Dispose()
        {
            if (parser != null && parser is IDisposable)
            {
                ((IDisposable)parser).Dispose();
            }
            if (!convertFragment && writer != null && writer != null)
            {
                ((IDisposable)writer).Dispose();
            }
            if (token != null && token is IDisposable)
            {
                ((IDisposable)token).Dispose();
            }
            parser = null;
            writer = null;
            token = null;
            GC.SuppressFinalize(this);
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

        bool IRestartable.CanRestart()
        {
            return writer != null && ((IRestartable)writer).CanRestart();
        }

        void IRestartable.Restart()
        {
            if (writer != null && !convertFragment)
            {
                ((IRestartable)writer).Restart();
            }
            endOfFile = false;
            token = null;
            styleIsCSS = true;
            insideCSS = false;
            headDivUnterminated = false;
            tagDropped = false;
            justTruncated = false;
            tagCallbackRequested = false;
            endTagCallbackRequested = false;
            ignoreAttrCallback = false;
            attrContinuationAction = HtmlFilterData.FilterAction.Unknown;
            currentLevel = 0;
            currentLevelDelta = 0;
            dropLevel = 2147483647;
            endTagActionStackTop = 0;
            copyPendingState = CopyPendingState.NotPending;
            metaInjected = false;
            insideHtml = false;
            insideHead = false;
            //insideBody = false;
        }

        void IRestartable.DisableRestart()
        {
            if (writer != null)
            {
                ((IRestartable)writer).DisableRestart();
            }
        }

        private void Process(HtmlTokenId tokenId)
        {
            token = parser.Token;
            if (!metaInjected && !InjectMetaTagIfNecessary())
            {
                return;
            }
            switch (tokenId)
            {
                case HtmlTokenId.EndOfFile:
                    ProcessEof();
                    break;
                case HtmlTokenId.Text:
                    ProcessText();
                    return;
                case HtmlTokenId.EncodingChange:
                    if (writer.HasEncoding && writer.CodePageSameAsInput)
                    {
                        int argument = token.Argument;
                        writer.Encoding = Charset.GetEncoding(argument);
                        return;
                    }
                    break;
                case HtmlTokenId.Tag:
                    if (!token.IsEndTag)
                    {
                        ProcessStartTag();
                        return;
                    }
                    ProcessEndTag();
                    return;
                case HtmlTokenId.Restart:
                    break;
                case HtmlTokenId.OverlappedClose:
                    ProcessOverlappedClose();
                    return;
                case HtmlTokenId.OverlappedReopen:
                    ProcessOverlappedReopen();
                    return;
                case HtmlTokenId.InjectionBegin:
                    ProcessInjectionBegin();
                    return;
                case HtmlTokenId.InjectionEnd:
                    ProcessInjectionEnd();
                    return;
                default:
                    return;
            }
        }

        private void ProcessStartTag()
        {
            AvailableTagParts availableTagParts = AvailableTagParts.None;
            if (insideCSS && token.TagIndex == HtmlTagIndex._COMMENT && filterHtml)
            {
                AppendCssFromTokenText();
                return;
            }
            if (token.IsTagBegin)
            {
                currentLevel++;
                tagIndex = token.TagIndex;
                tagDropped = false;
                justTruncated = false;
                endTagCallbackRequested = false;
                PreProcessStartTag();
                if (currentLevel >= dropLevel)
                {
                    tagDropped = true;
                }
                else if (!tagDropped)
                {
                    tagCallbackRequested = false;
                    ignoreAttrCallback = false;
                    if (filterHtml || callback != null)
                    {
                        HtmlFilterData.FilterAction filterAction = filterForFragment ? HtmlFilterData.filterInstructions[(int)token.NameIndex].tagFragmentAction : HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction;
                        if (callback != null && (byte)(filterAction & HtmlFilterData.FilterAction.Callback) != 0)
                        {
                            tagCallbackRequested = true;
                        }
                        else if (filterHtml)
                        {
                            ignoreAttrCallback = (0 != (byte)(filterAction & HtmlFilterData.FilterAction.IgnoreAttrCallbacks));
                            switch ((byte)(filterAction & HtmlFilterData.FilterAction.ActionMask))
                            {
                                case 1:
                                    tagDropped = true;
                                    dropLevel = currentLevel;
                                    break;
                                case 2:
                                    tagDropped = true;
                                    break;
                                case 3:
                                    dropLevel = currentLevel + 1;
                                    break;
                            }
                        }
                    }
                    if (!tagDropped)
                    {
                        attributeTriggeredCallback = false;
                        tagHasFilteredStyleAttribute = false;
                        availableTagParts = AvailableTagParts.TagBegin;
                    }
                }
            }
            if (!tagDropped)
            {
                HtmlToken.TagPartMinor tagPartMinor = token.MinorPart;
                if (token.IsTagEnd)
                {
                    availableTagParts |= AvailableTagParts.TagEnd;
                }
                if (tagIndex < HtmlTagIndex.Unknown)
                {
                    availableTagParts |= AvailableTagParts.UnstructuredContent;
                    attributeCount = 0;
                }
                else
                {
                    if (token.HasNameFragment || token.IsTagNameEnd)
                    {
                        availableTagParts |= AvailableTagParts.TagName;
                    }
                    ProcessTagAttributes();
                    if (attributeCount != 0)
                    {
                        availableTagParts |= AvailableTagParts.Attributes;
                    }
                }
                if (availableTagParts != AvailableTagParts.None)
                {
                    if (CopyPendingStateFlag != CopyPendingState.NotPending)
                    {
                        switch (CopyPendingStateFlag)
                        {
                            case CopyPendingState.TagCopyPending:
                                CopyInputTag(true);
                                if (tagCallbackRequested && (byte)(availableTagParts & AvailableTagParts.TagEnd) != 0)
                                {
                                    attributeCount = 0;
                                    token.Name.MakeEmpty();
                                    availableTagParts &= ~AvailableTagParts.TagEnd;
                                }
                                else
                                {
                                    availableTagParts = AvailableTagParts.None;
                                }
                                break;
                            case CopyPendingState.TagNameCopyPending:
                                token.Name.WriteTo(writer.WriteTagName());
                                if (token.IsTagNameEnd)
                                {
                                    CopyPendingStateFlag = CopyPendingState.NotPending;
                                }
                                token.Name.MakeEmpty();
                                availableTagParts &= ~AvailableTagParts.TagName;
                                tagPartMinor &= (HtmlToken.TagPartMinor)248;
                                break;
                            case CopyPendingState.AttributeCopyPending:
                                CopyInputAttribute(0);
                                attributeSkipCount = 1;
                                attributeCount--;
                                if (attributeCount == 0)
                                {
                                    availableTagParts &= ~AvailableTagParts.Attributes;
                                }
                                tagPartMinor &= (HtmlToken.TagPartMinor)199;
                                break;
                            case CopyPendingState.AttributeNameCopyPending:
                                CopyInputAttributeName(0);
                                if (1 == attributeCount && (byte)(token.Attributes[0].MinorPart & HtmlToken.AttrPartMinor.ContinueValue) == 0)
                                {
                                    attributeSkipCount = 1;
                                    attributeCount--;
                                    availableTagParts &= ~AvailableTagParts.Attributes;
                                    tagPartMinor &= (HtmlToken.TagPartMinor)199;
                                }
                                else
                                {
                                    token.Attributes[0].Name.MakeEmpty();
                                    token.Attributes[0].SetMinorPart(token.Attributes[0].MinorPart & (HtmlToken.AttrPartMinor)248);
                                }
                                break;
                            case CopyPendingState.AttributeValueCopyPending:
                                CopyInputAttributeValue(0);
                                attributeSkipCount = 1;
                                attributeCount--;
                                if (attributeCount == 0)
                                {
                                    availableTagParts &= ~AvailableTagParts.Attributes;
                                }
                                tagPartMinor &= (HtmlToken.TagPartMinor)199;
                                break;
                        }
                    }
                    if (availableTagParts != AvailableTagParts.None)
                    {
                        if (tagCallbackRequested)
                        {
                            if (callbackContext == null)
                            {
                                callbackContext = new HtmlToHtmlTagContext(this);
                            }
                            if (token.IsTagBegin || attributeTriggeredCallback)
                            {
                                callbackContext.InitializeTag(false, HtmlDtd.tags[(int)tagIndex].nameIndex, false);
                                attributeTriggeredCallback = false;
                            }
                            callbackContext.InitializeFragment(token.IsEmptyScope, attributeCount, new HtmlTagParts(token.MajorPart, token.MinorPart));
                            callback(callbackContext, writer);
                            callbackContext.UninitializeFragment();
                            if (token.IsTagEnd || truncateForCallback)
                            {
                                if (callbackContext.IsInvokeCallbackForEndTag)
                                {
                                    endTagCallbackRequested = true;
                                }
                                if (callbackContext.IsDeleteInnerContent)
                                {
                                    dropLevel = currentLevel + 1;
                                }
                                if (token.IsTagBegin && callbackContext.IsDeleteEndTag)
                                {
                                    tagDropped = true;
                                }
                                if (!tagDropped && !token.IsTagEnd)
                                {
                                    tagDropped = true;
                                    justTruncated = true;
                                    CopyPendingStateFlag = CopyPendingState.NotPending;
                                }
                            }
                        }
                        else
                        {
                            if (token.IsTagBegin)
                            {
                                CopyInputTag(false);
                            }
                            if (attributeCount != 0)
                            {
                                CopyInputTagAttributes();
                            }
                            if (token.IsTagEnd && tagIndex == HtmlTagIndex.Unknown)
                            {
                                writer.WriteTagEnd(token.IsEmptyScope);
                            }
                        }
                    }
                }
            }
            if (token.IsTagEnd)
            {
                if (writer.IsTagOpen)
                {
                    writer.WriteTagEnd();
                }
                if (!token.IsEmptyScope && tagIndex > HtmlTagIndex.Unknown)
                {
                    if (normalizedInput && currentLevel < dropLevel && ((tagDropped && !justTruncated) || endTagCallbackRequested))
                    {
                        if (endTagActionStack == null)
                        {
                            endTagActionStack = new EndTagActionEntry[4];
                        }
                        else if (endTagActionStack.Length == endTagActionStackTop)
                        {
                            EndTagActionEntry[] destinationArray = new EndTagActionEntry[endTagActionStack.Length * 2];
                            Array.Copy(endTagActionStack, 0, destinationArray, 0, endTagActionStackTop);
                            endTagActionStack = destinationArray;
                        }
                        endTagActionStack[endTagActionStackTop].tagLevel = currentLevel;
                        endTagActionStack[endTagActionStackTop].drop = (tagDropped && !justTruncated);
                        endTagActionStack[endTagActionStackTop].callback = endTagCallbackRequested;
                        endTagActionStackTop++;
                    }
                    currentLevel++;
                    PostProcessStartTag();
                    return;
                }
                currentLevel--;
                if (dropLevel != 2147483647 && currentLevel < dropLevel)
                {
                    dropLevel = 2147483647;
                }
            }
        }

        private void ProcessEndTag()
        {
            AvailableTagParts availableTagParts = AvailableTagParts.None;
            if (token.IsTagBegin)
            {
                if (currentLevel > 0)
                {
                    currentLevel--;
                }
                tagIndex = token.TagIndex;
                tagDropped = false;
                tagCallbackRequested = false;
                tagHasFilteredStyleAttribute = false;
                availableTagParts = AvailableTagParts.TagBegin;
                PreProcessEndTag();
                if (currentLevel >= dropLevel)
                {
                    tagDropped = true;
                }
                else
                {
                    if (endTagActionStackTop != 0 && tagIndex > HtmlTagIndex.Unknown && endTagActionStack[endTagActionStackTop - 1].tagLevel >= currentLevel)
                    {
                        if (endTagActionStack[endTagActionStackTop - 1].tagLevel == currentLevel)
                        {
                            endTagActionStackTop--;
                            tagDropped = endTagActionStack[endTagActionStackTop].drop;
                            tagCallbackRequested = endTagActionStack[endTagActionStackTop].callback;
                        }
                        else
                        {
                            int i = endTagActionStackTop;
                            while (i > 0 && endTagActionStack[i - 1].tagLevel > currentLevel)
                            {
                                i--;
                            }
                            for (int j = i; j < endTagActionStackTop; j++)
                            {
                                EndTagActionEntry[] expr_152_cp_0 = endTagActionStack;
                                int expr_152_cp_1 = j;
                                expr_152_cp_0[expr_152_cp_1].tagLevel = expr_152_cp_0[expr_152_cp_1].tagLevel - 2;
                            }
                            if (i > 0 && endTagActionStack[i - 1].tagLevel == currentLevel)
                            {
                                tagDropped = endTagActionStack[i - 1].drop;
                                tagCallbackRequested = endTagActionStack[i - 1].callback;
                                while (i < endTagActionStackTop)
                                {
                                    endTagActionStack[i - 1] = endTagActionStack[i];
                                    i++;
                                }
                                endTagActionStackTop--;
                            }
                        }
                    }
                    if (token.Argument == 1 && tagIndex == HtmlTagIndex.Unknown)
                    {
                        tagDropped = true;
                    }
                }
            }
            if (!tagDropped)
            {
                HtmlToken.TagPartMinor tagPartMinor = token.MinorPart & (HtmlToken.TagPartMinor)71;
                if (token.IsTagEnd)
                {
                    availableTagParts |= AvailableTagParts.TagEnd;
                }
                if (token.HasNameFragment)
                {
                    availableTagParts |= AvailableTagParts.TagName;
                }
                if (CopyPendingStateFlag == CopyPendingState.TagNameCopyPending)
                {
                    token.Name.WriteTo(writer.WriteTagName());
                    if (token.IsTagNameEnd)
                    {
                        CopyPendingStateFlag = CopyPendingState.NotPending;
                    }
                    token.Name.MakeEmpty();
                    availableTagParts &= ~AvailableTagParts.TagName;
                    tagPartMinor &= (HtmlToken.TagPartMinor)248;
                }
                if (availableTagParts != AvailableTagParts.None)
                {
                    if (tagCallbackRequested)
                    {
                        if (token.IsTagBegin)
                        {
                            callbackContext.InitializeTag(true, HtmlDtd.tags[(int)tagIndex].nameIndex, false);
                        }
                        callbackContext.InitializeFragment(false, 0, new HtmlTagParts(token.MajorPart, tagPartMinor));
                        callback(callbackContext, writer);
                        callbackContext.UninitializeFragment();
                    }
                    else if (token.IsTagBegin)
                    {
                        CopyInputTag(false);
                    }
                }
            }
            else if (tagCallbackRequested)
            {
                HtmlToken.TagPartMinor tagPartMinor = token.MinorPart & (HtmlToken.TagPartMinor)71;
                if (token.IsTagBegin)
                {
                    callbackContext.InitializeTag(true, HtmlDtd.tags[(int)tagIndex].nameIndex, true);
                }
                callbackContext.InitializeFragment(false, 0, new HtmlTagParts(token.MajorPart, tagPartMinor));
                callback(callbackContext, writer);
                callbackContext.UninitializeFragment();
            }
            if (token.IsTagEnd)
            {
                if (writer.IsTagOpen)
                {
                    writer.WriteTagEnd();
                }
                if (tagIndex > HtmlTagIndex.Unknown)
                {
                    if (currentLevel > 0)
                    {
                        currentLevel--;
                    }
                    if (dropLevel != 2147483647 && currentLevel < dropLevel)
                    {
                        dropLevel = 2147483647;
                        return;
                    }
                }
                else if (currentLevel > 0)
                {
                    currentLevel++;
                }
            }
        }

        private void ProcessOverlappedClose()
        {
            currentLevelDelta = token.Argument * 2;
            currentLevel -= currentLevelDelta;
        }

        private void ProcessOverlappedReopen()
        {
            currentLevel += token.Argument * 2;
            currentLevelDelta = 0;
        }

        private void ProcessText()
        {
            if (currentLevel >= dropLevel)
            {
                return;
            }
            if (insideCSS && filterHtml)
            {
                AppendCssFromTokenText();
                return;
            }
            if (token.Argument == 1)
            {
                writer.WriteCollapsedWhitespace();
                return;
            }
            if (token.Runs.MoveNext(true))
            {
                token.Text.WriteTo(writer.WriteText());
            }
        }

        private void ProcessInjectionBegin()
        {
            if (token.Argument == 0 && headDivUnterminated)
            {
                writer.WriteEndTag(HtmlNameIndex.Div);
                writer.WriteAutoNewLine(true);
                headDivUnterminated = false;
            }
        }

        private void ProcessInjectionEnd()
        {
            if (token.Argument != 0)
            {
                writer.WriteAutoNewLine(true);
                writer.WriteStartTag(HtmlNameIndex.Div);
                headDivUnterminated = true;
            }
        }

        private void ProcessEof()
        {
            writer.SetCopyPending(false);
            if (headDivUnterminated && dropLevel != 0)
            {
                writer.WriteEndTag(HtmlNameIndex.Div);
                writer.WriteAutoNewLine(true);
                headDivUnterminated = false;
            }
            if (!convertFragment)
            {
                writer.Flush();
            }
            endOfFile = true;
        }

        private void PreProcessStartTag()
        {
            if (tagIndex > HtmlTagIndex.Unknown)
            {
                if (tagIndex == HtmlTagIndex.Body)
                {
                    if (outputFragment)
                    {
                        //insideBody = true;
                        tagIndex = HtmlTagIndex.Div;
                        return;
                    }
                }
                else if (tagIndex == HtmlTagIndex.Meta)
                {
                    if (!filterHtml)
                    {
                        token.Attributes.Rewind();
                        HtmlToken.AttributeEnumerator enumerator = token.Attributes.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            HtmlAttribute current = enumerator.Current;
                            if (current.NameIndex == HtmlNameIndex.HttpEquiv)
                            {
                                if (current.Value.CaseInsensitiveCompareEqual("content-type") || current.Value.CaseInsensitiveCompareEqual("charset"))
                                {
                                    tagDropped = true;
                                    return;
                                }
                            }
                            else if (current.NameIndex == HtmlNameIndex.Charset)
                            {
                                tagDropped = true;
                                return;
                            }
                        }
                        return;
                    }
                }
                else if (tagIndex == HtmlTagIndex.Style)
                {
                    styleIsCSS = true;
                    if (token.Attributes.Find(HtmlNameIndex.Type))
                    {
                        HtmlAttribute current2 = token.Attributes.Current;
                        if (!current2.Value.CaseInsensitiveCompareEqual("text/css"))
                        {
                            styleIsCSS = false;
                            return;
                        }
                    }
                }
                else
                {
                    if (tagIndex == HtmlTagIndex.TC)
                    {
                        tagDropped = true;
                        return;
                    }
                    if (tagIndex == HtmlTagIndex.PlainText || tagIndex == HtmlTagIndex.Xmp)
                    {
                        if (filterHtml || (hasTailInjection && tagIndex == HtmlTagIndex.PlainText))
                        {
                            tagDropped = true;
                            writer.WriteAutoNewLine(true);
                            writer.WriteStartTag(HtmlNameIndex.TT);
                            writer.WriteStartTag(HtmlNameIndex.Pre);
                            writer.WriteAutoNewLine();
                            return;
                        }
                    }
                    else if (tagIndex == HtmlTagIndex.Image && filterHtml)
                    {
                        tagIndex = HtmlTagIndex.Img;
                    }
                }
            }
        }

        private void ProcessTagAttributes()
        {
            attributeSkipCount = 0;
            HtmlToken.AttributeEnumerator attributes = token.Attributes;
            if (filterHtml)
            {
                attributeCount = 0;
                attributeIndirect = true;
                attributeVirtualCount = 0;
                attributeVirtualScratch.Reset();
                if (attributeIndirectIndex == null)
                {
                    attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(attributes.Count + 1, 32)];
                }
                else if (attributeIndirectIndex.Length <= attributes.Count)
                {
                    attributeIndirectIndex = new AttributeIndirectEntry[Math.Max(attributeIndirectIndex.Length * 2, attributes.Count + 1)];
                }
                for (int i = 0; i < attributes.Count; i++)
                {
                    HtmlAttribute htmlAttribute = attributes[i];
                    HtmlFilterData.FilterAction filterAction;
                    if (htmlAttribute.IsAttrBegin)
                    {
                        filterAction = (filterForFragment ? HtmlFilterData.filterInstructions[(int)htmlAttribute.NameIndex].attrFragmentAction : HtmlFilterData.filterInstructions[(int)htmlAttribute.NameIndex].attrAction);
                        if ((byte)(filterAction & HtmlFilterData.FilterAction.HasExceptions) != 0 && (byte)(HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions) != 0)
                        {
                            for (int j = 0; j < HtmlFilterData.filterExceptions.Length; j++)
                            {
                                if (HtmlFilterData.filterExceptions[j].tagNameIndex == token.NameIndex && HtmlFilterData.filterExceptions[j].attrNameIndex == htmlAttribute.NameIndex)
                                {
                                    filterAction = (filterForFragment ? HtmlFilterData.filterExceptions[j].fragmentAction : HtmlFilterData.filterExceptions[j].action);
                                    break;
                                }
                            }
                        }
                        if (!outputFragment && (filterAction == HtmlFilterData.FilterAction.PrefixName || filterAction == HtmlFilterData.FilterAction.PrefixNameList))
                        {
                            filterAction = HtmlFilterData.FilterAction.Keep;
                        }
                        if (callback != null && !ignoreAttrCallback && (byte)(filterAction & HtmlFilterData.FilterAction.Callback) != 0)
                        {
                            if (token.IsTagBegin || !truncateForCallback)
                            {
                                attributeTriggeredCallback = (attributeTriggeredCallback || !tagCallbackRequested);
                                tagCallbackRequested = true;
                            }
                            else
                            {
                                filterAction = HtmlFilterData.FilterAction.KeepDropContent;
                            }
                        }
                        filterAction &= HtmlFilterData.FilterAction.ActionMask;
                        if (!htmlAttribute.IsAttrEnd)
                        {
                            attrContinuationAction = filterAction;
                        }
                    }
                    else
                    {
                        filterAction = attrContinuationAction;
                    }
                    if (filterAction != HtmlFilterData.FilterAction.Drop)
                    {
                        if (filterAction == HtmlFilterData.FilterAction.Keep)
                        {
                            attributeIndirectIndex[attributeCount].index = (short)i;
                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.PassThrough;
                            attributeCount++;
                        }
                        else if (filterAction == HtmlFilterData.FilterAction.KeepDropContent)
                        {
                            attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                            attributeIndirectIndex[attributeCount].index = (short)i;
                            attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                            attributeCount++;
                        }
                        else if (filterAction == HtmlFilterData.FilterAction.FilterStyleAttribute)
                        {
                            if (htmlAttribute.IsAttrBegin)
                            {
                                if (tagHasFilteredStyleAttribute)
                                {
                                    AppendCss(";");
                                }
                                tagHasFilteredStyleAttribute = true;
                            }
                            AppendCssFromAttribute(htmlAttribute);
                        }
                        else if (filterAction == HtmlFilterData.FilterAction.ConvertBgcolorIntoStyle)
                        {
                            if (htmlAttribute.IsAttrBegin)
                            {
                                if (tagHasFilteredStyleAttribute)
                                {
                                    AppendCss(";");
                                }
                                tagHasFilteredStyleAttribute = true;
                            }
                            AppendCss("background-color:");
                            AppendCssFromAttribute(htmlAttribute);
                        }
                        else
                        {
                            if (htmlAttribute.IsAttrBegin)
                            {
                                attributeLeadingSpaces = true;
                            }
                            if (attributeLeadingSpaces)
                            {
                                if (!htmlAttribute.Value.SkipLeadingWhitespace() && !htmlAttribute.IsAttrEnd)
                                {
                                    if (htmlAttribute.IsAttrBegin || htmlAttribute.HasNameFragment)
                                    {
                                        attributeIndirectIndex[attributeCount].index = (short)i;
                                        attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                        attributeCount++;
                                        goto IL_8F4;
                                    }
                                    goto IL_8F4;
                                }
                                else
                                {
                                    attributeLeadingSpaces = false;
                                    attributeActionScratch.Reset();
                                }
                            }
                            bool flag = false;
                            if (!attributeActionScratch.AppendHtmlAttributeValue(htmlAttribute, 4096))
                            {
                                flag = true;
                            }
                            if (!htmlAttribute.IsAttrEnd && !flag)
                            {
                                if (htmlAttribute.IsAttrBegin || htmlAttribute.HasNameFragment)
                                {
                                    attributeIndirectIndex[attributeCount].index = (short)i;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.NameOnlyFragment;
                                    attributeCount++;
                                }
                            }
                            else
                            {
                                attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                                if (filterAction == HtmlFilterData.FilterAction.SanitizeUrl)
                                {
                                    int length;
                                    int num2;
                                    int num3;
                                    switch (CheckUrl(attributeActionScratch.Buffer, attributeActionScratch.Length, tagCallbackRequested))
                                    {
                                        case CheckUrlResult.Inconclusive:
                                            if (attributeActionScratch.Length > 256 || !htmlAttribute.IsAttrEnd)
                                            {
                                                goto IL_663;
                                            }
                                            break;
                                        case CheckUrlResult.Unsafe:
                                            goto IL_663;
                                        case CheckUrlResult.Safe:
                                            break;
                                        case CheckUrlResult.LocalHyperlink:
                                            if (outputFragment)
                                            {
                                                int num = NonWhitespaceLength(attributeActionScratch.Buffer, 1, attributeActionScratch.Length - 1);
                                                if (num != 0)
                                                {
                                                    length = attributeVirtualScratch.Length;
                                                    num2 = 0;
                                                    num2 += attributeVirtualScratch.Append('#', 2147483647);
                                                    num2 += attributeVirtualScratch.Append(NamePrefix, 2147483647);
                                                    num2 += attributeVirtualScratch.Append(attributeActionScratch.Buffer, 1, num, 2147483647);
                                                    num3 = AllocateVirtualEntry(i, length, num2);
                                                    attributeIndirectIndex[attributeCount].index = (short)num3;
                                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                                    attributeCount++;
                                                    goto IL_8F4;
                                                }
                                                goto IL_663;
                                            }
                                            break;
                                        default:
                                            goto IL_663;
                                    }
                                    if (htmlAttribute.IsCompleteAttr)
                                    {
                                        attributeIndirectIndex[attributeCount].index = (short)i;
                                        attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.PassThrough;
                                        attributeCount++;
                                        goto IL_8F4;
                                    }
                                    length = attributeVirtualScratch.Length;
                                    num2 = attributeVirtualScratch.Append(attributeActionScratch.Buffer, 0, attributeActionScratch.Length, 2147483647);
                                    num3 = AllocateVirtualEntry(i, length, num2);
                                    attributeIndirectIndex[attributeCount].index = (short)num3;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                    attributeCount++;
                                    goto IL_8F4;
                                    IL_663:
                                    attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                                    attributeIndirectIndex[attributeCount].index = (short)i;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                    attributeCount++;
                                }
                                else if (filterAction == HtmlFilterData.FilterAction.PrefixName)
                                {
                                    int length = attributeVirtualScratch.Length;
                                    int num2 = 0;
                                    int num4 = NonWhitespaceLength(attributeActionScratch.Buffer, 0, attributeActionScratch.Length);
                                    if (num4 != 0)
                                    {
                                        num2 += attributeVirtualScratch.Append(NamePrefix, 2147483647);
                                        num2 += attributeVirtualScratch.Append(attributeActionScratch.Buffer, 0, num4, 2147483647);
                                    }
                                    int num3 = AllocateVirtualEntry(i, length, num2);
                                    attributeIndirectIndex[attributeCount].index = (short)num3;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                    attributeCount++;
                                }
                                else if (filterAction == HtmlFilterData.FilterAction.PrefixNameList)
                                {
                                    int length = attributeVirtualScratch.Length;
                                    int num2 = 0;
                                    int num5 = 0;
                                    int num6 = NonWhitespaceLength(attributeActionScratch.Buffer, num5, attributeActionScratch.Length - num5);
                                    if (num6 != 0)
                                    {
                                        do
                                        {
                                            num2 += attributeVirtualScratch.Append(NamePrefix, 2147483647);
                                            num2 += attributeVirtualScratch.Append(attributeActionScratch.Buffer, num5, num6, 2147483647);
                                            num5 += num6;
                                            num5 += WhitespaceLength(attributeActionScratch.Buffer, num5, attributeActionScratch.Length - num5);
                                            num6 = NonWhitespaceLength(attributeActionScratch.Buffer, num5, attributeActionScratch.Length - num5);
                                            if (num6 != 0)
                                            {
                                                num2 += attributeVirtualScratch.Append(' ', 2147483647);
                                            }
                                        }
                                        while (num6 != 0);
                                    }
                                    int num3 = AllocateVirtualEntry(i, length, num2);
                                    attributeIndirectIndex[attributeCount].index = (short)num3;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.Virtual;
                                    attributeCount++;
                                }
                                else
                                {
                                    attrContinuationAction = HtmlFilterData.FilterAction.Drop;
                                    attributeIndirectIndex[attributeCount].index = (short)i;
                                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.EmptyValue;
                                    attributeCount++;
                                }
                            }
                        }
                    }
                    IL_8F4:;
                }
                if (tagHasFilteredStyleAttribute && (token.IsTagEnd || (tagCallbackRequested && truncateForCallback)))
                {
                    attributeIndirectIndex[attributeCount].index = -1;
                    attributeIndirectIndex[attributeCount].kind = AttributeIndirectKind.FilteredStyle;
                    attributeCount++;
                    return;
                }
            }
            else
            {
                attributeCount = attributes.Count;
                attributeIndirect = false;
                if (callback != null && !tagCallbackRequested && !ignoreAttrCallback)
                {
                    for (int k = 0; k < attributes.Count; k++)
                    {
                        HtmlAttribute htmlAttribute = attributes[k];
                        if (htmlAttribute.IsAttrBegin)
                        {
                            HtmlFilterData.FilterAction filterAction = HtmlFilterData.filterInstructions[(int)htmlAttribute.NameIndex].attrAction;
                            if ((byte)(filterAction & HtmlFilterData.FilterAction.HasExceptions) != 0 && (byte)(HtmlFilterData.filterInstructions[(int)token.NameIndex].tagAction & HtmlFilterData.FilterAction.HasExceptions) != 0)
                            {
                                for (int l = 0; l < HtmlFilterData.filterExceptions.Length; l++)
                                {
                                    if (HtmlFilterData.filterExceptions[l].tagNameIndex == token.NameIndex && HtmlFilterData.filterExceptions[l].attrNameIndex == htmlAttribute.NameIndex)
                                    {
                                        filterAction = HtmlFilterData.filterExceptions[l].action;
                                        break;
                                    }
                                }
                            }
                            if ((byte)(filterAction & HtmlFilterData.FilterAction.Callback) != 0 && (token.IsTagBegin || !truncateForCallback))
                            {
                                attributeTriggeredCallback = (attributeTriggeredCallback || !tagCallbackRequested);
                                tagCallbackRequested = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static int WhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            int num = 0;
            while (remainingLength != 0 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
            {
                num++;
                remainingLength--;
            }
            return num;
        }

        private static int NonWhitespaceLength(char[] buffer, int offset, int remainingLength)
        {
            int num = 0;
            while (remainingLength != 0 && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(buffer[offset++])))
            {
                num++;
                remainingLength--;
            }
            return num;
        }

        private void PostProcessStartTag()
        {
            if (tagIndex == HtmlTagIndex.Style && styleIsCSS)
            {
                insideCSS = true;
            }
        }

        private void PreProcessEndTag()
        {
            if (tagIndex > HtmlTagIndex.Unknown)
            {
                if ((byte)(HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Entities) != 0)
                {
                    if (tagIndex == HtmlTagIndex.Style && insideCSS && filterHtml)
                    {
                        FlushCssInStyleTag();
                    }
                    insideCSS = false;
                    styleIsCSS = true;
                }
                if (tagIndex == HtmlTagIndex.PlainText || tagIndex == HtmlTagIndex.Xmp)
                {
                    if (filterHtml || (hasTailInjection && tagIndex == HtmlTagIndex.PlainText))
                    {
                        tagDropped = true;
                        writer.WriteEndTag(HtmlNameIndex.Pre);
                        writer.WriteEndTag(HtmlNameIndex.TT);
                        return;
                    }
                    if (tagIndex == HtmlTagIndex.PlainText && normalizedInput)
                    {
                        tagDropped = true;
                        dropLevel = 0;
                        endTagActionStackTop = 0;
                        return;
                    }
                }
                else if (tagIndex == HtmlTagIndex.Body)
                {
                    if (headDivUnterminated && dropLevel != 0)
                    {
                        writer.WriteEndTag(HtmlNameIndex.Div);
                        writer.WriteAutoNewLine(true);
                        headDivUnterminated = false;
                    }
                    if (outputFragment)
                    {
                        tagIndex = HtmlTagIndex.Div;
                        return;
                    }
                }
                else
                {
                    if (tagIndex == HtmlTagIndex.TC)
                    {
                        tagDropped = true;
                        return;
                    }
                    if (tagIndex == HtmlTagIndex.Image && filterHtml)
                    {
                        tagIndex = HtmlTagIndex.Img;
                        return;
                    }
                }
            }
            else if (tagIndex == HtmlTagIndex.Unknown && filterHtml)
            {
                tagDropped = true;
            }
        }

        internal void CopyInputTag(bool copyTagAttributes)
        {
            if (token.IsTagBegin)
            {
                writer.WriteTagBegin(HtmlDtd.tags[(int)tagIndex].nameIndex, null, token.IsEndTag, token.IsAllowWspLeft, token.IsAllowWspRight);
            }
            if (tagIndex <= HtmlTagIndex.Unknown)
            {
                if (tagIndex < HtmlTagIndex.Unknown)
                {
                    token.UnstructuredContent.WriteTo(writer.WriteUnstructuredTagContent());
                    if (token.IsTagEnd)
                    {
                        CopyPendingStateFlag = CopyPendingState.NotPending;
                        return;
                    }
                    CopyPendingStateFlag = CopyPendingState.TagCopyPending;
                    return;
                }
                else if (token.HasNameFragment)
                {
                    token.Name.WriteTo(writer.WriteTagName());
                    if (!token.IsTagNameEnd && !copyTagAttributes)
                    {
                        CopyPendingStateFlag = CopyPendingState.TagNameCopyPending;
                        return;
                    }
                }
            }
            if (!copyTagAttributes)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }
            if (attributeCount != 0)
            {
                CopyInputTagAttributes();
            }
            if (token.IsTagEnd)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }
            CopyPendingStateFlag = CopyPendingState.TagCopyPending;
        }

        private void CopyInputTagAttributes()
        {
            for (int i = 0; i < attributeCount; i++)
            {
                CopyInputAttribute(i);
            }
        }

        internal void CopyInputAttribute(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle)
            {
                if (!tagCallbackRequested)
                {
                    writer.WriteAttributeName(HtmlNameIndex.Style);
                    if (!cssParserInput.IsEmpty)
                    {
                        FlushCssInStyleAttribute(writer);
                        return;
                    }
                    writer.WriteAttributeValueInternal(string.Empty);
                    return;
                }
                else
                {
                    VirtualizeFilteredStyle(index);
                    attributeIndirectKind = AttributeIndirectKind.VirtualFilteredStyle;
                }
            }
            bool flag = true;
            if (attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                writer.WriteAttributeName(HtmlNameIndex.Style);
                int attributeVirtualEntryIndex = GetAttributeVirtualEntryIndex(index);
                if (attributeVirtualList[attributeVirtualEntryIndex].length != 0)
                {
                    writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[attributeVirtualEntryIndex].offset, attributeVirtualList[attributeVirtualEntryIndex].length);
                }
                else
                {
                    writer.WriteAttributeValueInternal(string.Empty);
                }
            }
            else
            {
                HtmlAttribute attribute = GetAttribute(index);
                if (attribute.IsAttrBegin && attribute.NameIndex != HtmlNameIndex.Unknown)
                {
                    writer.WriteAttributeName(attribute.NameIndex);
                }
                if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
                {
                    attribute.Name.WriteTo(writer.WriteAttributeName());
                }
                if (attributeIndirectKind == AttributeIndirectKind.NameOnlyFragment)
                {
                    flag = false;
                }
                else if (attributeIndirectKind == AttributeIndirectKind.EmptyValue)
                {
                    writer.WriteAttributeValueInternal(string.Empty);
                }
                else if (attributeIndirectKind == AttributeIndirectKind.Virtual)
                {
                    int attributeVirtualEntryIndex2 = GetAttributeVirtualEntryIndex(index);
                    if (attributeVirtualList[attributeVirtualEntryIndex2].length != 0)
                    {
                        writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[attributeVirtualEntryIndex2].offset, attributeVirtualList[attributeVirtualEntryIndex2].length);
                    }
                    else
                    {
                        writer.WriteAttributeValueInternal(string.Empty);
                    }
                }
                else
                {
                    if (attribute.HasValueFragment)
                    {
                        attribute.Value.WriteTo(writer.WriteAttributeValue());
                    }
                    flag = attribute.IsAttrEnd;
                }
            }
            if (flag)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }
            CopyPendingStateFlag = CopyPendingState.AttributeCopyPending;
        }

        internal void CopyInputAttributeName(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle || attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                writer.WriteAttributeName(HtmlNameIndex.Style);
                return;
            }
            HtmlAttribute attribute = GetAttribute(index);
            if (attribute.IsAttrBegin && attribute.NameIndex != HtmlNameIndex.Unknown)
            {
                writer.WriteAttributeName(attribute.NameIndex);
            }
            if (attribute.NameIndex == HtmlNameIndex.Unknown && (attribute.HasNameFragment || attribute.IsAttrBegin))
            {
                attribute.Name.WriteTo(writer.WriteAttributeName());
            }
            if (attribute.IsAttrNameEnd)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }
            CopyPendingStateFlag = CopyPendingState.AttributeNameCopyPending;
        }

        internal void CopyInputAttributeValue(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            bool flag = true;
            if (attributeIndirectKind != AttributeIndirectKind.PassThrough)
            {
                if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle)
                {
                    if (!tagCallbackRequested)
                    {
                        if (!cssParserInput.IsEmpty)
                        {
                            FlushCssInStyleAttribute(writer);
                            return;
                        }
                        writer.WriteAttributeValueInternal(string.Empty);
                        return;
                    }
                    else
                    {
                        VirtualizeFilteredStyle(index);
                        attributeIndirectKind = AttributeIndirectKind.VirtualFilteredStyle;
                    }
                }
                if (attributeIndirectKind == AttributeIndirectKind.Virtual || attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
                {
                    int attributeVirtualEntryIndex = GetAttributeVirtualEntryIndex(index);
                    if (attributeVirtualList[attributeVirtualEntryIndex].length != 0)
                    {
                        writer.WriteAttributeValueInternal(attributeVirtualScratch.Buffer, attributeVirtualList[attributeVirtualEntryIndex].offset, attributeVirtualList[attributeVirtualEntryIndex].length);
                    }
                    else
                    {
                        writer.WriteAttributeValueInternal(string.Empty);
                    }
                }
                else if (attributeIndirectKind == AttributeIndirectKind.NameOnlyFragment)
                {
                    flag = false;
                }
                else if (attributeIndirectKind == AttributeIndirectKind.EmptyValue)
                {
                    writer.WriteAttributeValueInternal(string.Empty);
                }
            }
            else
            {
                HtmlAttribute attribute = GetAttribute(index);
                if (attribute.HasValueFragment)
                {
                    attribute.Value.WriteTo(writer.WriteAttributeValue());
                }
                flag = attribute.IsAttrEnd;
            }
            if (flag)
            {
                CopyPendingStateFlag = CopyPendingState.NotPending;
                return;
            }
            CopyPendingStateFlag = CopyPendingState.AttributeValueCopyPending;
        }

        private static void ConfigureTextConverters()
        {
            lock (lockObject)
            {
                if (!textConvertersConfigured)
                {
                    safeUrlDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    bool flag = false;
                    bool flag2 = true;
                    CtsConfigurationSetting simpleConfigurationSetting = ApplicationServices.GetSimpleConfigurationSetting("TextConverters", "SafeUrlScheme");
                    if (simpleConfigurationSetting != null)
                    {
                        if (simpleConfigurationSetting.Arguments.Count != 1 || (!simpleConfigurationSetting.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase) && !simpleConfigurationSetting.Arguments[0].Name.Equals("Override", StringComparison.OrdinalIgnoreCase)))
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                        }
                        else
                        {
                            flag2 = simpleConfigurationSetting.Arguments[0].Name.Equals("Add", StringComparison.OrdinalIgnoreCase);
                            string[] array = simpleConfigurationSetting.Arguments[0].Value.Split(new char[]
                            {
                                ',',
                                ' ',
                                ';',
                                ':'
                            }, StringSplitOptions.RemoveEmptyEntries);
                            string text = "";
                            string[] array2 = array;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                string text2 = array2[i];
                                string text3 = text2.Trim().ToLower();
                                bool flag3 = false;
                                string text4 = text3;
                                for (int j = 0; j < text4.Length; j++)
                                {
                                    char c = text4[j];
                                    if (c > '\u007f' || (!char.IsLetterOrDigit(c) && c != '_' && c != '-' && c != '+'))
                                    {
                                        if (text.Length != 0)
                                        {
                                            text += " ";
                                        }
                                        text += text2;
                                        flag3 = true;
                                        break;
                                    }
                                }
                                if (!flag3 && !safeUrlDictionary.ContainsKey(text3))
                                {
                                    safeUrlDictionary.Add(text3, null);
                                }
                            }
                            if (text.Length != 0)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                            }
                            flag = true;
                        }
                    }
                    if (!flag || flag2)
                    {
                        safeUrlDictionary["http"] = null;
                        safeUrlDictionary["https"] = null;
                        safeUrlDictionary["ftp"] = null;
                        safeUrlDictionary["file"] = null;
                        safeUrlDictionary["mailto"] = null;
                        safeUrlDictionary["news"] = null;
                        safeUrlDictionary["gopher"] = null;
                        safeUrlDictionary["about"] = null;
                        safeUrlDictionary["wais"] = null;
                        safeUrlDictionary["cid"] = null;
                        safeUrlDictionary["mhtml"] = null;
                        safeUrlDictionary["ipp"] = null;
                        safeUrlDictionary["msdaipp"] = null;
                        safeUrlDictionary["meet"] = null;
                        safeUrlDictionary["tel"] = null;
                        safeUrlDictionary["sip"] = null;
                        safeUrlDictionary["conf"] = null;
                        safeUrlDictionary["im"] = null;
                        safeUrlDictionary["callto"] = null;
                        safeUrlDictionary["notes"] = null;
                        safeUrlDictionary["onenote"] = null;
                        safeUrlDictionary["groove"] = null;
                        safeUrlDictionary["mms"] = null;
                    }
                    textConvertersConfigured = true;
                }
            }
        }

        private static bool SafeUrlSchema(char[] urlBuffer, int schemaLength)
        {
            if (schemaLength < 2 || schemaLength > 20)
            {
                return false;
            }
            if (!textConvertersConfigured)
            {
                ConfigureTextConverters();
            }
            return safeUrlDictionary.ContainsKey(new string(urlBuffer, 0, schemaLength));
        }

        private static CheckUrlResult CheckUrl(char[] urlBuffer, int urlLength, bool callbackRequested)
        {
            if (urlLength > 0 && urlBuffer[0] == '#')
            {
                return CheckUrlResult.LocalHyperlink;
            }
            int i = 0;
            while (i < urlLength)
            {
                if (urlBuffer[i] == '/' || urlBuffer[i] == '\\')
                {
                    if (i != 0 || urlLength <= 1 || (urlBuffer[1] != '/' && urlBuffer[1] != '\\'))
                    {
                        return CheckUrlResult.Safe;
                    }
                    if (!callbackRequested)
                    {
                        return CheckUrlResult.Unsafe;
                    }
                    return CheckUrlResult.Safe;
                }
                else
                {
                    if (urlBuffer[i] == '?' || urlBuffer[i] == '#' || urlBuffer[i] == ';')
                    {
                        return CheckUrlResult.Safe;
                    }
                    if (urlBuffer[i] == ':')
                    {
                        if (SafeUrlSchema(urlBuffer, i))
                        {
                            return CheckUrlResult.Safe;
                        }
                        if (callbackRequested)
                        {
                            if (i == 1 && urlLength > 2 && ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(urlBuffer[0])) && (urlBuffer[2] == '/' || urlBuffer[2] == '\\'))
                            {
                                return CheckUrlResult.Safe;
                            }
                            BufferString bufferString = new BufferString(urlBuffer, 0, urlLength);
                            if (bufferString.EqualsToLowerCaseStringIgnoreCase("objattph://") || bufferString.EqualsToLowerCaseStringIgnoreCase("rtfimage://"))
                            {
                                return CheckUrlResult.Safe;
                            }
                        }
                        return CheckUrlResult.Unsafe;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return CheckUrlResult.Inconclusive;
        }

        private int AllocateVirtualEntry(int index, int offset, int length)
        {
            if (attributeVirtualList == null)
            {
                attributeVirtualList = new AttributeVirtualEntry[4];
            }
            else if (attributeVirtualList.Length == attributeVirtualCount)
            {
                AttributeVirtualEntry[] destinationArray = new AttributeVirtualEntry[attributeVirtualList.Length * 2];
                Array.Copy(attributeVirtualList, 0, destinationArray, 0, attributeVirtualCount);
                attributeVirtualList = destinationArray;
            }
            int num = attributeVirtualCount++;
            attributeVirtualList[num].index = (short)index;
            attributeVirtualList[num].offset = offset;
            attributeVirtualList[num].length = length;
            attributeVirtualList[num].position = 0;
            return num;
        }

        private void VirtualizeFilteredStyle(int index)
        {
            int length = attributeVirtualScratch.Length;
            FlushCssInStyleAttributeToVirtualScratch();
            int length2 = attributeVirtualScratch.Length - length;
            int num = AllocateVirtualEntry((int)attributeIndirectIndex[index + attributeSkipCount].index, length, length2);
            attributeIndirectIndex[index + attributeSkipCount].index = (short)num;
            attributeIndirectIndex[index + attributeSkipCount].kind = AttributeIndirectKind.VirtualFilteredStyle;
        }

        private bool InjectMetaTagIfNecessary()
        {
            if (filterForFragment || !writer.HasEncoding)
            {
                metaInjected = true;
            }
            else if (token.TokenId != HtmlTokenId.Restart && token.TokenId != HtmlTokenId.EncodingChange)
            {
                if (writer.Encoding.CodePage == 65000)
                {
                    OutputMetaTag();
                    metaInjected = true;
                }
                else if (token.TokenId == HtmlTokenId.Tag)
                {
                    if (!insideHtml && token.TagIndex == HtmlTagIndex.Html)
                    {
                        if (token.IsTagEnd)
                        {
                            insideHtml = true;
                        }
                    }
                    else if (!insideHead && token.TagIndex == HtmlTagIndex.Head)
                    {
                        if (token.IsTagEnd)
                        {
                            insideHead = true;
                        }
                    }
                    else if (token.TagIndex > HtmlTagIndex._ASP)
                    {
                        if (insideHtml && !insideHead)
                        {
                            writer.WriteNewLine(true);
                            writer.WriteStartTag(HtmlNameIndex.Head);
                            writer.WriteNewLine(true);
                            OutputMetaTag();
                            writer.WriteEndTag(HtmlNameIndex.Head);
                            writer.WriteNewLine(true);
                        }
                        else
                        {
                            if (insideHead)
                            {
                                writer.WriteNewLine(true);
                            }
                            OutputMetaTag();
                        }
                        metaInjected = true;
                    }
                }
                else if (token.TokenId == HtmlTokenId.Text)
                {
                    if (token.IsWhitespaceOnly)
                    {
                        return false;
                    }
                    token.Text.StripLeadingWhitespace();
                    if (insideHtml && !insideHead)
                    {
                        writer.WriteNewLine(true);
                        writer.WriteStartTag(HtmlNameIndex.Head);
                        writer.WriteNewLine(true);
                        OutputMetaTag();
                        writer.WriteEndTag(HtmlNameIndex.Head);
                        writer.WriteNewLine(true);
                    }
                    else
                    {
                        if (insideHead)
                        {
                            writer.WriteNewLine(true);
                        }
                        OutputMetaTag();
                    }
                    metaInjected = true;
                }
            }
            return true;
        }

        private void OutputMetaTag()
        {
            Encoding encoding = writer.Encoding;
            if (encoding.CodePage == 65000)
            {
                writer.Encoding = Encoding.ASCII;
            }
            writer.WriteStartTag(HtmlNameIndex.Meta);
            writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
            writer.WriteAttributeName(HtmlNameIndex.Content);
            writer.WriteAttributeValueInternal("text/html; charset=");
            writer.WriteAttributeValueInternal(Charset.GetCharset(encoding.CodePage).Name);
            if (encoding.CodePage == 65000)
            {
                writer.WriteTagEnd();
                writer.Encoding = encoding;
            }
        }

        private AttributeIndirectKind GetAttributeIndirectKind(int index)
        {
            if (!attributeIndirect)
            {
                return AttributeIndirectKind.PassThrough;
            }
            return attributeIndirectIndex[index + attributeSkipCount].kind;
        }

        private int GetAttributeVirtualEntryIndex(int index)
        {
            return (int)attributeIndirectIndex[index + attributeSkipCount].index;
        }

        private HtmlAttribute GetAttribute(int index)
        {
            if (!attributeIndirect)
            {
                return token.Attributes[index + attributeSkipCount];
            }
            if (attributeIndirectIndex[index + attributeSkipCount].kind != AttributeIndirectKind.Virtual)
            {
                return token.Attributes[(int)attributeIndirectIndex[index + attributeSkipCount].index];
            }
            return token.Attributes[(int)attributeVirtualList[(int)attributeIndirectIndex[index + attributeSkipCount].index].index];
        }

        internal HtmlAttributeId GetAttributeNameId(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle || attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                return HtmlAttributeId.Style;
            }
            HtmlAttribute attribute = GetAttribute(index);
            return HtmlNameData.names[(int)attribute.NameIndex].publicAttributeId;
        }

        internal HtmlAttributeParts GetAttributeParts(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle || attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                return CompleteAttributeParts;
            }
            HtmlAttribute attribute = GetAttribute(index);
            if (attributeIndirectKind == AttributeIndirectKind.NameOnlyFragment)
            {
                return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart & (HtmlToken.AttrPartMinor)199);
            }
            if (attributeIndirectKind == AttributeIndirectKind.EmptyValue || attributeIndirectKind == AttributeIndirectKind.Virtual)
            {
                return new HtmlAttributeParts(attribute.MajorPart | HtmlToken.AttrPartMajor.End, attribute.MinorPart | HtmlToken.AttrPartMinor.CompleteValue);
            }
            return new HtmlAttributeParts(attribute.MajorPart, attribute.MinorPart);
        }

        internal string GetAttributeName(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle || attributeIndirectKind == AttributeIndirectKind.VirtualFilteredStyle)
            {
                return HtmlNameData.names[40].name;
            }
            HtmlAttribute attribute = GetAttribute(index);
            if (attribute.NameIndex > HtmlNameIndex.Unknown)
            {
                if (!attribute.IsAttrBegin)
                {
                    return string.Empty;
                }
                return HtmlNameData.names[(int)attribute.NameIndex].name;
            }
            else
            {
                if (attribute.HasNameFragment)
                {
                    return attribute.Name.GetString(2147483647);
                }
                if (!attribute.IsAttrBegin)
                {
                    return string.Empty;
                }
                return "?";
            }
        }

        internal string GetAttributeValue(int index)
        {
            AttributeIndirectKind attributeIndirectKind = GetAttributeIndirectKind(index);
            if (attributeIndirectKind != AttributeIndirectKind.PassThrough)
            {
                if (attributeIndirectKind == AttributeIndirectKind.FilteredStyle)
                {
                    VirtualizeFilteredStyle(index);
                    attributeIndirectKind = AttributeIndirectKind.VirtualFilteredStyle;
                }
                if (attributeIndirectKind != AttributeIndirectKind.Virtual && attributeIndirectKind != AttributeIndirectKind.VirtualFilteredStyle)
                {
                    return string.Empty;
                }
                int attributeVirtualEntryIndex = GetAttributeVirtualEntryIndex(index);
                if (attributeVirtualList[attributeVirtualEntryIndex].length != 0)
                {
                    return new string(attributeVirtualScratch.Buffer, attributeVirtualList[attributeVirtualEntryIndex].offset, attributeVirtualList[attributeVirtualEntryIndex].length);
                }
                return string.Empty;
            }
            else
            {
                HtmlAttribute attribute = GetAttribute(index);
                if (!attribute.HasValueFragment)
                {
                    return string.Empty;
                }
                return attribute.Value.GetString(2147483647);
            }
        }

        internal void WriteTag(bool copyTagAttributes)
        {
            CopyInputTag(copyTagAttributes);
        }

        internal void WriteAttribute(int index, bool writeName, bool writeValue)
        {
            if (!writeName)
            {
                if (writeValue)
                {
                    CopyInputAttributeValue(index);
                }
                return;
            }
            if (writeValue)
            {
                CopyInputAttribute(index);
                return;
            }
            CopyInputAttributeName(index);
        }

        private void AppendCssFromTokenText()
        {
            if (cssParserInput == null)
            {
                cssParserInput = new ConverterBufferInput(524288, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4096, false);
            }
            token.Text.WriteTo(cssParserInput);
        }

        private void AppendCss(string css)
        {
            if (cssParserInput == null)
            {
                cssParserInput = new ConverterBufferInput(524288, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4096, false);
            }
            cssParserInput.Write(css);
        }

        private void AppendCssFromAttribute(HtmlAttribute attribute)
        {
            if (cssParserInput == null)
            {
                cssParserInput = new ConverterBufferInput(524288, progressMonitor);
                cssParser = new CssParser(cssParserInput, 4096, false);
            }
            attribute.Value.Rewind();
            attribute.Value.WriteTo(cssParserInput);
        }

        private void FlushCssInStyleTag()
        {
            if (cssParserInput != null)
            {
                writer.WriteNewLine();
                writer.WriteMarkupText("<!--");
                writer.WriteNewLine();
                bool agressiveFiltering = false;
                if (smallCssBlockThreshold != -1 && cssParserInput.MaxTokenSize > smallCssBlockThreshold)
                {
                    agressiveFiltering = true;
                }
                cssParser.SetParseMode(CssParseMode.StyleTag);
                bool flag = true;
                ITextSinkEx textSinkEx = writer.WriteText();
                CssTokenId cssTokenId;
                do
                {
                    cssTokenId = cssParser.Parse();
                    if ((CssTokenId.RuleSet == cssTokenId || CssTokenId.AtRule == cssTokenId) && cssParser.Token.Selectors.ValidCount != 0 && cssParser.Token.Properties.ValidCount != 0)
                    {
                        bool flag2 = CopyInputCssSelectors(cssParser.Token.Selectors, textSinkEx, agressiveFiltering);
                        if (flag2)
                        {
                            if (cssParser.Token.IsPropertyListBegin)
                            {
                                textSinkEx.Write("\r\n\t{");
                            }
                            CopyInputCssProperties(true, cssParser.Token.Properties, textSinkEx, ref flag);
                            if (cssParser.Token.IsPropertyListEnd)
                            {
                                textSinkEx.Write("}\r\n");
                                flag = true;
                            }
                        }
                    }
                }
                while (CssTokenId.EndOfFile != cssTokenId);
                cssParserInput.Reset();
                cssParser.Reset();
                writer.WriteMarkupText("-->");
                writer.WriteNewLine();
            }
        }

        private void FlushCssInStyleAttributeToVirtualScratch()
        {
            cssParser.SetParseMode(CssParseMode.StyleAttribute);
            if (virtualScratchSink == null)
            {
                virtualScratchSink = new VirtualScratchSink(this, 2147483647);
            }
            bool flag = true;
            CssTokenId cssTokenId;
            do
            {
                cssTokenId = cssParser.Parse();
                if (CssTokenId.Declarations == cssTokenId && cssParser.Token.Properties.ValidCount != 0)
                {
                    CopyInputCssProperties(false, cssParser.Token.Properties, virtualScratchSink, ref flag);
                }
            }
            while (CssTokenId.EndOfFile != cssTokenId);
            cssParserInput.Reset();
            cssParser.Reset();
        }

        private void FlushCssInStyleAttribute(HtmlWriter writer)
        {
            cssParser.SetParseMode(CssParseMode.StyleAttribute);
            ITextSinkEx sink = writer.WriteAttributeValue();
            bool flag = true;
            CssTokenId cssTokenId;
            do
            {
                cssTokenId = cssParser.Parse();
                if (CssTokenId.Declarations == cssTokenId && cssParser.Token.Properties.ValidCount != 0)
                {
                    CopyInputCssProperties(false, cssParser.Token.Properties, sink, ref flag);
                }
            }
            while (CssTokenId.EndOfFile != cssTokenId);
            cssParserInput.Reset();
            cssParser.Reset();
        }

        private bool CopyInputCssSelectors(CssToken.SelectorEnumerator selectors, ITextSinkEx sink, bool agressiveFiltering)
        {
            bool flag = false;
            bool flag2 = false;
            selectors.Rewind();
            CssToken.SelectorEnumerator enumerator = selectors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CssSelector current = enumerator.Current;
                if (!current.IsDeleted)
                {
                    if (flag2)
                    {
                        if (current.Combinator == CssSelectorCombinator.None)
                        {
                            sink.Write(", ");
                        }
                        else if (current.Combinator == CssSelectorCombinator.Descendant)
                        {
                            sink.Write(32);
                        }
                        else if (current.Combinator == CssSelectorCombinator.Adjacent)
                        {
                            sink.Write(" + ");
                        }
                        else
                        {
                            sink.Write(" > ");
                        }
                    }
                    flag2 = CopyInputCssSelector(current, sink, agressiveFiltering);
                    flag = (flag || flag2);
                }
            }
            return flag;
        }

        private bool CopyInputCssSelector(CssSelector selector, ITextSinkEx sink, bool agressiveFiltering)
        {
            if (filterForFragment && (!selector.HasClassFragment || (selector.ClassType != CssSelectorClassType.Regular && selector.ClassType != CssSelectorClassType.Hash)))
            {
                return false;
            }
            if (agressiveFiltering)
            {
                if (!selector.HasClassFragment || selector.ClassType != CssSelectorClassType.Regular)
                {
                    return false;
                }
                string @string = selector.ClassName.GetString(256);
                if (!@string.Equals("MsoNormal", StringComparison.Ordinal))
                {
                    return false;
                }
            }
            if (selector.NameId != HtmlNameIndex.Unknown && selector.NameId != HtmlNameIndex._NOTANAME)
            {
                sink.Write(HtmlNameData.names[(int)selector.NameId].name);
            }
            else if (selector.HasNameFragment)
            {
                selector.Name.WriteOriginalTo(sink);
            }
            if (selector.HasClassFragment)
            {
                if (selector.ClassType == CssSelectorClassType.Regular)
                {
                    sink.Write(".");
                }
                else if (selector.ClassType == CssSelectorClassType.Hash)
                {
                    sink.Write("#");
                }
                else if (selector.ClassType == CssSelectorClassType.Pseudo)
                {
                    sink.Write(":");
                }
                if (outputFragment)
                {
                    sink.Write(NamePrefix);
                }
                selector.ClassName.WriteOriginalTo(sink);
            }
            return true;
        }

        private void CopyInputCssProperties(bool inTag, CssToken.PropertyEnumerator properties, ITextSinkEx sink, ref bool firstProperty)
        {
            properties.Rewind();
            CssToken.PropertyEnumerator enumerator = properties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CssProperty current = enumerator.Current;
                if (current.IsPropertyBegin && !current.IsDeleted)
                {
                    CssData.FilterAction filterAction = CssData.filterInstructions[(int)current.NameId].propertyAction;
                    if (CssData.FilterAction.CheckContent == filterAction)
                    {
                        if (current.NameId == CssNameIndex.Display && current.HasValueFragment && current.Value.CaseInsensitiveContainsSubstring("none") && !preserveDisplayNoneStyle)
                        {
                            filterAction = CssData.FilterAction.Drop;
                        }
                        else if (current.NameId == CssNameIndex.Position && current.HasValueFragment && (current.Value.CaseInsensitiveContainsSubstring("absolute") || current.Value.CaseInsensitiveContainsSubstring("relative")) && outputFragment)
                        {
                            filterAction = CssData.FilterAction.Drop;
                        }
                        else
                        {
                            filterAction = CssData.FilterAction.Keep;
                        }
                    }
                    if (CssData.FilterAction.Keep == filterAction)
                    {
                        if (firstProperty)
                        {
                            firstProperty = false;
                        }
                        else
                        {
                            sink.Write(inTag ? ";\r\n\t" : "; ");
                        }
                        CopyInputCssProperty(current, sink);
                    }
                }
            }
        }

        private static void CopyInputCssProperty(CssProperty property, ITextSinkEx sink)
        {
            if (property.IsPropertyBegin && property.NameId != CssNameIndex.Unknown)
            {
                sink.Write(CssData.names[(int)property.NameId].name);
            }
            if (property.NameId == CssNameIndex.Unknown && property.HasNameFragment)
            {
                property.Name.WriteOriginalTo(sink);
            }
            if (property.IsPropertyNameEnd)
            {
                sink.Write(":");
            }
            if (property.HasValueFragment)
            {
                property.Value.WriteEscapedOriginalTo(sink);
            }
        }
    }
}
