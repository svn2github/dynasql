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
using Scryber.Generation;

namespace Scryber
{

    #region public interface IPDFParser

    /// <summary>
    /// A parser that can read a stream to generate a PDFComponent
    /// </summary>
    public interface IPDFParser
    {
        /// <summary>
        /// Parses the specified stream using the resolver to load any referecned files and returns the PDFComponent representation
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IPDFComponent Parse(string source, System.IO.Stream stream);
    }

    #endregion

    //
    // CodeDom Generation interfaces
    //

    #region public interface IPDFCodeDomFactory

    /// <summary>
    /// A contract for the CodeDomGeneratorFactory - creates IPDFCodeDomGenerators for specific extensions
    /// </summary>
    public interface IPDFCodeDomFactory
    {
        IPDFCodeDomGenerator CreateGenerator(PDFGeneratorSettings settings);
    }

    #endregion

    #region public interface IPDFCodeDomGenerator

    /// <summary>
    /// The contract a code dom generator must fulfil to create a code compile unit based upon a loaded file
    /// </summary>
    public interface IPDFCodeDomGenerator
    {
        event PDFCodeDomErrorHandler GenerationError;

        System.CodeDom.CodeCompileUnit GenerateUnit(System.IO.TextReader reader, string fileNamespace, string fullpath, string filename);
    }

    #endregion

    #region public interface IPDFCodeDomError

    /// <summary>
    /// Data interface that the code dom generator can call back with the PDFCodeDomErrorHandler event.
    /// </summary>
    public interface IPDFCodeDomError
    {
        int Level { get; }
        string Message { get; }
        int Line { get; }
        int Position { get; }
        CodeDomErrorType ErrorType { get; }
    }

    #endregion

    #region IPDFLoadableComponent

    /// <summary>
    /// Defines a component that can be loaded from a specific source. 
    /// This can then be identified where it was loaded from and how it was loaded
    /// </summary>
    public interface IPDFLoadableComponent
    {
        string LoadedSource { get; set; }

        ComponentLoadType LoadType { get; set; }
    }

    #endregion

    public interface IPDFTemplateGenerator
    {
        void InitTemplate(IPDFComponent owner, string xmlContent, System.Xml.XmlNamespaceManager namespaces);
    }

    public interface IPDFTextLiteral : IPDFComponent
    {
        string Text { get; set; }
        TextFormat Format { get; set; }
    }
}
