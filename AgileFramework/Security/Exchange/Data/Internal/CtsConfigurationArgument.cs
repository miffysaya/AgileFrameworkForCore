namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// Contains a configuration argument and its value.
    /// </summary>
    internal class CtsConfigurationArgument
    {
        /// <summary>
        /// Gets the argument name.
        /// </summary>
        /// <value>The argument name.</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <value>The argument value.</value>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationArgument" /> class.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        internal CtsConfigurationArgument(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
