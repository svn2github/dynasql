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
using Scryber.Drawing;
using Scryber.Components;
 
namespace Scryber
{
    [PDFParsableComponent("Outline")]
    public class PDFOutline : IPDFBindableComponent
    {

        #region Databinding events

        public event PDFDataBindEventHandler DataBinding;

        public event PDFDataBindEventHandler DataBound;

        protected virtual void OnDataBinding(PDFDataContext context)
        {
            if (null != this.DataBinding)
                this.DataBinding(this, new PDFDataBindEventArgs(context));
        }

        protected virtual void OnDataBound(PDFDataContext context)
        {
            if (null != this.DataBound)
                this.DataBound(this, new PDFDataBindEventArgs(context));
        }

        #endregion

        private PDFComponent _belongs;
        private string _title = string.Empty;
        private PDFColor _col = null;
        private bool _boldstyle = false;
        private bool _italicstyle = false;
        private bool _hasbold = false;
        private bool _hasitalic = false;
        private bool _hasopen = false;
        private bool _open = false;

        /// <summary>
        /// Gets the component this PDFOutline belongs to
        /// </summary>
        internal PDFComponent BelongsTo 
        { 
            get { return _belongs; } 
            set { _belongs = value; }
        }

        /// <summary>
        /// Gets the name of the destination in the outline dictionary
        /// </summary>
        public string DestinationName
        {
            get { return _belongs.UniqueID; }
        }

        /// <summary>
        /// Gets or sets the title of this Outline item
        /// </summary>
        [PDFAttribute("title")]
        public string Title 
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the color (if any) of this outline item
        /// </summary>
        [PDFAttribute("color")]
        public PDFColor Color 
        {
            get { return this._col; }
            set { this._col = value; }
        }

        /// <summary>
        /// Gets or sets the bold accent on the outline item
        /// </summary>
        [PDFAttribute("bold")]
        public bool FontBold
        {
            get { return _boldstyle; }
            set { this._boldstyle = value; _hasbold = true; }
        }

        /// <summary>
        /// Gets or sets whether the FontBold value is set
        /// </summary>
        public bool HasBold
        {
            get { return _hasbold; }
            set { _hasbold = value; }
        }

        /// <summary>
        /// Gets or set the italic accent on the outline item
        /// </summary>
        [PDFAttribute("italic")]
        public bool FontItalic
        {
            get { return _italicstyle; }
            set { _italicstyle = value; _hasitalic = true; }
        }

        /// <summary>
        /// Gets or sets whether the FontItalic value is set
        /// </summary>
        public bool HasItalic
        {
            get { return _hasitalic; }
            set { _hasitalic = value; }
        }

        /// <summary>
        /// Gets or sets the Open flag on the outline item
        /// </summary>
        [PDFAttribute("open")]
        public bool OutlineOpen
        {
            get { return _open; }
            set { this._open = value; _hasopen = true; }
        }

        /// <summary>
        /// Gets or sets whether the Open value is set
        /// </summary>
        public bool HasOpen
        {
            get { return _hasopen; }
            set { _hasopen = value; }
        }


        public PDFOutline()
        {            
        }

        #region IPDFBindableComponent Members

        

        public void DataBind(PDFDataContext context)
        {
            this.OnDataBinding(context);
            this.DoDataBind(context);
            this.OnDataBound(context);
        }

        protected virtual void DoDataBind(PDFDataContext context)
        {
        }

        #endregion
    }

    


    
}
