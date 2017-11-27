using AgileFramework.Security.Exchange.Data.TextConverters;
using System.IO;

namespace AgileFramework.Security.Application
{
    /// <summary>
    ///  Sanitizes input HTML to make it safe to be displayed on a 
    ///  browser by removing potentially dangerous tags.
    /// </summary>
    /// <remarks>
    ///  This santization library uses the Principle of Inclusions, 
    ///  sometimes referred to as "safe-listing" to provide protection 
    ///  against injection attacks.  With safe-listing protection, 
    ///  algorithms look for valid inputs and automatically treat 
    ///  everything outside that set as a potential attack.  This library 
    ///  can be used as a defense in depth approach with other mitigation 
    ///  techniques.
    /// </remarks>
    public static class Sanitizer
    {
        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="input">Malicious HTML Document</param>
        /// <returns>A santizied HTML document</returns>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static string GetSafeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            string result;
            using (TextReader textReader = new StringReader(input))
            {
                using (TextWriter textWriter = new StringWriter())
                {
                    HtmlToHtml htmlToHtml = new HtmlToHtml
                    {
                        FilterHtml = true,
                        OutputHtmlFragment = false,
                        NormalizeHtml = true
                    };
                    htmlToHtml.Convert(textReader, textWriter);
                    result = ((textWriter.ToString().Length != 0) ? textWriter.ToString() : string.Empty);
                }
            }
            return result;
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="input">Malicious HTML fragment</param>
        /// <returns>Safe HTML fragment</returns>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static string GetSafeHtmlFragment(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            string result;
            using (TextReader textReader = new StringReader(input))
            {
                using (TextWriter textWriter = new StringWriter())
                {
                    HtmlToHtml htmlToHtml = new HtmlToHtml
                    {
                        FilterHtml = true,
                        OutputHtmlFragment = true,
                        NormalizeHtml = true
                    };
                    htmlToHtml.Convert(textReader, textWriter);
                    if (textWriter.ToString().Length == 0)
                    {
                        result = string.Empty;
                    }
                    else
                    {
                        string text = textWriter.ToString();
                        if (text.Substring(0, 5).ToLower() == "<div>")
                        {
                            text = text.Substring(5);
                            text = text.Substring(0, text.Length - 8);
                        }
                        result = text;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationWriter">Text Writer to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtml(TextReader sourceReader, TextWriter destinationWriter)
        {
            HtmlToHtml htmlToHtml = new HtmlToHtml
            {
                FilterHtml = true,
                OutputHtmlFragment = false,
                NormalizeHtml = true
            };
            htmlToHtml.Convert(sourceReader, destinationWriter);
        }

        /// <summary>
        /// Sanitizes input HTML document for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationStream">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtml(TextReader sourceReader, Stream destinationStream)
        {
            HtmlToHtml htmlToHtml = new HtmlToHtml
            {
                FilterHtml = true,
                OutputHtmlFragment = false,
                NormalizeHtml = true
            };
            htmlToHtml.Convert(sourceReader, destinationStream);
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationWriter">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtmlFragment(TextReader sourceReader, TextWriter destinationWriter)
        {
            HtmlToHtml htmlToHtml = new HtmlToHtml
            {
                FilterHtml = true,
                OutputHtmlFragment = true,
                NormalizeHtml = true
            };
            htmlToHtml.Convert(sourceReader, destinationWriter);
        }

        /// <summary>
        /// Sanitizes input HTML fragment for safe display on browser.
        /// </summary>
        /// <param name="sourceReader">Source text reader with malicious HTML</param>
        /// <param name="destinationStream">Stream to write safe HTML</param>
        /// <remarks>
        /// The method transforms and filters HTML of executable scripts. 
        /// A safe list of tags and attributes are used to strip dangerous 
        /// scripts from the HTML. HTML is also normalized where tags are 
        /// properly closed and attributes are properly formatted.
        /// </remarks>
        public static void GetSafeHtmlFragment(TextReader sourceReader, Stream destinationStream)
        {
            HtmlToHtml htmlToHtml = new HtmlToHtml
            {
                FilterHtml = true,
                OutputHtmlFragment = true,
                NormalizeHtml = true
            };
            htmlToHtml.Convert(sourceReader, destinationStream);
        }
    }
}