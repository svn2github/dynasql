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

namespace Scryber
{
    public class PDFRenderContext : PDFContextStyleBase
    {

        private int _pgindex;

        /// <summary>
        /// Gets or sets the page index of this rendercontext (first page is ZERO)
        /// </summary>
        public int PageIndex
        {
            get { return _pgindex; }
            set 
            {
                _pgindex = value;
                if (this.TraceLog.ShouldLog(TraceLevel.Debug))
                    TraceLog.Add(TraceLevel.Debug, "RENDER", "Page index incremented to '" + _pgindex.ToString() + "'");
            }
        }

        private int _pgCount;

        public int PageCount
        {
            get { return _pgCount; }
            set { _pgCount = value; }
        }

        private DrawingOrigin _origin = DrawingOrigin.TopLeft;

        public DrawingOrigin DrawingOrigin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        private PDFPoint _offset = PDFPoint.Empty;

        public PDFPoint Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private PDFSize _space;

        public PDFSize Space
        {
            get { return _space; }
            set { _space = value; }
        }


        private PDFSize _size = PDFSize.Empty;

        public PDFSize PageSize
        {
            get { return this._size; }
            set { this._size = value; }
        }



        public PDFRenderContext(DrawingOrigin origin, int pageCount, Styles.PDFStyle root, PDFItemCollection items, PDFTraceLog log)
            : this(origin,pageCount,new Scryber.Styles.PDFStyleStack(root), items, log)
        {
        }

        internal PDFRenderContext(DrawingOrigin origin, int pageCount, Styles.PDFStyleStack stack, PDFItemCollection items, PDFTraceLog log) 
            : base(stack, items, log)
        {
            this._origin = origin;
            this._offset = new PDFPoint();
            this._space = new PDFSize();
            this._pgCount = pageCount;
            this._pgindex = 0;
            
        }
	

        
    }
}
