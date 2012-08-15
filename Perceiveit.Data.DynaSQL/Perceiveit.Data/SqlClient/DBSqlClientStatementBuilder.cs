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
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqlClient
{
    /// <summary>
    /// Generates SQL statements for the System.Data.SQLClient database engine
    /// </summary>
    /// <remarks>The base DBStatementBuilder uses the SQLClient as a reference model - not because it is a standard, 
    /// but because the engine supports the DbCommandBehavior option which all .NET providers should support to be fully compliant
    /// <para>Other implementations do have to override methods but with this one we don't</para></remarks>
    public class DBSQLClientStatementBuilder : DBStatementBuilder
    {
        private class RangeOptions
        {
            public int Count = 0;
            public int Offset = 0;
            public List<string> Fields = new List<string>();
            public List<string> Sorts = new List<string>();
            public int OrderStartIndex = 0;
            public int OderEndIndex = 0;
            public int SelectInjectOffset = 0;
            public StringBuilder Builder;

            public bool CollectingFields = false;
            public bool CollectingSorts = false;

        }

        private RangeOptions _range = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <param name="properties"></param>
        /// <param name="tw"></param>
        /// <param name="ownswriter"></param>
        internal protected DBSQLClientStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }

        public override void WriteTop(double count, double offset, TopType topType)
        {
            if (topType == TopType.Range)
            {
                this.IncrementStatementDepth();
                StringBuilder builder = null;
                System.IO.TextWriter writer = this.Writer;

                if (writer is System.IO.StringWriter)
                {
                    System.IO.StringWriter sw = writer as System.IO.StringWriter;
                    builder = sw.GetStringBuilder();
                }

                _range = new RangeOptions() { Count =  (int)count -1, //as we use between and we start from offset 1 - this works
                                              Offset = (int)offset, 
                                              SelectInjectOffset = builder.Length, 
                                              Builder = builder,
                                              CollectingFields = true};
                
            }
            else
                base.WriteTop(count, offset, topType);
        }

        public override void WriteAllFieldIdentifier(string schemaOwner, string sourceTable)
        {
            if (null != _range)
            {
                if (!string.IsNullOrEmpty(sourceTable))
                    _range.Fields.Add(String.Format("[{0}].*", sourceTable));
                else
                    _range.Fields.Add("*");
            }

            base.WriteAllFieldIdentifier(schemaOwner, sourceTable);
        }

        public override void BeginFromList()
        {
            if (_range != null)
                _range.CollectingFields = false;

            base.BeginFromList();
        }

        public override void BeginOrderStatement()
        {
            if (null != _range)
            {
                _range.OrderStartIndex = _range.Builder.Length;
                _range.CollectingSorts = true;
            }
            base.BeginOrderStatement();
        }

        public override void EndOrderStatement()
        {
            if (null != _range)
            {
                _range.OderEndIndex = _range.Builder.Length;
                _range.CollectingSorts = false;
            }
            base.EndOrderStatement();
        }

        public override void WriteSourceField(string schemaOwner, string sourceTable, string columnName, string alias)
        {
            if (null != _range)
            {
                string full = string.IsNullOrEmpty(alias) ? GetFieldString(schemaOwner, sourceTable, columnName) : alias;
                if (_range.CollectingFields)
                    _range.Fields.Add(full);
                else if(_range.CollectingSorts && _range.Fields.IndexOf(full) < 0)
                    _range.Sorts.Add(full);
            }

            base.WriteSourceField(schemaOwner, sourceTable, columnName, alias);
        }

        const string ROWSELECT_FORMAT = " ROW_NUMBER() OVER( {0}) AS {1}, {2}";
        const string ROWNUM_VAR = "_rowNum";
        const string ROWSELECT_TABLENAME = "_rowtable";
        const string OUTER_ROWSELECT_FRONT = "SELECT {0} FROM (";
        const string OUTER_ROWSELECT_END = " AS {0} WHERE {1} BETWEEN {2} AND {4} {5}";

        public override void EndSelectStatement()
        {
            base.EndSelectStatement();
            if (_range != null)
            {
                string[] fields = _range.Fields.ToArray();
                string order = "";
                if (_range.OrderStartIndex > 0)
                {
                    if (_range.OderEndIndex <= 0)
                        _range.OderEndIndex = _range.Builder.Length;
                    order = _range.Builder.ToString(_range.OrderStartIndex, _range.OderEndIndex - _range.OrderStartIndex);
                    _range.Builder.Remove(_range.OrderStartIndex, _range.OderEndIndex - _range.OrderStartIndex);
                }
                else
                    order = string.Join(",", fields);


                //Inject the SELECT ROW_NUMBER() OVER(...) AS _rowNum, [Sort1], [Sort2] into the SQL statement
                string extrasorts = _range.Sorts.Count == 0 ? "" : (string.Join(", ", _range.Sorts.ToArray()) + ", ");
                string rowselect = string.Format(ROWSELECT_FORMAT, order, ROWNUM_VAR, extrasorts);
                
                _range.Builder.Insert(_range.SelectInjectOffset, rowselect);

                rowselect = string.Format(OUTER_ROWSELECT_FRONT, string.Join(", ",fields));
                _range.Builder.Insert(0, rowselect);
                rowselect = string.Format(OUTER_ROWSELECT_END, ROWSELECT_TABLENAME, ROWNUM_VAR,
                                                               _range.Offset, _range.Count, _range.Count + _range.Offset
                                                               , order);

                _range.Builder.Append(rowselect);
                this.DecrementStatementDepth();
                _range = null;
            }
        }



        private string GetFieldString(string schemaOwner, string sourceTable, string columnName)
        {
            if (!string.IsNullOrEmpty(schemaOwner))
            {
                return "[" + schemaOwner + "].[" + sourceTable + "].[" + columnName + "]";
            }
            else if (!string.IsNullOrEmpty(sourceTable))
            {
                return "[" + sourceTable + "].[" + columnName + "]";
            }
            else
            {
                return "[" + columnName + "]";
            }
        }

        protected override string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, int accuracy, DBColumnFlags flags, out string options)
        {
            string native;
            options = "";
            switch (dbType)
            {
                case System.Data.DbType.String:

                    if (setSize <= 0)
                    {
                        native = "NVARCHAR";
                        options = "(MAX)";
                    }
                    else if (setSize < 4000)
                    {
                        native = "NVARCHAR";
                        options = "(" + setSize.ToString() + ")";
                    }
                    else
                        native = "NTEXT";
                    break;
                    
                case System.Data.DbType.StringFixedLength:
                    if (setSize <= 0)
                        setSize = 255;
                    if (setSize < 4001)
                    {
                        native = "NCHAR";
                        options = "(" + setSize.ToString() + ")";
                    }
                    else
                        native = "NTEXT";
                    break;
                
                case System.Data.DbType.AnsiString:
                    if (setSize <= 0)
                    {
                        native = "VARCHAR";
                        options = "(MAX)";
                    }
                    else if (setSize < 8000)
                    {
                        native = "VARCHAR";
                        options = "(" + setSize.ToString() + ")";
                    }
                    else
                        native = "TEXT";
                    break;

                case System.Data.DbType.AnsiStringFixedLength:
                   if (setSize <= 0)
                        setSize = 255;
                    if (setSize < 8001)
                    {
                        native = "CHAR";
                        options = "(" + setSize.ToString() + ")";
                    }
                    else
                        native = "TEXT";
                    break;

                case System.Data.DbType.Single:
                    if (setSize <= 0)
                        native = "REAL";
                    else
                    {
                        native = "FLOAT";
                        options = "(" + setSize.ToString();
                        if (accuracy > 0)
                            options += "," + accuracy.ToString();
                        options += ")";
                    }
                    break;

                case System.Data.DbType.Double:
                    if (setSize <= 0)
                        native = "REAL";
                    else
                    {
                        native = "FLOAT";
                        options = "(" + setSize.ToString();
                        if (accuracy > 0)
                            options += "," + accuracy.ToString();
                        options += ")";
                    }
                    break;

                case System.Data.DbType.Currency:
                    native = "MONEY";
                    break;

                default:
                    native = base.GetNativeTypeForDbType(dbType, setSize, accuracy, flags, out options);
                    break;
            }

            return native;
        }

    }
}
