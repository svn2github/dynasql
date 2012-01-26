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
using Scryber.Drawing;

namespace Scryber.Styles
{
    [PDFParsableComponent("Font")]
    public class PDFFontStyle : PDFStyleItem
    {

        //
        //constructors
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFFontStyle()
            : this(PDFObjectTypes.StyleFont, true)
        {
        }

        public PDFFontStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        //
        // style properties
        //

        #region public string FontFamily {get;set;} + RemoveFontFamily()

        [PDFAttribute("family")]
        public string FontFamily
        {
            get
            {
                string s;
                if (this.GetStringValue(StyleKeys.FontFamilyAttr, false, out s))
                    return s;
                else
                    return string.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.FontFamilyAttr, value);
            }
        }

        public void RemoveFontFamily()
        {
            this.Remove(StyleKeys.FontFamilyAttr);
        }

        #endregion

        #region public bool FontBold {get;set;} + RemoveFontBold()

        [PDFAttribute("bold")]
        public bool FontBold
        {
            get
            {
                bool b;
                if (this.GetBoolValue(StyleKeys.FontBoldAttr, false, out b))
                    return b;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.FontBoldAttr, value);
            }
        }

        public void RemoveFontBold()
        {
            this.Remove(StyleKeys.FontBoldAttr);
        }

        #endregion

        #region public bool FontItalic {get;set;} + RemoveFontItalic()

        [PDFAttribute("italic")]
        public bool FontItalic
        {
            get
            {
                bool b;
                if (this.GetBoolValue(StyleKeys.FontItalicAttr, false, out b))
                    return b;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.FontItalicAttr, value);
            }
        }

        public void RemoveFontItalic()
        {
            this.Remove(StyleKeys.FontItalicAttr);
        }

        #endregion

        #region public PDFUnit FontSize {get;set;} + RemoveFontSize()

        [PDFAttribute("size")]
        public PDFUnit FontSize
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.FontSizeAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.FontSizeAttr, value);
            }
        }

        public void RemoveFontSize()
        {
            this.Remove(StyleKeys.FontSizeAttr);
        }

        #endregion

        public virtual PDFFont CreateFont()
        {
            return Create(this);
        }

        protected static PDFFont Create(Styles.PDFFontStyle fontstyle)
        {
            if (fontstyle == null)
                throw new ArgumentNullException("fontstyle");

            string family = fontstyle.FontFamily;
            if (string.IsNullOrEmpty(family))
                family = PDFStyleConst.DefaultFontFamily;

            PDFUnit size = fontstyle.FontSize;
            if (size == PDFUnit.Empty)
                size = new PDFUnit(PDFStyleConst.DefaultFontSize, PageUnits.Points);

            
            FontStyle fs = FontStyle.Regular;
            if (fontstyle.FontBold)
                fs |= FontStyle.Bold;

            if (fontstyle.FontItalic)
                fs |= FontStyle.Italic;

            PDFFont font = new PDFFont(family, size, fs);
            
            return font;
        }
    }
}
