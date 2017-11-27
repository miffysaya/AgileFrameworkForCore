using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Values for the upper middle section of the UTF8 Unicode code tables, from U2DE0 to UA8DF
    /// </summary>
    [Flags]
    public enum UpperMidCodeCharts : long
    {
        /// <summary>
        /// No code charts from the lower region of the Unicode tables are safe-listed.
        /// </summary>
        None = 0L,
        /// <summary>
        /// The Cyrillic Extended-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2DE0.pdf</remarks>
        CyrillicExtendedA = 1L,
        /// <summary>
        /// The Supplemental Punctuation code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2E00.pdf</remarks>
        SupplementalPunctuation = 2L,
        /// <summary>
        /// The CJK Radicials Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2E80.pdf</remarks>
        CjkRadicalsSupplement = 4L,
        /// <summary>
        /// The Kangxi Radicials code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2F00.pdf</remarks>
        KangxiRadicals = 8L,
        /// <summary>
        /// The Ideographic Description Characters code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2FF0.pdf</remarks>
        IdeographicDescriptionCharacters = 16L,
        /// <summary>
        /// The CJK Symbols and Punctuation code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3000.pdf</remarks>
        CjkSymbolsAndPunctuation = 32L,
        /// <summary>
        /// The Hiragana code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3040.pdf</remarks>
        Hiragana = 64L,
        /// <summary>
        /// The Katakana code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U30A0.pdf</remarks>
        Katakana = 128L,
        /// <summary>
        /// The Bopomofo code table.
        /// <seealso cref="F:Microsoft.Security.Application.UpperMidCodeCharts.BopomofoExtended" />
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3100.pdf</remarks>
        Bopomofo = 256L,
        /// <summary>
        /// The Hangul Compatbility Jamo code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3130.pdf</remarks>
        HangulCompatibilityJamo = 512L,
        /// <summary>
        /// The Kanbun code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3190.pdf</remarks>
        Kanbun = 1024L,
        /// <summary>
        /// The Bopomofu Extended code table.
        /// <seealso cref="F:Microsoft.Security.Application.UpperMidCodeCharts.Bopomofo" />
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U31A0.pdf</remarks>
        BopomofoExtended = 2048L,
        /// <summary>
        /// The CJK Strokes code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U31C0.pdf</remarks>
        CjkStrokes = 4096L,
        /// <summary>
        /// The Katakana Phonetic Extensoins code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U31F0.pdf</remarks>
        KatakanaPhoneticExtensions = 8192L,
        /// <summary>
        /// The Enclosed CJK Letters and Months code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3200.pdf</remarks>
        EnclosedCjkLettersAndMonths = 16384L,
        /// <summary>
        /// The CJK Compatibility code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3300.pdf</remarks>
        CjkCompatibility = 32768L,
        /// <summary>
        /// The CJK Unified Ideographs Extension A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U3400.pdf</remarks>
        CjkUnifiedIdeographsExtensionA = 65536L,
        /// <summary>
        /// The Yijing Hexagram Symbols code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U4DC0.pdf</remarks>
        YijingHexagramSymbols = 131072L,
        /// <summary>
        /// The CJK Unified Ideographs code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U4E00.pdf</remarks>
        CjkUnifiedIdeographs = 262144L,
        /// <summary>
        /// The Yi Syllables code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA000.pdf</remarks>
        YiSyllables = 524288L,
        /// <summary>
        /// The Yi Radicals code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA490.pdf</remarks>
        YiRadicals = 1048576L,
        /// <summary>
        /// The Lisu code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA4D0.pdf</remarks>        
        Lisu = 2097152L,
        /// <summary>
        /// The Vai code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA500.pdf</remarks>
        Vai = 4194304L,
        /// <summary>
        /// The Cyrillic Extended-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA640.pdf</remarks>
        CyrillicExtendedB = 8388608L,
        /// <summary>
        /// The Bamum code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA6A0.pdf</remarks>
        Bamum = 16777216L,
        /// <summary>
        /// The Modifier Tone Letters code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA700.pdf</remarks>
        ModifierToneLetters = 33554432L,
        /// <summary>
        /// The Latin Extended-D code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA720.pdf</remarks>
        LatinExtendedD = 67108864L,
        /// <summary>
        /// The Syloti Nagri code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA800.pdf</remarks>
        SylotiNagri = 134217728L,
        /// <summary>
        /// The Common Indic Number Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA830.pdf</remarks>
        CommonIndicNumberForms = 268435456L,
        /// <summary>
        /// The Phags-pa code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA840.pdf</remarks>
        Phagspa = 536870912L,
        /// <summary>
        /// The Saurashtra code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA880.pdf</remarks>
        Saurashtra = 1073741824L
    }
}
