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

namespace Scryber.Styles
{
    public class PDFStyleValue
    {
        private string _txt;

        public string Text
        {
            get { return _txt; }
        }

        private object _val;

        public object Value
        {
            get { return _val; }
        }

        private bool _isparsed = false;

        public bool HasParsedValue
        {
            get { return this._isparsed; }
        }

        public void SetParsedValue(object value)
        {
            this._isparsed = true;
            this._val = value;
        }

        public PDFStyleValue(string text)
        {
            this._txt = text;
            this._val = null;
        }

        public PDFStyleValue(string text, object value)
        {
            this._txt = text;
            this._val = value;
            this._isparsed = true;
        }



    }
}
