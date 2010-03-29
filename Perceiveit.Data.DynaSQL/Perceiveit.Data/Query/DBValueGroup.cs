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
    public abstract class DBValueGroup : DBClause
    {

        private DBClauseList _items;

        //
        // properties
        //

        #region public DBClauseList GroupItems {get;set;}

        public DBClauseList GroupItems
        {
            get 
            {
                return _items; 
            }
            set { _items = value; }
        }

        #endregion

        //
        // ctors
        //

        #region protected DBValueGroup()

        protected DBValueGroup()
        { }

        #endregion

        //
        // methods
        //

        #region public DBValueGroup Add(DBClause item)

        public DBValueGroup Add(DBClause item)
        {
            if (this.GroupItems == null)
                this.GroupItems = new DBClauseList();

            this.GroupItems.Add(item);

            return this;
        }

        #endregion

        #region public DBValueGroup AddRange(IEnumerable<DBClause> items)

        public DBValueGroup AddRange(IEnumerable<DBClause> items)
        {
            if (this.GroupItems == null)
                this.GroupItems = new DBClauseList();
            if (null != items)
            {
                foreach (DBClause c in items)
                {
                    this.GroupItems.Add(c);
                }
            }

            return this;
        }

        #endregion

        //
        // static methods
        //

        #region public static DBValueGroup Empty()

        public static DBValueGroup Empty()
        {
            DBClauseList list = new DBClauseList();
            
            DBValueGroupRef values = new DBValueGroupRef();
            values.GroupItems = null;

            return values;
        }

        #endregion

        #region public static DBValueGroup All(params DBClause[] items) + 3 overloads

        public static DBValueGroup All(params DBClause[] items)
        {
            DBClauseList list = new DBClauseList();
            list.AddRange(items);
            DBValueGroupRef values = new DBValueGroupRef();
            values.GroupItems = list;

            return values;
        }

        public static DBValueGroup All(params int[] items)
        {
            DBClause[] all = ConvertToClause(items);
            
            return All(all);
        }

        public static DBValueGroup All(params string[] items)
        {
            DBClause[] all = ConvertToClause(items);

            return All(all);
        }

        public static DBValueGroup All(params Guid[] items)
        {
            DBClause[] all = ConvertToClause(items);

            return All(all);
        }

        public static DBValueGroup All(params double[] items)
        {
            DBClause[] all = ConvertToClause(items);

            return All(all);
        }

        private static DBClause[] ConvertToClause(Array items)
        {
            List<DBClause> converted = new List<DBClause>();
            foreach (object item in items)
            {
                converted.Add(DBConst.Const(item));
            }
            return converted.ToArray();
        }

        #endregion
    }

    internal class DBValueGroupRef : DBValueGroup
    {
        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginBlock();
            this.GroupItems.BuildStatement(builder);
            builder.EndBlock();
            return true;
        }

        #endregion

        //
        // XML Serializer
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.ValueGroup; }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.GroupItems != null)
            {
                this.GroupItems.WriteXml(writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.GroupItems == null)
                this.GroupItems = new DBClauseList();

            DBClause c = context.Factory.Read(reader.Name, reader, context);
            
            if (null != c)
            {
                this.GroupItems.Add(c);
                b = true;
            }
            else
                b = false;

            return b;
        }

        #endregion


    }
}
