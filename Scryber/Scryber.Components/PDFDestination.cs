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
using Scryber.Components;
using Scryber.Native;

namespace Scryber
{
    /// <summary>
    /// A Specific location within a pdf document. Initialize the 
    /// </summary>
    public class PDFDestination : IArtefactEntry
    {

        public PDFComponent Component { get; private set; }

        internal OutlineFit Fit { get; private  set; }

        internal string Extension { get; set; }

        private string _fullname;

        internal string FullName
        {
            get
            {
                //If we have a value set for the full name then use it
                if (!string.IsNullOrEmpty(_fullname))
                    return _fullname;

                //otherwise calculate it from the component
                if (null == this.Component)
                    throw RecordAndRaise.NullReference(Errors.NullArgument, "this.Component");

                string name = this.Component.Name;

                if (string.IsNullOrEmpty(name))
                    name = this.Component.UniqueID;
                if (!string.IsNullOrEmpty(this.Extension))
                    return name + ":" + Extension;
                else
                    return name;
            }
            set
            {
                this._fullname = value;
            }
        }

        internal PDFDestination(PDFComponent component, OutlineFit fit)
            : this(component, fit, string.Empty)
        {
        }

        internal PDFDestination(PDFComponent component, OutlineFit fit, string extension)
        {
            if (null == component)
                throw RecordAndRaise.ArgumentNull("component");

            this.Component = component;
            this.Fit = fit;
            this.Extension = extension;
        }


        public PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            PDFComponentArrangement arrange;
            arrange = this.Component.GetArrangement();
            //PDFComponentArrangement arrange = dest.Component.GetArrangement();
            if (null == arrange)
            {
                writer.WriteNullS();
                return null;
            }

            int pgindex = arrange.PageIndex;
            PDFObjectRef oref = writer.PageRefs[pgindex];
            if (null == oref)
            {
                writer.WriteNullS();
                return null;
            }
            writer.BeginArray();

            //Write the page reference
            writer.BeginArrayEntry();
            writer.WriteObjectRef(oref);
            writer.EndArrayEntry();

            //Write the page fit method
            switch (this.Fit)
            {
                case OutlineFit.FullPage:
                    writer.BeginArrayEntry();
                    writer.WriteName("Fit");
                    writer.EndArrayEntry();
                    break;
                case OutlineFit.PageWidth:
                    writer.BeginArrayEntry();
                    writer.WriteName("FitH");
                    writer.EndArrayEntry();
                    break;
                case OutlineFit.PageHeight: 
                    writer.BeginArrayEntry();
                    writer.WriteName("FitV");
                    writer.EndArrayEntry();
                    break;
                case OutlineFit.BoundingBox:
                    writer.BeginArrayEntry();
                    writer.WriteName("XYZ");
                    writer.EndArrayEntry();

                    writer.BeginArrayEntry();
                    writer.WriteReal(arrange.RenderBounds.X.Value);
                    writer.EndArrayEntry();

                    writer.BeginArrayEntry();
                    writer.WriteReal(arrange.RenderBounds.Y.Value + arrange.RenderBounds.Height.Value);
                    writer.EndArrayEntry();

                    writer.BeginArrayEntry();
                    writer.WriteReal(1.0);
                    writer.EndArrayEntry();
                    break;
                default:
                    break;
            }

            writer.EndArray();
            return null;
        }
    }
}
