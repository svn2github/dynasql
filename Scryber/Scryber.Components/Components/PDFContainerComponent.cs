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
using Scryber.Styles;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Components
{
    public abstract class PDFContainerComponent : PDFComponent, IPDFContainerComponent, IPDFRenderComponent
    {
        
        //
        // public properties
        //

        #region protected PDFVisualComponentList Content {get; set;} + protected virtual PDFComponentList CreateList() + public bool HasContent {get;}

        private PDFComponentList _children = null;

        /// <summary>
        /// Interface implemented property to contain all the Page Components that this item contains.
        /// Inheritors can make this list publicly accessible.
        /// </summary>
        protected PDFComponentList InnerContent
        {
            get
            {
                if (this._children == null)
                    this._children = this.CreateList();
                return this._children;
            }
            set
            {
                this._children = value;
            }
        }

        protected virtual PDFComponentList CreateList()
        {
            return new PDFComponentList(this, this.Type);
        }

        /// <summary>
        /// Interface implemented property that identifies whether this page Component contains other page Components.
        /// </summary>
        public bool HasContent
        {
            get { return this._children != null && this._children.Count > 0; }
        }

        #endregion

        #region PDFComponentList IPDFContainerComponent.Content {get;set;}

        /// <summary>
        /// Interface implementation that allows all content to be accessed via the IContainerComponent interface
        /// </summary>
        PDFComponentList IPDFContainerComponent.Content
        {
            get { return this.InnerContent; }
        }

        #endregion

        
        //
        //.ctor
        //

        #region .ctor(PDFObjectType)

        /// <summary>
        /// Creates a new instance of the PDFContainerComponent
        /// </summary>
        /// <param name="type"></param>
        public PDFContainerComponent(PDFObjectType type)
            : base(type)
        {
        }

        #endregion

        //
        // overrides + supporting methods
        //

        #region protected override void DoInit() + protected virtual void DoInitChildren()

        /// <summary>
        /// Performs the base initialization and then calls DoInitChildren to ensure that each of its children are initialized.
        /// </summary>
        protected override void DoInit(PDFInitContext context)
        {
            base.DoInit(context);
            this.DoInitChildren(context);
        }

        /// <summary>
        /// Initializes all children in the document
        /// </summary>
        protected virtual void DoInitChildren(PDFInitContext context)
        {
            if (this.HasContent)
            {
                foreach (PDFComponent ele in this.InnerContent)
                {
                    ele.Init(context);
                }
            }
        }

        #endregion

        #region protected override void DoLoad(PDFLoadContext context) + DoLoadChildren

        protected override void DoLoad(PDFLoadContext context)
        {
            base.DoLoad(context);
            this.DoLoadChildren(context);
        }

        protected virtual void DoLoadChildren(PDFLoadContext context)
        {
            if (this.HasContent)
            {
                foreach (PDFComponent ele in this.InnerContent)
                {
                    ele.Load(context);
                }
            }
        }

        #endregion

        #region protected override void DoDataBind(includeChildren) + protected virtual void DoDataBindChildren()

        /// <summary>
        /// Overrides the default implementation to optionally databind children
        /// </summary>
        /// <param name="includeChildren">Flag to identify if children should be databound too</param>
        protected override void DoDataBind(PDFDataContext context, bool includeChildren)
        {
            base.DoDataBind(context, includeChildren);

            if(includeChildren)
                this.DoDataBindChildren(context);
        }

        /// <summary>
        /// Databinds all the children in the container
        /// </summary>
        protected virtual void DoDataBindChildren(PDFDataContext context)
        {
            if (this.HasContent)
            {
                this.InnerContent.DataBind(context);
            }
        }

        #endregion

        protected override void DoRegisterArtefacts(PDFRegistrationContext context, PDFStyle fullstyle)
        {
            base.DoRegisterArtefacts(context, fullstyle);
            this.DoRegisterChildArtefacts(context);
        }

        protected virtual void DoRegisterChildArtefacts(PDFRegistrationContext context)
        {
            if (this.HasContent)
                this.InnerContent.RegisterArtefacts(context);
        }

        #region protected override PDFObjectRef DoRenderToPDF(context, writer) + protected virtual PDFObjectRef DoRenderChildrenToPDF(context, writer)

        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            PDFObjectRef oref;


            oref = base.DoRenderToPDF(context, fullstyle, graphics, writer);
            oref = this.DoRenderChildrenToPDF(context, fullstyle, graphics, writer);

            
            return oref;
        }

        /// <summary>
        /// Base implementation calls RenderToPDF on all the children if there are any
        /// </summary>
        /// <param name="context">The current PDFRenderContext</param>
        /// <param name="writer">The current PDFWriter</param>
        /// <returns>The PDFObjectRef returned from the last child</returns>
        protected virtual PDFObjectRef DoRenderChildrenToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            PDFObjectRef oref = null;

            if (this is IPDFTopAndTailedComponent)
            {
                IPDFTopAndTailedComponent tte = (IPDFTopAndTailedComponent)this;
                if (null != tte.Header)
                {
                    //TODO:Render the Header
                }
                if (null != tte.Footer)
                {
                    //TODO:Render the footer
                }
            }
            IEnumerable<PDFComponent> all = this.GetChildrenToRender(context);

            if (null != all)
            {
                HorizontalAlignment halign = PDFStyleConst.DefaultHorizontalAlign;
                VerticalAlignment valign = PDFStyleConst.DefaultVerticalAlign;
                PDFPositionStyle pos;

                if (fullstyle.TryGetPosition(out pos))
                {
                    halign = pos.HAlign;
                    valign = pos.VAlign;
                }

                PDFUnit voffset = PDFUnit.Zero;
                //if we are aligned from the bottom up, measure the total height and then 
                if (valign == VerticalAlignment.Bottom)
                    voffset = CalculateTopOffset(context);
                else if (valign == VerticalAlignment.Middle)
                    voffset = CalculateMiddleOffset(context);
                //retain the current offsets and size
                PDFPoint origoffset = context.Offset.Clone();
                PDFSize origsize = context.Space.Clone();
                
                

                //remember the position
                foreach (PDFComponent ele in all)
                {
                    PDFPoint offset = origoffset.Clone();
                    offset.Y += voffset;
                    context.Offset = offset;

                    PDFComponentArrangement arrange = ele.GetArrangement(context.PageIndex);
                    if (arrange != null)
                    {
                        switch (halign)
                        {
                            case HorizontalAlignment.Right:
                                offset.X += context.Space.Width - arrange.Bounds.Width;
                                break;
                            case HorizontalAlignment.Center:
                                offset.X += (context.Space.Width - arrange.Bounds.Width) / 2;
                                break;
                            case HorizontalAlignment.Justified:
                                break;
                            case HorizontalAlignment.Left:
                            default:
                                break;
                        }
                        context.Space = new PDFSize(arrange.Bounds.Width, arrange.Bounds.Height);
                        context.Offset = offset;

                        oref = ele.RenderToPDF(context, writer);
                        context.Offset = origoffset.Clone();
                        context.Space = origsize.Clone();
                    }
                }

            }

            return oref;
        }

        protected virtual IEnumerable<PDFComponent> GetChildrenToRender(PDFRenderContext context)
        {
            return this.InnerContent;
        }

        private PDFUnit CalculateMiddleOffset(PDFRenderContext context)
        {
            PDFUnit maxy = PDFUnit.Zero;
            PDFUnit voffset = PDFUnit.Zero;
            maxy = GetContentHeight(context);

            if (maxy < context.Space.Height)
            {
                PDFUnit mid = maxy / 2;
                mid = (context.Space.Height / 2) - mid;
                voffset = mid;
            }
            return voffset;
        }

       

        private PDFUnit CalculateTopOffset(PDFRenderContext context)
        {
            PDFUnit maxx = GetContentHeight(context);
            PDFUnit voffset = PDFUnit.Zero;

            if (maxx < context.Space.Height)
                voffset = context.Space.Height - maxx;
            return voffset;
        }

        private PDFUnit GetContentHeight(PDFRenderContext context)
        {
            PDFUnit maxy = PDFUnit.Zero;
            foreach (PDFComponent ele in this.InnerContent)
            {
                PDFComponentArrangement arrange = ele.GetArrangement(context.PageIndex);
                if (null != arrange)
                {
                    if (arrange.PositionMode == PositionMode.Flow)
                    {
                        if ((arrange.Bounds.Y + arrange.Bounds.Height) > maxy)
                            maxy = arrange.Bounds.Y + arrange.Bounds.Height;
                    }
                }
            }
            return maxy;
        }

        #endregion


        #region internal override void ResetChildIDs()

        /// <summary>
        /// Called when the parent has changed so child ID's can be reset where nescessary
        /// </summary>
        internal override void ResetChildIDs()
        {
            base.ResetChildIDs();
        }

        #endregion

        #region protected override void ChildIDHasChanged(PDFComponent child)

        /// <summary>
        /// Called when the ID of a child has changed.
        /// </summary>
        /// <param name="child">The child whose id has changed</param>
        protected override void ChildIDHasChanged(PDFComponent child)
        {
            base.ChildIDHasChanged(child);
            this.InnerContent.ChangeChildID(child);
        }

        #endregion

        protected override void EnsureAllChildrenAreOnThisPage(PDFComponentArrangement arrange)
        {
            foreach (PDFComponent child in this.InnerContent)
            {
                child.EnsureCorrectPage(arrange);
            }
        }

        #region protected override void Dispose(bool disposing) + protected virtual void DisposeChildren(disposing)

        /// <summary>
        /// Overrides the default implementation to dispose of the children too
        /// </summary>
        /// <param name="disposing">flag to identify if this method has been called from the Dispose method or finalize</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
                this.DisposeChildren(disposing);

            base.Dispose(disposing);
        }

        protected virtual void DisposeChildren(bool disposing)
        {
            if (disposing && this.HasContent)
            {
                foreach (PDFComponent ele in this.InnerContent)
                {
                    ele.Dispose();
                }
            }
        }

        #endregion


    }
}
