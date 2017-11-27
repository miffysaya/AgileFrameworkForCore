using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Format;
using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal static class HtmlSupport
    {
        [Flags]
        public enum NumberParseFlags
        {
            Integer = 1,
            Float = 2,
            AbsoluteLength = 4,
            EmExLength = 8,
            Percentage = 16,
            Multiple = 32,
            HtmlFontUnits = 64,
            NonNegative = 8192,
            StyleSheetProperty = 16384,
            Strict = 32768,
            Length = 28,
            NonNegativeLength = 8220,
            FontSize = 8284
        }

        public const int HtmlNestingLimit = 4096;

        public const int MaxAttributeSize = 4096;

        public const int MaxCssPropertySize = 4096;

        public const int MaxNumberOfNonInlineStyles = 128;

        public static readonly byte[] UnsafeAsciiMap = new byte[]
        {
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            0,
            0,
            2,
            2,
            0,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            0,
            2,
            3,
            2,
            2,
            2,
            3,
            2,
            2,
            2,
            2,
            3,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            2,
            3,
            2,
            3,
            2,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            2,
            2,
            2,
            0,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2,
            2,
            2,
            2,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3
        };

        public static readonly HtmlEntityIndex[] EntityMap = new HtmlEntityIndex[]
        {
            HtmlEntityIndex.nbsp,
            HtmlEntityIndex.iexcl,
            HtmlEntityIndex.cent,
            HtmlEntityIndex.pound,
            HtmlEntityIndex.curren,
            HtmlEntityIndex.yen,
            HtmlEntityIndex.brvbar,
            HtmlEntityIndex.sect,
            HtmlEntityIndex.uml,
            HtmlEntityIndex.copy,
            HtmlEntityIndex.ordf,
            HtmlEntityIndex.laquo,
            HtmlEntityIndex.not,
            HtmlEntityIndex.shy,
            HtmlEntityIndex.reg,
            HtmlEntityIndex.macr,
            HtmlEntityIndex.deg,
            HtmlEntityIndex.plusmn,
            HtmlEntityIndex.sup2,
            HtmlEntityIndex.sup3,
            HtmlEntityIndex.acute,
            HtmlEntityIndex.micro,
            HtmlEntityIndex.para,
            HtmlEntityIndex.middot,
            HtmlEntityIndex.cedil,
            HtmlEntityIndex.sup1,
            HtmlEntityIndex.ordm,
            HtmlEntityIndex.raquo,
            HtmlEntityIndex.frac14,
            HtmlEntityIndex.frac12,
            HtmlEntityIndex.frac34,
            HtmlEntityIndex.iquest,
            HtmlEntityIndex.Agrave,
            HtmlEntityIndex.Aacute,
            HtmlEntityIndex.Acirc,
            HtmlEntityIndex.Atilde,
            HtmlEntityIndex.Auml,
            HtmlEntityIndex.Aring,
            HtmlEntityIndex.AElig,
            HtmlEntityIndex.Ccedil,
            HtmlEntityIndex.Egrave,
            HtmlEntityIndex.Eacute,
            HtmlEntityIndex.Ecirc,
            HtmlEntityIndex.Euml,
            HtmlEntityIndex.Igrave,
            HtmlEntityIndex.Iacute,
            HtmlEntityIndex.Icirc,
            HtmlEntityIndex.Iuml,
            HtmlEntityIndex.ETH,
            HtmlEntityIndex.Ntilde,
            HtmlEntityIndex.Ograve,
            HtmlEntityIndex.Oacute,
            HtmlEntityIndex.Ocirc,
            HtmlEntityIndex.Otilde,
            HtmlEntityIndex.Ouml,
            HtmlEntityIndex.times,
            HtmlEntityIndex.Oslash,
            HtmlEntityIndex.Ugrave,
            HtmlEntityIndex.Uacute,
            HtmlEntityIndex.Ucirc,
            HtmlEntityIndex.Uuml,
            HtmlEntityIndex.Yacute,
            HtmlEntityIndex.THORN,
            HtmlEntityIndex.szlig,
            HtmlEntityIndex.agrave,
            HtmlEntityIndex.aacute,
            HtmlEntityIndex.acirc,
            HtmlEntityIndex.atilde,
            HtmlEntityIndex.auml,
            HtmlEntityIndex.aring,
            HtmlEntityIndex.aelig,
            HtmlEntityIndex.ccedil,
            HtmlEntityIndex.egrave,
            HtmlEntityIndex.eacute,
            HtmlEntityIndex.ecirc,
            HtmlEntityIndex.euml,
            HtmlEntityIndex.igrave,
            HtmlEntityIndex.iacute,
            HtmlEntityIndex.icirc,
            HtmlEntityIndex.iuml,
            HtmlEntityIndex.eth,
            HtmlEntityIndex.ntilde,
            HtmlEntityIndex.ograve,
            HtmlEntityIndex.oacute,
            HtmlEntityIndex.ocirc,
            HtmlEntityIndex.otilde,
            HtmlEntityIndex.ouml,
            HtmlEntityIndex.divide,
            HtmlEntityIndex.oslash,
            HtmlEntityIndex.ugrave,
            HtmlEntityIndex.uacute,
            HtmlEntityIndex.ucirc,
            HtmlEntityIndex.uuml,
            HtmlEntityIndex.yacute,
            HtmlEntityIndex.thorn,
            HtmlEntityIndex.yuml
        };

        public static PropertyValue ParseNumber(BufferString value, NumberParseFlags parseFlags)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            ulong num = 0uL;
            int num2 = 0;
            int num3 = 0;
            bool flag4 = false;
            int num4 = 0;
            int length = value.Length;
            while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
            {
                num4++;
            }
            if (num4 == length)
            {
                return PropertyValue.Null;
            }
            if (num4 < length && (value[num4] == '-' || value[num4] == '+'))
            {
                flag2 = true;
                flag3 = (value[num4] == '-');
                num4++;
            }
            while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
            {
                flag = true;
                if (num < 1844674407370955152uL)
                {
                    num = num * 10uL + (ulong)(value[num4] - '0');
                }
                else
                {
                    num2++;
                }
                num4++;
            }
            if (num4 < length && value[num4] == '.')
            {
                flag4 = true;
                num4++;
                while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
                {
                    flag = true;
                    if (num < 1844674407370955152uL)
                    {
                        num = num * 10uL + (ulong)(value[num4] - '0');
                        num2--;
                    }
                    num4++;
                }
                if (num2 >= 0 && (parseFlags & NumberParseFlags.Strict) != 0)
                {
                    return PropertyValue.Null;
                }
            }
            if (!flag)
            {
                return PropertyValue.Null;
            }
            while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
            {
                num4++;
            }
            if (num4 < length && (value[num4] | ' ') == 'e' && num4 + 1 < length && (value[num4 + 1] == '-' || value[num4 + 1] == '+' || ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4 + 1]))))
            {
                flag4 = true;
                num4++;
                bool flag5 = false;
                if (value[num4] == '-' || value[num4] == '+')
                {
                    flag5 = (value[num4] == '-');
                    num4++;
                }
                while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
                {
                    num3 = num3 * 10 + (value[num4++] - '0');
                }
                if (flag5)
                {
                    num3 = -num3;
                }
                while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
                {
                    num4++;
                }
            }
            uint num5 = flag4 ? 10000u : 1u;
            uint num6 = 1u;
            PropertyType propertyType = flag4 ? PropertyType.Fractional : PropertyType.Integer;
            bool flag6 = false;
            int num7 = 0;
            if (num4 + 1 < length)
            {
                if ((value[num4] | ' ') == 'p')
                {
                    if ((value[num4 + 1] | ' ') == 'c')
                    {
                        num5 = 1920u;
                        num6 = 1u;
                        flag6 = true;
                        propertyType = PropertyType.AbsLength;
                        num7 = 2;
                    }
                    else if ((value[num4 + 1] | ' ') == 't')
                    {
                        num5 = 160u;
                        num6 = 1u;
                        flag6 = true;
                        propertyType = PropertyType.AbsLength;
                        num7 = 2;
                    }
                    else if ((value[num4 + 1] | ' ') == 'x')
                    {
                        num5 = 11520u;
                        num6 = 120u;
                        propertyType = PropertyType.Pixels;
                        flag6 = true;
                        num7 = 2;
                    }
                }
                else if ((value[num4] | ' ') == 'e')
                {
                    if ((value[num4 + 1] | ' ') == 'm')
                    {
                        num5 = 160u;
                        num6 = 1u;
                        propertyType = PropertyType.Ems;
                        flag6 = true;
                        num7 = 2;
                    }
                    else if ((value[num4 + 1] | ' ') == 'x')
                    {
                        num5 = 160u;
                        num6 = 1u;
                        propertyType = PropertyType.Exs;
                        flag6 = true;
                        num7 = 2;
                    }
                }
                else if ((value[num4] | ' ') == 'i')
                {
                    if ((value[num4 + 1] | ' ') == 'n')
                    {
                        num5 = 11520u;
                        num6 = 1u;
                        flag6 = true;
                        propertyType = PropertyType.AbsLength;
                        num7 = 2;
                    }
                }
                else if ((value[num4] | ' ') == 'c')
                {
                    if ((value[num4 + 1] | ' ') == 'm')
                    {
                        num5 = 1152000u;
                        num6 = 254u;
                        flag6 = true;
                        propertyType = PropertyType.AbsLength;
                        num7 = 2;
                    }
                }
                else if ((value[num4] | ' ') == 'm' && (value[num4 + 1] | ' ') == 'm')
                {
                    num5 = 115200u;
                    num6 = 254u;
                    flag6 = true;
                    propertyType = PropertyType.AbsLength;
                    num7 = 2;
                }
            }
            if (!flag6 && num4 < length)
            {
                if (value[num4] == '%')
                {
                    num5 = 10000u;
                    num6 = 1u;
                    propertyType = PropertyType.Percentage;
                    num7 = 1;
                }
                else if (value[num4] == '*')
                {
                    num5 = 1u;
                    num6 = 1u;
                    propertyType = PropertyType.Multiple;
                    num7 = 1;
                }
            }
            num4 += num7;
            if (num4 < length)
            {
                while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
                {
                    num4++;
                }
                if (num4 < length && (parseFlags & (NumberParseFlags.StyleSheetProperty | NumberParseFlags.Strict)) != 0)
                {
                    return PropertyValue.Null;
                }
            }
            if (num != 0uL)
            {
                int num8 = num2 + num3;
                if (num8 > 0)
                {
                    if (num8 > 20)
                    {
                        num8 = 0;
                        num = 18446744073709551615uL;
                    }
                    else
                    {
                        while (num8 != 0)
                        {
                            if (num > 1844674407370955161uL)
                            {
                                num8 = 0;
                                num = 18446744073709551615uL;
                                break;
                            }
                            num *= 10uL;
                            num8--;
                        }
                    }
                }
                else if (num8 < -10)
                {
                    if (num8 < -21)
                    {
                        num8 = 0;
                        num = 0uL;
                    }
                    else
                    {
                        while (num8 != -10)
                        {
                            num /= 10uL;
                            num8++;
                        }
                    }
                }
                num *= num5;
                num /= num6;
                while (num8 != 0)
                {
                    num /= 10uL;
                    num8++;
                }
                if (num > 67108863uL)
                {
                    num = 67108863uL;
                }
            }
            int num9 = (int)num;
            if (flag3)
            {
                num9 = -num9;
            }
            if (propertyType == PropertyType.Integer)
            {
                if ((parseFlags & NumberParseFlags.Integer) == 0)
                {
                    if ((parseFlags & NumberParseFlags.HtmlFontUnits) != 0)
                    {
                        if (flag2)
                        {
                            if (num9 < -7)
                            {
                                num9 = -7;
                            }
                            else if (num9 > 7)
                            {
                                num9 = 7;
                            }
                            propertyType = PropertyType.RelHtmlFontUnits;
                        }
                        else
                        {
                            if (num9 < 1)
                            {
                                num9 = 1;
                            }
                            else if (num9 > 7)
                            {
                                num9 = 7;
                            }
                            propertyType = PropertyType.HtmlFontUnits;
                        }
                    }
                    else if ((parseFlags & NumberParseFlags.AbsoluteLength) != 0)
                    {
                        num = num * 11520uL / 120uL;
                        if (num > 67108863uL)
                        {
                            num = 67108863uL;
                        }
                        num9 = (int)num;
                        if (flag3)
                        {
                            num9 = -num9;
                        }
                        propertyType = PropertyType.Pixels;
                    }
                    else
                    {
                        if ((parseFlags & NumberParseFlags.Float) == 0)
                        {
                            return PropertyValue.Null;
                        }
                        num *= 10000uL;
                        if (num > 67108863uL)
                        {
                            num = 67108863uL;
                        }
                        num9 = (int)num;
                        if (flag3)
                        {
                            num9 = -num9;
                        }
                        propertyType = PropertyType.Fractional;
                    }
                }
            }
            else if (propertyType == PropertyType.Fractional)
            {
                if ((parseFlags & NumberParseFlags.Float) == 0)
                {
                    if ((parseFlags & NumberParseFlags.AbsoluteLength) == 0)
                    {
                        return PropertyValue.Null;
                    }
                    num = num * 11520uL / 120uL / 10000uL;
                    if (num > 67108863uL)
                    {
                        num = 67108863uL;
                    }
                    num9 = (int)num;
                    if (flag3)
                    {
                        num9 = -num9;
                    }
                    propertyType = PropertyType.Pixels;
                }
            }
            else if (propertyType == PropertyType.AbsLength || propertyType == PropertyType.Pixels)
            {
                if ((parseFlags & NumberParseFlags.AbsoluteLength) == 0)
                {
                    return PropertyValue.Null;
                }
            }
            else if (propertyType == PropertyType.Ems || propertyType == PropertyType.Exs)
            {
                if ((parseFlags & NumberParseFlags.EmExLength) == 0)
                {
                    return PropertyValue.Null;
                }
            }
            else if (propertyType == PropertyType.Percentage)
            {
                if ((parseFlags & NumberParseFlags.Percentage) == 0)
                {
                    return PropertyValue.Null;
                }
            }
            else if (propertyType == PropertyType.Multiple && (parseFlags & NumberParseFlags.Multiple) == 0)
            {
                return PropertyValue.Null;
            }
            if (num9 < 0 && (parseFlags & NumberParseFlags.NonNegative) != 0 && propertyType != PropertyType.RelHtmlFontUnits)
            {
                return PropertyValue.Null;
            }
            return new PropertyValue(propertyType, num9);
        }
    }
}
