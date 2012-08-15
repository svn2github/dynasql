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
    public abstract class DBDropViewQuery : DBDropQuery
    {
        public string ViewName { get; set; }

        public string ViewOwner { get; set; }

        protected DBDropViewQuery(string owner, string name)
        {
            this.ViewOwner = owner;
            this.ViewName = name;
        }

        /// <summary>
        /// Adds a check exists operation to the statement so no errors are raised.
        /// </summary>
        /// <returns></returns>
        public DBDropViewQuery IfExists()
        {
            this.CheckExists = DBExistState.Exists;
            return this;
        }

        //
        // static factory methods
        //

        /// <summary>
        /// Creates a new DROPW VIEW statement
        /// </summary>
        /// <returns></returns>
        public static DBDropViewQuery DropView()
        {
            return DropView(string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a new DROP VIEW statement for the view with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropViewQuery DropView(string name)
        {
            return DropView(string.Empty, name);
        }

        /// <summary>
        /// Creates a new DROP VIEW statement for the view with the specified owner.name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropViewQuery DropView(string owner, string name)
        {
            DBDropViewQueryRef drop = new DBDropViewQueryRef(owner, name);
            return drop;
        }

    }

    internal class DBDropViewQueryRef : DBDropViewQuery
    {
        internal DBDropViewQueryRef(string owner, string name)
            : base(owner, name)
        {
        }

        protected override string XmlElementName
        {
            get { return XmlHelper.DropView; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDropStatement(DBSchemaTypes.View, this.ViewOwner, this.ViewName, this.CheckExists == DBExistState.Exists);
            builder.EndDrop(DBSchemaTypes.View, this.CheckExists == DBExistState.Exists);

            return true;
        }

        

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.ViewName = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.ViewOwner = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteOptionalAttribute(writer, XmlHelper.Name, this.ViewName, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Owner, this.ViewOwner, context);

            return base.WriteAllAttributes(writer, context);
        }
    }
}
