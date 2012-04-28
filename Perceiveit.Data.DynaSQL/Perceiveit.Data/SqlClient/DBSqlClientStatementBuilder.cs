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
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqlClient
{
    /// <summary>
    /// Generates SQL statements for the System.Data.SQLClient database engine
    /// </summary>
    /// <remarks>The base DBStatementBuilder uses the SQLClient as a reference model - not because it is a standard, 
    /// but because the engine supports the DbCommandBehavior option which all .NET providers should support to be fully compliant
    /// <para>Other implementations do have to override methods but with this one we don't</para></remarks>
    public class DBSQLClientStatementBuilder : DBStatementBuilder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <param name="properties"></param>
        /// <param name="tw"></param>
        /// <param name="ownswriter"></param>
        internal protected DBSQLClientStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }


        

    }
}
