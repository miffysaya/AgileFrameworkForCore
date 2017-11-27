using AgileFramework.Security.Application.TextConverters.HTML;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct HtmlAttributeParts
    {
        private HtmlToken.AttrPartMajor major;

        private HtmlToken.AttrPartMinor minor;

        internal HtmlAttributeParts(HtmlToken.AttrPartMajor major, HtmlToken.AttrPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }
    }
}
