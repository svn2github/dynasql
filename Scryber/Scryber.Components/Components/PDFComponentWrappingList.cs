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

namespace Scryber.Components
{
    public abstract class PDFComponentWrappingList
    {
        private PDFComponentList _inner;
        public PDFComponentList InnerList
        {
            get { return _inner; }
        }

        public PDFComponentWrappingList(PDFComponentList inner)
        {
            if (null == inner)
                throw RecordAndRaise.ArgumentNull("innerList");
            this._inner = inner;
        }

    }
    public abstract class PDFComponentWrappingList<T> : PDFComponentWrappingList, ICollection<T> where T : PDFComponent
    {
        

        public PDFComponentWrappingList(PDFComponentList innerList)
            : base(innerList)
        {
        }

        public T this[int index]
        {
            get { return (T)this.InnerList[index]; }
        }

        public T this[string id]
        {
            get { return (T)this.InnerList[id]; }
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            this.InnerList.Add(item);
        }

        public void Clear()
        {
            this.InnerList.Clear();
        }

        public bool Contains(T item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.InnerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.InnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return this.InnerList.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return this.InnerList.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new WrappedEnumerator(this.InnerList.GetEnumerator());
        }

        private class WrappedEnumerator : IEnumerator<T>
        {
            private IEnumerator<PDFComponent> _innerEnumerator;

            public WrappedEnumerator(IEnumerator<PDFComponent> inner)
            {
                this._innerEnumerator = inner;
            }



            #region IEnumerator<T> Members

            public T Current
            {
                get { return (T)this._innerEnumerator.Current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this._innerEnumerator.Dispose();
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this._innerEnumerator.Current; }
            }

            public bool MoveNext()
            {
                bool b;
                do
                {
                    b = this._innerEnumerator.MoveNext();
                    if (b == false)
                        break;
                }
                while (this._innerEnumerator.Current.Type == PDFObjectTypes.NoOp);
                return b;
            }

            public void Reset()
            {
                this._innerEnumerator.Reset();
            }

            #endregion
        }

        #endregion



        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        #endregion
    }
}
