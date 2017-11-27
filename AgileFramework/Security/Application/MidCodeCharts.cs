using System;

namespace AgileFramework.Security.Application
{
    /// <summary>
    /// Values for the middle section of the UTF8 Unicode code tables, from U1F00 to U2DDF
    /// </summary>
    [Flags]
    public enum MidCodeCharts : long
    {
        /// <summary>
        /// No code charts from the lower region of the Unicode tables are safe-listed.
        /// </summary>
        None = 0L,
        /// <summary>
        /// The Greek Extended code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U1F00.pdf</remarks>
        GreekExtended = 1L,
        /// <summary>
        /// The General Punctuation code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2000.pdf</remarks>
        GeneralPunctuation = 2L,
        /// <summary>
        /// The Superscripts and Subscripts code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2070.pdf</remarks>
        SuperscriptsAndSubscripts = 4L,
        /// <summary>
        /// The Currency Symbols code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U20A0.pdf</remarks>
        CurrencySymbols = 8L,
        /// <summary>
        /// The Combining Diacritical Marks for Symbols code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U20D0.pdf</remarks>
        CombiningDiacriticalMarksForSymbols = 16L,
        /// <summary>
        /// The Letterlike Symbols code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2100.pdf</remarks>
        LetterlikeSymbols = 32L,
        /// <summary>
        /// The Number Forms code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2150.pdf</remarks>
        NumberForms = 64L,
        /// <summary>
        /// The Arrows code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2190.pdf</remarks>
        Arrows = 128L,
        /// <summary>
        /// The Mathematical Operators code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2200.pdf</remarks>
        MathematicalOperators = 256L,
        /// <summary>
        /// The Miscellaneous Technical code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2300.pdf</remarks>
        MiscellaneousTechnical = 512L,
        /// <summary>
        /// The Control Pictures code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2400.pdf</remarks>
        ControlPictures = 1024L,
        /// <summary>
        /// The Optical Character Recognition table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2440.pdf</remarks>
        OpticalCharacterRecognition = 2048L,
        /// <summary>
        /// The Enclosed Alphanumeric code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2460.pdf</remarks>
        EnclosedAlphanumerics = 4096L,
        /// <summary>
        /// The Box Drawing code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2500.pdf</remarks>
        BoxDrawing = 8192L,
        /// <summary>
        /// The Block Elements code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2580.pdf</remarks>
        BlockElements = 16384L,
        /// <summary>
        /// The Geometric Shapes code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U25A0.pdf</remarks>
        GeometricShapes = 32768L,
        /// <summary>
        /// The Miscellaneous Symbols code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2600.pdf</remarks>
        MiscellaneousSymbols = 65536L,
        /// <summary>
        /// The Dingbats code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2700.pdf</remarks>
        Dingbats = 131072L,
        /// <summary>
        /// The Miscellaneous Mathematical Symbols-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U27C0.pdf</remarks>
        MiscellaneousMathematicalSymbolsA = 262144L,
        /// <summary>
        /// The Supplemental Arrows-A code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U27F0.pdf</remarks>
        SupplementalArrowsA = 524288L,
        /// <summary>
        /// The Braille Patterns code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2800.pdf</remarks>
        BraillePatterns = 1048576L,
        /// <summary>
        /// The Supplemental Arrows-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2900.pdf</remarks>
        SupplementalArrowsB = 2097152L,
        /// <summary>
        /// The Miscellaneous Mathematical Symbols-B code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2980.pdf</remarks>                
        MiscellaneousMathematicalSymbolsB = 4194304L,
        /// <summary>
        /// The Supplemental Mathematical Operators code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2A00.pdf</remarks>
        SupplementalMathematicalOperators = 8388608L,
        /// <summary>
        /// The Miscellaneous Symbols and Arrows code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2B00.pdf</remarks>        
        MiscellaneousSymbolsAndArrows = 16777216L,
        /// <summary>
        /// The Glagolitic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2C00.pdf</remarks>
        Glagolitic = 33554432L,
        /// <summary>
        /// The Latin Extended-C code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2C60.pdf</remarks>        
        LatinExtendedC = 67108864L,
        /// <summary>
        /// The Coptic code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2C80.pdf</remarks>
        Coptic = 134217728L,
        /// <summary>
        /// The Georgian Supplement code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2D00.pdf</remarks>
        GeorgianSupplement = 268435456L,
        /// <summary>
        /// The Tifinagh code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2D30.pdf</remarks>
        Tifinagh = 536870912L,
        /// <summary>
        /// The Ethiopic Extended code table.
        /// </summary>
        /// <remarks>http://www.unicode.org/charts/PDF/U2D80.pdf</remarks>
        EthiopicExtended = 16384L
    }
}
