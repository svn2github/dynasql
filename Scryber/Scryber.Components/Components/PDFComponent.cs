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
using System.Drawing;
using Scryber.Native;
using Scryber.Styles;
using Scryber.Drawing;

namespace Scryber.Components
{
    /// <summary>
    /// Base class for all complex pdf Components
    /// </summary>
    public abstract class PDFComponent : PDFObject, IDisposable, IPDFComponent, IPDFBindableComponent, IPDFLoadableComponent
    {
        // ivars

        PDFStyle _appliedStyle = null; //local caching for a the style information on this Component
        PDFComponentArrangement _arrange = null; //the arrangement from the layout

        //
        //Events
        //

        #region EventHandler PreRender Event + OnPreRender(EventArgs)

        /// <summary>
        /// Notifies receivers that this instance is about to be Rendered. This is the last chance to change properties
        /// </summary>
        [PDFAttribute("on-prerender")]
        public event EventHandler PreRender;

        /// <summary>
        /// Raises the PreRender event. Inheritors can override this method to perfom their own actions
        /// </summary>
        /// <param name="e">The arguments</param>
        protected virtual void OnPreRender(EventArgs e)
        {
            if (this.PreRender != null)
                this.PreRender(this, e);
        }

        #endregion

        #region EventHandler PostRender Event + OnPostRender(EventArgs)

        /// <summary>
        /// Notifies receivers that this instance has been rendered. Clean up can now be performed
        /// </summary>
        [PDFAttribute("on-postrender")]
        public event EventHandler PostRender;

        /// <summary>
        /// Raises the PostRender event. Inheritors can override this method to perfom their own actions
        /// </summary>
        /// <param name="e">The arguments</param>
        protected virtual void OnPostRender(EventArgs e)
        {
            if (this.PostRender != null)
                this.PostRender(this, e);
        }

        #endregion

        #region EventHandler DataBinding Event + OnDataBinding(PDFDataBindEventArgs e)

        /// <summary>
        /// Notifies receivers that this instance is in the process of being data bound
        /// </summary>
        [PDFAttribute("on-databinding")]
        public event PDFDataBindEventHandler DataBinding;


        /// <summary>
        /// Raises the DataBinding event. Inheritors can override this method to perfom their own actions
        /// </summary>
        /// <param name="e">The arguments</param>
        protected virtual void OnDataBinding(PDFDataBindEventArgs e)
        {
            if (this.DataBinding != null)
                this.DataBinding(this, e);
        }

        #endregion

        #region EventHandler DataBound Event + OnDataBound(EventArgs)

        /// <summary>
        /// Notifies receivers that this instance has been databound
        /// </summary>
        [PDFAttribute("on-databound")]
        public event PDFDataBindEventHandler DataBound;

        /// <summary>
        /// Raises the DataBound event. Inheritors can override this method to perfom their own actions
        /// </summary>
        /// <param name="e">The arguments</param>
        protected virtual void OnDataBound(PDFDataBindEventArgs e)
        {
            if (this.DataBound != null)
                this.DataBound(this, e);
        }

        #endregion

        #region EventHandler Initialized Event + OnInitialized(EventArgs)

        /// <summary>
        /// Event that notifies receivers that this instance has now been fully initialized
        /// </summary>
        [PDFAttribute("on-initialized")]
        public event EventHandler Initialized;


        protected virtual void OnInitialized(EventArgs e)
        {
            if (this.Initialized != null)
                this.Initialized(this, e);
        }

        #endregion

        #region public event EventHandler Loaded + OnLoaded(EventArgs)

        /// <summary>
        /// Event that notifies receivers that this instance has now been loaded
        /// </summary>
        [PDFAttribute("on-loaded")]
        public event EventHandler Loaded;

        protected virtual void OnLoaded(EventArgs e)
        {
            if (null != this.Loaded)
                this.Loaded(this, e);
        }
        #endregion


        //
        // public properties
        //

        #region public PDFComponent Parent {get;}

        private PDFComponent _par;

        /// <summary>
        /// Gets or Sets the Parent of this Component in the PDF Hierarchy
        /// </summary>
        public PDFComponent Parent
        {
            get { return _par; }
            set 
            { 
                this._par = value;
                if (null != value)
                    this.ResetChildIDs();
            }
        }


        /// <summary>
        /// Explicit interface implementation
        /// </summary>
        IPDFComponent IPDFComponent.Parent
        {
            get { return this.Parent; }
            set 
            {
                if (value is PDFComponent)
                    this.Parent = (PDFComponent)value;
                else if (null == value)
                    this.Parent = null;
                else
                    throw RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, value.GetType().Name, "PDFComponent");
            }
        }


        

        /// <summary>
        /// Sets the Parent of prevValue to null if it is owned by this instance, 
        /// and sets the Parent of newValue to this if it is not owned by this instance
        /// </summary>
        /// <param name="prevvalue">The previously owned value if any</param>
        /// <param name="newvalue">The new owned value if any</param>
        /// <returns>The new owned value</returns>
        protected PDFComponent AssignSelfAsParent(PDFComponent prevchild, PDFComponent newchild)
        {
            if (null != prevchild && object.ReferenceEquals(this, prevchild.Parent))
                prevchild.Parent = null;

            if (null != newchild && object.ReferenceEquals(this, newchild.Parent) == false)
                newchild.Parent = this;

            return newchild;
        }

        #endregion

        #region public PDFDocument Document {get;}


        /// <summary>
        /// Gets the document that this Component belongs to.
        /// </summary>
        public PDFDocument Document
        {
            get
            {
                if (this is PDFDocument)
                    return this as PDFDocument;
                else if (this.Parent != null)
                    return this.Parent.Document;
                else
                    return null;
            }
        }

        #endregion

        IPDFDocument IPDFComponent.Document
        {
            get { return this.Document; }
        }

        #region ID {get;set;} + UniqueID{get;}

        private string _id;

        /// <summary>
        /// Gets or sets the ID for this instance
        /// </summary>
        [PDFAttribute("id")]
        public string ID
        {
            get 
            {
                if (String.IsNullOrEmpty(_id))
                {
                    if (this.Document != null)
                        _id = this.Document.GetIncrementID(this.Type);
                    else
                        _id = string.Empty;
                }
                return this._id;
            }
            set 
            {
                _id = value;
                if (this.Parent != null)
                    this.Parent.ChildIDHasChanged(this);
            }
        }

        protected virtual void ChildIDHasChanged(PDFComponent child)
        {
            
        }

        /// <summary>
        /// Gets the complete string representation of the ID of this instance separated using '$'
        /// </summary>
        public string UniqueID
        {
            get
            {
                return this.BuildUniqueID(Const.UniqueIDSeparator);
            }
        }

        /// <summary>
        /// Builds and returns the full id for this Component based upon its naming containers.
        /// </summary>
        /// <param name="separator">The string that is used to separate individual ids when building the ID</param>
        /// <returns>The full unique id</returns>
        protected string BuildUniqueID(string separator)
        {
            StringBuilder sb = new StringBuilder();
            this.BuildUniqueID(sb, separator);
            return sb.ToString();

        }

        private void BuildUniqueID(System.Text.StringBuilder sb, string separator)
        {
            PDFComponent ppe = this.Parent;
            Stack<string> names = new Stack<string>();

            while (ppe != null)
            {
                if (ppe is IPDFNamingContainer)
                {
                    names.Push(ppe.ID);
                }
                ppe = ppe.Parent;
            }
            while (names.Count > 0)
            {
                sb.Append(names.Pop());
                sb.Append(separator);
            }
            
            sb.Append(this.ID);

        }

        #endregion

        #region public string Name {get;set;}

        private string _name;
        /// <summary>
        /// Gets or sets the name of this component - used in the name dictionary and for linking
        /// </summary>
        [PDFAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region string StyleClass {get;set;}

        private string _class;
        
        /// <summary>
        /// Gets or sets the associated style class name for this Component
        /// </summary>
        [PDFAttribute("style-class")]
        public string StyleClass
        {
            get { return _class; }
            set 
            {
                _class = value; 
            }
        }

        #endregion

        #region public PDFPage Page {get;}

        /// <summary>
        /// Gets the page that this Component belongs to.
        /// </summary>
        public virtual PDFPage Page
        {
            get
            {
                PDFComponent pe = this.Parent;
                do
                {
                    if (pe is PDFPage)
                        return pe as PDFPage;
                    else if (pe != null)
                        pe = pe.Parent;
                }
                while (pe != null);

                return null;
            }
        }

        #endregion

        #region public bool Visible {get;set;}

        private bool _vis = true;

        /// <summary>
        /// Gets or sets the visibility of the Page Component
        /// </summary>
        [PDFAttribute("visible")]
        public virtual bool Visible
        {
            get
            {
                return _vis;
            }
            set
            {
                _vis = value;
            }
        }



        #endregion

        #region public string LoadedSource {get;set;}

        private string _src;

        /// <summary>
        /// Gets or sets the full source path this component was loaded from (if any)
        /// </summary>
        public virtual string LoadedSource
        {
            get { return _src; }
            set { _src = value; }
        }

        #endregion

        #region public ComponentLoadType LoadType

        private ComponentLoadType _loadtype = ComponentLoadType.None;

        /// <summary>
        /// Gets or sets a value that indicates the load type for this component. Inheritors can set this value.
        /// </summary>
        /// <remarks>Based upon this value we can identify if the component was loaded via the reflective parser, 
        /// or CodeDomGenerator, or a web build provider. If this components value is none, then the parent value is 
        /// checked and returned (and so on up the hierarchy)</remarks>
        public ComponentLoadType LoadType
        {
            get
            {
                if (this._loadtype == ComponentLoadType.None && this.Parent != null)
                    return this.Parent.LoadType;
                else
                    return _loadtype;
            }
            set { _loadtype = value; }
        }

        #endregion

        #region public PDFOutline Outline {get;set;} + bool HasOutline {get;}

        private PDFOutline _outline;

        /// <summary>
        /// Gets or sets the outline title for this component
        /// </summary>
        [PDFElement("Outline")]
        public virtual PDFOutline Outline
        {
            get 
            {
                if (null == this._outline)
                {
                    this._outline = new PDFOutline();
                    this._outline.BelongsTo = this;
                }
                return this._outline;
            }
            set
            {
                if (null != this._outline)
                    this._outline.BelongsTo = null;

                this._outline = value;
                
                if (null != value)
                    this._outline.BelongsTo = this;
            }
        }

        public bool HasOutline
        {
            get { return null != this._outline && !string.IsNullOrEmpty(this._outline.Title); }
        }

        #endregion


        //
        //constructors
        //

        #region protected .ctor(PDFObjectType)

        protected PDFComponent(PDFObjectType type): base(type)
        {
        }

        #endregion

        //
        //public methods
        //

        #region public void Init() + protected virtual void DoInit()

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public void Init(PDFInitContext context)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Init Component '" + this.UniqueID + "'");

            this.DoInit(context);
            this.OnInitialized(EventArgs.Empty);

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Init Component '" + this.UniqueID + "'");

        }

        /// <summary>
        /// Inheritors should override this method to perform their own initialization
        /// </summary>
        protected virtual void DoInit(PDFInitContext context)
        {
        }

        #endregion

        #region public void Load(PDFLoadContext context) + protected virtual void DoLoad()

        /// <summary>
        /// Load operation
        /// </summary>
        public void Load(PDFLoadContext context)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Load Component '" + this.UniqueID + "'");

            this.DoLoad(context);
            this.OnLoaded(EventArgs.Empty);

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Load Component '" + this.UniqueID + "'");

        }

        /// <summary>
        /// Inheritors should override this method to perform their own loading operations
        /// </summary>
        protected virtual void DoLoad(PDFLoadContext context)
        {
        }

        #endregion

        #region public void DataBind() + public void DataBind(bool includeChildren) + protected virtual void DoDataBind(bool includeChildren)

        /// <summary>
        /// Databinds this page Component and any children
        /// </summary>
        public void DataBind(PDFDataContext context)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Databind Component '" + this.UniqueID + "'");


            PDFDataBindEventArgs args = new PDFDataBindEventArgs(context);
            this.OnDataBinding(args);

            this.DoDataBind(context, true);

            this.OnDataBound(args);

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Databind Component '" + this.UniqueID + "'");
        }

        /// <summary>
        /// Inheritors should override this method to provide their own data binding implementations
        /// </summary>
        /// <param name="includeChildren"></param>
        protected virtual void DoDataBind(PDFDataContext context, bool includeChildren)
        {
            if (this._outline != null)
                this.Outline.DataBind(context);
        }

        #endregion


        public void RegisterArtefacts(PDFRegistrationContext context)
        {
            PDFStyle style = this.GetAppliedStyle();
            if (null != style)
                context.StyleStack.Push(style);
            PDFStyle full = context.StyleStack.GetFullStyle(this);

            //register the name
            object name = context.Document.RegisterCatalogEntry(context, PDFArtefactTypes.Names, new PDFDestination(this, OutlineFit.FullPage));

            object outline = null;
            //if we have a title then register the outline component
            if (this.HasOutline)
            {
                outline = context.Document.RegisterCatalogEntry(context, PDFArtefactTypes.Outlines, new PDFOutlineRef(this.Outline, full.Outline));
            }

            this.DoRegisterArtefacts(context, full);

            if (this.HasOutline)
                context.Document.CloseArtefactEntry(PDFArtefactTypes.Outlines, outline);

            context.Document.CloseArtefactEntry(PDFArtefactTypes.Names, name);

            if (null != style)
                context.StyleStack.Pop();
        }


        protected virtual void DoRegisterArtefacts(PDFRegistrationContext context, PDFStyle fullstyle)
        {
            
        }

        #region public virtual PDFObjectRef RenderToPDF(context,writer) + protected virtual PDFObjectRef DoRenderToPDF(context, writer)

        /// <summary>
        /// Renders the page Component and all children to the writer using the context
        /// </summary>
        /// <param name="context">The render context under which the rendering is occurring</param>
        /// <param name="writer">The PDFWriter that will contain the output</param>
        /// <returns>An optional indirect object reference to any new object that was created during the render process.</returns>
        public virtual PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Begin(TraceLevel.Debug, "Render Component '" + this.UniqueID + "'");

                    
            PDFObjectRef oref;
            //raise the PreRender event
            this.OnPreRender(EventArgs.Empty);

            PDFComponentArrangement arrange = this.GetArrangement(context.PageIndex);
            if (this.ShouldRender(arrange, context))
            {
                bool cliptosize = this.ClipGraphicsToSize(context);
                oref = this.InternalRender(arrange, context, writer, cliptosize);

                //Set the physical boundary in the page of this component
                PDFRect render = arrange.Bounds;
                render.X += context.Offset.X;
                render.Y += context.Offset.Y;
                if (context.DrawingOrigin == DrawingOrigin.TopLeft)
                    render.Y = context.PageSize.Height - (render.Y + render.Height);
                arrange.RenderBounds = render;
            }
            else
            {
                if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                    context.TraceLog.Add(TraceLevel.Debug, "RENDER", "Skipping render of component '" + this.ToString() + "' in page '" + context.PageIndex + "' as false returned from 'ShouldRender'");
                if (null != arrange)
                    arrange.RenderBounds = PDFRect.Empty;

                oref = null;
            }

            // raise the PostRender Event
            this.OnPostRender(EventArgs.Empty);
            
            

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.End(TraceLevel.Debug, "Render Component '" + this.UniqueID + "'");

            return oref;
        }

        protected virtual bool ShouldRender(PDFComponentArrangement arrange, PDFRenderContext context)
        {
            if (!this.Visible || null == arrange)
                return false;
            else if (arrange.PageIndex != context.PageIndex)
                return false;
            else
                return true;
        }

        
        /// <summary>
        /// Performs the standard rendering of an Component adding margins, 
        /// backgrounds, setting up the context for the DoRenderToPDF and finally applying borders
        /// </summary>
        /// <param name="arrange">The arrangement from the LayoutEngine</param>
        /// <param name="context">The rendering context</param>
        /// <param name="writer">The current writer</param>
        /// <param name="cliptosize">Flag to identify if this should clip</param>
        /// <returns></returns>
        internal PDFObjectRef InternalRender(PDFComponentArrangement arrange, PDFRenderContext context, PDFWriter writer, bool cliptosize)
        {
            PDFRect orig = new PDFRect(context.Offset, context.Space);
            
            //Set up the actual rect of this Component and the context
            PDFRect actual = arrange.Bounds;
            if (arrange.PositionMode != PositionMode.Absolute)
            {
                actual.Location = new PDFPoint(actual.Location.X + context.Offset.X, actual.Location.Y + context.Offset.Y);
                
                if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                    context.TraceLog.Add(TraceLevel.Debug,"RENDER", "Position of Component is relative at location '" + actual.Location + "'");

                
                context.Offset = actual.Location;
                context.Space = actual.Size;
            }
            else
            {
                if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                    context.TraceLog.Add(TraceLevel.Debug,"RENDER", "Position of Component is absolute at location '" + arrange.Bounds.Location + "'");

                context.Offset = arrange.Bounds.Location;
                context.Space = arrange.Bounds.Size;
            }

            PDFStyle full = arrange.AppliedStyle;


            //Get the border to draw
            PDFBorderStyle border;
            bool hasborder = full.TryGetBorder(out border);

            //Get any background
            PDFBackgroundStyle bg;
            bool hasBg = full.TryGetBackground(out bg);
            
            //The border rect
            PDFRect borderrect = actual;
            if (arrange.Margins.IsEmpty == false)
            {
                borderrect = borderrect.Inset(arrange.Margins);
                if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                    context.TraceLog.Add(TraceLevel.Debug, "RENDER", "Component inset with margins '" + arrange.Margins + "'");

                
            }


            PDFObjectRef oref;
            using (PDFGraphics g = this.CreateGraphics(writer, context.StyleStack, context))
            {
                if (hasBg)
                {
                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Add(TraceLevel.Debug,"RENDER", "Rendering background");

                    if (hasborder && border.CornerRadius > 0)
                    {
                        if (border.IsDefined(StyleKeys.SidesAttr) && (border.Sides != (Sides.Bottom & Sides.Left & Sides.Right & Sides.Top)))
                            g.FillRoundRectangle(bg.CreateBrush(), borderrect.Location, borderrect.Size, border.Sides, border.CornerRadius);
                        else
                            g.FillRoundRectangle(bg.CreateBrush(), borderrect, border.CornerRadius);
                    }
                    else
                        g.FillRectangle(bg.CreateBrush(), borderrect);
                }

                if (cliptosize)
                {
                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Add(TraceLevel.Debug, "RENDER", "Setting Clipping rect to " + borderrect.ToString());

                    g.SaveGraphicsState();
                    //Again - special case of clipping to inside the round rect border
                    if (hasborder && border.CornerRadius != PDFUnit.Zero)
                        g.SetClipRect(borderrect, border.Sides, border.CornerRadius);
                    else
                        g.SetClipRect(borderrect);
                }

                PDFRect contentbounds = borderrect;
                if (arrange.Padding.IsEmpty == false)
                    contentbounds = borderrect.Inset(arrange.Padding);

                context.Offset = contentbounds.Location;
                context.Space = contentbounds.Size;
                if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                    context.TraceLog.Add(TraceLevel.Debug,"RENDER", "Render contents of '" + this.UniqueID + "' with bounds [" + context.Offset.ToString() + "," + context.Space.ToString() + "]");

                oref = this.DoRenderToPDF(context, full, g, writer);

                if (cliptosize)
                {
                    g.RestoreGraphicsState();
                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Add(TraceLevel.Debug, "RENDER", "Cleared clipping rect");

                    
                }

                if (hasborder)
                {
                    if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                        context.TraceLog.Add(TraceLevel.Debug, "RENDER", "Drawing border rect of " + borderrect.ToString());

                    
                    PDFPen pen = border.CreatePen();
                    PDFUnit rad = border.CornerRadius;
                    if (rad == PDFUnit.Empty)
                        g.DrawRectangle(pen, borderrect.Location, borderrect.Size, border.Sides);
                    else
                        g.DrawRoundRectangle(pen, borderrect.Location, borderrect.Size, border.Sides, rad);
                }

            }
            context.Offset = orig.Location;
            context.Space = orig.Size;

            

            return oref;
        }
        

        /// <summary>
        /// Inheritors should override this method to perform their own rendering
        /// </summary>
        /// <param name="context">The current render context</param>
        /// <param name="writer">The PDFWriter that will contain the output</param>
        /// <returns>An optional indirect PDFObject reference</returns>
        protected virtual PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            return null;
        }

        #endregion


        //
        // private and protected methods
        //

        #region protected PDFGraphics CreateGraphics(PDFStyleStack styles) + 1 overload

        /// <summary>
        /// Creates a new PDFGraphics context within which drawing to the PDF Surface can take place
        /// </summary>
        /// <param name="styles">The styles of the new graphics context</param>
        /// <returns>A newly instantiated graphics context</returns>
        public PDFGraphics CreateGraphics(PDFStyleStack styles, PDFContextBase context)
        {
            return this.CreateGraphics(null, styles, context);
        }


        /// <summary>
        /// Creates a new PDFGraphics context within which drawing to the PDF Surface can take place
        /// </summary>
        /// <param name="writer">The writer used to write graphical instructions to</param>
        /// <param name="styles">The styles of the new graphics context</param>
        /// <returns>A newly instantiated graphics context</returns>
        public virtual PDFGraphics CreateGraphics(PDFWriter writer, PDFStyleStack styles, PDFContextBase context)
        {
            if (this.Parent == null)
                throw RecordAndRaise.NullReference(Errors.InvalidCallToGetGraphicsForStructure);
            else
                return this.Parent.CreateGraphics(writer, styles, context);
        }

        #endregion

        #region protected virtual PDFStyle GetAppliedStyle() + public virtual PDFStyle GetAppliedStyle(PDFComponent forComponent)
        /// <summary>
        /// Gets the Defined Style for this Component (Style Items that are to be applied directly)
        /// </summary>
        /// <returns></returns>
        public virtual PDFStyle GetAppliedStyle()
        {
            if (null == _appliedStyle)
            {
                PDFStyle s = this.GetAppliedStyle(this, GetBaseStyle());

                if (this is IPDFStyledComponent)
                {
                    IPDFStyledComponent styledComponent = this as IPDFStyledComponent;
                    if (styledComponent.HasStyle)
                        styledComponent.Style.MergeInto(s);
                }
                _appliedStyle = s;
            }
            return _appliedStyle;
        }

        /// <summary>
        /// Gets the base style for this component. Any styles that would be applied if no explict user styles override. Default is empty
        /// </summary>
        /// <returns></returns>
        protected virtual PDFStyle GetBaseStyle()
        {
            return new PDFStyle();
        }

        /// <summary>
        /// Traverses the hierarchy of Components to retrieve the defined style for the Component provided
        /// </summary>
        /// <param name="forComponent">The Component to get the style for</param>
        /// <param name="baseStyle"></param>
        /// <returns>The merged style</returns>
        public virtual PDFStyle GetAppliedStyle(PDFComponent forComponent, PDFStyle baseStyle)
        {
            if (this.Parent != null)
                return this.Parent.GetAppliedStyle(forComponent, baseStyle);
            else
                return baseStyle;
        }

        #endregion


        #region protected virtual bool ClipGraphicsToSize(PDFContextStyleBase context)

        /// <summary>
        /// Returns true of false if the current rendering should be clipped to the bounds, or allowed to overflow
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool ClipGraphicsToSize(PDFContextStyleBase context)
        {
            return false;
        }

        #endregion

        #region protected virtual PDFComponent FindComponent(string id)

        /// <summary>
        /// Finds an Component in the entire document hierarchy with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual PDFComponent FindDocumentComponentById(string id)
        {
            if (this.Document != null)
                return this.Document.FindAComponentById(id);
            else
                throw RecordAndRaise.Operation(Errors.CannotUseFindForComponentNotInDocumentHeirarchy);
        }

        #endregion

        #region protected virtual IPDFResourceContainer GetResourceContainer()

        /// <summary>
        /// Searches this components parent hirerachy for the IPDFResourceContainer (usually the page).
        /// </summary>
        /// <returns>The found container or null</returns>
        protected virtual IPDFResourceContainer GetResourceContainer()
        {
            PDFComponent comp = this.Parent;
            while(null != comp)
            {
                if (comp is IPDFResourceContainer)
                    return (IPDFResourceContainer)comp;
                else
                    comp = comp.Parent;
            }

            return null;
        }

        #endregion

        public string GetFullPath()
        {
            if (!string.IsNullOrEmpty(this.LoadedSource))
                return this.LoadedSource;
            else if (null != this.Parent)
                return this.Parent.GetFullPath();
            else
                return string.Empty;
        }

         

        #region public virtual string MapPath(string path)

        /// <summary>
        /// Returns a full path to a resource based upon the 
        /// provided path and the root path of the document. If the 
        /// path cannot be determined returns the original path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string MapPath(string path)
        {
            if (System.Web.VirtualPathUtility.IsAbsolute(path))
                return path;

            if (System.IO.Path.IsPathRooted(path))
                return path;

            if (!string.IsNullOrEmpty(this.LoadedSource))
            {
                bool isfile;
                path = this.MapPath(path, out isfile);
                return path;
            }
            else if (null == this.Parent)
                return path;
            else
                return this.Parent.MapPath(path);
        }

        /// <summary>
        /// Checks the source string and convers its to a full reference if possible. Returns true for isfile if the 
        /// resultant string is a local file reference.
        /// </summary>
        /// <param name="source">The orignal reference</param>
        /// <param name="isfile">Set to true if the result is now a local file</param>
        /// <returns>The converted full reference</returns>
        public virtual string MapPath(string source, out bool isfile)
        {
            string result;
            string path;

            if (string.IsNullOrEmpty(source))
                throw RecordAndRaise.ArgumentNull("source");

            if (Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute))
            {
                if (System.Web.VirtualPathUtility.IsAppRelative(source))
                {
                    result = System.Web.HttpContext.Current.Server.MapPath(source);
                    isfile = true;
                }
                else if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
                {
                    result = source;
                    isfile = false;
                }
                else if (!GetLocalPath(out path, out isfile))
                    throw RecordAndRaise.FileNotFound(source);
                else if (isfile)
                {
                    result = System.IO.Path.Combine(path, source.Replace('/', '\\'));
                    result = System.IO.Path.GetFullPath(result);
                }
                else
                    result = System.Web.VirtualPathUtility.Combine(path, source);

            }
            else if (System.IO.Path.IsPathRooted(source))
            {
                isfile = true;
                result = System.IO.Path.GetFullPath(source); //Normalize
            }
            else if (!GetLocalPath(out path, out isfile))
                throw RecordAndRaise.FileNotFound(source);

            else if (isfile)
            {
                result = System.IO.Path.Combine(path, source.Replace('/', '\\'));
                result = System.IO.Path.GetFullPath(result);
            }
            else
                result = System.Web.VirtualPathUtility.Combine(path, source);

            return result;
        }

        /// <summary>
        /// Gets the full path to this local document - setting isfile to true if the path is a file rather than a uri
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isfile"></param>
        /// <returns></returns>
        private bool GetLocalPath(out string path, out bool isfile)
        {
            path = this.LoadedSource;
            if (string.IsNullOrEmpty(path))
                path = this.GetRootDirectory();
            else
                path = System.IO.Path.GetDirectoryName(this.LoadedSource);

            if (string.IsNullOrEmpty(path))
            {
                isfile = false;
                return false;
            }
            else if (System.Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                isfile = false;
                return true;
            }
            else if (System.IO.Path.IsPathRooted(path))
            {
                isfile = true;
                return true;
            }
            else if (path.StartsWith("~"))
            {
                string exec = this.GetRootDirectory();
                if (path.Length > 1)
                {
                    //merge current directory with the path.
                    path = path.Substring(1);
                    if (!path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString())
                        && !exec.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                        exec += System.IO.Path.DirectorySeparatorChar.ToString();
                    path = exec + path;
                }
                else
                    path = exec;
                isfile = true;

                return true;
            }
            else
            {
                throw RecordAndRaise.FileNotFound(path);
            }
        }

        /// <summary>
        /// Gets the root (working) directory for this document - 
        /// root of the web application if its a web document or the current directory for executable
        /// </summary>
        /// <returns></returns>
        protected string GetRootDirectory()
        {
            if (this.LoadType == ComponentLoadType.WebBuildProvider)
            {
                return System.Web.HttpContext.Current.Server.MapPath("/");
            }
            else
            {
                return System.IO.Directory.GetCurrentDirectory();
            }
        }

        #endregion

        #region internal virtual string GetIncrementID(PDFObjectType type)
        
        /// <summary>
        /// Returns a new ID for this object type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetIncrementID(PDFObjectType type)
        {
            if (this.Parent != null)
                return this.Parent.GetIncrementID(type);
            else
                return TempIDFactory.GetTempID(type);
        }

        #endregion

        #region protected PDFStyleItem CreateStyleItem(PDFObjectType type)

        /// <summary>
        /// Creates a new PDFStyleItem for the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected PDFStyleItem CreateStyleItem(PDFObjectType type)
        {
            return PDFStyleFactory.CreateStyleItem(type);
        }


        #endregion


        protected virtual IPDFComponent ParseComponentAtPath(string path)
        {
            if (null == this.Parent)
                throw RecordAndRaise.NullReference(Errors.ParentDocumentCannotBeNull);
            else
                return this.Parent.ParseComponentAtPath(path);
        }

        #region internal virtual void ResetChildIDs()

        internal virtual void ResetChildIDs()
        {
            //Do Nothing
        }

        #endregion


        #region public void SetArrangement(PDFComponentArrangement arrange) + GetArrangement() + ClearArrangement()

        public virtual void SetArrangement(PDFComponentArrangement arrange)
        {
            if (arrange is PDFComponentMultiArrangement)
            {
                if (_arrange == null)
                {
                    _arrange = arrange;
                    this.EnsureAllChildrenAreOnThisPage(arrange);
                }

                else if (_arrange != null && _arrange is PDFComponentMultiArrangement)
                {
                    ((PDFComponentMultiArrangement)_arrange).AppendArrangement((PDFComponentMultiArrangement)arrange);
                }
                else
                    throw RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, typeof(PDFComponentArrangement), typeof(PDFComponentMultiArrangement));
            }
            else
                _arrange = arrange;
        }

        public virtual PDFComponentArrangement GetArrangement(int pageindex)
        {
            if (_arrange is PDFComponentMultiArrangement)
            {
                return ((PDFComponentMultiArrangement)_arrange).GetArrangementForPage(pageindex);
            }
            else if (null != _arrange && _arrange.PageIndex == pageindex)
                return _arrange;
            else
                return null;
        }

        public virtual PDFComponentArrangement GetArrangement()
        {
            return _arrange;
        }

        //public virtual PDFComponentArrangement Get

        internal virtual void ClearArrangement()
        {
            _arrange = null;
        }


        protected virtual void EnsureAllChildrenAreOnThisPage(PDFComponentArrangement arrange)
        {
            
        }

        protected internal void EnsureCorrectPage(PDFComponentArrangement arrange)
        {
            if (null != this._arrange)
            {
                if (this._arrange.PageIndex < arrange.PageIndex)
                    this._arrange.PageIndex = arrange.PageIndex;
            }
            this.EnsureAllChildrenAreOnThisPage(arrange);
        }

        #endregion

        #region public override string ToString()

        public override string ToString()
        {
            return String.Format("{0}:{1}",this.Type,string.IsNullOrEmpty(this.ID)?"[NO_ID]":this.ID);
        }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        ~PDFComponent()
        {
            this.Dispose(false);
        }
    }
}
