/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Definition of a table column in the SQL database schema. 
    /// This class is abstract - to create an instance use the static Field or AllField methods
    /// </summary>
    public abstract class DBField : DBCalculableClause
    {
        private string _name;
        private string _tbl;
        private string _owner;
        

        //
        // properties
        //

        #region public virtual string Name {get;set;}
        
        /// <summary>
        /// Gets or Sets the name of the Field
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region public string Table {get;set;}

        /// <summary>
        /// Gets or sets the optional name of the table / view this field is a column on
        /// </summary>
        public string Table
        {
            get { return _tbl; }
            set { _tbl = value; }
        }

        #endregion

        #region public string Owner {get;set;}

        /// <summary>
        /// Gets or sets the optional schema owner for this field
        /// </summary>
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        #endregion

        //
        // ctor(s)
        //

        #region protected DBField()

        protected DBField()
        {
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBField Field()

        public static DBField Field()
        {
            DBFieldRef fref = new DBFieldRef();
            return fref;
        }

        #endregion

        #region public static DBField Field(string field)

        public static DBField Field(string field)
        {
            DBFieldRef fref = new DBFieldRef();
            fref.Name = field;

            return fref;
        }

        #endregion

        #region public static DBField Field(string table, string field)

        public static DBField Field(string table, string field)
        {
            DBFieldRef fref = new DBFieldRef();
            fref.Name = field;
            fref.Table = table;

            return fref;
        }

        #endregion

        #region public static DBField Field(string owner, string table, string field)

        public static DBField Field(string owner, string table, string field)
        {
            DBFieldRef fref = new DBFieldRef();
            fref.Name = field;
            fref.Table = table;
            fref.Owner = owner;

            return fref;
        }

        #endregion


        #region public static DBField AllFields()

        public static DBField AllFields()
        {
            DBFieldAllRef all = new DBFieldAllRef();

            return all;
        }

        #endregion

        #region public static DBField AllFields(string table)

        public static DBField AllFields(string table)
        {
            DBFieldAllRef all = new DBFieldAllRef();
            all.Table = table;
            return all;
        }

        #endregion

        #region public static DBField AllFields(string owner, string table)

        public static DBField AllFields(string owner, string table)
        {
            DBFieldAllRef all = new DBFieldAllRef();
            all.Table = table;
            all.Owner = owner;
            return all;
        }

        #endregion

        public abstract DBField As(string alias);

    }

    internal class DBFieldRef : DBField, IDBAlias
    {
        private string _alias;

        //
        // properties
        //

        #region public string Alias {get;set;}

        /// <summary>
        /// Gets or Sets the Alias name for this field
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        #endregion
        
        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.AField; }
        }

        #endregion

        //
        // SQL statement build methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (string.IsNullOrEmpty(this.Name))
                return false;

            builder.WriteSourceField(this.Owner, this.Table, this.Name, this.Alias);

            return true;
        }

        #endregion

        //
        // XML Serialization
        //

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;

            if (this.IsAttributeMatch(XmlHelper.FieldOwner, reader, context))
            {
                this.Owner = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Table, reader, context))
            {
                this.Table = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
            {
                this.Alias = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.Owner) == false)
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);
            
            if (string.IsNullOrEmpty(this.Table) == false)
                this.WriteAttribute(writer, XmlHelper.Table, this.Table, context);
            
            if (string.IsNullOrEmpty(this.Name) == false)
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);

            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAttribute(writer, XmlHelper.Alias, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        //
        // Interface implementation
        //

        public override DBField As(string alias)
        {
            this.Alias = alias;
            return this;
        }

        #region void IDBAlias.As(string alias)

        void IDBAlias.As(string alias)
        {
            this.Alias = alias;
        }

        #endregion
    }

    public class DBFieldAllRef : DBField
    {

        #region public override string Name {get;set;}

        /// <summary>
        /// Overrides the default implementation to return '*' - setter does nothing
        /// </summary>
        public override string Name
        {
            get
            {
                return "*";
            }
            set
            {
                ;
            }
        }

        #endregion

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.AllFields; }
        }

        #endregion

        public override DBField As(string alias)
        {
            throw new NotSupportedException("The Alias As method is not supported on an all field reference.");
        }
        //
        // SQL Statement build methods
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.WriteAllFieldIdentifier(this.Owner, this.Table);

            return true;
        }

        #endregion

        //
        // Xml Serilaization
        //

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;

            if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Table, reader, context))
            {
                this.Table = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.Owner) == false)
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);

            if (string.IsNullOrEmpty(this.Table) == false)
                this.WriteAttribute(writer, XmlHelper.Table, this.Table, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

    }

    internal class DBFieldList : DBClauseList<DBField>
    {
    }
}
