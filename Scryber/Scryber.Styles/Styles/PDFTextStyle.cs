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
    [PDFParsableComponent("Text")]
    public class PDFTextStyle : PDFStyleItem
    {
        //
        // constructors
        //

        #region .ctor() + .ctor(type)

        public PDFTextStyle()
            : this(PDFObjectTypes.StyleText, true)
        {
        }

        protected PDFTextStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion


        //
        // style properties
        //

        
        #region public PDFUnit FirstLineInset {get;set;} + RemoveFirstLineInset()

        [PDFAttribute("first-indent")]
        public PDFUnit FirstLineInset
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.FirstInsetAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.FirstInsetAttr, value.ToString(), value);
            }
        }

        public void RemoveFirstLineInset()
        {
            this.Remove(StyleKeys.FirstInsetAttr);
        }

        #endregion

        #region public float WordSpacing {get;set} + RemoveWordSpacing()

        [PDFAttribute("word-spacing")]
        public float WordSpacing
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.WordSpacingAttr, false, out f))
                    return f;
                else
                    return 1.0F;
            }
            set
            {
                this.SetValue(StyleKeys.WordSpacingAttr, value.ToString(), value);
            }
        }

        public void RemoveWordSpacing()
        {
            this.Remove(StyleKeys.WordSpacingAttr);
        }

        #endregion

        #region public bool WrapText {get;set;} + RemoveWrapText()

        [PDFAttribute("wrap")]
        public WordWrap WrapText
        {
            get
            {
                object f;
                if (this.GetEnumValue(StyleKeys.WrappingAttr, typeof(WordWrap), false, out f))
                    return (WordWrap)f;
                else
                    return WordWrap.Auto;
            }
            set
            {
                this.SetValue(StyleKeys.WrappingAttr, value.ToString(), value);
            }
        }

        public void RemoveWrapText()
        {
            this.Remove(StyleKeys.WrappingAttr);
        }

        #endregion

        #region public PDFUnit Leading {get;set;} + RemoveLeadingText()

        [PDFAttribute("leading")]
        public PDFUnit Leading
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.FontLeadingAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Zero;
            }
            set
            {
                this.SetValue(StyleKeys.FontLeadingAttr, value.ToString(), value);
            }
        }

        public void RemoveLeading()
        {
            this.Remove(StyleKeys.FontLeadingAttr);
        }

        #endregion


        /// <summary>
        /// Creates a PDFTextLayout based upon the style properties in this instance and the 
        /// associated alignment properties in the position parameter (if any).
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PDFTextLayout CreateLayout(PDFPositionStyle position)
        {
            return Create(this, position);
        }

        
        protected static PDFTextLayout Create(PDFTextStyle txt, PDFPositionStyle pos)
        {
            PDFTextLayout layout = new PDFTextLayout();
            if (txt != null)
            {
                layout.FirstLineInset = txt.FirstLineInset;
                layout.WordSpacing = txt.WordSpacing;
                layout.WrapText = txt.WrapText;
                layout.Leading = txt.Leading;
            }
            if (pos != null)
            {
                layout.HAlign = pos.HAlign;
                layout.VAlign = pos.VAlign;
            }
            return layout;
        }
    }
}
