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
using System.Collections.Concurrent;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Scryber
{
    public static class PDFDataHelper
    {

        /// <summary>
        /// Returns the results of an xpath expression against the current data context.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static object EvaluateExpression(System.Xml.XPath.XPathExpression expr, PDFDataContext context)
        {
            if (null == expr)
                throw new ArgumentNullException("expr");
            if (null == context)
                throw new ArgumentNullException("context");
            if (null == context.DataStack.Current)
                throw new ArgumentNullException("context.DataStack.Current");

            object currentData = context.DataStack.Current;

            if (!(currentData is System.Xml.XPath.IXPathNavigable))
                throw new InvalidCastException(Errors.DatabindingSourceNotXPath);
            System.Xml.XPath.XPathNodeIterator itter = ((System.Xml.XPath.IXPathNavigable)currentData).CreateNavigator().Select(expr);

            return itter;
        }

        /// <summary>
        /// Tests an XPath expression against the current data and returns true if the expression returns a (True) value.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="currentData"></param>
        /// <returns></returns>
        public static bool EvaluateTestExpression(System.Xml.XPath.XPathExpression expr, PDFDataContext context)
        {
            if (null == expr)
                throw new ArgumentNullException("expr");
            if (null == context)
                throw new ArgumentNullException("context");
            if (null == context.DataStack.Current)
                throw new ArgumentNullException("context.DataStack.Current");


            object currentData = context.DataStack.Current;
            if (!(currentData is System.Xml.XPath.XPathNavigator))
                throw new InvalidCastException(Errors.DatabindingSourceNotXPath);

            object value = ((System.Xml.XPath.XPathNavigator)currentData).Evaluate(expr);

            if (value == null)
                return false;
            else if (value is bool)
                return (bool)value;
            else
                return false;
        }

        /// <summary>
        /// Returns the required binding data based upon the specified parameters or null if there is no required source.
        /// </summary>
        /// <param name="datasourceComponent">The Component in the document that implements the IPDFDataSource interface. 
        /// Set it to null to ignore this parameter.
        /// If it is set, and so is the select path, then this path will be used 
        /// by the IPDFDataSource to extract the required source.
        /// </param>
        /// <param name="datasourcevalue">The value of the data source. 
        /// If not null then the value will be returned unless a select path is set. 
        /// If the select path is set then this value must be IXPathNavigable and the returned value will be the result of a select on the navigator</param>
        /// <param name="stack">If neither the Component or value are set, then the current data from the stack will be 
        /// used (if and only if theres is a select path). If there is no select path then null will be returned.
        /// If there is a select path then the current data must implement IXPathNavigable</param>
        /// <param name="selectpath">The path to  use to extract values</param>
        /// <returns>The required data or null.</returns>
        public static object GetBindingData(IPDFDataSource datasourceComponent, object datasourcevalue, PDFDataStack stack, string selectpath)
        {
            object data = null;
            if (null != datasourceComponent)
            {
                data = datasourceComponent.Select(selectpath);
            }
            else if (datasourcevalue != null)
            {
                data = datasourcevalue;
                if (!string.IsNullOrEmpty(selectpath))
                {
                    if (data is System.Xml.XPath.IXPathNavigable)
                        data = ((System.Xml.XPath.IXPathNavigable)data).CreateNavigator().Select(selectpath);
                    else
                        new ArgumentException(Errors.DatabindingSourceNotXPath, "selectpath");
                }
            }
            else if (!string.IsNullOrEmpty(selectpath))
            {
                data = stack.Current;
                if (null == data || !(data is System.Xml.XPath.IXPathNavigable))
                    new ArgumentException(Errors.DatabindingSourceNotXPath, "stack.Current");
                data = ((System.Xml.XPath.IXPathNavigable)data).CreateNavigator().Select(selectpath);
            }

            return data;
        }

        public static readonly char[] ExpressionPartSeparator = new char[] { '.' };
        public static readonly char[] IndexExpressionStartChars = new char[] { '[', '(' };
        public static readonly char[] IndexExpressionEndChars = new char[] { ']', ')' };

        /// <summary>
        /// A cache of the property descriptors for each reflected type. The concurrent dictionary is thread safe.
        /// </summary>
        private static ConcurrentDictionary<Type, PropertyDescriptorCollection> _propertyCache = new ConcurrentDictionary<Type, PropertyDescriptorCollection>();

        public static object Evaluate(object container, string expr)
        {
            if (String.IsNullOrEmpty(expr))
                throw new ArgumentNullException("expr");
            expr = expr.Trim();
            if (expr.Length == 0)
                throw new ArgumentNullException("expr");
            if (null == container)
                return null;
            string[] parts = expr.Split(ExpressionPartSeparator);

            return Evaluate(container, parts);
        }

        private static object Evaluate(object container, string[] exprParts)
        {
            object value = container;
            for (int i = 0; i < exprParts.Length; i++)
            {
                string name = exprParts[i];
                if (name.IndexOfAny(IndexExpressionStartChars) < 0)
                {
                    value = GetPropertyValue(value, name);
                }
                else
                    value = GetIndexedPropertyValue(value, name);
            }

            return value;
        }

        public static object GetPropertyValue(object container, string propertyName)
        {
            if (null == container)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            PropertyDescriptor desc = GetPropertiesFromCache(container).Find(propertyName, true);
            if (null == desc)
                throw new NullReferenceException(String.Format(Errors.DatabindingPropertyNotFound, propertyName));

            return desc.GetValue(container);
        }

        public static object GetIndexedPropertyValue(object container, string propertyIndexedName)
        {
            if (null == container)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(propertyIndexedName))
                throw new ArgumentNullException("propertyIndexedName");

            object value = null;
            bool flag = false;

            int start = propertyIndexedName.IndexOfAny(IndexExpressionStartChars);
            int end = propertyIndexedName.IndexOfAny(IndexExpressionEndChars, start + 1);
            
            if (start < 0 || end < 0 || end == start + 1)
                throw new ArgumentException(String.Format(Errors.InvalidIndexerExpression, propertyIndexedName), "propertyIndexedName");

            string propname = null;
            string indexer = propertyIndexedName.Substring(start+1,(end-start)-1).Trim();
            object indexerValue = null;
            bool isint = false;

            if (start != 0)
            {
                propname = propertyIndexedName.Substring(0, start);
            }
            if (indexer.Length != 0)
            {
                if (indexer[0] == '"' && indexer[indexer.Length - 1] == '"')
                    //string indexer delimited by double qotes
                    indexerValue = (string)indexer.Substring(1, indexer.Length - 2);
                else if (indexer[0] == '\'' && indexer[indexer.Length - 1] == '\'')
                    //string indexer delimited by single quotes
                    indexerValue = (string)indexer.Substring(1, indexer.Length - 2);
                else if (char.IsDigit(indexer, 0))
                {
                    int num; //try to get an integer indexer
                    isint = int.TryParse(indexer, out num);
                    if (isint)
                        indexerValue = (int)num;
                    else
                        //first digit was a number, but still treat as a string
                        indexerValue = (string)indexer;
                }
                else
                    indexerValue = (string)indexer;
            }

            if (null == indexerValue)
                throw new ArgumentException(String.Format(Errors.InvalidIndexerExpression, propertyIndexedName), "propertyIndexedName");

            object propertyValue = null;

            if (!string.IsNullOrEmpty(propname))
                propertyValue = GetPropertyValue(container, propname);
            else
                propertyValue = container;

            if (propertyValue == null)
                return null;

            Array array = propertyValue as Array;
            if (null != array && isint)
                return array.GetValue((int)indexerValue);

            if (propertyValue is System.Collections.IList && isint)
                return ((System.Collections.IList)propertyValue)[(int)indexerValue];

            PropertyInfo info = propertyValue.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { indexerValue.GetType() }, null);
            if(null == info)
                throw new ArgumentException(String.Format(Errors.InvalidIndexerExpression, propertyIndexedName), "propertyIndexedName");

            return info.GetValue(propertyValue, new object[] { indexerValue });

        }

        private static PropertyDescriptorCollection GetPropertiesFromCache(object container)
        {
            if (container is ICustomTypeDescriptor)
            {
                return TypeDescriptor.GetProperties(container);
            }
            PropertyDescriptorCollection col = null;
            Type key = container.GetType();
            if (!_propertyCache.TryGetValue(key, out col))
            {
                col = TypeDescriptor.GetProperties(key);
                _propertyCache.TryAdd(key, col);
            }
            return col;
        }
    }
}
