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
using System.Linq;
using System.Text;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Resources
{
    public class PDFFontResource : PDFResource, IEquatable<PDFFontResource>
    {
        private string _fontName;
        private PDFFontDefinition _defn;

        public string FontName
        {
            get { return _fontName; }
        }

        public PDFFontDefinition Definition
        {
            get { return _defn; }
        }

        public override string ResourceType
        {
            get { return PDFResource.FontDefnResourceType; }
        }

        public override string ResourceKey
        {
            get { return this.Definition.FullName; }
        }

        private PDFFontResource(PDFFontDefinition defn, string resourceName)
            : base(PDFObjectTypes.FontResource)
        {
            if (null == defn)
                throw new ArgumentNullException("defn");

            _defn = defn;
            _fontName = defn.FullName;
            this.Name = (Native.PDFName)resourceName;
        }

        protected override PDFObjectRef DoRenderToPDF(PDFContextBase context, PDFWriter writer)
        {
            return this.Definition.RenderFont(this.Name.Value, context, writer);
        }


        public bool Equals(PDFFontResource other)
        {
            return this.Name == other.Name;
        }

        public bool Equals(PDFFont font)
        {
            return this.Definition.FullName == font.FullName;
        }

        public override bool Equals(string resourcetype, string name)
        {
            return String.Equals(this.ResourceType, resourcetype) && String.Equals(this.ResourceKey, name);
        }

        public static PDFFontResource Load(PDFFontDefinition defn, string resourceName)
        {
            if (null == defn)
                throw new ArgumentNullException("defn");

            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            return new PDFFontResource(defn, resourceName);
        }

         
    }
}
