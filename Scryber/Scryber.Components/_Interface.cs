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
using System.Drawing;
using Scryber.Drawing;
using Scryber.Components;

namespace Scryber
{

    //
    // PDFComponent interfaces and related contracts
    //

    

    public interface IPDFTemplateParser : IPDFComponent
    {
        IPDFComponent ParseTemplate(IPDFRemoteComponent comp, System.IO.TextReader reader);
    }

    public interface IPDFComponentList : IEnumerable<IPDFComponent>
    {
        void Insert(int index, IPDFComponent component);
    }

    #region public interface IPDFContainerComponent : IPDFComponent

    /// <summary>
    /// Interface that identifies a Page Component as a container for multiple child Components
    /// </summary>
    public interface IPDFContainerComponent : IPDFComponent
    {
        bool HasContent { get;}
        /// <summary>
        /// Gets a list of IPDFComponents that are children of this Component
        /// </summary>
        PDFComponentList Content { get;}
    }

    #endregion

    #region public interface IPDFTextComponent : IPDFComponent

    /// <summary>
    /// Interface for any text based Component (has visual content displayed as text on a page) 
    /// </summary>
    public interface IPDFTextComponent : IPDFComponent
    {
        Text.PDFTextBlock TextBlock { get; set; }

        Text.PDFTextReader CreateReader();

        void ResetTextBlock();
    }

    #endregion

    #region public interface IPDFGraphicPathComponent : IPDFComponent

    /// <summary>
    /// Interface for any Component that is displayed as a shape or path
    /// </summary>
    public interface IPDFGraphicPathComponent : IPDFComponent
    {
        PDFGraphicsPath CreatePath(PDFSize avail, Styles.PDFStyle fullstyle);

        /// <summary>
        /// Gets or sets the path generated with the CreatePath method
        /// </summary>
        Drawing.PDFGraphicsPath Path { get; set; }
    }

    #endregion

    #region public interface IPDFImageComponent : IPDFComponent

    public interface IPDFImageComponent : IPDFComponent
    {
        /// <summary>
        /// Gets the image resource data associated with this image. 
        /// Returns null if there is no image.
        /// </summary>
        /// <returns></returns>
        Scryber.Resources.PDFImageXObject GetImageObject();
    }

    #endregion


    #region public interface IPDFTopAndTailedComponent : IPDFContainerComponent

    /// <summary>
    /// Interface that extends the container Component to include a header and a footer
    /// </summary>
    public interface IPDFTopAndTailedComponent : IPDFContainerComponent
    {
        
        /// <summary>
        /// Gets or sets the list of Components in the header of this Component
        /// </summary>
        IPDFTemplate Header { get; set; }

        /// <summary>
        /// Gets or sets the list of Components in the footer of this Component
        /// </summary>
        IPDFTemplate Footer { get; set; }
    }

    #endregion

    #region public interface IPDFRenderComponent : IPDFComponent

    /// <summary>
    /// A PDF Component that supports rendering
    /// </summary>
    public interface IPDFRenderComponent : IPDFComponent
    {
        /// <summary>
        /// Event that is raised before the Component is rendered to the document
        /// </summary>
        event EventHandler PreRender;
        /// <summary>
        /// Event that is raised after the Component has been rendered
        /// </summary>
        event EventHandler PostRender;

        /// <summary>
        /// Method to render the Component to the PDFWriter
        /// </summary>
        /// <param name="context">The current PDFRenderContext</param>
        /// <param name="writer">The current PDFWriter</param>
        /// <returns>A reference to a PDFIndirectObject, or null</returns>
        PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer);
    }

    #endregion


    #region public interface IPDFVisualComponent : IPDFStyledComponent

    /// <summary>
    /// A PDF Visual Component that has a physical dimension and content
    /// </summary>
    public interface IPDFVisualComponent : IPDFStyledComponent
    {

        /// <summary>
        /// Gets or sets the X position of this component
        /// </summary>
        PDFUnit X { get; set; }
        
        /// <summary>
        /// Gets or sets the Y position of this component
        /// </summary>
        PDFUnit Y { get; set; }

        /// <summary>
        /// Gets or sets the explcit Width of this component
        /// </summary>
        PDFUnit Width { get; set; }

        /// <summary>
        /// Gets or sets the explicit Height of this component
        /// </summary>
        PDFUnit Height { get; set; }

        /// <summary>
        /// Gets the page that contains this Component
        /// </summary>
        PDFPage Page { get; }

        /// <summary>
        /// called when the component is loaded
        /// </summary>
        /// <param name="load"></param>
        void Load(PDFLoadContext load);

    }

    #endregion

    #region public interface IPDFNamingContainer : IPDFComponent

    /// <summary>
    /// Placeholder interface to identify instances that are included in creating a unique ID
    /// </summary>
    public interface IPDFNamingContainer : IPDFComponent
    {
    }

    #endregion

    #region public interface IPDFViewPortComponent : IPDFComponent

    /// <summary>
    /// Any Component that implements the IPDFViewPortComponent interface has it's own layout engine
    /// to arrange its child contents and return the size
    /// </summary>
    public interface IPDFViewPortComponent : IPDFComponent
    {
        IPDFLayoutEngine GetEngine(IPDFLayoutEngine parent, PDFLayoutContext context);
    }

    #endregion

    #region IPDFInvisibleContainer : IPDFContainerComponent

    /// <summary>
    /// Placeholder for a container that does not affect rendering, 
    /// but it's children are laid out directly in the engine as if the components contents
    /// were part of the parent collection
    /// </summary>
    public interface IPDFInvisibleContainer : IPDFContainerComponent
    {
    }

    #endregion

    #region IPDFPageBreak : IPDFComponent

    /// <summary>
    /// Placeholder interface for identifying a PageBreak component.
    /// </summary>
    public interface IPDFPageBreak : IPDFComponent
    {
    }

    #endregion

    //
    //Support interfaces
    //

    #region  public interface IPDFLayoutEngine
    /// <summary>
    /// Defines the interface for a layout engine. 
    /// This is the actual class that implements the layout of individual Components
    /// </summary>
    public interface IPDFLayoutEngine : IDisposable
    {
        event PDFPageRequestHandler RequestNewPage;

        PDFSize Layout(PDFSize avail, int startPageIndex);

        /// <summary>
        /// Checks this engines current component to see if it can be split.
        /// </summary>
        /// <returns></returns>
        bool CanSplitCurrentComponent();

        /// <summary>
        /// Gets the last page index 
        /// </summary>
        int LastPageIndex { get; }
    }

    #endregion

    #region public interface IPDFCacheProvider

    /// <summary>
    /// Defines the contract all CacheProviders must conform to 
    /// in order to support access to the data cache
    /// </summary>
    public interface IPDFCacheProvider
    {
        bool TryRetrieveFromCache(string type, string key, out object data);

        void AddToCache(string type, string key, object data);

        void AddToCache(string type, string key, object data, TimeSpan duration);

    }

    #endregion

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
        /// <param name="resolve"></param>
        /// <returns></returns>
        IPDFComponent Parse(string source, System.IO.Stream stream, PDFReferenceResolver resolve);
    }

    #endregion

    //
    // CodeDom Generation interfaces
    //

    //#region public interface IPDFCodeDomFactory

    ///// <summary>
    ///// A contract for the CodeDomGeneratorFactory - creates IPDFCodeDonGenerators for specific extensions
    ///// </summary>
    //public interface IPDFCodeDomFactory
    //{
    //    IPDFCodeDomGenerator CreateGenerator(string fileExtension);
    //}

    //#endregion

    //#region public interface IPDFCodeDomGenerator

    ///// <summary>
    ///// The contract a code dom generator must fulfil to create a code compile unit based upon a loaded file
    ///// </summary>
    //public interface IPDFCodeDomGenerator
    //{
    //    event PDFCodeDomErrorHandler GenerationError;

    //    System.CodeDom.CodeCompileUnit GenerateUnit(System.IO.TextReader reader, string fileNamespace, string fullpath, string filename);
    //}

    //#endregion

    //
    // Artifact interfaces
    //

    #region public interface IArtefactEntry

    /// <summary>
    /// Placeholder interface for an entry in an Artifact collection
    /// </summary>
    public interface IArtefactEntry
    {
    }

    #endregion

    #region public interface IArtefactCollection

    /// <summary>
    /// A contract for any collection of Atrifacts registered on a resource container
    /// </summary>
    public interface IArtefactCollection
    {
        string CollectionName { get; }

        object Register(IArtefactEntry catalogobject);

        void Close(object registration);

        PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer);
    }

    #endregion


}
