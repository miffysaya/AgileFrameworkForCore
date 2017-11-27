using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Base class for a text convertor.
    /// </summary>
    internal abstract class TextConverter : IResultsFeedback
    {
        /// <summary>
        /// Value indicating if boundary conditions are to be tested.
        /// </summary>
        //private bool testBoundaryConditions;

        /// <summary>
        /// The input buffer size.
        /// </summary>
        private int inputBufferSize = 4096;

        /// <summary>
        /// The output buffer size.
        /// </summary>
        private int outputBufferSize = 4096;

        /// <summary>
        /// Gets or sets the size of the input stream buffer.
        /// </summary>
        /// <value>The size of the input stream buffer.</value>
        public int InputStreamBufferSize
        {
            get
            {
                return this.inputBufferSize;
            }
        }

        /// <summary>
        /// Gets or sets the size of the output stream buffer.
        /// </summary>
        /// <value>The size of the output stream buffer.</value>
        public int OutputStreamBufferSize
        {
            get
            {
                return this.outputBufferSize;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether boundary conditions should be tested.
        /// </summary>
        /// <value>
        /// <c>true</c> if boundary conditions should be tested; otherwise, <c>false</c>.
        /// </value>
        internal bool TestBoundaryConditions
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Microsoft.Exchange.Data.TextConverters.TextConverter" /> is locked.
        /// </summary>
        /// <value><c>true</c> if locked; otherwise, <c>false</c>.</value>
        protected bool Locked
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the specified source reader.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="destinationStream">The destination stream.</param>
        public void Convert(TextReader sourceReader, Stream destinationStream)
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }
            Stream stream = new ConverterStream(sourceReader, this);
            byte[] array = new byte[this.outputBufferSize];
            while (true)
            {
                int num = stream.Read(array, 0, array.Length);
                if (num == 0)
                {
                    break;
                }
                destinationStream.Write(array, 0, num);
            }
            destinationStream.Flush();
        }

        /// <summary>
        /// Converts the specified source reader.
        /// </summary>
        /// <param name="sourceReader">The source reader.</param>
        /// <param name="destinationWriter">The destination writer.</param>
        public void Convert(TextReader sourceReader, TextWriter destinationWriter)
        {
            if (destinationWriter == null)
            {
                throw new ArgumentNullException("destinationWriter");
            }
            TextReader textReader = new ConverterReader(sourceReader, this);
            char[] array = new char[4096];
            while (true)
            {
                int num = textReader.Read(array, 0, array.Length);
                if (num == 0)
                {
                    break;
                }
                destinationWriter.Write(array, 0, num);
            }
            destinationWriter.Flush();
        }

        /// <summary>
        /// Sets the configuration parameter and its associated value.
        /// </summary>
        /// <param name="parameterId">The configuration parameter to set.</param>
        /// <param name="val">The value for the configuration parameter.</param>
        void IResultsFeedback.Set(ConfigParameter parameterId, object val)
        {
            this.SetResult(parameterId, val);
        }

        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterStream">The converter stream.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output);

        /// <summary>
        /// Creates the push chain.
        /// </summary>
        /// <param name="converterStream">The converter stream.</param>
        /// <param name="output">The output.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input Stream.</param>
        /// <param name="converterStream">The converter stream.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input TextReader.</param>
        /// <param name="converterStream">The converter stream.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="converterReader">The converter reader.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader);

        /// <summary>
        /// Creates the pull chain.
        /// </summary>
        /// <param name="input">The input TextReader.</param>
        /// <param name="converterReader">The converter reader.</param>
        /// <returns>An <see cref="T:Microsoft.Exchange.Data.TextConverters.IProducerConsumer" /> for use in a chain.</returns>
        internal abstract IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader);

        /// <summary>
        /// Sets the result for the specified parameter.
        /// </summary>
        /// <param name="parameterId">The parameter.</param>
        /// <param name="val">The value.</param>
        internal virtual void SetResult(ConfigParameter parameterId, object val)
        {
        }

        /// <summary>
        /// Asserts that this instance is not locked.
        /// </summary>
        internal void AssertNotLocked()
        {
            if (this.Locked)
            {
                throw new InvalidOperationException(TextConvertersStrings.ParametersCannotBeChangedAfterConverterObjectIsUsed);
            }
        }
    }
}
