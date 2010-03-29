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
    /// The DBSelectQuery encapsulates a select query  SQL Statement.
    /// </summary>
    public class DBSelectQuery : DBQuery
    {

        //
        // ivars
        //

        private DBSelectSet _select;
        private DBClause _top;
        private DBTableSet _root;
        private DBFilterSet _where;
        private DBFilterSet _having;
        private DBOrderSet _order;
        private DBGroupBySet _grpby;
        private DBClause _last;
        private bool _distinct = false;


        //
        // properties
        //

        #region internal DBSelectSet SelectSet {get;set;}

        /// <summary>
        /// Gets or Sets the DBSelectSet for this DBSelectQuery
        /// </summary>
        internal DBSelectSet SelectSet
        {
            get { return this._select; }
            set { this._select = value; }
        }

        #endregion 

        #region internal DBTableSet RootSet {get;set;}
        
        /// <summary>
        /// Gets or Sets the DBTableSet for this DBSelectQuery
        /// </summary>
        internal DBTableSet RootSet
        {
            get { return this._root; }
            set { this._root = value; }
        }

        #endregion

        #region internal DBFilterSet WhereSet {get;set;}

        /// <summary>
        /// Gets or Sets the DBFilterSet for this DBSelectQuery
        /// </summary>
        internal DBFilterSet WhereSet
        {
            get { return this._where; }
            set { this._where = value; }
        }

        #endregion

        #region internal DBOrderSet OrderSet {get;set;}

        /// <summary>
        /// Gets or sets the DBOrderBySet for this DBSelectQuery
        /// </summary>
        internal DBOrderSet OrderSet
        {
            get { return this._order; }
            set { this._order = value; }
        }

        #endregion

        #region internal DBGroupBySet GroupBySet {get;set;}

        /// <summary>
        /// Gets or Sets the DBGroupbySet for this DBSelectQuery
        /// </summary>
        internal DBGroupBySet GroupBySet
        {
            get { return this._grpby; }
            set { this._grpby = value; }
        }

        #endregion

        #region internal DBClause Last {get;set;}

        /// <summary>
        /// Gets or sets the last DBClause used in statement construction
        /// </summary>
        internal DBClause Last
        {
            get { return this._last; }
            set { this._last = value; }
        }

        #endregion

        #region internal DBClause Top {get; set;}

        /// <summary>
        /// Gets or Sets the DBClause for the Top statement
        /// </summary>
        internal DBClause Top 
        {
            get { return this._top; }
            set { this._top = value; }
        }

        #endregion

        #region internal bool IsDistinct {get;set;}

        /// <summary>
        /// Gets or sets the flag to identify if this is a distinct query
        /// </summary>
        internal bool IsDistinct
        {
            get { return this._distinct; }
            set { this._distinct = value; }
        }

        #endregion
         
        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Select;
            }
        }

        #endregion

        //
        // .ctors
        //

        #region internal DBSelectQuery()

        /// <summary>
        /// internal constructor - use the DBQuery.Select... methods to create a new instance
        /// </summary>
        internal DBSelectQuery()
        {
        }

        #endregion

        //
        // build statement methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Overrides the abstract base declaration to create a new Select query
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            this.ValidateStatement();

            builder.BeginSelectStatement();

            if (this.IsDistinct)
                builder.WriteDistinct();

            if (this._top != null)
                this._top.BuildStatement(builder);

            this._select.BuildStatement(builder);

            if (_root != null)
            {
                builder.BeginFromList();
                this._root.BuildStatement(builder);
                builder.EndFromList();
            }
            if (_where != null)
            {
                builder.BeginWhereStatement();
                this._where.BuildStatement(builder);
                builder.EndWhereStatement();
            }

            if (_grpby != null)
            {
                builder.BeginGroupByStatement();
                this._grpby.BuildStatement(builder);
                builder.EndGroupByStatement();
            }

            if (_having != null)
            {
                builder.BeginHavingStatement();
                this._having.BuildStatement(builder);
                builder.EndHavingStatement();
            }

            if (_order != null)
            {
                builder.BeginOrderStatement();
                this._order.BuildStatement(builder);
                builder.EndOrderStatement();
            }

            builder.EndSelectStatement();

            return true;
        }

        #endregion

        #region protected virtual void ValidateStatement()

        protected virtual void ValidateStatement()
        {
            if (this._select == null || this._select.Results.Count == 0)
                throw new Exception("No Fields were specified to be selected");
        }

        #endregion

        //
        // xml serialization methods
        //

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        /// <summary>
        /// Overrides the default implmentation to write the Distinct value attribute
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.IsDistinct)
                this.WriteAttribute(writer, XmlHelper.Distinct, bool.TrueString, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Overrides the base implementation to write the inner elements 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Top != null)
                this.Top.WriteXml(writer, context);

            if (this.SelectSet != null)
            {
                this.SelectSet.WriteXml(writer, context);
            }
            if (this.RootSet != null)
            {
                this.RootSet.WriteXml(writer, context);
            }
            if (this.WhereSet != null)
            {
                this.WhereSet.WriteXml(writer, context);
            }
            if (this.GroupBySet != null)
            {
                this.GroupBySet.WriteXml(writer, context);
            }
            if (this.OrderSet != null)
            {
                this.OrderSet.WriteXml(writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        //
        // xml de-seriaization methods
        //

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        /// <summary>
        /// Overrides the base implementation to read and assign the DBSelectQuery attribute(s)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool distinct;
            if (this.IsAttributeMatch(XmlHelper.Distinct, reader, context) && bool.TryParse(reader.Value, out distinct))
            {
                this.IsDistinct = distinct;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        /// <summary>
        /// Reads one of the inner elements from the xml being de-serialized 
        /// and if recognized - deserializes and assigns it to the correct property in this DBSelectQuery
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            switch (reader.LocalName)
            {
                case (XmlHelper.Top):
                    this.Top = context.Factory.Read(XmlHelper.Top, reader, context);
                    b = true;
                    break;
                case (XmlHelper.Fields):
                    this.SelectSet = context.Factory.Read(XmlHelper.Fields, reader, context) as DBSelectSet;
                    b = true;
                    break;
                case (XmlHelper.From):
                    this.RootSet = context.Factory.Read(XmlHelper.From, reader, context) as DBTableSet;
                    b = true;
                    break;
                case (XmlHelper.Where):
                    this.WhereSet = context.Factory.Read(XmlHelper.Where, reader, context) as DBFilterSet;
                    b = true;
                    break;
                case (XmlHelper.Group):
                    this.GroupBySet = context.Factory.Read(XmlHelper.Group, reader, context) as DBGroupBySet;
                    b = true;
                    break;
                case (XmlHelper.Order):
                    this.OrderSet = context.Factory.Read(XmlHelper.Order, reader, context) as DBOrderSet;
                    b = true;
                    break;
                default:
                    b = base.ReadAnInnerElement(reader, context);
                    break;
            }
            return b;
        }

        #endregion



        //
        // Statement Construction methods
        //




        //
        // Top, Distinct
        //

        #region public DBSelectQuery TopN(int count)

        public DBSelectQuery TopN(int count)
        {
            DBTop top = DBTop.Number(count);
            this.Top = top;
           
            return this;
        }

        #endregion

        #region public DBSelectQuery TopPercent(double count)

        public DBSelectQuery TopPercent(double count)
        {
            DBTop top = DBTop.Percent(count);
            this.Top = top;

            return this;
        }

        #endregion

        #region public DBSelectQuery Distinct()

        public DBSelectQuery Distinct()
        {
            this.IsDistinct = true;
            return this;
        }

        #endregion

        //
        // aggregate
        //

        #region public DBSelectQuery Sum(string field) + 3 overloads

        public DBSelectQuery Sum(string field)
        {
            DBField fld = DBField.Field(field);
            return Sum(fld);
        }

        public DBSelectQuery Sum(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Sum(fld);
        }

        public DBSelectQuery Sum(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Sum(fld);
        }

        public DBSelectQuery Sum(DBClause clause)
        {
            return Aggregate(AggregateFunction.Sum, clause);
        }

        #endregion

        #region public DBSelectQuery Avg(string field) + 3 overloads

        public DBSelectQuery Avg(string field)
        {
            DBField fld = DBField.Field(field);
            return Avg(fld);
        }

        public DBSelectQuery Avg(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Avg(fld);
        }

        public DBSelectQuery Avg(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Avg(fld);
        }

        public DBSelectQuery Avg(DBClause clause)
        {
            return Aggregate(AggregateFunction.Avg, clause);
        }

        #endregion

        #region public DBSelectQuery Min(string field) + 3 overloads

        public DBSelectQuery Min(string field)
        {
            DBField fld = DBField.Field(field);
            return Min(fld);
        }

        public DBSelectQuery Min(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Min(fld);
        }

        public DBSelectQuery Min(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Min(fld);
        }

        public DBSelectQuery Min(DBClause clause)
        {
            return Aggregate(AggregateFunction.Min, clause);
        }

        #endregion

        #region public DBSelectQuery Max(string field) + 3 overloads

        public DBSelectQuery Max(string field)
        {
            DBField fld = DBField.Field(field);
            return Max(fld);
        }

        public DBSelectQuery Max(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Max(fld);
        }

        public DBSelectQuery Max(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Max(fld);
        }

        public DBSelectQuery Max(DBClause clause)
        {
            return Aggregate(AggregateFunction.Max, clause);
        }

        #endregion


        #region public DBSelectQuery Count() + 4 overloads

        public DBSelectQuery Count()
        {
            return Count(DBField.AllFields());
        }

        public DBSelectQuery Count(string field)
        {
            DBField fld = DBField.Field(field);
            return Count(fld);
        }

        public DBSelectQuery Count(string table, string field)
        {
            DBField fld = DBField.Field(table,field);
            return Count(fld);
        }

        public DBSelectQuery Count(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Count(fld);
        }

        public DBSelectQuery Count(DBClause fref)
        {
            return Aggregate(AggregateFunction.Count, fref);
        }

        #endregion

        #region public DBSelectQuery Aggregate(AggregateFunction func, DBClause dbref)

        public DBSelectQuery Aggregate(AggregateFunction func, DBClause dbref)
        {
            if (_last == null)
            {
                if (_select == null)
                    _select = DBSelectSet.SelectAggregate(func, dbref);
                else
                    _select = _select.And(dbref);
                _last = _select;
            }
            else if (_last is IDBArregate)
                _last = ((IDBArregate)_last).Aggregate(func, dbref);
            else
                throw new InvalidOperationException("The current clause does not support aggregate operations");

            return this;
        }

        #endregion

        //
        // field
        //

        #region public DBSelectQuery Field(DBReference field) + 3 Overloads

        public DBSelectQuery Field(DBClause field)
        {
            if (this._last == null)
            {
                _select = DBSelectSet.Select(field);
                _last = _select;
            }
            else if (_last is IDBBoolean)
                _last = ((IDBBoolean)_last).And(field);
            return this;
        }

        public DBSelectQuery Field(string field)
        {
            DBField fld = DBField.Field(field);
            return this.Field(fld);
        }

        public DBSelectQuery Field(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return this.Field(fld);
        }

        public DBSelectQuery Field(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return this.Field(fld);
        }

        #endregion

        #region public DBSelectQuery Fields(params string[] fieldnames)

        public DBSelectQuery Fields(params string[] fieldnames)
        {
            DBSelectQuery last = this;
            foreach (string name in fieldnames)
            {
                DBField fld = DBField.Field(name);
                last = last.Field(fld);
            }
            return last;
        }

        #endregion

        //
        // From, Join, On
        //

        #region public DBQuery From(string table) + 2 overloads

        public DBSelectQuery From(string table)
        {
            DBTable tr = DBTable.Table(table);
            return From(tr);
        }

        public DBSelectQuery From(string owner, string table)
        {
            DBTable tr = DBTable.Table(owner, table);
            return From(tr);
        }

        public DBSelectQuery From(DBTable root)
        {
            DBTableSet ts = DBTableSet.From(root);
            this._root = ts;
            this._last = root;

            return this;
        }

        #endregion

        #region public DBQuery InnerJoin(string table, string parentfield, string childfield) + 4 overloads

        public DBSelectQuery InnerJoin(string table, string parentfield, string childfield)
        {
            this._last = this._root.InnerJoin(table, parentfield, Compare.Equals, childfield);
            return this;
        }

        public DBSelectQuery InnerJoin(string table, string parentfield, Compare op, string childfield)
        {
            this._last = this._root.InnerJoin(table, parentfield, op, childfield);
            return this;
        }

        public DBSelectQuery InnerJoin(DBTable table, DBComparison compare)
        {
            this._last = this._root.InnerJoin(table, compare);
            return this;
        }

        public DBSelectQuery InnerJoin(string table)
        {
            DBTable tbl = DBTable.Table(table);
            this._last = this._root.InnerJoin(tbl);
            return this;
        }

        public DBSelectQuery InnerJoin(string owner, string table)
        {
            DBTable tbl = DBTable.Table(owner, table);
            this._last = this._root.InnerJoin(tbl);
            return this;
        }

        public DBSelectQuery InnerJoin(DBSelectQuery inner)
        {
            if (inner == this)
                throw new ArgumentException("Circular reference");
            DBSubQuery subselect = DBSubQuery.SubSelect(inner);
            this._last = this._root.InnerJoin(subselect);
            return this;
        }

        #endregion

        #region public DBSelectQuery On(string parentfield, Compare comp, string childfield) + 2 overloads

        public DBSelectQuery On(string parentfield, Compare comp, string childfield)
        {
            DBField parent = DBField.Field(parentfield);
            DBField child = DBField.Field(childfield);

            this._last = this._root.On(parent, comp, child);
            return this;
        }

        public DBSelectQuery On(string parenttable, string parentfield, Compare comp, string childtable, string childfield)
        {
            DBField parent = DBField.Field(parenttable, parentfield);
            DBField child = DBField.Field(childtable, childfield);
            this._last = this._root.On(parent, comp, child);
            return this;
        }

        public DBSelectQuery On(DBClause parent, Compare comp, DBClause child)
        {
            this._last = this._root.On(parent, comp, child);
            return this;
        }

        #endregion

        //
        // As
        //

        #region public DBQuery As(string alias)

        /// <summary>
        /// Assigns the alias name to the last field or table in the select query. 
        /// It is an error to set the alias to the last item if it does not support alias names
        /// </summary>
        /// <param name="alias">The name to use as an alias</param>
        /// <returns>Itself</returns>
        public DBSelectQuery As(string alias)
        {
            if (this._last is IDBAlias)
                ((IDBAlias)this._last).As(alias);
            else if(null == this._last)
                throw new NullReferenceException("Cannot set an alias because there was no last item added in the query");
            else
                throw new NotSupportedException("Cannot alias a '" + this._last.GetType().ToString() + "' item in the query");

            return this;
        }

        #endregion

        //
        // Boolean Ops
        //

        #region public DBSelectQuery And(string field)

        public DBSelectQuery And(string field)
        {
            DBField fld = DBField.Field(field);
            return And(fld);
        }

        public DBSelectQuery And(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return And(fld);
        }

        public DBSelectQuery And(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return And(fld);
        }

        public DBSelectQuery And(DBClause clause)
        {
            if (_last == null)
                return this.Select(clause);
            else if (_last is IDBBoolean)
            {
                _last = ((IDBBoolean)_last).And(clause);
                return this;
            }
            else
                throw new ArgumentException("The last clause in the statement does not support 'and' operations");

        }

        #endregion

        #region public DBSelectQuery And(string parent, Compare comp, string child) + 3 overloads

        public DBSelectQuery And(string parent, Compare comp, string child)
        {
            DBField par = DBField.Field(parent);
            DBField chi = DBField.Field(child);
            return And(par, comp, chi);
        }

        public DBSelectQuery And(string parentTable, string parentField, Compare comp, string childTable, string childField)
        {
            DBField par = DBField.Field(parentTable,parentField);
            DBField chi = DBField.Field(childTable,childField);
            return And(par, comp, chi);
        }

        public DBSelectQuery And(string parentOwner, string parentTable, string parentField, Compare comp, string childOwner, string childTable, string childField)
        {
            DBField par = DBField.Field(parentOwner, parentTable, parentField);
            DBField chi = DBField.Field(childOwner, childTable, childField);
            return And(par, comp, chi);
        }

        public DBSelectQuery And(DBClause left, Compare comp, DBClause right)
        {
            DBComparison comarison = DBComparison.Compare(left, comp, right);
            return And(comarison);
        }

        #endregion

        //
        // ordering
        //

        #region public DBSelectQuery OrderBy(string field) + 6 overloads

        public DBSelectQuery OrderBy(string field)
        {
            return OrderBy(field, Order.Default);
        }

        public DBSelectQuery OrderBy(string table, string field)
        {
            return OrderBy(table, field, Order.Default);
        }

        public DBSelectQuery OrderBy(string owner, string table, string field)
        {
            return OrderBy(owner, table, field, Order.Default);
        }

        public DBSelectQuery OrderBy(string field, Order order)
        {
            DBClause clause = DBField.Field(field);
            return OrderBy(clause, order);
        }

        public DBSelectQuery OrderBy(string table, string field, Order order)
        {
            DBClause clause = DBField.Field(table, field);
            return OrderBy(clause, order);
        }
        public DBSelectQuery OrderBy(string owner, string table, string field, Order order)
        {
            DBClause clause = DBField.Field(owner, table, field);
            return OrderBy(clause, order);
        }

        public DBSelectQuery OrderBy(DBClause clause, Order order)
        {
            DBOrder oc = DBOrder.OrderBy(order, clause);
            if (this._order == null)
            {
                this._order = DBOrderSet.OrderBy(clause, order);
                this._last = this._order;
            }
            else
            {
                this._last = this._order.And(oc);
            }

            return this;
        }

        

        #endregion

        //
        // grouping
        //

        #region public static DBSelectQuery GroupBy(string field) + 3 overloads

        public DBSelectQuery GroupBy(string field)
        {
            DBField fld = DBField.Field(field);
            return GroupBy(fld);
        }

        public DBSelectQuery GroupBy(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return GroupBy(fld);
        }

        public DBSelectQuery GroupBy(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return GroupBy(fld);
        }

        public DBSelectQuery GroupBy(DBClause clause)
        {
            if (this._grpby == null)
                this._grpby = DBGroupBySet.GroupBy(clause);
            else
                this._grpby.And(clause);

            this._last = this._grpby;

            return this;
        }

        #endregion

        //
        // select
        //

        #region public DBSelectQuery Select(DBClause reference) + 3 overloads

        public DBSelectQuery Select(DBClause reference)
        {
            if (this._select == null)
                this._select = DBSelectSet.Select(reference);
            else
                this._select = (DBSelectSet)this._select.And(reference);
            _last = this._select;

            return this;
        }

        public DBSelectQuery Select(string field)
        {
            DBField fld = DBField.Field(field);
            return this.Select(fld);
        }

        public DBSelectQuery Select(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return this.Select(fld);
        }

        public DBSelectQuery Select(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return this.Select(fld);
        }

        #endregion

        //
        // where
        //

        #region public DBSelectQuery Where(DBReference left, ComparisonOperator compare, DBClause right) + 1 overload

        public DBSelectQuery Where(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBSelectQuery Where(string left, Compare compare, string right)
        {
            DBField fls = DBField.Field(left);
            DBField frs = DBField.Field(right);

            return Where(fls,compare,frs);
        }

        public DBSelectQuery Where(string field, Compare compare, DBClause clause)
        {
            DBField left = DBField.Field(field);

            return Where(left, compare, clause);
        }

        public DBSelectQuery Where(string lefttable, string leftfield, Compare compare, string righttable, string rightfield)
        {
            DBField fls = DBField.Field(lefttable, leftfield);
            DBField frs = DBField.Field(righttable, rightfield);

            return Where(fls, compare, frs);


        }

        public DBSelectQuery Where(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        #region public DBSelectQuery WhereFieldEquals(string field, DBClause value) + 1 overload

        public DBSelectQuery WhereFieldEquals(string field, DBClause value)
        {
            return WhereField(field, Compare.Equals, value);
        }

        public DBSelectQuery WhereFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return WhereField(fieldTable, fieldName, Compare.Equals, value);
        }

        #endregion

        #region public DBSelectQuery WhereField(string field, ComparisonOperator op, DBClause value) + 2 overload

        public DBSelectQuery WhereField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Where(fld, op, value);
        }

        public DBSelectQuery WhereField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Where(fld, op, value);
        }

        public DBSelectQuery WhereField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Where(fld, op, value);
        }

        #endregion

        #region public DBSelectQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBSelectQuery AndWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.And(left, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery AndWhere(string field, Compare op, DBClause right)
        {
            _where = _where.And(field, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery AndWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery AndWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery OrWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBSelectQuery OrWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.Or(left, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery OrWhere(string field, Compare op, DBClause right)
        {
            _where = _where.Or(field, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery OrWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBSelectQuery OrWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery WhereIn(string field, params object[] values) + 3 overloads

        public DBSelectQuery WhereIn(string field, params object[] values)
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
            DBComparison compare = DBComparison.In(fld,items.ToArray());
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBSelectQuery WhereIn(string table, string field, params DBClause[] values)
        {
            DBField fld = DBField.Field(table, field);
            DBComparison compare = DBComparison.In(fld, values);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBSelectQuery WhereIn(string field, DBSelectQuery select)
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

        #region public DBSelectQuery WhereExists(DBSelectQuery select)

        public DBSelectQuery WhereExists(DBSelectQuery select)
        {
            DBSubQuery subsel = DBSubQuery.SubSelect(select);
            DBComparison compare = DBComparison.Exists(subsel);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        //
        // Const()
        //

        #region public DBSelectQuery Const(int value) + 5 overloads

        public DBSelectQuery Const(int value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(bool value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(DateTime value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(string value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(double value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(Guid value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        public DBSelectQuery Const(System.Data.DbType type, object value)
        {
            DBConst con = DBConst.Const(type, value);
            return this.And(con);
        }

        #endregion

        //
        // having
        //

        #region public DBSelectQuery Having(DBReference left, ComparisonOperator compare, DBClause right) + 1 overload

        public DBSelectQuery Having(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._having = fs;
            this._last = fs;

            return this;
        }

        public DBSelectQuery Having(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._having = fs;
            this._last = fs;

            return this;
        }

        #endregion

        #region public DBSelectQuery HavingFieldEquals(string field, DBClause value) + 1 overload

        public DBSelectQuery HavingFieldEquals(string field, DBClause value)
        {
            return HavingField(field, Compare.Equals, value);
        }

        public DBSelectQuery HavingFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return HavingField(fieldTable, fieldName, Compare.Equals, value);
        }

        #endregion

        #region public DBSelectQuery HavingField(string field, ComparisonOperator op, DBClause value) + 2 overload

        public DBSelectQuery HavingField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Having(fld, op, value);
        }

        public DBSelectQuery HavingField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Having(fld, op, value);
        }

        public DBSelectQuery HavingField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Having(fld, op, value);
        }

        #endregion

        #region public DBSelectQuery AndHaving(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBSelectQuery AndHaving(DBClause left, Compare op, DBClause right)
        {
            _having = _having.And(left, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery AndHaving(string field, Compare op, DBClause right)
        {
            _having = _having.And(field, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery AndHaving(string table, string field, Compare op, DBClause right)
        {
            _having = _having.And(table, field, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery AndHaving(string owner, string table, string field, Compare op, DBClause right)
        {
            _having = _having.And(owner, table, field, op, right);
            _last = _having;

            return this;
        }

        #endregion

        #region public DBSelectQuery OrHaving(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBSelectQuery OrHaving(DBClause left, Compare op, DBClause right)
        {
            _having = _having.Or(left, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery OrHaving(string field, Compare op, DBClause right)
        {
            _having = _having.Or(field, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery OrHaving(string table, string field, Compare op, DBClause right)
        {
            _having = _having.Or(table, field, op, right);
            _last = _having;

            return this;
        }

        public DBSelectQuery OrHaving(string owner, string table, string field, Compare op, DBClause right)
        {
            _having = _having.Or(owner, table, field, op, right);
            _last = _having;

            return this;
        }

        #endregion
        
        //
        // calculation
        //

        #region IDBCalculable Implementation

        public DBSelectQuery Plus(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Add, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        public DBSelectQuery Minus(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Subtract, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        public DBSelectQuery Times(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Multiply, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        public DBSelectQuery Divide(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Divide, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        public DBSelectQuery Modulo(DBClause dbref)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Modulo, dbref);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

    }
}
