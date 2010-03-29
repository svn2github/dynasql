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
    /// DBSchemaItemRef encapsulates a reference to a specific 
    /// structure in a database such as a table or stored procedure
    /// </summary>
    [Serializable()]
    public class DBSchemaItemRef
    {
        #region ivars

        private string _name;
        private string _owner;
        private string _db;
        private DBSchemaTypes _type;
        private DBSchemaItemRef _cont;

        #endregion

        //
        // public properties
        //

        #region public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the name of the schema item
        /// </summary>
        [XmlAttribute("name")]
        public string Name 
        {
            get { return this._name; }
            set { this._name = value; }
        }

        #endregion

        #region public string Schema { get; set; }

        /// <summary>
        /// Gets or sets the owner of the schema item
        /// </summary>
        [XmlAttribute("schema")]
        public string Schema 
        {
            get { return this._owner; }
            set { this._owner = value; }
        }

        #endregion

        #region public string Catalog {get; set;}

        /// <summary>
        /// Gets or sets the Database name for this SchemaItem
        /// </summary>
        [XmlAttribute("catalog")]
        public string Catalog 
        {
            get { return _db; } 
            set { _db = value; }
        }

        #endregion

        #region public DBSchemaTypes Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the schema item
        /// </summary>
        [XmlAttribute("type")]
        public DBSchemaTypes Type 
        {
            get { return this._type; }
            set { this._type = value; }
        }

        #endregion

        #region public DBSchemaItemRef Container {get;set;}

        /// <summary>
        /// Gets or sets the container for this shcema item (e.g The table an index is declared on)
        /// </summary>
        public DBSchemaItemRef Container
        {
            get { return this._cont; }
            set { this._cont = value; }
        }

        #endregion

        //
        // .ctor
        //

        #region public DBSchemaItemRef()

        /// <summary>
        /// Creates a new empty schema item
        /// </summary>
        public DBSchemaItemRef()
            : this((DBSchemaTypes)0, string.Empty, string.Empty)
        {
        }

        #endregion

        #region public DBSchemaItemRef(DBSchemaTypes type, string name)

        /// <summary>
        /// Creates a new Schema Item reference with the specified type and name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public DBSchemaItemRef(DBSchemaTypes type, string name)
            : this(type, string.Empty, string.Empty, name)
        {
        }

        #endregion

        #region public DBSchemaItemRef(DBSchemaTypes type, string schema, string name)

        /// <summary>
        /// Creates a new schema item with the specified values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        public DBSchemaItemRef(DBSchemaTypes type, string schema, string name)
            : this(type, string.Empty, schema, name)
        {
            this.Type = type;
            this.Schema = schema;
            this.Name = name;
        }

        #endregion

        #region public DBSchemaItemRef(DBSchemaTypes type, string catalog, string schema, string name)

        /// <summary>
        /// Creates a new schema item with the specified values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        public DBSchemaItemRef(DBSchemaTypes type, string catalog, string schema, string name)
        {
            this.Type = type;
            this.Catalog = catalog;
            this.Schema = schema;
            this.Name = name;
        }

        #endregion


        //
        // pubic methods
        //

        #region public bool Equals(DBSchemaItemRef itemref)

        /// <summary>
        /// Compares a SchemaItemRef to this instance and returns true if 
        /// they have the same type, owner and name. The implmentation is case INsensitive
        /// </summary>
        /// <param name="itemref"></param>
        /// <returns></returns>
        public bool Equals(DBSchemaItemRef itemref)
        {
            if (null == itemref)
                throw new ArgumentNullException("itemref");

            return (this.Type == itemref.Type)
                && string.Equals(this.Catalog,itemref.Catalog,StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(this.Schema, itemref.Schema, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(this.Name, itemref.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion


        //
        // overriden methods
        //

        #region public override bool Equals(object obj)

        /// <summary>
        /// Determines whether the specified System.Object is equal to this instance
        /// by casting as a DBSchemaItemRef and comparing thier values
        /// </summary>
        /// <param name="obj">The object to compare this instance to</param>
        /// <returns>True of they are considered equal</returns>
        public override bool Equals(object obj)
        {
            return this.Equals((DBSchemaItemRef)obj);
        }

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hash code for this schema ref
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion

        #region public override string ToString()

        /// <summary>
        /// Returns a human readable representation of this schema item ref
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {3}.{1}.{2}", this.Type, this.Schema, this.Name, this.Catalog);
        }

        #endregion
    }
}
