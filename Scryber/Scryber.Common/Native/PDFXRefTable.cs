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

namespace Scryber.Native
{
    /// <summary>
    /// Implements the data storage for writing the XRef table in a PDF file
    /// </summary>
    public class PDFXRefTable
    {
        
        private int _gen;
        private long _offset;
        private List<IIndirectObject> _references;

        /// <summary>
        /// Gets the current generation of the XRefTable
        /// </summary>
        public int Generation
        {
            get { return _gen; }
        }

        
        //Gets or Sets the postion of the XRefTable in the file
        public long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        
        /// <summary>
        /// A List of all the IIndirectObjects in this generation of the PDF File
        /// </summary>
        public List<IIndirectObject> References
        {
            get { return this._references; }
        }


        public PDFXRefTable(int generation)
        {
            this._gen = generation;
            this._references = new List<IIndirectObject>();
            this._references.Add(new EmptyRef(0, generation, 0));
        }

        /// <summary>
        /// Adds an indirect object, and in turn setting its number and generation
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns>The added objects number</returns>
        public int Add(IIndirectObject obj)
        {
            obj.Number = _references.Count;
            this._references.Add(obj);
            obj.Generation = this.Generation;
            obj.Offset = -1;
            return obj.Number;
        }

        /// <summary>
        /// Removes the current IIndirectObject and fills the space with an empty item
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(IIndirectObject obj)
        {
            this.References[obj.Number] = new EmptyRef(obj.Number,this.Generation,obj.Offset);
        }


        #region private class EmptyRef : IIndirectObject

        /// <summary>
        /// An empty ref is a placeholder for a cell in the table that used to contain a reference, but it was removed
        /// </summary>
        private class EmptyRef : IIndirectObject
        {

            public EmptyRef(int num, int gen, long offset)
            {
                this._num = num;
                this._gen = gen;
                this._off = offset;
            }

            #region IIndirectObject Members

            private int _num;
            public int Number
            {
                get
                {
                    return this._num;
                }
                set
                {
                    
                }
            }

            private int _gen;
            public int Generation
            {
                get
                {
                    return this._gen;
                }
                set
                {
                   
                }
            }

            private long _off;
            public long Offset
            {
                get
                {
                    return _off;
                }
                set
                {
                    
                }
            }

            public PDFStream ObjectData
            {
                get { return null; }
            }

            public void WriteData(PDFWriter writer)
            {
                writer.WriteCommentLine("This cell is empty : {0} {1} R", this.Number, this.Generation);
            }

            public bool HasStream { get { return false; } }

            public PDFStream Stream { get { return null; } }

            public bool Deleted
            {
                get { return true; }
            }

            public byte[] GetObjectData() { return new byte[] {}; }

            public byte[] GetStreamData() { return new byte[] {}; }

            #endregion

            public void Dispose() { }

            public override string ToString()
            {
                return "[NULL]";
            }
        }

        #endregion

    }
}
