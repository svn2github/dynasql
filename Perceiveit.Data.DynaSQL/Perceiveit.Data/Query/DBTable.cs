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
    public abstract class DBTable : DBClause, IDBAlias, IDBJoinable, IDBBoolean
    {
        //
        // public properties
        //

        #region public string Name {get; set;}

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public string Owner {get;set;}

        private string _owner;

        public string Owner
        {
            get { return _owner; }
            protected set { _owner = value; }
        }

        #endregion

        #region public DBJoinList Joins {get;}

        private DBJoinList _joins;

        internal DBJoinList Joins
        {
            get
            {
                if (this._joins == null)
                    this._joins = new DBJoinList();
                return _joins;
            }
        }

        #endregion

        #region public string Alias {get; set;}

        private string _alias;

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        #endregion

        #region public bool HasJoins

        public bool HasJoins
        {
            get { return this._joins != null && this._joins.Count > 0; }
        }

        #endregion

        //
        // .ctor(s)
        //

        #region protected DBTable()

        /// <summary>
        /// 
        /// </summary>
        protected DBTable()
        {
        }

        #endregion

        //
        // instance methods
        //

        #region public DBJoin InnerJoin(string table, string parentfield, string childfield) + 2 overloads

        public DBJoin InnerJoin(string table, string parentfield, string childfield)
        {
            DBTable tbl = Table(table);
            DBField parent = DBField.Field(this.Owner, this.Name, parentfield);
            DBField child = DBField.Field(table, parentfield);

            return InnerJoin(tbl, parent, child);
        }

        public DBJoin InnerJoin(DBTable table, DBClause parentField, DBClause childField)
        {
            DBJoin join = DBJoin.InnerJoin(table, parentField, childField, Compare.Equals);
            this.Joins.Add(join);
            return join;
        }

        public DBJoin InnerJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.InnerJoin(foreign, compare);
            this.Joins.Add(join);
            return join;
        }

        #endregion

        #region public DBJoin Join(DBClause table, JoinType type, DBComparison comp)

        public DBJoin Join(DBClause table, JoinType type, DBComparison comp)
        {
            DBJoin join = DBJoin.Join(table, type, comp);
            this.Joins.Add(join);

            return join;
        }

        #endregion


        //
        // static factory methods
        //

        #region public static DBTable Table(string name)

        public static DBTable Table(string name)
        {
            DBTableRef tref = new DBTableRef();
            tref.Name = name;
            return tref;
        }

        #endregion

        #region public static DBTable Table(string owner, string name)

        public static DBTable Table(string owner, string name)
        {
            DBTableRef tref = new DBTableRef();
            tref.Name = name;
            tref.Owner = owner;
            return tref;
        }

        #endregion

        #region public static DBTable Table()

        public static DBTable Table()
        {
            DBTableRef tref = new DBTableRef();
            tref.Name = string.Empty;
            return tref;
        }

        #endregion

        //
        // Interface implementations
        //

        #region IDBAlias Members

        public void As(string aliasName)
        {
            this._alias = aliasName;
        }

        #endregion

        #region IDBJoinable Members

        public DBClause On(DBComparison compare)
        {
            IDBJoinable join = (IDBJoinable)this.Joins[this.Joins.Count - 1];
            join.On(compare);
            return (DBClause)join;
        }

        #endregion

        #region IDBBoolean Members

        public DBClause And(DBClause reference)
        {
            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.And(reference);
            return (DBClause)join;
        }

        public DBClause Or(DBClause reference)
        {
            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.Or(reference);
            return (DBClause)join;
        }

        #endregion

        
    }

    internal class DBTableRef : DBTable
    {
        //
        // SQL Statement
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;

            builder.WriteSourceTable(this.Owner, this.Name, this.Alias);

            if (this.HasJoins)
                this.Joins.BuildStatement(builder);

            return true;

        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Table; }
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
            {
                this.Alias = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsElementMatch(XmlHelper.JoinList, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.Joins.ReadXml(XmlHelper.JoinList, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.Owner) == false)
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);
            if (string.IsNullOrEmpty(this.Name) == false)
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);

            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAlias(writer, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Joins != null && this.Joins.Count > 0)
            {
                this.WriteStartElement(XmlHelper.JoinList, writer, context);
                this.Joins.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.JoinList, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

    }
}
