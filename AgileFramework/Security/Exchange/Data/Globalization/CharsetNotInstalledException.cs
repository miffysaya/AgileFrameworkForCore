using System;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// The exception thrown when a character set which is not installed is used.
    /// </summary>
    [Serializable]
    internal class CharsetNotInstalledException : InvalidCharsetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CharsetNotInstalledException" /> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The message.</param>
        public CharsetNotInstalledException(int codePage, string message) : base(codePage, message)
        {
        }
    }
}
