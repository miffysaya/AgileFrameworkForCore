﻿using System;

namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal static class HtmlDtd
    {
        internal enum TagScope : byte
        {
            EMPTY,
            OVERLAP,
            NESTED
        }

        internal enum TagTextType : byte
        {
            NEVER,
            ALWAYS,
            QUERY
        }

        internal enum TagTextScope : byte
        {
            NEUTRAL,
            EXCLUDE,
            INCLUDE
        }

        internal enum ContextType : byte
        {
            None,
            Root,
            Head,
            Title,
            Body,
            TableCell,
            Select,
            Pre,
            Frameset,
            Frame,
            IFrame,
            Object,
            Script,
            Style,
            Comment,
            NoShow
        }

        internal enum ContextTextType : byte
        {
            None,
            Discard = 0,
            Literal,
            Full
        }

        [Flags]
        internal enum Literal : byte
        {
            None = 0,
            Tags = 1,
            Entities = 2
        }

        internal enum FillCode : byte
        {
            NUL,
            PUT,
            EAT
        }

        internal struct TagFill
        {
            internal readonly byte value;

            public static readonly TagFill PUT_NUL_PUT_NUL = new TagFill(FillCode.PUT, FillCode.NUL, FillCode.PUT, FillCode.NUL);

            public static readonly TagFill NUL_NUL_NUL_NUL = new TagFill(FillCode.NUL, FillCode.NUL, FillCode.NUL, FillCode.NUL);

            public static readonly TagFill NUL_EAT_EAT_NUL = new TagFill(FillCode.NUL, FillCode.EAT, FillCode.EAT, FillCode.NUL);

            public static readonly TagFill PUT_EAT_PUT_EAT = new TagFill(FillCode.PUT, FillCode.EAT, FillCode.PUT, FillCode.EAT);

            public static readonly TagFill PUT_PUT_PUT_PUT = new TagFill(FillCode.PUT, FillCode.PUT, FillCode.PUT, FillCode.PUT);

            public static readonly TagFill EAT_EAT_EAT_PUT = new TagFill(FillCode.EAT, FillCode.EAT, FillCode.EAT, FillCode.PUT);

            public static readonly TagFill PUT_PUT_PUT_EAT = new TagFill(FillCode.PUT, FillCode.PUT, FillCode.PUT, FillCode.EAT);

            public static readonly TagFill PUT_EAT_PUT_PUT = new TagFill(FillCode.PUT, FillCode.EAT, FillCode.PUT, FillCode.PUT);

            public static readonly TagFill PUT_EAT_EAT_EAT = new TagFill(FillCode.PUT, FillCode.EAT, FillCode.EAT, FillCode.EAT);

            public static readonly TagFill EAT_EAT_EAT_EAT = new TagFill(FillCode.EAT, FillCode.EAT, FillCode.EAT, FillCode.EAT);

            public static readonly TagFill PUT_EAT_EAT_PUT = new TagFill(FillCode.PUT, FillCode.EAT, FillCode.EAT, FillCode.PUT);

            public FillCode LB
            {
                get
                {
                    return (FillCode)(value >> 6);
                }
            }

            public FillCode RB
            {
                get
                {
                    return (FillCode)(value >> 4 & 3);
                }
            }

            public FillCode LE
            {
                get
                {
                    return (FillCode)(value >> 2 & 3);
                }
            }

            public FillCode RE
            {
                get
                {
                    return (FillCode)(value & 3);
                }
            }

            private TagFill(FillCode lB, FillCode rB, FillCode lE, FillCode rE)
            {
                value = (byte)((int)lB << 6 | (int)rB << 4 | (int)lE << 2 | (int)rE);
            }
        }

        internal enum FmtCode : byte
        {
            AUT,
            BRK,
            NBR
        }

        internal struct TagFmt
        {
            internal readonly byte value;

            public static readonly TagFmt BRK_BRK_BRK_BRK = new TagFmt(FmtCode.BRK, FmtCode.BRK, FmtCode.BRK, FmtCode.BRK);

            public static readonly TagFmt AUT_AUT_AUT_AUT = new TagFmt(FmtCode.AUT, FmtCode.AUT, FmtCode.AUT, FmtCode.AUT);

            public static readonly TagFmt NBR_BRK_NBR_BRK = new TagFmt(FmtCode.NBR, FmtCode.BRK, FmtCode.NBR, FmtCode.BRK);

            public static readonly TagFmt BRK_NBR_NBR_BRK = new TagFmt(FmtCode.BRK, FmtCode.NBR, FmtCode.NBR, FmtCode.BRK);

            public static readonly TagFmt BRK_BRK_NBR_BRK = new TagFmt(FmtCode.BRK, FmtCode.BRK, FmtCode.NBR, FmtCode.BRK);

            public static readonly TagFmt BRK_NBR_NBR_NBR = new TagFmt(FmtCode.BRK, FmtCode.NBR, FmtCode.NBR, FmtCode.NBR);

            public FmtCode LB
            {
                get
                {
                    return (FmtCode)(value >> 6);
                }
            }

            public FmtCode RB
            {
                get
                {
                    return (FmtCode)(value >> 4 & 3);
                }
            }

            public FmtCode LE
            {
                get
                {
                    return (FmtCode)(value >> 2 & 3);
                }
            }

            public FmtCode RE
            {
                get
                {
                    return (FmtCode)(value & 3);
                }
            }

            private TagFmt(FmtCode lB, FmtCode rB, FmtCode lE, FmtCode rE)
            {
                value = (byte)((int)lB << 6 | (int)rB << 4 | (int)lE << 2 | (int)rE);
            }
        }

        public enum SetId : short
        {
            Null,
            Empty
        }

        public class TagDefinition
        {
            public HtmlNameIndex nameIndex;

            public HtmlTagIndex tagIndex;

            public Literal literal;

            public bool blockElement;

            public TagFill fill;

            public TagFmt fmt;

            public TagScope scope;

            public TagTextType textType;

            public SetId endContainers;

            public SetId beginContainers;

            public SetId maskingContainers;

            public SetId prohibitedContainers;

            public SetId requiredContainers;

            public HtmlTagIndex defaultContainer;

            public bool queueForRequired;

            public TagTextScope textScope;

            public HtmlTagIndex textSubcontainer;

            public SetId match;

            public HtmlTagIndex unmatchedSubstitute;

            public ContextType contextType;

            public ContextTextType contextTextType;

            public SetId accept;

            public SetId reject;

            public SetId ignoreEnd;

            public TagDefinition(HtmlTagIndex tagIndex, HtmlNameIndex nameIndex, Literal literal, bool blockElement, TagFill fill, TagFmt fmt, TagScope scope, TagTextType textType, SetId endContainers, SetId beginContainers, SetId maskingContainers, SetId prohibitedContainers, SetId requiredContainers, HtmlTagIndex defaultContainer, bool queueForRequired, TagTextScope textScope, HtmlTagIndex textSubcontainer, SetId match, HtmlTagIndex unmatchedSubstitute, ContextType contextType, ContextTextType contextTextType, SetId accept, SetId reject, SetId ignoreEnd)
            {
                this.tagIndex = tagIndex;
                this.nameIndex = nameIndex;
                this.literal = literal;
                this.blockElement = blockElement;
                this.fill = fill;
                this.fmt = fmt;
                this.scope = scope;
                this.textType = textType;
                this.endContainers = endContainers;
                this.beginContainers = beginContainers;
                this.maskingContainers = maskingContainers;
                this.prohibitedContainers = prohibitedContainers;
                this.requiredContainers = requiredContainers;
                this.defaultContainer = defaultContainer;
                this.queueForRequired = queueForRequired;
                this.textScope = textScope;
                this.textSubcontainer = textSubcontainer;
                this.match = match;
                this.unmatchedSubstitute = unmatchedSubstitute;
                this.contextType = contextType;
                this.contextTextType = contextTextType;
                this.accept = accept;
                this.reject = reject;
                this.ignoreEnd = ignoreEnd;
            }
        }

        public static TagDefinition[] tags = new TagDefinition[]
        {
            new TagDefinition(HtmlTagIndex._NULL, HtmlNameIndex._NOTANAME, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._ROOT, HtmlNameIndex._NOTANAME, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.Body, SetId.Null, HtmlTagIndex._NULL, ContextType.Root, ContextTextType.Full, SetId.Null, SetId.Empty, SetId.Null),
            new TagDefinition(HtmlTagIndex._COMMENT, HtmlNameIndex._COMMENT, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._CONDITIONAL, HtmlNameIndex._CONDITIONAL, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._BANG, HtmlNameIndex._BANG, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._DTD, HtmlNameIndex._DTD, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._ASP, HtmlNameIndex._ASP, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Null, SetId.Null, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Unknown, HtmlNameIndex.Unknown, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.A, HtmlNameIndex.A, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, (SetId)45, (SetId)60, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Abbr, HtmlNameIndex.Abbr, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Acronym, HtmlNameIndex.Acronym, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Address, HtmlNameIndex.Address, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Applet, HtmlNameIndex.Applet, Literal.None, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.QUERY, (SetId)135, (SetId)135, SetId.Null, (SetId)150, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Area, HtmlNameIndex.Area, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.QUERY, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.B, HtmlNameIndex.B, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Base, HtmlNameIndex.Base, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.BaseFont, HtmlNameIndex.BaseFont, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.QUERY, (SetId)30, (SetId)135, SetId.Null, (SetId)180, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Bdo, HtmlNameIndex.Bdo, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.BGSound, HtmlNameIndex.BGSound, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Big, HtmlNameIndex.Big, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Blink, HtmlNameIndex.Blink, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.BlockQuote, HtmlNameIndex.BlockQuote, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Body, HtmlNameIndex.Body, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)135, (SetId)135, (SetId)195, (SetId)210, (SetId)135, HtmlTagIndex.Html, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Body, ContextTextType.Full, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.BR, HtmlNameIndex.BR, Literal.None, false, TagFill.PUT_EAT_PUT_EAT, TagFmt.NBR_BRK_NBR_BRK, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Button, HtmlNameIndex.Button, Literal.None, false, TagFill.EAT_EAT_EAT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)90, SetId.Null, (SetId)45, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Caption, HtmlNameIndex.Caption, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)270, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Center, HtmlNameIndex.Center, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Cite, HtmlNameIndex.Cite, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Code, HtmlNameIndex.Code, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Col, HtmlNameIndex.Col, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)285, SetId.Null, (SetId)300, (SetId)285, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.ColGroup, HtmlNameIndex.ColGroup, Literal.None, false, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)270, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Comment, HtmlNameIndex.Comment, Literal.Tags | Literal.Entities, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Comment, ContextTextType.Literal, SetId.Null, SetId.Empty, SetId.Null),
            new TagDefinition(HtmlTagIndex.DD, HtmlNameIndex.DD, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)315, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Del, HtmlNameIndex.Del, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Dfn, HtmlNameIndex.Dfn, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Dir, HtmlNameIndex.Dir, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Div, HtmlNameIndex.Div, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.DL, HtmlNameIndex.DL, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.DT, HtmlNameIndex.DT, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)315, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.EM, HtmlNameIndex.EM, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Embed, HtmlNameIndex.Embed, Literal.None, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.FieldSet, HtmlNameIndex.FieldSet, Literal.None, true, TagFill.PUT_EAT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Font, HtmlNameIndex.Font, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Form, HtmlNameIndex.Form, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.QUERY, SetId.Empty, (SetId)135, (SetId)330, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Frame, HtmlNameIndex.Frame, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, (SetId)330, (SetId)345, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Frame, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.FrameSet, HtmlNameIndex.FrameSet, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)135, (SetId)135, SetId.Null, (SetId)360, (SetId)135, HtmlTagIndex.Html, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Frameset, ContextTextType.None, (SetId)375, SetId.Null, (SetId)135),
            new TagDefinition(HtmlTagIndex.H1, HtmlNameIndex.H1, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.H2, HtmlNameIndex.H2, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.H3, HtmlNameIndex.H3, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.H4, HtmlNameIndex.H4, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.H5, HtmlNameIndex.H5, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.H6, HtmlNameIndex.H6, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, (SetId)390, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Head, HtmlNameIndex.Head, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)135, (SetId)135, (SetId)165, SetId.Null, (SetId)135, HtmlTagIndex.Html, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Head, ContextTextType.None, SetId.Null, SetId.Empty, (SetId)405),
            new TagDefinition(HtmlTagIndex.HR, HtmlNameIndex.HR, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Html, HtmlNameIndex.Html, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)135, (SetId)135, (SetId)135, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.Body, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.I, HtmlNameIndex.I, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Iframe, HtmlNameIndex.Iframe, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, (SetId)420, (SetId)195, HtmlTagIndex.Body, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.IFrame, ContextTextType.Full, (SetId)420, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Image, HtmlNameIndex.Image, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Img, HtmlNameIndex.Img, Literal.None, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Input, HtmlNameIndex.Input, Literal.None, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.QUERY, SetId.Empty, (SetId)135, (SetId)45, (SetId)435, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Ins, HtmlNameIndex.Ins, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.IsIndex, HtmlNameIndex.IsIndex, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Kbd, HtmlNameIndex.Kbd, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Label, HtmlNameIndex.Label, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Legend, HtmlNameIndex.Legend, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)135, SetId.Null, SetId.Null, (SetId)450, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.LI, HtmlNameIndex.LI, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_NBR, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)465, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Link, HtmlNameIndex.Link, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Listing, HtmlNameIndex.Listing, Literal.None, true, TagFill.PUT_PUT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.Pre, ContextTextType.Literal, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.Map, HtmlNameIndex.Map, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Marquee, HtmlNameIndex.Marquee, Literal.None, true, TagFill.PUT_EAT_EAT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, (SetId)135, (SetId)135, SetId.Null, SetId.Null, (SetId)195, HtmlTagIndex.Body, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Menu, HtmlNameIndex.Menu, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Meta, HtmlNameIndex.Meta, Literal.None, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.BRK_BRK_BRK_BRK, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.NextId, HtmlNameIndex.NextId, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.NoBR, HtmlNameIndex.NoBR, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)135, SetId.Null, (SetId)480, (SetId)195, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.NoEmbed, HtmlNameIndex.NoEmbed, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, (SetId)495, (SetId)510, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.NoFrames, HtmlNameIndex.NoFrames, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, (SetId)495, (SetId)510, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.NoScript, HtmlNameIndex.NoScript, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, (SetId)495, (SetId)510, HtmlTagIndex.Head, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Object, HtmlNameIndex.Object, Literal.None, false, TagFill.EAT_EAT_EAT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.QUERY, (SetId)135, (SetId)135, SetId.Null, (SetId)150, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.OL, HtmlNameIndex.OL, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.OptGroup, HtmlNameIndex.OptGroup, Literal.None, true, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)435, (SetId)435, SetId.Null, (SetId)525, (SetId)435, HtmlTagIndex._NULL, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Option, HtmlNameIndex.Option, Literal.None, false, TagFill.PUT_EAT_EAT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)435, (SetId)435, SetId.Null, (SetId)540, (SetId)435, HtmlTagIndex._NULL, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.P, HtmlNameIndex.P, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Param, HtmlNameIndex.Param, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, (SetId)150, SetId.Null, SetId.Null, (SetId)150, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.PlainText, HtmlNameIndex.PlainText, Literal.Tags | Literal.Entities, true, TagFill.PUT_PUT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Pre, ContextTextType.Literal, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.Pre, HtmlNameIndex.Pre, Literal.None, true, TagFill.PUT_PUT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.Pre, ContextTextType.Literal, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.Q, HtmlNameIndex.Q, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.RP, HtmlNameIndex.RP, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.NEVER, (SetId)555, (SetId)570, SetId.Null, (SetId)585, (SetId)600, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.RT, HtmlNameIndex.RT, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.NEVER, (SetId)570, (SetId)570, SetId.Null, (SetId)615, (SetId)600, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Ruby, HtmlNameIndex.Ruby, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.NEVER, (SetId)30, (SetId)30, SetId.Null, (SetId)630, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.S, HtmlNameIndex.S, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Samp, HtmlNameIndex.Samp, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Script, HtmlNameIndex.Script, Literal.Tags | Literal.Entities, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Script, ContextTextType.Literal, SetId.Null, SetId.Empty, SetId.Null),
            new TagDefinition(HtmlTagIndex.Select, HtmlNameIndex.Select, Literal.None, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, SetId.Empty, (SetId)135, (SetId)45, (SetId)435, (SetId)195, HtmlTagIndex.Body, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Select, ContextTextType.Full, (SetId)645, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Small, HtmlNameIndex.Small, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Span, HtmlNameIndex.Span, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Strike, HtmlNameIndex.Strike, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Strong, HtmlNameIndex.Strong, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Style, HtmlNameIndex.Style, Literal.Tags | Literal.Entities, false, TagFill.NUL_NUL_NUL_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Style, ContextTextType.Literal, SetId.Null, SetId.Empty, SetId.Null),
            new TagDefinition(HtmlTagIndex.Sub, HtmlNameIndex.Sub, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Sup, HtmlNameIndex.Sup, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Table, HtmlNameIndex.Table, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.ALWAYS, (SetId)255, (SetId)660, SetId.Null, (SetId)675, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Tbody, HtmlNameIndex.Tbody, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)270, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.TC, HtmlNameIndex.Unknown, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)690, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.INCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.TD, HtmlNameIndex.TD, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)705, (SetId)255, SetId.Null, (SetId)720, (SetId)735, HtmlTagIndex.TR, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.TableCell, ContextTextType.Full, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.TextArea, HtmlNameIndex.TextArea, Literal.Tags, false, TagFill.PUT_PUT_PUT_PUT, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.ALWAYS, SetId.Empty, (SetId)135, (SetId)45, (SetId)435, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Tfoot, HtmlNameIndex.Tfoot, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)270, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.TH, HtmlNameIndex.TH, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)705, (SetId)255, SetId.Null, (SetId)720, (SetId)735, HtmlTagIndex.TR, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.TableCell, ContextTextType.Full, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.Thead, HtmlNameIndex.Thead, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)255, (SetId)255, SetId.Null, (SetId)270, (SetId)255, HtmlTagIndex._NULL, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Title, HtmlNameIndex.Title, Literal.Tags, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.NEVER, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Title, ContextTextType.Full, SetId.Null, SetId.Empty, SetId.Null),
            new TagDefinition(HtmlTagIndex.TR, HtmlNameIndex.TR, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_BRK_BRK, TagScope.NESTED, TagTextType.NEVER, (SetId)750, (SetId)255, SetId.Null, (SetId)765, (SetId)780, HtmlTagIndex.Tbody, false, TagTextScope.EXCLUDE, HtmlTagIndex.TC, SetId.Null, HtmlTagIndex.TD, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.TT, HtmlNameIndex.TT, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.U, HtmlNameIndex.U, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.UL, HtmlNameIndex.UL, Literal.None, true, TagFill.PUT_EAT_PUT_EAT, TagFmt.BRK_BRK_NBR_BRK, TagScope.NESTED, TagTextType.QUERY, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._IMPLICIT_BEGIN, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Var, HtmlNameIndex.Var, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.OVERLAP, TagTextType.QUERY, (SetId)30, (SetId)30, SetId.Null, SetId.Null, (SetId)75, HtmlTagIndex.Body, true, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Wbr, HtmlNameIndex.Wbr, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.ALWAYS, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex.Xmp, HtmlNameIndex.Xmp, Literal.Tags | Literal.Entities, false, TagFill.PUT_PUT_PUT_EAT, TagFmt.BRK_NBR_NBR_BRK, TagScope.NESTED, TagTextType.ALWAYS, (SetId)90, (SetId)105, SetId.Null, (SetId)120, (SetId)105, HtmlTagIndex.Body, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.Pre, ContextTextType.Literal, SetId.Null, (SetId)225, (SetId)240),
            new TagDefinition(HtmlTagIndex.Xml, HtmlNameIndex.Xml, Literal.Tags | Literal.Entities, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.NESTED, TagTextType.QUERY, SetId.Empty, (SetId)135, SetId.Null, SetId.Null, (SetId)165, HtmlTagIndex.Head, false, TagTextScope.INCLUDE, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._Pxml, HtmlNameIndex._Pxml, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._Import, HtmlNameIndex._Import, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null),
            new TagDefinition(HtmlTagIndex._Xml_Namespace, HtmlNameIndex._Xml_Namespace, Literal.None, false, TagFill.PUT_NUL_PUT_NUL, TagFmt.AUT_AUT_AUT_AUT, TagScope.EMPTY, TagTextType.NEVER, SetId.Empty, SetId.Empty, SetId.Null, SetId.Null, SetId.Null, HtmlTagIndex._NULL, false, TagTextScope.NEUTRAL, HtmlTagIndex._NULL, SetId.Null, HtmlTagIndex._NULL, ContextType.None, ContextTextType.None, SetId.Null, SetId.Null, SetId.Null)
        };

        public static byte[] sets = new byte[]
        {
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
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            64,
            0,
            32,
            0,
            0,
            0,
            128,
            4,
            0,
            0,
            0,
            0,
            1,
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
            1,
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
            64,
            3,
            0,
            0,
            0,
            0,
            32,
            0,
            0,
            0,
            128,
            4,
            0,
            0,
            0,
            64,
            3,
            0,
            0,
            16,
            0,
            32,
            0,
            0,
            0,
            240,
            46,
            0,
            0,
            0,
            96,
            3,
            40,
            0,
            0,
            0,
            104,
            64,
            16,
            0,
            240,
            46,
            1,
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
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            16,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            32,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            0,
            0,
            0,
            16,
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
            1,
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
            64,
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
            16,
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
            48,
            80,
            0,
            0,
            128,
            5,
            0,
            0,
            0,
            0,
            0,
            128,
            64,
            0,
            65,
            0,
            80,
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
            16,
            0,
            0,
            0,
            0,
            0,
            66,
            0,
            0,
            0,
            0,
            0,
            128,
            1,
            16,
            96,
            10,
            0,
            0,
            0,
            0,
            64,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            16,
            0,
            0,
            0,
            0,
            0,
            2,
            0,
            0,
            0,
            0,
            0,
            128,
            1,
            16,
            96,
            10,
            0,
            0,
            0,
            0,
            0,
            65,
            0,
            0,
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
            8,
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
            32,
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
            8,
            16,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            128,
            0,
            0,
            0,
            0,
            49,
            0,
            0,
            0,
            8,
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
            192,
            15,
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
            64,
            0,
            0,
            0,
            80,
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
            1,
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
            16,
            0,
            0,
            0,
            0,
            0,
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
            0,
            0,
            0,
            0,
            0,
            0,
            0,
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
            28,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            64,
            0,
            0,
            32,
            16,
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
            128,
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
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            64,
            0,
            32,
            0,
            128,
            1,
            128,
            4,
            0,
            0,
            0,
            0,
            3,
            0,
            0,
            64,
            0,
            32,
            0,
            0,
            1,
            128,
            4,
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
            64,
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
            1,
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
            192,
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
            128,
            1,
            0,
            0,
            0,
            128,
            0,
            0,
            0,
            0,
            0,
            0,
            8,
            0,
            128,
            1,
            8,
            240,
            47,
            0,
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
            208,
            4,
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
            128,
            1,
            16,
            80,
            0,
            0,
            0,
            0,
            0,
            66,
            0,
            0,
            0,
            0,
            0,
            128,
            1,
            16,
            192,
            4,
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
            16,
            32,
            0,
            0,
            1,
            0,
            70,
            0,
            200,
            15,
            0,
            16,
            128,
            1,
            16,
            192,
            4,
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
            32,
            0,
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
            176,
            14,
            0,
            0,
            1,
            0,
            70,
            0,
            200,
            15,
            0,
            16,
            128,
            1,
            16,
            64,
            32,
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
            32,
            10,
            0
        };

        public static bool IsTagInSet(HtmlTagIndex tag, SetId set)
        {
            return 0 != (sets[(int)(set + (short)((int)tag >> 3))] & (byte)(1 << (int)(tag & HtmlTagIndex.Unknown)));
        }
    }
}
