/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Perceiveit.Data.Query
{
    
    /// <summary>
    /// DBClause is the main base class for SQL Statement elements in the Query library
    /// It inherits the build statememt from DBToken, 
    /// but adds the ReadXml and WriteXml  methods
    /// </summary>
    public abstract class DBClause : DBToken
    {
        
        #region protected abstract string XmlElementName {get;}
        /// <summary>
        /// Abstract method for getting the element name for this SQL Statement clause
        /// </summary>
        protected abstract string XmlElementName {get;}

        #endregion

        //
        // Xml write methods
        //


        #region public bool WriteXml(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Main public method to write the DBClause to the XmlWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>Writes a start element with the name from XmlElementName property, 
        /// writes the attributes by calling WriteAllAttributes(), 
        /// and then all any inner elements by calling WriteInnerElements()
        /// and finally closes the element</remarks>
        public bool WriteXml(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            bool attrs;
            string name = this.XmlElementName;
            if (string.IsNullOrEmpty(name) == false)
            {
                this.WriteStartElement(name, writer, context);

                //if we are not an element then we cannot have attributes
                attrs = this.WriteAllAttributes(writer, context);
            }
            else
                attrs = true;

            bool eles = this.WriteInnerElements(writer, context);

            if(string.IsNullOrEmpty(name) == false) //did we write a start element?
                this.WriteEndElement(this.XmlElementName, writer, context);

            return attrs && eles;
        }

        #endregion


        #region protected virtual bool WriteInnerElements(XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Virtual (overridable) method to write the inner elements for a statement clause.
        /// Default implementation returns true
        /// </summary>
        /// <param name="writer">The current XmlWriter</param>
        /// <param name="context">The context to use when writing elements</param>
        /// <returns></returns>
        protected virtual bool WriteInnerElements(XmlWriter writer, XmlWriterContext context)
        {
            return true;
        }

        #endregion

        #region protected virtual bool WriteAllAttributes(XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Virtual (overridable) method to write the attributes for a statement element to the Xml writer.
        /// The Default implmentation simply returns true. Inheritors should add their own implementation to write their own attributes
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool WriteAllAttributes(XmlWriter writer, XmlWriterContext context)
        {
            return true;
        }

        #endregion

        #region protected void WriteAlias(System.Xml.XmlWriter xmlWriter, string alias, XmlWriterContext context)

        /// <summary>
        /// Writes the alias attribute to the xmlWriter at its current position
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="alias"></param>
        /// <param name="context"></param>
        protected void WriteAlias(System.Xml.XmlWriter xmlWriter, string alias, XmlWriterContext context)
        {
            XmlHelper.WriteAttribute(xmlWriter, XmlHelper.Alias, alias, context);
        }

        #endregion

        #region protected void WriteAttribute(System.Xml.XmlWriter writer, string attrname, string value, XmlWriterContext context)

        /// <summary>
        /// Helper method that writes an attribute with the specified name and value to the xmlWriter at the current position.
        /// Use this method to ensure that the correct prefix/namespace are applied when nescessary
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="attrname"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        protected void WriteAttribute(System.Xml.XmlWriter writer, string attrname, string value, XmlWriterContext context)
        {
            XmlHelper.WriteAttribute(writer, attrname, value, context);
        }

        /// <summary>
        /// Hepler method that writes an attribute if AND only if the value is not a null or empty string
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="attrname"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        protected void WriteOptionalAttribute(System.Xml.XmlWriter writer, string attrname, string value, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(value) == false)
                XmlHelper.WriteAttribute(writer, attrname, value, context);
        }

        #endregion

        #region protected void WriteStartElement(string eleName, XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Helper method that writes the starting element with the specified name to the specified XmlWriter. 
        /// Use this method to ensure that the correct prefix/namespace are applied when nescessary
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="eleName"></param>
        /// <param name="context"></param>
        protected void WriteStartElement(string eleName, XmlWriter writer, XmlWriterContext context)
        {
            XmlHelper.WriteStartElement(writer, eleName, context);
        }

        #endregion

        #region protected void WriteEndElement(string name, XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Writes the closing tags for the current element
        /// Use this method to ensure that the correct prefix/namespace are applied when nescessary
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        protected void WriteEndElement(string name, XmlWriter writer, XmlWriterContext context)
        {
            XmlHelper.WriteEndElement(writer, context);
        }

        #endregion

        //
        // Xml read methods
        //

        #region public bool ReadXml(System.Xml.XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads the element the reader is positioned at - initially reading the attributes then any inner elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool ReadXml(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            string name = reader.LocalName;
            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                this.ReadAllAttributes(reader, context);
            }
            if (!isEmpty)
            {
                while (reader.Read())
                {
                    
                    if(reader.NodeType == XmlNodeType.Element || 
                            reader.NodeType == XmlNodeType.Entity ||
                            reader.NodeType == XmlNodeType.CDATA ||
                            reader.NodeType == XmlNodeType.ProcessingInstruction ||
                            reader.NodeType == XmlNodeType.Text)
                    {
                        this.ReadAnInnerElement(reader, context);
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && XmlHelper.IsElementMatch(name, reader, context))
                        break;
                }
            }
            return true;
        }

        #endregion


        #region protected virtual bool ReadAnAttribute(XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Virtual method - Inheritors should override this to read individual attribute values
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool ReadAnAttribute(XmlReader reader, XmlReaderContext context)
        {
            return true;
        }

        #endregion

        #region protected virtual bool ReadAllAttributes(XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads all attributes on a element and calls 'ReadAnAttribute' for each of them
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool ReadAllAttributes(XmlReader reader, XmlReaderContext context)
        {
            bool b = true;

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (!this.ReadAnAttribute(reader, context))
                        b = false;
                }
                while (reader.MoveToNextAttribute());
            }
            return b;
        }

        #endregion

        #region protected virtual bool ReadAnInnerElement(XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Virtual method - Inheritors should override this method to read an specific inner elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool ReadAnInnerElement(XmlReader reader, XmlReaderContext context)
        {
            return true;
        }

        #endregion

        #region protected bool ReadInnerElementList(DBClauseList list, string endelement, XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads an inner list of elements, creating a new DBClause for each element and adding it to the collection
        /// </summary>
        /// <param name="list"></param>
        /// <param name="endelement"></param>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool ReadInnerElementList(DBClauseList list, string endelement, XmlReader reader, XmlReaderContext context)
        {
            int count = 0;

            do
            {
                if (reader.NodeType == XmlNodeType.EndElement && this.IsElementMatch(endelement, reader, context))
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    DBClause c = context.Factory.Read(reader.LocalName, reader, context);
                    if (c != null)
                    {
                        list.Add(c);
                        count++;
                    }
                }
            } while (reader.Read());

            return count > 0;
        }

        #endregion

        #region protected bool IsAttributeMatch(string attrname, System.Xml.XmlReader reader, XmlReaderContext context)
        
        /// <summary>
        /// Compares the specified attribute name with the readers current local name, 
        /// along with the prefix and name in the context when required. 
        /// Returns true if the readers node matches the required attribute, otherwise false
        /// </summary>
        /// <param name="attrname"></param>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool IsAttributeMatch(string attrname, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            return XmlHelper.IsAttributeMatch(attrname, reader, context);
        }

        #endregion

        #region protected bool IsElementMatch(string elename, System.Xml.XmlReader reader, XmlReaderContext context)
        
        /// <summary>
        /// Compares the specified element name with the readers current local name,
        /// along with the prefix and namespace in the context when required.
        /// Returns true if the readers node matches the required element, otherwise false
        /// </summary>
        /// <param name="elename"></param>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool IsElementMatch(string elename, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            return XmlHelper.IsElementMatch(elename, reader, context);
        }

        #endregion

        /// <summary>
        /// Reads the next element from the XMLReader and constructs then a new clause based upon this XML data
        /// </summary>
        /// <param name="endElement"></param>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected DBClause ReadNextInnerClause(string endElement, System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBClause clause = null;
            do
            {
                if (reader.NodeType == System.Xml.XmlNodeType.EndElement && this.IsElementMatch(endElement, reader, context))
                    break;
                else if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    clause = context.Factory.Read(reader.Name, reader, context);
                    break;
                }

            } while (reader.Read());
            

            return clause;
        }

    }

}
