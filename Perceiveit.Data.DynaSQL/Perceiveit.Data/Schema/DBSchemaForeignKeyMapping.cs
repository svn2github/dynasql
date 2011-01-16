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
    /// <summary>
    /// Defines the mapping between the foreign key 
    /// tables column and the primary key tables column
    /// </summary>
    public class DBSchemaForeignKeyMapping
    {
        private string _fkcol, _pkcol;

        /// <summary>
        /// Gets or sets the name of the column on the ForeignKey Table
        /// </summary>
        [XmlAttribute("foreign-column")]
        public string ForeignColumn { get { return _fkcol; } set { _fkcol = value; } }

        /// <summary>
        /// Gets or sets the name of the column on the PrimaryKey Table
        /// </summary>
        [XmlAttribute("primary-column")]
        public string PrimaryColumn { get { return _pkcol; } set { _pkcol = value; } }
        
    }

    /// <summary>
    /// A collection of ForeignKeyMapping's
    /// </summary>
    public class DBSchemaForeignKeyMappingCollection : List<DBSchemaForeignKeyMapping>
    {
    }
}
