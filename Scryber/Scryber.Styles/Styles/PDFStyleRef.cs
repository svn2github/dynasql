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
    public class PDFStyleRef : PDFStyleGroup
    {
        private string _src;

        public string Source
        {
            get { return _src; }
            set { _src = value; this.ClearInnerStyles(); }
        }

        private PDFStylesDocument _doc = null;
        public PDFStylesDocument Document
        {
            get 
            {
                return _doc;
            }
        }



        protected override PDFStyleCollection CreateInnerStyles()
        {
            if (String.IsNullOrEmpty(Source))
                return base.CreateInnerStyles();
            else
            {
                this._doc = this.LoadSourceStyles(this.Source);
                return this._doc.Styles;
            }
            
        }

        private PDFStylesDocument LoadSourceStyles(string path)
        {
            if (System.Web.HttpContext.Current == null)
                throw new NotSupportedException(String.Format(Errors.LoadingOfExternalFilesNotSupported, this.Source));
            else
            {
                object instance = System.Web.Compilation.BuildManager.CreateInstanceFromVirtualPath(this.Source, typeof(PDFStylesDocument));
                if ((instance is PDFStylesDocument) == false)
                    throw new InvalidCastException(String.Format(Errors.CanOnlyReferenceExternalStyleDefinitionsOfType, typeof(PDFStylesDocument).FullName));

                return instance as PDFStylesDocument;
            }
        }

        public PDFStyleRef()
            : this(PDFObjectTypes.StyleRef)
        {
        }

        public PDFStyleRef(PDFObjectType type)
            : base(type)
        {
        }
    }
}
