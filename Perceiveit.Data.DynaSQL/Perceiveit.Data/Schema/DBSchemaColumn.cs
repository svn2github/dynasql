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
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Encapsulates the standard data properties of a Database column 
    /// </summary>
    [Serializable()]
    public abstract class DBSchemaColumn
    {
        #region ivars

        private string _name;
        private DbType _type = DbType.Object;
        private Type _runtimeType = typeof(Object);
        private bool _assignedDbType = false;
        private bool _assignedRuntimeType = false;
        private int _size = -1;
        private int _ordinal = -1;

        private DBSchemaColumnFlags _flags;

        #endregion

        //
        // public properties
        //

        #region public DBSchemaColumnFlags ColumnFlags {get;set;}

        /// <summary>
        /// Gets or Sets the ColumnFlags for this column
        /// </summary>
        [XmlAttribute("flags")]
        [System.ComponentModel.Browsable(false)]
        public DBSchemaColumnFlags ColumnFlags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        #endregion

        #region public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the column
        /// </summary>
        [XmlAttribute("name")]
        public string Name 
        { 
            get { return this._name; } 
            set { this._name = value; }
        }

        #endregion

        #region public DbType DbType {get;set;}

        /// <summary>
        /// Gets or Sets the DbType of the column
        /// </summary>
        [XmlAttribute("db-type")]
        public DbType DbType
        {
            get { return this._type; }
            set 
            { 
                this._type = value;

                //if no code has explicitly set the Runtime Type value then set it here
                if(!this._assignedRuntimeType)
                    this._runtimeType = GetRuntimeTypeFromDbType(value);

                this._assignedDbType = true;
            }
        }

        #endregion

        #region public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the System.Type of the column
        /// </summary>
        [XmlIgnore()]
        public Type Type 
        {
            get { return this._runtimeType; }
            set 
            { 
                this._runtimeType = value;

                if (null != value)
                {
                    //if no code has explicitly set the dbtype then set it here
                    if (!this._assignedDbType)
                        this._type = GetDbTypeFromRuntimeType(value);

                    this._assignedRuntimeType = true;
                }
            }
        }

        #endregion

        #region public int Size { get; set; }

        /// <summary>
        /// Gets or sets the size (in bytes or characters) of the column data.
        /// Less than zero indicates (near) unlimited size
        /// </summary>
        [XmlAttribute("size")]
        public int Size 
        {
            get { return this._size; }
            set { this._size = value; }
        }

        #endregion

        #region public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or Sets the flag that identifies 
        /// if this column is readonly
        /// </summary>
        [XmlIgnore()]
        public bool ReadOnly 
        {
            get { return this.IsColumnFlagSet(DBSchemaColumnFlags.ReadOnly); }
            set { this.SetColumnFlag(DBSchemaColumnFlags.ReadOnly, value); }
        }

        #endregion

        #region public bool Nullable { get; set; }

        /// <summary>
        /// Gets or sets the flag that identifies if
        /// this column can contain a null value
        /// </summary>
        [XmlIgnore()]
        public bool Nullable 
        {
            get { return this.IsColumnFlagSet(DBSchemaColumnFlags.Nullable); }
            set { this.SetColumnFlag(DBSchemaColumnFlags.Nullable, value); }
        }

        #endregion

        #region public int OrdinalPosition {get;set;}

        /// <summary>
        /// Gets or sets the Ordinal position of this column in the Table
        /// </summary>
        [XmlAttribute("ordinal")]
        public int OrdinalPosition
        {
            get { return this._ordinal; }
            set { this._ordinal = value; }
        }

        #endregion

        //
        // .ctor
        //

        #region public DBSchemaColumn()

        /// <summary>
        /// Creates a new empty schema column
        /// </summary>
        public DBSchemaColumn()
            : this(string.Empty)
        {
        }

        #endregion

        #region public DBSchemaColumn(string name)

        /// <summary>
        /// Creates a new schema column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public DBSchemaColumn(string name)
        {
            this.Name = name;
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

        #region protected bool IsColumnFlagSet(DBSchemaColumnFlags flag)
        /// <summary>
        /// Returns true if this columns flag has been set
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool IsColumnFlagSet(DBSchemaColumnFlags flag)
        {
            return (this._flags & flag) > 0;
        }

        #endregion

        #region protected void SetColumnFlag(DBSchemaColumnFlags flag, bool value)
        /// <summary>
        /// Sets this columns flag or clears
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        protected void SetColumnFlag(DBSchemaColumnFlags flag, bool value)
        {
            if (value)
                SetColumnFlag(flag);
            else
                ClearColumnFlag(flag);
        }

        #endregion

        #region protected void SetColumnFlag(DBSchemaColumnFlags flag)
        /// <summary>
        /// Sets this columns flag
        /// </summary>
        /// <param name="flag"></param>
        protected void SetColumnFlag(DBSchemaColumnFlags flag)
        {
            _flags = _flags | flag;
        }

        #endregion 

        #region protected void ClearColumnFlag(DBSchemaColumnFlags flag)
        /// <summary>
        /// Clears the specified flag
        /// </summary>
        /// <param name="flag"></param>
        protected void ClearColumnFlag(DBSchemaColumnFlags flag)
        {
            _flags = _flags & ~flag;
        }

        #endregion

    }

    

    
}
