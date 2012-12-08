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
using System.Linq;
using System.Text;
using System.Data;

namespace Perceiveit.Data.Query
{
    public abstract class DBCreateTableQuery : DBCreateQuery
    {
        private DBClause _last = null;

        #region internal string TableName { get; set; }

        /// <summary>
        /// Gets or sets a reference to the table this query should create
        /// </summary>
        internal string TableName { get; set; }

        #endregion

        #region internal string TableOwner { get; set; }

        /// <summary>
        /// Gets or sets the name of the table owner
        /// </summary>
        internal string TableOwner { get; set; }

        #endregion

        #region internal DBColumnList Columns { get; }

        /// <summary>
        /// Gets the list of columns to create in this table 
        /// </summary>
        protected internal DBColumnList Columns { get; protected set; }

        #endregion

        #region internal DBConstraintList Constraints {get;}

        private DBConstraintList _constraints;

        /// <summary>
        /// gets the collection of Constraints defined on this create table query
        /// </summary>
        protected internal DBConstraintList ConstraintList
        {
            get
            {
                if (null == _constraints)
                    _constraints = new DBConstraintList();
                return _constraints;
            }
            protected set
            {
                _constraints = value;
            }
        }

        /// <summary>
        /// returns true if this create table query has constraints defined
        /// </summary>
        internal bool HasConstraints
        {
            get { return null != _constraints && _constraints.Count > 0; }
        }

        #endregion

        //
        // ctor(s)
        //

        #region protected DBCreateTableQuery(string owner, string name)

        /// <summary>
        /// protected constructor. Use the static factory
        /// methods to create and instance of this class
        /// </summary>
        /// <param name="table"></param>
        protected DBCreateTableQuery(string owner, string name)
        {
            this.TableName = name;
            this.TableOwner = owner;
            this.Columns = new DBColumnList();
            this._last = null;
        }

        #endregion

        //
        // instance methods
        //

        #region public DBCreateTableQuery Add(string columnname, DbType type) + 4 overloads

        /// <summary>
        /// Creates a new Column reference and Adds it to this tables list of columns
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DBCreateTableQuery Add(string columnname, DbType type)
        {
            DBColumn col = DBColumn.Column(columnname, type);
            return Add(col);
        }

        /// <summary>
        /// Creates a new Column reference and Adds it to this tables list of columns
        /// </summary>
        /// <param name="columnname"></param>
        /// <param name="type"></param>
        /// <param name="maxlength" ></param>
        /// <returns></returns>
        public DBCreateTableQuery Add(string columnName, DbType type, int maxlength)
        {
            DBColumn col = DBColumn.Column(columnName, type, maxlength);
            return Add(col);
        }

        /// <summary>
        /// Creates a new Column reference and Adds it to this tables list of columns
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public DBCreateTableQuery Add(string columnName, DbType type, DBColumnFlags flags)
        {
            DBColumn col = DBColumn.Column(columnName, type, flags);
            return Add(col);
        }

        /// <summary>
        /// Creates a new Column reference and Adds it to this tables list of columns
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="type"></param>
        /// <param name="maxlength"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public DBCreateTableQuery Add(string columnName, DbType type, int maxlength, DBColumnFlags flags)
        {
            DBColumn col = DBColumn.Column(columnName, type, maxlength, flags);
            return Add(col);
        }


        /// <summary>
        /// Adds the column reference to this tables list of columns
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public DBCreateTableQuery Add(DBColumn column)
        {
            if (_last is DBTableIndex)
            {
                DBTableIndex idx = _last as DBTableIndex;
                idx.AddColumn(column.Name);
            }
            else
            {
                this.Columns.Add(column);
                _last = column;
            }
            return this;
        }

        #endregion

        #region public DBCreateTableQuery Default(DBClause clause) + 1 overload

        public DBCreateTableQuery Default(object value)
        {
            if (!(value is DBClause))
                value = DBConst.Const(value);
            return Default((DBClause)value);
        }

        public DBCreateTableQuery Default(DBClause clause)
        {
            if (_last is DBColumn)
                ((DBColumn)_last).Default(clause);
            else
                throw new ArgumentException(Errors.NoDefaultForColumn);
            return this;
        }

        #endregion

        #region public DBCreateTableQuery ForeignKeys(params DBForeignKey[] foreignkeys)

        /// <summary>
        /// Adds the set of foreign keys to this create table statement
        /// </summary>
        /// <param name="foreignkeys"></param>
        /// <returns></returns>
        public DBCreateTableQuery Constraints(params DBConstraint[] constraints)
        {
            this.ConstraintList.AddRange(constraints);
            return this;
        }

        #endregion

        #region public DBCreateTableQuery IfNotExists()

        /// <summary>
        /// Ensures a check is done to ensure that this table does not exist before it is created
        /// </summary>
        /// <returns></returns>
        public DBCreateTableQuery IfNotExists()
        {
            this.CheckExists = DBExistState.NotExists;
            return this;
        }

        #endregion

        #region  public DBCreateTableQuery ClearDefaults()

        /// <summary>
        /// Clears all the default values from the columns
        /// </summary>
        /// <returns></returns>
        public DBCreateTableQuery ClearDefaults()
        {
            foreach (DBColumn col in this.Columns)
            {
                col.ClearDefault();
            }
            return this;
        }

        #endregion

        //
        // factory methods
        //

        #region public static DBCreateTableQuery Table(string name) + 1 overloads

        /// <summary>
        /// Instantiates and returns a new CREATE TABLE query
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static DBCreateTableQuery Table()
        {
            return Table(string.Empty, string.Empty);
        }

        /// <summary>
        /// Instantiates and returns a new CREATE TABLE query for the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateTableQuery Table(string name)
        {
            return Table(string.Empty, name);
        }

        /// <summary>
        /// Instantiates and returns a new CREATE TABLE query for the specified name and owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateTableQuery Table(string owner, string name)
        {
            return new DBCreateTableQueryRef(owner, name);
        }

        #endregion

    }

    internal class DBCreateTableQueryRef : DBCreateTableQuery
    {

        
        internal DBCreateTableQueryRef(string owner, string name)
            : base(owner, name)
        {

        }

        protected override string XmlElementName
        {
            get { return XmlHelper.CreateTable; }
        }

#if SILVERLIGHT
        // no statement building
#else

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            bool checknotexists = this.CheckExists == DBExistState.NotExists;
            builder.BeginCreate(DBSchemaTypes.Table, this.TableOwner,this.TableName, string.Empty, checknotexists);
            
            builder.BeginBlock(true);

            this.Columns.BuildStatement(builder, true, true);

            if (this.HasConstraints)
            {
                builder.BeginTableConstraints();
                this.ConstraintList.BuildStatement(builder, true, true);
                builder.EndTableConstraints();
            }

            builder.EndBlock(true); 
            builder.EndCreate(DBSchemaTypes.Table, checknotexists);
            return true;
        }

#endif

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.TableName))
                this.WriteAttribute(writer, XmlHelper.TableName, this.TableName, context);
            if (!string.IsNullOrEmpty(this.TableOwner))
                this.WriteAttribute(writer, XmlHelper.TableOwner, this.TableOwner, context);

            return base.WriteAllAttributes(writer, context);

        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.TableName, reader, context))
            {
                this.TableName = reader.Value;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.TableOwner, reader, context))
            {
                this.TableOwner = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Columns.Count > 0)
            {
                this.WriteStartElement(XmlHelper.ColumnList, writer, context);
                this.Columns.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.ColumnList, writer, context);
            }
            if (this.HasConstraints)
            {
                this.WriteStartElement(XmlHelper.ConstraintList, writer, context);
                this.ConstraintList.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.ConstraintList, writer, context);

            }
            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if(IsElementMatch(XmlHelper.ColumnList,reader,context))
            {
                reader.Read();
                DBColumnList all = new DBColumnList();
                all.ReadXml(XmlHelper.ColumnList, reader, context);
                this.Columns = all;
                return true;
            }
            else if(IsElementMatch(XmlHelper.ConstraintList,reader,context))
            {
                reader.Read();
                DBConstraintList list = new DBConstraintList();
                list.ReadXml(XmlHelper.ConstraintList, reader, context);
                this.ConstraintList = list;
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }
    }
}
