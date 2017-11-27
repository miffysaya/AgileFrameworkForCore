namespace AgileFramework.Security.Application.TextConverters.HTML
{
    internal enum HtmlTokenId : byte
    {
        None,
        EndOfFile,
        Text,
        EncodingChange,
        Tag,
        Restart,
        OverlappedClose,
        OverlappedReopen,
        InjectionBegin,
        InjectionEnd
    }
}
