using System.Text;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// The fallback ASCII encoder.
    /// </summary>
    internal class AsciiEncoderFallback : EncoderFallback
    {
        /// <summary>
        /// A buffer for the <see cref="T:Microsoft.Exchange.Data.Globalization.AsciiEncoderFallback" />.
        /// </summary>
        private sealed class AsciiFallbackBuffer : EncoderFallbackBuffer
        {
            /// <summary>
            /// The fallback index.
            /// </summary>
            private int fallbackIndex;

            /// <summary>
            /// The fallback string.
            /// </summary>
            private string fallbackString;

            /// <summary>
            /// Gets the maximum character count for the buffer.
            /// </summary>
            public static int MaxCharCount
            {
                get => 5;
            }

            /// <summary>
            /// Gets the remaining number of characters in the buffer.
            /// </summary>
            public override int Remaining
            {
                get => fallbackString == null ? 0 : fallbackString.Length - fallbackIndex;
            }

            /// <summary>
            /// Encodes the specified character using the fallback encoder.
            /// </summary>
            /// <param name="charUnknown">
            /// The unknown character to encode.
            /// </param>
            /// <param name="index">
            /// The index position in the buffer to encode into.
            /// </param>
            /// <returns>
            /// The encoded character.
            /// </returns>
            public override bool Fallback(char charUnknown, int index)
            {
                fallbackIndex = 0;
                fallbackString = (AsciiEncoderFallback.GetCharacterFallback(charUnknown) ?? "?");
                return true;
            }

            /// <summary>
            /// Encodes the specified high/low character combination using the fallback encoder.
            /// </summary>
            /// <param name="charUnknownHigh">
            /// The high byte of the character to encode.
            /// </param>
            /// <param name="charUnknownLow">
            /// The low byte of the character to encode.
            /// </param>
            /// <param name="index">
            /// The index position in the buffer to encode into.
            /// </param>
            /// <returns>
            /// The encoded character.
            /// </returns>
            public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
            {
                fallbackIndex = 0;
                fallbackString = "?";
                return true;
            }

            /// <summary>
            /// Gets the next character in the buffer.
            /// </summary>
            /// <returns>
            /// The next character in the buffer.
            /// </returns>
            public override char GetNextChar()
            {
                if (fallbackString == null || fallbackIndex == fallbackString.Length)
                {
                    return '\0';
                }
                return fallbackString[fallbackIndex++];
            }

            /// <summary>
            /// Moves to the previous character in the buffer.
            /// </summary>
            /// <returns>
            /// True if the move was sucessful, otherwise false.
            /// </returns>
            public override bool MovePrevious()
            {
                if (fallbackIndex > 0)
                {
                    fallbackIndex--;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Resets the buffer.
            /// </summary>
            public override void Reset()
            {
                fallbackString = "?";
                fallbackIndex = 0;
            }
        }

        /// <summary>
        /// Gets the maximum character count.
        /// </summary>
        public override int MaxCharCount
        {
            get
            {
                return AsciiFallbackBuffer.MaxCharCount;
            }
        }

        /// <summary>
        /// Encodes the specified character.
        /// </summary>
        /// <param name="charUnknown">
        /// The character to encode.
        /// </param>
        /// <returns>
        /// The encoded character.
        /// </returns>
        public static string GetCharacterFallback(char charUnknown)
        {
            if (charUnknown <= 'œ' && charUnknown >= '\u0082')
            {
                if (charUnknown <= 'æ')
                {
                    switch (charUnknown)
                    {
                        case '\u0082':
                        case '\u0091':
                        case '\u0092':
                            return "'";
                        case '\u0083':
                            return "f";
                        case '\u0084':
                        case '\u0093':
                        case '\u0094':
                            return "\"";
                        case '\u0085':
                            return "...";
                        case '\u0086':
                        case '\u0087':
                        case '\u0088':
                        case '\u0089':
                        case '\u008a':
                        case '\u008d':
                        case '\u008e':
                        case '\u008f':
                        case '\u0090':
                        case '\u009a':
                        case '\u009d':
                        case '\u009e':
                        case '\u009f':
                        case '¡':
                        case '£':
                        case '§':
                        case '¨':
                        case 'ª':
                        case '¬':
                        case '¯':
                        case '°':
                        case '±':
                        case '´':
                        case 'µ':
                        case '¶':
                        case 'º':
                        case '¿':
                        case 'À':
                        case 'Á':
                        case 'Â':
                        case 'Ã':
                        case 'Ä':
                        case 'Å':
                            break;
                        case '\u008b':
                            return "<";
                        case '\u008c':
                            return "OE";
                        case '\u0095':
                            return "*";
                        case '\u0096':
                            return "-";
                        case '\u0097':
                            return "-";
                        case '\u0098':
                            return "~";
                        case '\u0099':
                            return "(tm)";
                        case '\u009b':
                            return ">";
                        case '\u009c':
                            return "oe";
                        case '\u00a0':
                            return " ";
                        case '¢':
                            return "c";
                        case '¤':
                            return "$";
                        case '¥':
                            return "Y";
                        case '¦':
                            return "|";
                        case '©':
                            return "(c)";
                        case '«':
                            return "<";
                        case '­':
                            return string.Empty;
                        case '®':
                            return "(r)";
                        case '²':
                            return "^2";
                        case '³':
                            return "^3";
                        case '·':
                            return "*";
                        case '¸':
                            return ",";
                        case '¹':
                            return "^1";
                        case '»':
                            return ">";
                        case '¼':
                            return "(1/4)";
                        case '½':
                            return "(1/2)";
                        case '¾':
                            return "(3/4)";
                        case 'Æ':
                            return "AE";
                        default:
                            if (charUnknown == 'æ')
                            {
                                return "ae";
                            }
                            break;
                    }
                }
                else
                {
                    switch (charUnknown)
                    {
                        case 'Ĳ':
                            return "IJ";
                        case 'ĳ':
                            return "ij";
                        default:
                            switch (charUnknown)
                            {
                                case 'Œ':
                                    return "OE";
                                case 'œ':
                                    return "oe";
                            }
                            break;
                    }
                }
            }
            else if (charUnknown >= '\u2002' && charUnknown <= '™')
            {
                if (charUnknown <= '…')
                {
                    switch (charUnknown)
                    {
                        case '\u2002':
                        case '\u2003':
                            return " ";
                        default:
                            switch (charUnknown)
                            {
                                case '‑':
                                    return "-";
                                case '‒':
                                case '―':
                                case '‖':
                                case '‗':
                                case '‛':
                                case '‟':
                                case '†':
                                case '‡':
                                    break;
                                case '–':
                                case '—':
                                    return "-";
                                case '‘':
                                case '’':
                                case '‚':
                                    return "'";
                                case '“':
                                case '”':
                                case '„':
                                    return "\"";
                                case '•':
                                    return "*";
                                default:
                                    if (charUnknown == '…')
                                    {
                                        return "...";
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (charUnknown)
                    {
                        case '‹':
                            return "<";
                        case '›':
                            return ">";
                        default:
                            if (charUnknown == '€')
                            {
                                return "EUR";
                            }
                            if (charUnknown == '™')
                            {
                                return "(tm)";
                            }
                            break;
                    }
                }
            }
            else if (charUnknown >= '☹' && charUnknown <= '☺')
            {
                switch (charUnknown)
                {
                    case '☹':
                        return ":(";
                    case '☺':
                        return ":)";
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a fall back encoding buffer.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Microsoft.Exchange.Data.Globalization.AsciiEncoderFallback.AsciiFallbackBuffer" />.
        /// </returns>
        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new AsciiFallbackBuffer();
        }
    }
}
