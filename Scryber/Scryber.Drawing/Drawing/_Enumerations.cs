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
using System.Linq;
using System.Text;

namespace Scryber.Drawing
{
    /// <summary>
    /// Defines the type of Color - Gray, RGB etc.
    /// </summary>
    public enum ColorSpace
    {
        G,
        RGB,
        HSL,
        LAB,
        Custom
    }

    /// <summary>
    /// Available page units
    /// </summary>
    public enum PageUnits
    {
        //Reserved 0 for empty units
        Points = 0,
        Millimeters = 1,
        Inches = 2
    }

    /// <summary>
    /// The style of fill
    /// </summary>
    public enum FillStyle
    {
        None,
        Solid,
        Pattern,
        Image
    }


    [Flags()]
    public enum Sides
    {
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8
    }

    public enum LineStyle
    {
        None,
        Solid,
        Dash,
        Pattern
    }

    public enum Quadrants
    {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8
    }

    public enum LineCaps
    {
        Butt = 0,
        Round = 1,
        Projecting = 2
    }


    public enum LineJoin
    {
        Mitre = 0,
        Round = 1,
        Bevel = 2
    }

    public enum GraphicFillMode
    {
        EvenOdd,
        Winding
    }

    public enum StandardFont
    {
        Helvetica,
        Times,
        Courier,
        Symbol,
        Zapf_Dingbats
    }

    [Flags()]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Superscript = 4,
        Subscript = 8
    }

    public enum FontEncoding
    {
        MacRomanEncoding,
        Win32Encoding,
        WinAnsiEncoding
    }

    public enum FontType
    {
        Type1,
        Type0,
        Type3,
        TrueType
    }

    public enum FontStretch
    {
        UltraCondensed,
        ExtraCondensed,
        Condensed,
        SemiCondensed,
        Normal,
        SemiExpanded,
        Expanded,
        ExtraExpanded,
        UltraExpanded
    }

    [Flags()]
    public enum FontFlags
    {
        FixedPitch = 1,
        Serif = 2,
        Symbolic = 4,
        Script = 8,
        NonSymbolic = 32,
        Italic = 64,
        AllCap = 65536,
        SmallCap = 131072,
        Bold = 262144
    }

}
