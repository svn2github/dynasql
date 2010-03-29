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

namespace Perceiveit.Data.Query
{
    public abstract class DBParam : DBClause
    {
        public event EventHandler NameChanged;

        
        internal const string ParameterNamePrefix = "_param";

        #region public string Name {get;protected set;}

        private string _name;

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

        public bool HasName
        {
            get { return string.IsNullOrEmpty(this.Name) == false; }
        }

        #endregion

        #region public bool HasValue {get;}

        private bool _hasvalue = false;

        public virtual bool HasValue
        {
            get { return _hasvalue; }
        }

        #endregion

        #region public virtual object Value {get;set;}

        private object _val;
        
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

        #region public bool HasType {get;}

        private bool _hasType = false;

        public bool HasType
        {
            get { return this._hasType; }
        }

        #endregion

        #region public System.Data.DbType DbType

        private System.Data.DbType _type;
        

        public System.Data.DbType DbType
        {
            get 
            {
                return _type; 
            }
            set { _type = value; _hasType = true; }
        }

        #endregion

        #region public int Size {get;set;}

        private int _size = 0;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion

        #region public bool HasSize {get;}

        public bool HasSize
        {
            get { return this._size > 0; }
        }

        #endregion

        #region public System.Data.ParameterDirection Direction {get;set;}

        private System.Data.ParameterDirection _direction = System.Data.ParameterDirection.Input;

        public System.Data.ParameterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        #endregion

        
        //
        // .ctors

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

        protected override string XmlElementName
        {
            get { return XmlHelper.Parameter; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasName)
                this.WriteAttribute(writer, XmlHelper.Name, this.Name, context);

            if(context.Parameters.Contains(this) == false)
                context.Parameters.Add(this);

            return true;
            
        }

        #endregion

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

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
    
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            return true;
        }

        #endregion


        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b = true;
            if (this.IsAttributeMatch(XmlHelper.ParameterDirection, reader, context) && !string.IsNullOrEmpty(reader.Value))
            {
                this.Direction = (System.Data.ParameterDirection)Enum.Parse(typeof(System.Data.ParameterDirection), reader.Value, true);
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
                this.DbType = (System.Data.DbType)Enum.Parse(typeof(System.Data.DbType), reader.Value);
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
       
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

        #region public static DBParam Param(string name) + 1 overloads

        public static DBParam Param(string name)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;

            return pref;
        }

        public static DBParam Param(string name, System.Data.DbType type)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;
            pref.DbType = type;

            return pref;
        }

        public static DBParam Param(string name, System.Data.DbType type, int size)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;
            pref.DbType = type;
            pref.Size = size;
            return pref;
        }

        public static DBParam Param(string name, System.Data.DbType type, System.Data.ParameterDirection direction)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;
            pref.DbType = type;
            pref.Direction = direction;
            return pref;
        }

        public static DBParam Param(string name, System.Data.DbType type, int size, System.Data.ParameterDirection direction)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;
            pref.DbType = type;
            pref.Size = size;
            pref.Direction = direction;
            return pref;
        }



        #endregion

        #region public static DBParam ParamWithValue(object paramValue) + 3 overloads

        public static DBParam ParamWithValue(object paramValue)
        {
            DBParam pref = new DBParamRef();
            pref.Value = paramValue;
            return pref;
        }

        public static DBParam ParamWithValue(System.Data.DbType type, object paramValue)
        {
            DBParam pref = new DBParamRef();
            pref.Value = paramValue;
            return pref;
        }

        public static DBParam ParamWithValue(string name, object value)
        {
            DBParam pref = new DBParamRef();
            pref.Value = value;
            pref.Name = name;

            return pref;
        }

        public static DBParam ParamWithValue(string name, System.Data.DbType type, object value)
        {
            DBParam pref = new DBParamRef();
            pref.Name = name;
            pref.DbType = type;
            pref.Value = value;
            return pref;
        }


        #endregion

        #region public static DBParam ParamWithDelegate(ParamValue valueprovider) + 3 overloads

        public static DBParam ParamWithDelegate(ParamValue valueprovider)
        {
            DBDelegateParam del = new DBDelegateParam(valueprovider);
            
            return del;
        }

        public static DBParam ParamWithDelegate(System.Data.DbType type, ParamValue valueProvider)
        {
            DBDelegateParam del = new DBDelegateParam(valueProvider);
            del.DbType = type;
            return del;
        }

        public static DBParam ParamWithDelegate(string name, ParamValue valueprovider)
        {
            DBDelegateParam del = new DBDelegateParam(valueprovider);
            del.Name = name;
            return del;
        }

        public static DBParam ParamWithDelegate(string name, System.Data.DbType type, ParamValue valueprovider)
        {
            DBDelegateParam del = new DBDelegateParam(valueprovider);
            del.DbType = type;
            del.Name = name;
            return del;
        }

        #endregion

        #region internal static DBClause Param()

        internal static DBParam Param()
        {
            return new DBParamRef();
        }

        #endregion
    }


    internal class DBParamRef : DBParam, IDBValueSource
    {

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            this.Name = builder.RegisterParameter(this);

            builder.WriteParameterReference(this.Name);

            return true;

        }

    }

    internal class DBDelegateParam : DBParam, IDBValueSource
    {
        private ParamValue _val;

        public ParamValue ValueDelegate
        {
            get { return this._val; }
            set { this._val = value; }
        }

        public override bool HasValue
        {
            get
            {
                return this._val != null;
            }
        }

        public override object Value
        {
            get 
            { 
                return this.ValueDelegate();
            }
            set { throw new InvalidOperationException("Cannot set the value on a delegated parameter"); }
        }

        public DBDelegateParam(ParamValue value)
            : base()
        {
            this._val = value;
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            this.Name = builder.RegisterParameter(this);

            builder.WriteParameterReference(this.Name);

            return true;

        }
        

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

    public class DBParamList : System.Collections.ObjectModel.KeyedCollection<string, DBParam>
    {
        protected override string GetKeyForItem(DBParam item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, DBParam item)
        {
            if(null != item)
                item.NameChanged += new EventHandler(item_NameChanged);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            DBParam item = this[index];
            if (null != item)
                item.NameChanged -= new EventHandler(this.item_NameChanged);
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (DBParam item in this)
            {
                item.NameChanged -= new EventHandler(this.item_NameChanged);
            }
            base.ClearItems();
        }

        protected override void SetItem(int index, DBParam item)
        {
            DBParam orig = this[index];
            if (null != orig)
                orig.NameChanged -= new EventHandler(this.item_NameChanged);
            if(null != item)
                item.NameChanged += new EventHandler(this.item_NameChanged);

            base.SetItem(index, item);
        }


        void item_NameChanged(object sender, EventArgs e)
        {
            DBParam param = (DBParam)sender;
            this.ChangeItemKey(param, param.Name);
        }

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
    }
}
