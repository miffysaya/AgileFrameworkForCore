﻿using System.Collections;

namespace AgileFramework.Security.Application.CodeCharts
{
    /// <summary>
    /// Provides safe character positions for the lower middle section of the UTF code tables.
    /// </summary>
    internal static class LowerMiddle
    {
        /// <summary>
        /// Determines if the specified flag is set.
        /// </summary>
        /// <param name="flags">The value to check.</param>
        /// <param name="flagToCheck">The flag to check for.</param>
        /// <returns>true if the flag is set, otherwise false.</returns>
        public static bool IsFlagSet(LowerMidCodeCharts flags, LowerMidCodeCharts flagToCheck)
        {
            return (flags & flagToCheck) != LowerMidCodeCharts.None;
        }

        /// <summary>
        /// Provides the safe characters for the Myanmar code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Myanmar()
        {
            return CodeChartHelper.GetRange(4096, 4255);
        }

        /// <summary>
        /// Provides the safe characters for the Georgian code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>        
        public static IEnumerable Georgian()
        {
            return CodeChartHelper.GetRange(4256, 4348, (int i) => i >= 4294 && i <= 4303);
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Jamo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable HangulJamo()
        {
            return CodeChartHelper.GetRange(4352, 4607);
        }

        /// <summary>
        /// Provides the safe characters for the Ethiopic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Ethiopic()
        {
            return CodeChartHelper.GetRange(4608, 4988, (int i) => i == 4681 || i == 4686 || i == 4687 || i == 4695 || i == 4697 || i == 4702 || i == 4703 || i == 4745 || i == 4750 || i == 4751 || i == 4785 || i == 4790 || i == 4791 || i == 4799 || i == 4801 || i == 4806 || i == 4807 || i == 4823 || i == 4881 || i == 4886 || i == 4887 || (i >= 4955 && i <= 4958));
        }

        /// <summary>
        /// Provides the safe characters for the Ethiopic Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable EthiopicSupplement()
        {
            return CodeChartHelper.GetRange(4992, 5017);
        }

        /// <summary>
        /// Provides the safe characters for the Cherokee code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Cherokee()
        {
            return CodeChartHelper.GetRange(5024, 5108);
        }

        /// <summary>
        /// Provides the safe characters for the Unified Canadian Aboriginal Syllabic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable UnifiedCanadianAboriginalSyllabics()
        {
            return CodeChartHelper.GetRange(5120, 5759);
        }

        /// <summary>
        /// Provides the safe characters for the Ogham code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Ogham()
        {
            return CodeChartHelper.GetRange(5760, 5788);
        }

        /// <summary>
        /// Provides the safe characters for the Runic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Runic()
        {
            return CodeChartHelper.GetRange(5792, 5872);
        }

        /// <summary>
        /// Provides the safe characters for the Tagalog code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Tagalog()
        {
            return CodeChartHelper.GetRange(5888, 5908, (int i) => i == 5901);
        }

        /// <summary>
        /// Provides the safe characters for the Hanunoo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Hanunoo()
        {
            return CodeChartHelper.GetRange(5920, 5942);
        }

        /// <summary>
        /// Provides the safe characters for the Buhid code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Buhid()
        {
            return CodeChartHelper.GetRange(5952, 5971);
        }

        /// <summary>
        /// Provides the safe characters for the Tagbanwa code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Tagbanwa()
        {
            return CodeChartHelper.GetRange(5984, 6003, (int i) => i == 5997 || i == 6001);
        }

        /// <summary>
        /// Provides the safe characters for the Khmer code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Khmer()
        {
            return CodeChartHelper.GetRange(6016, 6137, (int i) => i == 6110 || i == 6111 || (i >= 6122 && i <= 6127));
        }

        /// <summary>
        /// Provides the safe characters for the Mongolian code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Mongolian()
        {
            return CodeChartHelper.GetRange(6144, 6314, (int i) => i == 6159 || (i >= 6170 && i <= 6175) || (i >= 6264 && i <= 6271));
        }

        /// <summary>
        /// Provides the safe characters for the Unified Canadian Aboriginal Syllabic Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable UnifiedCanadianAboriginalSyllabicsExtended()
        {
            return CodeChartHelper.GetRange(6320, 6389);
        }

        /// <summary>
        /// Provides the safe characters for the Limbu code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable Limbu()
        {
            return CodeChartHelper.GetRange(6400, 6479, (int i) => i == 6429 || i == 6430 || i == 6431 || (i >= 6444 && i <= 6447) || (i >= 6460 && i <= 6463) || i == 6465 || i == 6466 || i == 6467);
        }

        /// <summary>
        /// Provides the safe characters for the Tai Le code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable TaiLe()
        {
            return CodeChartHelper.GetRange(6480, 6516, (int i) => i == 6510 || i == 6511);
        }

        /// <summary>
        /// Provides the safe characters for the New Tai Lue code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>          
        public static IEnumerable NewTaiLue()
        {
            return CodeChartHelper.GetRange(6528, 6623, (int i) => (i >= 6572 && i <= 6575) || (i >= 6602 && i <= 6607) || (i >= 6619 && i <= 6621));
        }

        /// <summary>
        /// Provides the safe characters for the Khmer Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable KhmerSymbols()
        {
            return CodeChartHelper.GetRange(6624, 6655);
        }

        /// <summary>
        /// Provides the safe characters for the Khmer Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Buginese()
        {
            return CodeChartHelper.GetRange(6656, 6687, (int i) => i == 6684 || i == 6685);
        }

        /// <summary>
        /// Provides the safe characters for the Tai Tham code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable TaiTham()
        {
            return CodeChartHelper.GetRange(6688, 6829, (int i) => i == 6751 || i == 6781 || i == 6782 || (i >= 6794 && i <= 6799) || (i >= 6810 && i <= 6815));
        }

        /// <summary>
        /// Provides the safe characters for the Balinese code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Balinese()
        {
            return CodeChartHelper.GetRange(6912, 7036, (int i) => i >= 6988 && i <= 6991);
        }

        /// <summary>
        /// Provides the safe characters for the Sudanese code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Sudanese()
        {
            return CodeChartHelper.GetRange(7040, 7097, (int i) => i >= 7083 && i <= 7085);
        }

        /// <summary>
        /// Provides the safe characters for the Lepcha code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Lepcha()
        {
            return CodeChartHelper.GetRange(7168, 7247, (int i) => (i >= 7224 && i <= 7226) || (i >= 7242 && i <= 7244));
        }

        /// <summary>
        /// Provides the safe characters for the Ol Chiki code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable OlChiki()
        {
            return CodeChartHelper.GetRange(7248, 7295);
        }

        /// <summary>
        /// Provides the safe characters for the Vedic Extensions code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable VedicExtensions()
        {
            return CodeChartHelper.GetRange(7376, 7410);
        }

        /// <summary>
        /// Provides the safe characters for the Phonetic Extensions code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable PhoneticExtensions()
        {
            return CodeChartHelper.GetRange(7424, 7551);
        }

        /// <summary>
        /// Provides the safe characters for the Phonetic Extensions Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable PhoneticExtensionsSupplement()
        {
            return CodeChartHelper.GetRange(7552, 7615);
        }

        /// <summary>
        /// Provides the safe characters for the Combining Diacritical Marks Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable CombiningDiacriticalMarksSupplement()
        {
            return CodeChartHelper.GetRange(7616, 7679, (int i) => i >= 7655 && i <= 7676);
        }

        /// <summary>
        /// Provides the safe characters for the Latin Extended Addition code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable LatinExtendedAdditional()
        {
            return CodeChartHelper.GetRange(7680, 7935);
        }
    }
}
