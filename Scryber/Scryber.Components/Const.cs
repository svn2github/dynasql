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
using Scryber.Drawing;

namespace Scryber
{
    internal class Const
    {
        public const string ArrayStringStart = "[";
        public const string ArrayStringEnd = "]";
        public const string ArrayStringSeparator = ", ";
        public const char ArraySplitChar = ',';

        
        public const double GraphicsCircularityFactor = 0.552285;
        public const double PageUnitsPerPoint = 1.0;
        public const double PageUnitsPerInch = 72.0 * PageUnitsPerPoint; //72
        public const double PageUnitsPerMM = PageUnitsPerInch / 25.4; //2.83
        public const string DefaultParserEventPrefix = "on-";

        public const string UniqueIDSeparator = "_";
        

        public const string PDFXProducer = "PDFX - by PerceiveIT";

        public const int StreamEncoding = 1252;

        /// <summary>
        /// All the characters that should be escaped
        /// </summary>
        public static readonly char[] EscapeChars = new char[] { '\\', '(', ')' };
        /// <summary>
        /// String representations of the escape characters
        /// </summary>
        public static readonly string[] EscapeStrings = new string[] { "\\", "(", ")" };
        /// <summary>
        /// String representations of the escaped versions of the characters to replace
        /// Matches both count and order of the EscapeStrings
        /// </summary>
        public static readonly string[] ReplaceStrings = new string[] { "\\\\", "\\(", "\\)" };

        //
        // namespace constants
        //

        public static readonly string PDFDataNamespace = GetNamespaceAndAssemblyName(typeof(Scryber.Data.PDFForEach));//"Scryber.Styles, Scryber.Styles, Version=1.0.0.0, Culture=neutral, PublicKeyToken=872cbeb81db952fe";
        public static readonly string PDFComponentNamespace = GetNamespaceAndAssemblyName(typeof(Scryber.Components.PDFLabel));// "Scryber.Components, Scryber.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=872cbeb81db952fe";
        public static readonly string PDFStylesNamespace = GetNamespaceAndAssemblyName(typeof(Scryber.Styles.PDFStyle)); //"Scryber.Styles, Scryber.Styles, Version=1.0.0.0, Culture=neutral, PublicKeyToken=872cbeb81db952fe";

        //
        // PDF Name constants
        //

        internal const string PageTreeName = "PDFPageTree";


        public static string GetNamespaceAndAssemblyName(Type fortype)
        {
            string ns = fortype.Namespace;
            string assm = fortype.Assembly.FullName;
            return string.Format("{0}, {1}", ns, assm);
        }
    }
}
