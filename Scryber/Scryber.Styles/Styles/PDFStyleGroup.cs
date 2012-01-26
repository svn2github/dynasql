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
    public class PDFStyleGroup : PDFStyleBase, IEnumerable<PDFStyleBase>
    {
        private PDFStyleCollection _innerItems;

        protected PDFStyleCollection InnerItems
        {
            get 
            {
                if (this._innerItems == null)
                    this._innerItems = CreateInnerStyles();
                return this._innerItems;
            }
        }

        protected virtual PDFStyleCollection CreateInnerStyles()
        {
            return new PDFStyleCollection();
        }

        protected virtual void ClearInnerStyles()
        {
            this._innerItems = null;
        }


        public PDFStyleGroup()
            : this(PDFObjectTypes.StyleGroup)
        {
        }

        public PDFStyleGroup(PDFObjectType type)
            : base(type)
        {
        }

        #region IEnumerable<PDFStyleBase> Members

        public IEnumerator<PDFStyleBase> GetEnumerator()
        {
            return this.InnerItems.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public override void MergeInto(PDFStyle style, IPDFComponent Component, ComponentState state)
        {
            foreach (PDFStyle def in this.InnerItems)
            {
                def.MergeInto(style, Component, state);
            }
        }

        public override PDFStyle MatchClass(string classname)
        {
            return this.InnerItems.MatchClass(classname);
        }

        protected override void DoDataBind(PDFDataContext context, bool includechildren)
        {
            base.DoDataBind(context, includechildren);
            if (includechildren && this.InnerItems != null && this.InnerItems.Count > 0)
            {
                foreach (PDFStyleBase sb in this.InnerItems)
                {
                    sb.DataBind(context);
                }
            }
        }
    }
}
