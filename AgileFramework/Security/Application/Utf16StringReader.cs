namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Reads individual scalar values from a UTF-16 input string.
    /// </summary>
    /// <remarks>
    /// For performance reasons, this is a mutable struct. Use caution when capturing instances of this type.
    /// </remarks>
    internal struct Utf16StringReader
    {
        /// <summary>
        /// Starting code point for the UTF-16 leading surrogates.
        /// </summary>
        private const char LeadingSurrogateStart = '\ud800';

        /// <summary>
        /// Starting code point for the UTF-16 trailing surrogates.
        /// </summary>
        private const char TrailingSurrogateStart = '\udc00';

        /// <summary>
        /// The Unicode replacement character U+FFFD.
        /// </summary>
        /// <remarks>
        /// For more info, see http://www.unicode.org/charts/PDF/UFFF0.pdf.
        /// </remarks>
        private const int UnicodeReplacementCharacterCodePoint = 65533;

        /// <summary>
        /// The input string we're iterating on.
        /// </summary>
        private readonly string input;

        /// <summary>
        /// The current offset into 'input'.
        /// </summary>
        private int currentOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Security.Application.Utf16StringReader" /> struct with the given UTF-16 input string.
        /// </summary>
        /// <param name="input">The input string to decompose into scalar values.</param>
        public Utf16StringReader(string input)
        {
            this.input = input;
            this.currentOffset = 0;
        }

        /// <summary>
        /// Reads the next scalar value from the input string.
        /// </summary>
        /// <returns>The next scalar value. If the input string contains invalid UTF-16, the
        /// return value is the Unicode replacement character U+FFFD. If the end of the string
        /// is reached, returns -1.</returns>
        public int ReadNextScalarValue()
        {
            if (this.currentOffset >= this.input.Length)
            {
                return -1;
            }
            char c = this.input[this.currentOffset++];
            int num = (int)c;
            if (char.IsHighSurrogate(c) && this.currentOffset < this.input.Length)
            {
                char c2 = this.input[this.currentOffset];
                if (char.IsLowSurrogate(c2))
                {
                    this.currentOffset++;
                    num = Utf16StringReader.ConvertToUtf32(c, c2);
                }
            }
            if (Utf16StringReader.IsValidUnicodeScalarValue(num))
            {
                return num;
            }
            return 65533;
        }

        /// <summary>
        /// Similar to Char.ConvertToUtf32, but slightly faster in tight loops since parameter checks are not done.
        /// </summary>
        /// <param name="leadingSurrogate">The UTF-16 leading surrogate character.</param>
        /// <param name="trailingSurrogate">The UTF-16 trailing surrogate character.</param>
        /// <returns>The scalar value resulting from combining these two surrogate characters.</returns>
        /// <remarks>The caller must ensure that the inputs are valid surrogate characters. If not,
        /// the output of this routine is undefined.</remarks>
        private static int ConvertToUtf32(char leadingSurrogate, char trailingSurrogate)
        {
            return (int)((leadingSurrogate - '\ud800') * 'Ѐ' + (trailingSurrogate - '\udc00')) + 65536;
        }

        /// <summary>
        /// Determines whether a given code point is a valid Unicode scalar value.
        /// </summary>
        /// <param name="codePoint">The code point whose validity is to be checked.</param>
        /// <returns>True if the input is a valid Unicode scalar value, false otherwise.</returns>
        private static bool IsValidUnicodeScalarValue(int codePoint)
        {
            return (0 <= codePoint && codePoint <= 55295) || (57344 <= codePoint && codePoint <= 1114111);
        }
    }
}
