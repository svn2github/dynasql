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

namespace Scryber.Styles
{
    public static class PDFStyleFactory
    {
        public static PDFStyleItem CreateStyleItem(PDFObjectType type)
        {
            if (type == PDFObjectTypes.StyleBackground)
                return new PDFBackgroundStyle();
            else if (type == PDFObjectTypes.StyleBorder)
                return new PDFBorderStyle();
            else if (type == PDFObjectTypes.StyleStroke)
                return new PDFStrokeStyle();
            else if (type == PDFObjectTypes.StyleFill)
                return new PDFFillStyle();
            else if (type == PDFObjectTypes.StylePage)
                return new PDFPageStyle();
            else if (type == PDFObjectTypes.StyleText)
                return new PDFTextStyle();
            else if (type == PDFObjectTypes.StyleOverflow)
                return new PDFOverflowStyle();
            else if (type == PDFObjectTypes.StyleClip)
                return new PDFClipStyle();
            else if (type == PDFObjectTypes.StyleTransform)
                return new PDFTransformStyle();
            else if (type == PDFObjectTypes.StyleMargins)
                return new PDFMarginsStyle();
            else if (type == PDFObjectTypes.StylePadding)
                return new PDFPaddingStyle();
            else if (type == PDFObjectTypes.StylePosition)
                return new PDFPositionStyle();
            else if (type == PDFObjectTypes.StyleFont)
                return new PDFFontStyle();
            else
                throw new ArgumentOutOfRangeException("type", String.Format(Errors.StyleItemNotFound, type.ToString()));
        }

    }
}
