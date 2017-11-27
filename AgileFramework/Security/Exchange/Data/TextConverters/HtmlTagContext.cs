using AgileFramework.Security.Exchange.CtsResources;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal abstract class HtmlTagContext
    {
        internal enum TagWriteState
        {
            Undefined,
            Written,
            Deleted
        }

        private byte cookie;

        private bool valid;

        private bool invokeCallbackForEndTag;

        private bool deleteInnerContent;

        private bool deleteEndTag;

        private bool isEndTag;

        private bool isEmptyElementTag;

        private HtmlNameIndex tagNameIndex;

        private HtmlTagParts tagParts;

        /// <summary>
        /// Gets the index of the tag name.
        /// </summary>
        /// <value>The index of the tag name.</value>
        internal HtmlNameIndex TagNameIndex
        {
            get
            {
                AssertContextValid();
                return tagNameIndex;
            }
        }

        /// <summary>
        /// Gets the tag parts.
        /// </summary>
        /// <value>The tag parts.</value>
        internal HtmlTagParts TagParts
        {
            get
            {
                AssertContextValid();
                return tagParts;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can invoke callback for end tag.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can invoke callback for end tag; otherwise, <c>false</c>.
        /// </value>
        internal bool IsInvokeCallbackForEndTag
        {
            get
            {
                return invokeCallbackForEndTag;
            }
        }

        internal bool IsDeleteInnerContent
        {
            get
            {
                return deleteInnerContent;
            }
        }

        internal bool IsDeleteEndTag
        {
            get
            {
                return deleteEndTag;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.HtmlTagContext" /> class.
        /// </summary>
        internal HtmlTagContext()
        {
        }

        internal void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool droppedEndTag)
        {
            this.isEndTag = isEndTag;
            isEmptyElementTag = false;
            this.tagNameIndex = tagNameIndex;
            invokeCallbackForEndTag = false;
            deleteInnerContent = false;
            deleteEndTag = !this.isEndTag;
            cookie += 1;
        }

        internal void InitializeFragment(bool isEmptyElementTag, int attributeCount, HtmlTagParts tagParts)
        {
            if (attributeCount >= 16777215)
            {
                throw new TextConvertersException();
            }
            this.isEmptyElementTag = isEmptyElementTag;
            this.tagParts = tagParts;
            cookie += 1;
            valid = true;
        }

        internal void UninitializeFragment()
        {
            valid = false;
        }

        internal virtual bool GetCopyPendingStateImpl()
        {
            return false;
        }

        internal abstract string GetTagNameImpl();

        internal abstract HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex);

        internal abstract HtmlAttributeParts GetAttributePartsImpl(int attributeIndex);

        internal abstract string GetAttributeNameImpl(int attributeIndex);

        internal abstract string GetAttributeValueImpl(int attributeIndex);

        internal abstract void WriteTagImpl(bool writeAttributes);

        internal virtual void DeleteTagImpl()
        {
        }

        internal abstract void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue);

        internal void AssertContextValid()
        {
            if (!valid)
            {
                throw new InvalidOperationException(TextConvertersStrings.ContextNotValidInThisState);
            }
        }
    }
}
