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
using System.Data;
using System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    [XmlRoot("sproc", Namespace = "http://schemas.perceiveit.co.uk/Query/schema/")]
    public class DBSchemaSproc : DBSchemaItem
    {
        #region ivars

        private DBSchemaParameterCollection _params = new DBSchemaParameterCollection();

        #endregion


        //
        // public properties
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
        // .ctors
        //

        #region public DBSchemaSproc()

        /// <summary>
        /// Create a new empty Stored Procedure reference
        /// </summary>
        public DBSchemaSproc()
            : this(DBSchemaTypes.StoredProcedure, String.Empty, String.Empty)
        {
        }

        #endregion

        #region public DBSchemaSproc(string owner, string name)

        /// <summary>
        /// Creates a new StoredProcedure reference with the
        /// specified name and owner
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        public DBSchemaSproc(string owner, string name)
            : this(DBSchemaTypes.StoredProcedure, owner, name)
        {
            this.Name = name;
            this.Schema = owner;
        }

        #endregion

        #region protected DBSchemaSproc(DBSchemaTypes type, string owner, string name)

        /// <summary>
        /// proected primary constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBSchemaSproc(DBSchemaTypes type, string owner, string name)
            : base(name, owner, type)
        {
        }

        #endregion

        //
        // protected methods
        //

        #region protected virtual DbType GetDbTypeFromRuntimeType(Type value)

        /// <summary>
        /// Gets the DbType that is the closest match for the runtime Type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual DbType GetDbTypeFromRuntimeType(Type value)
        {
            if (null == value)
                return DbType.Object;
            else
                return DBHelper.GetDBTypeForRuntimeType(value);
        }

        #endregion

        #region protected virtual Type GetRuntimeTypeFromDbType(DbType value)

        /// <summary>
        /// Returns the default runtime type that is the closest match to the DbType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Type GetRuntimeTypeFromDbType(DbType value)
        {
            return DBHelper.GetRuntimeTypeForDbType(value);
        }

        #endregion


        
    }
}
