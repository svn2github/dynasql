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

namespace Scryber.Generation
{
    internal abstract class ParserXPathBinding
    {
        protected System.Xml.XPath.XPathExpression _expr;
        protected PDFValueConverter _converter;
        protected System.Reflection.PropertyInfo _property;

        protected ParserXPathBinding()
        {
            
        }


        internal abstract void BindComponent(object sender, PDFDataBindEventArgs args);
        

        internal static ParserXPathBinding Create(string expr, PDFValueConverter convert, System.Reflection.PropertyInfo property)
        {
            if (null == convert)
                throw new ArgumentNullException("convert");
            if (null == property)
                throw new ArgumentNullException("property");
            if (string.IsNullOrEmpty(expr))
                throw new ArgumentNullException("expr");

            System.Xml.XPath.XPathExpression compiled = System.Xml.XPath.XPathExpression.Compile(expr);
            ParserXPathBinding binding = null;
            switch (compiled.ReturnType)
            {

                case System.Xml.XPath.XPathResultType.Boolean:
                    binding = new ParserXPathBooleanBinding();
                    break;
                
                case System.Xml.XPath.XPathResultType.NodeSet:
                    binding = new ParserXPathNodeSetBinding();
                    break;

                case System.Xml.XPath.XPathResultType.Number:
                case System.Xml.XPath.XPathResultType.String:
                    binding = new ParserXPathValueBinding();
                    break;

                case System.Xml.XPath.XPathResultType.Error:
                    throw new PDFParserException(String.Format(Errors.InvalidXPathExpression, expr));

                case System.Xml.XPath.XPathResultType.Any:
                default:
                    throw new PDFParserException(String.Format(Errors.ReturnTypeOfXPathExpressionCouldNotBeDetermined, expr));

            }

            binding._converter = convert;
            binding._expr = compiled;
            binding._property = property;

            return binding;
        }
    }

    internal class ParserXPathNodeSetBinding : ParserXPathBinding
    {
        
        internal override void BindComponent(object sender, PDFDataBindEventArgs args)
        {
            System.Xml.XPath.XPathNavigator nav = (System.Xml.XPath.XPathNavigator)args.Context.DataStack.Current;

            nav = nav.SelectSingleNode(this._expr);
            if (null != nav)
            {
                string result = nav.Value;
                object value = _converter(result, _property.PropertyType);
                _property.SetValue(sender, value, null);
            }
        }
    }

    internal class ParserXPathValueBinding : ParserXPathBinding
    {
        internal override void BindComponent(object sender, PDFDataBindEventArgs args)
        {
            System.Xml.XPath.XPathNavigator nav = (System.Xml.XPath.XPathNavigator)args.Context.DataStack.Current;
            
            object value = nav.Evaluate(this._expr);
            if (null != value)
            {
                string result = value.ToString();
                value = _converter(result, _property.PropertyType);
                _property.SetValue(sender, value, null);
            }
        }
    }

    internal class ParserXPathBooleanBinding : ParserXPathBinding
    {
        internal override void BindComponent(object sender, PDFDataBindEventArgs args)
        {
            System.Xml.XPath.XPathNavigator nav = (System.Xml.XPath.XPathNavigator)args.Context.DataStack.Current;

            bool value = (bool)nav.Evaluate(this._expr);
            _property.SetValue(sender, value, null);
            
        }
    }

    internal class ParserXPathNavigatorBinding : ParserXPathBinding
    {
        internal override void BindComponent(object sender, PDFDataBindEventArgs args)
        {
            System.Xml.XPath.XPathNavigator nav = (System.Xml.XPath.XPathNavigator)args.Context.DataStack.Current;

            System.Xml.XPath.XPathNodeIterator itter = nav.Select(this._expr);
            if (itter.CurrentPosition < 0)
                itter.MoveNext();
            
            string value = itter.Current.Value;
            object converted = this._converter(value, _property.PropertyType);

            _property.SetValue(sender, converted, null);

        }
    }
}
