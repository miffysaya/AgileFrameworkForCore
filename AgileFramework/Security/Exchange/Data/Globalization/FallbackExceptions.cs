namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Value indidicating which fallback exceptions should be allowed.
    /// </summary>      
    internal enum FallbackExceptions
    {
        /// <summary>
        /// No fallback exceptions are allowed.
        /// </summary>
        None,
        /// <summary>
        /// Common fallback exceptions are allowed.
        /// </summary>
        Common,
        /// <summary>
        /// All fallback exceptions are allowed.
        /// </summary>
        All
    }
}
