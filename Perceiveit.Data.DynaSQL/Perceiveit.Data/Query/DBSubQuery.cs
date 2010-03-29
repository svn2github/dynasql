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
    public abstract class DBSubQuery : DBClause, IDBAlias, IDBJoinable, IDBBoolean
    {
        #region public string Alias

        private string _name;

        public string Alias
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public DBQuery InnerQuery

        private DBQuery _innerq;

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

        #region IDBAlias Members

        public void As(string aliasName)
        {
            this._name = aliasName;
        }

        #endregion

        #region public DBJoin InnerJoin(DBClause table) + 4 overloads

        public DBJoin InnerJoin(string table, string parentfield, string childfield)
        {
            DBTable tbl = DBTable.Table(table);
            DBField parent = DBField.Field(this.Alias, parentfield);
            DBField child = DBField.Field(table, parentfield);

            return InnerJoin(tbl, parent, child);
        }

        public DBJoin InnerJoin(DBTable table, DBClause parentField, DBClause childField)
        {
            DBJoin join = DBJoin.InnerJoin(table, parentField, childField, Compare.Equals);
            this.Joins.Add(join);
            return join;
        }

        public DBJoin InnerJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.InnerJoin(foreign, compare);
            this.Joins.Add(join);
            return join;
        }

        public DBJoin Join(DBClause table, JoinType type, DBComparison comp)
        {
            DBJoin join = DBJoin.Join(table, type, comp);
            this.Joins.Add(join);

            return join;
        }

        public DBJoin InnerJoin(DBClause table)
        {
            DBJoin join = DBJoin.Join(table, JoinType.InnerJoin);
            this.Joins.Add(join);

            return join;
        }

        #endregion



        #region IDBJoinable Members

        public DBClause On(DBComparison compare)
        {
            IDBJoinable join = (IDBJoinable)this.Joins[this.Joins.Count - 1];
            join.On(compare);
            return (DBClause)join;
        }

        #endregion

        #region IDBBoolean Members

        public DBClause And(DBClause reference)
        {
            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.And(reference);
            return (DBClause)join;
        }

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

        internal static DBSubQuery SubSelect(DBQuery inner)
        {
            DBSubQueryRef sub = new DBSubQueryRef();
            sub.InnerQuery = inner;
            inner.IsInnerQuery = true;
            return sub;
        }

        #endregion

        #region internal static DBSubQuery SubSelect()

        internal static DBSubQuery SubSelect()
        {
            DBSubQueryRef subref = new DBSubQueryRef();
            return subref;
        }

        #endregion
    }

    public class DBSubQueryRef : DBSubQuery
    {

        //
        // SQL Statement
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            InnerQuery.BuildStatement(builder);
            
            if (string.IsNullOrEmpty(this.Alias) == false)
            {
                builder.BeginAlias();
                builder.BeginIdentifier();
                builder.WriteRaw(this.Alias);
                builder.EndIdentifier();
                builder.EndAlias();
            }
            return true;
        }

        #endregion


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
