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
using System.Data;
using System.Data.Common;
using Perceiveit.Data.Query;

namespace Perceiveit.Data
{
    public partial class DBDatabase
    {
        
        //
        // PopulateDataSet with DBQuery
        //

        #region public void PopulateDataSet(DataSet ds, DBQuery query)

        /// <summary>
        /// Populates the specified DataSet with the results of the query, overwriting any changes.
        /// Multiple results sets from the query will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="query">The Query to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, DBQuery query)
        {
            this.PopulateDataSet(ds, query, LoadOption.OverwriteChanges);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DBQuery query, LoadOption option)

        /// <summary>
        /// Populates the specified DataSet with the results of the query using the specified load option.
        /// Multiple results sets from the query will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="query">The Query to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, DBQuery query, LoadOption option)
        {
            string[] tables = ExtractTableNames(ds);
            this.PopulateDataSet(ds, query, option, tables);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DBQuery query, params string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the query using the specified load option.
        /// Multiple results sets from the query will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        /// <param name="query">The Query to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, DBQuery query, params string[] tables)
        {
            PopulateDataSet(ds, query, LoadOption.OverwriteChanges, tables);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DBQuery query, LoadOption option, params string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the query using the specified load option.
        /// Multiple results sets from the query will be populated in the order of the tables specified.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="query">The Query to use to populate the data</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, DBQuery query, LoadOption option, params string[] tables)
        {
            if (null == ds)
                throw new ArgumentNullException("ds");
            if (null == query)
                throw new ArgumentNullException("query");

            this.ExecuteRead(query, delegate(DbDataReader reader)
            {
                ds.Load(reader, option, tables);
                return null;
            });

        }

        #endregion


        //
        // Populate DataSet with SQL text
        //

        #region public void PopulateDataSet(DataSet ds, string text)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text, overwriting any changes.
        /// Multiple results sets from the command text will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="text">The command text to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, string text)
        {
            using (DbCommand cmd = this.CreateCommand(text))
            {
                this.PopulateDataSet(ds, cmd);
            }
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, LoadOption option)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text using the specified load option.
        /// Multiple results sets from the command text will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="text">The command text to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, string text, LoadOption option)
        {
            this.PopulateDataSet(ds, text, CommandType.Text, option);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text overwriting any existing rows.
        /// Multiple results sets from the command text will be populated in the order of the specified table names.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, string text, string[] tables)
        {
            this.PopulateDataSet(ds, text, CommandType.Text, tables);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, LoadOption option, string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text using the specified load option.
        /// Multiple results sets from the command text will be populated in the order of the specified table names.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, string text, LoadOption option, string[] tables)
        {
            this.PopulateDataSet(ds, text, CommandType.Text, option, tables);
        }

        #endregion


        #region public void PopulateDataSet(DataSet ds, string text, CommandType type)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text overwriting any existing rows.
        /// Multiple results sets from the command text will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="type">The type of the command text</param>
        public void PopulateDataSet(DataSet ds, string text, CommandType type)
        {
            this.PopulateDataSet(ds, text, type, LoadOption.OverwriteChanges);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, CommandType type, LoadOption option)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text using the specified load option.
        /// Multiple results sets from the command text will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="type">The type of the command text</param>
        public void PopulateDataSet(DataSet ds, string text, CommandType type, LoadOption option)
        {
            this.PopulateDataSet(ds, text, type, option, this.ExtractTableNames(ds));
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, CommandType type, string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text using overwriting any existing rows.
        /// Multiple results sets from the command text will be populated in the order of the specified table names.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="type">The type of the command text</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, string text, CommandType type, string[] tables)
        {
            this.PopulateDataSet(ds, text, type, LoadOption.OverwriteChanges, tables);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, string text, CommandType type, LoadOption option, string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the command text using the specified load option.
        /// Multiple results sets from the command text will be populated in the order of the specified table names.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="text">The command text to use to populate the data</param>
        /// <param name="type">The type of the command text</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, string text, CommandType type, LoadOption option, string[] tables)
        {
            if (null == ds)
                throw new ArgumentNullException("ds");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                this.PopulateDataSet(ds, cmd, option, tables);
            }
        }

        #endregion


        //
        // Populate DataSet with DbCommand
        //

        #region public void PopulateDataSet(DataSet ds, DbCommand cmd)

        /// <summary>
        /// Populates the specified DataSet with the results of the command overwriting any changes.
        /// Multiple results sets from the command will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="cmd">The DbCommand to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, DbCommand cmd)
        {
            this.PopulateDataSet(ds, cmd, LoadOption.OverwriteChanges);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DbCommand cmd, LoadOption option)

        /// <summary>
        /// Populates the specified DataSet with the results of the command using the specified load option.
        /// Multiple results sets from the command will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="cmd">The DbCommand to use to populate the data</param>
        public void PopulateDataSet(DataSet ds, DbCommand cmd, LoadOption option)
        {
            this.PopulateDataSet(ds, cmd, option, this.ExtractTableNames(ds));
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DbCommand cmd, params string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the command overwriting any changes.
        /// Multiple results sets from the command will be populated in the order of the tables specified.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="cmd">The DbCommand to use to populate the data</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, DbCommand cmd, params string[] tables)
        {
            this.PopulateDataSet(ds, cmd, LoadOption.OverwriteChanges, tables);
        }

        #endregion

        #region public void PopulateDataSet(DataSet ds, DbCommand cmd, LoadOption option, params string[] tables)

        /// <summary>
        /// Populates the specified DataSet with the results of the query using the specified load option.
        /// Multiple results sets from the query will be populated in the order of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to Populate</param>
        /// <param name="option">Specifies the manner in which imported rows are handled</param>
        /// <param name="cmd">The DbCommand to use to populate the data</param>
        /// <param name="tables">The array of table names that are to be populated by the results of the query</param>
        public void PopulateDataSet(DataSet ds, DbCommand cmd, LoadOption option, params string[] tables)
        {
            this.ExecuteRead(cmd, delegate(DbDataReader reader)
            {
                ds.Load(reader, option, tables);
                return null;
            });
        }

        #endregion

    }
}
