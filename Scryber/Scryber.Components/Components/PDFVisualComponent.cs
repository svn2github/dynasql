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
using System.Drawing;
using Scryber.Styles;
using Scryber.Resources;
using Scryber.Drawing;

namespace Scryber.Components
{
    public abstract class PDFVisualComponent : PDFContainerComponent, IPDFVisualComponent, IPDFStyledComponent
    {

        #region public PDFStyle Style {get;set;} + public bool HasStyle{get;}

        private PDFStyle _style;

        /// <summary>
        /// Gets the applied style for this page Component
        /// </summary>
        [PDFElement("Style")]
        public PDFStyle Style
        {
            get 
            {
                if (_style == null)
                    _style = new PDFStyle();
                return _style; 
            }
        }

        

        /// <summary>
        /// Gets the flag to indicate if this page Component has style 
        /// information associated with it.
        /// </summary>
        public bool HasStyle
        {
            get { return this._style != null && this._style.HasItems; }
        }

        #endregion

        #region public PDFUnit X {get;set;} + public bool HasX {get;}

        /// <summary>
        /// Gets or Sets the X (Horizontal) position of this page Component
        /// </summary>
        [PDFAttribute("x")]
        public PDFUnit X
        {
            get
            {
                return this.Style.Position.X;
            }
            set
            {
                this.Style.Position.X = value;
            }
        }

        /// <summary>
        /// Gets the flag to identify if the X position has been set for this Page Component
        /// </summary>
        public bool HasX
        {
            get
            {
                PDFPositionStyle pos;
                return this._style != null && this._style.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.XAttr);
            }
        }

        #endregion

        #region public PDFUnit Y {get;set;} + public bool HasY {get;}

        /// <summary>
        /// Gets or sets the Y (vertical) position of the Page Component
        /// </summary>
        [PDFAttribute("y")]
        public PDFUnit Y
        {
            get
            {
                return this.Style.Position.Y;
            }
            set
            {
                this.Style.Position.Y = value;
            }
        }

        /// <summary>
        /// Gets the flag to identifiy is the Y value has been set on this page Component
        /// </summary>
        public bool HasY
        {
            get
            {
                PDFPositionStyle pos;
                return this._style != null && this._style.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.YAttr);
            }
        }

        #endregion

        #region public PDFUnit Width {get;set;} + public bool HasWidth {get;}

        /// <summary>
        /// Gets or Sets the Width of this page Component
        /// </summary>
        [PDFAttribute("width")]
        public PDFUnit Width
        {
            get
            {
                return this.Style.Position.Width;
            }
            set
            {
                this.Style.Position.Width = value;
            }
        }

        

        /// <summary>
        /// Gets the flag to identify if the Width has been set for this Page Component
        /// </summary>
        public bool HasWidth
        {
            get
            {
                PDFPositionStyle pos;
                return this._style != null && this._style.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.WidthAttr);
            }
        }

        #endregion

        #region public PDFUnit Height {get;set;} + public bool HasHeight {get;}

        /// <summary>
        /// Gets or sets the Height of the Page Component
        /// </summary>
        [PDFAttribute("height")]
        public PDFUnit Height
        {
            get
            {
                return this.Style.Position.Height;
            }
            set
            {
                this.Style.Position.Height = value;
            }
        }

        /// <summary>
        /// Gets the flag to identifiy is the Height has been set on this page Component
        /// </summary>
        public bool HasHeight
        {
            get
            {
                PDFPositionStyle pos;
                return this._style != null && this._style.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.HeightAttr);
            }
        }

        #endregion

        protected PDFVisualComponent(PDFObjectType type)
            : base(type)
        {
        }

        #region Databind()
        
        /// <summary>
        /// Inheritors should override this method to provide their own databing implementations.
        /// </summary>
        /// <param name="includeChildren">Flag to identifiy if children should be databound also</param>
        protected override void DoDataBind(PDFDataContext context, bool includeChildren)
        {
            if (includeChildren && this.HasStyle)
                this.Style.DataBind(context);
            base.DoDataBind(context, includeChildren);
        }

        #endregion

        

    }

    public class PDFVisualComponentList : PDFComponentWrappingList<PDFVisualComponent>
    {
        public PDFVisualComponentList(PDFComponentList innerList)
            : base(innerList)
        {
        }

    }
}
