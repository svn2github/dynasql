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
    public class PDFStyleItemCollection : ICollection<PDFStyleItem>
    {
        private List<PDFObject> _items = new List<PDFObject>();
        
        protected List<PDFObject> Items
        {
            get { return _items; }
        }

        public PDFStyleItem this[int index]
        {
            get { return this.Items[index] as PDFStyleItem; }
            set { this[index] = value; }
        }

        #region ICollection<PDFStyleItem> Members

        public void Add(PDFStyleItem item)
        {
            this.Items.Add(item);
        }

        public void AddRange(IEnumerable<PDFStyleItem> all)
        {
            foreach (PDFStyleItem item in all)
            {
                this.Items.Add(item);
            }
        }

        public void Clear()
        {
            this.Clear();
        }

        public bool Contains(PDFStyleItem item)
        {
            return this.Items.Contains(item);
        }

        public void CopyTo(PDFStyleItem[] array, int arrayIndex)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                PDFObject obj = this.Items[i];
                if (obj is PDFStyleItem)
                    array[arrayIndex] = (PDFStyleItem)obj;
                else
                    array[arrayIndex] = null;
                arrayIndex++;
            }
        }

        public int Count
        {
            get { return this.Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PDFStyleItem item)
        {
            return this.Items.Remove(item);
        }

        #endregion

        public void DataBind(PDFDataContext context)
        {
            foreach (PDFObject obj in this.Items)
            {
                if (obj is IPDFBindableComponent)
                    ((IPDFBindableComponent)obj).DataBind(context);
            }
        }

        #region IEnumerable<PDFStyleItem> Members

        public IEnumerator<PDFStyleItem> GetEnumerator()
        {
            return new StyleItemEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private class StyleItemEnumerator : IEnumerator<PDFStyleItem>
        {
            private IEnumerator<PDFObject> _inner;

            public StyleItemEnumerator(PDFStyleItemCollection col)
            {
                _inner = col.Items.GetEnumerator();
            }

            #region IEnumerator<PDFStyleItem> Members

            public PDFStyleItem Current
            {
                get
                {
                    return (PDFStyleItem)_inner.Current;
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {

                while (_inner.MoveNext())
                {
                    if (_inner.Current is PDFStyleItem)
                        return true;
                }
                return false;
            }

            public void Reset()
            {
                _inner.Reset();
            }

            #endregion
        }

        
    }

}
