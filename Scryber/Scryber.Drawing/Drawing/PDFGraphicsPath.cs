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

namespace Scryber.Drawing
{
    /// <summary>
    /// Contains path drawing operations
    /// </summary>
    public class PDFGraphicsPath : PDFObject
    {
        
        private List<Path> _paths = new List<Path>();
        private Stack<Path> _stack = new Stack<Path>();
        private PDFRect _bounds;
        private GraphicFillMode _mode = GraphicFillMode.Winding;

        internal GraphicFillMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        internal IEnumerable<Path> SubPaths
        {
            get { return _paths; }
        }

        private Path CurrentPath
        {
            get { return _stack.Peek(); }
        }

        /// <summary>
        /// Gets the bounds of this path
        /// </summary>
        public PDFRect Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Creates a new empty graphics path, ready to start adding operations to.
        /// </summary>
        public PDFGraphicsPath()
            : this(PDFObjectTypes.GraphicsPath)
        {

        }

        /// <summary>
        /// Creates a new empty graphics path, ready to start adding operations to.
        /// </summary>
        protected PDFGraphicsPath(PDFObjectType type)
            : base(type)
        {
            Path p = new Path();
            _paths = new List<Path>();
            _paths.Add(p);

            _stack = new Stack<Path>();
            _stack.Push(p);
            _bounds = PDFRect.Empty;
        }

        public void MoveTo(PDFPoint start)
        {
            PathMoveData move = new PathMoveData();
            move.MoveTo = start;
            CurrentPath.Add(move);
            IncludeInBounds(start);
        }

        public void LineTo(PDFPoint end)
        {
            PathLineData line = new PathLineData();
            line.LineTo = end;
            CurrentPath.Add(line);
            IncludeInBounds(end);
        }

        /// <summary>
        /// Closes the current path (drawing a line from the current point to the start point if the path is not already closed)
        /// And then ends the path if 'end' is true.
        /// </summary>
        public void ClosePath(bool end)
        {
            PathCloseData close = new PathCloseData();
            CurrentPath.Add(close);
            if(end)
                this.EndPath();
        }

        public void ArcTo(PDFPoint end, PDFPoint handleStart, PDFPoint handleEnd)
        {
            PathBezierCurveData arc = new PathBezierCurveData(end, handleStart, handleEnd, true, true);
            CurrentPath.Add(arc);
            IncludeInBounds(end);
            IncludeInBounds(handleStart);
            IncludeInBounds(handleEnd);
        }

        public void ArcToWithHandleStart(PDFPoint end, PDFPoint handleStart)
        {
            PathBezierCurveData arc = new PathBezierCurveData(end, handleStart, PDFPoint.Empty, true, false);
            CurrentPath.Add(arc);
            IncludeInBounds(end);
            IncludeInBounds(handleStart);
        }

        public void ArcToWithHandleEnd(PDFPoint end, PDFPoint handleEnd)
        {
            PathBezierCurveData arc = new PathBezierCurveData(end, PDFPoint.Empty, handleEnd, false, true);
            CurrentPath.Add(arc);
            IncludeInBounds(end);
            IncludeInBounds(handleEnd);
        }

        /// <summary>
        /// Ends the current path
        /// </summary>
        public void EndPath()
        {
            if (_stack.Count > 0)
            {
                _stack.Pop();
            }
        }

        /// <summary>
        /// Starts a new path
        /// </summary>
        public void BeginPath()
        {
            Path p = new Path();
            _stack.Push(p);
            _paths.Add(p);
        }

        private void IncludeInBounds(PDFPoint pt)
        {
            if (_bounds.Width < pt.X)
                _bounds.Width = pt.X;
            if (_bounds.Height < pt.Y)
                _bounds.Height = pt.Y;
        }

        
    }
}
