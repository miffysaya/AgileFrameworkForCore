namespace AgileFramework.Security.Exchange.Data.TextConverters.Internal.Html
{
    internal static class HtmlNameData
    {
        public struct NameDef
        {
            public short hash;

            public bool literalTag;

            public bool literalEnt;

            public string name;

            public HtmlTagIndex tagIndex;

            public HtmlTagId publicTagId;

            public HtmlAttributeId publicAttributeId;

            public NameDef(short hash, string name, HtmlTagId publicTagId, HtmlAttributeId publicAttributeId)
            {
                this = new NameDef(hash, name, HtmlTagIndex.Unknown, false, false, publicTagId, publicAttributeId);
            }

            public NameDef(short hash, string name, HtmlTagIndex tagIndex, bool literalTag, bool literalEnt, HtmlTagId publicTagId, HtmlAttributeId publicAttributeId)
            {
                this.hash = hash;
                this.literalTag = literalTag;
                this.literalEnt = literalEnt;
                this.name = name;
                this.tagIndex = tagIndex;
                this.publicTagId = publicTagId;
                this.publicAttributeId = publicAttributeId;
            }
        }

        public struct EntityDef
        {
            public short hash;

            public short value;

            public string name;

            public EntityDef(short hash, short value, string name)
            {
                this.hash = hash;
                this.name = name;
                this.value = value;
            }
        }

        public const short MAX_NAME = 14;

        public const short MAX_TAG_NAME = 14;

        public const short MAX_ENTITY_NAME = 8;

        public const short NAME_HASH_SIZE = 601;

        public const int NAME_HASH_MODIFIER = 221;

        public const short ENTITY_HASH_SIZE = 705;

        public const int ENTITY_HASH_MODIFIER = 230;

        public static HtmlNameIndex[] nameHashTable;

        public static HtmlEntityIndex[] entityHashTable;

        public static NameDef[] names;

        public static EntityDef[] entities;

        static HtmlNameData()
        {
            // 注意: 此类型已标记为 'beforefieldinit'.
            HtmlNameIndex[] array = new HtmlNameIndex[601];
            array[1] = HtmlNameIndex.Nofill;
            array[2] = HtmlNameIndex.Comment;
            array[3] = HtmlNameIndex.LI;
            array[4] = HtmlNameIndex.Version;
            array[8] = HtmlNameIndex.CellSpacing;
            array[10] = HtmlNameIndex.Kbd;
            array[14] = HtmlNameIndex.Scheme;
            array[15] = HtmlNameIndex.Multiple;
            array[21] = HtmlNameIndex.Ruby;
            array[23] = HtmlNameIndex.Code;
            array[24] = HtmlNameIndex.NoResize;
            array[25] = HtmlNameIndex.Alt;
            array[27] = HtmlNameIndex.HrefLang;
            array[30] = HtmlNameIndex.FlushRight;
            array[38] = HtmlNameIndex.Accept;
            array[45] = HtmlNameIndex.FrameBorder;
            array[52] = HtmlNameIndex.Shape;
            array[55] = HtmlNameIndex.Param;
            array[56] = HtmlNameIndex.Acronym;
            array[58] = HtmlNameIndex.For;
            array[59] = HtmlNameIndex.Color;
            array[63] = HtmlNameIndex.A;
            array[66] = HtmlNameIndex._Pxml;
            array[68] = HtmlNameIndex.Face;
            array[72] = HtmlNameIndex.RowSpan;
            array[73] = HtmlNameIndex.NoWrap;
            array[74] = HtmlNameIndex.Ins;
            array[85] = HtmlNameIndex.RP;
            array[86] = HtmlNameIndex.Script;
            array[88] = HtmlNameIndex.Char;
            array[100] = HtmlNameIndex.BGColor;
            array[123] = HtmlNameIndex.Style;
            array[126] = HtmlNameIndex.Width;
            array[128] = HtmlNameIndex.Headers;
            array[130] = HtmlNameIndex.Map;
            array[132] = HtmlNameIndex.Data;
            array[135] = HtmlNameIndex.Sub;
            array[136] = HtmlNameIndex.H2;
            array[137] = HtmlNameIndex.Image;
            array[141] = HtmlNameIndex.StandBy;
            array[143] = HtmlNameIndex.Select;
            array[145] = HtmlNameIndex.Profile;
            array[155] = HtmlNameIndex.Button;
            array[162] = HtmlNameIndex.Meta;
            array[166] = HtmlNameIndex.Rules;
            array[167] = HtmlNameIndex.Class;
            array[170] = HtmlNameIndex.Src;
            array[171] = HtmlNameIndex.Legend;
            array[177] = HtmlNameIndex.Scrolling;
            array[180] = HtmlNameIndex.Vlink;
            array[181] = HtmlNameIndex.Del;
            array[187] = HtmlNameIndex.Hspace;
            array[190] = HtmlNameIndex.Charset;
            array[196] = HtmlNameIndex.RT;
            array[198] = HtmlNameIndex.Italic;
            array[201] = HtmlNameIndex.Div;
            array[205] = HtmlNameIndex.Dir;
            array[206] = HtmlNameIndex.TT;
            array[208] = HtmlNameIndex.H6;
            array[209] = HtmlNameIndex.ValueType;
            array[210] = HtmlNameIndex.Declare;
            array[212] = HtmlNameIndex.Size;
            array[214] = HtmlNameIndex.FrameSet;
            array[216] = HtmlNameIndex.ReadOnly;
            array[218] = HtmlNameIndex.Language;
            array[220] = HtmlNameIndex.Area;
            array[222] = HtmlNameIndex.TopMargin;
            array[223] = HtmlNameIndex.NoEmbed;
            array[224] = HtmlNameIndex.BaseFont;
            array[229] = HtmlNameIndex.NoFrames;
            array[232] = HtmlNameIndex.Border;
            array[233] = HtmlNameIndex.Center;
            array[237] = HtmlNameIndex.Height;
            array[240] = HtmlNameIndex.Underline;
            array[242] = HtmlNameIndex.FlushBoth;
            array[244] = HtmlNameIndex.BGSound;
            array[249] = HtmlNameIndex.Var;
            array[252] = HtmlNameIndex.TD;
            array[253] = HtmlNameIndex.Id;
            array[254] = HtmlNameIndex.Rows;
            array[255] = HtmlNameIndex.H4;
            array[257] = HtmlNameIndex.Abbr;
            array[258] = HtmlNameIndex.HttpEquiv;
            array[259] = HtmlNameIndex.Span;
            array[268] = HtmlNameIndex.DD;
            array[271] = HtmlNameIndex.Address;
            array[273] = HtmlNameIndex.Applet;
            array[274] = HtmlNameIndex.Rel;
            array[278] = HtmlNameIndex.TextArea;
            array[279] = HtmlNameIndex.Tbody;
            array[283] = HtmlNameIndex.ParaIndent;
            array[286] = HtmlNameIndex.DT;
            array[292] = HtmlNameIndex.Checked;
            array[293] = HtmlNameIndex.Head;
            array[296] = HtmlNameIndex.Rev;
            array[297] = HtmlNameIndex.Small;
            array[299] = HtmlNameIndex.Cite;
            array[305] = HtmlNameIndex.Cols;
            array[306] = HtmlNameIndex.Sup;
            array[314] = HtmlNameIndex.Fixed;
            array[315] = HtmlNameIndex.Prompt;
            array[320] = HtmlNameIndex.Disabled;
            array[322] = HtmlNameIndex.Name;
            array[323] = HtmlNameIndex.Summary;
            array[324] = HtmlNameIndex.Object;
            array[331] = HtmlNameIndex.Label;
            array[332] = HtmlNameIndex.Content;
            array[333] = HtmlNameIndex.Target;
            array[340] = HtmlNameIndex.EM;
            array[344] = HtmlNameIndex.Clear;
            array[354] = HtmlNameIndex.Scope;
            array[356] = HtmlNameIndex.Compact;
            array[358] = HtmlNameIndex.Blink;
            array[372] = HtmlNameIndex.Selected;
            array[374] = HtmlNameIndex.MaxLength;
            array[380] = HtmlNameIndex.Frame;
            array[381] = HtmlNameIndex.Thead;
            array[389] = HtmlNameIndex.TabIndex;
            array[395] = HtmlNameIndex._Import;
            array[405] = HtmlNameIndex.Embed;
            array[406] = HtmlNameIndex.TH;
            array[407] = HtmlNameIndex.Caption;
            array[413] = HtmlNameIndex.Value;
            array[420] = HtmlNameIndex.Smaller;
            array[424] = HtmlNameIndex.DateTime;
            array[426] = HtmlNameIndex.ClassId;
            array[432] = HtmlNameIndex.Bold;
            array[433] = HtmlNameIndex.Strike;
            array[447] = HtmlNameIndex.FlushLeft;
            array[448] = HtmlNameIndex.NoShade;
            array[449] = HtmlNameIndex.LeftMargin;
            array[450] = HtmlNameIndex.Title;
            array[452] = HtmlNameIndex.Excerpt;
            array[454] = HtmlNameIndex.CellPadding;
            array[458] = HtmlNameIndex.Dfn;
            array[459] = HtmlNameIndex.CharOff;
            array[461] = HtmlNameIndex.IsIndex;
            array[462] = HtmlNameIndex.Tfoot;
            array[464] = HtmlNameIndex.NoBR;
            array[470] = HtmlNameIndex.Lang;
            array[472] = HtmlNameIndex.OptGroup;
            array[474] = HtmlNameIndex.AcceptCharset;
            array[476] = HtmlNameIndex.Big;
            array[477] = HtmlNameIndex.Font;
            array[479] = HtmlNameIndex.Type;
            array[482] = HtmlNameIndex.Href;
            array[484] = HtmlNameIndex.Img;
            array[486] = HtmlNameIndex.Vspace;
            array[487] = HtmlNameIndex.H3;
            array[493] = HtmlNameIndex.Align;
            array[497] = HtmlNameIndex.Wbr;
            array[499] = HtmlNameIndex.AccessKey;
            array[502] = HtmlNameIndex.Col;
            array[504] = HtmlNameIndex.Menu;
            array[506] = HtmlNameIndex.CodeBase;
            array[508] = HtmlNameIndex.Strong;
            array[510] = HtmlNameIndex.BR;
            array[512] = HtmlNameIndex.Archive;
            array[514] = HtmlNameIndex.UL;
            array[516] = HtmlNameIndex.NoScript;
            array[517] = HtmlNameIndex.PlainText;
            array[521] = HtmlNameIndex.Base;
            array[522] = HtmlNameIndex.Defer;
            array[523] = HtmlNameIndex.Body;
            array[524] = HtmlNameIndex.OL;
            array[526] = HtmlNameIndex.H1;
            array[528] = HtmlNameIndex.Valign;
            array[531] = HtmlNameIndex.Media;
            array[532] = HtmlNameIndex.Iframe;
            array[533] = HtmlNameIndex.DL;
            array[535] = HtmlNameIndex.ColSpan;
            array[538] = HtmlNameIndex.Axis;
            array[542] = HtmlNameIndex.MarginHeight;
            array[543] = HtmlNameIndex.Alink;
            array[545] = HtmlNameIndex._Xml_Namespace;
            array[546] = HtmlNameIndex.Method;
            array[549] = HtmlNameIndex.FontFamily;
            array[554] = HtmlNameIndex.FieldSet;
            array[556] = HtmlNameIndex.Pre;
            array[557] = HtmlNameIndex.Table;
            array[560] = HtmlNameIndex.TR;
            array[562] = HtmlNameIndex.Samp;
            array[563] = HtmlNameIndex.Link;
            array[564] = HtmlNameIndex.HR;
            array[568] = HtmlNameIndex.Form;
            array[569] = HtmlNameIndex.Input;
            array[570] = HtmlNameIndex.Xml;
            array[572] = HtmlNameIndex.UseMap;
            array[574] = HtmlNameIndex.Xmp;
            array[575] = HtmlNameIndex.CodeType;
            array[580] = HtmlNameIndex.MarginWidth;
            array[584] = HtmlNameIndex.Q;
            array[585] = HtmlNameIndex.ColGroup;
            array[586] = HtmlNameIndex.S;
            array[587] = HtmlNameIndex.P;
            array[588] = HtmlNameIndex.U;
            array[589] = HtmlNameIndex.Action;
            array[590] = HtmlNameIndex.EncType;
            array[592] = HtmlNameIndex.I;
            array[597] = HtmlNameIndex.B;
            array[598] = HtmlNameIndex.H5;
            array[599] = HtmlNameIndex.Background;
            nameHashTable = array;
            HtmlEntityIndex[] array2 = new HtmlEntityIndex[705];
            array2[1] = HtmlEntityIndex.omega;
            array2[2] = HtmlEntityIndex.rle;
            array2[5] = HtmlEntityIndex.Oacute;
            array2[8] = HtmlEntityIndex.fnof;
            array2[9] = HtmlEntityIndex.Oslash;
            array2[12] = HtmlEntityIndex.Ntilde;
            array2[14] = HtmlEntityIndex.larr;
            array2[15] = HtmlEntityIndex.psi;
            array2[20] = HtmlEntityIndex.Pi;
            array2[22] = HtmlEntityIndex.micro;
            array2[24] = HtmlEntityIndex.piv;
            array2[26] = HtmlEntityIndex.upsih;
            array2[28] = HtmlEntityIndex.Xi;
            array2[29] = HtmlEntityIndex.aring;
            array2[30] = HtmlEntityIndex.ni;
            array2[32] = HtmlEntityIndex.cap;
            array2[33] = HtmlEntityIndex.iuml;
            array2[34] = HtmlEntityIndex.chi;
            array2[38] = HtmlEntityIndex.frac14;
            array2[40] = HtmlEntityIndex.frac34;
            array2[41] = HtmlEntityIndex.ordm;
            array2[44] = HtmlEntityIndex.and;
            array2[47] = HtmlEntityIndex.brvbar;
            array2[49] = HtmlEntityIndex.zwsp;
            array2[50] = HtmlEntityIndex.forall;
            array2[52] = HtmlEntityIndex.pi;
            array2[53] = HtmlEntityIndex.otimes;
            array2[54] = HtmlEntityIndex.uacute;
            array2[55] = HtmlEntityIndex.ang;
            array2[56] = HtmlEntityIndex.iexcl;
            array2[57] = HtmlEntityIndex.lrm;
            array2[60] = HtmlEntityIndex.xi;
            array2[65] = HtmlEntityIndex.lre;
            array2[66] = HtmlEntityIndex.zwj;
            array2[68] = HtmlEntityIndex.Nu;
            array2[69] = HtmlEntityIndex.Mu;
            array2[71] = HtmlEntityIndex.lro;
            array2[73] = HtmlEntityIndex.COPY;
            array2[74] = HtmlEntityIndex.nsub;
            array2[83] = HtmlEntityIndex.thorn;
            array2[85] = HtmlEntityIndex.sum;
            array2[87] = HtmlEntityIndex.rsquo;
            array2[88] = HtmlEntityIndex.middot;
            array2[97] = HtmlEntityIndex.Ecirc;
            array2[98] = HtmlEntityIndex.thinsp;
            array2[100] = HtmlEntityIndex.times;
            array2[101] = HtmlEntityIndex.mu;
            array2[102] = HtmlEntityIndex.yen;
            array2[107] = HtmlEntityIndex.prod;
            array2[116] = HtmlEntityIndex.dArr;
            array2[118] = HtmlEntityIndex.euml;
            array2[122] = HtmlEntityIndex.Beta;
            array2[123] = HtmlEntityIndex.radic;
            array2[130] = HtmlEntityIndex.hearts;
            array2[131] = HtmlEntityIndex.TRADE;
            array2[134] = HtmlEntityIndex.rsaquo;
            array2[136] = HtmlEntityIndex.Auml;
            array2[137] = HtmlEntityIndex.ugrave;
            array2[142] = HtmlEntityIndex.ccedil;
            array2[143] = HtmlEntityIndex.OElig;
            array2[144] = HtmlEntityIndex.sect;
            array2[146] = HtmlEntityIndex.there4;
            array2[148] = HtmlEntityIndex.REG;
            array2[155] = HtmlEntityIndex.plusmn;
            array2[158] = HtmlEntityIndex.thetasym;
            array2[161] = HtmlEntityIndex.rArr;
            array2[162] = HtmlEntityIndex.iota;
            array2[163] = HtmlEntityIndex.rceil;
            array2[164] = HtmlEntityIndex.empty;
            array2[165] = HtmlEntityIndex.Phi;
            array2[166] = HtmlEntityIndex.Gamma;
            array2[167] = HtmlEntityIndex.ass;
            array2[171] = HtmlEntityIndex.lt;
            array2[177] = HtmlEntityIndex.Zeta;
            array2[179] = HtmlEntityIndex.Ograve;
            array2[187] = HtmlEntityIndex.spades;
            array2[193] = HtmlEntityIndex.zwnj;
            array2[196] = HtmlEntityIndex.delta;
            array2[200] = HtmlEntityIndex.reg;
            array2[203] = HtmlEntityIndex.isin;
            array2[204] = HtmlEntityIndex.Alpha;
            array2[207] = HtmlEntityIndex.Yuml;
            array2[211] = HtmlEntityIndex.cedil;
            array2[218] = HtmlEntityIndex.rfloor;
            array2[220] = HtmlEntityIndex.divide;
            array2[222] = HtmlEntityIndex.Omicron;
            array2[225] = HtmlEntityIndex.ordf;
            array2[227] = HtmlEntityIndex.clubs;
            array2[228] = HtmlEntityIndex.Uuml;
            array2[229] = HtmlEntityIndex.Eta;
            array2[231] = HtmlEntityIndex.Acirc;
            array2[232] = HtmlEntityIndex.Atilde;
            array2[233] = HtmlEntityIndex.Rho;
            array2[234] = HtmlEntityIndex.alefsym;
            array2[240] = HtmlEntityIndex.AElig;
            array2[248] = HtmlEntityIndex.hArr;
            array2[250] = HtmlEntityIndex.oline;
            array2[254] = HtmlEntityIndex.Aacute;
            array2[255] = HtmlEntityIndex.Ccedil;
            array2[256] = HtmlEntityIndex.theta;
            array2[257] = HtmlEntityIndex.or;
            array2[258] = HtmlEntityIndex.Egrave;
            array2[259] = HtmlEntityIndex.trade;
            array2[260] = HtmlEntityIndex.Int;
            array2[262] = HtmlEntityIndex.sup1;
            array2[263] = HtmlEntityIndex.phi;
            array2[265] = HtmlEntityIndex.cup;
            array2[268] = HtmlEntityIndex.equiv;
            array2[272] = HtmlEntityIndex.sim;
            array2[273] = HtmlEntityIndex.Yacute;
            array2[276] = HtmlEntityIndex.Prime;
            array2[277] = HtmlEntityIndex.uarr;
            array2[280] = HtmlEntityIndex.mdash;
            array2[281] = HtmlEntityIndex.acute;
            array2[285] = HtmlEntityIndex.ETH;
            array2[287] = HtmlEntityIndex.eacute;
            array2[290] = HtmlEntityIndex.weierp;
            array2[292] = HtmlEntityIndex.Kappa;
            array2[294] = HtmlEntityIndex.Theta;
            array2[304] = HtmlEntityIndex.pound;
            array2[307] = HtmlEntityIndex.rarr;
            array2[312] = HtmlEntityIndex.oelig;
            array2[313] = HtmlEntityIndex.sup;
            array2[314] = HtmlEntityIndex.igrave;
            array2[316] = HtmlEntityIndex.cent;
            array2[319] = HtmlEntityIndex.agrave;
            array2[321] = HtmlEntityIndex.dagger;
            array2[328] = HtmlEntityIndex.bull;
            array2[330] = HtmlEntityIndex.Ouml;
            array2[335] = HtmlEntityIndex.Aring;
            array2[338] = HtmlEntityIndex.loz;
            array2[345] = HtmlEntityIndex.lowast;
            array2[346] = HtmlEntityIndex.otilde;
            array2[351] = HtmlEntityIndex.euro;
            array2[352] = HtmlEntityIndex.uml;
            array2[354] = HtmlEntityIndex.sigma;
            array2[373] = HtmlEntityIndex.Epsilon;
            array2[376] = HtmlEntityIndex.lsaquo;
            array2[379] = HtmlEntityIndex.image;
            array2[380] = HtmlEntityIndex.lArr;
            array2[384] = HtmlEntityIndex.Iuml;
            array2[385] = HtmlEntityIndex.Chi;
            array2[390] = HtmlEntityIndex.eta;
            array2[394] = HtmlEntityIndex.harr;
            array2[397] = HtmlEntityIndex.aacute;
            array2[399] = HtmlEntityIndex.nads;
            array2[401] = HtmlEntityIndex.eth;
            array2[402] = HtmlEntityIndex.GT;
            array2[404] = HtmlEntityIndex.Sigma;
            array2[408] = HtmlEntityIndex.oslash;
            array2[409] = HtmlEntityIndex.aelig;
            array2[411] = HtmlEntityIndex.notin;
            array2[415] = HtmlEntityIndex.aafs;
            array2[416] = HtmlEntityIndex.yacute;
            array2[427] = HtmlEntityIndex.laquo;
            array2[431] = HtmlEntityIndex.prime;
            array2[432] = HtmlEntityIndex.Agrave;
            array2[433] = HtmlEntityIndex.lambda;
            array2[436] = HtmlEntityIndex.oplus;
            array2[442] = HtmlEntityIndex.real;
            array2[443] = HtmlEntityIndex.Ugrave;
            array2[448] = HtmlEntityIndex.supe;
            array2[450] = HtmlEntityIndex.para;
            array2[455] = HtmlEntityIndex.darr;
            array2[466] = HtmlEntityIndex.sube;
            array2[468] = HtmlEntityIndex.asymp;
            array2[469] = HtmlEntityIndex.Euml;
            array2[470] = HtmlEntityIndex.ocirc;
            array2[471] = HtmlEntityIndex.Lambda;
            array2[476] = HtmlEntityIndex.beta;
            array2[477] = HtmlEntityIndex.sup3;
            array2[486] = HtmlEntityIndex.infin;
            array2[487] = HtmlEntityIndex.Upsilon;
            array2[488] = HtmlEntityIndex.ucirc;
            array2[490] = HtmlEntityIndex.Delta;
            array2[506] = HtmlEntityIndex.Iacute;
            array2[513] = HtmlEntityIndex.Iota;
            array2[516] = HtmlEntityIndex.tilde;
            array2[517] = HtmlEntityIndex.bdquo;
            array2[520] = HtmlEntityIndex.scaron;
            array2[529] = HtmlEntityIndex.Tau;
            array2[531] = HtmlEntityIndex.zeta;
            array2[533] = HtmlEntityIndex.pdf;
            array2[537] = HtmlEntityIndex.perp;
            array2[546] = HtmlEntityIndex.shy;
            array2[547] = HtmlEntityIndex.icirc;
            array2[561] = HtmlEntityIndex.yuml;
            array2[565] = HtmlEntityIndex.gamma;
            array2[567] = HtmlEntityIndex.sigmaf;
            array2[569] = HtmlEntityIndex.rang;
            array2[572] = HtmlEntityIndex.crarr;
            array2[575] = HtmlEntityIndex.raquo;
            array2[577] = HtmlEntityIndex.minus;
            array2[578] = HtmlEntityIndex.ograve;
            array2[582] = HtmlEntityIndex.uuml;
            array2[583] = HtmlEntityIndex.Ocirc;
            array2[585] = HtmlEntityIndex.nods;
            array2[586] = HtmlEntityIndex.ldquo;
            array2[587] = HtmlEntityIndex.rho;
            array2[588] = HtmlEntityIndex.szlig;
            array2[601] = HtmlEntityIndex.Ucirc;
            array2[603] = HtmlEntityIndex.alpha;
            array2[606] = HtmlEntityIndex.quot;
            array2[615] = HtmlEntityIndex.Dagger;
            array2[616] = HtmlEntityIndex.Uacute;
            array2[620] = HtmlEntityIndex.Igrave;
            array2[622] = HtmlEntityIndex.Psi;
            array2[627] = HtmlEntityIndex.tau;
            array2[628] = HtmlEntityIndex.lsquo;
            array2[629] = HtmlEntityIndex.ge;
            array2[631] = HtmlEntityIndex.atilde;
            array2[632] = HtmlEntityIndex.nabla;
            array2[633] = HtmlEntityIndex.Scaron;
            array2[634] = HtmlEntityIndex.cong;
            array2[635] = HtmlEntityIndex.frasl;
            array2[638] = HtmlEntityIndex.ne;
            array2[639] = HtmlEntityIndex.curren;
            array2[640] = HtmlEntityIndex.le;
            array2[643] = HtmlEntityIndex.uArr;
            array2[648] = HtmlEntityIndex.epsilon;
            array2[649] = HtmlEntityIndex.iacute;
            array2[651] = HtmlEntityIndex.rlm;
            array2[652] = HtmlEntityIndex.Otilde;
            array2[653] = HtmlEntityIndex.sdot;
            array2[654] = HtmlEntityIndex.ndash;
            array2[657] = HtmlEntityIndex.egrave;
            array2[660] = HtmlEntityIndex.Icirc;
            array2[662] = HtmlEntityIndex.part;
            array2[663] = HtmlEntityIndex.diams;
            array2[664] = HtmlEntityIndex.copy;
            array2[667] = HtmlEntityIndex.ntilde;
            array2[670] = HtmlEntityIndex.rdquo;
            array2[671] = HtmlEntityIndex.iquest;
            array2[672] = HtmlEntityIndex.sbquo;
            array2[678] = HtmlEntityIndex.not;
            array2[679] = HtmlEntityIndex.iafs;
            array2[684] = HtmlEntityIndex.ouml;
            array2[689] = HtmlEntityIndex.ecirc;
            array2[691] = HtmlEntityIndex.kappa;
            array2[696] = HtmlEntityIndex.ensp;
            array2[699] = HtmlEntityIndex.prop;
            array2[702] = HtmlEntityIndex.omicron;
            array2[703] = HtmlEntityIndex.hellip;
            entityHashTable = array2;
            names = new NameDef[]
            {
                new NameDef(0, null, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex._COMMENT, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex._CONDITIONAL, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex._BANG, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex._ASP, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex._DTD, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(0, null, HtmlTagIndex.Unknown, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(1, "nofill", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(2, "comment", HtmlTagIndex.Comment, true, true, HtmlTagId.Comment, HtmlAttributeId.Unknown),
                new NameDef(3, "li", HtmlTagIndex.LI, false, false, HtmlTagId.LI, HtmlAttributeId.Unknown),
                new NameDef(4, "version", HtmlTagId.Unknown, HtmlAttributeId.Version),
                new NameDef(8, "cellspacing", HtmlTagId.Unknown, HtmlAttributeId.CellSpacing),
                new NameDef(10, "kbd", HtmlTagIndex.Kbd, false, false, HtmlTagId.Kbd, HtmlAttributeId.Unknown),
                new NameDef(14, "scheme", HtmlTagId.Unknown, HtmlAttributeId.Scheme),
                new NameDef(15, "multiple", HtmlTagId.Unknown, HtmlAttributeId.Multiple),
                new NameDef(21, "ruby", HtmlTagIndex.Ruby, false, false, HtmlTagId.Ruby, HtmlAttributeId.Unknown),
                new NameDef(23, "code", HtmlTagIndex.Code, false, false, HtmlTagId.Code, HtmlAttributeId.Code),
                new NameDef(24, "noresize", HtmlTagId.Unknown, HtmlAttributeId.NoResize),
                new NameDef(25, "alt", HtmlTagId.Unknown, HtmlAttributeId.Alt),
                new NameDef(27, "hreflang", HtmlTagId.Unknown, HtmlAttributeId.HrefLang),
                new NameDef(30, "flushright", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(38, "accept", HtmlTagId.Unknown, HtmlAttributeId.Accept),
                new NameDef(45, "frameborder", HtmlTagId.Unknown, HtmlAttributeId.FrameBorder),
                new NameDef(52, "shape", HtmlTagId.Unknown, HtmlAttributeId.Shape),
                new NameDef(55, "param", HtmlTagIndex.Param, false, false, HtmlTagId.Param, HtmlAttributeId.Unknown),
                new NameDef(56, "acronym", HtmlTagIndex.Acronym, false, false, HtmlTagId.Acronym, HtmlAttributeId.Unknown),
                new NameDef(56, "bdo", HtmlTagIndex.Bdo, false, false, HtmlTagId.Bdo, HtmlAttributeId.Unknown),
                new NameDef(58, "for", HtmlTagId.Unknown, HtmlAttributeId.For),
                new NameDef(58, "text", HtmlTagId.Unknown, HtmlAttributeId.Text),
                new NameDef(59, "color", HtmlTagId.Unknown, HtmlAttributeId.Color),
                new NameDef(63, "a", HtmlTagIndex.A, false, false, HtmlTagId.A, HtmlAttributeId.Unknown),
                new NameDef(66, "?pxml", HtmlTagIndex._Pxml, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(68, "face", HtmlTagId.Unknown, HtmlAttributeId.Face),
                new NameDef(72, "rowspan", HtmlTagId.Unknown, HtmlAttributeId.RowSpan),
                new NameDef(73, "nowrap", HtmlTagId.Unknown, HtmlAttributeId.NoWrap),
                new NameDef(74, "ins", HtmlTagIndex.Ins, false, false, HtmlTagId.Ins, HtmlAttributeId.Unknown),
                new NameDef(85, "rp", HtmlTagIndex.RP, false, false, HtmlTagId.RP, HtmlAttributeId.Unknown),
                new NameDef(86, "script", HtmlTagIndex.Script, true, true, HtmlTagId.Script, HtmlAttributeId.Unknown),
                new NameDef(88, "char", HtmlTagId.Unknown, HtmlAttributeId.Char),
                new NameDef(100, "bgcolor", HtmlTagId.Unknown, HtmlAttributeId.BGColor),
                new NameDef(123, "style", HtmlTagIndex.Style, true, true, HtmlTagId.Style, HtmlAttributeId.Style),
                new NameDef(126, "width", HtmlTagId.Unknown, HtmlAttributeId.Width),
                new NameDef(128, "headers", HtmlTagId.Unknown, HtmlAttributeId.Headers),
                new NameDef(130, "map", HtmlTagIndex.Map, false, false, HtmlTagId.Map, HtmlAttributeId.Unknown),
                new NameDef(130, "listing", HtmlTagIndex.Listing, false, false, HtmlTagId.Listing, HtmlAttributeId.Unknown),
                new NameDef(132, "data", HtmlTagId.Unknown, HtmlAttributeId.Data),
                new NameDef(135, "sub", HtmlTagIndex.Sub, false, false, HtmlTagId.Sub, HtmlAttributeId.Unknown),
                new NameDef(136, "h2", HtmlTagIndex.H2, false, false, HtmlTagId.H2, HtmlAttributeId.Unknown),
                new NameDef(137, "image", HtmlTagIndex.Image, false, false, HtmlTagId.Image, HtmlAttributeId.Unknown),
                new NameDef(141, "standby", HtmlTagId.Unknown, HtmlAttributeId.StandBy),
                new NameDef(143, "select", HtmlTagIndex.Select, false, false, HtmlTagId.Select, HtmlAttributeId.Unknown),
                new NameDef(145, "profile", HtmlTagId.Unknown, HtmlAttributeId.Profile),
                new NameDef(145, "nohref", HtmlTagId.Unknown, HtmlAttributeId.NoHref),
                new NameDef(155, "button", HtmlTagIndex.Button, false, false, HtmlTagId.Button, HtmlAttributeId.Unknown),
                new NameDef(162, "meta", HtmlTagIndex.Meta, false, false, HtmlTagId.Meta, HtmlAttributeId.Unknown),
                new NameDef(166, "rules", HtmlTagId.Unknown, HtmlAttributeId.Rules),
                new NameDef(167, "class", HtmlTagId.Unknown, HtmlAttributeId.Class),
                new NameDef(170, "src", HtmlTagId.Unknown, HtmlAttributeId.Src),
                new NameDef(171, "legend", HtmlTagIndex.Legend, false, false, HtmlTagId.Legend, HtmlAttributeId.Unknown),
                new NameDef(177, "scrolling", HtmlTagId.Unknown, HtmlAttributeId.Scrolling),
                new NameDef(180, "vlink", HtmlTagId.Unknown, HtmlAttributeId.Vlink),
                new NameDef(181, "del", HtmlTagIndex.Del, false, false, HtmlTagId.Del, HtmlAttributeId.Unknown),
                new NameDef(187, "hspace", HtmlTagId.Unknown, HtmlAttributeId.Hspace),
                new NameDef(190, "charset", HtmlTagId.Unknown, HtmlAttributeId.Charset),
                new NameDef(196, "rt", HtmlTagIndex.RT, false, false, HtmlTagId.RT, HtmlAttributeId.Unknown),
                new NameDef(198, "italic", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(201, "div", HtmlTagIndex.Div, false, false, HtmlTagId.Div, HtmlAttributeId.Unknown),
                new NameDef(205, "dir", HtmlTagIndex.Dir, false, false, HtmlTagId.Dir, HtmlAttributeId.Dir),
                new NameDef(206, "tt", HtmlTagIndex.TT, false, false, HtmlTagId.TT, HtmlAttributeId.Unknown),
                new NameDef(206, "lowsrc", HtmlTagId.Unknown, HtmlAttributeId.LowSrc),
                new NameDef(208, "h6", HtmlTagIndex.H6, false, false, HtmlTagId.H6, HtmlAttributeId.Unknown),
                new NameDef(209, "valuetype", HtmlTagId.Unknown, HtmlAttributeId.ValueType),
                new NameDef(210, "declare", HtmlTagId.Unknown, HtmlAttributeId.Declare),
                new NameDef(212, "size", HtmlTagId.Unknown, HtmlAttributeId.Size),
                new NameDef(214, "frameset", HtmlTagIndex.FrameSet, false, false, HtmlTagId.FrameSet, HtmlAttributeId.Unknown),
                new NameDef(216, "readonly", HtmlTagId.Unknown, HtmlAttributeId.ReadOnly),
                new NameDef(218, "language", HtmlTagId.Unknown, HtmlAttributeId.Language),
                new NameDef(220, "area", HtmlTagIndex.Area, false, false, HtmlTagId.Area, HtmlAttributeId.Unknown),
                new NameDef(220, "blockquote", HtmlTagIndex.BlockQuote, false, false, HtmlTagId.BlockQuote, HtmlAttributeId.Unknown),
                new NameDef(222, "topmargin", HtmlTagId.Unknown, HtmlAttributeId.TopMargin),
                new NameDef(223, "noembed", HtmlTagIndex.NoEmbed, false, false, HtmlTagId.NoEmbed, HtmlAttributeId.Unknown),
                new NameDef(224, "basefont", HtmlTagIndex.BaseFont, false, false, HtmlTagId.BaseFont, HtmlAttributeId.Unknown),
                new NameDef(229, "noframes", HtmlTagIndex.NoFrames, false, false, HtmlTagId.NoFrames, HtmlAttributeId.Unknown),
                new NameDef(232, "border", HtmlTagId.Unknown, HtmlAttributeId.Border),
                new NameDef(233, "center", HtmlTagIndex.Center, false, false, HtmlTagId.Center, HtmlAttributeId.Unknown),
                new NameDef(237, "height", HtmlTagId.Unknown, HtmlAttributeId.Height),
                new NameDef(240, "underline", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(242, "flushboth", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(244, "bgsound", HtmlTagIndex.BGSound, false, false, HtmlTagId.BGSound, HtmlAttributeId.Unknown),
                new NameDef(249, "var", HtmlTagIndex.Var, false, false, HtmlTagId.Var, HtmlAttributeId.Unknown),
                new NameDef(249, "start", HtmlTagId.Unknown, HtmlAttributeId.Start),
                new NameDef(252, "td", HtmlTagIndex.TD, false, false, HtmlTagId.TD, HtmlAttributeId.Unknown),
                new NameDef(253, "id", HtmlTagId.Unknown, HtmlAttributeId.Id),
                new NameDef(254, "rows", HtmlTagId.Unknown, HtmlAttributeId.Rows),
                new NameDef(255, "h4", HtmlTagIndex.H4, false, false, HtmlTagId.H4, HtmlAttributeId.Unknown),
                new NameDef(257, "abbr", HtmlTagIndex.Abbr, false, false, HtmlTagId.Abbr, HtmlAttributeId.Abbr),
                new NameDef(258, "http-equiv", HtmlTagId.Unknown, HtmlAttributeId.HttpEquiv),
                new NameDef(259, "span", HtmlTagIndex.Span, false, false, HtmlTagId.Span, HtmlAttributeId.Span),
                new NameDef(268, "dd", HtmlTagIndex.DD, false, false, HtmlTagId.DD, HtmlAttributeId.Unknown),
                new NameDef(271, "address", HtmlTagIndex.Address, false, false, HtmlTagId.Address, HtmlAttributeId.Unknown),
                new NameDef(273, "applet", HtmlTagIndex.Applet, false, false, HtmlTagId.Applet, HtmlAttributeId.Unknown),
                new NameDef(274, "rel", HtmlTagId.Unknown, HtmlAttributeId.Rel),
                new NameDef(278, "textarea", HtmlTagIndex.TextArea, true, false, HtmlTagId.TextArea, HtmlAttributeId.Unknown),
                new NameDef(279, "tbody", HtmlTagIndex.Tbody, false, false, HtmlTagId.Tbody, HtmlAttributeId.Unknown),
                new NameDef(283, "paraindent", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(286, "dt", HtmlTagIndex.DT, false, false, HtmlTagId.DT, HtmlAttributeId.Unknown),
                new NameDef(292, "checked", HtmlTagId.Unknown, HtmlAttributeId.Checked),
                new NameDef(292, "nextid", HtmlTagIndex.NextId, false, false, HtmlTagId.NextId, HtmlAttributeId.Unknown),
                new NameDef(293, "head", HtmlTagIndex.Head, false, false, HtmlTagId.Head, HtmlAttributeId.Unknown),
                new NameDef(296, "rev", HtmlTagId.Unknown, HtmlAttributeId.Rev),
                new NameDef(297, "small", HtmlTagIndex.Small, false, false, HtmlTagId.Small, HtmlAttributeId.Unknown),
                new NameDef(299, "cite", HtmlTagIndex.Cite, false, false, HtmlTagId.Cite, HtmlAttributeId.Cite),
                new NameDef(305, "cols", HtmlTagId.Unknown, HtmlAttributeId.Cols),
                new NameDef(305, "longdesc", HtmlTagId.Unknown, HtmlAttributeId.LongDesc),
                new NameDef(306, "sup", HtmlTagIndex.Sup, false, false, HtmlTagId.Sup, HtmlAttributeId.Unknown),
                new NameDef(314, "fixed", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(315, "prompt", HtmlTagId.Unknown, HtmlAttributeId.Prompt),
                new NameDef(320, "disabled", HtmlTagId.Unknown, HtmlAttributeId.Disabled),
                new NameDef(322, "name", HtmlTagId.Unknown, HtmlAttributeId.Name),
                new NameDef(322, "coords", HtmlTagId.Unknown, HtmlAttributeId.Coords),
                new NameDef(323, "summary", HtmlTagId.Unknown, HtmlAttributeId.Summary),
                new NameDef(324, "object", HtmlTagIndex.Object, false, false, HtmlTagId.Object, HtmlAttributeId.Object),
                new NameDef(331, "label", HtmlTagIndex.Label, false, false, HtmlTagId.Label, HtmlAttributeId.Label),
                new NameDef(332, "content", HtmlTagId.Unknown, HtmlAttributeId.Content),
                new NameDef(333, "target", HtmlTagId.Unknown, HtmlAttributeId.Target),
                new NameDef(340, "em", HtmlTagIndex.EM, false, false, HtmlTagId.EM, HtmlAttributeId.Unknown),
                new NameDef(344, "clear", HtmlTagId.Unknown, HtmlAttributeId.Clear),
                new NameDef(354, "scope", HtmlTagId.Unknown, HtmlAttributeId.Scope),
                new NameDef(356, "compact", HtmlTagId.Unknown, HtmlAttributeId.Compact),
                new NameDef(358, "blink", HtmlTagIndex.Blink, false, false, HtmlTagId.Blink, HtmlAttributeId.Unknown),
                new NameDef(372, "selected", HtmlTagId.Unknown, HtmlAttributeId.Selected),
                new NameDef(374, "maxlength", HtmlTagId.Unknown, HtmlAttributeId.MaxLength),
                new NameDef(380, "frame", HtmlTagIndex.Frame, false, false, HtmlTagId.Frame, HtmlAttributeId.Frame),
                new NameDef(381, "thead", HtmlTagIndex.Thead, false, false, HtmlTagId.Thead, HtmlAttributeId.Unknown),
                new NameDef(389, "tabindex", HtmlTagId.Unknown, HtmlAttributeId.TabIndex),
                new NameDef(395, "?import", HtmlTagIndex._Import, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(395, "marquee", HtmlTagIndex.Marquee, false, false, HtmlTagId.Marquee, HtmlAttributeId.Unknown),
                new NameDef(405, "embed", HtmlTagIndex.Embed, false, false, HtmlTagId.Embed, HtmlAttributeId.Unknown),
                new NameDef(406, "th", HtmlTagIndex.TH, false, false, HtmlTagId.TH, HtmlAttributeId.Unknown),
                new NameDef(407, "caption", HtmlTagIndex.Caption, false, false, HtmlTagId.Caption, HtmlAttributeId.Unknown),
                new NameDef(413, "value", HtmlTagId.Unknown, HtmlAttributeId.Value),
                new NameDef(420, "smaller", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(424, "datetime", HtmlTagId.Unknown, HtmlAttributeId.DateTime),
                new NameDef(426, "classid", HtmlTagId.Unknown, HtmlAttributeId.ClassId),
                new NameDef(432, "bold", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(433, "strike", HtmlTagIndex.Strike, false, false, HtmlTagId.Strike, HtmlAttributeId.Unknown),
                new NameDef(447, "flushleft", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(448, "noshade", HtmlTagId.Unknown, HtmlAttributeId.NoShade),
                new NameDef(449, "leftmargin", HtmlTagId.Unknown, HtmlAttributeId.LeftMargin),
                new NameDef(450, "title", HtmlTagIndex.Title, true, false, HtmlTagId.Title, HtmlAttributeId.Title),
                new NameDef(452, "excerpt", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(454, "cellpadding", HtmlTagId.Unknown, HtmlAttributeId.CellPadding),
                new NameDef(458, "dfn", HtmlTagIndex.Dfn, false, false, HtmlTagId.Dfn, HtmlAttributeId.Unknown),
                new NameDef(459, "charoff", HtmlTagId.Unknown, HtmlAttributeId.CharOff),
                new NameDef(461, "isindex", HtmlTagIndex.IsIndex, false, false, HtmlTagId.IsIndex, HtmlAttributeId.Unknown),
                new NameDef(462, "tfoot", HtmlTagIndex.Tfoot, false, false, HtmlTagId.Tfoot, HtmlAttributeId.Unknown),
                new NameDef(464, "nobr", HtmlTagIndex.NoBR, false, false, HtmlTagId.NoBR, HtmlAttributeId.Unknown),
                new NameDef(470, "lang", HtmlTagId.Unknown, HtmlAttributeId.Lang),
                new NameDef(472, "optgroup", HtmlTagIndex.OptGroup, false, false, HtmlTagId.OptGroup, HtmlAttributeId.Unknown),
                new NameDef(474, "accept-charset", HtmlTagId.Unknown, HtmlAttributeId.AcceptCharset),
                new NameDef(474, "option", HtmlTagIndex.Option, false, false, HtmlTagId.Option, HtmlAttributeId.Unknown),
                new NameDef(476, "big", HtmlTagIndex.Big, false, false, HtmlTagId.Big, HtmlAttributeId.Unknown),
                new NameDef(477, "font", HtmlTagIndex.Font, false, false, HtmlTagId.Font, HtmlAttributeId.Unknown),
                new NameDef(479, "type", HtmlTagId.Unknown, HtmlAttributeId.Type),
                new NameDef(482, "href", HtmlTagId.Unknown, HtmlAttributeId.Href),
                new NameDef(484, "img", HtmlTagIndex.Img, false, false, HtmlTagId.Img, HtmlAttributeId.Unknown),
                new NameDef(486, "vspace", HtmlTagId.Unknown, HtmlAttributeId.Vspace),
                new NameDef(487, "h3", HtmlTagIndex.H3, false, false, HtmlTagId.H3, HtmlAttributeId.Unknown),
                new NameDef(493, "align", HtmlTagId.Unknown, HtmlAttributeId.Align),
                new NameDef(497, "wbr", HtmlTagIndex.Wbr, false, false, HtmlTagId.Wbr, HtmlAttributeId.Unknown),
                new NameDef(499, "accesskey", HtmlTagId.Unknown, HtmlAttributeId.AccessKey),
                new NameDef(502, "col", HtmlTagIndex.Col, false, false, HtmlTagId.Col, HtmlAttributeId.Unknown),
                new NameDef(504, "menu", HtmlTagIndex.Menu, false, false, HtmlTagId.Menu, HtmlAttributeId.Unknown),
                new NameDef(504, "bigger", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(506, "codebase", HtmlTagId.Unknown, HtmlAttributeId.CodeBase),
                new NameDef(508, "strong", HtmlTagIndex.Strong, false, false, HtmlTagId.Strong, HtmlAttributeId.Unknown),
                new NameDef(510, "br", HtmlTagIndex.BR, false, false, HtmlTagId.BR, HtmlAttributeId.Unknown),
                new NameDef(512, "archive", HtmlTagId.Unknown, HtmlAttributeId.Archive),
                new NameDef(514, "ul", HtmlTagIndex.UL, false, false, HtmlTagId.UL, HtmlAttributeId.Unknown),
                new NameDef(516, "noscript", HtmlTagIndex.NoScript, false, false, HtmlTagId.NoScript, HtmlAttributeId.Unknown),
                new NameDef(517, "plaintext", HtmlTagIndex.PlainText, true, true, HtmlTagId.PlainText, HtmlAttributeId.Unknown),
                new NameDef(521, "base", HtmlTagIndex.Base, false, false, HtmlTagId.Base, HtmlAttributeId.Unknown),
                new NameDef(521, "ismap", HtmlTagId.Unknown, HtmlAttributeId.IsMap),
                new NameDef(522, "defer", HtmlTagId.Unknown, HtmlAttributeId.Defer),
                new NameDef(523, "body", HtmlTagIndex.Body, false, false, HtmlTagId.Body, HtmlAttributeId.Unknown),
                new NameDef(524, "ol", HtmlTagIndex.OL, false, false, HtmlTagId.OL, HtmlAttributeId.Unknown),
                new NameDef(526, "h1", HtmlTagIndex.H1, false, false, HtmlTagId.H1, HtmlAttributeId.Unknown),
                new NameDef(528, "valign", HtmlTagId.Unknown, HtmlAttributeId.Valign),
                new NameDef(531, "media", HtmlTagId.Unknown, HtmlAttributeId.Media),
                new NameDef(532, "iframe", HtmlTagIndex.Iframe, false, false, HtmlTagId.Iframe, HtmlAttributeId.Unknown),
                new NameDef(533, "dl", HtmlTagIndex.DL, false, false, HtmlTagId.DL, HtmlAttributeId.Unknown),
                new NameDef(535, "colspan", HtmlTagId.Unknown, HtmlAttributeId.ColSpan),
                new NameDef(538, "axis", HtmlTagId.Unknown, HtmlAttributeId.Axis),
                new NameDef(542, "marginheight", HtmlTagId.Unknown, HtmlAttributeId.MarginHeight),
                new NameDef(543, "alink", HtmlTagId.Unknown, HtmlAttributeId.Alink),
                new NameDef(545, "?xml:namespace", HtmlTagIndex._Xml_Namespace, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(546, "method", HtmlTagId.Unknown, HtmlAttributeId.Method),
                new NameDef(549, "fontfamily", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
                new NameDef(554, "fieldset", HtmlTagIndex.FieldSet, false, false, HtmlTagId.FieldSet, HtmlAttributeId.Unknown),
                new NameDef(556, "pre", HtmlTagIndex.Pre, false, false, HtmlTagId.Pre, HtmlAttributeId.Unknown),
                new NameDef(557, "table", HtmlTagIndex.Table, false, false, HtmlTagId.Table, HtmlAttributeId.Unknown),
                new NameDef(560, "tr", HtmlTagIndex.TR, false, false, HtmlTagId.TR, HtmlAttributeId.Unknown),
                new NameDef(562, "samp", HtmlTagIndex.Samp, false, false, HtmlTagId.Samp, HtmlAttributeId.Unknown),
                new NameDef(563, "link", HtmlTagIndex.Link, false, false, HtmlTagId.Link, HtmlAttributeId.Link),
                new NameDef(564, "hr", HtmlTagIndex.HR, false, false, HtmlTagId.HR, HtmlAttributeId.Unknown),
                new NameDef(568, "form", HtmlTagIndex.Form, false, false, HtmlTagId.Form, HtmlAttributeId.Unknown),
                new NameDef(569, "input", HtmlTagIndex.Input, false, false, HtmlTagId.Input, HtmlAttributeId.Unknown),
                new NameDef(570, "xml", HtmlTagIndex.Xml, true, true, HtmlTagId.Xml, HtmlAttributeId.Unknown),
                new NameDef(572, "usemap", HtmlTagId.Unknown, HtmlAttributeId.UseMap),
                new NameDef(574, "xmp", HtmlTagIndex.Xmp, true, true, HtmlTagId.Xmp, HtmlAttributeId.Unknown),
                new NameDef(574, "html", HtmlTagIndex.Html, false, false, HtmlTagId.Html, HtmlAttributeId.Unknown),
                new NameDef(575, "codetype", HtmlTagId.Unknown, HtmlAttributeId.CodeType),
                new NameDef(580, "marginwidth", HtmlTagId.Unknown, HtmlAttributeId.MarginWidth),
                new NameDef(584, "q", HtmlTagIndex.Q, false, false, HtmlTagId.Q, HtmlAttributeId.Unknown),
                new NameDef(585, "colgroup", HtmlTagIndex.ColGroup, false, false, HtmlTagId.ColGroup, HtmlAttributeId.Unknown),
                new NameDef(585, "dynsrc", HtmlTagId.Unknown, HtmlAttributeId.DynSrc),
                new NameDef(586, "s", HtmlTagIndex.S, false, false, HtmlTagId.S, HtmlAttributeId.Unknown),
                new NameDef(587, "p", HtmlTagIndex.P, false, false, HtmlTagId.P, HtmlAttributeId.Unknown),
                new NameDef(588, "u", HtmlTagIndex.U, false, false, HtmlTagId.U, HtmlAttributeId.Unknown),
                new NameDef(589, "action", HtmlTagId.Unknown, HtmlAttributeId.Action),
                new NameDef(590, "enctype", HtmlTagId.Unknown, HtmlAttributeId.EncType),
                new NameDef(592, "i", HtmlTagIndex.I, false, false, HtmlTagId.I, HtmlAttributeId.Unknown),
                new NameDef(597, "b", HtmlTagIndex.B, false, false, HtmlTagId.B, HtmlAttributeId.Unknown),
                new NameDef(598, "h5", HtmlTagIndex.H5, false, false, HtmlTagId.H5, HtmlAttributeId.Unknown),
                new NameDef(599, "background", HtmlTagId.Unknown, HtmlAttributeId.Background),
                new NameDef(600, null, HtmlTagId.Unknown, HtmlAttributeId.Unknown)
            };
            entities = new EntityDef[]
            {
                new EntityDef(0, 0, null),
                new EntityDef(1, 969, "omega"),
                new EntityDef(1, 8195, "emsp"),
                new EntityDef(2, 8235, "rle"),
                new EntityDef(5, 211, "Oacute"),
                new EntityDef(8, 402, "fnof"),
                new EntityDef(9, 216, "Oslash"),
                new EntityDef(12, 209, "Ntilde"),
                new EntityDef(14, 8592, "larr"),
                new EntityDef(15, 968, "psi"),
                new EntityDef(15, 34, "QUOT"),
                new EntityDef(20, 928, "Pi"),
                new EntityDef(22, 181, "micro"),
                new EntityDef(24, 982, "piv"),
                new EntityDef(26, 978, "upsih"),
                new EntityDef(28, 926, "Xi"),
                new EntityDef(28, 8968, "lceil"),
                new EntityDef(29, 229, "aring"),
                new EntityDef(30, 8715, "ni"),
                new EntityDef(30, 175, "macr"),
                new EntityDef(32, 8745, "cap"),
                new EntityDef(33, 239, "iuml"),
                new EntityDef(34, 967, "chi"),
                new EntityDef(38, 188, "frac14"),
                new EntityDef(40, 190, "frac34"),
                new EntityDef(41, 186, "ordm"),
                new EntityDef(41, 160, "nbsp"),
                new EntityDef(44, 8743, "and"),
                new EntityDef(47, 166, "brvbar"),
                new EntityDef(49, 8203, "zwsp"),
                new EntityDef(50, 8704, "forall"),
                new EntityDef(52, 960, "pi"),
                new EntityDef(53, 8855, "otimes"),
                new EntityDef(54, 250, "uacute"),
                new EntityDef(55, 8736, "ang"),
                new EntityDef(56, 161, "iexcl"),
                new EntityDef(57, 8206, "lrm"),
                new EntityDef(57, 965, "upsilon"),
                new EntityDef(60, 958, "xi"),
                new EntityDef(65, 8234, "lre"),
                new EntityDef(66, 8205, "zwj"),
                new EntityDef(68, 925, "Nu"),
                new EntityDef(69, 924, "Mu"),
                new EntityDef(71, 8237, "lro"),
                new EntityDef(73, 169, "COPY"),
                new EntityDef(74, 8836, "nsub"),
                new EntityDef(74, 8834, "sub"),
                new EntityDef(83, 254, "thorn"),
                new EntityDef(85, 8721, "sum"),
                new EntityDef(87, 8217, "rsquo"),
                new EntityDef(88, 183, "middot"),
                new EntityDef(97, 202, "Ecirc"),
                new EntityDef(97, 8240, "permil"),
                new EntityDef(98, 8201, "thinsp"),
                new EntityDef(100, 215, "times"),
                new EntityDef(100, 957, "nu"),
                new EntityDef(101, 956, "mu"),
                new EntityDef(102, 165, "yen"),
                new EntityDef(107, 8719, "prod"),
                new EntityDef(116, 8659, "dArr"),
                new EntityDef(118, 235, "euml"),
                new EntityDef(118, 226, "acirc"),
                new EntityDef(122, 914, "Beta"),
                new EntityDef(123, 8730, "radic"),
                new EntityDef(123, 189, "frac12"),
                new EntityDef(130, 9829, "hearts"),
                new EntityDef(131, 8482, "TRADE"),
                new EntityDef(134, 8250, "rsaquo"),
                new EntityDef(136, 196, "Auml"),
                new EntityDef(137, 249, "ugrave"),
                new EntityDef(142, 231, "ccedil"),
                new EntityDef(143, 338, "OElig"),
                new EntityDef(144, 167, "sect"),
                new EntityDef(144, 201, "Eacute"),
                new EntityDef(146, 8756, "there4"),
                new EntityDef(148, 174, "REG"),
                new EntityDef(155, 177, "plusmn"),
                new EntityDef(155, 9001, "lang"),
                new EntityDef(158, 977, "thetasym"),
                new EntityDef(161, 8658, "rArr"),
                new EntityDef(162, 953, "iota"),
                new EntityDef(163, 8969, "rceil"),
                new EntityDef(164, 8709, "empty"),
                new EntityDef(164, 38, "AMP"),
                new EntityDef(164, 62, "gt"),
                new EntityDef(165, 934, "Phi"),
                new EntityDef(166, 915, "Gamma"),
                new EntityDef(167, 8299, "ass"),
                new EntityDef(171, 60, "lt"),
                new EntityDef(177, 918, "Zeta"),
                new EntityDef(179, 210, "Ograve"),
                new EntityDef(179, 176, "deg"),
                new EntityDef(187, 9824, "spades"),
                new EntityDef(193, 8204, "zwnj"),
                new EntityDef(193, 8707, "exist"),
                new EntityDef(196, 948, "delta"),
                new EntityDef(200, 174, "reg"),
                new EntityDef(203, 8712, "isin"),
                new EntityDef(204, 913, "Alpha"),
                new EntityDef(207, 376, "Yuml"),
                new EntityDef(211, 184, "cedil"),
                new EntityDef(211, 178, "sup2"),
                new EntityDef(218, 8971, "rfloor"),
                new EntityDef(220, 247, "divide"),
                new EntityDef(222, 927, "Omicron"),
                new EntityDef(225, 170, "ordf"),
                new EntityDef(227, 9827, "clubs"),
                new EntityDef(228, 220, "Uuml"),
                new EntityDef(229, 919, "Eta"),
                new EntityDef(231, 194, "Acirc"),
                new EntityDef(232, 195, "Atilde"),
                new EntityDef(233, 929, "Rho"),
                new EntityDef(234, 8501, "alefsym"),
                new EntityDef(240, 198, "AElig"),
                new EntityDef(240, 8970, "lfloor"),
                new EntityDef(248, 8660, "hArr"),
                new EntityDef(250, 8254, "oline"),
                new EntityDef(254, 193, "Aacute"),
                new EntityDef(255, 199, "Ccedil"),
                new EntityDef(256, 952, "theta"),
                new EntityDef(257, 8744, "or"),
                new EntityDef(258, 200, "Egrave"),
                new EntityDef(259, 8482, "trade"),
                new EntityDef(260, 8747, "int"),
                new EntityDef(262, 185, "sup1"),
                new EntityDef(263, 966, "phi"),
                new EntityDef(265, 8746, "cup"),
                new EntityDef(268, 8801, "equiv"),
                new EntityDef(272, 8764, "sim"),
                new EntityDef(273, 221, "Yacute"),
                new EntityDef(276, 8243, "Prime"),
                new EntityDef(277, 8593, "uarr"),
                new EntityDef(280, 8212, "mdash"),
                new EntityDef(280, 38, "amp"),
                new EntityDef(281, 180, "acute"),
                new EntityDef(285, 208, "ETH"),
                new EntityDef(287, 233, "eacute"),
                new EntityDef(290, 8472, "weierp"),
                new EntityDef(292, 922, "Kappa"),
                new EntityDef(294, 920, "Theta"),
                new EntityDef(304, 163, "pound"),
                new EntityDef(307, 8594, "rarr"),
                new EntityDef(307, 937, "Omega"),
                new EntityDef(312, 339, "oelig"),
                new EntityDef(313, 8835, "sup"),
                new EntityDef(314, 236, "igrave"),
                new EntityDef(316, 162, "cent"),
                new EntityDef(319, 224, "agrave"),
                new EntityDef(321, 8224, "dagger"),
                new EntityDef(328, 8226, "bull"),
                new EntityDef(330, 214, "Ouml"),
                new EntityDef(335, 197, "Aring"),
                new EntityDef(338, 9674, "loz"),
                new EntityDef(345, 8727, "lowast"),
                new EntityDef(346, 245, "otilde"),
                new EntityDef(351, 8364, "euro"),
                new EntityDef(352, 168, "uml"),
                new EntityDef(354, 963, "sigma"),
                new EntityDef(373, 917, "Epsilon"),
                new EntityDef(376, 8249, "lsaquo"),
                new EntityDef(379, 8465, "image"),
                new EntityDef(380, 8656, "lArr"),
                new EntityDef(384, 207, "Iuml"),
                new EntityDef(385, 935, "Chi"),
                new EntityDef(390, 951, "eta"),
                new EntityDef(394, 8596, "harr"),
                new EntityDef(397, 225, "aacute"),
                new EntityDef(399, 8302, "nads"),
                new EntityDef(401, 240, "eth"),
                new EntityDef(402, 62, "GT"),
                new EntityDef(404, 931, "Sigma"),
                new EntityDef(404, 222, "THORN"),
                new EntityDef(408, 248, "oslash"),
                new EntityDef(409, 230, "aelig"),
                new EntityDef(409, 60, "LT"),
                new EntityDef(411, 8713, "notin"),
                new EntityDef(415, 8301, "aafs"),
                new EntityDef(416, 253, "yacute"),
                new EntityDef(427, 171, "laquo"),
                new EntityDef(431, 8242, "prime"),
                new EntityDef(431, 8298, "iss"),
                new EntityDef(432, 192, "Agrave"),
                new EntityDef(433, 955, "lambda"),
                new EntityDef(436, 8853, "oplus"),
                new EntityDef(442, 8476, "real"),
                new EntityDef(443, 217, "Ugrave"),
                new EntityDef(448, 8839, "supe"),
                new EntityDef(450, 182, "para"),
                new EntityDef(455, 8595, "darr"),
                new EntityDef(466, 8838, "sube"),
                new EntityDef(468, 8776, "asymp"),
                new EntityDef(469, 203, "Euml"),
                new EntityDef(470, 244, "ocirc"),
                new EntityDef(471, 923, "Lambda"),
                new EntityDef(476, 946, "beta"),
                new EntityDef(477, 179, "sup3"),
                new EntityDef(486, 8734, "infin"),
                new EntityDef(487, 933, "Upsilon"),
                new EntityDef(488, 251, "ucirc"),
                new EntityDef(490, 916, "Delta"),
                new EntityDef(490, 228, "auml"),
                new EntityDef(506, 205, "Iacute"),
                new EntityDef(506, 710, "circ"),
                new EntityDef(513, 921, "Iota"),
                new EntityDef(516, 732, "tilde"),
                new EntityDef(517, 8222, "bdquo"),
                new EntityDef(520, 353, "scaron"),
                new EntityDef(529, 932, "Tau"),
                new EntityDef(531, 950, "zeta"),
                new EntityDef(533, 8236, "pdf"),
                new EntityDef(537, 8869, "perp"),
                new EntityDef(546, 173, "shy"),
                new EntityDef(547, 238, "icirc"),
                new EntityDef(561, 255, "yuml"),
                new EntityDef(565, 947, "gamma"),
                new EntityDef(567, 962, "sigmaf"),
                new EntityDef(569, 9002, "rang"),
                new EntityDef(572, 8629, "crarr"),
                new EntityDef(575, 187, "raquo"),
                new EntityDef(577, 8722, "minus"),
                new EntityDef(578, 242, "ograve"),
                new EntityDef(582, 252, "uuml"),
                new EntityDef(583, 212, "Ocirc"),
                new EntityDef(585, 8303, "nods"),
                new EntityDef(586, 8220, "ldquo"),
                new EntityDef(587, 961, "rho"),
                new EntityDef(588, 223, "szlig"),
                new EntityDef(601, 219, "Ucirc"),
                new EntityDef(603, 945, "alpha"),
                new EntityDef(606, 34, "quot"),
                new EntityDef(615, 8225, "Dagger"),
                new EntityDef(616, 218, "Uacute"),
                new EntityDef(620, 204, "Igrave"),
                new EntityDef(622, 936, "Psi"),
                new EntityDef(627, 964, "tau"),
                new EntityDef(628, 8216, "lsquo"),
                new EntityDef(629, 8805, "ge"),
                new EntityDef(631, 227, "atilde"),
                new EntityDef(632, 8711, "nabla"),
                new EntityDef(633, 352, "Scaron"),
                new EntityDef(634, 8773, "cong"),
                new EntityDef(635, 8260, "frasl"),
                new EntityDef(638, 8800, "ne"),
                new EntityDef(639, 164, "curren"),
                new EntityDef(640, 8804, "le"),
                new EntityDef(643, 8657, "uArr"),
                new EntityDef(648, 949, "epsilon"),
                new EntityDef(649, 237, "iacute"),
                new EntityDef(651, 8207, "rlm"),
                new EntityDef(652, 213, "Otilde"),
                new EntityDef(653, 8901, "sdot"),
                new EntityDef(653, 8238, "rlo"),
                new EntityDef(654, 8211, "ndash"),
                new EntityDef(657, 232, "egrave"),
                new EntityDef(660, 206, "Icirc"),
                new EntityDef(660, 243, "oacute"),
                new EntityDef(662, 8706, "part"),
                new EntityDef(663, 9830, "diams"),
                new EntityDef(664, 169, "copy"),
                new EntityDef(667, 241, "ntilde"),
                new EntityDef(670, 8221, "rdquo"),
                new EntityDef(671, 191, "iquest"),
                new EntityDef(672, 8218, "sbquo"),
                new EntityDef(678, 172, "not"),
                new EntityDef(679, 8300, "iafs"),
                new EntityDef(684, 246, "ouml"),
                new EntityDef(689, 234, "ecirc"),
                new EntityDef(691, 954, "kappa"),
                new EntityDef(696, 8194, "ensp"),
                new EntityDef(699, 8733, "prop"),
                new EntityDef(702, 959, "omicron"),
                new EntityDef(703, 8230, "hellip"),
                new EntityDef(704, 0, null)
            };
        }
    }
}
