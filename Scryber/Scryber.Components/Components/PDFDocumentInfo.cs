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
    public class PDFDocumentInfo
    {
        #region ivars

        private string _title;
        private string _author;
        private string _subject;
        private string _keywords;
        private string _creator = Const.PDFXProducer;
        private string _producer = Const.PDFXProducer;
        private DateTime _creationdate = DateTime.Now;
        private DateTime _moddate = DateTime.MinValue;
        private object _trapping;
        private PDFDocumentInfoExtraCollection _extras;

        #endregion

        //
        // element properties
        //

        #region public string Title {get;set;}

        /// <summary>
        /// Gets or sets the set of title associated with this document
        /// </summary>
        [PDFElement("Title")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        #region public string Author {get;set;}

        /// <summary>
        /// Gets or sets the set of author associated with this document
        /// </summary>
        [PDFElement("Author")]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        #endregion

        #region public string Subject {get;set;}

        /// <summary>
        /// Gets or sets the set of subject associated with this document
        /// </summary>
        [PDFElement("Subject")]
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        #endregion

        #region public string Keywords {get;set;}

        /// <summary>
        /// Gets or sets the set of keywords associated with this document
        /// </summary>
        [PDFElement("Keywords")]
        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

        #endregion

        //
        // attribute properties
        //

        #region public string Creator {get;set;}

        /// <summary>
        /// Gets the Creator for this document. If there is no licence then setting the value will fail.
        /// </summary>  
        [PDFAttribute("creator")]
        public string Creator
        {
            get { return _creator; }
            set
            {
                Licensing.AssertIsValidLicence("Document Creator");
                _creator = value;
            }
        }

        #endregion

        #region public string Producer {get;set;}

        /// <summary>
        /// Gets the producer for this document. If there is no licence then setting the value will fail.
        /// </summary>
        [PDFAttribute("producer")]
        public string Producer
        {
            get { return _producer; }
            set
            {
                Licensing.AssertIsValidLicence("Document Producer");
                _producer = value;
            }
        }

        #endregion

        #region public DateTime CreationDate {get;set;}

        /// <summary>
        /// Gets or sets the last modified date for the document.
        /// A value of DateTime.MinValue represents a non-value. The default is the current date time
        /// </summary>
        [PDFAttribute("created-date")]
        public DateTime CreationDate
        {
            get { return _creationdate; }
            set { _creationdate = value; }
        }

        #endregion

        #region public DateTime ModifiedDate {get;set;}

        /// <summary>
        /// Gets or sets the last modified date for the document.
        /// A value of DateTime.MinValue is the default and represents a non-value.
        /// </summary>
        [PDFAttribute("modified-date")]
        public DateTime ModifiedDate
        {
            get { return _moddate; }
            set { _moddate = value; }
        }

        #endregion

        #region public bool Trapped {get; set;} + HasTrapping {get;set;}
        /// <summary>
        /// Gets or sets the Traping flag. 
        /// Returns true if the document has trapping information, or false if it does not (or has not been set).
        /// </summary>
        [PDFAttribute("trapped")]
        public bool Trapped
        {
            get { return (null == _trapping) ? false : (bool)_trapping; }
            set { _trapping = value; }
        }

        /// <summary>
        /// Gets or sets the has trapping information flag.
        /// Returns true if a value has been set (either true or false).
        /// </summary>
        public bool HasTrapping
        {
            get { return _trapping is bool; }
            set
            {
                if (value)
                    _trapping = true;
                else
                    _trapping = null;
            }
        }

        #endregion

        //
        // inner collection of extra elements
        //

        [PDFArray(typeof(PDFDocumentInfoExtra))]
        [PDFElement("")]
        public PDFDocumentInfoExtraCollection Extras
        {
            get { return _extras; }
            set { _extras = value; }
        }

        public bool HasExtras { get { return _extras != null && _extras.Count > 0; } }

    }

    [PDFParsableComponent("Extra")]
    public class PDFDocumentInfoExtra
    {
        [PDFAttribute("name")]
        public string Name { get; set; }

        [PDFElement("")]
        public string Value { get; set; }
    }

    public class PDFDocumentInfoExtraCollection : List<PDFDocumentInfoExtra>
    {
    }
}
