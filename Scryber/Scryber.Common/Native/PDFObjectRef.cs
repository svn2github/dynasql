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
using Scryber.Native;

namespace Scryber.Native
{
    /// <summary>
    /// Encapsulates reference to a PDFIndirectObject
    /// </summary>
    public class PDFObjectRef : IFileObject
    {

        public PDFObjectType Type { get { return PDFObjectTypes.ObjectRef; } }


        /// <summary>
        /// Gets this references objects Number
        /// </summary>
        public int Number
        {
            get { return this.Reference.Number; }
        }

        /// <summary>
        /// Gets this references objects Generation
        /// </summary>
        public int Generation
        {
            get { return this.Reference.Generation; }
        }

        private IIndirectObject _ref;

        /// <summary>
        /// Gets or sets the acutal object this reference refers to
        /// </summary>
        public IIndirectObject Reference
        {
            get { return _ref; }
            set { this._ref = value; }
        }

        protected internal PDFObjectRef()
            : this(null)
        {
        }

        protected internal PDFObjectRef(IIndirectObject reference)
        {
            this._ref = reference;
        }

        public void WriteData(PDFWriter writer)
        {
            writer.WriteObjectRefS(this);
        }

        public override string ToString()
        {
            return this.Reference.ToString();
        }
    }
}
