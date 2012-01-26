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

namespace Scryber.Styles
{
    [PDFParsableComponent("Clip")]
    public class PDFClipStyle : PDFStyleItem
    {

        //
        // constructors
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFClipStyle()
            : this(PDFObjectTypes.StyleClip, false)
        {
        }

        protected PDFClipStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion 

        //
        // style properties
        //

        #region public PositionMode ClipMode {get;set;} + RemoveClipMode()

        [PDFAttribute("mode")]
        public PositionMode ClipMode
        {
            get
            {
                object val;
                if (GetEnumValue(StyleKeys.PositionModeAttr, typeof(PositionMode), false, out val))
                    return (PositionMode)val;
                else
                    return PositionMode.Relative;
            }
            set
            {
                this.SetValue(StyleKeys.PositionModeAttr, value.ToString(), value);
            }
        }

        public void RemoveClipMode()
        {
            this.Remove(StyleKeys.PositionModeAttr);
        }

        #endregion

        #region public PDFUnit X {get;set;} + RemoveX()

        [PDFAttribute("x")]
        public PDFUnit X
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.XAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.XAttr, value);
            }
        }

        public void RemoveX()
        {
            this.Remove(StyleKeys.XAttr);
        }

        #endregion

        #region public PDFUnit Y {get;set;} + RemoveY()

        [PDFAttribute("y")]
        public PDFUnit Y
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.YAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.YAttr, value);
            }
        }

        public void RemoveY()
        {
            this.Remove(StyleKeys.YAttr);
        }

        #endregion

        #region public PDFUnit Width {get;set;} + RemoveWidth()

        [PDFAttribute("width")]
        public PDFUnit Width
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.WidthAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.WidthAttr, value);
            }
        }

        public void RemoveWidth()
        {
            this.Remove(StyleKeys.WidthAttr);
        }

        #endregion

        #region public PDFUnit Height {get;set;} + RemoveHeight()

        [PDFAttribute("height")]
        public PDFUnit Height
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.HeightAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;

            }
            set
            {
                this.SetValue(StyleKeys.HeightAttr, value);
            }
        }

        public void RemoveHeight()
        {
            this.Remove(StyleKeys.HeightAttr);
        }

        #endregion

    }
}
