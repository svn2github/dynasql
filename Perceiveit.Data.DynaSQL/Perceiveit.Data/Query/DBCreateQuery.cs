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
using System.Data;

namespace Perceiveit.Data.Query
{
    public abstract class DBCreateQuery : DBQuery
    {

        public DBExistState CheckExists { get; protected set; }


        public DBCreateQuery()
            : base()
        {
            this.CheckExists = DBExistState.Unknown;
        }


        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsAttributeMatch(XmlHelper.CheckExists, reader, context))
            {
                this.CheckExists = (DBExistState)Enum.Parse(typeof(DBExistState), reader.Value);
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if(this.CheckExists != DBExistState.Unknown)
                this.WriteAttribute(writer, XmlHelper.CheckExists, this.CheckExists.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }
        
    }
}
