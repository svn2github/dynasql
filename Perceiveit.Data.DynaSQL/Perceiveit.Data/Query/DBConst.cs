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
using System.Data;

namespace Perceiveit.Data.Query
{
    public abstract class DBConst : DBCalculableClause
    {
        //
        // propterties
        //

        #region public object Value

        private object _val;

        public object Value
        {
            get { return _val; }
            protected set { _val = value; }
        }

        #endregion

        #region public DbType Type

        private DbType _type = DbType.Object;

        public DbType Type
        {
            get { return _type; }
            protected set { _type = value; }
        }

        #endregion

        #region public string Alias
        
        private string _alias;

        /// <summary>
        /// Gets the alias name of this constant
        /// </summary>
        public string Alias 
        {
            get { return this._alias; }
            protected set { this._alias = value; }
        }

        #endregion

        //
        // static methods
        //

        #region public static DBConst Const(DbType type, object value)

        public static DBConst Const(DbType type, object value)
        {
            DBConst c = new DBConstRef();
            c.Type = type;
            c.Value = value;

            return c;
        }

        #endregion

        #region public static DBConst Const(object value)

        public static DBConst Const(object value)
        {
            return Const(GetDbType(value), value);
        }

        #endregion

        #region protected static System.Data.DbType GetDbType(object val)

        protected static System.Data.DbType GetDbType(object val)
        {
            System.Data.DbType type = DBHelper.GetDBTypeForObject(val);
            return type;
        }

        
        #endregion

        #region public static DBConst Int32(int value)

        public static DBConst Int32(int value)
        {
            return Const(DbType.Int32, value);
        }

        #endregion

        #region public static DBConst String(string value)

        public static DBConst String(string value)
        {
            return Const(DbType.String, value);
        }

        #endregion

        #region public static DBConst DateTime(DateTime value)

        public static DBConst DateTime(DateTime value)
        {
            return Const(DbType.DateTime, value);
        }

        #endregion

        #region public static DBConst Guid(Guid value)

        public static DBConst Guid(Guid value)
        {
            return Const(DbType.Object, value);
        }

        #endregion

        #region public static DBConst Double(double value)
        
        public static DBConst Double(double value)
        {
            return Const(DbType.Double, value);
        }

        #endregion

        #region public static DBConst Null()

        public static DBConst Null()
        {
            return Const(DbType.Object, DBNull.Value);
        }

        #endregion

        //
        // operator overloads
        //

        #region public static explicit operator DBConst(int value)

        public static explicit operator DBConst(int value)
        {
            return Const(value);
        }

        #endregion

        #region public static explicit operator DBConst(string value)

        public static explicit operator DBConst(string value)
        {
            return Const(value);
        }

        #endregion

        #region public static explicit operator DBConst(DateTime value)

        public static explicit operator DBConst(DateTime value)
        {
            return Const(value);
        }

        #endregion

        #region public static explicit operator DBConst(Guid value)

        public static explicit operator DBConst(Guid value)
        {
            return Const(value);
        }

        #endregion

        #region public static explicit operator DBConst(double value)

        public static explicit operator DBConst(double value)
        {
            return Const(value);
        }

        #endregion

        #region public static explicit operator DBConst(DBNull value)

        public static explicit operator DBConst(DBNull value)
        {
            return DBConst.Null();
        }

        #endregion



    }

    internal class DBConstRef : DBConst, IDBAlias
    {
        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Constant; }
        }

        #endregion

        //
        // ctor
        //

        #region internal DBConstRef()

        internal DBConstRef()
        {
        }

        #endregion

        //
        // SQL Statement builder methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (null == this.Value || this.Value is DBNull)
                builder.WriteNull();
            else
                builder.WriteLiteral(this.Type, this.Value);
            if (string.IsNullOrEmpty(this.Alias) == false)
                builder.WriteAlias(this.Alias);

            return true;
        }

        #endregion

        //
        // XML Serialization Methods
        //

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.DbType, this.Type.ToString(), context);

            if(string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAlias(writer, this.Alias, context);

            bool isNull = (null == this.Value || this.Value is DBNull);

            this.WriteAttribute(writer, XmlHelper.IsNull, isNull.ToString(), context);
            
            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            XmlHelper.WriteNativeValue(this.Value, writer, context);
            return true;
        }

        #endregion

       
        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;

            if (this.IsAttributeMatch(XmlHelper.DbType, reader, context))
                this.Type = (DbType)Enum.Parse(typeof(DbType), reader.Value, true);
            else if (this.IsAttributeMatch(XmlHelper.IsNull, reader, context) && bool.TrueString.Equals(reader.Value))
                this.Value = DBNull.Value;
            else if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
            {
                this.Alias = reader.Value;
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
            if (reader.NodeType == System.Xml.XmlNodeType.Text && reader.Value == XmlHelper.NullString)
                this.Value = DBNull.Value;
            else
            {
                if (reader.IsEmptyElement == false)
                {
                    string end = reader.LocalName;

                    do
                    {
                        if (reader.NodeType == System.Xml.XmlNodeType.Element && this.IsElementMatch(XmlHelper.ParameterValue, reader, context))
                        {
                            this.Value = XmlHelper.ReadNativeValue(reader, context);
                        }
                        if (reader.NodeType == System.Xml.XmlNodeType.EndElement && this.IsElementMatch(end, reader, context))
                            break;
                    }
                    while (reader.Read());
                }
            }
            return base.ReadAnInnerElement(reader, context);
        }

        #endregion

        #region IDBAlias Members

        public void As(string aliasName)
        {
            this.Alias = aliasName;
        }

        #endregion
    }
}
