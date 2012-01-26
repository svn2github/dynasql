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
using Scryber.Native;
using Scryber.Styles;

namespace Scryber.Components.Support
{
    internal class TableCellContentRef : TableCellRef
    {
        private PDFPoint _location;
        private PDFSize _explicitsize;
        private PDFSize _measuredsize;
        private PDFSize _actualsize;
        private PDFStyle _fullstyle;
        private PDFStyle _appliedstyle;
        private int _rowspan;
        private int _colspan;
        private int _pgIndex;
        private bool _vis = true;
        private PDFTableCell _cell;

        /// <summary>
        /// Gets or sets the relative location of the cell wrt the top left of the table grid
        /// </summary>
        internal PDFPoint Location { get { return this._location; } set { this._location = value; } }

        /// <summary>
        /// Gets or sets  the explicit width of the cell (hard written value of width)
        /// </summary>
        internal PDFUnit ExplictWidth { get { return _explicitsize.Width; } set { _explicitsize.Width = value; } }

        /// <summary>
        /// Gets or sets the explicit height of the cell (hard written value of height)
        /// </summary>
        internal PDFUnit ExplicitHeight { get { return _explicitsize.Height; } set { _explicitsize.Height = value; } }

        /// <summary>
        /// Gets or sets the measured size of the cell
        /// </summary>
        internal PDFSize MeasuredSize { get { return _measuredsize; } set { _measuredsize = value; } }

        /// <summary>
        /// Gets or sets the page index of this content cell
        /// </summary>
        internal int PageIndex { get { return _pgIndex; } set { _pgIndex = value; } }

        internal bool Visible
        {
            get { return _vis; }
            set
            {
                _vis = value;
            }
        }

        /// <summary>
        /// Gets or sets the Actual size of the cell (including marigns and padding)
        /// </summary>
        internal PDFSize ActualSize
        {
            get{ return _actualsize; }
            set { _actualsize = value; }
        }

        
        //
        // readonly properties
        //

        /// <summary>
        /// Gets the Cell type of this instance - Spanned
        /// </summary>
        internal override TableCellRefType CellType{get { return TableCellRefType.Content; } }

        /// <summary>
        /// Gets the full style of the inner table cell
        /// </summary>
        internal PDFStyle FullStyle { get { return _fullstyle; } }

        /// <summary>
        /// Gets the applied style for the inner cell
        /// </summary>
        internal PDFStyle AppliedStyle { get { return _appliedstyle; } }


        /// <summary>
        /// Gets the row span of the inner table cell
        /// </summary>
        internal int RowSpan { get { return _rowspan; } }

        /// <summary>
        /// Gets the column span of the inner table cell
        /// </summary>
        internal int ColumnSpan { get { return _colspan; } }

        /// <summary>
        /// Gets the inner PDFTableCell of this reference.
        /// </summary>
        internal PDFTableCell InnerCell { get { return _cell; } }

        /// <summary>
        /// Creates a new reference to a table cell
        /// </summary>
        /// <param name="cell"></param>
        internal TableCellContentRef(TableGrid grid, PDFTableCell cell, PDFStyle fullstyle, PDFStyle applied, PDFThickness margins, PDFThickness padding)
            : base(grid, margins, padding)
        {
            if (null == cell)
                throw new ArgumentNullException("cell");
            if (null == fullstyle)
                throw new ArgumentNullException("fullstyle");

            this._cell = cell;
            this._appliedstyle = applied;
            this._colspan = cell.ColumnSpan;
            this._rowspan = cell.RowSpan;
            this._fullstyle = fullstyle;
        }


        internal override PDFSize Layout(PDFSize avail, int pageindex, PDFLayoutContext context)
        {

            PDFSize sz = PDFSize.Empty;
            PDFUnit vinc = PDFUnit.Zero;
            PDFUnit hinc = PDFUnit.Zero;

            if (!this.Margins.IsEmpty)
            {
                hinc = this.Margins.Left + this.Margins.Right;
                vinc = this.Margins.Top + this.Margins.Bottom;
            }

            if (!this.Padding.IsEmpty)
            {
                hinc += this.Padding.Left + this.Padding.Right;
                vinc += this.Padding.Top + this.Padding.Bottom;
            }

            avail = new PDFSize(avail.Width - hinc, avail.Height - vinc);

            IPDFLayoutEngine engine = this.InnerCell.GetEngine(this.Grid.Engine, context);
            sz = engine.Layout(avail, pageindex);
            
            sz = new PDFSize(sz.Width + hinc, sz.Height + vinc);

            if (this.ExplicitHeight > 0)
                sz.Height = this.ExplicitHeight;
            if (this.ExplictWidth > 0)
                sz.Width = this.ExplictWidth;

            this.MeasuredSize = sz;
            return sz;
        }


        internal override PDFRect SetArrangement()
        {
            if (this.Visible)
            {
                PDFComponentMultiArrangement arrange = new PDFComponentMultiArrangement();

                arrange.Bounds = new PDFRect(this.Location.X, this.Location.Y, this.ActualSize.Width, this.ActualSize.Height);
                arrange.AppliedStyle = this.FullStyle;
                arrange.Display = true;
                arrange.Margins = this.Margins;
                arrange.Padding = this.Padding;
                arrange.PageIndex = this.PageIndex;
                arrange.PositionMode = PositionMode.Relative;
                this.InnerCell.SetArrangement(arrange);

                return arrange.Bounds;
            }
            else
            {
                this.InnerCell.ClearArrangement();
                return PDFRect.Empty;
            }
        }

        internal override void RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            this.InnerCell.RenderToPDF(context, writer);
        }

       


        public override string ToString()
        {
            return "Content : R" + this.RowIndex + ",C" + this.ColumnIndex + " -> ID:" + this.InnerCell.ID;
        }
    }
}
