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

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Defines a supported meta data collection
    /// </summary>
    public class DBSchemaMetaDataCollection
    {
        //
        // public properties
        //

        /// <summary>
        /// Gets or sets the name of the collection
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the number of restrictions
        /// </summary>
        public int NumberOfRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the number of parts
        /// </summary>
        public int NumberOfIdentifierParts { get; set; }

        /// <summary>
        /// Gets the known type of this collection
        /// </summary>
        public DBMetaDataCollectionType CollectionType { get; private set; }

        /// <summary>
        /// Gets or sets all the item references for this collection.
        /// </summary>
        internal DBSchemaItemRefCollection References { get; set; }


        //
        // .ctors
        //

        /// <summary>
        /// Creates a new (empty) DBSchemaMetaDataCollection with the specified type
        /// </summary>
        /// <param name="type"></param>
        public DBSchemaMetaDataCollection(DBMetaDataCollectionType type) : this(string.Empty, -1, -1, type) { }

        /// <summary>
        /// Creates a new DBSchemaMetaDataCollection with the specified name, restrictions, parts, and type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restrictions"></param>
        /// <param name="parts"></param>
        /// <param name="type"></param>
        public DBSchemaMetaDataCollection(string name, int restrictions, int parts, DBMetaDataCollectionType type)
        {
            this.CollectionName = name;
            this.NumberOfRestrictions = restrictions;
            this.NumberOfIdentifierParts = parts;
            this.CollectionType = type;
        }


        /// <summary>
        /// Returns a string representation of this meta data collection
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("MetaDataCollection {0} ({1}) :{2} restictions, {3} parts", CollectionType, CollectionName, NumberOfRestrictions, NumberOfIdentifierParts);
        }

    }

    /// <summary>
    /// Defines a set of DBSchemaMetaDataCollection's that can be enumerated over, or accessed by index or type
    /// </summary>
    public class DBSchemaMetaDataCollectionSet : IEnumerable<DBSchemaMetaDataCollection>
    {
        private Dictionary<DBMetaDataCollectionType,DBSchemaMetaDataCollection> _keys = new Dictionary<DBMetaDataCollectionType,DBSchemaMetaDataCollection>();
        private List<DBSchemaMetaDataCollection> _items = new List<DBSchemaMetaDataCollection>();

        /// <summary>
        /// Gets the number of meta data collections in this set
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Clears all items from the set
        /// </summary>
        public void Clear()
        {
            this._keys.Clear();
            this._items.Clear();
        }

        /// <summary>
        /// Gets the DBSchemaMetaDataCollection at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DBSchemaMetaDataCollection this[int index]
        {
            get { return this._items[index]; }
        }

        /// <summary>
        /// Gets the DBSchemaMetaDataCollection with the required type (cannot use 'Other'). 
        /// Returns null if no collection exists of the required type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DBSchemaMetaDataCollection this[DBMetaDataCollectionType type]
        {
            get
            {
                if (type == DBMetaDataCollectionType.Other)
                    throw new ArgumentOutOfRangeException("Cannot access by Collection type of other");
                DBSchemaMetaDataCollection col;
                this._keys.TryGetValue(type, out col);
                return col;
            }
        }

        /// <summary>
        /// Gets the first DBSchemaMetaDataCollection with the specified name (case in-sensitive).
        /// Returns null if not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaMetaDataCollection this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;
                foreach (DBSchemaMetaDataCollection col in this._items)
                {
                    if (string.Equals(col.CollectionName, name, StringComparison.OrdinalIgnoreCase))
                        return col;
                }
                return null;
            }

        }

        /// <summary>
        /// Adds a DBSchemaMetaDataCollection to this set. The collection type must be unique
        /// </summary>
        /// <param name="col"></param>
        public void Add(DBSchemaMetaDataCollection col)
        {
            if (col.CollectionType != DBMetaDataCollectionType.Other)
            {
                if (_keys.ContainsKey(col.CollectionType))
                    throw new ArgumentException("The collection type '" + col.CollectionType.ToString() + "' has already been added");
                _keys.Add(col.CollectionType, col);
            }
            _items.Add(col);
        }

        /// <summary>
        /// Removes the specified collection from the set
        /// </summary>
        /// <param name="col"></param>
        public void Remove(DBSchemaMetaDataCollection col)
        {
            if (_items.Remove(col) && _keys.ContainsKey(col.CollectionType))
                _keys.Remove(col.CollectionType);
        }


        #region IEnumerable<DBSchemaMetaDataCollection> Members

        /// <summary>
        /// Gets the set enumerator - will be in the order of addition to the set
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DBSchemaMetaDataCollection> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
