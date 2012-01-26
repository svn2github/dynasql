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
using System.Xml;
using Scryber.Drawing;

namespace Scryber
{
    


    

    public delegate void PDFPageRequestHandler(object sender, PDFPageRequestArgs args);

    public class PDFPageRequestArgs : EventArgs
    {
        #region ivars

        private bool _hasnew;
        private int _newpageindex;
        private int _currpage;
        private PDFRect _avail;
        private IPDFLayoutEngine _orig;
        private PDFSize _meas;

        #endregion

        //
        // properties
        //

        #region public bool HasNewPage {get;}

        /// <summary>
        /// Returns true if these arguments have a new page set. Update this value using the AllowNewPage or RejectNewPage methods
        /// </summary>
        public bool HasNewPage
        {
            get { return _hasnew; }
        }

        #endregion

        #region public int NewPageIndex {get;}

        /// <summary>
        /// Gets the index of the new page to restart layout on.
        /// </summary>
        public int NewPageIndex
        {
            get { return _newpageindex; }
        }

        #endregion

        #region public int CurrentPage {get;}

        /// <summary>
        /// Gets the index of the original page that needs to be overflowed
        /// </summary>
        public int CurrentPage
        {
            get { return _currpage; }
        }

        #endregion

        #region public PDFRect NewAvailableSpace {get; set;}

        /// <summary>
        /// Gets or sets the New space available for layout 
        /// </summary>
        public PDFRect NewAvailableSpace
        {
            get { return _avail; }
            set { _avail = value; }
        }

        #endregion

        #region public IPDFLayoutEngine OriginalRequestor

        /// <summary>
        /// Gets the layout engine that started the orginal request.
        /// </summary>
        public IPDFLayoutEngine OriginalRequestor
        {
            get { return _orig; }
        }

        #endregion

        #region public PDFSize MeasuredSize

        /// <summary>
        /// Gets or sets the measured size of the last component
        /// </summary>
        public PDFSize MeasuredSize
        {
            get { return _meas; }
            set { _meas = value; }
        }

        #endregion

        //
        // ctor
        //

        public PDFPageRequestArgs(int currentpage, IPDFLayoutEngine originalRequestor, PDFSize requiredspace)
        {
            this.MeasuredSize = requiredspace;
            this._currpage = currentpage;
            this._hasnew = false;
            this._avail = PDFRect.Empty;
            this._newpageindex = -1;
            this._orig = originalRequestor;
        }

        //
        // methods
        //

        /// <summary>
        /// Marks these page request args as allowing a new page with the specified bounds
        /// </summary>
        /// <param name="pgIndex"></param>
        /// <param name="bounds"></param>
        public void AllowNewPage(int pgIndex, PDFRect bounds)
        {
            this._hasnew = true;
            this._newpageindex = pgIndex;
            this._avail = bounds;
        }

        /// <summary>
        /// Marks these page request args as NOT allowing a new page
        /// </summary>
        public void RejectNewPage()
        {
            this._hasnew = false;
            this._newpageindex = -1;
            this._avail = PDFRect.Empty;
        }

    }


    

}
