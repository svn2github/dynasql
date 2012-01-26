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

namespace Scryber
{

    #region PDFParsableValue Attribute

    /// <summary>
    /// Placeholder attribute that defines that a class can be parsed from a string 
    /// using a public static Parse(string) method on the class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PDFParsableValueAttribute : Attribute
    {
    }

    #endregion



    #region PDFParsableComponent Attribute

    /// <summary>
    /// Identifies a root complex component that the PDFParser can reflect and parse. 
    /// This attribute is not inherited for good reason
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PDFParsableComponentAttribute : Attribute
    {
        private string _name;

        /// <summary>
        /// Gets or Sets the name of the elementthe parser should interpret 
        /// as a reference to the type this attribute is declared on.
        /// </summary>
        public string ElementName
        {
            get { return _name; }
            set { _name = value; }
        }



        /// <summary>
        /// Creates a new instance of the PDFComponent Attribute with the specific name
        /// </summary>
        /// <param name="name">The Components name</param>
        public PDFParsableComponentAttribute(string name)
        {
            this._name = name;
        }
    }

    #endregion

    #region PDFRemoteParsableComponent Attribute

    /// <summary>
    /// Defines a parsable complex component that can be referenced from an separate file 
    /// ('source' is the name of the attribute that indicates the source file)
    /// </summary>
    /// <remarks>This attribute is not inherited</remarks>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PDFRemoteParsableComponentAttribute : Attribute
    {
        private string _name;

        /// <summary>
        /// Gets or Sets the name of the elementthe parser should interpret 
        /// as a reference to the type this attribute is declared on.
        /// </summary>
        public string ElementName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Creates a new instance of the PDFComponent Attribute with the specific name
        /// </summary>
        /// <param name="name">The Components name</param>
        public PDFRemoteParsableComponentAttribute(string name)
        {
            this._name = name;
        }
    }

    #endregion

    #region PDFParserIgnore Attribute

    /// <summary>
    /// Any class, method, property or event decorated with this attribute will be ignored by the PDFParser (if the ignore flag is set to false)
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class PDFParserIgnoreAttribute : Attribute
    {
        private bool _ignore;

        /// <summary>
        /// Gets or sets whether to ignore the declaration that this attribute is applied to.
        /// </summary>
        public bool Ignore { get { return _ignore; } set { _ignore = value; } }

        /// <summary>
        /// Sets Ignore to true by default.
        /// </summary>
        public PDFParserIgnoreAttribute() : this(true) { }

        /// <summary>
        /// Sets Ignore to the value ofthe ignore parameter.
        /// </summary>
        /// <param name="ignore"></param>
        public PDFParserIgnoreAttribute(bool ignore) { this._ignore = ignore; }

    }

    #endregion

    #region PDFAttribute Attribute

    /// <summary>
    /// Defines that a property can be included as an PDF Components attribute
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    public class PDFAttributeAttribute : Attribute
    {
        private string _name;

        /// <summary>
        /// Gets or Sets the name of the PDF attribute - cannot be null or empty
        /// </summary>
        public string AttributeName
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// Creates a new instance of the PDFAttribute Attribute with the specific name
        /// </summary>
        /// <param name="name">The attributes name - cannot be null or empty</param>
        public PDFAttributeAttribute(string name)
        {
            this._name = name;
        }
    }

    #endregion

    #region PDFElement Attribute

    /// <summary>
    /// Identifies a property that can be included when parsing a component as a complex element or a collection of elements
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
    public class PDFElementAttribute : Attribute
    {
        private string _name;


        /// <summary>
        /// Gets or sets the name of the element that can be parsed to the value of the property this attribute is declared on.
        /// If the name is empty then it is the default property.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PDFElementAttribute(string name)
        {
            this._name = name;
        }
    }

    #endregion

    #region PDFArray Attribute

    /// <summary>
    /// Defines the property as an array of elements with a specific base type
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PDFArrayAttribute : Attribute
    {
        private Type _basetype;

        /// <summary>
        /// Defines the base type of all the contained elements if this is a collection
        /// </summary>
        public Type ContentBaseType
        {
            get { return _basetype; }
            set { _basetype = value; }
        }

        public PDFArrayAttribute()
            : this(typeof(PDFObject))
        {
        }

        public PDFArrayAttribute(Type basetype)
        {
            _basetype = basetype;
        }

    }

    #endregion

    #region PDFTemplate Attribute

    /// <summary>
    /// Defines the property as a TemplateContainer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PDFTemplateAttribute : Attribute
    {
        public PDFTemplateAttribute()
        { }
    }

    #endregion

    #region PDFGeneratedSource Attribute

    /// <summary>
    /// Defines the source path the component was generated from originally
    /// </summary>
    public class PDFGeneratedSourceAttribute : Attribute
    {
        private string _srcpath;

        public string SourcePath
        {
            get { return _srcpath; }
        }

        public PDFGeneratedSourceAttribute(string path)
            : base()
        {
            this._srcpath = path;
        }
    }

    #endregion
}
