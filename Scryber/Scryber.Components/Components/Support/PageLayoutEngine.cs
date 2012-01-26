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
using Scryber.Styles;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Components.Support
{
    internal class PageLayoutEngine : ContainerLayoutEngine
    {


        #region protected PDFPage Page {get;}

        private PDFPage _pg;

        protected PDFPage Page
        {
            get { return _pg; }
        }

        #endregion

        #region protected PDFStyle PageStyle {get;}

        private PDFStyle _pgstyle;

        protected PDFStyle PageStyle
        {
            get { return _pgstyle; }
        }

        #endregion

        #region protected PDFPageSize PageSize

        private PDFPageSize _pgsize;

        protected PDFPageSize PageSize
        {
            get { return _pgsize; }
            set { _pgsize = value; }
        }

        #endregion

        #region protected OverflowAction Overflow {get;}

        private OverflowAction _overflow;

        protected OverflowAction Overflow
        {
            get { return _overflow; }
            set { _overflow = value; }
        }

        #endregion

        #region protected PDFPageNumbering Numbering { get; }

        private PDFPageNumbering _numbers;

        protected PDFPageNumbering Numbering
        {
            get { return _numbers; }
        }

        #endregion

        #region protected int CurrentPageIndex { get; set; }

        private int _currpgindex;

        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        protected override int CurrentPageIndex
        {
            get { return _currpgindex; }
            set { _currpgindex = value; }
        }

        #endregion 

        public PageLayoutEngine(PDFPage pg, IPDFLayoutEngine parent, PDFLayoutContext context)
            : base(pg, parent, context)
        {
            if (null == pg)
                throw new ArgumentNullException("pg");
            
            this._pg = pg;
            this._pgstyle = context.StyleStack.GetFullStyle(pg);

            this.InitSizeAndOverflow();
        }

        protected virtual void InitSizeAndOverflow()
        {
            PDFPageStyle paper;
            if (this.PageStyle.TryGetPaper(out paper))
                _pgsize = paper.CreatePageSize();
            else
                _pgsize = PDFPageSize.A4;

            PDFOverflowStyle overflow;
            if (this.PageStyle.TryGetOverflow(out overflow))
                _overflow = overflow.Action;
            else
                _overflow = OverflowAction.None;

            _numbers = this.GetPageNumbering(this.PageStyle);
        }

        

        public override PDFSize Layout(PDFSize avail, int startPageIndex)
        {
            
            PDFSize total = this.PageSize.Size;
            PDFRect bounds = new PDFRect(PDFPoint.Empty, total);
            PDFThickness margthick;
            PDFThickness padthick;
            PDFRect contentrect;
            PDFRect border;
            contentrect = GetPageContentRect(bounds, out margthick, out padthick, out border);
            this.Page.RegisterPageNumbering(startPageIndex, this.Numbering);
            
            
            PDFSize size = base.Layout(contentrect.Size, startPageIndex);

            //We have to push the arangement for this engine onto the page
            this.SetNewPageArrangement(this.CurrentPageIndex, false);
            

            return size;
        }

        protected override void LayoutHeaderForContainer(IPDFContainerComponent container, IPDFTopAndTailedComponent topandtail)
        {
            if (topandtail == this.Page)
            {
                LayoutPageHeaderAndFooter();
            }
            else
                base.LayoutHeaderForContainer(container, topandtail);
        }

        private void LayoutPageHeaderAndFooter()
        {
            bool isHeader;
            IPDFTemplate headtemplate = this.GetCurrentHeaderTemplate(this.CurrentPageIndex);
            IPDFTemplate foottemplate = this.GetCurrentFooterTemplate(this.CurrentPageIndex);

            if (headtemplate != null)
            {
                PDFPageHeader header = new PDFPageHeader();
                this.Page.AddGeneratedHeader(header, this.CurrentPageIndex);

                InstantiateTemplateForPage(header, headtemplate);

                isHeader = true;
                PDFSize consumed = this.LayoutTopAndTailComponent(header, this.CurrentSpace, isHeader);
               
                this.CurrentSpace = new PDFRect(this.CurrentSpace.X, this.CurrentSpace.Y + consumed.Height, this.CurrentSpace.Width, this.CurrentSpace.Height - consumed.Height);
                
            }

            if (foottemplate != null)
            {
                PDFPageFooter footer = new PDFPageFooter();
                this.Page.AddGeneratedFooter(footer, this.CurrentPageIndex);

                InstantiateTemplateForPage(footer, foottemplate);
                
                isHeader = false;
                PDFSize consumed = this.LayoutTopAndTailComponent(footer, this.CurrentSpace, isHeader);
                
                this.CurrentSpace = new PDFRect(this.CurrentSpace.X, this.CurrentSpace.Y, this.CurrentSpace.Width, this.CurrentSpace.Height - consumed.Height);
            }
        }

        protected virtual IPDFTemplate GetCurrentHeaderTemplate(int pageIndex)
        {
            return this.Page.Header;
        }

        protected virtual IPDFTemplate GetCurrentFooterTemplate(int pageIndex)
        {
            return this.Page.Footer;
        }

        /// <summary>
        /// Creates a concrete instance of the correct template into the PDFLayoutTemplate component
        /// </summary>
        /// <param name="container">A non null instance of the layout template component to contain the generated template components</param>
        protected virtual void InstantiateTemplateForPage(PDFLayoutTemplateComponent container, IPDFTemplate template)
        {
            if (null == template)
                throw new ArgumentNullException("template");
            if (null == container)
                throw new ArgumentNullException("container");

            container.InstantiateTemplate(template, this.LayoutContext, this.CurrentSpace, this.CurrentPageIndex);
        }

        

        /// <summary>
        /// Lays out the PDFLayoutTemplateComponent (as a viewport) and then sets the arrarangement of that component
        /// either at the top of the page or the bottom - based on the flag 'putAtTop'
        /// </summary>
        /// <param name="template">The template component to layout</param>
        /// <param name="available">The available space to layout in</param>
        /// <param name="putAtTop">true if this should be at the top otherwise false for a footer</param>
        /// <returns>The total measured size of the component</returns>
        private PDFSize LayoutTopAndTailComponent(PDFLayoutTemplateComponent template, PDFRect available, bool putAtTop)
        {
            PDFStyle style = template.GetAppliedStyle();
            this.Styles.Push(style);
            PDFStyle full = this.Styles.GetFullStyle(template);

            PDFSize measured;
            using (IPDFLayoutEngine engine = template.GetEngine(this, this.LayoutContext))
            {
                PDFPageRequestHandler handler = new PDFPageRequestHandler(CannotOverflowPageHandler);
                engine.RequestNewPage += handler;
                measured = engine.Layout(available.Size, this.CurrentPageIndex);
                engine.RequestNewPage -= handler;
            }

            PDFRect bounds = new PDFRect(0, 0, available.Width, measured.Height);
            
            if (!putAtTop) // put at bottom
                bounds.Y = available.Y + available.Height - bounds.Height;

            PDFThickness margins = PDFThickness.Empty();
            PDFThickness padding = PDFThickness.Empty();

            this.SetComponentArrangement(template, this.CurrentPageIndex, bounds, PositionMode.Relative, full, margins, padding);

            //return used space
            return bounds.Size;
        }

        /// <summary>
        /// PDFPageRequestHandler implementation that does rejects and
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CannotOverflowPageHandler(object sender, PDFPageRequestArgs args)
        {
            args.RejectNewPage();
        }

        protected override void LayoutFooterForContainer(IPDFContainerComponent container, IPDFTopAndTailedComponent topandtail)
        {
            if (topandtail == this.Page)
            {
               //We do nothing here because we layout the footer in the LayoutHeaderForContainer method
            }
            else
                base.LayoutFooterForContainer(container, topandtail);
        }

        

        protected override void OnRequestNewPage(PDFPageRequestArgs args)
        {
            if (this.Overflow == OverflowAction.NewPage)
            {
                PDFRect content = this.SetNewPageArrangement(this.CurrentPageIndex, true);

                int index = this.CurrentPageIndex + 1;

                args.AllowNewPage(index, content);

                this.CurrentPageIndex = index;
                this.BeginNewPage(index, content);
                
               
            }
            else
                args.RejectNewPage();

        }

        /// <summary>
        /// Override the base implementation to layout the headers and footers
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="newbounds"></param>
        /// <returns></returns>
        protected override PDFRect BeginNewPage(int pageindex, PDFRect newbounds)
        {
            PDFRect avail = base.BeginNewPage(pageindex, newbounds);
            this.LayoutPageHeaderAndFooter();

            //return the current space as this will have been adjusted for any headers and footers
            return this.CurrentSpace;
        }

        /// <summary>
        /// Applies a new page arrangement with the specified index and returns the content rect
        /// </summary>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        protected PDFRect SetNewPageArrangement(int pageindex, bool registerPageNumbers)
        {
            PDFSize total = this.PageSize.Size;
            PDFRect bounds = new PDFRect(PDFPoint.Empty, total);
            PDFThickness margthick;
            PDFThickness padthick;
            PDFRect contentrect;
            PDFRect border;
            contentrect = GetPageContentRect(bounds, out margthick, out padthick, out border);
            
            if(registerPageNumbers)
                this.Page.RegisterPageNumbering(pageindex, this.Numbering);

            this.RootSpace = bounds;
            this.CurrentSpace = new PDFRect(PDFPoint.Empty, contentrect.Size);

            //Set this pages page arrangement
            this.SetComponentArrangement(Page, this.CurrentPageIndex, bounds, PositionMode.Flow, this.RootStyle, margthick, padthick);

            return contentrect;
        }

        protected virtual void SetComponentArrangement(PDFComponent comp, int pgindex,  PDFRect bounds, PositionMode positionMode, PDFStyle style, PDFThickness margthick, PDFThickness padthick)
        {
            PDFComponentMultiArrangement arrange = new PDFComponentMultiArrangement();
            arrange.Bounds = bounds;
            arrange.Display = true;
            arrange.Margins = margthick;
            arrange.Padding = padthick;
            arrange.PageIndex = pgindex;
            arrange.PositionMode = positionMode;
            arrange.Border = bounds.Inset(margthick);
            arrange.Content = arrange.Border.Inset(padthick);
            arrange.AppliedStyle = style;

            comp.SetArrangement(arrange);
        }

        protected PDFRect GetPageContentRect(PDFRect bounds, out PDFThickness margthick, out PDFThickness padthick, out PDFRect borderrect)
        {
            PDFMarginsStyle marg;
            PDFPaddingStyle pad;
            margthick = PDFThickness.Empty();
            padthick = PDFThickness.Empty();

            borderrect = bounds;
            if (this.PageStyle.TryGetMargins(out marg))
            {
                margthick = marg.GetThickness();
                borderrect = borderrect.Inset(margthick);
            }

            PDFRect contentrect = borderrect;
            if (this.PageStyle.TryGetPadding(out pad))
            {
                padthick = pad.GetThickness();
                contentrect = borderrect.Inset(padthick);
            }
            return contentrect;
        }

        protected PDFPageNumbering GetPageNumbering(PDFStyle style)
        {
            PDFPageNumbering num;
            PDFPageStyle pgstyle;
            if (!style.TryGetPaper(out pgstyle))
                num = null;
            else
                num = pgstyle.CreateNumbering();
            return num;
        }


        protected PDFPageSize GetPageSize(PDFStyle style)
        {
            PDFPageStyle pg;
            if (!style.TryGetPaper(out pg))
                return PDFPageSize.A4;
            else
                return pg.CreatePageSize();
        }

        
    }
}
