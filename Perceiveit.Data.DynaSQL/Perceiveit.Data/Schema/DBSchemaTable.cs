/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Defines the schema information of the table in a database
    /// </summary>
    [XmlRoot("table", Namespace="http://schemas.perceiveit.co.uk/Query/schema/")]
    [Serializable()]
    public class DBSchemaTable : DBSchemaItem
    {
        #region ivars

        private DBSchemaTableColumnCollection _cols;
        private DBSchemaIndexCollection _indexes;
        private DBSchemaForeignKeyCollection _fks;

        #endregion

        //
        // public properties
        //

        #region public DBSchemaTableColumnCollection Columns { get; set; }

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

        #region public DBSchemaItemRefCollection Indexes {get; set;}

        /// <summary>
        /// Gets the collection of DBSchemaIndexes in this table
        /// </summary>
        [XmlArray("indexes")]
        [XmlArrayItem("index", typeof(DBSchemaIndex))]
        public DBSchemaIndexCollection Indexes
        {
            get { return this._indexes; }
            set { this._indexes = value; }
        }

        #endregion

        #region public DBSchemaForeignKeyCollection ForeignKeys {get;set;}

        /// <summary>
        /// Gets the collection of DBSchemaForeignKeys in this table
        /// </summary>
        [XmlArray("foreign-keys")]
        [XmlArrayItem("fk", typeof(DBSchemaForeignKey))]
        public DBSchemaForeignKeyCollection ForeignKeys
        {
            get { return this._fks; }
            set { this._fks = value; }
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
        /// <summary>
        /// Creates an new DBSchemaTable
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
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
