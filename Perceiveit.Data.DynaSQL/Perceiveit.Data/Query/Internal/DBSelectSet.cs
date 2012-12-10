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
    internal class DBSelectSet : DBCalculableExpressionSet, IDBAlias, IDBBoolean, IDBAggregate
    {

        private DBClauseList _results;

        //
        // properties
        //

        #region public DBClauseList Results {get;}

        
        public DBClauseList Results
        {
            get 
            {
                if (_results == null)
                    _results = new DBClauseList();
                return _results;
            }
        }

        #endregion

        #region protected bool HasResults

        protected bool HasResults
        {
            get { return this._results != null && this._results.Count > 0; }
        }

        #endregion


#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this._results == null || this._results.Count == 0)
                return false;
            else
                return this._results.BuildStatement(builder, false, true);
        }

        #endregion

#endif

        //
        // XML serialization methods
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Fields;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasResults)
                this.Results.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            return this.Results.ReadXml(this.XmlElementName, reader, context);
        }

        #endregion

        //
        // interface implementations
        //

        #region IDBAlias - public void As(string alias)

        public void As(string alias)
        {
            if (this.Last is IDBAlias)
                ((IDBAlias)this.Last).As(alias);
            else if (null == this.Last)
                throw new NullReferenceException("Cannot set an alias because there was no last item added in the query");
            else
                throw new NotSupportedException("Cannot alias a '" + this.Last.GetType().ToString() + "' item in the query");

        }

        #endregion

        #region IDBCalculable - protected override DBCalculableExpressionSet Calculate(BinaryOp op, DBClause dbref)

        protected override DBCalculableExpressionSet Calculate(BinaryOp op, DBClause dbref)
        {
            if (this.Last is IDBCalculable)
            {
                this.Results.Remove(this.Last);
                this.Last = (DBClause)((IDBCalculable)this.Last).Calculate(op, dbref);
                this.Results.Add(this.Last);
                return this;
            }
            else
                throw new InvalidOperationException("The current clause does not support calculations");
        }

        #endregion

        #region IDBAggregate - public DBClause Aggregate(AggregateFunction func, DBClause dbref)

        public DBClause Aggregate(AggregateFunction func, DBClause dbref)
        {
            DBAggregate agg = DBAggregate.Aggregate(func, dbref);
            this.Last = agg;
            this.Results.Add(agg);
            return this;
        }

        #endregion

        #region public DBSelectSet And(string field)

        public DBSelectSet And(string field)
        {
            DBField fld = DBField.Field(field);
            return this.And(fld);
        }

        #endregion

        #region public DBSelectSet And(DBClause dbref)

        public DBSelectSet And(DBClause dbref)
        {
            this.Results.Add(dbref);
            this.Last = dbref;
            return this;
        }

        #endregion

        #region DBClause IDBBoolean.And(DBClause dbref)

        DBClause IDBBoolean.And(DBClause dbref)
        {
            return this.And(dbref);
        }

        #endregion

        #region DBClause IDBBoolean.Or(DBClause dbref) (NOT SUPPORTED)

        DBClause IDBBoolean.Or(DBClause dbref)
        {
            throw new NotSupportedException("The OR operator cannot be applied to a select set");
        }

        #endregion

        //
        // static methods
        //

        #region public static DBSelectSet SelectCount() + 4 overloads

        public static DBSelectSet SelectCount()
        {
            return SelectAggregate(AggregateFunction.Count, DBField.AllFields());
        }

        public static DBSelectSet SelectCount(string field)
        {
            return SelectAggregate(AggregateFunction.Count, field);
        }

        public static DBSelectSet SelectCount(string table, string field)
        {
            return SelectAggregate(AggregateFunction.Count, table, field);
        }

        public static DBSelectSet SelectCount(string owner, string table, string field)
        {
            return SelectAggregate(AggregateFunction.Count, owner, table, field);
        }

        public static DBSelectSet SelectCount(DBClause fref)
        {
            return SelectAggregate(AggregateFunction.Count, fref);
        }

        #endregion

        #region public static DBSelectSet SelectAggregate(AggregateFunction funct, string field) + 3 overloads

        public static DBSelectSet SelectAggregate(AggregateFunction funct, string field)
        {
            DBField fref = DBField.Field(field);

            return SelectAggregate(funct, fref);
        }

        public static DBSelectSet SelectAggregate(AggregateFunction funct, string owner, string table, string field)
        {
            DBField fref = DBField.Field(owner, table, field);
            return SelectAggregate(funct, fref);
        }

        public static DBSelectSet SelectAggregate(AggregateFunction funct, string table, string field)
        {
            DBField fref = DBField.Field(table, field);
            return SelectAggregate(funct, fref);
        }

        public static DBSelectSet SelectAggregate(AggregateFunction funct, DBClause field)
        {
            DBAggregate agg = DBAggregate.Aggregate(funct, field);
            DBSelectSet sel = DBSelectSet.Select(agg);
            sel.Last = agg;
            return sel;
        }

        #endregion

        #region public static DBSelectSet SelectFields(params string[] fields)

        public static DBSelectSet SelectFields(params string[] fields)
        {
            DBSelectSet set = new DBSelectSet();
            if (fields != null)
            {
                foreach (string fld in fields)
                {
                    DBField fref = DBField.Field(fld);
                    set.Results.Add(fref);
                    set.Last = fref;
                }
            }
            return set;
        }

        #endregion

        #region public static DBSelectSet SelectAll() + 2 overloads

        public static DBSelectSet SelectAll()
        {
            DBSelectSet set = new DBSelectSet();
            DBFieldAllRef fref = new DBFieldAllRef();

            set.Results.Add(fref);
            set.Last = fref;

            return set;
        }

        public static DBSelectSet SelectAll(string table)
        {
            DBSelectSet set = new DBSelectSet();
            DBFieldAllRef fref = new DBFieldAllRef();
            fref.Table = table;
            set.Results.Add(fref);
            set.Last = fref;

            return set;
        }

        public static DBSelectSet SelectAll(string owner, string table)
        {
            DBField allRef = DBField.AllFields(owner, table);
            return Select(allRef);
        }

        public static DBSelectSet SelectAll(string catalog, string owner, string table)
        {
            DBField allref = DBField.AllFields(catalog, owner, table);
            return Select(allref);
        }

        #endregion

        #region public static DBSelectSet Select(DBClause fref) + 3 overloads

        public static DBSelectSet Select()
        {
            DBSelectSet set = new DBSelectSet();
            return set;
        }

        public static DBSelectSet Select(DBClause fref)
        {
            DBSelectSet set = Select();
            set.Results.Add(fref);
            set.Last = fref;
            return set;

        }

        public static DBSelectSet Select(string field)
        {
            DBField fref = DBField.Field(field);
            return Select(fref);
        }

        public static DBSelectSet Select(string table, string field)
        {
            DBField fref = DBField.Field(table, field);
            return Select(fref);
        }

        public static DBSelectSet Select(string owner, string table, string field)
        {
            DBField fref = DBField.Field(owner, table, field);
            return Select(fref);
        }

        #endregion

    }
}
