using System;
using System.Globalization;

namespace AgileFramework.Security.Exchange.Data.Globalization
{
    /// <summary>
    /// Represents a culture 
    /// </summary>
    [Serializable]
    internal class Culture
    {
        /// <summary>
        /// The locale id for this culture.
        /// </summary>
        private readonly int lcid;

        /// <summary>
        /// The culture name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The codepage detection order, by priority.
        /// </summary>
        private int[] codepageDetectionPriorityOrder;

        /// <summary>
        /// The culture info.
        /// </summary>
        private CultureInfo cultureInfo;

        /// <summary>
        /// Gets or sets the Windows character set for the culture.
        /// </summary>
        public Charset WindowsCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME character set for the culture.
        /// </summary>
        public Charset MimeCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web character set for the culture.
        /// </summary>
        public Charset WebCharset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description for the culture.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the native description.
        /// </summary>
        public string NativeDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent culture.
        /// </summary>
        public Culture ParentCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the culture information for this culture.
        /// </summary>
        /// <returns>
        /// The <see cref="P:Microsoft.Exchange.Data.Globalization.Culture.CultureInfo" /> for this culture.
        /// </returns>
        public CultureInfo CultureInfo
        {
            set
            {
                cultureInfo = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Exchange.Data.Globalization.Culture" /> class.
        /// </summary>
        /// <param name="lcid">
        /// The locale identifier.
        /// </param>
        /// <param name="name">
        /// The name of the culture.
        /// </param>
        internal Culture(int lcid, string name)
        {
            this.lcid = lcid;
            this.name = name;
        }

        /// <summary>
        /// Gets code page detection priority order for the specified globalization data.
        /// </summary>
        /// <param name="data">
        /// The globalization data.
        /// </param>
        /// <returns>
        /// The code page detection priority.
        /// </returns>
        internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data)
        {
            int[] arg_3E_0;
            if ((arg_3E_0 = codepageDetectionPriorityOrder) == null)
            {
                arg_3E_0 = (codepageDetectionPriorityOrder = CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(this, (ParentCulture == null || ParentCulture == this) ? data.DefaultDetectionPriorityOrder : ParentCulture.GetCodepageDetectionPriorityOrder(data)));
            }
            return arg_3E_0;
        }

        /// <summary>
        /// Sets the code page detection prioity order.
        /// </summary>
        /// <param name="newCodepageDetectionPriorityOrder">
        /// The new code page detection priority order.
        /// </param>
        internal void SetCodepageDetectionPriorityOrder(int[] newCodepageDetectionPriorityOrder)
        {
            codepageDetectionPriorityOrder = newCodepageDetectionPriorityOrder;
        }
    }
}
