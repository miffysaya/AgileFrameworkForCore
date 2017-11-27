using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Values for the lower-mid section of the UTF8 Unicode code tables, from U1000 to U1EFF.
    /// </summary>
    [Flags]
    public enum LowerMidCodeCharts : long
    {
        /// <summary>
        /// No code charts from the lower-mid region of the Unicode tables are safe-listed.
        /// </summary>
        None = 0L,
        /// <summary>
        /// The Myanmar code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1000.pdf</remarks>
        Myanmar = 1L,
        /// <summary>
        /// The Georgian code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U10A0.pdf</remarks>
        Georgian = 2L,
        /// <summary>
        /// The Hangul Jamo code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1100.pdf</remarks>
        HangulJamo = 4L,
        /// <summary>
        /// The Ethiopic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1200.pdf</remarks>
        Ethiopic = 8L,
        /// <summary>
        /// The Ethiopic supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1380.pdf</remarks>
        EthiopicSupplement = 16L,
        /// <summary>
        /// The Cherokee code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U13A0.pdf</remarks>
        Cherokee = 32L,
        /// <summary>
        /// The Unified Canadian Aboriginal Syllabics code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1400.pdf</remarks>
        UnifiedCanadianAboriginalSyllabics = 64L,
        /// <summary>
        /// The Ogham code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1680.pdf</remarks>
        Ogham = 128L,
        /// <summary>
        /// The Runic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U16A0.pdf</remarks>
        Runic = 256L,
        /// <summary>
        /// The Tagalog code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1700.pdf</remarks>
        Tagalog = 512L,
        /// <summary>
        /// The Hanunoo code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1720.pdf</remarks>
        Hanunoo = 1024L,
        /// <summary>
        /// The Buhid code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1740.pdf</remarks>
        Buhid = 2048L,
        /// <summary>
        /// The Tagbanwa code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1760.pdf</remarks>
        Tagbanwa = 4096L,
        /// <summary>
        /// The Khmer code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1780.pdf</remarks>
        Khmer = 8192L,
        /// <summary>
        /// The Mongolian code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1800.pdf</remarks>
        Mongolian = 16384L,
        /// <summary>
        /// The Unified Canadian Aboriginal Syllabics Extended code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U18B0.pdf</remarks>
        UnifiedCanadianAboriginalSyllabicsExtended = 32768L,
        /// <summary>
        /// The Limbu code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1900.pdf</remarks>
        Limbu = 65536L,
        /// <summary>
        /// The Tai Le code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1950.pdf</remarks>
        TaiLe = 131072L,
        /// <summary>
        /// The New Tai Lue code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1980.pdf</remarks>
        NewTaiLue = 262144L,
        /// <summary>
        /// The Khmer Symbols code table
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U19E0.pdf</remarks>
        KhmerSymbols = 524288L,
        /// <summary>
        /// The Buginese code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1A00.pdf</remarks>
        Buginese = 1048576L,
        /// <summary>
        /// The Tai Tham code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1A20.pdf</remarks>
        TaiTham = 2097152L,
        /// <summary>
        /// The Balinese code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1B00.pdf</remarks>
        Balinese = 4194304L,
        /// <summary>
        /// The Sudanese code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1B80.pdf</remarks>
        Sudanese = 8388608L,
        /// <summary>
        /// The Lepcha code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1C00.pdf</remarks>
        Lepcha = 16777216L,
        /// <summary>
        /// The Ol Chiki code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1C50.pdf</remarks>
        OlChiki = 33554432L,
        /// <summary>
        /// The Vedic Extensions code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1CD0.pdf</remarks>
        VedicExtensions = 67108864L,
        /// <summary>
        /// The Phonetic Extensions code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1D00.pdf</remarks>
        PhoneticExtensions = 134217728L,
        /// <summary>
        /// The Phonetic Extensions Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1D80.pdf</remarks>
        PhoneticExtensionsSupplement = 268435456L,
        /// <summary>
        /// The Combining Diacritical Marks Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1DC0.pdf</remarks>        
        CombiningDiacriticalMarksSupplement = 536870912L,
        /// <summary>
        /// The Latin Extended Additional code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1E00.pdf</remarks>
        LatinExtendedAdditional = 1073741824L
    }
}
