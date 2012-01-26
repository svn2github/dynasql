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
using Scryber.Native;
using System.CodeDom;

namespace Scryber.Drawing
{
    [PDFParsableValue()]
    public class PDFColor : PDFObject, IEquatable<PDFColor>, IPDFSimpleCodeDomValue
    {
        private ColorSpace _cs;

        /// <summary>
        /// Gets or sets the current colorspace.
        /// </summary>
        public ColorSpace ColorSpace
        {
            get { return _cs; }
        }

        private System.Drawing.Color _c;
        /// <summary>
        /// Gets the internal color of the item
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return _c; }
        }

        public PDFReal Red
        {
            get { return new PDFReal(GetPDFColorComponent(Color.R)); }
        }

        public PDFReal Green
        {
            get { return new PDFReal(GetPDFColorComponent(Color.G)); }
        }

        public PDFReal Blue
        {
            get { return new PDFReal(GetPDFColorComponent(Color.B)); }
        }

        public PDFReal Alpha
        {
            get { return new PDFReal(GetPDFColorComponent(Color.A)); }
        }

        public PDFReal Gray
        {
            get
            {
                PDFReal val = this.Red + this.Green + this.Blue;
                val = new PDFReal(val.Value / 3.0);
                return val;
            }
        }

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        public PDFColor(ColorSpace cs, double one, double two, double three)
            : this(cs, System.Drawing.Color.FromArgb(
                                            GetSystemColorComponent(one),
                                            GetSystemColorComponent(two),
                                            GetSystemColorComponent(three)))
        {
            if (cs != ColorSpace.RGB && cs != ColorSpace.RGB)
                throw new ArgumentOutOfRangeException(String.Format(Errors.ColorValueIsNotCurrentlySupported, this.ColorSpace.ToString()), "cs");
        }


        public PDFColor(double gray)
            : this(ColorSpace.G, System.Drawing.Color.FromArgb(
                                            GetSystemColorComponent(gray),
                                            GetSystemColorComponent(gray),
                                            GetSystemColorComponent(gray)))
        {

        }

        /// <summary>
        /// Creates a new instance of the PDF Color with an RGB Color Space and Transparent colors
        /// </summary>
        public PDFColor(double red, double green, double blue)
            : this(ColorSpace.RGB, 
                    System.Drawing.Color.FromArgb(
                            GetSystemColorComponent(red), 
                            GetSystemColorComponent(green), 
                            GetSystemColorComponent(blue)))
        {
        }

        /// <summary>
        /// Creates a new instance of the PDF Color with the specified Color Space and Color
        /// </summary>
        /// <param name="cs">The color space to use</param>
        /// <param name="c">The specific color</param>
        public PDFColor(ColorSpace cs, System.Drawing.Color c)
            : this(cs,c,PDFObjectTypes.Color)
        {
            this._cs = cs;
            this._c = c;
        }

        protected PDFColor(ColorSpace cs, System.Drawing.Color c, PDFObjectType type)
            : base(type)
        {
            if (cs == ColorSpace.LAB || cs == ColorSpace.Custom)
                throw new ArgumentOutOfRangeException(String.Format(Errors.ColorValueIsNotCurrentlySupported, this.ColorSpace.ToString()), "cs");
            this._cs = cs;
            this._c = c;
        }

        #endregion

        public bool Equals(PDFColor other)
        {
            if (null == other)
                return false;
            else if (this.ColorSpace != other.ColorSpace)
                return false;
            else if (this.Color != other.Color)
                return false;
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is PDFColor)
                return this.Equals((PDFColor)obj);
            else
                return false;
        }

        #region public override string ToString()

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder(20);
            sb.Append(this.ColorSpace.ToString());
            sb.Append(" (");
            switch (this.ColorSpace)
            {
                case ColorSpace.G:
                    sb.Append(this.Alpha.Value.ToString());
                    sb.Append(",");
                    sb.Append(this.Gray.Value.ToString());
                    break;
                case ColorSpace.HSL:
                    sb.Append(this.Alpha.Value.ToString());
                    sb.Append(",");
                    sb.Append(this.Color.GetHue());
                    sb.Append(",");
                    sb.Append(this.Color.GetSaturation());
                    sb.Append(",");
                    sb.Append(this.Color.GetBrightness());
                    break;
                case ColorSpace.RGB:
                    sb.Append(this.Alpha.Value.ToString());
                    sb.Append(",");
                    sb.Append(this.Color.R);
                    sb.Append(",");
                    sb.Append(this.Color.G);
                    sb.Append(",");
                    sb.Append(this.Color.B);
                    break;
                case ColorSpace.LAB:
                case ColorSpace.Custom:
                default:
                    throw new ArgumentOutOfRangeException("this.ColorSpace",String.Format(Errors.ColorValueIsNotCurrentlySupported,this.ColorSpace.ToString()));
                    
            }
            sb.Append(")");
            return sb.ToString();
        }

        #endregion

        #region Parse(string)

        /// <summary>
        /// Creates a new PDFColor from the provided string  rgb(Red,Green,Blue) or g(Gray) or #GG or #RGB or #RRGGBB
        /// </summary>
        /// <param name="color">The string to parse</param>
        /// <returns>A new instance of the PDF Color</returns>
        public static PDFColor Parse(string color)
        {
            ColorSpace cs = ColorSpace.RGB;

            if (string.IsNullOrEmpty(color))
                return null;

            if (color.IndexOf("(") > 0)
            {
                string s = color.Trim().Substring(0, color.IndexOf("(")).ToUpper();
                color = color.Substring(s.Length + 1);
                int close = color.IndexOf(")");
                if (close < 0 || close >= color.Length)
                    throw new FormatException("The color string was in the incorrect format");

                color = color.Substring(0, close);//remove closing bracket

                if (Enum.IsDefined(typeof(ColorSpace), cs) == false)
                    throw new ArgumentOutOfRangeException("The color space pre-pend '" + cs + "' is not defined");
                else
                    cs = (ColorSpace)Enum.Parse(typeof(ColorSpace), s.Trim());

                string[] vals = color.Split(',');
                int[] rgbs = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    if (i < vals.Length)
                    {
                        rgbs[i] = int.Parse(vals[i]);
                    }
                    else
                    {
                        rgbs[i] = 0;
                    }
                }

                if (cs == ColorSpace.G)
                    return new PDFColor(cs, System.Drawing.Color.FromArgb(rgbs[0], rgbs[0], rgbs[0]));
                else if (cs == ColorSpace.RGB)
                    return new PDFColor(cs, System.Drawing.Color.FromArgb(rgbs[0], rgbs[1], rgbs[2]));
                else if (cs == ColorSpace.HSL)
                    return new PDFColor(cs, rgbs[0], rgbs[1], rgbs[2]);
                else
                    throw new NotSupportedException("This color space is not currently supported");

            }
            else if (color.StartsWith("#"))
            {
                cs = ColorSpace.RGB;


                string r;
                string g;
                string b;
                string a = "FF";
                if (color.Length == 2)
                {
                    cs = ColorSpace.G;
                    r = color.Substring(1, 1);
                    r = r + r;
                    g = r;
                    b = r;
                }
                else if (color.Length == 3)
                {
                    cs = ColorSpace.G;
                    r = color.Substring(1, 2);
                    g = r;
                    b = r;
                }
                else if (color.Length == 4)
                {
                    r = color.Substring(1, 1);
                    r = r + r;
                    g = color.Substring(2, 1);
                    g = g + g;
                    b = color.Substring(3, 1);
                    b = b + b;
                }
                else if (color.Length == 7)
                {
                    r = color.Substring(1, 2);
                    g = color.Substring(3, 2);
                    b = color.Substring(5, 2);
                }
                else
                    throw new ArgumentException("Could not understand the color string '" + color + "'. It must be in the format #G or #GG or #RGB or #RRGGBB");

                byte ab = byte.Parse(a, System.Globalization.NumberStyles.HexNumber);
                byte rb = byte.Parse(r, System.Globalization.NumberStyles.HexNumber);
                byte gb = byte.Parse(g, System.Globalization.NumberStyles.HexNumber);
                byte bb = byte.Parse(b, System.Globalization.NumberStyles.HexNumber);

                return new PDFColor(cs, System.Drawing.Color.FromArgb((int)ab, System.Drawing.Color.FromArgb((int)rb, (int)gb, (int)bb)));

            }
            else if (color.Equals("inherit", StringComparison.CurrentCultureIgnoreCase))
                return null;
            else
            {
                PDFColor c = PDFColors.FromName(color);
                return c;
            }
        }

        #endregion


        
        private static float GetPDFColorComponent(byte p)
        {
            return GetPDFColorComponent(Convert.ToSingle(p));
        }

        private static float GetPDFColorComponent(float val)
        {
            return (1.0F / 255.0F) * val;
        }

        private static int GetSystemColorComponent(double val)
        {
            if (val > 1.0 || val < 0.0)
                throw new ArgumentOutOfRangeException("val", String.Format(Errors.ColorComponentMustBeBetweenZeroAndOne, val));
            val = 255.0 * val;
            return (int)val;
        }

        public static implicit operator PDFColor(System.Drawing.Color rgbcolor)
        {
            return new PDFColor(ColorSpace.RGB, rgbcolor);
        }

        private static PDFColor _transparent = new PDFColor(ColorSpace.RGB, System.Drawing.Color.Transparent);

        public static PDFColor Transparent
        {
            get { return _transparent; }
        }

        public bool IsEmpty
        {
            get { return this.Color.IsEmpty; }
        }

        #region IPDFSimpleCodeDomValue Members

        public System.CodeDom.CodeExpression GetValueExpression()
        {
            //new PDFColor(ColorSpace,one,two,three)
            CodePrimitiveExpression one = new CodePrimitiveExpression(this.Red.Value);
            CodePrimitiveExpression two = new CodePrimitiveExpression(this.Green.Value);
            CodePrimitiveExpression three = new CodePrimitiveExpression(this.Blue.Value);
            CodePropertyReferenceExpression cs = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(ColorSpace)), this.ColorSpace.ToString());
            return new CodeObjectCreateExpression(typeof(PDFColor), cs, one, two, three);
        }

        #endregion
    }
}
