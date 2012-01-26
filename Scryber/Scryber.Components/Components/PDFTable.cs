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

namespace Scryber.Components
{
    [PDFParsableComponent("Table")]
    public class PDFTable : PDFVisualComponent, IPDFViewPortComponent
    {


        #region .ctor() + .ctor(PDFObjectType)
        /// <summary>
        /// Creates a new instance of the PDFTable
        /// </summary>
        public PDFTable()
            : this(PDFObjectTypes.Table)
        {
        }

        protected PDFTable(PDFObjectType type)
            : base(type)
        {
        }

        #endregion

        #region public PDFTableRowList Rows {get;set;}

        private PDFTableRowList _rows;
        /// <summary>
        /// Gets the collection of PDFTableRow(s) 
        /// </summary>
        [PDFArray(typeof(PDFTableRow))]
        [PDFElement("")]
        public PDFTableRowList Rows
        {
            get
            {
                if (this._rows == null)
                    this._rows = new PDFTableRowList(this.InnerContent);
                return this._rows;
            }
        }

        #endregion

        private Support.TableGrid _grid = null;

        internal Support.TableGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        

        protected override PDFObjectRef DoRenderChildrenToPDF(PDFRenderContext context, Styles.PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            if (_grid == null)
                throw new NullReferenceException(Errors.GridIsNullNotMeasured);
            PDFObjectRef[] orefs = _grid.RenderToPDF(context, fullstyle, graphics, writer);
            if (orefs != null && orefs.Length > 0)
            {
                PDFObjectRef root = writer.BeginObject();
                writer.WriteArrayRefEntries(true, orefs);
                writer.EndObject();
                return root;
            }
            else
                return null;

        }

        #region IPDFViewPortComponent Members

        IPDFLayoutEngine IPDFViewPortComponent.GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return new Support.TableLayoutEngine(this, parent, context);
        }

        #endregion
    }
}
