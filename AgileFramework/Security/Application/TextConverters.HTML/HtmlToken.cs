using AgileFramework.Security.Exchange.Data.TextConverters;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;
using System.Diagnostics;

namespace AgileFramework.Security.Application.TextConverters.HTML
{
    internal class HtmlToken : Token
    {
        [Flags]
        public enum TagFlags : byte
        {
            None = 0,
            EmptyTagName = 8,
            EndTag = 16,
            EmptyScope = 32,
            AllowWspLeft = 64,
            AllowWspRight = 128
        }

        public enum TagPartMajor : byte
        {
            None,
            Begin = 3,
            Continue = 2,
            End = 6,
            Complete
        }

        public enum TagPartMinor : byte
        {
            Empty,
            BeginName = 3,
            ContinueName = 2,
            EndName = 6,
            EndNameWithAttributes = 134,
            CompleteName = 7,
            CompleteNameWithAttributes = 135,
            BeginAttribute = 24,
            ContinueAttribute = 16,
            EndAttribute = 48,
            EndAttributeWithOtherAttributes = 176,
            AttributePartMask = 56,
            Attributes = 128
        }

        public enum AttrPartMajor : byte
        {
            None,
            Begin = 24,
            Continue = 16,
            End = 48,
            Complete = 56,
            EmptyName = 1,
            ValueQuoted = 64,
            Deleted = 128,
            MaskOffFlags = 56
        }

        public enum AttrPartMinor : byte
        {
            Empty,
            BeginName = 3,
            ContinueName = 2,
            EndName = 6,
            EndNameWithBeginValue = 30,
            EndNameWithCompleteValue = 62,
            CompleteName = 7,
            CompleteNameWithBeginValue = 31,
            CompleteNameWithCompleteValue = 63,
            BeginValue = 24,
            ContinueValue = 16,
            EndValue = 48,
            CompleteValue = 56
        }

        public struct AttributeEnumerator
        {
            private HtmlToken token;

            public int Count
            {
                get
                {
                    return token.attributeTail;
                }
            }

            public HtmlAttribute Current
            {
                get
                {
                    return new HtmlAttribute(token);
                }
            }

            public HtmlAttribute this[int i]
            {
                get
                {
                    if (i != token.currentAttribute)
                    {
                        token.attrNamePosition.Rewind(token.attributeList[i].name);
                        token.attrValuePosition.Rewind(token.attributeList[i].value);
                    }
                    token.currentAttribute = i;
                    return new HtmlAttribute(token);
                }
            }

            internal AttributeEnumerator(HtmlToken token)
            {
                this.token = token;
            }

            public bool MoveNext()
            {
                if (token.currentAttribute != token.attributeTail)
                {
                    token.currentAttribute++;
                    if (token.currentAttribute != token.attributeTail)
                    {
                        token.attrNamePosition.Rewind(token.attributeList[token.currentAttribute].name);
                        token.attrValuePosition.Rewind(token.attributeList[token.currentAttribute].value);
                    }
                }
                return token.currentAttribute != token.attributeTail;
            }

            public void Rewind()
            {
                token.currentAttribute = -1;
            }

            public AttributeEnumerator GetEnumerator()
            {
                return this;
            }

            public bool Find(HtmlNameIndex nameIndex)
            {
                for (int i = 0; i < token.attributeTail; i++)
                {
                    if (token.attributeList[i].nameIndex == nameIndex)
                    {
                        token.currentAttribute = i;
                        token.attrNamePosition.Rewind(token.attributeList[i].name);
                        token.attrValuePosition.Rewind(token.attributeList[i].value);
                        return true;
                    }
                }
                return false;
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct TagUnstructuredContentTextReader
        {
            private HtmlToken token;

            internal TagUnstructuredContentTextReader(HtmlToken token)
            {
                this.token = token;
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.unstructured, sink);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct TagNameTextReader
        {
            private HtmlToken token;

            internal TagNameTextReader(HtmlToken token)
            {
                this.token = token;
            }

            public void Rewind()
            {
                token.namePosition.Rewind(token.name);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.name, maxSize);
            }

            public void MakeEmpty()
            {
                token.name.Reset();
                Rewind();
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct AttributeNameTextReader
        {
            private HtmlToken token;

            internal AttributeNameTextReader(HtmlToken token)
            {
                this.token = token;
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.attributeList[token.currentAttribute].name, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.attributeList[token.currentAttribute].name, maxSize);
            }

            public void MakeEmpty()
            {
                token.attributeList[token.currentAttribute].name.Reset();
                token.attrNamePosition.Rewind(token.attributeList[token.currentAttribute].name);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        public struct AttributeValueTextReader
        {
            private HtmlToken token;

            public bool IsEmpty
            {
                get
                {
                    return token.IsFragmentEmpty(token.attributeList[token.currentAttribute].value);
                }
            }

            public bool IsContiguous
            {
                get
                {
                    return token.IsContiguous(token.attributeList[token.currentAttribute].value);
                }
            }

            public BufferString ContiguousBufferString
            {
                get
                {
                    return new BufferString(token.buffer, token.attributeList[token.currentAttribute].value.headOffset, token.runList[token.attributeList[token.currentAttribute].value.head].Length);
                }
            }

            internal AttributeValueTextReader(HtmlToken token)
            {
                this.token = token;
            }

            public int Read(char[] buffer, int offset, int count)
            {
                return token.Read(token.attributeList[token.currentAttribute].value, ref token.attrValuePosition, buffer, offset, count);
            }

            public void Rewind()
            {
                token.attrValuePosition.Rewind(token.attributeList[token.currentAttribute].value);
            }

            public void WriteTo(ITextSink sink)
            {
                token.WriteTo(token.attributeList[token.currentAttribute].value, sink);
            }

            public string GetString(int maxSize)
            {
                return token.GetString(token.attributeList[token.currentAttribute].value, maxSize);
            }

            public bool CaseInsensitiveCompareEqual(string str)
            {
                return token.CaseInsensitiveCompareEqual(token.attributeList[token.currentAttribute].value, str);
            }

            public bool SkipLeadingWhitespace()
            {
                return token.SkipLeadingWhitespace(token.attributeList[token.currentAttribute].value, ref token.attrValuePosition);
            }

            [Conditional("DEBUG")]
            private void AssertCurrent()
            {
            }
        }

        protected internal struct AttributeEntry
        {
            public HtmlNameIndex nameIndex;

            public byte quoteChar;

            public HtmlToken.AttrPartMajor partMajor;

            public HtmlToken.AttrPartMinor partMinor;

            public LexicalUnit name;

            public LexicalUnit localName;

            public LexicalUnit value;

            public bool IsCompleteAttr
            {
                get
                {
                    return MajorPart == HtmlToken.AttrPartMajor.Complete;
                }
            }

            public bool IsAttrBegin
            {
                get
                {
                    return (byte)(partMajor & HtmlToken.AttrPartMajor.Begin) == 24;
                }
            }

            public bool IsAttrEnd
            {
                get
                {
                    return (byte)(partMajor & HtmlToken.AttrPartMajor.End) == 48;
                }
            }

            public bool IsAttrNameEnd
            {
                get
                {
                    return (byte)(partMinor & HtmlToken.AttrPartMinor.EndName) == 6;
                }
            }

            public HtmlToken.AttrPartMajor MajorPart
            {
                get
                {
                    return partMajor & HtmlToken.AttrPartMajor.Complete;
                }
            }

            public HtmlToken.AttrPartMinor MinorPart
            {
                get
                {
                    return partMinor;
                }
                set
                {
                    partMinor = value;
                }
            }

            public bool IsAttrValueQuoted
            {
                get
                {
                    return (byte)(partMajor & HtmlToken.AttrPartMajor.ValueQuoted) == 64;
                }
                set
                {
                    partMajor = (value ? (partMajor | HtmlToken.AttrPartMajor.ValueQuoted) : (partMajor & (HtmlToken.AttrPartMajor)191));
                }
            }

            public bool IsAttrDeleted
            {
                get
                {
                    return (byte)(partMajor & HtmlToken.AttrPartMajor.Deleted) == 128;
                }
                set
                {
                    partMajor = (value ? (partMajor | HtmlToken.AttrPartMajor.Deleted) : (partMajor & (HtmlToken.AttrPartMajor)127));
                }
            }
        }

        protected internal HtmlTagIndex tagIndex;

        protected internal HtmlTagIndex originalTagIndex;

        protected internal HtmlNameIndex nameIndex;

        protected internal TagFlags flags;

        protected internal TagPartMajor partMajor;

        protected internal TagPartMinor partMinor;

        protected internal LexicalUnit unstructured;

        protected internal FragmentPosition unstructuredPosition;

        protected internal LexicalUnit name;

        protected internal LexicalUnit localName;

        protected internal FragmentPosition namePosition;

        protected internal AttributeEntry[] attributeList;

        protected internal int attributeTail;

        protected internal int currentAttribute;

        protected internal FragmentPosition attrNamePosition;

        protected internal FragmentPosition attrValuePosition;

        public new HtmlTokenId TokenId
        {
            get
            {
                return (HtmlTokenId)base.TokenId;
            }
        }

        public TagFlags Flags
        {
            get
            {
                return flags;
            }
            set
            {
                flags = value;
            }
        }

        public bool IsEndTag
        {
            get
            {
                return 0 != (byte)(flags & TagFlags.EndTag);
            }
        }

        public bool IsEmptyScope
        {
            get
            {
                return 0 != (byte)(flags & TagFlags.EmptyScope);
            }
        }

        public HtmlToken.TagPartMajor MajorPart
        {
            get
            {
                return partMajor;
            }
        }

        public HtmlToken.TagPartMinor MinorPart
        {
            get
            {
                return partMinor;
            }
        }

        public bool IsTagBegin
        {
            get
            {
                return (byte)(partMajor & TagPartMajor.Begin) == 3;
            }
        }

        public bool IsTagEnd
        {
            get
            {
                return (byte)(partMajor & TagPartMajor.End) == 6;
            }
        }

        public bool IsTagNameBegin
        {
            get
            {
                return (byte)(partMinor & TagPartMinor.BeginName) == 3;
            }
        }

        public bool IsTagNameEnd
        {
            get
            {
                return (byte)(partMinor & TagPartMinor.EndName) == 6;
            }
        }

        public bool HasNameFragment
        {
            get
            {
                return !IsFragmentEmpty(name);
            }
        }

        public HtmlNameIndex NameIndex
        {
            get
            {
                return nameIndex;
            }
        }

        public TagNameTextReader Name
        {
            get
            {
                return new TagNameTextReader(this);
            }
        }

        public TagUnstructuredContentTextReader UnstructuredContent
        {
            get
            {
                return new TagUnstructuredContentTextReader(this);
            }
        }

        public HtmlTagIndex TagIndex
        {
            get
            {
                return tagIndex;
            }
        }

        public HtmlTagIndex OriginalTagId
        {
            get
            {
                return originalTagIndex;
            }
        }

        public bool IsAllowWspLeft
        {
            get
            {
                return (byte)(flags & TagFlags.AllowWspLeft) == 64;
            }
        }

        public bool IsAllowWspRight
        {
            get
            {
                return (byte)(flags & TagFlags.AllowWspRight) == 128;
            }
        }

        public AttributeEnumerator Attributes
        {
            get
            {
                return new AttributeEnumerator(this);
            }
        }

        public HtmlToken()
        {
            Reset();
        }

        internal new void Reset()
        {
            tagIndex = (originalTagIndex = HtmlTagIndex._NULL);
            nameIndex = HtmlNameIndex._NOTANAME;
            flags = TagFlags.None;
            partMajor = TagPartMajor.None;
            partMinor = TagPartMinor.Empty;
            name.Reset();
            unstructured.Reset();
            namePosition.Reset();
            unstructuredPosition.Reset();
            attributeTail = 0;
            currentAttribute = -1;
            attrNamePosition.Reset();
            attrValuePosition.Reset();
        }
    }
}
