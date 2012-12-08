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
    public abstract class DBCreateViewQuery : DBCreateQuery
    {
        #region public string ViewName { get; set; }

        /// <summary>
        /// Gets or sets the name of the view
        /// </summary>
        public string ViewName { get; set; }

        #endregion

        #region public string ViewOwner { get; set; }

        /// <summary>
        /// Gets or sets the schema owner of the view
        /// </summary>
        public string ViewOwner { get; set; }

        #endregion

        #region public DBStatement Select { get; set; }

        /// <summary>
        /// Gets or sets the SELECT statement for the view
        /// </summary>
        public DBStatement Select { get; set; }

        #endregion
        
        //
        // ctor
        //

        #region internal DBCreateViewQuery(string owner, string name)

        internal DBCreateViewQuery(string owner, string name)
        {
            this.ViewName = name;
            this.ViewOwner = owner;
        }

        #endregion

        //
        // instance methods
        //

        public DBCreateViewQuery As(DBStatement select)
        {
            this.Select = select;
            return this;
        }

        public DBCreateViewQuery IfNotExists()
        {
            this.CheckExists = DBExistState.NotExists;
            return this;
        }

        //
        // static factory method
        //

        #region public static DBCreateViewQuery CreateView() + 2 overloads

        /// <summary>
        /// Creates a new view
        /// </summary>
        /// <returns></returns>
        public static DBCreateViewQuery CreateView()
        {
            return CreateView(string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a new view with the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateViewQuery CreateView(string name)
        {
            return CreateView(string.Empty, name);
        }

        /// <summary>
        /// Creates a new view with the owner and name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateViewQuery CreateView(string owner, string name)
        {
            return new DBCreateViewQueryRef(owner, name);
        }

        #endregion

    }


    internal class DBCreateViewQueryRef : DBCreateViewQuery
    {
        internal DBCreateViewQueryRef(string name, string owner)
            : base(name, owner)
        {
        }

        protected override string XmlElementName
        {
            get { return XmlHelper.CreateView; }
        }

#if SILVERLIGHT
        // no statement building
#else

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            bool check = (this.CheckExists == DBExistState.NotExists);

            builder.BeginCreate(DBSchemaTypes.View,this.ViewOwner, this.ViewName, string.Empty, check);
            builder.BeginEntityDefinition();
            this.Select.BuildStatement(builder);
            builder.EndEntityDefinition();
            builder.EndCreate(DBSchemaTypes.View, check);
            return true;
        }

#endif

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.ViewName))
                this.WriteAttribute(writer, XmlHelper.Name, this.ViewName, context);
            if (!string.IsNullOrEmpty(this.ViewOwner))
                this.WriteAttribute(writer, XmlHelper.Owner, this.ViewOwner, context);

            return base.WriteAllAttributes(writer, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (null != this.Select)
                this.Select.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.ViewName = reader.Value;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.ViewOwner = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBClause c = this.ReadNextInnerClause(reader.Name, reader, context);
            if (c is DBStatement)
            {
                this.Select = c as DBStatement;
                return true;
            }
            else
                return false;
        }
    }
}
