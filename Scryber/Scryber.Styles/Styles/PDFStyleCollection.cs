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

namespace Scryber.Styles
{
    public class PDFStyleCollection : List<PDFStyleBase>
    {
        
        
        public new PDFStyleBase this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }

        public void MergeInto(PDFStyle style, IPDFComponent forComponent, ComponentState state)
        {
            for (int i = 0; i < this.Count; i++)
            {
                PDFStyleBase inner = this[i];
                inner.MergeInto(style, forComponent, state);
            }
            
        }

        internal PDFStyle MatchClass(string classname)
        {
            PDFStyle style = new PDFStyle();
            foreach (PDFStyleBase stylebase in this)
            {
                PDFStyle matched = stylebase.MatchClass(classname);
                if (matched != null)
                    matched.MergeInto(style);
            }
            return style;
        }
    }
}
