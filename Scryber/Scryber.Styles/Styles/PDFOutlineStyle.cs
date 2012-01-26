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
    [PDFParsableComponent("Outline")]
    public class PDFOutlineStyle : PDFStyleItem
    {
        //
        // ctor(s)
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFOutlineStyle()
            : this(PDFObjectTypes.StyleOutline, false)
        {
        }

        protected PDFOutlineStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        #region public bool IsOutlined {get;set;} + RemoveOutline()

        /// <summary>
        /// Default is true - so titles will be shown when they are set
        /// </summary>
        [PDFAttribute(StyleKeys.IsOutlineAttr)]
        public bool IsOutlined
        {
            get
            {
                bool outline;
                if (this.GetBoolValue(StyleKeys.IsOutlineAttr, false, out outline))
                    return outline;
                else
                    return true;
            }
            set
            {
                this.SetValue(StyleKeys.IsOutlineAttr, value.ToString(), value);
            }
        }

        public void RemoveOutline()
        {
            this.Remove(StyleKeys.IsOutlineAttr);
        }

        #endregion

        #region public PDFColor Color {get;set;} + RemoveColor()

        [PDFAttribute(StyleKeys.ColorAttr)]
        public PDFColor Color
        {
            get
            {
                PDFColor outline;
                if (this.GetColorValue(StyleKeys.ColorAttr, false, out outline))
                    return outline;
                else
                    return PDFColors.Transparent;
            }
            set
            {
                this.SetValue(StyleKeys.ColorAttr, value.ToString(), value);
            }
        }

        public void RemoveColor()
        {
            this.Remove(StyleKeys.ColorAttr);
        }

        #endregion

        #region public bool FontBold {get;set;} + RemoveFontBold()

        [PDFAttribute(StyleKeys.FontBoldAttr)]
        public bool FontBold
        {
            get
            {
                bool bold;
                if (this.GetBoolValue(StyleKeys.FontBoldAttr, false, out bold))
                    return bold;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.FontBoldAttr, value.ToString(), value);
            }
        }

        public void RemoveFontBold()
        {
            this.Remove(StyleKeys.FontBoldAttr);
        }

        #endregion

        #region public bool FontItalic {get;set;} + RemoveFontItalic()

        [PDFAttribute(StyleKeys.FontItalicAttr)]
        public bool FontItalic
        {
            get
            {
                bool ital;
                if (this.GetBoolValue(StyleKeys.FontItalicAttr, false, out ital))
                    return ital;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.FontItalicAttr, value.ToString(), value);
            }
        }

        public void RemoveFontItalic()
        {
            this.Remove(StyleKeys.FontItalicAttr);
        }

        #endregion

        #region public bool Open {get;set;} + RemoveOpen()

        [PDFAttribute(StyleKeys.OutlineOpen)]
        public bool Open
        {
            get
            {
                bool open;
                if (this.GetBoolValue(StyleKeys.OutlineOpen, false, out open))
                    return open;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.OutlineOpen, value.ToString(), value);
            }
        }

        public void RemoveOpen()
        {
            this.Remove(StyleKeys.OutlineOpen);
        }

        #endregion
    }
}
