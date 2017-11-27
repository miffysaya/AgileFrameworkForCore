namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Text
{
    /// <summary>
    /// Image rendering callbak delegate.
    /// </summary>
    /// <param name="attachmentUrl">The attachement URL.</param>
    /// <param name="approximateRenderingPosition">The approximate rendering position.</param>
    /// <returns>
    /// <c>true</c> when image rendering callback is successful; otherwise <c>false</c>.
    /// </returns>
    internal delegate bool ImageRenderingCallbackInternal(string attachmentUrl, int approximateRenderingPosition);
}
