namespace AgileFramework.Security.Application
{
    /// <summary>
    /// The type of space encoding to use.
    /// </summary>
    internal enum EncodingType
    {
        /// <summary>
        /// Encode spaces for use in query strings
        /// </summary>
        QueryString = 1,
        /// <summary>
        /// Encode spaces for use in form data
        /// </summary>
        HtmlForm
    }
}
