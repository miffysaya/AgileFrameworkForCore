namespace AgileFramework.Security.Application.TextConverters.HTML
{
    internal enum HtmlRunKind : uint
    {
        Invalid,
        Text = 67108864u,
        TagPrefix = 134217728u,
        TagSuffix = 201326592u,
        Name = 268435456u,
        NamePrefixDelimiter = 285212672u,
        TagWhitespace = 335544320u,
        AttrEqual = 402653184u,
        AttrQuote = 469762048u,
        AttrValue = 536870912u,
        TagText = 603979776u
    }
}
