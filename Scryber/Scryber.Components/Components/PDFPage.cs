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
using System.Drawing;
using Scryber.Styles;
using Scryber.Resources;
using Scryber.Drawing;

namespace Scryber.Components
{
    [PDFParsableComponent("Page")]
    [PDFRemoteParsableComponent("Page-Ref")]
    public class PDFPage : PDFContainerComponent, IPDFStyledComponent, IPDFResourceContainer, 
                                                  IPDFViewPortComponent, IPDFRemoteComponent,
                                                  IPDFNamingContainer, IPDFTopAndTailedComponent
    {

        #region private int PageIndex { get; protected set; }

        private int _pageindex;

        /// <summary>
        /// Gets the first arranged (zero based) page index of this page in the document
        /// </summary>
        public int PageIndex
        {
            get { return _pageindex; }
            protected set { _pageindex = value; }
        }

        #endregion



        #region public int LastPageIndex {get; protected set;}

        private int _lastpageindex;

        public int LastPageIndex
        {
            get { return _lastpageindex; }
            protected set { _lastpageindex = value; }
        }

        #endregion

        #region public PDFComponentList Contents {get;}

        /// <summary>
        /// Gets the page contents
        /// </summary>
        [PDFElement("Content")]
        [PDFArray(typeof(PDFComponent))]
        public PDFComponentList Contents
        {
            get { return this.InnerContent; }
        }

        #endregion

        #region public PDFStyle Style {get;set;} + bool HasStyle {get;}

        /// <summary>
        /// private instance variable to contain any custom style information about this page
        /// </summary>
        private PDFStyle _style;

        /// <summary>
        /// Gets or Sets the custom style information for this page.
        /// </summary>
        /// <remarks>
        /// Also supports the IPDFStyledComponent Interface
        /// </remarks>
        [PDFElement("Style")]
        public PDFStyle Style
        {
            get 
            {
                if (_style == null)
                    _style = new PDFStyle();

                return _style;
            }
            set { _style = value; }
        }

        /// <summary>
        /// Returns true if this Component has a defined style.
        /// </summary>
        public bool HasStyle
        {
            get { return _style != null && _style.HasItems; }
        }

        #endregion

        #region public PDFResourceList Resources {get;set;} + DoInitResources()

        /// <summary>
        /// private instance variable to hold the list of resources
        /// </summary>
        private PDFResourceList _resources;
        
        /// <summary>
        /// Gets the list of resources this page and its contents use 
        /// </summary>
        /// <remarks>Also implements the IPDFResourceContainer interface</remarks>
        [System.ComponentModel.Browsable(false)]
        public PDFResourceList Resources
        {
            get 
            {
                if (_resources == null)
                    _resources = this.DoInitResources();
                return _resources;
            }
            protected set { _resources = value; }
        }

        /// <summary>
        /// Virtual method that creates a new PDFResourceList for holding a pages resources.
        /// </summary>
        /// <returns>A new PDFResourceList</returns>
        protected virtual PDFResourceList DoInitResources()
        {
            PDFResourceList list = new PDFResourceList(this);
            return list;
        }

        #endregion

        #region public PaperOrientation PageOrientation {get;set;}
        /// <summary>
        /// Gets or sets the orientation of the page
        /// </summary>
        [PDFAttribute("paper-orientation")]
        public PaperOrientation PaperOrientation
        {
            get { return this.Style.PageStyle.PaperOrientation; }
            set { this.Style.PageStyle.PaperOrientation = value; }
        }

        #endregion

        #region public PaperSize PaperSize  {get;set;}

        /// <summary>
        /// Gets or sets the size of the paper
        /// </summary>
        [PDFAttribute("paper-size")]
        public PaperSize PaperSize
        {
            get { return this.Style.PageStyle.PaperSize; }
            set { this.Style.PageStyle.PaperSize = value; }
        }

        #endregion

        #region protected PDFArtefactCollectionSet Artefacts

        private PDFArtefactCollectionSet _artefacts = null;

        /// <summary>
        /// Gets the set of Artefact collections for this Page
        /// </summary>
        protected PDFArtefactCollectionSet Artefacts
        {
            get
            {
                if (null == _artefacts)
                    _artefacts = new PDFArtefactCollectionSet();
                return _artefacts;
            }
        }

        #endregion


        #region  public IPDFTemplate Header {get;set;}

        private IPDFTemplate _header;

        /// <summary>
        /// Gets or sets the header of this Page
        /// </summary>
        [PDFTemplate()]
        [PDFElement("Header")]
        public IPDFTemplate Header
        {
            get { return _header; }
            set { _header = value; }
        }

        #endregion

        #region public IPDFTemplate Footer {get;set;}

        private IPDFTemplate _footer;

        /// <summary>
        /// Gets or sets the template for the footer of this Page
        /// </summary>
        [PDFTemplate()]
        [PDFElement("Footer")]
        public IPDFTemplate Footer
        {
            get { return _footer; }
            set { _footer = value; }
        }

        #endregion

        #region IPDFDocument IPDFResourceContainer.Document
        
        IPDFDocument IPDFResourceContainer.Document
        {
            get { return this.Document; }
        }

        #endregion

        private Dictionary<int, PDFPageHeader> _pageheaders;
        private Dictionary<int, PDFPageFooter> _pagefooters;
     
        #region .ctors

        /// <summary>
        /// Creates a new instance of a PDF Page
        /// </summary>
        public PDFPage()
            : this(PDFObjectTypes.Page)
        {
        }

        /// <summary>
        /// Protected constructor that can be called from subclasses to create a new PDF Page
        /// with a custom type
        /// </summary>
        /// <param name="type">The type name for the Page - usually 'Page'</param>
        protected PDFPage(PDFObjectType type)
            : base(type)
        {
            this._resources = null;
        }


        #endregion

        #region AddGeneratedHeader/Footer + GetGeneratedHeader/Footer

        /// <summary>
        /// Sets the PDFPageHeader Instance for the identified page
        /// </summary>
        /// <param name="header"></param>
        /// <param name="pageindex"></param>
        internal void AddGeneratedHeader(PDFPageHeader header, int pageindex)
        {
            if (null == _pageheaders)
                _pageheaders = new Dictionary<int, PDFPageHeader>();

            _pageheaders[pageindex] = header;
            header.Parent = this;
        }

        /// <summary>
        /// Gets the PDFPageHeader for the identified page and returns true if there is one.
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        internal bool GetHeaderForPage(int pageindex, out PDFPageHeader header)
        {
            if (null == _pageheaders)
            {
                header = null;
                return false;
            }
            else
                return _pageheaders.TryGetValue(pageindex, out header);
        }

        internal void AddGeneratedFooter(PDFPageFooter footer, int pageindex)
        {
            if (null == _pagefooters)
                _pagefooters = new Dictionary<int, PDFPageFooter>();

            _pagefooters[pageindex] = footer;
            footer.Parent = this;
        }

        internal bool GetFooterForPage(int pageindex, out PDFPageFooter footer)
        {
            if (null == _pagefooters)
            {
                footer = null;
                return false;
            }
            else
                return _pagefooters.TryGetValue(pageindex, out footer);
        }

        #endregion


        #region override RenderToPDF() + supporting methods

        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Starting to render PDFSection from starting page index of " + context.PageIndex);

            PDFObjectRef result;
            int count = (this.LastPageIndex - context.PageIndex) + 1;
            if (count == 0)
                result = null;
            else if (count == 1)
                result = this.RenderPageToPDF(context, writer);
            else
            {
                List<PDFObjectRef> pagerefs = new List<PDFObjectRef>();
                PDFObjectRef root = writer.LastObjectReference();
                PDFObjectRef kids = writer.BeginObject();

                for (int i = 0; i < count; i++)
                {
                    PDFObjectRef pg = this.RenderPageToPDF(context, writer);
                    pagerefs.Add(pg);
                }
                WriteSectionPageTree(pagerefs, root, kids, writer);

                writer.EndObject();//kids

                result = kids;
            }
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Completed render PDFSection with final page index '" + context.PageIndex + "'");

            return result;
        }

        private void WriteSectionPageTree(List<PDFObjectRef> pagerefs, PDFObjectRef root, PDFObjectRef kids, PDFWriter writer)
        {
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Pages");
            writer.WriteDictionaryObjectRefEntry("Parent", root);
            writer.BeginDictionaryEntry("Kids");
            writer.WriteArrayRefEntries(pagerefs.ToArray());
            writer.EndDictionaryEntry();
            writer.WriteDictionaryNumberEntry("Count", pagerefs.Count);
            writer.EndDictionary();

        }

        /// <summary>
        /// Renders the page to the writer for the page index of the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private PDFObjectRef RenderPageToPDF(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef parent = writer.LastObjectReference();

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Rendering Page ID: " + this.ID);

            //get the current style and aply it to the style stack
            PDFStyle style = this.GetAppliedStyle();
            context.StyleStack.Push(style);

            PDFPageSize pagesize = this.GetPageSize(context.StyleStack);
            PDFPageNumbering num = this.GetNumbering(context.StyleStack);
            //this.Document.RegisterPageNumbering(context.PageIndex, this, num);

            PDFObjectRef pg;

            pg = DoRenderPage(context, writer, parent, pagesize);

            

            context.StyleStack.Pop();

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Rendering Page ID: " + this.ID);

            return pg;
        }
        

        private PDFObjectRef DoRenderPage(PDFRenderContext context, PDFWriter writer, PDFObjectRef parent, PDFPageSize pageSize)
        {
            PDFObjectRef pg = writer.BeginPage(context.PageIndex);
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Page");
            writer.WriteDictionaryObjectRefEntry("Parent", parent);
            writer.BeginDictionaryEntry("MediaBox");
            writer.WriteArrayRealEntries(0.0, 0.0, pageSize.Width.ToPoints().Value, pageSize.Height.ToPoints().Value);
            writer.EndDictionaryEntry();
            
            
            context.PageSize = pageSize.Size;
            context.Offset = new PDFPoint();
            context.Space = context.PageSize;

            PDFObjectRef content = this.RenderContent(context, writer);
            if (content != null)
                writer.WriteDictionaryObjectRefEntry("Contents", content);

            PDFObjectRef ress = this.DoWriteResource(context, writer);
            if (ress != null)
                writer.WriteDictionaryObjectRefEntry("Resources", ress);

            DoWriteArtefacts(context, writer);
            writer.EndDictionary();
            writer.EndPage(context.PageIndex);

            context.PageIndex++;
            context.PageCount += 1;
            return pg;
        }

        private void DoWriteArtefacts(PDFRenderContext context, PDFWriter writer)
        {
            
            if (this._artefacts != null && this._artefacts.Count > 0)
            {
                foreach (IArtefactCollection col in _artefacts)
                {

                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Begin(TraceLevel.Debug, "Rendering artefact entry " + col.CollectionName);

                    PDFObjectRef artefact = col.RenderToPDF(context, writer);

                    if (null != artefact)
                        writer.WriteDictionaryObjectRefEntry(col.CollectionName, artefact);

                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Begin(TraceLevel.Debug, "Finished artefact entry " + col.CollectionName);

                    
                }
            }
        }

        
        private PDFObjectRef DoWriteResource(PDFRenderContext context, PDFWriter writer)
        {
            return this.Resources.WriteResourceList(context, writer);
        }

        private PDFObjectRef RenderContent(PDFRenderContext context, PDFWriter writer)
        {

            PDFObjectRef oref = writer.BeginObject();

            writer.BeginStream(oref);
            PDFPoint pt = context.Offset.Clone();
            PDFSize sz = context.Space.Clone();

            this.OnPreRender(EventArgs.Empty);

            PDFComponentArrangement arrange = this.GetArrangement(context.PageIndex);
            if (null != arrange)
            {
                base.InternalRender(arrange, context, writer, this.ClipGraphicsToSize(context));
                this.RenderHeaderAndFooter(context, writer);
            }
            else
            {
                if (context.Conformance == Scryber.ConformanceMode.Strict)
                    throw RecordAndRaise.NullReference(Errors.LayoutEngineHasNotSetArrangement);
                else
                    context.TraceLog.Add(TraceLevel.Error, "RENDERING", "No arrangement for known page index '" + context.PageIndex + "' of '" + context.PageCount + "'");
            }

            this.OnPostRender(EventArgs.Empty);

            long len = writer.EndStream();
            writer.BeginDictionary();
            writer.BeginDictionaryEntry("Length");
            writer.WriteNumberS(len);
            writer.EndDictionaryEntry();
            writer.EndDictionary();
            writer.EndObject();
            return oref;
        }

        private void RenderHeaderAndFooter(PDFRenderContext context, PDFWriter writer)
        {
            PDFPageHeader head;
            PDFPageFooter foot;
            if (this.GetHeaderForPage(context.PageIndex, out head))
                head.RenderToPDF(context, writer);

            if (this.GetFooterForPage(context.PageIndex, out foot))
                foot.RenderToPDF(context, writer);
        }

        #endregion

        public override void SetArrangement(PDFComponentArrangement arrange)
        {
            base.SetArrangement(arrange);
        }

        #region private PDFPageSize GetPageSize(PDFStyle appliedstyle)

        /// <summary>
        /// Loads the Applied page size from the style and returns the PDFPageSizeInstance
        /// </summary>
        /// <param name="appliedstyle">The current Applied style</param>
        /// <returns>A new PDFPageSize instance</returns>
        private PDFPageSize GetPageSize(PDFStyleStack styles)
        {
            Styles.PDFStyle style = styles.GetFullStyle(this);
            Styles.PDFPageStyle paper;
            PDFPageSize pagesize;
            if (style.TryGetPaper(out paper) == false)
            {
                paper = new PDFPageStyle();
                paper.PaperSize = PDFStyleConst.DefaultPaperSize;
                paper.PaperOrientation = PDFStyleConst.DefaultPaperOrientation;
            }
            pagesize = paper.CreatePageSize();
            return pagesize;
        }

        #endregion

        #region public virtual int RegisterPageNumbering(int pageindex, PDFPageNumbering num)

        private bool _setfirstpage = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public virtual int RegisterPageNumbering(int pageindex, PDFPageNumbering num)
        {
            int newindex = this.Document.RegisterPageNumbering(this, num);

            this.LastPageIndex = Math.Max(newindex, this.LastPageIndex);

            if (!_setfirstpage)
            {
                this.PageIndex = this.LastPageIndex;
                _setfirstpage = true;
            }

            return newindex;

            //this.PageIndex = this.Document.RegisterPageNumbering(this, num);
            //return this.PageIndex;
        }

        #endregion 

        #region private PDFPageNumbering GetNumbering(PDFStyleStack styles)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        private PDFPageNumbering GetNumbering(PDFStyleStack styles)
        {
            Styles.PDFStyle style = styles.GetFullStyle(this);
            Styles.PDFPageStyle paper;
            PDFPageNumbering num;
            if (style.TryGetPaper(out paper) == false)
            {
                paper = new PDFPageStyle();
                
            }
            num = paper.CreateNumbering();
            return num;
        }

        #endregion

        #region public virtual int GetPageIndex(PDFVisualComponent ele)

        /// <summary>
        /// Gets the zero based index of the page in the document where this Component appears 
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public virtual int GetPageIndex(PDFVisualComponent ele)
        {
            return this.PageIndex;
        }

        #endregion

        #region public override PDFGraphics CreateGraphics(PDFWriter writer, PDFStyleStack styles)

        public override PDFGraphics CreateGraphics(PDFWriter writer, PDFStyleStack styles, PDFContextBase context)
        {
            PDFPageSize pgsize = this.GetPageSize(styles);
            PDFSize sz = pgsize.Size;
            return PDFGraphics.Create(writer,false, this, DrawingOrigin.TopLeft, sz, context);
        }

        #endregion

        #region protected override void DoRegisterArtefacts(PDFRegistrationContext context, PDFStyle full)

        protected override void DoRegisterArtefacts(PDFRegistrationContext context, PDFStyle full)
        {
            //Set context page as this
            PDFPage last = context.Page;
            context.Page = this;

            base.DoRegisterArtefacts(context, full);

            context.Page = last;
        }

        #endregion

        protected override PDFStyle GetBaseStyle()
        {
            Scryber.Styles.PDFStyle flat = base.GetBaseStyle();
            flat.Overflow.Action = OverflowAction.None;

            return flat;
        }

        #region public object RegisterPageEntry(PDFRegistrationContext context, string artefactType, IArtefactEntry entry)

        public object RegisterPageEntry(PDFRegistrationContext context, string artefactType, IArtefactEntry entry)
        {
            IArtefactCollection col;
            if (!Artefacts.TryGetCollection(artefactType, out col))
            {
                col = context.Document.CreateArtefactCollection(artefactType);
                _artefacts.Add(col);
            }
            return col.Register(entry);
        }

        #endregion


        #region public void CloseCatalogEntry(string catalogtype, object entry)

        public void CloseArtefactEntry(string artefacttype, object entry)
        {
            IArtefactCollection col;
            if (this._artefacts.TryGetCollection(artefacttype, out col))
            {
                col.Close(entry);
            }
        }

        #endregion


        #region IResourceContainer Members

        public PDFName RegisterFont(Scryber.Drawing.PDFFont font)
        {            
            PDFFontResource defn = this.Document.GetFontResource(font, true);
            defn.RegisterUse(this.Resources, this);
            return defn.Name;
        }

        public PDFName Register(PDFResource reference)
        {
            if (null == reference.Name || string.IsNullOrEmpty(reference.Name.Value))
            {
                string name = this.Document.GetIncrementID(reference.Type);
                reference.Name = (PDFName)name;
            }
            reference.RegisterUse(this.Resources,this);
            return reference.Name;
        }

        #endregion

        #region IPDFViewPortComponent Members

        public virtual IPDFLayoutEngine GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return new Support.PageLayoutEngine(this, parent, context);
        }

        #endregion

    }



    public class PDFPageList : PDFComponentWrappingList<PDFPage>
    {
        public PDFPageList(PDFComponentList innerList)
            : base(innerList)
        {
        }
    }

    
}
