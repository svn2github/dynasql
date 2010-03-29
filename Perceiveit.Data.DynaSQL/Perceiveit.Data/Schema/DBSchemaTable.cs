/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    [XmlRoot("table", Namespace="http://schemas.perceiveit.co.uk/Query/schema/")]
    [Serializable()]
    public class DBSchemaTable : DBSchemaItem
    {
        #region ivars

        private DBSchemaTableColumnCollection _cols;
        private DbSchemaIndexCollection _indexes;

        #endregion

        //
        // public properties
        //

        #region public DBSchemaTableColumnCollection Columns { get; protected set; }

        /// <summary>
        /// Gets the collection of DBSchemaColumns in this table
        /// </summary>
        [XmlArray("columns")]
        [XmlArrayItem("column", typeof(DBSchemaTableColumn))]
        public DBSchemaTableColumnCollection Columns 
        {
            get { return this._cols; }
            set { this._cols = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaTable()

        /// <summary>
        /// Creates a new unnamed DBSchemaTable
        /// </summary>
        public DBSchemaTable()
            : base(DBSchemaTypes.Table)
        {
            this.Columns = new DBSchemaTableColumnCollection();
        }

        #endregion

        #region public DBSchemaTable(string owner, string name)

        public DBSchemaTable(string owner, string name)
            : this()
        {
            this.Name = name;
            this.Schema = owner;
        }

        #endregion

        #region protected DBSchemaTable(DBSchemaTypes type, string owner, string name)

        /// <summary>
        /// protected constructor that inheritors can use to specifiy the DBSchemaTypes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBSchemaTable(DBSchemaTypes type, string owner, string name)
            : base(name, owner, type)
        {
        }

        #endregion

        //
        // overriden methods
        //

        

    }
}
