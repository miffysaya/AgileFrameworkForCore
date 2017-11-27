using AgileFramework.Security.Exchange.Data.Globalization;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Text
{
    /// <summary>
    /// Manages the preparation of encoding output.
    /// </summary>
    internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
    {
        /// <summary>
        /// Array of white space characters.
        /// </summary>
        private static readonly char[] Whitespaces = new char[]
        {
            ' ',
            '\t',
            '\r',
            '\n',
            '\f'
        };

        /// <summary>
        /// The encoded output.
        /// </summary>
        private ConverterOutput output;

        /// <summary>
        /// Use line wrapping.
        /// </summary>
        private bool lineWrapping;

        /// <summary>
        /// Use the text-plain format parameter.
        /// </summary>
        private bool rfc2646;

        /// <summary>
        /// The offset of the longest non wrapped paragraph found.
        /// </summary>
        private int longestNonWrappedParagraph;

        /// <summary>
        /// The offset before the wrap.
        /// </summary>
        private int wrapBeforePosition;

        /// <summary>
        /// Preserve trailing spaces.
        /// </summary>
        private bool preserveTrailingSpace;

        /// <summary>
        /// Preserve the tabs.
        /// </summary>
        private bool preserveTabulation;

        /// <summary>
        /// Preserve the non break spaces.
        /// </summary>
        private bool preserveNbsp;

        /// <summary>
        /// The length of the line.
        /// </summary>
        private int lineLength;

        /// <summary>
        /// The length of the line before the soft wrap.
        /// </summary>
        private int lineLengthBeforeSoftWrap;

        /// <summary>
        /// The lenght flushed.
        /// </summary>
        private int flushedLength;

        /// <summary>
        /// Number of tail spaces.
        /// </summary>
        private int tailSpace;

        /// <summary>
        /// The break opportunity.
        /// </summary>
        private int breakOpportunity;

        /// <summary>
        /// The next break opportunity.
        /// </summary>
        private int nextBreakOpportunity;

        /// <summary>
        /// The level of quotes.
        /// </summary>
        private int quotingLevel;

        /// <summary>
        /// True if buffer is wrapped.
        /// </summary>
        private bool wrapped;

        /// <summary>
        /// The wrap buffer.
        /// </summary>
        private char[] wrapBuffer;

        /// <summary>
        /// Possible signature.
        /// </summary>
        private bool signaturePossible = true;

        /// <summary>
        /// True if new lines.
        /// </summary>
        private bool anyNewlines;

        /// <summary>
        /// True if end of paragraph found.
        /// </summary>
        private bool endParagraph;

        /// <summary>
        /// When <c>true</c> write this object; otherwise, write null.
        /// </summary>
        private bool fallbacks;

        /// <summary>
        /// True if escaping html.
        /// </summary>
        private bool htmlEscape;

        /// <summary>
        /// The anchor URL.
        /// </summary>
        private string anchorUrl;

        /// <summary>
        /// The line position.
        /// </summary>
        private int linePosition;

        /// <summary>
        /// The image rendering callback delegate.
        /// </summary>
        private ImageRenderingCallbackInternal imageRenderingCallback;

        /// <summary>
        /// Sets the anchor Url.
        /// </summary>
        /// <value>The anchor Url.</value>
        internal string AnchorUrl
        {
            set
            {
                anchorUrl = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether output code page is same as input.
        /// </summary>
        /// <value>
        /// <c>true</c> if the output code page is the same as input; otherwise, <c>false</c>.
        /// </value>
        internal bool OutputCodePageSameAsInput
        {
            get
            {
                return output is ConverterEncodingOutput && (output as ConverterEncodingOutput).CodePageSameAsInput;
            }
        }

        /// <summary>
        /// Sets the output encoding.
        /// </summary>
        /// <value>The output encoding.</value>
        internal Encoding OutputEncoding
        {
            set
            {
                if (output is ConverterEncodingOutput)
                {
                    (output as ConverterEncodingOutput).Encoding = value;
                    return;
                }
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether image rendering callback is defined.
        /// </summary>
        /// <value>
        /// <c>true</c> if image rendering callback is defined; otherwise, <c>false</c>.
        /// </value>
        internal bool ImageRenderingCallbackDefined
        {
            get
            {
                return imageRenderingCallback != null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.TextConverters.Internal.Text.TextOutput" /> class.
        /// </summary>
        /// <param name="output">The converted output.</param>
        /// <param name="lineWrapping">if set to <c>true</c> allow line wrapping.</param>
        /// <param name="flowed">if set to <c>true</c> allow flowing.</param>
        /// <param name="wrapBeforePosition">The wrap before position.</param>
        /// <param name="longestNonWrappedParagraph">The longest non wrapped paragraph.</param>
        /// <param name="imageRenderingCallback">The image rendering callback.</param>
        /// <param name="fallbacks">if set to <c>true</c> allow fallbacks.</param>
        /// <param name="htmlEscape">if set to <c>true</c> escape HTML.</param>
        /// <param name="preserveSpace">if set to <c>true</c> preserve spaces.</param>
        /// <param name="testTraceStream">The test trace stream.</param>
        public TextOutput(ConverterOutput output, bool lineWrapping, bool flowed, int wrapBeforePosition, int longestNonWrappedParagraph, ImageRenderingCallbackInternal imageRenderingCallback, bool fallbacks, bool htmlEscape, bool preserveSpace, Stream testTraceStream)
        {
            rfc2646 = flowed;
            this.lineWrapping = lineWrapping;
            this.wrapBeforePosition = wrapBeforePosition;
            this.longestNonWrappedParagraph = longestNonWrappedParagraph;
            if (!this.lineWrapping)
            {
                preserveTrailingSpace = preserveSpace;
                preserveTabulation = preserveSpace;
                preserveNbsp = preserveSpace;
            }
            this.output = output;
            this.fallbacks = fallbacks;
            this.htmlEscape = htmlEscape;
            this.imageRenderingCallback = imageRenderingCallback;
            wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
        }

        /// <summary>
        /// Gets the unsafe ASCII map.
        /// </summary>
        /// <param name="unsafeAsciiMask">The unsafe ASCII mask.</param>
        /// <returns>
        /// The unsafe ASCII mask byte[].
        /// </returns>
        byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
        {
            if (htmlEscape)
            {
                unsafeAsciiMask = 1;
                return HtmlSupport.UnsafeAsciiMap;
            }
            unsafeAsciiMask = 0;
            return null;
        }

        /// <summary>
        /// Determines whether has unsafe unicode.
        /// </summary>
        /// <returns>
        /// <c>true</c> if has unsafe unicode; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.HasUnsafeUnicode()
        {
            return htmlEscape;
        }

        /// <summary>
        /// Treat the non ASCII as unsafe.
        /// </summary>
        /// <param name="charset">The charset.</param>
        /// <returns>
        /// Always returns False
        /// </returns>
        bool IFallback.TreatNonAsciiAsUnsafe(string charset)
        {
            return false;
        }

        /// <summary>
        /// Determines whether is unsafe unicode.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <param name="isFirstChar">if set to <c>true</c> is first character.</param>
        /// <returns>
        /// <c>true</c> if is unsafe unicode; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
        {
            return htmlEscape && ((byte)(ch & 'ÿ') == 60 || (byte)(ch >> 8 & 'ÿ') == 60);
        }

        /// <summary>
        /// Encodes the character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        /// <param name="outputBuffer">The output buffer.</param>
        /// <param name="outputBufferCount">The output buffer count.</param>
        /// <param name="outputEnd">The output end.</param>
        /// <returns>
        /// <c>true</c> if encoding is successful; otherwise, <c>false</c>.
        /// </returns>
        bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
        {
            if (htmlEscape)
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
                    uint num = (uint)ch;
                    int num2 = (num < 16u) ? 1 : ((num < 256u) ? 2 : ((num < 4096u) ? 3 : 4));
                    if (outputEnd - outputBufferCount < num2 + 4)
                    {
                        return false;
                    }
                    outputBuffer[outputBufferCount++] = '&';
                    outputBuffer[outputBufferCount++] = '#';
                    outputBuffer[outputBufferCount++] = 'x';
                    int num3 = outputBufferCount + num2;
                    while (num != 0u)
                    {
                        uint num4 = num & 15u;
                        outputBuffer[--num3] = (char)((ulong)num4 + (ulong)((num4 < 10u) ? 48L : 55L));
                        num >>= 4;
                    }
                    outputBufferCount += num2;
                    outputBuffer[outputBufferCount++] = ';';
                }
            }
            else
            {
                string characterFallback = AsciiEncoderFallback.GetCharacterFallback(ch);
                if (characterFallback != null)
                {
                    if (outputEnd - outputBufferCount < characterFallback.Length)
                    {
                        return false;
                    }
                    characterFallback.CopyTo(0, outputBuffer, outputBufferCount, characterFallback.Length);
                    outputBufferCount += characterFallback.Length;
                }
                else
                {
                    outputBuffer[outputBufferCount++] = ch;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (output != null)
            {
                ((IDisposable)output).Dispose();
            }
            output = null;
            wrapBuffer = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines whether this instance can restart.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can restart; otherwise, <c>false</c>.
        /// </returns>
        bool IRestartable.CanRestart()
        {
            return output is IRestartable && ((IRestartable)output).CanRestart();
        }

        /// <summary>
        /// Restarts this instance.
        /// </summary>
        void IRestartable.Restart()
        {
            ((IRestartable)output).Restart();
            Reinitialize();
        }

        /// <summary>
        /// Disables the restart.
        /// </summary>
        void IRestartable.DisableRestart()
        {
            if (output is IRestartable)
            {
                ((IRestartable)output).DisableRestart();
            }
        }

        /// <summary>
        /// Initializes the specified new source or destination.
        /// </summary>
        /// <param name="newSourceOrDestination">The new source or destination.</param>
        void IReusable.Initialize(object newSourceOrDestination)
        {
            ((IReusable)output).Initialize(newSourceOrDestination);
            Reinitialize();
        }

        /// <summary>
        /// Add line terminator to output.
        /// </summary>
        internal void CloseDocument()
        {
            if (!anyNewlines)
            {
                output.Write("\r\n");
            }
            endParagraph = false;
        }

        /// <summary>
        /// Add new line to the output.
        /// </summary>
        internal void OutputNewLine()
        {
            if (lineWrapping)
            {
                FlushLine('\n');
                if (signaturePossible && lineLength == 2 && tailSpace == 1)
                {
                    output.Write(' ');
                    lineLength++;
                }
            }
            else if (preserveTrailingSpace && tailSpace != 0)
            {
                FlushTailSpace();
            }
            if (!endParagraph)
            {
                output.Write("\r\n");
                anyNewlines = true;
                linePosition += 2;
            }
            linePosition += lineLength;
            lineLength = 0;
            lineLengthBeforeSoftWrap = 0;
            flushedLength = 0;
            tailSpace = 0;
            breakOpportunity = 0;
            nextBreakOpportunity = 0;
            wrapped = false;
            signaturePossible = true;
        }

        /// <summary>
        /// Add a tab to the output.
        /// </summary>
        /// <param name="count">The count.</param>
        internal void OutputTabulation(int count)
        {
            if (preserveTabulation)
            {
                while (count != 0)
                {
                    OutputNonspace("\t", TextMapping.Unicode);
                    count--;
                }
                return;
            }
            int num = (lineLengthBeforeSoftWrap + lineLength + tailSpace) / 8 * 8 + 8 * count;
            count = num - (lineLengthBeforeSoftWrap + lineLength + tailSpace);
            OutputSpace(count);
        }

        /// <summary>
        /// Add spaces to the output.
        /// </summary>
        /// <param name="count">The number of spaces to add.</param>
        internal void OutputSpace(int count)
        {
            if (lineWrapping)
            {
                if (breakOpportunity == 0 || lineLength + tailSpace <= WrapBeforePosition())
                {
                    breakOpportunity = lineLength + tailSpace;
                    if (lineLength + tailSpace < WrapBeforePosition() && count > 1)
                    {
                        breakOpportunity += Math.Min(WrapBeforePosition() - (lineLength + tailSpace), count - 1);
                    }
                    if (breakOpportunity < lineLength + tailSpace + count - 1)
                    {
                        nextBreakOpportunity = lineLength + tailSpace + count - 1;
                    }
                    if (lineLength > flushedLength)
                    {
                        FlushLine(' ');
                    }
                }
                else
                {
                    nextBreakOpportunity = lineLength + tailSpace + count - 1;
                }
            }
            tailSpace += count;
        }

        /// <summary>
        /// Add non breaking spaces to the output.
        /// </summary>
        /// <param name="count">The number of non breaking spaces to add.</param>
        internal void OutputNbsp(int count)
        {
            if (preserveNbsp)
            {
                while (count != 0)
                {
                    OutputNonspace("\u00a0", TextMapping.Unicode);
                    count--;
                }
                return;
            }
            tailSpace += count;
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="buffer">The buffer to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (!lineWrapping && !endParagraph && textMapping == TextMapping.Unicode)
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }
                output.Write(buffer, offset, count, fallbacks ? this : null);
                lineLength += count;
                return;
            }
            OutputNonspaceImpl(buffer, offset, count, textMapping);
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="length">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
        {
            if (textMapping != TextMapping.Unicode)
            {
                for (int i = offset; i < length; i++)
                {
                    MapAndOutputSymbolCharacter(text[i], textMapping);
                }
                return;
            }
            if (endParagraph)
            {
                output.Write("\r\n");
                linePosition += 2;
                anyNewlines = true;
                endParagraph = false;
            }
            if (lineWrapping)
            {
                if (length != 0)
                {
                    WrapPrepareToAppendNonspace(length);
                    if (breakOpportunity == 0)
                    {
                        FlushLine(text[offset]);
                        output.Write(text, offset, length, fallbacks ? this : null);
                        flushedLength += length;
                    }
                    else
                    {
                        text.CopyTo(offset, wrapBuffer, lineLength - flushedLength, length);
                    }
                    lineLength += length;
                    if (lineLength > 2 || text[offset] != '-' || (length == 2 && text[offset + 1] != '-'))
                    {
                        signaturePossible = false;
                        return;
                    }
                }
            }
            else
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }
                output.Write(text, offset, length, fallbacks ? this : null);
                lineLength += length;
            }
        }

        /// <summary>
        /// Perform encoding.
        /// </summary>
        /// <param name="ucs32Literal">The ucs32 literal to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(int ucs32Literal, TextMapping textMapping)
        {
            if (textMapping != TextMapping.Unicode)
            {
                MapAndOutputSymbolCharacter((char)ucs32Literal, textMapping);
                return;
            }
            if (endParagraph)
            {
                output.Write("\r\n");
                linePosition += 2;
                anyNewlines = true;
                endParagraph = false;
            }
            if (lineWrapping)
            {
                int num = Token.LiteralLength(ucs32Literal);
                WrapPrepareToAppendNonspace(num);
                if (breakOpportunity == 0)
                {
                    FlushLine(Token.LiteralFirstChar(ucs32Literal));
                    output.Write(ucs32Literal, fallbacks ? this : null);
                    flushedLength += num;
                }
                else
                {
                    wrapBuffer[lineLength - flushedLength] = Token.LiteralFirstChar(ucs32Literal);
                    if (num != 1)
                    {
                        wrapBuffer[lineLength - flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
                    }
                }
                lineLength += num;
                if (lineLength > 2 || num != 1 || (ushort)ucs32Literal != 45)
                {
                    signaturePossible = false;
                    return;
                }
            }
            else
            {
                if (tailSpace != 0)
                {
                    FlushTailSpace();
                }
                output.Write(ucs32Literal, fallbacks ? this : null);
                lineLength += Token.LiteralLength(ucs32Literal);
            }
        }

        /// <summary>
        /// Flushes the converted output.
        /// </summary>
        internal void Flush()
        {
            if (lineWrapping)
            {
                if (lineLength != 0)
                {
                    FlushLine('\r');
                    OutputNewLine();
                }
            }
            else if (lineLength != 0)
            {
                OutputNewLine();
            }
            output.Flush();
        }

        /// <summary>
        /// Perform image encoding.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="imageAltText">The image alt text.</param>
        /// <param name="wdthPixels">The image width in pixels.</param>
        /// <param name="heightPixels">The image height in pixels.</param>
        internal void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
        {
            if (imageRenderingCallback != null && imageRenderingCallback(imageUrl, RenderingPosition()))
            {
                OutputSpace(1);
                return;
            }
            if ((wdthPixels == 0 || wdthPixels >= 8) && (heightPixels == 0 || heightPixels >= 8))
            {
                bool flag = tailSpace != 0;
                OutputNonspace("[", TextMapping.Unicode);
                if (!string.IsNullOrEmpty(imageAltText))
                {
                    int num2;
                    for (int num = 0; num != imageAltText.Length; num = num2 + 1)
                    {
                        num2 = imageAltText.IndexOfAny(TextOutput.Whitespaces, num);
                        if (num2 == -1)
                        {
                            OutputNonspace(imageAltText, num, imageAltText.Length - num, TextMapping.Unicode);
                            break;
                        }
                        if (num2 != num)
                        {
                            OutputNonspace(imageAltText, num, num2 - num, TextMapping.Unicode);
                        }
                        if (imageAltText[num] == '\t')
                        {
                            OutputTabulation(1);
                        }
                        else
                        {
                            OutputSpace(1);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(imageUrl))
                {
                    if (imageUrl.Contains("/") && !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        imageUrl = "X";
                    }
                    else if (imageUrl.IndexOf(' ') != -1)
                    {
                        imageUrl = imageUrl.Replace(" ", "%20");
                    }
                    OutputNonspace(imageUrl, TextMapping.Unicode);
                }
                else
                {
                    OutputNonspace("X", TextMapping.Unicode);
                }
                OutputNonspace("]", TextMapping.Unicode);
                if (flag)
                {
                    OutputSpace(1);
                }
            }
        }

        /// <summary>
        /// Add non spaces to the output.
        /// </summary>
        /// <param name="text">The text the replace with non spaces.</param>
        /// <param name="textMapping">The text mapping.</param>
        internal void OutputNonspace(string text, TextMapping textMapping)
        {
            OutputNonspace(text, 0, text.Length, textMapping);
        }

        /// <summary>
        /// Closes the anchor.
        /// </summary>
        internal void CloseAnchor()
        {
            if (!string.IsNullOrEmpty(anchorUrl))
            {
                bool flag = tailSpace != 0;
                string text = anchorUrl;
                if (text.IndexOf(' ') != -1)
                {
                    text = text.Replace(" ", "%20");
                }
                OutputNonspace("<", TextMapping.Unicode);
                OutputNonspace(text, TextMapping.Unicode);
                OutputNonspace(">", TextMapping.Unicode);
                if (flag)
                {
                    OutputSpace(1);
                }
                CancelAnchor();
            }
        }

        /// <summary>
        /// Resets the anchor to null.
        /// </summary>
        internal void CancelAnchor()
        {
            anchorUrl = null;
        }

        /// <summary>
        /// Renderings the position.
        /// </summary>
        /// <returns>
        /// Offset for the rendering position.
        /// </returns>
        private int RenderingPosition()
        {
            return linePosition + lineLength + tailSpace;
        }

        /// <summary>
        /// Reinitializes this instance.
        /// </summary>
        private void Reinitialize()
        {
            anchorUrl = null;
            linePosition = 0;
            lineLength = 0;
            lineLengthBeforeSoftWrap = 0;
            flushedLength = 0;
            tailSpace = 0;
            breakOpportunity = 0;
            nextBreakOpportunity = 0;
            quotingLevel = 0;
            wrapped = false;
            signaturePossible = true;
            anyNewlines = false;
            endParagraph = false;
        }

        /// <summary>
        /// Perform encoding while handling end of paragraph and line wrapping.
        /// </summary>
        /// <param name="buffer">The buffer to encode.</param>
        /// <param name="offset">The offset to start encoding at.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
        {
            if (count != 0)
            {
                if (textMapping != TextMapping.Unicode)
                {
                    for (int i = 0; i < count; i++)
                    {
                        MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
                    }
                    return;
                }
                if (endParagraph)
                {
                    output.Write("\r\n");
                    linePosition += 2;
                    anyNewlines = true;
                    endParagraph = false;
                }
                if (lineWrapping)
                {
                    WrapPrepareToAppendNonspace(count);
                    if (breakOpportunity == 0)
                    {
                        FlushLine(buffer[offset]);
                        output.Write(buffer, offset, count, fallbacks ? this : null);
                        flushedLength += count;
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset * 2, wrapBuffer, (lineLength - flushedLength) * 2, count * 2);
                    }
                    lineLength += count;
                    if (lineLength > 2 || buffer[offset] != '-' || (count == 2 && buffer[offset + 1] != '-'))
                    {
                        signaturePossible = false;
                        return;
                    }
                }
                else
                {
                    if (tailSpace != 0)
                    {
                        FlushTailSpace();
                    }
                    output.Write(buffer, offset, count, fallbacks ? this : null);
                    lineLength += count;
                }
            }
        }

        /// <summary>
        /// Finds offset before the wrap.
        /// </summary>
        /// <returns>
        /// The offset before the wrap position.
        /// </returns>
        private int WrapBeforePosition()
        {
            return wrapBeforePosition - (rfc2646 ? (quotingLevel + 1) : 0);
        }

        /// <summary>
        /// Find the Longest non wrapped paragraph.
        /// </summary>
        /// <returns>
        /// The offset at the beginning of the paragraph.
        /// </returns>
        private int LongestNonWrappedParagraph()
        {
            return longestNonWrappedParagraph - (rfc2646 ? (quotingLevel + 1) : 0);
        }

        /// <summary>
        /// Wrap in preparation to append non space.
        /// </summary>
        /// <param name="count">The count.</param>
        private void WrapPrepareToAppendNonspace(int count)
        {
            while (breakOpportunity != 0 && lineLength + tailSpace + count > (wrapped ? WrapBeforePosition() : LongestNonWrappedParagraph()))
            {
                if (flushedLength == 0 && rfc2646)
                {
                    for (int i = 0; i < quotingLevel; i++)
                    {
                        output.Write('>');
                    }
                    if (quotingLevel != 0 || wrapBuffer[0] == '>' || wrapBuffer[0] == ' ')
                    {
                        output.Write(' ');
                    }
                }
                if (breakOpportunity >= lineLength)
                {
                    do
                    {
                        if (lineLength - flushedLength == wrapBuffer.Length)
                        {
                            output.Write(wrapBuffer, 0, wrapBuffer.Length, fallbacks ? this : null);
                            flushedLength += wrapBuffer.Length;
                        }
                        wrapBuffer[lineLength - flushedLength] = ' ';
                        lineLength++;
                        tailSpace--;
                    }
                    while (lineLength != breakOpportunity + 1);
                }
                output.Write(wrapBuffer, 0, breakOpportunity + 1 - flushedLength, fallbacks ? this : null);
                anyNewlines = true;
                output.Write("\r\n");
                wrapped = true;
                lineLengthBeforeSoftWrap += breakOpportunity + 1;
                linePosition += breakOpportunity + 1 + 2;
                lineLength -= breakOpportunity + 1;
                int num = flushedLength;
                flushedLength = 0;
                if (lineLength != 0)
                {
                    if (nextBreakOpportunity == 0 || nextBreakOpportunity - (breakOpportunity + 1) >= lineLength || nextBreakOpportunity - (breakOpportunity + 1) == 0)
                    {
                        if (rfc2646)
                        {
                            for (int j = 0; j < quotingLevel; j++)
                            {
                                output.Write('>');
                            }
                            if (quotingLevel != 0 || wrapBuffer[breakOpportunity + 1 - num] == '>' || wrapBuffer[breakOpportunity + 1 - num] == ' ')
                            {
                                output.Write(' ');
                            }
                        }
                        output.Write(wrapBuffer, breakOpportunity + 1 - num, lineLength, fallbacks ? this : null);
                        flushedLength = lineLength;
                    }
                    else
                    {
                        Buffer.BlockCopy(wrapBuffer, (breakOpportunity + 1 - num) * 2, wrapBuffer, 0, lineLength * 2);
                    }
                }
                if (nextBreakOpportunity != 0)
                {
                    breakOpportunity = nextBreakOpportunity - (breakOpportunity + 1);
                    if (breakOpportunity > WrapBeforePosition())
                    {
                        if (lineLength < WrapBeforePosition())
                        {
                            nextBreakOpportunity = breakOpportunity;
                            breakOpportunity = WrapBeforePosition();
                        }
                        else if (breakOpportunity > lineLength)
                        {
                            nextBreakOpportunity = breakOpportunity;
                            breakOpportunity = lineLength;
                        }
                        else
                        {
                            nextBreakOpportunity = 0;
                        }
                    }
                    else
                    {
                        nextBreakOpportunity = 0;
                    }
                }
                else
                {
                    breakOpportunity = 0;
                }
            }
            if (tailSpace != 0)
            {
                if (breakOpportunity == 0)
                {
                    if (flushedLength == 0 && rfc2646)
                    {
                        for (int k = 0; k < quotingLevel; k++)
                        {
                            output.Write('>');
                        }
                        output.Write(' ');
                    }
                    flushedLength += tailSpace;
                    FlushTailSpace();
                    return;
                }
                do
                {
                    wrapBuffer[lineLength - flushedLength] = ' ';
                    lineLength++;
                    tailSpace--;
                }
                while (tailSpace != 0);
            }
        }

        /// <summary>
        /// Flushes the line.
        /// </summary>
        /// <param name="nextChar">The next char.</param>
        private void FlushLine(char nextChar)
        {
            if (flushedLength == 0 && rfc2646)
            {
                for (int i = 0; i < quotingLevel; i++)
                {
                    output.Write('>');
                }
                char c = (lineLength != 0) ? wrapBuffer[0] : nextChar;
                if (quotingLevel != 0 || c == '>' || c == ' ')
                {
                    output.Write(' ');
                }
            }
            if (lineLength != flushedLength)
            {
                output.Write(wrapBuffer, 0, lineLength - flushedLength, fallbacks ? this : null);
                flushedLength = lineLength;
            }
        }

        /// <summary>
        /// Add spaces to the end of the output.
        /// </summary>
        private void FlushTailSpace()
        {
            lineLength += tailSpace;
            do
            {
                output.Write(' ');
                tailSpace--;
            }
            while (tailSpace != 0);
        }

        /// <summary>
        /// Encodes the symbol character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        /// <param name="textMapping">The text mapping.</param>
        private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
        {
            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                OutputNonspace((int)ch, TextMapping.Unicode);
                return;
            }
            string text = null;
            if (textMapping == TextMapping.Wingdings)
            {
                if (ch <= 'Ø')
                {
                    switch (ch)
                    {
                        case 'J':
                            text = "☺";
                            break;
                        case 'K':
                            text = ":|";
                            break;
                        case 'L':
                            text = "☹";
                            break;
                        default:
                            if (ch == 'Ø')
                            {
                                text = ">";
                            }
                            break;
                    }
                }
                else
                {
                    switch (ch)
                    {
                        case 'ß':
                            text = "<--";
                            break;
                        case 'à':
                            text = "-->";
                            break;
                        default:
                            switch (ch)
                            {
                                case 'ç':
                                    text = "<==";
                                    break;
                                case 'è':
                                    text = "==>";
                                    break;
                                default:
                                    switch (ch)
                                    {
                                        case 'ï':
                                            text = "<=";
                                            break;
                                        case 'ð':
                                            text = "=>";
                                            break;
                                        case 'ó':
                                            text = "<=>";
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            if (text == null)
            {
                text = "•";
            }
            OutputNonspace(text, TextMapping.Unicode);
        }
    }
}
