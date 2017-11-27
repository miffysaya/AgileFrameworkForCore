using System.Diagnostics;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal struct TokenRun
    {
        private Token token;

        public bool IsTextRun
        {
            get
            {
                return token.runList[token.wholePosition.run].Type >= (RunType)2147483648u;
            }
        }

        public bool IsNormal
        {
            get
            {
                return token.runList[token.wholePosition.run].Type == (RunType)2147483648u;
            }
        }

        public bool IsLiteral
        {
            get
            {
                return token.runList[token.wholePosition.run].Type == (RunType)3221225472u;
            }
        }

        public RunTextType TextType
        {
            get
            {
                return token.runList[token.wholePosition.run].TextType;
            }
        }

        public char[] RawBuffer
        {
            get
            {
                return token.buffer;
            }
        }

        public int RawOffset
        {
            get
            {
                return token.wholePosition.runOffset;
            }
        }

        public int RawLength
        {
            get
            {
                return token.runList[token.wholePosition.run].Length;
            }
        }

        public int Literal
        {
            get
            {
                return token.runList[token.wholePosition.run].Value;
            }
        }

        public int Length
        {
            get
            {
                if (IsNormal)
                {
                    return RawLength;
                }
                if (!IsLiteral)
                {
                    return 0;
                }
                return Token.LiteralLength(Literal);
            }
        }

        public char FirstChar
        {
            get
            {
                if (!IsLiteral)
                {
                    return RawBuffer[RawOffset];
                }
                return Token.LiteralFirstChar(Literal);
            }
        }

        public char LastChar
        {
            get
            {
                if (!IsLiteral)
                {
                    return RawBuffer[RawOffset + RawLength - 1];
                }
                return Token.LiteralLastChar(Literal);
            }
        }

        public bool IsAnyWhitespace
        {
            get
            {
                return TextType <= RunTextType.UnusualWhitespace;
            }
        }

        internal TokenRun(Token token)
        {
            this.token = token;
        }

        [Conditional("DEBUG")]
        private void AssertCurrent()
        {
        }
    }
}
