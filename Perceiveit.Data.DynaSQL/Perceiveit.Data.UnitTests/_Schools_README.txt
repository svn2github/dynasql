
******************************************************
Using the Perceiveit.Data.UnitTests

Author:  Richard Hewitson
Date:    15 August 2012
Version: 1.5
******************************************************


The MySQL, Oracle, SQLClient and SQLite folders in this project contain a matching suite of tests
that can be executed against each database engine.

The suite will generated all database objects required, populate and interrogate this data, and 
finally drop all associated objects.

The Schools.cs file in each directory defines the connection details and schema of the database objects.
The associated [Engine]Complete.cs file contains the actual tests.
The [Engine]All.orderedtest file will run the complete suite in one go.

Change the connection string and optionally the Schema to your desired engine *before* executing in Schools.cs.


About the Schools Database schema.
---------------------------------

The schools database is a fictional schema for a series of departments, courses and staff
There are various data types used, along with indexes and foreign keys to related tables.
Those database engines that support Stored procedures will also have objects created.

DSQL_DEPARTMENTS - a table of departments with auto incrementing ID 
DSQL_COURSES - a table of courses within each department. Each department has one administrator.
DSQL_ONSITE_COURSES - a course related table for those that are available on site.
DSQL_ONLINE_COURSES - a course related table for those that are available online.
DSQL_PERSON - a table of individuals.
DSQL_INSTRUCTORS - a many to many look up between courses and people.

DSQL_ONLY_ONSITE_COURSES - a view of only on site courses linking both DSQL_ONSITE_COURSES and DSQL_COURSES
DSQL_ONLY_ONLINE_COURSES - a view of only online courses linking both DSQL_ONLINE_COURSES and DSQL_COURSES

DSQL_idx_COURSE_title - a unique index of course titles
DSQL_idx_DEPARTMENT_name - a unique index of department names

DSQL_sp_INSERT_ONLINE_COURSE - a stored procedure that will insert into both COURSE(s) and ONLINE_COURSE(s)
DSQL_sp_INSERT_ONSITE_COURSE - a correcsponding stored procedure for COURSE and ONLINE_COURSE
(the SQLite database engine does not support stored procedures)

Each of the objects is created (and dropped) using a DBQuery dynamic statement that is executed from the library.
This capability is new to this version of the the library.


Data Loading
------------

All the data that is inserted into the objects is loaded from CSV data (see CSVData.cs) that is stored in the
resources file SampleData.resx.
The actual text is in the SampleData project folder so can be added to as required.

Schema Validation
-----------------

The library includes full schema validation mechanism. So the metadata can be checked once it has been created
and compared against the defined schema in Schools.cs

Tear Down
---------

As part of the ordered tests the schema objects will be DROPped from the connected database
after suite completion.
All the ordered tests are set to continue running after a failure, so if you need to validate particular items
either remove the tear down test method, or run the other tests in order manually.

***************************************
Other Tests in this project
***************************************

XML
---

All the Query objects support XML serialization and deserialization.
The tests in the XML project folder build DBQuery instances and then validate that the
SQL statement built matches before and after conversion.
There is an ordered test (XMLSerialization) that will run all these tests in one go.

Northwind
---------

The DynaSQLCreateTests and DynaSQLTests will interact with a Northwind database instance
that is expected to be connected via the database connection string in NorthwindSchema.cs
These are considered legacy and will not be added to, but do perform specific checks that
are not at the moment included in the Schools unit tests



