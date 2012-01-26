/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Scryber.Components;

namespace Scryber.Data
{
    public abstract class PDFDataSourceBase : PDFVisualComponent, IPDFDataSource
    {

        #region public override bool Visible

        /// <summary>
        /// Overrides the default value to return false, 
        /// as this is not a visible Component of the page
        /// </summary>
        public override bool Visible
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        #endregion

        protected PDFDataSourceBase(PDFObjectType type)
            : base(type)
        {
        }

        private Dictionary<string, object> _sourced;



        #region IPDFDataSource Members

        public object Select(string path)
        {
            object data;
            if (path == null)
                path = string.Empty;

            if (this._sourced == null)
                this._sourced = new Dictionary<string, object>();

            if (this._sourced.Count == 0 || this._sourced.TryGetValue(path, out data) == false)
            {
                try
                {
                    data = this.DoSelectData(path);
                }
                catch (Exception ex)
                {
                    throw RecordAndRaise.Data(ex, Errors.CouldNotSelectData);
                }
                this._sourced.Add(path, data);
            }
            return data;
                
        }

        protected abstract object DoSelectData(string path);


        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this._sourced != null)
                {
                    foreach (object val in _sourced)
                    {
                        if (val is IDisposable)
                            (val as IDisposable).Dispose();
                    }
                }
            }
        }
    }

    public class PDFDataSourceList : System.Collections.ObjectModel.KeyedCollection<string, PDFDataSourceBase>
    {

        protected override string GetKeyForItem(PDFDataSourceBase item)
        {
            return item.ID;
        }

        public bool TryGetSourceWithID(string id, out PDFDataSourceBase source)
        {
            PDFDataSourceBase found;
            if (this.Count == 0 || !this.Dictionary.TryGetValue(id, out found))
            {
                source = null;
                return false;
            }
            else
            {
                source = found;
                return true;
            }

            //TODO: Support DataSourceList-Ref
        }
    }
}
