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
    /// Interface that all parameters passed to a query must implement so that the statement builder and 
    /// any other query can interrogate the instance about its source, type and value.
    /// </summary>
    public interface IDBValueSource
    {
        /// <summary>
        /// Returns true if this instance has a specific DbType
        /// </summary>
        bool HasType { get; }

        /// <summary>
        /// Gets the DbType of this intances value
        /// </summary>
        System.Data.DbType DbType { get; }

        /// <summary>
        /// Gets or sets the name of this value source
        /// </summary>
        string Name { get; set;}

        /// <summary>
        /// Gets the actual value of this source
        /// </summary>
        object Value { get;}

        /// <summary>
        /// Gets the size of this instances value (length for strings, bit count for others). Implementors should return -1 if not applicable
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the direction of this value source.
        /// </summary>
        System.Data.ParameterDirection Direction { get; }
    }


    /// <summary>
    /// Basic identifying interface used by query instances to state if they can be aliased (e.g. TableX AS X, columnX AS X).
    /// </summary>
    public interface IDBAlias
    {
        /// <summary>
        /// Sets the alias name of the implementing instance
        /// </summary>
        /// <param name="aliasName"></param>
        void As(string aliasName);
    }

    /// <summary>
    /// Interface implemented by classes that can support Boolean operations on them.
    /// </summary>
    /// <remarks>Any class that implements should 
    /// return a new DBBooleanOp clause with itself as the left parameter and the 
    /// passed clause as the right parameter</remarks>
    public interface IDBBoolean
    {
        /// <summary>
        /// Joins this instance and the passed parameter 
        /// with an AND operation and returns the new result clause.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        DBClause And(DBClause reference);

        /// <summary>
        /// Joins this instance and the passed clause with
        /// an OR operation and returns a new result clause.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        DBClause Or(DBClause reference);
    }

    /// <summary>
    /// Interface implemented by classes that can support calculations
    /// </summary>
    /// <remarks>Any class that implements should by default
    /// return a new DBCalculableClause instance with itself as the left parameter and the 
    /// passed clause as the right parameter</remarks>
    public interface IDBCalculable
    {
        /// <summary>
        /// Joins this instance and the passed clause with a new calculation using the binary operation
        /// and returns the new result clause.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="dbref"></param>
        /// <returns></returns>
        DBClause Calculate(BinaryOp op, DBClause dbref);
    }

    /// <summary>
    /// Interface implemented by classes that can support aggregation operations
    /// </summary>
    /// <remarks>Any class that implements this interface should by default
    /// return a new DBAggregate clause based on itself</remarks>
    public interface IDBAggregate
    {
        /// <summary>
        /// Creates a new Aggregate function for the passed clause and the function type.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="dbref"></param>
        /// <returns></returns>
        DBClause Aggregate(AggregateFunction func, DBClause dbref);
    }

    /// <summary>
    /// Interface implemented by classes that can support comparison
    /// </summary>
    /// <remarks>Any class that implements this interface should by default
    /// retrun a new DBComparison instance based on itself and the passed clause comparing using the op parameter</remarks>
    public interface IDBComparable
    {
        /// <summary>
        /// Adds a compare operation with this instance and the passed clause using the op comparison
        /// </summary>
        /// <param name="op"></param>
        /// <param name="dbref"></param>
        /// <returns></returns>
        DBClause Compare(Compare op, DBClause dbref);
    }

    /// <summary>
    /// Interface implemented by classes that support table joins
    /// </summary>
    public interface IDBJoinable
    {
        /// <summary>
        /// Sets the join on filter for this instance based on the passed comparison.
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        DBClause On(DBComparison compare);
    }

    
}
