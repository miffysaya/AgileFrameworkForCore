namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal class HtmlToHtmlTagContext : HtmlTagContext
    {
        private HtmlToHtmlConverter converter;

        public HtmlToHtmlTagContext(HtmlToHtmlConverter converter)
        {
            this.converter = converter;
        }

        internal override string GetTagNameImpl()
        {
            if (TagNameIndex > HtmlNameIndex.Unknown)
            {
                if (!TagParts.Begin)
                {
                    return string.Empty;
                }
                return HtmlNameData.names[(int)TagNameIndex].name;
            }
            else
            {
                if (TagParts.Name)
                {
                    return converter.token.Name.GetString(2147483647);
                }
                return string.Empty;
            }
        }

        internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
        {
            return converter.GetAttributeNameId(attributeIndex);
        }

        internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
        {
            return converter.GetAttributeParts(attributeIndex);
        }

        internal override string GetAttributeNameImpl(int attributeIndex)
        {
            return converter.GetAttributeName(attributeIndex);
        }

        internal override string GetAttributeValueImpl(int attributeIndex)
        {
            return converter.GetAttributeValue(attributeIndex);
        }

        internal override void WriteTagImpl(bool copyTagAttributes)
        {
            converter.WriteTag(copyTagAttributes);
        }

        internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
        {
            converter.WriteAttribute(attributeIndex, writeName, writeValue);
        }
    }
}
