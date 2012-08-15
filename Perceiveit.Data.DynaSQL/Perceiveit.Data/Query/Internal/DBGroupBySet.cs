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
    internal class DBGroupBySet : DBExpressionSet, IDBBoolean, IDBAggregate
    {
        private DBClauseList _grps;

        //
        // properties
        //

        #region protected DBClauseList Groupings {get;}

        protected DBClauseList Groupings
        {
            get
            {
                if (_grps == null)
                    _grps = new DBClauseList();
                return _grps;
            }
        }

        #endregion

        #region protected bool HasGroupings {get;}

        protected bool HasGroupings
        {
            get { return this._grps != null && this._grps.Count > 0; }
        }

        #endregion

        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this._grps == null || this._grps.Count == 0)
                return false;
            else
                return this._grps.BuildStatement(builder);
        }

        #endregion

        //
        // Xml serialization methods
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Group;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasGroupings)
                this.Groupings.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBClause c = context.Factory.Read(reader.LocalName, reader, context);
            if (c != null)
            {
                this.Groupings.Add(c);
                return true;
            }
            else
                return false;
        }

        #endregion

        //
        // Statement Factory methods
        //

        #region internal DBClause Aggregate(AggregateFunction func, DBClause dbref)

        internal DBClause Aggregate(AggregateFunction func, DBClause dbref)
        {
            DBAggregate agg = DBAggregate.Aggregate(func, dbref);
            this.Last = agg;
            this.Groupings.Add(agg);
            return this;
        }

        #endregion

        #region public DBSelectSet And(DBReference dbref)

        internal DBGroupBySet And(DBClause dbref)
        {
            this.Groupings.Add(dbref);
            this.Last = dbref;
            return this;
        }

        #endregion

        //
        // static factory methods
        //

        #region internal static DBGroupBySet GroupBy() + 4 overloads

        internal static DBGroupBySet GroupBy()
        {
            DBGroupBySet grp = new DBGroupBySet();
            return grp;
        }

        internal static DBGroupBySet GroupBy(string field)
        {
            DBField fld = DBField.Field(field);
            return GroupBy(fld);
        }

        internal static DBGroupBySet GroupBy(string table, string field)
        {
            DBField fld = DBField.Field(table, field);
            return GroupBy(fld);
        }

        internal static DBGroupBySet GroupBy(string owner, string table, string field)
        {
            DBField fld = DBField.Field(owner, table, field);
            return GroupBy(fld);
        }

        internal static DBGroupBySet GroupBy(DBClause clause)
        {
            DBGroupBySet grp = GroupBy();
            grp.Groupings.Add(clause);
            grp.Last = clause;

            return grp;
        }

        #endregion

        //
        // interface implementations
        //

        #region DBReference IDBBoolean.And(DBReference dbref)

        DBClause IDBBoolean.And(DBClause dbref)
        {
            return this.And(dbref);
        }

        #endregion

        #region DBReference IDBBoolean.Or(DBReference dbref) (NOT SUPPORTED)

        DBClause IDBBoolean.Or(DBClause dbref)
        {
            throw new NotSupportedException("The OR operator cannot be applied to a select set");
        }

        #endregion

        #region DBClause IDBArregate.Aggregate(AggregateFunction func, DBClause dbref)

        DBClause IDBAggregate.Aggregate(AggregateFunction func, DBClause dbref)
        {
            return this.Aggregate(func, dbref);
        }

        #endregion
    }
}
