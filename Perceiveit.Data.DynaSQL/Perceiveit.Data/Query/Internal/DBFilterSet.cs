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
    internal class DBFilterSet : DBExpressionSet, IDBBoolean
    {


        private DBClause _filters;

        //
        // properties
        //

        #region public DBClause Filters {get;set;}

        protected DBClause Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        #endregion

        #region protected bool HasFilters {get;}

        protected bool HasFilters
        {
            get { return this._filters != null; }
        }

        #endregion


        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (Filters == null)
                return false;
            else
            {
                //builder.BeginBlock();
                this.Filters.BuildStatement(builder);
                //builder.EndBlock();

                return true;
            }
        }

        #endregion

        //
        // Xml Serialization methods
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Where;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasFilters)
                this.Filters.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
  
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBClause filter = context.Factory.Read(reader.LocalName, reader, context);
            if (filter == null)
                return false;
            else
            {
                this.Filters = filter;
                return true;
            }
        }

        #endregion


        //
        // Statement factory methods
        //

        #region public DBFilterSet And(DBClause combine) + 4 overloads

        public DBFilterSet And(string field, Compare op, DBClause clause)
        {
            return And(DBField.Field(field), op, clause);
        }

        public DBFilterSet And(string table, string field, Compare op, DBClause clause)
        {
            return And(DBField.Field(table, field), op, clause);
        }

        public DBFilterSet And(string owner, string table, string field, Compare op, DBClause right)
        {
            return And(DBField.Field(owner, table, field), op, right);
        }

        public DBFilterSet And(DBClause left, Compare op, DBClause clause)
        {
            return And(DBComparison.Compare(left, op, clause));
        }

        public DBFilterSet And(DBClause combine)
        {
            if (this.Filters == null)
                throw new InvalidOperationException("There is no current comparison to combine with");
            DBBooleanOp bin = DBBooleanOp.Compare(this.Filters, BooleanOp.And, combine);
            this.Last = bin;
            this.Filters = bin;
            return this;
        }

        #endregion

        #region public DBFilterSet Or(DBClause combine) + 4 overloads

        public DBFilterSet Or(string field, Compare op, DBClause clause)
        {
            return Or(DBField.Field(field), op, clause);
        }

        public DBFilterSet Or(string table, string field, Compare op, DBClause clause)
        {
            return Or(DBField.Field(table, field), op, clause);
        }

        public DBFilterSet Or(string owner, string table, string field, Compare op, DBClause right)
        {
            return Or(DBField.Field(owner, table, field), op, right);
        }

        public DBFilterSet Or(DBClause left, Compare op, DBClause clause)
        {
            return Or(DBComparison.Compare(left, op, clause));
        }

        public DBFilterSet Or(DBClause combine)
        {
            if (this.Filters == null)
                throw new InvalidOperationException("There is no current comparison to combine with");
            DBBooleanOp bin = DBBooleanOp.Compare(this.Filters, BooleanOp.Or, combine);
            this.Last = bin;
            this.Filters = bin;

            return this;
        }

        #endregion

        //
        // static statement factory methods

        #region public static DBFilterSet Where(DBComparison compare) + 2 overloads

        public static DBFilterSet Where(DBClause leftRef, Compare op, DBClause right)
        {
            DBComparison comp = DBComparison.Compare(leftRef, op, right);
            return Where(comp);
        }

        public static DBFilterSet Where()
        {
            DBFilterSet fs = new DBFilterSet();
            return fs;
        }

        public static DBFilterSet Where(DBComparison compare)
        {
            DBFilterSet fs = new DBFilterSet();
            fs.Filters = compare;

            return fs;
        }

        #endregion

        //
        // Interface Implementations
        //

        #region IDBBoolean Members

        DBClause IDBBoolean.And(DBClause reference)
        {
            return this.And(reference);
        }

        DBClause IDBBoolean.Or(DBClause reference)
        {
            return this.Or(reference);
        }

        #endregion
    }
}
