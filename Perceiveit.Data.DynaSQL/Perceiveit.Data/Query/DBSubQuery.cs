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
    /// Defines a query included within an outer query. 
    /// For example an inner select on a SELECT statement, or a SELECT for an insert
    /// </summary>
    public abstract class DBSubQuery : DBClause, IDBAlias, IDBJoinable, IDBBoolean
    {
        #region public string Alias

        private string _name;

        /// <summary>
        /// Gets or sets the alias name of this subquery
        /// </summary>
        public string Alias
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public DBQuery InnerQuery

        private DBQuery _innerq;

        /// <summary>
        /// Gets or sets the inner query in this sub query
        /// </summary>
        public DBQuery InnerQuery
        {
            get { return _innerq; }
            set
            {
                _innerq = value;
                if (null != _innerq)
                    _innerq.IsInnerQuery = true;
            }
        }

        #endregion

        #region public DBJoinList Joins {get;}

        private DBJoinList _joins;
        /// <summary>
        /// Gets or sets the list of joins between the outer query and this inner query (if any). 
        /// </summary>
        internal DBJoinList Joins
        {
            get
            {
                if (this._joins == null)
                    this._joins = new DBJoinList();
                return _joins;
            }
        }

        #endregion

        #region public bool HasJoins

        /// <summary>
        /// Returns true if there are joins defined between the inner and outer queries
        /// </summary>
        public bool HasJoins
        {
            get { return this._joins != null && this._joins.Count > 0; }
        }

        #endregion

        #region IDBAlias Members

        /// <summary>
        /// Applies an alias name to this inner query
        /// </summary>
        /// <param name="aliasName"></param>
        public void As(string aliasName)
        {
            this._name = aliasName;
        }

        #endregion

        #region public DBJoin InnerJoin(DBClause table) + 4 overloads

        /// <summary>
        /// Adds an INNER JOIN from the results of this sub query to a second table 
        /// where this tables parent field matched the child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentfield"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(string table, string parentfield, string childfield)
        {
            DBTable tbl = DBTable.Table(table);
            DBField parent = DBField.Field(this.Alias, parentfield);
            DBField child = DBField.Field(table, parentfield);

            return InnerJoin(tbl, parent, child);
        }

        /// <summary>
        /// Adds an INNER JOIN from the results of this sub query to a second table 
        /// where this tables parent field matched the child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentField"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBTable table, DBClause parentField, DBClause childField)
        {
            DBJoin join = DBJoin.InnerJoin(table, parentField, childField, Compare.Equals);
            this.Joins.Add(join);
            return join;
        }

        /// <summary>
        /// Adds an INNER JOIN from the results of this sub query to a second foreign clause 
        /// with the comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.InnerJoin(foreign, compare);
            this.Joins.Add(join);
            return join;
        }

        #endregion

        //
        // other joins
        //

        /// <summary>
        /// Adds a LEFT OUTER JOIN from the results of this sub query to a second foreign clause 
        /// with the comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin LeftJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.Join(foreign, JoinType.LeftOuter, compare);
            this.Joins.Add(join);
            return join;
        }

        /// <summary>
        /// Adds a RIGHT OUTER JOIN from the results of this sub query to a second foreign clause 
        /// with the comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin RightJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.Join(foreign, JoinType.RightOuter, compare);
            this.Joins.Add(join);
            return join;
        }

        /// <summary>
        /// Adds a JOIN from the results of this sub query to a second foreign clause 
        /// with the comparison
        /// </summary>
        /// <param name="table"></param>
        /// <param name="type"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public DBJoin Join(DBClause table, JoinType type, DBComparison comp)
        {
            DBJoin join = DBJoin.Join(table, type, comp);
            this.Joins.Add(join);

            return join;
        }

        /// <summary>
        /// Adds an INNER JOIN from the results of this sub query to a second foreign clause. 
        /// Use the ON method to add restrictions
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBClause table)
        {
            DBJoin join = DBJoin.Join(table, JoinType.InnerJoin);
            this.Joins.Add(join);

            return join;
        }

        



        #region IDBJoinable Members

        /// <summary>
        /// Appends restrictions to the last joined clauses and this sub query
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBClause On(DBComparison compare)
        {
            IDBJoinable join = (IDBJoinable)this.Joins[this.Joins.Count - 1];
            join.On(compare);
            return (DBClause)join;
        }

        #endregion

        #region IDBBoolean Members

        /// <summary>
        /// Appends an AND restriction to this Sub selects last JOIN statements
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause And(DBClause reference)
        {
            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.And(reference);
            return (DBClause)join;
        }

        /// <summary>
        /// Appends an OR restriction to this sub selectes last JOIN statements
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause Or(DBClause reference)
        {
            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.Or(reference);
            return (DBClause)join;
        }

        #endregion

        //
        // static factory methods
        //

        #region internal static DBSubQuery SubSelect(DBQuery inner)

        /// <summary>
        /// Defines a new SubSelect with an inner query
        /// </summary>
        /// <param name="inner"></param>
        /// <returns></returns>
        internal static DBSubQuery SubSelect(DBQuery inner)
        {
            DBSubQueryRef sub = new DBSubQueryRef();
            sub.InnerQuery = inner;
            inner.IsInnerQuery = true;
            return sub;
        }

        #endregion

        #region internal static DBSubQuery SubSelect()

        /// <summary>
        /// Defines a new empty sub select
        /// </summary>
        /// <returns></returns>
        internal static DBSubQuery SubSelect()
        {
            DBSubQueryRef subref = new DBSubQueryRef();
            return subref;
        }

        #endregion
    }

    internal class DBSubQueryRef : DBSubQuery
    {

#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            InnerQuery.BuildStatement(builder);
            
            if (string.IsNullOrEmpty(this.Alias) == false)
            {
                builder.WriteSubQueryAlias(this.Alias);
            }

            if (this.HasJoins)
                this.Joins.BuildStatement(builder);

            return true;
        }

        #endregion

#endif
        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.InnerSelect; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAlias(writer, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.InnerQuery != null)
                this.InnerQuery.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;

            if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
                this.Alias = reader.Value;
            else
                b = base.ReadAnAttribute(reader, context);
            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBClause c = context.Factory.Read(reader.Name, reader, context);

            if (c is DBQuery)
            {
                this.InnerQuery = (DBQuery)c;
                return true;
            }
            else
            {
                throw new InvalidCastException(Errors.CanOnlyUseQueriesInInnerSelects);
            }
        }

        #endregion


    }
}
