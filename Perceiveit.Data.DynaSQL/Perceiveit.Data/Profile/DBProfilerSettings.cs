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
using System.Collections.Specialized;

namespace Perceiveit.Data.Profile
{
    /// <summary>
    /// A readonly collection of string names and values that can either be accessed by name, or by index
    /// </summary>
    public class DBProfilerSettings
    {
        private NameValueCollection _innerSettings;

        /// <summary>
        /// Creates a new collection of settings.
        /// </summary>
        /// <param name="settings"></param>
        public DBProfilerSettings(NameValueCollection settings)
        {
            _innerSettings = CloneCollection(settings);
        }

        /// <summary>
        /// Clones the NameValue collection
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private static NameValueCollection CloneCollection(NameValueCollection col)
        {
            if (null == col)
                return new NameValueCollection();
            else
                return new NameValueCollection(col);
        }
    }
}
