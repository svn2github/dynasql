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

namespace Scryber.Generation
{
    public class PDFGeneratorSettings
    {
        private Type _templateinstanceType;
        private Type _tempateGenType;
        private Type _textLiteralType;
        private PDFTraceLog _log;
        private ConformanceMode _conformance;
        private ComponentLoadType _loadtype;

        private PDFReferenceResolver _resolver;

        private string _extension;


        public string FileExtension
        {
            get { return _extension; }
        }

        public Type TemplateInstanceType
        {
            get { return _templateinstanceType; }
        }

        public Type TempateGeneratorType
        {
            get { return _tempateGenType; }
        }

        public Type TextLiteralType
        {
            get { return _textLiteralType; }
        }

        public ConformanceMode ConformanceMode
        {
            get { return _conformance; }
        }

        public PDFReferenceResolver Resolver
        {
            get { return _resolver; }
        }

        public ComponentLoadType LoadType
        {
            get { return _loadtype; }
        }

        public PDFTraceLog Log
        {
            get { return _log; }
        }

        public PDFGeneratorSettings(Type literaltype, Type templategenerator, Type templateinstance, 
                                PDFReferenceResolver resolver, ConformanceMode conformance, ComponentLoadType loadtype,
                                PDFTraceLog log)
        {
            this._textLiteralType = literaltype;
            this._tempateGenType = templategenerator;
            this._templateinstanceType = templateinstance;
            this._resolver = resolver;
            this._conformance = conformance;
            this._loadtype = loadtype;
            this._log = log;
        }

    }

}
