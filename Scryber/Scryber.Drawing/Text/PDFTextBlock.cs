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
using Scryber;

namespace Scryber.Text
{
    public class PDFTextBlock
    {
        public PDFUnit Width
        {
            get { return (PDFUnit)this._innerblock.Width; }
        }

        public PDFUnit Height
        {
            get { return (PDFUnit)this._innerblock.Height; }
        }

        public PDFSize Size
        {
            get { return new PDFSize(this.Width, this.Height); }
        }

        private PDFSize _measuredSize;

        public PDFSize MeasuredSize
        {
            get { return _measuredSize; }
            internal set { _measuredSize = value; }
        }

        private TextBlock _innerblock;

        internal TextBlock InnerBlock
        {
            get { return this._innerblock; }
        }

        
        internal PDFTextBlock(TextBlock innerblock)
        {
            if (innerblock == null)
                throw new ArgumentNullException("innerblock");

            _innerblock = innerblock;
        }

        public static PDFTextBlock Create(PDFTextReader reader, PDFSize size, PDFGraphics graphics, PDFTextRenderOptions renderoptions)
        {
            TextBlockBuilder builder = null;
            TextBlock block;
            try
            {
                block = new TextBlock(renderoptions);
                builder = new TextBlockBuilder(reader, size, graphics);
                builder.BuildBlock(block);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(Errors.TextLayoutFailed, ex);
            }
            finally
            {
                if(null != builder)
                    builder.Dispose();
            }
            return new PDFTextBlock(block);
        }
    }

}
