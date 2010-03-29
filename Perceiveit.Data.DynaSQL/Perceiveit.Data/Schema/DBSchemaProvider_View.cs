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

namespace Perceiveit.Data.Schema
{
    
    public partial class DBSchemaProvider
    {
        //
        // GetViewSchema
        //

        #region public DBSchemaView GetViewSchema(string catalog, string owner, string name) + 2 overloads

        public DBSchemaView GetViewSchema(string name)
        {
            return this.GetViewSchema(string.Empty, string.Empty, name);
        }

        public DBSchemaView GetViewSchema(string catalog, string owner, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBSchemaItemRef itemref = new DBSchemaItemRef();
            itemref.Catalog = catalog;
            itemref.Schema = owner;
            itemref.Name = name;
            itemref.Type = DBSchemaTypes.View;

            return this.GetViewSchema(itemref);
        }

        public DBSchemaView GetViewSchema(DBSchemaItemRef viewRef)
        {
            if (null == viewRef)
                throw new ArgumentNullException("viewRef");
            if (string.IsNullOrEmpty(viewRef.Name))
                throw new ArgumentNullException("viewRef.Name");
            if (viewRef.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("viewRef.Type");

            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                DBSchemaView view = this.LoadAView(con, viewRef);
                return view;
            }
        }

        #endregion

        protected abstract DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef forRef);
       
    }
}
