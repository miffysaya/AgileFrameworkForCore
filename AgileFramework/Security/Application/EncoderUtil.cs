using System;
using System.Text;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Provides helper methods common to all Anti-XSS encoders.
    /// </summary>
    internal static class EncoderUtil
    {
        /// <summary>
        /// Gets an appropriately-sized StringBuilder for the output of an encoding routine.
        /// </summary>
        /// <param name="inputLength">The length (in characters) of the input string.</param>
        /// <param name="worstCaseOutputCharsPerInputChar">The worst-case ratio of output characters per input character.</param>
        /// <returns>A StringBuilder appropriately-sized to hold the output string.</returns>
        internal static StringBuilder GetOutputStringBuilder(int inputLength, int worstCaseOutputCharsPerInputChar)
        {
            int capacity;
            if (inputLength >= 16384)
            {
                capacity = inputLength;
            }
            else
            {
                long val = inputLength * (long)worstCaseOutputCharsPerInputChar;
                capacity = (int)Math.Min(16384L, val);
            }
            return new StringBuilder(capacity);
        }
    }
}
