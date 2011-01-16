/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Defines a SET x = [value] statement
    /// </summary>
    public abstract class DBSet : DBStatement
    {
        private DBAssign _assign;

        /// <summary>
        /// The Assignment that is set
        /// </summary>
        public DBAssign Assignment
        {
            get { return this._assign; }
            protected set { this._assign = value; }
        }

        /// <summary>
        /// Creates and returns a new DBSet assignment
        /// </summary>
        /// <param name="assign"></param>
        /// <returns></returns>
        public static DBSet Set(DBAssign assign)
        {
            DBSet set = new DBSetRef();
            set.Assignment = assign;

            return set;
        }
    }

    internal class DBSetRef : DBSet
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.Set; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginSetStatement();
            this.Assignment.BuildStatement(builder);
            builder.EndSetStatement();
            return true;
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.Assignment.WriteXml(writer, context);
            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {

            if (this.IsElementMatch(XmlHelper.Assign, reader, context))
            {
                this.Assignment = context.Factory.Read(XmlHelper.Assign, reader, context) as DBAssign;
                return this.Assignment != null;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }
    }
}
