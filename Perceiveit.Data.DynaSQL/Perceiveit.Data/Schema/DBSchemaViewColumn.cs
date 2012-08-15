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
    /// Defines the schema of a view column in a database
    /// </summary>
    public class DBSchemaViewColumn : DBSchemaColumn
    {

        #region ivars

        private string _default = null;

        #endregion

        //
        // public properties
        //

        #region public bool HasDefault {get;set;}

        /// <summary>
        /// Gets or sets whether this column has a default value
        /// </summary>
        [XmlIgnore()]
        public bool HasDefault
        {
            get { return this.IsColumnFlagSet(DBColumnFlags.HasDefault); }
            set { this.SetColumnFlag(DBColumnFlags.HasDefault, value); }
        }

        #endregion

        #region public string DefaultValue {get;set;}

        /// <summary>
        /// Gets or sets the DbAssigned default value for this Table column
        /// </summary>
        [XmlElement("default")]
        public string DefaultValue
        {
            get { return this._default; }
            set 
            {
                this._default = value;
                if (null == value)
                    HasDefault = false;
                else
                    HasDefault = true;
            }
        }

        #endregion

        //
        // .ctors
        //

        /// <summary>
        /// Creates a new DBSchemaViewColumn
        /// </summary>
        public DBSchemaViewColumn()
            : base()
        { }

        /// <summary>
        /// Creates a new DBSchemaViewColumn with the specifed name
        /// </summary>
        /// <param name="name"></param>
        public DBSchemaViewColumn(string name)
            : base(name)
        {
            
        }
        //
        // object overrides
        //

        #region public override string ToString()
        /// <summary>
        /// returms a human readable string representing this instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Column '{0}' {1} (Runtime Type: {8}, Size: {2}, Read Only:{3}, Nullable:{4}, HasDefault:{6} ({5}), Ordinal:{7})",
                this.Name, this.DbType, this.Size, this.ReadOnly, this.Nullable, this.DefaultValue, this.HasDefault, this.OrdinalPosition, this.Type);
        }

        #endregion
    }


    /// <summary>
    /// Defines a collection of columns accessible by name or index
    /// </summary>
    public class DBSchemaViewColumnCollection : System.Collections.Generic.List<DBSchemaViewColumn>
    {
        /// <summary>
        /// 
        /// </summary>
        public DBSchemaViewColumnCollection()
            : base()
        {
        }

        /// <summary>
        /// Gets the first column with a matching name (initially case-sensitive, then a second case insensitive search is performed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaViewColumn this[string name]
        {
            get
            {
                foreach (DBSchemaViewColumn col in this)
                {
                    if (string.Equals(col.Name, name, StringComparison.InvariantCulture))
                        return col;
                }
                foreach (DBSchemaViewColumn col in this)
                {
                    if (string.Equals(col.Name, name, StringComparison.InvariantCultureIgnoreCase))
                        return col;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all the columns as an array
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaColumn> GetColumns()
        {
            DBSchemaViewColumn[] cols = new DBSchemaViewColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }

        /// <summary>
        /// returns true it this collection contains a column with the specifeid name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return this[name] != null;
        }
    }
}
