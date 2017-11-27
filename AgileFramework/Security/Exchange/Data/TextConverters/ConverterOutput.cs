using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal abstract class ConverterOutput : ITextSink, IDisposable
    {
        protected const int stringBufferMax = 128;

        protected const int stringBufferReserve = 20;

        protected const int stringBufferThreshold = 108;

        protected char[] stringBuffer;

        //private IFallback fallback;

        public abstract bool CanAcceptMore
        {
            get;
        }

        bool ITextSink.IsEnough
        {
            get
            {
                return false;
            }
        }

        public ConverterOutput()
        {
            stringBuffer = new char[128];
        }

        public abstract void Write(char[] buffer, int offset, int count, IFallback fallback);

        public abstract void Flush();

        public virtual void Write(string text)
        {
            Write(text, 0, text.Length, null);
        }

        public void Write(string text, IFallback fallback)
        {
            Write(text, 0, text.Length, fallback);
        }

        public void Write(string text, int offset, int count)
        {
            Write(text, offset, count, null);
        }

        public void Write(string text, int offset, int count, IFallback fallback)
        {
            if (stringBuffer.Length < count)
            {
                stringBuffer = new char[count * 2];
            }
            text.CopyTo(offset, stringBuffer, 0, count);
            Write(stringBuffer, 0, count, fallback);
        }

        public void Write(char ch)
        {
            Write(ch, null);
        }

        public void Write(char ch, IFallback fallback)
        {
            stringBuffer[0] = ch;
            Write(stringBuffer, 0, 1, fallback);
        }

        public void Write(int ucs32Literal, IFallback fallback)
        {
            if (ucs32Literal > 65535)
            {
                stringBuffer[0] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Literal);
                stringBuffer[1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Literal);
            }
            else
            {
                stringBuffer[0] = (char)ucs32Literal;
            }
            Write(stringBuffer, 0, (ucs32Literal > 65535) ? 2 : 1, fallback);
        }

        void ITextSink.Write(char[] buffer, int offset, int count)
        {
            Write(buffer, offset, count, null);
        }

        void ITextSink.Write(int ucs32Literal)
        {
            Write(ucs32Literal, null);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose()
        {
        }
    }
}
