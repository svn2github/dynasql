/*  Copyright 2012 Scryber Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Resources
{
    /// <summary>
    /// A complete font definition. Also contains the Descriptor, widths, and any embeddable file data
    /// </summary>
    public class PDFFontDefinition
    {

        //
        // properties
        //

        #region public FontType SubType {get; set;}

        private FontType _subtype = FontType.Type1;

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.DefaultValue(FontType.Type1)]
		[System.ComponentModel.Description("The FontType for the Font : TrueType, Type1 etc.")]
        public FontType SubType
        {
            get { return _subtype; }
            set 
            {
                _subtype = value;
                
            }
        }

        #endregion

        #region public string BaseType {get;set;}

        private string _base = "";

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.DefaultValue("")]
		[System.ComponentModel.Description("The BaseType for the Font : Helvetica, Helvetica-Bold, Times New Roman")]
		public string BaseType
        {
            get { return _base; }
            set 
            { 
                _base = value;
                
            }
        }

        #endregion

        #region public FontEncoding Encoding {get;set;}

        private FontEncoding _enc = FontEncoding.WinAnsiEncoding;

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.DefaultValue(FontEncoding.WinAnsiEncoding)]
		[System.ComponentModel.Description("The Encoding for the Font : MacRomanEncoding, Win32Encoding")]
		public FontEncoding Encoding
        {
            get { return _enc; }
            set 
            { 
                _enc = value;
                
            }
        }

        #endregion

        #region public PDFFontWidths Widths {get;set;}

        private PDFFontWidths _widths;

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Bindable(true)]
		[System.ComponentModel.DefaultValue((object)null)]
		[System.ComponentModel.Description("The character widths of the font")]
		public PDFFontWidths Widths
        {
            get { return _widths; }
            set { _widths = value; }
        }

        #endregion

        #region public virtual bool SupportsVariants {get;}

        /// <summary>
        /// Gets the flag to identify if this font supports variants such as bold and italic
        /// </summary>
        public virtual bool SupportsVariants
        {
            get { return true; }
        }

        #endregion

        #region public string Family

        private string _family;

        /// <summary>
        /// Gets or sets the family name for this font
        /// </summary>
        public string Family
        {
            get { return _family; }
            set { _family = value; }
        }

        #endregion

        #region public string FilePath {get;}

        private string _filepath;
        /// <summary>
        /// Gets the full path to the file this FontDescriptor was loaded from
        /// </summary>
        public string FilePath
        {
            get { return this._filepath; }
            private set { _filepath = value; }
        }

        #endregion

        #region public string WindowsName

        private string _winName;

        /// <summary>
        /// /Gets or sets the windows name for this font. If not set, returns the standard family name
        /// </summary>
        public string WindowsName
        {
            get 
            {
                if (string.IsNullOrEmpty(_winName))
                    return this.Family;
                else
                    return _winName;
            }
            set { _winName = value; }
        }

        #endregion

        #region public bool Bold {get;set;}

        private bool _bold;

        /// <summary>
        /// Gets or sets teh Bold flag on this font definition
        /// </summary>
        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; }
        }

        #endregion

        #region public bool Italic {get;set;}

        private bool _ital;

        public bool Italic
        {
            get { return _ital; }
            set { _ital = value; }
        }

        #endregion

        #region public PDFFontDescriptor Descriptor {get; set;}

        private PDFFontDescriptor _desc; 

        /// <summary>
        /// Gets or sets the PDFFontDescriptor for this font definition
        /// </summary>
        public PDFFontDescriptor Descriptor
        {
            get { return _desc; }
            set { _desc = value; }
        }

        #endregion

        #region public bool IsEmbedable {get;set;}

        private bool _embed;
        /// <summary>
        /// Gets the embedable flag
        /// </summary>
        public bool IsEmbedable
        {
            get { return _embed; }
            private set { _embed = true; }
        }

        #endregion

        #region public bool IsStandard {get;}

        /// <summary>
        /// Gets the flag that identifies if this font is a standard font (does not have a PDFFontDescriptor or PDFWidths)
        /// </summary>
        public bool IsStandard
        {
            get { return this._desc == null && this._widths == null; }
        }

        #endregion

        #region public string FulName {get;}

        private string _fullname;
        /// <summary>
        /// Gets the calculated full name of the font based
        /// upon the Family name, bold and italic flag
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(_fullname))
                {
                    string fn = PDFFont.GetFullName(this.Family,this.Bold,this.Italic);
                    
                    return fn;
                }
                else
                    return _fullname;
            }
            set { _fullname = value; }
        }

        #endregion


        protected PDFFontDefinition()
        {

        }

        //
        // instance methods
        //

        #region public virtual PDFObjectRef RenderFont(string name, PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// Renders the PDFFontDefinition onto the PDFWriter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public virtual PDFObjectRef RenderFont(string name, PDFContextBase context, PDFWriter writer)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Message))
                context.TraceLog.Begin(TraceLevel.Message, "Rendering font '" + this.FullName + "' to the document");

            PDFObjectRef font = writer.BeginObject(name);
            writer.BeginDictionary();
            writer.BeginDictionaryEntry("Type");
            writer.WriteName("Font");
            writer.EndDictionaryEntry();
            writer.BeginDictionaryEntry("Subtype");
            writer.WriteName(this.SubType.ToString());
            writer.EndDictionaryEntry();
            writer.BeginDictionaryEntry("Name");
            writer.WriteName(name);
            writer.EndDictionaryEntry();
            writer.BeginDictionaryEntry("BaseFont");
            writer.WriteName(this.BaseType);
            writer.EndDictionaryEntry();
            writer.BeginDictionaryEntry("Encoding");
            writer.WriteName("WinAnsiEncoding");
            writer.EndDictionaryEntry();

            //Render the widths
            if (this.Widths != null)
            {
                PDFObjectRef wid = this.Widths.RenderToPDF(context, writer);

                if (null != wid)
                {
                    writer.BeginDictionaryEntry("FirstChar");
                    writer.WriteNumber(this.Widths.FirstChar);
                    writer.EndDictionaryEntry();
                    writer.BeginDictionaryEntry("LastChar");
                    writer.WriteNumber(this.Widths.LastChar);
                    writer.EndDictionaryEntry();
                    writer.BeginDictionaryEntry("Widths");
                    writer.WriteObjectRef(wid);
                    writer.EndDictionaryEntry();
                }
            }

            //Render the FontDescriptor
            if (this.Descriptor != null)
            {
                PDFObjectRef desc = this.Descriptor.RenderToPDF(context, writer);
                if (null != desc)
                {
                    writer.BeginDictionaryEntry("FontDescriptor");
                    writer.WriteObjectRef(desc);
                    writer.EndDictionaryEntry();
                }
            }

            
            writer.EndDictionary();
            writer.EndObject();

            if (context.TraceLog.ShouldLog(TraceLevel.Message))
                context.TraceLog.End(TraceLevel.Message, "Completed rendering font '" + this.FullName + "' to the document");

            return font;
        }

        #endregion

        #region public System.Drawing.Font GetSystemFont(float pointsize)

        /// <summary>
        /// Gets the System.Drawing.Font that represents this PDFFontDefintion
        /// </summary>
        /// <param name="pointsize"></param>
        /// <returns></returns>
        public System.Drawing.Font GetSystemFont(float pointsize)
        {
            System.Drawing.FontStyle fs = System.Drawing.FontStyle.Regular;
            if (this.Bold)
                fs |= System.Drawing.FontStyle.Bold;
            if (this.Italic)
                fs |= System.Drawing.FontStyle.Italic;

            System.Drawing.Font f = new System.Drawing.Font(this.WindowsName, pointsize, fs);
            return f;
        }

        #endregion

        #region public override bool Equals(object obj) + 2 overload

        /// <summary>
        /// Overrides the default implementation to return true if the object 
        /// is a font definition and its Equal to this font definition
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PDFFontDefinition)
                return this.Equals((PDFFontDefinition)obj);
            else if (obj is PDFFont)
                return this.Equals((PDFFont)obj);
                return false;
        }

        /// <summary>
        /// Returns true if the PDFFontDefinition has the same full name as this definition
        /// </summary>
        /// <param name="defn"></param>
        /// <returns></returns>
        public bool Equals(PDFFontDefinition defn)
        {
            if (null == defn)
                return false;
            else
                return this.FullName == defn.FullName;
        }

        public bool Equals(PDFFont font)
        {
            if (null == font)
                return false;
            else
                return this.FullName == font.FullName;
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Overrides the default behaviour to return the hash of the full name
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.FullName.GetHashCode();
        }

        #endregion

        #region public override string ToString()

        /// <summary>
        /// returns a string representation of this PDFFontDefintion (its full name)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FullName;
        }

        #endregion

        

        //
        // static methods
        //

        internal static PDFFontDefinition LoadStandardFont(PDFFont font)
        {
            PDFFontDefinition defn;
            if (font.IsStandard)
            {
                defn = PDFFonts.GetStandardFontDefinition(font);
            }
            else
            {
                throw new InvalidOperationException("The font requested is not a standard font");
            }
            return defn;
        }

        public static PDFFontDefinition LoadStandardFont(string family, System.Drawing.FontStyle style)
        {
            PDFFontDefinition defn;
            if (PDFFont.IsStandardFontFamily(family))
                defn = PDFFonts.GetStandardFontDefinition(family, style);
            else
                throw new InvalidOperationException("The font requested is not a standard font");
            return defn;
        }

        private const int NameIDSubFamily = 2;
        private const int NameIDFamily = 1;
        private const int NameIDFullFamily = 4;
        private const int PDFGlyphUnits = 1000;

        internal static PDFFontDefinition LoadFontFile(string path, string familyname, System.Drawing.FontStyle style)
        {
            
            Scryber.OpenType.TTFFile ttf = new Scryber.OpenType.TTFFile(path);
            PDFFontDefinition defn = LoadFontFile(ttf, familyname, style);
            defn.FilePath = path;
           
            return defn;
        }

        internal static PDFFontDefinition LoadFontFile(byte[] data, string familyname, System.Drawing.FontStyle style)
        {

            Scryber.OpenType.TTFFile ttf = new Scryber.OpenType.TTFFile(data);
            PDFFontDefinition defn = LoadFontFile(ttf, familyname, style);
            return defn;
        }

        internal static PDFFontDefinition LoadFontFile(Scryber.OpenType.TTFFile ttf, string familyname, System.Drawing.FontStyle style)
        {
            PDFFontDefinition defn = new PDFFontDefinition();
            defn.Family = familyname;
            defn.SubType = FontType.TrueType;

            defn.BaseType = ttf.Tables.Names.Names[NameIDFullFamily].InvariantName.Replace(" ", "");
            defn.Bold = (style & System.Drawing.FontStyle.Bold) > 0;
            defn.Italic = (style & System.Drawing.FontStyle.Italic) > 0;
            defn.WindowsName = defn.BaseType;
            defn.IsEmbedable = GetEmbedding(ttf);
            defn.Descriptor = GetFontDescriptor(defn.BaseType, defn.IsEmbedable, ttf);
            defn.Widths = GetFontWidths(ttf);
            defn.Encoding = FontEncoding.WinAnsiEncoding;//TODO:Check encoding
            return defn;
        }

        private static bool GetEmbedding(Scryber.OpenType.TTFFile ttf)
        {
            Scryber.OpenType.SubTables.OS2Table os2 = ttf.Tables.WindowsMetrics;

            if (os2.FSType == Scryber.OpenType.SubTables.FontRestrictions.InstallableEmbedding)
                return true;
            else if ((os2.FSType & Scryber.OpenType.SubTables.FontRestrictions.NoEmbedding) > 0)
                return false;
            else if ((os2.FSType & Scryber.OpenType.SubTables.FontRestrictions.PreviewPrintEmbedding) > 0 ||
                     (os2.FSType & Scryber.OpenType.SubTables.FontRestrictions.EditableEmbedding) > 0)
                return true;
            else
                return false;

        }

        private static PDFFontWidths GetFontWidths(Scryber.OpenType.TTFFile ttf)
        {
            Scryber.OpenType.SubTables.FontHeader head = ttf.Tables.FontHeader;
            Scryber.OpenType.SubTables.CMAPTable cmap = ttf.Tables.CMap;
            Scryber.OpenType.SubTables.CMAPSubTable mac = cmap.GetOffsetTable(Scryber.OpenType.SubTables.CharacterPlatforms.Macintosh, (Scryber.OpenType.SubTables.CharacterEncodings)0);
            
            int unitsperem = head.UnitsPerEm;

            List<int> all = new List<int>();
            Scryber.OpenType.SubTables.HorizontalMetrics hmtc = ttf.Tables.HorizontalMetrics;
            for (int i = 32; i < 256; i++)
            {
                int moffset = (int)mac.GetCharacterGlyphOffset((char)i);
                //System.Diagnostics.Debug.WriteLine("Character '" + chars[i].ToString() + "' (" + ((byte)chars[i]).ToString() + ") has offset '" + moffset.ToString() + "' in mac encoding and '" + woffset + "' in windows encoding");

                if (moffset >= hmtc.HMetrics.Count)
                    moffset = hmtc.HMetrics.Count - 1;
                Scryber.OpenType.SubTables.HMetric metric = hmtc.HMetrics[moffset];

                int value = metric.AdvanceWidth; // - metric.LeftSideBearing;
                value = (value * PDFGlyphUnits) / unitsperem;
                all.Add(value);
            }

            PDFFontWidths widths = new PDFFontWidths(32, all.Count + 31, all);
            return widths;

        }

        private static PDFFontDescriptor GetFontDescriptor(string name, bool embed, Scryber.OpenType.TTFFile ttf)
        {
            PDFFontDescriptor desc = new PDFFontDescriptor();
            desc.FontName = name;
            desc.Flags = 0;//TODO:Flags
            Scryber.OpenType.SubTables.FontHeader head = ttf.Tables.FontHeader;
            int unitsperem = head.UnitsPerEm;
            desc.BoundingBox = new int[] { (head.XMin * PDFGlyphUnits) / unitsperem, 
                                           (head.YMin * PDFGlyphUnits) / unitsperem, 
                                           (head.XMax * PDFGlyphUnits) / unitsperem, 
                                           (head.YMax * PDFGlyphUnits) / unitsperem };
            Scryber.OpenType.SubTables.HorizontalHeader hhea = ttf.Tables.HorizontalHeader;
            Scryber.OpenType.SubTables.PostscriptTable post = ttf.Tables.PostscriptInformation;
            Scryber.OpenType.SubTables.OS2Table os2 = ttf.Tables.WindowsMetrics;
            desc.ItalicAngle = (int)post.ItalicAngle;

            desc.StemV = 80;// (int)os2.WidthClass;
            desc.Flags = 32;
            desc.AvgWidth = 342;
            desc.Ascent = ((int)os2.TypoAscender * PDFGlyphUnits) / unitsperem;
            desc.CapHeight = (os2.CapHeight * PDFGlyphUnits) / unitsperem;
            desc.Descent = (os2.TypoDescender * PDFGlyphUnits) / unitsperem;
            //MaxWidth 1086/FontWeight 700/XHeight 250/Leading 32
            desc.MaxWidth = (head.XMax * PDFGlyphUnits) / unitsperem;
            desc.XHeight = 250;
            desc.Leading = 32;
            
            if (embed)
                desc.FontFile = ttf.FileData;
            else
                desc.FontFile = null;

            return desc;
        }


        #region internal static PDFFont InitStdType1Mac(string name, string basetype)

        internal static PDFFontDefinition InitStdType1Mac(string name, string basetype)
        {
            return InitStdType1Mac(name, basetype, basetype, false, false);
        }

        internal static PDFFontDefinition InitStdSymbolType1Mac(string name, string basetype)
        {
            PDFSymbolFontDefinition f = new PDFSymbolFontDefinition();
            f.SubType = FontType.Type1;
            f.BaseType = basetype;
            f.Encoding = FontEncoding.MacRomanEncoding;
            f.Family = basetype;
            f.Bold = false;
            f.Italic = false;
            return f;
        }

        internal static PDFFontDefinition InitStdType1Mac(string name, string basetype, string family, bool bold, bool italic)
        {
            return InitStdType1Mac(name, basetype, family, family, bold, italic);
        }

        internal static PDFFontDefinition InitStdType1Mac(string name, string basetype, string family, string winName, bool bold, bool italic)
        {
            PDFFontDefinition f = new PDFFontDefinition();
            f.SubType = FontType.Type1;
            f.BaseType = basetype;
            f.Encoding = FontEncoding.MacRomanEncoding;
            f.Family = family;
            f.WindowsName = winName;
            f.Bold = bold;
            f.Italic = italic;
            return f;
        }

        #endregion

        #region internal static PDFFontDefinition InitType1Custom(string name, string basetype, PDFFontWidths widths, PDFFontDescriptor desc)

        internal static PDFFontDefinition InitType1Custom(string name, string basetype, PDFFontWidths widths, PDFFontDescriptor desc)
        {
            PDFFontDefinition f = new PDFFontDefinition();
            f.SubType = FontType.Type1;
            f.BaseType = basetype;
            f.Encoding = FontEncoding.MacRomanEncoding;
            f.Widths = widths;
            f.Descriptor = desc;

            return f;
        }

        #endregion

        #region internal static PDFFontWidths InitWidth(int firstchar, int lastchar, int[] widths)

        internal static PDFFontWidths InitWidth(int firstchar, int lastchar, int[] widths)
        {
            PDFFontWidths w = new PDFFontWidths(firstchar, lastchar, widths);
            return w;
        }

        #endregion

        #region internal static PDFFontDescriptor InitDesc(int ascent, int capheight, int descent, int flags,string fontname, double italicangle, double stemv,int xheight, RectangleF bbox)

        internal static PDFFontDescriptor InitDesc(
            int ascent, int capheight, int descent, int flags,
            string fontname, int italicangle, int stemv,
            int xheight, int[] bbox)
        {
            PDFFontDescriptor fd = new PDFFontDescriptor();
            fd.Ascent = ascent;
            fd.BoundingBox = bbox;
            fd.CapHeight = capheight;
            fd.Descent = descent;
            fd.Flags = flags;
            fd.FontName = fontname;
            fd.ItalicAngle = italicangle;
            fd.StemV = stemv;
            fd.XHeight = xheight;

            return fd;
        }

        #endregion

    }

    /// <summary>
    /// Overrides the default behaviour to make sure that equals(FontStyle returns true whether the font is bold or italic
    /// </summary>
    public class PDFSymbolFontDefinition : PDFFontDefinition
    {

        public override bool SupportsVariants
        {
            get { return false; }
        }

        public PDFSymbolFontDefinition()
            : base()
        {
        }

    }
}
