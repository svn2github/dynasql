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
    /// Defines a method signature that returns the required value of a Parameter for methods created with the DBParam.ParamWithDelegate(...)
    /// </summary>
    /// <returns>The value of any required parameter</returns>
    public delegate object ParamValue();

    //
    // callback methods for the DBDataBase.ExecuteRead method
    //

    /// <summary>
    /// Callback when an exception occurs during the execution of a statement
    /// </summary>
    /// <param name="onerror"></param>
    /// <returns></returns>
    public delegate void DBOnErrorCallback(DBExceptionEventArgs onerror);

    /// <summary>
    /// one of 3 callback method signatures from the DBDatabase.ExecuteRead(...) methods.
    /// </summary>
    /// <param name="reader">The reader created by the DBDatabase from the statement</param>
    /// <returns>Returns any object the the caller of the ExecuteRead statement requires.</returns>
    public delegate object DBCallback(System.Data.Common.DbDataReader reader);

    /// <summary>
    /// Call back method that sets a DbDataRecord rather than the DbDataReader
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public delegate object DBRecordCallback(System.Data.IDataRecord record);

    /// <summary>
    /// one of 3 callback methods signatures for the DBDatabase.ExecuteRead(...) methods. This signature supports the passing of a context object.
    /// </summary>
    /// <param name="reader">The reader created by the DBDatabase from the statement passed to ExecuteRead...</param>
    /// <param name="context">Any context required by the delegate method</param>
    /// <returns>Returns any object the call of the ExecuteRead statement requires.</returns>
    public delegate object DBContextCallback(System.Data.Common.DbDataReader reader, object context);

    /// <summary>
    /// Call back method that sets a DbDataRecord rather than the DbDataReader
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public delegate object DBContextRecordCallback(System.Data.IDataRecord record, object context);

    /// <summary>
    /// One of 3 callback methods for the DBDatabase.ExecuteRead(...) methods. This signature does not return a value so no value is required to be returned
    /// </summary>
    /// <param name="reader">The reader created by the DBDatabase from the statement passed to ExecuteRead(..</param>
    public delegate void DBEmptyCallback(System.Data.Common.DbDataReader reader);

    /// <summary>
    /// Call back method that sets a DbDataRecord rather than the DbDataReader with no return value
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public delegate void DBEmptyRecordCallback(System.Data.IDataRecord record);


    #region public delegate void DBExceptionHandler(object sender, DBExceptionEventArgs args); + public class DBExceptionEventArgs : EventArgs

    /// <summary>
    /// Event Handler delegate that is raised when a 
    /// decision is needed on how to handle an error from a database execution
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void DBExceptionHandler(object sender, DBExceptionEventArgs args);

    public class DBExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// If this exception has been handled then set 
        /// to true so that it is not rethrown by the caller
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the Exception that was raised and caught
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets the message that will be 
        /// rethrown by the caller if Handled is false
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Create a new instance of the DBExceptionEventArgs
        /// </summary>
        /// <param name="ex"></param>
        public DBExceptionEventArgs(Exception ex)
        {
            this.Handled = false;
            this.Exception = ex;
            this.Message = ex.Message;
        }

        
    }

    #endregion
}
