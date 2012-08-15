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
    /// Abstract class that extends DBExpressionSet to support calculations
    /// </summary>
    public abstract class DBCalculableExpressionSet : DBExpressionSet, IDBCalculable
    {
        /// <summary>
        /// Supports the calculation of the last expression added to this set and the provided clause using the binary operation
        /// </summary>
        /// <param name="op"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        protected abstract DBCalculableExpressionSet Calculate(BinaryOp op, DBClause clause);

        //
        // interface implementations
        //

        #region DBClause IDBCalculable.Calculate(BinaryOp op, DBClause dbref)
        /// <summary>
        /// Explicit implementation of the IDBCalculable.Calculate method
        /// </summary>
        /// <param name="op"></param>
        /// <param name="dbref"></param>
        /// <returns></returns>
        DBClause IDBCalculable.Calculate(BinaryOp op, DBClause dbref)
        {
            return this.Calculate(op, dbref);
        }

        #endregion

        //
        // operator overrides
        //

        #region public static DBCalculableExpressionSet operator +(DBCalculableExpressionSet left, DBClause right)

        /// <summary>
        /// Adds the last operation in the passed set with the right clause 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableExpressionSet operator +(DBCalculableExpressionSet left, DBClause right)
        {
            return left.Calculate(BinaryOp.Add, right);
        }

        #endregion

        #region public static DBCalculableExpressionSet operator -(DBCalculableExpressionSet left, DBClause right)
        /// <summary>
        /// Subtracts the last operation in the passed set with the right clause 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableExpressionSet operator -(DBCalculableExpressionSet left, DBClause right)
        {
            return left.Calculate(BinaryOp.Subtract, right);
        }

        #endregion

        #region public static DBCalculableExpressionSet operator *(DBCalculableExpressionSet left, DBClause right)
        /// <summary>
        /// Multiplies the last operation in the passed set with the right clause 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableExpressionSet operator *(DBCalculableExpressionSet left, DBClause right)
        {
            return left.Calculate(BinaryOp.Multiply, right);
        }

        #endregion

        #region public static DBCalculableExpressionSet operator /(DBCalculableExpressionSet left, DBClause right)
        /// <summary>
        /// Divides the last operation in the passed set with the right clause 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableExpressionSet operator /(DBCalculableExpressionSet left, DBClause right)
        {
            return left.Calculate(BinaryOp.Divide, right);
        }

        #endregion

        #region public static DBCalculableExpressionSet operator %(DBCalculableExpressionSet left, DBClause right)
        /// <summary>
        /// Modulo's the last operation in the passed set with the right clause 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableExpressionSet operator %(DBCalculableExpressionSet left, DBClause right)
        {
            return left.Calculate(BinaryOp.Modulo, right);
        }

        #endregion

    }
}
