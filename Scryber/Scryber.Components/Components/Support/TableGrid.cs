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
using Scryber.Drawing;
using Scryber.Styles;

namespace Scryber.Components.Support
{
    internal class TableGrid
    {
        private PDFTable _tbl;
        private PDFStyleStack _styles;
        private PDFStyle _tableStyle;

        private PDFUnit _cellpadding;
        private PDFUnit _cellspacing;
        private PDFSize _actsize;

        private TableGridSizes _explicit = null;
        private TableGridSizes _calculated = null;
        private TableGridSizes _measured = null;
        private TableGridSizes _actual = null;

        private PDFThickness _defmarg;
        private PDFThickness _defpad;

        private IPDFLayoutEngine _engine;
        private List<int> _pgbreaks;

        private TableColumnList _columns = new TableColumnList();

        internal PDFTable Table { get { return _tbl; } }

        internal IPDFLayoutEngine Engine { get { return _engine; } }

        internal PDFStyleStack Styles { get { return _styles; } }

        internal TableColumnList Columns { get { return _columns; } }

        internal PDFUnit CellSpacing { get { return _cellspacing; } set { _cellspacing = value; _defmarg = new PDFThickness(value); } }

        /// <summary>
        /// Gets or sets the padding on all the cells
        /// </summary>
        internal PDFUnit CellPadding { get { return _cellpadding; } set { _cellpadding = value; _defpad = new PDFThickness(value); } }

        /// <summary>
        /// Gets the actual final size of the table
        /// </summary>
        internal PDFSize ActualSize { get { return _actsize; } set { _actsize = value; } }

        /// <summary>
        /// Gets the Explicit sizes of the columns and rows table grid
        /// </summary>
        internal TableGridSizes ExplicitSizes
        {
            get
            {
                if (null == _explicit)
                    throw RecordAndRaise.Operation(Errors.GridNotClosed);
                return _explicit;
            }
        }

        /// <summary>
        /// Gets the table calculated sizes
        /// </summary>
        internal TableGridSizes CalculatedSizes
        {
            get
            {
                if (null == _explicit)//explcit is set at the same time as all the others. But cleared when a cell is added
                    throw RecordAndRaise.Operation(Errors.GridNotClosed);
                return _calculated;
            }
        }

        /// <summary>
        /// Gets the Actual sizes of the columns and rows table grid
        /// </summary>
        internal TableGridSizes ActualSizes
        {
            get
            {
                if (null == _explicit)//explcit is set at the same time as all the others. But cleared when a cell is added
                    throw RecordAndRaise.Operation(Errors.GridNotClosed);
                return _actual;
            }
        }

        /// <summary>
        /// Gets the Measured sizes of the columns and rows table grid
        /// </summary>
        internal TableGridSizes Measured
        {
            get
            {
                if (null == _explicit)//explcit is set at the same time as all the others. But cleared when a cell is added
                    throw RecordAndRaise.Operation(Errors.GridNotClosed);
                return _measured;
            }
        }

        /// <summary>
        /// Gets the Cell reference at the specified row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        internal TableCellRef this[int row, int col]
        {
            get { return this._columns[col][row]; }
        }

        internal PDFStyle TableStyle
        {
            get { return this._tableStyle; }
        }

        /// <summary>
        /// Gets the total number of coluns in this grid
        /// </summary>
        internal int ColumnCount { get { return _columns.Count; } }

        /// <summary>
        /// Gets the total number of rows in this grid
        /// </summary>
        internal int RowCount { get { return _columns.Count == 0 ? 0 : _columns[0].Count; } }

        internal TableGrid(PDFTable table, IPDFLayoutEngine engine, PDFStyleStack styles, PDFStyle tablestyle)
        {
            this._tbl = table;
            this._styles = styles;
            this._engine = engine;
            this._defpad = new PDFThickness(0);
            this._defmarg = new PDFThickness(0);
            this._tableStyle = tablestyle;
        }

        
        internal PDFObjectRef[] RenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            foreach (TableColumn tc in this._columns)
            {
                foreach (TableCellRef tcref in tc)
                {
                    tcref.RenderToPDF(context, writer);
                }
            }
            return new PDFObjectRef[] { };
        }

        internal void SetArrangement(int pageindex)
        {
            PDFSize fullbounds = PDFSize.Empty;
            
            for (int c = 0; c < this.ColumnCount; c++)
            {
                for (int r = 0; r < this.RowCount; r++)
                {
                    PDFRect cellbounds = this[r, c].SetArrangement();
                    
                    PDFUnit right = cellbounds.X + cellbounds.Width;
                    if (right > fullbounds.Width)
                        fullbounds.Width = right;

                    PDFUnit bottom = cellbounds.Y + cellbounds.Height;
                    if (bottom > fullbounds.Height)
                        fullbounds.Height = bottom;

                }
            }

            this._actsize = fullbounds;
        }


        /// <summary>
        /// Closes the grid by adding empty cells to any short columns so that a full square grid is created.
        /// Must be called after populating the grid with cells
        /// </summary>
        internal void CloseGrid()
        {
            
            int maxrow = 0;
            foreach (TableColumn tc in this._columns)
            {
                if (tc.Count > maxrow)
                    maxrow = tc.Count;
            }

            foreach (TableColumn tc in this._columns)
            {
                while (tc.Count < maxrow)
                {
                    TableCellEmptyRef empty = new TableCellEmptyRef(this, _defmarg, this._defpad);
                    empty.RowIndex = tc.Count;
                    empty.ColumnIndex = tc.ColumnIndex;
                    tc.Add(empty);
                }
            }

            _explicit = new TableGridSizes(this.ColumnCount, maxrow);
            _calculated = new TableGridSizes(this.ColumnCount, maxrow);
            _actual = new TableGridSizes(this.ColumnCount, maxrow);
            _measured = new TableGridSizes(this.ColumnCount, maxrow);
            
        }


        internal TableCellContentRef AddCellWithStyle(PDFTableCell cell, PDFStyle full, PDFStyle applied, int rowindex, int mincol, out int nextcolumn)
        {
            _explicit = null;
           

            int colcount = cell.ColumnSpan;
            int rowcount = cell.RowSpan;
            
            PDFThickness margins = this._defmarg;
            PDFThickness padding = this._defpad;
            PDFMarginsStyle ms;
            PDFPaddingStyle ps;
            if (full.TryGetMargins(out ms))
                margins = ms.GetThickness();
            if (full.TryGetPadding(out ps))
                padding = ps.GetThickness();

            int colindex = GetNextColumnWithSpace(rowindex, mincol, colcount);

            TableCellContentRef cref = new TableCellContentRef(this, cell, full, applied, margins, padding);
            cref.ColumnIndex = colindex;
            cref.RowIndex = rowindex;
            this.AddContentCell(cref);
            
            nextcolumn = cref.ColumnIndex + cref.ColumnSpan;

            return cref;
        }

        private void AddContentCell(TableCellContentRef cref)
        {
            for (int c = 0; c < cref.ColumnSpan; c++)
            {
                int cindex = cref.ColumnIndex + c;

                for (int r = 0; r < cref.RowSpan; r++)
                {
                    int rindex = cref.RowIndex + r;
                    TableCellRef actref;
                    if (c == 0 && r == 0)
                        actref = cref;
                    else
                    {
                        actref = new TableCellSpannedRef(this, cref, _defmarg, _defpad);
                        actref.ColumnIndex = cindex;
                        actref.RowIndex = rindex;
                    }
                    this.AddCellAt(actref, cindex, rindex);
                }
            }
        }

        private void AddCellAt(TableCellRef cref, int colindex, int rowindex)
        {
            while (this.ColumnCount <= colindex)
            {
                TableColumn col = new TableColumn(this.ColumnCount, this.RowCount);
                this._columns.Add(col);
            }

            this.AddToColumnAtRow(cref, this._columns[colindex], rowindex);
        }

        private void AddToColumnAtRow(TableCellRef cref, TableColumn col, int rowindex)
        {
            
            if (rowindex >= col.Count)
            {
                while (rowindex > col.Count)
                {
                    TableCellEmptyRef empty = new TableCellEmptyRef(this, _defmarg, _defpad);
                    empty.RowIndex = col.Count;
                    empty.ColumnIndex = col.ColumnIndex;
                    col.Add(empty);
                }
            
                col.Add(cref);
            }
            else
            {
                TableCellRef exist = col[rowindex];
                if (exist.CellType != TableCellRefType.Blank)
                    throw RecordAndRaise.Argument("rowindex", Errors.CannotAddCellToIndexAlreadyFull, rowindex);
                col[rowindex] = cref;
            }

           
        }

        private int GetNextColumnWithSpace(int rowindex, int mincol, int colcount)
        {
            int colindex = mincol;
            bool free;
            do
            {
                TableColumn col = EnsureColumn(colindex);
                free = IsFreeColumn(colindex, rowindex, colcount);
                if (!free)
                    colindex ++;

            } while (free == false);

            return colindex;
        }


        /// <summary>
        /// Check the column(s) at the rowindex to makes sure they are free (no content or spanned cells)
        /// </summary>
        /// <param name="colindex"></param>
        /// <param name="rowindex"></param>
        /// <param name="colcount"></param>
        /// <returns></returns>
        private bool IsFreeColumn(int colindex, int rowindex, int colcount)
        {
            for (int i = 0; i < colcount; i++)
            {
                int curr = colindex + i;
                if (curr < this.ColumnCount && !this._columns[curr].IsFree(rowindex))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Ensures that the columns are available up to index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private TableColumn EnsureColumn(int index)
        {
            while(this._columns.Count <= index)
            {
                this._columns.Add(new TableColumn(index, this.RowCount));
            }
            return this._columns[index];
        }


        #region internal void EnsureColumnsWideEnough(TableCellContentRef content, PDFUnit totalwidth)

        /// <summary>
        /// Takes a content cell with a colspan > 1 and tries to make sure that
        /// 1. If there is one variable column this column has it's width set to the remainder
        /// 2. Or if there are no variable columns then the explict space is sufficient 
        ///                                (if not increase the size of the last column).
        /// </summary>
        /// <param name="content"></param>
        /// <param name="totalwidth"></param>
        internal void EnsureColumnsWideEnough(TableCellContentRef content, PDFUnit totalwidth)
        {
            //Cannot do anything with a single column span or no explicit width
            if (content.ExplictWidth <= 0)
                return;
            if (content.ColumnSpan == 1)
                return;

            PDFUnit remainder = content.ExplictWidth;
            List<int> varCols = new List<int>();
            int varColCount = 0;
            int varColIndex = -1;

            for (int i = 0; i < content.ColumnSpan; i++)
            {
                int colindex = content.ColumnIndex + i;
                if (this.ExplicitSizes.HasWidth(colindex))
                    remainder -= this.ExplicitSizes.GetWidth(colindex);
                else
                {
                    varColCount++;
                    varCols.Add(colindex);
                    varColIndex = colindex;
                }
            }
            
            //TODO: Create a unit test for the ColumnSize cell scenarios

            if (varColCount == 1) //we have one variable column
            {
                if (remainder > 0)
                    this.ExplicitSizes.SetWidth(varColIndex, remainder);
            }
            else if (varColCount == 0)//all columns are fixed size
            {
                if (remainder < 0)//but they are not big enough so increase the last one
                {
                    this.ExplicitSizes.SetWidth(varColIndex, this.ExplicitSizes.GetWidth(varColIndex) + remainder);
                }
            }
            else if (varColCount > 1)
            {
                //Default behavour is just to increase the last column to fit
                if (remainder > 0)
                {
                    remainder = remainder / varColCount;
                    foreach (int colindex in varCols)
                    {
                        this.ExplicitSizes.SetWidth(colindex, remainder);
                    }
                    
                }
            }
        }

        #endregion

        #region internal void EnsureRowsHighEnough(TableCellContentRef content, PDFUnit totalheight)

        /// <summary>
        /// Takes a content cell with a rowspan > 1 and tries to make sure that
        /// 1. If there is one variable column this column has it's width set to the remainder
        /// 2. Or if there are no variable columns then the explict space is sufficient 
        ///                                (if not increase the size of the last column).
        /// </summary>
        /// <param name="content"></param>
        /// <param name="totalwidth"></param>
        internal void EnsureRowsHighEnough(TableCellContentRef content, PDFUnit totalheight)
        {
            //Cannot do anything with a single spanned row or no explicit height
            if (content.RowSpan == 0)
                return;
            if (content.ExplicitHeight <= 0)
                return;

            PDFUnit remainder = content.ExplicitHeight;
            int varRowCount = 0;
            int varRowIndex = -1;

            for (int i = 0; i < content.RowSpan; i++)
            {
                if (this.ExplicitSizes.HasHeight(content.RowIndex + i))
                    remainder -= this.ExplicitSizes.GetHeight(content.RowIndex + i);
                else
                {
                    varRowCount++;
                    varRowIndex = content.RowIndex + 1;
                }
            }

            //TODO: Create a unit test for the ColumnSize cell height scenarios

            if (varRowCount == 1) //we have one variable column
            {
                if (remainder > 0)
                    this.CalculatedSizes.SetHeight(varRowIndex, remainder);
            }
            else if (varRowCount == 0)//all columns are fixed size
            {
                if (remainder < 0)//but they are not big enough so increase the last one
                {
                    this.CalculatedSizes.SetWidth(varRowIndex, this.ExplicitSizes.GetWidth(varRowIndex) + remainder);
                }
            }
            else if (varRowCount > 1)
            {
                // Indeterminate state. More than one variable column
            }
        }

        #endregion


        #region internal void TryInferCellWidth(TableCellContentRef content)

        /// <summary>
        /// Tries to calculate the width of the cell from the 
        /// explicit width of the spanned column widths
        /// </summary>
        /// <param name="content"></param>
        internal void TryInferCellWidth(TableCellContentRef content)
        {
            if (content.ColumnSpan > 1)
            {
                bool allknown = true;
                PDFUnit total = PDFUnit.Zero;
                for (int i = 0; i < content.ColumnSpan; i++)
                {
                    if (!this.ExplicitSizes.HasWidth(content.ColumnIndex + i))
                    {
                        allknown = false;
                        break;
                    }
                    else
                        total += this.ExplicitSizes.GetWidth(content.ColumnIndex + i);
                }
                if(allknown)
                    content.ExplictWidth = total;
                
            }
        }

        #endregion

        #region internal void TryInferCellHeight(TableCellContentRef content)

        /// <summary>
        /// Tries to calculate the height of the cell from the 
        /// explicit height of the spanned row heights
        /// </summary>
        /// <param name="content"></param>
        internal void TryInferCellHeight(TableCellContentRef content)
        {
            if (content.RowSpan > 1)
            {
                bool allknown = true;
                PDFUnit total = PDFUnit.Zero;

                for (int i = 0; i < content.RowSpan; i++)
                {

                    if (!this.ExplicitSizes.HasHeight(content.RowIndex + i))
                    {
                        allknown = false;
                        break;
                    }
                    else
                        total += this.ExplicitSizes.GetWidth(content.RowIndex + i); ;
                }
                if (allknown)
                    content.ExplictWidth = total;

            }
        }

        #endregion


        #region internal PDFUnit GetTotalExplicitWidth(out int[] variable)

        /// <summary>
        /// Calculates the total known width of the grid and also returns the indexes of the 
        /// columns whose widths are not known (variable)
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        internal PDFUnit GetTotalExplicitWidth(out int[] variable)
        {
            PDFUnit tot = PDFUnit.Zero;
            List<int> variables = new List<int>();

            for (int c = 0; c < this.ColumnCount; c++)
			{
                if (this.ExplicitSizes.HasWidth(c))
                    tot += this.ExplicitSizes.GetWidth(c);
                else
                    variables.Add(c);
            }

            variable = variables.ToArray();
            return tot;
        }

        #endregion

        #region internal PDFUnit GetTotalExplicitHeight(out int[] variable)

        /// <summary>
        /// Calculates the total known height of the grid and also returns the indexes of the 
        /// rows whose heights are not known (variable)
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        internal PDFUnit GetTotalExplicitHeight(out int[] variable)
        {
            PDFUnit tot = PDFUnit.Zero;
            List<int> variables = new List<int>();

            for (int r = 0; r < this.RowCount; r++)
            {
                if (this.ExplicitSizes.HasHeight(r))
                    tot += this.ExplicitSizes.GetHeight(r);
                else
                    variables.Add(r);
            }
            
            variable = variables.ToArray();
            return tot;
        }

        #endregion

        /// <summary>
        /// Returns the total content size based upon the explicit widths of the columns and heights of the rows
        /// </summary>
        /// <param name="content"></param>
        /// <param name="unassignedWidth">The width to use if a cell width has not been explicitly set</param>
        /// <param name="unasignedHeight">The height to use if a cell height has not been explicitly set</param>
        /// <returns></returns>
        internal PDFSize GetLayoutSize(TableCellContentRef content, PDFUnit unassignedWidth, PDFUnit unasignedHeight)
        {
            PDFUnit w = PDFUnit.Zero;
            PDFUnit h = PDFUnit.Zero;

            if (content.ColumnSpan == 1)
            {
                if (this.ExplicitSizes.HasWidth(content.ColumnIndex))
                    w = this.ExplicitSizes.GetWidth(content.ColumnIndex);
                else if (this.CalculatedSizes.HasWidth(content.ColumnIndex))
                    w = this.CalculatedSizes.GetWidth(content.ColumnIndex);
                else
                    w = unassignedWidth;
            }
            else
            {
                for (int i = 0; i < content.ColumnSpan; i++)
                {
                    if (this.ExplicitSizes.HasWidth(content.ColumnIndex + i))
                        w += this.ExplicitSizes.GetWidth(content.ColumnIndex + i);
                    else if (this.CalculatedSizes.HasWidth(content.ColumnIndex + i))
                        w += this.CalculatedSizes.GetWidth(content.ColumnIndex + i);
                    else
                        w += unassignedWidth;
                }
            }
            if (content.RowSpan == 1)
            {
                if (this.ExplicitSizes.HasHeight(content.RowIndex))
                    h = this.ExplicitSizes.GetHeight(content.RowIndex);
                else if (this.CalculatedSizes.HasHeight(content.RowIndex))
                    h = this.CalculatedSizes.GetHeight(content.RowIndex);
                else
                    h = unasignedHeight;
            }
            else
            {
                for (int i = 0; i < content.RowSpan; i++)
                {
                    if (this.ExplicitSizes.HasHeight(content.RowIndex + i))
                        h += this.ExplicitSizes.GetWidth(content.RowIndex + i);
                    else if (this.CalculatedSizes.HasHeight(content.RowIndex + i))
                        h += this.CalculatedSizes.GetHeight(content.RowIndex + i);
                    else
                        h += unasignedHeight;
                }
            }

            return new PDFSize(w, h);
        }

        internal PDFSize GetMeasuredSize(TableCellContentRef content)
        {
            PDFUnit w = PDFUnit.Zero;
            PDFUnit h = PDFUnit.Zero;

            if (content.ColumnSpan == 1)
            {
                if (this.ExplicitSizes.HasWidth(content.ColumnIndex))
                    w = this.ExplicitSizes.GetWidth(content.ColumnIndex);
                else
                    w = this.Measured.GetWidth(content.ColumnIndex);
            }
            else
            {
                for (int i = 0; i < content.ColumnSpan; i++)
                {
                    if (this.ExplicitSizes.HasWidth(content.ColumnIndex + i))
                        w += this.ExplicitSizes.GetWidth(content.ColumnIndex + i);
                    else
                        w += this.Measured.GetWidth(content.ColumnIndex + i);
                }
            }

            if (content.RowSpan == 1)
            {
                if (this.ExplicitSizes.HasHeight(content.RowIndex))
                    h = this.ExplicitSizes.GetHeight(content.RowIndex);
                else
                    h = this.Measured.GetHeight(content.RowIndex);
            }
            else
            {
                for (int i = 0; i < content.RowSpan; i++)
                {
                    if (this.ExplicitSizes.HasHeight(content.RowIndex + i))
                        h += this.ExplicitSizes.GetHeight(content.RowIndex + i);
                    else
                        h += this.Measured.GetHeight(content.RowIndex + i);
                }
            }

            return new PDFSize(w, h);
        }

        internal void AddPageBreakRow(int index)
        {
            if (null == _pgbreaks)
                _pgbreaks = new List<int>();
            if (index > 0)
                _pgbreaks.Add(index - 1);
        }

        /// <summary>
        /// Gets the position of a cell based on previous cell heights and rows - could be faster
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal PDFPoint GetLocation(TableCellContentRef content)
        {
            PDFUnit w = PDFUnit.Zero;
            PDFUnit h = PDFUnit.Zero;

            for (int c = 0; c < content.ColumnIndex; c++)
            {
                if (this.ExplicitSizes.HasWidth(c))
                    w += this.ExplicitSizes.GetWidth(c);
                else
                    w += this.Measured.GetWidth(c);
            }

            for (int r = 0; r < content.RowIndex; r++)
            {
                //If we are on a page break then reset the height
                if (_pgbreaks != null && _pgbreaks.IndexOf(r) >= 0)
                    h = 0;
                else
                {
                    if (this.ExplicitSizes.HasHeight(r))
                        h += this.ExplicitSizes.GetHeight(r);
                    else
                        h += this.Measured.GetHeight(r);
                }
            }
            return new PDFPoint(w, h);
        }

        internal void ResetCalculations()
        {
            this._calculated = new TableGridSizes(this.ColumnCount, this.RowCount);
        }

        internal void ResetExplicits()
        {
            this._explicit = new TableGridSizes(this.ColumnCount, this.RowCount);
        }
    }

}
