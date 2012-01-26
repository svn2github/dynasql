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

namespace Scryber.Generation
{
    internal static class ParserDefintionFactory
    {
        private const char NamespaceSeparator = ',';

        /// <summary>
        /// A dictionary of all the looked up assemblies in this application
        /// </summary>
        private class ApplicationDefn : Dictionary<string,AssemblyDefn>
        {
        }

        /// <summary>
        /// An Assembly with a dictionary of all the looked up namespaces
        /// </summary>
        private class AssemblyDefn : Dictionary<string, NamespaceDefn>
        {
            private System.Reflection.Assembly _assm;

            internal System.Reflection.Assembly InnerAssembly
            {
                get { return _assm; }
                set { _assm = value; }
            }
        }

        /// <summary>
        /// A namespace with a dictionary of all the declared types in that namespace with the PDFParserComponent attribute
        /// </summary>
        private class NamespaceDefn : Dictionary<string, Type>
        {
            private Dictionary<string,string> _remotes;

            /// <summary>
            /// Gets a dictionary of the remote types defined in the namespace
            /// mapping the remote type name to the concrete type name
            /// </summary>
            public Dictionary<string,string> RemoteTypes
            {
                get
                {
                    if (null == _remotes)
                        _remotes = new Dictionary<string,string>();
                    return _remotes;
                }
            }
        }

        private static ApplicationDefn _application = new ApplicationDefn();
        private static Dictionary<Type, ParserClassDefinition> _parserclasses = new Dictionary<Type, ParserClassDefinition>();


        /// <summary>
        /// Gets the ParserClassDefintion for a specific element name in an assemblyQualifiedNamespace.
        /// </summary>
        /// <param name="parsertype"></param>
        /// <returns></returns>
        internal static ParserClassDefinition GetClassDefinition(string elementname, string assemblyQualifiedNamespace, out bool isremote)
        {
            if (null == assemblyQualifiedNamespace)
                assemblyQualifiedNamespace = string.Empty;
            if (string.IsNullOrEmpty(elementname))
                throw new ArgumentNullException("elementname");

            AssemblyDefn assmdefn;
            NamespaceDefn nsdefn;
            Type t;

            string assm;
            string ns;
            ExtractAssemblyAndNamespace(assemblyQualifiedNamespace, out assm, out ns);

            if (!_application.TryGetValue(assm, out assmdefn))
            {
                assmdefn = new AssemblyDefn();
                Assembly found = GetAssemblyByName(assm);
                if (null == found)
                    throw new PDFParserException(String.Format(Errors.ParserCannotFindAssemblyWithName, assm));

                assmdefn.InnerAssembly = found;
                _application[assm] = assmdefn;
            }

            if (!assmdefn.TryGetValue(ns, out nsdefn))
            {
                nsdefn = new NamespaceDefn();
                PopulateNamespaceFromAssembly(ns, assmdefn, nsdefn);
                assmdefn[ns] = nsdefn;
            }

            if (!nsdefn.TryGetValue(elementname, out t))
            {
                string actual;
                //it's not an actual type so check the remote types and if it's still not fount throw an exception
                if (!nsdefn.RemoteTypes.TryGetValue(elementname, out actual) || !nsdefn.TryGetValue(actual, out t))
                    throw new PDFParserException(String.Format(Errors.NoPDFComponentDeclaredWithNameInNamespace, elementname, assemblyQualifiedNamespace));
                else
                {
                    isremote = true;
                }
            }
            else
                isremote = false;

            return GetClassDefinition(t);
        }

        /// <summary>
        /// Gets the ParserClassDefintion for a specific type.
        /// </summary>
        /// <param name="parsertype"></param>
        /// <returns></returns>
        internal static ParserClassDefinition GetClassDefinition(Type parsabletype)
        {
            if (null == parsabletype)
                throw new ArgumentNullException("parsabletype");

            ParserClassDefinition found;
            if (!_parserclasses.TryGetValue(parsabletype, out found))
            {
                found = LoadClassDefinition(parsabletype);
                _parserclasses[parsabletype] = found;
            }
            return found;
        }

        /// <summary>
        /// Reflects all types in the AssembyDefn's addembly and extracts all the types in theasembly in the required 
        /// namespace that have the PDFParsableComponent attibute defined on the class - adding them to the NamespaceDefn
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="assmdefn"></param>
        /// <param name="nsdefn"></param>
        private static void PopulateNamespaceFromAssembly(string ns, AssemblyDefn assmdefn, NamespaceDefn nsdefn)
        {
            if (assmdefn == null)
                throw new ArgumentNullException("assmdefn");
            if (assmdefn.InnerAssembly == null)
                throw new ArgumentNullException("assmdefn.InnerAssembly");

            if (null == nsdefn)
                throw new ArgumentNullException("nsdefn");
            
            Type[] all = assmdefn.InnerAssembly.GetTypes();
            foreach (Type t in all)
            {
                if (string.Equals(t.Namespace, ns))
                {
                    object[] attrs = t.GetCustomAttributes(typeof(PDFParsableComponentAttribute), false);
                    if (null != attrs && attrs.Length > 0)
                    {
                        PDFParsableComponentAttribute compattr = (PDFParsableComponentAttribute)attrs[0];
                        string name = compattr.ElementName;
                        if (string.IsNullOrEmpty(name))
                            name = t.Name;

                        nsdefn.Add(name, t);

                        //check to see if it has a remote name too
                        attrs = t.GetCustomAttributes(typeof(PDFRemoteParsableComponentAttribute), false);
                        if (null != attrs && attrs.Length > 0)
                        {
                            PDFRemoteParsableComponentAttribute remattr = (PDFRemoteParsableComponentAttribute)attrs[0];
                            string remotename = remattr.ElementName;
                            if (string.IsNullOrEmpty(name))
                                remotename = t.Name + "-Ref";
                            nsdefn.RemoteTypes.Add(remotename, name);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Loads and returns an assembly with the specified name. If name is null or empty, then the entry assembly is returned.
        /// </summary>
        /// <param name="assm"></param>
        /// <returns></returns>
        private static Assembly GetAssemblyByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Assembly.GetEntryAssembly();
            else
                return Assembly.Load(name);
        }

        /// <summary>
        /// Splits the assembly qualified namespace into the assembly name, and the namespace value
        /// </summary>
        /// <param name="assemblyQualifiedNamespace"></param>
        /// <param name="assm"></param>
        /// <param name="ns"></param>
        private static void ExtractAssemblyAndNamespace(string assemblyQualifiedNamespace, out string assm, out string ns)
        {
            int index = assemblyQualifiedNamespace.IndexOf(NamespaceSeparator);
            if (index < 0)
            {
                assm = string.Empty;
                ns = assemblyQualifiedNamespace;
            }
            else
            {
                assm = assemblyQualifiedNamespace.Substring(index + 1).Trim();
                ns = assemblyQualifiedNamespace.Substring(0, index);
            }
        }


        private static ParserClassDefinition LoadClassDefinition(Type parsertype)
        {
            ParserClassDefinition defn = new ParserClassDefinition(parsertype);
            LoadClassProperties(parsertype, defn);
            LoadClassEvents(parsertype, defn);
            return defn;
        }

        private static void LoadClassEvents(Type parsertype, ParserClassDefinition defn)
        {
            EventInfo[] all = parsertype.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            foreach (EventInfo ei in all)
            {
                PDFParserIgnoreAttribute ignore = GetCustomAttribute<PDFParserIgnoreAttribute>(ei, true);
                if (null != ignore && ignore.Ignore)
                    continue; //Ignore the event

                PDFAttributeAttribute attr = GetCustomAttribute<PDFAttributeAttribute>(ei, true);
                if (null != attr)
                {
                    string name = attr.AttributeName;
                    if (string.IsNullOrEmpty(name))
                        throw new PDFParserException(String.Format(Errors.ParserAttributeNameCannotBeEmpty, ei.Name, parsertype.FullName));


                    ParserEventDefinition evt = new ParserEventDefinition(name, ei);
                    defn.Events.Add(evt);
                    
                }
            }
        }

        private static void LoadClassProperties(Type parsertype, ParserClassDefinition defn)
        {
            PropertyInfo[] all = parsertype.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in all)
            {
                PDFConverter convert;
                string name;
                PDFParserIgnoreAttribute ignore = GetCustomAttribute<PDFParserIgnoreAttribute>(pi, true);
                if (null != ignore && ignore.Ignore)
                    continue; //This property should be ignored.

                PDFAttributeAttribute attr = GetCustomAttribute<PDFAttributeAttribute>(pi, true);

                if (null != attr)
                {
                    name = attr.AttributeName;
                    bool iscustom = false;
                    if (string.IsNullOrEmpty(name))
                        throw new PDFParserException(String.Format(Errors.ParserAttributeNameCannotBeEmpty, pi.Name, parsertype.FullName));

                    if (!IsSimpleType(pi.PropertyType, out convert) && !IsCustomParsableType(pi.PropertyType, out convert, out iscustom))
                        throw new PDFParserException(String.Format(Errors.ParserAttributeMustBeSimpleOrCustomParsableType, pi.Name, parsertype.FullName, pi.PropertyType));

                    ParserAttributeDefinition ad = new ParserAttributeDefinition(attr.AttributeName, pi, convert, iscustom);
                    defn.Attributes.Add(ad);
                }

                PDFElementAttribute ele = GetCustomAttribute<PDFElementAttribute>(pi, true);
                if (null != ele)
                {
                    ParserPropertyDefinition propele;
                    bool isdefault = false;
                    name = ele.Name;

                    if (string.IsNullOrEmpty(name))
                        isdefault = true;
                    PDFArrayAttribute array = GetCustomAttribute<PDFArrayAttribute>(pi, true);
                    PDFTemplateAttribute template = GetCustomAttribute<PDFTemplateAttribute>(pi, true);
                    bool iscustom = false;

                    if (null != array)
                    {
                        Type basetype = array.ContentBaseType;
                        //ArrayCollection
                        if (null == basetype)
                            basetype = typeof(IPDFComponent);

                        propele = new ParserArrayDefinition(name, basetype, pi);

                    }
                    else if (null != template)
                    {
                        propele = new ParserTemplateDefintion(name, pi);
                    }
                    else if (IsSimpleType(pi.PropertyType, out convert) || IsCustomParsableType(pi.PropertyType, out convert, out iscustom))
                    {
                        //SimpleElement
                        propele = new ParserSimpleElementDefinition(name, pi, convert, iscustom);
                    }
                    else
                    {
                        //Complex Element
                        propele = new ParserComplexElementDefiniton(name, pi);
                    }

                    if (isdefault)
                    {
                        if (null != defn.DefaultElement)
                            throw new PDFParserException(String.Format(Errors.DuplicateDefaultElementOnClass, pi.Name, pi.DeclaringType));
                        else
                            defn.DefaultElement = propele;
                    }
                    else
                        defn.Elements.Add(propele);
                }
            }
        }

        internal static bool IsSimpleType(Type type, out PDFConverter convert)
        {
            bool result = false;
            if (type.IsEnum)
            {
                convert = new PDFConverter(Converters.ToEnum);
                return true;
            }
            TypeCode code = Type.GetTypeCode(type);
            
            switch (code)
            {
                case TypeCode.Boolean:
                    convert = new PDFConverter(Converters.ToBool);
                    result = true;
                    break;
                case TypeCode.Byte:
                    convert = new PDFConverter(Converters.ToByte);
                    result = true;
                    break;
                case TypeCode.Char:
                    convert = new PDFConverter(Converters.ToChar);
                    result = true;
                    break;
                case TypeCode.DBNull:
                    convert = new PDFConverter(Converters.ToDBNull);
                    result = true;
                    break;
                case TypeCode.DateTime:
                    convert = new PDFConverter(Converters.ToDateTime);
                    result = true;
                    break;
                case TypeCode.Decimal:
                    convert = new PDFConverter(Converters.ToDecimal);
                    result = true;
                    break;
                case TypeCode.Double:
                    convert = new PDFConverter(Converters.ToDouble);
                    result = true;
                    break;
                case TypeCode.Int16:
                    convert = new PDFConverter(Converters.ToInt16);
                    result = true;
                    break;
                case TypeCode.Int32:
                    convert = new PDFConverter(Converters.ToInt32);
                    result = true;
                    break;
                case TypeCode.Int64:
                    convert = new PDFConverter(Converters.ToInt64);
                    result = true;
                    break;
                case TypeCode.SByte:
                    convert = new PDFConverter(Converters.ToSByte);
                    result = true;
                    break;
                case TypeCode.Single:
                    convert = new PDFConverter(Converters.ToFloat);
                    result = true;
                    break;
                case TypeCode.String:
                    convert = new PDFConverter(Converters.ToString);
                    result = true;
                    break;
                case TypeCode.UInt16:
                    convert = new PDFConverter(Converters.ToUInt16);
                    result = true;
                    break;
                case TypeCode.UInt32:
                    convert = new PDFConverter(Converters.ToUInt32);
                    result = true;
                    break;
                case TypeCode.UInt64:
                    convert = new PDFConverter(Converters.ToUInt64);
                    result = true;
                    break;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                    {
                        convert = new PDFConverter(Converters.ToGuid);
                        result = true;
                    }
                    else if(type == typeof(DateTime))
                    {
                        convert = new PDFConverter(Converters.ToDateTime);
                        result = true;
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        convert = new PDFConverter(Converters.ToTimeSpan);
                        result = true;
                    }
                    else if (type == typeof(Uri))
                    {
                        convert = new PDFConverter(Converters.ToUri);
                        result = true;
                    }
                    else if (type == typeof(Type))
                    {
                        convert = new PDFConverter(Converters.ToType);
                        result = true;
                    }
                    else
                    {
                        convert = null;
                        result = false;
                    }
                    break;
                case TypeCode.Empty:
                default:
                    convert = null;
                    result = false;
                    break;
            }
            return result;
        }

        internal static bool IsSimpleObjectType(Type type, out PDFValueConverter convert)
        {
            bool result = false;
            if (type.IsEnum)
            {
                convert = new PDFValueConverter(ConvertObjects.ToEnum);
                return true;
            }
            TypeCode code = Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.Boolean:
                    convert = new PDFValueConverter(ConvertObjects.ToBool);
                    result = true;
                    break;
                case TypeCode.Byte:
                    convert = new PDFValueConverter(ConvertObjects.ToByte);
                    result = true;
                    break;
                case TypeCode.Char:
                    convert = new PDFValueConverter(ConvertObjects.ToChar);
                    result = true;
                    break;
                case TypeCode.DBNull:
                    convert = new PDFValueConverter(ConvertObjects.ToDBNull);
                    result = true;
                    break;
                case TypeCode.DateTime:
                    convert = new PDFValueConverter(ConvertObjects.ToDateTime);
                    result = true;
                    break;
                case TypeCode.Decimal:
                    convert = new PDFValueConverter(ConvertObjects.ToDecimal);
                    result = true;
                    break;
                case TypeCode.Double:
                    convert = new PDFValueConverter(ConvertObjects.ToDouble);
                    result = true;
                    break;
                case TypeCode.Int16:
                    convert = new PDFValueConverter(ConvertObjects.ToInt16);
                    result = true;
                    break;
                case TypeCode.Int32:
                    convert = new PDFValueConverter(ConvertObjects.ToInt32);
                    result = true;
                    break;
                case TypeCode.Int64:
                    convert = new PDFValueConverter(ConvertObjects.ToInt64);
                    result = true;
                    break;
                case TypeCode.SByte:
                    convert = new PDFValueConverter(ConvertObjects.ToSByte);
                    result = true;
                    break;
                case TypeCode.Single:
                    convert = new PDFValueConverter(ConvertObjects.ToFloat);
                    result = true;
                    break;
                case TypeCode.String:
                    convert = new PDFValueConverter(ConvertObjects.ToString);
                    result = true;
                    break;
                case TypeCode.UInt16:
                    convert = new PDFValueConverter(ConvertObjects.ToUInt16);
                    result = true;
                    break;
                case TypeCode.UInt32:
                    convert = new PDFValueConverter(ConvertObjects.ToUInt32);
                    result = true;
                    break;
                case TypeCode.UInt64:
                    convert = new PDFValueConverter(ConvertObjects.ToUInt64);
                    result = true;
                    break;
                case TypeCode.Object:
                    if (type == typeof(Guid))
                    {
                        convert = new PDFValueConverter(ConvertObjects.ToGuid);
                        result = true;
                    }
                    else if (type == typeof(DateTime))
                    {
                        convert = new PDFValueConverter(ConvertObjects.ToDateTime);
                        result = true;
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        convert = new PDFValueConverter(ConvertObjects.ToTimeSpan);
                        result = true;
                    }
                    else if (type == typeof(Uri))
                    {
                        convert = new PDFValueConverter(ConvertObjects.ToUri);
                        result = true;
                    }
                    else
                    {
                        convert = null;
                        result = false;
                    }
                    break;
                case TypeCode.Empty:
                default:
                    convert = null;
                    result = false;
                    break;
            }
            return result;
        }


        internal static bool IsCustomParsableType(Type type, out PDFConverter convert, out bool iscustom)
        {
            PDFParsableValueAttribute valattr = GetCustomAttribute<PDFParsableValueAttribute>(type, true);
            if (null != valattr)
            {
                iscustom = true;
                convert = Converters.GetParsableConverter(type);
                return null != convert;
            }
            else
            {
                iscustom = false;
                convert = null;
                return false;
            }
        }

        internal static bool IsCustomParsableObjectType(Type type, out PDFValueConverter convert)
        {
            PDFParsableValueAttribute valattr = GetCustomAttribute<PDFParsableValueAttribute>(type, true);
            if (null != valattr)
            {
                convert = Converters.GetParsableValueConverter(type);
                return null != convert;
            }
            else
            {
                convert = null;
                return false;
            }
        }



        //
        // GetCustomAttribute overloads
        //

        private static T GetCustomAttribute<T>(Type ontype,bool inherit) where T : Attribute
        {
            return GetCustomAttribute(ontype, typeof(T), inherit) as T;
        }

        private static Attribute GetCustomAttribute(Type declaring, Type attrType, bool inherit)
        {
            object[] found = declaring.GetCustomAttributes(attrType, inherit);
            if (found == null || found.Length == 0)
                return null;
            else
                return found[0] as Attribute;
        }

        private static T GetCustomAttribute<T>(MemberInfo pi, bool inherit) where T : Attribute
        {
            return GetCustomAttribute(pi, typeof(T), inherit) as T;
        }

        private static Attribute GetCustomAttribute(MemberInfo pi, Type attrType, bool inherit)
        {
            object[] found = pi.GetCustomAttributes(attrType, inherit);
            if (found == null || found.Length == 0)
                return null;
            else
                return found[0] as Attribute;
        }
    }
}
