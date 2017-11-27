namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// An interface for setting results
    /// </summary>
    internal interface IResultsFeedback
    {
        /// <summary>
        /// Sets the configuration parameter and its associated value.
        /// </summary>
        /// <param name="parameterId">The configuration parameter to set.</param>
        /// <param name="val">The value for the configuration parameter.</param>
        void Set(ConfigParameter parameterId, object val);
    }
}
