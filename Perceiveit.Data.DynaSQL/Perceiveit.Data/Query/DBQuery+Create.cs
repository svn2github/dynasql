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


        public static class Create
        {

            public static DBCreateTableQuery Table(string name)
            {

                return DBCreateTableQuery.Table(name);
            }

            public static DBCreateTableQuery Table(string owner, string name)
            {
                return DBCreateTableQuery.Table(owner, name);
            }

            public static DBCreateSequenceQuery Sequence(string name)
            {
                return DBCreateSequenceQuery.Sequence(name);
            }

            public static DBCreateSequenceQuery Sequence(string owner, string name)
            {
                return DBCreateSequenceQuery.Sequence(owner, name);
            }
            
            
            public static DBCreateViewQuery View(string name)
            {
                return DBCreateViewQuery.CreateView(name);
            }

            public static DBCreateViewQuery View(string owner, string name)
            {
                return DBCreateViewQuery.CreateView(owner, name);
            }



            public static DBCreateIndexQuery Index(string name)
            {
                return DBCreateIndexQuery.Index(name);
            }

            public static DBCreateIndexQuery Index(string owner, string name)
            {
                return DBCreateIndexQuery.Index(owner, name);
            }

            public static DBCreateIndexQuery Index(bool unique, string name)
            {
                DBCreateIndexQuery idx = Index(name);
                return idx.Unique();
            }

            public static DBCreateIndexQuery Index(bool unique, string owner, string name)
            {
                DBCreateIndexQuery idx = Index(owner, name);
                return idx.Unique();
            }

            public static DBCreateProcedureQuery StoredProcedure(string name)
            {
                DBCreateProcedureQuery q = DBCreateProcedureQuery.CreateProcedure(name);
                return q;
            }

            public static DBCreateProcedureQuery StoredProcedure(string owner, string name)
            {
                DBCreateProcedureQuery q = DBCreateProcedureQuery.CreateProcedure(owner, name);
                return q;
            }

        }
    }
}
