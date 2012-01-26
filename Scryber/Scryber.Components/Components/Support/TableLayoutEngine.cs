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
using Scryber.Styles;
using Scryber.Native;
using Scryber.Drawing;

namespace Scryber.Components.Support
{
    internal class TableLayoutEngine : ContainerLayoutEngine
    {
        private PDFTable _table;

        protected PDFTable Table
        {
            get { return _table; }
        }

        private PDFStyle _fulltablestyle;

        protected PDFStyle FullTableStyle
        {
            get { return _fulltablestyle; }
        }

        public TableLayoutEngine(PDFTable root, IPDFLayoutEngine parent, PDFLayoutContext context)
            : base(root, parent, context)
        {
            this._table = root;
            _fulltablestyle = context.StyleStack.GetFullStyle(root);
        }

        #region IPDFLayoutEngine Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avail"></param>
        /// <returns></returns>
        /// <remarks>Table layout strategy
        /// First we build a representational grid of the table. Then...
        /// If we have an explict width and height then use the Fixed strategy,
        /// If we have only explict width then use the WidthDominant strategy,
        /// If we have only height then use the HeightDominant strategy,
        /// Otherwise use the OpenWidthDominant strategy.
        /// </remarks>
        protected override void LayoutContainer(IPDFContainerComponent root)
        {
            
            
            try
            {
                PDFSize size = PDFSize.Empty;
                TableGrid grid = new TableGrid(this.Table, this, Styles, this.FullTableStyle);
                PDFPositionStyle pos;
                double tableW = -1;
                double tableH = -1;

                if (this.FullTableStyle.TryGetPosition(out pos))
                {
                    if (pos.IsDefined(StyleKeys.WidthAttr))
                        tableW = pos.Width.PointsValue;
                    if (pos.IsDefined(StyleKeys.HeightAttr))
                        tableH = pos.Height.PointsValue;
                }

                this.BuildGrid(grid);
                
                TableLayoutStrategy strategy = this.GetStrategy(grid, this.CurrentSpace.Size, tableW > 0, tableH > 0, tableW, tableH);
                PDFPageRequestHandler newpagehandler = new PDFPageRequestHandler(strategy_RequestNewPage);
                strategy.RequestNewPage += newpagehandler;
                strategy.LayoutGrid(grid);
                strategy.RequestNewPage -= newpagehandler;
                grid.SetArrangement(this.CurrentPageIndex);

                this.Table.Grid = grid;

                this.MaxWidth = grid.ActualSize.Width;
                this.MaxHeight = grid.ActualSize.Height;
            }
            catch (PDFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw RecordAndRaise.LayoutException(ex, Errors.CouldNotBuildTheTableGrid, this.Table);
            }
        }

        private void BuildGrid(TableGrid grid)
        {
            try
            {

                int rowindex = 0;
                List<double> heights = new List<double>();

                foreach (PDFTableRow tr in this.Table.Rows)
                {
                    if (tr.Visible)
                    {
                        PDFStyle rowstyle = tr.GetAppliedStyle();
                        if (null != rowstyle)
                            this.Styles.Push(rowstyle);
                        rowstyle = this.Styles.GetFullStyle(tr);
                        this.CurrentStyle = rowstyle;
                        PDFPositionStyle pos;
                        if (rowstyle.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.HeightAttr))
                            heights.Add(pos.Height.PointsValue);
                        else
                            heights.Add(-1);

                        int nextcolumn = 0;
                        int lastcolumn = 0;
                        for (int c = 0; c < tr.Cells.Count; c++)
                        {
                            PDFTableCell cell = tr.Cells[c];
                            PDFStyle cellstyle = cell.GetAppliedStyle();

                            if (null != cellstyle)
                                this.Styles.Push(cellstyle);
                            PDFStyle full = this.Styles.GetFullStyle(cell);
                            this.CurrentStyle = full;
                            TableCellContentRef cref = grid.AddCellWithStyle(cell, full, cellstyle, rowindex, lastcolumn, out nextcolumn);
                            
                            if (full.TryGetPosition(out pos))
                            {
                                if (pos.IsDefined(StyleKeys.HeightAttr))
                                {
                                    cref.ExplicitHeight = pos.Height;
                                }
                                if (pos.IsDefined(StyleKeys.WidthAttr))
                                {
                                    cref.ExplictWidth = pos.Width;
                                }
                            }

                            if (null != cellstyle)
                                this.Styles.Pop();

                            lastcolumn = nextcolumn;
                        }

                        if (rowstyle != null)
                            this.Styles.Pop();
                        rowindex++;
                    }

                }

                this.CurrentStyle = this._fulltablestyle;

                grid.CloseGrid();
            }
            catch (Exception ex)
            {
                throw RecordAndRaise.LayoutException(ex, Errors.CouldNotBuildTheTableGrid, Table);
            }
        }

        private TableLayoutStrategy GetStrategy(TableGrid grid, PDFSize available, bool haswidth, bool hasheight, double width, double height)
        {
            //TODO: Support the layout of Right to Left and top to bottom reading directions
            return new TableLeftToRightLayoutStrategy(available, width, height, this.CurrentPageIndex, this.LayoutContext);
            
        }


        #endregion

        /// <summary>
        /// Passes a new page request from the strategy on to this engines event for raising
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void strategy_RequestNewPage(object sender, PDFPageRequestArgs args)
        {
            this.OnRequestNewPage(args);
        }
    }
}
