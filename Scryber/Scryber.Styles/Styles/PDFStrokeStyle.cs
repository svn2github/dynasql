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
    [PDFParsableComponent("Stroke")]
    public class PDFStrokeStyle : PDFStyleItem
    {
        //
        //constructors
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFStrokeStyle()
            : this(PDFObjectTypes.StyleStroke, true)
        {
        }

        protected PDFStrokeStyle(PDFObjectType type, bool inherited)
            : base(type,inherited)
        {
        }

        #endregion

        //
        // public properties
        //

        #region public LineStyle LineStyle {get;set;} + RemoveLineStyle()

        [PDFAttribute("style")]
        public LineStyle LineStyle
        {
            get
            {
                object val;
                if (this.GetEnumValue(StyleKeys.StyleAttr, typeof(LineStyle), false, out val))
                    return (LineStyle)val;
                else if (this.IsDefined(StyleKeys.DashAttr) && this.Dash != PDFDash.None)
                    return LineStyle.Dash;
                else if (this.IsDefined(StyleKeys.ColorAttr))
                    return LineStyle.Solid;
                else
                    return LineStyle.None;
            }
            set
            {
                this.SetValue(StyleKeys.StyleAttr, value.ToString(), value);
            }
        }

        public void RemoveLineStyle()
        {
            this.Remove(StyleKeys.StyleAttr);
        }

        #endregion

        #region public PDFColor Color {get;set;} + RemoveColor()

        [PDFAttribute("color")]
        public PDFColor Color
        {
            get
            {
                PDFColor col;
                if (this.GetColorValue(StyleKeys.ColorAttr, false, out col))
                    return col;
                else
                    return PDFColor.Transparent;
            }
            set { this.SetValue(StyleKeys.ColorAttr, value.ToString(), value); }
        }

        public void RemoveColor()
        {
            this.Remove(StyleKeys.ColorAttr);
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
                this.SetValue(StyleKeys.WidthAttr, value.ToString(), value);
            }
        }

        public void RemoveWidth()
        {
            this.Remove(StyleKeys.WidthAttr);
        }

        #endregion

        #region public PDFDash Dash {get;set;} + RemoveDash()

        [PDFAttribute("dash")]
        public PDFDash Dash
        {
            get
            {
                PDFDash dash;
                if (this.GetDashValue(StyleKeys.DashAttr, false, out dash))
                    return dash;
                else
                    return PDFDash.None;
            }
            set
            {
                this.SetValue(StyleKeys.DashAttr, value.ToString(), value);
            }
        }

        public void RemoveDash()
        {
            this.Remove(StyleKeys.DashAttr);
        }

        #endregion

        #region public LineCaps LineCap {get;set;} + RemoveLineCap()

        [PDFAttribute("ending")]
        public LineCaps LineCap
        {
            get
            {
                object cap;
                if (this.GetEnumValue(StyleKeys.LineEndingAttr, typeof(LineCaps), false, out cap))
                    return (LineCaps)cap;
                else
                    return PDFStyleConst.DefaultLineCaps;
            }
            set
            {
                this.SetValue(StyleKeys.LineEndingAttr, value.ToString(), value);
            }
        }

        public void RemoveLineCap()
        {
            this.Remove(StyleKeys.LineEndingAttr);
        }

        #endregion

        #region public LineJoin LineJoin {get;set;} + RemoveLineJoin()

        [PDFAttribute("join")]
        public LineJoin LineJoin
        {
            get
            {
                object join;
                if (this.GetEnumValue(StyleKeys.LineJoinAttr, typeof(LineJoin), false, out join))
                    return (LineJoin)join;
                else
                    return PDFStyleConst.DefaultLineJoin;
            }
            set
            {
                this.SetValue(StyleKeys.LineJoinAttr, value.ToString(), value);
            }
        }

        public void RemoveLineJoin()
        {
            this.Remove(StyleKeys.LineJoinAttr);
        }

        #endregion

        #region public float Mitre {get;set;} + RemoveMitre()

        [PDFAttribute("mitre")]
        public float Mitre
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.LineMitreAttr, false, out f))
                    return f;
                else
                    return 0.0F;
            }
            set
            {
                this.SetValue(StyleKeys.LineMitreAttr, value.ToString(), value);
            }
        }

        public void RemoveMitre()
        {
            this.Remove(StyleKeys.LineMitreAttr);
        }

        #endregion

        #region public float Opacity {get; set} + RemoveOpacity()

        /// <summary>
        /// Gets or sets the opacity of the fill
        /// </summary>
        [PDFAttribute("opacity")]
        public double Opacity
        {
            get
            {
                double found;
                if (this.GetDoubleValue(StyleKeys.Opacity, false, out found))
                    return found;
                else
                    return 1.0; //Default of 100% opacity
            }
            set
            {
                this.SetValue(StyleKeys.Opacity, value);
            }
        }

        public void RemoveOpacity()
        {
            this.Remove(StyleKeys.Opacity);
        }

        #endregion

        public virtual PDFPen CreatePen()
        {
            return Create(this);
        }

        public static PDFPen Create(Styles.PDFStrokeStyle style)
        {

            PDFPen pen;
            LineStyle penstyle = style.LineStyle;
            PDFColor c = style.Color;
            if (penstyle == LineStyle.None)
            {
                if (style.IsDefined(StyleKeys.ColorAttr))
                {
                    if (style.IsDefined(StyleKeys.DashAttr))
                        pen = new PDFSolidPen();
                    else
                        pen = new PDFDashPen();
                }
                else
                    return null;
            }
            else if (penstyle == LineStyle.Dash)
            {
                pen = new PDFDashPen();
                (pen as PDFDashPen).Dash = style.Dash;
            }
            else
            {
                pen = new PDFSolidPen();
            }

            if (style.IsDefined(StyleKeys.ColorAttr))
                (pen as PDFSolidPen).Color = style.Color;

            if (style.IsDefined(StyleKeys.LineJoinAttr))
                pen.LineJoin = style.LineJoin;

            if (style.IsDefined(StyleKeys.LineEndingAttr))
                pen.LineCaps = style.LineCap;

            if (style.IsDefined(StyleKeys.LineMitreAttr))
                pen.MitreLimit = style.Mitre;

            if (style.IsDefined(StyleKeys.WidthAttr))
                pen.Width = style.Width;

            if (style.IsDefined(StyleKeys.Opacity))
                pen.Opacity = (Native.PDFReal)style.Opacity;

            return pen;
        }
        
    }
}
