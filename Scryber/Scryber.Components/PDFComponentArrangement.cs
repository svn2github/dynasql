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
using Scryber.Drawing;
using Scryber.Styles;

namespace Scryber
{
    /// <summary>
    /// Defines the rendering arrangement for a component
    /// </summary>
    public class PDFComponentArrangement
    {
        /// <summary>
        /// Gets or sets the page index of this component arrangement
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the complete bounds of this component - including any margins and padding
        /// </summary>
        public PDFRect Bounds { get; set; }

        /// <summary>
        /// Gets or sets the padding of this component
        /// </summary>
        public PDFThickness Padding { get; set; }

        /// <summary>
        /// Gets or sets the complete style for this component
        /// </summary>
        public PDFStyle AppliedStyle { get; set; }

        /// <summary>
        /// Gets or sets the margins for this component
        /// </summary>
        public PDFThickness Margins { get; set; }

        /// <summary>
        /// Ges or sets the position mode for this component
        /// </summary>
        public PositionMode PositionMode { get; set; }

        /// <summary>
        /// Gets or sets the layout mode for this component
        /// </summary>
        public LayoutMode LayoutMode { get; set; }

        /// <summary>
        /// Gets or sets the content rect for this component - relative to the bounds, after the margins and padding have been applied
        /// </summary>
        public PDFRect Content { get; set; }

        /// <summary>
        /// Gets or sets the border rect for this compontent - relative to the bounds after the margins have been applied
        /// </summary>
        public PDFRect Border { get; set; }

        /// <summary>
        /// Returns true if the component should be displayed
        /// </summary>
        public bool Display { get; set; }

        /// <summary>
        /// Gets or sets the actual PDF boundary rectangle this component will render in
        /// </summary>
        public PDFRect RenderBounds { get; set; }

        /// <summary>
        /// Creates a new PDFComponentArrangement
        /// </summary>
        public PDFComponentArrangement() { }

    }

    /// <summary>
    /// Defines the arrangement for a component that can be spread across one or more pages
    /// </summary>
    public class PDFComponentMultiArrangement : PDFComponentArrangement
    {
        private PDFComponentMultiArrangement _next;

        /// <summary>
        /// Gets or sets the next arrangement for any multiple display component
        /// </summary>
        public PDFComponentMultiArrangement NextArrangement
        {
            get { return _next; }
            set { _next = value; }
        }

        /// <summary>
        /// Gets the last arrangement in a multiple display component.
        /// </summary>
        public PDFComponentMultiArrangement LastArrangement
        {
            get
            {
                if (null == this.NextArrangement)
                    return this;
                else
                    return NextArrangement.LastArrangement;
            }
        }

        public bool IsLastArrangement
        {
            get{return this.NextArrangement == null;}
        }

        public void AppendArrangement(PDFComponentMultiArrangement arrange)
        {
            this.LastArrangement._next = arrange;
        }

        public PDFComponentArrangement GetArrangementForPage(int pageindex)
        {
            if(pageindex == this.PageIndex)
                return this;
            else if(this.IsLastArrangement)
                return null;
            else
                return this.NextArrangement.GetArrangementForPage(pageindex);
                
        }


        
    }
}
