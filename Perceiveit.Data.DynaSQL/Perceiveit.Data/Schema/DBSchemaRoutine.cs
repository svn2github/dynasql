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
    /// Base abstract class for the DBSchemaSproc and DBSchemaFunction - contans a set of parameters
    /// </summary>
    public abstract class DBSchemaRoutine : DBSchemaItem
    {
        #region ivars

        private DBSchemaParameterCollection _params = new DBSchemaParameterCollection();
        

        #endregion

        //
        // properties
        //


        #region public DBSchemaParameterCollection Parameters

        /// <summary>
        /// Gets the collection of parameters associated with this item
        /// </summary>
        [XmlArray("params")]
        [XmlArrayItem("param")]
        public DBSchemaParameterCollection Parameters
        {
            get { return this._params; }
            set { this._params = value; }
        }

        #endregion


        //
        // ctors
        //

        #region public DBSchemaRoutine()

        /// <summary>
        /// Creates a new empty DBSchemaRoutine
        /// </summary>
        public DBSchemaRoutine()
            : this(DBSchemaTypes.Function, string.Empty, string.Empty)
        {
        }

        #endregion

        #region public DBSchemaRoutine(string owner, string name)
        /// <summary>
        /// Creates a new DBSchemaRoutine with the specified nama and owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public DBSchemaRoutine(string owner, string name)
            : this(DBSchemaTypes.Function, owner, name)
        {
        }

        #endregion

        #region protected DBSchemaRoutine(DBSchemaTypes type, string owner, string name)

        /// <summary>
        /// protected primary constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBSchemaRoutine(DBSchemaTypes type, string owner, string name)
            : base(owner, name, type)
        {
        }

        #endregion
    }
}
