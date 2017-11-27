using AgileFramework.Security.Exchange.CtsResources;
using AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html;
using System;
using System.IO;
using System.Text;

namespace AgileFramework.Security.Exchange.Data.TextConverters
{
    /// <summary>
    /// A HTML to HTML converter.
    /// </summary>
    internal class HtmlToHtml : TextConverter
    {
        /// <summary>
        /// The input encoding.
        /// </summary>
        private Encoding inputEncoding;

        /// <summary>
        /// Value indicating whether encoding should be detected from the BOM
        /// </summary>
        private bool detectEncodingFromByteOrderMark = true;

        /// <summary>
        /// Value indicating whether encoding should be detected from the charset meta tag/
        /// </summary>
        private bool detectEncodingFromMetaTag = true;

        /// <summary>
        /// The output encoding.
        /// </summary>
        private Encoding outputEncoding;

        /// <summary>
        /// Value indicating if the output encoding should be the same as the input encoding.
        /// </summary>
        private bool outputEncodingSameAsInput = true;

        /// <summary>
        /// Value indicating if the HTML input should be normalized.
        /// </summary>
        private bool normalizeInputHtml;

        /// <summary>
        /// The format to use for header and footer injection.
        /// </summary>
        //private HeaderFooterFormat injectionFormat;

        /// <summary>
        /// The header to inject.
        /// </summary>
        //private string injectHead;

        /// <summary>
        /// The tail to inject.
        /// </summary>
        //private string injectTail;

        /// <summary>
        /// Value indicating if HTML should be filtered.
        /// </summary>
        private bool filterHtml;

        /// <summary>
        /// The call back to use when parsing HTML
        /// </summary>
        //private HtmlTagCallback htmlCallback;

        /// <summary>
        /// Value indicating if truncation should be tested for when a callback is performed.
        /// </summary>
        private bool testTruncateForCallback = true;

        /// <summary>
        /// Value indicating fragmented output can be generated.
        /// </summary>        
        private bool outputFragment;

        /// <summary>
        /// The maximum number of tokenisation runs to perform.
        /// </summary>
        private int testMaxTokenRuns = 512;

        /// <summary>
        /// The trace stream for tokenisation
        /// </summary>
        //private Stream testTraceStream;

        /// <summary>
        /// Value indicating if the test traces should show the token number.
        /// </summary>
        private bool testTraceShowTokenNum = true;

        /// <summary>
        /// The token number at which test tracing should stop.
        /// </summary>
        //private int testTraceStopOnTokenNum;

        //private Stream testNormalizerTraceStream;

        private bool testNormalizerTraceShowTokenNum = true;

        //private int testNormalizerTraceStopOnTokenNum;

        /// <summary>
        /// The maximum size of an HTML tag.
        /// </summary>
        private int maxHtmlTagSize = 32768;

        /// <summary>
        /// The maximum number of attributes for an HTML tag
        /// </summary>
        private int testMaxHtmlTagAttributes = 64;

        /// <summary>
        /// The maximum offset for parsing restarting.
        /// </summary>
        private int testMaxHtmlRestartOffset = 4096;

        /// <summary>
        /// The limit for nested tags.
        /// </summary>
        private int testMaxHtmlNormalizerNesting = 4096;

        /// <summary>
        /// The threshold for small CSS blocks.
        /// </summary>
        private int smallCssBlockThreshold = -1;

        /// <summary>
        /// Value indicating whether display styles should be reserved.
        /// </summary>
        //private bool preserveDisplayNoneStyle;

        internal bool NormalizeHtml
        {
            set
            {
                base.AssertNotLocked();
                this.normalizeInputHtml = value;
            }
        }

        internal bool OutputHtmlFragment
        {
            set
            {
                base.AssertNotLocked();
                this.outputFragment = value;
            }
        }

        internal bool FilterHtml
        {
            set
            {
                base.AssertNotLocked();
                this.filterHtml = value;
            }
        }

        internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, Stream output)
        {
            if (this.inputEncoding == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
            }
            ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, base.TestBoundaryConditions, this, null);
            ConverterOutput output2 = new ConverterEncodingOutput(output, true, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, base.TestBoundaryConditions, this);
            return this.CreateChain(input, output2, converterStream);
        }

        internal override IProducerConsumer CreatePushChain(ConverterStream converterStream, TextWriter output)
        {
            if (this.inputEncoding == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
            }
            this.outputEncoding = Encoding.Unicode;
            ConverterInput input = new ConverterDecodingInput(converterStream, true, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, base.TestBoundaryConditions, this, null);
            ConverterOutput output2 = new ConverterUnicodeOutput(output, true, true);
            return this.CreateChain(input, output2, converterStream);
        }

        internal override IProducerConsumer CreatePullChain(Stream input, ConverterStream converterStream)
        {
            if (this.inputEncoding == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
            }
            ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, base.TestBoundaryConditions, this, converterStream);
            ConverterOutput output = new ConverterEncodingOutput(converterStream, false, true, this.outputEncodingSameAsInput ? this.inputEncoding : this.outputEncoding, this.outputEncodingSameAsInput, base.TestBoundaryConditions, this);
            return this.CreateChain(input2, output, converterStream);
        }

        internal override IProducerConsumer CreatePullChain(TextReader input, ConverterStream converterStream)
        {
            this.inputEncoding = Encoding.Unicode;
            ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxHtmlTagSize, base.TestBoundaryConditions, converterStream);
            ConverterOutput output = new ConverterEncodingOutput(converterStream, false, false, this.outputEncodingSameAsInput ? Encoding.UTF8 : this.outputEncoding, this.outputEncodingSameAsInput, base.TestBoundaryConditions, this);
            return this.CreateChain(input2, output, converterStream);
        }

        internal override IProducerConsumer CreatePullChain(Stream input, ConverterReader converterReader)
        {
            if (this.inputEncoding == null)
            {
                throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
            }
            this.outputEncoding = Encoding.Unicode;
            ConverterInput input2 = new ConverterDecodingInput(input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, this.maxHtmlTagSize, this.testMaxHtmlRestartOffset, base.InputStreamBufferSize, base.TestBoundaryConditions, this, converterReader);
            ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, true);
            return this.CreateChain(input2, output, converterReader);
        }

        internal override IProducerConsumer CreatePullChain(TextReader input, ConverterReader converterReader)
        {
            this.inputEncoding = Encoding.Unicode;
            this.outputEncoding = Encoding.Unicode;
            ConverterInput input2 = new ConverterUnicodeInput(input, false, this.maxHtmlTagSize, base.TestBoundaryConditions, converterReader);
            ConverterOutput output = new ConverterUnicodeOutput(converterReader, false, false);
            return this.CreateChain(input2, output, converterReader);
        }

        internal override void SetResult(ConfigParameter parameterId, object val)
        {
            switch (parameterId)
            {
                case ConfigParameter.InputEncoding:
                    this.inputEncoding = (Encoding)val;
                    break;
                case ConfigParameter.OutputEncoding:
                    this.outputEncoding = (Encoding)val;
                    break;
            }
            base.SetResult(parameterId, val);
        }

        private IProducerConsumer CreateChain(ConverterInput input, ConverterOutput output, IProgressMonitor progressMonitor)
        {
            base.Locked = true;
            HtmlInjection htmlInjection = null;
            //if (this.injectHead != null || this.injectTail != null)
            //{
            //    htmlInjection = new HtmlInjection(this.injectHead, this.injectTail, this.injectionFormat, this.filterHtml, null, base.TestBoundaryConditions, null, progressMonitor);
            //    this.normalizeInputHtml = true;
            //}
            if (this.filterHtml || this.outputFragment)// || this.htmlCallback != null)
            {
                this.normalizeInputHtml = true;
            }
            IHtmlParser parser2;
            if (this.normalizeInputHtml)
            {
                HtmlParser parser = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, base.TestBoundaryConditions);
                parser2 = new HtmlNormalizingParser(parser, htmlInjection, false, this.testMaxHtmlNormalizerNesting, base.TestBoundaryConditions, null, this.testNormalizerTraceShowTokenNum, 0);
            }
            else
            {
                parser2 = new HtmlParser(input, this.detectEncodingFromMetaTag, false, this.testMaxTokenRuns, this.testMaxHtmlTagAttributes, base.TestBoundaryConditions);
            }
            HtmlWriter writer = new HtmlWriter(output, this.filterHtml, this.normalizeInputHtml);
            return new HtmlToHtmlConverter(parser2, writer, false, this.outputFragment, this.filterHtml, null, this.testTruncateForCallback, htmlInjection != null && htmlInjection.HaveTail, null, this.testTraceShowTokenNum, 0, this.smallCssBlockThreshold, false, progressMonitor);
        }
    }
}
