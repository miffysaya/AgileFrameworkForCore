using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct HashCode
    {
        private int hash1;

        private int hash2;

        private int offset;

        public HashCode(bool ignore)
        {
            offset = 0;
            hash1 = (hash2 = 5381);
        }

        public static int CalculateEmptyHash()
        {
            return 371857150;
        }

        public unsafe static int CalculateLowerCase(char[] buffer, int offset, int length)
        {
            int num = 5381;
            int num2 = num;
            CheckArgs(buffer, offset, length);
            fixed (char* ptr = buffer)
            {
                char* ptr2 = ptr + offset;
                while (length > 0)
                {
                    num = ((num << 5) + num ^ ParseSupport.ToLowerCase(*ptr2));
                    if (length == 1)
                    {
                        break;
                    }
                    num2 = ((num2 << 5) + num2 ^ ParseSupport.ToLowerCase(ptr2[1]));
                    ptr2 += 2;
                    length -= 2;
                }
            }
            return num + num2 * 1566083941;
        }

        public unsafe void AdvanceLowerCase(char* s, int len)
        {
            if ((offset & 1) != 0)
            {
                hash2 = ((hash2 << 5) + hash2 ^ ParseSupport.ToLowerCase(*s));
                s++;
                len--;
                offset++;
            }
            offset += len;
            while (len > 0)
            {
                hash1 = ((hash1 << 5) + hash1 ^ ParseSupport.ToLowerCase(*s));
                if (len == 1)
                {
                    return;
                }
                hash2 = ((hash2 << 5) + hash2 ^ ParseSupport.ToLowerCase(s[1]));
                s += 2;
                len -= 2;
            }
        }

        public void AdvanceLowerCase(int ucs32)
        {
            if (ucs32 < 65536)
            {
                AdvanceLowerCase((char)ucs32);
                return;
            }
            char c = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
            char c2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
            if (((offset += 2) & 1) == 0)
            {
                hash1 = ((hash1 << 5) + hash1 ^ c);
                hash2 = ((hash2 << 5) + hash2 ^ c2);
                return;
            }
            hash2 = ((hash2 << 5) + hash2 ^ c);
            hash1 = ((hash1 << 5) + hash1 ^ c2);
        }

        public int AdvanceAndFinalizeHash(char c)
        {
            if ((offset++ & 1) == 0)
            {
                hash1 = ((hash1 << 5) + hash1 ^ c);
            }
            else
            {
                hash2 = ((hash2 << 5) + hash2 ^ c);
            }
            return hash1 + hash2 * 1566083941;
        }

        public void AdvanceLowerCase(char c)
        {
            if ((offset++ & 1) == 0)
            {
                hash1 = ((hash1 << 5) + hash1 ^ ParseSupport.ToLowerCase(c));
                return;
            }
            hash2 = ((hash2 << 5) + hash2 ^ ParseSupport.ToLowerCase(c));
        }

        public unsafe void AdvanceLowerCase(char[] buffer, int offset, int length)
        {
            CheckArgs(buffer, offset, length);
            fixed (char* ptr = buffer)
            {
                AdvanceLowerCase(ptr + offset, length);
            }
        }

        private static void CheckArgs(char[] buffer, int offset, int length)
        {
            int num = buffer.Length;
            if (offset < 0 || offset > num)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            if (offset + length < offset || offset + length > num)
            {
                throw new ArgumentOutOfRangeException("offset + length");
            }
        }

        public int FinalizeHash()
        {
            return hash1 + hash2 * 1566083941;
        }
    }
}
