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
    public class PDFPageNumbering
    {
        /// <summary>
        /// Gets or sets the page numbering style
        /// </summary>
        public PageNumberStyle NumberStyle { get; set; }

        /// <summary>
        /// Gets or sets the starting page index
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the prefix to the page number
        /// </summary>
        public string Prefix { get; set; }

        public bool Equals(PDFPageNumbering num)
        {
            return this.NumberStyle == num.NumberStyle && this.StartIndex == num.StartIndex
                && string.Equals(this.Prefix, num.Prefix);
        }

        public override bool Equals(object obj)
        {
            return this.Equals((PDFPageNumbering)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation for the page
        /// </summary>
        /// <param name="forpageindex">The zero based page index</param>
        /// <returns></returns>
        public string GetPageNumber(int forpageindex)
        {
            int offset = forpageindex + this.StartIndex;
            string num;
            switch (this.NumberStyle)
            {
                case PageNumberStyle.Decimals:
                    num = offset.ToString();
                    break;
                case PageNumberStyle.UppercaseRoman:
                    num = GetRomanUpper(offset);
                    break;
                case PageNumberStyle.LowercaseRoman:
                    num = GetRomanLower(offset);
                    break;
                case PageNumberStyle.UppercaseLetters:
                    num = GetLetterUpper(offset);
                    break;
                case PageNumberStyle.LowercaseLetters:
                    num = GetLetterLower(offset);
                    break;
                default:
                    num = string.Empty;
                    break;
            }
            if (!string.IsNullOrEmpty(this.Prefix))
                return this.Prefix + num;
            else
                return num;
        }

        private static string GetRomanUpper(int page)
        {
            StringBuilder sb = new StringBuilder();
            GenerateNumber(sb, 1000, 'M', ref page);
            GenerateNumber(sb, 500, 'D', ref page);
            GenerateNumber(sb, 100, 'C', ref page);
            GenerateNumber(sb, 50, 'L', ref page);
            GenerateNumber(sb, 10, 'X', ref page);
            GenerateNumber(sb, 5, 'V', ref page);
            GenerateNumber(sb, 1, 'I', ref page);
            ReplaceRomanShortcuts(sb, true);
            return sb.ToString();
        }

        private static string GetRomanLower(int page)
        {
            StringBuilder sb = new StringBuilder();
            GenerateNumber(sb, 1000, 'm', ref page);
            GenerateNumber(sb, 500, 'd', ref page);
            GenerateNumber(sb, 100, 'c', ref page);
            GenerateNumber(sb, 50, 'l', ref page);
            GenerateNumber(sb, 10, 'x', ref page);
            GenerateNumber(sb, 5, 'v', ref page);
            GenerateNumber(sb, 1, 'i', ref page);
            ReplaceRomanShortcuts(sb, false);
            return sb.ToString();
        }

        private static void ReplaceRomanShortcuts(StringBuilder sb, bool upper)
        {
            if (upper)
                sb.Replace("IIII", "IV").Replace("VIV", "IX").Replace("XXXX", "XL").Replace("LXL", "XC").Replace("CCCC", "CD").Replace("DCD", "CM");
            else
                sb.Replace("iiii", "iv").Replace("viv", "ix").Replace("xxxx", "xl").Replace("lxl", "xc").Replace("cccc", "cd").Replace("dcd", "cm");
        }

        private static void GenerateNumber(StringBuilder sb,int magnitude, char letter, ref int page)
        {
            while (page >= magnitude)
            {
                page -= magnitude;
                sb.Append(letter);
            }
        }

        private const char UpperCharStart = (char)(((int)'A') - 1);
        private const char LowerCharStart = (char)(((int)'a') - 1);
        private static string GetLetterUpper(int page)
        {
            StringBuilder sb = new StringBuilder();
            GenerateLetters(sb, page, UpperCharStart);
            return sb.ToString();
        }

        private static string GetLetterLower(int page)
        {
            StringBuilder sb = new StringBuilder();
            GenerateLetters(sb, page, LowerCharStart);
            return sb.ToString();
        }

        private static void GenerateLetters(StringBuilder sb, int page, char first)
        {
            int num = 1;
            while (page > 26)
            {
                page -= 26;
                num++;
            }
            char charval = (char)(((int)first) + page);
            sb.Insert(0, charval.ToString(), num);
        }
    }

    
}
