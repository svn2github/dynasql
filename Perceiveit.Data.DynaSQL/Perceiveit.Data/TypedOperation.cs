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
using System.Linq;
using System.Text;

namespace Perceiveit.Data
{
    /// <summary>
    /// Describes a type and operation combination. 
    /// Uniqueness is identified by the hash code which is a combination of the Type and Operation values.
    /// Instances with the same Type and Operation are considered equal.
    /// </summary>
    /// <remarks>
    /// Use the static GetHashCode(type,op) method to generate a comparison hashcode with out creating an instance of the class.
    /// </remarks>
    public class TypedOperation : IEquatable<TypedOperation>
    {
        /// <summary>
        /// The scehma type
        /// </summary>
        public DBSchemaTypes Type { get; set; }

        /// <summary>
        /// The operation.
        /// </summary>
        public DBSchemaOperation Operation { get; set; }

        /// <summary>
        /// Create a new TypedOperation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="op"></param>
        public TypedOperation(DBSchemaTypes type, DBSchemaOperation op)
        {
            this.Type = type;
            this.Operation = op;
        }

        /// <summary>
        /// Converts this TypedOperation to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Type.ToString() + ":" + this.Operation.ToString();
        }

        /// <summary>
        /// Gets the unique hash code for this type and operation
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return GetHashCode(this.Type, this.Operation);
        }

        /// <summary>
        /// Calculates and returns the integer hashcode which
        /// should be unique for any combination of type and operation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int GetHashCode(DBSchemaTypes type, DBSchemaOperation op)
        {
            int val = (int)type;
            val = val << 16;
            val += (int)op;
            return val;
        }


        /// <summary>
        /// returns true if the provided object is a TypedOperation
        /// instance and considered equal (same Type and Operation) 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TypedOperation)
                return obj.GetHashCode() == this.GetHashCode();
            else
                return false;
        }

        /// <summary>
        ///  returns true if this instance is
        ///  considered equal (same Type and Operation) 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TypedOperation other)
        {
            return other.GetHashCode() == this.GetHashCode();
        }
    }

    /// <summary>
    /// A collection of TypedOperations that supports fast access 
    /// </summary>
    public sealed class TypedOperationCollection
    {

        /// <summary>
        /// internally store the operations in a dictionary keyed on the hash code.
        /// </summary>
        private Dictionary<int, TypedOperation> _ops = new Dictionary<int, TypedOperation>();

        /// <summary>
        /// Gets the number of operations in this collection
        /// </summary>
        public int Count
        {
            get { return _ops.Count; }
        }

        /// <summary>
        /// Adds a new TypedOperation to the collection based on the type and operation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        public void Add(DBSchemaTypes type, DBSchemaOperation operation)
        {
            TypedOperation value = new TypedOperation(type, operation);
            this.Add(value);
        }


        /// <summary>
        /// Adds a new TypedOperation to the collection
        /// </summary>
        /// <param name="op"></param>
        public void Add(TypedOperation op)
        {
            if (null == op)
                throw new ArgumentNullException("op");

            if (this.Contains(op) == false)
                _ops.Add(op.GetHashCode(), op);
        }


        /// <summary>
        /// Removes a TypedOperation from this collection that has a type and operation matching the parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        public void Remove(DBSchemaTypes type, DBSchemaOperation operation)
        {

            TypedOperation value = new TypedOperation(type, operation);
            this.Remove(value);
        }

        /// <summary>
        /// Removes any TypedOperation from this collection that has a matching type and operation to the provided op.
        /// </summary>
        /// <param name="op"></param>
        public void Remove(TypedOperation op)
        {
            int hash = op.GetHashCode();
            this._ops.Remove(hash);
        }


        /// <summary>
        /// Returns true if this collection contains a TypedOperation that matches the parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public bool Contains(DBSchemaTypes type, DBSchemaOperation operation)
        {
            TypedOperation value;
            int hash = TypedOperation.GetHashCode(type, operation);
            if (_ops.TryGetValue(hash, out value))
                return true;
            else
                return false;

        }

        /// <summary>
        ///  Returns true if this collection contains a TypedOperation that matches the parameter
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool Contains(TypedOperation op)
        {
            TypedOperation value;
            int hash = op.GetHashCode();
            if (_ops.TryGetValue(hash, out value))
                return true;
            else
                return false;
        }
    }
}
