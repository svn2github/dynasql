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
using Scryber.Text;
using Scryber.Drawing;

namespace Scryber.Styles
{
    /// <summary>
    /// Defines a single style with inner StyleItems that can be applied to an Component
    /// </summary>
    public class PDFStyle : PDFStyleBase
    {
        


        //
        //TODO: Add PDFLayoutstyle {display (none,hidden,inline,block), direction (l2r,r2l,t2b), column-count, column-gutter}
        //

        #region ID {get;set;} + UniqueID{get;}

        private string _id;

        /// <summary>
        /// Gets or sets the ID for this instance
        /// </summary>
        [PDFAttribute("id")]
        public string ID
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                {
                    _id = string.Empty;
                }
                return this._id;
            }
            set
            {
                _id = value;
            }
        }

        #endregion

        //
        // style item accessors
        //

        #region public PDFMarginsStyle Margins {get;}

        /// <summary>
        /// Gets the margins associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFMarginsStyle Margins
        {
            get
            {
                PDFMarginsStyle margin = this.GetStyleItem(PDFObjectTypes.StyleMargins,false) as PDFMarginsStyle;
                if(null == margin)
                {
                    margin = new PDFMarginsStyle();
                    this.Add(margin);
                }
                return margin;
            }
        }

        #endregion

        #region public PDFPaddingStyle Padding {get;}
        
        /// <summary>
        /// Gets the padding associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFPaddingStyle Padding
        {
            get
            {
                PDFPaddingStyle pad = this.GetStyleItem(PDFObjectTypes.StylePadding,false) as PDFPaddingStyle;
                if (null == pad)
                {
                    pad = new PDFPaddingStyle();
                    this.Add(pad);
                }
                return pad;
            }
        }

        #endregion

        #region public PDFBackgroundStyle Background {get;}

        /// <summary>
        /// Gets the background associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFBackgroundStyle Background
        {
            get
            {
                PDFBackgroundStyle style = this.GetStyleItem(PDFObjectTypes.StyleBackground, false) as PDFBackgroundStyle;
                if (null == style)
                {
                    style = new PDFBackgroundStyle();
                    this.Add(style);
                }
                return style;
            }
        }

        #endregion

        #region public PDFBorderStyle Border {get;}

        /// <summary>
        /// Gets the padding associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFBorderStyle Border
        {
            get
            {
                PDFBorderStyle bord = this.GetStyleItem(PDFObjectTypes.StyleBorder, false) as PDFBorderStyle;
                if (null == bord)
                {
                    bord = new PDFBorderStyle();
                    this.Add(bord);
                }
                return bord;
            }
        }

        #endregion

        #region public PDFFillStyle Fill {get;}

        /// <summary>
        /// Gets the fill associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFFillStyle Fill
        {
            get
            {
                PDFFillStyle f = this.GetStyleItem(PDFObjectTypes.StyleFill, false) as PDFFillStyle;
                if (null == f)
                {
                    f = new PDFFillStyle();
                    this.Add(f);
                }
                return f;
            }
        }

        #endregion

        #region public PDFFontStyle Font {get;}

        /// <summary>
        /// Gets the Font associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFFontStyle Font
        {
            get
            {
                PDFFontStyle f = this.GetStyleItem(PDFObjectTypes.StyleFont, false) as PDFFontStyle;
                if (null == f)
                {
                    f = new PDFFontStyle();
                    this.Add(f);
                }
                return f;
            }
        }

        #endregion

        #region public PDFPageStyle PageStyle {get;}

        /// <summary>
        /// Gets the page options associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFPageStyle PageStyle
        {
            get
            {
                PDFPageStyle pap = this.GetStyleItem(PDFObjectTypes.StylePage, false) as PDFPageStyle;
                if (null == pap)
                {
                    pap = new PDFPageStyle();
                    this.Add(pap);
                }
                return pap;
            }
        }

        #endregion

        #region public PDFPositionStyle Position {get;}

        /// <summary>
        /// Gets the position associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFPositionStyle Position
        {
            get
            {
                PDFPositionStyle pos = this.GetStyleItem(PDFObjectTypes.StylePosition, false) as PDFPositionStyle;
                if (null == pos)
                {
                    pos = new PDFPositionStyle();
                    this.Add(pos);
                }
                return pos;
            }
        }

        #endregion

        #region public PDFStrokeStyle Stroke {get;}

        /// <summary>
        /// Gets the stroke associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFStrokeStyle Stroke
        {
            get
            {

                PDFStrokeStyle str = this.GetStyleItem(PDFObjectTypes.StyleStroke, false) as PDFStrokeStyle;
                if (null == str)
                {
                    str = new PDFStrokeStyle();
                    this.Add(str);
                }
                return str;
            }
        }

        #endregion

        #region public PDFTextStyle Text {get;}

        /// <summary>
        /// Gets the text style associated with this style - if it is not currently defined a new item is created, added to this style then returned
        /// </summary>
        public PDFTextStyle Text
        {
            get
            {
                PDFTextStyle txt = this.GetStyleItem(PDFObjectTypes.StyleText, false) as PDFTextStyle;
                if (null == txt)
                {
                    txt = new PDFTextStyle();
                    this.Add(txt);
                }
                return txt;
            }
        }

        #endregion

        #region public PDFOverflowStyle Overflow {get;}

        public PDFOverflowStyle Overflow
        {
            get
            {
                PDFOverflowStyle over = this.GetStyleItem(PDFObjectTypes.StyleOverflow, false) as PDFOverflowStyle;
                if (null == over)
                {
                    over = new PDFOverflowStyle();
                    this.Add(over);
                }
                return over;
            }
        }

        #endregion

        #region public PDFOverflowStyle Overflow {get;}

        public PDFOutlineStyle Outline
        {
            get
            {
                PDFOutlineStyle outline = this.GetStyleItem(PDFObjectTypes.StyleOutline, false) as PDFOutlineStyle;
                if (null == outline)
                {
                    outline = new PDFOutlineStyle();
                    this.Add(outline);
                }
                return outline;
            }
        }

        #endregion

        #region protected PDFStyleItemCollection Items {get} + HasItems {get;}

        private PDFStyleItemCollection _items = null;

        /// <summary>
        /// Protected accessor to the style items defined in this style
        /// </summary>
        [PDFArray(typeof(PDFStyleItem))]
        [PDFElement("")]
        public PDFStyleItemCollection Items
        {
            get
            {
                if (this._items == null)
                    this._items = new PDFStyleItemCollection();
                return this._items;
            }
        }

        /// <summary>
        /// Returns true if this style has any defined style items
        /// </summary>
        public bool HasItems
        {
            get { return this._items != null && this._items.Count > 0; }
        }

        #endregion


        //
        // .ctor(s)
        //
        

        public PDFStyle()
            : this(PDFObjectTypes.Style)
        {
        }

        protected PDFStyle(PDFObjectType type)
            : base(type)
        {
        }

        /// <summary>
        /// Adds the specified item to this style
        /// </summary>
        /// <param name="item"></param>
        public void Add(PDFStyleItem item)
        {
            if (null == item)
                throw new ArgumentNullException("item");
            this.Items.Add(item);
        }

        /// <summary>
        /// removes the specified item from this style
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(PDFStyleItem item)
        {
            if (null == item)
                return false;
            else
                return this.Items.Remove(item);
        }

        /// <summary>
        /// Merges all style items into this 
        /// </summary>
        /// <param name="style"></param>
        public void MergeInto(PDFStyle style)
        {
            if (this.HasItems)
            {
                foreach (PDFStyleItem item in this.Items)
                {
                    style.MergeItem(item);
                }
            }
        }

        public override void MergeInto(PDFStyle style, IPDFComponent Component, ComponentState state)
        {
            if (this.HasItems)
            {
                foreach (PDFStyleItem item in this.Items)
                {
                    style.MergeItem(item);
                }
            }
        }

        public override PDFStyle MatchClass(string classname)
        {
            return null;
        }

        public virtual PDFStyle Flatten()
        {
            PDFStyle style = new PDFStyle();
            if (this.HasItems)
            {
                PDFUniqueStyle dict = new PDFUniqueStyle();
                for (int i = 0; i < this.Items.Count; i++)
                {
                    PDFStyleItem item = this.Items[i];
                    if (dict.Contains(item.Type) == false)
                        dict.Add(item);
                    else
                    {
                        PDFStyleItem exist = dict[item.Type];
                        item.MergeInto(exist);
                    }
                }

                PDFStyleItemCollection col = new PDFStyleItemCollection();
                col.AddRange(dict.Values);
                style._items = col;
            }
            return style;
        }

        protected virtual void MergeItem(PDFStyleItem item)
        {
            this.Items.Add(item.Clone());
        }

        public void MergeInherited(PDFStyle style, IPDFComponent Component, bool replace)
        {
            if (this.HasItems)
            {
                foreach (PDFStyleItem item in this.Items)
                {
                    if (item.IsInherited)
                        style.MergeItem(item);
                }
            }
        }

        public PDFStyleItem GetStyleItem(PDFObjectType type, bool flatten)
        {
            PDFStyleItem psi = null;

            foreach (PDFStyleItem item in this.Items)
            {
                if (item.Type == type)
                {
                    if (psi == null)
                    {
                        if (flatten)
                            psi = item.Clone();
                        else
                        {
                            psi = item;
                            break;
                        }
                    }
                    else if (flatten)
                        item.MergeInto(psi);
                }
            }
            return psi;
        }

        public bool IsDefined(PDFObjectType type)
        {
            foreach (PDFStyleItem si in this.Items)
            {
                if (si.Type == type)
                    return true;
            }
            return false;
        }

        public PDFStyle Clone()
        {
            PDFStyle style = base.MemberwiseClone() as PDFStyle;
            style.Items.Clear();
            foreach (PDFStyleItem si in this.Items)
            {
                style.Add(si.Clone());
            }
            return style;
        }



        public bool TryGetMargins(out PDFMarginsStyle margins)
        {
            margins = this.GetStyleItem(PDFObjectTypes.StyleMargins, true) as PDFMarginsStyle;
            return margins != null && margins.Count > 0;
        }

        public bool TryGetBackground(out PDFBackgroundStyle bg)
        {
            bg = this.GetStyleItem(PDFObjectTypes.StyleBackground, true) as PDFBackgroundStyle;
            return bg != null && bg.Count > 0;
        }

        public bool TryGetBorder(out PDFBorderStyle border)
        {
            border = this.GetStyleItem(PDFObjectTypes.StyleBorder, true) as PDFBorderStyle;
            return border != null && border.Count > 0;
        }

        public bool TryGetOverflow(out PDFOverflowStyle overflow)
        {
            overflow = this.GetStyleItem(PDFObjectTypes.StyleOverflow, true) as PDFOverflowStyle;
            return overflow != null && overflow.Count > 0;
        }

        public bool TryGetTextStyle(out PDFTextStyle text)
        {
            text = this.GetStyleItem(PDFObjectTypes.StyleText, true) as PDFTextStyle;
            return text != null && text.Count > 0;
        }

        public bool TryGetPaper(out PDFPageStyle paper)
        {
            paper = this.GetStyleItem(PDFObjectTypes.StylePage, true) as PDFPageStyle;
            return paper != null && paper.Count > 0;
        }

        public bool TryGetFill(out PDFFillStyle fill)
        {
            fill = this.GetStyleItem(PDFObjectTypes.StyleFill, true) as PDFFillStyle;
            return fill != null && fill.Count > 0;
        }

        public bool TryGetStroke(out PDFStrokeStyle stroke)
        {
            stroke = this.GetStyleItem(PDFObjectTypes.StyleStroke, true) as PDFStrokeStyle;
            return stroke != null && stroke.Count > 0;
        }

        public bool TryGetFont(out PDFFontStyle font)
        {
            font = this.GetStyleItem(PDFObjectTypes.StyleFont, true) as PDFFontStyle;
            return font != null && font.Count > 0;
        }

        public bool TryGetPosition(out PDFPositionStyle pos)
        {
            pos = this.GetStyleItem(PDFObjectTypes.StylePosition, true) as PDFPositionStyle;
            return pos != null && pos.Count > 0;
        }

        public bool TryGetPadding(out PDFPaddingStyle padding)
        {
            padding = this.GetStyleItem(PDFObjectTypes.StylePadding, true) as PDFPaddingStyle;
            return padding != null && padding.Count > 0;
        }

        public bool TryGetOutline(out PDFOutlineStyle outline)
        {
            outline = this.GetStyleItem(PDFObjectTypes.StyleOutline, true) as PDFOutlineStyle;
            return outline != null && outline.Count > 0;
        }

        public bool TryGetStyle(PDFObjectType type, out PDFStyleItem item)
        {
            item = this.GetStyleItem(type, true);
            return null != item && item.Count > 0;
        }

        public PDFTextRenderOptions CreateTextStyle()
        {
            return CreateTextStyle(this);
        }

        private static PDFTextRenderOptions CreateTextStyle(PDFStyle style)
        {
            PDFTextRenderOptions options = new PDFTextRenderOptions();
            Styles.PDFFontStyle font;
            if (style.TryGetFont(out font))
                options.Font = font.CreateFont();
            Styles.PDFFillStyle fill;
            if (style.TryGetFill(out fill))
                options.Brush = fill.CreateBrush();

            Styles.PDFBackgroundStyle bg;
            if (style.TryGetBackground(out bg))
                options.Background = bg.CreateBrush();

            Styles.PDFStrokeStyle stroke;
            if (style.TryGetStroke(out stroke))
                options.Stroke = stroke.CreatePen();

            Styles.PDFTextStyle txt;
            Styles.PDFPositionStyle pos = null;
            
            style.TryGetTextStyle(out txt);
            style.TryGetPosition(out pos);

            if(txt != null)
                options.Layout = txt.CreateLayout(pos);

            
            return options;
        }


        protected override void DoDataBind(PDFDataContext context, bool includechildren)
        {
            base.DoDataBind(context, includechildren);
            if (includechildren && this.HasItems)
                this.Items.DataBind(context);
        }

        //
        // inner classes
        //

        #region private class PDFUniqueStyle

        /// <summary>
        /// Inner private class to retain the ordering of Components in the style whilst flattening.
        /// </summary>
        private class PDFUniqueStyle : System.Collections.ObjectModel.KeyedCollection<PDFObjectType, PDFStyleItem>
        {
            protected override PDFObjectType GetKeyForItem(PDFStyleItem item)
            {
                return item.Type;
            }

            public IEnumerable<PDFStyleItem> Values
            {
                get { return this.Items; }
            }
        }

        #endregion
        
        
    }
}
