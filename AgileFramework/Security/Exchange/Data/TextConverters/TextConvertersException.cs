using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    [Serializable]
    internal class TextConvertersException : ExchangeDataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException" /> class.
        /// </summary>
        internal TextConvertersException() : base("internal text conversion error (document too complex)")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal TextConvertersException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.TextConvertersException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        internal TextConvertersException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
