using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Text;
using System;
using System.IO;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    internal class HtmlInjection : Injection
    {
        protected bool filterHtml;

        protected HtmlTagCallback callback;

        protected bool injectingHead;

        protected IProgressMonitor progressMonitor;

        protected IHtmlParser documentParser;

        protected HtmlParser fragmentParser;

        protected HtmlToHtmlConverter fragmentToHtmlConverter;

        protected HtmlToTextConverter fragmentToTextConverter;

        public bool Active
        {
            get
            {
                return documentParser != null;
            }
        }

        public bool InjectingHead
        {
            get
            {
                return injectingHead;
            }
        }

        public HtmlInjection(string injectHead, string injectTail, HeaderFooterFormat injectionFormat, bool filterHtml, HtmlTagCallback callback, bool testBoundaryConditions, Stream traceStream, IProgressMonitor progressMonitor)
        {
            this.injectHead = injectHead;
            this.injectTail = injectTail;
            this.injectionFormat = injectionFormat;
            this.filterHtml = filterHtml;
            this.callback = callback;
            this.testBoundaryConditions = testBoundaryConditions;
            this.progressMonitor = progressMonitor;
        }

        public IHtmlParser Push(bool head, IHtmlParser documentParser)
        {
            if (head)
            {
                if (injectHead != null && !headInjected)
                {
                    this.documentParser = documentParser;
                    if (fragmentParser == null)
                    {
                        fragmentParser = new HtmlParser(new ConverterBufferInput(injectHead, progressMonitor), false, injectionFormat == HeaderFooterFormat.Text, 64, 8, testBoundaryConditions);
                    }
                    else
                    {
                        fragmentParser.Initialize(injectHead, injectionFormat == HeaderFooterFormat.Text);
                    }
                    injectingHead = true;
                    return fragmentParser;
                }
            }
            else
            {
                if (injectHead != null && !headInjected)
                {
                    headInjected = true;
                }
                if (injectTail != null && !tailInjected)
                {
                    this.documentParser = documentParser;
                    if (fragmentParser == null)
                    {
                        fragmentParser = new HtmlParser(new ConverterBufferInput(injectTail, progressMonitor), false, injectionFormat == HeaderFooterFormat.Text, 64, 8, testBoundaryConditions);
                    }
                    else
                    {
                        fragmentParser.Initialize(injectTail, injectionFormat == HeaderFooterFormat.Text);
                    }
                    injectingHead = false;
                    return fragmentParser;
                }
            }
            return documentParser;
        }

        public IHtmlParser Pop()
        {
            if (injectingHead)
            {
                headInjected = true;
                if (injectTail == null)
                {
                    ((IDisposable)fragmentParser).Dispose();
                    fragmentParser = null;
                }
            }
            else
            {
                tailInjected = true;
                ((IDisposable)fragmentParser).Dispose();
                fragmentParser = null;
            }
            IHtmlParser result = documentParser;
            documentParser = null;
            return result;
        }

        public override void Inject(bool head, TextOutput output)
        {
            if (head)
            {
                if (injectHead != null && !headInjected)
                {
                    HtmlParser parser = new HtmlParser(new ConverterBufferInput(injectHead, progressMonitor), false, injectionFormat == HeaderFooterFormat.Text, 64, 8, testBoundaryConditions);
                    fragmentToTextConverter = new HtmlToTextConverter(parser, output, null, true, injectionFormat == HeaderFooterFormat.Text, false, null, true, 0);
                    while (!fragmentToTextConverter.Flush())
                    {
                    }
                    headInjected = true;
                    if (injectTail == null)
                    {
                        ((IDisposable)fragmentToTextConverter).Dispose();
                        fragmentToTextConverter = null;
                        return;
                    }
                }
            }
            else
            {
                if (injectHead != null && !headInjected)
                {
                    headInjected = true;
                }
                if (injectTail != null && !tailInjected)
                {
                    if (fragmentToTextConverter == null)
                    {
                        HtmlParser parser = new HtmlParser(new ConverterBufferInput(injectTail, progressMonitor), false, injectionFormat == HeaderFooterFormat.Text, 64, 8, testBoundaryConditions);
                        fragmentToTextConverter = new HtmlToTextConverter(parser, output, null, true, injectionFormat == HeaderFooterFormat.Text, false, null, true, 0);
                    }
                    else
                    {
                        fragmentToTextConverter.Initialize(injectTail, injectionFormat == HeaderFooterFormat.Text);
                    }
                    while (!fragmentToTextConverter.Flush())
                    {
                    }
                    ((IDisposable)fragmentToTextConverter).Dispose();
                    fragmentToTextConverter = null;
                    tailInjected = true;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (fragmentToHtmlConverter != null)
            {
                ((IDisposable)fragmentToHtmlConverter).Dispose();
                fragmentToHtmlConverter = null;
            }
            if (fragmentToTextConverter != null)
            {
                ((IDisposable)fragmentToTextConverter).Dispose();
                fragmentToTextConverter = null;
            }
            if (fragmentParser != null)
            {
                ((IDisposable)fragmentParser).Dispose();
                fragmentParser = null;
            }
            base.Reset();
            base.Dispose(disposing);
        }

        public override void InjectRtfFonts(int firstAvailableFontHandle)
        {
        }

        public override void InjectRtfColors(int nextColorIndex)
        {
        }
    }
}
