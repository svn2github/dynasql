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
    /// Defines a join from one clause to another along with the comparison.
    /// e.g INNER JOIN Table2 ON Table1.column1 = Table2.column1
    /// </summary>
    public abstract class DBJoin : DBClause, IDBAlias, IDBBoolean, IDBJoinable
    {
        private DBClause _fromcol;
        private DBClause _comp;
        private JoinType _jtype;

        //
        // public properties
        //

        #region public JoinType JoinType {get; protected set;}

        /// <summary>
        /// Gets the JoinType - Inheritors can set this value
        /// </summary>
        public JoinType JoinType
        {
            get { return _jtype; }
            protected set { _jtype = value; }
        }

        #endregion

        #region public DBClause JoinedTo {get; protected set;}

        /// <summary>
        /// Gets or sets the clause this Join refers to
        /// </summary>
        public DBClause JoinedTo
        {
            get { return _fromcol; }
            protected set { _fromcol = value; }
        }

        #endregion

        #region public DBComparison Comparison {get; protected set;}

        /// <summary>
        /// Gets or sets the Comparison used to match the joined clause with the owner
        /// </summary>
        public DBClause Comparison
        {
            get { return _comp; }
            protected set { _comp = value; }
        }

        #endregion

        //
        // Interface Implementations
        //


        #region IDBAlias - public void As(string aliasName)
        /// <summary>
        /// Assigns an alias to this joins table
        /// </summary>
        /// <param name="aliasName"></param>
        public void As(string aliasName)
        {
            ((IDBAlias)this.JoinedTo).As(aliasName);
        }

        #endregion

        #region IDBBoolean - public DBClause And(DBClause reference)
        /// <summary>
        /// Appends an AND condition to the comparison
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause And(DBClause reference)
        {
            if (this.Comparison == null)
                throw new ArgumentNullException("No existing join comparision to 'AND' with");
            else
                this.Comparison = DBBooleanOp.Compare(this.Comparison, BooleanOp.And, reference);
            return this;
        }

        #endregion

        #region IDBBoolean - public DBClause Or(DBClause reference)

        /// <summary>
        /// Appends an OR condition to the joins comparison
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause Or(DBClause reference)
        {
            if (this.Comparison == null)
                throw new ArgumentNullException("No existing join comparision to 'AND' with");
            else
                this.Comparison = DBBooleanOp.Compare(this.Comparison, BooleanOp.Or, reference);
            return this;
        }

        #endregion

        #region IDBJoinable - public DBClause On(DBComparison compare)
        /// <summary>
        /// Appends or sets the condition to this joins comparison
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBClause On(DBComparison compare)
        {
            if (this.Comparison != null)
                return this.And(compare);
            else
                this._comp = compare;
            return this;
        }

        #endregion

        

        //
        // static methods
        //

        #region public static DBJoin InnerJoin(DBReference jointo, DBFieldRef from, DBFieldRef to, ComparisonOperator comparison)
        /// <summary>
        /// Creates a new INNER JOIN with the ON set to the comparison of from and to
        /// </summary>
        /// <param name="jointo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static DBJoin InnerJoin(DBClause jointo, DBClause from, DBClause to, Compare comparison)
        {
            DBComparison compref = DBComparison.Compare(from, comparison, to);

            return InnerJoin(jointo, compref);
        }

        #endregion

        #region public static DBJoin InnerJoin(DBReference jointo, DBComparison comp)
        /// <summary>
        /// Creates a new INNER JOIN with the ON set to the comparison
        /// </summary>
        /// <param name="jointo"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static DBJoin InnerJoin(DBClause jointo, DBComparison comp)
        {
            return Join(jointo, JoinType.InnerJoin, comp);
        }

        #endregion


        #region public static DBJoin Join(DBReference jointo, JoinType joinType, DBComparison comp)

        /// <summary>
        /// Creates a new join of the specified type and the ON set to the specified comparison
        /// </summary>
        /// <param name="jointo"></param>
        /// <param name="joinType"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static DBJoin Join(DBClause jointo, JoinType joinType, DBComparison comp)
        {
            DBJoinRef jref = new DBJoinRef();
            jref.JoinType = joinType;
            jref.JoinedTo = jointo;
            jref.Comparison = comp;

            return jref;
        }

        #endregion

        #region public static DBJoin Join(DBClause jointo, JoinType joinType)

        /// <summary>
        /// Creates a new JOIN of the specified type with no comparison
        /// </summary>
        /// <param name="jointo"></param>
        /// <param name="joinType"></param>
        /// <returns></returns>
        public static DBJoin Join(DBClause jointo, JoinType joinType)
        {
            DBJoinRef join = new DBJoinRef();
            join.JoinedTo = jointo;
            join.JoinType = joinType;
            return join;
        }

        #endregion

        #region internal static DBJoin Join()
        /// <summary>
        /// Creates a new empty join
        /// </summary>
        /// <returns></returns>
        internal static DBJoin Join()
        {
            return new DBJoinRef();
        }

        #endregion
    }

    internal class DBJoinRef : DBJoin
    {
        //
        // properties
        //

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Overrides the abstract base method to return the Join element name
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Join; }
        }

        #endregion

#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement build methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (null == this.JoinedTo)
                throw new NullReferenceException(Errors.NoJoinRightReference);
            if (null == this.Comparison)
                throw new NullReferenceException(Errors.NoJoinLimits);

            builder.BeginJoin(this.JoinType);
            this.JoinedTo.BuildStatement(builder);
            builder.BeginJoinOnList();
            this.Comparison.BuildStatement(builder);
            builder.EndJoinOnList();
            builder.EndJoin(this.JoinType);
            return true;
        }

        #endregion

#endif

        //
        // Xml serialization
        //

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.JoinType, this.JoinType.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.JoinedTo != null)
            {
                this.WriteStartElement(XmlHelper.JoinTo, writer, context);
                this.JoinedTo.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.JoinTo, writer, context);
            }

            if (this.Comparison != null)
            {
                this.WriteStartElement(XmlHelper.JoinOn, writer, context);
                this.Comparison.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.JoinOn, writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.JoinType, reader, context))
            {
                string value = reader.Value;
                JoinType result;
                if (string.IsNullOrEmpty(value) == false && XmlHelper.TryParseEnum<JoinType>(value, out result))
                {
                    this.JoinType = result;
                    return true;
                }
            }
            return base.ReadAnAttribute(reader, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (IsElementMatch(XmlHelper.JoinTo, reader, context) && reader.IsEmptyElement == false
                && reader.Read())
            {
                this.JoinedTo = this.ReadNextInnerClause(XmlHelper.JoinTo, reader, context);
                b = true;
            }
            else if (IsElementMatch(XmlHelper.JoinOn, reader, context) && reader.IsEmptyElement == false && reader.Read())
            {
                this.Comparison = this.ReadNextInnerClause(XmlHelper.JoinOn, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

    }

    internal class DBJoinList : DBClauseList<DBJoin>
    {


#if SILVERLIGHT
        // no statement building
#else
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this.Count > 0)
            {
                
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].BuildStatement(builder);
                }

                return true;
            }
            else
                return false;
        }

#endif

    }
}
