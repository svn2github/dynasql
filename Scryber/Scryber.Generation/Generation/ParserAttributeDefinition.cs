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
    internal class ParserAttributeDefinition : ParserPropertyDefinition
    {
        PDFConverter _converter;
        bool _customparse;
        bool _iscodedom;

        internal PDFConverter Converter
        {
            get { return _converter; }
        }

        internal override bool IsCustomParsable
        {
            get
            {
                return _customparse;
            }
        }

        internal bool IsCodeDomGenerator
        {
            get
            {
                return _iscodedom;
            }
        }

        internal ParserAttributeDefinition(string name, PropertyInfo info, PDFConverter convert, bool iscustomparsable)
            : base(name, info, DeclaredParseType.Attribute)
        {
            this._converter = convert;
            this._customparse = iscustomparsable;
            this._iscodedom = Array.IndexOf<Type>(info.PropertyType.GetInterfaces(), typeof(IPDFSimpleCodeDomValue)) > -1;
        }

        internal override object GetValue(XmlReader reader)
        {
            return this.Converter(reader, this.PropertyInfo.PropertyType);
        }

        
    }
}
