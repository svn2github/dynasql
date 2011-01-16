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
    /// Defines a single order by clause
    /// </summary>
    public abstract class DBOrder : DBClause
    {
        //
        // public properties
        //

        #region public DBClause Clause {get;set;}

        private DBClause _clases;
        /// <summary>
        /// Gets or sets the order by XXX clause
        /// </summary>
        public DBClause Clause
        {
            get { return _clases; }
            set { _clases = value; }
        }

        #endregion

        #region public Order Order {get;set;}

        private Order _order;
        /// <summary>
        /// Gets or sets the direction of sort
        /// </summary>
        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBOrder OrderBy(Order order, DBClause clause)
        /// <summary>
        /// Creates a new OrderBy clause
        /// </summary>
        /// <param name="order"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public static DBOrder OrderBy(Order order, DBClause clause)
        {
            DBOrderRef orderC = new DBOrderRef();
            orderC.Order = order;
            orderC.Clause = clause;
            return orderC;
        }

        #endregion

        #region internal static DBClause OrderBy()
        /// <summary>
        /// Creates a new empty order by clause
        /// </summary>
        /// <returns></returns>
        internal static DBClause OrderBy()
        {
            return OrderBy(Order.Default, null);
        }

        #endregion
    }

    internal class DBOrderRef : DBOrder
    {
        //
        // SQL Statement builder
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)
        
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (Clause == null)
                return false;

            builder.BeginOrderClause(this.Order);
            this.Clause.BuildStatement(builder);
            builder.EndOrderClause(this.Order);

            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}
        
        protected override string XmlElementName
        {
            get { return XmlHelper.OrderBy; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.SortBy, this.Order.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {

            if (this.Clause != null)
            {
                this.Clause.WriteXml(writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;

            if (this.IsAttributeMatch(XmlHelper.SortBy, reader, context))
            {
                this.Order = (Order)Enum.Parse(typeof(Order), reader.Value, true);
                b = true;
            }
            else 
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            this.Clause = context.Factory.Read(reader.Name, reader, context);
            return true;
        }

        #endregion

    }

    internal class DBOrderList : DBClauseList<DBOrder>
    {
    }
}
