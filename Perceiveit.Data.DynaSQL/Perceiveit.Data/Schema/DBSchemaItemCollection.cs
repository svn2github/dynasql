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
using System.Collections.ObjectModel;

namespace Perceiveit.Data.Schema
{

    /// <summary>
    /// A collection of DBSchemaItem(s) which can be accessed by index or 
    /// DBSchemaItemRef, or names
    /// </summary>
    public class DBSchemaItemCollection : KeyedCollection<DBSchemaItemRef, DBSchemaItem>
    {
        //
        // public properties
        //

        #region public DBSchemaItem this[DBSchemaTypes type, string owner, string name] {get;}

        /// <summary>
        /// Gets the specifed DBSchemaItem in this collection that
        /// has the type, owner and name specified
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaItem this[DBSchemaTypes type, string owner, string name]
        {
            get
            {
                DBSchemaItemRef iref = new DBSchemaItemRef(type, owner, name);
                return this[iref];
            }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaItemCollection()

        /// <summary>
        /// Creates a new empty schema collection
        /// </summary>
        public DBSchemaItemCollection()
            : base()
        {

        }

        #endregion

        //
        // public methods
        //

        #region public IEnumerable<DBSchemaItem> GetAllItemsOfType(DBSchemaTypes type)

        /// <summary>
        /// Gets All the DBSchemaItems in the collection that are the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<DBSchemaItem> GetAllItemsOfType(DBSchemaTypes type)
        {
            List<DBSchemaItem> list = new List<DBSchemaItem>();
            foreach (DBSchemaItem item in this)
            {
                if ((item.Type & type) > 0)
                    list.Add(item);
            }

            return list.AsReadOnly();
        }

        #endregion


        //
        // protected overrides
        //

        #region protected override DBSchemaItemRef GetKeyForItem(DBSchemaItem item)

        /// <summary>
        /// Overrides the base abstract method to return a DBSchemaItemRef 
        /// for the specified DBSchemaItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DBSchemaItemRef GetKeyForItem(DBSchemaItem item)
        {
            return new DBSchemaItemRef(item.Type, item.Schema, item.Name);
        }

        #endregion

    }
}
