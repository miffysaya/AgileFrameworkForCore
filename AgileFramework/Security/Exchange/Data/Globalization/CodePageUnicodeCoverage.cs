namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// An indication of the code page Unicode support.
    /// </summary>
    internal enum CodePageUnicodeCoverage : byte
    {
        /// <summary>
        /// Unknown Unicode coverage
        /// </summary>
        Unknown,
        /// <summary>
        /// Partial Unicode coverage
        /// </summary>
        Partial,
        /// <summary>
        /// Complete Unicode coverage
        /// </summary>
        Complete
    }
}
