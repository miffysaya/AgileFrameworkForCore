namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Format
{
    internal struct RGBT
    {
        private uint rawValue;

        public bool IsTransparent
        {
            get
            {
                return Transparency == 7u;
            }
        }

        public uint Transparency
        {
            get
            {
                return rawValue >> 24 & 7u;
            }
        }

        public uint Red
        {
            get
            {
                return rawValue >> 16 & 255u;
            }
        }

        public uint Green
        {
            get
            {
                return rawValue >> 8 & 255u;
            }
        }

        public uint Blue
        {
            get
            {
                return rawValue & 255u;
            }
        }

        public RGBT(uint rawValue)
        {
            this.rawValue = rawValue;
        }

        public override string ToString()
        {
            if (!IsTransparent)
            {
                return string.Concat(new string[]
                {
                    "rgb(",
                    Red.ToString(),
                    ", ",
                    Green.ToString(),
                    ", ",
                    Blue.ToString(),
                    ")",
                    (Transparency != 0u) ? ("+t" + Transparency.ToString()) : ""
                });
            }
            return "transparent";
        }
    }
}
