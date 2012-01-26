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

namespace Scryber.Components.Support
{
    /// <summary>
    /// Represents a spanned cell (the actual content is referenced)
    /// </summary>
    internal class TableCellSpannedRef : TableCellRef
    {
        private TableCellContentRef _content;

        /// <summary>
        /// Gets the Cell type of this instance - Spanned
        /// </summary>
        internal override TableCellRefType CellType
        {
            get { return TableCellRefType.Spanned; }
        }

        /// <summary>
        /// Gets the content cell that this spanned cell is part of
        /// </summary>
        internal TableCellContentRef ContentCell
        {
            get { return _content; }
        }

        public TableCellSpannedRef(TableGrid grid, TableCellContentRef contentcell, PDFThickness margins, PDFThickness padding)
            : base(grid, margins, padding)
        {
            if (null == contentcell)
                throw new ArgumentNullException("content");
            _content = contentcell;
        }


        public override string ToString()
        {
            return "Spanned : R" + this.RowIndex + ",C" + this.ColumnIndex + " -> (" + this.ContentCell.ToString() + ")";
        }
    }
}
