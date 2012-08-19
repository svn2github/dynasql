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
    internal class DBTableSet : DBExpressionSet, IDBAlias, IDBJoinable, IDBBoolean
    {
        private IDBJoinable _root;

        //
        // properties
        //
        
        #region protected DBTable Root {get;set;}

        protected IDBJoinable Root
        {
            get { return _root; }
            set { _root = value; }
        }

        #endregion

        #region protected bool HasRoot {get;}

        protected bool HasRoot
        {
            get { return this._root != null; }
        }

        #endregion

        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this._root != null)
                return ((DBClause)this.Root).BuildStatement(builder);
            else
                return false;
        }

        #endregion

        //
        // Xml serialization methods
        //

        #region protected override string XmlElementName

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.From;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasRoot)
                ((DBClause)this.Root).WriteXml(writer, context);
            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            this.Root = (DBTable)context.Factory.Read(reader.Name, reader, context);
            return true;
        }

        #endregion


        //
        // Factory methods
        //

        #region public static DBTableSet From() + 3 overloads

        public static DBTableSet From()
        {
            DBTableSet ts = new DBTableSet();
            return ts;
        }

        public static DBTableSet From(string roottable)
        {
            DBTable tref = DBTable.Table(roottable);
            return From(tref);
        }

        public static DBTableSet From(string owner, string roottable)
        {
            DBTable tref = DBTable.Table(owner, roottable);
            return From(tref);
        }

        public static DBTableSet From(DBTable roottable)
        {
            DBTableSet set = new DBTableSet();
            set.Root = roottable;
            set.Last = roottable;
            return set;
        }

        public static DBTableSet From(DBSubQuery subquery)
        {
            DBTableSet set = new DBTableSet();
            set.Root = subquery;
            set.Last = subquery;
            return set;
        }

        #endregion

        #region public DBTableSet InnerJoin(DBClause table) + 4 overloads

        public DBTableSet InnerJoin(string table, string parentfield, string childfield)
        {
            return this.InnerJoin(table, parentfield, Compare.Equals, childfield);
        }

        public DBTableSet InnerJoin(string table, string parentfield, Compare op, string childfield)
        {
            DBTable tref = DBTable.Table(table);
            
            return this.InnerJoin(tref, DBField.Field(parentfield), op, DBField.Field(childfield));

        }

        public DBTableSet InnerJoin(DBTable table, DBClause parent, Compare op, DBClause child)
        {
            DBComparison comp = DBComparison.Compare(parent, op, child);

            if (this.Root is DBTable)
                ((DBTable)this.Root).InnerJoin(table, comp);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).InnerJoin(table, comp);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = table;

            return this;
        }

        public DBTableSet InnerJoin(DBTable table, DBComparison compare)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).InnerJoin(table, compare);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).InnerJoin(table, compare);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = table;

            return this;
        }

        

        public DBTableSet InnerJoin(DBClause table)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).InnerJoin(table, null);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).InnerJoin(table, null);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = table;

            return this;
        }

        #endregion

        public DBTableSet LeftJoin(DBTable table)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).LeftJoin(table, null);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).LeftJoin(table, null);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = table;

            return this;
        }

        public DBTableSet RightJoin(DBTable table)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).RightJoin(table, null);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).RightJoin(table, null);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = table;

            return this;
        }

        public DBTableSet LeftJoin(DBSubQuery sub)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).LeftJoin(sub, null);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).LeftJoin(sub, null);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = sub;

            return this;
        }

        public DBTableSet RightJoin(DBSubQuery sub)
        {
            if (this.Root is DBTable)
                ((DBTable)this.Root).RightJoin(sub, null);
            else if (this.Root is DBSubQuery)
                ((DBSubQuery)this.Root).RightJoin(sub, null);
            else
                throw new InvalidOperationException("Current root does not support joins");

            this.Last = sub;

            return this;
        }

        #region public DBTableSet On(DBComparison comp) + 1 overload

        public DBTableSet On(DBClause parent, Compare comp, DBClause child)
        {
            DBComparison compare = DBComparison.Compare(parent, comp, child);
            this.Last = this.Root.On(compare);
            return this;
        }

        public DBTableSet On(DBComparison comp)
        {
            this.Last = this.Root.On(comp);
            return this;
        }

        #endregion

        //
        // hints
        //

        #region public DBTableSet WithHint(DBTableHint hint) + 2 overload

        /// <summary>
        /// Adds a specific query hint to the current table in this statement
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public DBTableSet WithHint(DBTableHintOption hint)
        {
            if (this.Last is DBTable)
                (this.Last as DBTable).WithHint(hint);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }


        /// <summary>
        /// Adds a specific query hint to the current table in this statement
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public DBTableSet WithHint(DBTableHint hint)
        {
            if (this.Last is DBTable)
                (this.Last as DBTable).WithHint(hint);
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
        public DBTableSet WithHint(DBTableHint hint, params string[] options)
        {
            if (this.Last is DBTable)
                this.Last = (this.Last as DBTable).WithHint(hint, options);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }

        #endregion

        #region public DBTableSet WithHints(params DBTableHint[] hints)

        /// <summary>
        /// Adds the list of hints to the current table in the query
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public DBTableSet WithHints(params DBTableHint[] hints)
        {
            if (this.Last is DBTable)
                this.Last = (this.Last as DBTable).WithHints(hints);
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }
        #endregion

        #region public DBTableSet ClearTableHints()

        /// <summary>
        /// Clears any table hints on the current table
        /// </summary>
        /// <returns></returns>
        public DBTableSet ClearTableHints()
        {
            if (this.Last is DBTable)
                this.Last = (this.Last as DBTable).ClearTableHints();
            else
                throw new InvalidOperationException(Errors.NoRootOrLastForHint);
            return this;
        }

        #endregion


        //
        // Interface Implementations
        //

        #region IDBJoinable Members

        DBClause IDBJoinable.On(DBComparison comp)
        {
            return this.On(comp);
        }

        #endregion

        #region IDBAlias Members

        public void As(string aliasName)
        {
            if (this.Last is IDBAlias)
                ((IDBAlias)this.Last).As(aliasName);
            else
                throw new ArgumentException("The last statement does not support Alias names");
        }

        #endregion

        #region IDBBoolean Members

        public DBClause And(DBClause reference)
        {
            if (this.Last == null || !(this.Last is IDBBoolean))
                throw new ArgumentException("The last statement does not support 'And' operations");

            this.Last = ((IDBBoolean)this.Last).And(reference);
            return this;
        }

        public DBClause Or(DBClause reference)
        {
            if (this.Last == null || !(this.Last is IDBBoolean))
                throw new ArgumentException("The last statement does not support 'Or' operations");

            this.Last = ((IDBBoolean)this.Last).Or(reference);
            return this;
        }

        #endregion
    }
}
