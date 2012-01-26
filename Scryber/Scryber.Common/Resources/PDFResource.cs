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

namespace Scryber.Resources
{
    public abstract class PDFResource : PDFObject, IDisposable
    {

        #region public static string XObjectResourceType {get;}

        public static string XObjectResourceType
        {
            get { return "XObject"; }
        }

        #endregion

        #region public static string FontDefnResourceType {get;}

        public static string FontDefnResourceType
        {
            get { return "Font"; }
        }

        #endregion

        public static string GSStateResourceType
        {
            get { return "ExtGState"; }
        }

        public static string ProcSetResourceType
        {
            get { return "ProcSet"; }
        }

        public static string PatternResourceType
        {
            get { return "Pattern"; }
        }


        //
        // .ctor
        //

        protected PDFResource(PDFObjectType type)
            : base(type)
        {
        }



        public abstract string ResourceType { get; }

        public abstract string ResourceKey { get; }


        #region public PDFName Name {get;set;}

        private PDFName _name;

        public PDFName Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public bool Registered {get;}

        private bool _registered;

        public bool Registered
        {
            get { return _registered; }
        }

        #endregion

        #region public IPDFResourceContainer Container {get;set;}

        private IPDFResourceContainer _cont;

        public IPDFResourceContainer Container
        {
            get { return _cont; }
            set { _cont = value; }
        }

        #endregion

        #region protected PDFObjectRef RenderReference {get;set;}

        private PDFObjectRef _oref = null;

        protected PDFObjectRef RenderReference
        {
            get { return _oref; }
            set { _oref = value; }
        }

        #endregion


        protected abstract PDFObjectRef DoRenderToPDF(PDFContextBase context, PDFWriter writer);
        

        public PDFObjectRef EnsureRendered(PDFContextBase context, PDFWriter writer)
        {

            if (this.Registered)
            {
                if (null == this.RenderReference)
                {
                    this.RenderReference = this.DoRenderToPDF(context, writer);
                }
                return this.RenderReference;
            }
            else
                return null;
        }


        public virtual bool Equals(string resourcetype, string name)
        {
            return this.ResourceType == resourcetype && this.Name.Value == name;
        }

        public void RegisterUse(PDFResourceList resourcelist, IPDFComponent Component)
        {
            if (resourcelist != null)
            {
                resourcelist.EnsureInList(this);
                this.Container = resourcelist.Container;
                this._registered = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~PDFResource()
        {
            this.Dispose(false);
        }
    }
}
