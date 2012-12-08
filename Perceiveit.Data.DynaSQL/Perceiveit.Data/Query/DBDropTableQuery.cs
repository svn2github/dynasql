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
    public abstract class DBDropTableQuery : DBDropQuery
    {

        public string TableName { get; set; }

        public string TableOwner { get; set; }

        public bool Temporary { get; set; }

        protected DBDropTableQuery(string owner, string table)
        {
            this.TableName = table;
            this.TableOwner = owner;
        }

        /// <summary>
        /// Performs a check to ensure that this item exists before it is dropped
        /// </summary>
        /// <returns></returns>
        public DBDropTableQuery IfExists()
        {
            this.CheckExists = DBExistState.Exists;
            return this;
        }

        //
        // factory methods
        //

        #region public static DBDropTableQuery DropTable(string tableName) + 3 overloads

        /// <summary>
        /// Creates a new DROP TABLE method - specify the name separately
        /// </summary>
        /// <returns></returns>
        public static DBDropTableQuery DropTable()
        {
            return DropTable(string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a new DROP TABLE method that will remove the specified table from the default schema
        /// </summary>
        /// <param name="tableName">The table to drop</param>
        /// <returns></returns>
        public static DBDropTableQuery DropTable(string tableName)
        {
            return DropTable(string.Empty, tableName);
        }

        /// <summary>
        /// Creates a new DROP TABLE method that will remove the specified owner.table from the database
        /// </summary>
        /// <param name="owner">The table onwner</param>
        /// <param name="tableName">The table name</param>
        /// <returns></returns>
        public static DBDropTableQuery DropTable(string owner, string tableName)
        {
            return new DBDropTableQueryRef(owner, tableName);
        }

        /// <summary>
        /// Creates a new DROP TABLE method that will remove the specified table reference from the database
        /// </summary>
        /// <param name="tbl">The table to drop</param>
        /// <returns></returns>
        public static DBDropTableQuery DropTable(DBTable tbl)
        {
            return DropTable(tbl.Name, tbl.Owner);
        }

        #endregion

    }

    internal class DBDropTableQueryRef : DBDropTableQuery
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.DropTable; }
        }



        internal DBDropTableQueryRef(string owner, string table)
            : base(owner, table)
        {
        }


#if SILVERLIGHT
        // no statement building
#else


        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDropStatement(DBSchemaTypes.Table, this.TableOwner, this.TableName, this.CheckExists == DBExistState.Exists);
            builder.EndDrop(DBSchemaTypes.Table, this.CheckExists == DBExistState.Exists);

            return true;
        }

#endif

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.TableOwner = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.TableName = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Temp, reader, context))
            {
                bool temp;
                if (bool.TryParse(reader.Value, out temp))
                    this.Temporary = temp;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteOptionalAttribute(writer, XmlHelper.Name, this.TableName, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Owner, this.TableOwner, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Temp, this.Temporary.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }
    }
}
