using AgileFramework.Security.Application.TextConverters.HTML;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct HtmlTagParts
    {
        private HtmlToken.TagPartMajor major;

        private HtmlToken.TagPartMinor minor;

        public bool Begin
        {
            get
            {
                return 3 == (byte)(major & HtmlToken.TagPartMajor.Begin);
            }
        }

        public bool Name
        {
            get
            {
                return 2 == (byte)(minor & HtmlToken.TagPartMinor.ContinueName);
            }
        }

        internal HtmlTagParts(HtmlToken.TagPartMajor major, HtmlToken.TagPartMinor minor)
        {
            this.minor = minor;
            this.major = major;
        }

        public override string ToString()
        {
            return major.ToString() + " /" + minor.ToString() + "/";
        }
    }
}
