using AgileFramework.Security.Exchange.CtsResources;
using System;
using System.Runtime.Serialization;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Exception thrown when an invalid character set is used.
    /// </summary>
    [Serializable]
    internal class InvalidCharsetException : ExchangeDataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.InvalidCharsetException" /> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        public InvalidCharsetException(int codePage) : base(GlobalizationStrings.InvalidCodePage(codePage))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.InvalidCharsetException" /> class.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <param name="message">The exception message.</param>
        public InvalidCharsetException(int codePage, string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.InvalidCharsetException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info" /> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected InvalidCharsetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
