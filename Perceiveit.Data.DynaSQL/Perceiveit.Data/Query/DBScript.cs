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
    public class DBScript : DBQuery
    {

        #region internal List<DBQuery> Inner {get;}

        private DBStatementList _inner;

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

        public bool HasInnerStatements
        {
            get
            {
                return null != this._inner && this._inner.Count > 0;
            }
        }

        #endregion

        //
        // ctor(s)
        //

        #region internal DBScript()

        internal DBScript() : base()
        {
        }

        #endregion

        //
        // factory methods
        //

        #region public DBScript Append(DBStatement query)

        public DBScript Append(DBStatement query)
        {
            return this.Then(query);
        }

        #endregion

        #region public DBScript Then(DBStatement query)

        public DBScript Then(DBStatement query)
        {
            this.Inner.Add(query);
            return this;
        }

        #endregion

        #region public DBScript End()

        public DBScript End()
        {
            return this;
        }

        #endregion

        public static DBStatement Declare(DBParam param)
        {
            DBDeclaration dec = DBDeclaration.Declare(param);
            return dec;
        }

        public static DBStatement Declare(string name, System.Data.DbType type)
        {
            DBParam p = DBParam.Param(name, type);
            return Declare(p);
        }

        public static DBStatement Declare(string name, System.Data.DbType type, int size)
        {
            DBParam p = DBParam.Param(name, type, size);
            return Declare(p);
        }

        public static DBStatement Set(DBAssign assignment)
        {
            DBSet set = DBSet.Set(assignment);
            return set;
        }

        public static DBStatement Set(DBParam param, DBClause clause)
        {
            DBAssign assign = DBAssign.Set(param, clause);
            return Set(assign);
        }

        public static DBStatement Return()
        {
            DBReturn ret = DBReturn.Return();
            return ret;
        }

        public static DBStatement Return(DBClause clause)
        {
            DBReturn ret = DBReturn.Return(clause);
            return ret;
        }

        public static DBStatement Return(string paramName)
        {
            DBParam p = DBParam.Param(paramName);
            return Return(p);
        }

        public static DBStatement Return(DBParam param)
        {
            DBReturn ret = DBReturn.Return(param);
            return ret;
        }

        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this._inner != null && this._inner.Count > 0)
            {
                //confirm that this data provider can support multiple 
                //SQL statements
                if (_inner.Count > 1)
                {
                    if (builder.SupportsMultipleStatements == false)
                        throw new InvalidOperationException("The current database does not support multiple statements within a single command");
                }
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

        #endregion

        //
        // xml serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Script; }
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
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
