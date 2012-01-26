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
using Scryber.Styles;

namespace Scryber.Components.Support
{
    /// <summary>
    /// Abstract base class for all layout strategies
    /// </summary>
    internal abstract class TableLayoutStrategy
    {


        #region event RequestNewPage + OnRequestNewPage

        public event PDFPageRequestHandler RequestNewPage;

        protected virtual void OnRequestNewPage(PDFPageRequestArgs args)
        {
            if (null != RequestNewPage)
            {
                this.RequestNewPage(this, args);

            }
        }

        #endregion

        #region internal PDFSize AvailableSize {get; set;}
        
        private PDFSize _availablesize;

        /// <summary>
        /// Gets or sets the total available size
        /// </summary>
        internal PDFSize AvailableSize
        {
            get { return _availablesize; }
            set { _availablesize = value; }
        }

        #endregion

        int _startpg, _currpg;

        protected int StartPageIndex
        {
            get { return _startpg; }
            set { _startpg = value; }
        }

        protected int CurrentPageIndex
        {
            get { return _currpg; }
            set { _currpg = value; }
        }

        OverflowAction _defoverflow = OverflowAction.NewPage;

        public OverflowAction DefaultOverflowAction
        {
            get { return _defoverflow; }
            set { _defoverflow = value; }
        }

        public PDFLayoutContext LayoutContext { get; private set; }

        internal TableLayoutStrategy(PDFSize available, int startPageIndex, PDFLayoutContext context)
        {
            if (null == context)
                throw RecordAndRaise.ArgumentNull("context");

            this._availablesize = available;
            this.StartPageIndex = startPageIndex;
            this.CurrentPageIndex = startPageIndex;
            this.LayoutContext = context;
        }

        internal void LayoutGrid(TableGrid grid)
        {
            if (null == grid)
                throw RecordAndRaise.ArgumentNull("grid");
           
            
            try
            {
                this.DoLayoutGrid(grid);
            }
            catch (PDFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw RecordAndRaise.LayoutException(ex, Errors.CouldNotLayoutGridCells, grid.Table, ex.Message);
            }
        }

        internal abstract void DoLayoutGrid(TableGrid grid);
    }


    /// <summary>
    /// A strategy for laying out a table with a layout of left to right direction
    /// </summary>
    internal class TableLeftToRightLayoutStrategy : TableLayoutStrategy
    {
        #region public PDFUnit ExplicitWidth {get;}

        private PDFUnit _expwidth;
        
        /// <summary>
        /// Gets the actual width of the table
        /// </summary>
        public PDFUnit ExplictWidth
        {
            get { return _expwidth; }
        }

        #endregion

        #region public PDFUnit ExplicitHeight {get;}

        private PDFUnit _expheight;

        /// <summary>
        /// Gets the actual width of the table
        /// </summary>
        public PDFUnit ExplictHeight
        {
            get { return _expheight; }
        }

        #endregion

        #region public TableLeftToRightLayoutStrategy(PDFSize available, PDFUnit width, PDFUnit height, int pageIndex)

        public TableLeftToRightLayoutStrategy(PDFSize available, PDFUnit width, PDFUnit height, int pageIndex, PDFLayoutContext context)
            : base(available, pageIndex, context)
        {
            this._expwidth = width;
            this._expheight = height;
        }

        #endregion


        internal override void DoLayoutGrid(TableGrid grid)
        {
            ApplyExplictWidthsToGrid(grid);
            ApplyExpicitHeightsToGrid(grid);
            UpdateWidthsForSpannedColumns(grid);
            grid.ExplicitSizes.Lock();

            CopyExplicitToCalculatedForSingleCell(grid);
            //See if we can infer any explict widths of multispan content cells
            
            int[] variable;
            PDFUnit total = grid.GetTotalExplicitWidth(out variable);

            if (this.ExplictWidth > 0)
                ApplyRemainingWidthToVariableColumns(grid, total, variable);


            UpdateHeightsForSpannedRows(grid);
            total = grid.GetTotalExplicitHeight(out variable);
            if (this.ExplictHeight > 0)
                ApplyRemainingHeightToVariableRows(grid, total, variable);

            LayoutCellContents(grid);

            //TODO:Check the available space and offset
        }

        private void CopyExplicitToCalculatedForSingleCell(TableGrid grid)
        {
            int ccount = grid.ColumnCount;
            int rcount = grid.RowCount;
            PDFUnit[] widths = new PDFUnit[ccount];
            PDFUnit[] heights = new PDFUnit[rcount];

            for (int r = 0; r < rcount; r++)
            {
               for (int c = 0; c < ccount; c++)
                {
                    TableCellRef cref = grid[r, c];
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = cref as TableCellContentRef;
                        if (content.ColumnSpan == 1 && content.RowSpan == 1)
                        {
                            if (content.ExplictWidth > 0)
                                widths[c] = PDFUnit.Max(widths[c], content.ExplictWidth);
                            if (content.ExplicitHeight > 0)
                                heights[r] = PDFUnit.Max(heights[r], content.ExplicitHeight);
                        }
                    }
                }
            }

            for (int r = 0; r < rcount; r++)
            {
                grid.CalculatedSizes.SetHeight(r, heights[r]);
            }
            for (int c = 0; c < ccount; c++)
            {
                grid.CalculatedSizes.SetWidth(c, widths[c]);
            }
        }

        protected virtual void LayoutCellContents(TableGrid grid)
        {
            PDFUnit maxGridWidth = this.ExplictWidth;
            PDFUnit maxGridHeight = this.ExplictHeight;

            if (maxGridWidth < 0)
                maxGridWidth = this.AvailableSize.Width;
            if (maxGridHeight < 0)
                maxGridHeight = this.AvailableSize.Height;

            int[] variableWidths;
            int[] variableHeights;
            PDFUnit explictGridWidth = grid.ExplicitSizes.GetAssignedWidth(out variableWidths);
            PDFUnit explictGridHeight = grid.ExplicitSizes.GetAssignedHeight(out variableHeights);

            PDFUnit availableWidth = maxGridWidth - explictGridWidth;
            PDFUnit availableHeight = maxGridHeight - explictGridHeight;

            PDFUnit defaultCellWidth = 0;
            if(variableWidths.Length > 0)
                defaultCellWidth = availableWidth / variableWidths.Length;

            PDFUnit defaultCellHeight = 0;
            if(variableHeights.Length > 0)
                defaultCellHeight = availableHeight / variableHeights.Length;
            
            PDFUnit currGridWidth = PDFUnit.Zero;
            PDFPoint offset = PDFPoint.Empty;

            LayoutAllCells(grid,ref defaultCellWidth,ref defaultCellHeight, variableWidths, variableHeights, this.CurrentPageIndex);

            PDFUnit totalwidth = this.AdjustWidthsAfterLayout(grid, defaultCellWidth);

            this.AdjustHeightsAfterLayout(grid, defaultCellHeight, totalwidth);

            SetActualSizeAndLocationOfCells(grid);
        }

       

        /// <summary>
        /// Makes sure all the columns have the correct width, and returns the total width
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="defaultCellWidth"></param>
        /// <returns></returns>
        private PDFUnit AdjustWidthsAfterLayout(TableGrid grid, PDFUnit defaultCellWidth)
        {
            PDFUnit total = PDFUnit.Zero;

            //TODO: capture the changes in width as this will affect the layout.
            for (int c = 0; c < grid.ColumnCount; c++)
            {
                PDFUnit maxw = PDFUnit.Zero;
                TableColumn tc = grid.Columns[c];
                //For each of the varialbe columns go through the list of content cells
                //and update the maximum width. If non are set then use the default cell width
                foreach (TableCellRef cref in tc)
                {
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = (TableCellContentRef)cref;
                        if (content.ColumnSpan == 1)
                        {
                            PDFSize measured = content.MeasuredSize;
                            maxw = PDFUnit.Max(measured.Width, maxw);
                        }
                    }
                    //We are a spanned cell so if we are the last span on that content cell
                    //we can calculate width based upon the previous rows 
                    //(relies on the varaibleWidths being sorted)
                    else if (cref.CellType == TableCellRefType.Spanned)
                    {
                        TableCellContentRef content = ((TableCellSpannedRef)cref).ContentCell;
                        if (c == content.ColumnIndex + content.ColumnSpan - 1)
                        {
                            //We are the last. So get the previous column widths that 
                            //we have calculated and calculate the remainder
                            PDFUnit fullwidth = content.MeasuredSize.Width;
                            PDFUnit cwidth = fullwidth;
                            for (int i = 0; i < content.ColumnSpan - 1; i++)
                            {
                                cwidth -= grid.Measured.GetWidth(content.ColumnIndex + i);
                            }
                            maxw = PDFUnit.Max(maxw, cwidth);
                        }

                    }
                }

                //we now have the final width for this column so set it
                grid.Measured.SetWidth(c, maxw);
                total += maxw;


            }

            return total;
        }

        private PDFUnit AdjustHeightsAfterLayout(TableGrid grid, PDFUnit defaultCellHeight, PDFUnit totalWidth)
        {
            PDFUnit totalHeight = PDFUnit.Zero;
            
            for (int r = 0; r < grid.RowCount; r++ )
            {
                PDFUnit maxh = PDFUnit.Zero;

                //For each of the varialbe columns go through the list of content cells
                //and update the maximum width. If non are set then use the default cell width
                for (int c = 0; c < grid.ColumnCount; c++)
                {
                    TableCellRef cref = grid[r, c];
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = (TableCellContentRef)cref;
                        if (content.RowSpan == 1)
                        {
                            PDFSize measured = content.MeasuredSize;
                            maxh = PDFUnit.Max(measured.Height, maxh);
                        }
                        content.PageIndex = this.CurrentPageIndex;
                    }
                    //We are a spanned cell so if we are the last span on that content cell
                    //we can calculate height based upon the previous rows 
                    //(relies on the varaibleHeights being sorted)
                    else if (cref.CellType == TableCellRefType.Spanned)
                    {
                        TableCellContentRef content = ((TableCellSpannedRef)cref).ContentCell;
                        content.PageIndex = this.CurrentPageIndex;

                        if (r == content.RowIndex + content.RowSpan - 1)
                        {
                            //We are the last. So get the previous row heights that 
                            //we have calculated and calculate the remainder
                            PDFUnit fullheight = content.MeasuredSize.Height;
                            PDFUnit cheight = fullheight;
                            for (int i = 0; i < content.RowSpan - 1; i++)
                            {
                                cheight -= grid.Measured.GetHeight(content.RowIndex + i);
                            }
                            maxh = PDFUnit.Max(maxh, cheight);
                        }

                    }
                }

                if (totalHeight + maxh > AvailableSize.Height)
                {
                    bool hide;
                    bool overflowed = false;
                    //PAGEBREAK
                    if (this.CanOverflow(grid, out hide))
                    {
                        int breakablerow = GetFirstBreakableRowBefore(grid, r);
                        if (breakablerow > 0)
                        {
                            int newindex;
                            PDFSize newavail;
                            PDFSize required = new PDFSize(totalWidth, maxh);
                            overflowed = this.DoExtendLayoutToNewPage(grid, required, out newavail, out newindex);

                            if (overflowed)
                            {
                                this.CurrentPageIndex = newindex;
                                this.AvailableSize = newavail;
                                this.PushNewPageIndexToAllRowsBetween(grid, breakablerow, r, out totalHeight);
                                grid.AddPageBreakRow(breakablerow);
                            }
                            
                        }
                    }
                    else if (hide)
                    {
                        this.HideRowsIncludingAndAfter(grid, r);
                        return totalHeight;
                    }
                }
                else
                    totalHeight += maxh;

                //We now know the heigt of the row
                grid.Measured.SetHeight(r, maxh);
            }

            int[] vars;
            if (this.ExplictHeight > 0)
            {
                PDFUnit total = grid.Measured.GetAssignedHeight(out vars);
                if (total < this.ExplictHeight)
                {

                }
            }

            return totalHeight;
        }

        private void HideRowsIncludingAndAfter(TableGrid grid, int fromrow)
        {
            for (int r = fromrow; r < grid.RowCount; r++)
            {
                PDFUnit maxh = PDFUnit.Zero;

                //For each of the varialbe columns go through the list of content cells
                //and update the maximum width. If non are set then use the default cell width
                for (int c = 0; c < grid.ColumnCount; c++)
                {
                    TableCellRef cref = grid[r, c];
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = (TableCellContentRef)cref;
                        content.Visible = false;
                    }
                }
            }
        }

        private void PushNewPageIndexToAllRowsBetween(TableGrid grid, int fromrow, int torow, out PDFUnit totalHeight)
        {
            totalHeight = PDFUnit.Zero;

            for (int r = fromrow; r <= torow; r++)
            {
                PDFUnit maxh = PDFUnit.Zero;

                //For each of the varialbe columns go through the list of content cells
                //and update the maximum width. If non are set then use the default cell width
                for (int c = 0; c < grid.ColumnCount; c++)
                {
                    TableCellRef cref = grid[r, c];
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = (TableCellContentRef)cref;
                        content.PageIndex = this.CurrentPageIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first row that is a complete row (has no spans that cut across it) going from the last row up
        /// </summary>
        /// <param name="lastRow"></param>
        /// <returns></returns>
        private int GetFirstBreakableRowBefore(TableGrid grid, int lastRow)
        {
            for (int currRow = lastRow; currRow >= 0; currRow--)
            {
                bool cansplit = true;
                for (int c = 0; c < grid.ColumnCount; c++)
                {
                    TableCellRef cell = grid[currRow, c];
                    if (cell.CellType == TableCellRefType.Spanned)
                    {
                        TableCellSpannedRef span = cell as TableCellSpannedRef;
                        TableCellContentRef content = span.ContentCell;
                        if (content.RowIndex != currRow)
                        {
                            cansplit = false;
                            break;
                        }
                    }
                }

                //No cell spans this row
                if (cansplit)
                    return currRow;
            }

            //There is no row that can be split
            return 0;
        }

        /// <summary>
        /// Checks  the current style and if we have an overflow action of NewPage (or the default is to allow new pages
        /// then checks if we can split the component whose children we are currently enumerating over.
        /// </summary>
        /// <param name="hide">Set to true if we should hide (not render) the current component</param>
        /// <returns></returns>
        protected virtual bool CanOverflow(TableGrid grid, out bool hide)
        {

            PDFOverflowStyle over;
            bool overflow = false;
            hide = false;
            OverflowAction action;
            ;
            if (grid.TableStyle.TryGetOverflow(out over))
                action = over.Action;
            else
                action = DefaultOverflowAction;

            if (over.Action == OverflowAction.NewPage)
            {
                if (null == grid.Engine)
                    overflow = true;
                else
                    overflow = grid.Engine.CanSplitCurrentComponent();
            }
            else if (over.Action == OverflowAction.Truncate)
                hide = true;


            return overflow;
        }

        #region protected virtual bool DoExtendLayoutToNewPage()

        /// <summary>
        /// Builds the PageRequestArgs, calls the OnRequestNewPage event wrapper and returns true if a new page was granted
        /// </summary>
        /// <returns></returns>
        protected virtual bool DoExtendLayoutToNewPage(TableGrid grid, PDFSize requiredspace, out PDFSize avail, out int newPageIndex)
        {
            int page = this.CurrentPageIndex;
            PDFPageRequestArgs args = new PDFPageRequestArgs(page, grid.Engine, requiredspace);
            this.OnRequestNewPage(args);
            avail = args.NewAvailableSpace.Size;
            newPageIndex = args.NewPageIndex;

            return args.HasNewPage;
        }


        #endregion

        /// <summary>
        /// Performs the actual layout of each cell (by calling cell.Layout()),
        /// and based upon each content cells updates the column and row sizes.
        /// </summary>
        #region private void LayoutAllCells(TableGrid grid, ...)
        /// <param name="grid"></param>
        /// <param name="defaultCellWidth"></param>
        /// <param name="defaultCellHeight"></param>
        /// <param name="variableColumns"></param>
        /// <param name="variableRows"></param>
        /// <param name="pageindex"></param>
        private void LayoutAllCells(TableGrid grid
                                    , ref PDFUnit defaultCellWidth, ref PDFUnit defaultCellHeight
                                    , int[] variableColumns, int[] variableRows, int pageindex)
        {
            PDFUnit totalVariableWidth = defaultCellWidth * variableColumns.Length;

            for (int c = 0; c < grid.ColumnCount; c++)
            {
                bool colIsVariable = Array.IndexOf<int>(variableColumns, c) > -1;
                bool lastCol = (c == grid.ColumnCount - 1);

                if (defaultCellHeight <= 0)
                    defaultCellHeight = this.AvailableSize.Height;

                TableColumn col = grid.Columns[c];
                bool widthchanged = false;
                int resizefromcolumn = 0;
                PDFUnit resizedifference = PDFUnit.Zero;
                
                for (int r = 0; r < col.Count; r++)
                {
                    bool rowisVariable = Array.IndexOf<int>(variableRows, r) > -1;

                    TableCellRef cref = grid[r, c];

                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = cref as TableCellContentRef;
                        PDFSize avail = grid.GetLayoutSize(content, defaultCellWidth, defaultCellHeight);
                        if (lastCol && avail.Width == 0)
                            avail.Width = totalVariableWidth;

                        //perform the actual layout of the content
                        PDFSize measured = LayoutContentCell(pageindex, content, avail);

                        //Check for a change in the width on cells without an explict width
                        if (measured.Width > avail.Width && content.ExplictWidth <= 0 
                            && (content.ColumnSpan == 1) ) //only or last content cell
                        {
                            content.ExplictWidth = measured.Width;
                            widthchanged = true;
                            resizefromcolumn = Math.Max(content.ColumnIndex + content.ColumnSpan, resizefromcolumn);
                            resizedifference = PDFUnit.Max(measured.Width - avail.Width,resizedifference);
                        }
                        else if (measured.Width < avail.Width && this.ExplictWidth > 0)
                        {
                            measured.Width = avail.Width;
                            content.MeasuredSize = measured;
                        }

                        if (measured.Height > avail.Height && content.ExplicitHeight <= 0)
                        {
                        }
                        else if (measured.Height < avail.Height && this.ExplictHeight > 0)
                        {
                            measured.Height = avail.Height;
                            content.MeasuredSize = measured;
                        }
                    }
                }
                
                
                if (widthchanged)
                {
                    int canresize = 0;
                    PDFUnit newwidth = grid.CalculatedSizes.GetWidth(c) + resizedifference;
                    
                    grid.Measured.SetWidth(c, newwidth);
                    
                    foreach (int varcol in variableColumns)
                    {
                        if (varcol >= resizefromcolumn)
                            canresize++;
                    }

                    totalVariableWidth -= newwidth;

                    if (canresize > 0)
                    {
                        
                        defaultCellWidth = (totalVariableWidth / canresize);
                    }
                }
            }

            //TODO: Change the height of variable rows if the height increases
            
        }

        #endregion

        /// <summary>
        /// Calls the layout of the content cell and returns the measured sized, based upon the available size.
        /// There may be a request for a new page by the content at this point so we need to handle this here.
        /// </summary>
        #region private PDFSize LayoutContentCell(int pageindex, TableCellContentRef content, PDFSize avail)
        /// <param name="pageindex"></param>
        /// <param name="content"></param>
        /// <param name="avail"></param>
        /// <returns></returns>
        private PDFSize LayoutContentCell(int pageindex, TableCellContentRef content, PDFSize avail)
        {
            //TODO: Handle page request callbacks
            PDFSize measured = content.Layout(avail, pageindex, this.LayoutContext);
            return measured;
        }

        #endregion

        /// <summary>
        /// Loops through all the content cells and finally sets their actual size and location
        /// beased upon the measured size
        /// </summary>
        #region protected void SetActualSizeAndLocationOfCells(TableGrid grid)
        /// <param name="grid"></param>
        protected void SetActualSizeAndLocationOfCells(TableGrid grid)
        {
            foreach (TableColumn tc in grid.Columns)
            {
                foreach (TableCellRef cref in tc)
                {
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = (TableCellContentRef)cref;
                        PDFSize sz = grid.GetMeasuredSize(content);
                        PDFPoint pt = grid.GetLocation(content);
                        content.ActualSize = sz;
                        content.Location = pt;
                    }
                }
            }
        }

        #endregion


        /// <summary>
        /// Checks each row for any explicit heights on the contained cells.
        /// If there is at least one (or the maximum if there is more than one),
        /// then this is set as the explicit height of that row.
        /// </summary>
        #region protected virtual void ApplyExpicitHeightsToGrid(TableGrid grid)
        /// <param name="grid"></param>
        protected virtual void ApplyExpicitHeightsToGrid(TableGrid grid)
        {
            PDFUnit maxheight = PDFUnit.Zero;
            for (int r = 0; r < grid.RowCount; r++)
            {
                PDFUnit rowheight;
                //For this row try and get the maximum explicit height. 
                bool hasone = this.TryGetMaxExplicitHeight(grid, r, out rowheight);
                //If we have one then set it as the height for this row
                if (hasone)
                    grid.ExplicitSizes.SetHeight(r, rowheight);
            }
        }

        #endregion


        /// <summary>
        /// Checks each column for any explicit widths on the contained cells.
        /// If there is at least one (or the maximum is there is more than one),
        /// then this is set as the expicit width of that column.
        /// </summary>
        #region protected virtual void ApplyExplictWidthsToGrid(TableGrid grid)
        /// <param name="grid"></param>
        protected virtual void ApplyExplictWidthsToGrid(TableGrid grid)
        {
            PDFUnit maxwidth = PDFUnit.Zero;

            foreach (TableColumn col in grid.Columns)
            {
                //For this column find any maximum explict width and then set all the content cells (with colspan of 1) to this
                //explicit width
                PDFUnit colwidth;
                bool hasone = this.TryGetMaxExplicitWidth(col, out colwidth);

                //We have an explicit width so propogate it to the column and its cells
                if (hasone)
                    grid.ExplicitSizes.SetWidth(col.ColumnIndex, colwidth);
            }
        }

        #endregion


        /// <summary>
        /// If we have a remainder after an explicit width. We need to equally extend the width of the columns
        /// (that do not have an expicit width), so they fit.
        /// </summary>
        #region private void ApplyRemainingWidthToVariableColumns(TableGrid grid, PDFUnit total, int[] variable)
        /// <param name="grid">The grid to apply the widths to</param>
        private void ApplyRemainingWidthToVariableColumns(TableGrid grid, PDFUnit total, int[] variable)
        {
            PDFUnit remainder = this.ExplictWidth - total;

            if (total < this.ExplictWidth) //We are narrower than the explicit table width
            {
                if (variable.Length > 0)
                {
                    remainder = remainder / variable.Length;
                    foreach (int v in variable)
                    {
                        grid.CalculatedSizes.SetWidth(v, remainder);
                    }
                }
                else
                {
                    //There are no variable columns but we have a fixed width.
                    //Current strategy to extend the last column to fit the table width
                    int last = variable[variable.Length-1];
                    PDFUnit w = grid.ExplicitSizes.GetWidth(last);
                    w += remainder;
                    grid.CalculatedSizes.SetWidth(last, w);
                }
            }
            else
            {
                //Do Nothing - cannot fit the contents into the required value
            }
        }

        #endregion

        /// <summary>
        /// We have a remainder after an explicit height. So we need to equally extend the height of the rows
        /// (that do not have an explicit width), so that they fit.
        /// </summary>
        #region private void ApplyRemainingHeightToVariableRows(TableGrid grid, PDFUnit total, int[] variable)
        /// <param name="grid">The grid to apply the heights to</param>
        /// <param name="total">The total height of the table</param>
        private void ApplyRemainingHeightToVariableRows(TableGrid grid, PDFUnit total, int[] variable)
        {
            PDFUnit remainder = this.ExplictHeight - total;

            if (total < this.ExplictHeight) //We are shorter than the explicit table height
            {
                if (variable.Length > 0)
                {
                    remainder = remainder / variable.Length;
                    foreach (int v in variable)
                    {
                        grid.CalculatedSizes.SetHeight(v, remainder);
                    }
                }
                else
                {
                    //There are no variable columns but we have a fixed height.
                    //Current strategy to extend the last row to fit the table height
                    int last = variable[variable.Length - 1];
                    PDFUnit w = grid.CalculatedSizes.GetHeight(last);
                    w += remainder;
                    grid.CalculatedSizes.SetHeight(last, w);
                }
            }
            else
            {
                //Do Nothing - cannot fit the contents into the required value
            }
        }

        #endregion

        /// <summary>
        /// Loops through each cell in each column and if it has a span of more than one column, and an explicit width,
        /// then we need to ensure the total width of the columns are wide enough, and if no explicit width
        /// then attempts to infer the actual width from any explicit widths on surrounding cells
        /// </summary>
        #region private void UpdateWidthsForSpannedColumns(TableGrid grid)
        /// <param name="grid"></param>
        private void UpdateWidthsForSpannedColumns(TableGrid grid)
        {
            List<TableCellContentRef> nonexplicit = new List<TableCellContentRef>();
            
            foreach (TableColumn col in grid.Columns)
            {
                foreach (TableCellRef cref in col)
                {
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = cref as TableCellContentRef;
                        
                        if (content.ColumnSpan > 1)
                        {
                            //if we are explicit then make sure we have enough space
                            if (content.ExplictWidth > 0)
                            {
                                grid.EnsureColumnsWideEnough(content, content.ExplictWidth);
                            }
                            else
                            {
                                nonexplicit.Add(content);
                            }
                        }
                    }
                }
            }
            //loop through those that do not have explicit widths and 
            foreach (TableCellContentRef content in nonexplicit)
            {
                grid.TryInferCellWidth(content);
            }
        }

        #endregion

        /// <summary>
        /// Loops through each cell in each row and if it has a span of more than one column, and an explicit height,
        /// then we need to ensure the total height of the rows are wide enough, and if no explicit height
        /// then attempts to infer the actual height from any explicit heights on surrounding cells
        /// </summary>
        #region private static void UpdateHeightsForSpannedRows(TableGrid grid)
        /// <param name="grid"></param>
        private static void UpdateHeightsForSpannedRows(TableGrid grid)
        {
            List<TableCellContentRef> nonexplicit = new List<TableCellContentRef>();

            for (int r = 0; r < grid.RowCount; r++)
            {
                for (int c = 0; c < grid.ColumnCount; c++)
                {
                    TableCellRef cref = grid[r, c];
                    if (cref.CellType == TableCellRefType.Content)
                    {
                        TableCellContentRef content = cref as TableCellContentRef;
                        if (content.RowSpan > 1)
                        {
                            //if we are explicit then make sure we have enough space
                            if (content.ExplicitHeight > 0)
                            {
                                grid.EnsureRowsHighEnough(content, content.ExplicitHeight);
                            }
                            //otherwise we add to the nonexplicit collection (so we can operate on them in the next loop
                            else
                                nonexplicit.Add(content);
                        }
                    }
                }
            }

            //now try and infer the height of variable cells from those that surround it 
            foreach (TableCellContentRef content in nonexplicit)
            {
                grid.TryInferCellHeight(content);
            }
        }

        #endregion

        /// <summary>
        /// Loops through each cell in the column and a tries to extract the maximum explicit width of that column.
        /// Returning true if at least one was found and the maximum width of any of the cells found
        /// </summary>
        #region internal bool TryGetMaxExplicitWidth(TableColumn col, out PDFUnit colwidth)
        /// <param name="col"></param>
        /// <param name="colwidth"></param>
        /// <returns></returns>
        internal bool TryGetMaxExplicitWidth(TableColumn col, out PDFUnit colwidth)
        {
            colwidth = -1;
            bool hasone = false;

            foreach (TableCellRef cref in col)
            {
                if (cref.CellType == TableCellRefType.Content)
                {
                    TableCellContentRef content = cref as TableCellContentRef;
                    if (content.ExplictWidth > 0 && content.ColumnSpan == 1)
                    {
                        colwidth = PDFUnit.Max(colwidth, content.ExplictWidth);
                        hasone = true;
                    }
                }
            }
            return hasone;
        }

        #endregion

        /// <summary>
        /// Loops through each cell in the row and a tries to extract the maximum explicit height of that column.
        /// Returning true if at least one was found and the maximun height of all the explicit heights found
        /// </summary>
        #region internal bool TryGetMaxExplicitHeight(TableGrid grid, int rowIndex, out PDFUnit rowheight)
        /// <param name="col"></param>
        /// <param name="colwidth"></param>
        /// <returns></returns>
        internal bool TryGetMaxExplicitHeight(TableGrid grid, int rowIndex, out PDFUnit rowheight)
        {
            rowheight = -1;
            bool hasone = false;

            for (int c = 0; c < grid.ColumnCount; c++)
            {
                TableCellRef cref = grid[rowIndex, c];
                if (cref.CellType == TableCellRefType.Content)
                {
                    TableCellContentRef content = cref as TableCellContentRef;
                    if (content.ExplicitHeight > 0 && content.RowSpan == 1)
                    {
                        rowheight = PDFUnit.Max(rowheight, content.ExplicitHeight);
                        hasone = true;
                    }
                }
            }
            return hasone;
        }

        #endregion

    }

    
}
