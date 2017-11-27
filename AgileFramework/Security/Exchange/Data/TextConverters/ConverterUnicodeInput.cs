using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterUnicodeInput : ConverterInput, IReusable, IDisposable
    {
        private TextReader pullSource;

        private ConverterWriter pushSource;

        private char[] parseBuffer;

        private int parseStart;

        private int parseEnd;

        private char[] pushChunkBuffer;

        private int pushChunkStart;

        private int pushChunkCount;

        private int pushChunkUsed;

        public ConverterUnicodeInput(object source, bool push, int maxParseToken, bool testBoundaryConditions, IProgressMonitor progressMonitor) : base(progressMonitor)
        {
            if (push)
            {
                pushSource = (source as ConverterWriter);
            }
            else
            {
                pullSource = (source as TextReader);
            }
            maxTokenSize = maxParseToken;
            parseBuffer = new char[testBoundaryConditions ? 123 : 4096];
            if (pushSource != null)
            {
                pushSource.SetSink(this);
            }
        }

        private void Reinitialize()
        {
            parseStart = (parseEnd = 0);
            pushChunkStart = 0;
            pushChunkCount = 0;
            pushChunkUsed = 0;
            pushChunkBuffer = null;
            endOfFile = false;
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (pullSource != null && newSourceOrDestination != null)
            {
                TextReader textReader = newSourceOrDestination as TextReader;
                pullSource = textReader ?? throw new InvalidOperationException("cannot reinitialize this converter - new input should be a TextReader object");
            }
            Reinitialize();
        }

        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            int num = parseEnd - end;
            if (parseBuffer.Length - parseEnd <= 1 && !EnsureFreeSpace() && num == 0)
            {
                return true;
            }
            while (!endOfFile && parseBuffer.Length - parseEnd > 1)
            {
                if (pushSource != null)
                {
                    if (pushChunkCount == 0 && !pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out endOfFile))
                    {
                        break;
                    }
                    if (pushChunkCount - pushChunkUsed != 0)
                    {
                        int num2 = Math.Min(pushChunkCount - pushChunkUsed, parseBuffer.Length - parseEnd - 1);
                        Buffer.BlockCopy(pushChunkBuffer, (pushChunkStart + pushChunkUsed) * 2, parseBuffer, parseEnd * 2, num2 * 2);
                        pushChunkUsed += num2;
                        parseEnd += num2;
                        parseBuffer[parseEnd] = '\0';
                        num += num2;
                        if (pushChunkCount - pushChunkUsed == 0)
                        {
                            pushSource.ReportRead(pushChunkCount);
                            pushChunkStart = 0;
                            pushChunkCount = 0;
                            pushChunkUsed = 0;
                            pushChunkBuffer = null;
                        }
                    }
                }
                else
                {
                    int num3 = pullSource.Read(parseBuffer, parseEnd, parseBuffer.Length - parseEnd - 1);
                    if (num3 == 0)
                    {
                        endOfFile = true;
                    }
                    else
                    {
                        parseEnd += num3;
                        parseBuffer[parseEnd] = '\0';
                        num += num3;
                    }
                    if (progressMonitor != null)
                    {
                        progressMonitor.ReportProgress();
                    }
                }
            }
            buffer = parseBuffer;
            if (start != parseStart)
            {
                current = parseStart + (current - start);
                start = parseStart;
            }
            end = parseEnd;
            return num != 0 || endOfFile;
        }

        public override void ReportProcessed(int processedSize)
        {
            parseStart += processedSize;
        }

        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            if (gapEnd == parseEnd)
            {
                parseEnd = gapBegin;
                parseBuffer[gapBegin] = '\0';
                return gapBegin;
            }
            Buffer.BlockCopy(parseBuffer, gapEnd, parseBuffer, gapBegin, parseEnd - gapEnd);
            parseEnd = gapBegin + (parseEnd - gapEnd);
            parseBuffer[parseEnd] = '\0';
            return parseEnd;
        }

        public void GetInputBuffer(out char[] inputBuffer, out int inputOffset, out int inputCount, out int parseCount)
        {
            inputBuffer = parseBuffer;
            inputOffset = parseEnd;
            inputCount = parseBuffer.Length - parseEnd - 1;
            parseCount = parseEnd - parseStart;
        }

        public void Commit(int inputCount)
        {
            parseEnd += inputCount;
            parseBuffer[parseEnd] = '\0';
        }

        protected override void Dispose()
        {
            pullSource = null;
            pushSource = null;
            parseBuffer = null;
            pushChunkBuffer = null;
            base.Dispose();
        }

        private bool EnsureFreeSpace()
        {
            if (parseBuffer.Length - (parseEnd - parseStart) <= 1 || (parseStart < 1 && parseBuffer.Length < maxTokenSize + 1L))
            {
                if (parseBuffer.Length >= maxTokenSize + 1L)
                {
                    return false;
                }
                long num = parseBuffer.Length * 2;
                if (num > maxTokenSize + 1L)
                {
                    num = maxTokenSize + 1L;
                }
                if (num > 2147483647L)
                {
                    num = 2147483647L;
                }
                char[] dst = new char[(int)num];
                Buffer.BlockCopy(parseBuffer, parseStart * 2, dst, 0, (parseEnd - parseStart + 1) * 2);
                parseBuffer = dst;
                parseEnd -= parseStart;
                parseStart = 0;
            }
            else
            {
                Buffer.BlockCopy(parseBuffer, parseStart * 2, parseBuffer, 0, (parseEnd - parseStart + 1) * 2);
                parseEnd -= parseStart;
                parseStart = 0;
            }
            return true;
        }
    }
}
