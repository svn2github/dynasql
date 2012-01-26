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
using Scryber.Components;

namespace Scryber.Data
{
    /// <summary>
    /// Defines a template that will be bound and included
    /// if the test expression returns true or not null
    /// </summary>
    [PDFParsableComponent("If")]
    public class PDFIf : PDFBindingTemplateComponent
    {

        private string _test;
        private System.Xml.XPath.XPathExpression _expr;

        /// <summary>
        /// Gets or sets the test expression for the if entry
        /// </summary>
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



        //
        // .ctors
        //

        /// <summary>
        /// Use the NoOp opbject type so that inner content is not generated
        /// </summary>
        public PDFIf()
            : this(PDFObjectTypes.NoOp)
        {
        }

        protected PDFIf(PDFObjectType type)
            : base(type)
        {
        }

        //
        // binding methods
        //



        protected override void DoDataBindTemplate(PDFDataContext context, IPDFContainerComponent container)
        {
            if (!string.IsNullOrEmpty(this.Test))
            {
                if (null == this._expr)
                    this._expr = System.Xml.XPath.XPathExpression.Compile(this.Test);
                if (this._expr.ReturnType != System.Xml.XPath.XPathResultType.Boolean)
                    throw RecordAndRaise.BindException(Errors.InvalidXPathExpression, this.Test);

                //check the result and only call the base method if the return value is true
                if(this.EvaluateTestExpression(this._expr, context))
                    base.DoDataBindTemplate(context, container);
            }
            
        }

        protected virtual bool EvaluateTestExpression(System.Xml.XPath.XPathExpression expr, PDFDataContext context)
        {
            bool value = PDFDataHelper.EvaluateTestExpression(expr, context);

            return value;
        }

    }
}
