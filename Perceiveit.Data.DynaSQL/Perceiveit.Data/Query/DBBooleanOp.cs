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
    /// Defines a boolean operation - AND, OR, XOR
    /// </summary>
    public abstract class DBBooleanOp : DBCalculableClause
    {

        #region public DBClause Left {get;set;}

        private DBClause _left;

        /// <summary>
        /// Gets or sets the left argument of the operation
        /// </summary>
        public DBClause Left 
        {
            get { return _left; }
            set { _left = value; }
        }

        #endregion

        #region public BooleanOp Operator {get;set;}

        private BooleanOp _op;
        /// <summary>
        /// Gets or sets the boolean operator.
        /// </summary>
        public BooleanOp Operator
        {
            get { return _op; }
            set { _op = value; }
        }

        #endregion

        #region public DBClause Right {get;set;}

        private DBClause _right;
        /// <summary>
        /// Gets or sets the right argument of the operation.
        /// </summary>
        public DBClause Right
        {
            get { return _right; }
            set { _right = value; }
        }

        #endregion

        //
        // static factory method(s)
        //

        #region public static DBBooleanOp Compare(DBClause left, BooleanOp op, DBClause right)

        /// <summary>
        /// Creates a new DBBooleanOp to compare the left and right clauses using the specified boolean operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static DBBooleanOp Compare(DBClause left, BooleanOp op, DBClause right)
        {
            DBBooleanOpRef oref = new DBBooleanOpRef();
            oref.Left = left;
            oref.Right = right;
            oref.Operator = op;
            return oref;
        }

        #endregion

        #region internal static DBBooleanOp Compare()

        internal static DBBooleanOp Compare()
        {
            return new DBBooleanOpRef();
        }

        #endregion

    }

    internal class DBBooleanOpRef : DBBooleanOp
    {
        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginBlock();
            Left.BuildStatement(builder);
            builder.WriteOperator((Operator)this.Operator);
            Right.BuildStatement(builder);
            builder.EndBlock();
            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        protected override string XmlElementName
        {
            get { return XmlHelper.BooleanOperator; }
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.Operator, this.Operator.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

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

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Operator, reader, context))
            {
                this.Operator = (BooleanOp)Enum.Parse(typeof(BooleanOp), reader.Value, true);
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;

            if (this.IsElementMatch(XmlHelper.LeftOperand, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.Left = this.ReadNextInnerClause(XmlHelper.LeftOperand, reader, context);
                b = true;
            }
            else if (this.IsElementMatch(XmlHelper.RightOperand, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.Right = this.ReadNextInnerClause(XmlHelper.RightOperand, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);
            return b;
        }

        
    }
}
