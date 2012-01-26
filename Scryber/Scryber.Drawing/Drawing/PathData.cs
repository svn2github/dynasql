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
    #region internal abstract class PathData

    internal abstract class PathData
    {
        private PathDataType _type;

        internal PathDataType Type { get { return _type; } }

        internal PathData(PathDataType type)
        {
            this._type = type;
        }
    }

    #endregion

    #region internal class PathMoveData : PathData

    internal class PathMoveData : PathData
    {
        public PDFPoint MoveTo { get; set; }

        public PathMoveData() : base(PathDataType.Move) { }
    }

    #endregion

    #region internal class PathSubPathData : PathData

    internal class PathSubPathData : PathData
    {
        public Path InnerPath { get; set; }

        public PathSubPathData() : base(PathDataType.SubPath) { }
    }

    #endregion

    #region internal class PathLineData : PathData

    internal class PathLineData : PathData
    {
        internal PDFPoint LineTo { get; set; }

        internal PathLineData() : base(PathDataType.Line) { }
    }

    #endregion

    #region internal class PathRectData : PathData

    internal class PathRectData : PathData
    {
        internal PDFRect Rect { get; set; }

        internal PathRectData() : base(PathDataType.Rect) { }
    }

    #endregion

    #region internal class PathBezierCurveData : PathData

    internal class PathBezierCurveData : PathData
    {
        public PDFPoint[] Points { get; private set; }

        public PDFPoint EndPoint { get { return this.Points[0]; } }
        public PDFPoint StartHandle { get { return this.Points[1]; } }
        public PDFPoint EndHandle { get { return this.Points[2]; } }

        public bool HasStartHandle { get; private set; }

        public bool HasEndHandle { get; private set; }

        public PathBezierCurveData(PDFPoint end, PDFPoint startHandle, PDFPoint endHandle, bool hasStart, bool hasEnd) : base(PathDataType.Bezier) 
        {
            this.Points = new PDFPoint[] { end, startHandle, endHandle };
            this.HasEndHandle = hasEnd;
            this.HasStartHandle = hasStart;
        }
    }

    #endregion

    #region internal class PathCloseData : PathData

    internal class PathCloseData : PathData
    {
        public PathCloseData()
            : base(PathDataType.Close)
        {
        }
    }

    #endregion
}
