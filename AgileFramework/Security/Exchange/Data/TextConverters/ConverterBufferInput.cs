using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterBufferInput : ConverterInput, ITextSink, IDisposable
    {
        private const int DefaultMaxLength = 32768;

        private int maxLength;

        private string originalFragment;

        private char[] parseBuffer;

        public bool IsEnough
        {
            get
            {
                return maxTokenSize >= maxLength;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return maxTokenSize == 0;
            }
        }

        public ConverterBufferInput(int maxLength, IProgressMonitor progressMonitor) : base(progressMonitor)
        {
            this.maxLength = maxLength;
        }

        public ConverterBufferInput(string fragment, IProgressMonitor progressMonitor) : this(32768, fragment, progressMonitor)
        {
        }

        public ConverterBufferInput(int maxLength, string fragment, IProgressMonitor progressMonitor) : base(progressMonitor)
        {
            this.maxLength = maxLength;
            originalFragment = fragment;
            parseBuffer = new char[fragment.Length + 1];
            fragment.CopyTo(0, parseBuffer, 0, fragment.Length);
            parseBuffer[fragment.Length] = '\0';
            maxTokenSize = fragment.Length;
        }

        public void Write(string str)
        {
            int num = PrepareToBuffer(str.Length);
            if (num > 0)
            {
                str.CopyTo(0, parseBuffer, maxTokenSize, num);
                maxTokenSize += num;
                parseBuffer[maxTokenSize] = '\0';
            }
        }

        public void Write(char[] buffer, int offset, int count)
        {
            count = PrepareToBuffer(count);
            if (count > 0)
            {
                Buffer.BlockCopy(buffer, offset * 2, parseBuffer, maxTokenSize * 2, count * 2);
                maxTokenSize += count;
                parseBuffer[maxTokenSize] = '\0';
            }
        }

        public void Write(int ucs32Char)
        {
            if (ucs32Char > 65535)
            {
                int num = PrepareToBuffer(2);
                if (num > 0)
                {
                    parseBuffer[maxTokenSize] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Char);
                    parseBuffer[maxTokenSize + 1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Char);
                    maxTokenSize += num;
                    parseBuffer[maxTokenSize] = '\0';
                    return;
                }
            }
            else
            {
                int num = PrepareToBuffer(1);
                if (num > 0)
                {
                    parseBuffer[maxTokenSize++] = (char)ucs32Char;
                    parseBuffer[maxTokenSize] = '\0';
                }
            }
        }

        public void Reset()
        {
            maxTokenSize = 0;
            endOfFile = false;
        }

        public void Initialize(string fragment)
        {
            if (originalFragment != fragment)
            {
                originalFragment = fragment;
                parseBuffer = new char[fragment.Length + 1];
                fragment.CopyTo(0, parseBuffer, 0, fragment.Length);
                parseBuffer[fragment.Length] = '\0';
                maxTokenSize = fragment.Length;
            }
            endOfFile = false;
        }

        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            if (buffer == null)
            {
                buffer = parseBuffer;
                start = 0;
                end = maxTokenSize;
                current = 0;
                if (end != 0)
                {
                    return true;
                }
            }
            endOfFile = true;
            return true;
        }

        public override void ReportProcessed(int processedSize)
        {
            progressMonitor.ReportProgress();
        }

        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }

        protected override void Dispose()
        {
            parseBuffer = null;
            base.Dispose();
        }

        private int PrepareToBuffer(int count)
        {
            if (maxTokenSize + count > maxLength)
            {
                count = maxLength - maxTokenSize;
            }
            if (count > 0)
            {
                if (parseBuffer == null)
                {
                    parseBuffer = new char[count + 1];
                }
                else if (parseBuffer.Length <= maxTokenSize + count)
                {
                    char[] src = parseBuffer;
                    int num = (maxTokenSize + count) * 2;
                    if (num > maxLength)
                    {
                        num = maxLength;
                    }
                    parseBuffer = new char[num + 1];
                    if (maxTokenSize > 0)
                    {
                        Buffer.BlockCopy(src, 0, parseBuffer, 0, maxTokenSize * 2);
                    }
                }
            }
            return count;
        }
    }
}
