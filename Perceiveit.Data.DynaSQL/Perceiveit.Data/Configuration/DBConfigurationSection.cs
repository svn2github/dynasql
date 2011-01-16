/*  Copyright 2010 PerceiveIT Limited
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
using System.Configuration;

namespace Perceiveit.Data.Configuration
{
    /// <summary>
    /// Root configuration section. Add the config entry as a Perceiveit.Data element, and register profilers and factories
    /// </summary>
    public class DBConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Defines the name of the Configuration element where these properties are defined.
        /// </summary>
        public const string DBConfigSectionElement = "Perceiveit.Data";

        /// <summary>
        /// Gets or sets the Wrap exceptions option. This is configured as 'false' by default. 
        /// </summary>
        /// <remarks>If set to true then any exceptions raised during calls to the DBDatabase will be wrapped by
        /// a new DataException that does not divulge sensitive information. 
        /// The inner exception will contain the pertinant information about what happened.</remarks>
        [ConfigurationProperty("wrap-exceptions", DefaultValue = false)]
        public bool WrapExceptions
        {
            get
            {
                object val = this["wrap-exceptions"];
                if (val is bool)
                    return (bool)val;
                else
                    return false;
            }
            set
            {
                this["wrap-exceptions"] = value;
            }
        }

        /// <summary>
        /// Support property so the xmlns attribute can be 
        /// applied to the section element in the config file for 
        /// schema intellisense.
        /// </summary>
        [ConfigurationProperty("xmlns")]
        public string xmlns
        {
            get { return this["xmlns"] as string; }
            set { this["xmlns"] = value; }
        }

        /// <summary>
        /// Gets or sets the Profilers configuration.
        /// </summary>
        [ConfigurationProperty("Profilers", Options = ConfigurationPropertyOptions.None)]
        public DBProfilerConfigSection Profilers
        {
            get
            {
                DBProfilerConfigSection prof = this["Profilers"] as DBProfilerConfigSection;
                if (null == prof)
                {
                    prof = new DBProfilerConfigSection();
                    prof.Profilers = new DBProfilerDefinitionConfigElementCollection();
                    this.Profilers = prof;
                }
                return prof;
            }
            set
            {
                this["Profilers"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the DBFactories configuration.
        /// </summary>
        [ConfigurationProperty("Implementations", Options = ConfigurationPropertyOptions.None)]
        public DBProviderImplementationConfigSection Implementations
        {
            get
            {
                DBProviderImplementationConfigSection imps = this["Implementations"] as DBProviderImplementationConfigSection;
                if(null == imps)
                {
                    imps = new DBProviderImplementationConfigSection();
                    imps.Implementations = new DBProviderImplementationConfigElementCollection();
                    this.Implementations = imps;
                }
                return imps;
            }
            set
            {
                this["Implementations"] = value;
            }
        }


        private static DBConfigurationSection _defined;

        /// <summary>
        /// Returns the configured DBConfigurationSection. 
        /// If it does not exist then a default one is created and returned
        /// </summary>
        /// <returns></returns>
        public static DBConfigurationSection GetSection()
        {
            if (null == _defined)
            {
                _defined = System.Configuration.ConfigurationManager.GetSection(DBConfigurationSection.DBConfigSectionElement) as DBConfigurationSection;
                if(null == _defined)//There is no section so create an empty one.
                    _defined = CreateDefaultConfiguration();
            }
            return _defined;
        }

        /// <summary>
        /// Creates a new DBConfigurationSection. The Implementation &amp; Profiler collections are also 
        /// created so that any default values can be added.
        /// </summary>
        private static DBConfigurationSection CreateDefaultConfiguration()
        {
            DBConfigurationSection defined = new DBConfigurationSection();
            defined.Implementations = new DBProviderImplementationConfigSection();
            defined.Implementations.Implementations = new DBProviderImplementationConfigElementCollection();
            defined.Profilers = new DBProfilerConfigSection();
            defined.Profilers.Profilers = new DBProfilerDefinitionConfigElementCollection();

            return defined;
        }
    }
}
