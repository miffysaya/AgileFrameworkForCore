namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes with Test Sink.
    /// </summary>
    internal interface ITextSink
    {
        /// <summary>
        /// Gets a value indicating whether this instance is enough.
        /// </summary>
        /// <value><c>true</c> if this instance is enough; otherwise, <c>false</c>.</value>
        bool IsEnough
        {
            get;
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        void Write(char[] buffer, int offset, int count);

        /// <summary>
        /// Writes the specified ucs32 char.
        /// </summary>
        /// <param name="ucs32Char">The ucs32 char.</param>
        void Write(int ucs32Char);
    }
}
