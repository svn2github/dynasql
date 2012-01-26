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
    /// represetns an empty cell in a table
    /// </summary>
    internal class TableCellEmptyRef : TableCellRef
    {
        /// <summary>
        /// Gets the Cell type of this instance - Blank
        /// </summary>
        internal override TableCellRefType CellType
        {
            get { return TableCellRefType.Blank; }
        }


        internal TableCellEmptyRef(TableGrid grid, PDFThickness margins, PDFThickness padding)
            : base(grid, margins, padding)
        {
        }

        internal override void RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            if (context.TraceLog.RecordLevel == TraceRecordLevel.All)
                context.TraceLog.Add(TraceLevel.Debug, "TableGrid", "Empty cell at location '" + this.ColumnIndex + "C, " + this.RowIndex + "R");
                
            base.RenderToPDF(context, writer);
        }

        public override string ToString()
        {
            return "Empty : R" + this.RowIndex + ",C" + this.ColumnIndex;
        }
    }
}
