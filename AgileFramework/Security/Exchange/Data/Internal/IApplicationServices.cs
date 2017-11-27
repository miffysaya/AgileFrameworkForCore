using System.Collections.Generic;

namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// An interface for application configuration services.
    /// </summary>
    internal interface IApplicationServices
    {
        /// <summary>
        /// Gets the configuration subsection specified.
        /// </summary>
        /// <param name="subSectionName">Name of the subsection.</param>
        /// <returns>A list of <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationSetting" />s for the specified section.</returns>
        IList<CtsConfigurationSetting> GetConfiguration(string subSectionName);

        /// <summary>
        /// Refreshes the configuration from the application configuration file.
        /// </summary>
        void RefreshConfiguration();

        /// <summary>
        /// Logs an error during configuration processing.
        /// </summary>
        void LogConfigurationErrorEvent();
    }
}
