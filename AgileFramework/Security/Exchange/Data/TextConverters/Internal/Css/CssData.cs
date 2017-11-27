﻿namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Css
{
    internal static class CssData
    {
        public struct NameDef
        {
            public short hash;

            public string name;

            public CssNameIndex publicNameId;

            public NameDef(short hash, string name, CssNameIndex publicNameId)
            {
                this.hash = hash;
                this.name = name;
                this.publicNameId = publicNameId;
            }
        }

        public enum FilterAction : byte
        {
            Unknown,
            Drop,
            Keep,
            CheckContent
        }

        public struct FilterActionEntry
        {
            public FilterAction propertyAction;

            public FilterActionEntry(FilterAction propertyAction)
            {
                this.propertyAction = propertyAction;
            }
        }

        public const short MAX_NAME = 26;

        public const short MAX_TAG_NAME = 26;

        public const short NAME_HASH_SIZE = 329;

        public const int NAME_HASH_MODIFIER = 2;

        public static CssNameIndex[] nameHashTable = new CssNameIndex[]
        {
            CssNameIndex.ScrollbarArrowColor,
            CssNameIndex.Unknown,
            CssNameIndex.WhiteSpace,
            CssNameIndex.Unknown,
            CssNameIndex.LineBreak,
            CssNameIndex.Orphans,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.WritingMode,
            CssNameIndex.Unknown,
            CssNameIndex.Scrollbar3dLightColor,
            CssNameIndex.Unknown,
            CssNameIndex.TextAutospace,
            CssNameIndex.VerticalAlign,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderRight,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Bottom,
            CssNameIndex.LineHeight,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderBottom,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.ScrollbarBaseColor,
            CssNameIndex.MinWidth,
            CssNameIndex.BackgroundColor,
            CssNameIndex.Unknown,
            CssNameIndex.BorderTopStyle,
            CssNameIndex.Unknown,
            CssNameIndex.EmptyCells,
            CssNameIndex.Unknown,
            CssNameIndex.ListStyleType,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.TextAlign,
            CssNameIndex.Unknown,
            CssNameIndex.FontWeight,
            CssNameIndex.Unknown,
            CssNameIndex.OutlineWidth,
            CssNameIndex.CaptionSide,
            CssNameIndex.ScrollbarShadowColor,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Clip,
            CssNameIndex.Unknown,
            CssNameIndex.MarginLeft,
            CssNameIndex.BorderTopWidth,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Azimuth,
            CssNameIndex.Float,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.LayoutFlow,
            CssNameIndex.MinHeight,
            CssNameIndex.Content,
            CssNameIndex.Unknown,
            CssNameIndex.Padding,
            CssNameIndex.BorderBottomWidth,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Visibility,
            CssNameIndex.Unknown,
            CssNameIndex.Overflow,
            CssNameIndex.BorderLeftColor,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Pitch,
            CssNameIndex.Pause,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.OverflowY,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.ScrollbarHighlightColor,
            CssNameIndex.Unknown,
            CssNameIndex.Height,
            CssNameIndex.Unknown,
            CssNameIndex.WordWrap,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Top,
            CssNameIndex.ListStyle,
            CssNameIndex.Unknown,
            CssNameIndex.Margin,
            CssNameIndex.Unknown,
            CssNameIndex.TextKashidaSpace,
            CssNameIndex.VoiceFamily,
            CssNameIndex.CueBefore,
            CssNameIndex.Clear,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.TextOverflow,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderBottomStyle,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderColor,
            CssNameIndex.TextDecoration,
            CssNameIndex.Display,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.CounterReset,
            CssNameIndex.MarginBottom,
            CssNameIndex.BorderStyle,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.LayoutGrid,
            CssNameIndex.Quotes,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Accelerator,
            CssNameIndex.Border,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Zoom,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.OutlineStyle,
            CssNameIndex.Unknown,
            CssNameIndex.Width,
            CssNameIndex.Unknown,
            CssNameIndex.Color,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.PageBreakInside,
            CssNameIndex.Unknown,
            CssNameIndex.PitchRange,
            CssNameIndex.BorderCollapse,
            CssNameIndex.Cue,
            CssNameIndex.Unknown,
            CssNameIndex.Left,
            CssNameIndex.LayoutGridMode,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.SpeakPunctuation,
            CssNameIndex.LayoutGridLine,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderSpacing,
            CssNameIndex.Unknown,
            CssNameIndex.TextTransform,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderRightWidth,
            CssNameIndex.PageBreakBefore,
            CssNameIndex.TextIndent,
            CssNameIndex.LayoutGridChar,
            CssNameIndex.SpeechRate,
            CssNameIndex.PauseBefore,
            CssNameIndex.Unknown,
            CssNameIndex.ScrollbarFaceColor,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.PlayDuring,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.WordBreak,
            CssNameIndex.BorderBottomColor,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.MarginRight,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.SpeakNumeral,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.TextJustify,
            CssNameIndex.PaddingRight,
            CssNameIndex.BorderRightStyle,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.CounterIncrement,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.TextUnderlinePosition,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.WordSpacing,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Background,
            CssNameIndex.Unknown,
            CssNameIndex.OverflowX,
            CssNameIndex.BorderWidth,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.ZIndex,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.MaxWidth,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.ScrollbarDarkshadowColor,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.CueAfter,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.SpeakHeader,
            CssNameIndex.Unknown,
            CssNameIndex.Direction,
            CssNameIndex.FontVariant,
            CssNameIndex.Unknown,
            CssNameIndex.Richness,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Font,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Outline,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderRightColor,
            CssNameIndex.Unknown,
            CssNameIndex.FontStyle,
            CssNameIndex.MarginTop,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderLeft,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.ListStylePosition,
            CssNameIndex.Unknown,
            CssNameIndex.BorderLeftWidth,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.PaddingBottom,
            CssNameIndex.Unknown,
            CssNameIndex.LayoutGridType,
            CssNameIndex.PageBreakAfter,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.FontSize,
            CssNameIndex.Unknown,
            CssNameIndex.Position,
            CssNameIndex.BorderLeftStyle,
            CssNameIndex.PaddingLeft,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Right,
            CssNameIndex.PauseAfter,
            CssNameIndex.MaxHeight,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.LetterSpacing,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.Unknown,
            CssNameIndex.BorderTop
        };

        public static NameDef[] names = new NameDef[]
        {
            new NameDef(0, null, CssNameIndex.Unknown),
            new NameDef(0, "scrollbar-arrow-color", CssNameIndex.ScrollbarArrowColor),
            new NameDef(2, "white-space", CssNameIndex.WhiteSpace),
            new NameDef(4, "line-break", CssNameIndex.LineBreak),
            new NameDef(5, "orphans", CssNameIndex.Orphans),
            new NameDef(10, "writing-mode", CssNameIndex.WritingMode),
            new NameDef(12, "scrollbar-3dlight-color", CssNameIndex.Scrollbar3dLightColor),
            new NameDef(14, "text-autospace", CssNameIndex.TextAutospace),
            new NameDef(15, "vertical-align", CssNameIndex.VerticalAlign),
            new NameDef(18, "border-right", CssNameIndex.BorderRight),
            new NameDef(21, "bottom", CssNameIndex.Bottom),
            new NameDef(21, "font-family", CssNameIndex.FontFamily),
            new NameDef(22, "line-height", CssNameIndex.LineHeight),
            new NameDef(25, "border-bottom", CssNameIndex.BorderBottom),
            new NameDef(32, "scrollbar-base-color", CssNameIndex.ScrollbarBaseColor),
            new NameDef(33, "min-width", CssNameIndex.MinWidth),
            new NameDef(34, "background-color", CssNameIndex.BackgroundColor),
            new NameDef(36, "border-top-style", CssNameIndex.BorderTopStyle),
            new NameDef(38, "empty-cells", CssNameIndex.EmptyCells),
            new NameDef(40, "list-style-type", CssNameIndex.ListStyleType),
            new NameDef(45, "text-align", CssNameIndex.TextAlign),
            new NameDef(47, "font-weight", CssNameIndex.FontWeight),
            new NameDef(49, "outline-width", CssNameIndex.OutlineWidth),
            new NameDef(50, "caption-side", CssNameIndex.CaptionSide),
            new NameDef(51, "scrollbar-shadow-color", CssNameIndex.ScrollbarShadowColor),
            new NameDef(55, "clip", CssNameIndex.Clip),
            new NameDef(55, "volume", CssNameIndex.Volume),
            new NameDef(57, "margin-left", CssNameIndex.MarginLeft),
            new NameDef(58, "border-top-width", CssNameIndex.BorderTopWidth),
            new NameDef(61, "azimuth", CssNameIndex.Azimuth),
            new NameDef(61, "unicode-bidi", CssNameIndex.UnicodeBidi),
            new NameDef(62, "float", CssNameIndex.Float),
            new NameDef(66, "layout-flow", CssNameIndex.LayoutFlow),
            new NameDef(67, "min-height", CssNameIndex.MinHeight),
            new NameDef(68, "content", CssNameIndex.Content),
            new NameDef(70, "padding", CssNameIndex.Padding),
            new NameDef(71, "border-bottom-width", CssNameIndex.BorderBottomWidth),
            new NameDef(74, "visibility", CssNameIndex.Visibility),
            new NameDef(76, "overflow", CssNameIndex.Overflow),
            new NameDef(76, "table-layout", CssNameIndex.TableLayout),
            new NameDef(77, "border-left-color", CssNameIndex.BorderLeftColor),
            new NameDef(80, "pitch", CssNameIndex.Pitch),
            new NameDef(81, "pause", CssNameIndex.Pause),
            new NameDef(89, "overflow-y", CssNameIndex.OverflowY),
            new NameDef(93, "scrollbar-highlight-color", CssNameIndex.ScrollbarHighlightColor),
            new NameDef(95, "height", CssNameIndex.Height),
            new NameDef(97, "word-wrap", CssNameIndex.WordWrap),
            new NameDef(104, "top", CssNameIndex.Top),
            new NameDef(105, "list-style", CssNameIndex.ListStyle),
            new NameDef(107, "margin", CssNameIndex.Margin),
            new NameDef(109, "text-kashida-space", CssNameIndex.TextKashidaSpace),
            new NameDef(110, "voice-family", CssNameIndex.VoiceFamily),
            new NameDef(111, "cue-before", CssNameIndex.CueBefore),
            new NameDef(112, "clear", CssNameIndex.Clear),
            new NameDef(116, "text-overflow", CssNameIndex.TextOverflow),
            new NameDef(125, "border-bottom-style", CssNameIndex.BorderBottomStyle),
            new NameDef(128, "border-color", CssNameIndex.BorderColor),
            new NameDef(129, "text-decoration", CssNameIndex.TextDecoration),
            new NameDef(130, "display", CssNameIndex.Display),
            new NameDef(136, "counter-reset", CssNameIndex.CounterReset),
            new NameDef(137, "margin-bottom", CssNameIndex.MarginBottom),
            new NameDef(138, "border-style", CssNameIndex.BorderStyle),
            new NameDef(142, "layout-grid", CssNameIndex.LayoutGrid),
            new NameDef(143, "quotes", CssNameIndex.Quotes),
            new NameDef(147, "accelerator", CssNameIndex.Accelerator),
            new NameDef(148, "border", CssNameIndex.Border),
            new NameDef(151, "zoom", CssNameIndex.Zoom),
            new NameDef(154, "outline-style", CssNameIndex.OutlineStyle),
            new NameDef(156, "width", CssNameIndex.Width),
            new NameDef(158, "color", CssNameIndex.Color),
            new NameDef(163, "page-break-inside", CssNameIndex.PageBreakInside),
            new NameDef(165, "pitch-range", CssNameIndex.PitchRange),
            new NameDef(166, "border-collapse", CssNameIndex.BorderCollapse),
            new NameDef(166, "speak", CssNameIndex.Speak),
            new NameDef(167, "cue", CssNameIndex.Cue),
            new NameDef(169, "left", CssNameIndex.Left),
            new NameDef(170, "layout-grid-mode", CssNameIndex.LayoutGridMode),
            new NameDef(173, "speak-punctuation", CssNameIndex.SpeakPunctuation),
            new NameDef(174, "layout-grid-line", CssNameIndex.LayoutGridLine),
            new NameDef(179, "border-spacing", CssNameIndex.BorderSpacing),
            new NameDef(181, "text-transform", CssNameIndex.TextTransform),
            new NameDef(185, "border-right-width", CssNameIndex.BorderRightWidth),
            new NameDef(186, "page-break-before", CssNameIndex.PageBreakBefore),
            new NameDef(187, "text-indent", CssNameIndex.TextIndent),
            new NameDef(188, "layout-grid-char", CssNameIndex.LayoutGridChar),
            new NameDef(189, "speech-rate", CssNameIndex.SpeechRate),
            new NameDef(190, "pause-before", CssNameIndex.PauseBefore),
            new NameDef(192, "scrollbar-face-color", CssNameIndex.ScrollbarFaceColor),
            new NameDef(196, "play-during", CssNameIndex.PlayDuring),
            new NameDef(199, "word-break", CssNameIndex.WordBreak),
            new NameDef(200, "border-bottom-color", CssNameIndex.BorderBottomColor),
            new NameDef(208, "margin-right", CssNameIndex.MarginRight),
            new NameDef(211, "speak-numeral", CssNameIndex.SpeakNumeral),
            new NameDef(216, "text-justify", CssNameIndex.TextJustify),
            new NameDef(217, "padding-right", CssNameIndex.PaddingRight),
            new NameDef(218, "border-right-style", CssNameIndex.BorderRightStyle),
            new NameDef(221, "counter-increment", CssNameIndex.CounterIncrement),
            new NameDef(227, "text-underline-position", CssNameIndex.TextUnderlinePosition),
            new NameDef(233, "word-spacing", CssNameIndex.WordSpacing),
            new NameDef(236, "background", CssNameIndex.Background),
            new NameDef(238, "overflow-x", CssNameIndex.OverflowX),
            new NameDef(239, "border-width", CssNameIndex.BorderWidth),
            new NameDef(239, "widows", CssNameIndex.Widows),
            new NameDef(245, "z-index", CssNameIndex.ZIndex),
            new NameDef(245, "border-top-color", CssNameIndex.BorderTopColor),
            new NameDef(252, "max-width", CssNameIndex.MaxWidth),
            new NameDef(257, "scrollbar-darkshadow-color", CssNameIndex.ScrollbarDarkshadowColor),
            new NameDef(261, "cue-after", CssNameIndex.CueAfter),
            new NameDef(269, "speak-header", CssNameIndex.SpeakHeader),
            new NameDef(271, "direction", CssNameIndex.Direction),
            new NameDef(272, "font-variant", CssNameIndex.FontVariant),
            new NameDef(274, "richness", CssNameIndex.Richness),
            new NameDef(274, "stress", CssNameIndex.Stress),
            new NameDef(281, "font", CssNameIndex.Font),
            new NameDef(281, "elevation", CssNameIndex.Elevation),
            new NameDef(285, "outline", CssNameIndex.Outline),
            new NameDef(289, "border-right-color", CssNameIndex.BorderRightColor),
            new NameDef(291, "font-style", CssNameIndex.FontStyle),
            new NameDef(292, "margin-top", CssNameIndex.MarginTop),
            new NameDef(295, "border-left", CssNameIndex.BorderLeft),
            new NameDef(298, "list-style-position", CssNameIndex.ListStylePosition),
            new NameDef(298, "outline-color", CssNameIndex.OutlineColor),
            new NameDef(300, "border-left-width", CssNameIndex.BorderLeftWidth),
            new NameDef(305, "padding-bottom", CssNameIndex.PaddingBottom),
            new NameDef(307, "layout-grid-type", CssNameIndex.LayoutGridType),
            new NameDef(308, "page-break-after", CssNameIndex.PageBreakAfter),
            new NameDef(311, "font-size", CssNameIndex.FontSize),
            new NameDef(313, "position", CssNameIndex.Position),
            new NameDef(314, "border-left-style", CssNameIndex.BorderLeftStyle),
            new NameDef(314, "padding-top", CssNameIndex.PaddingTop),
            new NameDef(315, "padding-left", CssNameIndex.PaddingLeft),
            new NameDef(318, "right", CssNameIndex.Right),
            new NameDef(319, "pause-after", CssNameIndex.PauseAfter),
            new NameDef(320, "max-height", CssNameIndex.MaxHeight),
            new NameDef(323, "letter-spacing", CssNameIndex.LetterSpacing),
            new NameDef(328, "border-top", CssNameIndex.BorderTop),
            new NameDef(329, null, CssNameIndex.Unknown)
        };

        public static FilterActionEntry[] filterInstructions = new FilterActionEntry[]
        {
            new FilterActionEntry(FilterAction.Drop),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.CheckContent),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.CheckContent),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Keep),
            new FilterActionEntry(FilterAction.Drop)
        };
    }
}
