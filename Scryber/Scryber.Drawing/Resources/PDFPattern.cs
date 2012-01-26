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
using Scryber.Native;
using Scryber.Drawing;

namespace Scryber.Resources
{
    /// <summary>
    /// Collective base class for all tiling and shading patterns
    /// </summary>
    public abstract class PDFPattern : PDFResource
    {
        /// <summary>
        /// Gets the type of pattern this instance represents (Tiling or Shading)
        /// </summary>
        public PatternType PatternType
        { 
            get; 
            private set;
        }

        private string _key;

        /// <summary>
        /// Gets the unique resource key for this pattern
        /// </summary>
        public override string ResourceKey
        {
            get 
            {
                return _key;
            }
        }

        #region public IPDFComponent Container

        private IPDFComponent _container;

        /// <summary>
        /// Gets the container for this pattern
        /// </summary>
        public IPDFComponent Container
        {
            get { return this._container; }
            protected set { this._container = value; }
        }

        #endregion

        /// <summary>
        /// Gets the type of resource - Always PatternResourceType
        /// </summary>
        public override string ResourceType
        {
            get { return PDFResource.PatternResourceType; }
        }



        protected PDFPattern(IPDFComponent container, PatternType type, string fullkey)
            : base(PDFObjectTypes.Pattern)
        {
            this._container = container;
            this.PatternType = type;
            this._key = fullkey;

        }
    }

    public class PDFImageTilingPattern : PDFPattern, IPDFResourceContainer
    {

        public PatternPaintType PaintType 
        { 
            get;
            set;
        }

        public PatternTilingType TilingType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bounding box for the drawn area of the repeating pattern
        /// </summary>
        public PDFRect BoundingBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal distance between the start of each tile
        /// </summary>
        public PDFUnit XStep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical distance between the start of each tile
        /// </summary>
        public PDFUnit YStep
        {
            get;
            set;
        }

        private PDFImageXObject _img;
        /// <summary>
        /// Gets or sets the Image to be tiled
        /// </summary>
        public PDFImageXObject Image
        {
            get { return _img; }
            set {
                _img = value;
                if (_img != null)
                    this.Register(_img);
            }
        }


        private PDFResourceList Resources
        {
            get;
            set;
        }

        public PDFImageTilingPattern(IPDFComponent container, string key)
            : base(container, PatternType.TilingPattern, key)
        {
            this.Resources = new PDFResourceList(this);
        }

        protected override PDFObjectRef DoRenderToPDF(PDFContextBase context, PDFWriter writer)
        {

            PDFObjectRef pattern = writer.BeginObject();
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Pattern");
            writer.WriteDictionaryNumberEntry("PatternType", (int)this.PatternType);
            writer.WriteDictionaryNumberEntry("PaintType", (int)this.PaintType);
            writer.WriteDictionaryNumberEntry("TilingType", (int)this.TilingType);
            writer.BeginDictionaryEntry("BBox");


            writer.WriteArrayRealEntries(true, 
                this.BoundingBox.X.PointsValue,
                this.BoundingBox.Y.PointsValue, 
                this.BoundingBox.X.PointsValue + this.BoundingBox.Width.PointsValue,
                this.BoundingBox.Y.PointsValue + this.BoundingBox.Height.PointsValue);

            writer.EndDictionaryEntry();
            writer.WriteDictionaryRealEntry("XStep", this.XStep.PointsValue);
            writer.WriteDictionaryRealEntry("YStep", this.YStep.PointsValue);
            PDFObjectRef all = this.Resources.WriteResourceList(context, writer);
            writer.WriteDictionaryObjectRefEntry("Resources", all);
            writer.EndDictionary();
            writer.BeginStream(pattern);
            PDFSize size = new PDFSize(this.BoundingBox.Width, this.BoundingBox.Height);
            using (PDFGraphics g = PDFGraphics.Create(writer, false, this, DrawingOrigin.TopLeft, size , context))
            {
                PDFPoint pt = this.BoundingBox.Location;
                g.PaintImageRef(this.Image,size,pt);
            }
            writer.EndStream();

            writer.EndObject();

            return pattern;
            
        }

        #region IPDFResourceContainer Members

        public IPDFDocument Document
        {
            get { return this.Container.Document; }
        }

        public string MapPath(string source)
        {
            return this.Container.MapPath(source);
        }

        #endregion







        #region IPDFResourceContainer Members

        IPDFDocument IPDFResourceContainer.Document
        {
            get { return this.Container.Document; }
        }

        PDFName IPDFResourceContainer.Register(PDFResource rsrc)
        {
            return this.Register(rsrc);
        }

        PDFName Register(PDFResource rsrc)
        {
            if (null == rsrc.Name || string.IsNullOrEmpty(rsrc.Name.Value))
            {
                string name = this.Document.GetIncrementID(rsrc.Type);
                rsrc.Name = (PDFName)name;
            }
            rsrc.RegisterUse(this.Resources, this.Container);
            
            return rsrc.Name;
        }

        string IPDFResourceContainer.MapPath(string source)
        {
            return this.Container.MapPath(source);
        }

        #endregion
    }
}
