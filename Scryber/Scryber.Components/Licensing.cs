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

namespace Scryber
{
    public static class Licensing
    {
        
        static Licensing()
        {
            
        }

        public static bool IsValidLicense()
        {
            return System.ComponentModel.LicenseManager.IsValid(typeof(PDFLicensedComponent));
        }

        public static void AssertIsValidLicence(string message)
        {
            if (!IsValidLicense())
                throw RecordAndRaise.NoValidLicense(message);
        }

        [System.ComponentModel.LicenseProvider(typeof(Licensing.PDFLicenseProvider))]
        private class PDFLicensedComponent
        {
        }


        private class PDFLicense : System.ComponentModel.License
        {

            public PDFLicense(bool auth)
            {

            }

            public override void Dispose()
            {
                
            }

            public override string LicenseKey
            {
                get { return System.Environment.MachineName; }
            }
        }


        private class PDFLicenseProvider : System.ComponentModel.LicenseProvider
        {
            public override System.ComponentModel.License GetLicense(System.ComponentModel.LicenseContext context, Type type, object instance, bool allowExceptions)
            {
                if (type != typeof(PDFLicensedComponent))
                    throw new ArgumentException("This licence can only be used to validate the PDFX Framework");

                if (context.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    return new PDFLicense(true);
                else
                    return null;
            }
        }

        private class PDFLicenseKey
        {
            
        }


        private class PDFLicenseDecryptor
        {

        }

        private class PDFWebLicenseDecyptor : PDFLicenseDecryptor
        {
        }

        private class PDFApplicationLicenseDecrptor : PDFLicenseDecryptor
        {
        }

    }
}
