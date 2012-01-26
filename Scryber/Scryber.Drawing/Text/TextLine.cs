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

namespace Scryber.Text
{
    internal class TextLine : TextData
    {
        private List<TextSpan> _spans;

        public TextSpan[] TextSpans
        {
            get { return (_spans == null) ? (new TextSpan[] { }) : (_spans.ToArray()); }
        }

        public int SpanCount
        {
            get
            {
                return this._spans == null ? 0 : this._spans.Count;
            }
        }

        public override double Width
        {
            get
            {
                if (this._spans == null || _spans.Count == 0)
                    return 0.0;
                double w = base.Width;
                if (w < 0.0)
                {
                    w = 0.0;
                    foreach (TextSpan span in this._spans)
                    {
                        w += span.Width;
                    }
                    base.Width = w;
                }
                return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        private double _lineinset;

        public double LineInset
        {
            get { return _lineinset; }
            set { _lineinset = value; }
        }


        
        private double _assenth;

        public double AscentHeight
        {
            get { return _assenth; }
            set { _assenth = value; }
        }


        public void AddSpan(TextSpan span)
        {
            if (this._spans == null)
                this._spans = new List<TextSpan>();

            this._spans.Add(span);
            this.Width = -1.0;
        }

    }
}
