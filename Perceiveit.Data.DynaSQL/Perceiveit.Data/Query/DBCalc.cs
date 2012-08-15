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
    /// Creates a new binary calculation
    /// </summary>
    public abstract class DBCalc : DBCalculableClause
    {

        //
        // public properties
        //

        #region public BinaryOp BinaryOp {get;set;}

        private BinaryOp _op;
        /// <summary>
        /// Gets or sets the binary operation type
        /// </summary>
        public BinaryOp BinaryOp
        {
            get { return _op; }
            set { _op = value; }
        }

        #endregion

        #region public DBClause Left {get;set;}

        private DBClause _left;
        /// <summary>
        /// Gets or sets the left argument
        /// </summary>
        public DBClause Left
        {
            get { return _left; }
            set { _left = value; }
        }

        #endregion

        #region public DBClause Right {get;set;}

        private DBClause _right;
        /// <summary>
        /// Gets or sets the right argument
        /// </summary>
        public DBClause Right
        {
            get { return _right; }
            set { _right = value; }
        }

        #endregion

        #region public string Alias {get;set;}

        private string _as;

        /// <summary>
        /// Gets or sets the alias name
        /// </summary>
        public string Alias
        {
            get { return _as; }
            set { _as = value; }
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBCalc Calculate(DBClause left, BinaryOp op, DBClause right)
        /// <summary>
        /// Creates a new binary calculation with the left, op and right arguments
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Calculate(DBClause left, BinaryOp op, DBClause right)
        {
            if (null == left)
                throw new ArgumentNullException("left");
            if (null == right)
                throw new ArgumentNullException("right");

            DBCalcRef cref = new DBCalcRef();
            cref.BinaryOp = op;
            cref.Left = left;
            cref.Right = right;

            return cref;
        }

        #endregion

        #region public static DBCalc Plus(DBClause left, DBClause right)
        /// <summary>
        /// Creates a new Addition operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Plus(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.Add, right);
        }

        #endregion

        #region public static DBCalc Minus(DBClause left, DBClause right)
        /// <summary>
        /// Creates a new Subtract operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Minus(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.Subtract, right);
        }

        #endregion

        #region public static DBCalc Times(DBClause left, DBClause right)
        /// <summary>
        /// Creates a new  multiply operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Times(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.Multiply, right);
        }

        #endregion

        #region public static DBCalc Divide(DBClause left, DBClause right)

        /// <summary>
        /// Creates a new division operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Divide(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.Divide, right);
        }

        #endregion

        #region public static DBCalc Modulo(DBClause left, DBClause right)
        /// <summary>
        /// Creates a new Modulo operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Modulo(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.Modulo, right);
        }

        #endregion

        /// <summary>
        /// Creates a new Or (||) operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBCalc Or(DBClause left, DBClause right)
        {
            return Calculate(left, BinaryOp.BitwiseOr, right);
        }

        #region internal static DBCalc Calculate()

        internal static DBCalc Calculate()
        {
            DBCalcRef cref = new DBCalcRef();
            return cref;
        }

        #endregion


        
    }


    internal class DBCalcRef : DBCalc, IDBAlias
    {

        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginBlock();
            this.Left.BuildStatement(builder);
            builder.WriteOperator((Operator)this.BinaryOp);
            this.Right.BuildStatement(builder);
            builder.EndBlock();
            if (string.IsNullOrEmpty(this.Alias) == false)
            {
                builder.WriteAlias(this.Alias);
            }
            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override string XmlElementName
        
        protected override string XmlElementName
        {
            get { return XmlHelper.Calculation; }
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Operator, reader, context))
            {
                this.BinaryOp = (BinaryOp)Enum.Parse(typeof(BinaryOp), reader.Value, true);
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
            {
                this.Alias = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsElementMatch(XmlHelper.LeftOperand, reader, context)
                && !reader.IsEmptyElement && reader.Read())
            {
                this.Left = this.ReadNextInnerClause(XmlHelper.LeftOperand, reader, context);
                b = reader.Read();
            }
            else if (this.IsElementMatch(XmlHelper.RightOperand, reader, context)
                && !reader.IsEmptyElement && reader.Read())
            {
                this.Right = this.ReadNextInnerClause(XmlHelper.RightOperand, reader, context);
                b = reader.Read();
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.Operator, this.BinaryOp.ToString(), context);

            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAlias(writer, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Left != null)
            {
                this.WriteStartElement(XmlHelper.LeftOperand, writer, context);
                this.Left.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.LeftOperand, writer, context);
            }

            if (this.Right != null)
            {
                this.WriteStartElement(XmlHelper.RightOperand, writer, context);
                this.Right.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.RightOperand, writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        //
        // Interface Implementations
        //

        #region void IDBAlias.As(string aliasName)

        void IDBAlias.As(string aliasName)
        {
            this.Alias = aliasName;
        }

        #endregion
    }
}
