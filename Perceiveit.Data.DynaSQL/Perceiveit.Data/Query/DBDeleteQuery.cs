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
    public class DBDeleteQuery : DBQuery
    {
        private DBFilterSet _where;
        private DBTable _from;
        private DBClause _last;

        //
        // properties
        //

        #region internal DBFilterSet WhereSet {get;set;}

        internal DBFilterSet WhereSet
        {
            get { return _where; }
            set { _where = value; }
        }

        #endregion

        #region internal DBTable FromTable {get;set;}

        internal DBTable FromTable
        {
            get { return _from; }
            set { _from = value; }
        }

        #endregion

        #region internal DBClause Last {get;set;}

        internal DBClause Last
        {
            get { return this._last; }
            set { this._last = value; }
        }

        #endregion

        //
        // ctor(s)
        //

        internal DBDeleteQuery()
            : base()
        {
        }

        //
        // statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDeleteStatement();
            if(_from != null)
            {
                _from.BuildStatement(builder);
            }
            if (_where != null)
            {
                builder.BeginWhereStatement();
                _where.BuildStatement(builder);
                builder.EndWhereStatement();
            }
            builder.EndDeleteStatement();
            return true;

        }

        #endregion

        //
        // xml serialization
        //

        #region protected override string XmlElementName {get;}
        
        protected override string XmlElementName
        {
            get { return XmlHelper.Delete; }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        /// <summary>
        /// Overrides the default implementation to write the From statement and Where statement
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (_from != null)
            {
                this.WriteStartElement(XmlHelper.From, writer, context);
                _from.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.From, writer, context);
            }
            if (_where != null)
            {
                _where.WriteXml(writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;

            if (this.IsElementMatch(XmlHelper.From, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this._from = (DBTable)this.ReadNextInnerClause(XmlHelper.From, reader, context);
                reader.Read();
                b = true;
            }
            else if (this.IsElementMatch(XmlHelper.Where, reader, context))
            {
                this._where = (DBFilterSet)context.Factory.Read(XmlHelper.Where, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

        //
        // where methods
        //

        #region public DBDeleteQuery Where(DBClause clause, ComparisonOperator compare, DBClause right) + 1 overload

        

        public DBDeleteQuery Where(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBDeleteQuery Where(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        #region public DBDeleteQuery WhereFieldEquals(string field, DBClause value) + 1 overload

        public DBDeleteQuery WhereFieldEquals(string field, DBClause value)
        {
            return WhereField(field, Compare.Equals, value);
        }

        public DBDeleteQuery WhereFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return WhereField(fieldTable, fieldName, Compare.Equals, value);
        }

        #endregion

        #region public DBDeleteQuery WhereField(string field, ComparisonOperator op, DBClause value) + 2 overload

        public DBDeleteQuery WhereField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Where(fld, op, value);
        }

        public DBDeleteQuery WhereField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Where(fld, op, value);
        }

        public DBDeleteQuery WhereField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Where(fld, op, value);
        }

        #endregion

        #region public DBDeleteQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBDeleteQuery AndWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.And(left, op, right);
            _last = _where;

            return this;
        }

        public DBDeleteQuery AndWhere(string field, Compare op, DBClause right)
        {
            _where = _where.And(field, op, right);
            _last = _where;

            return this;
        }

        public DBDeleteQuery AndWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBDeleteQuery AndWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBDeleteQuery OrWhere(DBClause left, ComparisonOperator op, DBClause right) + 1 overloads

        public DBDeleteQuery OrWhere(DBClause comparison)
        {
            _where = _where.Or(comparison);
            _last = _where;

            return this;
        }

        public DBDeleteQuery OrWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.Or(left, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBDeleteQuery OrWhereField(string field, Compare op, DBClause right)
        
        public DBDeleteQuery OrWhereField(string field, Compare op, DBClause right)
        {
            _where = _where.Or(field, op, right);
            _last = _where;

            return this;
        }

        public DBDeleteQuery OrWhereField(string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBDeleteQuery OrWhereField(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery WhereIn(string field, params object[] values) + 3 overloads

        public DBDeleteQuery WhereIn(string field, params object[] values)
        {
            DBField fld = DBField.Field(field);
            List<DBClause> items = new List<DBClause>();
            if (values != null && values.Length > 0)
            {
                foreach (object val in values)
                {
                    items.Add(DBConst.Const(val));
                }
            }
            DBComparison compare = DBComparison.In(fld, items.ToArray());
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBDeleteQuery WhereIn(string table, string field, params DBClause[] values)
        {
            DBField fld = DBField.Field(table, field);
            DBComparison compare = DBComparison.In(fld, values);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }
        
        public DBDeleteQuery WhereIn(string field, DBSelectQuery select)
        {
            DBField fld = DBField.Field(field);
            DBSubQuery subsel = DBSubQuery.SubSelect(select);
            DBComparison compare = DBComparison.In(fld, subsel);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        //
        //IDBCalculable Implementation
        //

        #region public DBDeleteQuery Plus(DBClause dbref)

        public DBDeleteQuery Plus(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Add, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

        #region public DBDeleteQuery Minus(DBClause dbref)

        public DBDeleteQuery Minus(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Subtract, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }
        
        #endregion

        #region public DBDeleteQuery Times(DBClause dbref)

        public DBDeleteQuery Times(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Multiply, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

        #region public DBDeleteQuery Divide(DBClause dbref)

        public DBDeleteQuery Divide(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Divide, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

        #region public DBDeleteQuery Modulo(DBClause dbref)

        public DBDeleteQuery Modulo(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Modulo, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

        //
        // And Implementation
        //

        #region public DBDeleteQuery And(string field) + 3 overloads

        public DBDeleteQuery And(DBClause clause)
        {
            if (_last == null)
            {
                if (_where != null)
                    _last = _where;
            }
            if (_last is IDBBoolean)
            {
                _last = ((IDBBoolean)_last).And(clause);
                return this;
            }
            else
                throw new ArgumentException("The last clause in the statement does not support 'and' operations");

        }

        public DBDeleteQuery And(string parent, Compare comp, string child)
        {
            DBField par = DBField.Field(parent);
            DBField chi = DBField.Field(child);
            return And(par, comp, chi);
        }

        public DBDeleteQuery And(string parentTable, string parentField, Compare comp, string childTable, string childField)
        {
            DBField par = DBField.Field(parentTable, parentField);
            DBField chi = DBField.Field(childTable, childField);
            return And(par, comp, chi);
        }

        public DBDeleteQuery And(string parentOwner, string parentTable, string parentField, Compare comp, string childOwner, string childTable, string childField)
        {
            DBField par = DBField.Field(parentOwner, parentTable, parentField);
            DBField chi = DBField.Field(childOwner, childTable, childField);
            return And(par, comp, chi);
        }

        public DBDeleteQuery And(DBClause left, Compare comp, DBClause right)
        {
            DBComparison comarison = DBComparison.Compare(left, comp, right);
            return And(comarison);
        }



        #endregion


        //
        // static factory
        //

        #region internal static DBDeleteQuery Delete()

        /// <summary>
        /// internal factory method for the XmlFactory
        /// </summary>
        /// <returns></returns>
        internal static DBDeleteQuery Delete()
        {
            DBDeleteQuery q = new DBDeleteQuery();
            return q;
        }

        #endregion
    }
}
