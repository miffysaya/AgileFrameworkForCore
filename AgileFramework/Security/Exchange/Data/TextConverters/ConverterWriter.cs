using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class ConverterWriter : TextWriter, IProgressMonitor
    {
        private ConverterUnicodeInput sinkInputObject;

        private IProducerConsumer consumer;

        private bool madeProgress;

        //private int maxLoopsWithoutProgress;

        private char[] chunkToReadBuffer;

        private int chunkToReadIndex;

        private int chunkToReadCount;

        private object destination;

        private bool endOfFile;

        private bool inconsistentState;

        //private bool boundaryTesting;

        public override Encoding Encoding
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.ConverterWriter" /> class.
        /// </summary>
        private ConverterWriter()
        {
        }

        public override void Flush()
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }
            endOfFile = true;
            if (!inconsistentState)
            {
                long num = 0L;
                inconsistentState = true;
                while (!consumer.Flush())
                {
                    if (madeProgress)
                    {
                        num = 0L;
                        madeProgress = false;
                    }
                    else
                    {
                        long arg_4F_0 = 0;
                        long expr_4A = num;
                        num = expr_4A + 1L;
                        if (arg_4F_0 == expr_4A)
                        {
                            throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToFlushConverter);
                        }
                    }
                }
                inconsistentState = false;
            }
            if (destination is Stream)
            {
                ((Stream)destination).Flush();
                return;
            }
            ((TextWriter)destination).Flush();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (index < 0 || index > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("index", TextConvertersStrings.IndexOutOfRange);
            }
            if (count < 0 || count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
            }
            int parseCount = 10000;
            //if (!boundaryTesting)
            {
                char[] dst;
                int num;
                int num2;
                sinkInputObject.GetInputBuffer(out dst, out num, out num2, out parseCount);
                if (num2 >= count)
                {
                    Buffer.BlockCopy(buffer, index * 2, dst, num * 2, count * 2);
                    sinkInputObject.Commit(count);
                    return;
                }
            }
            WriteBig(buffer, index, count, parseCount);
        }

        public override void Write(string value)
        {
            if (destination == null)
            {
                throw new ObjectDisposedException("ConverterWriter");
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterWriterInInconsistentStare);
            }
            if (value == null)
            {
                return;
            }
            int parseCount = 10000;
            //if (!boundaryTesting)
            {
                char[] array;
                int destinationIndex;
                int num;
                sinkInputObject.GetInputBuffer(out array, out destinationIndex, out num, out parseCount);
                if (num >= value.Length)
                {
                    value.CopyTo(0, array, destinationIndex, value.Length);
                    sinkInputObject.Commit(value.Length);
                    return;
                }
            }
            char[] buffer = value.ToCharArray();
            WriteBig(buffer, 0, value.Length, parseCount);
        }

        public override void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        internal void SetSink(ConverterUnicodeInput sinkInputObject)
        {
            this.sinkInputObject = sinkInputObject;
        }

        internal bool GetInputChunk(out char[] chunkBuffer, out int chunkIndex, out int chunkCount, out bool eof)
        {
            chunkBuffer = chunkToReadBuffer;
            chunkIndex = chunkToReadIndex;
            chunkCount = chunkToReadCount;
            eof = (endOfFile && 0 == chunkToReadCount);
            return chunkToReadCount != 0 || endOfFile;
        }

        internal void ReportRead(int readCount)
        {
            if (readCount != 0)
            {
                chunkToReadCount -= readCount;
                chunkToReadIndex += readCount;
                if (chunkToReadCount == 0)
                {
                    chunkToReadBuffer = null;
                    chunkToReadIndex = 0;
                }
                madeProgress = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && destination != null)
            {
                if (!inconsistentState)
                {
                    Flush();
                }
                if (destination is Stream)
                {
                    ((Stream)destination).Close();
                }
                else
                {
                    ((TextWriter)destination).Close();
                }
            }
            if (consumer != null && consumer is IDisposable)
            {
                ((IDisposable)consumer).Dispose();
            }
            destination = null;
            consumer = null;
            sinkInputObject = null;
            chunkToReadBuffer = null;
            base.Dispose(disposing);
        }

        private void WriteBig(char[] buffer, int index, int count, int parseCount)
        {
            chunkToReadBuffer = buffer;
            chunkToReadIndex = index;
            chunkToReadCount = count;
            long num = 0L;
            inconsistentState = true;
            while (chunkToReadCount != 0)
            {
                consumer.Run();
                if (madeProgress)
                {
                    madeProgress = false;
                    num = 0L;
                }
                else
                {
                    long arg_4D_0 = 0;
                    long expr_48 = num;
                    num = expr_48 + 1L;
                    if (arg_4D_0 == expr_48)
                    {
                        throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProcessInput);
                    }
                }
            }
            inconsistentState = false;
        }

        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }
    }
}
