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

namespace Perceiveit.Data
{
    /// <summary>
    /// The Perceiveit.Data.DynaSQL library is a Data Access Layer and dynamic query generator that dramatically reduces the boiler plate code,
    /// with support for profiling, and database schema analysis.
    /// </summary>
    /// <remarks>
    /// <para>The first part of using the library is to get a reference to the <see cref="DBDatabase" /> using one of the overloaded <c>DBDatabase.Create</c> methods.<br/>
    /// With this instance you can execute any of the standard command methods without needing remember to open or dispose of any connections, commands etc.</para>
    /// <code>DBDatabase northwind = DBDatabase.Create("configConnection");</code>
    /// <para>As the DBDatabase instance does not need to be disposed, it can be held as a reference anywhere (including a static variable).</para>
    /// <code>
    ///       DBQuery sel = DBQuery.Select().AllFields()
    ///                      .From("mytable")
    ///                      .Where("myfield",Compare.Equals,DBConst(1));
    ///                      
    ///       northwind.ExecuteRead(sel,delegate(DBDataReader reader){ //do your read here 
    ///                         });
    /// </code>
    /// <para>Want to run dynamic sql queries? Use the <see cref="Perceiveit.Data.Query"/> namespace for the <see cref="Perceiveit.Data.Query.DBQuery"/> class.</para>
    /// </remarks>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// The Perceiveit.Data.Query namespace conatins the classes used to build the dynamic SQL statements that can be executed against 
    /// any of the supported database providers.
    /// </summary>
    /// <remarks>The <see cref="Perceiveit.Data.Query.DBQuery" /> class has the static methods that 
    /// enable the writing of SQL statements in a syntax similar to native SQL.
    /// <para>Use <see cref="DBQuery.Select()"/>... to start a select statement and keep using this dot 
    /// notation to include output fields, table joins, and filters to the statement.</para>
    /// <code>
    /// DBQuery sel = DBQuery.Select().AllFields()
    ///                      .From("mytable")
    ///                      .Where("myfield",Compare.Equals,DBConst(1));
    /// </code>
    /// <para>This select query can now be executed against the <see cref="Perceiveit.Data.DBDatabase"/> instance</para>
    /// </remarks>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.Profile
{
    /// <summary>
    /// The Perceiveit.Data.Profile namespace contains the classes that enable profiling of the 
    /// DBDatabase instance's interaction with an external database
    /// </summary>
    /// <remarks>Use the <see cref="DBDatabase.AttachProfiler"/> method to attach a new instance of the <see cref="IDBProfiler"/> explicity, or 
    /// define the profilers in your configuration file and atuomatically attach when new instances of DBDatabase are created.
    /// <para>Custom profilers are easy to create - just inherit from the <see cref="DBProfilerBase"/> class.</para>
    /// </remarks>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.Schema
{

    /// <summary>
    /// The Perceiveit.Data.Schema namespace enables the extraction and analysis of a database schema at runtime. 
    /// Use the <see cref="DBDatabase.GetSchemaProvider"/> method to get a reference to the <see cref="DBSchemaProvider"/> that
    /// will support the extraction of tables,views, stored procedures and even foreign keys and indexes.
    /// </summary>
    /// <remarks>The <see cref="DBSchemaProvider"/> returns 2 types of information. <para>The <see cref="DBSchemaItemRef"/> that 
    /// is a reference to any item in the database's schema (Table, view, sproc etc) and is loaded through
    /// the use of the GetAll... methods.</para>
    /// <para> An item ref can then be used (along with names) to get a DBSchemaTable, DBSchemaSproc etc with the provider, that conatins
    /// the full information about a schema item</para></remarks>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.Configuration
{
    /// <summary>
    /// Namespace for the configuration classes for dynamically extending the support of DBDatabase providers and implmentations
    /// </summary>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.MySqlClient
{
    /// <summary>
    /// Contains the classes specific to the MySql database engine. Including the implmentation provider, schema builder and statement builder
    /// </summary>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// Contains the classes specific to the Ole database engine (MS Access support defined only). Including the implmentation provider, schema builder and statement builder
    /// </summary>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.SqlClient
{
    /// <summary>
    /// Contains the classes specific to the SqlClient (MS Sql Server) database engine. Including the implmentation provider, schema builder and statement builder
    /// </summary>
    internal static class NamespaceDoc
    {
    }
}

namespace Perceiveit.Data.SqLite
{
    /// <summary>
    /// Contains the classes specific to the Sqlite database engine. Including the implmentation provider, schema builder and statement builder
    /// </summary>
    internal static class NamespaceDoc
    {
    }
}
