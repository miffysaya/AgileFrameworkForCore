using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterDecodingInput : ConverterInput, IReusable
    {
        private IResultsFeedback resultFeedback;

        private Stream pullSource;

        private ConverterStream pushSource;

        private bool rawEndOfFile;

        private Encoding originalEncoding;

        private Encoding encoding;

        private Decoder decoder;

        private bool encodingChanged;

        private int minDecodeBytes;

        private int minDecodeChars;

        private char[] parseBuffer;

        private int parseStart;

        private int parseEnd;

        private int readFileOffset;

        private byte[] readBuffer;

        private int readCurrent;

        private int readEnd;

        private byte[] pushChunkBuffer;

        private int pushChunkStart;

        private int pushChunkCount;

        private int pushChunkUsed;

        private bool detectEncodingFromByteOrderMark;

        private byte[] preamble;

        private IRestartable restartConsumer;

        private int restartMax;

        private ByteCache restartCache;

        private bool restarting;

        public Encoding Encoding
        {
            get
            {
                return encoding;
            }
        }

        public bool EncodingChanged
        {
            get
            {
                return encodingChanged;
            }
            set
            {
                encodingChanged = false;
            }
        }

        public ConverterDecodingInput(Stream source, bool push, Encoding encoding, bool detectEncodingFromByteOrderMark, int maxParseToken, int restartMax, int inputBufferSize, bool testBoundaryConditions, IResultsFeedback resultFeedback, IProgressMonitor progressMonitor) : base(progressMonitor)
        {
            this.resultFeedback = resultFeedback;
            this.restartMax = restartMax;
            if (push)
            {
                pushSource = (source as ConverterStream);
            }
            else
            {
                pullSource = source;
            }
            this.detectEncodingFromByteOrderMark = detectEncodingFromByteOrderMark;
            minDecodeBytes = (testBoundaryConditions ? 1 : 64);
            originalEncoding = encoding;
            SetNewEncoding(encoding);
            maxTokenSize = ((maxParseToken == 2147483647) ? maxParseToken : (testBoundaryConditions ? maxParseToken : ((maxParseToken + 1023) / 1024 * 1024)));
            parseBuffer = new char[testBoundaryConditions ? 55L : Math.Min(4096L, (long)maxTokenSize + (long)(minDecodeChars + 1))];
            if (pushSource != null)
            {
                readBuffer = new byte[Math.Max(minDecodeBytes * 2, 8)];
                return;
            }
            int num = Math.Max(CalculateMaxBytes(parseBuffer.Length), inputBufferSize);
            readBuffer = new byte[num];
        }

        private void Reinitialize()
        {
            parseStart = 0;
            parseEnd = 0;
            rawEndOfFile = false;
            SetNewEncoding(originalEncoding);
            encodingChanged = false;
            readFileOffset = 0;
            readCurrent = 0;
            readEnd = 0;
            pushChunkBuffer = null;
            pushChunkStart = 0;
            pushChunkCount = 0;
            pushChunkUsed = 0;
            if (restartCache != null)
            {
                restartCache.Reset();
            }
            restarting = false;
            endOfFile = false;
        }

        public override void SetRestartConsumer(IRestartable restartConsumer)
        {
            if (restartMax != 0 || restartConsumer == null)
            {
                this.restartConsumer = restartConsumer;
            }
        }

        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
        {
            if (parseBuffer.Length - parseEnd <= minDecodeChars && !EnsureFreeSpace())
            {
                return true;
            }
            int num = 0;
            while ((!rawEndOfFile || readEnd - readCurrent != 0 || restarting) && parseBuffer.Length - parseEnd > minDecodeChars)
            {
                if (readEnd - readCurrent >= ((readFileOffset == 0) ? Math.Max(4, minDecodeBytes) : minDecodeBytes) || (rawEndOfFile && !restarting))
                {
                    num += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                }
                else if (restarting)
                {
                    byte[] buffer2;
                    int num2;
                    int end2;
                    if (!GetRestartChunk(out buffer2, out num2, out end2))
                    {
                        restarting = false;
                    }
                    else
                    {
                        int num3 = num2;
                        num += DecodeFromBuffer(buffer2, ref num2, end2, readFileOffset, false);
                        readFileOffset += num2 - num3;
                        ReportRestartChunkUsed(num2 - num3);
                    }
                }
                else if (pushSource != null)
                {
                    if (pushChunkCount == 0)
                    {
                        if (!pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out rawEndOfFile))
                        {
                            break;
                        }
                    }
                    else if (pushChunkCount - pushChunkUsed == 0)
                    {
                        if (restartConsumer != null)
                        {
                            BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkCount, readFileOffset, false);
                        }
                        pushSource.ReportRead(pushChunkCount);
                        readFileOffset += pushChunkCount;
                        pushChunkCount = 0;
                        pushChunkUsed = 0;
                        break;
                    }
                    if (pushChunkCount - pushChunkUsed < ((readFileOffset == 0) ? Math.Max(4, minDecodeBytes) : minDecodeBytes))
                    {
                        if (pushChunkCount - pushChunkUsed != 0)
                        {
                            if (readBuffer.Length - readEnd < pushChunkCount - pushChunkUsed)
                            {
                                if (restartConsumer != null)
                                {
                                    BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                                }
                                Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);
                                readFileOffset += readCurrent;
                                readEnd -= readCurrent;
                                readCurrent = 0;
                            }
                            if (pushChunkUsed != 0)
                            {
                                if (restartConsumer != null)
                                {
                                    BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkUsed, readFileOffset + readEnd, false);
                                }
                                readFileOffset += pushChunkUsed;
                            }
                            Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, pushChunkCount - pushChunkUsed);
                            readEnd += pushChunkCount - pushChunkUsed;
                            pushSource.ReportRead(pushChunkCount);
                            pushChunkCount = 0;
                            pushChunkUsed = 0;
                            if (readEnd - readCurrent < ((readFileOffset == 0) ? Math.Max(4, minDecodeBytes) : minDecodeBytes))
                            {
                                break;
                            }
                        }
                        num += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                    }
                    else if (readEnd - readCurrent != 0)
                    {
                        if (readFileOffset == 0 && readCurrent == 0)
                        {
                            int num4 = Math.Max(4, minDecodeBytes) - (readEnd - readCurrent);
                            Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, num4);
                            readEnd += num4;
                            pushSource.ReportRead(num4);
                            pushChunkCount -= num4;
                            pushChunkStart += num4;
                        }
                        num += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, false);
                    }
                    if (parseBuffer.Length - parseEnd > minDecodeChars && pushChunkCount - pushChunkUsed != 0 && readEnd - readCurrent == 0)
                    {
                        if (readEnd != 0)
                        {
                            if (restartConsumer != null)
                            {
                                BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                            }
                            readFileOffset += readCurrent;
                            readEnd = 0;
                            readCurrent = 0;
                        }
                        int num5 = pushChunkStart + pushChunkUsed;
                        num += DecodeFromBuffer(pushChunkBuffer, ref num5, pushChunkStart + pushChunkCount, readFileOffset + pushChunkUsed, false);
                        pushChunkUsed = num5 - pushChunkStart;
                    }
                }
                else
                {
                    if (readBuffer.Length - readEnd < minDecodeBytes)
                    {
                        if (restartConsumer != null)
                        {
                            BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                        }
                        Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);
                        readFileOffset += readCurrent;
                        readEnd -= readCurrent;
                        readCurrent = 0;
                    }
                    int num6 = pullSource.Read(readBuffer, readEnd, readBuffer.Length - readEnd);
                    if (num6 == 0)
                    {
                        rawEndOfFile = true;
                    }
                    else
                    {
                        readEnd += num6;
                        if (progressMonitor != null)
                        {
                            progressMonitor.ReportProgress();
                        }
                    }
                    num += DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                }
            }
            if (rawEndOfFile && readEnd - readCurrent == 0)
            {
                endOfFile = true;
            }
            if (buffer != parseBuffer)
            {
                buffer = parseBuffer;
            }
            if (start != parseStart)
            {
                current = parseStart + (current - start);
                start = parseStart;
            }
            end = parseEnd;
            return num != 0 || endOfFile || encodingChanged;
        }

        public override void ReportProcessed(int processedSize)
        {
            parseStart += processedSize;
        }

        public override int RemoveGap(int gapBegin, int gapEnd)
        {
            parseEnd = gapBegin;
            parseBuffer[gapBegin] = '\0';
            return gapBegin;
        }

        public bool RestartWithNewEncoding(Encoding newEncoding)
        {
            if (encoding.CodePage == newEncoding.CodePage)
            {
                if (restartConsumer != null)
                {
                    restartConsumer.DisableRestart();
                    restartConsumer = null;
                    if (restartCache != null)
                    {
                        restartCache.Reset();
                        restartCache = null;
                    }
                }
                return false;
            }
            if (restartConsumer == null || !restartConsumer.CanRestart())
            {
                return false;
            }
            restartConsumer.Restart();
            SetNewEncoding(newEncoding);
            encodingChanged = true;
            if (readEnd != 0 && readFileOffset != 0)
            {
                BackupForRestart(readBuffer, 0, readEnd, readFileOffset, true);
                readEnd = 0;
                readFileOffset = 0;
            }
            readCurrent = 0;
            pushChunkUsed = 0;
            restartConsumer = null;
            parseStart = (parseEnd = 0);
            restarting = (restartCache != null && restartCache.Length != 0);
            return true;
        }

        private void SetNewEncoding(Encoding newEncoding)
        {
            encoding = newEncoding;
            decoder = encoding.GetDecoder();
            preamble = encoding.GetPreamble();
            minDecodeChars = GetMaxCharCount(minDecodeBytes);
            if (resultFeedback != null)
            {
                resultFeedback.Set(ConfigParameter.InputEncoding, newEncoding);
            }
        }

        protected override void Dispose()
        {
            if (restartCache != null && restartCache is IDisposable)
            {
                ((IDisposable)restartCache).Dispose();
            }
            restartCache = null;
            pullSource = null;
            pushSource = null;
            parseBuffer = null;
            readBuffer = null;
            pushChunkBuffer = null;
            preamble = null;
            restartConsumer = null;
            base.Dispose();
        }

        private int DecodeFromBuffer(byte[] buffer, ref int start, int end, int fileOffset, bool flush)
        {
            int num = 0;
            if (fileOffset == 0)
            {
                if (detectEncodingFromByteOrderMark)
                {
                    DetectEncoding(buffer, start, end);
                }
                if (preamble.Length != 0 && end - start >= preamble.Length)
                {
                    int num2 = 0;
                    while (num2 < preamble.Length && preamble[num2] == buffer[start + num2])
                    {
                        num2++;
                    }
                    if (num2 == preamble.Length)
                    {
                        start += preamble.Length;
                        num = preamble.Length;
                        if (restartConsumer != null)
                        {
                            restartConsumer.DisableRestart();
                            restartConsumer = null;
                        }
                    }
                }
                encodingChanged = true;
                preamble = null;
            }
            int num3 = end - start;
            if (GetMaxCharCount(num3) >= parseBuffer.Length - parseEnd)
            {
                num3 = CalculateMaxBytes(parseBuffer.Length - parseEnd - 1);
            }
            int chars = decoder.GetChars(buffer, start, num3, parseBuffer, parseEnd);
            parseEnd += chars;
            parseBuffer[parseEnd] = '\0';
            start += num3;
            return num3 + num;
        }

        private bool EnsureFreeSpace()
        {
            if (parseBuffer.Length - (parseEnd - parseStart) < minDecodeChars + 1 || (parseStart < minDecodeChars && (long)parseBuffer.Length < (long)maxTokenSize + (long)(minDecodeChars + 1)))
            {
                if ((long)parseBuffer.Length >= (long)maxTokenSize + (long)(minDecodeChars + 1))
                {
                    return false;
                }
                long num = (long)(parseBuffer.Length * 2);
                if (num > (long)maxTokenSize + (long)(minDecodeChars + 1))
                {
                    num = (long)maxTokenSize + (long)(minDecodeChars + 1);
                }
                if (num > 2147483647L)
                {
                    num = 2147483647L;
                }
                if (num - (long)(parseEnd - parseStart) < (long)(minDecodeChars + 1))
                {
                    return false;
                }
                char[] dst;
                try
                {
                    dst = new char[(int)num];
                }
                catch (OutOfMemoryException innerException)
                {
                    throw new TextConvertersException(TextConvertersStrings.TagTooLong, innerException);
                }
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

        private int GetMaxCharCount(int byteCount)
        {
            if (encoding.CodePage == 65001)
            {
                return byteCount + 1;
            }
            if (encoding.CodePage == 54936)
            {
                return byteCount + 3;
            }
            return encoding.GetMaxCharCount(byteCount);
        }

        private int CalculateMaxBytes(int charCount)
        {
            if (charCount == GetMaxCharCount(charCount))
            {
                return charCount;
            }
            if (charCount == GetMaxCharCount(charCount - 1))
            {
                return charCount - 1;
            }
            if (charCount == GetMaxCharCount(charCount - 3))
            {
                return charCount - 3;
            }
            int num = charCount - 4;
            int maxCharCount = GetMaxCharCount(num);
            int num2 = (int)((float)num * (float)charCount / (float)maxCharCount);
            while (GetMaxCharCount(num2) < charCount)
            {
                num2++;
            }
            do
            {
                num2--;
            }
            while (GetMaxCharCount(num2) > charCount);
            return num2;
        }

        private void DetectEncoding(byte[] buffer, int start, int end)
        {
            if (end - start < 2)
            {
                return;
            }
            Encoding encoding = null;
            if (buffer[start] == 254 && buffer[start + 1] == 255)
            {
                encoding = Encoding.BigEndianUnicode;
            }
            else if (buffer[start] == 255 && buffer[start + 1] == 254)
            {
                if (end - start >= 4 && buffer[start + 2] == 0 && buffer[start + 3] == 0)
                {
                    encoding = Encoding.UTF32;
                }
                else
                {
                    encoding = Encoding.Unicode;
                }
            }
            else if (end - start >= 3 && buffer[start] == 239 && buffer[start + 1] == 187 && buffer[start + 2] == 191)
            {
                encoding = Encoding.UTF8;
            }
            else if (end - start >= 4 && buffer[start] == 0 && buffer[start + 1] == 0 && buffer[start + 2] == 254 && buffer[start + 3] == 255)
            {
                encoding = new UTF32Encoding(true, true);
            }
            if (encoding != null)
            {
                this.encoding = encoding;
                decoder = this.encoding.GetDecoder();
                preamble = this.encoding.GetPreamble();
                minDecodeChars = GetMaxCharCount(minDecodeBytes);
                if (restartConsumer != null)
                {
                    restartConsumer.DisableRestart();
                    restartConsumer = null;
                }
            }
        }

        private void BackupForRestart(byte[] buffer, int offset, int count, int fileOffset, bool force)
        {
            if (!force && fileOffset > restartMax)
            {
                restartConsumer.DisableRestart();
                restartConsumer = null;
                preamble = null;
                return;
            }
            if (restartCache == null)
            {
                restartCache = new ByteCache();
            }
            byte[] dst;
            int dstOffset;
            restartCache.GetBuffer(count, out dst, out dstOffset);
            Buffer.BlockCopy(buffer, offset, dst, dstOffset, count);
            restartCache.Commit(count);
        }

        private bool GetRestartChunk(out byte[] restartChunk, out int restartStart, out int restartEnd)
        {
            if (restartCache.Length == 0)
            {
                restartChunk = null;
                restartStart = 0;
                restartEnd = 0;
                return false;
            }
            int num;
            restartCache.GetData(out restartChunk, out restartStart, out num);
            restartEnd = restartStart + num;
            return true;
        }

        private void ReportRestartChunkUsed(int count)
        {
            restartCache.ReportRead(count);
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            if (pullSource != null && newSourceOrDestination != null)
            {
                Stream stream = newSourceOrDestination as Stream;
                if (stream == null || !stream.CanRead)
                {
                    throw new InvalidOperationException("cannot reinitialize this converter - new input should be a readable Stream object");
                }
                pullSource = stream;
            }
            Reinitialize();
        }
    }
}
