namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Interface declaration for classes that are reusable.
    /// </summary>
    internal interface IReusable
    {
        /// <summary>
        /// Initializes the specified new source or destination.
        /// </summary>
        /// <param name="newSourceOrDestination">The new source or destination.</param>
        void Initialize(object newSourceOrDestination);
    }
}
