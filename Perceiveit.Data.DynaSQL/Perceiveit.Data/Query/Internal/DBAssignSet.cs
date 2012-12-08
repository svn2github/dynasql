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
    internal class DBAssignSet : DBExpressionSet
    {
        private DBClauseList _assignments;

        //
        // properties
        //

        #region protected DBClauseList<DBAssignSet> Assignments {get;}

        /// <summary>
        /// Gets the list of Assignments for this assign set.
        /// </summary>
        protected DBClauseList Assignments
        {
            get
            {
                if (_assignments == null)
                    _assignments = new DBClauseList();
                return _assignments;
            }
        }

        #endregion

        #region public bool HasAssignments

        public bool HasAssignments
        {
            get { return this._assignments != null && this._assignments.Count > 0; }
        }

        #endregion

        //
        // .ctor(s)
        //

        protected DBAssignSet(){}

        //
        // statement construction methods
        //

        #region public DBAssignSet AndSet(DBClause item, DBClause toValue)

        public DBAssignSet AndSet(DBClause item, DBClause toValue)
        {
            DBAssign assign = DBAssign.Set(item, toValue);
            return this.AndSet(assign);
        }

        #endregion

        #region internal DBAssignSet AndSet(DBClause assignment)

        internal DBAssignSet AndSet(DBClause assignment)
        {
            this.Last = assignment;
            this.Assignments.Add(assignment);
            return this;
        }

        #endregion


#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement Builder methods
        //
        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this._assignments == null || this._assignments.Count == 0)
                return false;
            else
            {
                builder.BeginSetValueList();
                this.Assignments.BuildStatement(builder);
                builder.EndSetValueList();
                return true;
            }
        }

        #endregion

#endif

        //
        // Xml Serialization methods
        //

        #region protected override string XmlElementName

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Assignments;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasAssignments)
                this.Assignments.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            return this.Assignments.ReadXml(reader.LocalName, reader, context);
        }

        #endregion

        //
        // static factory methods
        //

        #region internal static DBAssignSet Set(DBClause assignment) + 1 overload

        internal static DBAssignSet Set(DBClause item, DBClause toValue)
        {
            DBAssign assign = DBAssign.Set(item, toValue);
            return Set(assign);
        }

        internal static DBAssignSet Set(DBClause assignment)
        {
            DBAssignSet set = new DBAssignSet();
            set.Last = assignment;
            set.Assignments.Add(assignment);
            return set;
        }

        #endregion

        #region internal static DBAssignSet Assign()

        internal static DBAssignSet Assign()
        {
            DBAssignSet set = new DBAssignSet();
            return set;
        }

        #endregion

    }
}
