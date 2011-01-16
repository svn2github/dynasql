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
using xs = System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Defines a foreign key relationship between a table and a related table.
    /// </summary>
    public class DBSchemaForeignKey : DBSchemaItem
    {
        #region ivars

        private DBSchemaForeignKeyMappingCollection _mappings;
        private DBSchemaItemRef _fktable;
        private DBSchemaItemRef _pktable;

        #endregion

        //
        // public properties
        //

        #region public DBSchemaItemRef ForeignKeyTable {get;set;}

        /// <summary>
        /// Gets or sets the Foreign Key table
        /// </summary>
        [xs.XmlElement("foreign-table")]
        public DBSchemaItemRef ForeignKeyTable
        {
            get { return _fktable; }
            set { _fktable = value; }
        }

        #endregion

        #region public DBSchemaItemRef PrimaryKeyTable {get;set;}

        /// <summary>
        /// Gets or sets the related primary key table
        /// </summary>
        [xs.XmlElement("primary-table")]
        public DBSchemaItemRef PrimaryKeyTable
        {
            get { return _pktable; }
            set { _pktable = value; }
        }

        #endregion

        #region public DBSchemaForeignKeyMappingCollection Mappings

        /// <summary>
        /// Gets or sets the collection of column 
        /// mappings between Foreign and Primary key tables
        /// </summary>
        [xs.XmlArray("mappings")]
        [xs.XmlArrayItem("map",typeof(DBSchemaForeignKeyMapping))]
        public DBSchemaForeignKeyMappingCollection Mappings
        {
            get
            {
                if (_mappings == null)
                    _mappings = new DBSchemaForeignKeyMappingCollection();
                return _mappings;
            }
            set
            {
                _mappings = value;
            }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaForeignKey()

        /// <summary>
        /// Creates a new instance of a foreign key
        /// </summary>
        public DBSchemaForeignKey()
            : base(DBSchemaTypes.ForeignKey)
        {
        }

        #endregion


    }


    /// <summary>
    /// A list of DBSchemaForeignKey's accessible by index only
    /// </summary>
    public class DBSchemaForeignKeyCollection : List<DBSchemaForeignKey>
    {
    }
}
