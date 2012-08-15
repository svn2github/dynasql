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
using System.Data.Common;

namespace Perceiveit.Data.Profile
{
    public class ProfileConnection : IEquatable<ProfileConnection>
    {
        public string ConnectionString { get; private set; }

        public string ProviderName { get; private set; }
        

        public bool Equals(ProfileConnection other)
        {
            if (string.Equals(this.ProviderName, other.ProviderName)
                && string.Equals(this.ConnectionString, other.ConnectionString))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.ProviderName.GetHashCode() ^ this.ConnectionString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((ProfileConnection)obj);
        }

        public override string ToString()
        {
            return string.Format("Profile connection: {0} {1}", this.ProviderName, this.ConnectionString);
        }
    }
}
