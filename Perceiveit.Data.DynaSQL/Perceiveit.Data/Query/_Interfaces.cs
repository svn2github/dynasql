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

namespace Perceiveit.Data.Query
{
    public interface IDBValueSource
    {
        bool HasType { get; }
        System.Data.DbType DbType { get; }
        string Name { get; set;}
        object Value { get;}
        int Size { get; }
        System.Data.ParameterDirection Direction { get; }
    }

    public interface IDBAlias
    {
        void As(string aliasName);
    }

    public interface IDBBoolean
    {
        DBClause And(DBClause reference);

        DBClause Or(DBClause reference);
    }

    public interface IDBCalculable
    {
        DBClause Calculate(BinaryOp op, DBClause dbref);
    }

    public interface IDBArregate
    {
        DBClause Aggregate(AggregateFunction func, DBClause dbref);
    }

    public interface IDBComparable
    {
        DBClause Compare(Compare op, DBClause dbref);
    }

    public interface IDBJoinable
    {
        DBClause On(DBComparison compare);
    }

    
}
