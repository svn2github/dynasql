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
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Represents a DECLARE statement in a script
    /// </summary>
    public abstract class DBDeclaration : DBStatement
    {
        private DBParam _param;

        /// <summary>
        /// Gets the Parameter reference this instance is declaring
        /// </summary>
        public DBParam Parameter { get { return _param; } protected set { _param = value; } }

        /// <summary>
        /// Creates and returns a new declaration of the parameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DBDeclaration Declare(DBParam param)
        {
            DBDelcarationRef dref = new DBDelcarationRef();
            dref.Parameter = param;
            return dref;
        }

        public static DBDeclaration Declare()
        {
            DBDelcarationRef dref = new DBDelcarationRef();
            return dref;
        }
    }

    internal class DBDelcarationRef : DBDeclaration
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.Declare; }
        }

#if SILVERLIGHT
        // no statement building
#else
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDeclareStatement(this.Parameter);
            builder.EndDeclareStatement();
            return true;
        }


#endif
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            bool b = base.WriteInnerElements(writer, context);
            this.Parameter.WriteXml(writer, context);
            return b;
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {

            DBClause c = this.ReadNextInnerClause(this.XmlElementName, reader, context);
            if (c is DBParam)
            {
                DBParam p = (DBParam)c;
                if (context.Parameters.Contains(p.Name))
                    p = context.Parameters[p.Name];
                else
                    context.Parameters.Add(p);
                this.Parameter = p;
                return true;
            }
            else
                return false;
        }
    }
}
