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
    public abstract class DBCreateIndexQuery : DBCreateQuery
    {
        //
        // properties
        //


        /// <summary>
        /// Gets or sets the schema owner of this index
        /// </summary>
        protected internal string Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of this index
        /// </summary>
        protected internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the Table this Index is on
        /// </summary>
        protected internal string TableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the Schema Owner for the table this index is on
        /// </summary>
        protected internal string TableOwner { get; set; }

        /// <summary>
        /// Gets the list of columns (and their ordering) in this index
        /// </summary>
        protected internal DBOrderList ColumnOrders { get; protected set; }

        /// <summary>
        /// True if this index is constrained to be unique
        /// </summary>
        protected internal CreateOptions Options { get; set; }

        //
        // ctor
        //

        /// <summary>
        /// </summary>
        protected DBCreateIndexQuery()
        {
            this.ColumnOrders = new DBOrderList();
            Options = CreateOptions.None;
        }

        //
        // instance methods
        //
        
        /// <summary>
        /// Adds the unique constraint to this index
        /// </summary>
        /// <returns></returns>
        public DBCreateIndexQuery Unique()
        {
            this.Options |= CreateOptions.Unique;
            return this;
        }

        /// <summary>
        /// Adds the Clustered constraint to the index
        /// </summary>
        /// <returns></returns>
        public DBCreateIndexQuery Clustered()
        {
            this.Options |=  CreateOptions.Clustered;
            return this;
        }

        public DBCreateIndexQuery NonClustered()
        {
            this.Options |= CreateOptions.NonClustered;
            return this;
    }

        /// <summary>
        /// Adds an execution check step to the query so that it will only create 
        /// the index if it does not exist.
        /// </summary>
        /// <returns></returns>
        public DBCreateIndexQuery IfNotExists()
        {
            this.CheckExists = DBExistState.NotExists;
            return this;
        }

        /// <summary>
        /// Specifies the table the index should be created on.
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DBCreateIndexQuery On(string tablename)
        {
            return this.On(string.Empty, tablename);
        }

        /// <summary>
        /// Specifies the list of columns to use within the last index 
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public DBCreateIndexQuery On(string tableowner, string tablename)
        {
            this.TableName = tablename;
            this.TableOwner = tableowner;
            return this;
        }

        /// <summary>
        /// Adds a column to the index with default ordering.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public DBCreateIndexQuery Add(string column)
        {
            return this.Add(column, Order.Default);
        }

        /// <summary>
        /// Adds a column to the index with the ordering specified
        /// </summary>
        /// <param name="column"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DBCreateIndexQuery Add(string column, Order order)
        {
            this.ColumnOrders.Add(DBOrder.OrderBy(order, DBField.Field(column)));
            return this;
        }

        /// <summary>
        /// Adds a column to the  index with the ordering specified
        /// </summary>
        /// <param name="column"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DBCreateIndexQuery Columns(string column, Order order)
        {
            this.ColumnOrders.Add(DBOrder.OrderBy(order, DBField.Field(column)));
            return this;
        }

        /// <summary>
        /// Adds 2 columns to the index with the ordering as specified
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="order1"></param>
        /// <param name="column2"></param>
        /// <param name="order2"></param>
        /// <returns></returns>
        public DBCreateIndexQuery Columns(string column1, Order order1, string column2, Order order2)
        {
            this.ColumnOrders.Add(DBOrder.OrderBy(order1, DBField.Field(column1)));
            this.ColumnOrders.Add(DBOrder.OrderBy(order2, DBField.Field(column2)));

            return this;
        }

        /// <summary>
        /// Adds the columns to the index with their default ordering
        /// </summary>
        /// <param name="columnlist"></param>
        /// <returns></returns>
        public DBCreateIndexQuery Columns(params string[] columnlist)
        {
            if (null != columnlist)
            {
                foreach (string col in columnlist)
                {
                    this.ColumnOrders.Add(DBOrder.OrderBy(Order.Default, DBField.Field(col)));
                }
            }
            return this;
        }

        //
        // static factory methods
        //

        #region public static DBCreateIndexQuery Index(string name) + 1 overload

        
        /// <summary>
        /// Creates a new index
        /// </summary>
        /// <returns></returns>
        public static DBCreateIndexQuery Index()
        {
            return Index(string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a new index with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateIndexQuery Index(string name)
        {
            return Index(string.Empty, name);
        }

        

        /// <summary>
        /// Creates a new Index with the specified name and owner.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateIndexQuery Index(string owner, string name)
        {
            DBCreateIndexQuery idx = new DBCreateIndexQueryRef();
            idx.Name = name;
            idx.Owner = owner;
            return idx;
        }

        #endregion
    }


    internal class DBCreateIndexQueryRef : DBCreateIndexQuery
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.CreateIndex; }
        }

        #if SILVERLIGHT
        // no statement building
#else

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            string opts = GetConstraints(builder);

            builder.BeginCreate(DBSchemaTypes.Index, this.Owner, this.Name, opts, this.CheckExists == DBExistState.NotExists);
            builder.BeginReferenceOn();
            builder.WriteSourceTable(this.TableOwner, this.TableName, string.Empty);
            builder.EndReferenceOn();
            builder.BeginBlock();
            this.ColumnOrders.BuildStatement(builder);
            builder.EndBlock();
            builder.EndCreate(DBSchemaTypes.Index, this.CheckExists == DBExistState.NotExists);
            return true;
        }

        private string GetConstraints(DBStatementBuilder builder)
        {
            string unique = string.Empty;

            if (this.Options != CreateOptions.None)
                unique = builder.GetCreateOption(this.Options);
            else
                unique = string.Empty;

            return unique;
        }

#endif

        //
        // xml serialization
        //

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
                b = true;
            }
            else if (IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                b = true;
            }
            else if (IsAttributeMatch(XmlHelper.TableName, reader, context))
            {
                this.TableName = reader.Value;
                b = true;
            }
            else if (IsAttributeMatch(XmlHelper.TableOwner, reader, context))
            {
                this.TableOwner = reader.Value;
                b = true;
            }
            else if (IsAttributeMatch(XmlHelper.IndexOptions, reader, context))
            {
                CreateOptions value;
                if (string.IsNullOrEmpty(reader.Value))
                    value = CreateOptions.None;
                else
                    value = XmlHelper.ParseEnum<CreateOptions>(reader.Value);
                this.Options = value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsElementMatch(XmlHelper.IndexColumns, reader, context))
            {
                reader.Read();
                DBOrderList list = new DBOrderList();
                list.ReadXml(XmlHelper.IndexColumns, reader, context);
                this.ColumnOrders = list;
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.Name))
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);
            if (!string.IsNullOrEmpty(this.Owner))
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);
            if (!string.IsNullOrEmpty(this.TableName))
                this.WriteAttribute(writer, XmlHelper.TableName, this.TableName, context);
            if (!string.IsNullOrEmpty(this.TableOwner))
                this.WriteAttribute(writer, XmlHelper.TableOwner, this.TableOwner, context);
            if (this.Options != CreateOptions.None)
                this.WriteAttribute(writer, XmlHelper.IndexOptions, this.Options.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.ColumnOrders != null && this.ColumnOrders.Count > 0)
            {
                this.WriteStartElement(XmlHelper.IndexColumns, writer, context);
                this.ColumnOrders.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.IndexColumns, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }
    }

}
