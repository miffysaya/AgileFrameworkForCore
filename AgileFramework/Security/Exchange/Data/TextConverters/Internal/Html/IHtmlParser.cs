using AgileFramework.Security.Application.TextConverters.HTML;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal interface IHtmlParser
    {
        HtmlToken Token
        {
            get;
        }

        HtmlTokenId Parse();

        void SetRestartConsumer(IRestartable restartConsumer);
    }
}
