using AgileFramework.Security.Exchange.Data.Globalization;
using System;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterEncodingOutput : ConverterOutput, IByteSource, IRestartable, IReusable
    {
        private const int LineSpaceThreshold = 256;

        private const int SpaceThreshold = 32;

        protected IResultsFeedback resultFeedback;

        private Stream pushSink;

        private ConverterStream pullSink;

        private bool endOfFile;

        private bool restartablePushSink;

        private long restartPosition;

        private bool encodingSameAsInput;

        private bool restartable;

        private bool canRestart;

        private bool lineModeEncoding;

        private int minCharsEncode;

        private char[] lineBuffer;

        private int lineBufferCount;

        private int lineBufferLastNL;

        private ByteCache cache = new ByteCache();

        private Encoding originalEncoding;

        private Encoding encoding;

        private Encoder encoder;

        private bool encodingCompleteUnicode;

        private CodePageMap codePageMap = new CodePageMap();

        private bool isFirstChar = true;

        public Encoding Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                if (encoding != value)
                {
                    ChangeEncoding(value);
                    if (resultFeedback != null)
                    {
                        resultFeedback.Set(ConfigParameter.OutputEncoding, encoding);
                    }
                }
            }
        }

        public bool CodePageSameAsInput
        {
            get
            {
                return encodingSameAsInput;
            }
        }

        public override bool CanAcceptMore
        {
            get
            {
                return canRestart || pullSink == null || cache.Length == 0;
            }
        }

        public ConverterEncodingOutput(Stream destination, bool push, bool restartable, Encoding encoding, bool encodingSameAsInput, bool testBoundaryConditions, IResultsFeedback resultFeedback)
        {
            this.resultFeedback = resultFeedback;
            if (!push)
            {
                pullSink = (destination as ConverterStream);
                pullSink.SetSource(this);
            }
            else
            {
                pushSink = destination;
                if (restartable && destination.CanSeek && destination.Position == destination.Length)
                {
                    restartablePushSink = true;
                    restartPosition = destination.Position;
                }
            }
            canRestart = restartable;
            this.restartable = restartable;
            lineBuffer = new char[4096];
            minCharsEncode = (testBoundaryConditions ? 1 : 256);
            this.encodingSameAsInput = encodingSameAsInput;
            originalEncoding = encoding;
            ChangeEncoding(encoding);
            if (this.resultFeedback != null)
            {
                this.resultFeedback.Set(ConfigParameter.OutputEncoding, this.encoding);
            }
        }

        private void Reinitialize()
        {
            endOfFile = false;
            lineBufferCount = 0;
            lineBufferLastNL = 0;
            isFirstChar = true;
            cache.Reset();
            encoding = null;
            ChangeEncoding(originalEncoding);
            canRestart = restartable;
        }

        bool IRestartable.CanRestart()
        {
            return canRestart;
        }

        void IRestartable.Restart()
        {
            if (pullSink == null && restartablePushSink)
            {
                pushSink.Position = restartPosition;
                pushSink.SetLength(restartPosition);
            }
            Reinitialize();
            canRestart = false;
        }

        void IRestartable.DisableRestart()
        {
            canRestart = false;
            FlushCached();
        }

        void IReusable.Initialize(object newSourceOrDestination)
        {
            restartablePushSink = false;
            if (pushSink != null && newSourceOrDestination != null)
            {
                Stream stream = newSourceOrDestination as Stream;
                if (stream == null || !stream.CanWrite)
                {
                    throw new InvalidOperationException("cannot reinitialize this converter - new output should be a writable Stream object");
                }
                pushSink = stream;
                if (restartable && stream.CanSeek && stream.Position == stream.Length)
                {
                    restartablePushSink = true;
                    restartPosition = stream.Position;
                }
            }
            Reinitialize();
        }

        public override void Write(char[] buffer, int offset, int count, IFallback fallback)
        {
            if (fallback == null && !lineModeEncoding && lineBufferCount + count <= lineBuffer.Length - minCharsEncode)
            {
                if (count == 1)
                {
                    lineBuffer[lineBufferCount++] = buffer[offset];
                    return;
                }
                if (count < 16)
                {
                    if ((count & 8) != 0)
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBuffer[lineBufferCount + 2] = buffer[offset + 2];
                        lineBuffer[lineBufferCount + 3] = buffer[offset + 3];
                        lineBuffer[lineBufferCount + 4] = buffer[offset + 4];
                        lineBuffer[lineBufferCount + 5] = buffer[offset + 5];
                        lineBuffer[lineBufferCount + 6] = buffer[offset + 6];
                        lineBuffer[lineBufferCount + 7] = buffer[offset + 7];
                        lineBufferCount += 8;
                        offset += 8;
                    }
                    if ((count & 4) != 0)
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBuffer[lineBufferCount + 2] = buffer[offset + 2];
                        lineBuffer[lineBufferCount + 3] = buffer[offset + 3];
                        lineBufferCount += 4;
                        offset += 4;
                    }
                    if ((count & 2) != 0)
                    {
                        lineBuffer[lineBufferCount] = buffer[offset];
                        lineBuffer[lineBufferCount + 1] = buffer[offset + 1];
                        lineBufferCount += 2;
                        offset += 2;
                    }
                    if ((count & 1) != 0)
                    {
                        lineBuffer[lineBufferCount++] = buffer[offset];
                    }
                    return;
                }
            }
            WriteComplete(buffer, offset, count, fallback);
        }

        public void WriteComplete(char[] buffer, int offset, int count, IFallback fallback)
        {
            if (fallback != null || lineModeEncoding)
            {
                byte b = 0;
                byte[] array = null;
                uint num = 0u;
                bool flag = false;
                bool flag2 = false;
                if (fallback != null)
                {
                    array = fallback.GetUnsafeAsciiMap(out b);
                    if (array != null)
                    {
                        num = (uint)array.Length;
                    }
                    flag = fallback.HasUnsafeUnicode();
                    flag2 = fallback.TreatNonAsciiAsUnsafe(encoding.WebName);
                }
                while (count != 0)
                {
                    while (count != 0 && lineBufferCount != lineBuffer.Length)
                    {
                        char c = buffer[offset];
                        if (fallback != null && (((uint)c < num && (array[(int)c] & b) != 0) || (!encodingCompleteUnicode && (c >= '\u007f' || c < ' ') && codePageMap.IsUnsafeExtendedCharacter(c)) || (flag && c >= '\u007f' && (flag2 || fallback.IsUnsafeUnicode(c, isFirstChar)))))
                        {
                            if (!fallback.FallBackChar(c, lineBuffer, ref lineBufferCount, lineBuffer.Length))
                            {
                                break;
                            }
                            isFirstChar = false;
                        }
                        else
                        {
                            lineBuffer[lineBufferCount++] = c;
                            isFirstChar = false;
                            if (lineModeEncoding)
                            {
                                if (c == '\n' || c == '\r')
                                {
                                    lineBufferLastNL = lineBufferCount;
                                }
                                else if (lineBufferLastNL > lineBuffer.Length - 256)
                                {
                                    count--;
                                    offset++;
                                    break;
                                }
                            }
                        }
                        count--;
                        offset++;
                    }
                    if (lineModeEncoding && (lineBufferLastNL > lineBuffer.Length - 256 || (lineBufferCount > lineBuffer.Length - 32 && lineBufferLastNL != 0)))
                    {
                        EncodeBuffer(lineBuffer, 0, lineBufferLastNL, false);
                        lineBufferCount -= lineBufferLastNL;
                        if (lineBufferCount != 0)
                        {
                            Buffer.BlockCopy(lineBuffer, lineBufferLastNL * 2, lineBuffer, 0, lineBufferCount * 2);
                        }
                    }
                    else if (lineBufferCount > lineBuffer.Length - Math.Max(minCharsEncode, 32))
                    {
                        EncodeBuffer(lineBuffer, 0, lineBufferCount, false);
                        lineBufferCount = 0;
                    }
                    lineBufferLastNL = 0;
                }
                return;
            }
            if (count > minCharsEncode)
            {
                if (lineBufferCount != 0)
                {
                    EncodeBuffer(lineBuffer, 0, lineBufferCount, false);
                    lineBufferCount = 0;
                    lineBufferLastNL = 0;
                }
                EncodeBuffer(buffer, offset, count, false);
                return;
            }
            Buffer.BlockCopy(buffer, offset * 2, lineBuffer, lineBufferCount * 2, count * 2);
            lineBufferCount += count;
            if (lineBufferCount > lineBuffer.Length - minCharsEncode)
            {
                EncodeBuffer(lineBuffer, 0, lineBufferCount, false);
                lineBufferCount = 0;
                lineBufferLastNL = 0;
            }
        }

        public override void Write(string text)
        {
            if (text.Length == 0)
            {
                return;
            }
            if (lineModeEncoding || lineBufferCount + text.Length > lineBuffer.Length - minCharsEncode)
            {
                base.Write(text, 0, text.Length);
                return;
            }
            if (text.Length <= 4)
            {
                int num = text.Length;
                lineBuffer[lineBufferCount++] = text[0];
                if (--num != 0)
                {
                    lineBuffer[lineBufferCount++] = text[1];
                    if (--num != 0)
                    {
                        lineBuffer[lineBufferCount++] = text[2];
                        if (num - 1 != 0)
                        {
                            lineBuffer[lineBufferCount++] = text[3];
                            return;
                        }
                    }
                }
            }
            else
            {
                text.CopyTo(0, lineBuffer, lineBufferCount, text.Length);
                lineBufferCount += text.Length;
            }
        }

        public override void Flush()
        {
            if (endOfFile)
            {
                return;
            }
            canRestart = false;
            FlushCached();
            EncodeBuffer(lineBuffer, 0, lineBufferCount, true);
            lineBufferCount = 0;
            lineBufferLastNL = 0;
            if (pullSink == null)
            {
                pushSink.Flush();
            }
            else if (cache.Length == 0)
            {
                pullSink.ReportEndOfFile();
            }
            endOfFile = true;
        }

        bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength)
        {
            if (cache.Length == 0 || canRestart)
            {
                chunkBuffer = null;
                chunkOffset = 0;
                chunkLength = 0;
                return false;
            }
            cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
            return true;
        }

        void IByteSource.ReportOutput(int readCount)
        {
            cache.ReportRead(readCount);
            if (cache.Length == 0 && endOfFile)
            {
                pullSink.ReportEndOfFile();
            }
        }

        protected override void Dispose()
        {
            if (cache != null && cache is IDisposable)
            {
                ((IDisposable)cache).Dispose();
            }
            cache = null;
            pushSink = null;
            pullSink = null;
            lineBuffer = null;
            encoding = null;
            encoder = null;
            codePageMap = null;
            base.Dispose();
        }

        private void EncodeBuffer(char[] buffer, int offset, int count, bool flush)
        {
            int maxByteCount = encoding.GetMaxByteCount(count);
            byte[] array = null;
            int num = 0;
            int num2 = 0;
            bool flag = true;
            byte[] array2;
            int num3;
            if (canRestart || pullSink == null || cache.Length != 0)
            {
                cache.GetBuffer(maxByteCount, out array2, out num3);
            }
            else
            {
                pullSink.GetOutputBuffer(out array, out num, out num2);
                if (num2 >= maxByteCount)
                {
                    array2 = array;
                    num3 = num;
                    flag = false;
                }
                else
                {
                    cache.GetBuffer(maxByteCount, out array2, out num3);
                }
            }
            int num4 = encoder.GetBytes(buffer, offset, count, array2, num3, flush);
            if (flag)
            {
                cache.Commit(num4);
                if (pullSink == null)
                {
                    if (canRestart)
                    {
                        if (!restartablePushSink)
                        {
                            return;
                        }
                    }
                    while (cache.Length != 0)
                    {
                        int count2;
                        cache.GetData(out array2, out num3, out count2);
                        pushSink.Write(array2, num3, count2);
                        cache.ReportRead(count2);
                    }
                    return;
                }
                if (!canRestart)
                {
                    num4 = cache.Read(array, num, num2);
                    pullSink.ReportOutput(num4);
                    return;
                }
            }
            else
            {
                pullSink.ReportOutput(num4);
            }
        }

        internal void ChangeEncoding(Encoding newEncoding)
        {
            if (encoding != null)
            {
                EncodeBuffer(lineBuffer, 0, lineBufferCount, true);
                lineBufferCount = 0;
                lineBufferLastNL = 0;
            }
            encoding = newEncoding;
            encoder = newEncoding.GetEncoder();
            int codePage = newEncoding.CodePage;
            if (codePage == 1200 || codePage == 1201 || codePage == 12000 || codePage == 12001 || codePage == 65000 || codePage == 65001 || codePage == 65005 || codePage == 65006 || codePage == 54936)
            {
                lineModeEncoding = false;
                encodingCompleteUnicode = true;
                codePageMap.ChoseCodePage(1200);
                return;
            }
            encodingCompleteUnicode = false;
            codePageMap.ChoseCodePage(codePage);
            if (codePage == 50220 || codePage == 50221 || codePage == 50222 || codePage == 50225 || codePage == 50227 || codePage == 50229 || codePage == 52936)
            {
                lineModeEncoding = true;
            }
        }

        private bool FlushCached()
        {
            if (canRestart || cache.Length == 0)
            {
                return false;
            }
            if (pullSink == null)
            {
                while (cache.Length != 0)
                {
                    byte[] buffer;
                    int offset;
                    int num;
                    cache.GetData(out buffer, out offset, out num);
                    pushSink.Write(buffer, offset, num);
                    cache.ReportRead(num);
                }
            }
            else
            {
                byte[] buffer;
                int offset;
                int count;
                pullSink.GetOutputBuffer(out buffer, out offset, out count);
                int num = cache.Read(buffer, offset, count);
                pullSink.ReportOutput(num);
            }
            return true;
        }
    }
}
