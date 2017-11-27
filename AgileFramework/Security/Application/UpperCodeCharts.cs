using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Values for the upper section of the UTF8 Unicode code tables, from UA8E0 to UFFFD
    /// </summary>
    [Flags]
    public enum UpperCodeCharts
    {
        /// <summary>
        /// No code charts from the upper region of the Unicode tables are safe-listed.
        /// </summary>
        None = 0,
        /// <summary>
        /// The Devanagari Extended code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA8E0.pdf</remarks>
        DevanagariExtended = 1,
        /// <summary>
        /// The Kayah Li code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA900.pdf</remarks>
        KayahLi = 2,
        /// <summary>
        /// The Rejang code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA930.pdf</remarks>
        Rejang = 4,
        /// <summary>
        /// The Hangul Jamo Extended-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA960.pdf</remarks>
        HangulJamoExtendedA = 8,
        /// <summary>
        /// The Javanese code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UA980.pdf</remarks>
        Javanese = 16,
        /// <summary>
        /// The Cham code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UAA00.pdf</remarks>
        Cham = 32,
        /// <summary>
        /// The Myanmar Extended-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UAA60.pdf</remarks>
        MyanmarExtendedA = 64,
        /// <summary>
        /// The Tai Viet code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UAA80.pdf</remarks>
        TaiViet = 128,
        /// <summary>
        /// The Meetei Mayek code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UABC0.pdf</remarks>
        MeeteiMayek = 256,
        /// <summary>
        /// The Hangul Syllables code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UAC00.pdf</remarks>
        HangulSyllables = 512,
        /// <summary>
        /// The Hangul Jamo Extended-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UD7B0.pdf</remarks>
        HangulJamoExtendedB = 1024,
        /// <summary>
        /// The CJK Compatibility Ideographs code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UF900.pdf</remarks>
        CjkCompatibilityIdeographs = 2048,
        /// <summary>
        /// The Alphabetic Presentation Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFB00.pdf</remarks>
        AlphabeticPresentationForms = 4096,
        /// <summary>
        /// The Arabic Presentation Forms-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFB50.pdf</remarks>
        ArabicPresentationFormsA = 8192,
        /// <summary>
        /// The Variation Selectors code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE00.pdf</remarks>
        VariationSelectors = 16384,
        /// <summary>
        /// The Vertical Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE10.pdf</remarks>
        VerticalForms = 32768,
        /// <summary>
        /// The Combining Half Marks code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE20.pdf</remarks>
        CombiningHalfMarks = 65536,
        /// <summary>
        /// The CJK Compatibility Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE30.pdf</remarks>
        CjkCompatibilityForms = 131072,
        /// <summary>
        /// The Small Form Variants code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE50.pdf</remarks>
        SmallFormVariants = 262144,
        /// <summary>
        /// The Arabic Presentation Forms-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFE70.pdf</remarks>
        ArabicPresentationFormsB = 524288,
        /// <summary>
        /// The half width and full width Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFF00.pdf</remarks>
        HalfWidthAndFullWidthForms = 1048576,
        /// <summary>
        /// The Specials code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/UFFF0.pdf</remarks>
        Specials = 2097152
    }
}
