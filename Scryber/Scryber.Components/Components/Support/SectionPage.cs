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
using Scryber.Resources;
using Scryber.Native;

namespace Scryber.Components.Support
{
    internal class SectionPage : IPDFResourceContainer
    {
        internal Resources.PDFResourceList Resources { get; private set; }
        internal int PageIndex { get; private set; }
        internal List<PDFComponent> Components { get; private set; }
        internal PDFSection Owner { get; private set; }

        public SectionPage(PDFSection owner, int index)
        {
            this.Resources = new Scryber.Resources.PDFResourceList(this);
            this.PageIndex = index;
            this.Components = new List<PDFComponent>();
            this.Owner = owner;
        }



        #region IPDFResourceContainer Members

        public IPDFDocument Document { get { return this.Owner.Document; } }

        public PDFName RegisterFont(Scryber.Drawing.PDFFont font)
        {
            string path = null;

            PDFFontResource defn = this.Owner.Document.GetFontResource(font, true);
            defn.RegisterUse(this.Resources, this.Owner);
            return defn.Name;
        }

        public PDFName Register(Scryber.Resources.PDFResource rsrc)
        {
            if (null == rsrc.Name || string.IsNullOrEmpty(rsrc.Name.Value))
            {
                string name = this.Owner.Document.GetIncrementID(rsrc.Type);
                rsrc.Name = (PDFName)name;
            }
            rsrc.RegisterUse(this.Resources, this.Owner);
            return rsrc.Name;
        }

        public string MapPath(string source)
        {
            return this.Owner.MapPath(source);
        }

        #endregion
    }

    internal class SectionPageList : List<SectionPage>
    {
        private PDFSection _owner;

        internal PDFSection Owner
        {
            get { return _owner; }
        }

        private int _startindex;

        internal int StartIndex
        {
            get { return _startindex; }
        }

        internal SectionPage Last
        {
            get
            {
                if (this.Count == 0)
                    return null;
                else
                    return this[this.Count - 1];
            }
        }

        internal SectionPageList(PDFSection section, int startIndex)
        {
            if (null == section)
                throw RecordAndRaise.ArgumentNull("section");
            if (startIndex < 0)
                throw RecordAndRaise.ArgumentOutOfRange("startIndex");

            this._owner = section;
            this._startindex = startIndex;
        }

        internal int BeginNewPage()
        {
            int index = GetNextPageIndex();
            SectionPage spage = new SectionPage(this.Owner, index);
            this.Add(spage);

            return index;
        }

        private int GetNextPageIndex()
        {
            SectionPage last = this.Last;
            if (null == last)
                return this.StartIndex;
            else
                return last.PageIndex + 1;
        }
    }
}
