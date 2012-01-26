/*  Copyright 2012 Scryber Limited
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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using Scryber.Resources;
using Scryber.Configuration;

namespace Scryber.Drawing
{
    //TODO: change the enumeration names to PDF...

     
    public static class PDFFontFactory
    {
        //
        // Inner classes
        //

        #region private abstract class FontReference
        /// <summary>
        /// A unique font reference
        /// </summary>
        private abstract class FontReference
        {
            internal string FamilyName { get; private set; }
            internal System.Drawing.FontStyle Style { get; private set; }
            internal string FilePath { get; private set; }
            internal byte[] FileData { get; private set; }

            internal PDFFontDefinition Definition { get; set; }

            internal bool LoadedDefintion { get { return null != Definition; } }

            internal FontFamily SystemFamily { get; set; }

            internal object LoadLock { get; private set; }
            
            internal FontReference(FontFamily family, string name, System.Drawing.FontStyle style, string path)
            {
                this.SystemFamily = family;
                this.FamilyName = name;
                this.Style = style;
                this.FilePath = path;
                this.LoadLock = new object();
            }

            internal FontReference(FontFamily family, string name, System.Drawing.FontStyle style, byte[] data)
            {
                this.SystemFamily = family;
                this.FamilyName = name;
                this.Style = style;
                this.FilePath = null;
                this.LoadLock = new object();
                this.FileData = data;
            }
        }

        #endregion

        #region private class LinkedFontReference : FontReference

        /// <summary>
        /// A unique font reference that can be chained
        /// </summary>
        private class LinkedFontReference : FontReference
        {
            internal LinkedFontReference Next { get; set; }

            internal LinkedFontReference(FontFamily family, string familyname, System.Drawing.FontStyle style, string path)
                : base(family, familyname, style, path)
            {
            }

            internal LinkedFontReference(FontFamily family, string familyname, System.Drawing.FontStyle style, byte[] data)
                : base(family, familyname, style, data)
            {
            }

            internal void Append(LinkedFontReference reference)
            {
                if (null == this.Next)
                    this.Next = reference;
                else
                    this.Next.Append(reference);
            }

            internal LinkedFontReference GetStyle(System.Drawing.FontStyle style)
            {
                if (style == this.Style)
                    return this;
                else if (null == this.Next)
                    return null;
                else
                    return this.Next.GetStyle(style);
            }
        }

        #endregion

        #region private class FamilyReference

        /// <summary>
        /// A family of Font References
        /// </summary>
        private class FamilyReference
        {
            private FontFamily _sysfam;

            internal string FamilyName { get; private set; }
            internal FontFamily SystemFamily {
                get { return _sysfam; }
                set
                {
                    _sysfam = value;
                    LinkedFontReference link = _first;
                    while (null != link)
                    {
                        link.SystemFamily = value;
                        link = link.Next;
                    }
                }
            }

            private LinkedFontReference _first;

            internal FontReference First
            {
                get { return _first; }
            }

            internal FontReference this[System.Drawing.FontStyle style]
            {
                get
                {
                    if (null == _first)
                        return null;
                    else
                        return _first.GetStyle(style);
                }
            }

            internal FamilyReference(string name)
            {
                this.FamilyName = name;
            }

            internal void Add(System.Drawing.FontStyle style, string filepath)
            {

                LinkedFontReference fontref = new LinkedFontReference(this.SystemFamily, this.FamilyName, style, filepath);
                if (null == _first)
                    _first = fontref;
                else
                    _first.Append(fontref);
            }

            internal void Add(System.Drawing.FontStyle style, byte[] data)
            {
                LinkedFontReference fontref = new LinkedFontReference(this.SystemFamily, this.FamilyName, style, data);
                if (null == _first)
                    _first = fontref;
                else
                    _first.Append(fontref);
            }
        }

        #endregion

        #region private class FamilyReferenceBag

        /// <summary>
        /// A collection of Font Family references that can be acessed by family name and font style. 
        /// Does not throw an exception if the font does not exist.
        /// </summary>
        private class FamilyReferenceBag
        {
            private Dictionary<string, FamilyReference> _families;
            private FontCollection _collection;

            internal FontCollection FontCollection
            {
                get { return _collection; }
            }

            internal FamilyReferenceBag(FontCollection collection)
            {
                _collection = collection;
                _families = new Dictionary<string, FamilyReference>(StringComparer.OrdinalIgnoreCase);
            }

            internal FamilyReference this[string family]
            {
                get
                {
                    FamilyReference fm;
                    if (_families.TryGetValue(family, out fm))
                        return fm;
                    else
                        return null;
                }
            }

            internal FontReference this[string family, System.Drawing.FontStyle style]
            {
                get
                {
                    FontReference fnt;
                    FamilyReference fam;
                    if (_families.TryGetValue(family, out fam))
                    {
                        fnt = fam[style];
                    }
                    else
                        fnt = null;

                    return fnt;
                }
            }

            

            internal virtual void AddFontFile(string family, System.Drawing.FontStyle style, string path)
            {
                FamilyReference fam;
                if (_families.TryGetValue(family, out fam) == false)
                {
                    fam = new FamilyReference(family);
                    _families.Add(family, fam);
                }
                fam.Add(style, path);
            }

            internal void AddFontResource(string family, System.Drawing.FontStyle style, byte[] data)
            {
                FamilyReference fam;
                if (_families.TryGetValue(family, out fam) == false)
                {
                    fam = new FamilyReference(family);
                    _families.Add(family, fam);
                }
                fam.Add(style, data);
            }
        }

        #endregion


        //
        //Static variables
        //

        
        private static FamilyReferenceBag _systemfamilies;
        private static FamilyReferenceBag _customfamilies;
        private static bool _init;
        private static Exception _initex;
        private static object _initlock;

        //
        // ..ctor
        //

        #region static PDFFontFactory()

        static PDFFontFactory()
        {
            _systemfamilies = null;
            _customfamilies = null;
            _init = false;
            _initlock = new object();
            _initex = null;
        }

        #endregion

        //
        // public methods
        //

        #region  internal static string GetSystemFontFamilyNameForStandardFont(string pdffamilyname)

        /// <summary>
        /// Gets the system name of the font for the PDF family name
        /// </summary>
        /// <param name="pdffamilyname">The name of the PDF font family</param>
        /// <returns></returns>
        public static string GetSystemFontFamilyNameForStandardFont(string pdffamilyname)
        {
            if (string.IsNullOrEmpty(pdffamilyname))
                throw new ArgumentNullException("pdfFamilyName");

            string name = pdffamilyname.Replace(" ", "");
            string newName;
            switch (name.ToLower())
            {
                case "courier":
                    newName = "Courier New";
                    break;
                case "times":
                    newName = "Times New Roman";
                    break;
                case "arial":
                case "helvetica":
                    newName = "Arial";
                    break;
                case "symbol":
                    newName = "Symbol Regular";
                    break;
                case "zapfdingbats":
                    newName = "Zapf Dingbats";
                    break;
                default:
                    throw new ArgumentException(String.Format(Errors.FontNotFound, name),"font");

            }
            return newName;
        }

        #endregion

        #region internal static Font GetSystemFont(string family, System.Drawing.FontStyle style, float size)

        /// <summary>
        /// Gets a System.Drawing.Font based upon the family, style and size.
        /// </summary>
        /// <param name="family">The name of the font family</param>
        /// <param name="style">The style of the font</param>
        /// <param name="size">The size of the font</param>
        /// <returns>A new System.Drawing.Font</returns>
        public static Font GetSystemFont(string family, System.Drawing.FontStyle style, float size)
        {
            //Make sure we are initialized and OK
            AssertInitialized();

            bool usesystem = ScryberConfiguration.UseSystemFonts();
            bool usesubstitute = ScryberConfiguration.UseSubstituteFonts();
            
            if (string.IsNullOrEmpty(family))
                throw new ArgumentNullException("family");

            FontReference fref = _customfamilies[family, style];
            
            if (null == fref && usesystem)
                fref = _systemfamilies[family, style];
            if (null == fref)
            {
                if (usesubstitute)
                {
                    FamilyReference fam = _customfamilies[family];
                    if (null == fam && usesystem)
                        fam = _systemfamilies[family];

                    if (null != fam)
                        fref = fam.First;

                    //Fallback - use courier font definition
                    if (null == fref)
                    {
                        return new Font(GetSystemFontFamilyNameForStandardFont("Courier"), size, style, GraphicsUnit.Point);
                    }
                }
                else
                    throw new NullReferenceException(String.Format(Errors.FontNotFound, family + " " + style.ToString()));
            }
            Font f;
            if (null == fref.SystemFamily)
                f = new Font(fref.FamilyName, size, fref.Style, GraphicsUnit.Point);
            else
                f = new Font(fref.SystemFamily, size, fref.Style, GraphicsUnit.Point);
            return f;
        }

        #endregion

        #region internal static PDFFontDefinition GetFontDefinition(PDFFont font)

        public static PDFFontDefinition GetFontDefinition(PDFFont font)
        {
            //Make sure we are initialized and OK
            AssertInitialized();

            if (font.IsStandard)
            {
                return PDFFontDefinition.LoadStandardFont(font);
            }
            else
            {
                System.Drawing.FontStyle fs = font.GetDrawingStyle();
                string family = font.FamilyName;
                return GetFontDefinition(family, fs);
            }
        }

        #endregion

        public static PDFFontDefinition GetFontDefinition(string fullname)
        {
            int pos = fullname.IndexOf(",");
            System.Drawing.FontStyle fontstyle = System.Drawing.FontStyle.Regular;
            string family;
            if (pos > 0)
            {
                family = fullname.Substring(0, pos).Trim();
                string style = fullname.Substring(pos);
                if (style.IndexOf("bold", StringComparison.OrdinalIgnoreCase) > -1)
                    fontstyle = fontstyle | System.Drawing.FontStyle.Bold;

                if (style.IndexOf("italic", StringComparison.OrdinalIgnoreCase) > -1)
                    fontstyle = fontstyle | System.Drawing.FontStyle.Italic;
            }
            else
                family = fullname.Trim();

            return GetFontDefinition(family, fontstyle);
            
        }

        #region internal static PDFFontDefinition GetFontDefinition(string family, System.Drawing.FontStyle style)

        /// <summary>
        /// Gets the PDFFontDefinition for the specified famil and style
        /// </summary>
        /// <param name="family"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static PDFFontDefinition GetFontDefinition(string family, System.Drawing.FontStyle style)
        {
            //Make sure we are initialized and OK
            AssertInitialized();

            bool usesystem = ScryberConfiguration.UseSystemFonts();
            bool usesubstitute = ScryberConfiguration.UseSubstituteFonts();
            
            if (string.IsNullOrEmpty(family))
                throw new ArgumentNullException("family");

            if (PDFFont.IsStandardFontFamily(family))
                return PDFFontDefinition.LoadStandardFont(family, style);
            
            
            FontReference fref = _customfamilies[family, style];

            if (null == fref && usesystem)
                fref = _systemfamilies[family, style];

            if (null == fref)
            {
                //We dont have the explicit font so if we should substitue then 
                //try to find the family and return that otherwise use courier.
                if (usesubstitute)
                {
                    FamilyReference fam = _customfamilies[family];
                    if (null == fam && usesystem)
                        fam = _systemfamilies[family];

                    if (null != fam)
                        fref = fam.First;

                    //Fallback - use courier font definition
                    if (null == fref)
                    {
                        return PDFFonts.Courier;
                    }
                }
                else
                    throw new NullReferenceException(String.Format(Errors.FontNotFound, family + " " + style.ToString()));
                
            }

            lock (fref.LoadLock)
            {
                if(fref.LoadedDefintion == false)
                {
                    PDFFontDefinition defn;
                    if (null == fref.FileData)
                        //Load from a file path if we don't have the binary data
                        defn = PDFFontDefinition.LoadFontFile(fref.FilePath, family, style);
                    else
                        //We do have the binary data so load from this
                        defn = PDFFontDefinition.LoadFontFile(fref.FileData, family, style);

                    fref.Definition = defn;
                }
            }
            return fref.Definition;
        }

        #endregion

        //
        // private methods
        //

        #region private static void AssertInitialized()

        /// <summary>
        /// Thread safe intiailization. Must be called before the custom and system families are used
        /// </summary>
        private static void AssertInitialized()
        {
            if (_init == false)
            {
                lock (_initlock)
                {
                    try
                    {
                        //Set init here. We only want to do it once, even if if fails
                        _init = true;
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        _systemfamilies = LoadSystemFonts();
                        _customfamilies = LoadCustomFamilies();

                        sw.Stop();
                        System.Diagnostics.Debug.WriteLine("Loaded all system and custom fonts in :" + sw.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        _initex = ex;
                        throw new System.Configuration.ConfigurationException(String.Format(Errors.CouldNotInitializeTheFonts, ex.Message), ex);
                    }
                }
            }
            else if(null != _initex)
                throw  new System.Configuration.ConfigurationException(String.Format(Errors.CouldNotInitializeTheFonts, _initex.Message),_initex);
        }

        #endregion

        #region private static FamilyReferenceBag LoadSystemFonts()

        /// <summary>
        /// Loads all the system TrueType fonts from the system fonts folder
        /// </summary>
        /// <returns></returns>
        private static FamilyReferenceBag LoadSystemFonts()
        {
            InstalledFontCollection install = new InstalledFontCollection();
            FamilyReferenceBag bag = new FamilyReferenceBag(install);
            //Check to see if we are allowed to use the system fonts
            if (ScryberConfiguration.UseSystemFonts())
            {
                try
                {
                    string path = System.Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                    if (!string.IsNullOrEmpty(path))
                    {
                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                        if (dir.Exists)
                        {
                            Scryber.OpenType.TTFRef[] all = Scryber.OpenType.TTFRef.LoadRefs(dir);
                            foreach (Scryber.OpenType.TTFRef item in all)
                            {
                                bag.AddFontFile(item.FamilyName, GetFontStyle(item.FontSelection), item.FullPath);
                            }

                        }
                    }
                    FontFamily[] installedfamilies = install.Families;
                    foreach (FontFamily fam in installedfamilies)
                    {
                        FamilyReference famref = bag[fam.Name];
                        if (null != famref)
                            famref.SystemFamily = fam;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Could not load the system fonts", ex);
                }
            }
            return bag;
        }

        
        #endregion

        #region private static FamilyReferenceBag LoadCustomFamilies()

        /// <summary>
        /// Loads all the custom fonts definied in the configuration file
        /// </summary>
        /// <returns></returns>
        private static FamilyReferenceBag LoadCustomFamilies()
        {
            PrivateFontCollection priv = new PrivateFontCollection();
            FamilyReferenceBag bag = new FamilyReferenceBag(priv);

            //Load the explicit entries first
            FontMappingCollection known = ScryberConfiguration.GetExplictFontMappings();
            
            if ((null != known) && (known.Count > 0))
            {
                //For each of the configuration entries either load from the resources, or load from the file
                Dictionary<string, System.Resources.ResourceManager> mgrs = new Dictionary<string, System.Resources.ResourceManager>(StringComparer.OrdinalIgnoreCase);

                foreach (Scryber.Configuration.FontMapping map in known)
                {
                    string family = map.FamilyName;
                    System.Drawing.FontStyle style = map.FontStyle;

                    if (!string.IsNullOrEmpty(map.ResourceName))
                    {
                        //Load the font from a resource
                        string rsrcname = map.ResourceName;
                        string rsrcbase = map.ResourceBaseName;
                        System.Resources.ResourceManager mgr;
                        byte[] data;
                        try
                        {
                            if (!mgrs.TryGetValue(rsrcbase, out mgr))
                            {
                                mgr = LoadResourceManager(rsrcbase);
                                mgrs.Add(rsrcbase, mgr);
                            }


                            data = (byte[])mgr.GetObject(rsrcname);

                            //from the byte[] of data lock a pointer to it and add the memory font. 
                            unsafe
                            {
                                fixed (byte* ptr = data)
                                {
                                    priv.AddMemoryFont((IntPtr)ptr, data.Length);
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            throw new ConfigurationException(String.Format(Errors.CouldNotLoadTheFontResource, rsrcname, rsrcbase),ex);
                        }
                        bag.AddFontResource(family, style, data);
                    }
                    else if (!string.IsNullOrEmpty(map.FileName))
                    {
                        //Load from a file
                        string path = map.FileName;

                        try
                        {
                            path = GetFullPath(path);
                            priv.AddFontFile(path);
                        }
                        catch (Exception ex)
                        {
                            throw new ConfigurationException(String.Format(Errors.CouldNotLoadTheFontFile, path, ex.Message), ex);
                        }

                        bag.AddFontFile(family, style, path);
                    }
                    else
                    {
                        throw new ConfigurationException(String.Format(Errors.FontMappingMustHaveFilePathOrResourceName, family));
                    }
                }

            }

            string defaultdir = Scryber.Configuration.ScryberConfiguration.GetFontDefaultDirectory();
            
            if (!string.IsNullOrEmpty(defaultdir))
            {
                //Make sure we are not just referencing the fonts directory.
                if (!string.Equals(defaultdir, System.Environment.GetFolderPath(Environment.SpecialFolder.Fonts), StringComparison.OrdinalIgnoreCase))
                {
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(defaultdir);

                    if (dir.Exists)
                    {
                        //Load all the fonts from the directory
                        Scryber.OpenType.TTFRef[] all = Scryber.OpenType.TTFRef.LoadRefs(dir);

                        foreach (Scryber.OpenType.TTFRef item in all)
                        {
                            try
                            {
                                //try and add the font file
                                priv.AddFontFile(item.FullPath);
                            }
                            catch (Exception ex)
                            {
                                throw new ConfigurationException(String.Format(Errors.CouldNotLoadTheFontFile, item.FullPath, ex.Message), ex);
                            }

                            //font file added  - now register the family and style against the path
                            bag.AddFontFile(item.FamilyName, GetFontStyle(item.FontSelection), item.FullPath);
                        }

                    }
                }
            }

            FontFamily[] privatefamilies = priv.Families;
            foreach (FontFamily fam in privatefamilies)
            {
                FamilyReference famref = bag[fam.Name];
                if (null != famref)
                    famref.SystemFamily = fam;
            }


            return bag;
        }

        #endregion

        #region private static System.Resources.ResourceManager LoadResourceManager(string basename)

        /// <summary>
        /// Loads a resorce manager for a specific base name on the format 'basename [, assemblyname]'
        /// </summary>
        /// <param name="basename"></param>
        /// <returns></returns>
        /// <remarks>If the assembly name is not provided then the current executing assembly is used</remarks>
        private static System.Resources.ResourceManager LoadResourceManager(string basename)
        {
            //TODO: Check if we are wa web project and load from the web project assembly rather than the executing assembly.
            System.Resources.ResourceManager mgr;
            System.Reflection.Assembly assm;

            try
            {
                if (!string.IsNullOrEmpty(basename))
                {
                    int offset = basename.IndexOf(',');
                    if (offset > 0)
                    {
                        string assmname = basename.Substring(offset + 1).Trim();
                        basename = basename.Substring(0, offset).Trim();

                        assm = System.Reflection.Assembly.Load(assmname);
                    }
                    else
                        assm = System.Reflection.Assembly.GetExecutingAssembly();
                }
                else
                    assm = System.Reflection.Assembly.GetExecutingAssembly();

                mgr = new System.Resources.ResourceManager(basename, assm);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(String.Format(Errors.CouldNotLoadTheResourceManagerForBase, basename), ex);
            }
            return mgr;
        }

        #endregion

        #region private static string GetFullPath(string path)

        /// <summary>
        /// Gets the full path to a file if it is not rooted
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetFullPath(string path)
        {
            if (!System.IO.Path.IsPathRooted(path))
            {
                if (System.Web.HttpContext.Current != null)
                    path = System.Web.HttpContext.Current.Server.MapPath(path);
                else
                    path = System.IO.Path.Combine(System.Environment.CurrentDirectory, path);
            }
            return path;
        }

        #endregion

        #region private static System.Drawing.FontStyle GetFontStyle(Scryber.OpenType.SubTables.FontSelection fs)

        /// <summary>
        /// Converts a OpenType.FontSelection to a Drawing.FontStyle
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private static System.Drawing.FontStyle GetFontStyle(Scryber.OpenType.SubTables.FontSelection fs)
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;

            if ((fs & Scryber.OpenType.SubTables.FontSelection.Italic) > 0)
                style |= System.Drawing.FontStyle.Italic;
            if ((fs & Scryber.OpenType.SubTables.FontSelection.Underscore) > 0)
                style |= System.Drawing.FontStyle.Underline;
            if ((fs & Scryber.OpenType.SubTables.FontSelection.Strikeout) > 0)
                style |= System.Drawing.FontStyle.Strikeout;
            if ((fs & Scryber.OpenType.SubTables.FontSelection.Bold) > 0)
                style |= System.Drawing.FontStyle.Bold;

            return style;
        }

        #endregion

    }
}
