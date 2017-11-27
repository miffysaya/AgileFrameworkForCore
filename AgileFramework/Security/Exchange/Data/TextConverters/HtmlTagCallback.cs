namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// Delegate callback definition for the HTML tag.
    /// </summary>
    /// <param name="tagContext">An instance fo the HtmlTagContext object.</param>
    /// <param name="htmlWriter">An instance fo the HtmlWriter object.</param>
    internal delegate void HtmlTagCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter);
}
