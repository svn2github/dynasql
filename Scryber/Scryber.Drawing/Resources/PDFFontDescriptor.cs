/*  Copyright 2012 PerceiveIT Limited
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
using System.Drawing;
using Scryber.Native;
using Scryber.Drawing;

namespace Scryber.Resources
{
    public class PDFFontDescriptor : PDFObject
    {
        private int _asc;

        public int Ascent
        {
            get { return _asc; }
            set 
            { 
                _asc = value;
                
            }
        }

        private int _weight;

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }


        private int _caps;

        public int CapHeight
        {
            get { return _caps; }
            set 
            { 
                _caps = value;
                
            }
        }

        private int _desc;

        public int Descent
        {
            get { return _desc; }
            set 
            { 
                _desc = value;
                
            }
        }

        private long _flags;

        public long Flags
        {
            get { return _flags; }
            set 
            { 
                _flags = value;
                
            }
        }

        private int[] _bbox;

        public int[] BoundingBox
        {
            get { return _bbox; }
            set 
            { 
                _bbox = value;
                
            }
        }

        private string _fname;

        public string FontName
        {
            get { return _fname; }
            set 
            { 
                _fname = value;
                
            }
        }

        private int _italic;

        public int ItalicAngle
        {
            get { return _italic; }
            set 
            {
                _italic = value;
                
            }
        }

        private int _stem;

        public int StemV
        {
            get { return _stem; }
            set 
            {
                _stem = value;
                
            }
        }

        private int _xheight;

        public int XHeight
        {
            get { return _xheight; }
            set 
            { 
                _xheight = value;
            }
        }

        private FontStretch _stretch;

        public FontStretch FontStretch
        {
            get { return _stretch; }
            set { _stretch = value; }
        }

        private string _ffamily;

        public string FontFamily
        {
            get { return _ffamily; }
            set { _ffamily = value; }
        }

        private int _lead;

        public int Leading
        {
            get { return _lead; }
            set { _lead = value; }
        }

        private int _stemh;

        public int StemH
        {
            get { return _stemh; }
            set { _stemh = value; }
        }

        private int _avgw;

        public int AvgWidth
        {
            get { return _avgw; }
            set { _avgw = value; }
        }

        private int _maxw;

        public int MaxWidth
        {
            get { return _maxw; }
            set { _maxw = value; }
        }

        private int _misw;

        public int MissingWidth
        {
            get { return _misw; }
            set { _misw = value; }
        }

        private byte[] _fontdata;

        public byte[] FontFile
        {
            get { return _fontdata; }
            set { _fontdata = value;  }
        }

        private FontType _ftype = FontType.TrueType;

        public FontType FontType
        {
            get { return _ftype; }
            set { _ftype = value; }
        }
        
        public PDFFontDescriptor() : base(PDFObjectTypes.FontDescriptor)
        {
            this.BoundingBox = null;
            this.FontStretch = FontStretch.Normal;
            this.FontFamily = String.Empty;
            this.Weight = 400;
            this.Flags = 0;
            this.Leading = 0;
            this.ItalicAngle = 25;
            this.StemH = 0;
            this.AvgWidth = 0;
            this.MaxWidth = 0;
            this.MissingWidth = 0;
        }

        public PDFObjectRef RenderToPDF(PDFContextBase context, PDFWriter writer)
        {
            PDFObjectRef oref = writer.BeginObject();
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type","FontDescriptor");
            writer.WriteDictionaryNameEntry("FontName",this.FontName);
            
            if (this.FontFamily != String.Empty)
                writer.WriteDictionaryStringEntry("FontFamily", this.FontFamily);

            if (this.BoundingBox != null && this.BoundingBox.Length > 0)
            {
                writer.BeginDictionaryEntry("FontBBox");
                writer.WriteArrayNumberEntries(this.BoundingBox);
                writer.EndDictionaryEntry();
            }

            if(this.FontStretch != FontStretch.Normal)
                writer.WriteDictionaryStringEntry("FontStretch", this.FontStretch.ToString());

            if(this.Weight != 400)
                writer.WriteDictionaryNumberEntry("FontWeight", this.Weight);

            writer.WriteDictionaryNumberEntry("FontWeight", 700);
            writer.WriteDictionaryNumberEntry("Flags", (int)this.Flags);
            writer.WriteDictionaryNumberEntry("Ascent",this.Ascent);
            writer.WriteDictionaryNumberEntry("Descent", this.Descent);

            if (this.Leading != 0.0)
                writer.WriteDictionaryNumberEntry("Leading", this.Leading);

            if(this.CapHeight != 0.0)
                writer.WriteDictionaryNumberEntry("CapHeight", this.CapHeight);

            if(this.XHeight != 0.0)
                writer.WriteDictionaryNumberEntry("XHeight", this.XHeight);

            writer.WriteDictionaryNumberEntry("StemV", this.StemV);
            
            writer.WriteDictionaryNumberEntry("ItalicAngle", this.ItalicAngle);

            if (this.StemH != 0.0)
                writer.WriteDictionaryNumberEntry("StemH", this.StemH);

            if (this.AvgWidth != 0.0)
                writer.WriteDictionaryNumberEntry("AvgWidth", this.AvgWidth);

            if (this.MaxWidth != 0.0)
                writer.WriteDictionaryNumberEntry("MaxWidth", this.MaxWidth);

            if (this.MissingWidth != 0.0)
                writer.WriteDictionaryNumberEntry("MissingWidth", this.MissingWidth);

            if (this.FontFile != null)
            {
                PDFObjectRef fileref = writer.BeginObject();
                writer.BeginDictionary();
                writer.WriteDictionaryNumberEntry("Length", this.FontFile.Length);
                if (this.FontType == FontType.TrueType)
                    writer.WriteDictionaryNumberEntry("Length1", this.FontFile.Length);
                else
                    throw new ArgumentOutOfRangeException("FontType");
                writer.EndDictionary();
                writer.BeginStream(fileref);
                writer.WriteRaw(this.FontFile, 0, this.FontFile.Length);
                writer.EndStream();
                writer.EndObject();

                //We know this is a true type font program from above
                writer.WriteDictionaryObjectRefEntry("FontFile2", fileref);
            }
            writer.EndDictionary();
            writer.EndObject();

            return oref;
            
        }
    }
}
