﻿using System.Resources;

namespace AgileFramework.Security.Exchange.CtsResources
{
    /// <summary>
    /// Strings used for globalization.
    /// </summary>
    internal static class GlobalizationStrings
    {
        /// <summary>
        /// Resource identifiers
        /// </summary>
        public enum ResourceIdentifier
        {
            /// <summary>
            /// The maximum number of characters cannot be negative.
            /// </summary>
            MaxCharactersCannotBeNegative,
            /// <summary>
            /// The code page priority list includes a code page which cannot be detected.
            /// </summary>
            PriorityListIncludesNonDetectableCodePage,
            /// <summary>
            /// Index out of range.
            /// </summary>
            IndexOutOfRange,
            /// <summary>
            /// The count is too large.
            /// </summary>
            CountTooLarge,
            /// <summary>
            /// The offset is out of range.
            /// </summary>
            OffsetOutOfRange,
            /// <summary>
            /// The count is out of range.
            /// </summary>
            CountOutOfRange
        }

        /// <summary>
        /// Parameter identifiers
        /// </summary>
        public enum ParameterIdentifier
        {
            /// <summary>
            /// Invalid character set.
            /// </summary>
            InvalidCharset,
            /// <summary>
            /// Invalid locale identifier.
            /// </summary>
            InvalidLocaleId,
            /// <summary>
            /// The code page is not installed.
            /// </summary>
            NotInstalledCodePage,
            /// <summary>
            /// The character set is not installed.
            /// </summary>
            NotInstalledCharset,
            /// <summary>
            /// The code page is invalid.
            /// </summary>
            InvalidCodePage,
            /// <summary>
            /// The code page and the character set are not installed.
            /// </summary>
            NotInstalledCharsetCodePage,
            /// <summary>
            /// The culture name is invalid.
            /// </summary>
            InvalidCultureName
        }

        /// <summary>
        /// The resource manager for the globalization strings resources.
        /// </summary>
        private static readonly ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.GlobalizationStrings", typeof(GlobalizationStrings).Assembly);

        /// <summary>
        /// Gets the string for the Invalid Code Page error.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <returns>The Invalid Code Page error string.</returns>
        internal static string InvalidCodePage(int codePage)
        {
            return string.Format(ResourceManager.GetString("InvalidCodePage"), codePage);
        }
    }
}
