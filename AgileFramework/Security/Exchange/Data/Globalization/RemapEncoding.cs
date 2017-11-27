using System;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Represents remap instructions
    /// </summary>
    [Serializable]
    internal class RemapEncoding : Encoding
    {
        /// <summary>
        /// The encoding used to decode.
        /// </summary>
        private readonly Encoding decodingEncoding;

        /// <summary>
        /// The encoding used to encode.
        /// </summary>
        private readonly Encoding encodingEncoding;

        /// <summary>
        /// Gets the encoding code page.
        /// </summary>
        public override int CodePage
        {
            get
            {
                return encodingEncoding.CodePage;
            }
        }

        /// <summary>
        /// Gets encoding body name.
        /// </summary>
        public override string BodyName
        {
            get
            {
                return encodingEncoding.BodyName;
            }
        }

        /// <summary>
        /// Gets the encoding name.
        /// </summary>
        public override string EncodingName
        {
            get
            {
                return encodingEncoding.EncodingName;
            }
        }

        /// <summary>
        /// Gets the encoding header name.
        /// </summary>
        public override string HeaderName
        {
            get
            {
                return encodingEncoding.HeaderName;
            }
        }

        /// <summary>
        /// Gets the encoding web name.
        /// </summary>
        public override string WebName
        {
            get
            {
                return encodingEncoding.WebName;
            }
        }

        /// <summary>
        /// Gets the encoding Windows code page.
        /// </summary>
        public override int WindowsCodePage
        {
            get
            {
                return encodingEncoding.WindowsCodePage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the encoding class can be used for browsers displaying content.
        /// </summary>
        public override bool IsBrowserDisplay
        {
            get
            {
                return encodingEncoding.IsBrowserDisplay;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the encoding class can be used for browsers for saving content.
        /// </summary>
        public override bool IsBrowserSave
        {
            get
            {
                return encodingEncoding.IsBrowserSave;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the encoding class can be used for mail and news clients for displaying content.
        /// </summary>
        public override bool IsMailNewsDisplay
        {
            get
            {
                return encodingEncoding.IsMailNewsDisplay;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the encoding class can be used for mail and news clients for saving content.
        /// </summary>
        public override bool IsMailNewsSave
        {
            get
            {
                return encodingEncoding.IsMailNewsSave;
            }
        }

        /// <summary>
        /// gets a value indicating whether the current encoding uses single-byte code points.
        /// </summary>
        public override bool IsSingleByte
        {
            get
            {
                return encodingEncoding.IsSingleByte;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.RemapEncoding" /> class.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// Thrown if the code page is unknown and cannot be remapped.
        /// </exception>
        public RemapEncoding(int codePage) : base(codePage)
        {
            if (codePage == 28591)
            {
                encodingEncoding = Encoding.GetEncoding(28591);
                decodingEncoding = Encoding.GetEncoding(1252);
                return;
            }
            if (codePage != 28599)
            {
                throw new ArgumentException();
            }
            encodingEncoding = Encoding.GetEncoding(28599);
            decodingEncoding = Encoding.GetEncoding(1254);
        }

        /// <summary>
        /// Returns a sequence of bytes that specifies the encoding used.
        /// </summary>
        /// <returns>
        /// A byte array containing a sequence of bytes that specifies the encoding used.
        /// -or- 
        /// A byte array of length zero, if a preamble is not required.
        /// </returns>
        public override byte[] GetPreamble()
        {
            return encodingEncoding.GetPreamble();
        }

        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">
        /// The number of characters to encode. 
        /// </param>
        /// <returns>
        /// The maximum number of bytes produced by encoding the specified number of characters.
        /// </returns>
        public override int GetMaxByteCount(int charCount)
        {
            return encodingEncoding.GetMaxByteCount(charCount);
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">
        /// The number of bytes to decode. 
        /// </param>
        /// <returns>
        /// The maximum number of characters produced by decoding the specified number of bytes.
        /// </returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return decodingEncoding.GetMaxCharCount(byteCount);
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of characters from the specified character array.
        /// </summary>
        /// <param name="chars">
        /// The character array containing the set of characters to encode. 
        /// </param>
        /// <param name="index">
        /// The index of the first character to encode. 
        /// </param>
        /// <param name="count">
        /// The number of characters to encode. 
        /// </param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return encodingEncoding.GetByteCount(chars, index, count);
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding the characters in the specified <see cref="T:System.String" />. 
        /// </summary>
        /// <param name="s">
        /// The <see cref="T:System.String" /> containing the set of characters to encode. 
        /// </param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        public override int GetByteCount(string s)
        {
            return encodingEncoding.GetByteCount(s);
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of characters starting at the specified character pointer.
        /// </summary>
        /// <param name="chars">
        /// A pointer to the first character to encode. 
        /// </param>
        /// <param name="count">
        /// The number of characters to encode. 
        /// </param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        public unsafe override int GetByteCount(char* chars, int count)
        {
            return encodingEncoding.GetByteCount(chars, count);
        }

        /// <summary>
        /// Encodes a set of characters from the specified String into the specified byte array. 
        /// </summary>
        /// <param name="s">
        /// The <see cref="T:System.String" /> containing the set of characters to encode. 
        /// </param>
        /// <param name="charIndex">
        /// The index of the first character to encode. 
        /// </param>
        /// <param name="charCount">
        /// The number of characters to encode. 
        /// </param>
        /// <param name="bytes">
        /// The byte array to contain the resulting sequence of bytes. 
        /// </param>
        /// <param name="byteIndex">
        /// The index at which to start writing the resulting sequence of bytes.
        /// </param>
        /// <returns>
        /// The actual number of bytes written into bytes. 
        /// </returns>
        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encodingEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
        }

        /// <summary>
        /// Encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">
        /// The character array containing the set of characters to encode. 
        /// </param>
        /// <param name="charIndex">
        /// The index of the first character to encode. 
        /// </param>
        /// <param name="charCount">
        /// The number of characters to encode. 
        /// </param>
        /// <param name="bytes">
        /// The byte array to contain the resulting sequence of bytes. 
        /// </param>
        /// <param name="byteIndex">
        /// The index at which to start writing the resulting sequence of bytes. 
        /// </param>
        /// <returns>
        /// The actual number of bytes written into bytes. 
        /// </returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encodingEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        /// <summary>
        /// Encodes a set of characters starting at the specified character pointer into a sequence of bytes that are stored starting at the specified byte pointer.
        /// </summary>
        /// <param name="chars">
        /// A pointer to the first character to encode. 
        /// </param>
        /// <param name="charCount">
        /// The number of characters to encode. 
        /// </param>
        /// <param name="bytes">
        /// A pointer to the location at which to start writing the resulting sequence of bytes. 
        /// </param>
        /// <param name="byteCount">
        /// The maximum number of bytes to write. 
        /// </param>
        /// <returns>
        /// The actual number of bytes written at the location indicated by the bytes parameter.
        /// </returns>
        public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
        {
            return encodingEncoding.GetBytes(chars, charCount, bytes, byteCount);
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">
        /// The byte array containing the sequence of bytes to decode. 
        /// </param>
        /// <param name="index">
        /// The index of the first byte to decode. 
        /// </param>
        /// <param name="count">
        /// The number of bytes to decode. 
        /// </param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes.
        /// </returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return decodingEncoding.GetCharCount(bytes, index, count);
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes starting at the specified byte pointer.
        /// </summary>
        /// <param name="bytes">
        /// A pointer to the first byte to decode.
        /// </param>
        /// <param name="count">
        /// The number of bytes to decode.
        /// </param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes.
        /// </returns>
        public unsafe override int GetCharCount(byte* bytes, int count)
        {
            return decodingEncoding.GetCharCount(bytes, count);
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">
        /// The byte array containing the sequence of bytes to decode. 
        /// </param>
        /// <param name="byteIndex">
        /// The index of the first byte to decode. 
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to decode. 
        /// </param>
        /// <param name="chars">
        /// The character array to contain the resulting set of characters. 
        /// </param>
        /// <param name="charIndex">
        /// The index at which to start writing the resulting set of characters.
        /// </param>
        /// <returns>
        /// The actual number of characters written into chars. 
        /// </returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return decodingEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        /// <summary>
        /// Decodes a sequence of bytes starting at the specified byte pointer into a set of characters that are stored starting at the specified character pointer.
        /// </summary>
        /// <param name="bytes">
        /// The bytes to decode.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to decode.
        /// </param>
        /// <param name="chars">
        /// The characters written.
        /// </param>
        /// <param name="charCount">
        /// The maximum number of characters to write. 
        /// </param>
        /// <returns>
        /// The actual number of characters written at the location indicated by the chars parameter. 
        /// </returns>
        public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
        {
            return decodingEncoding.GetChars(bytes, byteCount, chars, charCount);
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into a string.
        /// </summary>
        /// <param name="bytes">
        /// The byte array containing the sequence of bytes to decode.
        /// </param>
        /// <param name="index">
        /// The index of the first byte to decode. 
        /// </param>
        /// <param name="count">
        /// The number of bytes to decode. 
        /// </param>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the results of decoding the specified sequence of bytes. 
        /// </returns>
        public override string GetString(byte[] bytes, int index, int count)
        {
            return decodingEncoding.GetString(bytes, index, count);
        }

        /// <summary>
        /// Gets the decoder for this map.
        /// </summary>
        /// <returns>
        /// The decoder for this map.
        /// </returns>
        public override Decoder GetDecoder()
        {
            return decodingEncoding.GetDecoder();
        }

        /// <summary>
        /// Gets the encoder for this map.
        /// </summary>
        /// <returns>
        /// The encoder for this map.
        /// </returns>
        public override Encoder GetEncoder()
        {
            return encodingEncoding.GetEncoder();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of the current instance.</returns>
        public override object Clone()
        {
            return (Encoding)MemberwiseClone();
        }
    }
}
