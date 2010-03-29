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
    public class DBTokenList : List<DBToken>
    {
        public virtual string ClauseSeparator { get { return ", "; } }

        public virtual string ListStart { get { return ""; } }
        public virtual string ListEnd { get { return ""; } }

        public virtual bool BuildStatement(DBStatementBuilder builder)
        {
            return this.BuildStatement(builder, false, false);
        }

        public virtual bool BuildStatement(DBStatementBuilder builder, bool eachOnNewLine, bool indent)
        {
            if (this.Count > 0)
            {
                bool outputSeparator = false;
                int count = 0;

                builder.WriteRaw(this.ListStart);

                for (int i = 0; i < this.Count; i++)
                {
                    DBToken clause = this[i];

                    if (outputSeparator)
                    {
                        builder.AppendReferenceSeparator();

                        if (eachOnNewLine)
                            builder.BeginNewLine();
                        count++;
                    }
                    outputSeparator = clause.BuildStatement(builder);
                }

                builder.WriteRaw(this.ListEnd);

                return count > 0;
            }
            else
                return false;
        }

        
    }

    public class DBTokenList<T> : DBTokenList where T : DBToken
    {
        
    }

    public class DBClauseList : DBTokenList<DBClause>
    {
        public bool WriteXml(System.Xml.XmlWriter xmlWriter, XmlWriterContext context)
        {
            if (this.Count > 0)
            {
                foreach (DBClause tkn in this)
                {
                    tkn.WriteXml(xmlWriter, context);
                }
                return true;
            }
            else
                return false;
        }


        public bool ReadXml(string endElement, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool isEmpty = reader.IsEmptyElement && XmlHelper.IsElementMatch(endElement, reader, context);

            do
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    

                    DBClause c = context.Factory.Read(reader.LocalName, reader, context);
                    if (c != null)
                        this.Add(c);

                    if (isEmpty)
                        return true;
                }

                if (reader.NodeType == System.Xml.XmlNodeType.EndElement && XmlHelper.IsElementMatch(endElement, reader, context))
                    break;
            }
            while (reader.Read());

            return true;
            
        }
    }

    public class DBClauseList<T> : DBClauseList where T : DBClause
    {
    }
}
