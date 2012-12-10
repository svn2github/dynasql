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
    /// Defines a table reference in a DBQuery (Catalog.Owner.Name)
    /// </summary>
    public abstract class DBTable : DBClause, IDBAlias, IDBJoinable, IDBBoolean
    {
        //
        // public properties
        //

        #region public string Name {get; set;}

        private string _name;
        /// <summary>
        /// Gets or sets the Name of the table
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public string Owner {get;set;}

        private string _owner;

        /// <summary>
        /// Gets or sets the schema owner of the table
        /// </summary>
        public string Owner
        {
            get { return _owner; }
            protected set { _owner = value; }
        }

        #endregion

        #region public string Catalog { get; set }

        private string _catalog;

        /// <summary>
        /// Gets or sets the catalog for this table.
        /// </summary>
        public string Catalog
        {
            get { return this._catalog; }
            set { this._catalog = value; }
        }

        #endregion

        #region public bool Temporary {get;set;}

        private bool _istemp = false;
        /// <summary>
        /// Gets or sets the temporary flag.
        /// </summary>
        public bool Temporary
        {
            get { return _istemp; }
            set { _istemp = value; }
        }

        #endregion

        #region public DBJoinList Joins {get;}

        private DBJoinList _joins;
        /// <summary>
        /// Gets the list of JOINS on this table
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Alias name of this table
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        #endregion

        #region public bool HasJoins

        /// <summary>
        /// Returns true if this table reference has joins
        /// </summary>
        public bool HasJoins
        {
            get { return this._joins != null && this._joins.Count > 0; }
        }

        #endregion

        #region public DBTableHintList Hints { get; set; }

        private DBTableHintSet _hints;

        /// <summary>
        /// Gets the list of hints on this table
        /// </summary>
        internal DBTableHintSet Hints
        {
            get
            {
                if (_hints == null)
                    _hints = new DBTableHintSet(this);

                return _hints;
            }
        }

        #endregion

        #region public bool HasHints

        /// <summary>
        /// Returns true is this table has hints defined
        /// </summary>
        public bool HasHints
        {
            get
            {
                return this._hints != null;
            }
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

        #region public DBJoin InnerJoin(string table, string parentfield, string childfield) + 4 overloads
        
        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentfield"></param>
        /// <param name="childfield"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(string table, string parentfield, string childfield)
        {
            DBTable tbl = Table(table);
            return InnerJoin(tbl, parentfield, childfield);
        }

        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentField"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(string owner, string table, string parentfield, string childfield)
        {
            DBTable tbl = Table(owner, table);
            return InnerJoin(tbl, parentfield, childfield);
        }

        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentField"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(string catalog, string owner, string table, string parentfield, string childfield)
        {
            DBTable tbl = Table(catalog, owner, table);
            return InnerJoin(tbl, parentfield, childfield);
        }

        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentField"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBTable table, string parentfield, string childfield)
        {
            DBField parent = DBField.Field(this.Catalog, this.Owner, this.Name, parentfield);
            DBField child = DBField.Field(table.Catalog, table.Owner, table.Name, childfield);
            return InnerJoin(table, parent, child);
        }

        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="table"></param>
        /// <param name="parentField"></param>
        /// <param name="childField"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBTable table, DBClause parentField, DBClause childField)
        {
            DBJoin join = DBJoin.InnerJoin(table, parentField, childField, Compare.Equals);
            this.Joins.Add(join);
            return join;
        }

        /// <summary>
        /// Appends a new INNER JOIN between this table an the specified clause matching with the specified comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin InnerJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.InnerJoin(foreign, compare);
            this.Joins.Add(join);
            return join;
        }

        #endregion

        #region public DBJoin LeftJoin(DBTable tbl, DBClause parent, Compare comp, DBClause child)

        /// <summary>
        /// Appends a new LEFT OUTER JOIN between this table and the specified table matching between this tables parentfield and the other tables child field
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="parent"></param>
        /// <param name="comp"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public DBJoin LeftJoin(DBTable tbl, DBClause parent, Compare comp, DBClause child)
        {
            DBComparison c = DBComparison.Compare(parent, comp, child);
            return LeftJoin(tbl, c); 
        }

        /// <summary>
        /// Appends a new LEFT OUTER JOIN between this table an the specified clause matching on the comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin LeftJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.Join(foreign, JoinType.LeftOuter, compare);
            this.Joins.Add(join);
            return join;
        }

        /// <summary>
        /// Appends a new RIGHT OUTER JOIN between this table an the specified clause matching on the comparison
        /// </summary>
        /// <param name="foreign"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBJoin RightJoin(DBClause foreign, DBComparison compare)
        {
            DBJoin join = DBJoin.Join(foreign, JoinType.RightOuter, compare);
            this.Joins.Add(join);
            return join;
        }

        #endregion


        #region public DBJoin Join(DBClause table, JoinType type, DBComparison comp)

        /// <summary>
        /// Appends a new [type] JOIN between this table an the specified clause matching on the comparison
        /// </summary>
        /// <param name="table"></param>
        /// <param name="type"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public DBJoin Join(DBClause table, JoinType type, DBComparison comp)
        {
            DBJoin join = DBJoin.Join(table, type, comp);
            this.Joins.Add(join);

            return join;
        }

        #endregion

        #region WithHint(DBTableHint hint) + ClearHints()


        /// <summary>
        /// Appends a Query execution table hint
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public DBTable WithHint(DBTableHint hint)
        {
            this.Hints.WithHint(hint);
            return this;
        }

        /// <summary>
        /// Appends a Query execution table hint and option
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public DBTable WithHint(DBTableHint hint, params string[] options)
        {
            this.Hints.WithHint(hint, options);
            return this;
        }

        /// <summary>
        /// Appends a Query execution table hint and option
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        public DBTable WithHint(DBTableHintOption hint)
        {
            this.Hints.WithHint(hint);
            return this;
        }

        /// <summary>
        /// Appends all the Query execution table hints
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public DBTable WithHints(params DBTableHint[] hints)
        {
            if (null != hints && hints.Length > 0)
            {
                foreach (DBTableHint hint in hints)
                {
                    this.WithHint(hint);
                }
            }
            return this;
        }

        public DBTable ClearTableHints()
        {
            this.Hints.Clear();
            return this;
        }

        #endregion


        //
        // static factory methods
        //

        #region public static DBTable Table(string name)

        /// <summary>
        /// Creates and returns a new DBTable reference with the specifed name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBTable Table(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBTableRef tref = new DBTableRef();
            tref.Name = name;
            return tref;
        }

        #endregion

        #region public static DBTable Table(string owner, string name)
        /// <summary>
        /// Creates and returns a new DBTable reference with the specifed schema owner name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBTable Table(string owner, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBTableRef tref = new DBTableRef();
            tref.Name = name;
            tref.Owner = owner;
            return tref;
        }

        #endregion

        #region public static DBTable Table(string catalog, string owner, string name)

        /// <summary>
        /// Creates an returns a new DBTable reference with the specified schema owner and catalog / database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBTable Table(string catalog, string owner, string name)
        {
            DBTable table = Table(owner, name);
            table.Catalog = catalog;
            return table;
        }

        #endregion

        #region public static DBTable Table()
        /// <summary>
        /// Creates and returns a new DBTable reference with the specifed name
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Specifies the alias name for this table
        /// </summary>
        /// <param name="aliasName"></param>
        public void As(string aliasName)
        {
            this._alias = aliasName;
        }

        #endregion

        #region IDBJoinable Members

        /// <summary>
        /// Adds a comparison to the last join for this table
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public DBClause On(DBComparison compare)
        {
            if (this.HasJoins == false)
                throw new InvalidOperationException("No joined tables or sub queries to join to");

            IDBJoinable join = (IDBJoinable)this.Joins[this.Joins.Count - 1];
            join.On(compare);
            return (DBClause)join;
        }

        #endregion

        #region IDBBoolean Members

        /// <summary>
        /// Adds an AND comparison to the last join
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause And(DBClause reference)
        {
            if (this.HasJoins == false)
                throw new InvalidOperationException("No joined tables or sub queries to join to");

            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.And(reference);
            return (DBClause)join;
        }

        /// <summary>
        /// Adds an OR comparison to the last join
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public DBClause Or(DBClause reference)
        {
            if (this.HasJoins == false)
                throw new InvalidOperationException("No joined tables or sub queries to join to");

            IDBBoolean join = (IDBBoolean)this.Joins[this.Joins.Count - 1];
            join.Or(reference);
            return (DBClause)join;
        }

        #endregion

        
    }

    internal class DBTableRef : DBTable
    {


#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;
            if (this.Temporary)
                builder.WriteSourceTable(this.Owner, builder.DatabaseProperties.TemporaryTablePrefix + this.Name, this.Alias);
            else
                builder.WriteSourceTable(this.Owner, this.Name, this.Alias);

            if (this.HasHints)
                this.Hints.BuildStatement(builder);
            

            if (this.HasJoins)
                this.Joins.BuildStatement(builder);

            return true;

        }

        #endregion


#endif
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
            else if (this.IsElementMatch(XmlHelper.TableHintSet, reader, context))
            {
                this.Hints.Clear();
                this.Hints.ReadXml(reader, context);
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
            if (this.HasHints)
            {
                this.Hints.WriteXml(writer, context);
            }
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
