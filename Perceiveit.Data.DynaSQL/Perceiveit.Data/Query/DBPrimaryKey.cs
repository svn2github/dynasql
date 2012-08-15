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

namespace Perceiveit.Data.Query
{
    public abstract class DBPrimaryKey : DBConstraint
    {

        #region public string[] Columns { get; }

        private DBColumnList _columns;

        /// <summary>
        /// Gets the list of columns in this primary key
        /// </summary>
        protected internal DBColumnList PKColumns
        {
            get { return _columns; }
            protected set { _columns = value; }
        }

        #endregion

        //
        // ctors
        //

        #region protected DBPrimaryKey()

        /// <summary>
        /// Protected constructor for abstract class
        /// </summary>
        protected DBPrimaryKey()
            : base()
        {
            _columns = new DBColumnList();
        }

        #endregion

        //
        // public instance methods
        //

        #region public DBPrimaryKey Columns(params string[] names)

        /// <summary>
        /// Adds all the names of the columns to this Primary Key
        /// </summary>
        /// <param name="names">All the names which cannot be null or empty</param>
        /// <returns></returns>
        public DBPrimaryKey Columns(params string[] names)
        {
            
            foreach (string name in names)
            {
                this.ValidateColumnName(name);
                _columns.Add(DBColumn.Column(name));
            }
            return this;
        }

        /// <summary>
        /// Adds all of the columns to this Primary Key
        /// </summary>
        /// <param name="cols">All the column.Names cannot be null or empty</param>
        /// <returns></returns>
        public DBPrimaryKey Columns(params DBColumn[] cols)
        {
            foreach (DBColumn c in cols)
            {
                this.ValidateColumnName(c.Name);
                _columns.Add(c);
            }
            return this;
        }

        #endregion

        #region public DBPrimaryKey Column(string name)

        /// <summary>
        /// Adds the name of a column to this PrimaryKey (synonomous with Add)
        /// </summary>
        /// <param name="name">Cannot be null or empty</param>
        /// <returns>Itself</returns>
        public DBPrimaryKey Column(string name)
        {
            this.ValidateColumnName(name);
            DBColumn col = DBColumn.Column(name);
            _columns.Add(col);
            return this;
        }

        /// <summary>
        /// Adds the column to this PrimaryKey
        /// </summary>
        /// <param name="column">Column.Name cannot be null or empty</param>
        /// <returns>Itself</returns>
        public DBPrimaryKey Column(DBColumn column)
        {
            this.ValidateColumnName(column.Name);
            _columns.Add(column);
            return this;
        }

        #endregion

        #region public DBPrimaryKey Add(string name)


        /// <summary>
        /// Adds the name of a column to this PrimaryKey (synonomous with Column)
        /// </summary>
        /// <param name="name">Cannot be null or empty</param>
        /// <returns>Itself</returns>
        public DBPrimaryKey Add(string name)
        {
            return Column(name);
        }

        #endregion

        //
        // support methods
        //

        #region protected void ValidateColumnName(string name)

        /// <summary>
        /// Checks to make sure the name of the column is valid (
        /// </summary>
        /// <param name="name"></param>
        protected void ValidateColumnName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
        }

        #endregion

    }


    internal class DBPrimaryKeyRef : DBPrimaryKey
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.PrimaryKey; }
        }


        /// <summary>
        /// TODO:BuildStatement
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginCreate(DBSchemaTypes.PrimaryKey, string.Empty, this.Name, string.Empty, false);

            this.BuildColumnListStatement(builder, this.PKColumns);

            builder.EndCreate(DBSchemaTypes.PrimaryKey, false);

            return true;
        }

        private const char COLUMN_SEPARATOR = ',';

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsElementMatch(XmlHelper.ColumnList, reader, context))
            {
                this.PKColumns = new DBColumnList();
                this.PKColumns.ReadReferenceXml(reader, context);
                return true;
            }
            return base.ReadAnInnerElement(reader, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.PKColumns != null && this.PKColumns.Count > 0)
            {
                this.PKColumns.WriteReferenceXml(XmlHelper.ColumnList, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }
    }
}
