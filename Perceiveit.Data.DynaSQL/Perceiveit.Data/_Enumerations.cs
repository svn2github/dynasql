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
using System.Xml.Serialization;

namespace Perceiveit.Data
{
    /// <summary>
    /// Enumeration of all Comparison operators.
    /// </summary>
    /// <remarks>There are 5 different types of operator defined, and each can be cast to the standard 'Operator' enumeration without clashing with other types</remarks>
    public enum Compare : int
    {
        /// <summary>
        /// =
        /// </summary>
        Equals = 1,

        /// <summary>
        /// &lt;
        /// </summary>
        LessThan = 2,

        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan = 3,

        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanEqual = 4,

        /// <summary>
        /// &gt;=
        /// </summary>
        GreaterThanEqual = 5,

        /// <summary>
        /// !=, &lt;&gt;
        /// </summary>
        NotEqual = 6,

        /// <summary>
        /// special 'LIKE' operator
        /// </summary>
        Like = 7,

        /// <summary>
        /// special 'IS' operator
        /// </summary>
        Is = 8,

        /// <summary>
        /// special 'IN' operator (EXISTS IN)
        /// </summary>
        In = 9,

        /// <summary>
        /// opposite of 'IN'
        /// </summary>
        NotIn = 10
    }

    /// <summary>
    /// Enumeration of all binary operators (take 2 parameters and calcuate a result
    /// </summary>
    /// <remarks>There are 5 different types of operator defined, and each can be cast to the standard 'Operator' enumeration without clashing with other types</remarks>
    public enum BinaryOp : int
    {
        /// <summary>
        /// +
        /// </summary>
        Add = 100,

        /// <summary>
        /// -
        /// </summary>
        Subtract = 101,

        /// <summary>
        /// *
        /// </summary>
        Multiply = 102,

        /// <summary>
        /// /
        /// </summary>
        Divide = 103,

        /// <summary>
        /// &lt;&lt;
        /// </summary>
        BitShiftLeft = 104,

        /// <summary>
        /// &gt;&gt;
        /// </summary>
        BitShiftRight = 105,

        /// <summary>
        /// %
        /// </summary>
        Modulo = 106,

        /// <summary>
        /// &amp;
        /// </summary>
        BitwiseAnd = 107,

        /// <summary>
        /// |
        /// </summary>
        BitwiseOr = 108,

        /// <summary>
        /// + for strings
        /// </summary>
        Concat = 109
    }

    /// <summary>
    /// Enumeration of the Unary operators (take a single argument)
    /// </summary>
    /// <remarks>There are 5 different types of operator defined, and each can be cast to the standard 'Operator' enumeration without clashing with other types</remarks>
    public enum UnaryOp : int
    {
        /// <summary>
        /// !
        /// </summary>
        Not = 200,

        /// <summary>
        /// EXISTS
        /// </summary>
        Exists = 201
    }

    /// <summary>
    /// Enumeration of the Teriary operators (3 arguments)
    /// </summary>
    public enum TertiaryOp : int
    {
        /// <summary>
        /// special BETWEEN ... AND ...
        /// </summary>
        Between = 300
    }

    /// <summary>
    /// Boolean logic table operators for 2 arguments
    /// </summary>
    /// <remarks>There are 5 different types of operator defined, and each can be cast to the standard 'Operator' enumeration without clashing with other types</remarks>
    public enum BooleanOp : int
    {
        /// <summary>
        /// &amp;&amp;
        /// </summary>
        And = 350,

        /// <summary>
        /// ||
        /// </summary>
        Or = 351,

        /// <summary>
        /// special XOr operator
        /// </summary>
        XOr = 352,
    }

    /// <summary>
    /// The operator enumeration contains all 5 operator types in a single enumeration
    /// </summary>
    public enum Operator : int
    {
        /// <summary>=</summary>
        Equals = 1,
        /// <summary>&lt;</summary>
        LessThan = 2,
        /// <summary>&gt;</summary>
        GreaterThan = 3,
        /// <summary>&lt;=</summary>
        LessThanEqual = 4,
        /// <summary>&gt;=</summary>
        GreaterThanEqual = 5,
        /// <summary>!=</summary>
        NotEqual = 6,
        /// <summary>LIKE</summary>
        Like = 7,
        /// <summary>IS</summary>
        Is = 8,
        /// <summary>IN</summary>
        In = 9,
        /// <summary>NOT IN</summary>
        NotIn = 10,

        /// <summary>+</summary>
        Add = 100,
        /// <summary>-</summary>
        Subtract = 101,
        /// <summary>*</summary>
        Multiply = 102,
        /// <summary>/</summary>
        Divide = 103,
        /// <summary>&lt;&lt;</summary>
        BitShiftLeft = 104,
        /// <summary>&gt;&gt;</summary>
        BitShiftRight = 105,
        /// <summary>%</summary>
        Modulo = 106,
        /// <summary>&amp;</summary>
        BitwiseAnd = 107,
        /// <summary>|</summary>
        BitwiseOr = 108,
        /// <summary>+ for strings</summary>
        Concat = 109,

        /// <summary>!</summary>
        Not = 200,
        /// <summary>EXISTS</summary>
        Exists = 201,

        /// <summary>BETWEEN ... AND ...</summary>
        Between = 300,

        /// <summary>&amp;&amp;</summary>
        And = 350,
        /// <summary>||</summary>
        Or = 351,
        /// <summary>!|</summary>
        XOr = 352

    }

    /// <summary>
    /// Defines the set of aggregate functions
    /// </summary>
    public enum AggregateFunction
    {
        /// <summary>COUNT(*)</summary>
        Count = 400,
        /// <summary>SUM(column)</summary>
        Sum = 401,
        /// <summary>AVG(column)</summary>
        Avg = 402,
        /// <summary>MIN(column)</summary>
        Min = 403,
        /// <summary>MAX(column)</summary>
        Max = 404
    }

    /// <summary>
    /// Enumeration of the Sorting Order - Default is providied to that the database can define its optimum ordering
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// A..Z, 1..9 etc.
        /// </summary>
        Ascending,
        /// <summary>
        /// Z..A, 9..1 etc.
        /// </summary>
        Descending,
        /// <summary>
        /// undefined - upto the engine implementation
        /// </summary>
        Default
    }

    /// <summary>
    /// Enumeration of the avaiable join types
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// Intersection - match both sides to be included
        /// </summary>
        InnerJoin,
        /// <summary>
        /// All left rows and any rows on the right side that match
        /// </summary>
        LeftOuter,

        /// <summary>
        /// All left rows joined to all right rows
        /// </summary>
        CrossProduct,

        /// <summary>
        /// All right rows, and any left rows that match
        /// </summary>
        RightOuter,

        /// <summary>
        /// Undefined constraint
        /// </summary>
        Join
    }

    /// <summary>
    /// Basic set of known functions
    /// </summary>
    public enum Function
    {
        /// <summary>Special custom function that is known to be implemented by the author but is not standard</summary>
        Unknown = -1,

        /// <summary>
        /// Gets the current date
        /// </summary>
        GetDate = 0,

        /// <summary>
        /// Gets the auto numbered ID of the last inserted row
        /// </summary>
        LastID = 1,

        /// <summary>
        /// Checks the first argument and returns it unless it's null - returns the second. 
        /// </summary>
        IsNull = 2,

        /// <summary>
        /// Gets the auto numbered  next vaue in the sequence
        /// </summary>
        NextID = 3,

        /// <summary>
        /// A string concatenation function
        /// </summary>
        Concatenate = 4
    }

    /// <summary>
    /// All the different returned range options - limits
    /// </summary>
    public enum TopType
    {
        /// <summary>
        /// Explicit number to return
        /// </summary>
        Count,
        /// <summary>
        /// The maximum percentage to return
        /// </summary>
        Percent,

        /// <summary>
        /// Within a specific range - not curently implemented as not fully suported in all db engines.
        /// </summary>
        Range
    }

    /// <summary>
    /// Defines the schema types in a database
    /// </summary>
    [Flags()]
    public enum DBSchemaTypes
    {
        //Not creating anything
        None = 0,

        /// <summary>
        /// A database table
        /// </summary>
        Table = 1,

        /// <summary>
        /// A prefined view on one or more tables
        /// </summary>
        View = 2,

        /// <summary>
        /// A predefined executable procedure 
        /// </summary>
        StoredProcedure = 4,

        /// <summary>
        /// A predfined function that can be caled inline.
        /// </summary>
        Function = 8,

        /// <summary>
        /// A fast access lookup
        /// </summary>
        Index = 16,

        /// <summary>
        /// A set of statements
        /// </summary>
        CommandScripts = 32,

        /// <summary>
        /// An indexed link between 2 tables
        /// </summary>
        ForeignKey = 64,

        /// <summary>
        /// A primary key on a single table
        /// </summary>
        PrimaryKey = 128,

        /// <summary>
        /// A complete database
        /// </summary>
        Database = 256,

        /// <summary>
        /// A database user
        /// </summary>
        User = 512,

        /// <summary>
        /// A databage group or role
        /// </summary>
        Group = 1024,

        /// <summary>
        /// A sequence of numbers
        /// </summary>
        Sequence = 2048


    }

    /// <summary>
    /// A list of possible operations that can be performed on a schema item
    /// </summary>
    public enum DBSchemaOperation
    {
        CheckExists,
        CheckNotExists,
        /// <summary>
        /// e.g create index ... ON table... checks that ON is supported
        /// </summary>
        CreateOn,
        Create,
        Drop,
        Select,
        Insert,
        Update,
        Delete,
        Exec,
        Grant,
        Deny
    }

    /// <summary>
    /// Gets the create options for some schema items
    /// </summary>
    [Flags()]
    public enum CreateOptions
    {
        None = 0,
        Unique = 1,
        Temporary = 2,
        Clustered = 4,
        NonClustered = 8
    }

    /// <summary>
    /// All the collections that can be accessed from the .NET meta data implementation
    /// </summary>
    /// <remarks>The DBSchemaProviders use this implementation to quickly query meta-data information from a database connection</remarks>
    public enum DBMetaDataCollectionType
    {
        /// <summary>
        /// A table of information about the currently connected engine
        /// </summary>
        DataSourceInformation,

        /// <summary>
        /// All the (accessible) databases in the connected engine
        /// </summary>
        Databases,

        /// <summary>
        /// All the tables in the connected engine(restricted to a database)
        /// </summary>
        Tables,

        /// <summary>
        /// All the columns in the connected engine(restricted to a database and table)
        /// </summary>
        Columns,

        /// <summary>
        /// All the views in the connected engine (restricted to a database)
        /// </summary>
        Views,

        /// <summary>
        /// Al the columns in the connected engine (restricted to database and view)
        /// </summary>
        ViewColumns,

        /// <summary>
        /// All the functions and stored procedures in the connected engine (restricted to database).
        /// </summary>
        Procedures,

        /// <summary>
        /// All the parameters in a procedure or function in the connected engine (restricted to database and procedure)
        /// </summary>
        ProcedureParameters,

        /// <summary>
        /// All the foreign keys in the connected engine (restricted to database)
        /// </summary>
        ForeignKeys,

        /// <summary>
        /// All columns associated with the foreign key (restricted to database, table and foreign key)
        /// </summary>
        ForeignKeyColumns,

        /// <summary>
        /// All the indexes in the connected engine (restricted to database and table)
        /// </summary>
        Indexes,

        /// <summary>
        /// All the columns in a specific index (restricted to a database and table and index)
        /// </summary>
        IndexColumns,

        /// <summary>
        /// A set of all the Metadata collections that the current database engine supports retrieval of.
        /// </summary>
        MetaDataCollections,

        /// <summary>
        /// Not known MetaData collection.
        /// </summary>
        Other
    }

    /// <summary>
    /// Defines the type of parameter referencing used by the database engine.
    /// </summary>
    public enum DBParameterLayout
    {
        /// <summary>
        /// The order the parameters are declated in does not matter. The names of the parameters are used as identifiers.
        /// </summary>
        Named,
        /// <summary>
        /// The names of the parameters are ignored and the position (order of referencing and declaration) is the way values are determined
        /// </summary>
        Positional
    }

    /// <summary>
    /// A set of flags to describe the definition of a DBSchemaColumn.
    /// </summary>
    [Flags()]
    public enum DBColumnFlags
    {
        /// <summary>
        /// If set, this column cannot be written to
        /// </summary>
        [XmlEnum("read")]
        ReadOnly = 1,

        /// <summary>
        /// If set, the database engine with automatically assign a value for this column
        /// </summary>
        [XmlEnum("auto")]
        AutoAssign = 2,

        /// <summary>
        /// If set, this is (one of) the unique primary key columns on the table
        /// </summary>
        [XmlEnum("pk")]
        PrimaryKey = 4,

        /// <summary>
        /// If set, the value of this column in a row can be NULL
        /// </summary>
        [XmlEnum("null")]
        Nullable = 8,

        /// <summary>
        /// If set, this column has a default value that will be used if no other value is defined.
        /// </summary>
        [XmlEnum("default")]
        HasDefault = 16
    }


    /// <summary>
    /// The state of a database construct - whether it exists or not (or unknown)
    /// </summary>
    public enum DBExistState
    {
        Unknown,
        Exists,
        NotExists
    }

    /// <summary>
    /// The actions that can be performed on referenced rows forced by a Foreign Key
    /// </summary>
    public enum DBFKAction
    {
        Undefined,
        Cascade,
        NoAction
    }


    /// <summary>
    /// THe available options for ordering of sequence values
    /// </summary>
    public enum DBSequenceOrdering
    {
        None,
        Ordered,
        NotOrdered
    }

    /// <summary>
    /// Available options for the cycling of sequence values
    /// </summary>
    public enum DBSequenceCycling
    {
        None,
        Cycle,
        NoCycle
    }


    public enum DBSequenceBuilderOption
    {
        Minimum,

        Maximim,

        StartValue,

        Cycling,
        NoCycling,

        Ordered,
        NotOrdered,

        Increment,

        Cache,
        NoCaching
    }

    public enum DBTableHint
    {
        NoExpand,
        NoLock,
        HoldLock,
        Index,
        ForceScan,
        ForceSeek,
        NoWait,
        PageLock,
        ReadCommitted,
        ReadCommittedBlock,
        ReadPast,
        ReadUncommitted,
        RepeatableRead,
        RowLock,
        Serializable,
        TabLock,
        TabLockX,
        UpdLock,
        XLock,
        Raw
    }

    public enum DBQueryOption
    {
        HashGroup,
        OrderGroup,
        ConcatUnion,
        HashUnion,
        MergeUnion,
        LoopJoin,
        MergeJoin,
        HashJoin,
        ExpandViews,
        Fast,
        ForceOrder,
        KeepPlan,
        KeepFixedPlan,
        MaxDOP,
        MaxRecursion,
        OptimizeFor,
        OptimiseForUnknown,
        ParametrizationSimple,
        ParameterizationForced,
        Recompile,
        RobustPlan,
        UsePlan,
        Raw

    }


}

#if SILVERLIGHT

namespace System.Data
{
    
    // Summary:
    //     Clone of the System.Data.DbType enumeration in the System.Data dll - which is not available in Silverlight
    public enum DbType
    {
        // Summary:
        //     A variable-length stream of non-Unicode characters ranging between 1 and
        //     8,000 characters.
        AnsiString = 0,
        //
        // Summary:
        //     A variable-length stream of binary data ranging between 1 and 8,000 bytes.
        Binary = 1,
        //
        // Summary:
        //     An 8-bit unsigned integer ranging in value from 0 to 255.
        Byte = 2,
        //
        // Summary:
        //     A simple type representing Boolean values of true or false.
        Boolean = 3,
        //
        // Summary:
        //     A currency value ranging from -2 63 (or -922,337,203,685,477.5808) to 2 63
        //     -1 (or +922,337,203,685,477.5807) with an accuracy to a ten-thousandth of
        //     a currency unit.
        Currency = 4,
        //
        // Summary:
        //     A type representing a date value.
        Date = 5,
        //
        // Summary:
        //     A type representing a date and time value.
        DateTime = 6,
        //
        // Summary:
        //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
        //     7.9 x 10 28 with 28-29 significant digits.
        Decimal = 7,
        //
        // Summary:
        //     A floating point type representing values ranging from approximately 5.0
        //     x 10 -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        Double = 8,
        //
        // Summary:
        //     A globally unique identifier (or GUID).
        Guid = 9,
        //
        // Summary:
        //     An integral type representing signed 16-bit integers with values between
        //     -32768 and 32767.
        Int16 = 10,
        //
        // Summary:
        //     An integral type representing signed 32-bit integers with values between
        //     -2147483648 and 2147483647.
        Int32 = 11,
        //
        // Summary:
        //     An integral type representing signed 64-bit integers with values between
        //     -9223372036854775808 and 9223372036854775807.
        Int64 = 12,
        //
        // Summary:
        //     A general type representing any reference or value type not explicitly represented
        //     by another DbType value.
        Object = 13,
        //
        // Summary:
        //     An integral type representing signed 8-bit integers with values between -128
        //     and 127.
        SByte = 14,
        //
        // Summary:
        //     A floating point type representing values ranging from approximately 1.5
        //     x 10 -45 to 3.4 x 10 38 with a precision of 7 digits.
        Single = 15,
        //
        // Summary:
        //     A type representing Unicode character strings.
        String = 16,
        //
        // Summary:
        //     A type representing a time value.
        Time = 17,
        //
        // Summary:
        //     An integral type representing unsigned 16-bit integers with values between
        //     0 and 65535.
        UInt16 = 18,
        //
        // Summary:
        //     An integral type representing unsigned 32-bit integers with values between
        //     0 and 4294967295.
        UInt32 = 19,
        //
        // Summary:
        //     An integral type representing unsigned 64-bit integers with values between
        //     0 and 18446744073709551615.
        UInt64 = 20,
        //
        // Summary:
        //     A variable-length numeric value.
        VarNumeric = 21,
        //
        // Summary:
        //     A fixed-length stream of non-Unicode characters.
        AnsiStringFixedLength = 22,
        //
        // Summary:
        //     A fixed-length string of Unicode characters.
        StringFixedLength = 23,
        //
        // Summary:
        //     A parsed representation of an XML document or fragment.
        Xml = 25,
        //
        // Summary:
        //     Date and time data. Date value range is from January 1,1 AD through December
        //     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an
        //     accuracy of 100 nanoseconds.
        DateTime2 = 26,
        //
        // Summary:
        //     Date and time data with time zone awareness. Date value range is from January
        //     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through
        //     23:59:59.9999999 with an accuracy of 100 nanoseconds. Time zone value range
        //     is -14:00 through +14:00.
        DateTimeOffset = 27
    }


    // Summary:
    //     Clone of the ParameterDirection enumeration from the System.Data dll that is not available in Silverlight
    public enum ParameterDirection
    {
        // Summary:
        //     The parameter is an input parameter.
        Input = 1,
        //
        // Summary:
        //     The parameter is an output parameter.
        Output = 2,
        //
        // Summary:
        //     The parameter is capable of both input and output.
        InputOutput = 3,
        //
        // Summary:
        //     The parameter represents a return value from an operation such as a stored
        //     procedure, built-in function, or user-defined function.
        ReturnValue = 6
    }
}

#endif