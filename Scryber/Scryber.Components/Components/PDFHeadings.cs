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
using Scryber.Styles;
using Scryber.Drawing;

namespace Scryber.Components
{
    public abstract class PDFHeadingBase : PDFTextBase
    {
        

        [PDFElement("")]
        public string Text
        {
            get { return this.BaseText; }
            set { this.BaseText = value; }
        }

        

        public PDFHeadingBase(PDFObjectType type)
            : base(type)
        {
        }

        protected static PDFStyle GetBaseStyles(PDFUnit fontsize, bool bold, bool italic)
        {
            PDFStyle fs = new PDFStyle();
            fs.Font.FontSize = fontsize;
            fs.Font.FontBold = bold;
            fs.Font.FontItalic = italic;
           
            return fs;
        }

        
    }

    [PDFParsableComponent("H1")]
    public class PDFHead1 : PDFHeadingBase
    {

        public PDFHead1()
            : base(PDFObjectTypes.H1)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(36,true, false);
        }
    }

    [PDFParsableComponent("H2")]
    public class PDFHead2 : PDFHeadingBase
    {

        public PDFHead2()
            : base(PDFObjectTypes.H2)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(30, true, true);
        }
    }

    [PDFParsableComponent("H3")]
    public class PDFHead3 : PDFHeadingBase
    {

        public PDFHead3()
            : base(PDFObjectTypes.H3)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(24, true, false);
        }
    }

    [PDFParsableComponent("H4")]
    public class PDFHead4 : PDFHeadingBase
    {

        public PDFHead4()
            : base(PDFObjectTypes.H4)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(20, true, true);
        }
    }

    [PDFParsableComponent("H5")]
    public class PDFHead5 : PDFHeadingBase
    {

        public PDFHead5()
            : base(PDFObjectTypes.H5)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(17, true, false);
        }
    }

    [PDFParsableComponent("H6")]
    public class PDFHead6 : PDFHeadingBase
    {

        public PDFHead6()
            : base(PDFObjectTypes.H6)
        { }

        protected override PDFStyle GetBaseStyle()
        {
            return PDFHeadingBase.GetBaseStyles(15, true, true);
        }
    }
}
