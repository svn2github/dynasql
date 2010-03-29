/*  Copyright 2009 PerceiveIT Limited
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
using System.Data;
using System.Data.Common;

namespace Perceiveit.Data.Query
{
    internal sealed class DBDisposerDataReader : DbDataReader
    {
        //
        // events
        //

        #region public event EventHandler Disposed; + OnDispose(EventArgs)

        public event EventHandler Disposed;

        private void OnDispose(EventArgs e)
        {
            if (this.Disposed != null)
                this.Disposed(this, e);
        }

        #endregion

        //
        // properties
        //

        #region protected DbDataReader Inner {get;}

        private DbDataReader _inner;

        private DbDataReader Inner
        {
            get { return _inner; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBDisposerDataReader(DbDataReader inner)

        public DBDisposerDataReader(DbDataReader inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");
            this._inner = inner;

        }

        #endregion

        //
        // DbDataReader Implementation
        //



        public override void Close()
        {
            this.Inner.Close();

        }

        public override int Depth
        {
            get { return this.Inner.Depth; }
        }

        public override int FieldCount
        {
            get { return this.Inner.FieldCount; }
        }

        public override bool GetBoolean(int ordinal)
        {
            return this.Inner.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return this.Inner.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return this.Inner.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return this.Inner.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return this.Inner.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.Inner.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return this.Inner.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return this.Inner.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return this.Inner.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return this.Inner.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.Inner.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return this.Inner.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return this.Inner.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return this.Inner.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return this.Inner.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return this.Inner.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return this.Inner.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.Inner.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return this.Inner.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return this.Inner.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return this.Inner.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return this.Inner.GetValues(values);
        }

        public override bool HasRows
        {
            get { return this.Inner.HasRows; }
        }

        public override bool IsClosed
        {
            get { return this.Inner.IsClosed; }
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.Inner.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return this.Inner.NextResult();
        }

        public override bool Read()
        {
            return this.Inner.Read();
        }

        public override int RecordsAffected
        {
            get { return this.Inner.RecordsAffected; }
        }

        public override object this[string name]
        {
            get { return this.Inner[name]; }
        }

        public override object this[int ordinal]
        {
            get { return this.Inner[ordinal]; }
        }

        protected override void Dispose(bool disposing)
        {
            this.OnDispose(EventArgs.Empty);
            base.Dispose(disposing);
        }
    }
}
