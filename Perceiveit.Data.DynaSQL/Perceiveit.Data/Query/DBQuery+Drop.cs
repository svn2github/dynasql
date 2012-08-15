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
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    public partial class DBQuery
    {

        public static class Drop
        {

            public static DBDropTableQuery Table(string name)
            {
                return DBDropTableQuery.DropTable(name);
            }

            public static DBDropTableQuery Table(string owner, string name)
            {
                return DBDropTableQuery.DropTable(owner, name);
            }

            public static DBDropSequenceQuery Sequence(string name)
            {
                return DBDropSequenceQuery.DropSequence(name);
            }

            public static DBDropSequenceQuery Sequence(string owner, string name)
            {
                return DBDropSequenceQuery.DropSequence(owner, name);
            }

            public static DBDropViewQuery View(string name)
            {
                return DBDropViewQuery.DropView(name);
            }

            public static DBDropViewQuery View(string owner, string name)
            {
                return DBDropViewQuery.DropView(owner, name);
            }


            public static DBDropIndexQuery Index(string name)
            {
                return DBDropIndexQuery.DropIndex(name);
            }

            public static DBDropIndexQuery Index(string owner, string name)
            {
                return DBDropIndexQuery.DropIndex(owner, name);
            }

            public static DBDropProcedureQuery StoredProcedure(string name)
            {
                return DBDropProcedureQuery.DropProcedure(name);
            }

            public static DBDropProcedureQuery StoredProcedure(string owner, string name)
            {
                return DBDropProcedureQuery.DropProcedure(owner, name);
            }
            
        }
    }
}
