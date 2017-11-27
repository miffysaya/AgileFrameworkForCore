using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// A conversion class presented as a text reader.
    /// </summary>
    internal class ConverterReader : TextReader, IProgressMonitor
    {
        /// <summary>
        /// The number of conversion loops to attempt without any progress before the conversion is cancelled.
        /// </summary>
        private readonly int maxLoopsWithoutProgress;

        /// <summary>
        /// The output of the convertor.
        /// </summary>
        private ConverterUnicodeOutput sourceOutputObject;

        /// <summary>
        /// The conversion producer and consume.
        /// </summary>
        private IProducerConsumer producer;

        /// <summary>
        /// Value indicating if any progress has been made during conversion.
        /// </summary>
        private bool madeProgress;

        /// <summary>
        /// The internal write buffer.
        /// </summary>
        private char[] writeBuffer;

        /// <summary>
        /// The position within the internal write buffer.
        /// </summary>
        private int writeIndex;

        /// <summary>
        /// A running total of the number of characters written.
        /// </summary>
        private int writeCount;

        /// <summary>
        /// The conversion source.
        /// </summary>
        private object source;

        /// <summary>
        /// Value indicating if the end of the file has been reached.
        /// </summary>
        private bool endOfFile;

        /// <summary>
        /// Value indicating if the conversion is in an inconsitent state.
        /// </summary>
        private bool inconsistentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.ConverterReader" /> class.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="converter">The converter to use.</param>
        public ConverterReader(TextReader sourceReader, TextConverter converter)
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
            source = sourceReader;
            maxLoopsWithoutProgress = 100000 + converter.InputStreamBufferSize + converter.OutputStreamBufferSize;
        }

        /// <summary>
        /// Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the input stream.
        /// </summary>
        /// <returns>
        /// An integer representing the next character to be read, or -1 if no more characters are available or the stream does not support seeking.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextReader" /> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override int Peek()
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
            }
            long num = 0L;
            inconsistentState = true;
            while (!endOfFile)
            {
                char[] array;
                int num2;
                int num3;
                if (sourceOutputObject.GetOutputChunk(out array, out num2, out num3))
                {
                    inconsistentState = false;
                    return array[num2];
                }
                producer.Run();
                if (madeProgress)
                {
                    madeProgress = false;
                    num = 0L;
                }
                else
                {
                    long arg_7C_0 = maxLoopsWithoutProgress;
                    long expr_77 = num;
                    num = expr_77 + 1L;
                    if (arg_7C_0 == expr_77)
                    {
                        throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
                    }
                }
            }
            inconsistentState = false;
            return -1;
        }

        /// <summary>
        /// Reads the next character from the input stream and advances the character position by one character.
        /// </summary>
        /// <returns>
        /// The next character from the input stream, or -1 if no more characters are available. The default implementation returns -1.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextReader" /> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override int Read()
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
            }
            if (inconsistentState)
            {
                throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
            }
            long num = 0L;
            inconsistentState = true;
            while (!endOfFile)
            {
                char[] array;
                int num2;
                int num3;
                if (sourceOutputObject.GetOutputChunk(out array, out num2, out num3))
                {
                    sourceOutputObject.ReportOutput(1);
                    inconsistentState = false;
                    return array[num2];
                }
                producer.Run();
                if (madeProgress)
                {
                    madeProgress = false;
                    num = 0L;
                }
                else
                {
                    long arg_88_0 = maxLoopsWithoutProgress;
                    long expr_83 = num;
                    num = expr_83 + 1L;
                    if (arg_88_0 == expr_83)
                    {
                        throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
                    }
                }
            }
            inconsistentState = false;
            return -1;
        }

        /// <summary>
        /// Reads a maximum of <paramref name="count" /> characters from the current stream and writes the data to <paramref name="buffer" />, beginning at <paramref name="index" />.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The place in <paramref name="buffer" /> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the stream is reached before <paramref name="count" /> of characters is read into <paramref name="buffer" />, the current method returns.</param>
        /// <returns>
        /// The number of characters that have been read. The number will be less than or equal to <paramref name="count" />, depending on whether the data is available within the stream. This method returns zero if called when no more characters are left to read.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.IO.TextReader" /> is closed.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        public override int Read(char[] buffer, int index, int count)
        {
            if (source == null)
            {
                throw new ObjectDisposedException("ConverterReader");
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
                throw new InvalidOperationException(TextConvertersStrings.ConverterReaderInInconsistentStare);
            }
            int num = count;
            char[] src;
            int num2;
            int val;
            while (count != 0 && sourceOutputObject.GetOutputChunk(out src, out num2, out val))
            {
                int num3 = Math.Min(val, count);
                Buffer.BlockCopy(src, num2 * 2, buffer, index * 2, num3 * 2);
                index += num3;
                count -= num3;
                sourceOutputObject.ReportOutput(num3);
            }
            if (count != 0)
            {
                long num4 = 0L;
                writeBuffer = buffer;
                writeIndex = index;
                writeCount = count;
                inconsistentState = true;
                while (writeCount != 0 && !endOfFile)
                {
                    producer.Run();
                    if (madeProgress)
                    {
                        madeProgress = false;
                        num4 = 0L;
                    }
                    else
                    {
                        long arg_124_0 = maxLoopsWithoutProgress;
                        long expr_11E = num4;
                        num4 = expr_11E + 1L;
                        if (arg_124_0 == expr_11E)
                        {
                            throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
                        }
                    }
                }
                count = writeCount;
                writeBuffer = null;
                writeIndex = 0;
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
        /// Sets the data source for conversion.
        /// </summary>
        /// <param name="converterUnicodeOutputSource">The data source for conversion.</param>
        internal void SetSource(ConverterUnicodeOutput converterUnicodeOutputSource)
        {
            sourceOutputObject = converterUnicodeOutputSource;
        }

        /// <summary>
        /// Gets the output buffer.
        /// </summary>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputIndex">Current index position in the output buffer.</param>
        /// <param name="outputCount">The number of characters in the output buffer.</param>
        internal void GetOutputBuffer(out char[] outputBuffer, out int outputIndex, out int outputCount)
        {
            outputBuffer = writeBuffer;
            outputIndex = writeIndex;
            outputCount = writeCount;
        }

        /// <summary>
        /// Notes that output has been written.
        /// </summary>
        /// <param name="outputCount">The number of characters written.</param>
        internal void ReportOutput(int outputCount)
        {
            if (outputCount != 0)
            {
                writeCount -= outputCount;
                writeIndex += outputCount;
                madeProgress = true;
            }
        }

        /// <summary>
        /// Reports that the end of the file has been reached.
        /// </summary>
        internal void ReportEndOfFile()
        {
            endOfFile = true;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextReader" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && source != null)
            {
                if (source is Stream)
                {
                    ((Stream)source).Close();
                }
                else
                {
                    ((TextReader)source).Close();
                }
            }
            if (producer != null && producer is IDisposable)
            {
                ((IDisposable)producer).Dispose();
            }
            source = null;
            producer = null;
            sourceOutputObject = null;
            writeBuffer = null;
            base.Dispose(disposing);
        }
    }
}
