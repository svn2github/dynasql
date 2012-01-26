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
    public abstract class PDFMergedListStyleItem : PDFStyleItem
    {
        protected PDFMergedListStyleItem(PDFObjectType type, bool inherited)
            : base(type, inherited)
        {
        }

        #region Transform Linked List Ops

        private PDFMergedListStyleItem _next = null;

        public PDFMergedListStyleItem Next
        {
            get { return this._next; }
            set 
            {
                this.ValidateCircular(value);
                this._next = value;
            }
        }

        private void ValidateCircular(PDFMergedListStyleItem value)
        {
            PDFMergedListStyleItem parent = this;
            do
            {
                if (parent == value)
                    throw new ArgumentException("Cannot set the next value to the same as one of the parents. This would create a circular reference and cause never ending loops");
                parent = null;
            } while (parent != null);
        }

        public bool HasNext
        {
            get { return this._next != null; }
        }

        public PDFMergedListStyleItem Last
        {
            get
            {
                PDFMergedListStyleItem last = this;
                while (last.Next != null)
                {
                    if (last.Next == this)
                        throw new InvalidOperationException("Circular reference in MergedStyleItem, the Next item cannot be the same as the last");

                    last = last.Next;
                }
                return last;
            }
        }

        #endregion

        #region public override void MergeInto(PDFStyleItem si)

        public override void MergeInto(PDFStyleItem si)
        {
            if (si is PDFMergedListStyleItem && si.Type == this.Type)
            {
                PDFMergedListStyleItem other = si as PDFMergedListStyleItem;
                other.Last.Next = this;
            }
            else if (si == null)
                throw new ArgumentNullException("si",String.Format(Errors.CanOnlyMergeItemsOfSameType, StyleKeys.TransformItem, "NULL"));
            else
                throw new ArgumentException(String.Format(Errors.CanOnlyMergeItemsOfSameType, StyleKeys.TransformItem, si.Type),"si");

            //base.MergeInto(si);
        }

        #endregion

    }
}
