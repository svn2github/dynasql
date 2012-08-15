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
    /// Contains helper methods for the Query framework
    /// </summary>
    internal static class DBHelper
    {
        #region internal static System.Data.DbType GetDBTypeForObject(object val)

        /// <summary>
        /// Helper method that Attempts to match the closet DbType for this object
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal static System.Data.DbType GetDBTypeForObject(object val)
        {
            if (null == val || val is DBNull)
                return System.Data.DbType.Object;

            Type runtime = val.GetType();
            return GetDBTypeForRuntimeType(runtime);
        }

        #endregion


        #region internal static System.Data.DbType GetDBTypeForRuntimeType(Type runtimeType)

        /// <summary>
        /// Helper method that attempts to match the closest DbType based upon the runtime type
        /// </summary>
        /// <param name="runtimeType"></param>
        /// <returns></returns>
        internal static System.Data.DbType GetDBTypeForRuntimeType(Type runtimeType)
        {
            System.Data.DbType type;
            TypeCode code = System.Type.GetTypeCode(runtimeType);
            switch (code)
            {
                case TypeCode.Boolean:
                    type = System.Data.DbType.Boolean;
                    break;
                case TypeCode.Byte:
                    type = System.Data.DbType.Byte;
                    break;
                case TypeCode.Char:
                    type = System.Data.DbType.AnsiStringFixedLength;
                    break;
                case TypeCode.DBNull:
                    type = System.Data.DbType.Object;
                    break;
                case TypeCode.DateTime:
                    type = System.Data.DbType.DateTime;
                    break;
                case TypeCode.Decimal:
                    type = System.Data.DbType.Decimal;
                    break;
                case TypeCode.Double:
                    type = System.Data.DbType.Double;
                    break;
                case TypeCode.Empty:
                    type = System.Data.DbType.Object;
                    break;
                case TypeCode.Int16:
                    type = System.Data.DbType.Int16;
                    break;
                case TypeCode.Int32:
                    type = System.Data.DbType.Int32;
                    break;
                case TypeCode.Int64:
                    type = System.Data.DbType.Int64;
                    break;
                case TypeCode.SByte:
                    type = System.Data.DbType.SByte;
                    break;
                case TypeCode.Single:
                    type = System.Data.DbType.Single;
                    break;
                case TypeCode.String:
                    type = System.Data.DbType.String;
                    break;
                case TypeCode.UInt16:
                    type = System.Data.DbType.UInt16;
                    break;
                case TypeCode.UInt32:
                    type = System.Data.DbType.UInt32;
                    break;
                case TypeCode.UInt64:
                    type = System.Data.DbType.UInt64;
                    break;

                case TypeCode.Object:
                default:
                    if (runtimeType == typeof(byte[]))
                        type = System.Data.DbType.Binary;
                    else if (runtimeType == typeof(Guid))
                        type = System.Data.DbType.Guid;
                    else if (runtimeType == typeof(TimeSpan))
                        type = System.Data.DbType.DateTimeOffset;
                    else
                        type = System.Data.DbType.Object;

                    break;
            }
            return type;
        }

        #endregion


        #region internal static Type GetRuntimeTypeForDbType(System.Data.DbType dbtype)

        /// <summary>
        /// Helper method that attempts to find the closest 
        /// match runtime Type based upon the dbType
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        internal static Type GetRuntimeTypeForDbType(System.Data.DbType dbtype)
        {
            Type runtime;
            switch (dbtype)
            {
                case System.Data.DbType.AnsiString:
                case System.Data.DbType.AnsiStringFixedLength:
                case System.Data.DbType.String:
                case System.Data.DbType.StringFixedLength:
                    runtime = typeof(string);
                    break;

                case System.Data.DbType.Binary:
                    runtime = typeof(byte[]);
                    break;

                case System.Data.DbType.Boolean:
                    runtime = typeof(bool);
                    break;

                case System.Data.DbType.Byte:
                    runtime = typeof(byte);
                    break;

                case System.Data.DbType.Date:
                case System.Data.DbType.DateTime:
                case System.Data.DbType.DateTime2:
                case System.Data.DbType.Time:
                    runtime = typeof(DateTime);
                    break;

                case System.Data.DbType.DateTimeOffset:
                    runtime = typeof(TimeSpan);
                    break;

                case System.Data.DbType.Double:
                    runtime = typeof(Double);
                    break;

                case System.Data.DbType.Guid:
                    runtime = typeof(Guid);
                    break;

                case System.Data.DbType.Int16:
                    runtime = typeof(Int16);
                    break;

                case System.Data.DbType.Int32:
                    runtime = typeof(Int32);
                    break;

                case System.Data.DbType.Int64:
                    runtime = typeof(Int64);
                    break;

                case System.Data.DbType.Object:
                    runtime = typeof(Object);
                    break;

                case System.Data.DbType.SByte:
                    runtime = typeof(SByte);
                    break;

                case System.Data.DbType.Single:
                    runtime = typeof(Single);
                    break;
                
                case System.Data.DbType.UInt16:
                    runtime = typeof(UInt16);
                    break;

                case System.Data.DbType.UInt32:
                    runtime = typeof(UInt32);
                    break;

                case System.Data.DbType.UInt64:
                    runtime = typeof(UInt64);
                    break;

                case System.Data.DbType.VarNumeric:
                case System.Data.DbType.Currency:
                case System.Data.DbType.Decimal:
                    runtime = typeof(Decimal);
                    break;

                case System.Data.DbType.Xml:
                    runtime = typeof(System.Xml.XmlNode);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("dbtype");

            }
            return runtime;
        }

        #endregion





        /* This is not required at the moment */
        //internal static bool DbTypeHasSystemSize(System.Data.DbType type, out int size)
        //{
        //    bool fixedwidth = false;
        //    size = -1;
        //    switch (type)
        //    {
        //        case System.Data.DbType.AnsiString:
        //        case System.Data.DbType.AnsiStringFixedLength:
        //        case System.Data.DbType.String:
        //        case System.Data.DbType.StringFixedLength:
        //        case System.Data.DbType.Binary:
        //            fixedwidth = false;
        //            break;

        //        case System.Data.DbType.Boolean:
        //        case System.Data.DbType.Byte:
        //            fixedwidth = true;
        //            size = 1;
        //            break;

        //        case System.Data.DbType.Currency:
        //            break;
        //        case System.Data.DbType.Date:
        //            break;
        //        case System.Data.DbType.DateTime:
        //            break;
        //        case System.Data.DbType.DateTime2:
        //            break;
        //        case System.Data.DbType.DateTimeOffset:
        //            break;
        //        case System.Data.DbType.Decimal:
        //            break;
        //        case System.Data.DbType.Double:
        //            break;
        //        case System.Data.DbType.Guid:
        //            break;
        //        case System.Data.DbType.Int16:
        //            break;
        //        case System.Data.DbType.Int32:
        //            break;
        //        case System.Data.DbType.Int64:
        //            break;
        //        case System.Data.DbType.Object:
        //            break;
        //        case System.Data.DbType.SByte:
        //            break;
        //        case System.Data.DbType.Single:
        //            break;
        //        case System.Data.DbType.Time:
        //            break;
        //        case System.Data.DbType.UInt16:
        //            break;
        //        case System.Data.DbType.UInt32:
        //            break;
        //        case System.Data.DbType.UInt64:
        //            break;
        //        case System.Data.DbType.VarNumeric:
        //            break;
        //        case System.Data.DbType.Xml:
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException("dbtype");
        //            break;

        //    }
        //}

    }
}
