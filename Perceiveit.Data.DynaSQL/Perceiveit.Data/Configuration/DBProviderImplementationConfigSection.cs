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
    /// Configuration element that contains the list of registered DBProviderImplementations.
    /// </summary>
    public class DBProviderImplementationConfigSection : ConfigurationElement
    {

        #region public DBProviderImplementationConfigElementCollection Implementations {get;set;}

        /// <summary>
        /// Gets or sets the collection of DBFactoryConfigElements
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(DBProfilerConfigElement)
            , AddItemName = "Register", ClearItemsName = "Clear", RemoveItemName = "Remove"
            , CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public DBProviderImplementationConfigElementCollection Implementations
        {
            get
            {
                return (DBProviderImplementationConfigElementCollection)this[""];
            }
            set
            {
                if (null == value) //not allowed to be null.
                    value = new DBProviderImplementationConfigElementCollection();

                this[""] = value;
            }
        }

        #endregion

        #region public bool HasImplementations {get;}

        /// <summary>
        /// Gets the boolean flag that identifies if this section has any defined implementations
        /// </summary>
        public bool HasImplementations
        {
            get { return this.Implementations != null && this.Implementations.Count > 0; }
        }

        #endregion

        //
        // methods
        //

        /// <summary>
        /// Adds a custom provider to the configured set of providers
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="implemementation"></param>
        internal void Add(string providerName, DBProviderImplementation implemementation)
        {
            DBProviderImplementationConfigElement ele = new DBProviderImplementationConfigElement(
                providerName,
                implemementation.GetType().AssemblyQualifiedName,
                implemementation);
            this.Implementations.Add(ele);
        }

        /// <summary>
        /// Removes an implementation from the configured collection
        /// </summary>
        /// <param name="providerName"></param>
        internal void Remove(string providerName)
        {
            this.Implementations.Remove(providerName);
        }

        /// <summary>
        /// Gets the implementation from the configured collection with the specified name
        /// </summary>
        /// <param name="providername"></param>
        /// <returns></returns>
        internal DBProviderImplementation Get(string providername)
        {
            DBProviderImplementationConfigElement ele = this.Implementations.Get(providername);
            if (null == ele)
                throw new NullReferenceException(String.Format(Errors.NoProviderImplementationWithTheName, providername));

            return ele.Implementation;
        }

        

        /// <summary>
        /// returns true if the configured collection 
        /// </summary>
        /// <param name="providername"></param>
        /// <returns></returns>
        internal bool Contains(string providername)
        {
            return this.Implementations.Contains(providername);
        }
       
    }
}
