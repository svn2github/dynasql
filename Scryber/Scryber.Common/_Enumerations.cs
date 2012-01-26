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
using System.Linq;
using System.Text;

namespace Scryber
{
    /// <summary>
    /// Available trace levels with which to output messages
    /// </summary>
    /// <remarks>
    /// Each trace message can specify one of the following levels of severity. The levels are cascading with the <see cref="TraceRecordLevel"/>.
    /// If recording messages, then trace entries with the levels of Message, Warning, Error and Failure should also be recorded.
    /// If recording 'Errors', then only trace entries of Error and Failure should be recorded.
    /// </remarks>
    public enum TraceLevel
    {
        /// <summary>
        /// Identifies a complete and utter failure of the stystem that nothing can be done about to recover from.
        /// </summary>
        Failure = 20,
        /// <summary>
        /// Identfies that an error has occurred that can and should be external handled by the calling code.
        /// </summary>
        Error = 10,
        /// <summary>
        /// Identifies an invalid state of some object or component, but the system can continue as normal.
        /// </summary>
        Warning = 5,
        /// <summary>
        /// A message identifies no error or warning state, but may be of use in tracking program operation
        /// </summary>
        Message = 3,
        /// <summary>
        /// A diagnostic message that should not normally be recorded, unless trying to diagnose a specific issue.
        /// </summary>
        Debug = 1,
    }

    public enum TraceRecordLevel
    {
        All = 0,
        Messages = 3,
        Warnings = 5,
        Errors = 10,
        Off = 100
    }

    public enum ConformanceMode
    {
        Strict,
        Lax
    }

    public enum DrawingOrigin
    {
        TopLeft,
        BottomLeft
    }

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Justified
    }

    public enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom,
        Justified
    }

    public enum TextFormat
    {
        Plain,
        XML,

        [Obsolete("The RTF Format is not currently supported")]
        RTF
    }

    public enum PatternType
    {
        TilingPattern = 1,
        ShadingPattern = 2
    }

    public enum PatternPaintType
    {
        ColoredTile = 1,
        UncolouredTile = 2
    }

    public enum PatternTilingType
    {
        ConstantSpacing = 1,
        NoDistortion = 2,
        ConstantFast = 3
    }
}
