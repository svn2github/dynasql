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

#define RENDER_LAX

using System;
using System.Collections.Generic;
using System.Text;
using Scryber.Components;
using Scryber.Native;
using Scryber.Styles;
using Scryber.Resources;
using Scryber.Drawing;
using Scryber.Data;
using Scryber.Generation;

namespace Scryber.Components
{
    [PDFParsableComponent("Document")]
    public class PDFDocument : PDFContainerComponent, IPDFDocument, IPDFViewPortComponent, IPDFRemoteComponent, IPDFTemplateParser
    {
        //
        // ctors
        // 

        #region public PDFDocument()

        /// <summary>
        /// Default constructor - creates a new instance of the document
        /// </summary>
        public PDFDocument()
            : this(PDFObjectTypes.Document)
        {
            
        }

        #endregion

        #region protected PDFDocument(PDFObjectType type)

        protected PDFDocument(PDFObjectType type)
            : base(type)
        {
            this._incrementids = new Dictionary<PDFObjectType, int>();
        }

        #endregion

        //
        // properties
        //

        #region public PageUnits PageUnits {get; set;}

        private PageUnits _units;

        /// <summary>
        /// Gets or sets the page units for this document
        /// </summary>
        public PageUnits PageUnits
        {
            get { return _units; }
            set { _units = value; }
        }

        #endregion

        #region public PDFPageList Pages {get;}

        private PDFPageList _pages = null;
        /// <summary>
        /// Gets the list of pages in this document
        /// </summary>
        [PDFElement("Pages")]
        [PDFArray(typeof(PDFPage))]
        public PDFPageList Pages
        {
            get 
            {
                if (this._pages == null)
                    this._pages = new PDFPageList(this.InnerContent);
                return _pages; }
        }

        #endregion

        #region public PDFStyleCollection Styles {get;} + protected virtual PDFStyleCollection CreateStyleCollection()

        private PDFStyleCollection _styles;

        /// <summary>
        /// Gets the collection of styles in this document
        /// </summary>
        [PDFElement("Styles")]
        [PDFArray(typeof(PDFStyleBase))]
        public PDFStyleCollection Styles
        {
            get 
            {
                if (_styles == null)
                    _styles = CreateStyleCollection();
                return _styles;
            }
        }

        /// <summary>
        /// Creates a new empty style collection
        /// </summary>
        /// <returns></returns>
        protected virtual PDFStyleCollection CreateStyleCollection()
        {
            return new PDFStyleCollection();
        }
        
        #endregion

        #region public bool AutoBind {get;set;}

        private bool _autobind = false;

        /// <summary>
        /// Flag to identify if the document should automatically call databaind when processing
        /// </summary>
        [PDFAttribute("auto-bind")]
        public bool AutoBind
        {
            get { return _autobind; }
            set { _autobind = true; }
        }

        #endregion

        #region public PDFDocumentInfo Info {get;set;}

        private PDFDocumentInfo _info = new PDFDocumentInfo();

        /// <summary>
        /// Gets or sets the information for this document
        /// </summary>
        [PDFElement("Info")]
        public PDFDocumentInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        #endregion

        #region public virtual bool IsWebDocument {get;}
        /// <summary>
        /// Gets the flags that identified if this document is being served in a web context, 
        /// or as part of a service or executable. Inheritors can override to return their own checks
        /// </summary>
        public virtual bool IsWebDocument
        {
            get
            {
                return System.Web.HttpContext.Current != null;
            }
        }

        #endregion

        #region public IPDFCacheProvider CacheProvider {get;} + protected virtual IPDFCacheProvider CreateCacheProvider()

        private IPDFCacheProvider _cacheprov;
        /// <summary>
        /// Gets the caching provider for this instance
        /// </summary>
        public IPDFCacheProvider CacheProvider
        {
            get
            {
                if (null == _cacheprov)
                    _cacheprov = this.CreateCacheProvider();
                return _cacheprov;
            }
        }

        protected virtual IPDFCacheProvider CreateCacheProvider()
        {
            if (this.IsWebDocument)
                return Caching.PDFCacheProvider.GetWeb();
            else
                return Caching.PDFCacheProvider.GetStatic();
        }

        #endregion

        #region public int TotalPageCount {get;}

        /// <summary>
        /// Gets the total number of document pages in this document after layout
        /// </summary>
        public int TotalPageCount
        {
            get
            {
                if (null == this._numbers)
                    return -1;
                else
                    return _numbers.TotalPageCount;
            }
        }

        private PageNumberingCollection _numbers = new PageNumberingCollection();
        /// <summary>
        /// Gets the numbering collection
        /// </summary>
        private PageNumberingCollection Numbers
        {
            get { return _numbers; }
        }

        #endregion

        #region public PDFItemCollection Items {get;}

        private PDFItemCollection _items = new PDFItemCollection();

        /// <summary>
        /// Gets a document centered collection of objects that can be accessed by name or index
        /// </summary>
        public PDFItemCollection Items
        {
            get { return _items; }
        }

        #endregion

        #region public string CurrentDirectory {get;}

        private string _currdirectory;

        /// <summary>
        /// Gets the full path to this document's directory. If the document is not loaded from a 
        /// specific file this will be either the root of the current web application, 
        /// or the executable location of the binary. 
        /// </summary>
        public string CurrentDirectory
        {
            get 
            {
                if (string.IsNullOrEmpty(_currdirectory))
                {
                    if (!string.IsNullOrEmpty(this.LoadedSource))
                        _currdirectory = System.IO.Path.GetDirectoryName(this.LoadedSource);
                    else
                        _currdirectory = GetRootDirectory();
                }
                return _currdirectory;
            }
            private set 
            { 
                _currdirectory = value;
            }
        }

        #endregion

        #region protected PDFArtefactCollectionSet Artefacts {get;}

        private PDFArtefactCollectionSet _artefacts = new PDFArtefactCollectionSet();

        /// <summary>
        /// Gets the set of artefact collections
        /// </summary>
        protected PDFArtefactCollectionSet Artefacts
        {
            get
            {
                return _artefacts;
            }
        }

        #endregion


        //
        // Style methods
        //

        #region public override PDFStyle GetAppliedStyle(PDFComponent forComponent)

        /// <summary>
        /// Overrides the default behaviour to get all the appropriate styles for the Components based on the inline and externally 
        /// refferences style definitions, starting with the Default style.
        /// </summary>
        /// <param name="forComponent">The Component to get the styles for</param>
        /// <param name="baseStyle">The base set of styles for the component</param>
        /// <returns>A newly constructed style appropriate for the Component</returns>
        public override PDFStyle GetAppliedStyle(PDFComponent forComponent, PDFStyle baseStyle)
        {
            if (null == baseStyle)
                baseStyle = new PDFStyle();
            this.Styles.MergeInto(baseStyle, forComponent, ComponentState.Normal);
            return baseStyle;
        }

        #endregion

        #region protected virtual PDFStyle CreateDefaultStyle()

        /// <summary>
        /// Creates the standard default style - setting page size, font + size, fill color - and returns the new PDFStyle instance.
        /// Inheritors can override this to adjust the default style for any document
        /// </summary>
        /// <returns></returns>
        protected virtual PDFStyle CreateDefaultStyle()
        {
            PDFStyle style = new PDFStyle();
            style.Add(new PDFBackgroundStyle());
            style.Add(new PDFBorderStyle());
            style.Add(new PDFStrokeStyle());
            PDFFillStyle fill = new PDFFillStyle();
            fill.Color = new PDFColor(ColorSpace.RGB, System.Drawing.Color.Black);
            style.Add(fill);
            
            PDFPageStyle defpaper = new PDFPageStyle();
            defpaper.PaperSize = PaperSize.A4;
            defpaper.PaperOrientation = PaperOrientation.Portrait;
            style.Add(defpaper);

            style.Add(new PDFTextStyle());
            style.Add(new PDFOverflowStyle());
            style.Add(new PDFClipStyle());
            style.Add(new PDFTransformStyle());
            style.Add(new PDFMarginsStyle());
            style.Add(new PDFPaddingStyle());
            style.Add(new PDFPositionStyle());
            PDFFontStyle fs = new PDFFontStyle();
            fs.FontFamily = "Helvetica";
            fs.FontSize = new PDFUnit(24.0, PageUnits.Points);
            style.Add(fs);
            return style;
        }

        #endregion

        //
        // resources
        //


        #region public PDFResourceCollection SharedResources {get;} + protected virtual PDFResourceCollection CreateResourceCollection()

        private PDFResourceCollection _resx;

        /// <summary>
        /// Gets the shared resources in this document - eg Images, Fonts, ProcSets
        /// </summary>
        public PDFResourceCollection SharedResources
        {
            get
            {
                if (_resx == null)
                    _resx = CreateResourceCollection();
                return _resx;
            }
        }

        protected virtual PDFResourceCollection CreateResourceCollection()
        {
            PDFResourceCollection resxcol = new PDFResourceCollection(this);
            //PDFFonts.InitStdFonts(resxcol);
            return resxcol;
        }

        #endregion

        #region GetImageResource(string fullpath, bool create)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public PDFImageXObject GetImageResource(string fullpath, bool create)
        {
            fullpath = fullpath.ToLower();
            PDFImageXObject img = this.GetResource(PDFResource.XObjectResourceType, fullpath, create) as PDFImageXObject;

            return img;
        }

        
        #endregion

        #region public PDFFontResource GetFontResource(PDFFont font)

        /// <summary>
        /// Retrieves a PDFFontResource for the specified font, based upon it's full name
        /// </summary>
        /// <param name="font">The font to get the resource for</param>
        /// <param name="create">true if the PDFFontResource should be created if it is not already listed</param>
        /// <returns>A PDFFontResource that will be included in the document (or null if it is not loaded and should not be created)</returns>
        public virtual PDFFontResource GetFontResource(PDFFont font, bool create)
        {
            string fullname = font.FullName;
            string type = PDFResource.FontDefnResourceType;

            PDFFontResource rsrc = this.GetResource(type, fullname, true) as PDFFontResource;
            return rsrc;
        }

        #endregion

        #region public virtual PDFResource GetResource(string resourceType, string resourceKey, bool create)

        /// <summary>
        /// Implements the IPDFDocument.GetResource interface and supports the extraction and creation of document specific resources.
        /// </summary>
        /// <param name="resourceType">The Type of resource to get - XObject, Font</param>
        /// <param name="resourceKey">The resource specific key name (Fonts is full name, images is the full path or resource key)</param>
        /// <param name="create">Specify true to attempt the loading of the resource if it is not currently in the collection</param>
        /// <returns>The loaded resource if any</returns>
        public virtual PDFResource GetResource(string resourceType, string resourceKey, bool create)
        {
            PDFResource found = this.SharedResources.GetResource(resourceType, resourceKey);
            if (null == found && create)
            {
                found = this.CreateAndAddResource(resourceType, resourceKey);
            }
            return found;
        }

        #endregion

        #region protected virtual PDFResource CreateAndAddResource(string resourceType, string resourceKey)

        /// <summary>
        /// Creates a new PDFResource of the required type based on the specified key and adds it to the SharedResource collection
        /// </summary>
        /// <param name="resourceType">The type of resource to create (image XObject or Font)</param>
        /// <param name="resourceKey"></param>
        /// <returns>A new PDFResoure sub class instance</returns>
        protected virtual PDFResource CreateAndAddResource(string resourceType, string resourceKey)
        {
            PDFResource created;
            if (resourceType == PDFResource.FontDefnResourceType)
            {
                created = CreateFontResource(resourceKey);

            }
            else if (resourceType == PDFResource.XObjectResourceType)
            {
                created = CreateImageResource(resourceKey);
            }
            else
                throw new ArgumentOutOfRangeException("resourceType");

            this.SharedResources.Add(created);
            return created;
        }

        #endregion

        #region private PDFImageXObject CreateImageResource(string fullpath)

        private PDFImageXObject CreateImageResource(string fullpath)
        {
            PDFImageData data = this.LoadImageData(fullpath);
            string id = this.GetIncrementID(PDFObjectTypes.ImageXObject);
            PDFImageXObject img = PDFImageXObject.Load(data, id);
            return img;
        }

        #endregion

        #region private PDFFontResource CreateFontResource(string fullname)

        private PDFFontResource CreateFontResource(string fullname)
        {
            PDFFontDefinition defn = PDFFontFactory.GetFontDefinition(fullname);
            string id = this.GetIncrementID(PDFObjectTypes.FontResource);
            PDFFontResource rsrc = PDFFontResource.Load(defn, id);
            return rsrc;
        }

        #endregion

        //
        // page numbering
        //

        #region public void RegisterPageNumbering(int pageindex, PDFPage page, PDFPageNumbering num)

        /// <summary>
        /// Registers the current page numbering options for the specified page at the specified index
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="page"></param>
        /// <param name="num"></param>
        public int RegisterPageNumbering(PDFPage page, PDFPageNumbering num)
        {
            int index = this.Numbers.TotalPageCount;
            this.Numbers.Register(index, page, num);
            return index;
        }

        #endregion

        #region public PDFPageNumbering GetNumbering(int pageindex)

        /// <summary>
        /// Gets the page numbering options for the page at the spacified index
        /// </summary>
        /// <param name="pageindex">The zero based page index in </param>
        /// <returns></returns>
        public PDFPageNumbering GetNumbering(int pageindex, out int numberingindex, out int numberingmax)
        {
            return this.Numbers.GetNumbering(pageindex, out numberingindex, out numberingmax);
        }


        #endregion

        #region GetIncrementID

        private Dictionary<PDFObjectType, int> _incrementids = null;

        /// <summary>
        /// Gets the next unique id for an Component of a specific type
        /// </summary>
        /// <param name="type">The type of Component to create the new ID for</param>
        /// <returns>A unique id</returns>
        public override string GetIncrementID(PDFObjectType type)
        {
            if (this._incrementids == null)
                this._incrementids = new Dictionary<PDFObjectType, int>();
            int lastindex;
            
            if (this._incrementids.TryGetValue(type, out lastindex) == false)
                lastindex = 0;

            this._incrementids[type] = ++lastindex;

            return type.ToString() + lastindex.ToString();

        }

        #endregion

        //
        // document processing
        //

        #region public void ProcessDocument(System.IO.Stream stream, bool bind) + 1 overload

        /// <summary>
        /// Performs a complete initialization, load and then renders the document to the output stream.
        /// Uses the Autobind property to stipulate if data binding should also take place
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bind"></param>
        public void ProcessDocument(System.IO.Stream stream)
        {
            this.ProcessDocument(stream, this.AutoBind);
        }

        /// <summary>
        /// Performs a complete initialization, load and then render the document to the path.
        /// Uses the Autobind property to stipulate if data binding should also take place
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        public void ProcessDocument(string path, System.IO.FileMode mode)
        {
            this.ProcessDocument(path, mode, this.AutoBind);
            
        }
        /// <summary>
        /// Performs a complete initialization, load and then renders the document to the output stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bind"></param>
        public void ProcessDocument(System.IO.Stream stream, bool bind)
        {
            if (null == stream)
                throw new ArgumentNullException("stream");

            this.InitializeAndLoad();

            if (bind)
                this.DataBind();

            this.RenderToPDF(stream);
        }

        /// <summary>
        /// Performs a complete initialization, load and then render the document to the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        public void ProcessDocument(string path, System.IO.FileMode mode, bool bind)
        {
            using (System.IO.Stream stream = this.DoOpenFileStream(path, mode))
            {
                this.ProcessDocument(stream, bind);
            }
        }

        #endregion

        #region public void InitializeAndLoad()

        /// <summary>
        /// Performs any initializing and loading, but does not render the document
        /// </summary>
        public void InitializeAndLoad()
        {
            PDFTraceLog log = PDFTraceContext.GetLog();
            PDFInitContext icontext = new PDFInitContext(this.Items, log);

            log.Begin(TraceLevel.Message, "Document Initialize");
            this.Init(icontext);
            log.End(TraceLevel.Message, "Document Initialize");

            PDFLoadContext loadcontext = new PDFLoadContext(this.Items, log);

            log.Begin(TraceLevel.Message, "Document Load");
            this.Load(loadcontext);
            log.End(TraceLevel.Message, "Document Load");
        }

        #endregion

        #region public void DataBind()

        /// <summary>
        /// Data binds the entire document
        /// </summary>
        public void DataBind()
        {
            PDFDataContext context = this.CreateDataContext();

            context.TraceLog.Begin(TraceLevel.Message, "Document Databind");
            this.DataBind(context);
            context.TraceLog.End(TraceLevel.Message, "Document Databind");
        }

        /// <summary>
        /// Creates a new data context that is passed to the main data binding method
        /// </summary>
        /// <returns></returns>
        protected virtual PDFDataContext CreateDataContext()
        {
            PDFTraceLog log = PDFTraceContext.GetLog();
            return new PDFDataContext(this.Items, log);
        }

        #endregion

        #region protected override void DoDataBind(PDFDataContext context, bool includeChildren)

        /// <summary>
        /// Overrides the default implementation, calling the base method within it
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeChildren"></param>
        protected override void DoDataBind(PDFDataContext context, bool includeChildren)
        {
            context.TraceLog.Add(TraceLevel.Message, "Document", "Beginning Databind"); 
            base.DoDataBind(context, includeChildren);
            context.TraceLog.Add(TraceLevel.Message, "Document", "Completed Databind");
        }

        #endregion

        //
        // rendering
        //

        #region RenderToPDF(string path, System.IO.FileMode mode) 2 Overloads

        /// <summary>
        /// Renders the complete document to a file at the specified path, using the specified file mode. It is up to callers to
        /// to use temporary files and replacement
        /// </summary>
        /// <param name="path">The complete path at which to create the file.</param>
        /// <param name="mode">The FileMode option for CreateNew, Append etc.</param>
        public void RenderToPDF(string path, System.IO.FileMode mode)
        {
            using (System.IO.Stream stream = this.DoOpenFileStream(path, mode))
            {
                this.RenderToPDF(stream);
            }
        }

        /// <summary>
        /// Renders the complete document to an IO Stream
        /// </summary>
        /// <param name="tostream">The stream to write the document to</param>
        public void RenderToPDF(System.IO.Stream tostream)
        {
            PDFRenderContext context = this.DoCreateRenderContext();

            using (PDFWriter writer = this.DoCreateRenderWriter(tostream, context))
            {
                this.RenderToPDF(context, writer);
            }
        }

        /// <summary>
        /// Preforms the actual rendering of the document to the writer with the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        /// <remarks>This method performs the 3 required actions to render a PDF document.
        /// 
        /// 1. Artefact registration - allows components to register actions, annotations, image sources and other resources
        /// 
        /// 2. Layout - measures and explicitly lays out each visual component 
        /// 
        /// 3. Rendering - performs the actual output onto the writer</remarks>
        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            //Clear the artefacts
            this.ResetRenderArtefacts();

            PDFStyle style = this.CreateDefaultStyle();

            //Register the artefacts
            context.TraceLog.Begin(TraceLevel.Message, "Artefact registration");
            PDFRegistrationContext regcontext = CreateRegistrationContext(style,context.Items, context.TraceLog);
            this.RegisterArtefacts(regcontext);
            context.TraceLog.End(TraceLevel.Message, "Artefact registration");

            PDFLayoutContext layoutcontext = CreateLayoutContext(style, context.Items, context.TraceLog);

            //Layout components before rendering
            context.TraceLog.Begin(TraceLevel.Message, "Document layout");
            using (IPDFLayoutEngine engine = this.GetEngine(null, layoutcontext))
            {
                int startpageindex = context.PageIndex;
                engine.Layout(PDFSize.Empty, startpageindex);
            }
            context.TraceLog.End(TraceLevel.Message, "Document layout");


            context.TraceLog.Begin("Beginning render of the document");
            //Reset the page count and page index
            context.PageCount = 0;// this.Numbers.TotalPageCount;
            context.PageIndex = 0;

            PDFObjectRef root = this.DoRenderToPDF(context, style, null, writer);

            context.TraceLog.End(TraceLevel.Message, "Ended render of the document");
            
            return root;
        }

        #endregion

        #region protected virtual PDFRegistrationContext CreateRegistrationContext(PDFStyle style, PDFItemCollection items)

        /// <summary>
        /// Creates and returns a new registration context.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected virtual PDFRegistrationContext CreateRegistrationContext(PDFStyle style, PDFItemCollection items, PDFTraceLog log)
        {
            return new PDFRegistrationContext(this, style, items, log);
        }

        #endregion

        protected virtual PDFLayoutContext CreateLayoutContext(PDFStyle style, PDFItemCollection items, PDFTraceLog log)
        {
            return new PDFLayoutContext(style, items, log);
        }

        #region public IPDFLayoutEngine GetEngine(...)

        IPDFLayoutEngine IPDFViewPortComponent.GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return this.GetEngine(parent, context);
        }

        public virtual IPDFLayoutEngine GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return new Scryber.Components.Support.DocumentLayoutEngine(this, parent, context);
        }

        #endregion

        #region protected virtual System.IO.Stream DoOpenFileStream(string path, System.IO.FileMode mode)

        /// <summary>
        /// Opens a new stream onto a file at the specified path. Inheritors can override this method to provide custom implementation
        /// </summary>
        /// <param name="path">The complete path</param>
        /// <param name="mode">The open file mode option</param>
        /// <returns>A new FileStream onto the path</returns>
        protected virtual System.IO.Stream DoOpenFileStream(string path, System.IO.FileMode mode)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            System.IO.FileStream fs = new System.IO.FileStream(path, mode);

            return fs;
        }

        #endregion

        #region protected virtual PDFRenderContext DoCreateRenderContext()

        /// <summary>
        /// Creates the render context for this document. Inheritors can override this method to provide custom implementation
        /// </summary>
        /// <returns>A newly constructed render context for the operation</returns>
        protected virtual PDFRenderContext DoCreateRenderContext()
        {
            PDFTraceLog log = PDFTraceContext.GetLog();
            PDFStyle def = this.CreateDefaultStyle();
            PDFRenderContext context = new PDFRenderContext(DrawingOrigin.TopLeft, 0, def, this.Items, log);
#if RENDER_LAX
            context.Conformance = ConformanceMode.Lax;
#else
            context.Conformance = ConformanceMode.Strict;
#endif
            return context;
        }

        #endregion

        #region protected virtual PDFWriter DoCreateRenderWriter(System.IO.Stream tostream, PDFRenderContext context)

        /// <summary>
        /// Creates a new PDFWriter instance for this document. Inheritors can override this method to provide custom implementation
        /// </summary>
        /// <param name="tostream">The output stream the writer should use</param>
        /// <returns>A new PDFWriter</returns>
        protected virtual PDFWriter DoCreateRenderWriter(System.IO.Stream tostream, PDFRenderContext context)
        {
            //TODO: Add configuration for document writer version support.
            PDFWriter14 writer = new PDFWriter14(tostream, 0, context.TraceLog);

            return writer;
        }

        #endregion
        
        #region protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// Inner method to render the document to the specified writer using the render context. All RenderToPDF methods eventually call this method
        /// </summary>
        /// <param name="context">The current render context</param>
        /// <param name="writer">The current PDF Writer</param>
        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            writer.Open();
            PDFObjectRef catalog = this.RenderCatalog(context, writer);
            PDFObjectRef info = this.RenderInfo(context, writer);
            writer.Close(null);

            return catalog;
        }

        #endregion

        #region private PDFObjectRef RenderCatalog(PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// Renders the document catalog (usually the first Component) and then calls render on each of the documents 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private PDFObjectRef RenderCatalog(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef catalog = writer.BeginObject("Catalog");
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Catalog");

            // Pages
            context.TraceLog.Begin(TraceLevel.Debug, "Rendering Pages");
            PDFObjectRef pglist = this.RenderPages(context, writer);
            writer.WriteDictionaryObjectRefEntry("Pages", pglist);
            context.TraceLog.End(TraceLevel.Debug, "Rendering Pages");

            // Page Labels
            context.TraceLog.Begin(TraceLevel.Debug, "Rendering Page Labels");
            PDFObjectRef pglabels = this.RenderPageLabels(context, writer);
            if(null != pglabels)
                writer.WriteDictionaryObjectRefEntry("PageLabels", pglabels);
            context.TraceLog.End(TraceLevel.Debug, "Rendering Page Labels");

            RenderArtefacts(context, writer);

            writer.EndDictionary();
            writer.EndObject();

            return catalog;
        }

        #endregion

        #region protected virtual void ResetRenderArtefacts()

        /// <summary>
        /// Resets all previously registered artefacts
        /// </summary>
        protected virtual void ResetRenderArtefacts()
        {
            this._artefacts.Clear();
        }

        #endregion

        #region private void RenderArtefacts(PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// If this document has artefacts then the collection is rendered and 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        private void RenderArtefacts(PDFRenderContext context, PDFWriter writer)
        {
            if (this.Artefacts != null && this.Artefacts.Count > 0)
            {
                foreach (IArtefactCollection col in this._artefacts)
                {
                    context.TraceLog.Begin(TraceLevel.Debug, "Rendering artefact catalog entry collection " + col.CollectionName);
                    writer.BeginDictionaryEntry(col.CollectionName);

                    PDFObjectRef entry = col.RenderToPDF(context, writer);
                    if (entry != null)
                        writer.WriteObjectRef(entry);
                    else
                        writer.WriteNull();
                    writer.EndDictionaryEntry();

                    context.TraceLog.End(TraceLevel.Debug, "Finished artefact catalog entry collection " + col.CollectionName);
                }
            }
        }

        #endregion

        #region private PDFObjectRef RenderInfo(PDFRenderContext context, PDFWriter writer) + support methods

        /// <summary>
        /// renders the collection of document info entries
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private PDFObjectRef RenderInfo(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef inforef = writer.BeginObject("Info");

            writer.BeginDictionary();
            RenderInfoEntry("Title",this.Info.Title, writer);
            RenderInfoEntry("Subject", this.Info.Subject, writer);
            RenderInfoEntry("Author", this.Info.Author, writer);
            RenderInfoEntry("Keywords", this.Info.Keywords, writer);
            RenderInfoEntry("Producer", this.Info.Producer, writer);
            RenderInfoEntry("Creator", this.Info.Creator, writer);
            RenderInfoEntry("CreationDate", this.Info.CreationDate, writer);
            RenderInfoEntry("ModDate", this.Info.ModifiedDate, writer);

            if (this.Info.HasTrapping)
                writer.WriteDictionaryNameEntry("Trapped", this.Info.Trapped.ToString());

            if (this.Info.HasExtras)
            {
                foreach (PDFDocumentInfoExtra extra in this.Info.Extras)
                {
                    RenderInfoEntry(extra.Name, extra.Value, writer);
                }
            }
            writer.EndDictionary();
            writer.EndObject();

            return inforef;
        }


        private static void RenderInfoEntry(string name, DateTime value, PDFWriter writer)
        {
            if (value > DateTime.MinValue)
            {
                writer.BeginDictionaryEntry(name);
                writer.WriteDate(value);
                writer.EndDictionaryEntry();
            }
        }


        private static void RenderInfoEntry(string name, string value, PDFWriter writer)
        {
            if (!string.IsNullOrEmpty(value))
                writer.WriteDictionaryStringEntry(name, value.Trim());
        }

        #endregion

        #region private PDFObjectRef RenderPageLabels(PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// Renders the collection of page labels and returns a reference to this collection
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private PDFObjectRef RenderPageLabels(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef labels = writer.BeginObject("PageLabels");
            writer.BeginDictionary();
            writer.BeginDictionaryEntry("Nums");
            writer.BeginArrayS();
            foreach (PageNumberingEntry entry in this.Numbers)
            {
                writer.BeginArrayEntry();
                writer.WriteNumberS(entry.PageIndex);
                writer.BeginDictionaryS();
                string type;
                switch (entry.NumberingFormat.NumberStyle)
                {
                    case PageNumberStyle.Decimals:
                        type = "D";
                        break;
                    case PageNumberStyle.UppercaseRoman:
                        type = "R";
                        break;
                    case PageNumberStyle.LowercaseRoman:
                        type = "r";
                        break;
                    case PageNumberStyle.UppercaseLetters:
                        type = "A";
                        break;
                    case PageNumberStyle.LowercaseLetters:
                        type = "a";
                        break;
                    default:
                        type = "";
                        break;
                }
                if(!string.IsNullOrEmpty(type))
                    writer.WriteDictionaryNameEntry("S", type);
                if (!string.IsNullOrEmpty(entry.NumberingFormat.Prefix))
                    writer.WriteDictionaryStringEntry("P", entry.NumberingFormat.Prefix);
                if (entry.NumberingFormat.StartIndex > 1)
                    writer.WriteDictionaryNumberEntry("St", entry.NumberingFormat.StartIndex);
                writer.EndDictionary();
                writer.EndArrayEntry();
            }
            writer.EndArray();
            writer.EndDictionaryEntry();
            writer.EndDictionary();
            writer.EndObject();
            return labels;

        }

        #endregion

        #region protected PDFObjectRef RenderPages(PDFRenderContext context, PDFWriter writer)

        /// <summary>
        /// Renders the first Page tree Component and calls Render on each of the pages.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="writer">The current writer</param>
        /// <returns>A reference to the current page tree root Component</returns>
        protected PDFObjectRef RenderPages(PDFRenderContext context, PDFWriter writer)
        {
            //Begin the Pages object and dictionary
            PDFObjectRef pgs = writer.BeginObject(Const.PageTreeName);
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Pages");
            
            List<PDFObjectRef> pagerefs = new List<PDFObjectRef>(this.InnerContent.Count);
            
            //allow each page to render itself, and add the returned PDFObjectRef to the list for the kids dictionary entry below. 
            foreach (PDFComponent ppe in this.InnerContent)
            {
                PDFObjectRef oref = ppe.RenderToPDF(context, writer);
                if(oref != null)
                    pagerefs.Add(oref);
                
            }

            //write the kids array entry in the dictionary
            writer.BeginDictionaryEntry("Kids");
            writer.BeginArray();
            foreach (PDFObjectRef kid in pagerefs)
            {
                writer.BeginArrayEntry();
                writer.WriteFileObject(kid);
                writer.EndArrayEntry();
            }
            writer.EndArray();
            //Write the total number of pages to the dictionary
            writer.EndDictionaryEntry();
            writer.BeginDictionaryEntry("Count");
            writer.WriteNumber(context.PageCount);
            writer.EndDictionaryEntry();

            //close the ditionary and the object
            writer.EndDictionary();

            writer.EndObject();

            return pgs;
        }

        #endregion


        //
        // Artefacts
        //

        

        #region public object RegisterCatalogEntry(PDFRegistrationContext context, string catalogtype, ICatalogEntry entry)

        /// <summary>
        /// Registers the IArtefactEntry values with the artefact collection of type 'catalogtype'.
        /// The method returns a reference object that must be passed back to CloseArtefactEntry when the entry should be closed.
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="catalogtype"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object RegisterCatalogEntry(PDFRegistrationContext context, string catalogtype, IArtefactEntry entry)
        {
            IArtefactCollection col;
            if (!this._artefacts.TryGetCollection(catalogtype, out col))
            {
                col = this.CreateArtefactCollection(catalogtype);
                _artefacts.Add(col);
            }
            return col.Register(entry);
        }

        #endregion

        #region protected virtual ICatalogCollection CreateArtefactCollection(string type)

        /// <summary>
        /// Overridable method that creates the specific collections for individual artefact types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal protected virtual IArtefactCollection CreateArtefactCollection(string type)
        {
            switch (type)
            {
                case (PDFArtefactTypes.Annotations):
                    return new PDFAnnotationCollection(type);
                case (PDFArtefactTypes.Names):
                    return new PDFNameDictionary(type);
                case (PDFArtefactTypes.Outlines):
                    return new PDFOutlineStack(type);

                default:
                    throw RecordAndRaise.NotSupported("The catalog type {0} is not a known catalog type", type);

            }
        }

        #endregion

        #region public void CloseArtefactEntry(string catalogtype, object entry)

        /// <summary>
        /// Closes the last artefact entry that was started with the 'RegisterCatalogEntry'
        /// </summary>
        /// <param name="catalogtype"></param>
        /// <param name="entry"></param>
        public void CloseArtefactEntry(string catalogtype, object entry)
        {
            IArtefactCollection col;
            if (this._artefacts.TryGetCollection(catalogtype, out col))
            {
                col.Close(entry);
            }
        }

        #endregion


        //
        // Path manipulation and loading
        //

        #region protected internal PDFImageData LoadImageData(string source)

        /// <summary>
        /// Loads a file name or url from a source and returns the correct image data encapsulating the image stream
        /// </summary>
        /// <param name="source">The full path or absolute location of the image data file</param>
        /// <returns></returns>
        protected internal PDFImageData LoadImageData(string path)
        {
            bool isfile;
            
            if (System.Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                isfile = false;
            }
            else if (System.IO.Path.IsPathRooted(path))
            {
                isfile = true;
            }
            else
                throw RecordAndRaise.Argument(Errors.CannotLoadFileWithRelativePath);

            PDFImageData data;
            object cached;
            string key = path;
            if (!this.CacheProvider.TryRetrieveFromCache(PDFObjectTypes.ImageData.ToString(), key, out cached))
            {
                if (isfile)
                    data = PDFImageData.LoadImageFromLocalFile(path);
                else
                    data = PDFImageData.LoadImageFromURI(path);

                this.CacheProvider.AddToCache(PDFObjectTypes.ImageData.ToString(), key, data);
            }
            else
                data = (PDFImageData)cached;
            
            return data;
        }

        #endregion


        #region protected override string MapPath(string path)

        /// <summary>
        /// Overrides the default behavior to map any file reference to either an absolute uri or filepath.
        /// </summary>
        /// <param name="path">The path to map</param>
        /// <returns></returns>
        public override string MapPath(string path)
        {
            bool isfile;
            return this.MapPath(path, out isfile);
        }

        

        //
        // mappath support methods
        //
       

        

        #endregion

        #region  public PDFComponent FindAComponent(string id) + 1 overload

        public PDFComponent FindAComponentById(string id)
        {
            PDFComponent ele = null;
            if (this.ID.Equals(id))
                return this;
            else if (this.FindAComponentById(this.Pages.InnerList, id, out ele))
                return ele;
            else
                return null;
        }

        private bool FindAComponentById(PDFComponentList list, string id, out PDFComponent found)
        {
            if (list != null)
            {
                foreach (PDFComponent ele in list)
                {
                    if (ele.ID.Equals(id))
                    {
                        found = ele;
                        return true;
                    }
                    else if (ele is IPDFContainerComponent)
                    {
                        IPDFContainerComponent container = ele as IPDFContainerComponent;
                        if (container.HasContent && this.FindAComponentById(container.Content, id, out found))
                            return true;
                    }
                }
            }
            found = null;
            return false;
        }

        #endregion

        #region  public PDFComponent FindAComponentByName(string name) + 1 overload

        public PDFComponent FindAComponentByName(string name)
        {
            PDFComponent ele = null;
            if (string.Equals(this.Name, name))
                return this;
            else if (this.FindAComponentByName(this.Pages.InnerList, name, out ele))
                return ele;
            else
                return null;
        }

        private bool FindAComponentByName(PDFComponentList list, string name, out PDFComponent found)
        {
            if (list != null)
            {
                foreach (PDFComponent ele in list)
                {
                    if (string.Equals(name, ele.Name))
                    {
                        found = ele;
                        return true;
                    }
                    else if (ele is IPDFContainerComponent)
                    {
                        IPDFContainerComponent container = ele as IPDFContainerComponent;
                        if (container.HasContent && this.FindAComponentByName(container.Content, name, out found))
                            return true;
                    }
                }
            }
            found = null;
            return false;
        }

        #endregion

        //
        // parse methods
        //

        #region protected override IPDFComponent ParseComponentAtPath(string path)

        /// <summary>
        /// Overrides  the default behaviour to parse the component using the static Document.Parse method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override IPDFComponent ParseComponentAtPath(string path)
        {
            return PDFDocument.Parse(path);
        }

        #endregion

        #region public static IPDFComponent Parse(string fullpath) + 2 overloads

        public static IPDFComponent Parse(string fullpath)
        {
            using (System.IO.Stream stream = new System.IO.FileStream(fullpath,System.IO.FileMode.Open,System.IO.FileAccess.Read))
            {
                PDFReferenceChecker checker = new PDFReferenceChecker(fullpath);

                IPDFComponent comp = Parse(fullpath, stream, checker.Resolver);

                if (comp is IPDFRemoteComponent)
                    ((IPDFRemoteComponent)comp).LoadedSource = fullpath;

                return comp;
            }
        }



        public static IPDFComponent Parse(string fullpath, PDFReferenceResolver resolver)
        {
            using (System.IO.Stream stream = new System.IO.FileStream(fullpath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                IPDFComponent comp = Parse(fullpath, stream, resolver);
                if (comp is IPDFRemoteComponent)
                    ((IPDFRemoteComponent)comp).LoadedSource = fullpath;
                return comp;
            }
        }

        public static IPDFComponent Parse(string source, System.IO.Stream stream, PDFReferenceResolver resolver)
        {
            PDFGeneratorSettings settings = CreateGeneratorSettings(resolver, ConformanceMode.Strict, ComponentLoadType.ReflectiveParser, PDFTraceContext.GetLog());

            Generation.PDFXMLParser xmlparser = new Scryber.Generation.PDFXMLParser(settings);
            IPDFComponent comp = xmlparser.Parse(source, stream);
            return comp;
        }

        public static IPDFComponent Parse(string source, System.IO.TextReader textreader, PDFReferenceResolver resolver)
        {
            PDFGeneratorSettings settings = CreateGeneratorSettings(resolver, ConformanceMode.Strict, ComponentLoadType.ReflectiveParser, PDFTraceContext.GetLog());
            Generation.PDFXMLParser xmlparser = new Scryber.Generation.PDFXMLParser(settings);
            
            IPDFComponent comp = xmlparser.Parse(source, textreader);
            return comp;
        }

        #endregion

        #region public IPDFComponent ParseTemplate(IPDFRemoteComponent owner, string referencepath, Stream stream) + 2 overloads

        public IPDFComponent ParseTemplate(IPDFRemoteComponent owner, System.IO.TextReader reader)
        {
            return ParseTemplate(owner, owner.LoadedSource, reader);
        }

        public IPDFComponent ParseTemplate(IPDFRemoteComponent owner, string referencepath, System.IO.Stream stream)
        {
            using (System.Xml.XmlReader xml = System.Xml.XmlReader.Create(stream))
            {
                return ParseTemplate(owner, referencepath, xml);
            }
        }



        public IPDFComponent ParseTemplate(IPDFComponent owner, string referencepath, System.IO.TextReader reader)
        {
            using (System.Xml.XmlReader xml = System.Xml.XmlReader.Create(reader))
            {
                return ParseTemplate(owner, referencepath, xml);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="referencepath"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual IPDFComponent ParseTemplate(IPDFComponent owner, string referencepath, System.Xml.XmlReader reader)
        {
            if (null == owner)
                throw RecordAndRaise.ArgumentNull("owner");
            if (string.IsNullOrEmpty(referencepath))
                throw RecordAndRaise.ArgumentNull("reader");

            IPDFComponent comp;
            
            PDFReferenceChecker checker = new PDFReferenceChecker(referencepath);
            PDFGeneratorSettings settings = CreateGeneratorSettings(checker.Resolver, ConformanceMode.Strict, ComponentLoadType.ReflectiveParser, PDFTraceContext.GetLog());

            Generation.PDFXMLParser parser = new Scryber.Generation.PDFXMLParser(settings);
            
            parser.RootComponent = owner;

            comp = parser.Parse(referencepath, reader);
            
            
            return comp;
        }

        #endregion

        #region protected virtual PDFGeneratorSettings CreateGeneratorSettings(PDFReferenceResolver resolver)
        /// <summary>
        /// Creates the generator settings required to parse the XML files
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected virtual PDFGeneratorSettings CreateGeneratorSettings(PDFReferenceResolver resolver)
        {
            return CreateGeneratorSettings(resolver, ConformanceMode.Lax, ComponentLoadType.ReflectiveParser, PDFTraceContext.GetLog());
        }

        protected static PDFGeneratorSettings CreateGeneratorSettings(PDFReferenceResolver resolver, ConformanceMode conformance, ComponentLoadType loadtype, PDFTraceLog log)
        {
            PDFGeneratorSettings settings = new PDFGeneratorSettings(typeof(PDFTextLiteral)
                                                                    , typeof(PDFParsableTemplateGenerator)
                                                                    , typeof(PDFTemplateInstance),
                                                                    resolver,
                                                                    conformance,
                                                                    loadtype,
                                                                    log);
            return settings;
        }

        #endregion

        /// <summary>
        /// Loads a PDFDocument from the specified full or releative path.
        /// </summary>
        /// <remarks>If the method is called with a non rooted path then the full path is built based upon either the
        /// current web request, or the current working directory</remarks>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PDFDocument LoadDocument(string path)
        {
            if (!System.IO.Path.IsPathRooted(path))
            {
                if (System.Web.HttpContext.Current != null)
                {
                    path = System.Web.HttpContext.Current.Server.MapPath(path);
                }
                else
                {
                    string root = System.Environment.CurrentDirectory;
                    path = System.IO.Path.Combine(root, path);
                    path = System.IO.Path.GetFullPath(path);
                }
            }

            IPDFComponent parsed = Parse(path);
            if (!(parsed is PDFDocument))
                throw new InvalidCastException(String.Format(Errors.CannotConvertObjectToType, parsed.GetType(), typeof(PDFDocument)));

            return parsed as PDFDocument;

        }
        //
        // inner classes
        //


        #region private class PageNumberingEntry + private class PageNumberingCollection

        /// <summary>
        /// Defines a single page number format that starts at the specified index
        /// </summary>
        private class PageNumberingEntry
        {
            public int PageIndex { get; set; }

            public PDFPage Page { get; set; }

            public PDFPageNumbering NumberingFormat { get; set; }

            public string GetPageString(int forpageindex)
            {
                forpageindex = forpageindex - this.PageIndex;
                return NumberingFormat.GetPageNumber(forpageindex);
            }
        }

        /// <summary>
        /// Retains a sorted list of the Page Number format entries
        /// </summary>
        private class PageNumberingCollection : IEnumerable<PageNumberingEntry>
        {
            private int _total;

            public int TotalPageCount { get { return _total; } }

            private List<PageNumberingEntry> _entries = new List<PageNumberingEntry>();

            public void Register(int pageindex, PDFPage page, PDFPageNumbering num)
            {
                this._total++;

                if (_entries.Count == 0)
                {
                    _entries.Add(new PageNumberingEntry() { PageIndex = pageindex, NumberingFormat = num, Page = page });
                }
                else
                {
                    for (int i = _entries.Count - 1; i >= 0; i--)
                    {
                        PageNumberingEntry entry = _entries[i];
                        if (entry.PageIndex == pageindex)
                        {
                            if (entry.Page != page)
                                throw new ArgumentException("Duplicate page indexes for different pages. Cannot be registered");
                            entry.NumberingFormat = num;
                        }
                        if (entry.PageIndex < pageindex)
                        {
                            if (entry.NumberingFormat.Equals(num) == false)
                            {
                                entry = new PageNumberingEntry();
                                entry.NumberingFormat = num;
                                entry.Page = page;
                                entry.PageIndex = pageindex;
                                _entries.Add(entry);
                                break;
                            }
                            else
                                break;//we have already registered this type
                        }
                    }
                }
            }

            public PDFPageNumbering GetNumbering(int pageindex, out int numberindex, out int numberingmax)
            {
                if (_entries.Count == 0)
                    throw new ArgumentOutOfRangeException("No pages registered");
                for (int i = this._entries.Count - 1; i >= 0; i--)
                {
                    PageNumberingEntry entry = this._entries[i];
                    if (entry.PageIndex <= pageindex)
                    {
                        numberindex = pageindex - entry.PageIndex;
                        if (i == this._entries.Count - 1)
                            numberingmax = this.TotalPageCount - entry.PageIndex - 1;
                        else
                            numberingmax = _entries[i + 1].PageIndex - entry.PageIndex - 1;
                        return entry.NumberingFormat;
                    }
                }
                numberindex = pageindex;
                numberingmax = this.TotalPageCount;
                return _entries[0].NumberingFormat;
            }

            #region IEnumerable<PageNumberingEntry> Members

            public IEnumerator<PageNumberingEntry> GetEnumerator()
            {
                return this._entries.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region private class PDFReferenceChecker

        /// <summary>
        /// Tracks the references to paths and resolves relative paths in each file to be parsed.
        /// </summary>
        private class PDFReferenceChecker
        {
            private string _rootpath;
            private Stack<string> _route;
            private PDFReferenceResolver _resolver;

            
            public PDFReferenceChecker(string fullpath)
            {
                _rootpath = fullpath;
                _route = new Stack<string>();
                _route.Push(NormalizePath(fullpath));
                _resolver = new PDFReferenceResolver(this.Resolve);
            }

            private string NormalizePath(string fullpath)
            {
                return System.IO.Path.GetFullPath(fullpath).ToLower();
            }

            public IPDFComponent Resolve(string path)
            {
                string fullpath;
                if (System.Uri.IsWellFormedUriString(path, UriKind.Absolute))
                    fullpath = path.ToLower();
                else if (System.IO.Path.IsPathRooted(path))
                    fullpath = path.ToLower();
                else
                {
                    string current = _route.Peek();
                    current = System.IO.Path.GetDirectoryName(current);
                    fullpath = System.IO.Path.Combine(current, path);
                    fullpath = NormalizePath(fullpath);
                }

                if (_route.Contains(fullpath))
                    throw RecordAndRaise.ParserException(Errors.CircularReferenceToPath, path);

                _route.Push(fullpath);
                IPDFComponent comp = PDFDocument.Parse(fullpath, this.Resolver);
                if (_route.Pop() != fullpath)
                    throw RecordAndRaise.ParserException(Errors.PathStackIsUnbalanced);

                return comp;
            }

            public PDFReferenceResolver Resolver
            {
                get { return _resolver; }
            }
        }

        #endregion

    }
}
