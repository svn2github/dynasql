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
using System.Drawing;
using Scryber.Resources;

namespace Scryber
{
    public static class PDFFonts
    {
        //Standard font definitions

        #region Helvetica, HelveticaBold, HelveticaOblique, HelveticaBoldOblique

        private static PDFFontDefinition _helvetica = null;

        public static PDFFontDefinition Helvetica
        {
            get
            {
                if (_helvetica == null)
                {
                    _helvetica = PDFFontDefinition.InitStdType1Mac("Fhel", "Helvetica", "Helvetica", false, false);
                }
                return _helvetica;
            }

        }


        private static PDFFontDefinition _helveticabold = null;

        public static PDFFontDefinition HelveticaBold
        {
            get
            {
                if (_helveticabold == null)
                    _helveticabold = PDFFontDefinition.InitStdType1Mac("FhelBl", "Helvetica-Bold", "Helvetica", true, false);
                return _helveticabold;
            }
        }


        private static PDFFontDefinition _helveticaobl = null;

        public static PDFFontDefinition HelveticaOblique
        {
            get
            {
                if (_helveticaobl == null)
                    _helveticaobl = PDFFontDefinition.InitStdType1Mac("FhelOb", "Helvetica-Oblique", "Helvetica", false, true);
                return _helveticaobl;
            }
        }


        private static PDFFontDefinition _helveticabolobl = null;

        public static PDFFontDefinition HelveticaBoldOblique
        {
            get
            {
                if (_helveticabolobl == null)
                    _helveticabolobl = PDFFontDefinition.InitStdType1Mac("FhelObBl", "Helvetica-BoldOblique", "Helvetica", true, true);
                return _helveticabolobl;
            }
        }


        #endregion

        #region TimesRoman, TimesBold, TimesItalic, TimesBoldItalic

        private static PDFFontDefinition _times = null;

        public static PDFFontDefinition TimesRoman
        {
            get
            {
                if (_times == null)
                    _times = PDFFontDefinition.InitStdType1Mac("Ftimes", "Times-Roman", "Times", false, false);
                return _times;
            }
        }


        private static PDFFontDefinition _timesbold = null;

        public static PDFFontDefinition TimesBold
        {
            get
            {
                if (_timesbold == null)
                    _timesbold = PDFFontDefinition.InitStdType1Mac("FtimesBo", "Times-Bold", "Times", true, false);
                return _timesbold;
            }
        }


        private static PDFFontDefinition _timesboldital = null;

        public static PDFFontDefinition TimesBoldItalic
        {
            get
            {
                if (_timesboldital == null)
                    _timesboldital = PDFFontDefinition.InitStdType1Mac("FtimesBoIt", "Times-BoldItalic", "Times", true, true);
                return _timesboldital;
            }
        }


        private static PDFFontDefinition _timesital = null;

        public static PDFFontDefinition TimesItalic
        {
            get
            {
                if (_timesital == null)
                    _timesital = PDFFontDefinition.InitStdType1Mac("FtimesIt", "Times-Italic", "Times", false, true);
                return _timesital;
            }
        }

        #endregion

        #region Courier, CourierBold, CourierOblique, CourierBoldOblique

        private static PDFFontDefinition _cour = null;

        public static PDFFontDefinition Courier
        {
            get
            {
                if (_cour == null)
                    _cour = PDFFontDefinition.InitStdType1Mac("Fcour", "Courier", "Courier", "Courier New", false, false);
                return _cour;
            }
        }


        private static PDFFontDefinition _courbold = null;

        public static PDFFontDefinition CourierBold
        {
            get
            {
                if (_courbold == null)
                    _courbold = PDFFontDefinition.InitStdType1Mac("FcourBo", "Courier-Bold", "Courier", "Courier New", true, false);
                return _courbold;
            }
        }

        private static PDFFontDefinition _courital = null;

        public static PDFFontDefinition CourierOblique
        {
            get
            {
                if (_courital == null)
                    _courital = PDFFontDefinition.InitStdType1Mac("FcourOb", "Courier-Oblique", "Courier", "Courier New", false, true);
                return _courital;
            }
        }

        private static PDFFontDefinition _courboldital = null;

        public static PDFFontDefinition CourierBoldOblique
        {
            get
            {
                if (_courboldital == null)
                    _courboldital = PDFFontDefinition.InitStdType1Mac("FcourBoOb", "Courier-BoldOblique", "Courier", "Courier New", true, true);
                return _courboldital;
            }
        }

        #endregion

        #region ZapfDingbats

        private static PDFFontDefinition _zaph = null;

        public static PDFFontDefinition ZapfDingbats
        {
            get
            {
                if (_zaph == null)
                {
                    _zaph = PDFFontDefinition.InitStdSymbolType1Mac("Fzapf", "ZapfDingbats");
                    _zaph.Family = "Zapf Dingbats";
                    _zaph.WindowsName = "WingDings";
                }
                return _zaph;
            }
        }

        #endregion

        #region Symbol

        private static PDFFontDefinition _sym = null;

        public static PDFFontDefinition Symbol
        {
            get
            {
                if (_sym == null)
                    _sym = PDFFontDefinition.InitStdSymbolType1Mac("Fsym", "Symbol");
                return _sym;
            }
        }

        #endregion

        //Standard font list intialized in the class constructor

        #region public static List<PDFFontDefinition> StandardFonts

        private static List<PDFFontDefinition> _stds;

        /// <summary>
        /// Gets the collection of standard font defintions.
        /// </summary>
        public static List<PDFFontDefinition> StandardFonts
        {
            get { return _stds; }
        }

        #endregion

        /// <summary>
        /// Initializes the standard fonts.
        /// </summary>
        static PDFFonts()
        {
            _stds = new List<PDFFontDefinition>();
            _stds.Add(Helvetica);
            _stds.Add(HelveticaBold);
            _stds.Add(HelveticaBoldOblique);
            _stds.Add(HelveticaOblique);
            _stds.Add(TimesRoman);
            _stds.Add(TimesBold);
            _stds.Add(TimesBoldItalic);
            _stds.Add(TimesItalic);
            _stds.Add(Courier);
            _stds.Add(CourierBold);
            _stds.Add(CourierBoldOblique);
            _stds.Add(CourierOblique);
            _stds.Add(ZapfDingbats);
            _stds.Add(Symbol);
        }

        /// <summary>
        /// Gets a font definition for a specific font
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static PDFFontDefinition GetStandardFontDefinition(Scryber.Drawing.PDFFont font)
        {
            foreach (PDFFontDefinition defn in _stds)
            {
                if (defn.Equals(font))
                    return defn;
            }
            return null;
        }

        public static PDFFontDefinition GetStandardFontDefinition(string name, FontStyle style)
        {
            bool italic = (style & FontStyle.Italic) > 0;
            bool bold = (style & FontStyle.Bold) > 0;

            foreach (PDFFontDefinition defn in _stds)
            {
                if (defn.Family.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    if (defn.Bold == bold && defn.Italic == italic)
                        return defn;
                }
            } 
            return null;
        }
    }
}
