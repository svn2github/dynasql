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
    internal abstract class ParserPropertyDefinition
    {
        #region ivars

        private PropertyInfo _desc;
        private Type _proptype;
        private string _name;
        private DeclaredParseType _parsetype;

        #endregion

        /// <summary>
        /// Gets the PropertyDescriptor for this definition
        /// </summary>
        internal PropertyInfo PropertyInfo
        {
            get { return _desc; }
        }

        /// <summary>
        /// Gets the Type that this properties values are (must be)
        /// </summary>
        internal Type ValueType
        {
            get { return _proptype; }
        }

        /// <summary>
        /// Gets the Parse type (attribute, element etc) this property is read from
        /// </summary>
        internal DeclaredParseType ParseType
        {
            get { return _parsetype; }
        }

        /// <summary>
        /// Gets the name of the node to parse this definition from.
        /// </summary>
        internal string Name
        {
            get { return _name; }
        }

        internal virtual bool IsCustomParsable
        {
            get { return false; }
        }

        internal virtual object GetValue(System.Xml.XmlReader reader)
        {
            throw new NotSupportedException("GetValue is not supported on the root property type");
        }
        /// <summary>
        /// Creates a new PropertyDefinition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="parsetype"></param>
        protected ParserPropertyDefinition(string name, PropertyInfo pi, DeclaredParseType parsetype)
        {
            if (null == name)
                throw new ArgumentNullException("name");
            if (null == pi)
                throw new ArgumentNullException("desc");
            this._desc = pi;
            this._name = name;
            this._parsetype = parsetype;
            this._proptype = pi.PropertyType;
        }
    }


    internal class ParserPropertyDefinitionCollection : System.Collections.ObjectModel.KeyedCollection<string,ParserPropertyDefinition>
    {
        protected override string GetKeyForItem(ParserPropertyDefinition item)
        {
            return item.Name;
        }

        internal bool TryGetPropertyDefinition(string name, out ParserPropertyDefinition defn)
        {
            if (this.Count == 0)
            {
                defn = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(name, out defn);
        }
    }
}
