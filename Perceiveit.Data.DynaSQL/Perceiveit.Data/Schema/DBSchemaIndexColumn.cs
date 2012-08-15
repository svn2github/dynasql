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
    /// Encapsulates the schema properties of a Table index
    /// </summary>
    public class DBSchemaIndexColumn
    {
        #region ivars

        private string _colname;
        private Order _order = Order.Default;
        

        #endregion

        //
        // public properties
        //

        #region public string ColumnName {get;set;}
        /// <summary>
        /// Gets or sets the column name for the Index Column
        /// </summary>
        [XmlAttribute("name")]
        public string ColumnName
        {
            get { return this._colname; }
            set { this._colname = value; }
        }

        #endregion

        #region public Order SortOrder {get;set;}
        /// <summary>
        /// Gets or Sets the sort ordering of the index column - or Default
        /// </summary>
        [XmlAttribute("order")]
        public Order SortOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaIndexColumn()

        /// <summary>
        /// Creates a new DBSchemaIndexColumn
        /// </summary>
        public DBSchemaIndexColumn()
            : base()
        {
        }

        #endregion

        #region public DBSchemaIndexColumn(string name)

        /// <summary>
        /// Creates a new DBSchemaIndexColumn with the specified name
        /// </summary>
        /// <param name="name"></param>
        public DBSchemaIndexColumn(string name)
            : base()
        {
            this.ColumnName = name;
        }


        #endregion

        /// <summary>
        /// returns a string representation of this index column 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Index Column '{0}'",
                this.ColumnName);
        }
    }


    /// <summary>
    /// A collection of DBSchemaIndexColumns accessible by name or by index
    /// </summary>
    public class DBSchemaIndexColumnCollection : System.Collections.ObjectModel.KeyedCollection<string, DBSchemaIndexColumn>
    {

        //
        // .ctors
        //

        #region public DBSchemaIndexColumnCollection()
        /// <summary>
        /// Create a new collection instance
        /// </summary>
        public DBSchemaIndexColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        #endregion

        //
        // protected overrides
        //

        #region protected override string GetKeyForItem(DBSchemaIndexColumn item)

        /// <summary>
        /// Gets the key Name value for the Index Column
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(DBSchemaIndexColumn item)
        {
            string name = item.ColumnName;
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException(Errors.CannotAddToSchemaWithNullOrEmptyName);

            return name;
        }

        #endregion

        /// <summary>
        /// Gets all the columns in the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaIndexColumn> GetColumns()
        {
            DBSchemaIndexColumn[] cols = new DBSchemaIndexColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }

        /// <summary>
        /// Adds a range of columns
        /// </summary>
        /// <param name="columns"></param>
        internal void AddRange(IEnumerable<DBSchemaIndexColumn> columns)
        {
            if (null != columns)
            {
                foreach (DBSchemaIndexColumn ixcol in columns)
                {
                    if(!this.Contains(ixcol.ColumnName))
                        this.Add(ixcol);
                }
            }
        }
    }

}
