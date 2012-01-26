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
using Scryber.Native;
using Scryber.Resources;

namespace Scryber.Drawing
{
    public abstract class PDFBrush : PDFGraphicsAdapter
    {
        public abstract FillStyle FillStyle {get;}

    }

    public class PDFSolidBrush : PDFBrush
    {
        public override FillStyle FillStyle
        {
            get { return FillStyle.Solid; }
        }

        private PDFColor _col;

        public PDFColor Color
        {
            get { return _col; }
            set { _col = value; }
        }

        private PDFReal _op;

        public PDFReal Opacity
        {
            get { return _op; }
            set { _op = value; }
        }

        public PDFSolidBrush()
            : this(null)
        {
        }

        public PDFSolidBrush(PDFColor color)
            : this(color, -1)
        {
        }

        public PDFSolidBrush(PDFColor color, double opacity)
            : this(color, (PDFReal)opacity)
        {
        }

        public PDFSolidBrush(PDFColor color, PDFReal opacity)
        {
            this._col = color;
            this._op = opacity;
        }

        public override void SetUpGraphics(PDFGraphics g, PDFRect bounds)
        {
            if (this.Opacity.Value >= 0.0)
            {
                g.SetFillOpacity(this.Opacity);
            }
            if(this.Color != null && this.Color.IsEmpty == false)
                g.SetFillColor(this.Color);
        }

        public override void ReleaseGraphics(PDFGraphics g, PDFRect bounds)
        {
            
        }


        public static PDFSolidBrush Create(PDFColor color)
        {
            return new PDFSolidBrush(color);
        }
    }

    public class PDFNoBrush : PDFBrush
    {
        public override FillStyle FillStyle
        {
            get { return FillStyle.None; }
        }

        public override void SetUpGraphics(PDFGraphics g, PDFRect bounds)
        {
        }

        public override void ReleaseGraphics(PDFGraphics g, PDFRect bounds)
        {
        }

        public PDFNoBrush()
            : base()
        {
        }
    }


    public class PDFImageBrush : PDFBrush
    {

        private PDFReal _op;

        public PDFReal Opacity
        {
            get { return _op; }
            set { _op = value; }
        }

        private string _source;

        public string ImageSource
        {
            get { return _source; }
            set { _source = value; }
        }

        private PDFUnit _imgw;

        public PDFUnit ImageWidth
        {
            get { return _imgw; }
            set { _imgw = value; }
        }

        private PDFUnit _imgh;

        public PDFUnit ImageHeight
        {
            get { return _imgh; }
            set { _imgh = value; }
        }

        private PDFUnit _xpos;
        /// <summary>
        /// Gets or sets the horizontal offset at which the pattern starts.
        /// The default 0 will use the left as the starting point
        /// </summary>
        public PDFUnit XPostion
        {
            get { return _xpos; }
            set { _xpos = value; }
        }

        private PDFUnit _ypos;
        /// <summary>
        /// Gets or sets the vertical offset at which the pattern starts.
        /// The default 0 will use the top as the starting point
        /// </summary>
        public PDFUnit YPostion
        {
            get { return _ypos; }
            set { _ypos = value; }
        }
        
        private PDFUnit _xstep;
        /// <summary>
        /// Gets or sets the horizontal repeat step that the pattern will move to render each pattern
        /// The default 0 will use the native dimensions of the image as the offset
        /// </summary>
        public PDFUnit XStep
        {
            get { return _xstep; }
            set { _xstep = value; }
        }

        private PDFUnit _ystep;
        /// <summary>
        /// Gets or sets the horizontal repeat step that the pattern will move to render each pattern. 
        /// The default 0 will use the native dimensions of the image as the offset
        /// </summary>
        public PDFUnit YStep
        {
            get { return _ystep; }
            set { _ystep = value; }
        }

        
        public override FillStyle FillStyle
        {
            get { return FillStyle.Image; }
        }

        public PDFImageBrush()
            : this(null)
        {
        }

        public PDFImageBrush(string source)
        {
            this._source = source;
        }


        public override void SetUpGraphics(PDFGraphics g, PDFRect bounds)
        {
            Scryber.Resources.PDFImageXObject imagex;

            string fullpath = g.Container.MapPath(_source);
            //TODO: Add XStep, YStep etc.
            string resourcekey = GetImagePatternKey(fullpath);
            

            PDFResource rsrc = g.Container.Document.GetResource(PDFResource.PatternResourceType,resourcekey,false);
            if (null == rsrc)
            {

                //The container of a pattern is the document as this is the scope
                PDFImageTilingPattern tile = new PDFImageTilingPattern(g.Container.Document, resourcekey);
                //Create the image
                imagex = g.Container.Document.GetResource(Scryber.Resources.PDFResource.XObjectResourceType, fullpath, true) as PDFImageXObject;
                tile.Image = imagex;
                tile.PaintType = PatternPaintType.ColoredTile;
                tile.TilingType = PatternTilingType.NoDistortion;

                //Calculate the bounds of the pattern

                PDFUnit width;
                PDFUnit height;
                PDFSize imgsize = CalculateAppropriateImageSize(imagex.ImageData);
                width = imgsize.Width;
                height = imgsize.Height;

                
                //Patterns are drawn from the bottom of the page so Y is the container height minus the vertical position and offset
                PDFUnit y = g.ContainerSize.Height - (bounds.Y + height + this.YPostion);
                //X is simply the horizontal position plus offset
                PDFUnit x = bounds.X + this.XPostion;

                tile.BoundingBox = new PDFRect(x, y, width, height);

                if (this.XStep == 0)
                    tile.XStep = width;
                else
                    tile.XStep = this.XStep;
                if (this.YStep == 0)
                    tile.YStep = height;
                else
                    tile.YStep = this.YStep;

                PDFName name = g.Container.Register(tile);
                
                g.SetFillPattern(name);
            }
        }

        private PDFSize CalculateAppropriateImageSize(PDFImageData imgdata)
        {
            if (this.ImageWidth > 0 && this.ImageHeight > 0)
            {
                //We have both explicit widths
                return new PDFSize(this.ImageWidth, this.ImageHeight);
            }

            PDFUnit imgw = new PDFUnit((double)imgdata.PixelWidth / imgdata.HorizontalResolution, PageUnits.Inches);
            PDFUnit imgh = new PDFUnit((double)imgdata.PixelHeight / imgdata.VerticalResolution, PageUnits.Inches);

            //If we have one dimension, then calculate the other proportionally.
            if (this.ImageWidth > 0)
            {
                imgh = this.ImageWidth * (imgh.PointsValue / imgw.PointsValue);
                imgw = this.ImageWidth;
            }
            else if (this.ImageHeight > 0)
            {
                imgw = this.ImageHeight * (imgw.PointsValue / imgh.PointsValue);
                imgh = this.ImageHeight;
            }

            return new PDFSize(imgw, imgh);
        }

        private const string IMAGEPATTERNRESOURCEKEY = "Scryber.Resources.ImageTile:{0}/{1}";

        private string GetImagePatternKey(string fullpath)
        {
            int code = this.GetHashCode(); //reference to this brush and image file
            return string.Format(IMAGEPATTERNRESOURCEKEY, fullpath.ToLower(), code);
        }

        public override void ReleaseGraphics(PDFGraphics g, PDFRect bounds)
        {
            g.ClearFillPattern();
        }
    }

    
}
