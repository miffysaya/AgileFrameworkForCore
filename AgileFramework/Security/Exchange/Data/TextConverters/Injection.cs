using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Text;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal abstract class Injection : IDisposable
    {
        protected HeaderFooterFormat injectionFormat;

        protected string injectHead;

        protected string injectTail;

        protected bool headInjected;

        protected bool tailInjected;

        protected bool testBoundaryConditions;

        //protected Stream traceStream;

        public HeaderFooterFormat HeaderFooterFormat
        {
            get
            {
                return injectionFormat;
            }
        }

        public bool HaveHead
        {
            get
            {
                return injectHead != null;
            }
        }

        public bool HaveTail
        {
            get
            {
                return injectTail != null;
            }
        }

        public bool HeadDone
        {
            get
            {
                return headInjected;
            }
        }

        public bool TailDone
        {
            get
            {
                return tailInjected;
            }
        }

        public abstract void Inject(bool head, TextOutput output);

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void Reset()
        {
            headInjected = false;
            tailInjected = false;
        }

        public abstract void InjectRtfFonts(int firstAvailableFontHandle);

        public abstract void InjectRtfColors(int nextColorIndex);
    }
}
