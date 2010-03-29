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
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqLite
{
    internal class DBSqLiteSchemaProvider : DBSchemaProvider
    {
        //
        // .ctor
        //

        internal DBSqLiteSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
            : base(forDatabase, properties)
        {
        }

        //
        // DoGetSchemaItems + Support methods
        //

        #region protected override IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types)

        protected override IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types)
        {
            List<DBSchemaItemRef> items = new List<DBSchemaItemRef>();
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                if (IsSchemaType(types, DBSchemaTypes.Table))
                    this.LoadTableRefs(con, items);
                if (IsSchemaType(types, DBSchemaTypes.View))
                    this.LoadViewRefs(con, items);
                if (IsSchemaType(types, DBSchemaTypes.Index))
                    this.LoadIndexRefs(con, items);
            }

            return items;
        }

        #endregion

        #region protected int LoadTableRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the Tables for a specific Database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadTableRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaTablesName);
            DataColumn catalog = dt.Columns["TABLE_CATALOG"];
            DataColumn schema = dt.Columns["TABLE_SCHEMA"];
            DataColumn name = dt.Columns["TABLE_NAME"];
            DataColumn type = dt.Columns["TABLE_TYPE"];

            foreach (DataRow row in dt.Rows)
            {
                if (row[type].ToString().ToUpper() == "TABLE")
                {
                    DBSchemaItemRef iref = new DBSchemaItemRef();
                    iref.Catalog = row[catalog].ToString();
                    iref.Schema = row[schema].ToString();
                    iref.Name = row[name].ToString();
                    iref.Type = DBSchemaTypes.Table;

                    addtocollection.Add(iref);
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region protected int LoadViewRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the Views for a specific database
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadViewRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaViewsName);
            DataColumn catalog = dt.Columns["TABLE_CATALOG"];
            DataColumn schema = dt.Columns["TABLE_SCHEMA"];
            DataColumn name = dt.Columns["TABLE_NAME"];

            foreach (DataRow row in dt.Rows)
            {
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                iref.Type = DBSchemaTypes.View;

                addtocollection.Add(iref);
                count++;
            }
            return count;
        }

        #endregion

        #region protected int LoadIndexRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the indexes for a specific database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadIndexRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaIndexesName);
            DataColumn catalog = dt.Columns["INDEX_CATALOG"];
            DataColumn schema = dt.Columns["INDEX_SCHEMA"];
            DataColumn name = dt.Columns["INDEX_NAME"];

            foreach (DataRow row in dt.Rows)
            {
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                iref.Type = DBSchemaTypes.Index;

                addtocollection.Add(iref);
                count++;
            }
            return count;
        }

        #endregion


        protected override DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef forRef)
        {
            throw new NotImplementedException();
        }

        //
        // DoCheckExists + support methods
        // 

        protected override bool DoCheckExists(DBSchemaItemRef itemRef)
        {
            throw new NotImplementedException();
        }

        //
        // DoGenerateCreateScript + support methods
        //

        protected override string DoGenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script, DBStatementBuilder builder)
        {
            throw new NotImplementedException();
        }

        //
        // DoGenerateDropScript + support methods
        //

        protected override string DoGenerateDropScript(DBSchemaItemRef itemRef, DBStatementBuilder builder)
        {
            throw new NotImplementedException();
        }

        protected override DbType GetDbTypeForSqlType(string sqlType)
        {
            throw new NotImplementedException();
        }
    }
}
