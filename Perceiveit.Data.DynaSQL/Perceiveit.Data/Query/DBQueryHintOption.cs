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
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Defines a DBQueryHint to optimse the execution of a whole query
    /// </summary>
    public abstract class DBQueryHintOption : DBHintOption
    {

        #region public DBClause Clause { get; set; }

        /// <summary>
        /// Gets or sets the clause that completes the query option
        /// </summary>
        public DBClause Clause { get; set; }

        #endregion

        #region public DBQueryOption Option { get; set; }

        /// <summary>
        /// Gets or sets the DBQueryOption for this hint
        /// </summary>
        public DBQueryOption Option { get; set; }

        #endregion

        //
        // ctor
        //

        #region protected DBQueryHintOption()

        /// <summary>
        /// protected ctor
        /// </summary>
        protected DBQueryHintOption()
        {
        }

        #endregion


        //
        // static factory methods
        //

        #region public static DBQueryHintOption QueryOption()

        /// <summary>
        /// Parmeterless version primarily for searialization and deserialization 
        /// </summary>
        /// <returns></returns>
        public static DBQueryHintOption QueryOption()
        {
            return new DBQueryHintOptionRef();
        }

        #endregion

        #region public static DBQueryHintOption QueryOption(DBQueryOption option)

        /// <summary>
        /// Creates and returns a new DBQueryHintOption with the specified option value
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static DBQueryHintOption QueryOption(DBQueryOption option)
        {
            DBQueryHintOptionRef hint = new DBQueryHintOptionRef();
            hint.Option = option;
            return hint;
        }

        #endregion

        #region public static DBQueryHintOption QueryOption(DBQueryOption option, DBClause value)

        /// <summary>
        ///  Creates and returns a new DBQueryHintOption with the specified option value and value clause
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DBQueryHintOption QueryOption(DBQueryOption option, DBClause value)
        {
            DBQueryHintOption hint = QueryOption(option);
            hint.Clause = value;
            return hint;
        }

        #endregion

    }


    /// <summary>
    /// Implementation class
    /// </summary>
    internal class DBQueryHintOptionRef : DBQueryHintOption
    {

        #region protected override string XmlElementName

        /// <summary>
        /// Gets the element name for this instance when it is serialized and deserialized from xml
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.QueryOption; }
        }

        #endregion

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Generates the SQL statement for this DBQueryHint
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginAQueryHint(this.Option);
            if (null != this.Clause)
            {
                builder.BeginHintParameterList();
                Clause.BuildStatement(builder);
                builder.EndHintParameterList();
            }
            builder.EndAQueryHint(this.Option);
            return true;

        }

        #endregion

        //
        // XML Serialization
        //

        #region  protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Writes the attributes
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.HintOption, this.Option.ToString(), context);
            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Writes the child elements
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (null != Clause)
            {
                this.WriteStartElement(XmlHelper.HintParameter, writer, context);
                this.Clause.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.HintParameter, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads the element attribute(s)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.HintOption, reader, context))
            {
                object parsed = Enum.Parse(typeof(DBQueryOption), reader.Value);
                this.Option = (DBQueryOption)parsed;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads any inner elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsElementMatch(XmlHelper.HintParameter, reader, context))
            {
                reader.Read();
                DBClause clause = this.ReadNextInnerClause(XmlHelper.HintParameter, reader, context);
                this.Clause = clause;
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }

        #endregion

    }

    /// <summary>
    /// A list of DBQueryHints accessible by index or DBQueryOption. 
    /// It's an error to define more than on DBQueryHintOption with the same option value
    /// </summary>
    public class DBQueryHintList : System.Collections.ObjectModel.KeyedCollection<DBQueryOption, DBQueryHintOption>
    {

        #region protected override DBQueryOption GetKeyForItem(DBQueryHintOption item)

        /// <summary>
        /// Returns the provided hint's DBQueryOptio0n
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DBQueryOption GetKeyForItem(DBQueryHintOption item)
        {
            return item.Option;
        }

        #endregion

    }
}
