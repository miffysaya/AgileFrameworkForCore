using AgileFramework.Security.Exchange.CtsResources;
using AgileFramework.Security.Application.TextConverters.HTML;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlNormalizingParser : IHtmlParser, IRestartable, IReusable, IDisposable
    {
        private enum QueueItemKind : byte
        {
            Empty,
            None,
            Eof,
            BeginElement,
            EndElement,
            OverlappedClose,
            OverlappedReopen,
            PassThrough,
            Space,
            Text,
            Suspend,
            InjectionBegin,
            InjectionEnd,
            EndLastTag
        }

        [Flags]
        private enum QueueItemFlags : byte
        {
            AllowWspLeft = 1,
            AllowWspRight = 2
        }

        private struct Context
        {
            public int topElement;

            public HtmlDtd.ContextType type;

            public HtmlDtd.ContextTextType textType;

            public HtmlDtd.SetId accept;

            public HtmlDtd.SetId reject;

            public HtmlDtd.SetId ignoreEnd;

            public char lastCh;

            public bool oneNL;

            public bool hasSpace;

            public bool eatSpace;
        }

        private struct QueueItem
        {
            public QueueItemKind kind;

            public HtmlTagIndex tagIndex;

            public QueueItemFlags flags;

            public int argument;
        }

        private class DocumentState
        {
            private int queueHead;

            private int queueTail;

            private HtmlToken inputToken;

            private int elementStackTop;

            private int currentRun;

            private int currentRunOffset;

            private int numRuns;

            private HtmlTagIndex[] savedElementStackEntries = new HtmlTagIndex[5];

            private int savedElementStackEntriesCount;

            private bool hasSpace;

            private bool eatSpace;

            private bool validRTC;

            private HtmlTagIndex tagIdRTC;

            public int SavedStackTop
            {
                get
                {
                    return elementStackTop;
                }
            }

            public void Save(HtmlNormalizingParser document, int stackLevel)
            {
                if (stackLevel != document.elementStackTop)
                {
                    Array.Copy(document.elementStack, stackLevel, savedElementStackEntries, 0, document.elementStackTop - stackLevel);
                    savedElementStackEntriesCount = document.elementStackTop - stackLevel;
                    document.elementStackTop = stackLevel;
                }
                else
                {
                    savedElementStackEntriesCount = 0;
                }
                elementStackTop = document.elementStackTop;
                queueHead = document.queueHead;
                queueTail = document.queueTail;
                inputToken = document.inputToken;
                currentRun = document.currentRun;
                currentRunOffset = document.currentRunOffset;
                numRuns = document.numRuns;
                hasSpace = document.context.hasSpace;
                eatSpace = document.context.eatSpace;
                validRTC = document.validRTC;
                tagIdRTC = document.tagIdRTC;
                document.queueStart = document.queueTail;
                document.queueHead = (document.queueTail = document.queueStart);
            }

            public void Restore(HtmlNormalizingParser document)
            {
                if (savedElementStackEntriesCount != 0)
                {
                    Array.Copy(savedElementStackEntries, 0, document.elementStack, document.elementStackTop, savedElementStackEntriesCount);
                    document.elementStackTop += savedElementStackEntriesCount;
                }
                document.queueStart = 0;
                document.queueHead = queueHead;
                document.queueTail = queueTail;
                document.inputToken = inputToken;
                document.currentRun = currentRun;
                document.currentRunOffset = currentRunOffset;
                document.numRuns = numRuns;
                document.context.hasSpace = hasSpace;
                document.context.eatSpace = eatSpace;
                document.validRTC = validRTC;
                document.tagIdRTC = tagIdRTC;
            }
        }

        private class SmallTokenBuilder : HtmlToken
        {
            private char[] spareBuffer = new char[1];

            private Token.RunEntry[] spareRuns = new Token.RunEntry[1];

            public void BuildTagToken(HtmlTagIndex tagIndex, bool closingTag, bool allowWspLeft, bool allowWspRight, bool endOnly)
            {
                tokenId = (TokenId)4;
                argument = 1;
                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
                originalTagIndex = tagIndex;
                this.tagIndex = tagIndex;
                nameIndex = HtmlDtd.tags[(int)tagIndex].nameIndex;
                if (!endOnly)
                {
                    partMajor = HtmlToken.TagPartMajor.Complete;
                    partMinor = HtmlToken.TagPartMinor.CompleteName;
                }
                else
                {
                    partMajor = HtmlToken.TagPartMajor.End;
                    partMinor = HtmlToken.TagPartMinor.Empty;
                }
                flags = (HtmlToken.TagFlags)((byte)((closingTag ? 16 : 0) | (allowWspLeft ? 64 : 0)) | (allowWspRight ? 128 : 0));
            }

            public void BuildOverlappedToken(bool close, int argument)
            {
                tokenId = (close ? ((TokenId)6) : ((TokenId)7));
                this.argument = argument;
                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }

            public void BuildInjectionToken(bool begin, bool head)
            {
                tokenId = (begin ? ((TokenId)8) : ((TokenId)9));
                argument = (head ? 1 : 0);
                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }

            public void BuildSpaceToken()
            {
                tokenId = TextConverters.TokenId.Text;
                argument = 1;
                buffer = spareBuffer;
                runList = spareRuns;
                buffer[0] = ' ';
                runList[0].Initialize((RunType)2147483648u, RunTextType.Space, 67108864u, 1, 0);
                whole.Reset();
                whole.tail = 1;
                wholePosition.Rewind(whole);
            }

            public void BuildTextSliceToken(Token source, int startRun, int startRunOffset, int numRuns)
            {
                tokenId = TextConverters.TokenId.Text;
                argument = 0;
                buffer = source.buffer;
                runList = source.runList;
                whole.Initialize(startRun, startRunOffset);
                whole.tail = whole.head + numRuns;
                wholePosition.Rewind(whole);
            }

            public void BuildEofToken()
            {
                tokenId = TextConverters.TokenId.EndOfFile;
                argument = 0;
                buffer = spareBuffer;
                runList = spareRuns;
                whole.Reset();
                wholePosition.Rewind(whole);
            }
        }

        private HtmlParser parser;

        private IRestartable restartConsumer;

        private int maxElementStack;

        private Context context;

        private Context[] contextStack;

        private int contextStackTop;

        private HtmlTagIndex[] elementStack;

        private int elementStackTop;

        private QueueItem[] queue;

        private int queueHead;

        private int queueTail;

        private int queueStart;

        private bool ensureHead = true;

        private int[] closeList;

        private HtmlTagIndex[] openList;

        private bool validRTC;

        private HtmlTagIndex tagIdRTC;

        private HtmlToken token;

        private HtmlToken inputToken;

        private bool ignoreInputTag;

        private int currentRun;

        private int currentRunOffset;

        private int numRuns;

        private bool allowWspLeft;

        private bool allowWspRight;

        private SmallTokenBuilder tokenBuilder;

        private HtmlInjection injection;

        private DocumentState saveState;

        public HtmlToken Token
        {
            get
            {
                return token;
            }
        }

        public HtmlNormalizingParser(HtmlParser parser, HtmlInjection injection, bool ensureHead, int maxNesting, bool testBoundaryConditions, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
        {
            this.parser = parser;
            this.parser.SetRestartConsumer(this);
            this.injection = injection;
            if (injection != null)
            {
                saveState = new DocumentState();
            }
            this.ensureHead = ensureHead;
            int num = testBoundaryConditions ? 1 : 32;
            maxElementStack = (testBoundaryConditions ? 30 : maxNesting);
            openList = new HtmlTagIndex[8];
            closeList = new int[8];
            elementStack = new HtmlTagIndex[num];
            contextStack = new Context[testBoundaryConditions ? 1 : 4];
            elementStack[elementStackTop++] = HtmlTagIndex._ROOT;
            context.type = HtmlDtd.ContextType.Root;
            context.textType = HtmlDtd.ContextTextType.Full;
            context.reject = HtmlDtd.SetId.Empty;
            queue = new QueueItem[testBoundaryConditions ? 1 : (num / 4)];
            tokenBuilder = new SmallTokenBuilder();
        }

        private void Reinitialize()
        {
            elementStackTop = 0;
            contextStackTop = 0;
            ignoreInputTag = false;
            elementStack[elementStackTop++] = HtmlTagIndex._ROOT;
            context.topElement = 0;
            context.type = HtmlDtd.ContextType.Root;
            context.textType = HtmlDtd.ContextTextType.Full;
            context.accept = HtmlDtd.SetId.Null;
            context.reject = HtmlDtd.SetId.Empty;
            context.ignoreEnd = HtmlDtd.SetId.Null;
            context.hasSpace = false;
            context.eatSpace = false;
            context.oneNL = false;
            context.lastCh = '\0';
            queueHead = 0;
            queueTail = 0;
            queueStart = 0;
            validRTC = false;
            tagIdRTC = HtmlTagIndex._NULL;
            token = null;
            if (injection != null)
            {
                if (injection.Active)
                {
                    parser = (HtmlParser)injection.Pop();
                }
                injection.Reset();
            }
        }

        public void SetRestartConsumer(IRestartable restartConsumer)
        {
            this.restartConsumer = restartConsumer;
        }

        public HtmlTokenId Parse()
        {
            while (QueueEmpty())
            {
                Process(parser.Parse());
            }
            return GetTokenFromQueue();
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
            }
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            ((IReusable)parser).Initialize(newSourceOrDestination);
            Reinitialize();
            parser.SetRestartConsumer(this);
        }

        public void Initialize(string fragment, bool preformatedText)
        {
            parser.Initialize(fragment, preformatedText);
            Reinitialize();
        }

        void IDisposable.Dispose()
        {
            if (parser != null)
            {
                ((IDisposable)parser).Dispose();
            }
            parser = null;
            restartConsumer = null;
            contextStack = null;
            queue = null;
            closeList = null;
            openList = null;
            token = null;
            inputToken = null;
            tokenBuilder = null;
            GC.SuppressFinalize(this);
        }

        private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
        {
            return HtmlDtd.tags[(int)tagIndex];
        }

        private void Process(HtmlTokenId tokenId)
        {
            if (tokenId == HtmlTokenId.None)
            {
                EnqueueHead(QueueItemKind.None);
                return;
            }
            inputToken = parser.Token;
            switch (tokenId)
            {
                case HtmlTokenId.EndOfFile:
                    HandleTokenEof();
                    return;
                case HtmlTokenId.Text:
                    HandleTokenText(parser.Token);
                    return;
                case HtmlTokenId.EncodingChange:
                    EnqueueTail(QueueItemKind.PassThrough);
                    return;
                case HtmlTokenId.Tag:
                    if (parser.Token.NameIndex < HtmlNameIndex.Unknown)
                    {
                        HandleTokenSpecialTag(parser.Token);
                        return;
                    }
                    HandleTokenTag(parser.Token);
                    return;
                case HtmlTokenId.Restart:
                    EnqueueTail(QueueItemKind.PassThrough);
                    return;
                default:
                    return;
            }
        }

        private void HandleTokenEof()
        {
            if (queueHead != queueTail && queue[queueHead].kind == QueueItemKind.Suspend)
            {
                QueueItem queueItem = DoDequeueFirst();
                EnqueueHead(QueueItemKind.EndLastTag, queueItem.tagIndex, 0 != (byte)(queueItem.flags & QueueItemFlags.AllowWspLeft), 0 != (byte)(queueItem.flags & QueueItemFlags.AllowWspRight));
                return;
            }
            if (injection == null || !injection.Active)
            {
                if (injection != null)
                {
                    int num = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
                    if (num == -1)
                    {
                        CloseAllProhibitedContainers(GetTagDefinition(HtmlTagIndex.Body));
                        OpenContainer(HtmlTagIndex.Body);
                        return;
                    }
                    CloseAllContainers(num + 1);
                    if (queueHead != queueTail)
                    {
                        return;
                    }
                    if (injection.HaveTail && !injection.TailDone)
                    {
                        parser = (HtmlParser)injection.Push(false, parser);
                        saveState.Save(this, elementStackTop);
                        EnqueueTail(QueueItemKind.InjectionBegin, 0);
                        if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                        {
                            OpenContainer(HtmlTagIndex.TT);
                            OpenContainer(HtmlTagIndex.Pre);
                        }
                        return;
                    }
                }
                CloseAllContainers();
                EnqueueTail(QueueItemKind.Eof);
                return;
            }
            CloseAllContainers(saveState.SavedStackTop);
            if (queueHead != queueTail)
            {
                return;
            }
            saveState.Restore(this);
            EnqueueHead(QueueItemKind.InjectionEnd, injection.InjectingHead ? 1 : 0);
            parser = (HtmlParser)injection.Pop();
        }

        private void HandleTokenTag(HtmlToken tag)
        {
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;
            if (tag.IsTagBegin)
            {
                StartTagProcessing(tagIndex, tag);
                return;
            }
            if (!ignoreInputTag)
            {
                if (tag.IsTagEnd)
                {
                    DoDequeueFirst();
                }
                if (inputToken.TagIndex != HtmlTagIndex.Unknown)
                {
                    bool flag = GetTagDefinition(tagIndex).scope == HtmlDtd.TagScope.EMPTY;
                    inputToken.Flags = (flag ? (inputToken.Flags | HtmlToken.TagFlags.EmptyScope) : (inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope));
                }
                EnqueueHead(QueueItemKind.PassThrough);
                return;
            }
            if (tag.IsTagEnd)
            {
                ignoreInputTag = false;
            }
        }

        private void HandleTokenSpecialTag(HtmlToken tag)
        {
            tag.Flags |= HtmlToken.TagFlags.EmptyScope;
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)tag.NameIndex].tagIndex;
            if (tag.IsTagBegin)
            {
                StartSpecialTagProcessing(tagIndex, tag);
                return;
            }
            if (!ignoreInputTag)
            {
                if (tag.IsTagEnd)
                {
                    DoDequeueFirst();
                }
                EnqueueHead(QueueItemKind.PassThrough);
                return;
            }
            if (tag.IsTagEnd)
            {
                ignoreInputTag = false;
            }
        }

        private void HandleTokenText(HtmlToken token)
        {
            HtmlTagIndex htmlTagIndex = validRTC ? tagIdRTC : RequiredTextContainer();
            int num = 0;
            Token.RunEnumerator runs = inputToken.Runs;
            if (htmlTagIndex != HtmlTagIndex._NULL)
            {
                while (runs.MoveNext(true))
                {
                    TokenRun current = runs.Current;
                    if (current.TextType > RunTextType.UnusualWhitespace)
                    {
                        break;
                    }
                }
                if (!runs.IsValidPosition)
                {
                    return;
                }
                CloseAllProhibitedContainers(GetTagDefinition(htmlTagIndex));
                OpenContainer(htmlTagIndex);
            }
            else if (context.textType != HtmlDtd.ContextTextType.Literal)
            {
                while (runs.MoveNext(true))
                {
                    TokenRun current2 = runs.Current;
                    if (current2.TextType > RunTextType.UnusualWhitespace)
                    {
                        break;
                    }
                    int arg_95_0 = num;
                    TokenRun current3 = runs.Current;
                    num = arg_95_0 + ((current3.TextType == RunTextType.NewLine) ? 1 : 2);
                }
            }
            if (context.textType == HtmlDtd.ContextTextType.Literal)
            {
                EnqueueTail(QueueItemKind.PassThrough);
                return;
            }
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                if (num != 0)
                {
                    AddSpace(num == 1);
                }
                currentRun = runs.CurrentIndex;
                currentRunOffset = runs.CurrentOffset;
                if (runs.IsValidPosition)
                {
                    TokenRun current4 = runs.Current;
                    char firstChar = current4.FirstChar;
                    char lastChar;
                    TokenRun current6;
                    do
                    {
                        TokenRun current5 = runs.Current;
                        lastChar = current5.LastChar;
                        if (!runs.MoveNext(true))
                        {
                            break;
                        }
                        current6 = runs.Current;
                    }
                    while (current6.TextType > RunTextType.UnusualWhitespace);
                    AddNonspace(firstChar, lastChar);
                }
                numRuns = runs.CurrentIndex - currentRun;
            }
        }

        private void StartTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            if ((context.reject != HtmlDtd.SetId.Null && !HtmlDtd.IsTagInSet(tagIndex, context.reject)) || (context.accept != HtmlDtd.SetId.Null && HtmlDtd.IsTagInSet(tagIndex, context.accept)))
            {
                if (!tag.IsEndTag)
                {
                    if (!ProcessOpenTag(tagIndex, GetTagDefinition(tagIndex)))
                    {
                        ProcessIgnoredTag(tagIndex, tag);
                        return;
                    }
                }
                else
                {
                    if (context.ignoreEnd != HtmlDtd.SetId.Null && HtmlDtd.IsTagInSet(tagIndex, context.ignoreEnd))
                    {
                        ProcessIgnoredTag(tagIndex, tag);
                        return;
                    }
                    if (!ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                    {
                        ProcessIgnoredTag(tagIndex, tag);
                        return;
                    }
                }
            }
            else if (context.type == HtmlDtd.ContextType.Select && tagIndex == HtmlTagIndex.Select)
            {
                if (!ProcessEndTag(tagIndex, GetTagDefinition(tagIndex)))
                {
                    ProcessIgnoredTag(tagIndex, tag);
                    return;
                }
            }
            else
            {
                ProcessIgnoredTag(tagIndex, tag);
            }
        }

        private void StartSpecialTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            EnqueueTail(QueueItemKind.PassThrough);
            if (!tag.IsTagEnd)
            {
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }
        }

        private void ProcessIgnoredTag(HtmlTagIndex tagIndex, HtmlToken tag)
        {
            if (!tag.IsTagEnd)
            {
                ignoreInputTag = true;
            }
        }

        private bool ProcessOpenTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            if (!PrepareContainer(tagIndex, tagDef))
            {
                return false;
            }
            PushElement(tagIndex, true, tagDef);
            return true;
        }

        private bool ProcessEndTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            if (tagIndex == HtmlTagIndex.Unknown)
            {
                PushElement(tagIndex, true, tagDef);
                return true;
            }
            bool flag = true;
            bool flag2 = false;
            int num;
            if (tagDef.match != HtmlDtd.SetId.Null)
            {
                num = FindContainer(tagDef.match, tagDef.endContainers);
                if (num >= 0 && elementStack[num] != tagIndex)
                {
                    flag = false;
                }
            }
            else
            {
                num = FindContainer(tagIndex, tagDef.endContainers);
            }
            if (num < 0)
            {
                HtmlTagIndex unmatchedSubstitute = tagDef.unmatchedSubstitute;
                if (unmatchedSubstitute == HtmlTagIndex._NULL)
                {
                    return false;
                }
                if (unmatchedSubstitute == HtmlTagIndex._IMPLICIT_BEGIN)
                {
                    if (!PrepareContainer(tagIndex, tagDef))
                    {
                        return false;
                    }
                    HtmlToken expr_79 = inputToken;
                    expr_79.Flags &= ~HtmlToken.TagFlags.EndTag;
                    num = PushElement(tagIndex, flag, tagDef);
                    flag2 |= flag;
                    flag = false;
                }
                else
                {
                    num = FindContainer(unmatchedSubstitute, GetTagDefinition(unmatchedSubstitute).endContainers);
                    if (num < 0)
                    {
                        return false;
                    }
                    flag = false;
                }
            }
            if (num >= 0 && num < elementStackTop)
            {
                flag &= inputToken.IsEndTag;
                flag2 |= flag;
                CloseContainer(num, flag);
            }
            return flag2;
        }

        private bool PrepareContainer(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
        {
            if (tagIndex == HtmlTagIndex.Unknown)
            {
                return true;
            }
            if (tagDef.maskingContainers != HtmlDtd.SetId.Null)
            {
                int num = FindContainer(tagDef.maskingContainers, tagDef.beginContainers);
                if (num >= 0)
                {
                    return false;
                }
            }
            CloseAllProhibitedContainers(tagDef);
            HtmlTagIndex htmlTagIndex = HtmlTagIndex._NULL;
            if (tagDef.textType == HtmlDtd.TagTextType.ALWAYS || (tagDef.textType == HtmlDtd.TagTextType.QUERY && QueryTextlike(tagIndex)))
            {
                htmlTagIndex = (validRTC ? tagIdRTC : RequiredTextContainer());
                if (htmlTagIndex != HtmlTagIndex._NULL)
                {
                    CloseAllProhibitedContainers(GetTagDefinition(htmlTagIndex));
                    if (-1 == OpenContainer(htmlTagIndex))
                    {
                        return false;
                    }
                }
            }
            if (htmlTagIndex == HtmlTagIndex._NULL && tagDef.requiredContainers != HtmlDtd.SetId.Null)
            {
                int num2 = FindContainer(tagDef.requiredContainers, tagDef.beginContainers);
                if (num2 < 0)
                {
                    CloseAllProhibitedContainers(GetTagDefinition(tagDef.defaultContainer));
                    if (-1 == OpenContainer(tagDef.defaultContainer))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int OpenContainer(HtmlTagIndex tagIndex)
        {
            int num = 0;
            while (tagIndex != HtmlTagIndex._NULL)
            {
                openList[num++] = tagIndex;
                HtmlDtd.TagDefinition tagDefinition = GetTagDefinition(tagIndex);
                if (tagDefinition.requiredContainers == HtmlDtd.SetId.Null)
                {
                    break;
                }
                int num2 = FindContainer(tagDefinition.requiredContainers, tagDefinition.beginContainers);
                if (num2 >= 0)
                {
                    break;
                }
                tagIndex = tagDefinition.defaultContainer;
            }
            if (tagIndex == HtmlTagIndex._NULL)
            {
                return -1;
            }
            int result = -1;
            for (int i = num - 1; i >= 0; i--)
            {
                tagIndex = openList[i];
                result = PushElement(tagIndex, false, GetTagDefinition(tagIndex));
            }
            return result;
        }

        private void CloseContainer(int stackPos, bool useInputTag)
        {
            if (stackPos != elementStackTop - 1)
            {
                bool flag = false;
                int num = 0;
                closeList[num++] = stackPos;
                if (GetTagDefinition(elementStack[stackPos]).scope == HtmlDtd.TagScope.NESTED)
                {
                    flag = true;
                }
                for (int i = stackPos + 1; i < elementStackTop; i++)
                {
                    HtmlDtd.TagDefinition tagDefinition = GetTagDefinition(elementStack[i]);
                    if (num == closeList.Length)
                    {
                        int[] destinationArray = new int[closeList.Length * 2];
                        Array.Copy(closeList, 0, destinationArray, 0, num);
                        closeList = destinationArray;
                    }
                    if (flag && tagDefinition.scope == HtmlDtd.TagScope.NESTED)
                    {
                        closeList[num++] = i;
                    }
                    else
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if (HtmlDtd.IsTagInSet(elementStack[closeList[j]], tagDefinition.endContainers))
                            {
                                closeList[num++] = i;
                                flag = (flag || tagDefinition.scope == HtmlDtd.TagScope.NESTED);
                                break;
                            }
                        }
                    }
                }
                for (int k = num - 1; k > 0; k--)
                {
                    PopElement(closeList[k], false);
                }
            }
            PopElement(stackPos, useInputTag);
        }

        private void CloseAllProhibitedContainers(HtmlDtd.TagDefinition tagDef)
        {
            HtmlDtd.SetId prohibitedContainers = tagDef.prohibitedContainers;
            if (prohibitedContainers != HtmlDtd.SetId.Null)
            {
                while (true)
                {
                    int num = FindContainer(prohibitedContainers, tagDef.beginContainers);
                    if (num < 0)
                    {
                        break;
                    }
                    CloseContainer(num, false);
                }
                return;
            }
        }

        private void CloseAllContainers()
        {
            for (int i = elementStackTop - 1; i > 0; i--)
            {
                CloseContainer(i, false);
            }
        }

        private void CloseAllContainers(int level)
        {
            for (int i = elementStackTop - 1; i >= level; i--)
            {
                CloseContainer(i, false);
            }
        }

        private int FindContainer(HtmlDtd.SetId matchSet, HtmlDtd.SetId stopSet)
        {
            int num = elementStackTop - 1;
            while (num >= 0 && !HtmlDtd.IsTagInSet(elementStack[num], matchSet))
            {
                if (HtmlDtd.IsTagInSet(elementStack[num], stopSet))
                {
                    return -1;
                }
                num--;
            }
            return num;
        }

        private int FindContainer(HtmlTagIndex match, HtmlDtd.SetId stopSet)
        {
            int num = elementStackTop - 1;
            while (num >= 0 && elementStack[num] != match)
            {
                if (HtmlDtd.IsTagInSet(elementStack[num], stopSet))
                {
                    return -1;
                }
                num--;
            }
            return num;
        }

        private HtmlTagIndex RequiredTextContainer()
        {
            validRTC = true;
            for (int i = elementStackTop - 1; i >= 0; i--)
            {
                HtmlDtd.TagDefinition tagDefinition = GetTagDefinition(elementStack[i]);
                if (tagDefinition.textScope == HtmlDtd.TagTextScope.INCLUDE)
                {
                    tagIdRTC = HtmlTagIndex._NULL;
                    return tagIdRTC;
                }
                if (tagDefinition.textScope == HtmlDtd.TagTextScope.EXCLUDE)
                {
                    tagIdRTC = tagDefinition.textSubcontainer;
                    return tagIdRTC;
                }
            }
            tagIdRTC = HtmlTagIndex._NULL;
            return tagIdRTC;
        }

        private int PushElement(HtmlTagIndex tagIndex, bool useInputTag, HtmlDtd.TagDefinition tagDef)
        {
            int num;
            if (ensureHead)
            {
                if (tagIndex == HtmlTagIndex.Body)
                {
                    num = PushElement(HtmlTagIndex.Head, false, HtmlDtd.tags[52]);
                    PopElement(num, false);
                }
                else if (tagIndex == HtmlTagIndex.Head)
                {
                    ensureHead = false;
                }
            }
            if (tagDef.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                validRTC = false;
            }
            if (elementStackTop == elementStack.Length && !EnsureElementStackSpace())
            {
                throw new TextConvertersException(TextConvertersStrings.HtmlNestingTooDeep);
            }
            bool flag = tagDef.scope == HtmlDtd.TagScope.EMPTY;
            if (useInputTag)
            {
                if (inputToken.TagIndex != HtmlTagIndex.Unknown)
                {
                    inputToken.Flags = (flag ? (inputToken.Flags | HtmlToken.TagFlags.EmptyScope) : (inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope));
                }
                else
                {
                    flag = true;
                }
            }
            num = elementStackTop++;
            elementStack[num] = tagIndex;
            LFillTagB(tagDef);
            EnqueueTail(useInputTag ? QueueItemKind.PassThrough : QueueItemKind.BeginElement, tagIndex, allowWspLeft, allowWspRight);
            if (useInputTag && !inputToken.IsTagEnd)
            {
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }
            RFillTagB(tagDef);
            if (!flag)
            {
                if (tagDef.contextType != HtmlDtd.ContextType.None)
                {
                    if (contextStackTop == contextStack.Length)
                    {
                        EnsureContextStackSpace();
                    }
                    contextStack[contextStackTop++] = context;
                    context.topElement = num;
                    context.type = tagDef.contextType;
                    context.textType = tagDef.contextTextType;
                    context.accept = tagDef.accept;
                    context.reject = tagDef.reject;
                    context.ignoreEnd = tagDef.ignoreEnd;
                    context.hasSpace = false;
                    context.eatSpace = false;
                    context.oneNL = false;
                    context.lastCh = '\0';
                    if (context.textType != HtmlDtd.ContextTextType.Full)
                    {
                        allowWspLeft = false;
                        allowWspRight = false;
                    }
                    RFillTagB(tagDef);
                }
            }
            else
            {
                elementStackTop--;
            }
            return num;
        }

        private void PopElement(int stackPos, bool useInputTag)
        {
            HtmlTagIndex tagIndex = elementStack[stackPos];
            HtmlDtd.TagDefinition tagDefinition = GetTagDefinition(tagIndex);
            if (tagDefinition.textScope != HtmlDtd.TagTextScope.NEUTRAL)
            {
                validRTC = false;
            }
            if (stackPos == context.topElement)
            {
                if (context.textType == HtmlDtd.ContextTextType.Full)
                {
                    LFillTagE(tagDefinition);
                }
                context = contextStack[--contextStackTop];
            }
            LFillTagE(tagDefinition);
            if (stackPos != elementStackTop - 1)
            {
                EnqueueTail(QueueItemKind.OverlappedClose, elementStackTop - stackPos - 1);
            }
            EnqueueTail(useInputTag ? QueueItemKind.PassThrough : QueueItemKind.EndElement, tagIndex, allowWspLeft, allowWspRight);
            if (useInputTag && !inputToken.IsTagEnd)
            {
                EnqueueTail(QueueItemKind.Suspend, tagIndex, allowWspLeft, allowWspRight);
            }
            RFillTagE(tagDefinition);
            if (stackPos != elementStackTop - 1)
            {
                EnqueueTail(QueueItemKind.OverlappedReopen, elementStackTop - stackPos - 1);
                Array.Copy(elementStack, stackPos + 1, elementStack, stackPos, elementStackTop - stackPos - 1);
                if (context.topElement > stackPos)
                {
                    context.topElement = context.topElement - 1;
                    int num = contextStackTop - 1;
                    while (num > 0 && contextStack[num].topElement >= stackPos)
                    {
                        Context[] expr_15C_cp_0 = contextStack;
                        int expr_15C_cp_1 = num;
                        expr_15C_cp_0[expr_15C_cp_1].topElement = expr_15C_cp_0[expr_15C_cp_1].topElement - 1;
                        num--;
                    }
                }
            }
            elementStackTop--;
        }

        private void AddNonspace(char firstChar, char lastChar)
        {
            if (context.hasSpace)
            {
                context.hasSpace = false;
                if (context.lastCh == '\0' || !context.oneNL || !ParseSupport.TwoFarEastNonHanguelChars(context.lastCh, firstChar))
                {
                    EnqueueTail(QueueItemKind.Space);
                }
            }
            EnqueueTail(QueueItemKind.Text);
            context.eatSpace = false;
            context.lastCh = lastChar;
            context.oneNL = false;
        }

        private void AddSpace(bool oneNL)
        {
            if (!context.eatSpace)
            {
                context.hasSpace = true;
            }
            if (context.lastCh != '\0')
            {
                if (oneNL && !context.oneNL)
                {
                    context.oneNL = true;
                    return;
                }
                context.lastCh = '\0';
            }
        }

        private bool QueryTextlike(HtmlTagIndex tagIndex)
        {
            HtmlDtd.ContextType type = context.type;
            int num = contextStackTop;
            while (num != 0)
            {
                switch (type)
                {
                    case HtmlDtd.ContextType.Head:
                        if (tagIndex == HtmlTagIndex.Object)
                        {
                            return false;
                        }
                        break;
                    case HtmlDtd.ContextType.Body:
                        if (tagIndex <= HtmlTagIndex.Div)
                        {
                            if (tagIndex != HtmlTagIndex.A && tagIndex != HtmlTagIndex.Applet && tagIndex != HtmlTagIndex.Div)
                            {
                                return false;
                            }
                        }
                        else if (tagIndex != HtmlTagIndex.Input && tagIndex != HtmlTagIndex.Object && tagIndex != HtmlTagIndex.Span)
                        {
                            return false;
                        }
                        return true;
                }
                type = contextStack[--num].type;
            }
            return tagIndex == HtmlTagIndex.Object || tagIndex == HtmlTagIndex.Applet;
        }

        private void LFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                LFill(FillCodeFromTag(tagDef).LB, FillCodeFromTag(tagDef).RB);
            }
        }

        private void RFillTagB(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                RFill(FillCodeFromTag(tagDef).RB);
            }
        }

        private void LFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                LFill(FillCodeFromTag(tagDef).LE, FillCodeFromTag(tagDef).RE);
            }
        }

        private void RFillTagE(HtmlDtd.TagDefinition tagDef)
        {
            if (context.textType == HtmlDtd.ContextTextType.Full)
            {
                RFill(FillCodeFromTag(tagDef).RE);
            }
        }

        private void LFill(HtmlDtd.FillCode codeLeft, HtmlDtd.FillCode codeRight)
        {
            allowWspLeft = (context.hasSpace || codeLeft == HtmlDtd.FillCode.EAT);
            context.lastCh = '\0';
            if (context.hasSpace)
            {
                if (codeLeft == HtmlDtd.FillCode.PUT)
                {
                    EnqueueTail(QueueItemKind.Space);
                    context.eatSpace = true;
                }
                context.hasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
            }
            allowWspRight = (context.hasSpace || codeRight == HtmlDtd.FillCode.EAT);
        }

        private void RFill(HtmlDtd.FillCode code)
        {
            if (code == HtmlDtd.FillCode.EAT)
            {
                context.hasSpace = false;
                context.eatSpace = true;
                return;
            }
            if (code == HtmlDtd.FillCode.PUT)
            {
                context.eatSpace = false;
            }
        }

        private bool QueueEmpty()
        {
            return queueHead == queueTail || queue[queueHead].kind == QueueItemKind.Suspend;
        }

        private QueueItemKind QueueHeadKind()
        {
            if (queueHead == queueTail)
            {
                return QueueItemKind.Empty;
            }
            return queue[queueHead].kind;
        }

        private void EnqueueTail(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }
            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = tagIndex;
            queue[queueTail].flags = (QueueItemFlags)((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
            queue[queueTail].argument = 0;
            queueTail++;
        }

        private void EnqueueTail(QueueItemKind kind, int argument)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }
            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = HtmlTagIndex._NULL;
            queue[queueTail].flags = (QueueItemFlags)0;
            queue[queueTail].argument = argument;
            queueTail++;
        }

        private void EnqueueTail(QueueItemKind kind)
        {
            if (queueTail == queue.Length)
            {
                ExpandQueue();
            }
            queue[queueTail].kind = kind;
            queue[queueTail].tagIndex = HtmlTagIndex._NULL;
            queue[queueTail].flags = (QueueItemFlags)0;
            queue[queueTail].argument = 0;
            queueTail++;
        }

        private void EnqueueHead(QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
        {
            if (queueHead != queueStart)
            {
                queueHead--;
            }
            else
            {
                queueTail++;
            }
            queue[queueHead].kind = kind;
            queue[queueHead].tagIndex = tagIndex;
            queue[queueHead].flags = (QueueItemFlags)((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
            queue[queueHead].argument = 0;
        }

        private void EnqueueHead(QueueItemKind kind)
        {
            EnqueueHead(kind, 0);
        }

        private void EnqueueHead(QueueItemKind kind, int argument)
        {
            if (queueHead != queueStart)
            {
                queueHead--;
            }
            else
            {
                queueTail++;
            }
            queue[queueHead].kind = kind;
            queue[queueHead].tagIndex = HtmlTagIndex._NULL;
            queue[queueHead].flags = (QueueItemFlags)0;
            queue[queueHead].argument = argument;
        }

        private HtmlTokenId GetTokenFromQueue()
        {
            switch (QueueHeadKind())
            {
                case QueueItemKind.None:
                    DoDequeueFirst();
                    token = null;
                    return HtmlTokenId.None;
                case QueueItemKind.Eof:
                    tokenBuilder.BuildEofToken();
                    token = tokenBuilder;
                    break;
                case QueueItemKind.BeginElement:
                case QueueItemKind.EndElement:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        tokenBuilder.BuildTagToken(queueItem.tagIndex, queueItem.kind == QueueItemKind.EndElement, (byte)(queueItem.flags & QueueItemFlags.AllowWspLeft) == 1, (byte)(queueItem.flags & QueueItemFlags.AllowWspRight) == 2, false);
                        token = tokenBuilder;
                        if (queueItem.kind == QueueItemKind.BeginElement && token.OriginalTagId == HtmlTagIndex.Body && injection != null && injection.HaveHead && !injection.HeadDone)
                        {
                            int num = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
                            parser = (HtmlParser)injection.Push(true, parser);
                            saveState.Save(this, num + 1);
                            EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                OpenContainer(HtmlTagIndex.TT);
                                OpenContainer(HtmlTagIndex.Pre);
                            }
                        }
                        return token.TokenId;
                    }
                case QueueItemKind.OverlappedClose:
                case QueueItemKind.OverlappedReopen:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        tokenBuilder.BuildOverlappedToken(queueItem.kind == QueueItemKind.OverlappedClose, queueItem.argument);
                        token = tokenBuilder;
                        return token.TokenId;
                    }
                case QueueItemKind.PassThrough:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        token = inputToken;
                        if (token.TokenId == HtmlTokenId.Tag)
                        {
                            HtmlToken expr_84 = token;
                            expr_84.Flags |= (HtmlToken.TagFlags)((((byte)(queueItem.flags & QueueItemFlags.AllowWspLeft) == 1) ? 64 : 0) | (((byte)(queueItem.flags & QueueItemFlags.AllowWspRight) == 2) ? 128 : 0));
                            if (token.OriginalTagId == HtmlTagIndex.Body && token.IsTagEnd && injection != null && injection.HaveHead && !injection.HeadDone)
                            {
                                int num2 = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
                                parser = (HtmlParser)injection.Push(true, parser);
                                saveState.Save(this, num2 + 1);
                                EnqueueTail(QueueItemKind.InjectionBegin, 1);
                                if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                                {
                                    OpenContainer(HtmlTagIndex.TT);
                                    OpenContainer(HtmlTagIndex.Pre);
                                }
                            }
                        }
                        return token.TokenId;
                    }
                case QueueItemKind.Space:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        tokenBuilder.BuildSpaceToken();
                        token = tokenBuilder;
                        return token.TokenId;
                    }
                case QueueItemKind.Text:
                    {
                        bool flag = false;
                        int argument = 0;
                        QueueItem queueItem = DoDequeueFirst();
                        if (queueHead != queueTail)
                        {
                            flag = true;
                            argument = queue[queueHead].argument;
                            DoDequeueFirst();
                        }
                        tokenBuilder.BuildTextSliceToken(inputToken, currentRun, currentRunOffset, numRuns);
                        token = tokenBuilder;
                        Token.RunEnumerator runs = inputToken.Runs;
                        if (runs.IsValidPosition)
                        {
                            int num3 = 0;
                            TokenRun current2;
                            do
                            {
                                int arg_453_0 = num3;
                                TokenRun current = runs.Current;
                                num3 = arg_453_0 + ((current.TextType == RunTextType.NewLine) ? 1 : 2);
                                if (!runs.MoveNext(true))
                                {
                                    break;
                                }
                                current2 = runs.Current;
                            }
                            while (current2.TextType <= RunTextType.UnusualWhitespace);
                            if (num3 != 0)
                            {
                                AddSpace(num3 == 1);
                            }
                            currentRun = runs.CurrentIndex;
                            currentRunOffset = runs.CurrentOffset;
                            if (runs.IsValidPosition)
                            {
                                TokenRun current3 = runs.Current;
                                char firstChar = current3.FirstChar;
                                char lastChar;
                                TokenRun current5;
                                do
                                {
                                    TokenRun current4 = runs.Current;
                                    lastChar = current4.LastChar;
                                    if (!runs.MoveNext(true))
                                    {
                                        break;
                                    }
                                    current5 = runs.Current;
                                }
                                while (current5.TextType > RunTextType.UnusualWhitespace);
                                AddNonspace(firstChar, lastChar);
                            }
                            numRuns = runs.CurrentIndex - currentRun;
                        }
                        else
                        {
                            currentRun = runs.CurrentIndex;
                            currentRunOffset = runs.CurrentOffset;
                            numRuns = 0;
                        }
                        if (flag)
                        {
                            EnqueueTail(QueueItemKind.InjectionEnd, argument);
                        }
                        return token.TokenId;
                    }
                case QueueItemKind.InjectionBegin:
                case QueueItemKind.InjectionEnd:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        tokenBuilder.BuildInjectionToken(queueItem.kind == QueueItemKind.InjectionBegin, queueItem.argument != 0);
                        token = tokenBuilder;
                        break;
                    }
                case QueueItemKind.EndLastTag:
                    {
                        QueueItem queueItem = DoDequeueFirst();
                        tokenBuilder.BuildTagToken(queueItem.tagIndex, false, (byte)(queueItem.flags & QueueItemFlags.AllowWspLeft) == 1, (byte)(queueItem.flags & QueueItemFlags.AllowWspRight) == 2, true);
                        token = tokenBuilder;
                        if (queueItem.kind == QueueItemKind.BeginElement && token.OriginalTagId == HtmlTagIndex.Body && injection != null && injection.HaveHead && !injection.HeadDone)
                        {
                            int num4 = FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
                            parser = (HtmlParser)injection.Push(true, parser);
                            saveState.Save(this, num4 + 1);
                            EnqueueTail(QueueItemKind.InjectionBegin, 1);
                            if (injection.HeaderFooterFormat == HeaderFooterFormat.Text)
                            {
                                OpenContainer(HtmlTagIndex.TT);
                                OpenContainer(HtmlTagIndex.Pre);
                            }
                        }
                        return token.TokenId;
                    }
            }
            return token.TokenId;
        }

        private void ExpandQueue()
        {
            QueueItem[] destinationArray = new QueueItem[queue.Length * 2];
            Array.Copy(queue, queueHead, destinationArray, queueHead, queueTail - queueHead);
            if (queueStart != 0)
            {
                Array.Copy(queue, 0, destinationArray, 0, queueStart);
            }
            queue = destinationArray;
        }

        private QueueItem DoDequeueFirst()
        {
            int num = queueHead;
            queueHead++;
            if (queueHead == queueTail)
            {
                queueHead = (queueTail = queueStart);
            }
            return queue[num];
        }

        private HtmlDtd.TagFill FillCodeFromTag(HtmlDtd.TagDefinition tagDef)
        {
            if (context.type == HtmlDtd.ContextType.Select && tagDef.tagIndex != HtmlTagIndex.Option)
            {
                return HtmlDtd.TagFill.PUT_PUT_PUT_PUT;
            }
            if (context.type == HtmlDtd.ContextType.Title)
            {
                return HtmlDtd.TagFill.NUL_EAT_EAT_NUL;
            }
            return tagDef.fill;
        }

        private bool EnsureElementStackSpace()
        {
            if (elementStackTop == elementStack.Length)
            {
                if (elementStack.Length >= maxElementStack)
                {
                    return false;
                }
                int num = (maxElementStack / 2 > elementStack.Length) ? (elementStack.Length * 2) : maxElementStack;
                HtmlTagIndex[] destinationArray = new HtmlTagIndex[num];
                Array.Copy(elementStack, 0, destinationArray, 0, elementStackTop);
                elementStack = destinationArray;
            }
            return true;
        }

        private void EnsureContextStackSpace()
        {
            if (contextStackTop + 1 > contextStack.Length)
            {
                Context[] destinationArray = new Context[contextStack.Length * 2];
                Array.Copy(contextStack, 0, destinationArray, 0, contextStackTop);
                contextStack = destinationArray;
            }
        }
    }
}
