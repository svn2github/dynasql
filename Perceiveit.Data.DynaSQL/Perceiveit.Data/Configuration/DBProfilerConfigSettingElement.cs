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
    /// Each DBProfilerConfigElement supports a key value collection of settings to customise set-up. 
    /// See the documentation for each Profiler to understand what keys are supported
    /// </summary>
    public class DBProfilerConfigSettingElement : ConfigurationElement
    {

        #region public string Key {get;set;}

        /// <summary>
        /// Gets or sets the key for this Setting
        /// </summary>
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return this["key"] as string; }
            set { this["key"] = value; }
        }

        #endregion

        #region public string Value {get;set;}

        /// <summary>
        /// Gets or sets the value for this setting
        /// </summary>
        [ConfigurationProperty("value", IsRequired = false, IsKey = false)]
        public string Value
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }

        #endregion

    }

    #region public class DBProfilerConfigSettingCollection : ConfigurationElementCollection

    /// <summary>
    /// Defines a collection of settings for a configured profiler
    /// </summary>
    public class DBProfilerConfigSettingCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// overrides the base method to return a new DBProfilerConfigSettingElement
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DBProfilerConfigSettingElement();
        }

        /// <summary>
        /// Gets the Key for this DBProfilerConfigSettingElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DBProfilerConfigSettingElement)element).Key;
        }
    }

    #endregion

}
