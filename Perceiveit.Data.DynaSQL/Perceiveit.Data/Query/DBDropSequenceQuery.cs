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
    /// A reference to a DROP SEQUENCE query.
    /// </summary>
    public abstract class DBDropSequenceQuery : DBDropQuery
    {

        #region public string SequenceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence this query should create
        /// </summary>
        public string SequenceName { get; set; }

        #endregion

        #region public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence owner
        /// </summary>
        public string SequenceOwner { get; set; }

        #endregion

        
        //
        // ctors
        //

        #region protected DBDropSequenceQuery(string owner, string name)

        /// <summary>
        /// protected constructor. use the static (shared) factory methods to create an instance
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBDropSequenceQuery(string owner, string name)
        {
            this.SequenceOwner = owner;
            this.SequenceName = name;
        }

        #endregion

        //
        // implementaion methods
        //

        #region public DBDropSequenceQuery IfExists()

        /// <summary>
        /// Extends the DROP sequence to incude a check for the object before dropping
        /// </summary>
        /// <returns></returns>
        public DBDropSequenceQuery IfExists()
        {
            this.CheckExists = DBExistState.Exists;
            return this;
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBDropSequenceQuery Sequence(string name) + 1 overload

        public static DBDropSequenceQuery DropSequence()
        {
            return DropSequence(string.Empty, string.Empty);
        }
        /// <summary>
        /// Instantiates and returns a query that will drop a sequence with the specified name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropSequenceQuery DropSequence(string name)
        {
            return DropSequence(string.Empty, name);
        }

        /// <summary>
        /// Instantiates and returns a query that will drop a sequence with the specified owner and name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBDropSequenceQuery DropSequence(string owner, string name)
        {
            return new DBDropSequenceQueryRef(owner, name);
        }

        #endregion
    }


    public class DBDropSequenceQueryRef : DBDropSequenceQuery
    {

        public DBDropSequenceQueryRef(string owner, string name)
            : base(owner, name)
        {
        }


        protected override string XmlElementName
        {
            get { return XmlHelper.DropSequence; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDropStatement(DBSchemaTypes.Sequence, this.SequenceOwner, this.SequenceName, this.CheckExists == DBExistState.Exists);
            builder.EndDrop(DBSchemaTypes.Sequence, this.CheckExists == DBExistState.Exists);
            return true;
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.SequenceName = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.SequenceOwner = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteOptionalAttribute(writer, XmlHelper.Name, this.SequenceName, context);
            this.WriteOptionalAttribute(writer, XmlHelper.Owner, this.SequenceOwner, context);

            return base.WriteAllAttributes(writer, context);
        }
    }
}
