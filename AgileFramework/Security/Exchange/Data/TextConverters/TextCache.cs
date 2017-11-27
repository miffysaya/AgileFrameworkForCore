using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class TextCache
    {
        internal class CacheEntry
        {
            private const int DefaultMaxLength = 4096;

            private char[] buffer;

            private int count;

            private int offset;

            private CacheEntry next;

            public int Length
            {
                get
                {
                    return count;
                }
            }

            public CacheEntry Next
            {
                get
                {
                    return next;
                }
                set
                {
                    next = value;
                }
            }

            public CacheEntry(int size)
            {
                AllocateBuffer(size);
            }

            public void Reset()
            {
                count = 0;
            }

            public bool GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
            {
                if (count == 0)
                {
                    this.offset = 0;
                    if (this.buffer.Length < size)
                    {
                        AllocateBuffer(size);
                    }
                }
                if (this.buffer.Length - (this.offset + count) >= size)
                {
                    buffer = this.buffer;
                    offset = this.offset + count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }
                if (count < 64 && this.buffer.Length - count >= size)
                {
                    Buffer.BlockCopy(this.buffer, this.offset * 2, this.buffer, 0, count * 2);
                    this.offset = 0;
                    buffer = this.buffer;
                    offset = this.offset + count;
                    realSize = this.buffer.Length - offset;
                    return true;
                }
                buffer = null;
                offset = 0;
                realSize = 0;
                return false;
            }

            public void Commit(int count)
            {
                this.count += count;
            }

            public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
            {
                outputBuffer = buffer;
                outputOffset = offset;
                outputCount = count;
            }

            public void ReportRead(int count)
            {
                offset += count;
                this.count -= count;
            }

            public int Read(char[] buffer, int offset, int count)
            {
                int num = Math.Min(count, this.count);
                Buffer.BlockCopy(this.buffer, this.offset * 2, buffer, offset * 2, num * 2);
                this.count -= num;
                this.offset += num;
                count -= num;
                offset += num;
                return num;
            }

            private void AllocateBuffer(int size)
            {
                if (size < 2048)
                {
                    size = 2048;
                }
                size = (size * 2 + 1023) / 1024 * 1024;
                buffer = new char[size];
            }
        }

        private int cachedLength;

        private CacheEntry headEntry;

        private CacheEntry tailEntry;

        private CacheEntry freeList;

        public int Length
        {
            get
            {
                return cachedLength;
            }
        }

        public void Reset()
        {
            while (headEntry != null)
            {
                headEntry.Reset();
                CacheEntry cacheEntry = headEntry;
                headEntry = headEntry.Next;
                if (headEntry == null)
                {
                    tailEntry = null;
                }
                cacheEntry.Next = freeList;
                freeList = cacheEntry;
            }
            cachedLength = 0;
        }

        public void GetBuffer(int size, out char[] buffer, out int offset, out int realSize)
        {
            if (tailEntry != null && tailEntry.GetBuffer(size, out buffer, out offset, out realSize))
            {
                return;
            }
            AllocateTail(size);
            tailEntry.GetBuffer(size, out buffer, out offset, out realSize);
        }

        public void Commit(int count)
        {
            tailEntry.Commit(count);
            cachedLength += count;
        }

        public void GetData(out char[] outputBuffer, out int outputOffset, out int outputCount)
        {
            headEntry.GetData(out outputBuffer, out outputOffset, out outputCount);
        }

        public void ReportRead(int count)
        {
            headEntry.ReportRead(count);
            cachedLength -= count;
            if (headEntry.Length == 0)
            {
                CacheEntry cacheEntry = headEntry;
                headEntry = headEntry.Next;
                if (headEntry == null)
                {
                    tailEntry = null;
                }
                cacheEntry.Next = freeList;
                freeList = cacheEntry;
            }
        }

        public int Read(char[] buffer, int offset, int count)
        {
            int num = 0;
            while (count != 0)
            {
                int num2 = headEntry.Read(buffer, offset, count);
                offset += num2;
                count -= num2;
                num += num2;
                cachedLength -= num2;
                if (headEntry.Length == 0)
                {
                    CacheEntry cacheEntry = headEntry;
                    headEntry = headEntry.Next;
                    if (headEntry == null)
                    {
                        tailEntry = null;
                    }
                    cacheEntry.Next = freeList;
                    freeList = cacheEntry;
                }
                if (count == 0 || headEntry == null)
                {
                    break;
                }
            }
            return num;
        }

        private void AllocateTail(int size)
        {
            CacheEntry cacheEntry = freeList;
            if (cacheEntry != null)
            {
                freeList = cacheEntry.Next;
                cacheEntry.Next = null;
            }
            else
            {
                cacheEntry = new CacheEntry(size);
            }
            if (tailEntry != null)
            {
                tailEntry.Next = cacheEntry;
            }
            else
            {
                headEntry = cacheEntry;
            }
            tailEntry = cacheEntry;
        }
    }
}
