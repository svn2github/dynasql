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
using Scryber.Native;

namespace Scryber.Drawing
{
    public partial class PDFGraphics : IDisposable
    {

        protected static readonly double CircularityFactor = 0.55;

        #region public DrawingOrigin Origin {get;set;}

        private DrawingOrigin _origin = DrawingOrigin.TopLeft;

        public DrawingOrigin Origin
        {
            get { return _origin; }
        }

        #endregion

        #region public bool FillModeEvenOdd {get;set;}

        private bool _fillEvenOdd;

        public bool FillModeEvenOdd
        {
            get { return _fillEvenOdd; }
            set { _fillEvenOdd = value; }
        }

        #endregion

        #region protected PDFWriter Writer {get;}

        private PDFWriter _writer;
        public PDFWriter Writer
        {
            get { return _writer; }
        }

        #endregion

        #region public PDFContextBase Context {get;}

        private PDFContextBase _context;

        /// <summary>
        /// Gets the current context associated with this PDFGraphics
        /// </summary>
        public PDFContextBase Context
        {
            get { return _context; }
            private set { _context = value; }
        }

        #endregion

        #region public IPDFResourceContainer Container{get;}

        private IPDFResourceContainer _rsrc;
        public IPDFResourceContainer Container
        {
            get { return _rsrc; }
        }

        #endregion

        #region protected bool OwnsWriter {get;}

        private bool _ownswriter;
        protected bool OwnsWriter
        {
            get { return _ownswriter; }
            private set { this._ownswriter = value; }
        }

        #endregion

        #region public PDFSize ContainerSize

        private PDFSize _pageSize;

        public PDFSize ContainerSize
        {
            get { return this._pageSize; }
        }

        #endregion

        #region protected .ctor(writer, size, container)

        protected PDFGraphics(PDFWriter writer, PDFSize size, IPDFResourceContainer container)
        {
            this._writer = writer;
            this._pageSize = size;
            this._rsrc = container;
        }

        #endregion

        #region internal static PDFGraphics Create(writer, ownswriter, container, origin, size)

        public static PDFGraphics Create(PDFWriter writer, bool ownswriter, IPDFResourceContainer rsrc, DrawingOrigin origin, PDFSize size, PDFContextBase context)
        {
            if (origin == DrawingOrigin.BottomLeft)
                throw new ArgumentException(Errors.GraphicsOnlySupportsTopDownDrawing, "origin");

            PDFGraphics g = new PDFGraphics(writer, size, rsrc);
            g.OwnsWriter = ownswriter;
            g.Context = context;
            return g;
        }

        #endregion

        #region SaveGraphicsState() + RestoreGraphicsState()

        public void SaveGraphicsState()
        {
            this.Writer.WriteOpCodeS(PDFOpCode.SaveState);
        }

        public void RestoreGraphicsState()
        {
            if (null != _currState)
                _currState = null;
            this.Writer.WriteOpCodeS(PDFOpCode.RestoreState);
        }

        #endregion

        private Resources.PDFExtGSState _currState;

        protected Resources.PDFExtGSState EnsureExternalState()
        {
            if (null == _currState)
            {
                _currState = new Scryber.Resources.PDFExtGSState();
                this.Container.Register(_currState);
                _currState.Name.WriteData(this.Writer);
                this.Writer.WriteOpCodeS(PDFOpCode.GraphApplyState);
            }
            return _currState;
        }

        #region System.Drawing.Graphics CreateWinGraphics()

        private System.Drawing.Bitmap _winbitmap = null;
        private System.Drawing.Graphics _wingraphics = null;

        protected internal System.Drawing.Graphics CreateWinGraphics()
        {
            if (_winbitmap == null)
                this._winbitmap = new System.Drawing.Bitmap(1, 1);
            if (_wingraphics == null)
            {
                _wingraphics = System.Drawing.Graphics.FromImage(this._winbitmap);
                this._wingraphics.PageUnit = System.Drawing.GraphicsUnit.Point;
            }
            return this._wingraphics;
        }

        #endregion

        

        #region public SetStrokeColor() + SetFillColor()

        public void SetStrokeColor(PDFColor color)
        {
            switch (color.ColorSpace)
            {
                case ColorSpace.G:
                    this.Writer.WriteOpCodeS(PDFOpCode.ColorStrokeGrayscaleSpace, color.Gray);
                    break;
                case ColorSpace.RGB:
                    this.Writer.WriteOpCodeS(PDFOpCode.ColorStrokeRGBSpace, color.Red, color.Green, color.Blue);
                    break;
                case ColorSpace.HSL:
                case ColorSpace.LAB:
                case ColorSpace.Custom:
                default:
                    throw new ArgumentOutOfRangeException("color.ColorSpace", String.Format(Errors.ColorValueIsNotCurrentlySupported, color.ColorSpace));

            }
        }

        public void SetFillColor(PDFColor color)
        {
            switch (color.ColorSpace)
            {
                case ColorSpace.G:
                    this.Writer.WriteOpCodeS(PDFOpCode.ColorFillGrayscaleSpace, color.Gray);
                    break;
                case ColorSpace.RGB:
                    this.Writer.WriteOpCodeS(PDFOpCode.ColorFillRGBSpace, color.Red, color.Green, color.Blue);
                    break;
                case ColorSpace.HSL:
                case ColorSpace.LAB:
                case ColorSpace.Custom:
                    throw new NotSupportedException(String.Format(Errors.ColorValueIsNotCurrentlySupported, color.ColorSpace));

                default:
                    throw new ArgumentOutOfRangeException("color.ColorSpace", String.Format(Errors.ColorValueIsNotCurrentlySupported, color.ColorSpace));

            }
        }

        /// <summary>
        /// If we are rendering with a pattern then we need to use a transformation
        /// matrix when rendering so that the pattern starts at the start of the bounds
        /// </summary>
        protected bool UsePatternFillTransformation = false;

        public void SetFillPattern(PDFName patternName)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.ColorSetFillSpace, (PDFName)"Pattern");
            this.Writer.WriteOpCodeS(PDFOpCode.ColorFillPattern, patternName);
            this.UsePatternFillTransformation = true;
        }

        public void ClearFillPattern()
        {
            this.UsePatternFillTransformation = false;
        }


        public void SetStrokeOpacity(PDFReal opacity)
        {
            if (opacity.Value < 0.0 || opacity.Value > 1.0)
                throw new ArgumentOutOfRangeException("Opacity values can only be between 0.0 (transparent) and 1.0 (blockout)");
            Resources.PDFExtGSState state = this.EnsureExternalState();
            state.States[Resources.PDFExtGSState.ColorStrokeOpacity] = opacity;
        }

        public void SetFillOpacity(PDFReal opacity)
        {
            if (opacity.Value < 0.0 || opacity.Value > 1.0)
                throw new ArgumentOutOfRangeException("Opacity values can only be between 0.0 (transparent) and 1.0 (blockout)");
            Resources.PDFExtGSState state = this.EnsureExternalState();
            state.States[Resources.PDFExtGSState.ColorFillOpactity] = opacity;
        }

        #endregion

        #region protected internal RenderLineWidth() + RenderLineDash() + RenderLineJoin() + RenderLineCap() + RenderLineMitre()

        public void RenderLineWidth(PDFUnit width)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphLineWidth, width.ToPoints().RealValue);
        }

        public void RenderLineDash(PDFDash dash)
        {
            this.Writer.WriteArrayNumberEntries(true, dash.Pattern);
            this.Writer.WriteOpCodeS(PDFOpCode.GraphDashPattern, dash.Phase);
        }

        public void RenderLineJoin(LineJoin linejoin)
        {
            PDFNumber join = (PDFNumber)(int)linejoin;
            Writer.WriteOpCodeS(PDFOpCode.GraphLineJoin, join);
        }

        public void RenderLineCap(LineCaps linecap)
        {
            PDFNumber cap = (PDFNumber)(int)linecap;
            Writer.WriteOpCodeS(PDFOpCode.GraphLineCap, cap);
        }

        public void RenderLineMitre(float mitreLimit)
        {
            PDFReal real = (PDFReal)mitreLimit;
            Writer.WriteOpCodeS(PDFOpCode.GraphMiterLimit, real);
        }

        #endregion

        #region protected RenderStrokePathOp(), RenderFillPathOp(), RenderFillAndStrokePathOp()

        protected void RenderStrokePathOp()
        {
            Writer.WriteOpCodeS(PDFOpCode.GraphStrokePath);
        }

        protected void RenderCloseStrokePathOp()
        {
            Writer.WriteOpCodeS(PDFOpCode.GraphCloseAndStroke);
        }

        protected void RenderFillPathOp()
        {
            this.RenderFillPathOp(this.FillModeEvenOdd);
        }

        protected void RenderFillPathOp(bool evenodd)
        {
            if (evenodd)
                Writer.WriteOpCodeS(PDFOpCode.GraphFillPathEvenOdd);
            else
                Writer.WriteOpCodeS(PDFOpCode.GraphFillPath);
        }

        protected void RenderClosePathOp()
        {
            Writer.WriteOpCodeS(PDFOpCode.GraphClose);
        }



        protected void RenderFillAndStrokePathOp()
        {
            RenderFillAndStrokePathOp(this.FillModeEvenOdd); 
        }

        protected void RenderFillAndStrokePathOp(bool evenodd)
        {
            if (evenodd)
                Writer.WriteOpCodeS(PDFOpCode.GraphFillAndStrokeEvenOdd);
            else
                Writer.WriteOpCodeS(PDFOpCode.GraphFillAndStroke);
        }

        #endregion

        protected void RenderMoveTo(PDFUnit x, PDFUnit y)
        {
            this.RenderMoveTo(x.RealValue, y.RealValue);
        }

        protected virtual void RenderMoveTo(PDFReal x, PDFReal y)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphMove, GetXPosition(x), GetYPosition(y));
        }


        protected void RenderLineTo(PDFUnit x, PDFUnit y)
        {
            this.RenderLineTo(x.RealValue, y.RealValue);
        }

        protected virtual void RenderLineTo(PDFReal x, PDFReal y)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphLineTo, GetXPosition(x), GetYPosition(y));
        }

        protected void RenderArcTo(PDFUnit endX, PDFUnit endY, PDFUnit starthandleX, PDFUnit startHandleY, PDFUnit endhandleX, PDFUnit endhandleY)
        {
            this.RenderArcTo(endX.RealValue, endY.RealValue, starthandleX.RealValue, startHandleY.RealValue, endhandleX.RealValue, endhandleY.RealValue);
        }

        protected virtual void RenderArcTo(PDFReal endX, PDFReal endY, PDFReal starthandleX, PDFReal startHandleY, PDFReal endhandleX, PDFReal endhandleY)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphCurve2Handle, GetXPosition(starthandleX), GetYPosition(startHandleY), GetXPosition(endhandleX), GetYPosition(endhandleY), GetXPosition(endX), GetYPosition(endY));
        }


        protected void RenderArcToWithStartHandleOnly(PDFUnit endX, PDFUnit endY, PDFUnit starthandleX, PDFUnit startHandleY)
        {
            this.RenderArcToWithStartHandleOnly(endX.RealValue, endY.RealValue, starthandleX.RealValue, startHandleY.RealValue);
        }

        protected virtual void RenderArcToWithStartHandleOnly(PDFReal endX, PDFReal endY, PDFReal starthandleX, PDFReal startHandleY)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphCurve1HandleBegin, GetXPosition(starthandleX), GetYPosition(startHandleY), GetXPosition(endX), GetYPosition(endY));
        }


        protected void RenderArcToWithEndHandleOnly(PDFUnit endX, PDFUnit endY, PDFUnit endhandleX, PDFUnit endHandleY)
        {
            this.RenderArcToWithEndHandleOnly(endX.RealValue, endY.RealValue, endhandleX.RealValue, endHandleY.RealValue);
        }

        protected virtual void RenderArcToWithEndHandleOnly(PDFReal endX, PDFReal endY, PDFReal endhandleX, PDFReal endHandleY)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphCurve1HandleEnd, GetXPosition(endhandleX), GetYPosition(endHandleY), GetXPosition(endX), GetYPosition(endY));
        }


        /// <summary>
        /// Actual implementation method to render the operations to append a rectangle shape to the current path
        /// </summary>
        /// <param name="x">The left position</param>
        /// <param name="y">The top position</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        protected virtual void RenderRectangle(PDFUnit x, PDFUnit y, PDFUnit width, PDFUnit height)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphRect, x.RealValue, this.ContainerSize.Height.RealValue - y.RealValue - height.RealValue, width.RealValue, height.RealValue);
        }

        protected virtual void DoOutputLine(PDFUnit x1, PDFUnit y1, PDFUnit x2, PDFUnit y2)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphMove, GetXPosition(x1), GetYPosition(y1));
            this.Writer.WriteOpCodeS(PDFOpCode.GraphLineTo, GetXPosition(x2), GetYPosition(y2));
        }

        protected virtual void DoOutputContinuationLine(PDFUnit x, PDFUnit y)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphLineTo, GetXPosition(x), GetYPosition(y));
        }

        protected virtual void DoOutputContinuationLine(PDFReal x, PDFReal y)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.GraphLineTo, GetXPosition(x), GetYPosition(y));
        }

        #region PDFReal GetXPosition(PDFUnit x) + 4 Overloads

        public PDFReal GetXPosition(PDFUnit ux)
        {
            return ux.RealValue;
        }

        public PDFReal GetXPosition(PDFUnit ux, PDFUnit width)
        {
            return ux.RealValue;
        }

        public PDFReal GetXPosition(PDFUnit ux, PDFReal width)
        {
            return ux.RealValue;
        }

        public PDFReal GetXPosition(PDFReal x, PDFReal width)
        {
            return x;
        }

        public PDFReal GetXPosition(PDFReal x)
        {
            return x;
        }

        public PDFReal GetXPosition(double d)
        {
            return (PDFReal)d;
        }

        #endregion

        #region PDFReal GetYPosition() + 5 overloads

        public PDFReal GetYPosition(PDFUnit uy)
        {
            return this.ContainerSize.Height.RealValue - uy.RealValue;
        }

        public PDFReal GetYPosition(PDFUnit uy, PDFReal height)
        {
            return this.ContainerSize.Height.RealValue - uy.RealValue - height;
        }

        public PDFReal GetYPosition(PDFReal y)
        {
            return this.ContainerSize.Height.RealValue - y;
        }

        public PDFReal GetYPosition(PDFReal y, PDFReal height)
        {
            return this.ContainerSize.Height.RealValue - y - height;
        }

        #endregion

        #region PDFReal GetYOffset() + 1 overload

        public PDFReal GetYOffset(PDFUnit uy)
        {
            return PDFReal.Zero - uy.RealValue;
        }

        public PDFReal GetYOffset(PDFReal y)
        {
            return PDFReal.Zero - y;
        }

        #endregion

        #region PDFReal GetXOffset() + 1 overloads

        public PDFReal GetXOffset(PDFUnit uy)
        {
            return uy.RealValue;
        }

        public PDFReal GetXOffset(PDFReal y)
        {
            return y;
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
                if (this.OwnsWriter && this.Writer != null)
                    this.Writer.Dispose();

                if (this._wingraphics != null)
                    this._wingraphics.Dispose();

                if (this._winbitmap != null)
                    this._winbitmap.Dispose();
            }
        }

        #endregion

        public void SetClipRect(PDFRect rectangle)
        {
            this.SetClipRect(rectangle.Location, rectangle.Size);
        }

        public void SetClipRect(PDFPoint pt, PDFSize sz)
        {

            this.Writer.WriteOpCodeS(PDFOpCode.GraphSetClip);
            this.Writer.WriteOpCodeS(PDFOpCode.GraphNoOp);
        }

        public void SetClipRect(PDFRect rect, Sides sides, PDFUnit cornerradius)
        {
            this.SetClipRect(rect.Location, rect.Size, sides, cornerradius);
        }

        public void SetClipRect(PDFPoint pt, PDFSize sz, Sides sides, PDFUnit cornerradius)
        {
            if (cornerradius > PDFUnit.Zero)
            {
                this.DoOutputRoundRectangleWithSidesFill(pt.X, pt.Y, sz.Width, sz.Height, cornerradius, sides);
            }
            else
                this.RenderRectangle(pt.X, pt.Y, sz.Width, sz.Height);

            this.Writer.WriteOpCodeS(PDFOpCode.GraphSetClip);
            this.Writer.WriteOpCodeS(PDFOpCode.GraphNoOp);
        }

        public void PaintImageRef(Scryber.Resources.PDFImageXObject img, PDFSize imgsize, PDFPoint pos)
        {
            if (string.IsNullOrEmpty(img.Name.Value))
                throw new ArgumentNullException("img.Name");
            PDFReal width = (PDFReal)(imgsize.Width.RealValue);
            PDFReal height = (PDFReal)(imgsize.Height.RealValue); 
            this.Writer.WriteOpCodeS(PDFOpCode.GraphTransformMatrix,
                        width, PDFReal.Zero,
                        PDFReal.Zero, height, pos.X.RealValue, pos.Y.RealValue);

            this.Writer.WriteOpCodeS(PDFOpCode.XobjPaint, img.Name);
        }
    }
}
