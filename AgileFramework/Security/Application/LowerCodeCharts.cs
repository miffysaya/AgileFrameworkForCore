using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Values for the lowest section of the UTF8 Unicode code tables, from U0000 to U0FFF.
    /// </summary>
    [Flags]
    public enum LowerCodeCharts : long
    {
        /// <summary>
        /// No code charts from the lower region of the Unicode tables are safe-listed.
        /// </summary>
        None = 0L,
        /// <summary>
        /// The Basic Latin code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0000.pdf</remarks>
        BasicLatin = 1L,
        /// <summary>
        /// The C1 Controls and Latin-1 Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0080.pdf</remarks>
        C1ControlsAndLatin1Supplement = 2L,
        /// <summary>
        /// The Latin Extended-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0100.pdf</remarks>
        LatinExtendedA = 4L,
        /// <summary>
        /// The Latin Extended-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0180.pdf</remarks>
        LatinExtendedB = 8L,
        /// <summary>
        /// The IPA Extensions code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0250.pdf</remarks>
        IpaExtensions = 16L,
        /// <summary>
        /// The Spacing Modifier Letters code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U02B0.pdf</remarks>
        SpacingModifierLetters = 32L,
        /// <summary>
        /// The Combining Diacritical Marks code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0300.pdf</remarks>
        CombiningDiacriticalMarks = 64L,
        /// <summary>
        /// The Greek and Coptic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0370.pdf</remarks>
        GreekAndCoptic = 128L,
        /// <summary>
        /// The Cyrillic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0400.pdf</remarks>
        Cyrillic = 256L,
        /// <summary>
        /// The Cyrillic Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0500.pdf</remarks>
        CyrillicSupplement = 512L,
        /// <summary>
        /// The Armenian code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0530.pdf</remarks>
        Armenian = 1024L,
        /// <summary>
        /// The Hebrew code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0590.pdf</remarks>
        Hebrew = 2048L,
        /// <summary>
        /// The Arabic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0600.pdf</remarks>
        Arabic = 4096L,
        /// <summary>
        /// The Syriac code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0700.pdf</remarks>
        Syriac = 8192L,
        /// <summary>
        /// The Arabic Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0750.pdf</remarks>
        ArabicSupplement = 16384L,
        /// <summary>
        /// The Thaana code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0780.pdf</remarks>
        Thaana = 32768L,
        /// <summary>
        /// The Nko code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U07C0.pdf</remarks>
        Nko = 65536L,
        /// <summary>
        /// The Samaritan code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0800.pdf</remarks>
        Samaritan = 131072L,
        /// <summary>
        /// The Devanagari code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0900.pdf</remarks>
        Devanagari = 262144L,
        /// <summary>
        /// The Bengali code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0980.pdf</remarks>
        Bengali = 524288L,
        /// <summary>
        /// The Gurmukhi code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0A00.pdf</remarks>
        Gurmukhi = 1048576L,
        /// <summary>
        /// The Gujarati code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0A80.pdf</remarks>
        Gujarati = 2097152L,
        /// <summary>
        /// The Oriya code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0B00.pdf</remarks>
        Oriya = 4194304L,
        /// <summary>
        /// The Tamil code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0B80.pdf</remarks>
        Tamil = 8388608L,
        /// <summary>
        /// The Telugu code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0C00.pdf</remarks>
        Telugu = 16777216L,
        /// <summary>
        /// The Kannada code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0C80.pdf</remarks>
        Kannada = 33554432L,
        /// <summary>
        /// The Malayalam code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0D00.pdf</remarks>
        Malayalam = 67108864L,
        /// <summary>
        /// The Sinhala code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0D80.pdf</remarks>
        Sinhala = 134217728L,
        /// <summary>
        /// The Thai code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0E00.pdf</remarks>
        Thai = 268435456L,
        /// <summary>
        /// The Lao code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0E80.pdf</remarks>
        Lao = 536870912L,
        /// <summary>
        /// The Tibetan code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U0F00.pdf</remarks>
        Tibetan = 1073741824L,
        /// <summary>
        /// The default code tables marked as safe on initialisation.
        /// </summary>
        Default = 127L
    }
}
