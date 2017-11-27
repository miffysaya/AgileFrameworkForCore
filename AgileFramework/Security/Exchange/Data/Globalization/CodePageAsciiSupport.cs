namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// An indication of the code page ASCII support.
    /// </summary>
    internal enum CodePageAsciiSupport : byte
    {
        /// <summary>
        /// Unknown ASCII support
        /// </summary>
        Unknown,
        /// <summary>
        /// No ASCII support
        /// </summary>
        None,
        /// <summary>
        /// Incomplete ASCII support
        /// </summary>
        Incomplete,
        /// <summary>
        /// Some remapping is required
        /// </summary>
        Remap,
        /// <summary>
        /// ASCII support is fine for most purposes
        /// </summary>
        Fine,
        /// <summary>
        /// Complete ASCII support
        /// </summary>
        Complete
    }
}
