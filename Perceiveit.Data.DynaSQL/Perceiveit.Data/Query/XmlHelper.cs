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
using System.Xml;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Internal class of static helper methods and constants
    /// </summary>
    internal static class XmlHelper
    {

        //
        // exceptions
        //

        #region public static XmlException CreateException(string message, XmlReader reader, Exception inner) + 2 overloads

        public static XmlException CreateException(string message, XmlReader reader, Exception inner, object param1)
        {
            return CreateException(string.Format(message, param1), reader, inner);
        }

        public static XmlException CreateException(string message, XmlReader reader, Exception inner, object param1, object param2)
        {
            return CreateException(string.Format(message, param1, param2), reader, inner);
        }

        public static XmlException CreateException(string message, XmlReader reader, Exception inner)
        {
            if (reader is IXmlLineInfo)
            {
                return new XmlException(message, inner, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition);
            }
            else
                return new XmlException(message, inner);
        }

        #endregion

        //
        // Xml match methods
        //

        #region public static bool IsAttributeMatch(string attrName, XmlReader reader, XmlReaderContext context)

        public static bool IsAttributeMatch(string attrName, XmlReader reader, XmlReaderContext context)
        {
            if (context.QualifiedAttribute)
            {
                bool pre = string.Equals(reader.Prefix, context.Prefix, StringComparison.Ordinal);
                bool attr = string.Equals(reader.LocalName, attrName, StringComparison.Ordinal);
                return attr && pre;
            }
            else
                return string.Equals(reader.LocalName, attrName);
        }

        #endregion

        #region public static bool IsElementMatch(string elename, System.Xml.XmlReader reader, XmlReaderContext context)
        
        public static bool IsElementMatch(string elename, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (context.QualifiedElement)
            {
                bool ele = elename.Equals(reader.LocalName);
                bool pre = (string.IsNullOrEmpty(reader.Prefix) && string.IsNullOrEmpty(context.Prefix)) || reader.Prefix.Equals(context.Prefix);

                return ele && pre;
            }
            else
                return string.Equals(elename, reader.LocalName);
        }

        #endregion

        #region public static T ParseEnum<T>(string value) where T: struct

        /// <summary>
        /// Parses the value to a specific enumeration type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value) where T: struct
        {
#if SILVERLIGHT
            T result;
            if (Enum.TryParse<T>(value, true, out result))
                return result;
            else
                throw new XmlException(string.Format(Errors.CannotParseValueToType, value, typeof(T).FullName));
#else

            return (T)Enum.Parse(typeof(T), value);
#endif
        }

        #endregion

        public static bool TryParseEnum<T>(string value, out T result) where T: struct
        {
#if SILVERLIGHT
            return Enum.TryParse<T>(value, out result);
#else
            int index = Array.IndexOf<string>(Enum.GetNames(typeof(T)), value);
            if (index > -1)
            {
                result = (T)Enum.Parse(typeof(T), value, true);
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
#endif
        }

        //
        // write methods
        //

        #region public static void WriteAttribute(XmlWriter writer, string attrname, string value, XmlWriterContext context)
        
        public static void WriteAttribute(XmlWriter writer, string attrname, string value, XmlWriterContext context)
        {
            if (context.QualifiedAttribute && string.IsNullOrEmpty(context.Prefix) == false)
                writer.WriteAttributeString(context.Prefix, attrname, context.NameSpace, value);
            else
                writer.WriteAttributeString(attrname, value);
        }

        #endregion

        #region public static void WriteStartElement(XmlWriter writer, string eleName, XmlWriterContext context)
        
        public static void WriteStartElement(XmlWriter writer, string eleName, XmlWriterContext context)
        {
            if (context.QualifiedElement && string.IsNullOrEmpty(context.Prefix) == false)
                writer.WriteStartElement(context.Prefix, eleName, context.NameSpace);
            else
                writer.WriteStartElement(eleName);
        }

        #endregion

        #region public static void WriteEndElement(XmlWriter writer, XmlWriterContext context)
        
        public static void WriteEndElement(XmlWriter writer, XmlWriterContext context)
        {
            writer.WriteEndElement();
        }

        #endregion

        private const string GuidTypeCode = "Guid";
        private const string TimeSpanTypeCode = "TimeSpan";
        private const string BinaryTypeCode = "Binary";
        private const string ObjectTypeCode = "Object";
        private const string XmlTypeCode = "XML";

        #region public static void WriteNativeValue(object value, System.Xml.XmlWriter writer, XmlWriterContext context)

        public static void WriteNativeValue(object value, System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (null == value || value is DBNull)
            {
                writer.WriteString(XmlHelper.NullString);
            }
            else
            {
                TypeCode tc = Type.GetTypeCode(value.GetType());
                string code;
                string svalue;
                bool isCData = false;
                switch (tc)
                {
                    case TypeCode.Empty:
                    case TypeCode.Object:
                        if (value is Guid)
                        {
                            code = GuidTypeCode;
                            svalue = value.ToString();
                        }
                        else if (value is TimeSpan)
                        {
                            code = TimeSpanTypeCode;
                            svalue = value.ToString();
                        }
                        else if (value is Byte[])
                        {
                            isCData = true;
                            svalue = Convert.ToBase64String((byte[])value);
                            code = BinaryTypeCode;
                        }
                        
                            
#if SILVERLIGHT
                        else
                        {
                            //only option is to convert to a string
                            isCData = false;
                            code = ObjectTypeCode;
                            svalue = value.ToString();
                        }
#else
                        else if (value is System.Xml.XmlNode)
                        {
                            isCData = false;
                            svalue = ((System.Xml.XmlNode)value).OuterXml;
                            code = XmlTypeCode;
                        }
                        
                        else
                        {
                            isCData = true;
                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                            {
                                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                                bf.Serialize(ms, value);
                                value = ms.ToArray();
                            }
                            svalue = Convert.ToBase64String((byte[])value, Base64FormattingOptions.InsertLineBreaks);
                            code = ObjectTypeCode;
                        }
#endif
                        break;
                    default:
                        isCData = false;
                        svalue = value.ToString();
                        code = tc.ToString();
                        break;
                }

                WriteStartElement(writer, XmlHelper.ParameterValue, context);
                WriteAttribute(writer, XmlHelper.SystemTypeCode, code, context);

                if (isCData)
                    writer.WriteCData(svalue);
                else
                    writer.WriteString(svalue);

                WriteEndElement(writer, context);
            }
        }

        #endregion

        //
        // read methods
        //

        #region public static object ReadNativeValue(System.Xml.XmlReader reader, XmlReaderContext context)

        public static object ReadNativeValue(System.Xml.XmlReader reader, XmlReaderContext context)
        {

            object value = null;

            if (reader.HasAttributes && reader.MoveToFirstAttribute())
            {
                if (IsAttributeMatch(XmlHelper.SystemTypeCode, reader, context))
                {
                    string code = reader.Value;
                    if (reader.Read() && (reader.NodeType == System.Xml.XmlNodeType.Text
                                      || reader.NodeType == XmlNodeType.CDATA))
                    {
                        string svalue;
                        svalue = reader.Value;
                        if (code == GuidTypeCode)
                        {
                            value = new Guid(svalue);
                        }
                        else if (code == TimeSpanTypeCode)
                        {
                            value = TimeSpan.Parse(svalue);
                        }
                        else if (code == BinaryTypeCode)
                        {
                            value = Convert.FromBase64String(svalue);
                        }
                        else if (code == ObjectTypeCode)
                        {
#if SILVERLIGHT
                            throw new NotSupportedException(Errors.CannotDeserializeObjectTypesInSilverlight);
#else
                            byte[] data = Convert.FromBase64String(svalue);
                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
                            {
                                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                                value = bf.Deserialize(ms);
                            }
#endif
                        }
                        else if (code == XmlTypeCode)
                        {
#if SILVERLIGHT
                            throw new NotSupportedException(Errors.CannotDeserializeObjectTypesInSilverlight);
#else
                            value = new System.Xml.XmlDocument().ReadNode(reader.ReadSubtree());
#endif
                        }
                        else
                        {
                            TypeCode tc = XmlHelper.ParseEnum<TypeCode>(code);
                            switch (tc)
                            {
                                case TypeCode.Boolean:
                                    value = bool.Parse(svalue);
                                    break;
                                case TypeCode.Byte:
                                    value = byte.Parse(svalue);
                                    break;
                                case TypeCode.Char:
                                    value = svalue[0];
                                    break;
                                case TypeCode.DBNull:
                                    value = DBNull.Value;
                                    break;
                                case TypeCode.DateTime:
                                    value = DateTime.Parse(svalue);
                                    break;
                                case TypeCode.Decimal:
                                    value = Decimal.Parse(svalue);
                                    break;
                                case TypeCode.Double:
                                    value = Double.Parse(svalue);
                                    break;
                                case TypeCode.Empty:
                                    throw new NotSupportedException("Cannot parse an Empty object value");
                                    
                                case TypeCode.Int16:
                                    value = short.Parse(svalue);
                                    break;
                                case TypeCode.Int32:
                                    value = int.Parse(svalue);
                                    break;
                                case TypeCode.Int64:
                                    value = long.Parse(svalue);
                                    break;
                                case TypeCode.Object:
                                    throw new NotSupportedException("Cannot parse an Empty object value");
                                    
                                case TypeCode.SByte:
                                    value = sbyte.Parse(svalue);
                                    break;
                                case TypeCode.Single:
                                    value = float.Parse(svalue);
                                    break;
                                case TypeCode.String:
                                    value = svalue;
                                    break;
                                case TypeCode.UInt16:
                                    value = ushort.Parse(svalue);
                                    break;
                                case TypeCode.UInt32:
                                    value = uint.Parse(svalue);
                                    break;
                                case TypeCode.UInt64:
                                    value = ulong.Parse(svalue);
                                    break;
                                default:
                                    value = null;
                                    break;
                            }
                        }
                        
                    }
                }
            }

            return value;
        }

        #endregion
        
        //
        // xml name constants
        //

        public const string EmptyName = "";

        public const string Statement = "statement";
        public const string Select = "select";
        public const string Update = "update";
        public const string Insert = "insert";
        public const string Delete = "delete";
        public const string Exec = "exec";
        public const string Script = "script";

        public const string Alias = "alias";
        public const string Distinct = "distinct";
        public const string Fields = "fields";
        public const string FieldOwner = "owner";
        public const string FieldTable = "table";
        public const string Into = "into";
        public const string AllFields = "all-field";
        public const string Top = "top";
        public const string TopValue = "value";
        public const string TopType = "type";
        public const string From = "from";
        public const string Where = "where";
        public const string Group = "group";
        public const string Order = "order";
        public const string OrderBy = "order-by";
        public const string Table = "table";
        public const string Assignments = "assignments";
        public const string Values = "values";
        public const string Owner = "owner";
        public const string Name = "name";
        public const string Parameters = "parameters";
        public const string AField = "field";
        public const string Join = "join";
        public const string JoinType = "type";
        public const string JoinOn = "on";
        public const string JoinTo = "to";
        public const string KnownFunction = "func";
        public const string Function = "function";
        public const string FunctionParameter = "param";
        public const string FunctionName = "name";
        public const string Constant = "const";
        public const string DbType = "db-type";
        public const string IsNull = "is-null";
        public const string Operator = "op";
        public const string UnaryOp = "unary";
        public const string LeftOperand = "left";
        public const string RightOperand = "right";
        public const string Compare = "compare";
        public const string Between = "between";
        public const string MinValue = "min";
        public const string MaxValue = "max";
        public const string Parameter = "param";
        public const string ParameterSize = "size";
        public const string ParameterDirection = "direction";
        public const string NullString = "NULL";
        public const string ParameterValue = "value";
        public const string SystemTypeCode = "type-code";
        public const string Aggregate = "aggregate";
        public const string ValueGroup = "value-group";
        public const string Calculation = "calc";
        public const string BooleanOperator = "bool-op";
        public const string JoinList = "joins";
        public const string Item = "item";
        public const string Value = "value";
        public const string Assign = "assign";
        public const string InnerSelect = "inner-select";
        public const string SortBy = "sort-by";
        public const string Declare = "declaration";
        public const string Returns = "return";
        public const string Set = "set";
        public const string Multiple = "multi";
        public const string CheckExists = "exist";
        public const string TableName = "table-name";
        public const string TableOwner = "table-owner";
        public const string Temp = "temp";
        public const string Unique = "unique";
        public const string IndexOptions = "idx-options";

        public const string ColumnDefinition = "column-defn";
        public const string OtherType = "other";
        public const string Length = "length";
        public const string Precision = "precision";
        public const string ColumnFlags = "col-flags";
        public const string Default = "default";
        public const string TableIndex = "table-index";

        public const string CreateIndex = "create-index";
        public const string DropIndex = "drop-index";
        public const string IndexColumns = "columns";

        public const string CreateView = "create-view";
        public const string DropView = "drop-view";

        public const string CreateSproc = "create-sproc";
        public const string DropSproc = "drop-sproc";
        public const string SprocScript = "sproc-script";
        public const string SprocParams = "sproc-params";

        public const string CreateTable = "create-table";
        public const string DropTable = "drop-table";
        public const string ColumnList = "columns";
        public const string ConstraintList = "constraints";

        public const string PrimaryKey = "primary-key";

        public const string ForeignKey = "foreign-key";
        public const string UpdateAction = "on-update";
        public const string DeleteAction = "on-delete";
        public const string ReferenceOwner = "refer-owner";
        public const string ReferenceTable = "refer-table";
        public const string ReferenceColumns = "reference-columns";

        public const string CreateSequence = "create-sequence";
        public const string DropSequence = "drop-sequence";
        public const string SequenceMin = "min";
        public const string SequenceMax = "max";
        public const string SequenceStart = "start";
        public const string SequenceIncrement = "inc-by";
        public const string SequenceNoCache = "no-cache";
        public const string SequenceCache = "cache";
        public const string SequenceCycling = "cycling";
        public const string SequenceOrdering = "ordering";

        public const string TableHintSet = "with-hints";
        public const string HintOption = "option";

        public const string TableHint = "table-hint";
        public const string HintParameter = "hint-param";

        public const string QueryOptionSet = "with-options";
        public const string QueryOption = "query-option";
        
    }

}
