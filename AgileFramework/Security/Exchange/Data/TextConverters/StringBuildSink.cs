using System;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class StringBuildSink : ITextSinkEx, ITextSink
    {
        private StringBuilder sb;

        private int maxLength;

        public bool IsEnough
        {
            get
            {
                return sb.Length >= maxLength;
            }
        }

        public StringBuildSink()
        {
            sb = new StringBuilder();
        }

        public void Reset(int maxLength)
        {
            this.maxLength = maxLength;
            sb.Length = 0;
        }

        public void Write(char[] buffer, int offset, int count)
        {
            count = Math.Min(count, maxLength - sb.Length);
            sb.Append(buffer, offset, count);
        }

        public void Write(int ucs32Char)
        {
            if (Token.LiteralLength(ucs32Char) == 1)
            {
                sb.Append((char)ucs32Char);
                return;
            }
            sb.Append(Token.LiteralFirstChar(ucs32Char));
            if (!IsEnough)
            {
                sb.Append(Token.LiteralLastChar(ucs32Char));
            }
        }

        public void Write(string value)
        {
            sb.Append(value);
        }

        public void WriteNewLine()
        {
            sb.Append('\r');
            if (!IsEnough)
            {
                sb.Append('\n');
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
