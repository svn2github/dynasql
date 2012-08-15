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
using System.Data;
using System.Text;
using System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Defines a existing parameter used when calling stored procedures
    /// </summary>
    public class DBSchemaParameter
    {

        #region ivars

        private string _pname;
        private string _invarname;
        private System.Data.DbType _pdbtype = System.Data.DbType.Object;
        private Type _pruntype = typeof(Object);
        private int _psize = -1;
        private int _pindex = -1;
        private ParameterDirection _pdir = ParameterDirection.Input;
        private bool _assignedDbType = false;
        private bool _assignedRuntimeType = false;

        #endregion

        //
        // public properties
        //

        #region public string Name {get;set;}

        /// <summary>
        /// Gets or sets the native name of the parameter - specific to the provider
        /// </summary>
        [XmlAttribute("name")]
        public string NativeName 
        {
            get { return this._pname; }
            set { this._pname = value; }
        }

        #endregion

        #region public string Name {get;set;}

        /// <summary>
        /// Gets or sets the name of the parameter without any provider specific identifiers
        /// </summary>
        [XmlAttribute("invariant-name")]
        public string InvariantName
        {
            get { return this._invarname; }
            set { this._invarname = value; }
        }

        #endregion



        #region public DbType DbType {get;set;}

        /// <summary>
        /// Gets or sets the DbType of this parameter
        /// </summary>
        /// <remarks>If the runtime type has not been set then 
        /// it will be inferred for the value here</remarks>
        [XmlAttribute("db-type")]
        public DbType DbType
        {
            get { return this._pdbtype; }
            set 
            { 
                this._pdbtype = value;
                if (value != DbType.Object)
                {
                    if (!_assignedRuntimeType)
                        this._pruntype = GetRuntimeTypeFromDbType(value);
                    _assignedDbType = true;
                }
            }
        }

        #endregion

        #region public Type RuntimeType { get;set;}

        /// <summary>
        /// Gets or sets the runtime Type of this parameter
        /// </summary>
        /// <remarks>If the DbType has not explicitly been set then the DbType 
        /// will be inferred from the value here</remarks>
        [XmlIgnore()]
        public Type RuntimeType
        {
            get { return this._pruntype; }
            set
            {
                this._pruntype = value;
                if (null != value)
                {
                    if (!_assignedDbType)
                        this._pdbtype = this.GetDbTypeFromRuntimeType(value);
                    _assignedRuntimeType = true;
                }
            }
        }

        #endregion

        #region public string NativeType { get; set; }

        /// <summary>
        /// Gets or sets the native (db engine) type for the column
        /// </summary>
        public string NativeType { get; set; }

        #endregion

        #region public int ParameterSize {get;set;}

        /// <summary>
        /// Gets or sets the maximum size of this parameters value
        /// </summary>
        [XmlAttribute("size")]
        public int ParameterSize
        {
            get { return this._psize; }
            set { this._psize = value; }
        }

        #endregion

        #region public int ParameterIndex { get;set;}

        /// <summary>
        /// Gets or sets the parameter index
        /// </summary>
        [XmlAttribute("index")]
        public int ParameterIndex
        {
            get { return this._pindex; }
            set { this._pindex = value; }
        }

        #endregion

        #region public ParameterDirection Direction {get;set;}

        /// <summary>
        /// Gets or sets the ParameterDirection
        /// </summary>
        [XmlAttribute("direction")]
        public ParameterDirection Direction
        {
            get { return this._pdir; }
            set { this._pdir = value; }
        }

        #endregion


        //
        // .ctors
        //

        #region public DBSchemaParameter()

        /// <summary>
        /// Creates a new empty DBSchemaParameter
        /// </summary>
        public DBSchemaParameter()
            : this(string.Empty)
        {
        }

        #endregion

        #region public DBSchemaParameter(string name)

        /// <summary>
        /// Creates a new DBSchemaParameter with the specified (provider independant name)
        /// </summary>
        /// <param name="name">The provider independant name</param>
        public DBSchemaParameter(string name)
        {
            this._pname = name;
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

        //
        // object overrides
        //

        #region public override string ToString()
        /// <summary>
        /// Returns a string representation of this parameter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Param: [{0}] (DbType: {1} ({5}), Direction : {2}, Position: {3}, Size: {4})", this.InvariantName, this.DbType, this.Direction, this.ParameterIndex, this.ParameterSize, this.RuntimeType);
        }

        #endregion


    }


    /// <summary>
    /// A collection of DBSchemaParameter objects accessible by name or by index.
    /// Note: the string comparison is case INsensitive
    /// </summary>
    public class DBSchemaParameterCollection 
        : List<DBSchemaParameter>
    {

        //
        // public ctors
        //

        #region public DBSchemaParameterCollection()

        /// <summary>
        /// Creates a new DBSchemaParameterCollection
        /// </summary>
        public DBSchemaParameterCollection()
            : base()
        {
        }

        #endregion

    }


}
