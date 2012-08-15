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
using Perceiveit.Data.Profile;

namespace Perceiveit.Data.Configuration
{
    /// <summary>
    /// Defines a single element in the configuration file for a profiler factory and its associated name.
    /// </summary>
    public class DBProfilerConfigElement : ConfigurationElement
    {

        #region ivars

        private IDBProfilerFactory _factory = null; //local cache of the factory instance.

        #endregion

        //
        // propeties
        //

        #region public string Name {get;set;}

        /// <summary>
        /// Gets or sets the name of the profiler
        /// </summary>
        [ConfigurationProperty("name",IsRequired=true,IsKey=true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        #endregion

        #region public string FactoryTypeName {get;set;}

        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the IDBProfilerFactory.
        /// </summary>
        [ConfigurationProperty("factory",IsRequired=true,IsKey=false)]
        public string FactoryTypeName
        {
            get { return (string)this["factory"]; }
            set 
            { 
                this["factory"] = value;
                _factory = null;
            }
        }

        #endregion

        #region public DBProfilerConfigSettingCollection Settings

        /// <summary>
        /// Gets or sets the collection of custom settings for configuring any optional 
        /// settings on the Profiler
        /// </summary>
        [ConfigurationProperty("",IsDefaultCollection=true,IsRequired=false, IsKey=false)]
        [ConfigurationCollection(typeof(DBProfilerConfigSettingElement),
            CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap,
            AddItemName="Set",ClearItemsName="Clear", RemoveItemName="Remove")]
        public DBProfilerConfigSettingCollection Settings
        {
            get { return (DBProfilerConfigSettingCollection)this[""]; }
            set { this[""] = value; }
        }

        #endregion

        #region public IDBProfilerFactory Factory {get;}

        /// <summary>
        /// Gets the profiler factory defined in this config element.
        /// </summary>
        /// <exception cref="ArgumentNullException" >Thown if this elements FactroyTypeName is null or empty</exception>
        /// <exception cref="ConfigurationErrorsException">Thrown if the type cannot be found or an instance cannot be created</exception>
        public IDBProfilerFactory Factory
        {
            get
            {
                if (null == _factory)
                    _factory = CreateFactory(this.FactoryTypeName);
                return _factory;
            }
        }

        #endregion

        //
        // methods
        //

        #region protected virtual IDBProfilerFactory CreateFactory(string typename)

        /// <summary>
        /// Creates a new instance of the IDBProfilerFactory based upon the configured typename specified.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        protected virtual IDBProfilerFactory CreateFactory(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                throw new ArgumentNullException("factory");
            
            Type factorytype = Type.GetType(typename, false);
            if (null == factorytype)
                throw new System.Configuration.ConfigurationErrorsException(String.Format(Errors.ProfilerFactoryTypeNotFound, this.Name));
            
            IDBProfilerFactory factory;
            System.Collections.Specialized.NameValueCollection settings = GetFactorySettings();
            try
            {
                object instance = Activator.CreateInstance(factorytype);
                factory = (IDBProfilerFactory)instance;
                factory.Settings = settings;
                factory.Name = this.Name;
            }
            catch (Exception ex)
            {
                throw new System.Configuration.ConfigurationErrorsException(String.Format(Errors.ProfilerFactoryTypeCouldNotBeInstantiated, this.Name), ex);
            }

            return factory;
        }

        #endregion

        #region private Specialized.NameValueCollection GetFactorySettings()

        /// <summary>
        /// Builds a new NameValueCollection for the configured settings
        /// </summary>
        /// <returns></returns>
        private System.Collections.Specialized.NameValueCollection GetFactorySettings()
        {
            System.Collections.Specialized.NameValueCollection all = new System.Collections.Specialized.NameValueCollection();
            DBProfilerConfigSettingCollection col = this.Settings;
            if (null != col && col.Count > 0)
            {
                foreach (DBProfilerConfigSettingElement ele in col)
                {
                    all.Add(ele.Key, ele.Value);
                }
            }
            return all;
        }

        #endregion

    }


    #region public class DBProfilerDefinitionConfigElementCollection : ConfigurationElementCollection

    /// <summary>
    /// A collection of DBProfilerDefinitionConfigElement(s).
    /// </summary>
    public class DBProfilerDefinitionConfigElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Overrides the base implementation to return a new DBProfilerConigElement
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DBProfilerConfigElement();
        }

        /// <summary>
        /// Overrides the base implementation to return the name of the element
        /// </summary>
        /// <param name="element">The configuration element to get the name of</param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            if ((element is DBProfilerConfigElement) == false)
                throw new InvalidCastException(Errors.ConfigElementNotDBProfiler);

            string name = ((DBProfilerConfigElement)element).Name;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("element.Name", Errors.ProfilerNameNotSetInConfig);
            return name;
        }
    }

    #endregion
}
