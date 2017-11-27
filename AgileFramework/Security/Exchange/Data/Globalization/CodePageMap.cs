namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Represents the logic to choose a codepage.
    /// </summary>
    internal class CodePageMap : CodePageMapData
    {
        /// <summary>
        /// The current codepage.
        /// </summary>
        private int codePage;

        /// <summary>
        /// The ranges for the current codepage.
        /// </summary>
        private CodePageRange[] ranges;

        /// <summary>
        /// The previous index used for range operations.
        /// </summary>
        private int lastRangeIndex;

        /// <summary>
        /// The last codepage range used.
        /// </summary>
        private CodePageRange lastRange;

        /// <summary>
        /// Chooses the current code page.
        /// </summary>
        /// <param name="newCodePage">The code page to choose.</param>
        /// <returns>True if the selection is succesful, otherwise false.</returns>
        public bool ChoseCodePage(int newCodePage)
        {
            if (newCodePage == codePage)
            {
                return true;
            }
            codePage = newCodePage;
            ranges = null;
            if (newCodePage == 1200)
            {
                return true;
            }
            for (int i = CodePages.Length - 1; i >= 0; i--)
            {
                if (CodePages[i].Id == newCodePage)
                {
                    ranges = CodePages[i].Ranges;
                    lastRangeIndex = ranges.Length / 2;
                    lastRange = ranges[lastRangeIndex];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Decides if an extended chracter is unsafe for the current codepage.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>True if the character is unsafe, otherwise false.</returns>
        public bool IsUnsafeExtendedCharacter(char ch)
        {
            if (ranges == null)
            {
                return false;
            }
            if (ch <= (char)lastRange.Last)
            {
                if (ch >= (char)lastRange.First)
                {
                    return lastRange.Offset != 65535 && (Bitmap[lastRange.Offset + (ushort)(ch - (char)lastRange.First)] & lastRange.Mask) == 0;
                }
                int num = lastRangeIndex;
                while (--num >= 0)
                {
                    if (ch >= (char)ranges[num].First)
                    {
                        if (ch > (char)ranges[num].Last)
                        {
                            break;
                        }
                        if (ch == (char)ranges[num].First)
                        {
                            return false;
                        }
                        lastRangeIndex = num;
                        lastRange = ranges[num];
                        return lastRange.Offset != 65535 && (Bitmap[lastRange.Offset + (ushort)(ch - (char)lastRange.First)] & lastRange.Mask) == 0;
                    }
                }
            }
            else
            {
                int num2 = lastRangeIndex;
                while (++num2 < ranges.Length)
                {
                    if (ch <= (char)ranges[num2].Last)
                    {
                        if (ch < (char)ranges[num2].First)
                        {
                            break;
                        }
                        if (ch == (char)ranges[num2].First)
                        {
                            return false;
                        }
                        lastRangeIndex = num2;
                        lastRange = ranges[num2];
                        return lastRange.Offset != 65535 && (Bitmap[lastRange.Offset + (ushort)(ch - (char)lastRange.First)] & lastRange.Mask) == 0;
                    }
                }
            }
            return true;
        }
    }
}
