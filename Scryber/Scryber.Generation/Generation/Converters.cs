/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Scryber.Generation
{
    internal static class Converters
    {
        internal static object ToType(XmlReader reader, Type requiredType)
        {
            string value = reader.Value;
            string prefix;
            string name;
            int sep = value.IndexOf(':');
            if (sep > 0)
            {
                name = value.Substring(sep + 1);
                prefix = value.Substring(0, sep);
            }
            else
            {
                prefix = "";
                name = value;
            }

            string ns = reader.LookupNamespace(prefix);
            bool isremote;
            ParserClassDefinition cdefn = ParserDefintionFactory.GetClassDefinition(name, ns, out isremote);
            if (isremote)
                throw new ArgumentOutOfRangeException("isremote", String.Format(Errors.CannotUseRemoteTypeReferencesInATypeAttribute, reader.Value));
            return cdefn.ClassType;
        }

        internal static object ToInt32(XmlReader reader, Type requiredType)
        {
            return int.Parse(reader.Value);
        }

        internal static object ToInt16(XmlReader reader, Type requiredType)
        {
            return short.Parse(reader.Value);
        }

        internal static object ToInt64(XmlReader reader, Type requiredType)
        {
            return long.Parse(reader.Value);
        }

        internal static object ToUInt32(XmlReader reader, Type requiredType)
        {
            return uint.Parse(reader.Value);
        }

        internal static object ToUInt16(XmlReader reader, Type requiredType)
        {
            return ushort.Parse(reader.Value);
        }

        internal static object ToUInt64(XmlReader reader, Type requiredType)
        {
            return ulong.Parse(reader.Value);
        }

        internal static object ToFloat(XmlReader reader, Type requiredType)
        {
            return float.Parse(reader.Value);
        }

        internal static object ToDouble(XmlReader reader, Type requiredType)
        {
            return double.Parse(reader.Value);
        }

        internal static object ToDecimal(XmlReader reader, Type requiredType)
        {
            return decimal.Parse(reader.Value);
        }

        internal static object ToString(XmlReader reader, Type requiredType)
        {
            if (reader.NodeType == XmlNodeType.Attribute)
                return reader.Value;
            else if (reader.NodeType == XmlNodeType.Element)
                return reader.ReadElementContentAsString();
            else
                return reader.ReadContentAsString();
        }

        internal static object ToDateTime(XmlReader reader, Type requiredType)
        {
            return DateTime.Parse(reader.Value);
        }

        internal static object ToTimeSpan(XmlReader reader, Type requiredType)
        {
            return TimeSpan.Parse(reader.Value);
        }

        internal static object ToEnum(XmlReader reader, Type requiredType)
        {
            string value = reader.Value;
            if (value.IndexOf(' ') > -1)
                value = value.Replace(' ', ',');
            return Enum.Parse(requiredType, value);
        }

        internal static object ToByte(XmlReader reader, Type requiredType)
        {
            return byte.Parse(reader.Value);
        }

        internal static object ToSByte(XmlReader reader, Type requiredType)
        {
            return sbyte.Parse(reader.Value);
        }

        internal static object ToChar(XmlReader reader, Type requiredType)
        {
            return char.Parse(reader.Value);
        }

        internal static object ToGuid(XmlReader reader, Type requiredType)
        {
            return new Guid(reader.Value);
        }

        internal static object ToBool(XmlReader reader, Type requiredType)
        {
            return bool.Parse(reader.Value);
        }

        internal static object ToDBNull(XmlReader reader, Type requiredType)
        {
            return DBNull.Value;
        }

        internal static object ToUri(XmlReader reader, Type requiredType)
        {
            return new Uri(reader.Value);
        }

        //
        // Parseable Converter
        // Creating generic classes and methods from reflection to
        // hold and call a classes Parse method.

        internal delegate T Parse<T>(string value);

        internal abstract class ParseableConverter
        {
            internal Type Type { get; private set; }
            internal abstract object Convert(XmlReader reader, Type requiredType);
            internal abstract object ConvertValue(string value, Type requiredType);

            internal PDFConverter Converter { get; private set; }
            internal PDFValueConverter ValueConverter { get; private set; }

            internal abstract Delegate Parse { get; set; }

            protected ParseableConverter(Type parsetype)
            {
                this.Type = parsetype;
                Converter = new PDFConverter(this.Convert);
                ValueConverter = new PDFValueConverter(this.ConvertValue);
            }
        }

        internal class ParseableConverter<T> : ParseableConverter
        {
            private Parse<T> _parse;

            internal override Delegate Parse
            {
                get { return _parse; }
                set { _parse = (Parse<T>)value; }

            }

            public ParseableConverter()
                : base(typeof(T))
            {
            }

            internal override object Convert(XmlReader reader, Type requiredType)
            {
                return _parse(reader.Value);
            }

            internal override object ConvertValue(string value, Type requiredType)
            {
                return _parse(value);
            }
        }

        private static List<ParseableConverter> _parsables = new List<ParseableConverter>();

        private static bool TryGetExistingParseable(Type t, out ParseableConverter found)
        {
            foreach (ParseableConverter pc in _parsables)
            {
                if (pc.Type == t)
                {
                    found = pc;
                    return true;
                }
            }
            found = null;
            return false;
        }

        private static ParseableConverter GetConverter(Type t)
        {
            ParseableConverter conv;
            if (!TryGetExistingParseable(t, out conv))
            {
                MethodInfo parse = null;

                MethodInfo[] mis = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (MethodInfo mi in mis)
                {
                    if (mi.ReturnType != t)
                        continue;
                    if (mi.Name != "Parse")
                        continue;
                    ParameterInfo[] pis = mi.GetParameters();
                    if (pis.Length != 1)
                        continue;
                    if (pis[0].ParameterType != typeof(string))
                        continue;

                    //We have a match
                    parse = mi;
                    break;
                }

                if (null == parse)
                    throw new PDFParserException(Errors.ParsableValueMustHaveParseMethod);

                Type gen = typeof(Parse<>);
                Type genwithtype = gen.MakeGenericType(t);

                Delegate method = Delegate.CreateDelegate(genwithtype, parse);
                Type genconverter = typeof(ParseableConverter<>).MakeGenericType(t);
                object instance = Activator.CreateInstance(genconverter);
                conv = (ParseableConverter)instance;
                conv.Parse = method;
                _parsables.Add(conv);
            }
            return conv;
        }

        internal static PDFValueConverter GetParsableValueConverter(Type t)
        {
            ParseableConverter conv = GetConverter(t);
            return conv.ValueConverter;
        }


        internal static PDFConverter GetParsableConverter(Type t)
        {
            ParseableConverter conv = GetConverter(t);
            return conv.Converter;
        }
    }
}
