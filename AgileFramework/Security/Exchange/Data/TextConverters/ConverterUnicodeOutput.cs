using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterUnicodeOutput : ConverterOutput, IRestartable, IReusable, IDisposable
    {
        private const int FallbackExpansionMax = 16;

        private TextWriter pushSink;

        private ConverterReader pullSink;

        private bool endOfFile;

        private bool restartable;

        private bool canRestart;

        private bool isFirstChar = true;

        private TextCache cache = new TextCache();

        public override bool CanAcceptMore
        {
            get
            {
                return canRestart || pullSink == null || cache.Length == 0;
            }
        }

        public ConverterUnicodeOutput(object destination, bool push, bool restartable)
        {
            if (push)
            {
                pushSink = (destination as TextWriter);
            }
            else
            {
                pullSink = (destination as ConverterReader);
                pullSink.SetSource(this);
            }
            canRestart = restartable;
            this.restartable = restartable;
        }

        private void Reinitialize()
        {
            endOfFile = false;
            cache.Reset();
            canRestart = restartable;
            isFirstChar = true;
        }

        bool IRestartable.CanRestart()
        {
            return canRestart;
        }

        void IRestartable.Restart()
        {
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
            if (pushSink != null && newSourceOrDestination != null)
            {
                TextWriter textWriter = newSourceOrDestination as TextWriter;
                pushSink = textWriter ?? throw new InvalidOperationException("cannot reinitialize this converter - new output should be a TextWriter object");
            }
            Reinitialize();
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
            base.Dispose();
        }

        public override void Write(char[] buffer, int offset, int count, IFallback fallback)
        {
            byte unsafeAsciiMask = 0;
            byte[] unsafeAsciiMap = (fallback == null) ? null : fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
            bool hasUnsafeUnicode = fallback != null && fallback.HasUnsafeUnicode();
            if (cache.Length == 0)
            {
                if (!canRestart)
                {
                    if (pullSink != null)
                    {
                        char[] array;
                        int num;
                        int num2;
                        pullSink.GetOutputBuffer(out array, out num, out num2);
                        if (num2 != 0)
                        {
                            if (fallback != null)
                            {
                                int num3 = num;
                                while (count != 0 && num2 != 0)
                                {
                                    char c = buffer[offset];
                                    if (IsUnsafeCharacter(c, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                                    {
                                        int num4 = num;
                                        if (!fallback.FallBackChar(c, array, ref num, num + num2))
                                        {
                                            break;
                                        }
                                        num2 -= num - num4;
                                    }
                                    else
                                    {
                                        array[num++] = c;
                                        num2--;
                                    }
                                    isFirstChar = false;
                                    count--;
                                    offset++;
                                }
                                pullSink.ReportOutput(num - num3);
                            }
                            else
                            {
                                int num5 = Math.Min(num2, count);
                                Buffer.BlockCopy(buffer, offset * 2, array, num * 2, num5 * 2);
                                isFirstChar = false;
                                count -= num5;
                                offset += num5;
                                pullSink.ReportOutput(num5);
                                num += num5;
                                num2 -= num5;
                            }
                        }
                        while (count != 0)
                        {
                            if (fallback != null)
                            {
                                char[] array2;
                                int num6;
                                int num7;
                                cache.GetBuffer(16, out array2, out num6, out num7);
                                int num8 = num6;
                                while (count != 0 && num7 != 0)
                                {
                                    char c2 = buffer[offset];
                                    if (IsUnsafeCharacter(c2, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                                    {
                                        int num9 = num6;
                                        if (!fallback.FallBackChar(c2, array2, ref num6, num6 + num7))
                                        {
                                            break;
                                        }
                                        num7 -= num6 - num9;
                                    }
                                    else
                                    {
                                        array2[num6++] = c2;
                                        num7--;
                                    }
                                    isFirstChar = false;
                                    count--;
                                    offset++;
                                }
                                cache.Commit(num6 - num8);
                            }
                            else
                            {
                                int size = Math.Min(count, 256);
                                char[] array2;
                                int num6;
                                int num7;
                                cache.GetBuffer(size, out array2, out num6, out num7);
                                int num10 = Math.Min(num7, count);
                                Buffer.BlockCopy(buffer, offset * 2, array2, num6 * 2, num10 * 2);
                                isFirstChar = false;
                                cache.Commit(num10);
                                offset += num10;
                                count -= num10;
                            }
                        }
                        while (num2 != 0)
                        {
                            if (cache.Length == 0)
                            {
                                return;
                            }
                            char[] src;
                            int num11;
                            int val;
                            cache.GetData(out src, out num11, out val);
                            int num12 = Math.Min(val, num2);
                            Buffer.BlockCopy(src, num11 * 2, array, num * 2, num12 * 2);
                            cache.ReportRead(num12);
                            pullSink.ReportOutput(num12);
                            num += num12;
                            num2 -= num12;
                        }
                    }
                    else
                    {
                        if (fallback != null)
                        {
                            char[] array3;
                            int num13;
                            int num14;
                            cache.GetBuffer(1024, out array3, out num13, out num14);
                            int num15 = num13;
                            int num16 = num14;
                            while (count != 0)
                            {
                                while (count != 0 && num14 != 0)
                                {
                                    char c3 = buffer[offset];
                                    if (IsUnsafeCharacter(c3, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                                    {
                                        int num17 = num13;
                                        if (!fallback.FallBackChar(c3, array3, ref num13, num13 + num14))
                                        {
                                            break;
                                        }
                                        num14 -= num13 - num17;
                                    }
                                    else
                                    {
                                        array3[num13++] = c3;
                                        num14--;
                                    }
                                    isFirstChar = false;
                                    count--;
                                    offset++;
                                }
                                if (num13 - num15 != 0)
                                {
                                    pushSink.Write(array3, num15, num13 - num15);
                                    num13 = num15;
                                    num14 = num16;
                                }
                            }
                            return;
                        }
                        if (count != 0)
                        {
                            pushSink.Write(buffer, offset, count);
                            isFirstChar = false;
                        }
                    }
                    return;
                }
            }
            while (count != 0)
            {
                if (fallback != null)
                {
                    char[] array4;
                    int num18;
                    int num19;
                    cache.GetBuffer(16, out array4, out num18, out num19);
                    int num20 = num18;
                    while (count != 0 && num19 != 0)
                    {
                        char c4 = buffer[offset];
                        if (IsUnsafeCharacter(c4, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, isFirstChar, fallback))
                        {
                            int num21 = num18;
                            if (!fallback.FallBackChar(c4, array4, ref num18, num18 + num19))
                            {
                                break;
                            }
                            num19 -= num18 - num21;
                        }
                        else
                        {
                            array4[num18++] = c4;
                            num19--;
                        }
                        isFirstChar = false;
                        count--;
                        offset++;
                    }
                    cache.Commit(num18 - num20);
                }
                else
                {
                    int size2 = Math.Min(count, 256);
                    char[] array4;
                    int num18;
                    int num19;
                    cache.GetBuffer(size2, out array4, out num18, out num19);
                    int num22 = Math.Min(num19, count);
                    Buffer.BlockCopy(buffer, offset * 2, array4, num18 * 2, num22 * 2);
                    isFirstChar = false;
                    cache.Commit(num22);
                    offset += num22;
                    count -= num22;
                }
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

        public bool GetOutputChunk(out char[] chunkBuffer, out int chunkOffset, out int chunkLength)
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

        public void ReportOutput(int readCount)
        {
            cache.ReportRead(readCount);
            if (cache.Length == 0 && endOfFile)
            {
                pullSink.ReportEndOfFile();
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
                    char[] buffer;
                    int num;
                    int num2;
                    cache.GetData(out buffer, out num, out num2);
                    pushSink.Write(buffer, num, num2);
                    cache.ReportRead(num2);
                }
            }
            else
            {
                char[] buffer;
                int num;
                int count;
                pullSink.GetOutputBuffer(out buffer, out num, out count);
                int num2 = cache.Read(buffer, num, count);
                pullSink.ReportOutput(num2);
                if (cache.Length == 0 && endOfFile)
                {
                    pullSink.ReportEndOfFile();
                }
            }
            return true;
        }

        private static bool IsUnsafeCharacter(char ch, byte[] unsafeAsciiMap, byte unsafeAsciiMask, bool hasUnsafeUnicode, bool isFirstChar, IFallback fallback)
        {
            return unsafeAsciiMap != null && (ch < unsafeAsciiMap.Length && (unsafeAsciiMap[ch] & unsafeAsciiMask) != 0 || (hasUnsafeUnicode && ch >= '\u007f' && fallback.IsUnsafeUnicode(ch, isFirstChar)));
        }
    }
}
