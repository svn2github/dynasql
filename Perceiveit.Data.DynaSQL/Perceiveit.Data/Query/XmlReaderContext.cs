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

namespace Perceiveit.Data.Query
{
    public class XmlReaderContext
    {
        private string _ns;

        public string NameSpace
        {
            get { return _ns; }
        }

        private string _pref;
        public string Prefix
        {
            get { return _pref; }
        }

        private bool _qele;

        public bool QualifiedElement
        {
            get { return _qele; }
        }

        private bool _qattr;

        public bool QualifiedAttribute
        {
            get { return _qattr; }
        }

        private XmlFactory _fact;
        public XmlFactory Factory
        {
            get { return _fact; }
        }

        private DBParamList _params;
        public DBParamList Parameters
        {
            get { return _params; }
        }

        public XmlReaderContext(string ns, string pref)
            : this(ns, pref, XmlFactory.Create())
        {
        }
        public XmlReaderContext(string ns, string pref, XmlFactory factory)
            : this(ns, pref, factory, true, false)
        {
        }

        public XmlReaderContext(string ns, string pref, XmlFactory factory, bool qualifiedElement, bool qualifiedAttribute)
        {
            this._fact = factory;
            this._ns = ns;
            this._pref = pref;
            this._qattr = qualifiedAttribute;
            this._qele = qualifiedElement;
            this._params = new DBParamList();
        }
    }
}
