using System;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Flags detailing if the codepage is detectable and/or is a 7bit code page.
    /// </summary>
    [Flags]
    internal enum CodePageFlags : byte
    {
        /// <summary>
        /// The codepage is not detectable or 7 bit
        /// </summary>
        None = 0,
        /// <summary>
        /// The code page is detectable
        /// </summary>
        Detectable = 1,
        /// <summary>
        /// The code page is a 7bit codepage
        /// </summary>
        SevenBit = 2
    }
}
