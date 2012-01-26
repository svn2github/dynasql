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
    internal class ParserClassDefinition
    {

        private Type _classtype;
        private ParserPropertyDefinition _default;
        private ParserPropertyDefinitionCollection _attributes = new ParserPropertyDefinitionCollection();
        private ParserPropertyDefinitionCollection _elements = new ParserPropertyDefinitionCollection();
        private ParserEventDefinitionCollection _events = new ParserEventDefinitionCollection();

        /// <summary>
        /// Gets the Type of this class defintion
        /// </summary>
        public Type ClassType
        {
            get { return _classtype; }
        }

        public ParserEventDefinitionCollection Events
        {
            get { return _events; }
        }

        /// <summary>
        /// Gets the named attributes that can be parsed in this class
        /// </summary>
        public ParserPropertyDefinitionCollection Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Gets the named elements that canbe parsed in this class definition
        /// </summary>
        public ParserPropertyDefinitionCollection Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// Gets or sets the 'default' property definition
        /// </summary>
        public ParserPropertyDefinition DefaultElement
        {
            get { return _default; }
            set { _default = value; }
        }



        public ParserClassDefinition(Type classtype)
        {
            this._classtype = classtype;
        }

       
    }
}
