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
    /// Encapsulates a DELETE sql query statement
    /// </summary>
    public class DBDeleteQuery : DBQuery
    {
        private DBFilterSet _where;
        private DBTable _from;
        private DBClause _last;

        //
        // properties
        //

        #region internal DBFilterSet WhereSet {get;set;}

        /// <summary>
        /// Gets or sets the WHERE filter for the delete statement
        /// </summary>
        internal DBFilterSet WhereSet
        {
            get { return _where; }
            set { _where = value; }
        }

        #endregion

        #region internal DBTable FromTable {get;set;}

        /// <summary>
        /// Gets or sets the DBTable this delete statement refers to
        /// </summary>
        internal DBTable FromTable
        {
            get { return _from; }
            set { _from = value; }
        }

        #endregion

        #region internal DBClause Last {get;set;}

        /// <summary>
        /// Gets or sets the last clause that was modified - for chaining statements
        /// </summary>
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

        /// <summary>
        /// Generates the SQL statement using the provided builder for this DELETE query
        /// </summary>
        /// <param name="builder">The provider specific builder</param>
        /// <returns>true if the statement was built</returns>
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

        /// <summary>
        /// Gets the XmlElement name for this DBDeleteQuery
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Delete; }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        /// <summary>
        /// Overrides the default implementation to write the From statement and Where elements
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
        
        /// <summary>
        /// Overrides the base implementation to read the From and Where elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        
        /// <summary>
        /// Creates the first where clause in this DBDeleteQuery using the left and right clauses as operands in the comparison
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="compare">The comparison type - Equals etc...</param>
        /// <param name="right">The right operand</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery Where(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._where = fs;
            this._last = fs;

            return this;
        }

        /// <summary>
        /// Creates the first where clause in this DBDeleteQuery using the DBComparison as the filter.
        /// </summary>
        /// <param name="compare">The DBComparison to use</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery Where(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        #region public DBDeleteQuery WhereFieldEquals(string field, DBClause value) + 1 overload

        /// <summary>
        /// Creates the first where clause by using an equals comparison between the specified field and the clause
        /// </summary>
        /// <param name="field">The field to compare against</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery WhereFieldEquals(string field, DBClause value)
        {
            return WhereField(field, Compare.Equals, value);
        }

        /// <summary>
        /// Creates the first where clause by using an equals comparison between the specified table.field and the clause
        /// </summary>
        /// <param name="fieldTable">The name of the table the field belongs to</param>
        /// <param name="fieldName">The name of the field to comapre against</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery WhereFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return WhereField(fieldTable, fieldName, Compare.Equals, value);
        }

        #endregion

        #region public DBDeleteQuery WhereField(string field, ComparisonOperator op, DBClause value) + 2 overload

        /// <summary>
        /// Creates the first where clause by using the specified comparison between the specified field and the clause
        /// </summary>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery WhereField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Where(fld, op, value);
        }

        /// <summary>
        /// Creates the first where clause by using the specified comparison between the specified table.field and the clause
        /// </summary>
        /// <param name="fieldTable">The table the field belongs to</param>
        /// <param name="fieldName">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery WhereField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Where(fld, op, value);
        }

        /// <summary>
        /// Creates the first where clause by using the specified comparison between the specified owner.table.field and the clause
        /// </summary>
        /// <param name="fieldOwner">The schema owner of the table</param>
        /// <param name="fieldTable">The table the field belongs to</param>
        /// <param name="fieldName">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery WhereField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Where(fld, op, value);
        }

        #endregion

        #region public DBDeleteQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 4 overloads

        /// <summary>
        /// Creates a futher where clause in this DBDeleteQuery using the left and right 
        /// clauses as operands in the comparison. Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="op">The comparison type - Equals etc...</param>
        /// <param name="right">The right operand</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery AndWhere(DBClause left, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.And(left, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery using the specified comparison.
        /// Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="comparison">The comparison filter</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery AndWhere(DBComparison comparison)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.And(comparison);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBDeleteQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 4 overloads

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified field and the clause.
        /// Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="right">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery AndWhereField(string field, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.And(field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified table.field and the clause.
        /// Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="table">The table the field belongs to</param>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery AndWhereField(string table, string field, Compare op, DBClause value)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.And(table, field, op, value);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified owner.table.field and the clause.
        /// Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="owner">The schema owner of the table</param>
        /// <param name="table">The table the field belongs to</param>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="value">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery AndWhereField(string owner, string table, string field, Compare op, DBClause value)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.And(owner, table, field, op, value);
            _last = _where;

            return this;
        }

        

        #endregion

        #region public DBDeleteQuery OrWhere(DBClause left, ComparisonOperator op, DBClause right) + 1 overloads

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery using the specified comparison.
        /// Then combines with the previous filter in a boolean OR operation
        /// </summary>
        /// <param name="comparison">The comparison filter</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery OrWhere(DBClause comparison)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.Or(comparison);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a futher where clause in this DBDeleteQuery using the left and right 
        /// clauses as operands in the comparison. Then combines with the previous filter in a boolean AND operation
        /// </summary>
        /// <param name="left">The left operand</param>
        /// <param name="op">The comparison type - Equals etc...</param>
        /// <param name="right">The right operand</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery OrWhere(DBClause left, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.Or(left, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBDeleteQuery OrWhereField(string field, Compare op, DBClause right)

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified field and the clause.
        /// Then combines with the previous filter in a boolean OR operation
        /// </summary>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="right">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery OrWhereField(string field, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.Or(field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified table.field and the clause.
        /// Then combines with the previous filter in a boolean OR operation
        /// </summary>
        /// <param name="table">The table the field belongs to</param>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="right">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery OrWhereField(string table, string field, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.Or(table, field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Creates a further where clause in this DBDeleteQuery between the specified owner.table.field and the clause.
        /// Then combines with the previous filter in a boolean OR operation
        /// </summary>
        /// <param name="owner">The schema owner of the table</param>
        /// <param name="table">The table the field belongs to</param>
        /// <param name="field">The field to compare against</param>
        /// <param name="op">The comparison operator</param>
        /// <param name="right">The value clause</param>
        /// <returns>Itself so further statements can be combined</returns>
        public DBDeleteQuery OrWhereField(string owner, string table, string field, Compare op, DBClause right)
        {
            if (null == _where)
                throw new NullReferenceException(Errors.CannotAppendWhereClauseWithoutInitial);

            _where = _where.Or(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery WhereIn(string field, params object[] values) + 3 overloads

        /// <summary>
        /// Creates the first where clause in this DBDeleteQuery using the fields and a series of constant values
        /// </summary>
        /// <param name="field">The field which will be compared to the psecified values</param>
        /// <param name="values">The set of values to limit the deletion to</param>
        /// <returns>Itself so further statements can be chained</returns>
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

        /// <summary>
        /// Creates the first where clause in this DBDeleteQuery using the fields and a series of DBClauses that are evaluated at execution time
        /// </summary>
        /// <param name="fld">The field which will be compared to the specified values</param>
        /// <param name="values">The set of DBClauses to be evaluated and compared</param>
        /// <returns>Itself so further statements can be chained</returns>
        public DBDeleteQuery WhereIn(DBField fld, params DBClause[] values)
        {
            DBComparison compare = DBComparison.In(fld, values);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        /// <summary>
        /// Creates the first where clause in this DBDeleteQuery using the fields and a sub select that is evaluated at execution time
        /// </summary>
        /// <param name="field">The field which will be compared to the specified values</param>
        /// <param name="select">The sub select to be evaluated and compared</param>
        /// <returns>Itself so further statements can be chained</returns>
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
        // static factory
        //

        #region internal static DBDeleteQuery Delete()

        /// <summary>
        /// internal factory method for the DBDeleteQuery
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
