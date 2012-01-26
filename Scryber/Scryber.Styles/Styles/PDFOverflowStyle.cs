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
    [PDFParsableComponent("Overflow")]
    public class PDFOverflowStyle : PDFStyleItem
    {
        
        //
        // constructors
        //

        #region .ctor() + .ctor(type, inherited)

        public PDFOverflowStyle()
            : this(PDFObjectTypes.StyleOverflow,true)
        {
        }

        protected PDFOverflowStyle(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #endregion

        //
        // style properties
        //

        #region public OverflowAction Action {get;set;} + RemoveAction()

        [PDFAttribute("action")]
        public OverflowAction Action
        {
            get
            {
                object act;
                if (this.GetEnumValue(StyleKeys.OverflowActionAttr, typeof(OverflowAction), false, out act))
                    return (OverflowAction)act;
                else
                    return OverflowAction.None;
            }
            set
            {
                this.SetValue(StyleKeys.OverflowActionAttr, value.ToString(), value);
            }
        }

        public void RemoveAction()
        {
            this.Remove(StyleKeys.OverflowActionAttr);
        }

        #endregion

        #region public OverflowSplit Split {get;set;}

        [PDFAttribute("split")]
        public OverflowSplit Split
        {
            get
            {
                object split;
                if (this.GetEnumValue(StyleKeys.OverflowSplitAttr, typeof(OverflowSplit), false, out split))
                    return (OverflowSplit)split;
                else
                    return OverflowSplit.Any;
            }
            set
            {
                this.SetValue(StyleKeys.OverflowSplitAttr, value.ToString(), value);
            }
        }

        public void RemoveSplit()
        {
            this.Remove(StyleKeys.OverflowSplitAttr);
        }

        #endregion
    }
}
