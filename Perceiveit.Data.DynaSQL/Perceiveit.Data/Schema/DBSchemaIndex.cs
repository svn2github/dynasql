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
    /// Defines a database Index and its associated columns
    /// </summary>
    [Serializable()]
    [XmlRoot("index", Namespace = "http://schemas.perceiveit.co.uk/Query/schema/")]
    public class DBSchemaIndex : DBSchemaItem
    {
        #region ivars

        private DBSchemaIndexColumnCollection _cols = new DBSchemaIndexColumnCollection();
        private DBSchemaItemRef _tblref;
        private bool _isPk;
        private bool _isUnique;

        #endregion

        //
        // public properties
        //

        #region public DBSchemaIndexColumnCollection Columns {get protected set;}

        /// <summary>
        /// Gets the collection of DBSchemaIndexColumns in this index.
        /// Inheritors can set this value to their own collection type
        /// </summary>
        [XmlArray("columns")]
        [XmlArrayItem("column", typeof(DBSchemaIndexColumn))]
        public DBSchemaIndexColumnCollection Columns 
        {
            get
            {
                if (null == _cols)
                    _cols = new DBSchemaIndexColumnCollection();
                return this._cols;
            }
            set { this._cols = value; }
        }

        #endregion

        #region public DBSchemaItemRef TableReference {get;set;}

        /// <summary>
        /// Gets or Sets the TableReference for this Index.
        /// </summary>
        [XmlElement("table-ref")]
        public DBSchemaItemRef TableReference
        {
            get
            {
                return _tblref;
            }
            set
            {
                this._tblref = value;
            }
        }

        #endregion

        #region public bool IsPrimaryKey {get;set;}

        /// <summary>
        /// Gets or sets the falg that identifies if this is a PrimaryKey index
        /// </summary>
        [XmlAttribute("is-pk")]
        public bool IsPrimaryKey
        {
            get { return this._isPk; }
            set { this._isPk = value; }
        }

        #endregion

        #region public bool IsUnique {get;set;}

        /// <summary>
        /// Gets or sets the falg that identifies if this Index required unique values
        /// </summary>
        [XmlAttribute("is-unique")]
        public bool IsUnique
        {
            get { return this._isUnique; }
            set { this._isUnique = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaIndex()

        /// <summary>
        /// Creates a new unnamed schema index
        /// </summary>
        public DBSchemaIndex()
            : base(DBSchemaTypes.Index)
        {
        }

        #endregion

        #region public DBSchemaIndex(string owner, string name)

        /// <summary>
        /// Gets a new schema index with the specified name and owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public DBSchemaIndex(string owner, string name)
            : this()
        {
            this.Name = name;
            this.Schema = owner;
        }

        #endregion

        #region protected DBSchemaIndex(DBSchemaTypes type, string owner, string name)
        /// <summary>
        /// Creates a new DBSchema index
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBSchemaIndex(DBSchemaTypes type, string owner, string name)
            : base(name, owner, type)
        { }

        #endregion

    }

    /// <summary>
    /// A collection of DBSchemaIndexes
    /// </summary>
    [Serializable()]
    public class DBSchemaIndexCollection : System.Collections.ObjectModel.KeyedCollection<DBSchemaItemRef, DBSchemaIndex>
    {
        /// <summary>
        /// Creates a new instance of the DBSchemaIndexCollection
        /// </summary>
        public DBSchemaIndexCollection()
            : base()
        {
        }

        /// <summary>
        /// Gets the DBSchemaItemRef key for the index
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DBSchemaItemRef GetKeyForItem(DBSchemaIndex item)
        {
            return item.GetReference();
        }
        /// <summary>
        /// Attempts to retrieve and index by it's reference
        /// </summary>
        /// <param name="indexRef"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetIndex(DBSchemaItemRef indexRef, out DBSchemaIndex index)
        {
            if (this.Count == 0)
            {
                index = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(indexRef, out index);
        }
    }
}
