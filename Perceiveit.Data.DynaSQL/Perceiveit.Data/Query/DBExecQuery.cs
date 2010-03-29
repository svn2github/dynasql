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
    public class DBExecQuery : DBQuery
    {
        private DBClauseList _params;
        private string _owner;
        private string _name;

        //
        // properties
        //

        #region internal DBClauseList Parameters {get;set;}

        internal DBClauseList Parameters
        {
            get 
            {
                if (_params == null)
                    _params = new DBClauseList();
                return _params;

            }
            set { _params = value; }
        }
        #endregion

        #region protected virtual bool HasParameters {get;}

        protected virtual bool HasParameters
        {
            get { return this._params != null && this._params.Count > 0; }
        }

        #endregion

        #region public string Owner {get; set;}

        /// <summary>
        /// Gets or sets the schema owner for the stored procedure that this ExecQuery will Run
        /// </summary>
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        #endregion

        #region public string SprocName {get;set;}

        /// <summary>
        /// Gets or sets the name of the StoredProcedure that this ExecQuery will run.
        /// </summary>
        public string SprocName
        {
            get { return _name; }
            internal set { _name = value; }
        }

        #endregion


        //
        // SQL statement builder methods
        //

        #region protected internal override System.Data.CommandType GetCommandType()

        protected internal override System.Data.CommandType GetCommandType()
        {
            return System.Data.CommandType.StoredProcedure;
        }

        #endregion

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginExecuteStatement();
            if (string.IsNullOrEmpty(this.Owner) == false)
            {
                builder.BeginIdentifier();
                builder.WriteRaw(this.Owner);
                builder.EndIdentifier();
                builder.AppendIdSeparator();
            }
            builder.BeginIdentifier();
            builder.WriteRaw(this.SprocName);
            builder.EndIdentifier();


            if (this.HasParameters)
            {
                builder.BeginExecuteParameters();
                foreach (DBClause c in this.Parameters)
                {
                    builder.BeginExecuteAParameter();
                    c.BuildStatement(builder);
                    builder.EndExecuteAParameter();
                }
                builder.EndExecuteParameters();
            }

            if (this.IsInnerQuery == false)
                builder.EndExecuteStatement();

            return true;
        }

        #endregion

        //
        // Xml serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Exec; }
        }

        #endregion


        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.Owner) == false)
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);
            if (string.IsNullOrEmpty(this.SprocName) == false)
                this.WriteAttribute(writer, XmlHelper.Name, this.SprocName, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasParameters)
            {
                this.WriteStartElement(XmlHelper.Parameters, writer, context);
                this.Parameters.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.Parameters, writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion


        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                b = true;
            }
            else if (IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.SprocName = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (IsElementMatch(XmlHelper.Parameters, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                this.Parameters.ReadXml(XmlHelper.Parameters, reader, context);
                b = true;
            }
            else 
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

        //
        // WithParam... methods
        //

        #region public DBExecQuery WithParam(DBParam param)

        public DBExecQuery WithParam(DBParam param)
        {
            this.Parameters.Add(param);
            return this;
        }

        #endregion

        #region public DBParam WithParamValue(object paramValue) + 3 overloads

        public DBExecQuery WithParamValue(object paramValue)
        {
            DBParam p = DBParam.ParamWithValue(paramValue);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamValue(System.Data.DbType type, object paramValue)
        {
            DBParam p = DBParam.ParamWithValue(type, paramValue);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamValue(string name, object value)
        {
            DBParam p = DBParam.ParamWithValue(name, value);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamValue(string name, System.Data.DbType type, object value)
        {
            DBParam p = DBParam.ParamWithValue(name, type, value);
            return this.WithParam(p);
        }


        #endregion

        #region public DBParam WithParamDelegate(ParamValue valueprovider) + 3 overloads

        public DBExecQuery WithParamDelegate(ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(valueprovider);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamDelegate(System.Data.DbType type, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(type, valueprovider);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamDelegate(string name, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(name, valueprovider);
            return this.WithParam(p);
        }

        public DBExecQuery WithParamDelegate(string name, System.Data.DbType type, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(name, type, valueprovider);
            return this.WithParam(p);
        }

        #endregion

    }
}
