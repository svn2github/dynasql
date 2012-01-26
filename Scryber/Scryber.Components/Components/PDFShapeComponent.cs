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
using Scryber.Styles;
using Scryber.Native;

namespace Scryber.Components
{
    public abstract class PDFShapeComponent : PDFVisualComponent, IPDFGraphicPathComponent
    {

        public PDFShapeComponent(PDFObjectType type) : base(type) { }

        private PDFGraphicsPath _path;

        protected PDFGraphicsPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

        PDFGraphicsPath IPDFGraphicPathComponent.Path
        {
            get { return this.Path; }
            set { this.Path = value; }
        }

        protected abstract PDFGraphicsPath CreatePath(PDFSize available, PDFStyle fullstyle);

        PDFGraphicsPath IPDFGraphicPathComponent.CreatePath(PDFSize available, PDFStyle fullstyle)
        {
            return this.CreatePath(available, fullstyle);
        }

        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            if (null != this.Path)
            {
                PDFBrush brush = null;
                PDFPen pen = null;
                PDFStrokeStyle stroke;
                PDFFillStyle fill;

                if (fullstyle.TryGetStroke(out stroke))
                    pen = stroke.CreatePen();
                if (fullstyle.TryGetFill(out fill))
                    brush = fill.CreateBrush();

                
                graphics.RenderPath(brush, pen, context.Offset, this.Path);
            }

            return base.DoRenderToPDF(context, fullstyle, graphics, writer);
        }

    }
}
