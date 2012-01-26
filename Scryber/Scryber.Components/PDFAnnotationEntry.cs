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
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber
{
    public abstract class PDFAnnotationEntry : IArtefactEntry
    {
        public abstract PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer);
    }

    /// <summary>
    /// An annotation that links to a component
    /// </summary>
    internal class PDFLinkAnnotationEntry : PDFAnnotationEntry
    {
        public PDFAction Action { get; set; }

        public PDFComponent Component { get; private set; }

        public string AlternateText { get; set; }

        public PDFLinkAnnotationEntry(PDFComponent component)
        {
            this.Component = component;
        }

        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef annotref = writer.BeginObject();
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Annot");
            writer.WriteDictionaryNameEntry("Subtype", "Link");
            if (!string.IsNullOrEmpty(this.AlternateText))
                writer.WriteDictionaryStringEntry("Contents", this.AlternateText);
            PDFRect bounds = this.GetComponentBounds(this.Component);
            if (bounds != PDFRect.Empty)
            {
                writer.BeginDictionaryEntry("Rect");
                writer.WriteArrayRealEntries(bounds.X.Value, bounds.Y.Value, bounds.X.Value + bounds.Width.Value, bounds.Y.Value + bounds.Height.Value);
                writer.EndDictionaryEntry();
            }
            writer.WriteDictionaryStringEntry("NM", this.Component.UniqueID);
            writer.BeginDictionaryEntry("Border");
            writer.WriteArrayNumberEntries(0, 0, 0);
            writer.EndDictionaryEntry();

            writer.BeginDictionaryEntry("A");
            PDFObjectRef actionref = this.Action.RenderToPDF(context, writer);
            if (null != actionref)
                writer.WriteObjectRefS(actionref);
            writer.EndDictionaryEntry();
            
            writer.EndDictionary();
            writer.EndObject();

            return annotref;
        }

        protected virtual PDFRect GetComponentBounds(PDFComponent comp)
        {
            PDFComponentArrangement arrange = comp.GetArrangement();
            if (null != arrange)
                return arrange.RenderBounds;
            else
                return PDFRect.Empty;
        }

    }

    /// <summary>
    /// An Annotation that links to a text component (so that lines are tracked individually)
    /// </summary>
    internal class PDFLinkTextAnnotationEntry : PDFLinkAnnotationEntry
    {

        public PDFLinkTextAnnotationEntry(PDFComponent textcomp, int lineindex)
            : base(textcomp)
        {
            if (!(textcomp is IPDFTextComponent))
                throw RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, textcomp.GetType(), typeof(IPDFTextComponent));
        }
    }
}
