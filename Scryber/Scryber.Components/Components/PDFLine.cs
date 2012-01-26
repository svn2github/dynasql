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
using Scryber.Styles;
using Scryber.Drawing;
namespace Scryber.Components
{
    public class PDFLine : PDFShapeComponent
    {

        public PDFLine()
            : base(PDFObjectTypes.ShapeLine)
        {
        }

        public PDFColor Color
        {
            get { return this.Style.Stroke.Color; }
            set { this.Style.Stroke.Color = value; }
        }

        public PDFUnit LineWidth
        {
            get { return this.Style.Stroke.Width; }
            set { this.Style.Stroke.Width = value; }
        }


        protected override PDFGraphicsPath CreatePath(PDFSize available, PDFStyle fullstyle)
        {
            PDFPositionStyle pos;
            PDFPaddingStyle pad;
            PDFMarginsStyle marg;
            PDFThickness inset;
            
            //Because the explicit sizes are set based upon the margins and padding
            //we need to take account of these in the explicit heights.

            if (fullstyle.TryGetPadding(out pad))
                inset = pad.GetThickness();
            else
                inset = PDFThickness.Empty();

            if (fullstyle.TryGetMargins(out marg))
            {
                PDFThickness m = marg.GetThickness();
                inset = new PDFThickness(inset.Top + m.Top, inset.Left + m.Left, inset.Bottom + m.Bottom, inset.Right + m.Right);
            }

            PDFPoint start = new PDFPoint(inset.Left, inset.Top);
            PDFPoint end = new PDFPoint(inset.Left, inset.Top);
            
            if (fullstyle.TryGetPosition(out pos))
            {
                if (pos.IsDefined(StyleKeys.XAttr))
                    start.X = pos.X + inset.Left;
                if (pos.IsDefined(StyleKeys.YAttr))
                    start.Y = pos.Y + inset.Top;

                if (pos.IsDefined(StyleKeys.WidthAttr))
                {
                    end.X = pos.Width - (start.X + inset.Left + inset.Right);

                    if (pos.IsDefined(StyleKeys.HeightAttr))
                        end.Y = pos.Height - (start.Y + inset.Top + inset.Bottom);
                    else // no hight so this is a horizontal line
                        end.Y = start.Y;
                }
                //no width so this is a vertical line
                else if (pos.IsDefined(StyleKeys.HeightAttr))
                {
                    end.Y = pos.Height - (start.Y + inset.Top + inset.Bottom);
                    end.X = start.X;
                }
                else //default is a horizontal line
                {
                    end.X = available.Width - (start.X + inset.Left + inset.Right);
                    end.Y = start.Y;
                }
            }
            else
            {
                end.X = available.Width;
                end.Y = start.Y;
            }

            PDFGraphicsPath path = new PDFGraphicsPath();
            path.MoveTo(start);
            path.LineTo(end);

            return path;
        }
        
    }
}
