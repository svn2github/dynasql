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
using System.Xml;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Defines a parameter for a database query. 
    /// Use the static (shared) methods to create a new instance
    /// </summary>
    /// <remarks>
    /// A DBParam can either be a specific value, or a delegate method evaluated at execution time. 
    /// If a name is not provided then it be dynamically set when the SQL statement is built.<br/>
    /// The delegate values are not carried across a serialized Xml query. 
    /// The parameter will be evaulated and used as a constant when deserialized.</remarks>
    public abstract class DBParam : DBClause
    {
        /// <summary>
        /// Event that is raised when this parameters name is changed
        /// </summary>
        public event EventHandler NameChanged;

        
        internal const string ParameterNamePrefix = "_param";

        #region public string Name {get; set;}

        private string _name;

        /// <summary>
        /// Gets or sets the name of the parameter - raises the name changed event when set.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set 
            {
                string orig = _name;
                _name = value;

                if (string.Equals(orig, this._name) == false)
                {
                    this.OnNameChanged(EventArgs.Empty);
                }
            }
        }

        #endregion

        #region public bool HasName {get;}

        /// <summary>
        /// Returns true if this parameter has a non-null name
        /// </summary>
        public bool HasName
        {
            get { return string.IsNullOrEmpty(this.Name) == false; }
        }

        #endregion


        #region public virtual bool HasValue {get;}

        private bool _hasvalue = false;
        /// <summary>
        /// Returns true if this parameter has an explicit value - inheritors can override
        /// </summary>
        public virtual bool HasValue
        {
            get { return _hasvalue; }
        }

        #endregion

        #region public virtual object Value {get;set;}

        private object _val;

        /// <summary>
        /// Gets the value of the parameter 
        /// (inheritors can override this property - but should also override the HasValue property)
        /// </summary>
        public virtual object Value
        {
            get { return _val; }
            set 
            {
                this._hasvalue = true;
                _val = value;
            }
        }

        #endregion


        #region public System.Data.DbType DbType

        private System.Data.DbType _type;

        /// <summary>
        /// Gets or sets the DbType of this parameter
        /// </summary>
        public System.Data.DbType DbType
        {
            get
            {
                return _type;
            }
            set { _type = value; _hasType = true; }
        }

        #endregion

        #region public bool HasType {get;}

        private bool _hasType = false;
        /// <summary>
        /// Returns true if this parameter has an explicit DbType
        /// </summary>
        public bool HasType
        {
            get { return this._hasType; }
        }

        #endregion


        #region public int Size {get;set;}

        private int _size = 0;
        /// <summary>
        /// Gets or sets the explicit size of this parameter
        /// </summary>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion

        #region public int Precision {get;set;}

        private int _precision;

        /// <summary>
        /// Gets or sets the precision of this data type
        /// </summary>
        public int Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        #endregion

        #region public bool HasSize {get;}

        /// <summary>
        /// Returns true if this parameter has an explicit size
        /// </summary>
        public bool HasSize
        {
            get { return this._size > 0; }
        }

        #endregion

        #region public System.Data.ParameterDirection Direction {get;set;}


        private System.Data.ParameterDirection _direction = System.Data.ParameterDirection.Input;

        /// <summary>
        /// Gets or sets the direction of this parameter. 
        /// To access Output parameters they must be explicitly declared in your code
        /// </summary>
        public System.Data.ParameterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        #endregion

        
        //
        // .ctors

        /// <summary>
        /// protected parsmeterless ctor
        /// </summary>
        protected DBParam() { }


        //
        // methods
        //

        #region protected virtual void OnNameChanged(EventArgs args)

        /// <summary>
        /// Raises the NameChanged event
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnNameChanged(EventArgs args)
        {
            if (null != this.NameChanged)
                this.NameChanged(this, args);
        }

        #endregion

        #region protected virtual object GetDbValue(object value)

        /// <summary>
        /// Gets the Database appropriate value - base implementation returns DBNull for null (Nothing).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual object GetDbValue(object value)
        {
            if (null == value)
                return DBNull.Value;
            else
                return value;
        }

        #endregion

       
        //
        // Xml Serialization
        //

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Returns the name for this XmlElement
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Parameter; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        /// <summary>
        /// Writes all the Xml Attributes for this parameter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasName)
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);

            if(context.Parameters.Contains(this) == false)
                context.Parameters.Add(this);

            return true;
            
        }

        #endregion

        #region public bool WriteFullParameterXml(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Writes all the values for this DBParam instance
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool WriteFullParameterXml(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            writer.WriteStartElement(this.XmlElementName);
            
            if (this.HasName)
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);
            if (this.HasType)
                this.WriteAttribute(writer, XmlHelper.DbType, this.DbType.ToString(), context);
            if (this.HasSize)
                this.WriteAttribute(writer, XmlHelper.ParameterSize, this.Size.ToString(), context);
            
            this.WriteAttribute(writer, XmlHelper.ParameterDirection, this.Direction.ToString(), context);

            base.WriteAllAttributes(writer, context);

            XmlHelper.WriteNativeValue(this.Value, writer, context);

            writer.WriteEndElement();

            return true;
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        /// <summary>
        /// Overrides the base implementation an returns true
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            return true;
        }

        #endregion


        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        /// <summary>
        /// Reads the attributes of this parameter and returns true if it was a known attribute. 
        /// Otherwise returns the base method output
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            if (this.IsAttributeMatch(XmlHelper.ParameterDirection, reader, context) && !string.IsNullOrEmpty(reader.Value))
            {
                this.Direction = XmlHelper.ParseEnum<System.Data.ParameterDirection>(reader.Value);
            }
            else if (this.IsAttributeMatch(XmlHelper.ParameterSize, reader, context) && !string.IsNullOrEmpty(reader.Value))
            {
                this.Size = int.Parse(reader.Value);
            }
            else if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.Name = reader.Value;
            }
            else if (this.IsAttributeMatch(XmlHelper.DbType, reader, context) && !string.IsNullOrEmpty(reader.Value))
            {
                this.DbType = XmlHelper.ParseEnum<System.Data.DbType>(reader.Value);
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
       
        /// <summary>
        /// Reads any known iner elements
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (reader.NodeType == System.Xml.XmlNodeType.Text && reader.Value == XmlHelper.NullString)
                this.Value = DBNull.Value;
            else
            {
                if (reader.IsEmptyElement == false)
                {
                    string end = reader.LocalName;

                    do
                    {
                        if (reader.NodeType == System.Xml.XmlNodeType.EndElement && this.IsElementMatch(end, reader, context))
                            break;
                        else if (reader.NodeType == System.Xml.XmlNodeType.Element && this.IsElementMatch(XmlHelper.ParameterValue, reader, context))
                        {
                            this.Value = XmlHelper.ReadNativeValue(reader, context);
                        }
                    }
                    while (reader.Read());
                }
            }
            return base.ReadAnInnerElement(reader, context);
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBParam Param(string name) + 3 overloads

        /// <summary>
        /// Creates a new DBParam clause with the specified generic name. Cannot support delegate methods as values
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <returns></returns>
        public static DBParam Param(string genericName)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;

            return pref;
        }

        /// <summary>
        /// Creates a new DBParam clause with the specified name and DBType. Cannot support delegate methods as values
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <returns></returns>
        public static DBParam Param(string genericName, System.Data.DbType type)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;

            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, DBtype and size. Cannot support delegate methods as values
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="size">The (maximum) size of the parameter</param>
        /// <returns></returns>
        public static DBParam Param(string genericName, System.Data.DbType type, int size)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;
            pref.Size = size;
            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, DBType and direction. Cannot support delegate methods as values
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="direction">The parameters direction</param>
        /// <returns></returns>
        public static DBParam Param(string genericName, System.Data.DbType type, System.Data.ParameterDirection direction)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;
            pref.Direction = direction;
            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, DBType, size and direction. Cannot support delegate methods as values
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="size">The (maximum) size of the parameter value</param>
        /// <param name="direction">The parameters direction</param>
        /// <returns></returns>
        public static DBParam Param(string genericName, System.Data.DbType type, int size, System.Data.ParameterDirection direction)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;
            pref.Size = size;
            pref.Direction = direction;
            return pref;
        }



        #endregion

        #region public static DBParam ParamWithValue(object paramValue) + 3 overloads

        /// <summary>
        /// Creates a new DBParam with an constant value. 
        /// NOTE: The parameter type cannot be determined if null or DBUnll is passed as the value
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static DBParam ParamWithValue(object paramValue)
        {
            DBParam pref = new DBParamRef();
            pref.Value = paramValue;
            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the DbType and constant value.
        /// </summary>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="paramValue">The value of the parameter</param>
        /// <returns></returns>
        public static DBParam ParamWithValue(System.Data.DbType type, object paramValue)
        {
            DBParam pref = new DBParamRef();
            pref.Value = paramValue;
            pref.DbType = type;
            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the name and constant value.
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public static DBParam ParamWithValue(string genericName, object value)
        {
            DBParam pref = new DBParamRef();
            pref.Value = value;
            pref.Name = genericName;

            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, type, and constant value
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <returns></returns>
        public static DBParam ParamWithValue(string genericName, System.Data.DbType type, object value)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;
            pref.Value = value;
            return pref;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, type, length and value
        /// </summary>
        /// <param name="genericName"></param>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DBParam ParamWithValue(string genericName, System.Data.DbType type, int length, object value)
        {
            DBParam pref = new DBParamRef();
            pref.Name = genericName;
            pref.DbType = type;
            pref.Value = value;
            pref.Size = length;
            return pref;
        }


        #endregion

        #region public static DBParam ParamWithDelegate(ParamValue valueprovider) + 3 overloads

        /// <summary>
        /// Creates a new DBParam with a delegate method as the value provider (no parameters, returns object).
        /// NOTE: The parameter type cannot be determined if null or DBUnll is passed as the value
        /// </summary>
        /// <param name="valueprovider">The delegate method (cannot be null)</param>
        /// <returns></returns>
        /// <remarks>
        ///  The method will be executed when any queries using this parameter are converted to their SQL statements
        /// </remarks>
        public static DBParam ParamWithDelegate(ParamValue valueprovider)
        {
            if (null == valueprovider)
                throw new ArgumentNullException("valueProvider");

            DBDelegateParam del = new DBDelegateParam(valueprovider);
            
            return del;
        }

        /// <summary>
        /// Creates a new DBParam with a delegate method as the value provider (no parameters, returns object). 
        /// </summary>
        /// <param name="type">The DbType of the parameter </param>
        /// <param name="valueProvider">The delegate method (cannot be null)</param>
        /// <returns></returns>
        /// <remarks>
        ///  The valueProvider method will be executed when any queries using this parameter are converted to their SQL statements
        /// </remarks>
        public static DBParam ParamWithDelegate(System.Data.DbType type, ParamValue valueProvider)
        {
            if (null == valueProvider)
                throw new ArgumentNullException("valueProvider");

            DBDelegateParam del = new DBDelegateParam(valueProvider);
            del.DbType = type;
            return del;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name and a delegate method as the value provider (no parameters, returns object).
        /// NOTE: The parameter type cannot be determined if null or DBUnll is passed as the value
        /// </summary>
        /// <param name="genericName">The name of the parameter</param>
        /// <param name="valueprovider">The delegate method (cannot be null)</param>
        /// <returns></returns>
        /// <remarks>
        ///  The method will be executed when any queries using this parameter are converted to their SQL statements
        /// </remarks>
        public static DBParam ParamWithDelegate(string genericName, ParamValue valueprovider)
        {
            if (null == valueprovider)
                throw new ArgumentNullException("valueProvider");

            DBDelegateParam del = new DBDelegateParam(valueprovider);
            del.Name = genericName;
            return del;
        }


        /// <summary>
        /// Creates a new DBParam with the specified name, type and a delegate method as the value provider (no parameters, returns object).
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="valueprovider">The delegate method (cannot be null)</param>
        /// <returns></returns>
        /// <remarks>
        ///  The method will be executed when any queries using this parameter are converted to their SQL statements
        /// </remarks>
        public static DBParam ParamWithDelegate(string genericName, System.Data.DbType type, ParamValue valueprovider)
        {
            if (null == valueprovider)
                throw new ArgumentNullException("valueProvider");

            DBDelegateParam del = new DBDelegateParam(valueprovider);
            del.DbType = type;
            del.Name = genericName;
            return del;
        }

        /// <summary>
        /// Creates a new DBParam with the specified name, type, size and a delegate method as the value provider (no parameters, returns object).
        /// </summary>
        /// <param name="genericName">The generic name of the parameter (e.g. param1 rather than @param1)</param>
        /// <param name="type">The DbType of the parameter</param>
        /// <param name="size">The maximum size of the parameter value</param>
        /// <param name="valueprovider">The delegate method (cannot be null)</param>
        /// <returns></returns>
        /// <remarks>
        ///  The method will be executed when any queries using this parameter are converted to their SQL statements
        /// </remarks>
        public static DBParam ParamWithDelegate(string genericName, System.Data.DbType type, int size, ParamValue valueprovider)
        {
            if (null == valueprovider)
                throw new ArgumentNullException("valueProvider");

            DBDelegateParam del = new DBDelegateParam(valueprovider);
            del.DbType = type;
            del.Size = size;
            del.Name = genericName;
            return del;
        }

        #endregion

        #region internal static DBClause Param()

        /// <summary>
        /// Internal method to create an empty parameter
        /// </summary>
        /// <returns></returns>
        internal static DBParam Param()
        {
            return new DBParamRef();
        }

        #endregion
    }

    //
    // subclasses
    //

    #region internal class DBParamRef : DBParam, IDBValueSource

    /// <summary>
    /// Implementation of a value parameter
    /// </summary>
    internal class DBParamRef : DBParam, IDBValueSource
    {

#if SILVERLIGHT
        // no statement building
#else

        /// <summary>
        /// Overrides the default behaviour to write the parameter
        /// </summary>
        /// <param name="builder">The SQL statement builder</param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            this.Name = builder.RegisterParameter(this);

            builder.WriteNativeParameterReference(this.Name);

            return true;

        }

#endif

    }

    #endregion


    #region internal class DBDelegateParam : DBParam, IDBValueSource

    /// <summary>
    /// Implementation of a DBParam parameter that gets its value from a delgate method
    /// </summary>
    internal class DBDelegateParam : DBParam, IDBValueSource
    {
        private ParamValue _val;

        /// <summary>
        /// Gets or sets the delegate
        /// </summary>
        public ParamValue ValueDelegate
        {
            get { return this._val; }
            set { this._val = value; }
        }

        /// <summary>
        /// Overrides to return true if there is a delegate set
        /// </summary>
        public override bool HasValue
        {
            get
            {
                return this._val != null;
            }
        }

        /// <summary>
        /// Overrides to return the value from the executed delegate
        /// </summary>
        public override object Value
        {
            get 
            { 
                return this.ValueDelegate();
            }
            set { throw new InvalidOperationException("Cannot set the value on a delegated parameter"); }
        }

        /// <summary>
        /// Creates a new DBDelegateParam
        /// </summary>
        /// <param name="value"></param>
        public DBDelegateParam(ParamValue value)
            : base()
        {
            this._val = value;
        }


#if SILVERLIGHT
        // no statement building
#else

        /// <summary>
        /// Overrides the default behaviour to register the parameter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            this.Name = builder.RegisterParameter(this);

            builder.WriteNativeParameterReference(this.Name);

            return true;

        }
        
#endif


        #region XMLSerialization Read methods - NOT Supported

        //
        // cannot read a delegate parameter
        // it's value has been fixed on serialization and becomes simply a parameter
        //

        protected override bool ReadAllAttributes(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            throw new NotSupportedException(Errors.NotSerializeDelegate);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            throw new NotSupportedException(Errors.NotSerializeDelegate);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            throw new NotSupportedException(Errors.NotSerializeDelegate);
        }

        

        #endregion

    }

    #endregion

    //
    // collection classes
    //

    #region public class DBParamList : System.Collections.ObjectModel.KeyedCollection<string, DBParam>

    /// <summary>
    /// Defines a collection of DBParam's that can be accessed by index or generic Name
    /// </summary>
    public class DBParamList : System.Collections.ObjectModel.KeyedCollection<string, DBParam>
    {
        /// <summary>
        /// Overriden to return the name of the parameter
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(DBParam item)
        {
            return item.Name;
        }

        /// <summary>
        /// overriden to attach to the NameChanged event so the key for this item can be updated
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, DBParam item)
        {
            if(null != item)
                item.NameChanged += new EventHandler(item_NameChanged);
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Overriden to detach from the NameChanged event handler
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            DBParam item = this[index];
            if (null != item)
                item.NameChanged -= new EventHandler(this.item_NameChanged);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Overriden to detach all contained parameters from their NameChanged event
        /// </summary>
        protected override void ClearItems()
        {
            foreach (DBParam item in this)
            {
                item.NameChanged -= new EventHandler(this.item_NameChanged);
            }
            base.ClearItems();
        }
        
        /// <summary>
        /// Overriden to detach the current and attach the new name changed event handlers
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, DBParam item)
        {
            DBParam orig = this[index];
            if (null != orig)
                orig.NameChanged -= new EventHandler(this.item_NameChanged);
            if(null != item)
                item.NameChanged += new EventHandler(this.item_NameChanged);

            base.SetItem(index, item);
        }

        /// <summary>
        /// Handles an items name changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_NameChanged(object sender, EventArgs e)
        {
            DBParam param = (DBParam)sender;
            this.ChangeItemKey(param, param.Name);
        }

        /// <summary>
        /// Attempts to retrieve the parameter with the specified name, 
        /// and returns true if one was found, otherwise false
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aparam"></param>
        /// <returns></returns>
        internal bool TryGetParameter(string name, out DBParam aparam)
        {
            if (this.Count == 0)
            {
                aparam = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(name, out aparam);
        }


#if SILVERLIGHT
        // no statement building
#else

        public bool BuildStatement(DBStatementBuilder builder, bool newline, bool indent)
        {
            bool outputseparator = false;
            int count = 0;
            foreach (DBParam param in this)
            {
                if (outputseparator)
                {
                    builder.AppendReferenceSeparator();

                    if (newline)
                        builder.BeginNewLine();
                }

                outputseparator = param.BuildStatement(builder);

                if (outputseparator)
                    count++;
            }

            return count > 0;
        }

#endif
        public bool ReadXml(string endElement, XmlReader reader, XmlReaderContext context)
        {
            bool isEmpty = reader.IsEmptyElement && XmlHelper.IsElementMatch(endElement, reader, context);

            do
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {

                    DBParam p = DBParam.Param();
                        this.Add(p);

                    if (isEmpty)
                        return true;
                }

                if (reader.NodeType == System.Xml.XmlNodeType.EndElement && XmlHelper.IsElementMatch(endElement, reader, context))
                    break;
            }
            while (reader.Read());

            return true;
        }

        public bool WriteXml(XmlWriter writer, XmlWriterContext context)
        {
            if (this.Count > 0)
            {
                foreach (DBClause tkn in this)
                {
                    tkn.WriteXml(writer, context);
                }
                return true;
            }
            else
                return false;
        }
    }

    #endregion
}
