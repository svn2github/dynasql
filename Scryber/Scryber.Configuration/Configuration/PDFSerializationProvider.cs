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

namespace Adis.OCMS.PDFCreator.Configuration
{
    public class PDFSerializationProvider : System.Configuration.Provider.ProviderBase
    {
        private string _type = null;
        private string _prefix = null;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            this._type = config["pdfSerializerType"];
            if (String.IsNullOrEmpty(this._type))
                throw new System.Configuration.Provider.ProviderException("The 'pdfSerializerTypeAttribute' was not provided for the SerializationProviders configuration Component");

            this._prefix = config["tagPrefix"];
            if (this._prefix == null)
                this._prefix = "";
            
        }
    }

    

    
}
