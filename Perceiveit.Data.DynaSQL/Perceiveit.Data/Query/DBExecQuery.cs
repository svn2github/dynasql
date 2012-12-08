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
 *  along with DynaSQL in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Encapsualtes an EXEC sql statement. 
    /// All exec statements use explicit parameters rather than constansts to define their arguments
    /// </summary>
    public class DBExecQuery : DBQuery
    {
        private DBClauseList _params;
        private string _owner;
        private string _name;

        //
        // properties
        //

        #region internal DBClauseList Parameters {get;set;}

        /// <summary>
        /// Gets or sets the list of parameters for this EXEC statement
        /// </summary>
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

        /// <summary>
        /// Returns true if this EXEC statment has parameters
        /// </summary>
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


#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL statement builder methods
        //

        #region protected internal override System.Data.CommandType GetCommandType()

        /// <summary>
        /// Gets the command type of this EXEC query - StoredProcedure
        /// </summary>
        /// <returns></returns>
        protected internal override System.Data.CommandType GetCommandType()
        {
            return System.Data.CommandType.Text;
        }

        #endregion

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Overrides the base implementation to generate the SQL statement for this EXEC query
        /// </summary>
        /// <param name="builder">The builder to use to generate the execute statement</param>
        /// <returns>true</returns>
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

#endif

        //
        // Xml serialization
        //

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Gets the name of this queries XmlElement
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Exec; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Overrides the default implementation to append all the attributes for this EXEC query (owner, name, etc..)
        /// </summary>
        /// <param name="writer">The current XmlWriter</param>
        /// <param name="context">The XmlWriterContext</param>
        /// <returns>the base result</returns>
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

        /// <summary>
        /// Overrides the default implementation to write all the parameters for this EXEC query
        /// </summary>
        /// <param name="writer">The XmlWriter</param>
        /// <param name="context">The XmlWriterContext</param>
        /// <returns>the base result</returns>
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
        
        /// <summary>
        /// Overrides the default implementation to read an EXEC element attribute
        /// </summary>
        /// <param name="reader">The current XmlReader</param>
        /// <param name="context">The xmlReaderContext</param>
        /// <returns>True is it is a known attribute otherwise the base result</returns>
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
        /// <summary>
        /// Overrides the base implementation to read the inner elements.
        /// </summary>
        /// <param name="reader">The current XmlReader</param>
        /// <param name="context">The current XmlReaderContext</param>
        /// <returns>true if this is a known element, otherwise returns the base element evaluation</returns>
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

        /// <summary>
        /// Appends a parameter to this EXEC statement
        /// </summary>
        /// <param name="param">The parameter to append</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParam(DBParam param)
        {
            if (null == param)
                throw new ArgumentNullException("param");

            this.Parameters.Add(param);
            return this;
        }

        #endregion

        #region public DBExecQuery WithParams(params DBParam[] all)

        /// <summary>
        /// Appends all the parameters to this exec statement
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public DBExecQuery WithParams(params DBParam[] all)
        {
            DBExecQuery exec = this;
            foreach (DBParam p in all)
            {
                exec = exec.WithParam(p);
            }
            return exec;
        }

        #endregion

        #region public DBExecQuery WithParamValue(object paramValue) + 3 overloads

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the specified value. 
        /// WARNING - specifying null or DBNull will make the DbType undiscoverable at runtime, and may cause execution errors
        /// </summary>
        /// <param name="paramValue">The value of the parameter</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamValue(object paramValue)
        {
            DBParam p = DBParam.ParamWithValue(paramValue);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement of the specified type and with the specified value
        /// </summary>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="paramValue">The value of the parameter</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamValue(System.Data.DbType type, object paramValue)
        {
            DBParam p = DBParam.ParamWithValue(type, paramValue);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the specified name and value. 
        /// WARNING - specifying null or DBNull will make the DbType undiscoverable at runtime, and may cause execution errors
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamValue(string name, object value)
        {
            DBParam p = DBParam.ParamWithValue(name, value);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the specified name, type and value. 
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamValue(string name, System.Data.DbType type, object value)
        {
            DBParam p = DBParam.ParamWithValue(name, type, value);
            return this.WithParam(p);
        }


        public DBExecQuery WithParamValue(string name, System.Data.DbType type, int length, object value)
        {
            DBParam p = DBParam.ParamWithValue(name, type, length, value);
            return this.WithParam(p);
        }


        #endregion

        #region public DBParam WithParamDelegate(ParamValue valueprovider) + 3 overloads
        /// <summary>
        /// Appends a new parameter to this EXEC statement with the delegate 
        /// method that will be evaulated at statement generation time ot extract the value
        /// </summary>
        /// <param name="valueprovider">The delegate method that will return the value</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamDelegate(ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(valueprovider);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the type and the delegate 
        /// method that will be evaulated at statement generation time ot extract the value
        /// </summary>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="valueprovider">The delegate method that will return the value</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamDelegate(System.Data.DbType type, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(type, valueprovider);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the type and the delegate 
        /// method that will be evaulated at statement generation time to extract the value
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="valueprovider">The delegate method that will return the value</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamDelegate(string name, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(name, valueprovider);
            return this.WithParam(p);
        }

        /// <summary>
        /// Appends a new parameter to this EXEC statement with the type and the delegate 
        /// method that will be evaulated at statement generation time to extract the value
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="valueprovider">The delegate method that will return the value</param>
        /// <returns>Itself to support statement chaining</returns>
        public DBExecQuery WithParamDelegate(string name, System.Data.DbType type, ParamValue valueprovider)
        {
            DBParam p = DBParam.ParamWithDelegate(name, type, valueprovider);
            return this.WithParam(p);
        }

        #endregion

    }
}
