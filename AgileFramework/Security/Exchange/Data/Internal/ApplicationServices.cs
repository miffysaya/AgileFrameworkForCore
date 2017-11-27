using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// Provides functions for parsing application configuration data.
    /// </summary>
    internal static class ApplicationServices
    {
        /// <summary>
        /// Loads the application service provider.
        /// </summary>
        private static readonly IApplicationServices ServicesProvider = LoadServices();

        /// <summary>
        /// Gets the application service provider.
        /// </summary>
        public static IApplicationServices Provider
        {
            get
            {
                return ServicesProvider;
            }
        }

        /// <summary>
        /// Gets the specified configuration setting.
        /// </summary>
        /// <param name="subSectionName">Name of the configuration sub section.</param>
        /// <param name="settingName">Name of the configuration setting.</param>
        /// <returns>A <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationSetting" /> for the sepecified setting from the specified sub section.</returns>
        public static CtsConfigurationSetting GetSimpleConfigurationSetting(string subSectionName, string settingName)
        {
            CtsConfigurationSetting ctsConfigurationSetting = null;
            IList<CtsConfigurationSetting> configuration = Provider.GetConfiguration(subSectionName);
            foreach (CtsConfigurationSetting current in from setting in configuration
                                                        where string.Equals(setting.Name, settingName, StringComparison.OrdinalIgnoreCase)
                                                        select setting)
            {
                if (ctsConfigurationSetting != null)
                {
                    Provider.LogConfigurationErrorEvent();
                    break;
                }
                ctsConfigurationSetting = current;
            }
            return ctsConfigurationSetting;
        }

        /// <summary>
        /// Initializes the application services.
        /// </summary>
        /// <returns>An instance of the default Application Services class.</returns>
        private static IApplicationServices LoadServices()
        {
            return new DefaultApplicationServices();
        }
    }
}
