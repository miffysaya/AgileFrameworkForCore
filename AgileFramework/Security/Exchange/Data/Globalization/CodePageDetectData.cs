namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Encapsulates code page detection data.
    /// </summary>
    internal class CodePageDetectData
    {
        /// <summary>
        /// Represents a code page.
        /// </summary>
        internal struct CodePage
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            public ushort Id;

            /// <summary>
            /// The Mask for this codepage.
            /// </summary>
            public uint Mask;

            /// <summary>
            /// True if the codepage is a windows codepage, otherwise false.
            /// </summary>
            public bool IsWindowsCodePage;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CodePageDetectData.CodePage" /> struct.
            /// </summary>
            /// <param name="id">The code page identifier.</param>
            /// <param name="mask">The code page Mask.</param>
            /// <param name="isWindowsCodePage">if set to <c>true</c> the code page is a Windows codepage..</param>
            public CodePage(ushort id, uint mask, bool isWindowsCodePage)
            {
                this.Id = id;
                this.Mask = mask;
                this.IsWindowsCodePage = isWindowsCodePage;
            }
        }

        /// <summary>
        /// The list of code pages and their masks.
        /// </summary>
        internal static readonly CodePage[] CodePages = new CodePage[]
        {
            new CodePage(20127, 1u, false),
            new CodePage(28591, 2u, false),
            new CodePage(28592, 4u, false),
            new CodePage(20866, 8u, false),
            new CodePage(21866, 16u, false),
            new CodePage(28595, 32u, false),
            new CodePage(28597, 64u, false),
            new CodePage(28593, 128u, false),
            new CodePage(28594, 256u, false),
            new CodePage(28596, 512u, false),
            new CodePage(38598, 1024u, false),
            new CodePage(28605, 2048u, false),
            new CodePage(28599, 4096u, false),
            new CodePage(1252, 8192u, true),
            new CodePage(1250, 16384u, true),
            new CodePage(1251, 32768u, true),
            new CodePage(1253, 65536u, true),
            new CodePage(1254, 131072u, true),
            new CodePage(1257, 262144u, true),
            new CodePage(1258, 524288u, true),
            new CodePage(1256, 1048576u, true),
            new CodePage(1255, 2097152u, true),
            new CodePage(874, 4194304u, true),
            new CodePage(50220, 8388608u, false),
            new CodePage(932, 16777216u, true),
            new CodePage(949, 33554432u, true),
            new CodePage(950, 67108864u, true),
            new CodePage(936, 134217728u, true),
            new CodePage(51932, 268435456u, false),
            new CodePage(51949, 536870912u, false),
            new CodePage(50225, 1073741824u, false),
            new CodePage(52936, 2147483648u, false)
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CodePageDetectData" /> class.
        /// </summary>
        private CodePageDetectData()
        {
        }
    }
}
