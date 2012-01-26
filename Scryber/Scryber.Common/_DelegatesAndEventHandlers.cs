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
using System.Linq;
using System.Text;

namespace Scryber
{

    


    /// <summary>
    /// Event handler for the databind event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void PDFDataBindEventHandler(object sender, PDFDataBindEventArgs args);

    /// <summary>
    /// Arguments for the PDFDataBindEventHandler
    /// </summary>
    public class PDFDataBindEventArgs : EventArgs
    {
        /// <summary>
        /// instance storage of PFDataContext
        /// </summary>
        private PDFDataContext _context;

        /// <summary>
        /// Gets the context associated with the current databind operation
        /// </summary>
        public PDFDataContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Creates a new instance of the PDFDataBindEventArgs
        /// </summary>
        /// <param name="context"></param>
        public PDFDataBindEventArgs(PDFDataContext context)
        {
            this._context = context;
        }
    }
}
