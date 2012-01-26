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
    [PDFParsableComponent("Border")]
    public class PDFBorderStyle : PDFStrokeStyle
    {
        

        //
        // constructors
        //

        #region .ctor() + .ctor(type,inherited)

        public PDFBorderStyle()
            : this(PDFObjectTypes.StyleBorder, false)
        {
        }

        protected PDFBorderStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        //
        // style properties
        //

        #region public PDFUnit CornerRadius {get;set;} + RemoveCornerRadius()

        [PDFAttribute("corner-radius")]
        public PDFUnit CornerRadius
        {
            get
            {
                PDFUnit rad;
                if (this.GetUnitValue(StyleKeys.CornerRadiusAttr, false, out rad))
                    return rad;
                else
                    return PDFUnit.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.CornerRadiusAttr, value.ToString(), value);
            }
        }

        public void RemoveCornerRadius()
        {
            this.Remove(StyleKeys.CornerRadiusAttr);
        }

        #endregion

        #region public Sides Sides {get; set;} + RemoveSides()

        [PDFAttribute("sides")]
        public Sides Sides
        {
            get
            {
                object side;
                if(this.GetFlagsEnumValue(StyleKeys.SidesAttr,typeof(Sides),false,out side))
                    return (Sides)side;
                else
                    return (Sides.Left | Sides.Right | Sides.Top | Sides.Bottom);
            }
            set
            {
                this.SetValue(StyleKeys.SidesAttr, value.ToString(), value);
            }
        }

        public void RemoveSides()
        {
            this.Remove(StyleKeys.SidesAttr);
        }

        #endregion

        //#region public LineStyle LineStyle {get;set;} + RemoveLineStyle()

        //public LineStyle LineStyle
        //{
        //    get
        //    {
        //        object val;
        //        if (this.GetEnumValue(StyleKeys.StyleAttr, typeof(LineStyle), false, out val))
        //            return (LineStyle)val;
        //        else
        //            return LineStyle.None;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.StyleAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveLineStyle()
        //{
        //    this.Remove(StyleKeys.StyleAttr);
        //}

        //#endregion

        //#region public PDFColor Color {get;set;} + RemoveColor()

        //public PDFColor Color
        //{
        //    get
        //    {
        //        PDFColor col;
        //        if (this.GetColorValue(StyleKeys.ColorAttr, false, out col))
        //            return col;
        //        else
        //            return PDFColor.Transparent;
        //    }
        //    set { this.SetValue(StyleKeys.ColorAttr, value.ToString(), value); }
        //}

        //public void RemoveColor()
        //{
        //    this.Remove(StyleKeys.ColorAttr);
        //}

        //#endregion

        //#region public PDFUnit Width {get;set;} + RemoveWidth()

        //public PDFUnit Width
        //{
        //    get
        //    {
        //        PDFUnit f;
        //        if (this.GetUnitValue(StyleKeys.WidthAttr, false, out f))
        //            return f;
        //        else
        //            return PDFUnit.Empty;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.WidthAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveWidth()
        //{
        //    this.Remove(StyleKeys.WidthAttr);
        //}

        //#endregion

        //#region public PDFDash Dash {get;set;} + RemoveDash()

        //public PDFDash Dash
        //{
        //    get
        //    {
        //        PDFDash dash;
        //        if (this.GetDashValue(StyleKeys.DashAttr, false, out dash))
        //            return dash;
        //        else
        //            return PDFDash.None;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.DashAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveDash()
        //{
        //    this.Remove(StyleKeys.DashAttr);
        //}

        //#endregion

        //#region public LineCaps LineCap {get;set;} + RemoveLineCap()

        //public LineCaps LineCap
        //{
        //    get
        //    {
        //        object cap;
        //        if (this.GetEnumValue(StyleKeys.LineEndingAttr, typeof(LineCaps), false, out cap))
        //            return (LineCaps)cap;
        //        else
        //            return Const.DefaultLineCaps;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.LineEndingAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveLineCap()
        //{
        //    this.Remove(StyleKeys.LineEndingAttr);
        //}

        //#endregion

        //#region public LineJoin LineJoin {get;set;} + RemoveLineJoin()

        //public LineJoin LineJoin
        //{
        //    get
        //    {
        //        object join;
        //        if (this.GetEnumValue(StyleKeys.LineJoinAttr, typeof(LineJoin), false, out join))
        //            return (LineJoin)join;
        //        else
        //            return Const.DefaultLineJoin;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.LineJoinAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveLineJoin()
        //{
        //    this.Remove(StyleKeys.LineJoinAttr);
        //}

        //#endregion

        //#region public float Mitre {get;set;} + RemoveMitre()

        //public float Mitre
        //{
        //    get
        //    {
        //        float f;
        //        if (this.GetFloatValue(StyleKeys.LineMitreAttr, false, out f))
        //            return f;
        //        else
        //            return 0.0F;
        //    }
        //    set
        //    {
        //        this.SetValue(StyleKeys.LineMitreAttr, value.ToString(), value);
        //    }
        //}

        //public void RemoveMitre()
        //{
        //    this.Remove(StyleKeys.LineMitreAttr);
        //}

        //#endregion
    }
}
