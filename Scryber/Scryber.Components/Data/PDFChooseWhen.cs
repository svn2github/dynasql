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
using Scryber.Components;

namespace Scryber.Data
{
    [PDFParsableComponent("When")]
    public class PDFChooseWhen : PDFChooseTemplateContainer
    {
        private string _test;
        private System.Xml.XPath.XPathExpression _expr;

        [PDFAttribute("test")]
        public string Test
        {
            get { return _test; }
            set
            {
                _test = value;
                _expr = null;
            }
        }

        public PDFChooseWhen()
            : base(PDFObjectTypes.DataWhen)
        {
        }

        public bool EvaluateTest(PDFDataContext context)
        {
            if (string.IsNullOrEmpty(_test))
                return false;
            if (null == _expr)
                _expr = System.Xml.XPath.XPathExpression.Compile(_test);

            return PDFDataHelper.EvaluateTestExpression(_expr, context);
        }
    }



    public class PDFChooseWhenList : PDFComponentWrappingList<PDFChooseWhen>
    {
        public PDFChooseWhenList(PDFComponentList inner)
            : base(inner)
        {
        }
    }
}
