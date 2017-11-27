using System.Collections.Generic;

namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// Contains a configuration name and its arguments.
    /// </summary>
    internal class CtsConfigurationSetting
    {
        /// <summary>
        /// The configuration name.
        /// </summary>
        private readonly string configurationName;

        /// <summary>
        /// The configuration arguments.
        /// </summary>
        private readonly IList<CtsConfigurationArgument> arguments;

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        /// <value>The name of the setting.</value>
        public string Name
        {
            get
            {
                return configurationName;
            }
        }

        /// <summary>
        /// Gets the argument list for the setting.
        /// </summary>
        /// <value>The argument list.</value>
        public IList<CtsConfigurationArgument> Arguments
        {
            get
            {
                return arguments;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationSetting" /> class.
        /// </summary>
        /// <param name="name">The setting name.</param>
        internal CtsConfigurationSetting(string name)
        {
            configurationName = name;
            arguments = new List<CtsConfigurationArgument>();
        }

        /// <summary>
        /// Adds the specified argument to the configuration setting.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        internal void AddArgument(string name, string value)
        {
            arguments.Add(new CtsConfigurationArgument(name, value));
        }
    }
}
