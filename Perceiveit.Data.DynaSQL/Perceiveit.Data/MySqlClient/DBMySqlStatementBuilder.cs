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
using System.Data;
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.MySqlClient
{
    /// <summary>
    /// Implements the MySql Statement builder
    /// </summary>
    public class DBMySqlStatementBuilder : DBStatementBuilder
    {

        
        #region staticvars

        public static string StdMySqlBlockDelimiter = "$$";
        public static string DefaultTableEngine = "InnoDB";
        

        #endregion

        #region ivars

        private bool _buildingsproc;
        private string _mysqldelim;
        private int _limits = -1;
        private int _offset = -1;
        private string _engine = DefaultTableEngine;

        #endregion

        #region public virtual string TableEngine {get;}
        
        public virtual string TableEngine
        {
            get { return _engine; }
        }

        #endregion

        #region protected virtual string ProcedureDelimiter {get;set;}

        protected virtual string ProcedureDelimiter
        {
            get { return this._mysqldelim; }
            set { this._mysqldelim = value; }
        }

        #endregion

        //
        // ctor
        //


        #region .ctor(database, properties, writer, ownswriter)

        public DBMySqlStatementBuilder(DBDatabase database, DBDatabaseProperties properties, System.IO.TextWriter writer, bool ownsWriter)
            : base(database, properties, writer, ownsWriter)
        {
            _buildingsproc = false;
            _mysqldelim = StdMySqlBlockDelimiter;
        }

        #endregion


        //
        // overridden methods
        //

        public override void WriteStatementTerminator()
        {
            base.WriteStatementTerminator();
        }

        protected override string GetNativeTypeForDbType(DbType dbType, int setSize, int accuracy, DBColumnFlags flags, out string options)
        {
            string value = base.GetNativeTypeForDbType(dbType, setSize, accuracy, flags, out options);
            if (dbType == DbType.String || dbType == DbType.StringFixedLength)
            {
                if (null == options)
                    options = "";
                options += " character set UTF8";
            }
            return value;
        }

        public override void WriteTop(double count, double offset, TopType topType)
        {
            if (Array.IndexOf<TopType>(this.DatabaseProperties.SupportedTopTypes, topType) < 0)
                throw new NotSupportedException("The top type '" + topType.ToString() + "' is not supported by this database");

            if (this.StatementDepth == 1)
            {
                this._limits = (int)count;

                if (topType == TopType.Range)
                    _offset = (int)offset;
            }
        }

        public override void EndSelectStatement()
        {
            if (this.StatementDepth == 1 && this._limits > 0)
            {
                this.WriteRawSQLString(" LIMIT ");
                this.Write(_limits);
                if (_offset > 0)
                {
                    this.WriteRawSQLString(" OFFSET ");
                    this.Write(_offset);
                }
            }
            base.EndSelectStatement();
        }

        

        public override char GetStartIdentifier()
        {
           return '`';
        }

        public override char GetEndIdentifier()
        {
            return '`';
        }

        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
                this.WriteRawSQLString("LAST_INSERT_ID");
            else if (function == Function.GetDate)
                this.WriteRawSQLString("NOW");
            else
                base.BeginFunction(function, name);
        }
        

        public override void WriteNativeParameterReference(string paramname)
        {
            if (this._buildingsproc)
                this.WriteObjectName(paramname);
            else
                base.WriteNativeParameterReference(paramname);
        }

        private int _laststatementdepth;

        /// <summary>
        /// 
        /// </summary>
        public override void BeginScript()
        {
            

            //If we are creating a stored procedure then we want to 
            //change the statement depth to zero so we are in a full statement as the start of the procedure only
            if (this.CurrentlyCreating == DBSchemaTypes.StoredProcedure && _laststatementdepth == 0)
            {
                this.Writer.Write("BEGIN");
                this.BeginNewLine();
                this._laststatementdepth = this.StatementDepth;
                this.StatementDepth = 0;
            }
            else
            {
                this.IncrementStatementBlock();
                this.BeginNewLine();
            }
        }

        public override void EndScript()
        {
            if (this.CurrentlyCreating == DBSchemaTypes.StoredProcedure && this.StatementDepth == 0)
            {
                this.StatementDepth = _laststatementdepth;
                _laststatementdepth = 0;
                this.BeginNewLine();
                this.Writer.WriteLine("END");
            }
            else
            {
                this.BeginNewLine();
                this.DecrementStatementBlock();
            }
        }


        /// <summary>
        /// MySQL execute is a CALL(....) statement
        /// </summary>
        public override void BeginExecuteStatement()
        {
            if (!this.DatabaseProperties.CheckSupports(DBSchemaTypes.StoredProcedure))
                throw new System.Data.DataException("Current database does not support stored procedures");

            this.Writer.Write("CALL ");
        }

        public override void BeginExecuteParameters()
        {
            this.Writer.Write(" (");
        }

        public override void EndExecuteParameters()
        {
            this.Writer.Write(")");
        }

        /// <summary>
        /// We don't declare variables, but SET them
        /// </summary>
        /// <param name="param"></param>
        public override void BeginDeclareStatement(DBParam param)
        {
            this.ParameterExclusions.Add(param.Name);
            this.WriteRawSQLString("SET ");
            bool writeType = false;
            bool writeDirection = false;
            this.WriteParameter(param, writeType, writeDirection);

            string value = GetDefaultValueForType(param.DbType);
            this.WriteOperator(Operator.Equals);
            this.WriteSpace();
            //Should be safe as we return this value internally
            this.WriteRawSQLString(value);

            
        }

        

        private string GetDefaultValueForType(DbType type)
        {
            string value;
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    value = "\"\"";
                    break;
                case DbType.Boolean:
                    value = "FALSE";
                    break;
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                    value = "0.0";
                    break;
                case DbType.Byte:
                case DbType.SByte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                case DbType.VarNumeric:
                    value = "0";
                    break;
                case DbType.Binary:
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                case DbType.DateTimeOffset:
                case DbType.Object:
                case DbType.Guid:
                case DbType.Xml:
                default:
                    value = "NULL";
                    break;
            }

            return value;
        }


        public override void EndDeclareStatement()
        {
            this.WriteStatementTerminator();
            this.BeginNewLine();
        }

        //Drop overrides for the supported syntax - DROP .... IF EXISTS
        //Rather than the default IF EXISTS (SELECT FROM INFO SCHEMA)

        public override void BeginDropStatement(DBSchemaTypes type, string owner, string name, bool checkExists)
        {
            if (checkExists)
            {
                if (type == DBSchemaTypes.Index)
                    throw new ArgumentOutOfRangeException("checkExists", string.Format(Errors.DBDoesntSupportIfExistsForThisType, "MySQL", type));
            }
            this.BeginDropStatement(type);
            
            if (checkExists)
                this.WriteCheckExists(type, owner, name);

            this.WriteSourceTable(owner, name, string.Empty);
        }

        private void WriteCheckExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString(" IF EXISTS ");
        }

        public override void BeginCheckExists(DBSchemaTypes type, string owner, string name)
        {
            if (type == DBSchemaTypes.Index)
                throw new ArgumentOutOfRangeException("checkExists", string.Format(Errors.DBDoesntSupportIfExistsForThisType, "MySQL", type));
        }

        public override void EndCheckExists(DBSchemaTypes type, string owner, string name)
        {
            
        }


        public override string GetNativeParameterName(string paramName, bool forStatement)
        {
            if (this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
                return paramName;

            else if (this.ParameterExclusions.Contains(paramName))
            {
                return "@" + paramName;
            }
            else
                return base.GetNativeParameterName(paramName, forStatement);
        }

        public override void BeginCreate(DBSchemaTypes type, string owner, string name, string options, bool checknotexists)
        {
            if (checknotexists)
            {
                if (type == DBSchemaTypes.Index || type == DBSchemaTypes.View)
                    throw new ArgumentOutOfRangeException("checknotexisits",string.Format(Errors.DBDoesntSupportIfExistsForThisType, "MySQL", type));

            }

            bool isconstraint = IsConstraintType(type);

            if (isconstraint)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    this.WriteRawSQLString("CONSTRAINT ");
                    this.BeginIdentifier();
                    this.WriteObjectName(name);
                    this.EndIdentifier();
                    this.WriteSpace();
                }
            }
            
            this.BeginCreate(type, options);

            if (checknotexists)
                this.WriteCheckNotExists(type, owner, name);

            if(!this.IsConstraintType(type))
                this.WriteSourceTable(owner, name, string.Empty); 
            
        }

        public override void EndCreate(DBSchemaTypes type, bool checknotexists)
        {
            if (type == DBSchemaTypes.Table)
            {
                this.WriteRawSQLString(" ENGINE = ");
                this.WriteObjectName(this.TableEngine);
            }

            this.DecrementStatementDepth();
            
        }

        public override void BeginEntityDefinition()
        {
            if (this.CurrentlyCreating != DBSchemaTypes.StoredProcedure)
                base.BeginEntityDefinition();
            else
            {
                //We don't want the AS keyword for StoredProcedures
            }
        }

        public override void EndEntityDefinition()
        {
            
        }

        public override string GetCreateOption(CreateOptions option)
        {
            if ((option & CreateOptions.Clustered) > 0 || (option & CreateOptions.NonClustered) > 0)
                throw new ArgumentOutOfRangeException(Errors.CannotUseClusteredOrNonClustered);

            return base.GetCreateOption(option);
        }
        private void WriteCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString(" IF NOT EXISTS ");
        }

        public override void BeginCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            if (type == DBSchemaTypes.Index || type == DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("checknotexisits", string.Format(Errors.DBDoesntSupportIfExistsForThisType, "MySQL", type));
        }

        public override void EndCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            
        }
        //
        // generate CREATE scripts
        //

        #region public override void GenerateCreateTableScript(DBSchemaTable schemaTable)

        public override void GenerateCreateTableScript(DBSchemaTable schemaTable)
        {
            if (string.IsNullOrEmpty(schemaTable.Name))
                throw new ArgumentNullException("schemaTable.Name");

            if (null == schemaTable.Columns || schemaTable.Columns.Count == 0)
                throw new ArgumentNullException("schemaTable.Columns");

            this.BeginCreate(DBSchemaTypes.Table, "");

            
            if (string.IsNullOrEmpty(schemaTable.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaTable.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                
            }
            if (string.IsNullOrEmpty(schemaTable.Schema) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaTable.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (string.IsNullOrEmpty(schemaTable.Catalog) == false)
            {
                this.AppendIdSeparator();
            }
            this.BeginIdentifier();
            this.WriteObjectName(schemaTable.Name);
            this.EndIdentifier();

            this.BeginBlock();

            List<DBSchemaTableColumn> pks = new List<DBSchemaTableColumn>();

            List<DBSchemaColumn> sorted = this.SortColumnsByOrdinal(schemaTable.Columns.GetColumns());

            int index = 0;
            this.IncrementStatementDepth();
            bool hasauto = false;

            foreach (DBSchemaTableColumn tc in sorted)
            {
                if (index > 0)
                    this.BeginNewLine();
                if (tc.PrimaryKey)
                    pks.Add(tc);

                if (tc.AutoAssign)
                {
                    hasauto = true;
                    if (!tc.PrimaryKey)
                        throw new ArgumentException(Errors.MySqlAutoIncrementMustBeKey);
                }

                
                this.ScriptCreateTableColumn(tc, index);
                
                index++;

                if(index < sorted.Count)
                    this.WriteReferenceSeparator();
            }

            if (pks.Count > 1 && hasauto)
                throw new ArgumentException(Errors.MySqlAutoIncrementMustBeKey);

            if (pks.Count > 0)
            {
                this.WriteReferenceSeparator();
                this.BeginNewLine();
                this.WriteRawSQLString("PRIMARY KEY");
                this.BeginBlock();

                index = 0;

                foreach (DBSchemaTableColumn tc in pks)
                {
                    this.BeginIdentifier();
                    this.WriteObjectName(tc.Name);
                    this.EndIdentifier();

                    index++;

                    if (index < pks.Count)
                        this.WriteReferenceSeparator();
                }

                this.EndBlock();
            }

            this.DecrementStatementDepth();
            this.EndBlock();
            this.EndCreate(DBSchemaTypes.Table, false);
        }

        #endregion

        #region private void ScriptCreateTableColumn(DBSchemaTableColumn tc, int index)

        private void ScriptCreateTableColumn(DBSchemaTableColumn tc, int index)
        {
            if (string.IsNullOrEmpty(tc.Name))
                throw new ArgumentNullException("schemaTable.Columns[" + tc.OrdinalPosition + "].Name");
            this.BeginIdentifier();
            this.WriteObjectName(tc.Name);
            this.EndIdentifier();
            this.WriteSpace();

            string options;
            string type = this.GetNativeTypeForDbType(tc.DbType, tc.Size, tc.Precision, tc.ColumnFlags, out options);

            this.WriteRawSQLString(type);
            if (string.IsNullOrEmpty(options) == false)
                this.WriteRawSQLString(options);

            if (tc.Nullable == false)
                this.WriteRawSQLString(" NOT NULL");
            if (tc.AutoAssign)
                this.WriteRawSQLString(" AUTO_INCREMENT");
            //if (tc.PrimaryKey)
            //    this.WriteRaw(" PRIMARY KEY");

            if (tc.HasDefault && string.IsNullOrEmpty(tc.DefaultValue))
            {
                this.WriteRawSQLString(" DEFAULT ");
                this.WriteRawSQLString(tc.DefaultValue);
            }


        }

        #endregion

        #region public override void GenerateCreateIndexScript(DBSchemaIndex schemaIndex)

        public override void GenerateCreateIndexScript(DBSchemaIndex schemaIndex)
        {
            if (null == schemaIndex)
                throw new ArgumentNullException("schemaIndex");

            if (string.IsNullOrEmpty(schemaIndex.Name))
                throw new ArgumentNullException("schemaIndex.Name");

            if (schemaIndex.Type != DBSchemaTypes.Index)
                throw new ArgumentOutOfRangeException("schemaIndex.Type");

            if (null == schemaIndex.TableReference)
                throw new ArgumentNullException("schemaIndex.TableReference");

            if (schemaIndex.Columns == null)
                throw new ArgumentNullException("schemaIndex.Columns");

            else if (schemaIndex.Columns.Count == 0)
                throw new ArgumentOutOfRangeException("schemaIndex.Columns");

            if (schemaIndex.IsPrimaryKey)
                throw new NotSupportedException("Cannot create a primary key on a table using the CREATE INDEX statement with MySQL. Use the alter table methods instead");

            this.BeginCreate(DBSchemaTypes.Index, schemaIndex.IsUnique ? "UNIQUE" : "");

            this.BeginIdentifier();
            this.WriteObjectName(schemaIndex.Name);
            this.EndIdentifier();
            this.WriteRawSQLString(" ON ");
            DBSchemaItemRef tbl = schemaIndex.TableReference;

            this.WriteSourceTable(tbl.Catalog, tbl.Schema, tbl.Name, null);
            this.BeginBlock();
            DBSchemaIndexColumnCollection sorted = schemaIndex.Columns;
            
            int index = 0;
            foreach (DBSchemaIndexColumn col in sorted)
            {
                if (string.IsNullOrEmpty(col.ColumnName))
                    throw new ArgumentNullException("DBSchemaIndexColumn.Name");

                this.BeginOrderClause(col.SortOrder);
                this.BeginIdentifier();
                this.WriteObjectName(col.ColumnName);
                this.EndIdentifier();
                this.EndOrderClause(col.SortOrder);

                index++;
                if(index < sorted.Count)
                    this.WriteReferenceSeparator();
            }

            this.EndBlock();
        }

        #endregion

        #region public override void GenerateCreateViewScript(DBSchemaView schemaView, DBQuery script)

        public override void GenerateCreateViewScript(DBSchemaView schemaView, DBQuery script)
        {
            if (null == schemaView)
                throw new ArgumentNullException("schemaView");
            
            if (string.IsNullOrEmpty(schemaView.Name))
                throw new ArgumentNullException("schemaView.Name");

            if (schemaView.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("schemaView.Type");

            if (null == script)
                throw new ArgumentNullException("script");

            this.BeginCreate(DBSchemaTypes.View, null);
            this.WriteSourceTable(schemaView.Catalog, schemaView.Schema, schemaView.Name, null);

            if (schemaView.Columns != null && schemaView.Columns.Count > 0)
            {
                List<DBSchemaColumn> col = this.SortColumnsByOrdinal(schemaView.Columns.GetColumns());
                this.BeginBlock();

                for (int i = 0; i < col.Count; i++)
                {
                    this.WriteSourceField(null, null, null, col[i].Name, null);
                    if (i < col.Count - 1)
                        this.WriteReferenceSeparator();
                }
                this.EndBlock();
            }

            this.WriteRawSQLString(" AS ");

            script.BuildStatement(this);
        }

        #endregion

        #region public override void GenerateCreateFunctionScript(DBSchemaFunction schemaFunc, DBScript script)

        public override void GenerateCreateFunctionScript(DBSchemaFunction schemaFunc, DBScript script)
        {
            if (null == schemaFunc)
                throw new ArgumentNullException("schemaFunc");
            if (string.IsNullOrEmpty(schemaFunc.Name))
                throw new ArgumentNullException("schemaFunc.Name");
            if (schemaFunc.Type != DBSchemaTypes.Function)
                throw new ArgumentOutOfRangeException("schemaFunc.Type");
            if (null == script)
                throw new ArgumentNullException("script");

            this._buildingsproc = true;
            this.BeginCreate(DBSchemaTypes.Function, null);
            this.WriteSource(schemaFunc.Catalog, schemaFunc.Schema, schemaFunc.Name);

            try
            {
                this.GenerateRoutineParameters(schemaFunc, false);
                WriteFunctionReturns(schemaFunc.ReturnParameter.DbType, schemaFunc.ReturnParameter.ParameterSize);
                this.BeginNewLine();

                script.BuildStatement(this);
            }
            finally
            {
                this._buildingsproc = false;
            }

        }

        private void WriteFunctionReturns(System.Data.DbType type, int size)
        {
            string opt;
            string ntype = GetNativeTypeForDbType(type, size, -1, DBColumnFlags.Nullable, out opt);
            this.WriteRawSQLString("RETURNS ");
            this.WriteRawSQLString(ntype);
            if (string.IsNullOrEmpty(opt) == false)
                this.WriteRawSQLString(opt);
        }

        #endregion

        #region public override void GenerateCreateViewScript(DBSchemaSproc schemaView, DBScript script)

        public override void GenerateCreateProcedureScript(DBSchemaSproc schemaSproc, DBScript script)
        {
            if (null == schemaSproc)
                throw new ArgumentNullException("schemaSproc");

            if (string.IsNullOrEmpty(schemaSproc.Name))
                throw new ArgumentNullException("schemaSproc.Name");

            if (schemaSproc.Type != DBSchemaTypes.StoredProcedure)
                throw new ArgumentOutOfRangeException("schemaSproc.Type");

            if (null == script)
                throw new ArgumentNullException("script");

            this._buildingsproc = true;

            this.BeginCreate(DBSchemaTypes.StoredProcedure, null);
            this.WriteSource(schemaSproc.Catalog, schemaSproc.Schema, schemaSproc.Name);
            
            try
            {
                GenerateRoutineParameters(schemaSproc, true);

                //now just output the script as SQL
                script.BuildStatement(this);
            }
            finally
            {
                this._buildingsproc = false;
            }

        }

        private void GenerateRoutineParameters(DBSchemaRoutine schemaRoutine, bool annotateDirection)
        {
            if (schemaRoutine.Parameters != null && schemaRoutine.Parameters.Count > 0)
            {
                List<DBSchemaParameter> param = this.SortColumnsByOrdinal(schemaRoutine.Parameters);
                this.BeginBlock();


                for (int i = 0; i < param.Count; i++)
                {
                    this.BeginNewLine();
                    DBSchemaParameter p = param[i];
                    
                    if (annotateDirection)
                    {
                        switch (p.Direction)
                        {
                            case System.Data.ParameterDirection.Input:
                                this.WriteRawSQLString("IN ");
                                break;
                            case System.Data.ParameterDirection.InputOutput:
                                this.WriteRawSQLString("INOUT ");
                                break;
                            case System.Data.ParameterDirection.Output:
                                this.WriteRawSQLString("OUT ");
                                break;
                            case System.Data.ParameterDirection.ReturnValue:
                                //Skip the return parameters
                                continue;
                                
                            default:
                                throw new ArgumentOutOfRangeException("DBSchemaParameter.Direction");
                        }
                    }
                    this.WriteNativeParameterReference(p.InvariantName);
                    this.WriteSpace();
                    string options;
                    string type = this.GetNativeTypeForDbType(p.DbType, p.ParameterSize, -1, DBColumnFlags.Nullable, out options);
                    this.WriteRawSQLString(type);
                    if (string.IsNullOrEmpty(options) == false)
                        this.WriteRawSQLString(options);
                    if (i < param.Count - 1)
                        this.WriteReferenceSeparator();

                }
                this.EndBlock();

            }
            this.BeginNewLine();
        }

        #endregion

        /// <summary>
        /// Writes the flags associated with a colums data type and identity
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="defaultValue"></param>
        public override void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)
        {
            if ((flags & DBColumnFlags.PrimaryKey) > 0)
                this.WriteRawSQLString(" PRIMARY KEY");

            if ((flags & DBColumnFlags.Nullable) > 0)
                this.WriteRawSQLString(" NULL");
            else
                this.WriteRawSQLString(" NOT NULL");

            if ((flags & DBColumnFlags.AutoAssign) > 0)
                this.WriteRawSQLString(" AUTO_INCREMENT");

            if ((flags & DBColumnFlags.HasDefault) > 0)
            {
                this.WriteRawSQLString(" DEFAULT ");
                defaultValue.BuildStatement(this);
            }
        }

       
    }
}
