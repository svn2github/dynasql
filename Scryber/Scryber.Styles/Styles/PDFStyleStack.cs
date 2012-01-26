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
    /// <summary>
    /// Serves as a stack, but enumerates from the bottom to the top to merge items
    /// </summary>
    public class PDFStyleStack
    {
        private List<PDFStyle> _styles;

        public PDFStyleStack(PDFStyle root)
        {
            this._styles = new List<PDFStyle>();
            this._styles.Add(root);
        }

        [Obsolete("Just Temporary",true)]
        public PDFStyle this[int index]
        {
            get { return this._styles[index]; }
        }

        public void Push(PDFStyle style)
        {
            this._styles.Add(style);
        }

        public PDFStyle Pop()
        {
            if (this._styles.Count == 0)
                throw new ArgumentOutOfRangeException();
            PDFStyle last = this._styles[this._styles.Count - 1];
            this._styles.RemoveAt(this._styles.Count - 1);
            return last;
        }

        /// <summary>
        /// Creates a new style, populates all based upon the current styles
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        public PDFStyle GetFullStyle(IPDFComponent Component)
        {
            PDFStyle style = new PDFStyle();
            int last = this._styles.Count-1;
            if (last >= 0)
            {
                for (int i = 0; i < last; i++)
                {
                    this._styles[i].MergeInherited(style, Component, true);
                }
                this._styles[last].MergeInto(style, Component, ComponentState.Normal);
            }

            style = style.Flatten();

            return style;
        }

        public PDFStyle Current
        {
            get
            {
                if (this._styles.Count > 0)
                    return this._styles[this._styles.Count - 1];
                else
                    return null;
            }
        }

    }
}
