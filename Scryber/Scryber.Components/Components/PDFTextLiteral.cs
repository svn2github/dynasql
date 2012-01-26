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
using System.Drawing;
using Scryber.Styles;
using Scryber.Native;
using Scryber.Text;

namespace Scryber.Components
{
    [PDFParsableComponent("Text")]
    public class PDFTextLiteral : PDFTextBase, IPDFTextLiteral
    {
        

        public string Text
        {
            get
            {
                return this.BaseText;
            }
            set 
            {
                this.BaseText = value;
            }
        }

        public TextFormat Format
        {
            get { return this.BaseFormat; }
            set { this.BaseFormat = value; }
        }

        public PDFTextLiteral()
            : base(PDFObjectTypes.Text)
        {

        }

        public PDFTextLiteral(string text): this()
        {
            this.Text = text;
        }
        
    }

}
