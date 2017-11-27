using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;
using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal struct CssSelector
    {
        private CssToken token;

        public bool IsDeleted
        {
            get
            {
                return token.selectorList[token.currentSelector].IsSelectorDeleted;
            }
        }

        public HtmlNameIndex NameId
        {
            get
            {
                return token.selectorList[token.currentSelector].nameId;
            }
        }

        public bool HasNameFragment
        {
            get
            {
                return !token.selectorList[token.currentSelector].name.IsEmpty;
            }
        }

        public CssToken.SelectorNameTextReader Name
        {
            get
            {
                return new CssToken.SelectorNameTextReader(token);
            }
        }

        public bool HasClassFragment
        {
            get
            {
                return !token.selectorList[token.currentSelector].className.IsEmpty;
            }
        }

        public CssToken.SelectorClassTextReader ClassName
        {
            get
            {
                return new CssToken.SelectorClassTextReader(token);
            }
        }

        public CssSelectorClassType ClassType
        {
            get
            {
                return token.selectorList[token.currentSelector].classType;
            }
        }

        public bool IsSimple
        {
            get
            {
                return token.selectorList[token.currentSelector].combinator == CssSelectorCombinator.None && (token.selectorTail == token.currentSelector + 1 || token.selectorList[token.currentSelector + 1].combinator == CssSelectorCombinator.None);
            }
        }

        public CssSelectorCombinator Combinator
        {
            get
            {
                return token.selectorList[token.currentSelector].combinator;
            }
        }

        internal CssSelector(CssToken token)
        {
            this.token = token;
        }

        [Conditional("DEBUG")]
        private void AssertCurrent()
        {
        }
    }
}
