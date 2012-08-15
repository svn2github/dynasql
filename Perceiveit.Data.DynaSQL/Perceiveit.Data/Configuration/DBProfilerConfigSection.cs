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
    /// Configuration element that defines the collection of profilers that are automatically attached to 
    /// a new instance of the DBDatabase
    /// </summary>
    /// <remarks>Use the 'attach', 'detach' and 'clear' elements in the config file to use the predefined (or any custom) db profilers</remarks>
    public class DBProfilerConfigSection : System.Configuration.ConfigurationElement
    {

        #region public bool AutoStart {get;set;}

        /// <summary>
        /// Gets or sets the Auto start option. Unusually this is configured as 'true' by default.
        /// </summary>
        [ConfigurationProperty("auto-start",DefaultValue=true)]
        public bool AutoStart
        {
            get
            {
                object val = this["auto-start"];
                if (val is bool)
                    return (bool)val;
                else
                    return true;
            }
            set
            {
                this["auto-start"] = value;
            }
        }

        #endregion

        #region public DBProfilerDefinitionConfigElementCollection Profilers {get;set;} + public bool HasProfilers {get;}

        /// <summary>
        /// Gets or sets the collection of DBProfilerConfigElements
        /// </summary>
        [ConfigurationProperty("",IsDefaultCollection=true)]
        [ConfigurationCollection(typeof(DBProfilerConfigElement)
            ,AddItemName="Attach",ClearItemsName="Clear",RemoveItemName="Detach"
            ,CollectionType=ConfigurationElementCollectionType.AddRemoveClearMap)]
        public DBProfilerDefinitionConfigElementCollection Profilers
        {
            get
            {
                return (DBProfilerDefinitionConfigElementCollection)this[""];
            }
            set
            {
                this[""] = value;
            }
        }

        /// <summary>
        /// Returns true if the this section has any defined profilers
        /// </summary>
        public bool HasProfilers
        {
            get { return this.Profilers != null && this.Profilers.Count > 0; }
        }

        #endregion

        //
        // methods
        //

        #region public IDBProfilerFactory[] GetProfilerFactories()

        /// <summary>
        /// helper var for returning an empty array (which could happen many times
        /// </summary>
        private static readonly IDBProfilerFactory[] _empty = new IDBProfilerFactory[] { };

        /// <summary>
        /// returns an array of instaniated DBProfilerFactories based upon the configuration file
        /// </summary>
        /// <returns></returns>
        public IDBProfilerFactory[] GetProfilerFactories()
        {
            if (!this.HasProfilers)
                return _empty;

            DBProfilerDefinitionConfigElementCollection col = this.Profilers;

            List<IDBProfilerFactory> all = new List<IDBProfilerFactory>(col.Count);
            foreach (DBProfilerConfigElement ele in col)
            {
                all.Add(ele.Factory);
            }
            return all.ToArray();
        }

        #endregion
    }
}
