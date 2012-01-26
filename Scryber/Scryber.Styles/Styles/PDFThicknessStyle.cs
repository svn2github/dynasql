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
    public abstract class PDFThicknessStyle : PDFStyleItem
    {

        //
        // constructors
        //

        #region .ctor(type)

        public PDFThicknessStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion 

        //
        // style properties
        //

        #region public PDFUnit Left {get;set;} + RemoveLeft()

        [PDFAttribute("left")]
        public PDFUnit Left
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.LeftAttr, false, out f))
                    return f;
                else
                    return this.All;

            }
            set
            {
                this.SetValue(StyleKeys.LeftAttr, value);
            }
        }

        public void RemoveLeft()
        {
            this.Remove(StyleKeys.LeftAttr);
        }

        #endregion

        #region public PDFUnit Top {get;set;} + RemoveTop()

        [PDFAttribute("top")]
        public PDFUnit Top
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.TopAttr, false, out f))
                    return f;
                else
                    return this.All;

            }
            set
            {
                this.SetValue(StyleKeys.TopAttr, value);
            }
        }

        public void RemoveTop()
        {
            this.Remove(StyleKeys.TopAttr);
        }

        #endregion

        #region public PDFUnit Right {get;set;} + RemoveRight()

        [PDFAttribute("right")]
        public PDFUnit Right
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.RightAttr, false, out f))
                    return f;
                else
                    return this.All;

            }
            set
            {
                this.SetValue(StyleKeys.RightAttr, value);
            }
        }

        public void RemoveRight()
        {
            this.Remove(StyleKeys.RightAttr);
        }

        #endregion

        #region public PDFUnit Bottom {get;set;} + RemoveBottom()

        [PDFAttribute("bottom")]
        public PDFUnit Bottom
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.BottomAttr, false, out f))
                    return f;
                else
                    return this.All; 

            }
            set
            {
                this.SetValue(StyleKeys.BottomAttr, value);
            }
        }

        public void RemoveBottom()
        {
            this.Remove(StyleKeys.BottomAttr);
        }

        #endregion

        #region public PDFUnit All {get;set;} + RemoveAll()

        [PDFAttribute("all")]
        public PDFUnit All
        {
            get
            {
                PDFUnit f;
                if (this.GetUnitValue(StyleKeys.AllAttr, false, out f))
                    return f;
                else
                    return PDFUnit.Empty;
            }
            set
            {
                this.SetValue(StyleKeys.AllAttr, value);
            }

        }

        public void RemoveAll()
        {
            this.Remove(StyleKeys.AllAttr);
        }


        #endregion

        public PDFThickness GetThickness()
        {
            PDFThickness t = new PDFThickness(this.Top, this.Left, this.Bottom, this.Right);
            return t;
        }
    }
}
