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
    /// <summary>
    /// A collection of objects that is unique to a PDFDocument, 
    /// but is accessible in any context of the document lifecycle (Init, Load, Databind, Layout and Render)
    /// </summary>
    public class PDFItemCollection : System.Collections.Specialized.NameObjectCollectionBase
    {
        /// <summary>
        /// Creates a new empty instance of the PDFItemCollection
        /// </summary>
        public PDFItemCollection()
        {
        }

        /// <summary>
        /// Creates a new instance of the PDFItemCollection and adds the specified items to the collection
        /// </summary>
        /// <param name="contents"></param>
        public PDFItemCollection(IDictionary<string, object> contents)
            : this()
        {
            if (null != contents)
            {
                foreach (string str in contents.Keys)
                {
                    this.Add(str, contents[str]);
                }
            }
        }

        public object this[int index]
        {
            get { return this.BaseGet(index); }
        }

        public object this[string name]
        {
            get { return this[name]; }
        }

        public int Add(string name, object value)
        {
            int index = this.Count;
            this.BaseAdd(name, value);
            return index;
        }

        public void Remove(string name)
        {
            this.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Clear()
        {
            this.BaseClear();
        }
    }
}
