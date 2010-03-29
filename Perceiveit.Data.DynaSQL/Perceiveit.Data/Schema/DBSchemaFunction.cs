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
    public class DBSchemaFunction : DBSchemaSproc
    {
        #region ivars

        private bool _assignedDbType = false;
        private bool _assignedRuntimeType = false;
        private DbType _retdbtype = DbType.Object;
        private Type _retruntype = null;
        private int _retsize = -1;

        #endregion


        #region public DbType ReturnDbType { get;set }

        /// <summary>
        /// Gets or sets the Return dbType for this function
        /// </summary>
        [XmlAttribute("return-type")]
        public DbType ReturnDbType
        {
            get { return this._retdbtype; }
            set
            {
                this._retdbtype = value;
                if (value != DbType.Object)
                {
                    if (!this._assignedRuntimeType)
                        this._retruntype = this.GetRuntimeTypeFromDbType(value);

                    _assignedDbType = true;
                }
            }
        }

        #endregion

        #region public Type ReturnRuntimeType {get;set;}

        /// <summary>
        /// Gets or sets the Return Runtime Type
        /// </summary>
        [XmlIgnore()]
        public Type ReturnRuntimeType
        {
            get { return this._retruntype; }
            set
            {
                this._retruntype = value;
                if (null != value)
                {
                    if (!_assignedDbType)
                        _retdbtype = this.GetDbTypeFromRuntimeType(value);
                    _assignedRuntimeType = true;
                }
            }
        }

        #endregion

        #region public int ReturnSize {get;set;}

        public int ReturnSize
        {
            get { return this._retsize; }
            set { this._retsize = value; }
        }

        #endregion

        //
        // ctors
        //

        #region public DBSchemaFunction()

        /// <summary>
        /// Creates a new empty DBSchemaFunction
        /// </summary>
        public DBSchemaFunction()
            : this(DBSchemaTypes.Function, string.Empty, string.Empty)
        {
        }

        #endregion

        #region public DBSchemaFunction(string owner, string name)
        /// <summary>
        /// Creates a new DBSchemaFunction with the specified nama and owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public DBSchemaFunction(string owner, string name)
            : this(DBSchemaTypes.Function, owner, name)
        {
        }

        #endregion

        #region protected DBSchemaFunction(DBSchemaTypes type, string owner, string name)

        /// <summary>
        /// protected primary constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBSchemaFunction(DBSchemaTypes type, string owner, string name)
            : base(type, owner, name)
        {
        }

        #endregion


        //
        // object overrides
        //

        #region protected override void ToString(StringBuilder sb)

        /// <summary>
        /// Overrides the base ToString implementation to add the parameters onto the collection
        /// </summary>
        /// <param name="sb"></param>
        protected override void ToString(StringBuilder sb)
        {
            base.ToString(sb);

            if (this._assignedDbType || this._assignedRuntimeType)
            {
                sb.Append(" (Returns:");
                sb.Append(this.ReturnDbType.ToString());
                sb.Append(", ");
                sb.Append(this.ReturnRuntimeType.ToString());
                sb.Append(")");

            }
        }

        #endregion


    }
}
