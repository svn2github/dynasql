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
    /// <summary>
    /// represents a defined function in the database schema
    /// </summary>
    public class DBSchemaFunction : DBSchemaRoutine
    {
        #region ivars

        private DBSchemaParameter _retparam = null;

        #endregion
        /// <summary>
        /// Gets or sets the return parameter of this function
        /// </summary>
        public DBSchemaParameter ReturnParameter
        {
            get { return _retparam; }
            set { _retparam = value; }
        }


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

            if (null != this.ReturnParameter)
            {
                sb.Append(" (Returns:");
                sb.Append(this.ReturnParameter);
                sb.Append(")");

            }
        }

        #endregion


    }
}
