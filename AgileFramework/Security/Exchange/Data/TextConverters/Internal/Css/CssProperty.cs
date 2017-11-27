using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal struct CssProperty
    {
        private CssToken token;

        public bool IsPropertyBegin
        {
            get
            {
                return token.propertyList[token.currentProperty].IsPropertyBegin;
            }
        }

        public bool IsPropertyNameEnd
        {
            get
            {
                return token.propertyList[token.currentProperty].IsPropertyNameEnd;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return token.propertyList[token.currentProperty].IsPropertyDeleted;
            }
        }

        public CssNameIndex NameId
        {
            get
            {
                return token.propertyList[token.currentProperty].nameId;
            }
        }

        public bool HasNameFragment
        {
            get
            {
                return !token.propertyList[token.currentProperty].name.IsEmpty;
            }
        }

        public CssToken.PropertyNameTextReader Name
        {
            get
            {
                return new CssToken.PropertyNameTextReader(token);
            }
        }

        public bool HasValueFragment
        {
            get
            {
                return !token.propertyList[token.currentProperty].value.IsEmpty;
            }
        }

        public CssToken.PropertyValueTextReader Value
        {
            get
            {
                return new CssToken.PropertyValueTextReader(token);
            }
        }

        internal CssProperty(CssToken token)
        {
            this.token = token;
        }

        [Conditional("DEBUG")]
        private void AssertCurrent()
        {
        }
    }
}
