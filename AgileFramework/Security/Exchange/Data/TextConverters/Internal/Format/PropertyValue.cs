namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Format
{
    internal struct PropertyValue
    {
        private const uint TypeMask = 4160749568u;

        private const int TypeShift = 27;

        private const uint ValueMask = 134217727u;

        private const int ValueShift = 5;

        public const int ValueMax = 67108863;

        public const int ValueMin = -67108863;

        private uint rawValue;

        public static readonly PropertyValue Null = default(PropertyValue);

        public static readonly PropertyValue True = new PropertyValue(true);

        public static readonly PropertyValue False = new PropertyValue(false);

        internal static readonly int[] sizesInTwips = new int[]
        {
            151,
            200,
            240,
            271,
            360,
            480,
            720
        };

        internal static readonly int[] maxSizesInTwips = new int[]
        {
            160,
            220,
            260,
            320,
            420,
            620
        };

        public uint RawType
        {
            get
            {
                return rawValue & 4160749568u;
            }
        }

        public PropertyType Type
        {
            get
            {
                return (PropertyType)((rawValue & 4160749568u) >> 27);
            }
        }

        public int Value
        {
            get
            {
                return (int)(rawValue & 134217727u) << 5 >> 5;
            }
        }

        public uint UnsignedValue
        {
            get
            {
                return rawValue & 134217727u;
            }
        }

        public bool IsAbsRelLength
        {
            get
            {
                return RawType == PropertyValue.GetRawType(PropertyType.AbsLength) || RawType == PropertyValue.GetRawType(PropertyType.RelLength) || RawType == PropertyValue.GetRawType(PropertyType.Pixels);
            }
        }

        public int StringHandle
        {
            get
            {
                return Value;
            }
        }

        public int MultiValueHandle
        {
            get
            {
                return Value;
            }
        }

        public bool Bool
        {
            get
            {
                return UnsignedValue != 0u;
            }
        }

        public int Enum
        {
            get
            {
                return (int)UnsignedValue;
            }
        }

        public RGBT Color
        {
            get
            {
                return new RGBT(UnsignedValue);
            }
        }

        public float Percentage
        {
            get
            {
                return Value / 10000f;
            }
        }

        public float Fractional
        {
            get
            {
                return Value / 10000f;
            }
        }

        public int Integer
        {
            get
            {
                return Value;
            }
        }

        public int Milliseconds
        {
            get
            {
                return Value;
            }
        }

        public int kHz
        {
            get
            {
                return Value;
            }
        }

        public int Degrees
        {
            get
            {
                return Value;
            }
        }

        public float Points
        {
            get
            {
                return Value / 160f;
            }
        }

        public float Inches
        {
            get
            {
                return Value / 11520f;
            }
        }

        public float Millimeters
        {
            get
            {
                return Value / 453.5433f;
            }
        }

        public int HtmlFontUnits
        {
            get
            {
                return Value;
            }
        }

        public float Pixels
        {
            get
            {
                return Value / 96f;
            }
        }

        public int PixelsInteger
        {
            get
            {
                return Value / 96;
            }
        }

        public float Ems
        {
            get
            {
                return Value / 160f;
            }
        }

        public float Exs
        {
            get
            {
                return Value / 160f;
            }
        }

        public int RelativeHtmlFontUnits
        {
            get
            {
                return Value;
            }
        }

        public PropertyValue(bool value)
        {
            rawValue = PropertyValue.ComposeRawValue(value);
        }

        public PropertyValue(PropertyType type, int value)
        {
            rawValue = PropertyValue.ComposeRawValue(type, value);
        }

        private static uint ComposeRawValue(bool value)
        {
            return PropertyValue.GetRawType(PropertyType.Bool) | (value ? 1u : 0u);
        }

        private static uint ComposeRawValue(PropertyType type, int value)
        {
            return (uint)((int)type << 27 | (value & 134217727));
        }

        public static uint GetRawType(PropertyType type)
        {
            return (uint)type << 27;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case PropertyType.Null:
                    return "null";
                case PropertyType.Calculated:
                    return "calculated";
                case PropertyType.Bool:
                    return Bool.ToString();
                case PropertyType.String:
                    return "string: " + StringHandle.ToString();
                case PropertyType.MultiValue:
                    return "multi: " + MultiValueHandle.ToString();
                case PropertyType.Enum:
                    return "enum: " + Enum.ToString();
                case PropertyType.Color:
                    return "color: " + Color.ToString();
                case PropertyType.Integer:
                    return Integer.ToString() + " (integer)";
                case PropertyType.Fractional:
                    return Fractional.ToString() + " (fractional)";
                case PropertyType.Percentage:
                    return Percentage.ToString() + "%";
                case PropertyType.AbsLength:
                    return string.Concat(new string[]
                    {
                    Points.ToString(),
                    "pt (",
                    Inches.ToString(),
                    "in, ",
                    Millimeters.ToString(),
                    "mm) (abs)"
                    });
                case PropertyType.RelLength:
                    return string.Concat(new string[]
                    {
                    Points.ToString(),
                    "pt (",
                    Inches.ToString(),
                    "in, ",
                    Millimeters.ToString(),
                    "mm) (rel)"
                    });
                case PropertyType.Pixels:
                    return Pixels.ToString() + "px";
                case PropertyType.Ems:
                    return Ems.ToString() + "em";
                case PropertyType.Exs:
                    return Exs.ToString() + "ex";
                case PropertyType.HtmlFontUnits:
                    return HtmlFontUnits.ToString() + " (html font units)";
                case PropertyType.RelHtmlFontUnits:
                    return RelativeHtmlFontUnits.ToString() + " (relative html font units)";
                case PropertyType.Multiple:
                    return Integer.ToString() + "*";
                case PropertyType.Milliseconds:
                    return Milliseconds.ToString() + "ms";
                case PropertyType.kHz:
                    return kHz.ToString() + "kHz";
                case PropertyType.Degrees:
                    return Degrees.ToString() + "deg";
                default:
                    return "unknown value type";
            }
        }

        public static bool operator ==(PropertyValue x, PropertyValue y)
        {
            return x.rawValue == y.rawValue;
        }

        public static bool operator !=(PropertyValue x, PropertyValue y)
        {
            return x.rawValue != y.rawValue;
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyValue && rawValue == ((PropertyValue)obj).rawValue;
        }

        public override int GetHashCode()
        {
            return (int)rawValue;
        }
    }
}
