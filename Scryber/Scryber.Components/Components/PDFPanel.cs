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

namespace Scryber.Components
{
    [PDFParsableComponent("Panel")]
    public class PDFPanel : PDFVisualComponent, IPDFViewPortComponent
    {

        public PDFPanel()
            : this(PDFObjectTypes.Panel)
        {
        }

        public PDFPanel(PDFObjectType type)
            : base(type)
        {

        }

        #region public PDFVisualComponentList Contents {get;}

        private PDFVisualComponentList _content;

        /// <summary>
        /// Gets the content collection of page Components in this panel
        /// </summary>
        [PDFArray(typeof(PDFVisualComponent))]
        [PDFElement("Content")]
        public PDFVisualComponentList Contents
        {
            get
            {
                if (null == _content)
                {
                    _content = new PDFVisualComponentList(this.InnerContent);
                }
                return _content;
            }
        }

        #endregion


        public override void SetArrangement(PDFComponentArrangement arrange)
        {
            base.SetArrangement(arrange);
        }

        protected override Styles.PDFStyle GetBaseStyle()
        {
            Styles.PDFStyle style = base.GetBaseStyle();
            style.Position.LayoutMode = LayoutMode.Block;
            return style;
        }

        #region IPDFViewPortComponent Members

        IPDFLayoutEngine IPDFViewPortComponent.GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return this.CreateLayoutEngine(parent, context);
        }

        protected virtual IPDFLayoutEngine CreateLayoutEngine(IPDFLayoutEngine parent, PDFLayoutContext context)
        {
            return new Support.ContainerLayoutEngine(this, parent, context);
        }

        #endregion
    }
}
