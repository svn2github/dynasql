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

namespace Scryber
{
    internal class PDFAnnotationCollection : IArtefactCollection
    {
        private List<PDFAnnotationEntry> _annots = new List<PDFAnnotationEntry>();
        private string _name;

        string IArtefactCollection.CollectionName
        {
            get { return _name; }
        }

        internal PDFAnnotationCollection(string catalogname)
        {
            _name = catalogname;
        }

        object IArtefactCollection.Register(IArtefactEntry catalogobject)
        {
            if (null == catalogobject)
                throw RecordAndRaise.ArgumentNull("catalogobject");

            if (!(catalogobject is PDFAnnotationEntry))
                throw RecordAndRaise.InvalidCast(Errors.CannotConvertObjectToType, catalogobject.GetType(), typeof(PDFAnnotationEntry));
            
            _annots.Add((PDFAnnotationEntry)catalogobject);
            return catalogobject;
        }

        void IArtefactCollection.Close(object registration)
        {
           
        }

        public PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef annot = writer.BeginObject();
            List<PDFObjectRef> entries = new List<PDFObjectRef>();
            //TODO:Render annotations
            foreach (PDFAnnotationEntry entry in this._annots)
            {
                PDFObjectRef oref = entry.RenderToPDF(context, writer);
                if (oref != null)
                    entries.Add(oref);
            }
            writer.WriteArrayRefEntries(entries.ToArray());
            writer.EndObject();
            return annot;
        }
    }
}
