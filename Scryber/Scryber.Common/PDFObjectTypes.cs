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

namespace Scryber
{
    public static class PDFObjectTypes
    {
        public static readonly PDFObjectType Number = (PDFObjectType)"NUM ";
        public static readonly PDFObjectType Array = (PDFObjectType)"ARRY";
        public static readonly PDFObjectType String = (PDFObjectType)"STR ";
        public static readonly PDFObjectType Dictionary = (PDFObjectType)"DICT";
        public static readonly PDFObjectType Object = (PDFObjectType)"OBJ ";
        public static readonly PDFObjectType ObjectRef = (PDFObjectType)"OREF";
        public static readonly PDFObjectType Name = (PDFObjectType)"NAME";
        public static readonly PDFObjectType Real = (PDFObjectType)"REAL";
        public static readonly PDFObjectType Stream = (PDFObjectType)"STEA";
        public static readonly PDFObjectType Null = (PDFObjectType)"NULL";
        public static readonly PDFObjectType Boolean = (PDFObjectType)"BOOL";
        public static readonly PDFObjectType IndirectObject = (PDFObjectType)"IOBJ";
        public static readonly PDFObjectType NoOp = (PDFObjectType)"NOOP";
        
        public static readonly PDFObjectType Color = (PDFObjectType)"colr";
        public static readonly PDFObjectType Thickness = (PDFObjectType)"thck";
        public static readonly PDFObjectType ProcSet = (PDFObjectType)"proc";
        public static readonly PDFObjectType Font = (PDFObjectType)"font";
        public static readonly PDFObjectType FontResource = (PDFObjectType)"frsc";
        public static readonly PDFObjectType FontWidths = (PDFObjectType)"fwdt";
        public static readonly PDFObjectType FontDescriptor = (PDFObjectType)"fdec";
        public static readonly PDFObjectType ImageXObject = (PDFObjectType)"imgx";
        public static readonly PDFObjectType ExtGState = (PDFObjectType)"gxst";
        public static readonly PDFObjectType GraphicsPath = (PDFObjectType)"gpth";
        public static readonly PDFObjectType Pattern = (PDFObjectType)"patt";


        public static readonly PDFObjectType Document = (PDFObjectType)"doc ";
        public static readonly PDFObjectType Text = (PDFObjectType)"text";
        public static readonly PDFObjectType Page = (PDFObjectType)"page";
        public static readonly PDFObjectType Section = (PDFObjectType)"sect";
        public static readonly PDFObjectType PageBreak = (PDFObjectType)"pgbk";
        public static readonly PDFObjectType PageSize = (PDFObjectType)"pgsz";
        public static readonly PDFObjectType Image = (PDFObjectType)"img ";
        public static readonly PDFObjectType ImageData = (PDFObjectType)"imgd";
        public static readonly PDFObjectType Table = (PDFObjectType)"tble";
        public static readonly PDFObjectType TableRow = (PDFObjectType)"trow";
        public static readonly PDFObjectType TopAndTail = (PDFObjectType)"tatp";
        public static readonly PDFObjectType TableCell = (PDFObjectType)"tcel";
        public static readonly PDFObjectType Canvas = (PDFObjectType)"canv";
        public static readonly PDFObjectType Panel = (PDFObjectType)"panl";
        public static readonly PDFObjectType XmlData = (PDFObjectType)"xdat";
        public static readonly PDFObjectType Layer = (PDFObjectType)"layr";
        public static readonly PDFObjectType LayerGroup = (PDFObjectType)"lygp";
        public static readonly PDFObjectType UserComponent = (PDFObjectType)"ucmp";
        public static readonly PDFObjectType Link = (PDFObjectType)"link";
        public static readonly PDFObjectType Template = (PDFObjectType)"temp";
        public static readonly PDFObjectType DataOtherwise = (PDFObjectType)"doth";
        public static readonly PDFObjectType DataWhen = (PDFObjectType)"dwhn";
        public static readonly PDFObjectType PageHeader = (PDFObjectType)"pghd";
        public static readonly PDFObjectType PageFooter = (PDFObjectType)"pgft";

        public static readonly PDFObjectType StyleMargins = (PDFObjectType)"smar";
        public static readonly PDFObjectType StyleFill = (PDFObjectType)"sfil";
        public static readonly PDFObjectType StyleBackground = (PDFObjectType)"sbak";
        public static readonly PDFObjectType StyleBorder = (PDFObjectType)"sbdr";
        public static readonly PDFObjectType StyleClip = (PDFObjectType)"sclp";
        public static readonly PDFObjectType StyleDocument = (PDFObjectType)"sdoc";
        public static readonly PDFObjectType StyleText = (PDFObjectType)"stxt";
        public static readonly PDFObjectType StyleTransform = (PDFObjectType)"stra";
        public static readonly PDFObjectType StyleStroke = (PDFObjectType)"sstk";
        public static readonly PDFObjectType StylePage = (PDFObjectType)"spag";
        public static readonly PDFObjectType StyleOverflow = (PDFObjectType)"sovr";
        public static readonly PDFObjectType StylePosition = (PDFObjectType)"spos";
        public static readonly PDFObjectType StyleFont = (PDFObjectType)"sfnt";
        public static readonly PDFObjectType StyleOutline = (PDFObjectType)"sout";
        public static readonly PDFObjectType Style = (PDFObjectType)"styl";
        
        public static readonly PDFObjectType StylePadding = (PDFObjectType)"spad";
        public static readonly PDFObjectType StyleGroup = (PDFObjectType)"sgrp";
        public static readonly PDFObjectType StyleRef = (PDFObjectType)"sref";

        public static readonly PDFObjectType ShapeLine = (PDFObjectType)"line";
        public static readonly PDFObjectType ShapeCircle = (PDFObjectType)"scir";
        public static readonly PDFObjectType ShapeElipse = (PDFObjectType)"selp";
        public static readonly PDFObjectType ShapePath = (PDFObjectType)"path";
        public static readonly PDFObjectType ShapePolygon = (PDFObjectType)"poly";

        public static readonly PDFObjectType H1 = (PDFObjectType)"txh1";
        public static readonly PDFObjectType H2 = (PDFObjectType)"txh2";
        public static readonly PDFObjectType H3 = (PDFObjectType)"txh3";
        public static readonly PDFObjectType H4 = (PDFObjectType)"txh4";
        public static readonly PDFObjectType H5 = (PDFObjectType)"txh5";
        public static readonly PDFObjectType H6 = (PDFObjectType)"txh6";

    }
}
