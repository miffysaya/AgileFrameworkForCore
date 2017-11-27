using AgileFramework.Security.Application.TextConverters.HTML;
using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct ScratchBuffer
    {
        private char[] buffer;

        private int count;

        public char[] Buffer
        {
            get
            {
                return buffer;
            }
        }

        public int Length
        {
            get
            {
                return count;
            }
        }

        public BufferString BufferString
        {
            get
            {
                return new BufferString(buffer, 0, count);
            }
        }

        public char this[int offset]
        {
            get
            {
                return buffer[offset];
            }
            set
            {
                buffer[offset] = value;
            }
        }

        public void Reset()
        {
            count = 0;
        }

        public bool AppendHtmlAttributeValue(HtmlAttribute attr, int maxSize)
        {
            int num = 0;
            int num2;
            while ((num2 = GetSpace(maxSize)) != 0 && (num2 = attr.Value.Read(buffer, count, num2)) != 0)
            {
                count += num2;
                num += num2;
            }
            return num != 0;
        }

        public int Append(char ch, int maxSize)
        {
            if (GetSpace(maxSize) == 0)
            {
                return 0;
            }
            buffer[count++] = ch;
            return 1;
        }

        public int Append(string str, int maxSize)
        {
            int num = 0;
            int num2;
            while ((num2 = Math.Min(GetSpace(maxSize), str.Length - num)) != 0)
            {
                str.CopyTo(num, buffer, count, num2);
                count += num2;
                num += num2;
            }
            return num;
        }

        public int Append(char[] buffer, int offset, int length, int maxSize)
        {
            int num = 0;
            int num2;
            while ((num2 = Math.Min(GetSpace(maxSize), length)) != 0)
            {
                System.Buffer.BlockCopy(buffer, offset * 2, this.buffer, count * 2, num2 * 2);
                count += num2;
                offset += num2;
                length -= num2;
                num += num2;
            }
            return num;
        }

        private int GetSpace(int maxSize)
        {
            if (count >= maxSize)
            {
                return 0;
            }
            if (buffer == null)
            {
                buffer = new char[64];
            }
            else if (buffer.Length == count)
            {
                char[] dst = new char[buffer.Length * 2];
                System.Buffer.BlockCopy(buffer, 0, dst, 0, count * 2);
                buffer = dst;
            }
            return buffer.Length - count;
        }
    }
}
