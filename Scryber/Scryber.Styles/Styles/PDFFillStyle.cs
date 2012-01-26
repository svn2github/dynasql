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
using Scryber.Drawing;
using Scryber.Native;
namespace Scryber.Styles
{
    [PDFParsableComponent("Fill")]
    public class PDFFillStyle : PDFStyleItem
    {

        //
        // constructors
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFFillStyle()
            : this(PDFObjectTypes.StyleFill, true)
        {
        }

        protected PDFFillStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        //
        // style properties
        //

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
                    return this.DefaultColor;
            }
            set { this.SetValue(StyleKeys.ColorAttr, value.ToString(), value); }
        }

        protected virtual PDFColor DefaultColor
        {
            get { return PDFStyleConst.DefaultFillColor; }
        }

        public void RemoveColor()
        {
            this.Remove(StyleKeys.ColorAttr);
        }

        #endregion

        #region public string ImageSource {get;set;} + RemoveImageSource()

        [PDFAttribute("img-src")]
        public string ImageSource
        {
            get
            {
                string val;
                if (this.GetStringValue(StyleKeys.ImgSrcAttr, false, out val))
                    return val;
                else
                    return String.Empty;
            }
            set { this.SetValue(StyleKeys.ImgSrcAttr, value, value); }
        }

        public void RemoveImageSource()
        {
            this.Remove(StyleKeys.ImgSrcAttr);
        }

        #endregion

        #region public PatternRepeat PatternRepeat {get;set;} + RemovePatternRepeat()

        [PDFAttribute("repeat")]
        public PatternRepeat PatternRepeat
        {
            get
            {
                object rep;
                if (this.GetEnumValue(StyleKeys.RepeatAttr, typeof(PatternRepeat), false, out rep))
                    return (PatternRepeat)rep;
                else
                    return PatternRepeat.RepeatBoth;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatAttr, value.ToString(), value);
            }
        }

        public void RemovePatternRepeat()
        {
            this.Remove(StyleKeys.RepeatAttr);
        }

        #endregion

        #region public PDFUnit PatternXPosition {get;set;} + RemovePatternXPosition()

        [PDFAttribute("x-pos")]
        public PDFUnit PatternXPosition
        {
            get
            {
                PDFUnit xpos;
                if (this.GetUnitValue(StyleKeys.RepeatPatternXPosition, false, out xpos))
                    return xpos;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternXPosition, value);
            }
        }

        public void RemovePatternXPosition()
        {
            this.Remove(StyleKeys.RepeatPatternXPosition);
        }

        #endregion

        #region public PDFUnit PatternYPosition {get;set;} + RemovePatternYPosition()

        [PDFAttribute("y-pos")]
        public PDFUnit PatternYPosition
        {
            get
            {
                PDFUnit ypos;
                if (this.GetUnitValue(StyleKeys.RepeatPatternYPosition, false, out ypos))
                    return ypos;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternYPosition, value);
            }
        }

        public void RemoveRepeatPatternYPosition()
        {
            this.Remove(StyleKeys.RepeatPatternYPosition);
        }

        #endregion

        #region public PDFUnit PatternXStep {get;set;} + RemovePatternXStep()

        [PDFAttribute("x-step")]
        public PDFUnit PatternXStep
        {
            get
            {
                PDFUnit xpos;
                if (this.GetUnitValue(StyleKeys.RepeatPatternXStep, false, out xpos))
                    return xpos;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternXStep, value);
            }
        }

        public void RemovePatternXStep()
        {
            this.Remove(StyleKeys.RepeatPatternXStep);
        }

        #endregion

        #region public PDFUnit PatternYStep {get;set;} + RemovePatternYStep()

        [PDFAttribute("y-step")]
        public PDFUnit PatternYStep
        {
            get
            {
                PDFUnit ystep;
                if (this.GetUnitValue(StyleKeys.RepeatPatternYStep, false, out ystep))
                    return ystep;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternYStep, value);
            }
        }

        public void RemovePatternYStep()
        {
            this.Remove(StyleKeys.RepeatPatternYStep);
        }

        #endregion


        #region public PDFUnit PatternXSize {get;set;} + RemovePatternXSize()

        [PDFAttribute("x-size")]
        public PDFUnit PatternXSize
        {
            get
            {
                PDFUnit imgw;
                if (this.GetUnitValue(StyleKeys.RepeatPatternWidth, false, out imgw))
                    return imgw;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternWidth, value);
            }
        }

        public void RemovePatternXSize()
        {
            this.Remove(StyleKeys.RepeatPatternWidth);
        }

        #endregion

        #region public PDFUnit PatternYSize {get;set;} + RemovePatternYSize()

        [PDFAttribute("y-size")]
        public PDFUnit PatternYSize
        {
            get
            {
                PDFUnit imgh;
                if (this.GetUnitValue(StyleKeys.RepeatPatternHeight, false, out imgh))
                    return imgh;
                else
                    return 0;
            }
            set
            {
                this.SetValue(StyleKeys.RepeatPatternHeight, value);
            }
        }

        public void RemovePatternYSize()
        {
            this.Remove(StyleKeys.RepeatPatternHeight);
        }

        #endregion

        #region public FillStyle Style {get; set} + RemoveFillStyle()

        [PDFAttribute("style")]
        public FillStyle FillStyle
        {
            get
            {
                object found;
                if (this.GetEnumValue(StyleKeys.FillStyleAttr, typeof(FillStyle), false, out found))
                    return (FillStyle)found;
                else
                    return FillStyle.None;
            }
            set
            {
                this.SetValue(StyleKeys.FillStyleAttr, value);
            }
        }

        public void RemoveFillStyle()
        {
            this.Remove(StyleKeys.FillStyleAttr);
        }

        #endregion

        #region public PDFReal Opacity {get; set} + RemoveOpacity()

        /// <summary>
        /// Gets or sets the opacity of the fill
        /// </summary>
        [PDFAttribute("opacity")]
        public PDFReal Opacity
        {
            get
            {
                double found;
                if (this.GetDoubleValue(StyleKeys.Opacity, false, out found))
                    return (PDFReal)found;
                else
                    return (PDFReal)1.0; //Default of 100% opacity
            }
            set
            {
                this.SetValue(StyleKeys.Opacity, value.Value);
            }
        }

        public void RemoveOpacity()
        {
            this.Remove(StyleKeys.Opacity);
        }

        #endregion


        public virtual PDFBrush CreateBrush()
        {
            return Create(this);
        }

        protected static PDFBrush Create(Styles.PDFFillStyle fill)
        {
            if (fill == null)
                throw new ArgumentNullException("fill");

            if (fill.IsDefined(StyleKeys.ImgSrcAttr))
            {
                PDFImageBrush brush = new PDFImageBrush(fill.ImageSource);
                PatternRepeat repeat = fill.PatternRepeat;

                if (repeat == PatternRepeat.RepeatX || repeat == PatternRepeat.RepeatBoth)
                    brush.XStep = fill.PatternXStep;
                else
                    brush.XStep = int.MaxValue;

                if (repeat == PatternRepeat.RepeatY || repeat == PatternRepeat.RepeatBoth)
                    brush.YStep = fill.PatternYStep;
                else
                    brush.YStep = int.MaxValue;

                brush.XPostion = fill.PatternXPosition;
                brush.YPostion = fill.PatternYPosition;
                if(fill.IsDefined(StyleKeys.RepeatPatternWidth))
                    brush.ImageWidth = fill.PatternXSize;
                if(fill.IsDefined(StyleKeys.RepeatPatternHeight))
                    brush.ImageHeight = fill.PatternYSize;

                if (fill.IsDefined(StyleKeys.Opacity))
                    brush.Opacity = fill.Opacity;
                
                return brush;
            }
            else if (fill.IsDefined(StyleKeys.Opacity))
                return new PDFSolidBrush(fill.Color, fill.Opacity);

            else if (fill.IsDefined(StyleKeys.ColorAttr))
                return new PDFSolidBrush(fill.Color);

            else
                return new PDFNoBrush();
        }
    }
}
