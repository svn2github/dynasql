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
    public class DBSchemaViewColumn : DBSchemaColumn
    {

        #region ivars

        private bool _hasDefault = false;
        private string _default = string.Empty;

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
                if (null == value)
                    _hasDefault = false;
                else
                    _hasDefault = true;
            }
        }

        #endregion

        //
        // .ctors
        //

        public DBSchemaViewColumn()
            : base()
        { }

        public DBSchemaViewColumn(string name)
            : base(name)
        {
            
        }
        //
        // object overrides
        //

        #region public override string ToString()

        public override string ToString()
        {
            return string.Format("Column '{0}' {1} (Runtime Type: {8}, Size: {2}, Read Only:{3}, Nullable:{4}, HasDefault:{6} ({5}), Ordinal:{7})",
                this.Name, this.DbType, this.Size, this.ReadOnly, this.Nullable, this.DefaultValue, this.HasDefault, this.OrdinalPosition, this.Type);
        }

        #endregion
    }


    public class DBSchemaViewColumnCollection : System.Collections.ObjectModel.KeyedCollection<string, DBSchemaViewColumn>
    {
        public DBSchemaViewColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        protected override string GetKeyForItem(DBSchemaViewColumn item)
        {
            return item.Name;
        }

        public IEnumerable<DBSchemaColumn> GetColumns()
        {
            DBSchemaViewColumn[] cols = new DBSchemaViewColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }
    }
}
