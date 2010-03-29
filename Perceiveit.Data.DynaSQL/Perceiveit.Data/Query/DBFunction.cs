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
    public abstract class DBFunction : DBClause, IDBAlias
    {
        private Function _func;
        private string _funcname;
        private DBClauseList _params;
        private string _alias;

        //
        // properties
        //


        #region public Function KnownFunction {get;set;}

        /// <summary>
        /// Gets or Sets the function to execute in the statement
        /// </summary>
        public Function KnownFunction
        {
            get { return _func; }
            set { _func = value; this._funcname = null; }
        }

        #endregion

        #region public string FunctionName {get;set;}

        /// <summary>
        /// Gets or Sets the name of the Function to Execute in the statement
        /// </summary>
        public string FunctionName
        {
            get 
            {
                if (null == _funcname)
                {
                    if (this._func != Perceiveit.Data.Function.Unknown)
                        _funcname = this._func.ToString();
                    else
                        _funcname = string.Empty;
                }
                return _funcname;
            }
            set 
            {
                _funcname = value;
                if (string.IsNullOrEmpty(_funcname))
                    this._func = Perceiveit.Data.Function.Unknown;
                else
                {
                    if (Array.IndexOf<string>(Enum.GetNames(typeof(Function)), value) > -1)
                    {
                        this._func = (Function)Enum.Parse(typeof(Function), value);
                    }
                }

            }
        }

        #endregion

        #region internal DBClauseList Parameters {get;} + bool HasParameters {get;}

        /// <summary>
        /// Gets the parameter list for this function
        /// </summary>
        internal DBClauseList Parameters
        {
            get 
            {
                if (this._params == null)
                    this._params = new DBClauseList();
                return _params;
            }
        }

        public bool HasParameters
        {
            get { return this._params != null && this._params.Count > 0; }
        }

        #endregion

        #region public string Alias {get;set;}

        /// <summary>
        /// Gets or sets the optional alias name for the output of this Function
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBFunction IsNull(string field, DBClause otherwise) + 3 overloads

        public static DBFunction IsNull(string field, DBClause otherwise)
        {
            return IsNull(DBField.Field(field), otherwise);
        }

        public static DBFunction IsNull(string table, string field, DBClause otherwise)
        {
            return IsNull(DBField.Field(table, field), otherwise);
        }

        public static DBFunction IsNull(string owner, string table, string field, DBClause otherwise)
        {
            return IsNull(DBField.Field(owner, table, field), otherwise);
        }

        public static DBFunction IsNull(DBClause match, DBClause otherwise)
        {
            DBFunctionRef fref = new DBFunctionRef();
            fref.KnownFunction = Perceiveit.Data.Function.IsNull;
            fref.Parameters.Add(match);
            fref.Parameters.Add(otherwise);

            return fref;
        }

        #endregion

        #region public static DBFunction GetDate()

        public static DBFunction GetDate()
        {
            DBFunction func = new DBFunctionRef();
            func.KnownFunction = Perceiveit.Data.Function.GetDate;

            return func;
        }

        #endregion

        #region public static DBFunction LastID()

        public static DBFunction LastID()
        {
            DBFunction func = new DBFunctionRef();
            func.KnownFunction = Perceiveit.Data.Function.LastID;

            return func;
        }

        #endregion


        #region  public static DBFunction Function(Function func, params DBClause[] parameters) + 3 overload

        public static DBFunction Function()
        {
            DBFunction func = new DBFunctionRef();
            return func;
        }

        public static DBFunction Function(Function func)
        {
            DBFunction f = DBFunction.Function();
            f.KnownFunction = func;
            return f;
        }

        public static DBFunction Function(Function func, params DBClause[] parameters)
        {
            DBFunction f = new DBFunctionRef();
            f.KnownFunction = func;
            if (null != parameters && parameters.Length > 0)
            {
                foreach (DBClause clause in parameters)
                {
                    f.Parameters.Add(clause);
                }
            }
            return f;
        }

        public static DBFunction Function(string func, params DBClause[] parameters)
        {
            DBFunction f = new DBFunctionRef();
            f.FunctionName = func;
            if (null != parameters && parameters.Length > 0)
            {
                foreach (DBClause clause in parameters)
                {
                    f.Parameters.Add(clause);
                }
            }
            return f;
        }

        #endregion

        //
        // Interface Implementations
        //

        #region IDBAlias Members

        void IDBAlias.As(string aliasName)
        {
            this.Alias = aliasName;
        }

        #endregion
    }


    internal class DBFunctionRef : DBFunction
    {
        //
        // properties
        //

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Gets the name of the xml element for this Function
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Function; }
        }

        #endregion

        //
        // SQL Statement builder
        // 

        #region public override bool BuildStatement(DBStatementBuilder builder)

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (string.IsNullOrEmpty(this.FunctionName))
                return false;

            builder.BeginFunction(this.KnownFunction, this.FunctionName);
            builder.BeginFunctionParameterList();

            if (this.HasParameters)
            {
                this.Parameters.BuildStatement(builder);
            }
            builder.EndFunctionParameterList();

            builder.EndFunction(this.KnownFunction, this.FunctionName);

            if (string.IsNullOrEmpty(this.Alias) == false)
                builder.WriteAlias(this.Alias);

            return true;
        }

        #endregion

        //
        // XML serialization
        //

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.KnownFunction != Perceiveit.Data.Function.Unknown)
                this.WriteAttribute(writer, XmlHelper.KnownFunction, this.KnownFunction.ToString(), context);
            else if (string.IsNullOrEmpty(this.FunctionName) == false)
                this.WriteAttribute(writer, XmlHelper.FunctionName, this.FunctionName, context);

            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAlias(writer, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasParameters)
            {
                this.WriteStartElement(XmlHelper.FunctionParameter, writer, context);
                
                foreach (DBClause c in this.Parameters)
                {
                    c.WriteXml(writer, context);
                }

                this.WriteEndElement(XmlHelper.FunctionParameter, writer, context);
            }

            return base.WriteInnerElements(writer, context);
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
                this.Alias = reader.Value;
            else if (this.IsAttributeMatch(XmlHelper.KnownFunction, reader, context))
                this.KnownFunction = (Function)Enum.Parse(typeof(Function), reader.Value);
            else if (this.IsAttributeMatch(XmlHelper.FunctionName, reader, context))
                this.FunctionName = reader.Value;
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsElementMatch(XmlHelper.FunctionParameter, reader, context) && !reader.IsEmptyElement && reader.Read())
            {
                b = this.Parameters.ReadXml(XmlHelper.FunctionParameter, reader, context);
            }
            else
                b = base.ReadAnInnerElement(reader, context);

            return b;
        }

        #endregion

    }
}
