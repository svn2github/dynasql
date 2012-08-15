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

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Exception that is thrown when the extraction of a database schema caused an error
    /// </summary>
    [global::System.Serializable]
    public class DBSchemaProviderException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of the exception witout any details
        /// </summary>
        public DBSchemaProviderException() { }

        /// <summary>
        /// Creates a new instance of the exception with the specified message
        /// </summary>
        /// <param name="message"></param>
        public DBSchemaProviderException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the exception with the specified message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public DBSchemaProviderException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// For binary serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DBSchemaProviderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
