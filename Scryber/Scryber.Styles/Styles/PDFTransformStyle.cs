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

namespace Scryber.Styles
{
    [PDFParsableComponent("Transform")]
    public class PDFTransformStyle : PDFStyleItem
    {
        

        //
        // constructors
        //

        #region .ctor

        public PDFTransformStyle()
            : this(PDFObjectTypes.StyleTransform, false)
        {
        }

        #endregion

        #region .ctor(type)

        protected PDFTransformStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion 

        //
        // style properties
        //

        #region public float ScaleX {get;set;} + RemoveScaleX()

        [PDFAttribute("scale-x")]
        public float ScaleX
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.ScaleXAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.ScaleXAttr, value);
            }
        }

        public void RemoveScaleX()
        {
            this.Remove(StyleKeys.ScaleXAttr);
        }

        #endregion

        #region public float ScaleY {get;set;} + RemoveScaleY()

        [PDFAttribute("scale-y")]
        public float ScaleY
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.ScaleYAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.ScaleYAttr, value);
            }
        }

        public void RemoveScaleY()
        {
            this.Remove(StyleKeys.ScaleYAttr);
        }

        #endregion

        #region public float Rotate {get;set;} + RemoveRotate()

        [PDFAttribute("rotate")]
        public float Rotate
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.RotateAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.RotateAttr, value);
            }
        }

        public void RemoveRotate()
        {
            this.Remove(StyleKeys.RotateAttr);
        }

        #endregion

        #region public float SkewX {get;set;} + RemoveSkewX()

        [PDFAttribute("skew-x")]
        public float SkewX
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.SkewXAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.SkewXAttr, value);
            }
        }

        public void RemoveSkewX()
        {
            this.Remove(StyleKeys.SkewXAttr);
        }

        #endregion

        #region public float SkewY {get;set;} + RemoveSkewY()

        [PDFAttribute("skew-y")]
        public float SkewY
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.SkewYAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.SkewYAttr, value);
            }
        }

        public void RemoveSkewY()
        {
            this.Remove(StyleKeys.SkewYAttr);
        }

        #endregion

        #region public float OffsetH {get;set;} + RemoveOffsetH()

        [PDFAttribute("offset-h")]
        public float OffsetH
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.OffsetHAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.OffsetHAttr, value);
            }
        }

        public void RemoveOffsetH()
        {
            this.Remove(StyleKeys.OffsetHAttr);
        }

        #endregion

        #region public float OffsetV {get;set;} + RemoveOffsetV()

        [PDFAttribute("offset-v")]
        public float OffsetV
        {
            get
            {
                float f;
                if (this.GetFloatValue(StyleKeys.OffsetVAttr, false, out f))
                    return f;
                else
                    return 0.0F;

            }
            set
            {
                this.SetValue(StyleKeys.OffsetVAttr, value);
            }
        }

        public void RemoveOffsetV()
        {
            this.Remove(StyleKeys.OffsetVAttr);
        }

        #endregion

    }
}
