using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Encapsulates the schema properties of a Table index
    /// </summary>
    public class DBSchemaIndexColumn : DBSchemaColumn
    {
        #region ivars

        private Order _sort = Order.Default;
        

        #endregion

        //
        // public properties
        //

        #region public Order SortOrder {get;set;}

        /// <summary>
        /// Gets or Sets the order of the column index
        /// </summary>
        public Order SortOrder 
        {
            get { return this._sort; }
            set { this._sort = value; }
        }

        #endregion

        //
        // .ctors
        //

        #region public DBSchemaIndexColumn()

        /// <summary>
        /// Creates a new DBSchemaIndexColumn
        /// </summary>
        public DBSchemaIndexColumn()
            : base()
        {
        }

        #endregion

        #region public DBSchemaIndexColumn(string name)

        /// <summary>
        /// Creates a new DBSchemaIndexColumn with the specified name
        /// </summary>
        /// <param name="name"></param>
        public DBSchemaIndexColumn(string name)
            : base(name)
        { }

        #endregion


        public override string ToString()
        {
            return string.Format("Column '{0}' {1} (Runtime Type: {6}, Size: {2}, Read Only:{3}, Nullable:{4},  Ordinal:{5})",
                this.Name, this.DbType, this.Size, this.ReadOnly, this.Nullable,  this.OrdinalPosition, this.Type);
        }
    }


    /// <summary>
    /// A collection of DBSchemaIndexColumns accessible by name or by index
    /// </summary>
    public class DBSchemaIndexColumnCollection : System.Collections.ObjectModel.KeyedCollection<string, DBSchemaIndexColumn>
    {

        //
        // .ctors
        //

        #region public DBSchemaIndexColumnCollection()

        public DBSchemaIndexColumnCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        #endregion

        //
        // protected overrides
        //

        #region protected override string GetKeyForItem(DBSchemaIndexColumn item)

        /// <summary>
        /// Gets the key Name value for the Index Column
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(DBSchemaIndexColumn item)
        {
            string name = item.Name;
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException(Errors.CannotAddToSchemaWithNullOrEmptyName);

            return name;
        }

        #endregion


        public IEnumerable<DBSchemaColumn> GetColumns()
        {
            DBSchemaIndexColumn[] cols = new DBSchemaIndexColumn[this.Count];
            this.CopyTo(cols, 0);
            return cols;
        }
    }

}
