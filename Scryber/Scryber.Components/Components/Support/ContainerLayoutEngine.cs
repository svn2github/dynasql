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

//TODO: Make sure HasSplitCurrent is updated in the loop

namespace Scryber.Components.Support
{
    /// <summary>
    /// Internal class to layout the child components of a PDFContainerComponent
    /// </summary>
    internal class ContainerLayoutEngine : IPDFLayoutEngine
    {
        #region event RequestNewPage + OnRequestNewPage

        public event PDFPageRequestHandler RequestNewPage;

        protected virtual void OnRequestNewPage(PDFPageRequestArgs args)
        {
            if (null != RequestNewPage)
            {
                this.RequestNewPage(this, args);
                
            }
        }

        #endregion

        //
        // Root Properties - do not change over the lifetime of the layout engine
        //

        #region protected PDFContainerComponent RootComponent { get; set; }

        /// <summary>
        /// Gets or sets the root component this engine should layout the child components of.
        /// </summary>
        protected PDFContainerComponent RootComponent { get; set; }

        #endregion

        #region protected PDFRect RootSpace { get; set; }

        /// <summary>
        /// Gets or sets the original space available
        /// </summary>
        protected PDFRect RootSpace { get; set; }

        #endregion

        #region protected int RootPageIndex { get; set; }

        /// <summary>
        /// Gets or sets the first page index
        /// </summary>
        protected int RootPageIndex { get; set; }

        #endregion

        #region protected PDFStyle RootStyle { get; set; }

        /// <summary>
        /// Gets the PDFStyle associated with the root component
        /// </summary>
        protected PDFStyle RootStyle { get; set; }

        #endregion

        #region protected PDFStyleStack Styles { get; }

        /// <summary>
        /// Gets or sets the Style Stack
        /// </summary>
        protected PDFStyleStack Styles { get { return this.LayoutContext.StyleStack; } }

        #endregion

        #region protected IPDFLayoutEngine ParentEngine {get;}

        private IPDFLayoutEngine _parent;
        /// <summary>
        /// Gets the engine that invoked the layout with this engine
        /// </summary>
        protected IPDFLayoutEngine ParentEngine
        {
            get { return this._parent; }
            private set { this._parent = value; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the current layout context
        /// </summary>
        protected PDFLayoutContext LayoutContext { get; set; }

        //
        // Current Properties - based upon the child component
        //

        #region protected PDFComponent CurrentComponent { get; set; }

        /// <summary>
        /// gets or sets the current component
        /// </summary>
        protected PDFComponent CurrentComponent { get; set; }

        #endregion

        #region protected PDFRect CurrentSpace { get; set; }
        /// <summary>
        /// Gets or sets the current available space
        /// </summary>
        protected PDFRect CurrentSpace { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the space that is available to layout the current component 
        /// (after margins and padding have been applied to the current space)
        /// </summary>
        protected PDFRect AvailableSpace { get; set; }

        /// <summary>
        /// Gets or sets the space that is required to completely contain the current component 
        /// (excluding margins and padding)
        /// </summary>
        protected PDFRect MeasuredSpace { get; set; }

        /// <summary>
        /// Gets or sets the space required by the current component based upon the measured space and
        /// any margins or padding
        /// </summary>
        protected PDFRect RequiredSpace { get; set; }


        #region protected int CurrentPageIndex { get; set; }

        private int _currpgindex;

        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        protected virtual int CurrentPageIndex 
        {
            get { return _currpgindex; }
            set { _currpgindex = value; }
        }

        #endregion 

        #region protected PDFStyle CurrentStyle { get; set; }

        /// <summary>
        /// Gets or sets the full style for the current component
        /// </summary>
        protected PDFStyle CurrentStyle { get; set; }

        #endregion

        #region protected PDFThickness CurrentMargins { get; set; }

        /// <summary>
        /// Gets or sets the Margins on the current component
        /// </summary>
        protected PDFThickness CurrentMargins { get; set; }

        #endregion

        #region protected PDFThickness CurrentPadding { get; set; }

        /// <summary>
        /// Gets or sets the padding on the current component
        /// </summary>
        protected PDFThickness CurrentPadding { get; set; }

        #endregion

        #region protected PositionOptions CurrentLayoutOptions { get; set; }

        /// <summary>
        /// Gets or sets the layout options for the current child component
        /// </summary>
        protected PositionOptions CurrentLayoutOptions { get; set; }

        #endregion

        #region protected PDFRect CurrentBounds { get; set; }
        /// <summary>
        /// Gets or sets the current calculated bounds for a component
        /// </summary>
        protected PDFRect CurrentBounds { get; set; }

        #endregion

        #region protected bool HasSplitCurrent {get;set;}

        private bool _split = false;

        protected bool HasSplitCurrent
        {
            get { return _split; }
            set { _split = value; }
        }

        #endregion

        #region protected bool ContineLayout {get;set;}

        private bool _cont = true;

        /// <summary>
        /// Returns true if we should continue laying out the components (otherwise false)
        /// </summary>
        protected bool ContineLayout
        {
            get { return _cont; }
            set { _cont = value; }
        }

        #endregion

        #region int IPDFLayoutEngine.LastPageIndex {get;}

        /// <summary>
        /// Implements the LastPageIndex of the IPDFLayoutEngine
        /// </summary>
        int IPDFLayoutEngine.LastPageIndex { get { return this.CurrentPageIndex; } }

        #endregion

        /// <summary>
        /// Gets or sets the last text block that was laid out.
        /// </summary>
        internal Text.PDFTextBlock LastBlock { get; set; }

        //
        // Cumulative properties. Updated as the layout progresses
        //

        #region protected PDFUnit MaxWidth { get; set; }
        
        /// <summary>
        /// Gets or sets the maximum width of all the layed out components
        /// </summary>
        protected PDFUnit MaxWidth { get; set; }

        #endregion

        #region protected PDFUnit MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum height of all the layed out components
        /// </summary>
        protected PDFUnit MaxHeight { get; set; }

        #endregion

        //
        // Support properties
        //

        #region protected PDFTraceLog TraceLog { get; }

        /// <summary>
        /// Gets or sets the Trace Log
        /// </summary>
        protected PDFTraceLog TraceLog { get { return this.LayoutContext.TraceLog; } }

        #endregion

        #region protected PDFGraphics Graphics {get;}

        private PDFGraphics _g;

        /// <summary>
        /// Gets a PDFGraphics context to call drawing and measuring methods to.
        /// </summary>
        protected PDFGraphics Graphics
        {
            get
            {
                if (null == _g)
                    _g = this.RootComponent.CreateGraphics(this.Styles, this.LayoutContext);
                return _g;
            }
        }

        #endregion

        //
        // ctors
        //

        #region .ctor(container, styles, parent, log)

        /// <summary>
        /// Creates a new ContainerLayoutEngine to layout child components
        /// </summary>
        /// <param name="container"></param>
        /// <param name="styles"></param>
        /// <param name="parent"></param>
        /// <param name="log"></param>
        public ContainerLayoutEngine(PDFContainerComponent container, IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            if (null == container)
                throw new ArgumentNullException("container");
            if (null == context)
                throw new ArgumentNullException("context");

            this.LayoutContext = context;
            this.RootComponent = container;
            this.RootStyle = this.Styles.GetFullStyle(container);
            this.ParentEngine = parent;
        }

        #endregion


        public virtual PDFSize Layout(PDFSize avail, int startPageIndex)
        {
            this.RootSpace = new PDFRect(PDFPoint.Empty, avail);
            this.CurrentSpace = new PDFRect(PDFPoint.Empty, avail);
            this.RootPageIndex = startPageIndex;
            this.CurrentPageIndex = startPageIndex;
            this.MaxWidth = 0;
            this.MaxHeight = 0;
            this.LayoutContainer(this.RootComponent);

            return new PDFSize(this.MaxWidth, this.MaxHeight);
        }


        #region protected virtual void LayoutContainer(IPDFContainerComponent container)

        /// <summary>
        /// Enumerates through all the child components in a container and lays them out
        /// </summary>
        /// <param name="container"></param>
        protected virtual void LayoutContainer(IPDFContainerComponent container)
        {
            PDFComponent[] items = null;
            if (container.HasContent)
            {
                items = container.Content.ToArray();
            }

            if (container is IPDFTopAndTailedComponent)
            {
                IPDFTopAndTailedComponent topandtail = (IPDFTopAndTailedComponent)container;
                LayoutHeaderForContainer(container, topandtail);
            }
            if (null != items)
            {
                foreach (PDFComponent comp in items)
                {
                    if (this.ContineLayout)
                        LayoutChildComponent(comp);
                    else
                        this.ClearCurrentArrangement(comp);
                }
            }
            if (container is IPDFTopAndTailedComponent)
            {
                IPDFTopAndTailedComponent topandtail = (IPDFTopAndTailedComponent)container;
                LayoutFooterForContainer(container, topandtail);
            }
        }

        protected virtual void LayoutHeaderForContainer(IPDFContainerComponent container, IPDFTopAndTailedComponent topandtail)
        {
            if (null != topandtail.Header)
            {
                LayoutContainerTemplate(container, topandtail.Header, this.CurrentPageIndex - this.RootPageIndex);
            }
        }

        protected virtual void LayoutFooterForContainer(IPDFContainerComponent container, IPDFTopAndTailedComponent topandtail)
        {
            if (null != topandtail.Footer)
            {
                LayoutContainerTemplate(container, topandtail.Footer, this.CurrentPageIndex - this.RootPageIndex);
            }
        }

        
        protected virtual void LayoutContainerTemplate(IPDFContainerComponent container, IPDFTemplate template, int pgindex)
        {
            IEnumerable<IPDFComponent> all = template.Instantiate(pgindex);
            if (null != all)
            {
                PDFTraceLog log = this.TraceLog;
                PDFItemCollection items = this.LayoutContext.Items;
                PDFInitContext init = new PDFInitContext(items, log);
                PDFLoadContext load = new PDFLoadContext(items, log);
                int index = 0;
                PDFComponentList list = container.Content;
                
                IPDFDocument doc = container.Document;
                bool bind = false;
                if (doc is PDFDocument && ((PDFDocument)doc).AutoBind)
                    bind = true;

                List<PDFComponent> added = new List<PDFComponent>();

                foreach (IPDFComponent icomp in all)
                {
                    if (!(icomp is PDFComponent))
                        throw RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, icomp.GetType(), "PDFVisualComponent");
                    PDFComponent comp = icomp as PDFComponent;
                    comp.Init(init);
                    
                    container.Content.Add(comp);
                    comp.Load(load);
                    added.Add(comp);
                }

                if (bind)
                {
                    PDFDataContext datacontext = new PDFDataContext(items, log);
                    foreach (PDFComponent comp in added)
                    {
                        if (comp is IPDFBindableComponent)
                        {
                            (comp as IPDFBindableComponent).DataBind(datacontext);
                        }
                    }
                }

                foreach (PDFComponent comp in added)
                {
                    this.LayoutChildComponent(comp);
                }
            }
        }

        private void LayoutChildComponent(PDFComponent comp)
        {
            this.CurrentComponent = comp;
            this.HasSplitCurrent = false;

            if (!comp.Visible || comp.Type == PDFObjectTypes.NoOp)
            {

                Log(TraceLevel.Debug, "Skipping component '{0}' as it is not visible or is a NoOp component", comp);
                this.ClearCurrentArrangement(comp);
                return;
            }
            Begin(TraceLevel.Debug, "Beginning layout of child component '{0}'", comp);

            this.BeginComponentLayout(comp);
            
            this.AvailableSpace = ApplyMarginsAndPaddingBeforeLayout(this.CurrentSpace);

            bool setarrange;
            //Perform the actual layout
            this.LayoutChildComponent(comp, out setarrange);

            

            if (setarrange)
            {
                if (this.CurrentLayoutOptions.HasWidth == false && this.CurrentLayoutOptions.LayoutMode == LayoutMode.Block)
                {
                    if (this.CurrentLayoutOptions.FillWidth)
                    {
                        Log(TraceLevel.Debug, "Setting the width to the available space as we are laying out for full width");
                        this.MeasuredSpace = new PDFRect(this.MeasuredSpace.X, this.MeasuredSpace.Y, this.AvailableSpace.Width, this.MeasuredSpace.Height);
                    }
                }

                this.RequiredSpace =  new PDFRect(this.CurrentSpace.Location, this.MeasuredSpace.Size);

                this.RequiredSpace = ReapplyMarginsAndPaddingAfterMeasure(this.RequiredSpace);

                this.CurrentBounds = this.RequiredSpace;


                if (this.CurrentLayoutOptions.PositionMode == PositionMode.Relative)
                {
                    this.MaxWidth = PDFUnit.Max(this.RequiredSpace.Width, this.MaxWidth);
                    this.MaxHeight = PDFUnit.Max(this.RequiredSpace.Height, this.MaxHeight);
                    this.SetCurrentArrangement();
                }
                else if (this.CurrentLayoutOptions.PositionMode == PositionMode.Absolute)
                {
                    this.SetCurrentArrangement();
                }
                else //position mode == None so we need to subtract from the available space
                {
                    bool hide = false;
                    bool overflowed = false;
                    if (this.RequiredSpace.Height > this.CurrentSpace.Height && !HasSplitCurrent)
                    {
                        if (this.CanOverflow(out hide))
                        {
                            overflowed = this.DoExtendLayoutToNewPage(this.RequiredSpace.Size);
                            if (overflowed)
                                this.CurrentBounds = new PDFRect(PDFPoint.Empty, this.RequiredSpace.Size);
                            else
                                this.ContineLayout = false; //We don't have a new page so cannot continue
                        }
                        else
                            this.ContineLayout = false; //can't overflow
                    }
                    if (!hide)
                    {
                        this.SetCurrentArrangement();
                        this.CurrentSpace = new PDFRect(this.CurrentSpace.X, this.CurrentSpace.Y + this.RequiredSpace.Height, 
                            this.CurrentSpace.Width, this.CurrentSpace.Height - this.RequiredSpace.Height);
                        this.MaxWidth = PDFUnit.Max(this.RequiredSpace.Width, this.MaxWidth);
                        this.MaxHeight += this.RequiredSpace.Height;
                    }
                    else if (!overflowed)
                    {
                        this.ClearCurrentArrangement(comp);
                        this.ContineLayout = false;
                    }

                }


            }
            this.EndComponentLayout(comp);

            End(TraceLevel.Debug, "Completed layout of child component '{0}'", comp);
        }

        #region private PDFRect ReapplyMarginsAndPaddingAfterMeasure(PDFRect used)
        
        private PDFRect ReapplyMarginsAndPaddingAfterMeasure(PDFRect used)
        {
            //If we have a padding then increase the size to take account of this.
            if (!this.CurrentPadding.IsEmpty)
                used = new PDFRect(used.Location.X,
                                    used.Location.Y,
                                    used.Width + this.CurrentPadding.Left + this.CurrentPadding.Right,
                                    used.Height + this.CurrentPadding.Top + this.CurrentPadding.Bottom);

            //If we have an explicit X or Y then they must be set after the padding
            if (this.CurrentLayoutOptions.HasX)
                used.X = this.CurrentLayoutOptions.X;
            if (this.CurrentLayoutOptions.HasY)
                used.Y = this.CurrentLayoutOptions.Y;


            if (this.CurrentLayoutOptions.HasHeight)
                used.Height = this.CurrentLayoutOptions.Height;
            if (this.CurrentLayoutOptions.HasWidth)
                used.Width = this.CurrentLayoutOptions.Width;

            if (!this.CurrentMargins.IsEmpty)
            {
                used = new PDFRect(used.X,
                                   used.Y,
                                   used.Width + this.CurrentMargins.Left + this.CurrentMargins.Right,
                                   used.Height + this.CurrentMargins.Top + this.CurrentMargins.Bottom);
            }
            return used;
        }

        #endregion

        #region private PDFRect ApplyMarginsAndPaddingBeforeLayout(PDFRect avail)

        private PDFRect ApplyMarginsAndPaddingBeforeLayout(PDFRect avail)
        {
            //Width and Height are inclusive of padding, but exclusive of Margins

            //So if something is defined as 200x200 with 10 padding and 10 margins
            //then final size will be 220x220 and available  space for inner items
            //will be 180x180.

            if (!this.CurrentMargins.IsEmpty)
                avail = avail.Inset(this.CurrentMargins);

            if (this.CurrentLayoutOptions.HasWidth)
                avail.Width = this.CurrentLayoutOptions.Width;

            if (this.CurrentLayoutOptions.HasHeight)
                avail.Height = this.CurrentLayoutOptions.Height;

            if (!this.CurrentPadding.IsEmpty)
                avail = avail.Inset(this.CurrentPadding);
            return avail;
        }

        #endregion


        #endregion

        #region protected virtual PDFSize LayoutChildComponent(PDFComponent comp, PDFRect avail)

        protected virtual void LayoutChildComponent(PDFComponent comp, out bool cont)
        {
            cont = true;
            PDFSize childsize;
            PDFRect avail = this.AvailableSpace;

            if (comp is IPDFViewPortComponent)
            {
                IPDFViewPortComponent viewport = (IPDFViewPortComponent)comp;
                childsize = this.LayoutViewPortComponent(viewport, avail);
            }
            else if (comp is IPDFTextComponent)
            {
                IPDFTextComponent text = (IPDFTextComponent)comp;
                childsize = this.LayoutTextComponent(text, avail);
            }
            else if (comp is IPDFGraphicPathComponent)
            {
                IPDFGraphicPathComponent graph = (IPDFGraphicPathComponent)comp;
                childsize = this.LayoutGraphicComponent(graph, avail);
            }
            else if (comp is IPDFImageComponent)
            {
                IPDFImageComponent img = (IPDFImageComponent)comp;
                childsize = this.LayoutImageComponent(img, avail);
            }
            else if (comp is IPDFInvisibleContainer)
            {
                IPDFInvisibleContainer invis = (IPDFInvisibleContainer)comp;
                this.LayoutInvisibleContents(invis);
                childsize = PDFSize.Empty;
                cont = false;
            }
            else if (comp is IPDFPageBreak)
            {
                IPDFPageBreak pgbr = (IPDFPageBreak)comp;
                this.LayoutPageBreak(pgbr);
                childsize = PDFSize.Empty;
                cont = false;
            }
            else
                throw RecordAndRaise.LayoutException(Errors.ComponentIsNotKnownToLayoutEngine, comp.Type);

            this.MeasuredSpace = new PDFRect(PDFPoint.Empty, childsize);

        }

        #endregion

        #region protected virtual PDFSize LayoutViewPortComponent(IPDFViewPortComponent viewport, PDFRect avail)

        /// <summary>
        /// Lays out components that implement the IPDFViewPortComponent interface
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="avail"></param>
        /// <returns></returns>
        protected virtual PDFSize LayoutViewPortComponent(IPDFViewPortComponent viewport, PDFRect avail)
        {
            using (IPDFLayoutEngine engine = viewport.GetEngine(this,this.LayoutContext))
            {
                PDFPageRequestHandler handler = new PDFPageRequestHandler(ChildEngineRequestNewPage);
                engine.RequestNewPage += handler;
                PDFSize childsize = engine.Layout(avail.Size, this.CurrentPageIndex);
                engine.RequestNewPage -= handler;
                return childsize;
            }
        }

        #endregion

        #region protected virtual PDFSize LayoutTextComponent(IPDFTextComponent text, PDFRect avail)

        /// <summary>
        /// Lays out components that implement the IPDFTextComponent interface
        /// </summary>
        /// <param name="text"></param>
        /// <param name="avail"></param>
        /// <returns></returns>
        protected virtual PDFSize LayoutTextComponent(IPDFTextComponent text, PDFRect avail)
        {
            //TODO: Handle the layout if the block runs over the end of the page
            using (Text.PDFTextReader reader = text.CreateReader())
            {
                Text.PDFTextRenderOptions opts = this.CurrentStyle.CreateTextStyle();
                Text.PDFTextBlock block = this.Graphics.MeasureBlock(reader, opts, avail.Size);
                text.TextBlock = block;
                this.LastBlock = block;
                return block.Size;
            }
        }

        #endregion

        #region protected virtual PDFSize LayoutGraphicComponent(IPDFGraphicPathComponent graph, PDFRect avail)

        /// <summary>
        /// Lays out components that implement the IPDFGraphicPathComponent interface
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="avail"></param>
        /// <returns></returns>
        protected virtual PDFSize LayoutGraphicComponent(IPDFGraphicPathComponent graph, PDFRect avail)
        {
            PDFGraphicsPath path = graph.CreatePath(avail.Size, this.CurrentStyle);
            graph.Path = path;
            return path.Bounds.Size;
        }

        #endregion

        #region protected virtual PDFSize LayoutImageComponent(IPDFImageComponent img, PDFRect avail)

        /// <summary>
        /// Lays out components that implement the IPDFImageComponent interface
        /// </summary>
        /// <param name="img"></param>
        /// <param name="avail"></param>
        /// <returns></returns>
        protected virtual PDFSize LayoutImageComponent(IPDFImageComponent img, PDFRect avail)
        {
            Resources.PDFImageXObject xobj = img.GetImageObject();
            PDFSize size = xobj.GetImageSize();
            size = PDFImage.AdjustImageSize(this.CurrentStyle, size);
            return size;
        }

        #endregion

        #region protected virtual void LayoutInvisibleContents(IPDFInvisibleContainer container)

        /// <summary>
        /// Lays out components that implement the IPDFInvsibleContainer interface
        /// </summary>
        /// <param name="container"></param>
        protected virtual void LayoutInvisibleContents(IPDFInvisibleContainer container)
        {
            Begin(TraceLevel.Debug, "Laying out the inner components of invisible container {0}", container);
            int startpage = this.CurrentPageIndex;
            PDFStyle style = this.CurrentStyle;

            PDFComponentList contents = container.Content;
            
            foreach (PDFComponent comp in contents)
            {
                if (this.ContineLayout)
                    this.LayoutChildComponent(comp);
                else
                    this.ClearCurrentArrangement(comp);
            }
            int endpage = this.CurrentPageIndex;
            ((PDFComponent)container).SetArrangement(GetInvisibleArrangement(startpage, endpage, style));
            End(TraceLevel.Debug, "Completed the layout of invisible container {0}", container);
        }

        private PDFComponentArrangement GetInvisibleArrangement(int startpage, int endpage, PDFStyle style)
        {
            PDFComponentMultiArrangement arrange = null;

            int pg = startpage;
            while (pg <= endpage)
            {
                PDFComponentMultiArrangement curr = new PDFComponentMultiArrangement()
                {
                    Content = PDFRect.Empty,
                    AppliedStyle = style,
                    Border = PDFRect.Empty,
                    Display = true,
                    Margins = PDFThickness.Empty(),
                    Bounds = PDFRect.Empty,
                    Padding = PDFThickness.Empty(),
                    PageIndex = pg,
                    PositionMode = PositionMode.Flow
                };

                if (null == arrange)
                    arrange = curr;
                else
                    arrange.AppendArrangement(curr);
                pg++;
            }

            return arrange;
        }

        #endregion

        #region protected virtual void LayoutPageBreak(IPDFPageBreak pgbreak)
        
        /// <summary>
        /// Lays out components that implement the IPDFPageBreak interface
        /// </summary>
        /// <param name="pgbreak"></param>
        protected virtual void LayoutPageBreak(IPDFPageBreak pgbreak)
        {
            if (!this.DoExtendLayoutToNewPage(PDFSize.Empty))
                this.ContineLayout = false;
        }

        #endregion

        //
        // Support methods
        //

        protected object GetCurrentComponentData()
        {
            //if (null != this.CurrentComponent)
            //{
            //    this.Styles.Pop();

            //    return this.CurrentComponent;
            //}
            //else
                return null;
        }

        protected void RestoreCurrentComponent(object data)
        {
            //if (null != data)
            //{
            //    PDFComponent comp = (PDFComponent)data;
            //    this.BeginComponentLayout(comp);
            //}
        }

        #region private void BeginComponentLayout(PDFComponent comp)

        /// <summary>
        /// Sets up the engine instance variables to begin laying out a child component
        /// </summary>
        /// <param name="comp"></param>
        protected virtual void BeginComponentLayout(PDFComponent comp)
        {
            this.CurrentComponent = comp;
            PDFStyle style = comp.GetAppliedStyle();
            if (null == style)
                style = new PDFStyle();
            this.Styles.Push(style);

            this.CurrentStyle = this.Styles.GetFullStyle(comp);

            SetUpCurrentPropertiesFromStyle(this.CurrentStyle);
            this.AvailableSpace = PDFRect.Empty;
            this.MeasuredSpace = PDFRect.Empty;
            this.RequiredSpace = PDFRect.Empty;
        }

        protected void SetUpCurrentPropertiesFromStyle(PDFStyle style)
        {
            PDFMarginsStyle margins;
            PDFPaddingStyle padding;
            if (style.TryGetMargins(out margins))
                this.CurrentMargins = margins.GetThickness();
            else
                this.CurrentMargins = PDFThickness.Empty();

            if (style.TryGetPadding(out padding))
                this.CurrentPadding = padding.GetThickness();
            else
                this.CurrentPadding = PDFThickness.Empty();

            PDFPositionStyle pos;
            if (style.TryGetPosition(out pos))
                this.CurrentLayoutOptions = new PositionOptions(pos);
            else
                this.CurrentLayoutOptions = new PositionOptions();

            this.CurrentBounds = PDFRect.Empty;
            
        }

        #endregion

        #region private void EndComponentLayout(PDFComponent comp)

        /// <summary>
        /// Clears the instance variables storing the set up of the component being laid out.
        /// </summary>
        /// <param name="comp"></param>
        protected virtual void EndComponentLayout(PDFComponent comp)
        {
            this.CurrentComponent = null;
            this.CurrentStyle = null;
            this.Styles.Pop();
            this.CurrentMargins = PDFThickness.Empty();
            this.CurrentPadding = PDFThickness.Empty();
            this.CurrentLayoutOptions = null;
            this.MeasuredSpace = PDFRect.Empty;
            this.AvailableSpace = PDFRect.Empty;
            this.RequiredSpace = PDFRect.Empty;
        }

        #endregion


        #region protected void SetCurrentArrangement() + 1 overload

        /// <summary>
        /// Pushes all the current measurement arrangements on to this component
        /// </summary>
        protected void SetCurrentArrangement()
        {
            this.SetArrangement(this.CurrentComponent, this.CurrentBounds, true, this.CurrentPageIndex);
        }

        protected virtual void SetArrangement(PDFComponent component, PDFRect bounds, bool display, int pgIndex)
        {
            PDFComponentMultiArrangement arrange = new PDFComponentMultiArrangement();
            arrange.Bounds = bounds;
            arrange.AppliedStyle = this.CurrentStyle;
            arrange.Display = display;
            arrange.PageIndex = pgIndex;
            arrange.PositionMode = this.CurrentLayoutOptions.PositionMode;
            arrange.Padding = this.CurrentPadding;
            arrange.Margins = this.CurrentMargins;
            arrange.Border = bounds.Inset(this.CurrentMargins);
            arrange.Content = arrange.Border.Inset(this.CurrentPadding);

            component.SetArrangement(arrange);
        }

        #endregion

        #region private void ClearCurrentArrangement(PDFComponent comp)

        /// <summary>
        /// Clears any and all arrangements on the specified component
        /// </summary>
        /// <param name="comp"></param>
        private void ClearCurrentArrangement(PDFComponent comp)
        {
            comp.ClearArrangement();
        }

        #endregion

        //
        // New page requests
        //


        private const bool DefaultAllowOverflow = true;

        #region protected virtual bool CanOverflow(out bool hide)

        /// <summary>
        /// Checks  the current style and if we have an overflow action of NewPage (or the default is to allow new pages
        /// then checks if we can split the component whose children we are currently enumerating over.
        /// </summary>
        /// <param name="hide">Set to true if we should hide (not render) the current component</param>
        /// <returns></returns>
        protected virtual bool CanOverflow(out bool hide)
        {

            PDFOverflowStyle over;
            bool overflow = false;
            hide = false;
            if (this.CurrentStyle.TryGetOverflow(out over) || DefaultAllowOverflow)
            {
                if (over.Action == OverflowAction.NewPage)
                {
                    if (null == this.ParentEngine)
                        overflow = true;
                    else
                        overflow = this.ParentEngine.CanSplitCurrentComponent();
                }
                else if (over.Action == OverflowAction.Truncate)
                    hide = true;
            }

            return overflow;
        }

        #endregion

        #region protected virtual bool DoExtendLayoutToNewPage()

        /// <summary>
        /// Builds the PageRequestArgs, calls the OnRequestNewPage event wrapper and returns true if a new page was granted
        /// </summary>
        /// <returns></returns>
        protected virtual bool DoExtendLayoutToNewPage(PDFSize requiredspace)
        {
            int page = this.CurrentPageIndex;
            PDFPageRequestArgs args = new PDFPageRequestArgs(page, this, requiredspace);
            this.OnRequestNewPage(args);
            if (args.HasNewPage)
            {
                PDFPoint offset = PDFPoint.Empty;
                offset.X += this.CurrentMargins.Left + this.CurrentPadding.Left;
                offset.Y += this.CurrentMargins.Top + this.CurrentPadding.Top;
                this.BeginNewPage(args.NewPageIndex, new PDFRect(offset, args.NewAvailableSpace.Size));
            }
            return args.HasNewPage;
        }


        #endregion

        #region public bool CanSplitCurrentComponent()

        /// <summary>
        /// Checks the component hierarchy to see if any of the
        /// current components cannot be split across multiple pages.
        /// </summary>
        /// <returns></returns>
        public bool CanSplitCurrentComponent()
        {
            PDFOverflowStyle over;
            bool overflow = false;
            if (this.CurrentStyle.TryGetOverflow(out over) || DefaultAllowOverflow)
            {
                if (over.Split == OverflowSplit.Any)
                {
                    if (null == this.ParentEngine)
                        overflow = true;
                    else
                        overflow = this.ParentEngine.CanSplitCurrentComponent();
                }
            }
            return overflow;
        }

        #endregion

        #region protected PDFRect BeginNewPage(int pageindex, PDFRect newbounds)

        protected virtual PDFRect BeginNewPage(int pageindex, PDFRect newbounds)
        {
            this.CurrentPageIndex = pageindex;
            this.CurrentSpace = newbounds;
            
            this.HasSplitCurrent = true;
            this.MaxHeight = 0;
            //Don't reset the width as this is cumulative over time no matter how many pages
            return newbounds;
        }

        #endregion

        protected virtual void ChildEngineRequestNewPage(object sender, PDFPageRequestArgs args)
        {
            //Record the current available space.
            //As the child is requesting a new page, we know it requires more than the current space.
            PDFUnit width = args.MeasuredSize.Width;
            PDFPositionStyle pos;
            if (this.CurrentStyle.TryGetPosition(out pos) && pos.IsDefined(StyleKeys.WidthAttr))
                width = pos.Width;

            PDFRect childused = new PDFRect(this.CurrentSpace.Location, new PDFSize(width, this.CurrentSpace.Height));

            PDFSize measured = args.MeasuredSize;
            int pg = this.CurrentPageIndex;

            PDFUnit w = PDFUnit.Max(this.MaxWidth, args.MeasuredSize.Width);
            PDFUnit h = this.CurrentSpace.Height;

            args.MeasuredSize = new PDFSize(w, h);

            this.OnRequestNewPage(args);
            if (args.HasNewPage)
            {
                this.SetArrangement(this.CurrentComponent,
                                        childused, true, pg);

                PDFPoint offset = PDFPoint.Empty;
                //offset.X += this.CurrentMargins.Left + this.CurrentPadding.Left;
                //offset.Y += this.CurrentMargins.Top + this.CurrentPadding.Top;
                PDFRect newbounds = this.BeginNewPage(args.NewPageIndex, new PDFRect(offset, args.NewAvailableSpace.Size));
                args.NewAvailableSpace = newbounds;
            }
            args.MeasuredSize = measured;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _g)
                    _g.Dispose();

                if (this.TraceLog.ShouldLog(TraceLevel.Debug))
                    Log(TraceLevel.Debug, "Disposing Layout engine for component '{0}'", this.RootComponent.UniqueID);
            }
        }

        ~ContainerLayoutEngine()
        {
            this.Dispose(false);
        }

        #endregion

        #region Logging Methods

        private const string LayoutEngineCategory = "LAYOUT ENGINE";

        protected void Log(TraceLevel level, string message, params object[] parameters)
        {
            if (this.TraceLog.ShouldLog(level))
            {
                if (null != parameters && parameters.Length > 0)
                    message = string.Format(message, parameters);
                this.TraceLog.Add(level, LayoutEngineCategory, message);
            }
        }

        protected void Begin(TraceLevel level, string message, params object[] parameters)
        {
            if (this.TraceLog.ShouldLog(level))
            {
                if (null != parameters && parameters.Length > 0)
                    message = string.Format(message, parameters);
                this.TraceLog.Begin(level, message);
            }
        }

        protected void End(TraceLevel level, string message, params object[] parameters)
        {
            if (this.TraceLog.ShouldLog(level))
            {
                if (null != parameters && parameters.Length > 0)
                    message = string.Format(message, parameters);
                this.TraceLog.End(level, message);
            }
        }

        #endregion
    }
}
