using AgileFramework.Security.Exchange.Data.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// The culture character set database
    /// </summary>
    internal static class CultureCharsetDatabase
    {
        /// <summary>
        /// The character set name.
        /// </summary>
        private struct CharsetName
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The code page name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// Gets the name of the character set name.
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Gets the code page identifier.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return codePage;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.CharsetName" /> struct.
            /// </summary>
            /// <param name="name">
            /// The code page name.
            /// </param>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            public CharsetName(string name, int codePage)
            {
                this.name = name;
                this.codePage = codePage;
            }
        }

        /// <summary>
        /// The code page culture override.
        /// </summary>
        private struct CodePageCultureOverride
        {
            /// <summary>
            /// The code page identifier.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// Gets the code page identifier.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return codePage;
                }
            }

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return cultureName;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.CodePageCultureOverride" /> struct.
            /// </summary>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            public CodePageCultureOverride(int codePage, string cultureName)
            {
                this.codePage = codePage;
                this.cultureName = cultureName;
            }
        }

        /// <summary>
        /// The culture and code page override structure.
        /// </summary>
        private struct CultureCodePageOverride
        {
            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// The mime code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return cultureName;
                }
            }

            /// <summary>
            /// Gets the MIME code page identifier.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page identifier.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return webCodePage;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.CultureCodePageOverride" /> struct.
            /// </summary>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            /// <param name="mimeCodePage">
            /// The mime code page.
            /// </param>
            /// <param name="webCodePage">
            /// The web code page.
            /// </param>
            public CultureCodePageOverride(string cultureName, int mimeCodePage, int webCodePage)
            {
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
            }
        }

        /// <summary>
        /// A structure representing the data for culture.
        /// </summary>
        private struct CultureData
        {
            /// <summary>
            /// The culture description.
            /// </summary>
            private readonly string description;

            /// <summary>
            /// The locale identifier.
            /// </summary>
            private readonly int localeId;

            /// <summary>
            /// The mime code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The parent culture name.
            /// </summary>
            private readonly string parentCultureName;

            /// <summary>
            /// The web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// The Windows code page.
            /// </summary>
            private readonly int windowsCodePage;

            /// <summary>
            /// Gets the culture locale identifier.
            /// </summary>
            public int LocaleId
            {
                get
                {
                    return localeId;
                }
            }

            /// <summary>
            /// Gets the culture name.
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Gets the Windows code page for this culture.
            /// </summary>
            public int WindowsCodePage
            {
                get
                {
                    return windowsCodePage;
                }
            }

            /// <summary>
            /// Gets the MIME code page.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return webCodePage;
                }
            }

            /// <summary>
            /// Gets the name of the parent culture.
            /// </summary>
            public string ParentCultureName
            {
                get
                {
                    return parentCultureName;
                }
            }

            /// <summary>
            /// Gets the description of the culture.
            /// </summary>
            public string Description
            {
                get
                {
                    return description;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.CultureData" /> struct.
            /// </summary>
            /// <param name="localeId">
            /// The local identifier.
            /// </param>
            /// <param name="name">
            /// The culture name.
            /// </param>
            /// <param name="windowsCodePage">
            /// The windows code page.
            /// </param>
            /// <param name="mimeCodePage">
            /// The mime code page.
            /// </param>
            /// <param name="webCodePage">
            /// The web code page.
            /// </param>
            /// <param name="parentCultureName">
            /// The parent culture name.
            /// </param>
            /// <param name="description">
            /// The culture description.
            /// </param>
            public CultureData(int localeId, string name, int windowsCodePage, int mimeCodePage, int webCodePage, string parentCultureName, string description)
            {
                this.localeId = localeId;
                this.name = name;
                this.windowsCodePage = windowsCodePage;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.parentCultureName = parentCultureName;
                this.description = description;
            }
        }

        /// <summary>
        /// A structure encapsulting Windows code page.
        /// </summary>
        private struct InternalWindowsCodePage
        {
            /// <summary>
            /// The code page.
            /// </summary>
            private readonly int codePage;

            /// <summary>
            /// The culture name.
            /// </summary>
            private readonly string cultureName;

            /// <summary>
            /// The generic culture description.
            /// </summary>
            private readonly string genericCultureDescription;

            /// <summary>
            /// The locale identifier.
            /// </summary>
            private readonly int localeId;

            /// <summary>
            /// The MIME code page.
            /// </summary>
            private readonly int mimeCodePage;

            /// <summary>
            /// The code page name.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The equivilant web code page.
            /// </summary>
            private readonly int webCodePage;

            /// <summary>
            /// Gets the code page number.
            /// </summary>
            public int CodePage
            {
                get
                {
                    return codePage;
                }
            }

            /// <summary>
            /// Gets the name of the code page.
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
            }

            /// <summary>
            /// Gets the locale identifier.
            /// </summary>
            public int LocaleId
            {
                get
                {
                    return localeId;
                }
            }

            /// <summary>
            /// Gets the name of the culture.
            /// </summary>
            public string CultureName
            {
                get
                {
                    return cultureName;
                }
            }

            /// <summary>
            /// Gets the MIME code page.
            /// </summary>
            public int MimeCodePage
            {
                get
                {
                    return mimeCodePage;
                }
            }

            /// <summary>
            /// Gets the web code page.
            /// </summary>
            public int WebCodePage
            {
                get
                {
                    return webCodePage;
                }
            }

            /// <summary>
            /// Gets the generic culture description.
            /// </summary>
            public string GenericCultureDescription
            {
                get
                {
                    return genericCultureDescription;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.InternalWindowsCodePage" /> struct.
            /// </summary>
            /// <param name="codePage">
            /// The code page.
            /// </param>
            /// <param name="name">
            /// The name of the code page.
            /// </param>
            /// <param name="localeId">
            /// The locale id.
            /// </param>
            /// <param name="cultureName">
            /// The culture name.
            /// </param>
            /// <param name="mimeCodePage">
            /// The MIME code page.
            /// </param>
            /// <param name="webCodePage">
            /// The equivilant web code page.
            /// </param>
            /// <param name="genericCultureDescription">
            /// The generic culture description.
            /// </param>
            public InternalWindowsCodePage(int codePage, string name, int localeId, string cultureName, int mimeCodePage, int webCodePage, string genericCultureDescription)
            {
                this.codePage = codePage;
                this.name = name;
                this.localeId = localeId;
                this.cultureName = cultureName;
                this.mimeCodePage = mimeCodePage;
                this.webCodePage = webCodePage;
                this.genericCultureDescription = genericCultureDescription;
            }
        }

        /// <summary>
        /// Encapsultates globalization data
        /// </summary>
        internal class GlobalizationData
        {
            /// <summary>
            /// The local identifier to culture map.
            /// </summary>
            private Dictionary<int, Culture> localeIdToCulture = new Dictionary<int, Culture>(IntComparerInstance);

            /// <summary>
            /// The name to character set map.
            /// </summary>
            private Dictionary<string, Charset> nameToCharset = new Dictionary<string, Charset>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// The name to culture map.
            /// </summary>
            private Dictionary<string, Culture> nameToCulture = new Dictionary<string, Culture>(StringComparer.OrdinalIgnoreCase);

            /// <summary>
            /// The code page to charset map.
            /// </summary>
            private Dictionary<int, Charset> codePageToCharset = new Dictionary<int, Charset>(IntComparerInstance);

            /// <summary>
            /// Gets or sets the default culture.
            /// </summary>
            public Culture DefaultCulture
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the unicode charset.
            /// </summary>
            public Charset UnicodeCharset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the utf 8 charset.
            /// </summary>
            public Charset Utf8Charset
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the default detection priority order.
            /// </summary>
            public int[] DefaultDetectionPriorityOrder
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the code page to charset map.
            /// </summary>
            public Dictionary<int, Charset> CodePageToCharset
            {
                get
                {
                    return codePageToCharset;
                }
                set
                {
                    codePageToCharset = value;
                }
            }

            /// <summary>
            /// Gets or sets the name to character set map.
            /// </summary>
            public Dictionary<string, Charset> NameToCharset
            {
                get
                {
                    return nameToCharset;
                }
                set
                {
                    nameToCharset = value;
                }
            }

            /// <summary>
            /// Gets or sets the local identifier to culture map.
            /// </summary>
            public Dictionary<int, Culture> LocaleIdToCulture
            {
                get
                {
                    return localeIdToCulture;
                }
                set
                {
                    localeIdToCulture = value;
                }
            }

            /// <summary>
            /// Gets or sets the maximum length for a character set name.
            /// </summary>
            internal int MaxCharsetNameLength
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name to culture map.
            /// </summary>
            internal Dictionary<string, Culture> NameToCulture
            {
                get
                {
                    return nameToCulture;
                }
                set
                {
                    nameToCulture = value;
                }
            }

            /// <summary>
            /// Gets or sets the ascii charset.
            /// </summary>
            internal Charset AsciiCharset
            {
                get;
                set;
            }
        }

        /// <summary>
        /// An integer comparator.
        /// </summary>
        private sealed class IntComparer : IEqualityComparer<int>
        {
            /// <summary>
            /// Determines whether the specified integers are equal.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>True if the integers are equal, otherwise false.</returns>
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            /// <summary>
            /// Returns a hash code for the specified integer.
            /// </summary>
            /// <param name="obj">
            /// The integer for which a hash code is to be returned.
            /// </param>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            public int GetHashCode(int obj)
            {
                return obj;
            }
        }

        /// <summary>
        /// An instance of the integer comparer.
        /// </summary>
        private static readonly IntComparer IntComparerInstance = new IntComparer();

        /// <summary>
        /// The internal globalization data.
        /// </summary>
        private static GlobalizationData internalGlobalizationDataStore = LoadGlobalizationData(null);

        /// <summary>
        /// Gets or sets the internal globalization data.
        /// </summary>
        internal static GlobalizationData InternalGlobalizationData
        {
            get
            {
                return internalGlobalizationDataStore;
            }
            set
            {
                internalGlobalizationDataStore = value;
            }
        }

        /// <summary>
        /// Gets the code page detection list, ordered by priority for the specified Culture.
        /// </summary>
        /// <param name="culture">
        /// The culture to get the ordered detection list for.
        /// </param>
        /// <param name="originalPriorityList">
        /// The original priority list.
        /// </param>
        /// <returns>
        /// An ordered list of the code page ids.
        /// </returns>
        internal static int[] GetCultureSpecificCodepageDetectionPriorityOrder(Culture culture, int[] originalPriorityList)
        {
            int[] array = new int[CodePageDetectData.CodePages.Length];
            int listCount = 0;
            array[listCount++] = 20127;
            if (culture.MimeCharset.IsDetectable && !IsDbcs(culture.MimeCharset.CodePage) && !InList(culture.MimeCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.MimeCharset.CodePage;
            }
            if (culture.WebCharset.IsDetectable && !IsDbcs(culture.WebCharset.CodePage) && !InList(culture.WebCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.WebCharset.CodePage;
            }
            if (culture.WindowsCharset.IsDetectable && !IsDbcs(culture.WindowsCharset.CodePage) && !InList(culture.WindowsCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.WindowsCharset.CodePage;
            }
            if (originalPriorityList != null)
            {
                for (int i = 0; i < originalPriorityList.Length; i++)
                {
                    if (!IsDbcs(originalPriorityList[i]) && !InList(originalPriorityList[i], array, listCount) && IsSameLanguage(originalPriorityList[i], culture.WindowsCharset.CodePage))
                    {
                        array[listCount++] = originalPriorityList[i];
                    }
                }
            }
            else
            {
                for (int j = 0; j < CodePageDetectData.CodePages.Length; j++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[j].Id) && !InList(CodePageDetectData.CodePages[j].Id, array, listCount) && IsSameLanguage(CodePageDetectData.CodePages[j].Id, culture.WindowsCharset.CodePage))
                    {
                        array[listCount++] = CodePageDetectData.CodePages[j].Id;
                    }
                }
            }
            if (originalPriorityList != null)
            {
                for (int k = 0; k < originalPriorityList.Length; k++)
                {
                    if (!IsDbcs(originalPriorityList[k]) && !InList(originalPriorityList[k], array, listCount))
                    {
                        array[listCount++] = originalPriorityList[k];
                    }
                }
            }
            else
            {
                for (int l = 0; l < CodePageDetectData.CodePages.Length; l++)
                {
                    if (!IsDbcs(CodePageDetectData.CodePages[l].Id) && !InList(CodePageDetectData.CodePages[l].Id, array, listCount))
                    {
                        array[listCount++] = CodePageDetectData.CodePages[l].Id;
                    }
                }
            }
            if (culture.MimeCharset.IsDetectable && IsDbcs(culture.MimeCharset.CodePage) && !InList(culture.MimeCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.MimeCharset.CodePage;
            }
            if (culture.WebCharset.IsDetectable && IsDbcs(culture.WebCharset.CodePage) && !InList(culture.WebCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.WebCharset.CodePage;
            }
            if (culture.WindowsCharset.IsDetectable && IsDbcs(culture.WindowsCharset.CodePage) && !InList(culture.WindowsCharset.CodePage, array, listCount))
            {
                array[listCount++] = culture.WindowsCharset.CodePage;
            }
            if (originalPriorityList != null)
            {
                for (int m = 0; m < originalPriorityList.Length; m++)
                {
                    if (IsDbcs(originalPriorityList[m]) && !InList(originalPriorityList[m], array, listCount) && IsSameLanguage(originalPriorityList[m], culture.WindowsCharset.CodePage))
                    {
                        array[listCount++] = originalPriorityList[m];
                    }
                }
            }
            else
            {
                for (int n = 0; n < CodePageDetectData.CodePages.Length; n++)
                {
                    if (IsDbcs(CodePageDetectData.CodePages[n].Id) && !InList(CodePageDetectData.CodePages[n].Id, array, listCount) && IsSameLanguage(CodePageDetectData.CodePages[n].Id, culture.WindowsCharset.CodePage))
                    {
                        array[listCount++] = CodePageDetectData.CodePages[n].Id;
                    }
                }
            }
            if (originalPriorityList != null)
            {
                for (int num = 0; num < originalPriorityList.Length; num++)
                {
                    if (!InList(originalPriorityList[num], array, listCount))
                    {
                        array[listCount++] = originalPriorityList[num];
                    }
                }
            }
            else
            {
                for (int num2 = 0; num2 < CodePageDetectData.CodePages.Length; num2++)
                {
                    if (!InList(CodePageDetectData.CodePages[num2].Id, array, listCount))
                    {
                        array[listCount++] = CodePageDetectData.CodePages[num2].Id;
                    }
                }
            }
            if (originalPriorityList != null)
            {
                for (int num3 = 0; num3 < originalPriorityList.Length; num3++)
                {
                    if (array[num3] != originalPriorityList[num3])
                    {
                        return array;
                    }
                }
                return originalPriorityList;
            }
            return array;
        }

        /// <summary>
        /// Loads the globalization data with the specified default culture name.
        /// </summary>
        /// <param name="defaultCultureName">
        /// The default culture name.
        /// </param>
        /// <returns>
        /// A <see cref="T:Microsoft.Exchange.Data.Globalization.CultureCharsetDatabase.GlobalizationData" /> class using the default culture nane specified.
        /// </returns>
        private static GlobalizationData LoadGlobalizationData(string defaultCultureName)
        {
            InternalWindowsCodePage[] array = new InternalWindowsCodePage[]
            {
                new InternalWindowsCodePage(1200, "unicode", 0, null, 65001, 65001, "Unicode generic culture"),
                new InternalWindowsCodePage(1250, "windows-1250", 0, null, 28592, 1250, "Central European generic culture"),
                new InternalWindowsCodePage(1251, "windows-1251", 0, null, 20866, 1251, "Cyrillic generic culture"),
                new InternalWindowsCodePage(1252, "windows-1252", 0, null, 28591, 1252, "Western European generic culture"),
                new InternalWindowsCodePage(1253, "windows-1253", 8, "el", 28597, 1253, null),
                new InternalWindowsCodePage(1254, "windows-1254", 0, null, 28599, 1254, "Turkish / Azeri generic culture"),
                new InternalWindowsCodePage(1255, "windows-1255", 13, "he", 1255, 1255, null),
                new InternalWindowsCodePage(1256, "windows-1256", 0, null, 1256, 1256, "Arabic generic culture"),
                new InternalWindowsCodePage(1257, "windows-1257", 0, null, 1257, 1257, "Baltic generic culture"),
                new InternalWindowsCodePage(1258, "windows-1258", 42, "vi", 1258, 1258, null),
                new InternalWindowsCodePage(874, "windows-874", 30, "th", 874, 874, null),
                new InternalWindowsCodePage(932, "windows-932", 17, "ja", 50220, 932, null),
                new InternalWindowsCodePage(936, "windows-936", 4, "zh-CHS", 936, 936, null),
                new InternalWindowsCodePage(949, "windows-949", 18, "ko", 949, 949, null),
                new InternalWindowsCodePage(950, "windows-950", 31748, "zh-CHT", 950, 950, null)
            };
            CodePageCultureOverride[] array2 = new CodePageCultureOverride[]
            {
                new CodePageCultureOverride(37, "en"),
                new CodePageCultureOverride(437, "en"),
                new CodePageCultureOverride(860, "pt"),
                new CodePageCultureOverride(861, "is"),
                new CodePageCultureOverride(863, "fr-CA"),
                new CodePageCultureOverride(1141, "de"),
                new CodePageCultureOverride(1144, "it"),
                new CodePageCultureOverride(1145, "es"),
                new CodePageCultureOverride(1146, "en-GB"),
                new CodePageCultureOverride(1147, "fr"),
                new CodePageCultureOverride(1149, "is"),
                new CodePageCultureOverride(10010, "ro"),
                new CodePageCultureOverride(10017, "uk"),
                new CodePageCultureOverride(10079, "is"),
                new CodePageCultureOverride(10082, "hr"),
                new CodePageCultureOverride(20106, "de"),
                new CodePageCultureOverride(20107, "sv"),
                new CodePageCultureOverride(20108, "no"),
                new CodePageCultureOverride(20127, "en"),
                new CodePageCultureOverride(20273, "de"),
                new CodePageCultureOverride(20280, "it"),
                new CodePageCultureOverride(20284, "es"),
                new CodePageCultureOverride(20285, "en-GB"),
                new CodePageCultureOverride(20297, "fr"),
                new CodePageCultureOverride(20866, "ru"),
                new CodePageCultureOverride(20871, "is"),
                new CodePageCultureOverride(20880, "ru"),
                new CodePageCultureOverride(21866, "uk"),
                new CodePageCultureOverride(57003, "bn-IN"),
                new CodePageCultureOverride(57004, "ta"),
                new CodePageCultureOverride(57005, "te"),
                new CodePageCultureOverride(57008, "kn"),
                new CodePageCultureOverride(57009, "ml-IN"),
                new CodePageCultureOverride(57010, "gu"),
                new CodePageCultureOverride(57011, "pa")
            };
            CultureCodePageOverride[] array3 = new CultureCodePageOverride[]
            {
                new CultureCodePageOverride("et", 28605, 28605),
                new CultureCodePageOverride("lt", 28603, 28603),
                new CultureCodePageOverride("lv", 28603, 28603),
                new CultureCodePageOverride("uk", 21866, 1251),
                new CultureCodePageOverride("az-AZ-Cyrl", 1251, 1251),
                new CultureCodePageOverride("be", 1251, 1251),
                new CultureCodePageOverride("bg", 1251, 1251),
                new CultureCodePageOverride("mk", 1251, 1251),
                new CultureCodePageOverride("sr", 1251, 1251),
                new CultureCodePageOverride("sr-BA-Cyrl", 1251, 1251),
                new CultureCodePageOverride("sr-Cyrl-CS", 1251, 1251),
                new CultureCodePageOverride("ky", 1251, 1251),
                new CultureCodePageOverride("kk", 1251, 1251),
                new CultureCodePageOverride("tt", 1251, 1251),
                new CultureCodePageOverride("uz-UZ-Cyrl", 1251, 1251),
                new CultureCodePageOverride("mn", 65001, 65001)
            };
            CharsetName[] array4 = new CharsetName[]
            {
                new CharsetName("_autodetect", 50932),
                new CharsetName("_autodetect_all", 50001),
                new CharsetName("_autodetect_kr", 50949),
                new CharsetName("_iso-2022-jp$ESC", 50221),
                new CharsetName("_iso-2022-jp$SIO", 50222),
                new CharsetName("437", 437),
                new CharsetName("ANSI_X3.4-1968", 20127),
                new CharsetName("ANSI_X3.4-1986", 20127),
                new CharsetName("arabic", 28596),
                new CharsetName("ascii", 20127),
                new CharsetName("ASMO-708", 708),
                new CharsetName("Big5-HKSCS", 950),
                new CharsetName("Big5", 950),
                new CharsetName("CCSID00858", 858),
                new CharsetName("CCSID00924", 20924),
                new CharsetName("CCSID01140", 1140),
                new CharsetName("CCSID01141", 1141),
                new CharsetName("CCSID01142", 1142),
                new CharsetName("CCSID01143", 1143),
                new CharsetName("CCSID01144", 1144),
                new CharsetName("CCSID01145", 1145),
                new CharsetName("CCSID01146", 1146),
                new CharsetName("CCSID01147", 1147),
                new CharsetName("CCSID01148", 1148),
                new CharsetName("CCSID01149", 1149),
                new CharsetName("chinese", 936),
                new CharsetName("cn-big5", 950),
                new CharsetName("CN-GB", 936),
                new CharsetName("CP00858", 858),
                new CharsetName("CP00924", 20924),
                new CharsetName("CP01140", 1140),
                new CharsetName("CP01141", 1141),
                new CharsetName("CP01142", 1142),
                new CharsetName("CP01143", 1143),
                new CharsetName("CP01144", 1144),
                new CharsetName("CP01145", 1145),
                new CharsetName("CP01146", 1146),
                new CharsetName("CP01147", 1147),
                new CharsetName("CP01148", 1148),
                new CharsetName("CP01149", 1149),
                new CharsetName("cp037", 37),
                new CharsetName("cp1025", 21025),
                new CharsetName("CP1026", 1026),
                new CharsetName("cp1256", 1256),
                new CharsetName("CP273", 20273),
                new CharsetName("CP278", 20278),
                new CharsetName("CP280", 20280),
                new CharsetName("CP284", 20284),
                new CharsetName("CP285", 20285),
                new CharsetName("cp290", 20290),
                new CharsetName("cp297", 20297),
                new CharsetName("cp367", 20127),
                new CharsetName("cp420", 20420),
                new CharsetName("cp423", 20423),
                new CharsetName("cp424", 20424),
                new CharsetName("cp437", 437),
                new CharsetName("CP500", 500),
                new CharsetName("cp50227", 50227),
                new CharsetName("cp50229", 50229),
                new CharsetName("cp819", 28591),
                new CharsetName("cp850", 850),
                new CharsetName("cp852", 852),
                new CharsetName("cp855", 855),
                new CharsetName("cp857", 857),
                new CharsetName("cp858", 858),
                new CharsetName("cp860", 860),
                new CharsetName("cp861", 861),
                new CharsetName("cp862", 862),
                new CharsetName("cp863", 863),
                new CharsetName("cp864", 864),
                new CharsetName("cp865", 865),
                new CharsetName("cp866", 866),
                new CharsetName("cp869", 869),
                new CharsetName("CP870", 870),
                new CharsetName("CP871", 20871),
                new CharsetName("cp875", 875),
                new CharsetName("cp880", 20880),
                new CharsetName("CP905", 20905),
                new CharsetName("cp930", 50930),
                new CharsetName("cp933", 50933),
                new CharsetName("cp935", 50935),
                new CharsetName("cp937", 50937),
                new CharsetName("cp939", 50939),
                new CharsetName("csASCII", 20127),
                new CharsetName("csbig5", 950),
                new CharsetName("csEUCKR", 51949),
                new CharsetName("csEUCPkdFmtJapanese", 51932),
                new CharsetName("csGB2312", 936),
                new CharsetName("csGB231280", 936),
                new CharsetName("csIBM037", 37),
                new CharsetName("csIBM1026", 1026),
                new CharsetName("csIBM273", 20273),
                new CharsetName("csIBM277", 20277),
                new CharsetName("csIBM278", 20278),
                new CharsetName("csIBM280", 20280),
                new CharsetName("csIBM284", 20284),
                new CharsetName("csIBM285", 20285),
                new CharsetName("csIBM290", 20290),
                new CharsetName("csIBM297", 20297),
                new CharsetName("csIBM420", 20420),
                new CharsetName("csIBM423", 20423),
                new CharsetName("csIBM424", 20424),
                new CharsetName("csIBM500", 500),
                new CharsetName("csIBM870", 870),
                new CharsetName("csIBM871", 20871),
                new CharsetName("csIBM880", 20880),
                new CharsetName("csIBM905", 20905),
                new CharsetName("csIBMThai", 20838),
                new CharsetName("csISO2022JP", 50221),
                new CharsetName("csISO2022KR", 50225),
                new CharsetName("csISO58GB231280", 936),
                new CharsetName("csISOLatin1", 28591),
                new CharsetName("csISOLatin2", 28592),
                new CharsetName("csISOLatin3", 28593),
                new CharsetName("csISOLatin4", 28594),
                new CharsetName("csISOLatin5", 28599),
                new CharsetName("csISOLatin9", 28605),
                new CharsetName("csISOLatinArabic", 28596),
                new CharsetName("csISOLatinCyrillic", 28595),
                new CharsetName("csISOLatinGreek", 28597),
                new CharsetName("csISOLatinHebrew", 38598),
                new CharsetName("csKOI8R", 20866),
                new CharsetName("csKSC56011987", 949),
                new CharsetName("csPC8CodePage437", 437),
                new CharsetName("csShiftJIS", 932),
                new CharsetName("csUnicode11UTF7", 65000),
                new CharsetName("csWindows31J", 932),
                new CharsetName("Windows-31J", 932),
                new CharsetName("cyrillic", 28595),
                new CharsetName("DIN_66003", 20106),
                new CharsetName("DOS-720", 720),
                new CharsetName("DOS-862", 862),
                new CharsetName("DOS-874", 874),
                new CharsetName("ebcdic-cp-ar1", 20420),
                new CharsetName("ebcdic-cp-be", 500),
                new CharsetName("ebcdic-cp-ca", 37),
                new CharsetName("ebcdic-cp-ch", 500),
                new CharsetName("EBCDIC-CP-DK", 20277),
                new CharsetName("ebcdic-cp-es", 20284),
                new CharsetName("ebcdic-cp-fi", 20278),
                new CharsetName("ebcdic-cp-fr", 20297),
                new CharsetName("ebcdic-cp-gb", 20285),
                new CharsetName("ebcdic-cp-gr", 20423),
                new CharsetName("ebcdic-cp-he", 20424),
                new CharsetName("ebcdic-cp-is", 20871),
                new CharsetName("ebcdic-cp-it", 20280),
                new CharsetName("ebcdic-cp-nl", 37),
                new CharsetName("EBCDIC-CP-NO", 20277),
                new CharsetName("ebcdic-cp-roece", 870),
                new CharsetName("ebcdic-cp-se", 20278),
                new CharsetName("ebcdic-cp-tr", 20905),
                new CharsetName("ebcdic-cp-us", 37),
                new CharsetName("ebcdic-cp-wt", 37),
                new CharsetName("ebcdic-cp-yu", 870),
                new CharsetName("EBCDIC-Cyrillic", 20880),
                new CharsetName("ebcdic-de-273+euro", 1141),
                new CharsetName("ebcdic-dk-277+euro", 1142),
                new CharsetName("ebcdic-es-284+euro", 1145),
                new CharsetName("ebcdic-fi-278+euro", 1143),
                new CharsetName("ebcdic-fr-297+euro", 1147),
                new CharsetName("ebcdic-gb-285+euro", 1146),
                new CharsetName("ebcdic-international-500+euro", 1148),
                new CharsetName("ebcdic-is-871+euro", 1149),
                new CharsetName("ebcdic-it-280+euro", 1144),
                new CharsetName("EBCDIC-JP-kana", 20290),
                new CharsetName("ebcdic-Latin9--euro", 20924),
                new CharsetName("ebcdic-no-277+euro", 1142),
                new CharsetName("ebcdic-se-278+euro", 1143),
                new CharsetName("ebcdic-us-37+euro", 1140),
                new CharsetName("ECMA-114", 28596),
                new CharsetName("ECMA-118", 28597),
                new CharsetName("ELOT_928", 28597),
                new CharsetName("euc-cn", 51936),
                new CharsetName("euc-jp", 51932),
                new CharsetName("euc-kr", 51949),
                new CharsetName("Extended_UNIX_Code_Packed_Format_for_Japanese", 51932),
                new CharsetName("GB_2312-80", 936),
                new CharsetName("GB18030", 54936),
                new CharsetName("GB2312-80", 936),
                new CharsetName("GB2312", 936),
                new CharsetName("GB231280", 936),
                new CharsetName("GBK", 936),
                new CharsetName("German", 20106),
                new CharsetName("greek", 28597),
                new CharsetName("greek8", 28597),
                new CharsetName("hebrew", 38598),
                new CharsetName("hz-gb-2312", 52936),
                new CharsetName("IBM-Thai", 20838),
                new CharsetName("IBM00858", 858),
                new CharsetName("IBM00924", 20924),
                new CharsetName("IBM01047", 1047),
                new CharsetName("IBM01140", 1140),
                new CharsetName("IBM01141", 1141),
                new CharsetName("IBM01142", 1142),
                new CharsetName("IBM01143", 1143),
                new CharsetName("IBM01144", 1144),
                new CharsetName("IBM01145", 1145),
                new CharsetName("IBM01146", 1146),
                new CharsetName("IBM01147", 1147),
                new CharsetName("IBM01148", 1148),
                new CharsetName("IBM01149", 1149),
                new CharsetName("IBM037", 37),
                new CharsetName("IBM1026", 1026),
                new CharsetName("IBM273", 20273),
                new CharsetName("IBM277", 20277),
                new CharsetName("IBM278", 20278),
                new CharsetName("IBM280", 20280),
                new CharsetName("IBM284", 20284),
                new CharsetName("IBM285", 20285),
                new CharsetName("IBM290", 20290),
                new CharsetName("IBM297", 20297),
                new CharsetName("IBM367", 20127),
                new CharsetName("IBM420", 20420),
                new CharsetName("IBM423", 20423),
                new CharsetName("IBM424", 20424),
                new CharsetName("IBM437", 437),
                new CharsetName("IBM500", 500),
                new CharsetName("ibm737", 737),
                new CharsetName("ibm775", 775),
                new CharsetName("ibm819", 28591),
                new CharsetName("IBM850", 850),
                new CharsetName("IBM852", 852),
                new CharsetName("IBM855", 855),
                new CharsetName("IBM857", 857),
                new CharsetName("IBM860", 860),
                new CharsetName("IBM861", 861),
                new CharsetName("IBM862", 862),
                new CharsetName("IBM863", 863),
                new CharsetName("IBM864", 864),
                new CharsetName("IBM865", 865),
                new CharsetName("IBM866", 866),
                new CharsetName("IBM869", 869),
                new CharsetName("IBM870", 870),
                new CharsetName("IBM871", 20871),
                new CharsetName("IBM880", 20880),
                new CharsetName("IBM905", 20905),
                new CharsetName("irv", 20105),
                new CharsetName("ISO-10646-UCS-2", 1200),
                new CharsetName("iso-2022-jp", 50220),
                new CharsetName("iso-2022-jpdbcs", 50220),
                new CharsetName("iso-2022-jpesc", 50221),
                new CharsetName("iso-2022-jpsio", 50222),
                new CharsetName("iso-2022-jpeuc", 51932),
                new CharsetName("iso-2022-kr-7", 50225),
                new CharsetName("iso-2022-kr-7bit", 50225),
                new CharsetName("iso-2022-kr-8", 51949),
                new CharsetName("iso-2022-kr-8bit", 51949),
                new CharsetName("iso-2022-kr", 50225),
                new CharsetName("iso-8859-1", 28591),
                new CharsetName("iso-8859-11", 874),
                new CharsetName("iso-8859-13", 28603),
                new CharsetName("iso-8859-15", 28605),
                new CharsetName("iso-8859-2", 28592),
                new CharsetName("iso-8859-3", 28593),
                new CharsetName("iso-8859-4", 28594),
                new CharsetName("iso-8859-5", 28595),
                new CharsetName("iso-8859-6", 28596),
                new CharsetName("iso-8859-7", 28597),
                new CharsetName("iso-8859-8-i", 38598),
                new CharsetName("ISO-8859-8 Visual", 28598),
                new CharsetName("iso-8859-8", 28598),
                new CharsetName("iso-8859-9", 28599),
                new CharsetName("iso-ir-100", 28591),
                new CharsetName("iso-ir-101", 28592),
                new CharsetName("iso-ir-109", 28593),
                new CharsetName("iso-ir-110", 28594),
                new CharsetName("iso-ir-126", 28597),
                new CharsetName("iso-ir-127", 28596),
                new CharsetName("iso-ir-138", 38598),
                new CharsetName("iso-ir-144", 28595),
                new CharsetName("iso-ir-148", 28599),
                new CharsetName("iso-ir-149", 949),
                new CharsetName("iso-ir-58", 936),
                new CharsetName("iso-ir-6", 20127),
                new CharsetName("ISO_646.irv:1991", 20127),
                new CharsetName("iso_8859-1", 28591),
                new CharsetName("iso_8859-1:1987", 28591),
                new CharsetName("ISO_8859-15", 28605),
                new CharsetName("iso_8859-2", 28592),
                new CharsetName("iso_8859-2:1987", 28592),
                new CharsetName("ISO_8859-3", 28593),
                new CharsetName("ISO_8859-3:1988", 28593),
                new CharsetName("ISO_8859-4", 28594),
                new CharsetName("ISO_8859-4:1988", 28594),
                new CharsetName("ISO_8859-5", 28595),
                new CharsetName("ISO_8859-5:1988", 28595),
                new CharsetName("ISO_8859-6", 28596),
                new CharsetName("ISO_8859-6:1987", 28596),
                new CharsetName("ISO_8859-7", 28597),
                new CharsetName("ISO_8859-7:1987", 28597),
                new CharsetName("ISO_8859-8", 28598),
                new CharsetName("ISO_8859-8:1988", 28598),
                new CharsetName("ISO_8859-9", 28599),
                new CharsetName("ISO_8859-9:1989", 28599),
                new CharsetName("ISO646-US", 20127),
                new CharsetName("646", 20127),
                new CharsetName("iso8859-1", 28591),
                new CharsetName("iso8859-2", 28592),
                new CharsetName("Johab", 1361),
                new CharsetName("koi", 20866),
                new CharsetName("koi8-r", 20866),
                new CharsetName("koi8-ru", 21866),
                new CharsetName("koi8-u", 21866),
                new CharsetName("koi8", 20866),
                new CharsetName("koi8r", 20866),
                new CharsetName("korean", 949),
                new CharsetName("ks-c-5601", 949),
                new CharsetName("ks-c5601", 949),
                new CharsetName("ks_c_5601-1987", 949),
                new CharsetName("ks_c_5601-1989", 949),
                new CharsetName("ks_c_5601", 949),
                new CharsetName("ks_c_5601_1987", 949),
                new CharsetName("KSC_5601", 949),
                new CharsetName("KSC5601", 949),
                new CharsetName("l1", 28591),
                new CharsetName("l2", 28592),
                new CharsetName("l3", 28593),
                new CharsetName("l4", 28594),
                new CharsetName("l5", 28599),
                new CharsetName("l9", 28605),
                new CharsetName("latin1", 28591),
                new CharsetName("latin2", 28592),
                new CharsetName("latin3", 28593),
                new CharsetName("latin4", 28594),
                new CharsetName("latin5", 28599),
                new CharsetName("latin9", 28605),
                new CharsetName("logical", 38598),
                new CharsetName("macintosh", 10000),
                new CharsetName("MacRoman", 10000),
                new CharsetName("ms_Kanji", 932),
                new CharsetName("Norwegian", 20108),
                new CharsetName("NS_4551-1", 20108),
                new CharsetName("PC-Multilingual-850+euro", 858),
                new CharsetName("SEN_850200_B", 20107),
                new CharsetName("shift-jis", 932),
                new CharsetName("shift_jis", 932),
                new CharsetName("sjis", 932),
                new CharsetName("Swedish", 20107),
                new CharsetName("TIS-620", 874),
                new CharsetName("ucs-2", 1200),
                new CharsetName("unicode-1-1-utf-7", 65000),
                new CharsetName("unicode-1-1-utf-8", 65001),
                new CharsetName("unicode-2-0-utf-7", 65000),
                new CharsetName("unicode-2-0-utf-8", 65001),
                new CharsetName("unicode", 1200),
                new CharsetName("unicodeFFFE", 1201),
                new CharsetName("us-ascii", 20127),
                new CharsetName("us", 20127),
                new CharsetName("utf-16", 1200),
                new CharsetName("UTF-16BE", 1201),
                new CharsetName("UTF-16LE", 1200),
                new CharsetName("utf-32", 12000),
                new CharsetName("UTF-32BE", 12001),
                new CharsetName("UTF-32LE", 12000),
                new CharsetName("utf-7", 65000),
                new CharsetName("utf-8", 65001),
                new CharsetName("utf7", 65000),
                new CharsetName("utf8", 65001),
                new CharsetName("visual", 28598),
                new CharsetName("windows-1250", 1250),
                new CharsetName("windows-1251", 1251),
                new CharsetName("windows-1252", 1252),
                new CharsetName("windows-1253", 1253),
                new CharsetName("Windows-1254", 1254),
                new CharsetName("windows-1255", 1255),
                new CharsetName("windows-1256", 1256),
                new CharsetName("windows-1257", 1257),
                new CharsetName("windows-1258", 1258),
                new CharsetName("windows-874", 874),
                new CharsetName("x-ansi", 1252),
                new CharsetName("x-Chinese-CNS", 20000),
                new CharsetName("x-Chinese-Eten", 20002),
                new CharsetName("x-cp1250", 1250),
                new CharsetName("x-cp1251", 1251),
                new CharsetName("x-cp20001", 20001),
                new CharsetName("x-cp20003", 20003),
                new CharsetName("x-cp20004", 20004),
                new CharsetName("x-cp20005", 20005),
                new CharsetName("x-cp20261", 20261),
                new CharsetName("x-cp20269", 20269),
                new CharsetName("x-cp20936", 20936),
                new CharsetName("x-cp20949", 20949),
                new CharsetName("x-cp21027", 21027),
                new CharsetName("x-cp50227", 50227),
                new CharsetName("x-cp50229", 50229),
                new CharsetName("X-EBCDIC-JapaneseAndUSCanada", 50931),
                new CharsetName("X-EBCDIC-KoreanExtended", 20833),
                new CharsetName("x-euc-cn", 51936),
                new CharsetName("x-euc-jp", 51932),
                new CharsetName("x-euc", 51932),
                new CharsetName("x-Europa", 29001),
                new CharsetName("x-IA5-German", 20106),
                new CharsetName("x-IA5-Norwegian", 20108),
                new CharsetName("x-IA5-Swedish", 20107),
                new CharsetName("x-IA5", 20105),
                new CharsetName("x-iscii-as", 57006),
                new CharsetName("x-iscii-be", 57003),
                new CharsetName("x-iscii-de", 57002),
                new CharsetName("x-iscii-gu", 57010),
                new CharsetName("x-iscii-ka", 57008),
                new CharsetName("x-iscii-ma", 57009),
                new CharsetName("x-iscii-or", 57007),
                new CharsetName("x-iscii-pa", 57011),
                new CharsetName("x-iscii-ta", 57004),
                new CharsetName("x-iscii-te", 57005),
                new CharsetName("x-mac-arabic", 10004),
                new CharsetName("x-mac-ce", 10029),
                new CharsetName("x-mac-chinesesimp", 10008),
                new CharsetName("x-mac-chinesetrad", 10002),
                new CharsetName("x-mac-croatian", 10082),
                new CharsetName("x-mac-cyrillic", 10007),
                new CharsetName("x-mac-greek", 10006),
                new CharsetName("x-mac-hebrew", 10005),
                new CharsetName("x-mac-icelandic", 10079),
                new CharsetName("x-mac-japanese", 10001),
                new CharsetName("x-mac-korean", 10003),
                new CharsetName("x-mac-romanian", 10010),
                new CharsetName("x-mac-thai", 10021),
                new CharsetName("x-mac-turkish", 10081),
                new CharsetName("x-mac-ukrainian", 10017),
                new CharsetName("x-ms-cp932", 932),
                new CharsetName("x-sjis", 932),
                new CharsetName("x-unicode-1-1-utf-7", 65000),
                new CharsetName("x-unicode-1-1-utf-8", 65001),
                new CharsetName("x-unicode-2-0-utf-7", 65000),
                new CharsetName("x-unicode-2-0-utf-8", 65001),
                new CharsetName("x-user-defined", 1252),
                new CharsetName("x-x-big5", 950)
            };
            CultureData[] array5 = new CultureData[]
            {
                new CultureData(1025, "ar-SA", 1256, 1256, 1256, "ar", "Arabic (Saudi Arabia)"),
                new CultureData(1026, "bg-BG", 1251, 1251, 1251, "bg", "Bulgarian (Bulgaria)"),
                new CultureData(1027, "ca-ES", 1252, 28591, 1252, "ca", "Catalan (Catalan)"),
                new CultureData(1028, "zh-TW", 950, 950, 950, "zh-CHT", "Chinese (Taiwan)"),
                new CultureData(1029, "cs-CZ", 1250, 28592, 1250, "cs", "Czech (Czech Republic)"),
                new CultureData(1030, "da-DK", 1252, 28591, 1252, "da", "Danish (Denmark)"),
                new CultureData(1031, "de-DE", 1252, 28591, 1252, "de", "German (Germany)"),
                new CultureData(1032, "el-GR", 1253, 28597, 1253, "el", "Greek (Greece)"),
                new CultureData(1033, "en-US", 1252, 28591, 1252, "en", "English (United States)"),
                new CultureData(1035, "fi-FI", 1252, 28591, 1252, "fi", "Finnish (Finland)"),
                new CultureData(1036, "fr-FR", 1252, 28591, 1252, "fr", "French (France)"),
                new CultureData(1037, "he-IL", 1255, 1255, 1255, "he", "Hebrew (Israel)"),
                new CultureData(1038, "hu-HU", 1250, 28592, 1250, "hu", "Hungarian (Hungary)"),
                new CultureData(1039, "is-IS", 1252, 28591, 1252, "is", "Icelandic (Iceland)"),
                new CultureData(1040, "it-IT", 1252, 28591, 1252, "it", "Italian (Italy)"),
                new CultureData(1041, "ja-JP", 932, 50220, 932, "ja", "Japanese (Japan)"),
                new CultureData(1042, "ko-KR", 949, 949, 949, "ko", "Korean (Korea)"),
                new CultureData(1043, "nl-NL", 1252, 28591, 1252, "nl", "Dutch (Netherlands)"),
                new CultureData(1044, "nb-NO", 1252, 28591, 1252, "no", "Norwegian - Bokm†l (Norway)"),
                new CultureData(1045, "pl-PL", 1250, 28592, 1250, "pl", "Polish (Poland)"),
                new CultureData(1046, "pt-BR", 1252, 28591, 1252, "pt", "Portuguese (Brazil)"),
                new CultureData(1048, "ro-RO", 1250, 28592, 1250, "ro", "Romanian (Romania)"),
                new CultureData(1049, "ru-RU", 1251, 20866, 1251, "ru", "Russian (Russia)"),
                new CultureData(1050, "hr-HR", 1250, 28592, 1250, "hr", "Croatian (Croatia)"),
                new CultureData(1051, "sk-SK", 1250, 28592, 1250, "sk", "Slovak (Slovakia)"),
                new CultureData(1053, "sv-SE", 1252, 28591, 1252, "sv", "Swedish (Sweden)"),
                new CultureData(1054, "th-TH", 874, 874, 874, "th", "Thai (Thailand)"),
                new CultureData(1055, "tr-TR", 1254, 28599, 1254, "tr", "Turkish (Turkey)"),
                new CultureData(1056, "ur-PK", 1256, 1256, 1256, "ur", "Urdu (Islamic Republic of Pakistan)"),
                new CultureData(1057, "id-ID", 1252, 28591, 1252, "id", "Indonesian (Indonesia)"),
                new CultureData(1058, "uk-UA", 1251, 21866, 1251, "uk", "Ukrainian (Ukraine)"),
                new CultureData(1060, "sl-SI", 1250, 28592, 1250, "sl", "Slovenian (Slovenia)"),
                new CultureData(1061, "et-EE", 1257, 28605, 28605, "et", "Estonian (Estonia)"),
                new CultureData(1062, "lv-LV", 1257, 28603, 28603, "lv", "Latvian (Latvia)"),
                new CultureData(1063, "lt-LT", 1257, 28603, 28603, "lt", "Lithuanian (Lithuania)"),
                new CultureData(1065, "fa-IR", 1256, 1256, 1256, "fa", "Persian (Iran)"),
                new CultureData(1066, "vi-VN", 1258, 1258, 1258, "vi", "Vietnamese (Vietnam)"),
                new CultureData(1069, "eu-ES", 1252, 28591, 1252, "eu", "Basque (Basque)"),
                new CultureData(1081, "hi-IN", 1200, 65001, 65001, "hi", "Hindi (India)"),
                new CultureData(1086, "ms-MY", 1252, 28591, 1252, "ms", "Malay (Malaysia)"),
                new CultureData(1087, "kk-KZ", 1251, 1251, 1251, "kk", "Kazakh (Kazakhstan)"),
                new CultureData(1110, "gl-ES", 1252, 28591, 1252, "gl", "Galician (Galician)"),
                new CultureData(1124, "fil-PH", 1252, 28591, 1252, "fil", "Filipino (Philippines)"),
                new CultureData(2052, "zh-CN", 936, 936, 936, "zh-CHS", "Chinese (People's Republic of China)"),
                new CultureData(2070, "pt-PT", 1252, 28591, 1252, "pt", "Portuguese (Portugal)"),
                new CultureData(2074, "sr-Latn-CS", 1250, 28592, 1250, "sr", "Serbian (Latin - Serbia and Montenegro)"),
                new CultureData(3076, "zh-HK", 950, 950, 950, "zh-CHT", "Chinese (Hong Kong S.A.R.)"),
                new CultureData(3082, "es-ES", 1252, 28591, 1252, "es", "Spanish (Spain)"),
                new CultureData(3098, "sr-Cyrl-CS", 1251, 1251, 1251, "sr", "Serbian (Cyrillic)"),
                new CultureData(33809, "ja-JP-Yomi", 932, 50220, 932, "ja", "Japanese (Phonetic)"),
                new CultureData(1, "ar", 1256, 1256, 1256, null, "Arabic"),
                new CultureData(2, "bg", 1251, 1251, 1251, null, "Bulgarian"),
                new CultureData(3, "ca", 1252, 28591, 1252, null, "Catalan"),
                new CultureData(4, "zh-CHS", 936, 936, 936, null, "Chinese (Simplified)"),
                new CultureData(5, "cs", 1250, 28592, 1250, null, "Czech"),
                new CultureData(6, "da", 1252, 28591, 1252, null, "Danish"),
                new CultureData(7, "de", 1252, 28591, 1252, null, "German"),
                new CultureData(8, "el", 1253, 28597, 1253, null, "Greek"),
                new CultureData(9, "en", 1252, 28591, 1252, null, "English"),
                new CultureData(10, "es", 1252, 28591, 1252, null, "Spanish"),
                new CultureData(11, "fi", 1252, 28591, 1252, null, "Finnish"),
                new CultureData(12, "fr", 1252, 28591, 1252, null, "French"),
                new CultureData(13, "he", 1255, 1255, 1255, null, "Hebrew"),
                new CultureData(14, "hu", 1250, 28592, 1250, null, "Hungarian"),
                new CultureData(15, "is", 1252, 28591, 1252, null, "Icelandic"),
                new CultureData(16, "it", 1252, 28591, 1252, null, "Italian"),
                new CultureData(17, "ja", 932, 50220, 932, null, "Japanese"),
                new CultureData(18, "ko", 949, 949, 949, null, "Korean"),
                new CultureData(19, "nl", 1252, 28591, 1252, null, "Dutch"),
                new CultureData(20, "no", 1252, 28591, 1252, null, "Norwegian"),
                new CultureData(21, "pl", 1250, 28592, 1250, null, "Polish"),
                new CultureData(22, "pt", 1252, 28591, 1252, null, "Portuguese"),
                new CultureData(24, "ro", 1250, 28592, 1250, null, "Romanian"),
                new CultureData(25, "ru", 1251, 20866, 1251, null, "Russian"),
                new CultureData(26, "hr", 1250, 28592, 1250, null, "Croatian"),
                new CultureData(27, "sk", 1250, 28592, 1250, null, "Slovak"),
                new CultureData(29, "sv", 1252, 28591, 1252, null, "Swedish"),
                new CultureData(30, "th", 874, 874, 874, null, "Thai"),
                new CultureData(31, "tr", 1254, 28599, 1254, null, "Turkish"),
                new CultureData(32, "ur", 1256, 1256, 1256, null, "Urdu"),
                new CultureData(33, "id", 1252, 28591, 1252, null, "Indonesian"),
                new CultureData(34, "uk", 1251, 21866, 1251, null, "Ukrainian"),
                new CultureData(36, "sl", 1250, 28592, 1250, null, "Slovenian"),
                new CultureData(37, "et", 1257, 28605, 28605, null, "Estonian"),
                new CultureData(38, "lv", 1257, 28603, 28603, null, "Latvian"),
                new CultureData(39, "lt", 1257, 28603, 28603, null, "Lithuanian"),
                new CultureData(41, "fa", 1256, 1256, 1256, null, "Persian"),
                new CultureData(42, "vi", 1258, 1258, 1258, null, "Vietnamese"),
                new CultureData(45, "eu", 1252, 28591, 1252, null, "Basque"),
                new CultureData(57, "hi", 1200, 65001, 65001, null, "Hindi"),
                new CultureData(62, "ms", 1252, 28591, 1252, null, "Malay"),
                new CultureData(63, "kk", 1251, 1251, 1251, null, "Kazakh"),
                new CultureData(86, "gl", 1252, 28591, 1252, null, "Galician"),
                new CultureData(100, "fil", 1252, 28591, 1252, null, "Filipino"),
                new CultureData(31748, "zh-CHT", 950, 950, 950, null, "Chinese (Traditional)"),
                new CultureData(31770, "sr", 1251, 1251, 1251, null, "Serbian")
            };
            GlobalizationData globalizationData = new GlobalizationData();
            Culture culture = null;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            CultureInfo[] array6 = cultures;
            for (int i = 0; i < array6.Length; i++)
            {
                CultureInfo cultureInfo = array6[i];
                Culture culture2;
                if (!globalizationData.LocaleIdToCulture.TryGetValue(cultureInfo.LCID, out culture2))
                {
                    culture2 = new Culture(cultureInfo.LCID, cultureInfo.Name);
                    globalizationData.LocaleIdToCulture.Add(cultureInfo.LCID, culture2);
                    culture2.Description = cultureInfo.EnglishName;
                    culture2.NativeDescription = cultureInfo.NativeName;
                    culture2.CultureInfo = cultureInfo;
                    if (cultureInfo.LCID == 127)
                    {
                        culture = culture2;
                    }
                }
                if (!globalizationData.NameToCulture.ContainsKey(cultureInfo.Name))
                {
                    globalizationData.NameToCulture.Add(cultureInfo.Name, culture2);
                }
            }
            CultureData[] array7 = array5;
            for (int i = 0; i < array7.Length; i++)
            {
                CultureData cultureData = array7[i];
                Culture culture2;
                if (!globalizationData.LocaleIdToCulture.TryGetValue(cultureData.LocaleId, out culture2))
                {
                    culture2 = new Culture(cultureData.LocaleId, cultureData.Name);
                    globalizationData.LocaleIdToCulture.Add(cultureData.LocaleId, culture2);
                    culture2.Description = cultureData.Description;
                    culture2.NativeDescription = cultureData.Description;
                }
                if (!globalizationData.NameToCulture.ContainsKey(cultureData.Name))
                {
                    globalizationData.NameToCulture.Add(cultureData.Name, culture2);
                }
            }
            array6 = cultures;
            for (int i = 0; i < array6.Length; i++)
            {
                CultureInfo cultureInfo2 = array6[i];
                Culture culture2 = globalizationData.LocaleIdToCulture[cultureInfo2.LCID];
                if (cultureInfo2.Parent != null)
                {
                    Culture parentCulture;
                    if (globalizationData.LocaleIdToCulture.TryGetValue(cultureInfo2.Parent.LCID, out parentCulture))
                    {
                        culture2.ParentCulture = parentCulture;
                    }
                    else
                    {
                        culture2.ParentCulture = culture2;
                    }
                }
                else
                {
                    culture2.ParentCulture = culture2;
                }
            }
            array7 = array5;
            for (int i = 0; i < array7.Length; i++)
            {
                CultureData cultureData2 = array7[i];
                Culture culture2 = globalizationData.LocaleIdToCulture[cultureData2.LocaleId];
                if (culture2.ParentCulture == null)
                {
                    if (cultureData2.ParentCultureName != null)
                    {
                        Culture parentCulture2;
                        if (globalizationData.NameToCulture.TryGetValue(cultureData2.ParentCultureName, out parentCulture2))
                        {
                            culture2.ParentCulture = parentCulture2;
                        }
                        else
                        {
                            culture2.ParentCulture = culture2;
                        }
                    }
                    else
                    {
                        culture2.ParentCulture = culture2;
                    }
                }
            }
            EncodingInfo[] encodings = Encoding.GetEncodings();
            EncodingInfo[] array8 = encodings;
            for (int i = 0; i < array8.Length; i++)
            {
                EncodingInfo encodingInfo = array8[i];
                Charset charset;
                if (!globalizationData.CodePageToCharset.TryGetValue(encodingInfo.CodePage, out charset))
                {
                    charset = new Charset(encodingInfo.CodePage, encodingInfo.Name)
                    {
                        Description = encodingInfo.DisplayName
                    };
                    globalizationData.CodePageToCharset.Add(encodingInfo.CodePage, charset);
                }
                if (!globalizationData.NameToCharset.ContainsKey(encodingInfo.Name))
                {
                    globalizationData.NameToCharset.Add(encodingInfo.Name, charset);
                    if (encodingInfo.Name.Length > globalizationData.MaxCharsetNameLength)
                    {
                        globalizationData.MaxCharsetNameLength = encodingInfo.Name.Length;
                    }
                }
            }
            InternalWindowsCodePage[] array9 = array;
            for (int i = 0; i < array9.Length; i++)
            {
                InternalWindowsCodePage internalWindowsCodePage = array9[i];
                Culture culture2;
                if (internalWindowsCodePage.LocaleId == 0)
                {
                    if (internalWindowsCodePage.CodePage == 1200 && culture != null)
                    {
                        culture2 = culture;
                    }
                    else
                    {
                        culture2 = new Culture(0, null)
                        {
                            ParentCulture = culture
                        };
                    }
                    culture2.Description = internalWindowsCodePage.GenericCultureDescription;
                }
                else if (!globalizationData.LocaleIdToCulture.TryGetValue(internalWindowsCodePage.LocaleId, out culture2) && !globalizationData.NameToCulture.TryGetValue(internalWindowsCodePage.CultureName, out culture2))
                {
                    culture2 = new Culture(internalWindowsCodePage.LocaleId, internalWindowsCodePage.CultureName);
                    globalizationData.LocaleIdToCulture.Add(internalWindowsCodePage.LocaleId, culture2);
                    globalizationData.NameToCulture.Add(internalWindowsCodePage.CultureName, culture2);
                }
                Charset charset;
                if (!globalizationData.CodePageToCharset.TryGetValue(internalWindowsCodePage.CodePage, out charset))
                {
                    charset = new Charset(internalWindowsCodePage.CodePage, internalWindowsCodePage.Name);
                    globalizationData.NameToCharset.Add(charset.Name, charset);
                    globalizationData.CodePageToCharset.Add(charset.CodePage, charset);
                    if (charset.Name.Length > globalizationData.MaxCharsetNameLength)
                    {
                        globalizationData.MaxCharsetNameLength = charset.Name.Length;
                    }
                }
                charset.IsWindowsCharset = true;
                culture2.WindowsCharset = charset;
                charset.Culture = culture2;
                if (!globalizationData.CodePageToCharset.TryGetValue(internalWindowsCodePage.MimeCodePage, out charset))
                {
                    charset = globalizationData.CodePageToCharset[internalWindowsCodePage.CodePage];
                }
                culture2.MimeCharset = charset;
                if (!globalizationData.CodePageToCharset.TryGetValue(internalWindowsCodePage.WebCodePage, out charset))
                {
                    charset = globalizationData.CodePageToCharset[internalWindowsCodePage.CodePage];
                }
                culture2.WebCharset = charset;
            }
            CharsetName[] array10 = array4;
            for (int i = 0; i < array10.Length; i++)
            {
                CharsetName charsetName = array10[i];
                Charset charset;
                if (!globalizationData.NameToCharset.TryGetValue(charsetName.Name, out charset))
                {
                    if (globalizationData.CodePageToCharset.TryGetValue(charsetName.CodePage, out charset))
                    {
                        globalizationData.NameToCharset.Add(charsetName.Name, charset);
                        if (charsetName.Name.Length > globalizationData.MaxCharsetNameLength)
                        {
                            globalizationData.MaxCharsetNameLength = charsetName.Name.Length;
                        }
                    }
                }
                else if (charset.CodePage != charsetName.CodePage && globalizationData.CodePageToCharset.TryGetValue(charsetName.CodePage, out charset))
                {
                    globalizationData.NameToCharset[charsetName.Name] = charset;
                }
            }
            for (int j = 0; j < CodePageMapData.CodePages.Length; j++)
            {
                Charset charset;
                if (globalizationData.CodePageToCharset.TryGetValue(CodePageMapData.CodePages[j].Id, out charset))
                {
                    charset.MapIndex = j;
                }
                if (charset.Culture == null)
                {
                    Charset charset2 = globalizationData.CodePageToCharset[CodePageMapData.CodePages[j].WindowsId];
                    charset.Culture = charset2.Culture;
                }
            }
            CultureCodePageOverride[] array11 = array3;
            for (int i = 0; i < array11.Length; i++)
            {
                CultureCodePageOverride cultureCodePageOverride = array11[i];
                Culture culture2;
                if (globalizationData.NameToCulture.TryGetValue(cultureCodePageOverride.CultureName, out culture2))
                {
                    Charset charset;
                    if (globalizationData.CodePageToCharset.TryGetValue(cultureCodePageOverride.MimeCodePage, out charset))
                    {
                        culture2.MimeCharset = charset;
                    }
                    if (globalizationData.CodePageToCharset.TryGetValue(cultureCodePageOverride.WebCodePage, out charset))
                    {
                        culture2.MimeCharset = charset;
                    }
                }
            }
            array6 = cultures;
            for (int i = 0; i < array6.Length; i++)
            {
                CultureInfo cultureInfo3 = array6[i];
                Culture culture2 = globalizationData.LocaleIdToCulture[cultureInfo3.LCID];
                if (culture2.WindowsCharset == null)
                {
                    int num = cultureInfo3.TextInfo.ANSICodePage;
                    if (num <= 500)
                    {
                        num = 1200;
                    }
                    else
                    {
                        int page = num;
                        if (!array.Any((InternalWindowsCodePage wcp) => page == wcp.CodePage))
                        {
                            num = 1200;
                        }
                    }
                    Charset charset = globalizationData.CodePageToCharset[num];
                    culture2.WindowsCharset = charset;
                    if (culture2 != culture2.ParentCulture && culture2.WindowsCharset == culture2.ParentCulture.WindowsCharset)
                    {
                        culture2.MimeCharset = culture2.ParentCulture.MimeCharset;
                        culture2.WebCharset = culture2.ParentCulture.WebCharset;
                    }
                    if (culture2.MimeCharset == null)
                    {
                        culture2.MimeCharset = charset.Culture.MimeCharset;
                    }
                    if (culture2.WebCharset == null)
                    {
                        culture2.WebCharset = charset.Culture.WebCharset;
                    }
                }
            }
            array7 = array5;
            for (int i = 0; i < array7.Length; i++)
            {
                CultureData cultureData3 = array7[i];
                Culture culture2 = globalizationData.LocaleIdToCulture[cultureData3.LocaleId];
                if (culture2.WindowsCharset == null)
                {
                    int windowsCodePage2 = cultureData3.WindowsCodePage;
                    Charset charset = globalizationData.CodePageToCharset[windowsCodePage2];
                    culture2.WindowsCharset = charset;
                    Charset charset3;
                    if (globalizationData.CodePageToCharset.TryGetValue(cultureData3.MimeCodePage, out charset3))
                    {
                        culture2.MimeCharset = charset3;
                    }
                    if (globalizationData.CodePageToCharset.TryGetValue(cultureData3.WebCodePage, out charset3))
                    {
                        culture2.WebCharset = charset3;
                    }
                    if (culture2 != culture2.ParentCulture && culture2.WindowsCharset == culture2.ParentCulture.WindowsCharset)
                    {
                        if (culture2.MimeCharset == null)
                        {
                            culture2.MimeCharset = culture2.ParentCulture.MimeCharset;
                        }
                        if (culture2.WebCharset == null)
                        {
                            culture2.WebCharset = culture2.ParentCulture.WebCharset;
                        }
                    }
                    if (culture2.MimeCharset == null)
                    {
                        culture2.MimeCharset = charset.Culture.MimeCharset;
                    }
                    if (culture2.WebCharset == null)
                    {
                        culture2.WebCharset = charset.Culture.WebCharset;
                    }
                }
            }
            array8 = encodings;
            for (int i = 0; i < array8.Length; i++)
            {
                EncodingInfo encodingInfo2 = array8[i];
                Charset charset = globalizationData.CodePageToCharset[encodingInfo2.CodePage];
                if (charset.Culture == null)
                {
                    int windowsCodePage = 1200;
                    Encoding encoding;
                    if (charset.TryGetEncoding(out encoding))
                    {
                        windowsCodePage = encoding.WindowsCodePage;
                        if (!array.Any((InternalWindowsCodePage wcp) => windowsCodePage == wcp.CodePage))
                        {
                            windowsCodePage = 1200;
                        }
                    }
                    Charset charset4 = globalizationData.CodePageToCharset[windowsCodePage];
                    charset.Culture = charset4.Culture;
                }
            }
            CodePageCultureOverride[] array12 = array2;
            for (int i = 0; i < array12.Length; i++)
            {
                CodePageCultureOverride codePageCultureOverride = array12[i];
                Culture culture2;
                Charset charset;
                if (globalizationData.CodePageToCharset.TryGetValue(codePageCultureOverride.CodePage, out charset) && globalizationData.NameToCulture.TryGetValue(codePageCultureOverride.CultureName, out culture2))
                {
                    charset.Culture = culture2;
                }
            }
            Culture defaultCulture;
            if (!globalizationData.LocaleIdToCulture.TryGetValue(CultureInfo.CurrentUICulture.LCID, out defaultCulture))
            {
                globalizationData.DefaultCulture = defaultCulture;
                if (!globalizationData.LocaleIdToCulture.TryGetValue(CultureInfo.CurrentCulture.LCID, out defaultCulture))
                {
                    globalizationData.DefaultCulture = globalizationData.LocaleIdToCulture[1033];
                }
            }
            IList<CtsConfigurationSetting> configuration = ApplicationServices.Provider.GetConfiguration("Globalization");
            string text = null;
            string text2 = null;
            string text3 = null;
            string text4 = null;
            foreach (CtsConfigurationSetting current in configuration)
            {
                string name = current.Name;
                IList<CtsConfigurationArgument> arguments = current.Arguments;
                string key;
                switch (key = name.ToLower())
                {
                    case "overridecharsetdefaultname":
                        {
                            if (arguments.Count != 2)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            string text5;
                            string text6;
                            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
                            {
                                text5 = arguments[0].Value.Trim();
                                text6 = arguments[1].Value.Trim();
                            }
                            else
                            {
                                if (!arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) || !arguments[0].Name.Equals("DefaultName", StringComparison.OrdinalIgnoreCase))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                text5 = arguments[1].Value.Trim();
                                text6 = arguments[0].Value.Trim();
                            }
                            Charset charset;
                            int key2;
                            if (int.TryParse(text5, out key2))
                            {
                                if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCharset.TryGetValue(text5, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            Charset charset5;
                            if (!globalizationData.NameToCharset.TryGetValue(text6, out charset5))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            if (charset != charset5)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            charset.Name = text6;
                            continue;
                        }
                    case "addcharsetaliasname":
                        {
                            if (arguments.Count != 2)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            string text5;
                            string text7;
                            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                            {
                                text5 = arguments[0].Value.Trim();
                                text7 = arguments[1].Value.Trim();
                            }
                            else
                            {
                                if (!arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) || !arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                text5 = arguments[1].Value.Trim();
                                text7 = arguments[0].Value.Trim();
                            }
                            Charset charset;
                            int key2;
                            if (int.TryParse(text5, out key2))
                            {
                                if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCharset.TryGetValue(text5, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            Charset charset5;
                            if (globalizationData.NameToCharset.TryGetValue(text7, out charset5))
                            {
                                if (charset != charset5)
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                continue;
                            }
                            else
                            {
                                globalizationData.NameToCharset.Add(text7, charset);
                                if (text7.Length > globalizationData.MaxCharsetNameLength)
                                {
                                    globalizationData.MaxCharsetNameLength = text7.Length;
                                    continue;
                                }
                                continue;
                            }
                            //break;
                        }
                    case "overridecharsetculture":
                        {
                            if (arguments.Count != 2)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            string text5;
                            if (arguments[0].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                            {
                                text5 = arguments[0].Value.Trim();
                                text = arguments[1].Value.Trim();
                            }
                            else
                            {
                                if (!arguments[1].Name.Equals("CharSet", StringComparison.OrdinalIgnoreCase) || !arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                text5 = arguments[1].Value.Trim();
                                text = arguments[0].Value.Trim();
                            }
                            Charset charset;
                            int key2;
                            if (int.TryParse(text5, out key2))
                            {
                                if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCharset.TryGetValue(text5, out charset))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            Culture culture2;
                            int key3;
                            if (int.TryParse(text, out key3))
                            {
                                if (!globalizationData.LocaleIdToCulture.TryGetValue(key3, out culture2))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCulture.TryGetValue(text, out culture2))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            charset.Culture = culture2;
                            continue;
                        }
                    case "addculturealiasname":
                        {
                            if (arguments.Count != 2)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            string text7;
                            if (arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) && arguments[1].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                            {
                                text = arguments[0].Value.Trim();
                                text7 = arguments[1].Value.Trim();
                            }
                            else
                            {
                                if (!arguments[1].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase) || !arguments[0].Name.Equals("AliasName", StringComparison.OrdinalIgnoreCase))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                text = arguments[1].Value.Trim();
                                text7 = arguments[0].Value.Trim();
                            }
                            Culture culture2;
                            int key3;
                            if (int.TryParse(text, out key3))
                            {
                                if (!globalizationData.LocaleIdToCulture.TryGetValue(key3, out culture2))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCulture.TryGetValue(text, out culture2))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            Culture culture3;
                            if (!globalizationData.NameToCulture.TryGetValue(text7, out culture3))
                            {
                                globalizationData.NameToCulture.Add(text7, culture2);
                                continue;
                            }
                            if (culture2 != culture3)
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            continue;
                        }
                    case "overrideculturecharset":
                        {
                            text = null;
                            text2 = null;
                            text3 = null;
                            text4 = null;
                            foreach (CtsConfigurationArgument current2 in arguments)
                            {
                                if (current2.Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                                {
                                    text = current2.Value.Trim();
                                }
                                else if (current2.Name.Equals("WindowsCharset", StringComparison.OrdinalIgnoreCase))
                                {
                                    text2 = current2.Value.Trim();
                                }
                                else if (current2.Name.Equals("MimeCharset", StringComparison.OrdinalIgnoreCase))
                                {
                                    text3 = current2.Value.Trim();
                                }
                                else
                                {
                                    if (!current2.Name.Equals("WebCharset", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                                        break;
                                    }
                                    text4 = current2.Value.Trim();
                                }
                            }
                            if (string.IsNullOrEmpty(text) || (string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3) && string.IsNullOrEmpty(text4)))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            Culture culture2;
                            int key3;
                            if (int.TryParse(text, out key3))
                            {
                                if (!globalizationData.LocaleIdToCulture.TryGetValue(key3, out culture2))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCulture.TryGetValue(text, out culture2))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            if (!string.IsNullOrEmpty(text2))
                            {
                                Charset charset;
                                int key2;
                                if (int.TryParse(text2, out key2))
                                {
                                    if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                    {
                                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                                        continue;
                                    }
                                }
                                else if (!globalizationData.NameToCharset.TryGetValue(text2, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                if (!charset.IsWindowsCharset)
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                culture2.WindowsCharset = charset;
                            }
                            if (!string.IsNullOrEmpty(text3))
                            {
                                Charset charset;
                                int key2;
                                if (int.TryParse(text3, out key2))
                                {
                                    if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                    {
                                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                                        continue;
                                    }
                                }
                                else if (!globalizationData.NameToCharset.TryGetValue(text3, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                culture2.MimeCharset = charset;
                            }
                            if (!string.IsNullOrEmpty(text4))
                            {
                                Charset charset;
                                int key2;
                                if (int.TryParse(text4, out key2))
                                {
                                    if (!globalizationData.CodePageToCharset.TryGetValue(key2, out charset))
                                    {
                                        ApplicationServices.Provider.LogConfigurationErrorEvent();
                                        continue;
                                    }
                                }
                                else if (!globalizationData.NameToCharset.TryGetValue(text4, out charset))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                                culture2.WebCharset = charset;
                                continue;
                            }
                            continue;
                        }
                    case "defaultculture":
                        {
                            if (arguments.Count != 1 || !arguments[0].Name.Equals("Culture", StringComparison.OrdinalIgnoreCase))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            text = arguments[0].Value.Trim();
                            Culture culture2;
                            int key3;
                            if (int.TryParse(text, out key3))
                            {
                                if (!globalizationData.LocaleIdToCulture.TryGetValue(key3, out culture2))
                                {
                                    ApplicationServices.Provider.LogConfigurationErrorEvent();
                                    continue;
                                }
                            }
                            else if (!globalizationData.NameToCulture.TryGetValue(text, out culture2))
                            {
                                ApplicationServices.Provider.LogConfigurationErrorEvent();
                                continue;
                            }
                            globalizationData.DefaultCulture = culture2;
                            continue;
                        }
                }
                ApplicationServices.Provider.LogConfigurationErrorEvent();
            }
            if (defaultCultureName != null)
            {
                globalizationData.DefaultCulture = globalizationData.NameToCulture[defaultCultureName];
            }
            globalizationData.DefaultDetectionPriorityOrder = GetCultureSpecificCodepageDetectionPriorityOrder(globalizationData.DefaultCulture, null);
            culture.SetCodepageDetectionPriorityOrder(globalizationData.DefaultDetectionPriorityOrder);
            globalizationData.DefaultCulture.GetCodepageDetectionPriorityOrder(globalizationData);
            globalizationData.AsciiCharset = globalizationData.CodePageToCharset[20127];
            globalizationData.Utf8Charset = globalizationData.CodePageToCharset[65001];
            globalizationData.UnicodeCharset = globalizationData.CodePageToCharset[1200];
            return globalizationData;
        }

        /// <summary>
        /// Returns a value indicating if the code page is a double byte code page
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <returns>
        /// True if the code page is a double byte code page otherwise false.
        /// </returns>
        private static bool IsDbcs(int codePage)
        {
            if (codePage <= 50220)
            {
                if (codePage <= 936)
                {
                    if (codePage != 932 && codePage != 936)
                    {
                        return false;
                    }
                }
                else
                {
                    switch (codePage)
                    {
                        case 949:
                        case 950:
                            break;
                        default:
                            if (codePage != 50220)
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            else if (codePage <= 51932)
            {
                if (codePage != 50225 && codePage != 51932)
                {
                    return false;
                }
            }
            else if (codePage != 51949 && codePage != 52936)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a boolean indicating if the specified code page is in the specified list.
        /// </summary>
        /// <param name="codePage">
        /// The code page to detect.
        /// </param>
        /// <param name="list">
        /// The code page list.
        /// </param>
        /// <param name="listCount">
        /// The size of the list.
        /// </param>
        /// <returns>
        /// True if the code page is in the list, otherwise false.
        /// </returns>
        private static bool InList(int codePage, int[] list, int listCount)
        {
            for (int i = 0; i < listCount; i++)
            {
                if (list[i] == codePage)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Dectects if the specified code page and the windows code page are for the same language.
        /// </summary>
        /// <param name="codePage">
        /// The code page.
        /// </param>
        /// <param name="windowsCodePage">
        /// The windows code page.
        /// </param>
        /// <returns>
        /// True if the code page and the windows code page are equivilant.
        /// </returns>
        private static bool IsSameLanguage(int codePage, int windowsCodePage)
        {
            return windowsCodePage == codePage || (windowsCodePage == 1250 && codePage == 28592) || (windowsCodePage == 1251 && (codePage == 28595 || codePage == 20866 || codePage == 21866)) || (windowsCodePage == 1252 && (codePage == 28591 || codePage == 28605)) || (windowsCodePage == 1253 && codePage == 28597) || (windowsCodePage == 1254 && codePage == 28599) || (windowsCodePage == 1255 && codePage == 38598) || (windowsCodePage == 1256 && codePage == 28596) || (windowsCodePage == 1257 && codePage == 28594) || (windowsCodePage == 932 && (codePage == 50220 || codePage == 51932)) || (windowsCodePage == 949 && (codePage == 50225 || codePage == 51949)) || (windowsCodePage == 936 & codePage == 52936);
        }
    }
}
