using System.Collections.Generic;
using System.Configuration;

namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// Wrapper for CTS application settings.
    /// </summary>
    internal class DefaultApplicationServices : IApplicationServices
    {
        /// <summary>
        /// A blank sub section.
        /// </summary>
        private static readonly IList<CtsConfigurationSetting> EmptySubSection = new List<CtsConfigurationSetting>();

        /// <summary>
        /// The lock used for thread safe syncronization.
        /// </summary>
        private readonly object lockObject = new object();

        /// <summary>
        /// The configuration sub sections from the CTS application settings.
        /// </summary>
        private volatile Dictionary<string, IList<CtsConfigurationSetting>> configurationSubSections;

        /// <summary>
        /// Gets the configuration subsection specified.
        /// </summary>
        /// <param name="subSectionName">Name of the subsection.</param>
        /// <returns>
        /// A list of <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationSetting" />s for the specified section.
        /// </returns>
        public IList<CtsConfigurationSetting> GetConfiguration(string subSectionName)
        {
            IList<CtsConfigurationSetting> list;
            if (this.configurationSubSections == null)
            {
                lock (this.lockObject)
                {
                    if (this.configurationSubSections == null)
                    {
                        try
                        {
                            CtsConfigurationSection ctsConfigurationSection = ConfigurationManager.GetSection("CTS") as CtsConfigurationSection;
                            if (ctsConfigurationSection != null)
                            {
                                this.configurationSubSections = ctsConfigurationSection.SubSectionsDictionary;
                            }
                            else
                            {
                                this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>
                                {
                                    {
                                        string.Empty,
                                        new List<CtsConfigurationSetting>()
                                    }
                                };
                            }
                            string value = ConfigurationManager.AppSettings["TemporaryStoragePath"];
                            if (!string.IsNullOrEmpty(value))
                            {
                                CtsConfigurationSetting ctsConfigurationSetting = new CtsConfigurationSetting("TemporaryStorage");
                                ctsConfigurationSetting.AddArgument("Path", value);
                                list = this.configurationSubSections[string.Empty];
                                list.Add(ctsConfigurationSetting);
                            }
                            ConfigurationManager.RefreshSection("CTS");
                        }
                        catch (ConfigurationErrorsException)
                        {
                            ApplicationServices.Provider.LogConfigurationErrorEvent();
                            this.configurationSubSections = new Dictionary<string, IList<CtsConfigurationSetting>>
                            {
                                {
                                    string.Empty,
                                    new List<CtsConfigurationSetting>()
                                }
                            };
                        }
                    }
                }
            }
            if (subSectionName == null)
            {
                subSectionName = string.Empty;
            }
            if (!this.configurationSubSections.TryGetValue(subSectionName, out list))
            {
                list = DefaultApplicationServices.EmptySubSection;
            }
            return list;
        }

        /// <summary>
        /// Refreshes the configuration from the application configuration file.
        /// </summary>
        public void RefreshConfiguration()
        {
            ConfigurationManager.RefreshSection("appSettings");
            this.configurationSubSections = null;
        }

        /// <summary>
        /// Logs an error during configuration processing.
        /// </summary>
        public void LogConfigurationErrorEvent()
        {
        }
    }
}
