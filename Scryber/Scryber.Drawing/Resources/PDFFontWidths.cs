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

namespace Scryber.Resources
{

    public class PDFFontWidths : PDFObject
    {

        private int _first;

        public int FirstChar
        {
            get { return _first; }
            set 
            { 
                _first = value;
                
            }
        }

        private int _last;

        public int LastChar
        {
            get { return _last; }
            set 
            { 
                _last = value; 
                
            }
        }

        private List<int> _widths = new List<int>(255);

        public int Count
        {
            get { return this._widths.Count; }
        }

        public void RemoveAt(int index)
        {
            this._widths.RemoveAt(index);
        }

        public void Clear()
        {
            this._widths.Clear();
        }

        public void AddRange(IEnumerable<int> ws)
        {
            this._widths.AddRange(ws);
        }

        public void Add(int w)
        {
            this._widths.Add(w);
        }

        public int this[int index]
        {
            get { return this._widths[index]; }
            set
            {
                this._widths[index] = value;
                
            }
        }


        public PDFFontWidths() : base(PDFObjectTypes.FontWidths)
        {
        }

        
        public PDFFontWidths(int first, int last, IEnumerable<int> widths): this()
        {
            this._first = first;
            this._last = last;
            this.AddRange(widths);
        }


        public PDFObjectRef RenderToPDF(PDFContextBase context, PDFWriter writer)
        {
            PDFObjectRef oref = writer.BeginObject();
            writer.WriteArrayNumberEntries(this._widths.ToArray());
            writer.EndObject();
            return oref;
        }

    }
}
