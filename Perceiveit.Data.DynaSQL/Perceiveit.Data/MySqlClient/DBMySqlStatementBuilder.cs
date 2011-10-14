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

        #endregion

        #region ivars

        private bool _buildingsproc;
        private string _mysqldelim;
        private int _limits = -1;
        private int _offset = -1;
        
        #endregion


        #region protected virtual string ProcedureDelimiter {get;set;}

        protected virtual string ProcedureDelimiter
        {
            get { return this._mysqldelim; }
            set { this._mysqldelim = value; }
        }

        #endregion



        #region .ctor(database, properties, writer, ownswriter)

        public DBMySqlStatementBuilder(DBDatabase database, DBDatabaseProperties properties, System.IO.TextWriter writer, bool ownsWriter)
            : base(database, properties, writer, ownsWriter)
        {
            _buildingsproc = false;
            _mysqldelim = StdMySqlBlockDelimiter;
        }

        #endregion



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
                this.WriteRaw(" LIMIT ");
                this.WriteRaw(_limits.ToString());
                if (_offset > 0)
                {
                    this.Writer.Write(" OFFSET ");
                    this.Writer.Write(_offset);
                }
            }
            base.EndSelectStatement();
        }

        public override void BeginIdentifier()
        {
            this.WriteRaw("`");
        }

        public override void EndIdentifier()
        {
            this.WriteRaw("`");
        }

        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
                this.WriteRaw("LAST_INSERT_ID");
            else
                base.BeginFunction(function, name);
        }
        

        public override void WriteParameterReference(string paramname)
        {
            if (this._buildingsproc)
                this.WriteRaw(paramname);
            else
                base.WriteParameterReference(paramname);
        }

        public override void BeginScript()
        {
            //this.Writer.Write("BEGIN");
            this.IncrementStatementBlock();
            //this.BeginNewLine();
        }

        public override void EndScript()
        {
            this.DecrementStatementBlock();
            //this.BeginNewLine();
            //this.Writer.WriteLine("END");
        }

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

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(schemaTable.Catalog) == false)
            {
                this.WriteRaw(schemaTable.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }
            if (string.IsNullOrEmpty(schemaTable.Schema) == false)
            {
                this.WriteRaw(schemaTable.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }
            this.WriteRaw(schemaTable.Name);
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
                    this.AppendReferenceSeparator();
            }

            if (pks.Count > 1 && hasauto)
                throw new ArgumentException(Errors.MySqlAutoIncrementMustBeKey);

            if (pks.Count > 0)
            {
                this.AppendReferenceSeparator();
                this.BeginNewLine();
                this.WriteRaw("PRIMARY KEY");
                this.BeginBlock();

                index = 0;

                foreach (DBSchemaTableColumn tc in pks)
                {
                    this.BeginIdentifier();
                    this.WriteRaw(tc.Name);
                    this.EndIdentifier();

                    index++;

                    if (index < pks.Count)
                        this.AppendReferenceSeparator();
                }

                this.EndBlock();
            }

            this.DecrementStatementDepth();
            this.EndBlock();
            this.EndCreate(DBSchemaTypes.Table);
        }

        #endregion

        #region private void ScriptCreateTableColumn(DBSchemaTableColumn tc, int index)

        private void ScriptCreateTableColumn(DBSchemaTableColumn tc, int index)
        {
            if (string.IsNullOrEmpty(tc.Name))
                throw new ArgumentNullException("schemaTable.Columns[" + tc.OrdinalPosition + "].Name");
            this.BeginIdentifier();
            this.WriteRaw(tc.Name);
            this.EndIdentifier();
            this.WriteRaw(" ");

            string options;
            string type = this.GetNativeTypeForDbType(tc.DbType, tc.Size, out options);
            this.WriteRaw(type);
            if (string.IsNullOrEmpty(options) == false)
                this.WriteRaw(options);

            if (tc.Nullable == false)
                this.WriteRaw(" NOT NULL");
            if (tc.AutoAssign)
                this.WriteRaw(" AUTO_INCREMENT");
            //if (tc.PrimaryKey)
            //    this.WriteRaw(" PRIMARY KEY");

            if (tc.HasDefault && string.IsNullOrEmpty(tc.DefaultValue))
                this.WriteRaw(" DEFAULT " + tc.DefaultValue);


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
            this.WriteRaw(schemaIndex.Name);
            this.EndIdentifier();
            this.WriteRaw(" ON ");
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
                this.WriteRaw(col.ColumnName);
                this.EndIdentifier();
                this.EndOrderClause(col.SortOrder);

                index++;
                if(index < sorted.Count)
                    this.AppendReferenceSeparator();
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
                this.WriteRaw(" (");

                for (int i = 0; i < col.Count; i++)
                {
                    this.WriteSourceField(null, null, col[i].Name, null);
                    if (i < col.Count - 1)
                        this.WriteRaw(", ");
                }
                this.WriteRaw(") ");
            }

            this.WriteRaw(" AS ");

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
            string ntype = GetNativeTypeForDbType(type, size, out opt);
            this.WriteRaw("RETURNS " + ntype);
            if (string.IsNullOrEmpty(opt) == false)
                this.WriteRaw(opt);
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
                this.WriteRaw(" (");


                for (int i = 0; i < param.Count; i++)
                {
                    this.BeginNewLine();
                    DBSchemaParameter p = param[i];
                    
                    if (annotateDirection)
                    {
                        switch (p.Direction)
                        {
                            case System.Data.ParameterDirection.Input:
                                this.WriteRaw("IN ");
                                break;
                            case System.Data.ParameterDirection.InputOutput:
                                this.WriteRaw("INOUT ");
                                break;
                            case System.Data.ParameterDirection.Output:
                                this.WriteRaw("OUT ");
                                break;
                            case System.Data.ParameterDirection.ReturnValue:
                                //Skip the return parameters
                                continue;
                                
                            default:
                                throw new ArgumentOutOfRangeException("DBSchemaParameter.Direction");
                        }
                    }
                    this.WriteParameterReference(p.InvariantName);
                    this.WriteRaw(" ");
                    string options;
                    string type = this.GetNativeTypeForDbType(p.DbType, p.ParameterSize, out options);
                    this.WriteRaw(type);
                    if (string.IsNullOrEmpty(options) == false)
                        this.WriteRaw(options);
                    if (i < param.Count - 1)
                        this.WriteRaw(", ");

                }
                this.WriteRaw(") ");

            }
            this.BeginNewLine();
        }

        #endregion
    }
}
