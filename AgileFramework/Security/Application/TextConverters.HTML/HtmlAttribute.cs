using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System.Diagnostics;

namespace AgileFramework.Security.Application.TextConverters.HTML
{
    internal struct HtmlAttribute
    {
        private HtmlToken token;

        public int Index
        {
            get
            {
                return token.currentAttribute;
            }
        }

        public HtmlToken.AttrPartMajor MajorPart
        {
            get
            {
                return token.attributeList[token.currentAttribute].MajorPart;
            }
        }

        public HtmlToken.AttrPartMinor MinorPart
        {
            get
            {
                return token.attributeList[token.currentAttribute].MinorPart;
            }
        }

        public bool IsCompleteAttr
        {
            get
            {
                return token.attributeList[token.currentAttribute].IsCompleteAttr;
            }
        }

        public bool IsAttrBegin
        {
            get
            {
                return token.attributeList[token.currentAttribute].IsAttrBegin;
            }
        }

        public bool IsAttrEnd
        {
            get
            {
                return token.attributeList[token.currentAttribute].IsAttrEnd;
            }
        }

        public bool IsAttrNameEnd
        {
            get
            {
                return token.attributeList[token.currentAttribute].IsAttrNameEnd;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return token.attributeList[token.currentAttribute].IsAttrDeleted;
            }
        }

        public HtmlNameIndex NameIndex
        {
            get
            {
                return token.attributeList[token.currentAttribute].nameIndex;
            }
        }

        public bool HasNameFragment
        {
            get
            {
                return !token.IsFragmentEmpty(token.attributeList[token.currentAttribute].name);
            }
        }

        public HtmlToken.AttributeNameTextReader Name
        {
            get
            {
                return new HtmlToken.AttributeNameTextReader(token);
            }
        }

        public bool HasValueFragment
        {
            get
            {
                return !token.IsFragmentEmpty(token.attributeList[token.currentAttribute].value);
            }
        }

        public HtmlToken.AttributeValueTextReader Value
        {
            get
            {
                return new HtmlToken.AttributeValueTextReader(token);
            }
        }

        internal HtmlAttribute(HtmlToken token)
        {
            this.token = token;
        }

        public void SetMinorPart(HtmlToken.AttrPartMinor newMinorPart)
        {
            token.attributeList[token.currentAttribute].MinorPart = newMinorPart;
        }

        [Conditional("DEBUG")]
        private void AssertCurrent()
        {
        }
    }
}
