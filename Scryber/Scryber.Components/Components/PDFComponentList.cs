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
using System.Collections.ObjectModel;
using Scryber.Native;

namespace Scryber.Components
{
    public class PDFComponentList : PDFObject, ICollection<PDFComponent>, IPDFComponentList, IDisposable
    {
        #region protected class InnerComponentList

        protected class InnerComponentList : KeyedCollection<string, PDFComponent>
        {
            internal PDFComponentList owner;

            public InnerComponentList(PDFComponentList owner)
            {
                this.owner = owner;
            }

            protected override string GetKeyForItem(PDFComponent item)
            {
                string s = item.ID;
                if (String.IsNullOrEmpty(s))
                {
                    s = this.owner.GetNextID(item);
                }
                return s;
            }

            public bool TryGetComponent(string id, out PDFComponent Component)
            {
                if (this.Dictionary != null)
                    return this.Dictionary.TryGetValue(id, out Component);
                else
                {
                    Component = null;
                    return false;
                }
            }



            internal PDFComponent[] ToArray()
            {
                PDFComponent[] arry = new PDFComponent[this.Count];
                this.Items.CopyTo(arry, 0);
                return arry;
            }

            internal void RegisterKeyChange(PDFComponent child)
            {
                this.ChangeItemKey(child, child.ID);
            }

            internal string GetKeyForComponent(PDFComponent ele)
            {
                return this.GetKeyForItem(ele);
            }
        }

        #endregion

        private int _noopcount;

        public int NoOpCount
        {
            get { return this._noopcount; }
        }

        protected void IncrementNoOps()
        {
            this._noopcount++;
        }

        protected void DecrementNoOps()
        {
            if ((--this._noopcount) < 0)
                throw RecordAndRaise.ArgumentOutOfRange(Errors.NoOpCountOutOfRange);
        }

        private PDFComponent _owner;
        public PDFComponent Owner
        {
            get { return _owner; }
        }
        
        private string GetNextID(PDFComponent forComponent)
        {
            return this.Owner.GetIncrementID(forComponent.Type);
        }

        private InnerComponentList _items;

        protected InnerComponentList Items
        {
            get { return this._items; }
        }

        public PDFComponentList(PDFComponent owner, PDFObjectType Componenttype)
            : base(Componenttype)
        {
            this._owner = owner;
            this._items = new InnerComponentList(this);
            this._noopcount = 0;
        }

        public PDFComponent this[int index]
        {
            get
            {
                PDFComponent Component;
                if (index > -1 && index < Count)
                    Component = this.Items[index];
                else
                    Component = null;

                return Component;
            }
        }

        public PDFComponent this[string id]
        {
            get
            {
                PDFComponent Component;
                this.Items.TryGetComponent(id, out Component);
                return Component;
            }
        }

        public void AddRange(IEnumerable<PDFComponent> items)
        {
            
            foreach (PDFComponent item in items)
            {
                this.Add(item);
            }
        }

        public int IndexOf(PDFComponent Component)
        {
            return this.Items.IndexOf(Component);
        }

        public void Insert(int index, PDFComponent Component)
        {
            this.Items.Insert(index, Component);
            Component.Parent = this.Owner;
            if (Component.Type == PDFObjectTypes.NoOp)
                this.IncrementNoOps();
        }

        #region ICollection<PDFComponent> Members

        public void Add(PDFComponent item)
        {
            item.Parent = this.Owner;
            this.Items.Add(item);
            if (item.Type == PDFObjectTypes.NoOp)
                this.IncrementNoOps();
        }

        public void Clear()
        {
            foreach (PDFComponent Component in this.Items)
            {
                if (Component.Parent == this.Owner)
                    Component.Parent = null;
            }
            this.Items.Clear();
            this._noopcount = 0;
        }

        public bool Contains(PDFComponent item)
        {
            return this.Items.Contains(item);
        }

        public void CopyTo(PDFComponent[] array, int arrayIndex)
        {
            this.Items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(PDFComponent item)
        {
            bool b = this.Items.Remove(item);
            if (b)
            {
                if (this.Owner.Equals(item.Parent))
                {
                    item.Parent = null;
                }
                if (item.Type == PDFObjectTypes.NoOp)
                    this.DecrementNoOps();
            }
            return b;
        }

        #endregion


        #region IEnumerable<PDFComponent> Members

        public IEnumerator<PDFComponent> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (PDFComponent item in this.Items)
                {
                    IDisposable id = item as IDisposable;

                    if(id != null)
                        id.Dispose();
                }
            }
        }

        ~PDFComponentList()
        {
            this.Dispose(false);
        }

        #endregion


        public void DataBind(PDFDataContext context)
        {
            PDFComponent[] all = this.Items.ToArray();
            foreach (PDFObject obj in all)
            {
                if (obj is IPDFBindableComponent)
                    ((IPDFBindableComponent)obj).DataBind(context);
            }
        }

        public void RegisterArtefacts(PDFRegistrationContext context)
        {
            PDFComponent[] all = this.Items.ToArray();
            foreach (PDFObject obj in all)
            {
                if (obj is PDFComponent)
                    ((PDFComponent)obj).RegisterArtefacts(context);
            }
        }
        
        //
        //Arrangement methods
        //

        internal PDFComponent[] ToArray()
        {
            return this.Items.ToArray();
        }

        internal void ChangeChildID(PDFComponent child)
        {
            if (this.Items.Contains(child))
                this.Items.RegisterKeyChange(child);
        }


        #region IPDFComponentList Members

        void IPDFComponentList.Insert(int index, IPDFComponent component)
        {
            if (component is PDFComponent)
                this.Insert(index, (PDFComponent)component);
            else
                RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, component.GetType(), "PDFComponent");
        }

        #endregion

        #region IEnumerable<IPDFComponent> Members

        IEnumerator<IPDFComponent> IEnumerable<IPDFComponent>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
    
    
}
