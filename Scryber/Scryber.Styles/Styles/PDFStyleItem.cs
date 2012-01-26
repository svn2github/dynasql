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
using Scryber.Drawing;

namespace Scryber.Styles
{
    public class PDFStyleItem : PDFObject, ICloneable, IPDFBindableComponent
    {
        /// <summary>
        /// Simple delegate that when invoked, can convert from a string to the requested type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected delegate T ParseHandler<T>(string value);

        #region public event PDFDataBindEventHandler DataBinding + OnDataBinding(args)

        public event PDFDataBindEventHandler DataBinding;

        protected virtual void OnDataBinding(PDFDataBindEventArgs args)
        {
            if (this.DataBinding != null)
                this.DataBinding(this, args);
        }

        #endregion

        #region public event EventHandler DataBound + OnDataBound(args)

        public event PDFDataBindEventHandler DataBound;

        protected virtual void OnDataBound(PDFDataBindEventArgs args)
        {
            if (this.DataBound != null)
                this.DataBound(this, args);
        }

        #endregion

        private Dictionary<string, PDFStyleValue> _attributes = new Dictionary<string, PDFStyleValue>();

        protected Dictionary<string, PDFStyleValue> Attributes
        {
            get { return this._attributes; }
        }

        public int Count
        {
            get { return this._attributes.Count; }
        }

        public string this[string attribute]
        {
            get
            {
                PDFStyleValue val;
                if (this.Attributes.TryGetValue(attribute, out val))
                    return val.Text;
                else
                    return string.Empty;
            }
            set
            {
                this._attributes[attribute] = new PDFStyleValue(value);
            }
        }

        private bool _inherited;

        /// <summary>
        /// Gets the flag to identify if this style item is inherited by child Components (Font, Fill, Stroke, etc...) 
        /// </summary>
        public bool IsInherited
        {
            get { return _inherited; }
        }

        ///// <summary>
        ///// This is a special attribute that can be used when parsing to remove any set values.
        ///// 
        ///// </summary>
        //[PDFAttribute("clear")]
        //public string ClearAttribute
        //{
        //    get { return string.Empty; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            return;
        //        else if (value == "*")
        //            this.Clear();
        //        else if (value.IndexOf(' ') < 0)
        //            this.Remove(value);
        //        else
        //        {
        //            string[] all = value.Split(' ');
        //            foreach (string item in all)
        //            {
        //                if (!string.IsNullOrEmpty(item))
        //                    this.Remove(item);
        //            }
        //        }
        //    }

        //}

        public PDFStyleItem(PDFObjectType type, bool inherited)
            : base(type)
        {
            this._inherited = inherited;
        }

        public bool Contains(string attribute)
        {
            return this.Attributes.ContainsKey(attribute);
        }

        public void Set(string attribute, string value)
        {
            if (this._attributes.ContainsKey(attribute))
                this._attributes.Remove(attribute);
            this._attributes.Add(attribute, new PDFStyleValue(value));
        }

        public bool IsDefined(string attribute)
        {
            return this.Attributes.ContainsKey(attribute);
        }

        public void Remove(string attribute)
        {
            this._attributes.Remove(attribute);
        }

        public void Clear()
        {
            this._attributes.Clear();
        }

        protected void SetValue(string attribute, object value)
        {
            this.Attributes[attribute] = new PDFStyleValue(value.ToString(), value);
        }

        protected void SetValue(string attribute, string text, object value)
        {
            this.Attributes[attribute] = new PDFStyleValue(text, value);
        }

        protected PDFStyleValue GetRequiredValue(string attribute)
        {
            return this.Attributes[attribute];
        }

        protected PDFStyleValue GetOptionalValue(string attribute)
        {
            PDFStyleValue val;
            this.Attributes.TryGetValue(attribute,out val);
            return val;
        }

        protected bool GetStringValue(string attribute, bool required, out string value)
        {
            return GetParsedAttribute<String>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                delegate(string val) { return val; }, //anonymous delegate to return the attribute value as the parsed value
                out value); 
        }

        protected bool GetIntegerValue(string attribute, bool required, out int value)
        {
            return GetParsedAttribute<int>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<int>(int.Parse),
                out value);
        }

        protected bool GetEnumValue(string attribute, Type enumType, bool required, out object value)
        {
            return GetParsedAttribute<object>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                delegate(string val) { return Enum.Parse(enumType, val, true); },
                out value);
        }

        protected bool GetFlagsEnumValue(string attribute, Type enumType, bool required, out object value)
        {
            return GetParsedAttribute<object>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                delegate(string val) 
                {
                    return String.IsNullOrEmpty(val) ? (object)0 : Enum.Parse(enumType, val.Replace(' ', ','), true);
                },
                out value);
        }

        protected bool GetFloatValue(string attribute, bool required, out float value)
        {
            return GetParsedAttribute<float>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<float>(float.Parse),
                out value);
        }

        protected bool GetDoubleValue(string attribute, bool required, out double value)
        {
            return GetParsedAttribute<double>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<double>(double.Parse),
                out value);
        }

        protected bool GetUnitValue(string attribute, bool required, out PDFUnit value)
        {
            return GetParsedAttribute<PDFUnit>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<PDFUnit>(PDFUnit.Parse),
                out value);
        }

        protected bool GetDashValue(string attribute, bool required, out PDFDash value)
        {
            return GetParsedAttribute<PDFDash>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<PDFDash>(PDFDash.Parse),
                out value);
        }

        protected bool GetPointValue(string attribute, bool required, out System.Drawing.PointF value)
        {
            return GetParsedAttribute<System.Drawing.PointF>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                delegate(string val)
                {
                    if (string.IsNullOrEmpty(val))
                        return System.Drawing.PointF.Empty;
                    else
                    {
                        try
                        {
                            string[] items = val.Split(',');
                            return new System.Drawing.PointF(float.Parse(items[0].Trim()), float.Parse(items[1].Trim()));
                        }
                        catch (Exception ex)
                        {
                            throw new FormatException("Could not parse the string '" + val + "' to a Point. The expected format is D.D,D.D where D is a digit", ex);
                        }
                    }
                },
                out value);
        }

        protected bool GetSizeValue(string attribute, bool required, out System.Drawing.SizeF value)
        {
            return GetParsedAttribute<System.Drawing.SizeF>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                delegate(string val)
                {
                    if (string.IsNullOrEmpty(val))
                        return System.Drawing.SizeF.Empty;
                    else
                    {
                        try
                        {
                            string[] items = val.Split(',');
                            return new System.Drawing.SizeF(float.Parse(items[0].Trim()), float.Parse(items[1].Trim()));
                        }
                        catch (Exception ex)
                        {
                            throw new FormatException("Could not parse the string '" + val + "' to a Size. The expected format is D.D,D.D where D is a digit", ex);
                        }
                    }
                },
                out value);
        }

        protected bool GetColorValue(string attribute, bool required, out PDFColor color)
        {
            return GetParsedAttribute<PDFColor>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<PDFColor>(PDFColor.Parse),
                out color);
        }

        protected bool GetBoolValue(string attribute, bool required, out bool value)
        {
            return GetParsedAttribute<bool>(required ? GetRequiredValue(attribute) : GetOptionalValue(attribute),
                new ParseHandler<bool>(bool.Parse),
                out value);
        }

        #region protected static bool GetParsedAttribute<T>(PDFStyleValue sval, ParseHandler<T> parser, out T parsed)

        protected static bool GetParsedAttribute<T>(PDFStyleValue sval, ParseHandler<T> parser, out T parsed)
        {
            bool success = false;
            if (sval == null)
            {
                parsed = default(T);
            }
            else if (sval.HasParsedValue)
            {
                try
                {
                    parsed = (T)sval.Value;
                    success = true;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException("The style attribute for this item is not a " + typeof(T).Name + " value", ex);
                }
            }
            else
            {
                try
                {
                    parsed = parser(sval.Text);
                }
                catch (OutOfMemoryException) { throw; }
                catch (StackOverflowException) { throw; }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception ex)
                {
                    throw new InvalidCastException("The style attribute for this item is not a " + typeof(T).Name + " value", ex);
                }

                sval.SetParsedValue(parsed);
                success = true;
            }


            return success;

        }

        #endregion

        public virtual void MergeInto(PDFStyleItem si)
        {
            foreach (string attr in this.Attributes.Keys)
            {
                si.Attributes[attr] = this.Attributes[attr];
            }
        }

        //
        // databinding
        //

        public void DataBind(PDFDataContext context)
        {
            PDFDataBindEventArgs args = new PDFDataBindEventArgs(context);
            this.OnDataBinding(args);
            this.DoDataBind(context, true);
            this.OnDataBound(args);
        }

        protected virtual void DoDataBind(PDFDataContext context, bool includechildren)
        {

        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.Type);
            sb.Append("{");
            bool first = true;
            foreach (KeyValuePair<string,PDFStyleValue> kvp in this.Attributes)
            {
                if (first == false)
                    sb.Append("; ");
                sb.Append(kvp.Key);
                sb.Append(":");
                sb.Append(kvp.Value.Text);
                first = false;
            }
            sb.Append("}");
            return sb.ToString();
        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region ICloneable<PDFStyleItem> Members

        public PDFStyleItem Clone()
        {
            PDFStyleItem si = (PDFStyleItem)base.MemberwiseClone();
            si._attributes = new Dictionary<string, PDFStyleValue>();
            this.MergeInto(si);
            return si;
            
        }

        #endregion

        
    }
}
