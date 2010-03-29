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
using System.Collections.ObjectModel;
using System.Text;

namespace Perceiveit.Data.Schema
{
    [Serializable()]
    public class DBSchemaTableColumn : DBSchemaColumn
    {

        #region ivars

        private bool _hasDefault = false;
        private string _default = string.Empty;
        private bool _auto = false;
        private bool _pk = false;

        #endregion

        //
        // public properties
        //

        #region public bool HasDefault {get;set;}

        /// <summary>
        /// Gets or sets whether this column has a default value
        /// </summary>
        public bool HasDefault
        {
            get { return this._hasDefault; }
            set { this._hasDefault = value; }
        }

        #endregion

        #region public string DefaultValue {get;set;}

        /// <summary>
        /// Gets or sets the DbAssigned default value for this Table column
        /// </summary>
        public string DefaultValue
        {
            get { return this._default; }
            set 
            { 
                this._default = value;
            }
        }

        #endregion

        #region public bool AutoAssign {get;set;}

        /// <summary>
        /// Gets or Sets the flag that identifies 
        /// if this columns value is assigned by the database
        /// </summary>
        public bool AutoAssign
        {
            get { return this._auto; }
            set { this._auto = value; }
        }


        #endregion

        #region public bool PrimaryKey { get; set; }

        /// <summary>
        /// Gets or Sets the flag that identifies
        /// if this column forms (part of) the primary key
        /// </summary>
        public bool PrimaryKey
        {
            get { return this._pk; }
            set { this._pk = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaTableColumn()

        public DBSchemaTableColumn()
            : this(string.Empty)
        {
        }

        #endregion

        #region public DBSchemaTableColumn(string name)

        public DBSchemaTableColumn(string name)
            : base(name)
        {
        }

        #endregion

        //
        // object overrides
        // 

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

        public DBSchemaTableColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        { }


        protected override string GetKeyForItem(DBSchemaTableColumn item)
        {
            string name = item.Name;
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException(Errors.CannotAddToSchemaWithNullOrEmptyName);

            return name;
        }

        public IEnumerable<DBSchemaColumn> GetColumns()
        {
            DBSchemaTableColumn[] cols = new DBSchemaTableColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }
    }
}
