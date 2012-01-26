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
using Scryber.Native;
using Scryber.Resources;
using Scryber.Styles;

namespace Scryber.Components
{
    [PDFParsableComponent("Image")]
    public class PDFImage : PDFVisualComponent, IPDFImageComponent
    {
        private string _src;

        [PDFAttribute("src")]
        public string Source
        {
            get { return _src; }
            set 
            {
                if (value != _src)
                {
                    if (null != _xobj)
                        _xobj = null;

                    _src = value;

                }
                
            }
        }

        private PDFImageData _data;

        [PDFParserIgnore()]
        public PDFImageData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _xobj = null;
                _src = null == _data ? string.Empty : _data.SourcePath;
            }
        }

        private PDFImageXObject _xobj = null;

        protected PDFImageXObject XObject
        {
            get { return _xobj; }
            set { _xobj = value; }
        }
        
        public PDFImage()
            : this(PDFObjectTypes.Image)
        {
        }

        protected PDFImage(PDFObjectType type)
            : base(type)
        {
        }


        public PDFImageXObject GetImageObject()
        {
            if (null == this._xobj)
                this.InitImageXObject();

            return this._xobj;
        }

        private Resources.PDFImageXObject InitImageXObject()
        {
            PDFDocument doc = this.Document;
            if (null == doc)
                throw RecordAndRaise.NullReference(Errors.ParentDocumentCannotBeNull);

            if (null != this.Data)
            {
                _xobj = this.Document.GetImageResource(this.Data.SourcePath, false);
                if (null == _xobj)
                {
                    string name = this.Document.GetIncrementID(PDFObjectTypes.ImageXObject);
                    _xobj = PDFImageXObject.Load(this.Data, name);
                    this.Document.SharedResources.Add(_xobj);
                }
            }
            else if (String.IsNullOrEmpty(this.Source) == false)
            {
                string fullpath = this.MapPath(this.Source);
                _xobj = this.Document.GetImageResource(fullpath, true);
                if(null == _xobj)
                {
                    PDFImageData data = this.Document.LoadImageData(fullpath);
                    string name = this.Document.GetIncrementID(PDFObjectTypes.ImageXObject);
                    _xobj = PDFImageXObject.Load(data, name);
                    this.Document.SharedResources.Add(_xobj);
                }
            }

                
            return _xobj;
        }

        /// <summary>
        /// Overrides the base implementation to register the ImageXObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fullstyle"></param>
        protected override void DoRegisterArtefacts(PDFRegistrationContext context, PDFStyle fullstyle)
        {
            IPDFResourceContainer resources = this.GetResourceContainer();
            if (null == resources)
                throw RecordAndRaise.NullReference(Errors.ResourceContainerOfComponnetNotFound, "Image", this.ID);
            PDFImageXObject xobj = this.GetImageObject();
            resources.Register(xobj);
        }

        internal static PDFSize AdjustImageSize(PDFStyle style, PDFSize size)
        {
            PDFSize rendersize = size;
            PDFPositionStyle pos;

            if (style.TryGetPosition(out pos))
            {
                bool scaleWidth = false; //proportional flags
                bool scaleHeight = false;

                if (pos.IsDefined(StyleKeys.WidthAttr))
                {
                    rendersize.Width = pos.Width;
                    scaleHeight = true;
                }
                if (pos.IsDefined(StyleKeys.HeightAttr))
                {
                    rendersize.Height = pos.Height;
                    scaleWidth = true;
                }

                
                if (scaleWidth && scaleHeight)
                {
                    //Do nothing as the size is set for both height and width.
                }
                else if (scaleWidth)
                {
                    double val = rendersize.Width.PointsValue;
                    double scale = rendersize.Height.PointsValue / size.Height.PointsValue;
                    rendersize.Width = (PDFUnit)(val * scale);
                }
                else if (scaleHeight)
                {
                    double val = rendersize.Height.PointsValue;
                    double scale = rendersize.Width.PointsValue / size.Width.PointsValue;
                    rendersize.Height = (PDFUnit)(val * scale);
                }

                size = rendersize;
            }
            return size;
        }
        



        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            PDFImageXObject img = this.GetImageObject();
            if (img != null)
            {
                PDFPoint pos = context.Offset;
                
                
                PDFSize imgsize = context.Space;

                //the pictures are drawn from their bottom left corner, so take off the height.
                if (context.DrawingOrigin == DrawingOrigin.TopLeft)
                    pos.Y = context.PageSize.Height - pos.Y - imgsize.Height;
                
                graphics.SaveGraphicsState();

                PDFFillStyle fill;
                if (context.StyleStack.Current.TryGetFill(out fill))
                {
                    PDFReal op = fill.Opacity;
                    if (op < 1.0)
                    {
                        graphics.SetFillOpacity(op);
                    }
                }
                img.EnsureRendered(context, writer);
                graphics.PaintImageRef(img, imgsize, pos);
                graphics.RestoreGraphicsState();

            }
            return base.DoRenderToPDF(context, fullstyle,graphics, writer);
        }

    }
}
