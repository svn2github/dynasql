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
using System.Linq;
using System.Text;
using System.Collections;

namespace Scryber
{
    public class PDFDataStack : IEnumerable
    {
        private Stack<object> stack = new Stack<object>();

        public bool HasData
        {
            get { return this.stack != null && this.stack.Count > 0; }
        }

        public IEnumerator GetEnumerator()
        {
            return this.stack.GetEnumerator();
        }


        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.HasData)
            {
                foreach (object item in stack)
                {
                    if (item is IDisposable)
                        (item as IDisposable).Dispose();
                }
                this.stack = null;
            }
        }

        public void Push(object data)
        {
            this.stack.Push(data);
        }

        public object Pop()
        {
            return this.stack.Pop();
        }

        public object Current
        {
            get { return this.stack.Peek(); }
        }
    }
}
