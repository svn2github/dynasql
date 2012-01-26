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
using Scryber.Styles;

namespace Scryber.Components.Support
{
    internal class DocumentLayoutEngine : IPDFLayoutEngine
    {


        #region public event PDFPageRequestHandler RequestNewPage; + OnRequestNewPage

        public event PDFPageRequestHandler RequestNewPage;

        protected virtual void OnRequestNewPage(PDFPageRequestArgs args)
        {
            throw RecordAndRaise.Operation("Cannot request a new page from the document layout engine");
        }

        #endregion

        private PDFDocument _doc;
        private IPDFLayoutEngine _par;
        private int _pgindex;
        private PDFLayoutContext _context;

        public PDFDocument Document
        {
            get { return _doc; }
        }

        protected PDFStyleStack Styles
        {
            get { return this._context.StyleStack; }
        }

        protected PDFTraceLog TraceLog
        {
            get { return this._context.TraceLog; }
        }

        protected PDFLayoutContext Context
        {
            get { return _context; }
        }

        protected IPDFLayoutEngine Parent
        {
            get { return _par; }
        }

        protected int CurrentPageIndex
        {
            get { return _pgindex; }
            set { _pgindex = value; }
        }

        int IPDFLayoutEngine.LastPageIndex { get { return this.CurrentPageIndex; } }

        internal DocumentLayoutEngine(PDFDocument doc, IPDFLayoutEngine parent, PDFLayoutContext context)
            : base()
        {
            this._doc = doc;
            this._par = parent;
            this._context = context;
        }

        public PDFSize Layout(PDFSize avail, int startPageIndex)
        {
            //we start incrementing from one less than the start index
            //because we add one before each page is laid out.
            //then we end up with current page index always on the last page index

            this.CurrentPageIndex = startPageIndex - 1;

            foreach (PDFPage page in this.Document.Pages)
            {
                this.CurrentPageIndex++;
                this.LayoutPage(page);
            }
            return PDFSize.Empty;
        }
       
        protected PDFSize LayoutPage(PDFPage pg)
        {
            PDFStyle style = pg.GetAppliedStyle();
            
            if(null != style)
                this.Styles.Push(style);
            PDFStyle full = this.Styles.GetFullStyle(pg);

            PDFSize childsize;

            using (IPDFLayoutEngine engine = pg.GetEngine(this, this.Context))
            {
                
                PDFPageRequestHandler handler = new PDFPageRequestHandler(ChildEngineRequestNewPage);
                engine.RequestNewPage += handler;
                childsize = engine.Layout(PDFSize.Empty, this.CurrentPageIndex);
                engine.RequestNewPage -= handler;
                PDFRect bounds = new PDFRect(PDFPoint.Empty,childsize);

                this.CurrentPageIndex = engine.LastPageIndex;

            }
            if (null != style)
                this.Styles.Pop();

            return childsize;
        }


        bool IPDFLayoutEngine.CanSplitCurrentComponent()
        {
            return true;
        }


        private void ChildEngineRequestNewPage(object sender, PDFPageRequestArgs args)
        {
            //Do Nothing
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
        }


    }
}
