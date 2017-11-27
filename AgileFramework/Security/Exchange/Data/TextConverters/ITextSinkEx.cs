namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes needing to write.
    /// </summary>
    internal interface ITextSinkEx : ITextSink
    {
        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Write(string value);

        /// <summary>
        /// Writes the new line.
        /// </summary>
        void WriteNewLine();
    }
}
