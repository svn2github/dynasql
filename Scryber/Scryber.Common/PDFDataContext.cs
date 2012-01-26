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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Scryber
{
    public class PDFDataContext : PDFContextBase
    {

        public PDFDataContext(PDFItemCollection items, PDFTraceLog log)
            : this(items, log, new PDFDataStack())
        {
        }

        public PDFDataContext(PDFItemCollection items, PDFTraceLog log, PDFDataStack stack)
            : base(items, log)
        {
            this._datastack = stack;
        }

        private PDFDataStack _datastack;
        public PDFDataStack DataStack
        {
            get { return _datastack; }
        }

        private int _currindex;

        public int CurrentIndex
        {
            get { return _currindex; }
            set { _currindex = value; }
        }
	

        public object Eval(string expression)
        {
            if (this.DataStack.HasData)
                return PDFDataHelper.Evaluate(this.DataStack.Current, expression);
            else
                return null;
        }

        public System.Xml.XPath.XPathNavigator EvalXPath(System.Xml.XPath.XPathExpression expr)
        {
            if (this.DataStack.HasData)
            {
                if (this.DataStack.Current is System.Xml.XPath.IXPathNavigable)
                {
                    System.Xml.XPath.IXPathNavigable nav = this.DataStack.Current as System.Xml.XPath.IXPathNavigable;
                    return nav.CreateNavigator().Select(expr).Current;
                }
                else
                    throw new ArgumentException(Errors.DatabindingSourceNotXPath, "expr");
            }
            else
                return null;
        }

        public string EvalXPathAsString(System.Xml.XPath.XPathExpression expr)
        {
            System.Xml.XPath.XPathNavigator nav = (System.Xml.XPath.XPathNavigator)this.DataStack.Current;
            string result;
            if (null == nav)
                return string.Empty;
            else
            {
                switch (expr.ReturnType)
                {
                    case System.Xml.XPath.XPathResultType.Boolean:
                        bool value = (bool)nav.Evaluate(expr);
                        result = value.ToString();
                        break;

                    case System.Xml.XPath.XPathResultType.NodeSet:
                        nav = nav.SelectSingleNode(expr);
                        if (null == nav)
                            result = string.Empty;
                        else
                            result = nav.Value;
                        break;

                    case System.Xml.XPath.XPathResultType.Number:
                        result = nav.Evaluate(expr).ToString();
                        break;

                    case System.Xml.XPath.XPathResultType.String: //Includes navigator!!!!
                        result = nav.Evaluate(expr).ToString();
                        break;

                    case System.Xml.XPath.XPathResultType.Error:
                        throw new ArgumentException(String.Format(Errors.XPathExpressionCouldNotBeEvaluated, expr.Expression), "expr");

                    case System.Xml.XPath.XPathResultType.Any:
                    default:
                        result = nav.Evaluate(expr).ToString();
                        break;
                }
            }

            return result;
        }

        

        public System.Xml.XPath.XPathNavigator EvalXPath(string xpath)
        {
            if (this.DataStack.HasData)
            {
                if (this.DataStack.Current is System.Xml.XPath.IXPathNavigable)
                {
                    System.Xml.XPath.IXPathNavigable nav = this.DataStack.Current as System.Xml.XPath.IXPathNavigable;
                    return nav.CreateNavigator().Select(xpath).Current;
                }
                else
                    throw new ArgumentException(Errors.DatabindingSourceNotXPath, "xpath");
            }
            else
                return null;
        }

    }

    
}
