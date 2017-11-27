using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// A stream class used for conversion.
    /// </summary>
    internal class ConverterStream : Stream, IProgressMonitor
    {
        /// <summary>
        /// The number of conversion loops to attempt without any progress before the conversion is cancelled.
        /// </summary>
        private readonly int maxLoopsWithoutProgress;

        /// <summary>
        /// The conversion consumer.
        /// </summary>
        private IProducerConsumer consumer;

        /// <summary>
        /// Value indicating if any progress has been made during conversion.
        /// </summary>
        private bool madeProgress;

        /// <summary>
        /// The read buffer
        /// </summary>
        private byte[] chunkToReadBuffer;

        /// <summary>
        /// The offset to start reading from
        /// </summary>
        private int chunkToReadOffset;

        /// <summary>
        /// The number of bytes read.
        /// </summary>
        private int chunkToReadCount;

        /// <summary>
        /// The data source to be converted.
        /// </summary>
        private IByteSource byteSource;

        /// <summary>
        /// The conversion producer.
        /// </summary>
        private IProducerConsumer producer;

        /// <summary>
        /// The write buffer.
        /// </summary>
        private byte[] writeBuffer;

        /// <summary>
        /// The offset to write from.
        /// </summary>
        private int writeOffset;

        /// <summary>
        /// The number of characters written.
        /// </summary>
        private int writeCount;

        /// <summary>
        /// The object source or destination.
        /// </summary>
        private object sourceOrDestination;

        /// <summary>
        /// Value indicating if the end of the file has been reached.
        /// </summary>
        private bool endOfFile;

        /// <summary>
        /// Value indicating if the conversion is in an inconsitent state.
        /// </summary>
        private bool inconsistentState;

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.
        /// </returns>
        public override bool CanRead
        {
            get
            {
                return producer != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.
        /// </returns>
        public override bool CanWrite
        {
            get
            {
                return consumer != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.
        /// </returns>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// A class derived from Stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
            }
            set
            {
                throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.ConverterStream" /> class.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="converter">The converter to use.</param>
        public ConverterStream(TextReader sourceReader, TextConverter converter)
        {
            if (sourceReader == null)
            {
                throw new ArgumentNullException("sourceReader");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            producer = converter.CreatePullChain(sourceReader, this);
            sourceOrDestination = sourceReader;
            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(TextConvertersStrings.SeekUnsupported);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }
            if (consumer == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.WriteUnsupported);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
            }
            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
            }
            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(TextConvertersStrings.WriteAfterFlush);
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterStreamInInconsistentStare);
            }
            chunkToReadBuffer = buffer;
            chunkToReadOffset = offset;
            chunkToReadCount = count;
            long num = 0L;
            inconsistentState = true;
            while (chunkToReadCount != 0)
            {
                consumer.Run();
                if (madeProgress)
                {
                    num = 0L;
                    madeProgress = false;
                }
                else
                {
                    long arg_F3_0 = maxLoopsWithoutProgress;
                    long expr_EE = num;
                    num = expr_EE + 1L;
                    if (arg_F3_0 == expr_EE)
                    {
                        throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProcessInput);
                    }
                }
            }
            inconsistentState = false;
            chunkToReadBuffer = null;
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// Methods were called on a read-only stream.
        /// </exception>
        /// <exception cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
        public override void Flush()
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }
            if (consumer == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.WriteUnsupported);
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
                        long arg_62_0 = maxLoopsWithoutProgress;
                        long expr_5D = num;
                        num = expr_5D + 1L;
                        if (arg_62_0 == expr_5D)
                        {
                            throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToFlushConverter);
                        }
                    }
                }
                inconsistentState = false;
            }
            if (sourceOrDestination is Stream)
            {
                ((Stream)sourceOrDestination).Flush();
                return;
            }
            if (sourceOrDestination is TextWriter)
            {
                ((TextWriter)sourceOrDestination).Flush();
            }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            try
            {
                if (sourceOrDestination != null && consumer != null && !inconsistentState)
                {
                    Flush();
                }
                if (producer != null && producer is IDisposable)
                {
                    ((IDisposable)producer).Dispose();
                }
                if (consumer != null && consumer is IDisposable)
                {
                    ((IDisposable)consumer).Dispose();
                }
            }
            finally
            {
                if (sourceOrDestination != null)
                {
                    if (sourceOrDestination is Stream)
                    {
                        ((Stream)sourceOrDestination).Close();
                    }
                    else if (sourceOrDestination is TextReader)
                    {
                        ((TextReader)sourceOrDestination).Close();
                    }
                    else
                    {
                        ((TextWriter)sourceOrDestination).Close();
                    }
                }
                sourceOrDestination = null;
                consumer = null;
                producer = null;
                chunkToReadBuffer = null;
                writeBuffer = null;
                byteSource = null;
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        /// <exception cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException">
        /// There were too many iterations without progress during conversion.
        /// </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (sourceOrDestination == null)
            {
                throw new ObjectDisposedException("ConverterStream");
            }
            if (producer == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.ReadUnsupported);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset > buffer.Length || offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
            }
            if (count > buffer.Length || count < 0)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
            }
            if (count + offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterStreamInInconsistentStare);
            }
            int num = count;
            if (byteSource != null)
            {
                byte[] src;
                int srcOffset;
                int val;
                while (count != 0 && byteSource.GetOutputChunk(out src, out srcOffset, out val))
                {
                    int num2 = Math.Min(val, count);
                    Buffer.BlockCopy(src, srcOffset, buffer, offset, num2);
                    offset += num2;
                    count -= num2;
                    byteSource.ReportOutput(num2);
                }
            }
            if (count != 0)
            {
                long num3 = 0L;
                writeBuffer = buffer;
                writeOffset = offset;
                writeCount = count;
                inconsistentState = true;
                while (writeCount != 0 && !endOfFile)
                {
                    producer.Run();
                    if (madeProgress)
                    {
                        num3 = 0L;
                        madeProgress = false;
                    }
                    else
                    {
                        long arg_139_0 = maxLoopsWithoutProgress;
                        long expr_133 = num3;
                        num3 = expr_133 + 1L;
                        if (arg_139_0 == expr_133)
                        {
                            throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
                        }
                    }
                }
                count = writeCount;
                writeBuffer = null;
                writeOffset = 0;
                writeCount = 0;
                inconsistentState = false;
            }
            return num - count;
        }

        /// <summary>
        /// Report the progress of the current operation.
        /// </summary>
        void IProgressMonitor.ReportProgress()
        {
            madeProgress = true;
        }

        /// <summary>
        /// Sets the source of the information to be converted.
        /// </summary>
        /// <param name="newByteSource">The byte source.</param>
        internal void SetSource(IByteSource newByteSource)
        {
            byteSource = newByteSource;
        }

        /// <summary>
        /// Gets the output buffer.
        /// </summary>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputCount">The output count.</param>
        internal void GetOutputBuffer(out byte[] outputBuffer, out int outputOffset, out int outputCount)
        {
            outputBuffer = writeBuffer;
            outputOffset = writeOffset;
            outputCount = writeCount;
        }

        /// <summary>
        /// Reports the output.
        /// </summary>
        /// <param name="outputCount">The output count.</param>
        internal void ReportOutput(int outputCount)
        {
            if (outputCount != 0)
            {
                madeProgress = true;
                writeCount -= outputCount;
                writeOffset += outputCount;
            }
        }

        /// <summary>
        /// Reports the end of file.
        /// </summary>
        internal void ReportEndOfFile()
        {
            endOfFile = true;
        }

        /// <summary>
        /// Gets the input chunk.
        /// </summary>
        /// <param name="chunkBuffer">The chunk buffer.</param>
        /// <param name="chunkOffset">The chunk offset.</param>
        /// <param name="chunkCount">The chunk count.</param>
        /// <param name="eof">Set to true if the EOF was reached.</param>
        /// <returns>false if there are no more bytes to read, otherwise true.</returns>
        internal bool GetInputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkCount, out bool eof)
        {
            chunkBuffer = chunkToReadBuffer;
            chunkOffset = chunkToReadOffset;
            chunkCount = chunkToReadCount;
            eof = (endOfFile && 0 == chunkToReadCount);
            return chunkToReadCount != 0 || endOfFile;
        }

        /// <summary>
        /// Reports the number of characters read.
        /// </summary>
        /// <param name="readCount">The read count.</param>
        internal void ReportRead(int readCount)
        {
            if (readCount != 0)
            {
                madeProgress = true;
                chunkToReadCount -= readCount;
                chunkToReadOffset += readCount;
                if (chunkToReadCount == 0)
                {
                    chunkToReadBuffer = null;
                    chunkToReadOffset = 0;
                }
            }
        }
    }
}
