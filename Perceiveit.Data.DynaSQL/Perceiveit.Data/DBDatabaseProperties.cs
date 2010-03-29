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

namespace Perceiveit.Data
{
    public class DBDatabaseProperties
    {

        public static readonly string Unknown = String.Empty;


        private string _edition;
        public string ServerEdition
        {
            get { return _edition; }
        }

        private string _productlevel;
        public string ProductLevel
        {
            get { return _productlevel; }
        }

        private string _name;
        public string ProductName
        {
            get { return _name; }
        }

        private Version _vers;
        public Version ProductVersion
        {
            get { return _vers; }
        }

        private string _paramformat;
        public string ParameterFormat
        {
            get { return _paramformat; }
        }

        private DBSchemaTypes _supportedschemas;
        public DBSchemaTypes SupportedSchemas 
        {
            get { return _supportedschemas; }
            protected set { _supportedschemas = value; }
        }

        private bool _casesensitive;
        public bool CaseSensitiveNames
        {
            get { return _casesensitive; }
            protected set { _casesensitive = value; }
        }

        private DBParameterLayout _paramlayout;
        public DBParameterLayout ParameterLayout
        {
            get { return _paramlayout; }
            protected set { _paramlayout = value; }
        }

        private System.Data.DbType[] _supportedtypes;
        public System.Data.DbType[] SupportedDbTypes
        {
            get { return _supportedtypes; }
            protected set { _supportedtypes = value; }
        }


        public DBDatabaseProperties(string productName, 
                                    string productLevel, 
                                    string serverEdition, 
                                    string parameterFormat, 
                                    string version, 
                                    DBSchemaTypes supports, 
                                    bool caseSensitive, 
                                    DBParameterLayout layout,
                                    System.Data.DbType[] supportedDbTypes)
            : this(productName, productLevel, serverEdition, parameterFormat, new Version(version), supports, caseSensitive, layout, supportedDbTypes)
        {
        }

        public DBDatabaseProperties(string productName, 
            string productLevel, 
            string serverEdition, 
            string parameterFormat, 
            Version version, 
            DBSchemaTypes supports, 
            bool caseSensitive, 
            DBParameterLayout layout,
            System.Data.DbType[] supportedDbTypes)
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
        }


        public bool CheckSupports(DBSchemaTypes schema)
        {
            return (this.SupportedSchemas & schema) > 0;
        }

        public bool CheckSupportsType(System.Data.DbType type)
        {
            return Array.IndexOf<System.Data.DbType>(SupportedDbTypes, type) > -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DBDatabaseProperties {");
            this.ToString(sb);
            sb.Append("}");
            return sb.ToString();
        }

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

    }
}
