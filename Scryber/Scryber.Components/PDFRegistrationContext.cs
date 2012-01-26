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
using Scryber.Styles;

namespace Scryber
{
    public class PDFRegistrationContext : PDFContextStyleBase
    {
        private PDFDocument _doc;
        private PDFPage _pg;

        public PDFDocument Document
        {
            get { return _doc; }
            set { _doc = value; }
        }

        public PDFPage Page
        {
            get { return _pg; }
            set { _pg = value; }
        }

        internal PDFRegistrationContext(PDFDocument doc, PDFStyle style, PDFItemCollection items, PDFTraceLog log)
            : this(doc, null, new PDFStyleStack(style), items, log)
        {
        }

        public PDFRegistrationContext(PDFDocument doc, PDFPage page, PDFStyleStack styles, PDFItemCollection items, PDFTraceLog log)
            : base(styles, items, log)
        {
            if (null == doc)
                throw RecordAndRaise.ArgumentNull("doc");
            if (null == styles)
                throw RecordAndRaise.ArgumentNull("styles");

            this._doc = doc;
            this._pg = page;
        }
    }
}
