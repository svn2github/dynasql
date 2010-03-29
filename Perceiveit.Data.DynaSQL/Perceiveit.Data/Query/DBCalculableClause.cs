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
    public abstract class DBCalculableClause : DBClause, IDBCalculable
    {
        public virtual DBCalc Calculate(BinaryOp op, DBClause dbref)
        {
            DBCalc calc = DBCalc.Calculate(this, op, dbref);
            return calc;
        }

        public DBCalc Plus(int value)
        {
            return Plus(DBConst.Const(value));
        }

        public DBCalc Plus(double value)
        {
            return Plus(DBConst.Const(value));
        }

        public virtual DBCalc Plus(DBClause dbref)
        {
            DBCalc calc = DBCalc.Plus(this, dbref);
            return calc;
        }

        public DBCalc Minus(int value)
        {
            return Minus(DBConst.Const(value));
        }

        public DBCalc Minus(double value)
        {
            return Minus(DBConst.Const(value));
        }

        public virtual DBCalc Minus(DBClause dbref)
        {
            DBCalc calc = DBCalc.Minus(this, dbref);
            return calc;
        }

        public DBCalc Times(int value)
        {
            return Times(DBConst.Const(value));
        }

        public DBCalc Times(double value)
        {
            return Times(DBConst.Const(value));
        }

        public virtual DBCalc Times(DBClause dbref)
        {
            DBCalc calc = DBCalc.Times(this, dbref);
            return calc;
        }

        public DBCalc Divide(int value)
        {
            return Divide(DBConst.Const(value));
        }

        public DBCalc Divide(double value)
        {
            return Divide(DBConst.Const(value));
        }

        public virtual DBCalc Divide(DBClause dbref)
        {
            DBCalc calc = DBCalc.Divide(this, dbref);
            return calc;
        }

        public virtual DBCalc Modulo(DBClause dbref)
        {
            DBCalc calc = DBCalc.Modulo(this, dbref);
            return calc;
        }


        #region IDBCalculable Members


        DBClause IDBCalculable.Calculate(BinaryOp op, DBClause dbref)
        {
            return this.Calculate(op, dbref);
        }

        #endregion

        //
        // operator overrides
        //

        public static DBCalculableClause operator +(DBCalculableClause left, DBClause right)
        {
            return left.Plus(right);
        }


        public static DBCalculableClause operator -(DBCalculableClause left, DBClause right)
        {
            return left.Minus(right);
        }


        public static DBCalculableClause operator *(DBCalculableClause left, DBClause right)
        {
            return left.Times(right);
        }

        public static DBCalculableClause operator /(DBCalculableClause left, DBClause right)
        {
            return left.Divide(right);
        }

        public static DBCalculableClause operator %(DBCalculableClause left, DBClause right)
        {
            return left.Modulo(right);
        }
    }
}
