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
    public abstract class DBDropProcedureQuery : DBDropQuery
    {

        /// <summary>
        /// Gets or sets the name of the stored procedure to drop.
        /// </summary>
        public string SprocName { get; set; }

        /// <summary>
        /// Gets or sets the owner of the stored procedure to drop
        /// </summary>
        public string SprocOwner { get; set; }


        /// <summary>
        /// Adds a check exists statement to the query
        /// </summary>
        /// <returns></returns>
        public DBDropProcedureQuery IfExists()
        {
            this.CheckExists = DBExistState.Exists;
            return this;
        }

        //
        // static factory methods
        //

        /// <summary>
        /// Creates a new DROP PROCEDURE statement
        /// </summary>
        /// <returns></returns>
        public static DBDropProcedureQuery DropProcedure()
        {
            return DropProcedure(string.Empty, string.Empty);
        }

        /// <summary>
        /// creates and returns a new DROP PROCEDURE statement with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropProcedureQuery DropProcedure(string name)
        {
            return DropProcedure(string.Empty, name);
        }

        /// <summary>
        /// creates and returns a new DROP PROCEDURE statement with the specified owner.name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropProcedureQuery DropProcedure(string owner, string name)
        {
            DBDropProcedureQuery q = new DBDropProcedureQueryRef();
            q.SprocName = name;
            q.SprocOwner = owner;
            return q;
        }
    }


    internal class DBDropProcedureQueryRef : DBDropProcedureQuery
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.DropSproc; }
        }


        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDropStatement(DBSchemaTypes.StoredProcedure, this.SprocOwner, this.SprocName, this.CheckExists == DBExistState.Exists);
            builder.EndDrop(DBSchemaTypes.Table, this.CheckExists == DBExistState.Exists);

            return true;
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.SprocName = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.SprocOwner = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteOptionalAttribute(writer, XmlHelper.Name, this.SprocName, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Owner, this.SprocOwner, context);

            return base.WriteAllAttributes(writer, context);
        }

    }
}
