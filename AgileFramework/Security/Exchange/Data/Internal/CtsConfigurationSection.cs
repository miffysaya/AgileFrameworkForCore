using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace AgileFramework.Security.Exchange.Data.Internal
{
    /// <summary>
    /// Provides access to the configuration section.
    /// </summary>
    internal sealed class CtsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The subsections in the configuration file.
        /// </summary>
        private readonly Dictionary<string, IList<CtsConfigurationSetting>> subSections = new Dictionary<string, IList<CtsConfigurationSetting>>();

        /// <summary>
        /// The configuration properties.
        /// </summary>
        private static ConfigurationPropertyCollection properties;

        /// <summary>
        /// Gets a dictionary for the subsections in the configuration file.
        /// </summary>
        public Dictionary<string, IList<CtsConfigurationSetting>> SubSectionsDictionary
        {
            get
            {
                return subSections;
            }
        }

        /// <summary>
        /// Gets a collection of all configuration properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection arg_14_0;
                if ((arg_14_0 = properties) == null)
                {
                    arg_14_0 = (properties = new ConfigurationPropertyCollection());
                }
                return arg_14_0;
            }
        }

        /// <summary>
        /// Deserailizes a configuration section.
        /// </summary>
        /// <param name="reader">An XmlReader containing the section to deserialize.</param>
        protected override void DeserializeSection(XmlReader reader)
        {
            IList<CtsConfigurationSetting> list = new List<CtsConfigurationSetting>();
            subSections.Add(string.Empty, list);
            if (!reader.Read() || reader.NodeType != XmlNodeType.Element)
            {
                throw new ConfigurationErrorsException("error", reader);
            }
            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    XmlNodeType nodeType = reader.NodeType;
                    switch (nodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.IsEmptyElement)
                            {
                                CtsConfigurationSetting item = DeserializeSetting(reader);
                                list.Add(item);
                            }
                            else
                            {
                                string name = reader.Name;
                                IList<CtsConfigurationSetting> list2;
                                if (!subSections.TryGetValue(name, out list2))
                                {
                                    list2 = new List<CtsConfigurationSetting>();
                                    subSections.Add(name, list2);
                                }
                                while (reader.Read())
                                {
                                    XmlNodeType nodeType2 = reader.NodeType;
                                    switch (nodeType2)
                                    {
                                        case XmlNodeType.Element:
                                            {
                                                if (!reader.IsEmptyElement)
                                                {
                                                    throw new ConfigurationErrorsException("error", reader);
                                                }
                                                CtsConfigurationSetting item2 = DeserializeSetting(reader);
                                                list2.Add(item2);
                                                break;
                                            }
                                        case XmlNodeType.Attribute:
                                            break;
                                        case XmlNodeType.Text:
                                        case XmlNodeType.CDATA:
                                            throw new ConfigurationErrorsException("error", reader);
                                        default:
                                            if (nodeType2 != XmlNodeType.EndElement)
                                            {
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case XmlNodeType.Attribute:
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            throw new ConfigurationErrorsException("error", reader);
                        default:
                            if (nodeType != XmlNodeType.EndElement)
                            {
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes an individual configuration setting.
        /// </summary>
        /// <param name="reader">The XmlReader containing the setting to deserialize.</param>
        /// <returns>A <see cref="T:Microsoft.Exchange.Data.Internal.CtsConfigurationSetting" /> instance containing the configuration setting.</returns>
        private static CtsConfigurationSetting DeserializeSetting(XmlReader reader)
        {
            string name = reader.Name;
            CtsConfigurationSetting ctsConfigurationSetting = new CtsConfigurationSetting(name);
            if (reader.AttributeCount > 0)
            {
                while (reader.MoveToNextAttribute())
                {
                    string name2 = reader.Name;
                    string value = reader.Value;
                    ctsConfigurationSetting.AddArgument(name2, value);
                }
            }
            return ctsConfigurationSetting;
        }
    }
}
