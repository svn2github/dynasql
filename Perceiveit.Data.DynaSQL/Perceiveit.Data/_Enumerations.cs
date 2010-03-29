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

namespace Perceiveit.Data
{
    public enum Compare
    {
        Equals = 1,
        LessThan = 2,
        GreaterThan = 3,
        LessThanEqual = 4,
        GreaterThanEqual = 5,
        NotEqual = 6,
        Like = 7,
        Is = 8,
        In = 9,
        NotIn = 10
    }

    public enum BinaryOp
    {
        Add = 100,
        Subtract = 101,
        Multiply = 102,
        Divide = 103,
        BitShiftLeft = 104,
        BitShiftRight = 105,
        Modulo = 106,
        BitwiseAnd = 107,
        BitwiseOr = 108,
        Concat = 109
    }

    public enum UnaryOp
    {
        Not = 200,
        Exists = 201
    }

    public enum TertiaryOp
    {
        Between = 300
    }

    public enum Operator
    {
        Equals = 1,
        LessThan = 2,
        GreaterThan = 3,
        LessThanEqual = 4,
        GreaterThanEqual = 5,
        NotEqual = 6,
        Like = 7,
        Is = 8,
        In = 9,
        NotIn = 10,

        Add = 100,
        Subtract = 101,
        Multiply = 102,
        Divide = 103,
        BitShiftLeft = 104,
        BitShiftRight = 105,
        Modulo = 106,
        BitwiseAnd = 107,
        BitwiseOr = 108,
        Concat = 109,

        Not = 200,
        Exists = 201,

        Between = 300,

        And = 350,
        Or = 351,
        XOr = 352

    }

    public enum BooleanOp
    {
        And = 350,
        Or = 351,
        XOr = 352,
    }

    public enum AggregateFunction
    {
        Count = 400,
        Sum = 401,
        Avg = 402,
        Min = 403,
        Max = 404
    }

    public enum Order
    {
        Ascending,
        Descending,
        Default
    }

    public enum JoinType
    {
        InnerJoin,
        LeftOuter,
        CrossProduct,
        RightOuter,
        Join
    }

    public enum Function
    {
        Unknown = -1,
        GetDate = 0,
        LastID = 1,
        IsNull = 2
    }

    public enum TopType
    {
        Count,
        Percent
    }

    /// <summary>
    /// Defines the schema types in a database
    /// </summary>
    [Flags()]
    public enum DBSchemaTypes
    {
        Table = 1,
        View = 2,
        StoredProcedure = 4,
        Function = 8,
        Index = 16,
        CommandScripts = 32
    }

    public enum DBParameterLayout
    {
        Named,
        Positional
    }


}
