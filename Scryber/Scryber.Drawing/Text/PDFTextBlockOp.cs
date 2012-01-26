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
    public class PDFTextBlockOp
    {
        private int _charsfitter;

        public int CharsFitted
        {
            get { return _charsfitter; }
            set { this._charsfitter = value; }
        }

        private int _length;
        public int CharsLength
        {
            get { return _length; }
            set { this._length = value; }
        }

        public bool AllCharactersFitted
        {
            get { return this._charsfitter >= _length; }
        }

        private PDFUnit _firstlineh;

        public PDFUnit FirstLineHeight
        {
            get { return _firstlineh; }
            set { _firstlineh = value; }
        }


        private List<PDFTextOp> _ops;

        public List<PDFTextOp> InnerOps
        {
            get { return this._ops; }
        }

        private PDFTextRenderOptions _renderop;

        public PDFTextRenderOptions RenderOptions
        {
            get { return _renderop; }
            set { _renderop = value; }
        }


        private PDFSize _measuredSize;

        public PDFSize MeasuredSize
        {
            get { return _measuredSize; }
            set { _measuredSize = value; }
        }

        private PDFRect _rect;

        public PDFRect Bounds
        {
            get { return _rect; }
            set { _rect = value; }
        }

        private PDFSize _remainder;

        public PDFSize Remainder
        {
            get { return _remainder; }
            set { _remainder = value; }
        }


        public PDFTextBlockOp()
            : this(null)
        {

        }

        public PDFTextBlockOp(IEnumerable<PDFTextOp> ops)
        {
            this._ops = new List<PDFTextOp>(ops);
        }




    }
}
