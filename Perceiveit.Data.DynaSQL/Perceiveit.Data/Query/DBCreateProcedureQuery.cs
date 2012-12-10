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
using System.Data;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Defines a CREATE PROCEDURE statement along with its' parameters and the script to execute when the procudure is called.
    /// </summary>
    public abstract class DBCreateProcedureQuery : DBCreateQuery
    {

        #region internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of this stored procedure
        /// </summary>
        internal string Name { get; set; }

        #endregion

        #region internal string Owner { get; set; }

        /// <summary>
        /// Gets or sets the owner of this stored procedure
        /// </summary>
        internal string Owner { get; set; }

        #endregion

        #region internal DBParamList Parameters { get; set; } + HasParameters {get;}

        /// <summary>
        /// Gets or sets the list of parameters in this stored procedure
        /// </summary>
        internal DBParamList Parameters { get; set; }

        /// <summary>
        /// Checks if this procedure has one or more parameters
        /// </summary>
        internal bool HasParameters
        {
            get { return this.Parameters.Count > 0; }
        }

        #endregion

        #region internal DBScript Script { get; set; }

        /// <summary>
        /// Gets or sets the reference script to be executed when this Stored procedure is created an invoked
        /// </summary>
        internal DBScript Script { get; set; }

        #endregion

        //
        // ctor
        //

        public DBCreateProcedureQuery()
        {
            this.Parameters = new DBParamList();
        }

        //
        // methods
        //

        #region public DBCreateProcedureQuery WithParam(DBParam parameter) + 4 overloads

        /// <summary>
        /// Adds an input/output parameter to the declaration of this StoredProcedure
        /// </summary>
        /// <param name="genericname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParam(string genericname, DbType type)
        {
            DBParam p = DBParam.Param(genericname, type);
            return this.WithParam(p);
        }

        /// <summary>
        ///  Adds an input/output parameter to the declaration of this StoredProcedure
        /// </summary>
        /// <param name="genericname"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParam(string genericname, DbType type, int size)
        {
            DBParam p = DBParam.Param(genericname, type, size);
            return this.WithParam(p);
        }

        /// <summary>
        ///  Adds an input/output parameter to the declaration of this StoredProcedure
        /// </summary>
        /// <param name="genericname"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParam(string genericname, DbType type, ParameterDirection direction)
        {
            DBParam p = DBParam.Param(genericname, type, direction);
            return this.WithParam(p);
        }

        /// <summary>
        ///  Adds an input/output parameter to the declaration of this StoredProcedure
        /// </summary>
        /// <param name="genericname"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParam(string genericname, DbType type, int size, ParameterDirection direction)
        {
            DBParam p = DBParam.Param(genericname, type, size, direction);
            return this.WithParam(p);
        }

        /// <summary>
        ///  Adds an input/output parameter to the declaration of this StoredProcedure
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParam(DBParam parameter)
        {
            this.Parameters.Add(parameter);
            return this;
        }

        #endregion

        #region public DBCreateProcedureQuery WithParams(params DBParam[] parameters)

        /// <summary>
        /// Adds the defined parameters to the input/output parameter list in this StoredProcedure
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery WithParams(params DBParam[] parameters)
        {
            foreach (DBParam p in parameters)
            {
                this.Parameters.Add(p);
            }
            return this;
        }

        #endregion

        #region public DBCreateProcedureQuery As(DBScript script) + 1 overload

        /// <summary>
        /// Sets the actual exection steps in the stored procedure as the script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery As(DBScript script)
        {
            if (null == this.Script)
                this.Script = script;
            else
                this.Script.Append(script);
            this.Script.IsInnerQuery = true;
            return this;
        }

        /// <summary>
        /// Sets or appends the actual execution step(s) in the stored procedure
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        public DBCreateProcedureQuery As(params DBStatement[] statements)
        {
            if (null == this.Script)
                this.Script = DBScript.Script(statements);
            else
            {
                foreach (DBStatement state in statements)
                {
                    this.Script.Append(state);
                }
            }
            this.Script.IsInnerQuery = true;
            return this;
        }

        #endregion

        public DBCreateProcedureQuery IfNotExists()
        {
            this.CheckExists = DBExistState.NotExists;
            return this;
        }

        //
        // static factory methid
        //

        #region public static DBCreateProcedureQuery Procedure(string owner, string name) + 1 overload

        public static DBCreateProcedureQuery CreateProcedure()
        {
            return CreateProcedure(string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a new CREATE PROCEDURE statement with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateProcedureQuery CreateProcedure(string name)
        {
            return CreateProcedure(string.Empty, name);
        }

        /// <summary>
        /// Creates a new CREATE PROCEDURE statement with the specified owner and name
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateProcedureQuery CreateProcedure(string owner, string name)
        {
            DBCreateProcedureQuery q = new DBCreateProcedureQueryRef();
            q.Name = name;
            q.Owner = owner;

            return q;
        }

        #endregion

    }


    public class DBCreateProcedureQueryRef : DBCreateProcedureQuery
    {

        protected override string XmlElementName
        {
            get { return XmlHelper.CreateSproc; }
        }


#if SILVERLIGHT
        // no statement building
#else

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            bool checknotexists = (this.CheckExists == DBExistState.NotExists);
            
            builder.BeginCreate(DBSchemaTypes.StoredProcedure, this.Owner, this.Name, string.Empty, checknotexists);
            builder.StopRegisteringParameters();
            try
            {
                if (this.HasParameters)
                {
                    builder.BeginProcedureParameters();
                    bool outputSeparator = false;
                    foreach (DBParam p in this.Parameters)
                    {
                        if (outputSeparator)
                            builder.WriteReferenceSeparator();
                        builder.WriteSpace();
                        builder.WriteParameter(p, true);
                        outputSeparator = true;
                    }
                    builder.EndProcedureParameters();
                }
                builder.BeginNewLine();
                builder.BeginEntityDefinition();
                if (this.Script != null)
                {
                    this.Script.BuildStatement(builder);
                }
                builder.EndEntityDefinition();

                builder.EndCreate(DBSchemaTypes.StoredProcedure, checknotexists);
            }
            finally
            {
                builder.ResumeRegisteringParameters();
            }
            //Clear the parameters as we are declaring them rather than actually using them
            return true;
        }

#endif

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsElementMatch(XmlHelper.SprocParams, reader, context))
            {
                DBParamList paramlist = new DBParamList();
                while (reader.Read())
                {
                    if (IsElementMatch(XmlHelper.Parameter, reader, context))
                    {
                        DBParam p = DBParam.Param();
                        p.ReadXml(reader, context);
                        if (context.Parameters.Contains(p.Name))
                            p = context.Parameters[p.Name];
                        else
                            context.Parameters.Add(p);
                        paramlist.Add(p);
                    }
                    else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlHelper.SprocParams)
                        break;
                }                
                this.Parameters = paramlist;
                return true;
            }
            else if(IsElementMatch(XmlHelper.Script,reader, context))
            {
                DBClause c = this.ReadNextInnerClause(reader.LocalName, reader, context);
                if (c is DBScript)
                    this.Script = (DBScript)c;
                else
                    throw new System.Xml.XmlException("Inner content of a StoredProcedure must be a Script");
                return true;
            }
            return base.ReadAnInnerElement(reader, context);
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.Name))
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);
            
            if (!string.IsNullOrEmpty(this.Owner))
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);

            return base.WriteAllAttributes(writer, context);
        }


        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (null != this.Parameters && this.Parameters.Count > 0)
            {
                this.WriteStartElement(XmlHelper.SprocParams, writer, context);
                this.Parameters.WriteXml(writer, context);
                this.WriteEndElement(XmlHelper.SprocParams, writer, context);
            }
            if (null != this.Script)
            {
                this.Script.WriteXml(writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }
    }
}
