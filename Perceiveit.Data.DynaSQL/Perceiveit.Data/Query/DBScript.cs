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
    /// Represents a batch of DBStatement statements that can be passed to the 
    /// database engine in one command and executed
    /// </summary>
    public abstract class DBScript : DBQuery, IEnumerable<DBStatement>
    {

        #region internal List<DBQuery> Inner {get;}

        private DBStatementList _inner;
       /// <summary>
       /// Gets the list of statements in this script
       /// </summary>
        internal DBStatementList Inner
        {
            get 
            {
                if (_inner == null)
                    _inner = new DBStatementList();
                return _inner;
            }
        }

        #endregion

        #region public bool HasInnerStatements {get;}

        /// <summary>
        /// Returns true if this script has one or more inner statements
        /// </summary>
        public bool HasInnerStatements
        {
            get
            {
                return null != this._inner && this._inner.Count > 0;
            }
        }

        #endregion

        #region protected DBScript Last {get;set;}

        private DBScript _last = null;

        /// <summary>
        /// Gets or sets the last inner script for nesting
        /// </summary>
        protected DBScript Last
        {
            get { return _last; }
            set { _last = value; }
        }

        #endregion

        //
        // ctor(s)
        //

        #region internal DBScript()
        /// <summary>
        /// Creates a new DBScript
        /// </summary>
        internal DBScript() : base()
        {
        }

        #endregion

        //
        // factory methods
        //

        #region public DBScript Append(DBStatement query)
        /// <summary>
        /// Appends the statement onto this script
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DBScript Append(DBStatement query)
        {
            return this.Then(query);
        }

        #endregion

        #region public DBScript Then(DBStatement query)
        /// <summary>
        /// Appends the statement onto this script
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DBScript Then(DBStatement query)
        {
            if (Last != null)
                this.Last.Then(query);
            else
                this.Inner.Add(query);
            return this;
        }

        #endregion

        #region public DBScript Begin()

        /// <summary>
        /// Begins a new Inner Statement block
        /// </summary>
        /// <returns></returns>
        public DBScript Begin()
        {
            if (Last != null)
                return Last.Begin();
            else
            {
                DBScript inner = DBQuery.Begin();
                this.Inner.Add(inner);
                this.Last = inner;
                return this;
            }
        }

        #endregion

        #region public DBScript End()

        /// <summary>
        /// Closes the script
        /// </summary>
        /// <returns></returns>
        public DBScript End()
        {
            DBScript toclear = this.GetLastParentBlock();
            if (null != toclear)
                toclear.Last = null;

            return this;
        }

        /// <summary>
        /// Returns the parent block that has only 1 nested script
        /// </summary>
        /// <returns></returns>
        protected virtual DBScript GetLastParentBlock()
        {
            if (null == Last)
                return null;
            else if (null == Last.Last)
            {
                //we have an inner script, but are the last parent
                return this;
            }
            else
            {
                //Our inner script has inner scripts so get them to do the work.
                return Last.GetLastParentBlock();
            }
        }

        #endregion

        #region public DBScript Declare(DBParam param)

        /// <summary>
        /// creates a parameter Declaration in this script
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public DBScript Declare(DBParam param)
        {
            DBDeclaration dec = DBDeclaration.Declare(param);
            this.Then(dec);
            return this;
        }

        #endregion

        #region public DBScript Set(DBAssign assignment)

        /// <summary>
        /// creates a parameter assignment in this script
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        public DBScript Set(DBAssign assignment)
        {
            DBSet set = DBSet.Set(assignment);
            this.Then(set);
            return this;
        }

        /// <summary>
        /// creates a parameter assignment statement in this script
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBScript Set(DBParam param, DBClause clause)
        {
            DBAssign assign = DBAssign.Set(param, clause);
            return this.Set(assign);
        }

        #endregion

        #region public DBScript Return() + 3 overloads

        /// <summary>
        /// Creates a return statement in this script
        /// </summary>
        /// <returns></returns>
        public DBScript Return()
        {
            DBReturn ret = DBReturn.Return();
            this.Then(ret);
            return this;
        }

        /// <summary>
        /// Creates a return statement with value in this script
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DBStatement Return(DBClause clause)
        {
            DBReturn ret = DBReturn.Return(clause);
            this.Then(ret);
            return ret;
        }

        /// <summary>
        /// Creates a return statement with the parameter in this script
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public DBStatement Return(string paramName)
        {
            DBParam p = DBParam.Param(paramName);
            return Return(p);
        }


        /// <summary>
        /// Creates a return statement with the parameter in this script
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public DBStatement Return(DBParam param)
        {
            DBReturn ret = DBReturn.Return(param);
            this.Then(ret);
            return ret;
        }

        #endregion 

        #region private class StatementEnumerator : IEnumerator<DBStatement>

        /// <summary>
        /// private wrapper of the base classes DBClause enumerator to return only DBStatements
        /// </summary>
        private class StatementEnumerator : IEnumerator<DBStatement>
        {
            private IEnumerator<DBClause> _baseEnum;

            public StatementEnumerator(IEnumerator<DBClause> baseEnum)
            {
                this._baseEnum = baseEnum;
            }

            public DBStatement Current
            {
                get { return _baseEnum.Current as DBStatement; }
            }

            public void Dispose()
            {
                _baseEnum.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _baseEnum.Current; }
            }

            public bool MoveNext()
            {
                return _baseEnum.MoveNext();
            }

            public void Reset()
            {
                _baseEnum.Reset();
            }
        }

        #endregion

        #region public IEnumerator<DBStatement> GetEnumerator()

        /// <summary>
        /// Returns a statement enumerator that will 
        /// traverse across all the DBStatements in this script
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DBStatement> GetEnumerator()
        {
            return new StatementEnumerator(this.Inner.GetEnumerator());
        }

        /// <summary>
        /// Explicit interface implementation
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }



    public class DBScriptRef : DBScript
    {

#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Builds the script with the specified statement builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this.HasInnerStatements)
            {
                //confirm that this data provider can support multiple 
                //SQL statements
                ValidateScriptExecution(builder);
                builder.BeginScript();
                foreach (DBStatement q in this.Inner)
                {
                    q.BuildStatement(builder);
                }
                builder.EndScript();
                return true;
            }
            else
                return false;
        }

        private void ValidateScriptExecution(DBStatementBuilder builder)
        {

#if STRICT_SQL
            if (_inner.Count > 1)
            {
                if (builder.SupportsMultipleStatements == false)
                    throw new InvalidOperationException("The current database does not support multiple statements within a single command");
            }
#endif
        }

        #endregion


#endif
        //
        // xml serialization
        //

        #region protected override string XmlElementName {get;}
        /// <summary>
        /// Gets the xml element name for this script
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Script; }
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        /// <summary>
        /// reads the inner elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            DBClause c = this.ReadNextInnerClause(reader.Name, reader, context);
            if (c != null)
            {
                this.Inner.Add(c);
                b = true;
            }
            else
                b = false;

            return b;
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        /// <summary>
        /// writes all the inner elements in this script
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasInnerStatements)
            {
                this.Inner.WriteXml(writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion


    }

    internal class DBQueryList : DBClauseList<DBQuery>
    {
    }

    internal class DBStatementList : DBClauseList<DBStatement>
    {

    }
}
