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
    internal class DBValueSet : DBCalculableExpressionSet, IDBBoolean
    {
        private DBClauseList _clauses;

        //
        // properties
        //

        #region public DBClauseList Clauses {get;}

        public DBClauseList Clauses
        {
            get 
            {
                if (_clauses == null)
                    _clauses = new DBClauseList();
                return _clauses;
            }
        }

        #endregion

        #region public bool HasClauses {get;}

        public bool HasClauses
        {
            get { return this._clauses != null && this._clauses.Count > 0; }
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
            if (this._clauses != null && this._clauses.Count > 0)
                return this.Clauses.BuildStatement(builder, false, true);
            else
                return false;
        }

        #endregion

#endif

        //
        // XML serialization methods
        //

        #region protected override string XmlElementName

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Values;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasClauses)
                this.Clauses.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            return this.Clauses.ReadXml(reader.LocalName, reader, context);
        }

        #endregion

        //
        // Interface implmentations
        //

        #region IDBCalculable - protected override DBCalculableExpressionSet Calculate(BinaryOp op, DBClause dbref)

        protected override DBCalculableExpressionSet Calculate(BinaryOp op, DBClause dbref)
        {
            if (this.Last == null)
                throw new InvalidOperationException("There is no previous value to calculate with");

            int index = this.Clauses.IndexOf(this.Last);
            DBCalc calc = DBCalc.Calculate(this.Last, op, dbref);
            this.Clauses[index] = calc;
            this.Last = calc;
            return this;
        }

        #endregion

        #region public DBClause And(DBClause clause)

        public DBClause And(DBClause clause)
        {
            this.Clauses.Add(clause);
            this.Last = clause;
            return this;
        }


        #endregion

        #region DBClause IDBBoolean.And(DBClause reference)

        DBClause IDBBoolean.And(DBClause reference)
        {
            if (this.Last is IDBBoolean)
                ((IDBBoolean)this.Last).And(reference);
            else
                this.And(reference);

            return this;
        }

        #endregion

        #region DBClause IDBBoolean.Or(DBClause reference)

        DBClause IDBBoolean.Or(DBClause reference)
        {
            if (this.Last is IDBBoolean)
                ((IDBBoolean)this.Last).Or(reference);
            else
                throw new NotSupportedException("The OR method is not directly supported on the ValueSet.");

            return this;
        }

        #endregion

        //
        // static statement factory methods
        //

        public static DBValueSet Values()
        {
            DBValueSet vs = new DBValueSet();
            return vs;
        }
    }
}
