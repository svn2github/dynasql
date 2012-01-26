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

namespace Scryber.Components
{
    [PDFParsableComponent("Row")]
    public class PDFTableRow : PDFContainerComponent
    {
        #region .ctor() + .ctor(PDFObjectType)

        /// <summary>
        /// Creates a new instance of the table row
        /// </summary>
        public PDFTableRow()
            : this(PDFObjectTypes.TableRow)
        {
        }


        /// <summary>
        /// Protected constructor that sub classes 
        /// can use to create an instance of their class using a different ObjectType
        /// </summary>
        /// <param name="type">The type identifier</param>
        protected PDFTableRow(PDFObjectType type)
            : base(type)
        {

        }

        #endregion

        #region PDFTableCellList Cells {get;}

        private PDFTableCellList _cells;

        [PDFArray(typeof(PDFTableCell))]
        [PDFElement("")]
        public PDFTableCellList Cells
        {
            get 
            {
                if (_cells == null)
                    _cells = new PDFTableCellList(this.InnerContent);

                return _cells;
            }
        }

        #endregion

        #region public PDFTable ContainingTable {get;}

        /// <summary>
        /// Gets the table that contains this Row (or null)
        /// </summary>
        public PDFTable ContainingTable
        {
            get
            {
                PDFComponent Component = this.Parent;
                while (Component != null)
                {
                    if (Component is PDFTable)
                        return Component as PDFTable;
                    else
                        Component = Component.Parent;
                }
                return null; //not found.
            }
        }

        #endregion

    }

    public class PDFTableRowList : PDFComponentWrappingList<PDFTableRow>
    {
        public PDFTableRowList(PDFComponentList inner)
            : base(inner)
        {
        }
    }
}
