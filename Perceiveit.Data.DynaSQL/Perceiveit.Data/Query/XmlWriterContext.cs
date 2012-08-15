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

namespace Perceiveit.Data.Query
{
    public class XmlWriterContext
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

        private DBParamList _params;

        public DBParamList Parameters
        {
            get { return _params; }
            set { _params = value; }
        }


        #region public bool SerializeDelegateParameters {get;set;}

        private bool _serDelegated = true;

        public bool SerializeDelegateParameters
        {
            get { return _serDelegated; }
            set { _serDelegated = value; }
        }

        #endregion

        public XmlWriterContext(string ns, string prefix)
            : this(ns, prefix, true, false)
        {
        }

        public XmlWriterContext(string ns, string prefix, bool qualifiedElement, bool qualifiedAttribute)
        {
            this._ns = ns;
            this._pref = prefix;
            this._qattr = qualifiedAttribute;
            this._qele = qualifiedElement;
            this._params = new DBParamList();
        }
    }
}
