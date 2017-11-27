using AgileFramework.Security.Exchange.CtsResources;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;
using System.Globalization;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class HtmlWriter : IRestartable, IFallback, IDisposable, ITextSinkEx, ITextSink
    {
        internal enum OutputState
        {
            OutsideTag,
            TagStarted,
            WritingUnstructuredTagContent,
            WritingTagName,
            BeforeAttribute,
            WritingAttributeName,
            AfterAttributeName,
            WritingAttributeValue
        }

        private ConverterOutput output;

        private OutputState outputState;

        private bool filterHtml;

        private bool autoNewLines;

        private bool allowWspBeforeFollowingTag;

        private bool lastWhitespace;

        private int lineLength;

        private int longestLineLength;

        private int textLineLength;

        private int literalWhitespaceNesting;

        private bool literalTags;

        private bool literalEntities;

        private bool cssEscaping;

        private IFallback fallback;

        private HtmlNameIndex tagNameIndex;

        private HtmlNameIndex previousTagNameIndex;

        private bool isEndTag;

        private bool isEmptyScopeTag;

        private bool copyPending;

        internal bool HasEncoding
        {
            get
            {
                return output is ConverterEncodingOutput;
            }
        }

        internal bool CodePageSameAsInput
        {
            get
            {
                return (output as ConverterEncodingOutput).CodePageSameAsInput;
            }
        }

        internal Encoding Encoding
        {
            get
            {
                return (output as ConverterEncodingOutput).Encoding;
            }
            set
            {
                (output as ConverterEncodingOutput).Encoding = value;
            }
        }

        internal bool IsTagOpen
        {
            get
            {
                return outputState != OutputState.OutsideTag;
            }
        }

        bool ITextSink.IsEnough
        {
            get
            {
                return false;
            }
        }

        internal HtmlWriter(ConverterOutput output, bool filterHtml, bool autoNewLines)
        {
            this.output = output;
            this.filterHtml = filterHtml;
            this.autoNewLines = autoNewLines;
        }

        public void Flush()
        {
            if (copyPending)
            {
                throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
            }
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            output.Flush();
        }

        internal void SetCopyPending(bool copyPending)
        {
            this.copyPending = copyPending;
        }

        internal void WriteStartTag(HtmlNameIndex nameIndex)
        {
            WriteTagBegin(nameIndex, null, false, false, false);
        }

        internal void WriteEndTag(HtmlNameIndex nameIndex)
        {
            WriteTagBegin(nameIndex, null, true, false, false);
            WriteTagEnd();
        }

        internal void WriteTagBegin(HtmlNameIndex nameIndex, string name, bool isEndTag, bool allowWspLeft, bool allowWspRight)
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            if (literalTags && nameIndex >= HtmlNameIndex.Unknown && (!isEndTag || nameIndex != tagNameIndex))
            {
                throw new InvalidOperationException(TextConvertersStrings.CannotWriteOtherTagsInsideElement(HtmlNameData.names[(int)tagNameIndex].name));
            }
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)nameIndex].tagIndex;
            if (nameIndex > HtmlNameIndex.Unknown)
            {
                isEmptyScopeTag = (HtmlDtd.tags[(int)tagIndex].scope == HtmlDtd.TagScope.EMPTY);
                if (isEndTag && isEmptyScopeTag)
                {
                    if (HtmlDtd.tags[(int)tagIndex].unmatchedSubstitute != HtmlTagIndex._IMPLICIT_BEGIN)
                    {
                        output.Write("<!-- </");
                        lineLength += 7;
                        if (nameIndex > HtmlNameIndex.Unknown)
                        {
                            output.Write(HtmlNameData.names[(int)nameIndex].name);
                            lineLength += HtmlNameData.names[(int)nameIndex].name.Length;
                        }
                        else
                        {
                            output.Write((name != null) ? name : "???");
                            lineLength += ((name != null) ? name.Length : 3);
                        }
                        output.Write("> ");
                        lineLength += 2;
                        tagNameIndex = HtmlNameIndex._COMMENT;
                        outputState = OutputState.WritingUnstructuredTagContent;
                        return;
                    }
                    isEndTag = false;
                }
            }
            if (autoNewLines && literalWhitespaceNesting == 0)
            {
                bool flag = lastWhitespace;
                HtmlDtd.TagFill fill = HtmlDtd.tags[(int)tagIndex].fill;
                if (lineLength != 0)
                {
                    HtmlDtd.TagFmt fmt = HtmlDtd.tags[(int)tagIndex].fmt;
                    if ((!isEndTag && fmt.LB == HtmlDtd.FmtCode.BRK) || (isEndTag && fmt.LE == HtmlDtd.FmtCode.BRK) || (lineLength > 80 && (lastWhitespace || allowWspBeforeFollowingTag || (!isEndTag && fill.LB == HtmlDtd.FillCode.EAT) || (isEndTag && fill.LE == HtmlDtd.FillCode.EAT))))
                    {
                        if (lineLength > longestLineLength)
                        {
                            longestLineLength = lineLength;
                        }
                        output.Write("\r\n");
                        lineLength = 0;
                        lastWhitespace = false;
                    }
                }
                allowWspBeforeFollowingTag = (((!isEndTag && fill.RB == HtmlDtd.FillCode.EAT) || (isEndTag && fill.RE == HtmlDtd.FillCode.EAT) || (flag && ((!isEndTag && fill.RB == HtmlDtd.FillCode.NUL) || (isEndTag && fill.RE == HtmlDtd.FillCode.NUL)))) && (nameIndex != HtmlNameIndex.Body || !isEndTag));
            }
            if (lastWhitespace)
            {
                output.Write(' ');
                lineLength++;
                lastWhitespace = false;
            }
            if (HtmlDtd.tags[(int)tagIndex].blockElement || tagIndex == HtmlTagIndex.BR)
            {
                textLineLength = 0;
            }
            output.Write('<');
            lineLength++;
            if (nameIndex >= HtmlNameIndex.Unknown)
            {
                if (isEndTag)
                {
                    if ((byte)(HtmlDtd.tags[(int)tagIndex].literal & HtmlDtd.Literal.Tags) != 0)
                    {
                        literalTags = false;
                        literalEntities = false;
                        cssEscaping = false;
                    }
                    if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                    {
                        literalWhitespaceNesting--;
                    }
                    output.Write('/');
                    lineLength++;
                }
                if (nameIndex != HtmlNameIndex.Unknown)
                {
                    output.Write(HtmlNameData.names[(int)nameIndex].name);
                    lineLength += HtmlNameData.names[(int)nameIndex].name.Length;
                    outputState = OutputState.BeforeAttribute;
                }
                else
                {
                    if (name != null)
                    {
                        output.Write(name);
                        lineLength += name.Length;
                        outputState = OutputState.BeforeAttribute;
                    }
                    else
                    {
                        outputState = OutputState.TagStarted;
                    }
                    isEmptyScopeTag = false;
                }
            }
            else
            {
                previousTagNameIndex = tagNameIndex;
                if (nameIndex == HtmlNameIndex._COMMENT)
                {
                    output.Write("!--");
                    lineLength += 3;
                }
                else if (nameIndex == HtmlNameIndex._ASP)
                {
                    output.Write('%');
                    lineLength++;
                }
                else if (nameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    output.Write("!--[");
                    lineLength += 4;
                }
                else if (nameIndex == HtmlNameIndex._DTD)
                {
                    output.Write('?');
                    lineLength++;
                }
                else
                {
                    output.Write('!');
                    lineLength++;
                }
                outputState = OutputState.WritingUnstructuredTagContent;
                isEmptyScopeTag = true;
            }
            tagNameIndex = nameIndex;
            this.isEndTag = isEndTag;
        }

        internal void WriteTagEnd()
        {
            WriteTagEnd(isEmptyScopeTag);
        }

        internal void WriteTagEnd(bool emptyScopeTag)
        {
            HtmlTagIndex tagIndex = HtmlNameData.names[(int)tagNameIndex].tagIndex;
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }
            if (tagNameIndex > HtmlNameIndex.Unknown)
            {
                output.Write('>');
                lineLength++;
            }
            else
            {
                if (tagNameIndex == HtmlNameIndex._COMMENT)
                {
                    output.Write("-->");
                    lineLength += 3;
                }
                else if (tagNameIndex == HtmlNameIndex._ASP)
                {
                    output.Write("%>");
                    lineLength += 2;
                }
                else if (tagNameIndex == HtmlNameIndex._CONDITIONAL)
                {
                    output.Write("]-->");
                    lineLength += 4;
                }
                else if (tagNameIndex == HtmlNameIndex.Unknown && emptyScopeTag)
                {
                    output.Write(" />");
                    lineLength += 3;
                }
                else
                {
                    output.Write('>');
                    lineLength++;
                }
                tagNameIndex = previousTagNameIndex;
            }
            if (isEndTag && (tagIndex == HtmlTagIndex.LI || tagIndex == HtmlTagIndex.DD || tagIndex == HtmlTagIndex.DT))
            {
                lineLength = 0;
            }
            if (autoNewLines && literalWhitespaceNesting == 0)
            {
                HtmlDtd.TagFmt fmt = HtmlDtd.tags[(int)tagIndex].fmt;
                HtmlDtd.TagFill fill = HtmlDtd.tags[(int)tagIndex].fill;
                if ((!isEndTag && fmt.RB == HtmlDtd.FmtCode.BRK) || (isEndTag && fmt.RE == HtmlDtd.FmtCode.BRK) || (lineLength > 80 && (allowWspBeforeFollowingTag || (!isEndTag && fill.RB == HtmlDtd.FillCode.EAT) || (isEndTag && fill.RE == HtmlDtd.FillCode.EAT))))
                {
                    if (lineLength > longestLineLength)
                    {
                        longestLineLength = lineLength;
                    }
                    output.Write("\r\n");
                    lineLength = 0;
                }
            }
            if (!isEndTag && !emptyScopeTag)
            {
                HtmlDtd.Literal literal = HtmlDtd.tags[(int)tagIndex].literal;
                if ((byte)(literal & HtmlDtd.Literal.Tags) != 0)
                {
                    literalTags = true;
                    literalEntities = (0 != (byte)(literal & HtmlDtd.Literal.Entities));
                    cssEscaping = (tagIndex == HtmlTagIndex.Style);
                }
                if (HtmlDtd.tags[(int)tagIndex].contextTextType == HtmlDtd.ContextTextType.Literal)
                {
                    literalWhitespaceNesting++;
                }
            }
            outputState = OutputState.OutsideTag;
        }

        internal void WriteAttribute(HtmlNameIndex nameIndex, string value)
        {
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }
            OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
            if (value != null)
            {
                OutputAttributeValue(value);
                OutputAttributeEnd();
            }
            outputState = OutputState.BeforeAttribute;
        }

        internal void WriteAttributeName(HtmlNameIndex nameIndex)
        {
            if (outputState > OutputState.BeforeAttribute)
            {
                OutputAttributeEnd();
            }
            OutputAttributeName(HtmlNameData.names[(int)nameIndex].name);
        }

        internal void WriteAttributeValueInternal(string value)
        {
            OutputAttributeValue(value);
        }

        internal void WriteAttributeValueInternal(char[] buffer, int index, int count)
        {
            OutputAttributeValue(buffer, index, count);
        }

        public void WriteMarkupText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (copyPending)
            {
                throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
            }
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            if (lastWhitespace)
            {
                OutputLastWhitespace(value[0]);
            }
            output.Write(value, null);
            lineLength += value.Length;
            allowWspBeforeFollowingTag = false;
        }

        internal ITextSinkEx WriteUnstructuredTagContent()
        {
            fallback = null;
            return this;
        }

        internal ITextSinkEx WriteTagName()
        {
            outputState = OutputState.WritingTagName;
            fallback = null;
            return this;
        }

        internal ITextSinkEx WriteAttributeName()
        {
            if (outputState != OutputState.WritingAttributeName)
            {
                if (outputState > OutputState.BeforeAttribute)
                {
                    OutputAttributeEnd();
                }
                output.Write(' ');
                lineLength++;
            }
            outputState = OutputState.WritingAttributeName;
            fallback = null;
            return this;
        }

        internal ITextSinkEx WriteAttributeValue()
        {
            if (outputState != OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }
            outputState = OutputState.WritingAttributeValue;
            fallback = this;
            return this;
        }

        internal ITextSinkEx WriteText()
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            allowWspBeforeFollowingTag = false;
            if (lastWhitespace)
            {
                OutputLastWhitespace('\u3000');
            }
            fallback = this;
            return this;
        }

        internal void WriteNewLine()
        {
            WriteNewLine(false);
        }

        internal void WriteNewLine(bool optional)
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            if (!optional || (lineLength != 0 && literalWhitespaceNesting == 0))
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }
                output.Write("\r\n");
                lineLength = 0;
                lastWhitespace = false;
                allowWspBeforeFollowingTag = false;
            }
        }

        internal void WriteAutoNewLine()
        {
            WriteNewLine(false);
        }

        internal void WriteAutoNewLine(bool optional)
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            if (autoNewLines && (!optional || (lineLength != 0 && literalWhitespaceNesting == 0)))
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }
                output.Write("\r\n");
                lineLength = 0;
                lastWhitespace = false;
                allowWspBeforeFollowingTag = false;
            }
        }

        internal void WriteCollapsedWhitespace()
        {
            if (outputState != OutputState.OutsideTag)
            {
                WriteTagEnd();
            }
            lastWhitespace = true;
            allowWspBeforeFollowingTag = false;
        }

        private void OutputLastWhitespace(char nextChar)
        {
            if (lineLength > 255 && autoNewLines)
            {
                if (lineLength > longestLineLength)
                {
                    longestLineLength = lineLength;
                }
                lineLength = 0;
                if (ParseSupport.FarEastNonHanguelChar(nextChar))
                {
                    output.Write(' ');
                    lineLength++;
                }
            }
            else
            {
                output.Write(' ');
                lineLength++;
            }
            textLineLength++;
            lastWhitespace = false;
        }

        private void OutputAttributeName(string name)
        {
            output.Write(' ');
            output.Write(name);
            lineLength += name.Length + 1;
            outputState = OutputState.AfterAttributeName;
        }

        private void OutputAttributeValue(string value)
        {
            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }
            output.Write(value, this);
            lineLength += value.Length;
            outputState = OutputState.WritingAttributeValue;
        }

        private void OutputAttributeValue(char[] value, int index, int count)
        {
            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }
            output.Write(value, index, count, this);
            lineLength += count;
            outputState = OutputState.WritingAttributeValue;
        }

        private void OutputAttributeEnd()
        {
            if (outputState < OutputState.WritingAttributeValue)
            {
                output.Write("=\"");
                lineLength += 2;
            }
            output.Write('"');
            lineLength++;
        }

        bool IRestartable.CanRestart()
        {
            return output is IRestartable && ((IRestartable)output).CanRestart();
        }

        void IRestartable.Restart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).Restart();
            }
            allowWspBeforeFollowingTag = false;
            lastWhitespace = false;
            lineLength = 0;
            longestLineLength = 0;
            literalWhitespaceNesting = 0;
            literalTags = false;
            literalEntities = false;
            cssEscaping = false;
            tagNameIndex = HtmlNameIndex._NOTANAME;
            previousTagNameIndex = HtmlNameIndex._NOTANAME;
            isEndTag = false;
            isEmptyScopeTag = false;
            copyPending = false;
            outputState = OutputState.OutsideTag;
        }

        void IRestartable.DisableRestart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (literalEntities)
            {
                unsafeAsciiMask = 0;
                return null;
            }
            if (filterHtml)
            {
                unsafeAsciiMask = 1;
            }
            else
            {
                unsafeAsciiMask = 1;
            }
            return HtmlSupport.UnsafeAsciiMap;
        }

        bool IFallback.HasUnsafeUnicode()
        {
            return filterHtml;
        }

        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return filterHtml && charset.StartsWith("x-", StringComparison.OrdinalIgnoreCase);
        }

        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return filterHtml && ((byte)(ch & 'ÿ') == 60 || (byte)(ch >> 8 & 'ÿ') == 60 || (!isFirstChar && ch == '﻿') || char.GetUnicodeCategory(ch) == UnicodeCategory.PrivateUse);
        }

        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (literalEntities)
            {
                if (cssEscaping)
                {
                    uint num = (uint)ch;
                    int num2 = (num < 16u) ? 1 : ((num < 256u) ? 2 : ((num < 4096u) ? 3 : 4));
                    if (outputEnd - outputBufferCount < num2 + 2)
                    {
                        return false;
                    }
                    outputBuffer[outputBufferCount++] = '\\';
                    int num3 = outputBufferCount + num2;
                    while (num != 0u)
                    {
                        uint num4 = num & 15u;
                        outputBuffer[--num3] = (char)((ulong)num4 + (ulong)((num4 < 10u) ? 48L : 55L));
                        num >>= 4;
                    }
                    outputBufferCount += num2;
                    outputBuffer[outputBufferCount++] = ' ';
                }
                else
                {
                    if (outputEnd - outputBufferCount < 1)
                    {
                        return false;
                    }
                    outputBuffer[outputBufferCount++] = (filterHtml ? '?' : ch);
                }
            }
            else
            {
                HtmlEntityIndex htmlEntityIndex = (HtmlEntityIndex)0;
                if (ch <= '>')
                {
                    if (ch == '>')
                    {
                        htmlEntityIndex = HtmlEntityIndex.gt;
                    }
                    else if (ch == '<')
                    {
                        htmlEntityIndex = HtmlEntityIndex.lt;
                    }
                    else if (ch == '&')
                    {
                        htmlEntityIndex = HtmlEntityIndex.amp;
                    }
                    else if (ch == '"')
                    {
                        htmlEntityIndex = HtmlEntityIndex.quot;
                    }
                }
                else if ('\u00a0' <= ch && ch <= 'ÿ')
                {
                    htmlEntityIndex = HtmlSupport.EntityMap[(int)(ch - '\u00a0')];
                }
                if (htmlEntityIndex != (HtmlEntityIndex)0)
                {
                    string name = HtmlNameData.entities[(int)htmlEntityIndex].name;
                    if (outputEnd - outputBufferCount < name.Length + 2)
                    {
                        return false;
                    }
                    outputBuffer[outputBufferCount++] = '&';
                    name.CopyTo(0, outputBuffer, outputBufferCount, name.Length);
                    outputBufferCount += name.Length;
                    outputBuffer[outputBufferCount++] = ';';
                }
                else
                {
                    uint num5 = (uint)ch;
                    int num6 = (num5 < 10u) ? 1 : ((num5 < 100u) ? 2 : ((num5 < 1000u) ? 3 : ((num5 < 10000u) ? 4 : 5)));
                    if (outputEnd - outputBufferCount < num6 + 3)
                    {
                        return false;
                    }
                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';
                    int num7 = outputBufferCount + num6;
                    while (num5 != 0u)
                    {
                        uint num8 = num5 % 10u;
                        outputBuffer[--num7] = (char)(num8 + 48u);
                        num5 /= 10u;
                    }
                    outputBufferCount += num6;
                    outputBuffer[outputBufferCount++] = ';';
                }
            }
            return true;
        }

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            lineLength += count;
            textLineLength += count;
            output.Write(buffer, offset, count, fallback);
        }

        void ITextSink.Write(int ucs32Char)
        {
            lineLength++;
            textLineLength++;
            output.Write(ucs32Char, fallback);
        }

        void ITextSinkEx.Write(string text)
        {
            lineLength += text.Length;
            textLineLength += text.Length;
            output.Write(text, fallback);
        }

        void ITextSinkEx.WriteNewLine()
        {
            if (lineLength > longestLineLength)
            {
                longestLineLength = lineLength;
            }
            output.Write("\r\n");
            lineLength = 0;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (output != null)
                {
                    if (!copyPending)
                    {
                        Flush();
                    }
                    if (output != null)
                    {
                        ((IDisposable)output).Dispose();
                    }
                }
                GC.SuppressFinalize(this);
            }
            output = null;
        }
    }
}
