namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal enum RunType : uint
    {
        Invalid,
        Special = 1073741824u,
        Normal = 2147483648u,
        Literal = 3221225472u,
        Mask = 3221225472u
    }
}
