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
using System.Configuration;
namespace Scryber.Configuration
{
    /// <summary>
    /// Static (shared) accessor class for the Scryber configuration section
    /// </summary>
    /// <remarks>PDFX supports the use of configuration for tracing and font-mappings by adding a &lt;Scryber&gt section to the config file.
    /// Register the section using 
    /// &ltSection name="Scryber" 
    ///         type="Scryber.Configuration.PDFXConfigurationSection, Scryber.Configuration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=872cbeb81db952fe"/&gt;
    /// In the sections element of the config file</remarks>
    public static class ScryberConfiguration
    {
        private const string ScryberConfigSectionKey = "Scryber";
        internal const bool DefaultUseSystemFonts = false;
        internal const TraceRecordLevel DefaultTraceLevel = TraceRecordLevel.Messages;

        /// <summary>
        /// Gets the configuration section
        /// </summary>
        private static ScryberConfigurationSection ConfigSection
        {
            get
            {
                return ConfigurationManager.GetSection(ScryberConfigSectionKey) as ScryberConfigurationSection;
            }
        }

        #region public static TraceRecordLevel GetTraceLevel()

        /// <summary>
        /// Gets the configured trace level
        /// </summary>
        /// <returns></returns>
        public static TraceRecordLevel GetTraceLevel()
        {
            ScryberConfigurationSection section = ConfigSection;
            if (null == section || section.Tracing == null)
                return ScryberConfigurationSection.DefaultTraceRecordingLevel;
            else
                return section.Tracing.TraceLevel;
        }

        #endregion

        public static PDFTraceLog GetLog()
        {
            ScryberConfigurationSection section = ConfigSection;
            if (null == section || null == section.Tracing)
                return new DoNothingTraceLog();
            else
                return section.Tracing.GetLog();
        }

        #region public static FontMappingCollection GetExplictFontMappings()

        /// <summary>
        /// Gets the collection of fonts explicitly declared in the configuration file
        /// </summary>
        /// <returns></returns>
        public static FontMappingCollection GetExplictFontMappings()
        {
            ScryberConfigurationSection config = ConfigSection;
            if (null == config)
                return null; 
            FontMappingSection section = ConfigSection.FontMappings;
            if (null == section)
                return null;
            else
                return section.FontNames;
        }

        #endregion

        #region internal static string GetFontDefaultDirectory()

        /// <summary>
        /// Returns the default directory to scan for fonts that can be used in PDF files
        /// </summary>
        /// <returns></returns>
        public static string GetFontDefaultDirectory()
        {
            ScryberConfigurationSection config = ConfigSection;
            if (null == config)
                return string.Empty; 

            FontMappingSection section = ConfigSection.FontMappings;
            if (null == section)
                return string.Empty;
            else
                return section.DefaultDirectory;
        }

        #endregion

        #region public static bool UseSubstituteFonts()

        internal const bool DefaultUseFontSubstitution = false;

        /// <summary>
        /// Returns true if a substitute font should be used when the actual
        /// font cannot be found - same family no style, or if still not found - Courier
        /// </summary>
        /// <returns></returns>
        public static bool UseSubstituteFonts()
        {
            ScryberConfigurationSection config = ConfigSection;
            if (null == config)
                return DefaultUseFontSubstitution; 
            FontMappingSection section = ConfigSection.FontMappings;
            if (null == section)
                return DefaultUseFontSubstitution;
            else
                return section.UseFontSubstitution;
        }

        #endregion

        #region internal static bool UseSystemFonts()

        

        /// <summary>
        /// Returns true if the local system fonts can be used in PDF's. False if only explicit fonts can be used
        /// </summary>
        /// <returns></returns>
        public static bool UseSystemFonts()
        {
            ScryberConfigurationSection config = ConfigSection;
            if (null == config)
                return DefaultUseSystemFonts; 
            FontMappingSection section = config.FontMappings;
            if (null == section)
                return DefaultUseSystemFonts;
            else
                return section.UseSystemFonts;
        }

        #endregion
    }
}
