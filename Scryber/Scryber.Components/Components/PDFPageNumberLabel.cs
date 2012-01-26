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

namespace Scryber.Components
{
    [PDFParsableComponent("PageNumber")]
    public class PDFPageNumberLabel : PDFTextBase
    {
        

        #region public string PageLabelFormat 

        private string _format;
        /// <summary>
        /// Gets or sets the format of the string to be displayed. Using the indexes of the DisplayFields
        /// </summary>
        [PDFAttribute("format")]
        public string PageLabelFormat 
        { 
            get { return this._format; }
            set { this._format = value; }
        }

        #endregion

        //Flag to identify if we are 
        //rendering otherwise we are just laying out
        private bool _rendering = false;

        //local render value of the current page index
        private int _pageindex = -1;

        protected override string BaseText
        {
            get
            {
                return this.GetDisplayText(_rendering);
            }
            set
            {
                throw new InvalidOperationException(Errors.CannotSetBaseTextOfPageNumber);
                //Do Nothing
            }
        }


        public PDFPageNumberLabel()
            : base(PDFObjectTypes.Text)
        {
        }

        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, Scryber.Styles.PDFStyle fullstyle, Scryber.Drawing.PDFGraphics graphics, PDFWriter writer)
        {
            this.TextBlock = null;
            this._rendering = true;
            this._pageindex = context.PageIndex;
            try
            {
                return base.DoRenderToPDF(context, fullstyle, graphics, writer);
            }
            finally
            {
                this._rendering = false;
                this._pageindex = -1;
            }
        }

        #region private string GetDisplayText(bool rendering)

        /// <summary>
        /// Gets the full text for the page label
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>
        /// 1. If there is no PageFormat, then the current page index in the label style for this page is used
        /// 2. If there is a format then the format is used with the following fields...
        ///                     {0} = page label, {1} = total label for this pages numbering group, 
        ///                     {2} global page index, {3} global page count
        /// </remarks>
        private string GetDisplayText(bool rendering)
        {
            //HACK: Because we don't know howmany pages there are until we have layed out
            //we use a sample and hope it's OK!
            int pageindex = this._pageindex;
            int totalpages =  this.Document.TotalPageCount;

            if (!rendering && totalpages < 1000)
                totalpages = 999;

            int sectionindex, sectioncount;
            PDFPageNumbering num = this.Document.GetNumbering(pageindex, out sectionindex, out sectioncount);
            //HACK: Again sampe number
            if (!rendering && sectioncount < 1000)
                sectioncount = 50;

            pageindex++; // convert to '1' based index
            string numname = num.GetPageNumber(sectionindex);
            string sectname = num.GetPageNumber(sectioncount);

            if (string.IsNullOrEmpty(this.PageLabelFormat))
                return numname;
            else
                return String.Format(this.PageLabelFormat, numname, sectname, pageindex, totalpages);
            
            
        }

        #endregion

        #region private string GetDisplayNameValue(int index, int total, PDFPageNumbering num, PageNumberDisplay numtype)
        
        /// <summary>
        /// Gets the actual string value for an individual PageNumberDisplay field
        /// </summary>
        /// <param name="index"></param>
        /// <param name="total"></param>
        /// <param name="num"></param>
        /// <param name="numtype"></param>
        /// <returns></returns>
        private string GetDisplayNameValue(int index, int total, PDFPageNumbering num, PageNumberDisplay numtype)
        {
            string val;
            switch (numtype)
            {
                case PageNumberDisplay.PageIndex:
                    val = index.ToString();
                    break;
                case PageNumberDisplay.PageCount:
                    val = total.ToString();
                    break;
                case PageNumberDisplay.PageIndexLabel:
                    val = num.GetPageNumber(index);
                    break;
                case PageNumberDisplay.PageCountLabel:
                    val = num.GetPageNumber(total);
                    break;
                case PageNumberDisplay.SectionOffset:
                    val = this.Page.GetPageIndex(this).ToString();
                    break;
                case PageNumberDisplay.SectionOffsetLabel:
                    val = num.GetPageNumber(this.Page.GetPageIndex(this));
                    break;
                case PageNumberDisplay.SectionCount:
                case PageNumberDisplay.SectionCountLabel:
                default:
                    //TODO: Support the declaration of sections and their labels in PageLabel
                    throw new NotSupportedException(numtype.ToString());
            }
            return val;
        }

        #endregion

    }
}
