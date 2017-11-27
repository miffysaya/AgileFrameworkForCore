using System;
using System.Runtime.Serialization;

namespace AgileFramework.Security.Exchange.Data
{
    /// <summary>
    /// Thrown when a data exception occurs.
    /// </summary>
    [Serializable]
    internal class ExchangeDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.ExchangeDataException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ExchangeDataException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.ExchangeDataException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExchangeDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.ExchangeDataException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info" /> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected ExchangeDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
