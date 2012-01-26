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

namespace Scryber.Text
{
    internal class TextParagraph : TextData
    {
        private List<TextLine> _lines = null;

        internal TextLine[] Lines
        {
            get { return (_lines == null) ? (new TextLine[] { }) : (_lines.ToArray()); }
        }

        internal int LineCount
        {
            get
            {
                if (this._lines == null)
                    return 0;
                else
                    return _lines.Count;
            }
        }

        private PDFUnit _firstinset;

        public PDFUnit FirstLineInset
        {
            get
            {
                return _firstinset;
            }
            set
            {
                _firstinset = value;
            }
        }
        
        public override double Width
        {
            get
            {
                if (this.LineCount == 0)
                    return 0.0;
                double w = base.Width;

                if (w < 0.0)
                {
                    w = 0.0;
                    foreach (TextLine line in this._lines)
                    {
                        w = Math.Max(w, line.Width + line.LineInset);
                    }
                    base.Width = w;
                }
                return w;
            }
        }

        public override double Height
        {
            get
            {
                if (this.LineCount == 0)
                    return 0.0;
                double h = base.Height;
                if (h < 0.0)
                {
                    h = 0.0;
                    foreach (TextLine line in this._lines)
                    {
                        h += line.Height;
                    }
                    base.Height = h;
                }
                return h;
            }
        }


        internal void AddLine(TextLine textLine)
        {
            if (_lines == null)
                _lines = new List<TextLine>(1);
            _lines.Add(textLine);
            this.Width = -1.0;
            this.Height = -1.0;
            if (_lines.Count == 1)
                textLine.LineInset = this.FirstLineInset.RealValue.Value;
            else
                textLine.LineInset = 0.0;
        }
    }
}
