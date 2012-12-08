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
    internal class DBOrderSet : DBExpressionSet, IDBBoolean
    {
        private DBOrderList _orders;

        //
        // properties
        //

        #region public DBOrderList OrderList {get;}

        public DBOrderList OrderList
        {
            get {
                if (_orders == null)
                    _orders = new DBOrderList();
                return _orders; 
            }
        }

        #endregion

        #region public bool HasOrderBy {get;}

        public bool HasOrderBy
        {
            get
            {
                return this._orders != null && this._orders.Count > 0;
            }
        }

        #endregion


#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {

            if (this._orders == null || this._orders.Count == 0)
                return false;
            else
            {
                this.OrderList.BuildStatement(builder);
                return true;
            }
        }

        #endregion

#endif

        //
        // xml serialization
        //

        #region protected override string XmlElementName

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.Order;
            }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasOrderBy)
                this.OrderList.WriteXml(writer, context);

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            this.OrderList.ReadXml(reader.LocalName, reader, context);
            return this.OrderList.Count > 0;
        }

        #endregion

        //
        // static factory methods
        //

        #region internal static DBOrderSet OrderBy() + 3 overloads

        internal static DBOrderSet OrderBy()
        {
            return new DBOrderSet();
        }

        internal static DBOrderSet OrderBy(string field)
        {
            return OrderBy(field, Order.Default);
        }

        internal static DBOrderSet OrderBy(DBClause value)
        {
            return OrderBy(value, Order.Default);
        }

        internal static DBOrderSet OrderBy(string field, Order order)
        {
            DBField fld = DBField.Field(field);
            return OrderBy(fld, order);
        }

        internal static DBOrderSet OrderBy(DBClause value, Order order)
        {
            DBOrder oref = DBOrder.OrderBy(order, value);
            
            DBOrderSet set = new DBOrderSet();
            set.OrderList.Add(oref);
            set.Last = oref;

            return set;
        }

        #endregion

        //
        // interface implementations
        // 

        #region IDBBoolean Members (OR not supported)

        
        public DBClause And(DBClause reference)
        {

            DBOrder order;
            if(reference is DBOrder)
                order = (DBOrder)reference;
            else
                order = DBOrder.OrderBy(Order.Default, reference);
            this.OrderList.Add(order);
            this.Last = order;
            return order;
        }

        DBClause IDBBoolean.Or(DBClause reference)
        {
            throw new InvalidOperationException("Cannot operate the OR operation on an order set.");
        }

        #endregion
    }

}
