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
        private DBQueryHintOptionSet _options;


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

        #region internal DBQueryHintOptionSet QueryOptions {get;set;}

        /// <summary>
        /// Gets or sets the Query hints for this select statement
        /// </summary>
        internal DBQueryHintOptionSet QueryOptions
        {
            get
            {
                return _options;
            }
            set { _options = value; }
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

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
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

            builder.BeginSelectList();
            this._select.BuildStatement(builder);
            builder.EndSelectList();

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

            if (_options != null)
            {
                builder.BeginQueryHints();
                this._options.BuildStatement(builder);
                builder.EndQueryHints();
            }

            builder.EndSelectStatement();

            return true;
        }

        #endregion

        #region protected virtual void ValidateStatement()
        /// <summary>
        /// validates the statement as it is - minimum requirements are at least one return column
        /// </summary>
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

            if (this._options != null)
                this.QueryOptions.WriteXml(writer, context);


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
                case(XmlHelper.QueryOptionSet):
                    this.QueryOptions = context.Factory.Read(XmlHelper.QueryOptionSet, reader, context) as DBQueryHintOptionSet;
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
        /// <summary>
        /// Restricts the return results to the specified count
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public DBSelectQuery TopN(int count)
        {
            DBTop top = DBTop.Number(count);
            this.Top = top;
           
            return this;
        }

        #endregion

        #region public DBSelectQuery TopPercent(double count)
        /// <summary>
        /// Restricts the return results to the specified percentage of the total results
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public DBSelectQuery TopPercent(double count)
        {
            DBTop top = DBTop.Percent(count);
            this.Top = top;

            return this;
        }

        #endregion

        #region public DBSelectQuery TopRange(int index, int count)

        /// <summary>
        /// Adds a Range limits to the select query
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DBSelectQuery TopRange(int index, int count)
        {
            DBTop top = DBTop.Range(index, count);
            this.Top = top;

            return this;
        }

        #endregion

        #region public DBSelectQuery Distinct()
        /// <summary>
        /// Restricts the return results to only unique results
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Adds an aggregate SUM([field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Sum(string field)
        {
            DBField fld = DBField.Field(field);
            return Sum(fld);
        }

        /// <summary>
        /// Adds an aggregate SUM([table].[field]) to the select columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Sum(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Sum(fld);
        }

        /// <summary>
        /// Adds an aggregate SUM([table].[field]) to the select columns
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Sum(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Sum(fld);
        }

        /// <summary>
        /// Adds an aggregate SUM(clause) to the select columns
        /// </summary>
        /// <param name="clause">Any individual or combination of sql clauses</param>
        /// <returns></returns>
        public DBSelectQuery Sum(DBClause clause)
        {
            return Aggregate(AggregateFunction.Sum, clause);
        }

        #endregion

        #region public DBSelectQuery Avg(string field) + 3 overloads

        /// <summary>
        /// Adds an aggregate AVG([field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Avg(string field)
        {
            DBField fld = DBField.Field(field);
            return Avg(fld);
        }

        /// <summary>
        /// Adds an aggregate AVG([table].[field]) to the select columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Avg(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Avg(fld);
        }

        /// <summary>
        /// Adds an aggregate AVG([owner].[table].[field]) to the select columns
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Avg(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Avg(fld);
        }

        /// <summary>
        /// Adds an aggregate AVG([clause]) to the select columns
        /// </summary>
        /// <param name="clause">Any individual or combination of clauses</param>
        /// <returns></returns>
        public DBSelectQuery Avg(DBClause clause)
        {
            return Aggregate(AggregateFunction.Avg, clause);
        }

        #endregion

        #region public DBSelectQuery Min(string field) + 3 overloads
        /// <summary>
        ///  Adds an aggregate MIN([field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Min(string field)
        {
            DBField fld = DBField.Field(field);
            return Min(fld);
        }

        /// <summary>
        ///  Adds an aggregate MIN([table].[field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery Min(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Min(fld);
        }

        /// <summary>
        ///  Adds an aggregate MIN([owner].[table].[field]) to the select columns
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Min(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Min(fld);
        }

        /// <summary>
        /// Adds an aggregate MIN([clause]) to the select columns
        /// </summary>
        /// <param name="clause">Any individual or combination of clauses</param>
        /// <returns></returns>
        public DBSelectQuery Min(DBClause clause)
        {
            return Aggregate(AggregateFunction.Min, clause);
        }

        #endregion

        #region public DBSelectQuery Max(string field) + 3 overloads

        /// <summary>
        /// Adds an aggregate MAX([field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Max(string field)
        {
            DBField fld = DBField.Field(field);
            return Max(fld);
        }

        /// <summary>
        /// Adds an aggregate MAX([table].[field]) to the select columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Max(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return Max(fld);
        }

        /// <summary>
        /// Adds an aggregate MAX([owner].[table].[field]) to the select columns
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Max(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Max(fld);
        }

        /// <summary>
        /// Adds an aggregate MAX([clause]) to the select columns
        /// </summary>
        /// <param name="clause">Any individual or combination of clauses</param>
        /// <returns></returns>
        public DBSelectQuery Max(DBClause clause)
        {
            return Aggregate(AggregateFunction.Max, clause);
        }

        #endregion


        #region public DBSelectQuery Count() + 4 overloads

        /// <summary>
        /// Adds an aggregate COUNT(*) to the select columns
        /// </summary>
        /// <returns></returns>
        public DBSelectQuery Count()
        {
            return Count(DBField.AllFields());
        }

        /// <summary>
        /// Adds an aggregate COUNT([field]) to the select columns
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Count(string field)
        {
            DBField fld = DBField.Field(field);
            return Count(fld);
        }

        /// <summary>
        /// Adds an aggregate COUNT([table].[field]) to the select columns
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Count(string table, string field)
        {
            DBField fld = DBField.Field(table,field);
            return Count(fld);
        }

        /// <summary>
        /// Adds an aggregate COUNT([owner].[table].[field]) to the select columns
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Count(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return Count(fld);
        }

        /// <summary>
        /// Adds an aggregate COUNT([clause]) to the select columns
        /// </summary>
        /// <param name="clause">Any individual or combination of clauses</param>
        /// <returns></returns>
        public DBSelectQuery Count(DBClause clause)
        {
            return Aggregate(AggregateFunction.Count, clause);
        }

        #endregion

        #region public DBSelectQuery Aggregate(AggregateFunction func, DBClause dbref)

        /// <summary>
        /// Adds an aggregate function to the select list (or the last clause).
        /// </summary>
        /// <param name="func">The aggregate function</param>
        /// <param name="dbref">The clause to aggregate</param>
        /// <returns>Itself to support chaining of statements</returns>
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
            else if (_last is IDBAggregate)
                _last = ((IDBAggregate)_last).Aggregate(func, dbref);
            else
                throw new InvalidOperationException("The current clause does not support aggregate operations");

            return this;
        }

        #endregion

        //
        // field
        //

        #region public DBSelectQuery Field(DBReference field) + 3 Overloads

        /// <summary>
        /// Adds a clause to the select list (or the last clause in the statement)
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Field(DBClause clause)
        {
            if (this._last == null)
            {
                _select = DBSelectSet.Select(clause);
                _last = _select;
            }
            else if (_last is IDBBoolean)
                _last = ((IDBBoolean)_last).And(clause);
            return this;
        }

        /// <summary>
        /// Adds a [field] clause to the select list, or the last clause in the statement
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Field(string field)
        {
            DBField fld = DBField.Field(field);
            return this.Field(fld);
        }

        /// <summary>
        /// Adds a [table].[field] clause to the select list, or the last clause in the statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Field(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return this.Field(fld);
        }

        /// <summary>
        /// Adds a [owner].[table].[field] clause to the select list, or the last clause in the statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Field(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return this.Field(fld);
        }

        #endregion

        #region public DBSelectQuery Fields(params string[] fieldnames)

        /// <summary>
        /// Adds a set of fields to the select list, or the last clause in the statement
        /// </summary>
        /// <param name="fieldnames">The names of the fields to the list</param>
        /// <returns></returns>
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

        /// <summary>
        /// Specifies the root table or view to select data from in this statement
        /// </summary>
        /// <param name="table">The name of the table</param>
        /// <returns></returns>
        public DBSelectQuery From(string table)
        {
            DBTable tr = DBTable.Table(table);
            return From(tr);
        }

        /// <summary>
        /// Specifies the root owner.table or owner.view to select data from in this statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery From(string owner, string table)
        {
            DBTable tr = DBTable.Table(owner, table);
            return From(tr);
        }

        /// <summary>
        /// Specifies the root table or view to select data from in this statement
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public DBSelectQuery From(DBTable root)
        {
            DBTableSet ts = DBTableSet.From(root);
            this._root = ts;
            this._last = root;

            return this;
        }

        /// <summary>
        /// Specifies the root sub select statement or view to select data from in this statement
        /// </summary>
        /// <param name="subselect"></param>
        /// <returns></returns>
        public DBSelectQuery From(DBSelectQuery subselect)
        {
            DBSubQuery sub = DBSubQuery.SubSelect(subselect);
            DBTableSet ts = DBTableSet.From(sub);
            this._root = ts;
            this._last = sub;

            return this;
        }

        #endregion


        #region public DBQuery InnerJoin(string table, string parentfield, string childfield) + 4 overloads
        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] ON [parentfield] = [childfield]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentfield"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(string table, string parentfield, string childfield)
        {
            return this.InnerJoin(table, parentfield, Compare.Equals, childfield);
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] ON [parentfield] [op] [childfield]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentfield"></param>
        /// <param name="op"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(string table, string parentfield, Compare op, string childfield)
        {
            DBTable tbl = DBTable.Table(table);
            DBField par = DBField.Field(parentfield);
            DBField child = DBField.Field(childfield);

            return this.InnerJoin(tbl, par, op, child);
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] ON [parentfield] [op] [childfield]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentfield"></param>
        /// <param name="op"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(DBTable table, DBField parentfield, Compare op, DBField childfield)
        {
            this._last = this._root.InnerJoin(table, parentfield, op, childfield);
            return this;
        }

        /// <summary>
        ///  Specifies an INNER JOIN on the specified [table] ON [left] [op] [right]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(string table, DBClause left, Compare op, DBClause right)
        {
            DBTable tbl = DBTable.Table(table);
            this._last = this._root.InnerJoin(tbl, left, op, right);
            return this;
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] ON [left] [op] [right]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(DBTable table, DBClause left, Compare op, DBClause right)
        {
            this._last = this._root.InnerJoin(table, left, op, right);
            return this;
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] ON [comparison]
        /// </summary>
        /// <param name="table"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(DBTable table, DBComparison compare)
        {
            this._last = this._root.InnerJoin(table, compare);
            return this;
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] - follow with an .On(...) statement
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(string table)
        {
            DBTable tbl = DBTable.Table(table);
            return this.InnerJoin(tbl);
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [owner].[table] - follow with an .On(...) statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(string owner, string table)
        {
            DBTable tbl = DBTable.Table(owner, table);
            return this.InnerJoin(tbl) ;
        }

        /// <summary>
        /// Specifies an INNER JOIN on the specified [table] - follow with an .On(...) statement
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(DBTable table)
        {
            this._last = this._root.InnerJoin(table);
            return this;
        }


        /// <summary>
        /// Specifies an INNER JOIN on the specified sub select statement - follow with an .On(...) statement
        /// </summary>
        /// <param name="inner">An inner select query</param>
        /// <returns></returns>
        public DBSelectQuery InnerJoin(DBSelectQuery inner)
        {
            if (inner == this)
                throw new ArgumentException("Circular reference");

            DBSubQuery subselect = DBSubQuery.SubSelect(inner);
            this._last = this._root.InnerJoin(subselect);
            return this;
        }

        #endregion

        #region public DBSelectQuery LeftJoin(string table) + 3 overloads

        /// <summary>
        /// Performs a Left Join from the previous table onto the table with the specified name
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery LeftJoin(string table)
        {
            DBTable tbl = DBTable.Table(table);
            return LeftJoin(tbl);
        }

        /// <summary>
        /// Performs a Left Join from the previous table onto the table with the specified name and schema/owner.
        /// </summary>
        /// <param name="owner">The name of the table schema/owner</param>
        /// <param name="table">The name of the table</param>
        /// <returns></returns>
        public DBSelectQuery LeftJoin(string owner, string table)
        {
            DBTable tbl = DBTable.Table(owner, table);
            return LeftJoin(tbl);
        }

        /// <summary>
        /// Performs a left join from the previous table onto the specified table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DBSelectQuery LeftJoin(DBTable table)
        {
            this._last = this._root.LeftJoin(table);
            return this;
        }

        /// <summary>
        /// Performs a Left Join from the previous table to the specified Select sub query
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public DBSelectQuery LeftJoin(DBSelectQuery select)
        {
            if (select == this)
                throw new ArgumentException("Circular Reference");

            DBSubQuery subselect = DBSubQuery.SubSelect(select);
            this._last = this._root.LeftJoin(subselect);
            return this;
        }

        #endregion

        #region public DBSelectQuery RightJoin(string table) + 3 overloads

        /// <summary>
        /// Performs a Right Join from the previous table to the table with the specified name.
        /// </summary>
        /// <param name="table">The name of the table to join to.</param>
        /// <returns></returns>
        public DBSelectQuery RightJoin(string table)
        {
            DBTable tble = DBTable.Table(table);
            return RightJoin(tble);
        }

        /// <summary>
        /// Performs a Right Join from the previous table to the table with the specified name and owner.
        /// </summary>
        /// <param name="owner">The table schema/owner</param>
        /// <param name="table">The name of the table to join to</param>
        /// <returns></returns>
        public DBSelectQuery RightJoin(string owner, string table)
        {
            DBTable tbl = DBTable.Table(table);
            return RightJoin(tbl);
        }

        /// <summary>
        /// Preforms a Right Join from the previous table to the specified table
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DBSelectQuery RightJoin(DBTable tbl)
        {
            this._last = this._root.RightJoin(tbl);
            return this;
        }

        /// <summary>
        /// Performs a Right Join from the previous table to the specified Select sub query
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        public DBSelectQuery RightJoin(DBSelectQuery select)
        {
            DBSubQuery subselect = DBSubQuery.SubSelect(select);
            this._last = this._root.RightJoin(subselect);
            return this;
        }

        #endregion

        #region public DBSelectQuery On(string parentfield, Compare comp, string childfield) + 2 overloads
        /// <summary>
        /// Specifies an ON restriction on the previously joined tables (views, or sub selects)
        /// </summary>
        /// <param name="parentfield"></param>
        /// <param name="comp"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBSelectQuery On(string parentfield, Compare comp, string childfield)
        {
            DBField parent = DBField.Field(parentfield);
            DBField child = DBField.Field(childfield);

            this._last = this._root.On(parent, comp, child);
            return this;
        }

        /// <summary>
        /// Specifies an ON restriction on the specified joined tables (views, or sub selects)
        /// </summary>
        /// <param name="parenttable"></param>
        /// <param name="parentfield"></param>
        /// <param name="comp"></param>
        /// <param name="childtable"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBSelectQuery On(string parenttable, string parentfield, Compare comp, string childtable, string childfield)
        {
            DBField parent = DBField.Field(parenttable, parentfield);
            DBField child = DBField.Field(childtable, childfield);
            this._last = this._root.On(parent, comp, child);
            return this;
        }

        /// <summary>
        /// Specifies an ON restriction on the previously joined tables (views, or sub selects)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="comp"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public DBSelectQuery On(DBClause parent, Compare comp, DBClause child)
        {
            this._last = this._root.On(parent, comp, child);
            return this;
        }

        #endregion

        //
        // hints (table and query)
        //

        #region public DBSelectQuery WithHint(DBTableHint hint) + 1 overload

        /// <summary>
        /// Adds a specific query hint to the current table in this statement
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public DBSelectQuery WithHint(DBTableHint hint)
        {
            if (this._last is DBTableSet)
                (this._last as DBTableSet).WithHint(hint);
            else if (this._root != null)
                this._last = this._root.WithHint(hint);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }

        /// <summary>
        /// Adds a specific query hint with options to the current table in this statement
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public DBSelectQuery WithHint(DBTableHint hint, params string[] options)
        {
            if (this._last is DBTableSet)
                this._last = (this._last as DBTableSet).WithHint(hint, options);
            else if (this._root != null)
                this._last = this._root.WithHint(hint, options);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }

        #endregion

        #region public DBSelectQuery WithHints(params DBTableHint[] hints)

        /// <summary>
        /// Adds the list of hints to the current table in the query
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public DBSelectQuery WithHints(params DBTableHint[] hints)
        {
            if (this._last is DBTableSet)
                this._last = (this._last as DBTableSet).WithHints(hints);
            else if (this._root != null)
                this._last = this._root.WithHints(hints);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }
        #endregion

        #region public DBSelectQuery ClearTableHints()

        /// <summary>
        /// Clears any table hints on the current table
        /// </summary>
        /// <returns></returns>
        public DBSelectQuery ClearTableHints()
        {
            if (this._last is DBTableSet)
                this._last = (this._last as DBTableSet).ClearTableHints();
            else if (this._root != null)
                this._last = this._root.ClearTableHints();
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }

        #endregion


        #region public DBSelectQuery WithQueryOption(DBQueryOption option) + 3 overlaods

        /// <summary>
        /// Adds a query option to this Select Statement
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public DBSelectQuery WithQueryOption(DBQueryOption option)
        {
            DBQueryHintOption hint = DBQueryHintOption.QueryOption(option);
            return this.WithQueryOption(hint);
        }

        /// <summary>
        /// Adds a query option to this Select Statement with the parameter clause required for the option. 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DBSelectQuery WithQueryOption(DBQueryOption option, DBClause param)
        {
            DBQueryHintOption hint = DBQueryHintOption.QueryOption(option, param);
            return this.WithQueryOption(hint);

        }

        /// <summary>
        /// Adds a query option to this Select Statement and it's associated integer parameter - eg FAST 10
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WithQueryOption(DBQueryOption option, int value)
        {
            return WithQueryOption(option, DBConst.Int32(value));
        }

        /// <summary>
        /// Adds all the specified options to the Select Query
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DBSelectQuery WithQueryOptions(params DBQueryOption[] options)
        {
            if (null != options && options.Length > 0)
            {
                DBSelectQuery toreturn = this;
                foreach (DBQueryOption option in options)
                {
                    toreturn = toreturn.WithQueryOption(option);
                }
                return toreturn;
            }
            else
                return this;
        }

        public DBSelectQuery WithQueryOption(DBQueryHintOption hintoption)
        {
            if (null == this.QueryOptions)
                this.QueryOptions = new DBQueryHintOptionSet();

            this.QueryOptions.Add(hintoption);
            return this;
        }

        #endregion

        #region ClearQueryOption()

        /// <summary>
        /// Clears any assigned query options
        /// </summary>
        /// <returns></returns>
        public DBSelectQuery ClearQueryOption()
        {
            if (null != this.QueryOptions)
                this.QueryOptions = null;

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

        /// <summary>
        /// Appends a field to this statement
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery And(string field)
        {
            DBField fld = DBField.Field(field);
            return And(fld);
        }

        /// <summary>
        /// Appends the [table].[field] to this statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery And(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return And(fld);
        }

        /// <summary>
        /// Appends the [owner].[table].[field] to this statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery And(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return And(fld);
        }

        /// <summary>
        /// Appends the clause to this statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Appends a field to field comparison to the last statement in this select query
        /// </summary>
        /// <param name="left"></param>
        /// <param name="comp"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery And(string left, Compare comp, string right)
        {
            DBField par = DBField.Field(left);
            DBField chi = DBField.Field(right);
            return And(par, comp, chi);
        }

        /// <summary>
        /// Appends a field to field comparison to the last statement in this select query
        /// </summary>
        /// <param name="parentTable"></param>
        /// <param name="parentField"></param>
        /// <param name="comp"></param>
        /// <param name="childTable"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBSelectQuery And(string parentTable, string parentField, Compare comp, string childTable, string childField)
        {
            DBField par = DBField.Field(parentTable,parentField);
            DBField chi = DBField.Field(childTable,childField);
            return And(par, comp, chi);
        }

        /// <summary>
        /// Appends a field to field comparison to the last statement in this select query
        /// </summary>
        /// <param name="parentOwner"></param>
        /// <param name="parentTable"></param>
        /// <param name="parentField"></param>
        /// <param name="comp"></param>
        /// <param name="childOwner"></param>
        /// <param name="childTable"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBSelectQuery And(string parentOwner, string parentTable, string parentField, Compare comp, string childOwner, string childTable, string childField)
        {
            DBField par = DBField.Field(parentOwner, parentTable, parentField);
            DBField chi = DBField.Field(childOwner, childTable, childField);
            return And(par, comp, chi);
        }

        /// <summary>
        /// Appends a clause to clause comparison to the last statement in this select query
        /// </summary>
        /// <param name="left"></param>
        /// <param name="comp"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Appends an ORDER BY [field] to this statement
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string field)
        {
            return OrderBy(field, Order.Default);
        }

        /// <summary>
        /// Appends an ORDER BY [table][field] to this statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string table, string field)
        {
            return OrderBy(table, field, Order.Default);
        }

        /// <summary>
        /// Appends an ORDER BY [owner].[table].[field] to this statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string owner, string table, string field)
        {
            return OrderBy(owner, table, field, Order.Default);
        }

        /// <summary>
        /// Appends an ORDER BY [field] ASC|DESC to this statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string field, Order order)
        {
            DBClause clause = DBField.Field(field);
            return OrderBy(clause, order);
        }

        /// <summary>
        /// Appends an ORDER BY [table].[field] ASC|DESC to this statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string table, string field, Order order)
        {
            DBClause clause = DBField.Field(table, field);
            return OrderBy(clause, order);
        }

        /// <summary>
        /// Appends an ORDER BY [owner].[table].[field] ASC|DESC to this statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DBSelectQuery OrderBy(string owner, string table, string field, Order order)
        {
            DBClause clause = DBField.Field(owner, table, field);
            return OrderBy(clause, order);
        }

        /// <summary>
        /// Appends an ORDER BY [any clause] ASC|DESC to this statement
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="order"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Appends a GROUP BY [field] to this statement
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery GroupBy(string field)
        {
            DBField fld = DBField.Field(field);
            return GroupBy(fld);
        }

        /// <summary>
        /// Appends a GROUP BY [table].[field] to this statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery GroupBy(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return GroupBy(fld);
        }

        /// <summary>
        /// Appends a GROUP BY [owner].[table].[field] to this statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery GroupBy(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return GroupBy(fld);
        }

        /// <summary>
        /// Appends a GROUP BY [any clause] to this statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Appends the clause onto the select set
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public new DBSelectQuery Select(DBClause clause)
        {
            if (this._select == null)
                this._select = DBSelectSet.Select(clause);
            else
                this._select = (DBSelectSet)this._select.And(clause);
            _last = this._select;

            return this;
        }

        /// <summary>
        /// Appends the [field] onto the select set.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Select(string field)
        {
            DBField fld = DBField.Field(field);
            return this.Select(fld);
        }

        /// <summary>
        /// Appends the [table].[field] onto the select set
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Select(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return this.Select(fld);
        }

        /// <summary>
        /// Appends the [owner].[table].[field] onto the select set
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public DBSelectQuery Select(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return this.Select(fld);
        }

        #endregion

        //
        // where
        //

        #region public DBSelectQuery Where(DBReference left, ComparisonOperator compare, DBClause right) + 10 overload

        /// <summary>
        /// Appends the WHERE comparison clause onto the select set
        /// </summary>
        /// <param name="left"></param>
        /// <param name="compare"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery Where(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._where = fs;
            this._last = fs;

            return this;
        }

        /// <summary>
        /// Appends the WHERE field to field comparison clause onto the select set
        /// </summary>
        /// <param name="left"></param>
        /// <param name="compare"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery Where(string left, Compare compare, string right)
        {
            DBField fls = DBField.Field(left);
            DBField frs = DBField.Field(right);

            return Where(fls,compare,frs);
        }

        /// <summary>
        /// Appends the WHERE field to clause comparison clause onto the select set
        /// </summary>
        /// <param name="field"></param>
        /// <param name="compare"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Where(string field, Compare compare, DBClause clause)
        {
            DBField left = DBField.Field(field);

            return Where(left, compare, clause);
        }

        /// <summary>
        /// Appends the WHERE [table].[field] to [table].[field] comparison clause onto the select set
        /// </summary>
        /// <param name="lefttable"></param>
        /// <param name="leftfield"></param>
        /// <param name="compare"></param>
        /// <param name="righttable"></param>
        /// <param name="rightfield"></param>
        /// <returns></returns>
        public DBSelectQuery Where(string lefttable, string leftfield, Compare compare, string righttable, string rightfield)
        {
            DBField fls = DBField.Field(lefttable, leftfield);
            DBField frs = DBField.Field(righttable, rightfield);

            return Where(fls, compare, frs);


        }

        /// <summary>
        /// Appends the WHERE comparison clause onto the select set
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBSelectQuery Where(DBComparison compare)
        {
            DBFilterSet fs;
            if (null == this._where)
                fs = DBFilterSet.Where(compare);
            else
                fs = this._where.And(compare);

            this._where = fs;
            this._last = fs;

            return this;
        }

        /// <summary>
        /// Appends a WHERE comparison for a collection of OR'd clauses e.g WHERE (A=1) OR (A=2) OR (A=3) OR....
        /// </summary>
        /// <param name="any"></param>
        /// <returns></returns>
        public DBSelectQuery WhereAny(params DBComparison[] any)
        {
            DBComparison joined = DBComparison.Any(any);
            return this.Where(joined);
        }

        /// <summary>
        /// Appends a WHERE comparison for a collection of AND'd clauses e.g WHERE (A=1) AND (B=2) AND (C=3) AND...
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public DBSelectQuery WhereAll(params DBComparison[] all)
        {
            DBComparison joined = DBComparison.All(all);
            return this.Where(joined);
        }

        /// <summary>
        /// Appends a WHERE comparison for a collection of AND NOT'd clause WHERE (NOT (A=1)) AND (NOT (A=3)) AND (NOT ...
        /// </summary>
        /// <param name="none"></param>
        /// <returns></returns>
        public DBSelectQuery WhereNone(params DBComparison[] none)
        {
            DBComparison joined = DBComparison.None(none);
            return this.Where(joined);
        }

        #endregion

        #region public DBSelectQuery WhereFieldEquals(string field, DBClause value) + 1 overload

        /// <summary>
        /// Appends the WHERE [field] = .... to this select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WhereFieldEquals(string field, DBClause value)
        {
            return WhereField(field, Compare.Equals, value);
        }

        /// <summary>
        ///  Appends the WHERE [table].[field] = .... to this select statement
        /// </summary>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WhereFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return WhereField(fieldTable, fieldName, Compare.Equals, value);
        }

        #endregion

        #region public DBSelectQuery WhereField(string field, ComparisonOperator op, DBClause value) + 2 overload

        /// <summary>
        ///  Appends the WHERE [field] op .... to this select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WhereField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Where(fld, op, value);
        }

        /// <summary>
        ///  Appends the WHERE [table].[field] op .... to this select statement
        /// </summary>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WhereField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Where(fld, op, value);
        }

        /// <summary>
        /// Appends the WHERE [owner].[table].[field] op .... to this select statement
        /// </summary>
        /// <param name="fieldOwner"></param>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery WhereField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Where(fld, op, value);
        }

        #endregion

        #region public DBSelectQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads
        /// <summary>
        /// Appends the AND ( [compare] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhere(DBComparison compare)
        {
            _where = _where.And(compare);
            _last = _where;
            return this;
        }
        
        /// <summary>
        ///  Appends the AND ( [left] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.And(left, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the AND ( [field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhere(string field, Compare op, DBClause right)
        {
            _where = _where.And(field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the AND ( [table].[field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(table, field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        /// Appends the AND ( [owner].[table].[field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery AndWhereXXX(params DBComparison[] any)

        /// <summary>
        /// Appends to the WHERE with an AND (compare OR compare OR compare)
        /// </summary>
        /// <param name="any"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereAny(params DBComparison[] any)
        {
            DBComparison joined = DBComparison.Any(any);
            return AndWhere(joined);
        }

        /// <summary>
        /// Appends to the WHERE with an AND (compare AND compare AND compare)
        /// </summary>
        /// <param name="any"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereAll(params DBComparison[] all)
        {
            DBComparison joined = DBComparison.All(all);
            return AndWhere(joined);
        }

        /// <summary>
        /// Appends to the WHERE with an AND ((NOT compare) AND (NOT compare) AND (NOT compare))
        /// </summary>
        /// <param name="any"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereNone(params DBComparison[] none)
        {
            DBComparison joined = DBComparison.None(none);
            return AndWhere(joined);
        }

        #endregion

        #region public DBSelectQuery OrWhereXXX(params DBComparison[] any)

        /// <summary>
        /// Appends to the WHERE with an OR (compare OR compare OR compare)
        /// </summary>
        /// <param name="any">All the comparisons to join into a single or statement</param>
        /// <returns></returns>
        public DBSelectQuery OrWhereAny(params DBComparison[] any)
        {
            DBComparison joined = DBComparison.Any(any);
            return OrWhere(joined);
        }

        /// <summary>
        /// Appends to the WHERE with an AND (compare AND compare AND compare)
        /// </summary>
        /// <param name="any">All the comparisons to join into a single or statement</param>
        /// <returns></returns>
        public DBSelectQuery OrWhereAll(params DBComparison[] all)
        {
            DBComparison joined = DBComparison.All(all);
            return OrWhere(joined);
        }

        /// <summary>
        /// Appends to the WHERE with an AND ((NOT compare) AND (NOT compare) AND (NOT compare))
        /// </summary>
        /// <param name="any">All the comparisons to join into a single or statement</param>
        /// <returns></returns>
        public DBSelectQuery OrWhereNone(params DBComparison[] none)
        {
            DBComparison joined = DBComparison.None(none);
            return OrWhere(joined);
        }

        #endregion

        #region public DBSelectQuery OrWhere(DBClause left, ComparisonOperator op, DBClause right) + 4 overloads

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public DBSelectQuery OrWhere(DBComparison comp)
        {
            _where = _where.Or(comp);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the OR ( [left] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.Or(left, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the OR ( [field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrWhere(string field, Compare op, DBClause right)
        {
            _where = _where.Or(field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the OR ( [table].[field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(table, field, op, right);
            _last = _where;

            return this;
        }

        /// <summary>
        ///  Appends the OR ( [owner].[table].[field] op [right] ) to the WHERE clause in the select statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBSelectQuery WhereIn(string field, params object[] values) + 3 overloads

        /// <summary>
        ///  Appends the WHERE [field] IN ( [value1], .... ) to the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Appends the WHERE DBField IN ( [value1], .... ) to the select statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DBSelectQuery WhereIn(DBField fld, params DBClause[] values)
        {
            DBComparison compare = DBComparison.In(fld, values);
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBSelectQuery WhereIn(string field, params DBClause[] value)
        {
            DBField fld = DBField.Field(field);
            return WhereIn(fld, value);
        }

        /// <summary>
        /// Appends the WHERE [table].[field] IN (SELECT ... ) to the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="select"></param>
        /// <returns></returns>
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

        #region public DBSelectQuery AndWhereIn(string field, params object[] values) + 3 overloads

        /// <summary>
        ///  Appends the WHERE [field] IN ( [value1], .... ) to the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereIn(string field, params object[] values)
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

            return this.AndWhere(compare);

        }

        /// <summary>
        /// Appends the WHERE [table].[field] IN ( [value1], .... ) to the select statement
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereIn(string table, string field, params DBClause[] values)
        {
            DBField fld = DBField.Field(table, field);
            
            DBComparison compare = DBComparison.In(fld, values);

            return this.AndWhere(compare);
        }

        /// <summary>
        /// Appends the WHERE [owner].[table].[field] IN ( [value1], .... ) to the select statement
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereIn(string owner, string table, string field, params DBClause[] values)
        {
            DBField fld = DBField.Field(owner, table, field);
            DBComparison compare = DBComparison.In(fld, values);
            
            return this.AndWhere(compare);
        }

        /// <summary>
        /// Appends the WHERE [table].[field] IN (SELECT ... ) to the select statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public DBSelectQuery AndWhereIn(string field, DBSelectQuery select)
        {
            DBField fld = DBField.Field(field);
            DBSubQuery subsel = DBSubQuery.SubSelect(select);
            DBComparison compare = DBComparison.In(fld, subsel);

            return this.AndWhere(compare);
        }

        #endregion

        #region public DBSelectQuery WhereExists(DBSelectQuery select)

        /// <summary>
        /// Appends the WHERE EXISTS (SELECT ... ) to the select statement
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Appends  the constant integer value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(int value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant boolean value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(bool value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant date time value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(DateTime value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant string value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(string value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant double value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(double value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant Guid value to the last clause in the statement
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery Const(Guid value)
        {
            DBConst con = DBConst.Const(value);
            return this.And(con);
        }

        /// <summary>
        /// Appends  the constant value of the specified type to the last clause in the statement
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Appends the first HAVING [left] op [right] clause to this SQL statement 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery Having(DBClause left, Compare op, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, op, right);
            this._having = fs;
            this._last = fs;

            return this;
        }

        /// <summary>
        /// Appends the first HAVING [compare] clause to this SQL statement 
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBSelectQuery Having(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._having = fs;
            this._last = fs;

            return this;
        }

        #endregion

        #region public DBSelectQuery HavingFieldEquals(string field, DBClause value) + 1 overload

        /// <summary>
        /// Appends the first HAVING [field] = [value] clause to this SQL statement 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingFieldEquals(string field, DBClause value)
        {
            return HavingField(field, Compare.Equals, value);
        }

        /// <summary>
        /// Appends the first HAVING [table].[field] = [value] clause to this SQL statement 
        /// </summary>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return HavingField(fieldTable, fieldName, Compare.Equals, value);
        }

        /// <summary>
        /// Appends the first HAVING [owner].[table].[field] = [value] clause to this SQL statement 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="owner"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingFieldEquals(string owner, string table, string field, DBClause value)
        {
            return HavingField(owner, table, field, Compare.Equals, value);
        }

        #endregion

        #region public DBSelectQuery HavingField(string field, ComparisonOperator op, DBClause value) + 2 overload
        /// <summary>
        /// Appends the first HAVING [field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Having(fld, op, value);
        }

        /// <summary>
        /// Appends the first HAVING [owner].[table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Having(fld, op, value);
        }

        /// <summary>
        /// Appends the first HAVING [owner].[table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="fieldOwner"></param>
        /// <param name="fieldTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBSelectQuery HavingField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Having(fld, op, value);
        }

        #endregion

        #region public DBSelectQuery AndHaving(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads
        /// <summary>
        /// Appends another AND HAVING [left] op [right] clause to this SQL statement 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndHaving(DBClause left, Compare op, DBClause right)
        {
            _having = _having.And(left, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another AND HAVING [field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndHaving(string field, Compare op, DBClause right)
        {
            _having = _having.And(field, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another AND HAVING [table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndHaving(string table, string field, Compare op, DBClause right)
        {
            _having = _having.And(table, field, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another AND HAVING [owner].[table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery AndHaving(string owner, string table, string field, Compare op, DBClause right)
        {
            _having = _having.And(owner, table, field, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another AND HAVING [comparison] clause to this SQL statement 
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public DBSelectQuery AndHaving(DBComparison comp)
        {
            _having = _having.And(comp);
            _last = _having;

            return this;
        }

        #endregion

        #region public DBSelectQuery OrHaving(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        /// <summary>
        ///  Appends another OR HAVING [left] op [right] clause to this SQL statement 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrHaving(DBClause left, Compare op, DBClause right)
        {
            _having = _having.Or(left, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another OR HAVING [field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrHaving(string field, Compare op, DBClause right)
        {
            _having = _having.Or(field, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another OR HAVING [table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrHaving(string table, string field, Compare op, DBClause right)
        {
            _having = _having.Or(table, field, op, right);
            _last = _having;

            return this;
        }
        /// <summary>
        /// Appends another OR HAVING [owner].[table].[field] op [value] clause to this SQL statement 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DBSelectQuery OrHaving(string owner, string table, string field, Compare op, DBClause right)
        {
            _having = _having.Or(owner, table, field, op, right);
            _last = _having;

            return this;
        }

        /// <summary>
        /// Appends another OR HAVING [comparison] clause to this SQL statement 
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public DBSelectQuery OrHaving(DBComparison comp)
        {
            _having = _having.Or(comp);
            _last = _having;

            return this;
        }

        #endregion
        
        //
        // calculation
        //

        #region IDBCalculable Implementation

        /// <summary>
        /// Appends a '+' [clause] statement to this Select statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Plus(DBClause clause)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Add, clause);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        /// <summary>
        /// Appends a '-' [clause] statement to this Select statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Minus(DBClause clause)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Subtract, clause);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        /// <summary>
        /// Appends a '*' [clause] statement to this Select statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Times(DBClause clause)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Multiply, clause);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        /// <summary>
        /// Appends a '/' [clause] statement to this Select statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Divide(DBClause clause)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Divide, clause);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        /// <summary>
        /// Appends a '%' [clause] statement to this Select statement
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBSelectQuery Modulo(DBClause clause)
        {
            if (this._last is IDBCalculable)
                this._last = ((IDBCalculable)this._last).Calculate(BinaryOp.Modulo, clause);
            else
                throw new InvalidOperationException("Cannot support calculations on the this query set");
            return this;
        }

        #endregion

    }
}
