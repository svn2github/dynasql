/*  Copyright 2010 PerceiveIT Limited
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
using System.Configuration;

namespace Perceiveit.Data.Configuration
{
    /// <summary>
    /// Defines a specific DBProviderImplementation for a known provider name (System.Data.SqlClient etc.).
    /// Instances of this class are usually created by the configuration manager rather than direct instantiation.
    /// </summary>
    public class DBProviderImplementationConfigElement : ConfigurationElement
    {

        #region ivars

        private DBProviderImplementation _implementation = null;//local instance cache

        #endregion

        //
        // public properties
        //

        #region public string ProviderName {get;}

        /// <summary>
        /// Gets or sets the .NET common ProviderName for this element (e.g. System.Data.SqlClient).
        /// Attribute name is 'provider'
        /// </summary>
        [ConfigurationProperty("provider", IsRequired = true, IsKey = true)]
        public string ProviderName
        {
            get { return (string)this["provider"]; }
            set { this["provider"] = value; }
        }

        #endregion

        #region public string ImplementationTypeName {get; set;}

        /// <summary>
        /// The assembly qualified type name for the implementation
        /// </summary>
        [ConfigurationProperty("implementation-type", IsRequired = true, IsKey = false)]
        public string ImplementationTypeName
        {
            get { return (string)this["implementation-type"]; }
            set { this["implementation-type"] = value; _implementation = null; }
        }

        #endregion

        #region public DBProviderImplementation Implementation {get;}

        /// <summary>
        /// Gets the instance of the DBProviderImplementation identified by the ImplementationTypeName
        /// </summary>
        public DBProviderImplementation Implementation
        {
            get
            {
                if (null == _implementation)
                    _implementation = CreateImplementation(this.ImplementationTypeName);
                return _implementation;
            }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBProviderImplementationConfigElement()

        /// <summary>
        /// Public default parameterless constructor
        /// </summary>
        public DBProviderImplementationConfigElement() { }

        #endregion

        #region internal DBProviderImplementationConfigElement(string providername, string typename, DBProviderImplementation implementation)

        /// <summary>
        /// Internal constructor for the default implementations
        /// </summary>
        /// <param name="providername"></param>
        /// <param name="typename"></param>
        /// <param name="implementation"></param>
        internal DBProviderImplementationConfigElement(string providername, string typename, DBProviderImplementation implementation)
            : this()
        {
            this.ProviderName = providername;
            this.ImplementationTypeName = typename;
            this._implementation = implementation;
        }

        #endregion

        //
        // methods
        //

        #region protected virtual DBProviderImplementation CreateImplementation(string typename)

        /// <summary>
        /// Creates a new instance of the DBProviderImplementation from the specified (assembly qualified) type name.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        protected virtual DBProviderImplementation CreateImplementation(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                throw new ArgumentNullException("typename");

            Type imptype = Type.GetType(typename, false);
            if (null == imptype)
                throw new System.Configuration.ConfigurationErrorsException(String.Format(Errors.ProfilerFactoryTypeNotFound, this.ProviderName));

            DBProviderImplementation factory;
            try
            {
                object instance = Activator.CreateInstance(imptype);
                factory = (DBProviderImplementation)instance;
            }
            catch (Exception ex)
            {
                throw new System.Configuration.ConfigurationErrorsException(String.Format(Errors.ProfilerFactoryTypeCouldNotBeInstantiated, this.ProviderName), ex);
            }

            return factory;
        }

        #endregion
    }


    #region public class DBProviderImplementationConfigElementCollection : System.Configuration.ConfigurationElementCollection
    
    /// <summary>
    /// Defines a collection of DBProviderImplementationConfigElements.
    /// </summary>
    /// <remarks>Any instance of this collection is initially populated with the default set of known implementations.
    /// Keys are case in-sensitive ('SYSTEM.data.SQLClient' is the same as 'system.data.sqlclient')</remarks>
    public class DBProviderImplementationConfigElementCollection : ConfigurationElementCollection
    {

        /// <summary>
        /// Creates a new collection and initializes the default values
        /// </summary>
        public DBProviderImplementationConfigElementCollection()
            : base(System.StringComparer.InvariantCultureIgnoreCase)
        {
            this.InitDefaultValues();
        }

        /// <summary>
        /// Overrides the default implementation to return a new DBProviderImplementationConfigElement.
        /// </summary>
        /// <returns></returns>
        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new DBProviderImplementationConfigElement();
        }

        /// <summary>
        /// Overrides the base implementation to return the provider name for the ProviderImplementation
        /// </summary>
        /// <param name="element">The element to get the name for</param>
        /// <returns></returns>
        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            string name = ((DBProviderImplementationConfigElement)element).ProviderName;
            if(string.IsNullOrEmpty(name))
                throw new NullReferenceException(Errors.ProviderNameNotSetInProviderImplementation);

            return name;
        }

        /// <summary>
        /// Initializes the collection with the know ProviderImplementations
        /// </summary>
        protected virtual void InitDefaultValues()
        {
            DBProviderImplementation imp = new SqlClient.DBSqlClientImplementation();
            this.Add(imp);

            imp = new SqLite.DBSqLiteImplementaion();
            this.Add(imp);

            imp = new MySqlClient.DBMySqlImplementation();
            this.Add(imp);

            imp = new OleDb.DBOleDbImplementation();
            this.Add(imp);

            imp = new Oracle.DBOracleImplementation();
            this.Add(imp);
        }

        //
        // collection modification methods
        //

        /// <summary>
        /// Adds a new implementation by taking the values from the instance itself and adding to the collection.
        /// It is an error if there is already an implementation with the same name (case in-sensitive)
        /// </summary>
        /// <param name="imp">The implementation to add</param>
        public void Add(DBProviderImplementation imp)
        {
            string name = imp.ProviderName;
            string typename = imp.GetType().AssemblyQualifiedName;
            DBProviderImplementationConfigElement ele = new DBProviderImplementationConfigElement(
                name, typename, imp);
            this.Add(ele);
        }

        /// <summary>
        /// Adds a configured element to the collection
        /// </summary>
        /// <param name="element"></param>
        internal void Add(DBProviderImplementationConfigElement element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        /// Gets a implementation element with the specified name (case in-sensitive)
        /// </summary>
        /// <param name="providerName">The name of the provider</param>
        /// <returns>Any instance if found</returns>
        internal DBProviderImplementationConfigElement Get(string providerName)
        {
            return this.BaseGet(providerName) as DBProviderImplementationConfigElement;
        }

        /// <summary>
        /// Returns true if there is an element with the configured name
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        internal bool Contains(string providerName)
        {
            return this.Get(providerName) != null;
        }

        /// <summary>
        /// Removes the elelment with the specified name
        /// </summary>
        /// <param name="providerName"></param>
        internal void Remove(string providerName)
        {
            this.BaseRemove(providerName);
        }

    }

    #endregion
}
