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

namespace Scryber.Generation
{
    internal static class ParserHelper
    {
        internal const string typeAttr = "type-name";
        internal const string codebehindAttr = "code-behind";
        internal const string xmlnsAttr = "xmlns";
        internal const string initMethod = "InitializeComponents";
        internal const string parseMethodName = "ParseComponentAtPath";
        internal const string idAttributeName = "id";
        internal const string idPropertyName = "ID";
        internal const string addMethodName = "Add";
        internal const string loadedSourceProperty = "LoadedSource";
        internal const string loadtypeProperty = "LoadType";
        internal const string remoteSourceTypeAttribute = "type";
        internal const string remoteSourcePathAttribute = "source";
        internal const string mapPathMethod = "MapPath";
        internal const string documentPropertyName = "Document";
        internal const string instantiateOverrideMethodName = "InitializeComponents";
        internal const string bindingContextPropertyName = "Context";
        internal const string evalXPathMethod = "EvalXPathAsString";

        internal const int defaultErrorLevel = 4;

        internal static Type[] knownObjectTypes = new Type[] { typeof(Guid), typeof(DateTime), typeof(TimeSpan), typeof(Type), typeof(System.Uri), typeof(System.Version) };
        
        //internal static Type textLiteralType = typeof(Scryber.Components.PDFTextLiteral);

        internal static char[] trimmingCharacters = new char[] { '\r', '\n', '\t', ' ' };
        internal static string[] specialRootAttributes = new string[] { xmlnsAttr, codebehindAttr, typeAttr };
        internal static string[] standardImports = new string[] { "Scryber", "Scryber.Components", "Scryber.Data" };

        internal static bool IsSimpleType(ParserAttributeDefinition prop)
        {

            TypeCode tc = Type.GetTypeCode(prop.ValueType);

            if (tc != TypeCode.Empty && tc != TypeCode.Object)
                return true;
            else if (prop.ValueType.IsEnum)
                return true;
            else if (Array.IndexOf<Type>(ParserHelper.knownObjectTypes, prop.ValueType) >= 0)
                return true;
            else
                return false;
        }

        internal static BindingType IsBindingExpression(string value, out string expression)
        {
            expression = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return BindingType.None;
            }
            else if (!(value.StartsWith("{") && value.EndsWith("}")))
            {
                expression = string.Empty;
                return BindingType.None;
            }
            else if (value.StartsWith("{xpath:"))
            {
                int len = "{xpath:".Length;

                expression = value.Substring(len, value.Length - len - 1);
                return BindingType.XPath;
            }
            else if (value.StartsWith("{code:"))
            {
                int len = "{code:".Length;
                expression = value.Substring(len, value.Length - len - 1);
                return BindingType.Code;
            }
            else
            {
                return BindingType.None;
            }
        }



        #region private string GetSafeTypeName(string typename)

        /// <summary>
        /// Converts a string into a safe type name by replacing any non valid characters with an underscore
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        internal static string GetSafeTypeName(string typename)
        {
            StringBuilder sb = new StringBuilder();
            bool isfirst = true;
            foreach (char c in typename)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
                else if (char.IsDigit(c))
                {
                    if (isfirst)
                        sb.Append("_");
                    else
                        sb.Append(c);
                }
                else
                    sb.Append('_');

                isfirst = false;
            }
            return sb.ToString();
        }

        #endregion

        //internal static Type GetTypeFromValue(System.Xml.XmlTextReader reader)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
