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
    public class DBUpdateQuery : DBQuery, IDBBoolean
    {
        //
        // properties
        //

        #region internal DBTableSet TableSet {get;set;}

        private DBTableSet _table;

        /// <summary>
        /// Gets or sets the DBTableSet in this update query
        /// </summary>
        internal DBTableSet TableSet
        {
            get { return this._table; }
            set { this._table = value; }
        }

        #endregion

        #region internal DBAssignSet AssignSet {get;set;}

        private DBAssignSet _assigns;

        /// <summary>
        /// Gets or sets the DBAssignSet in this update query
        /// </summary>
        internal DBAssignSet AssignSet
        {
            get { return this._assigns; }
            set { this._assigns = value; }
        }

        #endregion

        #region internal DBFilterSet WhereSet {get; set;}

        private DBFilterSet _where;

        /// <summary>
        /// Gets and sets the DBFilterSet for this update query
        /// </summary>
        internal DBFilterSet WhereSet
        {
            get { return this._where; }
            set { this._where = value; }
        }

        #endregion

        #region internal DBClause Last {get;set;}

        private DBClause _last;

        /// <summary>
        /// Gets or Sets the last clause used in the statement construction
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

        #region internal DBUpdateQuery()

        /// <summary>
        /// internal constructor - use the DBQuery.Update(...) methods to create a new DBUpdateQuery
        /// </summary>
        internal DBUpdateQuery()
        {
        }

        #endregion


        //
        // statement construction methods
        //

        #region public DBUpdateQuery Set(string field, DBClause toValue) + 2 overloads

        public DBUpdateQuery Set(string field, DBClause toValue)
        {
            DBField fld = DBField.Field(field);
            return Set(fld, toValue);
        }

        public DBUpdateQuery Set(DBClause item, DBClause toValue)
        {
            if(_assigns == null)
                _last = _assigns = DBAssignSet.Set(item,toValue);
            else
                _last = _assigns.AndSet(item,toValue);
            return this;
        }

        public DBUpdateQuery Set(DBClause assignment)
        {
            if (_assigns == null)
                _last = _assigns = DBAssignSet.Set(assignment);
            else
                _last = _assigns.AndSet(assignment);

            return this;
        }

        #endregion


        //
        // Where... methods
        //

        #region public DBUpdateQuery Where(DBClause left, Compare compare, DBClause right) + 1 overload

        public DBUpdateQuery Where(DBClause left, Compare compare, DBClause right)
        {
            DBFilterSet fs = DBFilterSet.Where(left, compare, right);
            this._where = fs;
            this._last = fs;

            return this;
        }

        public DBUpdateQuery Where(DBComparison compare)
        {
            DBFilterSet fs = DBFilterSet.Where(compare);
            this._where = fs;
            this._last = fs;

            return this;
        }

        #endregion

        /// <summary>
        /// Appends a WHERE comparison for a collection of OR'd clauses e.g WHERE (A=1) OR (A=2) OR (A=3) OR....
        /// </summary>
        /// <param name="any"></param>
        /// <returns></returns>
        public DBUpdateQuery WhereAny(params DBComparison[] any)
        {
            DBComparison joined = DBComparison.Any(any);
            return this.Where(joined);
        }

        /// <summary>
        /// Appends a WHERE comparison for a collection of AND'd clauses e.g WHERE (A=1) AND (B=2) AND (C=3) AND...
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public DBUpdateQuery WhereAll(params DBComparison[] all)
        {
            DBComparison joined = DBComparison.All(all);
            return this.Where(joined);
        }

        /// <summary>
        /// Appends a WHERE comparison for a collection of AND NOT'd clause WHERE (NOT (A=1)) AND (NOT (A=3)) AND (NOT ...
        /// </summary>
        /// <param name="none"></param>
        /// <returns></returns>
        public DBUpdateQuery WhereNone(params DBComparison[] none)
        {
            DBComparison joined = DBComparison.None(none);
            return this.Where(joined);
        }

        #region public DBUpdateQuery WhereFieldEquals(string field, DBClause value) + 1 overload

        public DBUpdateQuery WhereFieldEquals(string field, DBClause value)
        {
            return WhereField(field, Compare.Equals, value);
        }

        public DBUpdateQuery WhereFieldEquals(string fieldTable, string fieldName, DBClause value)
        {
            return WhereField(fieldTable, fieldName, Compare.Equals, value);
        }


        #endregion

        #region public DBUpdateQuery WhereField(string field, ComparisonOperator op, DBClause value) + 2 overload

        public DBUpdateQuery WhereField(string field, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(field);
            return Where(fld, op, value);
        }

        public DBUpdateQuery WhereField(string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldTable, fieldName);
            return Where(fld, op, value);
        }

        public DBUpdateQuery WhereField(string fieldOwner, string fieldTable, string fieldName, Compare op, DBClause value)
        {
            DBField fld = DBField.Field(fieldOwner, fieldTable, fieldName);
            return Where(fld, op, value);
        }

        #endregion

        #region public DBUpdateQuery AndWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBUpdateQuery AndWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.And(left, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery AndWhere(string field, Compare op, DBClause right)
        {
            _where = _where.And(field, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery AndWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery AndWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.And(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        #region public DBUpdateQuery OrWhere(DBClause left, ComparisonOperator op, DBClause right) + 3 overloads

        public DBUpdateQuery OrWhere(DBClause left, Compare op, DBClause right)
        {
            _where = _where.Or(left, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery OrWhere(string field, Compare op, DBClause right)
        {
            _where = _where.Or(field, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery OrWhere(string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(table, field, op, right);
            _last = _where;

            return this;
        }

        public DBUpdateQuery OrWhere(string owner, string table, string field, Compare op, DBClause right)
        {
            _where = _where.Or(owner, table, field, op, right);
            _last = _where;

            return this;
        }

        #endregion

        //
        // statement build method
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Overriden BuildStatement method to construct a valid SQL Update statement using the DBStatementBuilder parameter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginUpdateStatement();

            if (this._table == null)
                throw new InvalidOperationException("No Source was set on the update statement");

            this._table.BuildStatement(builder);

            if(_assigns == null)
                throw new InvalidOperationException("No Source was set on the update statement");
            this._assigns.BuildStatement(builder);

            if (this._where != null)
            {
                builder.BeginWhereStatement();
                this._where.BuildStatement(builder);
                builder.EndWhereStatement();
            }

            builder.EndUpdateStatement();
            return true;
        }

        #endregion

        //
        // xml serialization methods
        //

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Gets the Xml Element name for this DBUpdateQuery
        /// </summary>
        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Update;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        /// <summary>
        /// Overrides the default implementation to write all the inner elements to the XmlWriter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this._table != null)
            {
                this._table.WriteXml(writer, context);
            }

            if (this._assigns != null)
            {
                this.AssignSet.WriteXml(writer, context);
            }

            if (this._where != null)
            {
                this.WhereSet.WriteXml(writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Overrides the default implementation to read the table, assignments and where elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsElementMatch(XmlHelper.From, reader, context))
            {
                this.TableSet = (DBTableSet)context.Factory.Read(XmlHelper.From, reader, context);
                b = true;
            }
            else if (this.IsElementMatch(XmlHelper.Assignments, reader, context))
            {
                this.AssignSet = (DBAssignSet)context.Factory.Read(XmlHelper.Assignments, reader, context);
                b = true;
            }
            else if (this.IsElementMatch(XmlHelper.Where, reader, context))
            {
                this.WhereSet = (DBFilterSet)context.Factory.Read(XmlHelper.Where, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);


            return b;
        }

        #endregion

        

        #region IDBBoolean Members

        public DBUpdateQuery AndSet(DBClause reference)
        {
            return this.Set(reference);
        }

        public DBUpdateQuery AndSet(DBClause item, DBClause toValue)
        {
            return this.Set(item, toValue);
        }

        public DBUpdateQuery AndSet(string field, DBClause toValue)
        {
            DBField fld = DBField.Field(field);
            return this.AndSet(fld, toValue);
        }

        


        DBClause IDBBoolean.And(DBClause reference)
        {
            return this.AndSet(reference);
        }

        DBClause IDBBoolean.Or(DBClause reference)
        {
            throw new Exception("The Or operation is not supported on a DBUpdateQuery.");
        }

        #endregion
    }
}
