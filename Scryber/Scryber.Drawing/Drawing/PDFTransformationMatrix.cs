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
    public class PDFTransformationMatrix : PDFGraphicsAdapter
    {

        #region InnerClasses

        private enum TransformOp
        {
            Offset,
            Scale,
            Rotate,
            Skew
        }

        private class Transform
        {
            public TransformOp Operation;
            public double Value1;
            public double Value2;

            public Transform(TransformOp op, double val1, double val2)
            {
                this.Operation = op;
                this.Value1 = val1;
                this.Value2 = val2;
            }
        }

        #endregion

        private List<Transform> _transforms;
        public double[] Components
        {
            get { return GetComponents(); }
        }

        private double[] GetComponents()
        {
            double[] val = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1};
            //TODO: Implement transformations.
            return val;
        }

        

        public PDFTransformationMatrix()
        {
            _transforms = new List<Transform>();
        }

        public PDFTransformationMatrix(double offsetX, double offsetY, double angle, double scaleX, double scaleY):this()
        {
            this._transforms.Add(new Transform(TransformOp.Scale, scaleX, scaleY));
            this._transforms.Add(new Transform(TransformOp.Rotate, angle, 0.0));
            this._transforms.Add(new Transform(TransformOp.Offset,offsetX,offsetY));
        }

        //
        // graphics adapters
        //

        public override void SetUpGraphics(PDFGraphics graphics, PDFRect bounds)
        {
            //TODO:Implement transformations
            throw new NotImplementedException();
        }

        public override void ReleaseGraphics(PDFGraphics g, PDFRect bounds)
        {
            //TODO:Implement transformations
            throw new NotImplementedException();
        }
    }
}
