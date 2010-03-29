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
    public abstract class DBTop : DBClause
    {
        //
        // properties
        //

        #region public double TopValue {get;set;}

        private double _val;

        public double TopValue
        {
            get { return _val; }
            set { _val = value; }
        }

        #endregion

        #region public TopType Type {get;set;}

        private TopType _type;

        public TopType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBTop Number(int count)

        public static DBTop Number(int count)
        {
            DBTop top = new DBTopRef();
            top.TopValue = count;
            top.Type = TopType.Count;
            return top;
        }

        #endregion

        #region public static DBTop Percent(double percent)

        public static DBTop Percent(double percent)
        {
            DBTop top = new DBTopRef();
            top.TopValue = percent;
            top.Type = TopType.Percent;
            return top;
        }

        #endregion

        #region public static DBTop Top(double value, TopType type)

        public static DBTop Top(double value, TopType type)
        {
            DBTop top = new DBTopRef();
            top.Type = type;
            top.TopValue = value;

            return top;
        }

        #endregion

        #region internal static DBClause Top()

        internal static DBClause Top()
        {
            DBTopRef top = new DBTopRef();
            return top;
        }

        #endregion

    }

    public class DBTopRef : DBTop
    {

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Top; }
        }

        #endregion

        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this.TopValue > 0.0)
            {
                builder.WriteTop(this.TopValue, this.Type);
                return true;
            }
            else
                return false;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            if (this.IsAttributeMatch(XmlHelper.TopValue, reader, context))
                this.TopValue = int.Parse(reader.Value);
            else if (this.IsAttributeMatch(XmlHelper.TopType, reader, context))
                this.Type = (TopType)Enum.Parse(typeof(TopType), reader.Value);
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
    
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.TopValue, this.TopValue.ToString(), context);
            this.WriteAttribute(writer, XmlHelper.TopType, this.Type.ToString(), context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

    }
}
