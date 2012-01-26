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
    internal abstract class TableCellRef
    {
        private PDFThickness _padding;
        private PDFThickness _margins;
        private TableGrid _grid;
        private int _rowindex;
        private int _colindex;

        /// <summary>
        /// Gets the Cell type of this instance
        /// </summary>
        internal abstract TableCellRefType CellType { get; }

        /// <summary>
        /// Gets or sets the owner grid of this cell reference
        /// </summary>
        internal TableGrid Grid { get { return _grid; } }

        /// <summary>
        /// Gets or sets the row index of the inner table cell
        /// </summary>
        internal int RowIndex { get { return _rowindex; } set { _rowindex = value; } }

        /// <summary>
        /// Gets or sets the column index of the inner table cell
        /// </summary>
        internal int ColumnIndex { get { return _colindex; } set { this._colindex = value; } }

        /// <summary>
        /// Gets or sets the padding on the cell
        /// </summary>
        internal PDFThickness Padding { get { return _padding; } }

        /// <summary>
        /// Gets or sets the margins on the cell
        /// </summary>
        internal PDFThickness Margins { get { return _margins; } }

        internal TableCellRef(TableGrid grid, PDFThickness margins, PDFThickness padding)
        {
            this._grid = grid;
            this._margins = margins;
            this._padding = padding;
        }

        internal virtual PDFSize Layout(PDFSize avail, int pageindex, PDFLayoutContext context)
        {
            return PDFSize.Empty;
        }

        /// <summary>
        /// Sets the cell arrangement and returns the bounds rectangle
        /// </summary>
        /// <returns></returns>
        internal virtual PDFRect SetArrangement()
        {
            return PDFRect.Empty;
        }

        protected PDFPoint GetLocation()
        {
            return new PDFPoint((this.ColumnIndex * 42) + 1, (this.RowIndex * 42) + 1);
        }

        protected virtual PDFSize GetSize()
        {
            return new PDFSize(40, 40);
        }

        internal virtual void RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            
        }
    }
}
