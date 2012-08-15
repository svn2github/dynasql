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
    public abstract class DBConstraint : DBStatement
    {


        #region internal string Name { get; protected set; }

        /// <summary>
        /// Gets the name of this Foreign Key
        /// </summary>
        public string Name { get; protected set; }

        #endregion


        #region protected virtual void BuildColumnListStatement(DBStatementBuilder builder, DBColumnList tooutput)

        /// <summary>
        /// Generates the SQL statement for a list of columns outputting only their names
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tooutput"></param>
        protected virtual void BuildColumnListStatement(DBStatementBuilder builder, DBColumnList tooutput)
        {
            builder.BeginBlock();
            bool first = true;
            foreach (DBColumn col in tooutput)
            {
                if (first)
                    first = false;
                else
                    builder.AppendReferenceSeparator();
                builder.WriteSourceField(string.Empty, string.Empty, col.Name, string.Empty);
            }
            builder.EndBlock();
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBPrimaryKey PrimaryKey()

        /// <summary>
        /// Creates a new unnamed PrimaryKey
        /// </summary>
        /// <returns></returns>
        public static DBPrimaryKey PrimaryKey()
        {
            return PrimaryKey(string.Empty);
        }

        #endregion

        #region  public static DBPrimaryKey PrimaryKey(string name)

        /// <summary>
        /// Creates a new PrimaryKey with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBPrimaryKey PrimaryKey(string name)
        {
            DBPrimaryKeyRef pkr = new DBPrimaryKeyRef();
            pkr.Name = name;
            return pkr;
        }

        #endregion

        #region public static DBForeignKey ForeignKey()

        /// <summary>
        /// Creates a new unnamed ForeignKey
        /// </summary>
        /// <returns></returns>
        public static DBForeignKey ForeignKey()
        {
            return ForeignKey(string.Empty);
        }

        #endregion

        #region  public static DBForeignKey ForeignKey(string name)

        /// <summary>
        /// Creates a new ForeignKey with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBForeignKey ForeignKey(string name)
        {
            DBForeignKeyRef fkr = new DBForeignKeyRef();
            fkr.Name = name;
            return fkr;
        }

        #endregion

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.Name))
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);

            return base.WriteAllAttributes(writer, context);
        }
    }


    public class DBConstraintList : DBClauseList<DBConstraint>
    {
    }
}
