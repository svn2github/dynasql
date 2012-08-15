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
    public abstract class DBDropIndexQuery : DBDropQuery
    {
        public string Owner { get; set; }

        public string Name { get; set; }

        public string TableOwner { get; set; }

        public string TableName { get; set; }


        public DBDropIndexQuery IfExists()
        {
            this.CheckExists = DBExistState.Exists;
            return this;
        }

        public DBDropIndexQuery On(string tablename)
        {
            this.TableName = tablename;
            return this;
        }


        public DBDropIndexQuery On(string tableowner, string tablename)
        {
            this.TableName = tablename;
            this.TableOwner = tableowner;
            return this;
        }

        //
        // static factory methods
        //

        public static DBDropIndexQuery DropIndex()
        {
            return DropIndex(string.Empty, string.Empty);
        }

        public static DBDropIndexQuery DropIndex(string name)
        {
            return DropIndex(string.Empty, name);
        }

        public static DBDropIndexQuery DropIndex(string owner, string name)
        {
            DBDropIndexQuery q = new DBDropIndexQueryRef();
            q.Owner = owner;
            q.Name = name;
            return q;
        }
    }

    internal class DBDropIndexQueryRef : DBDropIndexQuery
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.DropIndex; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDropStatement(DBSchemaTypes.Index, this.Owner, this.Name, this.CheckExists == DBExistState.Exists);
            if (!string.IsNullOrEmpty(this.TableName))
            {
                builder.BeginReferenceOn();
                builder.WriteSourceTable(this.TableOwner, this.TableName, string.Empty);
                builder.EndReferenceOn();
            }
            builder.EndDrop(DBSchemaTypes.Index, this.CheckExists == DBExistState.Exists);
            return true;
        }

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
            else if (this.IsAttributeMatch(XmlHelper.TableName, reader, context))
            {
                this.TableName = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.TableOwner, reader, context))
            {
                this.TableOwner = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteOptionalAttribute(writer, XmlHelper.Name, this.Name, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Owner, this.Owner, context);
            this.WriteOptionalAttribute(writer, XmlHelper.TableName, this.TableName, context);
            this.WriteOptionalAttribute(writer, XmlHelper.TableOwner, this.TableOwner, context);

            return base.WriteAllAttributes(writer, context);
        }
    }
}
