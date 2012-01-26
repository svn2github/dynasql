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
    [PDFParsableComponent("Page")]
    public class PDFPageStyle : PDFStyleItem
    {

        #region .ctor() + .ctor(type)

        public PDFPageStyle()
            : this(PDFObjectTypes.StylePage, true)
        {
        }

        protected PDFPageStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        #region public PaperSize PaperSize {get;set;} + RemovePaperSize()

        

        [PDFAttribute("size")]
        public PaperSize PaperSize
        {
            get
            {
                object val;
                if (this.GetEnumValue(StyleKeys.PaperSizeAttr, typeof(PaperSize), false, out val))
                    return (PaperSize)val;
                else
                    return PDFStyleConst.DefaultPaperSize;
            }
            set
            {
                this.SetValue(StyleKeys.PaperSizeAttr, value.ToString(), value);
            }
        }

        public void RemovePaperSize()
        {
            this.Remove(StyleKeys.PaperSizeAttr);
        }

        #endregion

        #region public PaperOrientation PaperOrientation {get;set} + RemovePaperOrientation()

        

        [PDFAttribute("orientation")]
        public PaperOrientation PaperOrientation
        {
            get
            {
                object val;
                if (this.GetEnumValue(StyleKeys.PaperOrientationAttr, typeof(PaperOrientation), false, out val))
                    return (PaperOrientation)val;
                else
                    return PDFStyleConst.DefaultPaperOrientation;
            }
            set
            {
                this.SetValue(StyleKeys.PaperOrientationAttr, value.ToString(), value);
            }
        }

        public void RemovePaperOrientation()
        {
            this.Remove(StyleKeys.PaperOrientationAttr);
        }

        #endregion

        #region public PDFUnit Width {get;set;} + RemoveWidth()

        [PDFAttribute("width")]
        public PDFUnit Width
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.WidthAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.WidthAttr, value);
            }
        }

        public void RemoveWidth()
        {
            this.Remove(StyleKeys.WidthAttr);
        }

        #endregion

        #region public PDFUnit Height {get;set;} + RemoveHeight()

        [PDFAttribute("height")]
        public PDFUnit Height
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.HeightAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.HeightAttr, value);
            }
        }

        public void RemoveHeight()
        {
            this.Remove(StyleKeys.HeightAttr);
        }

        #endregion

        #region public PageNumberStyle NumberStyle {get;set;} + NumberStyle()

        [PDFAttribute("number-style")]
        public PageNumberStyle NumberStyle
        {
            get
            {
                object style;
                if (this.GetEnumValue(StyleKeys.PageNumberStyleAttr, typeof(PageNumberStyle), false, out style))
                    return (PageNumberStyle)style;
                else
                    return PageNumberStyle.Decimals;
            }
            set
            {
                this.SetValue(StyleKeys.PageNumberStyleAttr, value);
            }
        }

        public void RemoveNumberStyle()
        {
            this.Remove(StyleKeys.PageNumberStyleAttr);
        }

        #endregion

        #region public string NumberPrefix {get;set;}


        [PDFAttribute("number-prefix")]
        public string NumberPrefix
        {
            get
            {
                string pref;
                if (this.GetStringValue(StyleKeys.PageNumberPrefix, false, out pref))
                    return pref;
                else
                    return string.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.PageNumberPrefix, value);
            }
        }

        public void RemoveNumberPrefix()
        {
            this.Remove(StyleKeys.PageNumberPrefix);
        }

        #endregion

        #region public int NumberStartIndex {get;set;}

        [PDFAttribute("number-start-index")]
        public int NumberStartIndex
        {
            get
            {
                int start;
                if (this.GetIntegerValue(StyleKeys.PageNumberStart, false, out start))
                    return start;
                else
                    return 1;
            }
            set
            {
                this.SetValue(StyleKeys.PageNumberStart, value);
            }
        }

        public void RemoveNumberStartIndex()
        {
            this.Remove(StyleKeys.PageNumberStart);
        }

        #endregion 


        public virtual PDFPageSize CreatePageSize()
        {
            PDFUnit w = this.Width;
            PDFUnit h = this.Height;
            if (w != PDFUnit.Empty && h != PDFUnit.Empty)
                return new PDFPageSize(new PDFSize(w, h));
            else
                return new PDFPageSize(this.PaperSize, this.PaperOrientation);
        }

        public virtual PDFPageNumbering CreateNumbering()
        {
            PDFPageNumbering num = new PDFPageNumbering();
            num.NumberStyle = this.NumberStyle;
            num.Prefix = this.NumberPrefix;
            num.StartIndex = this.NumberStartIndex;
            return num;
        }
    }
}
