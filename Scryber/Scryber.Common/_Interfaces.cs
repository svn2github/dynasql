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
using Scryber.Resources;

namespace Scryber
{
    #region public interface ITypedObject

    /// <summary>
    /// Implementors must have an explicit PDFObjectType
    /// </summary>
    public interface ITypedObject
    {
        /// <summary>
        /// Gets the name for the object type
        /// </summary>
        PDFObjectType Type { get; }

    }

    #endregion

    #region public interface IFileObject : ITypedObject

    /// <summary>
    /// Base abstract class of all native file objects (PDFBoolean, PDFNumber etc...)
    /// </summary>
    public interface IFileObject : ITypedObject
    {
        /// <summary>
        /// Writes the underlying data of the file object to the passed text writer
        /// </summary>
        /// <param name="tw">The text writer object to write data to</param>
        void WriteData(PDFWriter writer);

    }

    #endregion

    #region public interface IIndirectObject : IDisposable

    /// <summary>
    /// Defines the interface that all indirect objects must adhere to.
    /// </summary>
    public interface IIndirectObject : IDisposable
    {
        int Number { get; set; }
        int Generation { get; set; }

        long Offset { get; set; }

        PDFStream ObjectData { get; }

        void WriteData(PDFWriter writer);

        bool Deleted { get; }

        bool HasStream { get; }

        PDFStream Stream { get; }

        byte[] GetObjectData();

        byte[] GetStreamData();
    }

    #endregion

    #region public interface IStreamFilter

    /// <summary>
    /// Defines the interface that all Stream Filters must adhere to 
    /// </summary>
    public interface IStreamFilter
    {
        /// <summary>
        /// Gets or Sets the name of the filter
        /// </summary>
        string FilterName
        {
            get;
            set;
        }

        /// <summary>
        /// Filters the stream reading from the TextReader, applying the filter and writing to the TextWriter
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        void FilterStream(System.IO.TextReader read, System.IO.TextWriter write);

        /// <summary>
        /// Performs a filter on the original data array, and returns the filtered data as a new byte[]
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        byte[] FilterStream(byte[] orig);
    }

    #endregion

    #region public interface IObjectContainer

    /// <summary>
    /// Interface that defines a container of IFileObjects
    /// </summary>
    public interface IObjectContainer
    {
        void Add(IFileObject obj);
    }

    #endregion

    #region public interface IPDFObject : ITypedObject

    /// <summary>
    /// Base interface for all pdf objects
    /// </summary>
    public interface IPDFObject : ITypedObject //, IDisposable
    {
    }

    #endregion

    #region public interface IPDFResourceContainer

    /// <summary>
    /// Interface for any items that hold a collection of resources
    /// </summary>
    public interface IPDFResourceContainer
    {

        IPDFDocument Document { get; }

        Scryber.Native.PDFName Register(PDFResource rsrc);

        string MapPath(string source);

    }

    #endregion

    public interface IPDFResource
    {
        string ResourceType { get; }

        string ResourceKey { get; }

        PDFObjectRef EnsureRendered(PDFContextBase context, PDFWriter writer);
    }


    #region public interface IPDFTemplate

    /// <summary>
    /// Interface for a class that supports the instantiation of one or more copies its own content into the container
    /// </summary>
    public interface IPDFTemplate
    {
        /// <summary>
        /// Creates a copy of any content of this template in the specified container
        /// </summary>
        /// <param name="index">The current index of the instantiation</param>
        IEnumerable<IPDFComponent> Instantiate(int index);
    }

    #endregion

    #region public interface IPDFComponent : IPDFObject

    /// <summary>
    /// Interface that complex pdf objects should support, including initialization and disposal
    /// </summary>
    public interface IPDFComponent : IPDFObject, IDisposable
    {
        /// <summary>
        /// Event that is raised when the object is initialized
        /// </summary>
        event EventHandler Initialized;
        

        /// <summary>
        /// Initializes a PDFComponent
        /// </summary>
        void Init(PDFInitContext context);

        /// <summary>
        /// Gets or sets the ID of the component
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Gets the document that contains this PDFComponent, and forms the root of the PDF Component hierarchy
        /// </summary>
        IPDFDocument Document { get; }

        /// <summary>
        /// Gets or sets the containing parent of this PDFComponent
        /// </summary>
        IPDFComponent Parent { get; set; }

        /// <summary>
        /// Returns the full path to a file relative to the component or its contianer(s).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        string MapPath(string source);

    }

    #endregion

    #region public interface IPDFRemoteComponent : IPDFComponent

    /// <summary>
    /// Defines a component interface that can be remotely loaded and the FileSource set as 
    /// the path the file was loaded from
    /// </summary>
    public interface IPDFRemoteComponent : IPDFComponent
    {
        /// <summary>
        /// Gets or sets the full path to the file the component was loaded from
        /// </summary>
        string LoadedSource { get; set; }
    }

    #endregion

    #region public interface IPDFBindableComponent

    /// <summary>
    /// Interface that identifies the Databinding features of an Component
    /// </summary>
    public interface IPDFBindableComponent
    {
        /// <summary>
        /// Event that is raised before an Component is databound
        /// </summary>
        event PDFDataBindEventHandler DataBinding;
        /// <summary>
        /// Event that is raised after an object has been databound
        /// </summary>
        event PDFDataBindEventHandler DataBound;

        /// <summary>
        /// Databinds a PFComponent
        /// </summary>
        void DataBind(PDFDataContext context);
    }

    #endregion

    public interface IPDFDocument : IPDFComponent
    {

        /// <summary>
        /// Loads a specific PDFResource based on the requested resource type and key
        /// </summary>
        /// <param name="type">The resource type</param>
        /// <param name="key">The resource key </param>
        /// <returns></returns>
        PDFResource GetResource(string type, string key, bool create);

        /// <summary>
        /// Returns a document unique identifier for a particular object type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetIncrementID(PDFObjectType type);

    }

    #region IPDFSimpleCodeDomValue

    /// <summary>
    /// Interface that simple types can implement to return  custom construction code expression
    /// </summary>
    public interface IPDFSimpleCodeDomValue
    {
        System.CodeDom.CodeExpression GetValueExpression();
    }

    #endregion

    #region public interface IPDFDataSource

    /// <summary>
    /// Defines the interface that all controls must implement if 
    /// they want to be used as a source of data within the page.
    /// </summary>
    public interface IPDFDataSource
    {
        /// <summary>
        /// Performs the selection of Data (optionally with a restriction xpath expression).
        /// </summary>
        /// <param name="path">The optional restriction.</param>
        /// <returns>The data associated with this DataSource control.</returns>
        object Select(string path);
    }

    #endregion

    #region public interface IPDFTraceLogFactory

    /// <summary>
    /// Contract that the trace log factories, defined in the configuration file must support to create new loggers
    /// </summary>
    public interface IPDFTraceLogFactory
    {
        PDFTraceLog CreateLog(TraceRecordLevel level);
    }

    #endregion

}
