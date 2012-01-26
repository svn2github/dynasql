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

namespace Scryber.Text
{
    public class PDFTextRenderOptions : ICloneable
    {
        private PDFFont _font;

        public PDFFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        private PDFPen _scolor;

        public PDFPen Stroke
        {
            get { return _scolor; }
            set { _scolor = value; }
        }

        private PDFBrush _brush;

        public PDFBrush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        private PDFBrush _bg;

        public PDFBrush Background
        {
            get { return _bg; }
            set { _bg = value; }
        }

        private PDFTextLayout _layout;

        public PDFTextLayout Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public PDFTextRenderOptions Clone()
        {
            return this.MemberwiseClone() as PDFTextRenderOptions;
        }


        
    }
}
