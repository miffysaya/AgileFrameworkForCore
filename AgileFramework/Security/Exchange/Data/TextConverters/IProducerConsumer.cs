namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for Producer Consumer.
    /// </summary>
    internal interface IProducerConsumer
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        void Run();

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        /// <returns>
        /// <c>true</c> if flush is successful; otherwise <c>false</c>.
        /// </returns>
        bool Flush();
    }
}
