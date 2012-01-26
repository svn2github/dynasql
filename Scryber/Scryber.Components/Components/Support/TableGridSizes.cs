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
using Scryber.Drawing;

namespace Scryber.Components.Support
{
    internal class TableGridSizes
    {
        private PDFUnit[] _widths;
        private PDFUnit[] _heights;
        private bool _lock;

        /// <summary>
        /// Gets a flag that specifies if this set of sizes are locked (cannot be modified)
        /// Once locked (via the Lock() method) a set cannot be modified
        /// </summary>
        internal bool Locked
        {
            get { return _lock; }
        }

        

        internal TableGridSizes(int width, int height)
        {
            _widths = FillEmpty(width);
            _heights = FillEmpty(height);
        }

        internal PDFUnit GetWidth(int colIndex)
        {
            Validate(colIndex, _widths, "column");
            return _widths[colIndex];
        }

        internal PDFUnit GetHeight(int rowIndex)
        {
            Validate(rowIndex, _heights, "row");
            return _heights[rowIndex];
        }

        internal bool HasWidth(int colIndex)
        {
            Validate(colIndex, _widths, "column");
            return _widths[colIndex] >= 0;
        }

        internal bool HasHeight(int rowIndex)
        {
            Validate(rowIndex, _heights, "row");
            return _heights[rowIndex] >= 0;
        }

        internal void SetWidth(int colIndex, PDFUnit w)
        {
            AssertNotLocked();
            Validate(colIndex, _widths, "column");
            _widths[colIndex] = w;
        }

        internal void SetHeight(int rowIndex, PDFUnit h)
        {
            AssertNotLocked();
            Validate(rowIndex, _heights, "row");
            _heights[rowIndex] = h;
        }

        internal PDFUnit GetAssignedWidth(out int[] unassigned)
        {
            List<int> all = new List<int>();
            PDFUnit w = PDFUnit.Zero;
            for (int i = 0; i < this._widths.Length; i++)
            {
                if (this.HasWidth(i))
                    w += this.GetWidth(i);
                else
                    all.Add(i);
            }
            unassigned = all.ToArray();
            return w;
        }

        internal PDFUnit GetAssignedHeight(out int[] unassigned)
        {
            List<int> all = new List<int>();
            PDFUnit h = PDFUnit.Zero;
            for (int i = 0; i < this._heights.Length; i++)
            {
                if (this.HasHeight(i))
                    h += this.GetHeight(i);
                else
                    all.Add(i);
            }
            unassigned = all.ToArray();
            return h;
        }

        internal void Lock()
        {
            this._lock = true;
        }

        private void AssertNotLocked()
        {
            if (_lock)
                throw RecordAndRaise.Operation(Errors.CannotModifyTableGridSizes);
        }

        private static void Validate(int index, PDFUnit[] sizes, string dimension)
        {
            if (index < 0 || index > sizes.Length)
                throw RecordAndRaise.ArgumentOutOfRange("index", Errors.CannotAccessGridOffsetInTable, index, dimension, sizes.Length);
        }

        private static PDFUnit[] FillEmpty(int count)
        {
            PDFUnit[] all = new PDFUnit[count];
            for (int i = 0; i < count; i++)
            {
                all[i] = -1;
            }
            return all;
        }


    }
}
