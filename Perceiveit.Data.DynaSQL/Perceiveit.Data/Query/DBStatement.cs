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
    /// Abstract base class for statements - exist independently as an executable block. They also the malles component that supports the ToSQLString
    /// </summary>
    public abstract class DBStatement : DBClause
    {

        //
        // SQL String methods
        //

        #region public string ToSQLString(DBDatabase fordatabase)

        /// <summary>
        /// Generates the implementation specific sql statement for this query. 
        /// </summary>
        /// <param name="fordatabase">The DBDatabase to use for generation of the SQL statement</param>
        /// <returns>This SQL statement as a string</returns>
        public string ToSQLString(DBDatabase fordatabase)
        {
            return fordatabase.GetCommandText(this);
        }

        #endregion
    }
}
