namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal enum RunTextType : uint
    {
        Unknown,
        Space = 134217728u,
        NewLine = 268435456u,
        Tabulation = 402653184u,
        UnusualWhitespace = 536870912u,
        LastWhitespace = 536870912u,
        Nbsp = 671088640u,
        NonSpace = 805306368u,
        LastText = 805306368u,
        Last = 805306368u,
        Mask = 939524096u
    }
}
