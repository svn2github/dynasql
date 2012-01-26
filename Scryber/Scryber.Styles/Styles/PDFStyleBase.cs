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

namespace Scryber.Styles
{
    public abstract class PDFStyleBase : PDFObject, IPDFBindableComponent
    {

        #region public event PDFDataBindEventHandler DataBinding + OnDataBinding(args)

        public event PDFDataBindEventHandler DataBinding;

        protected virtual void OnDataBinding(PDFDataBindEventArgs args)
        {
            if (this.DataBinding != null)
                this.DataBinding(this, args);
        }

        #endregion

        #region public event EventHandler DataBound + OnDataBound(args)

        public event PDFDataBindEventHandler DataBound;

        protected virtual void OnDataBound(PDFDataBindEventArgs args)
        {
            if (this.DataBound != null)
                this.DataBound(this, args);
        }

        #endregion

        public PDFStyleBase(PDFObjectType type)
            : base(type)
        {
        }

        public abstract void MergeInto(PDFStyle style, IPDFComponent Component, ComponentState state);

        public abstract PDFStyle MatchClass(string classname);
        
        public void DataBind(PDFDataContext context)
        {
            PDFDataBindEventArgs args = new PDFDataBindEventArgs(context);
            this.OnDataBinding(args);
            this.DoDataBind(context, true);
            this.OnDataBound(args);
        }

        protected virtual void DoDataBind(PDFDataContext context, bool includechildren)
        {
            
        }


        
    }
}
