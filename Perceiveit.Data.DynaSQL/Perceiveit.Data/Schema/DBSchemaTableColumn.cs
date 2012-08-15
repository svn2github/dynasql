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
using System.Collections.ObjectModel;
using System.Text;
using xs = System.Xml.Serialization;
namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Defines the schema of a table column in a database
    /// </summary>
    [Serializable()]
    public class DBSchemaTableColumn : DBSchemaColumn
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
        [xs.XmlIgnore()]
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
        [xs.XmlElement("default")]
        public string DefaultValue
        {
            get { return this._default; }
            set 
            { 
                this._default = value;
                this.HasDefault = !string.IsNullOrEmpty(value);
            }
        }

        #endregion

        #region public bool AutoAssign {get;set;}

        /// <summary>
        /// Gets or Sets the flag that identifies 
        /// if this columns value is assigned by the database
        /// </summary>
        [xs.XmlIgnore()]
        public bool AutoAssign
        {
            get { return this.IsColumnFlagSet(DBColumnFlags.AutoAssign); }
            set { this.SetColumnFlag(DBColumnFlags.AutoAssign,value); }
        }


        #endregion

        #region public bool PrimaryKey { get; set; }

        /// <summary>
        /// Gets or Sets the flag that identifies
        /// if this column forms (part of) the primary key
        /// </summary>
        [xs.XmlIgnore()]
        public bool PrimaryKey
        {
            get { return this.IsColumnFlagSet(DBColumnFlags.PrimaryKey); }
            set { this.SetColumnFlag(DBColumnFlags.PrimaryKey, value); }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaTableColumn()
        /// <summary>
        /// Creates a new empty DBschemaTableColumn definition
        /// </summary>
        public DBSchemaTableColumn()
            : this(string.Empty)
        {
        }

        #endregion

        #region public DBSchemaTableColumn(string name)

        /// <summary>
        /// Creates a new DBSchemaTableColumn with the specified name
        /// </summary>
        /// <param name="name"></param>
        public DBSchemaTableColumn(string name)
            : base(name)
        {
        }

        #endregion

        //
        // object overrides
        // 

        /// <summary>
        /// Returns a string representation of the table column
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Column '{0}' {1} (Runtime Type: {10}, Size: {2}, Read Only:{3}, Auto Assign:{4}, Primary Key:{5}, Nullable:{6}, HasDefault:{8} ({7}), Ordinal:{9})",
                this.Name, this.DbType, this.Size, this.ReadOnly, this.AutoAssign, this.PrimaryKey, this.Nullable, this.DefaultValue, this.HasDefault, this.OrdinalPosition, this.Type);
        }
    }

    /// <summary>
    /// A collection of DBSchemaColumn objects accessible by name or by index.
    /// Note: the string comparison is case INsensitive
    /// </summary>
    [Serializable()]
    public class DBSchemaTableColumnCollection : KeyedCollection<string, DBSchemaTableColumn>
    {
        /// <summary>
        /// Creates a new collection
        /// </summary>
        public DBSchemaTableColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(DBSchemaTableColumn item)
        {
            string name = item.Name;
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException(Errors.CannotAddToSchemaWithNullOrEmptyName);

            return name;
        }
        
        /// <summary>
        /// Gets all the colums in this collection as an array
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaColumn> GetColumns()
        {
            DBSchemaTableColumn[] cols = new DBSchemaTableColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }

        /// <summary>
        /// attempts to get the column with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tblcol"></param>
        /// <returns></returns>
        internal bool TryGetColumn(string name, out DBSchemaTableColumn tblcol)
        {
            if (this.Count == 0)
            {
                tblcol = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(name, out tblcol);
        }
    }
}
