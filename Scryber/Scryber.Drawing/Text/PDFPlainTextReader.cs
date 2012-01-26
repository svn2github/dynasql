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
    public class PDFPlainTextReader : PDFTextReader
    {
        private string[] _lines;
        private int _index = -1;
        

        public override int Length
        {
            get
            {
                int count = 0;
                foreach (string s in _lines)
                {
                    if (s != "\n")
                        count += s.Length;
                }
                return count;
            }
        }
        public PDFPlainTextReader(string text)
            : base()
        {
            this._lines = string.IsNullOrEmpty(text) ? new string[] { } : text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (_lines.Length > 0)
            {
                List<string> act = new List<string>();
                foreach (string s in this._lines)
                {
                    act.Add(s.TrimEnd());
                    act.Add("\n");
                }
                act.RemoveAt(act.Count - 1);
                _lines = act.ToArray();
            }

            _index = -1;
        }

        public override bool Read()
        {
            _index++;
            _curr = null;
            return _index < _lines.Length;
        }

        public override bool EOF
        {
            get { return _index >= _lines.Length; }
        }

        private PDFTextOp _curr = null;

        public override PDFTextOp Value
        {
            get
            {
                if (_curr == null)
                {
                    if (_index < 0)
                        throw new ArgumentOutOfRangeException(Errors.CannotAccessStreamWithoutRead);
                    if (_index >= _lines.Length)
                        throw new ArgumentOutOfRangeException(Errors.CannotReadPastTheEOF);
                    string s = this._lines[_index];
                    if (s == "\n")
                        _curr = new PDFTextNewLineOp();
                    else
                        _curr = new PDFTextDrawOp(s);
                }
                return _curr;
            }
        }

        protected override void ResetTextMarkers()
        {
            this._index = -1;
            this._curr = null;
        }
    }
}
