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

//#define SHOW_SIZEINFO

using System;
using System.Collections.Generic;
using System.Text;
using Scryber.Styles;
using Scryber.Drawing;

namespace Scryber.Components
{
    [PDFParsableComponent("Cell")]
    public class PDFTableCell : PDFVisualComponent, IPDFViewPortComponent
    {
        //
        // properties
        //

        #region public int ColumnSpan {get; set;}

        private int _colspan = 1;

        /// <summary>
        /// Gets or sets the ColumnSpan for the Cell
        /// </summary>
        [PDFAttribute("col-span")]
        public int ColumnSpan
        {
            get { return _colspan; }
            set { _colspan = value; }
        }

        #endregion

        #region public int RowSpan {get;set;}


        private int _rowspan = 1;

        /// <summary>
        /// Gets or sets the Row Span for the cell
        /// </summary>
        [PDFAttribute("row-span")]
        public int RowSpan
        {
            get { return _rowspan; }
            set { _rowspan = value; }
        }

        #endregion

        #region public PDFTableRow ContainingRow

        /// <summary>
        /// Gets the row that contains this cell, or null.
        /// </summary>
        public PDFTableRow ContainingRow
        {
            get
            {
                PDFComponent Component = this.Parent;
                while (Component != null)
                {
                    if (Component is PDFTableRow)
                        return Component as PDFTableRow;
                    else
                        Component = Component.Parent;
                }
                return null; //not found.
            }
        }

        #endregion 

        #region public PDFComponentList Contents {get;}

        /// <summary>
        /// Gets the contents of the cell
        /// </summary>
        [PDFArray(typeof(PDFComponent))]
        [PDFElement("Content")]
        public PDFComponentList Contents
        {
            get { return this.InnerContent; }
        }

        #endregion

        //
        // ctor(s)
        //

        #region .ctor() + .ctor(PDFObjectType)

        /// <summary>
        /// Creates a new instance of the PDFTableCell
        /// </summary>
        public PDFTableCell()
            : this(PDFObjectTypes.TableCell)
        {
        }
        
        /// <summary>
        /// Protected constructor that sub classes 
        /// can use to create an instance of their class using a different ObjectType
        /// </summary>
        /// <param name="type">The type identifier</param>
        protected PDFTableCell(PDFObjectType type)
            : base(type)
        {
        }

        #endregion
    

        //
        // interface implementation
        //

        #region IPDFViewPortComponent Members

        public IPDFLayoutEngine GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return new Support.ContainerLayoutEngine(this, parent, context);
        }

        #endregion

#if SHOW_SIZEINFO

        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, Scryber.Drawing.PDFGraphics graphics, PDFWriter writer)
        {
            PDFObjectRef oref = base.DoRenderToPDF(context, fullstyle, graphics, writer);

            if (context.TraceLog.RecordLevel == TraceRecordLevel.All)
            {
                PDFComponentArrangement arrange = this.GetArrangement();
                Drawing.PDFSolidBrush brush = new Scryber.Drawing.PDFSolidBrush(PDFColors.White);
                Drawing.PDFSolidBrush cyan = new PDFSolidBrush(PDFColors.Aqua);

                PDFFont f = new PDFFont(StandardFont.Helvetica, 8);
                PDFPoint pt = context.Offset;
                PDFSize sz = arrange.Bounds.Size;

                PDFPaddingStyle pad;
                if (fullstyle.TryGetPadding(out pad))
                {
                    pt.X -= pad.Left;
                    pt.Y -= pad.Top;
                }
                string size = sz.Width.PointsValue.ToString("#.0") + ", " + sz.Height.PointsValue.ToString("#.0");

                Text.PDFTextBlock block = graphics.MeasureBlock(size, brush, f);

                graphics.FillRectangle(cyan, pt, block.Size);
                graphics.FillText(block, pt);
            }


            return oref;
        }
#endif

    }



    public class PDFTableCellList : PDFComponentWrappingList<PDFTableCell>
    {
        public PDFTableCellList(PDFComponentList inner)
            : base(inner)
        {
        }
    }
}
