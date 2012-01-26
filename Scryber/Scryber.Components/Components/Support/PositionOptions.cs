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
    internal class PositionOptions
    {
        public LayoutMode LayoutMode { get; private set; }

        public PositionMode PositionMode { get; private set; }

        public PDFUnit X { get; private set; }

        public PDFUnit Y { get; private set; }

        public PDFUnit Width { get; private set; }

        public PDFUnit Height { get; private set; }

        public bool HasX { get; private set; }

        public bool HasY { get; private set; }

        public bool HasWidth { get; private set; }

        public bool HasHeight { get; private set; }

        public bool FillWidth { get; private set; }

        public PositionOptions()
        {
            this.PositionMode = PositionMode.Flow;
            this.LayoutMode = LayoutMode.Block;
            this.HasWidth = this.HasHeight = this.HasX = this.HasY = false;
        }


        public PositionOptions(Styles.PDFPositionStyle pos)
        {
            this.PositionMode = pos.PositionMode;
            this.LayoutMode = pos.LayoutMode;

            if (pos.IsDefined(StyleKeys.XAttr))
            {
                this.X = pos.X;
                this.HasX = true;
            }
            else
                this.HasX = false;

            if (pos.IsDefined(StyleKeys.YAttr))
            {
                this.Y = pos.Y;
                this.HasY = true;
            }
            else
                this.HasY = false;

            if (pos.IsDefined(StyleKeys.WidthAttr))
            {
                this.Width = pos.Width;
                this.HasWidth = true;
            }
            else
                this.HasWidth = false;

            if (pos.IsDefined(StyleKeys.HeightAttr))
            {
                this.Height = pos.Height;
                this.HasHeight = true;
            }
            else
                this.HasHeight = false;

            if (pos.IsDefined(StyleKeys.ExpandWidthAttr))
                this.FillWidth = pos.FillWidth;
            else
                this.FillWidth = false;
        }
    }
}
