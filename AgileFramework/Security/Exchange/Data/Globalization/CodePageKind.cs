namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// The character set type of the code page.
    /// </summary>
    internal enum CodePageKind : byte
    {
        /// <summary>
        /// Unknown character set
        /// </summary>
        Unknown,
        /// <summary>
        /// Single Byte character set
        /// </summary>
        Sbcs,
        /// <summary>
        /// Double Byte character set
        /// </summary>
        Dbcs,
        /// <summary>
        /// Multi-byte character set
        /// </summary>
        Mbcs,
        /// <summary>
        /// Unicode character set
        /// </summary>
        Unicode
    }
}
