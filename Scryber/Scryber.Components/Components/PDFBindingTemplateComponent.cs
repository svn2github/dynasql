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
using System.Collections;

namespace Scryber.Components
{
    public abstract class PDFBindingTemplateComponent : PDFVisualComponent
    {

        private IPDFTemplate _template;

        /// <summary>
        /// Gets or sets the IPDFTemplate used to instantiate child Components
        /// </summary>
        [PDFTemplate()]
        [PDFElement("Template")]
        public virtual IPDFTemplate Template
        {
            get { return _template; }
            set { _template = value; }
        }

        public bool HasTemplate
        {
            get { return null != this.Template; }
        }

        /// <summary>
        /// The list of Components that were added to the
        /// parents Component collection when the template was bound.
        /// </summary>
        private List<IPDFComponent> _addedonbind;
        /// <summary>
        /// The parent Component the items were added to.
        /// </summary>
        private IPDFContainerComponent _toparent;
        

        protected PDFBindingTemplateComponent(PDFObjectType type)
            : base(type)
        {
        }

        

        protected override void DoDataBind(PDFDataContext context, bool includeChildren)
        {
            if (_addedonbind != null && _addedonbind.Count > 0)
                this.ClearPreviousBoundComponents(_addedonbind, _toparent);
            else if(null == _addedonbind)
                _addedonbind = new List<IPDFComponent>();

            //call the base method first
            base.DoDataBind(context, includeChildren);

            if (includeChildren)
            {
                IPDFContainerComponent container = GetContainerParent();
                DoDataBindTemplate(context, container);
            }
        }


        protected virtual void DoDataBindTemplate(PDFDataContext context, IPDFContainerComponent container)
        {
            //If we have a template and should be binding on it
            if (this.Template != null)
            {
                int oldindex = context.CurrentIndex;
                int index = container.Content.IndexOf(this);
                DoBindDataWithTemplate(container, index, this.Template, context);
                _toparent = container;
                context.CurrentIndex = oldindex;
            }
        }

        protected IPDFContainerComponent GetContainerParent()
        {
            PDFComponent ele = this;
            while (null != ele)
            {
                PDFComponent par = ele.Parent;
                if (par == null)
                    throw RecordAndRaise.ArgumentNull(Errors.TemplateComponentParentMustBeContainer);

                else if ((par is IPDFContainerComponent) == false)
                    ele = par;
                else
                    return par as IPDFContainerComponent;
            }

            //If we get this far then we haven't got a viable container to add our items to.
            throw RecordAndRaise.ArgumentNull(Errors.TemplateComponentParentMustBeContainer);
        }


        protected void DoBindDataWithTemplate(IPDFContainerComponent container, int containerposition, IPDFTemplate template, PDFDataContext context)
        {
            object data = context.DataStack.Current;

            int count = 0;
            int added = 0;
            if (data is System.Xml.XPath.XPathNodeIterator)
            {
                System.Xml.XPath.XPathNodeIterator itter = (System.Xml.XPath.XPathNodeIterator)data;
                while (itter.MoveNext())
                {
                    context.CurrentIndex = count++;
                    context.DataStack.Push(itter.Current);
                    added += InstantiateAndAddFromTemplate(context, count, added + containerposition, container, template);

                    context.DataStack.Pop();
                }
            }
            if (data is IEnumerable)
            {
                IEnumerable ienum = data as IEnumerable;

                foreach (object dataitem in ienum)
                {
                    context.CurrentIndex = count++;
                    context.DataStack.Push(dataitem);
                    added += InstantiateAndAddFromTemplate(context, count, added + containerposition, container, template);

                    context.DataStack.Pop();
                }
            }
            else if (data != null)
            {
                context.CurrentIndex = 0;
                added += this.InstantiateAndAddFromTemplate(context, count, added + containerposition, container, template);
            }
        }

        private int InstantiateAndAddFromTemplate(PDFDataContext context, int count, int index, IPDFContainerComponent container, IPDFTemplate template)
        {
            if (null == template)
                return 0;
            PDFTraceLog log = context.TraceLog;
            PDFInitContext init = new PDFInitContext(context.Items, log);
            PDFLoadContext load = new PDFLoadContext(context.Items, log);
            
            IEnumerable<IPDFComponent> created = template.Instantiate(count);
            int added = 0;
            if (created != null)
            {
                foreach (IPDFComponent ele in ((IEnumerable)created))
                {
                    InsertComponentInContainer(container, index, ele, init, load);
                    if (ele is IPDFBindableComponent)
                    {
                        ((IPDFBindableComponent)ele).DataBind(context);
                    }
                    index++;
                    added++;
                }

            }
            return added;
        }

        /// <summary>
        /// Removes any previously bound Components
        /// </summary>
        /// <param name="all"></param>
        /// <param name="container"></param>
        private void ClearPreviousBoundComponents(ICollection<IPDFComponent> all, IPDFContainerComponent container)
        {
            //dispose and clear
            foreach (PDFComponent ele in all)
            {
                container.Content.Remove(ele);
                ele.Dispose();
            }
            all.Clear();
        }

        /// <summary>
        /// Inserts a new Component in the container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="index"></param>
        /// <param name="ele"></param>
        private void InsertComponentInContainer(IPDFContainerComponent container, int index, IPDFComponent ele, PDFInitContext init, PDFLoadContext load)
        {
            ele.Init(init);
            IPDFComponentList list = container.Content as IPDFComponentList;
            list.Insert(index, ele);
            _addedonbind.Add(ele);

            if (ele is IPDFVisualComponent)
            {
                ((IPDFVisualComponent)ele).Load(load);
            }
        }
    }
}
