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
    /// <summary>
    /// A reference to a column in a create or alter statement
    /// </summary>
    public abstract class DBColumn : DBClause
    {
        //
        // properties
        //

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the data type of this column
        /// </summary>
        public DbType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the data type of this column if it's not a standard known type
        /// </summary>
        public string OtherType { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum length of this columns data type
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// Gets or sets the accuracy of this column
        /// </summary>
        public int Precision { get; protected set; }

        /// <summary>
        /// Gets or sets the flags associated with this column
        /// </summary>
        public DBColumnFlags Flags { get; protected set; }

        #region public object DefaultValue {get;set;} + public bool HasDefault {get;}

        private DBClause _default;
        
        /// <summary>
        /// Gets or sets the default value for the column. 
        /// Use ClearDefault() to clear any set value
        /// </summary>
        public DBClause DefaultValue
        {
            get { return _default; }
            protected set
            {
                _default = value;
                this.Flags |= DBColumnFlags.HasDefault;
            }
        }
        
        /// <summary>
        /// Returns true if a default value has been set on this column. 
        /// </summary>
        public bool HasDefault
        {
            get { return (this.Flags & DBColumnFlags.HasDefault) > 0; }
        }

        #endregion


        //
        // ctor(s)
        //

        #region protected DBColumn(string name)

        /// <summary>
        /// protected constructor for an abstract class
        /// </summary>
        /// <param name="name"></param>
        protected DBColumn(string name)
        {
            this.Name = name;
        }

        #endregion

        //
        // instance methods
        //

        #region public DBColumn Default(DBClause value)

        /// <summary>
        /// Sets the Default value 
        /// for this column and returns itself
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBColumn Default(DBClause value)
        {
            this.DefaultValue = value;
            return this;
        }

        #endregion

        #region public void ClearDefault()

        /// <summary>
        /// Removes any set value for this columns default
        /// </summary>
        public void ClearDefault()
        {
            this.DefaultValue = null;
            this.Flags &= ~DBColumnFlags.HasDefault;
        }

        #endregion

        

        //
        // factory methods
        //

        internal static DBColumn Column()
        {
            return Column(string.Empty);
        }

        public static DBColumn Column(string name)
        {
            DBColumn col = new DBColumnRef(name);
            return col;
        }

        public static DBColumn Column(string name, DbType type)
        {
            DBColumn col = Column(name);
            col.Type = type;
            return col;
        }

        public static DBColumn Column(string name, DbType type, int maxLength)
        {
            DBColumn col = Column(name);
            col.Type = type;
            col.Length = maxLength;
            return col;
        }

        public static DBColumn Column(string name, DbType type, DBColumnFlags flags)
        {
            DBColumn col = Column(name);
            col.Type = type;
            col.Flags = flags;
            return col;
        }

        public static DBColumn Column(string name, DbType type, int maxLength, DBColumnFlags flags)
        {
            DBColumn col = Column(name);
            col.Type = type;
            col.Flags = flags;
            col.Length = maxLength;
            return col;
        }

        public static DBColumn Column(string name, DbType type, int maxLength, int precision, DBColumnFlags flags)
        {
            DBColumn col = Column(name);
            col.Type = type;
            col.Flags = flags;
            col.Length = maxLength;
            return col;
        }



        
    }

    public class DBColumnRef : DBColumn
    {

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Gets the element name for this column
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.ColumnDefinition; }
        }

        #endregion

        public DBColumnRef(string name)
            : base(name)
        {
        }



        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginIdentifier();
            builder.WriteRaw(this.Name);
            builder.EndIdentifier();
            builder.WriteRaw(" ");
            builder.WriteColumnDataType(this.Type, this.Length, this.Precision, this.Flags);
            builder.WriteColumnFlags(this.Flags, this.DefaultValue);

            return true;

        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            base.WriteAllAttributes(writer, context);
            this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);
            if (!string.IsNullOrEmpty(this.OtherType))
                this.WriteAttribute(writer, XmlHelper.OtherType, this.OtherType, context);
            else
                this.WriteAttribute(writer, XmlHelper.DbType, this.Type.ToString(), context);
            
            if (this.Length > 0)
                this.WriteAttribute(writer, XmlHelper.Length, this.Length.ToString(), context);
            
            if (this.Precision > 0)
                this.WriteAttribute(writer, XmlHelper.Precision, this.Precision.ToString(), context);

            this.WriteAttribute(writer, XmlHelper.ColumnFlags, this.Flags.ToString(), context);

            return true;
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasDefault)
            {
                this.WriteStartElement(XmlHelper.Default, writer, context);
                this.DefaultValue.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.Default, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            switch (reader.LocalName)
            {
                case(XmlHelper.Name):
                    this.Name = reader.Value;
                    break;
                case(XmlHelper.Length):
                    this.Length = int.Parse(reader.Value);
                    break;
                case(XmlHelper.Precision):
                    this.Precision = int.Parse(reader.Value);
                    break;
                case(XmlHelper.OtherType):
                    this.OtherType = reader.Value;
                    break;
                case(XmlHelper.DbType):
                    this.Type = (DbType)Enum.Parse(typeof(DbType), reader.Value);
                    break;
                case(XmlHelper.ColumnFlags):
                    this.Flags = (DBColumnFlags)Enum.Parse(typeof(DBColumnFlags), reader.Value);
                    break;
                default:
                    b = base.ReadAnAttribute(reader, context);
                    break;
            }
            return b;
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            DBClause read = this.ReadNextInnerClause(reader.Name, reader, context);
            if (null == read)
                b = false;
            else
            {
                this.DefaultValue = read;
                b = true;
            }
            return b;
        }
    }

    public class DBColumnList : DBTokenList<DBColumn>
    {
        

        /// <summary>
        /// Gets the characters that should be appended to the list after being built
        /// </summary>
        public override string ListEnd { get { return ""; } }

        

        public DBColumnList()
        {
            
        }

        internal void ReadXml(string p, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            while (reader.Read())
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    DBColumn c = (DBColumn)context.Factory.Read(reader.LocalName, reader, context);
                    this.Add(c);
                }
                else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && string.Equals(reader.LocalName, p))
                    break;
            }
        }

        internal void WriteXml(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            foreach (DBColumn col in this)
            {
                col.WriteXml(writer, context);
            }
        }


        private const char COLUMN_SEPARATOR = ',';

        /// <summary>
        /// Writes the list of columns as references by name
        /// </summary>
        /// <param name="elementname"></param>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        internal void WriteReferenceXml(string elementname, System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            writer.WriteStartElement(elementname, context.NameSpace);
            StringBuilder sb = new StringBuilder();
            foreach (DBColumn c in this)
            {
                if (sb.Length > 0)
                    sb.Append(COLUMN_SEPARATOR);
                sb.Append(c.Name);
            }
            writer.WriteString(sb.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Reads the list of column references
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        internal void ReadReferenceXml(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            string all = reader.ReadElementContentAsString();
            if (!string.IsNullOrEmpty(all))
            {
                string[] each = all.Split(COLUMN_SEPARATOR);
                
                foreach (string s in each)
                {
                    DBColumn col = DBColumn.Column(s);
                    this.Add(col);
                }
            }
        }
    }
}
