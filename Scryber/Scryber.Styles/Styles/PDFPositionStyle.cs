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
    [PDFParsableComponent("Position")]
    public class PDFPositionStyle : PDFStyleItem
    {

        //
        // constructors
        //

        #region .ctor()

        public PDFPositionStyle()
            : this(PDFObjectTypes.StylePosition, false)
        {
        }

        #endregion

        #region .ctor(type, inherited)

        protected PDFPositionStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion 

        //
        // style properties
        //

        #region public PositionMode PositionMode {get;set;} + RemovePositionMode()

        [PDFAttribute("position")]
        public PositionMode PositionMode
        {
            get
            {
                object val;
                if (GetEnumValue(StyleKeys.PositionModeAttr, typeof(PositionMode), false, out val))
                    return (PositionMode)val;
                else if (this.IsDefined(StyleKeys.XAttr) || this.IsDefined(StyleKeys.YAttr))
                    return PositionMode.Relative;
                else
                    return PositionMode.Flow;
            }
            set
            {
                this.SetValue(StyleKeys.PositionModeAttr, value.ToString(), value);
            }
        }

        public void RemovePositionMode()
        {
            this.Remove(StyleKeys.PositionModeAttr);
        }

        #endregion

        #region public LayoutMode LayoutMode {get;set;} + RemoveLayoutMode()

        [PDFAttribute("layout")]
        public LayoutMode LayoutMode
        {
            get
            {
                object val;
                if (GetEnumValue(StyleKeys.LayoutModeAttr, typeof(LayoutMode), false, out val))
                    return (LayoutMode)val;
                else
                    return LayoutMode.Inline;
            }
            set
            {
                this.SetValue(StyleKeys.LayoutModeAttr, value.ToString(), value);
            }
        }

        public void RemoveLayoutMode()
        {
            this.Remove(StyleKeys.LayoutModeAttr);
        }

        #endregion

        #region public PDFUnit X {get;set;} + RemoveLeft()

        [PDFAttribute("x")]
        public PDFUnit X
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.XAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.XAttr, value);
            }
        }

        public void RemoveX()
        {
            this.Remove(StyleKeys.XAttr);
        }

        #endregion

        #region public PDFUnit Y {get;set;} + RemoveTop()

        [PDFAttribute("y")]
        public PDFUnit Y
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.YAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.YAttr, value);
            }
        }

        public void RemoveY()
        {
            this.Remove(StyleKeys.YAttr);
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

        #region public VerticalAlignment VAlign {get;set;} + RemoveVAlign()

        [PDFAttribute("v-align")]
        public VerticalAlignment VAlign
        {
            get
            {
                object va;
                if (this.GetEnumValue(StyleKeys.VerticalAlignAttr, typeof(VerticalAlignment), false, out va))
                    return (VerticalAlignment)va;
                else
                    return PDFStyleConst.DefaultVerticalAlign;
            }
            set
            {
                this.SetValue(StyleKeys.VerticalAlignAttr, value.ToString(), value);
            }
        }

        public void RemoveVAlign()
        {
            this.Remove(StyleKeys.VerticalAlignAttr);
        }

        #endregion

        #region  public HorizontalAlignment HAlign {get;set;} + RemoveHAlign()

        [PDFAttribute("h-align")]
        public HorizontalAlignment HAlign
        {
            get
            {
                object va;
                if (this.GetEnumValue(StyleKeys.HorizontalAlignAttr, typeof(HorizontalAlignment), false, out va))
                    return (HorizontalAlignment)va;
                else
                    return PDFStyleConst.DefaultHorizontalAlign;
            }
            set
            {
                this.SetValue(StyleKeys.HorizontalAlignAttr, value.ToString(), value);
            }
        }

        public void RemoveHAlign()
        {
            this.Remove(StyleKeys.HorizontalAlignAttr);
        }

        #endregion

        [PDFAttribute(StyleKeys.ExpandWidthAttr)]
        public bool FillWidth
        {
            get
            {
                bool va;
                if (this.GetBoolValue(StyleKeys.ExpandWidthAttr, false, out va))
                    return va;
                else
                    return false;
            }
            set
            {
                this.SetValue(StyleKeys.ExpandWidthAttr, value);
            }
        }

        public void RemoveFillWidth()
        {
            this.Remove(StyleKeys.ExpandWidthAttr);
        }
        
    }
}
