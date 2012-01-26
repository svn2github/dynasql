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

namespace Scryber
{
    public abstract class PDFAttributeCollection : PDFObject, IDictionary<string,string>
    {
        protected PDFAttributeCollection(PDFObjectType type) : base(type)
        {
        }

        private Dictionary<string, string> _attributes = new Dictionary<string, string>();
        
        #region IDictionary<string,string> Members

        public void Add(string key, string value)
        {
            this._attributes.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this._attributes.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return this._attributes.Keys; }
        }

        public bool Remove(string key)
        {
            return this._attributes.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return this._attributes.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return this._attributes.Values; }
        }

        public string this[string key]
        {
            get
            {
                string value;
                if (this._attributes.TryGetValue(key, out value))
                    return value;
                else
                    return String.Empty;
                
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this._attributes.Remove(value);
                }
                else
                    this._attributes[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        void ICollection<KeyValuePair<string,string>>.Add(KeyValuePair<string, string> item)
        {
            ((ICollection<KeyValuePair<string, string>>)this._attributes).Add(item);
        }

        public void Clear()
        {
            this._attributes.Clear();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>)this._attributes).Contains(item);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, string>>)this._attributes).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._attributes.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string,string>>)this._attributes).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>)this._attributes).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this._attributes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
