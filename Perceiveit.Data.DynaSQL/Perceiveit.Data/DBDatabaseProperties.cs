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
    /// Encapsulates specific information about a database such as version and supported features. 
    /// </summary>
    /// <remarks>Use the DBDatabase.GetProperties() method to get the fully populated instance for a specific connection</remarks>
    public class DBDatabaseProperties
    {
        
        /// <summary>
        /// Empty unknown database properties.
        /// </summary>
        public static readonly DBDatabaseProperties Unknown = new DBDatabaseProperties("Unknown", "Unknown", "Unknown", "Unknown", "{0}", new Version(0, 0), (DBSchemaTypes)0, true, DBParameterLayout.Named, null, null, Schema.DBSchemaInformation.CreateDefault(), new TypedOperationCollection());

        //
        // properties
        //

        #region public string ServerEdition {get;}

        private string _edition;

        /// <summary>
        /// Gets the server edition
        /// </summary>
        public string ServerEdition
        {
            get { return _edition; }
        }

        #endregion

        #region public string ProductLevel

        private string _productlevel;

        /// <summary>
        /// Gets the Product level
        /// </summary>
        public string ProductLevel
        {
            get { return _productlevel; }
        }

        #endregion

        #region public string ProductName {get;}

        private string _name;

        /// <summary>
        /// Gets the name of the database engine product
        /// </summary>
        public string ProductName
        {
            get { return _name; }
        }

        #endregion

        #region public Version ProductVersion

        private Version _vers;

        /// <summary>
        /// Gets the product version
        /// </summary>
        public Version ProductVersion
        {
            get { return _vers; }
        }

        #endregion

        #region public string ParameterFormat {get;}

        private string _paramformat;

        /// <summary>
        /// Gets the format string for converting a generic parameter name to an implementation specific name
        /// </summary>
        public string ParameterFormat
        {
            get { return _paramformat; }
        }

        #endregion

        #region public DBParameterLayout ParameterLayout {get; protected set;}

        private DBParameterLayout _paramlayout;

        /// <summary>
        /// Gets the layout type of parameters - Positional or Named
        /// </summary>
        /// <remarks>Inheritors can set this value</remarks>
        public DBParameterLayout ParameterLayout
        {
            get { return _paramlayout; }
            protected set { _paramlayout = value; }
        }

        #endregion

        #region public DBSchemaTypes SupportedSchemas {get; protected set;}

        private DBSchemaTypes _supportedschemas;

        /// <summary>
        /// Gets the supported schema types for this database - Tables, Views, Procedures, Functions etc...
        /// </summary>
        /// <remarks>Inheritors can set this value</remarks>
        public DBSchemaTypes SupportedSchemas 
        {
            get { return _supportedschemas; }
            protected set { _supportedschemas = value; }
        }

        #endregion

        #region public bool CaseSensitiveNames {get; protected set;}

        private bool _casesensitive;

        /// <summary>
        /// Gets the flag that defines if this database uses case sensitive identifiers
        /// </summary>
        /// <remarks>Inheritors can set this value</remarks>
        public bool CaseSensitiveNames
        {
            get { return _casesensitive; }
            protected set { _casesensitive = value; }
        }

        #endregion

        #region public System.Data.DbType[] SupportedDbTypes {get; protected set;}

        private System.Data.DbType[] _supportedtypes;

        /// <summary>
        /// Gets an array of supported DbTypes for this database
        /// </summary>
        /// <remarks>Inheritors can set this value</remarks>
        public System.Data.DbType[] SupportedDbTypes
        {
            get { return _supportedtypes; }
            protected set { _supportedtypes = value; }
        }

        #endregion

        #region public TopType[] SupportedTopTypes {get; protected set;}

        private TopType[] _supportedTopTypes;

        /// <summary>
        /// Gets the supported top types - Count, Percent and Range
        /// </summary>
        /// <remarks>Inheritors can set this value</remarks>
        public TopType[] SupportedTopTypes
        {
            get { return _supportedTopTypes; }
            protected set { _supportedTopTypes = value; }
        }

        #endregion

        #region public string TemporaryTableConstruct {get;set;}

        /// <summary>
        /// Gets or sets the identifer name used when creating temporary tables
        /// </summary>
        public string TemporaryTableConstruct
        {
            get { return _tempconstruct; }
            set { _tempconstruct = value; }
        }
        
        private string _tempconstruct = "TEMPORARY";

        #endregion

        #region public string TemporaryTablePrefix {get;set;}

        /// <summary>
        /// Gets or sets the name prefix for a temporary table
        /// </summary>
        public string TemporaryTablePrefix
        {
            get { return _tempprefix; }
            set { _tempprefix = value; }
        }

        private string _tempprefix = "";

        #endregion

        #region public Schema.DBSchemaInformation SchemaInformation {get;}

        private Schema.DBSchemaInformation _schemainfo;

        /// <summary>
        /// Gets the information schema lookups that can be used to check the existance of
        /// objects in the database
        /// </summary>
        public Schema.DBSchemaInformation SchemaInformation
        {
            get { return _schemainfo; }
        }

        #endregion

        #region protected TypedOperationCollection UnsupportedOps

        private TypedOperationCollection _unsupportedOps;

        /// <summary>
        /// Gets the collection of Operations that are NOT supported in this provider
        /// </summary>
        protected TypedOperationCollection UnsupportedOps
        {
            get { return _unsupportedOps; }
        }

        #endregion

        //
        // .ctor
        //

        #region internal protected DBDatabaseProperties(...)

        /// <summary>
        /// Creates a new instance of the DBDatabaseProperties
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="productName"></param>
        /// <param name="productLevel"></param>
        /// <param name="serverEdition"></param>
        /// <param name="parameterFormat"></param>
        /// <param name="version"></param>
        /// <param name="supports"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="layout"></param>
        /// <param name="supportedDbTypes"></param>
        /// <param name="supportedTopTypes"></param>
        internal protected DBDatabaseProperties(string databaseName,
            string productName, 
            string productLevel, 
            string serverEdition, 
            string parameterFormat, 
            Version version, 
            DBSchemaTypes supports, 
            bool caseSensitive, 
            DBParameterLayout layout,
            System.Data.DbType[] supportedDbTypes,
            TopType[] supportedTopTypes,
            Schema.DBSchemaInformation schemaInfo,
            TypedOperationCollection unsupportedOps)
        {
            this._vers = version;
            this._edition = serverEdition;
            this._productlevel = productLevel;
            this._name = productName;
            this._paramformat = parameterFormat;
            this.SupportedSchemas = supports;
            this.CaseSensitiveNames = caseSensitive;
            this.ParameterLayout = layout;
            this.SupportedDbTypes = supportedDbTypes;
            this.SupportedTopTypes = supportedTopTypes;
            this._schemainfo = schemaInfo;
            this._unsupportedOps = unsupportedOps;
        }

        #endregion

        //
        // public methods
        //

        #region public bool CheckSupports(DBSchemaTypes schema)

        /// <summary>
        /// Returns true if the Database supports the required schema type
        /// </summary>
        /// <param name="schema">The schema type to check (can be multiple)</param>
        /// <returns>True if the schema type is supported</returns>
        public bool CheckSupports(DBSchemaTypes schema)
        {
            return (this.SupportedSchemas & schema) > 0;
        }

        #endregion

        #region public bool CheckSupports(DBSchemaTypes schema, DBSchemaOperation op)

        /// <summary>
        /// Check if this database supports a specific operation on the database.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool CheckSupports(DBSchemaTypes schema, DBSchemaOperation op)
        {
            if (this.UnsupportedOps.Contains(schema,op))
                return false;
            else
                return CheckSupports(schema);
        }

        #endregion

        #region public bool CheckSupportsType(System.Data.DbType type)

        /// <summary>
        /// Returns true if the database supports the required data type
        /// </summary>
        /// <param name="type">The data type to check</param>
        /// <returns>True if it is supported</returns>
        public bool CheckSupportsDataType(System.Data.DbType type)
        {
            return Array.IndexOf<System.Data.DbType>(SupportedDbTypes, type) > -1;
        }

        #endregion

        #region public override string ToString() + 1 overload

        /// <summary>
        /// Builds a summary string of the properties of this database
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DBDatabaseProperties {");
            this.ToString(sb);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Appends key properties and their values to the StringBuilder. 
        /// Inheritors can override this method to append other properties
        /// </summary>
        /// <param name="sb"></param>
        protected virtual void ToString(StringBuilder sb)
        {
            sb.Append("product:");
            sb.Append(this.ProductName);
            sb.Append(", level:");
            sb.Append(this.ProductLevel);
            sb.Append(", edition:");
            sb.Append(this.ServerEdition);
            sb.Append(", version:");
            sb.Append(this.ProductVersion);
        }

        #endregion

    }


    
}
