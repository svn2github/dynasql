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

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Simple helper class to identify specific sets of DBSchemaTypes
    /// </summary>
    /// <remarks>By using a separate class for a flags enumeration we do 
    /// not upset the ToString() and Parse() methods.</remarks>
    internal static class SupportedSchemaOptions
    {
        /// <summary>
        /// All DBSchemaTypes
        /// </summary>
        internal static DBSchemaTypes All
        {
            get
            {
                return DBSchemaTypes.Function
                        | DBSchemaTypes.StoredProcedure
                        | DBSchemaTypes.Table
                        | DBSchemaTypes.Index
                        | DBSchemaTypes.View
                        | DBSchemaTypes.CommandScripts;
            }
        }

        /// <summary>
        /// Only the Tables, Views ands Indexes flags
        /// </summary>
        internal static DBSchemaTypes TablesViewsAndIndexes
        {
            get
            {
                return DBSchemaTypes.Table | DBSchemaTypes.View | DBSchemaTypes.Index;
            }
        }
    }
}
