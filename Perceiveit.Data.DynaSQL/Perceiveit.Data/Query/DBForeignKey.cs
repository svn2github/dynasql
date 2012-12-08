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
    public abstract class DBForeignKey : DBConstraint
    {

        #region protected bool HasReference

        private bool _hasreference;

        /// <summary>
        /// If the reference has been set on this foreign key. Once set colums should be added to the ReferenceColumns rather than the TableColumns
        /// </summary>
        protected bool HasReference
        {
            get { return _hasreference; }
            set { _hasreference = value; }
        }

        #endregion

        #region  internal string ReferenceOwner { get; set; }

        /// <summary>
        /// Gets or sets the name of the referenced object owner in this FK
        /// </summary>
        internal string ReferenceOwner { get; set; }

        #endregion

        #region internal string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the referenced object in this FK
        /// </summary>
        internal string ReferenceName { get; set; }

        #endregion

        #region internal DBColumnList TableColumns { get;}

        /// <summary>
        /// Gets the Columns on the declared table
        /// </summary>
        protected internal DBColumnList TableColumns { get; protected set; }

        #endregion

        #region internal DBColumnList ReferenceColumns { get; }

        /// <summary>
        /// Gets the Columns on the reference
        /// </summary>
        protected internal DBColumnList ReferenceColumns { get; protected set; }

        #endregion

        #region internal DBFKAction OnDeleteAction { get; set; }

        /// <summary>
        /// Gets or sets the delete action
        /// </summary>
        internal DBFKAction OnDeleteAction { get; set; }

        #endregion

        #region internal DBFKAction OnUpdateAction { get; set; }

        /// <summary>
        /// Gets  or sets the update action
        /// </summary>
        internal DBFKAction OnUpdateAction { get; set; }

        #endregion

        //
        // ctor
        //

        #region protected DBForeignKey()

        /// <summary>
        /// 
        /// </summary>
        protected DBForeignKey()
        {
            this.ReferenceColumns = new DBColumnList();
            this.TableColumns = new DBColumnList();
            this.OnDeleteAction = DBFKAction.Undefined;
            this.OnUpdateAction = DBFKAction.Undefined;
        }

        #endregion

        //
        // methods
        //

        #region public DBForeignKey Column(string name)

        /// <summary>
        /// Appends a column to the Foreign Key constraint either with the table 
        /// called or the referenced table depending on the order of the statement
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBForeignKey Column(string name)
        {
            DBColumn fld = DBColumn.Column(name);
            if (_hasreference)
                this.ReferenceColumns.Add(fld);
            else
                this.TableColumns.Add(fld);
            return this;
        }

        public DBForeignKey Column(DBColumn column)
        {
            if (_hasreference)
                this.ReferenceColumns.Add(column);
            else
                this.TableColumns.Add(column);
            return this;
        }

        #endregion

        #region public DBForeignKey Columns(params string[] names)

        /// <summary>
        /// Appends all the columns to the Foreign Key constraint either with the table 
        /// called or the referenced table depending on the order of the statement
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public DBForeignKey Columns(params string[] names)
        {
            foreach (string name in names)
            {
                Column(name);
            }
            return this;
        }

        /// <summary>
        /// Appends all the columns to the Foreign Key constraint either with the table 
        /// called or the referenced table depending on the order of the statement
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public DBForeignKey Columns(params DBColumn[] cols)
        {
            foreach (DBColumn col in cols)
            {
                Column(col);
            }

            return this;
        }

        #endregion


        #region public DBForeignKey References(string name) + 1 overload

        /// <summary>
        /// Sets the reference table name this foreign key links to
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBForeignKey References(string name)
        {
            return this.References(string.Empty, name);
        }

       
        /// <summary>
        /// Sets the reference table name and owner this Foreign Key links to.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBForeignKey References(string owner, string name)
        {
            this.ReferenceOwner = owner;
            this.ReferenceName = name;
            this._hasreference = true;
            return this;
        }

        #endregion


        #region public DBForeignKey OnDelete(DBFKAction action)

        /// <summary>
        /// Sets the action for the Foreign Key when a row is deleted.
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <returns></returns>
        public DBForeignKey OnDelete(DBFKAction action)
        {
            this.OnDeleteAction = action;
            return this;
        }

        #endregion

        #region  public DBForeignKey OnUpdate(DBFKAction action)

        /// <summary>
        /// Sets the action for the Foreign Key when one of the referenced fields is updated
        /// </summary>
        /// <param name="action">the action to perform</param>
        /// <returns></returns>
        public DBForeignKey OnUpdate(DBFKAction action)
        {
            this.OnUpdateAction = action;
            return this;
        }

        #endregion



    }



    /// <summary>
    /// Non abstract subclass that implements the serialization and deserialization
    /// </summary>
    internal class DBForeignKeyRef : DBForeignKey
    {


        protected override string XmlElementName
        {
            get { return XmlHelper.ForeignKey; }
        }

#if SILVERLIGHT
        // no statement building
#else

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginCreate(DBSchemaTypes.ForeignKey, string.Empty, this.Name, string.Empty, false);
            

            BuildColumnListStatement(builder, this.TableColumns);

            builder.BeginReferences(this.ReferenceOwner,this.ReferenceName);

            BuildColumnListStatement(builder, this.ReferenceColumns);

            builder.EndReferences(this.ReferenceOwner, this.ReferenceName);

            if (this.OnDeleteAction != DBFKAction.Undefined || this.OnUpdateAction != DBFKAction.Undefined)
            {
                builder.BeginForeignKeyUpdateActions();

                if (this.OnDeleteAction != DBFKAction.Undefined)
                {
                    builder.WriteDeleteAction(this.OnDeleteAction);
                }
                if (this.OnUpdateAction != DBFKAction.Undefined)
                {
                    builder.WriteUpdateAction(this.OnUpdateAction);
                }

                builder.EndForeignKeyUpdateActions();
            }

            builder.EndCreate(DBSchemaTypes.ForeignKey, false);
            return true;
        }

#endif

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.OnUpdateAction != DBFKAction.Undefined)
                this.WriteAttribute(writer, XmlHelper.UpdateAction, this.OnUpdateAction.ToString(), context);
            if (this.OnDeleteAction != DBFKAction.Undefined)
                this.WriteAttribute(writer, XmlHelper.DeleteAction, this.OnDeleteAction.ToString(), context);
            if(!string.IsNullOrEmpty(this.ReferenceOwner))
                this.WriteAttribute(writer,XmlHelper.ReferenceOwner,this.ReferenceOwner,context);
            if (!string.IsNullOrEmpty(this.ReferenceName))
                this.WriteAttribute(writer, XmlHelper.ReferenceTable, this.ReferenceName, context);

            return base.WriteAllAttributes(writer, context);
        }


        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if(IsAttributeMatch(XmlHelper.UpdateAction,reader,context))
            {
                this.OnUpdateAction = XmlHelper.ParseEnum<DBFKAction>(reader.Value);
                return true;
            }
            else if (IsAttributeMatch(XmlHelper.DeleteAction, reader, context))
            {
                this.OnDeleteAction = XmlHelper.ParseEnum<DBFKAction>(reader.Value);
                return true;
            }
            else if (IsAttributeMatch(XmlHelper.ReferenceOwner, reader, context))
            {
                this.ReferenceOwner = reader.Value;
                return true;
            }
            else if (IsAttributeMatch(XmlHelper.ReferenceTable, reader, context))
            {
                this.ReferenceName = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.TableColumns != null && this.TableColumns.Count > 0)
            {
                this.TableColumns.WriteReferenceXml(XmlHelper.ColumnList, writer, context);
            }
            if (this.ReferenceColumns != null && this.ReferenceColumns.Count > 0)
            {
                this.ReferenceColumns.WriteReferenceXml(XmlHelper.ReferenceColumns, writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsElementMatch(XmlHelper.ColumnList, reader, context))
            {
                DBColumnList cols = new DBColumnList();
                cols.ReadReferenceXml(reader, context);
                this.TableColumns = cols;
                return true;
            }
            else if (this.IsElementMatch(XmlHelper.ReferenceColumns, reader, context))
            {
                DBColumnList cols = new DBColumnList();
                cols.ReadReferenceXml(reader,context);
                this.ReferenceColumns = cols;
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }
    }

    public class DBForeignKeyList : DBClauseList<DBForeignKey>
    {
        public DBForeignKeyList()
            : base()
        {
        }
    }
}
