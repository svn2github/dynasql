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
    /// DBComparison is an abstract class with static 
    /// factory methods to create concrete implementations of comparison operations
    /// </summary>
    public abstract class DBComparison : DBClause
    {
        
        //
        // static factory methods
        //

        #region public static DBComparison Equal(DBClause from, DBClause to)
        /// <summary>
        /// Creates an equal comparison for the left and right clauses
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static DBComparison Equal(DBClause from, DBClause to)
        {
            DBBinaryComparisonRef compref = new DBBinaryComparisonRef();
            compref.Left = from;
            compref.Right = to;
            compref.CompareOperator = Perceiveit.Data.Compare.Equals;

            return compref;
        }

        #endregion

        #region public static DBComparison Compare(DBClause from, Compare op, DBClause to)

        /// <summary>
        /// Creates a comparison operation between the left and right clauses
        /// </summary>
        /// <param name="from"></param>
        /// <param name="op"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static DBComparison Compare(DBClause from, Compare op, DBClause to)
        {
            if (null == from)
                throw new ArgumentNullException("from");
            if (null == to)
                throw new ArgumentNullException("to");

            DBBinaryComparisonRef compref = new DBBinaryComparisonRef();
            compref.Left = from;
            compref.Right = to;
            compref.CompareOperator = op;

            return compref;
        }

        #endregion

        #region public static DBComparison Between(DBClause value, DBClause min, DBClause max)

        /// <summary>
        /// Creates a teriary between operation for the value and the min and max clauses
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static DBComparison Between(DBClause value, DBClause min, DBClause max)
        {
            DBTertiaryComparisonRef tri = new DBTertiaryComparisonRef();
            tri.MinValue = min;
            tri.MaxValue = max;
            tri.TertiaryOperator = TertiaryOp.Between;
            tri.CompareTo = value;

            return tri;
        }

        #endregion

        #region public static DBComparison In(DBClause fld, params DBClause[] values)

        /// <summary>
        /// Creates a included in comparison using the parameter values
        /// </summary>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DBComparison In(DBClause fld, params DBClause[] values)
        {
            DBValueGroup grp = DBValueGroup.All(values);

            return Compare(fld, Perceiveit.Data.Compare.In, grp);
        }

        #endregion

        #region public static DBComparison In(DBClause fld, DBClause clause)
        /// <summary>
        /// Creates an included in clause
        /// </summary>
        /// <param name="fld"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static DBComparison In(DBClause fld, DBClause clause)
        {
            return Compare(fld, Perceiveit.Data.Compare.In, clause);
        }

        #endregion

        #region public static DBComparison Exists(DBClause clause)

        /// <summary>
        /// Creates an EXISTS comparison
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static DBComparison Exists(DBClause clause)
        {
            DBUnaryComparisonRef un = new DBUnaryComparisonRef();
            un.UnaryOperator = Perceiveit.Data.UnaryOp.Exists;
            un.ToOperateOn = clause;

            return un;
        }

        #endregion

        #region public static DBComparison Not(DBClause clause)

        /// <summary>
        /// Creates an unary NOT comparison
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static DBComparison Not(DBClause clause)
        {
            DBUnaryComparisonRef un = new DBUnaryComparisonRef();
            un.UnaryOperator = UnaryOp.Not;
            un.ToOperateOn = clause;
            return un;
        }

        #endregion

        //
        // internal parameterless factory methods
        //

        #region internal static DBComparison Not()

        internal static DBComparison Not()
        {
            DBUnaryComparisonRef un = new DBUnaryComparisonRef();
            un.UnaryOperator = UnaryOp.Not;
            un.ToOperateOn = null;
            return un;
        }

        #endregion

        #region internal static DBComparison Between()

        internal static DBComparison Between()
        {
            DBTertiaryComparisonRef comp = new DBTertiaryComparisonRef();
            comp.MinValue = null;
            comp.MaxValue = null;
            comp.CompareTo = null;
            comp.TertiaryOperator = TertiaryOp.Between;

            return comp;
        }

        #endregion

        #region internal static DBComparison Compare()

        internal static DBComparison Compare()
        {
            DBBinaryComparisonRef comp = new DBBinaryComparisonRef();
            comp.Left = null;
            comp.Right = null;
            comp.CompareOperator = Perceiveit.Data.Compare.Equals;

            return comp;
        }

        #endregion
    }

    /// <summary>
    /// internal implementation of an Unary comparison (one operand).
    /// </summary>
    internal class DBUnaryComparisonRef : DBComparison
    {

        #region public UnaryOp UnaryOperator {get;set;}

        private UnaryOp _unaryop;
        /// <summary>
        /// The Unary operator this comparison uses
        /// </summary>
        public UnaryOp UnaryOperator
        {
            get { return _unaryop; }
            set { _unaryop = value; }
        }

        #endregion

        #region public DBClause ToOperateOn {get;set;}

        private DBClause _toop;

        /// <summary>
        /// The clause that will have the operation applied to
        /// </summary>
        public DBClause ToOperateOn
        {
            get { return _toop; }
            set { _toop = value; }
        }

        #endregion

        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this.ToOperateOn != null)
            {
                builder.BeginBlock();
                builder.WriteOperator((Operator)this.UnaryOperator);
                this.ToOperateOn.BuildStatement(builder);
                builder.EndBlock();
                return true;
            }
            else
                return false;
        }

        #endregion

        //
        // XML Serializtion
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.UnaryOp; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.Operator, this.UnaryOperator.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.ToOperateOn.WriteXml(writer, context);
            return true;
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;

            if (this.IsAttributeMatch(XmlHelper.Operator, reader, context))
                this.UnaryOperator = (UnaryOp)Enum.Parse(typeof(UnaryOp), reader.Value, true);
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            this.ToOperateOn = this.ReadNextInnerClause(this.XmlElementName, reader, context);
            return true;
        }

        #endregion

    }

    /// <summary>
    /// internal implementation of a binary comparison (2 operands)
    /// </summary>
    internal class DBBinaryComparisonRef : DBComparison
    {

        #region public DBClause Left {get; set;}

        private DBClause _left;

        public DBClause Left
        {
            get { return _left; }
            set { _left = value; }
        }

        #endregion

        #region public DBClause Right {get; set;}

        private DBClause _right;

        public DBClause Right
        {
            get { return _right; }
            internal set { _right = value; }
        }

        #endregion

        #region public Compare CompareOperator {get; set;}

        private Compare _op;

        public Compare CompareOperator
        {
            get { return _op; }
            internal set { _op = value; }
        }

        #endregion

        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginBlock();
            this.Left.BuildStatement(builder);
            builder.WriteOperator((Operator)this.CompareOperator);
            this.Right.BuildStatement(builder);
            builder.EndBlock();

            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Compare; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.Operator, this.CompareOperator.ToString(), context);

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

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            if (this.IsAttributeMatch(XmlHelper.Operator, reader, context))
                this.CompareOperator = (Compare)Enum.Parse(typeof(Compare), reader.Value, true);
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = false;
            if (this.IsElementMatch(XmlHelper.LeftOperand, reader, context)
                 && !reader.IsEmptyElement && reader.Read())
            {
                this.Left = ReadNextInnerClause(XmlHelper.LeftOperand, reader, context);
            }
            else if (this.IsElementMatch(XmlHelper.RightOperand, reader, context)
                && !reader.IsEmptyElement && reader.Read())
            {
                this.Right = this.ReadNextInnerClause(XmlHelper.RightOperand, reader, context);
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        
        #endregion

    }

    /// <summary>
    /// internal implementation of a tertiary comparison (3 operands)
    /// </summary>
    internal class DBTertiaryComparisonRef : DBComparison
    {
        //
        // public properties
        //

        #region public DBClause CompareTo {get;set;}

        private DBClause _comp;

        public DBClause CompareTo
        {
            get { return _comp; }
            set { _comp = value; }
        }

        #endregion

        #region public DBClause MaxValue {get;set}

        private DBClause _max;

        public DBClause MaxValue
        {
            get { return _max; }
            set { _max = value; }
        }

        #endregion

        #region public DBClause MinValue {get;set;}

        private DBClause _min;

        public DBClause MinValue
        {
            get { return this._min; }
            set { this._min = value; }
        }

        #endregion

        #region public TertiaryOp TertiaryOperator

        private TertiaryOp _op;

        public TertiaryOp TertiaryOperator
        {
            get { return _op; }
            internal set { _op = value; }
        }

        #endregion


        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginBlock();
            this.CompareTo.BuildStatement(builder);
            builder.BeginTertiaryOperator((Operator)this.TertiaryOperator);
            this.MinValue.BuildStatement(builder);
            builder.ContinueTertiaryOperator((Operator)this.TertiaryOperator);
            this.MaxValue.BuildStatement(builder);
            builder.EndTertiaryOperator((Operator)this.TertiaryOperator);
            builder.EndBlock();

            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Between; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.Operator, this.TertiaryOperator.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.CompareTo != null)
            {
                this.WriteStartElement(XmlHelper.Compare, writer, context);
                this.CompareTo.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.Compare, writer, context);
            }
            if (this.MinValue != null)
            {
                this.WriteStartElement(XmlHelper.MinValue, writer, context);
                this.MinValue.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.MinValue, writer, context);
            }
            if (this.MaxValue != null)
            {
                this.WriteStartElement(XmlHelper.MaxValue, writer, context);
                this.MaxValue.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.MaxValue, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = false;
            if (this.IsElementMatch(XmlHelper.Compare, reader, context)
                 && !reader.IsEmptyElement && reader.Read())
            {
                this.CompareTo = this.ReadNextInnerClause(XmlHelper.Compare, reader, context);
                b = (this.CompareTo != null);
            }
            else if (this.IsElementMatch(XmlHelper.MinValue, reader, context)
                     && !reader.IsEmptyElement && reader.Read())
            {
                this.MinValue = this.ReadNextInnerClause(XmlHelper.MinValue, reader, context);
                b = (this.MinValue != null);
            }
            else if (this.IsElementMatch(XmlHelper.MaxValue, reader, context)
                        && !reader.IsEmptyElement && reader.Read())
            {
                this.MaxValue = this.ReadNextInnerClause(XmlHelper.MaxValue, reader, context);
                b = (this.MaxValue != null);
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

    }
}
