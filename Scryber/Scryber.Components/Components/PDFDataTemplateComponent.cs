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

namespace Scryber.Components
{
    /// <summary>
    /// Adds a DataSource and select path to the template Component so that it can add new sources
    /// to the context before binding.
    /// </summary>
    public abstract class PDFDataTemplateComponent : PDFBindingTemplateComponent
    {

        #region public string DataSourceID {get;set;}

        private string _srcid;

        /// <summary>
        /// Gets or sets the ID of a Data Source Component such as PDFXMLDataSource. 
        /// Cannot be set if there is an existing 'DataSource' value.
        /// </summary>
        [PDFAttribute("datasource-id")]
        public string DataSourceID
        {
            get { return _srcid; }
            set
            {
                if (string.IsNullOrEmpty(value) == false && _source != null)
                    throw RecordAndRaise.Operation(Errors.CannotSetDataSourceAndDataSourceID);
                _srcid = value;
            }
        }

        #endregion

        #region public object DataSource {get;set;}

        private object _source = null;

        /// <summary>
        /// Gets or sets the data that will be used in binding.
        /// Cannot be set if there is an assigned 'DataSourceID'
        /// </summary>
        public object DataSource
        {
            get
            {
                return _source;
            }
            set
            {
                if (value != null && string.IsNullOrEmpty(this._srcid) == false)
                    throw RecordAndRaise.Operation(Errors.CannotSetDataSourceAndDataSourceID);
                _source = value;
            }
        }

        #endregion

        #region public string SelectPath {get;set;}

        private string _sourcepath;
        /// <summary>
        /// Gets or sets any XPath expression to use to extract the results
        /// </summary>
        [PDFAttribute("select")]
        public string SelectPath
        {
            get { return _sourcepath; }
            set { _sourcepath = value; }
        }


        #endregion

        #region protected bool HasCustomDataSource {get;}

        /// <summary>
        /// Returns true if this data template has any settings that should change the current
        /// data context.
        /// </summary>
        protected bool HasCustomDataSource
        {
            get
            {
                return string.IsNullOrEmpty(this.DataSourceID) == false || this.DataSource != null || !string.IsNullOrEmpty(this.SelectPath);
            }
        }

        #endregion



        public PDFDataTemplateComponent(PDFObjectType type)
            : base(type)
        {
        }


        

        /// <summary>
        /// Returns the required binding data for this DataTemplateComponent (using the DataSourceID, DataSource, stack and any select path).
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        protected virtual object GetBindingData(PDFDataStack stack)
        {
            return GetBindingData(this.DataSourceID, this.DataSource, stack, this.SelectPath);
        }

        /// <summary>
        /// Returns the required binding data based upon the specified parameters or null if there is no required source.
        /// </summary>
        /// <param name="datasourceComponentID">The ID of an Component in the document that implements the IPDFDataSource interface. 
        /// Set it to null or an empty string to ignore this parameter.
        /// If it is set, and so is the select path, then this path will be used by the IPDFDataSource to extract the required source
        /// </param>
        /// <param name="datasourcevalue">The value of the data source. 
        /// If not null then the value will be returned unless a select path is set. 
        /// If the select path is set then this value must be IXPathNavigable and the returned value will be the result of a select on the navigator</param>
        /// <param name="stack">If neither the id or value are set, then the current data from the stack will be 
        /// used if and only if theres is a select path. If there is no select path then null will be returned.
        /// If there is a select path then the current data must implement IXPathNavigable</param>
        /// <param name="selectpath">The path to  use to extract values</param>
        /// <returns>The required data or null.</returns>
        protected virtual object GetBindingData(string datasourceComponentID, object datasourcevalue, PDFDataStack stack, string selectpath)
        {
            IPDFDataSource datasourceComponent = null;
            if (string.IsNullOrEmpty(datasourceComponentID) == false)
            {
                PDFComponent found = base.FindDocumentComponentById(datasourceComponentID);
                if (found == null)
                    throw RecordAndRaise.ArgumentNull("DataSourceID", Errors.CouldNotFindControlWithID, datasourceComponentID);
                else if (!(found is IPDFDataSource))
                    throw RecordAndRaise.Argument("DataSourceID", Errors.AssignedDataSourceIsNotIPDFDataSource, datasourceComponentID);
                else
                    datasourceComponent = ((IPDFDataSource)found);

            }
            return PDFDataHelper.GetBindingData(datasourceComponent, datasourcevalue, stack, selectpath);
        }


        
    }
}
