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
    /// Abstract base class for the concrete schema items 
    /// such as DBSchemaTable, DBSchemaSproc etc.
    /// </summary>
    [Serializable()]
    public abstract class DBSchemaItem
    {
        #region ivars

        private string _catalog;
        private string _name;
        private string _owner;
        private DBSchemaTypes _type;

        #endregion

        //
        // public properties
        //

        #region public string Catalog {get;set;}

        /// <summary>
        /// Gets or sets the database catalog of the DBSchemaItem
        /// </summary>
        [XmlAttribute("catalog")]
        public string Catalog
        {
            get { return this._catalog; }
            set { this._catalog = value; }
        }

        #endregion

        #region public string Name {get;set;}

        /// <summary>
        /// Gets or sets the name of the DBSchemaItem
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
        /// Gets or sets the owner of the DBSchemaItem
        /// </summary>
        [XmlAttribute("schema")]
        public string Schema 
        {
            get { return this._owner; }
            set { this._owner = value; }
        }

        #endregion

        #region public DBSchemaTypes Type {get;}

        /// <summary>
        /// Gets the DBSchemaType of this DBSchemaItem
        /// </summary>
        /// <remarks>This value is set in the constructor and cannot then be changed</remarks>
        [XmlIgnore()]
        public DBSchemaTypes Type 
        {
            get { return this._type; }
            private set { this._type = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region protected DBSchemaItem(DBSchemaTypes type)

        /// <summary>
        /// Protected constructor which sets this DBSchemaItem's Type
        /// </summary>
        /// <param name="type"></param>
        protected DBSchemaItem(DBSchemaTypes type)
            : this(string.Empty, string.Empty, type)
        {
        }

        #endregion

        #region protected DBSchemaItem(string name, string owner, DBSchemaTypes type)

        /// <summary>
        /// Protected constructor which sets the base values for this DBSchemaItem
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="type"></param>
        protected DBSchemaItem(string name, string owner, DBSchemaTypes type)
        {
            this.Name = name;
            this.Schema = owner;
            this.Type = type;
        }

        #endregion

        //
        // public methods
        //

        #region public DBSchemaItemRef GetReference()

        /// <summary>
        /// Gets a fully qualified reference to this Item
        /// </summary>
        /// <returns></returns>
        public DBSchemaItemRef GetReference()
        {
            return new DBSchemaItemRef(this.Type, this.Catalog, this.Schema, this.Name);
        }

        #endregion


        #region public override string ToString()

        /// <summary>
        /// returns a string that represents or describes the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            this.ToString(sb);
            return sb.ToString();
        }

        #endregion

        //
        // protected methods
        //

        #region protected virtual void ToString(System.Text.StringBuilder sb)

        /// <summary>
        /// Appends the details of this DBSchemaItem onto the string builder
        /// </summary>
        /// <param name="sb"></param>
        protected virtual void ToString(System.Text.StringBuilder sb)
        {
            sb.Append(this.Type.ToString().ToUpper());
            sb.Append(" [");
            if (string.IsNullOrEmpty(this.Catalog) == false)
            {
                sb.Append(this.Catalog);
                sb.Append("].[");
            }

            if (string.IsNullOrEmpty(this.Schema) == false)
            {
                sb.Append(this.Schema);
                sb.Append("].[");
            }
            sb.Append(this.Name);
            sb.Append("]");
        }

        #endregion


    }

   
}
