using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceiveit.Data;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.Oracle
{
    public class DBOracleStatementBuilder : Query.DBStatementBuilder
    {

        #region const

        private const string FUNC_SEQUENCE_CURRVALUE = "CURRVAL";
        private const string FUNC_SYSDATE = "SYSDATE";
        private const string FUNC_SEQUENCE_NEXTVALUE = "NEXTVAL";

        //
        // wrapping the select statement to use the ROWNUM for selecting TOP N or Range
        //

        private const string SELECTTOPSTART = "SELECT ";
        private const string SELECTTOPMIDDLE = " FROM (";
        private const string SELECTTOPNEND = ") WHERE \"" + ROWNUMALIAS + "\" <= {0}";
        private const string SELECTTOPRANGEEND = ") WHERE \"" + ROWNUMALIAS + "\" >= {0} AND \"" + ROWNUMALIAS + "\" < {1}";
        private const string SELECTTOPFIELDSTEM = "__assign";
        private const string SELECTROWNUM = "ROWNUM";
        private const string ROWNUMALIAS = "__rownum";

        #endregion

        #region ivars

        /// <summary>
        /// True if the current function that is being written should not have parentheses
        /// </summary>
        private bool _isFunctionThatExcludesParentheses = false;

        /// <summary>
        /// True when we are creating a new procedure 
        /// and we are writing the list of parameters that should be accepted
        /// </summary>
        private bool _isInProcedureParameters = false;

        /// <summary>
        /// counter for the number of inner BEGIN scripts
        /// </summary>
        private int _scriptDepth = 0;

        /// <summary>
        /// counter for the number of declarations in a script
        /// </summary>
        private int _scriptDeclareCount = 0;

        //
        // select TOP ivars
        //

        /// <summary>
        /// True if we are currently writing the select list fields
        /// </summary>
        private bool _isInSelectList = false;

        /// <summary>
        /// True if the current select statement has a top n value
        /// </summary>
        private bool _hastop = false;

        /// <summary>
        /// The position on the inner StringBuilder that the SELECT ... statement starts
        /// </summary>
        private int _selectstart = -1;

        /// <summary>
        /// The requested top n
        /// </summary>
        private double _topcount = -1;
        /// <summary>
        /// The requested top range offset 
        /// </summary>
        private double _offsetcount = -1;

        /// <summary>
        /// The requested top type (Count or Range only are supported)
        /// </summary>
        private TopType _toptype = (TopType)0;

        /// <summary>
        /// True if the Select has an order statement
        /// </summary>
        private bool _hasorder = false;

        /// <summary>
        /// List of Receivers in the select statement (Assignments on the SELECT)
        /// </summary>
        private List<DBClause> _assignReceivers = null;

        /// <summary>
        /// List of Values in the select statement
        /// </summary>
        private List<DBClause> _assignValues = null;

        #endregion

        public DBOracleStatementBuilder(DBDatabase db, DBDatabaseProperties props, 
                                        System.IO.TextWriter writer, bool ownsWriter)
            : base(db, props, writer, ownsWriter)
        {
            
        }

        
        public override void BeginIdentifier()
        {
            this.WriteRaw("\"");
        }

        public override void EndIdentifier()
        {
            this.WriteRaw("\"");
        }

        /// <summary>
        /// Oracle expects table name alias's to be straight after 
        /// the identifier, rather than with and ... AS ALIAS
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="alias"></param>
        public override void WriteSourceTable(string catalog, string schemaOwner, string sourceTable, string alias)
        {
            base.WriteSourceTable(catalog, schemaOwner, sourceTable, String.Empty);

            if (!string.IsNullOrEmpty(alias))
            {
                this.WriteSpace();
                this.BeginIdentifier();
                this.WriteRaw(alias);
                this.EndIdentifier();
            }
        }

        /// <summary>
        /// Extends to support SEQUENCEs
        /// </summary>
        /// <param name="type"></param>
        protected override void WriteDropType(DBSchemaTypes type)
        {
            if(type == DBSchemaTypes.Sequence)
            {
                this.WriteDropType("SEQUENCE");
            }
            else
                base.WriteDropType(type);
        }

        /// <summary>
        /// Extends to Support SEQUENCEs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <param name="isconstraint"></param>
        protected override void WriteCreateType(DBSchemaTypes type, string options, bool isconstraint)
        {
            if (type == DBSchemaTypes.Sequence)
            {
                string native = "SEQUENCE";
                this.WriteCreateType(native, options, isconstraint);
            }
            else
                base.WriteCreateType(type, options, isconstraint);
        }

        #region public override void WriteSequenceOption(DBSequenceBuilderOption option) + 1 overload

        /// <summary>
        /// Writes the specific sequence option to the output
        /// </summary>
        /// <param name="option"></param>
        public override void WriteSequenceOption(DBSequenceBuilderOption option)
        {
            this.WriteSequenceOption(option, 0);
        }

        /// <summary>
        /// Writes the specific sequence option and value to the output
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public override void WriteSequenceOption(DBSequenceBuilderOption option, int value)
        {
            switch (option)
            {
                case DBSequenceBuilderOption.Minimum:
                    this.WriteRaw(" MINVALUE ");
                    this.WriteRaw(value.ToString());
                    break;
                case DBSequenceBuilderOption.Maximim:
                    this.WriteRaw(" MAXVALUE ");
                    this.WriteRaw(value.ToString());
                    break;
                case DBSequenceBuilderOption.StartValue:
                    this.WriteRaw(" START WITH ");
                    this.WriteRaw(value.ToString());
                    break;
                case DBSequenceBuilderOption.Cycling:
                    this.WriteRaw(" CYCLE");
                    break;
                case DBSequenceBuilderOption.NoCycling:
                    this.WriteRaw(" NOCYCLE");
                    break;
                case DBSequenceBuilderOption.Ordered:
                    this.WriteRaw(" ORDER");
                    break;
                case DBSequenceBuilderOption.NotOrdered:
                    this.WriteRaw(" NOORDER");
                    break;
                case DBSequenceBuilderOption.Increment:
                    this.WriteRaw(" INCREMENT BY ");
                    this.WriteRaw(value.ToString());
                    break;
                case DBSequenceBuilderOption.Cache:
                    this.WriteRaw(" CACHE ");
                    this.WriteRaw(value.ToString());
                    break;
                case DBSequenceBuilderOption.NoCaching:
                    this.WriteRaw(" NOCACHE");
                    break;
                default:
                    break;
            }
        }

        #endregion

        public override string RegisterParameter(Query.IDBValueSource source)
        {
            string name = base.RegisterParameter(source);
            return name;
        }


        public override void BeginCreate(DBSchemaTypes type, string owner, string name, string options, bool checknotexists)
        {
            base.BeginCreate(type, owner, name, options, checknotexists);
        }

        public override void BeginCheckExists(DBSchemaTypes type, string owner, string name)
        {
            throw new NotSupportedException(Errors.CheckExistsIsNotSupported);
        }

        public override void BeginCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            throw new NotSupportedException(Errors.CheckNotExistsIsNotSupported);
        }
        /// <summary>
        /// Oracle can't handle parameters that start with _ (underscore);
        /// </summary>
        /// <returns></returns>
        public override string GetUniqueParameterName()
        {
            return "orc_param" + this.GetNextID();
        }

       
        /// <summary>
        /// Oracle expects parameters to have the ':' as a reference to the parameter in the statement
        /// but not in the command declaration.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="forstatement"></param>
        /// <returns></returns>
        public override string GetNativeParameterName(string paramName, bool forstatement)
        {
            if (forstatement && !_isInProcedureParameters)
                return base.GetNativeParameterName(paramName, forstatement);
            else
                return paramName;
        }

        
        

        public override void BeginScript()
        {
            _scriptDepth++;
            _scriptDeclareCount = 0;
            if(_scriptDepth > 1 || this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
                base.BeginScript();
        }

        public override void EndScript()
        {
            _scriptDeclareCount = 0;
            if (_scriptDepth > 1 || this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
                base.EndScript(); 
            _scriptDepth--;
        }

        public override void BeginDeclareStatement(DBParam param)
        {
            if (_scriptDeclareCount == 0)
                this.WriteRaw("DECLARE ");
            this.ParameterExclusions.Add(param.Name);
            this.WriteParameter(param, false);
            _scriptDeclareCount ++;
        }

        

        public override void BeginSelectList()
        {
            this._isInSelectList = true;
            base.BeginSelectList();
        }

        public override void EndSelectList()
        {
            if (this._hastop)
            {
                this.AppendRowNumberField();
            }
            base.EndSelectList();
            this._isInSelectList = false;
        }

        /// <summary>
        /// Appends a specific reference to the ROWNUM value in what will be a subselect
        /// </summary>
        private void AppendRowNumberField()
        {
            this.AppendReferenceSeparator();
            this.WriteRaw(SELECTROWNUM);
            this.BeginAlias();
            this.BeginIdentifier();
            this.WriteRaw(ROWNUMALIAS);
            this.EndIdentifier();
            this.EndAlias();
        }

        public override void WriteAssignValue(DBClause receiver, DBClause value)
        {
            if (_isInSelectList)
            {
                if (_hastop)
                {
                    if (null == _assignReceivers)
                        _assignReceivers = new List<DBClause>();
                    if (null == _assignValues)
                        _assignValues = new List<DBClause>();
                    _assignReceivers.Add(receiver);
                    _assignValues.Add(value);
                    value.BuildStatement(this);
                    this.WriteAlias(SELECTTOPFIELDSTEM + _assignValues.Count.ToString());
                }
                else
                {
                    value.BuildStatement(this);
                    this.WriteIntoOperator();
                    receiver.BuildStatement(this);
                }
            }
            else
                base.WriteAssignValue(receiver, value);
        }

        protected virtual void WriteIntoOperator()
        {
            this.WriteRaw(" INTO ");
        }
        

        public override void WriteNativeParameterReference(string paramname)
        {
            if (this._scriptDepth > 0 && this.ParameterExclusions.Contains(paramname))
                this.WriteRaw(paramname);
            else
                base.WriteNativeParameterReference(paramname);
        }

        public override void EndSelectStatement()
        {
            if (this._hastop)
            {
                this.WrapSelectInTopROWNUM();
                this._hastop = false;
            }
            if (this.StatementDepth > 1)
                base.EndSelectStatement();
            else
            {
                if (this._scriptDepth > 0)
                    this.WriteStatementTerminator();

                this.DecrementStatementDepth();
            }
        }

        public override void EndInsertStatement()
        {
            if (this.StatementDepth > 1)
                base.EndInsertStatement();
            else
            {
                if (this._scriptDepth > 0)
                    this.WriteStatementTerminator();

                this.DecrementStatementDepth();
            }
        }

        public override void EndUpdateStatement()
        {
            if (this.StatementDepth > 1)
                base.EndUpdateStatement();
            else
            {
                if (this._scriptDepth > 0)
                    this.WriteStatementTerminator();

                this.DecrementStatementDepth();
            }
        }

        public override void EndDeleteStatement()
        {
            if (this.StatementDepth > 1)
                base.EndDeleteStatement();
            else
            {
                if (this._scriptDepth > 0)
                    this.WriteStatementTerminator();
                this.DecrementStatementDepth();
            }
        }

        public override void EndSubStatement()
        {
            //We need to check that we are not in a CREATE PROCEDURE ... state
            if (this.StatementDepth <= 2 && this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
            {
                this.WriteStatementTerminator();
            }
            else
            {
                this.Writer.Write(") ");
                this.BeginNewLine();
            }
        }

        

        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
            {
                this.WriteRaw(FUNC_SEQUENCE_CURRVALUE);
                _isFunctionThatExcludesParentheses = true;
            }
            else if (function == Function.GetDate)
            {
                this.WriteRaw(FUNC_SYSDATE);
                _isFunctionThatExcludesParentheses = true;
            }
            else if (function == Function.NextID)
            {
                this.WriteRaw(FUNC_SEQUENCE_NEXTVALUE);
                _isFunctionThatExcludesParentheses = true;
            }
            else
                base.BeginFunction(function, name);
        }

        public override void BeginFunctionParameterList()
        {
            if (!_isFunctionThatExcludesParentheses)
                base.BeginFunctionParameterList();
        }

        public override void EndFunctionParameterList()
        {
            if (!_isFunctionThatExcludesParentheses)
                base.EndFunctionParameterList();
        }


        public override void BeginProcedureParameters()
        {
            this._isInProcedureParameters = true;
            base.BeginProcedureParameters();
        }

        public override void EndProcedureParameters()
        {
            base.EndProcedureParameters();
            this._isInProcedureParameters = false;
        }

        public override void EndFunction(Function function, string name)
        {
            base.EndFunction(function, name);
            _isFunctionThatExcludesParentheses = false;
        }

        //Strange Syntax for Executing a Stored Procedure as a CommandType.Text
        //BEGIN proc(params);END;
        //But it's supported.

        public override void BeginExecuteStatement()
        {
            
            this.Writer.Write("BEGIN ");
        }

        public override void BeginExecuteParameters()
        {
            this.Writer.Write("(");
            base.BeginExecuteParameters();
        }

        public override void EndExecuteParameters()
        {
            base.EndExecuteParameters();
            this.Writer.Write(")");
        }

        public override void EndExecuteStatement()
        {
            this.WriteStatementTerminator();
            this.Writer.Write("END;");
            base.EndExecuteStatement();
        }


        /// <summary>
        /// Writing TOP N is not supported we need to wrap in an outer SELECT with
        /// ROWNUM as one of the fields on the inner SELECT so we can select ranges.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <param name="topType"></param>
        public override void WriteTop(double count, double offset, TopType topType)
        {
            StringBuilder sb = this.GetWritersInnerBuilder();
            if (null == sb)
                throw new InvalidOperationException(Errors.CannotUseTopAsInnerWriterNotSupportPosition);
            string full = sb.ToString();
            if (full.EndsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
            {
                _selectstart = full.Length - "SELECT ".Length;
                _assignReceivers = null;
                _assignValues = null;
                _hastop = true;
                _topcount = count;
                _toptype = topType;
                _offsetcount = offset;
                _hasorder = false;
            }
            else
                throw new InvalidOperationException("Cannot use Top N outside of the start of a SELECT statement");
        }

        

        /// <summary>
        /// We are really hacking the underlying String builder. Not Pretty
        /// </summary>
        protected virtual void WrapSelectInTopROWNUM()
        {
            StringBuilder sb = this.GetWritersInnerBuilder();
            if (null == sb)
                throw new InvalidOperationException(Errors.CannotUseTopAsInnerWriterNotSupportPosition);
            if (_hastop == false)
                throw new InvalidOperationException("Calling wrap top but not set as a TOP N");
            string subselect = sb.ToString(_selectstart, sb.Length - _selectstart);
            sb.Length = _selectstart;
            sb.Append(SELECTTOPSTART);

            if (null != _assignReceivers && _assignReceivers.Count > 0)
            {
                bool lastwasselect = _isInSelectList;
                _isInSelectList = true;
                _hastop = false;
                for (int i = 0; i < _assignReceivers.Count; i++)
                {
                    if (i > 0)
                        sb.Append(",");

                    this.WriteAssignValue(_assignReceivers[i], DBField.Field(SELECTTOPFIELDSTEM + (i+1).ToString()));
                }
                _isInSelectList = lastwasselect;
            }
            else
                sb.Append("*");

            sb.Append(SELECTTOPMIDDLE);
            sb.Append(subselect);

            string end;
            if (_toptype == TopType.Range)
            {
                end = string.Format(SELECTTOPRANGEEND, _offsetcount, _offsetcount + _topcount);
            }
            else if (_toptype == TopType.Count)
            {
                end = string.Format(SELECTTOPNEND, _topcount);
            }
            else
                throw new NotSupportedException("The TOP operation '" + _toptype + " is not supported");

            sb.Append(end);
        }

        protected virtual StringBuilder GetWritersInnerBuilder()
        {
            if (this.Writer is System.IO.StringWriter)
            {
                System.IO.StringWriter sw = (System.IO.StringWriter)this.Writer;
                return sw.GetStringBuilder();
            }
            else
                return null;
        }

        protected override string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, int accuracy, DBColumnFlags flags, out string options)
        {
            if (dbType == System.Data.DbType.Date
                || dbType == System.Data.DbType.DateTime
                || dbType == System.Data.DbType.DateTime2
                || dbType == System.Data.DbType.Time)
            {
                options = string.Empty;
                return "DATE";
            }
            else if (dbType == System.Data.DbType.String)
            {
                if (setSize >= 2000)
                {
                    options = string.Empty;
                    return "CLOB";
                }
                else
                {
                    if (!_isInProcedureParameters)
                        options = "(" + setSize.ToString() + ")";
                    else
                        options = "";
                    return "NVARCHAR2";
                }
            }
            else if (dbType == System.Data.DbType.AnsiString)
            {
                if (setSize >= 4000)
                {
                    options = string.Empty;
                    return "CLOB";
                }
                else
                {
                    if (!_isInProcedureParameters)
                        options = "(" + setSize.ToString() + ")";
                    else
                        options = "";
                    return "VARCHAR2";
                }
            }
            else if (dbType == System.Data.DbType.StringFixedLength)
            {
                if (setSize >= 2000)
                {
                    options = string.Empty;
                    return "CLOB";
                }
                else
                {
                    if (!_isInProcedureParameters)
                        options = "(" + setSize.ToString() + ")";
                    else
                        options = "";
                    return "NCHAR";
                }
            }
            else if (dbType == System.Data.DbType.AnsiStringFixedLength)
            {
                if (setSize >= 4000)
                {
                    options = string.Empty;
                    return "CLOB";
                }
                else
                {
                    if (!_isInProcedureParameters)
                        options = "(" + setSize.ToString() + ")";
                    else
                        options = "";
                    return "CHAR";
                }
            }
            else if (dbType == System.Data.DbType.Int64)
            {
                options = "";
                return "NUMBER";
            }
            else
            {

                return base.GetNativeTypeForDbType(dbType, setSize, accuracy, flags, out options);
            }
        }

        public override void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)
        {
            if ((flags & DBColumnFlags.PrimaryKey) > 0)
                this.WriteRaw(" PRIMARY KEY");

            if ((flags & DBColumnFlags.HasDefault) == 0)
            {
                if ((flags & DBColumnFlags.Nullable) > 0)
                    this.WriteRaw(" NULL");
                else
                    this.WriteRaw(" NOT NULL");
            }

            if ((flags & DBColumnFlags.AutoAssign) > 0)
                ;

            if ((flags & DBColumnFlags.HasDefault) > 0)
            {
                this.WriteRaw(" DEFAULT ");
                defaultValue.BuildStatement(this);
            }
        }

        public override void WriteLiteral(System.Data.DbType dbType, object value)
        {
            if (dbType == System.Data.DbType.Time)
            {
                WriteLiteralToDate(value, "HH:mm:ss", "HH24:MI:SS");
                
            }
            else if (dbType == System.Data.DbType.Date)
            {
                WriteLiteralToDate(value, "yyyy-MM-dd", "YYYY-MM-DD");
            }
            else if (dbType == System.Data.DbType.DateTime || dbType == System.Data.DbType.DateTime2)
            {
                WriteLiteralToDate(value, "yyyy-MM-dd HH:mm:ss", "YYYY-MM-DD HH24:MI:SS");
            }
            else
                base.WriteLiteral(dbType, value);
        }

        private void WriteLiteralToDate(object value, string internalformat, string nativeformat)
        {
            if (null == value || value is DBNull)
            {
                this.WriteNull();
            }
            else
            {
                DateTime dtval = (DateTime)value;
                this.BeginFunction(Function.Unknown, "to_date");
                this.BeginFunctionParameterList();
                this.BeginDateLiteral();
                this.WriteRaw(dtval.ToString(internalformat));
                this.EndDateLiteral();
                this.AppendReferenceSeparator();
                this.BeginDateLiteral();
                this.WriteRaw(nativeformat);
                this.EndDateLiteral();
                this.EndFunctionParameterList();
                this.EndFunction(Function.Unknown, "to_date");
            }
        }

        public override void WriteOperator(Operator operation)
        {
            base.WriteOperator(operation);
        }

        public override string GetCreateOption(CreateOptions option)
        {
            if ((option & CreateOptions.Unique) > 0)
                return "UNIQUE";
            else
                return string.Empty;
        }
    }
}
