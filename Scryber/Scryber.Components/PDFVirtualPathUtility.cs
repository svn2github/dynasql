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

namespace Scryber
{
    


    public abstract class PDFVirtualPathUtility
    {
        private PDFDocument _doc;
        private string _docdirectory;

        protected PDFDocument Document
        {
            get { return _doc; }
        }

        /// <summary>
        /// Gets the absolute path to the documents containing directory (ending in a DirectorySeparatorChar)
        /// </summary>
        protected string DocumentDirectory
        {
            get
            {
                return _docdirectory;
            }
        }

        //public abstract string MapPathFromDocument(string pathrelative2document);

        //public abstract IPDFComponent LoadComponentForPath(string pathrelative2document);

        protected PDFVirtualPathUtility(PDFDocument document)
        {
            this._doc = document;
            this._docdirectory = System.IO.Path.GetDirectoryName(document.LoadedSource);
        }

        [System.Obsolete("PDFVirtualPathUtility is not implemented",true)]
        internal static PDFVirtualPathUtility GetUtility(PDFDocument document)
        {
            throw new NotImplementedException("PDFVirtualPathUtility is not implemented");
            
        }


        private class PDFParserPathUtility : PDFVirtualPathUtility
        {
            public PDFParserPathUtility(PDFDocument doc)
                : base(doc)
            {
            }
        }

        private class PDFGeneratedPathUtility : PDFVirtualPathUtility
        {
            public PDFGeneratedPathUtility(PDFDocument doc)
                : base(doc)
            {
            }
        }

        private class PDFWebPathUtility : PDFVirtualPathUtility
        {
            public PDFWebPathUtility(PDFDocument doc)
                : base(doc)
            {
            }
        }
    }
}
