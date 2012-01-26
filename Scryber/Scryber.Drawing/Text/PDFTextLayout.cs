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
using System.Globalization;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Text
{
    public class PDFTextLayout
    {

        public static readonly WordWrap DefaultWrapText = WordWrap.Auto;
        private WordWrap _wrap = DefaultWrapText;

        public WordWrap WrapText
        {
            get { return _wrap; }
            set { _wrap = value; }
        }

        private PDFUnit _fistlineinset = PDFUnit.Empty;

        public PDFUnit FirstLineInset
        {
            get { return _fistlineinset; }
            set { _fistlineinset = value; }
        }

        private PDFUnit _leading;

        public PDFUnit Leading
        {
            get { return _leading; }
            set { _leading = value; }
        }


        private float _wordspace = 0.0F;

        public float WordSpacing
        {
            get { return _wordspace; }
            set { _wordspace = value; }
        }

        private HorizontalAlignment _halign = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public HorizontalAlignment HAlign
        {
            get { return _halign; }
            set { _halign = value; }
        }

        private VerticalAlignment _valign = VerticalAlignment.Top;

        public VerticalAlignment VAlign
        {
            get { return _valign; }
            set { _valign = value; }
        }

        
    }
}
