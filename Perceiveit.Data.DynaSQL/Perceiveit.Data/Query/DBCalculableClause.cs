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
    /// <summary>
    /// DBClaculable clause extends the DBClause class to support calculation operations such as plus, minus etc.
    /// It also implements the operator overloads for the standard operations (+,-,/, etc...)
    /// </summary>
    /// <remarks>If you want your classes to support inclusion in calculations then inherit from this class</remarks>
    public abstract class DBCalculableClause : DBClause, IDBCalculable
    {

        #region public virtual DBCalc Calculate(BinaryOp op, DBClause clause)

        /// <summary>
        /// Add a calculation operation to the statement using this instance as the left side, 
        /// and the clause as the right side, with the specified operation
        /// </summary>
        /// <param name="op"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public virtual DBCalc Calculate(BinaryOp op, DBClause clause)
        {
            DBCalc calc = DBCalc.Calculate(this, op, clause);
            return calc;
        }

        #endregion

        #region public DBCalc Plus(int value) + 2 overloads

        /// <summary>
        /// Add an addition operation to this clause
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Plus(int value)
        {
            return Plus(DBConst.Const(value));
        }

        /// <summary>
        /// Add an addition operation to this clause
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Plus(double value)
        {
            return Plus(DBConst.Const(value));
        }

        /// <summary>
        /// Add an addition operation to this clause
        /// </summary>
        /// <param name="dbref"></param>
        /// <returns></returns>
        public virtual DBCalc Plus(DBClause dbref)
        {
            DBCalc calc = DBCalc.Plus(this, dbref);
            return calc;
        }

        #endregion

        #region public DBCalc Minus(int value) + 2 overloads

        /// <summary>
        /// Add a subtraction to this operation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Minus(int value)
        {
            return Minus(DBConst.Const(value));
        }

        /// <summary>
        /// Add a subtraction operation to this clause
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Minus(double value)
        {
            return Minus(DBConst.Const(value));
        }

        /// <summary>
        /// Add a subtraction operation to this clause
        /// </summary>
        /// <param name="dbref"></param>
        /// <returns></returns>
        public virtual DBCalc Minus(DBClause dbref)
        {
            DBCalc calc = DBCalc.Minus(this, dbref);
            return calc;
        }

        #endregion

        #region public DBCalc Times(int value) + 2 overloads

        /// <summary>
        /// Add a multipy operation to this clause
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Times(int value)
        {
            return Times(DBConst.Const(value));
        }
        
        /// <summary>
        /// Add a multiply operation to this clause
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Times(double value)
        {
            return Times(DBConst.Const(value));
        }

        /// <summary>
        /// Add a multiply operation to this clause
        /// </summary>
        /// <param name="dbref"></param>
        /// <returns></returns>
        public virtual DBCalc Times(DBClause dbref)
        {
            DBCalc calc = DBCalc.Times(this, dbref);
            return calc;
        }

        #endregion

        #region public DBCalc Divide(int value)

        /// <summary>
        /// Adds a division operation to the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Divide(int value)
        {
            return Divide(DBConst.Const(value));
        }

        /// <summary>
        ///  Adds a division operation to the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBCalc Divide(double value)
        {
            return Divide(DBConst.Const(value));
        }

        /// <summary>
        ///  Adds a division operation to the statement
        /// </summary>
        /// <param name="dbref"></param>
        /// <returns></returns>
        public virtual DBCalc Divide(DBClause dbref)
        {
            DBCalc calc = DBCalc.Divide(this, dbref);
            return calc;
        } 
        
        #endregion

        #region public virtual DBCalc Modulo(DBClause dbref)

        /// <summary>
        ///  Adds a modulo (remainder) operation to the statement
        /// </summary>
        /// <param name="dbref"></param>
        /// <returns></returns>
        public virtual DBCalc Modulo(DBClause dbref)
        {
            DBCalc calc = DBCalc.Modulo(this, dbref);
            return calc;
        }

        #endregion


        #region IDBCalculable Members


        DBClause IDBCalculable.Calculate(BinaryOp op, DBClause dbref)
        {
            return this.Calculate(op, dbref);
        }

        #endregion

        //
        // operator overrides
        //

        /// <summary>
        /// Add left and right clauses
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableClause operator +(DBCalculableClause left, DBClause right)
        {
            return left.Plus(right);
        }

        /// <summary>
        /// Subtract left and right clauses
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableClause operator -(DBCalculableClause left, DBClause right)
        {
            return left.Minus(right);
        }

        /// <summary>
        /// Multiply left and right clauses
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableClause operator *(DBCalculableClause left, DBClause right)
        {
            return left.Times(right);
        }

        /// <summary>
        /// Divide left and right clauses
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableClause operator /(DBCalculableClause left, DBClause right)
        {
            return left.Divide(right);
        }


        /// <summary>
        /// Get the modulo of the left and right clauses
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalculableClause operator %(DBCalculableClause left, DBClause right)
        {
            return left.Modulo(right);
        }
    }
}
