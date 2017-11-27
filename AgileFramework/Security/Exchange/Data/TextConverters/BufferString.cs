using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct BufferString
    {
        private char[] buffer;

        private int offset;

        private int count;

        public static readonly BufferString Null = default(BufferString);

        public char this[int index]
        {
            get
            {
                return buffer[offset + index];
            }
        }

        public int Length
        {
            get
            {
                return count;
            }
        }

        public BufferString(char[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
        }

        public void TrimWhitespace()
        {
            while (count != 0 && ParseSupport.WhitespaceCharacter(buffer[offset]))
            {
                offset++;
                count--;
            }
            if (count != 0)
            {
                int num = offset + count - 1;
                while (ParseSupport.WhitespaceCharacter(buffer[num--]))
                {
                    count--;
                }
            }
        }

        public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
        {
            if (count != rightPart.Length)
            {
                return false;
            }
            for (int i = 0; i < rightPart.Length; i++)
            {
                if (ParseSupport.ToLowerCase(buffer[offset + i]) != rightPart[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            if (buffer == null)
            {
                return null;
            }
            if (count != 0)
            {
                return new string(buffer, offset, count);
            }
            return string.Empty;
        }

        [Conditional("DEBUG")]
        private static void AssertStringIsLowerCase(string rightPart)
        {
        }
    }
}
