namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes that are restartable.
    /// </summary>
    internal interface IRestartable
    {
        /// <summary>
        /// Determines whether this instance can restart.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can restart; otherwise, <c>false</c>.
        /// </returns>
        bool CanRestart();

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        void Restart();

        /// <summary>
        /// Disables the restart.
        /// </summary>
        void DisableRestart();
    }
}
