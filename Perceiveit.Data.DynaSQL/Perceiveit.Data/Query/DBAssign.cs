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
    /// Creates a new Assignment (X = Y) clause. 
    /// Used specifically in UPDATE operations (SET field1 = param1, field2 = param2 etc.)
    /// </summary>
    public abstract class DBAssign : DBClause
    {

        #region public DBClause Item {get;set;}

        private DBClause _item;
        /// <summary>
        /// Gets or sets the receiver of the value
        /// </summary>
        public DBClause Item
        {
            get { return _item; }
            set { this._item = value; }
        }

        #endregion

        #region public DBClause ToValue {get; set;}

        private DBClause _toval;
        /// <summary>
        /// Gets or sets the value to be assigned
        /// </summary>
        public DBClause ToValue
        {
            get { return _toval; }
            set { this._toval = value; }
        }

        #endregion

        //
        // .ctor
        //

        #region protected DBAssign()

        /// <summary>
        /// To support inherited constructors
        /// </summary>
        protected DBAssign()
        {
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBAssign Set(DBClause item, DBClause toValue)

        /// <summary>
        /// Creates a new Assignment clause
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toValue"></param>
        /// <returns></returns>
        public static DBAssign Set(DBClause item, DBClause toValue)
        {
            DBAssignRef aref = new DBAssignRef();
            aref._item = item;
            aref._toval = toValue;

            return aref;
        }

        #endregion

        #region internal static DBAssign Assign()

        /// <summary>
        /// Creates a new Empty assignment
        /// </summary>
        /// <returns></returns>
        internal static DBAssign Assign()
        {
            DBAssignRef aref = new DBAssignRef();
            return aref;
        }

        #endregion
    }


    internal class DBAssignRef : DBAssign
    {
        //
        // .ctor(s)
        //

        #region public DBAssignRef()

        public DBAssignRef()
        {
        }

        #endregion

        //
        // SQL Statement 
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginAssignValue();
            this.Item.BuildStatement(builder);
            builder.WriteOperator(Operator.Equals);
            this.ToValue.BuildStatement(builder);
            builder.EndAssignValue();

            return true;
        }

        #endregion

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Assign; }
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Item != null)
            {
                this.WriteStartElement(XmlHelper.Item, writer, context);
                this.Item.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.Item, writer, context);
            }

            if (this.ToValue != null)
            {
                this.WriteStartElement(XmlHelper.Value, writer, context);
                this.ToValue.WriteXml(writer,context);
                this.WriteEndElement(XmlHelper.Value, writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsElementMatch(XmlHelper.Item, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.Item = this.ReadNextInnerClause(XmlHelper.Item, reader, context);
                b = true;
            }
            else if (this.IsElementMatch(XmlHelper.Value, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.ToValue = this.ReadNextInnerClause(XmlHelper.Value, reader, context);
                b = true;
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion


    }
}
