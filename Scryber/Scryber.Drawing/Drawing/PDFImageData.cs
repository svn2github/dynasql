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
using Scryber.Native;

namespace Scryber.Drawing
{
    public abstract class PDFImageData : PDFObject
    {

        //
        // properties
        //

        #region public string SourcePath {get;set;}

        private string _path;
        /// <summary>
        /// Gets the source path this image data was loaded from
        /// </summary>
        public string SourcePath
        {
            get { return _path; }
            protected set { _path = value; }
        }

        #endregion


        #region public int PixelWidth {get;set;}

        private int _w;
        /// <summary>
        /// Gets the total number of pixels in one row of the image
        /// </summary>
        public int PixelWidth
        {
            get { return _w; }
            protected set { _w = value; }
        }

        #endregion

        #region public int PixelHeight {get;set;}

        private int _h;

        public int PixelHeight
        {
            get { return _h; }
            protected set { _h = value; }
        }

        #endregion


        #region public byte[] Data {get;set;}

        private byte[] _data;

        /// <summary>
        /// Gets the byte array of image data that represent the image
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            internal protected set { _data = value; }
        }

        #endregion


        #region  public int BitsPerColor {get;set;}

        private int _bitdepth;

        /// <summary>
        /// Gets or set the number of bits per single color sample
        /// </summary>
        public int BitsPerColor
        {
            get { return _bitdepth; }
            internal protected set { _bitdepth = value; }
        }

        #endregion

        #region public int BytesPerLine {get; protected internal set;}

        private int _bperline;

        /// <summary>
        /// Gets the total number of bytes in one line
        /// </summary>
        public int BytesPerLine
        {
            get { return _bperline; }
            protected internal set { _bperline = value; }
        }

        #endregion

        #region public ColorSpace ColorSpace {get;set;}

        private ColorSpace _cs = ColorSpace.RGB;

        public ColorSpace ColorSpace
        {
            get { return _cs; }
            internal protected set { _cs = value; }
        }

        #endregion

        #region public int ColorsPerSample {get; set;}

        private int _colspsample;

        /// <summary>
        /// Gets the numbers of colour values that make up an individual pixel.
        /// </summary>
        public int ColorsPerSample
        {
            get { return _colspsample; }
            protected internal set { _colspsample = value; }
        }

        #endregion

        #region public double HorizontalResolution {get;set;}

        private double _hres;
        /// <summary>
        /// Gets the horizontal resolution of the image (pixels per inch)
        /// </summary>
        public double HorizontalResolution
        {
            get { return _hres; }
            internal protected set { _hres = value; }
        }

        #endregion

        #region public double VerticalResolution {get;set;}

        private double _vres;
        /// <summary>
        /// Gets the vertical resolution of the image (pixels per inch)
        /// </summary>
        public double VerticalResolution
        {
            get { return _vres; }
            internal protected set { _vres = value; }
        }

        #endregion

        #region public string Filter {get;set;}

        private string _filter;

        /// <summary>
        /// Gets the stream filter
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            internal protected set { _filter = value; }
        }

        #endregion

        //
        // ctor(s)
        //

        #region protected PDFImageData(string source, int w, int h)

        /// <summary>
        /// Protected constructor - accepts the source path, width and height (in pixels)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        protected PDFImageData(string source, int w, int h)
            : base(PDFObjectTypes.ImageData)
        {
            this._path = source;
            this._w = w;
            this._h = h;
        }

        #endregion

        //
        // rendering

        public PDFSize GetSize()
        {
            double w = ((double)this.PixelWidth) / this.HorizontalResolution;
            double h = ((double)this.PixelHeight) / this.VerticalResolution;

            return new PDFSize(new PDFUnit(w, PageUnits.Inches), new PDFUnit(h, PageUnits.Inches));
        }

        internal PDFObjectRef Render(PDFName name, PDFContextBase context, PDFWriter writer)
        {

            context.TraceLog.Add(TraceLevel.Debug, "IMAGE", "Rendering image data");

            PDFObjectRef renderref = writer.BeginObject(name.Value);

            writer.BeginDictionaryS();
            writer.WriteDictionaryNameEntry("Name", name.Value);
            writer.WriteDictionaryNameEntry("Type", "XObject");
            writer.WriteDictionaryNameEntry("Subtype", "Image");
            
            RenderImageInformation(context, writer);
           
            writer.EndDictionary();
            writer.BeginStream(renderref);

            this.RenderImageStreamData(context, writer);
           
            writer.EndStream();

            writer.EndObject();

            if (context.TraceLog.ShouldLog(TraceLevel.Debug))
                context.TraceLog.Add(TraceLevel.Debug, "IMAGE", String.Format("Completed render of the image XObject for source '{0}'", name));



            return renderref;
        }

        protected virtual void RenderImageInformation(PDFContextBase context,  PDFWriter writer)
        {
            writer.WriteDictionaryNumberEntry("Width", this.PixelWidth);
            writer.WriteDictionaryNumberEntry("Height", this.PixelHeight);
            writer.WriteDictionaryNumberEntry("Length", this.Data.LongLength);

            if (this.ColorSpace == ColorSpace.G)
                writer.WriteDictionaryNameEntry("ColorSpace", "DeviceGray");
            else if (this.ColorSpace == ColorSpace.Custom)
                this.RenderCustomColorSpace(writer);
            else
                writer.WriteDictionaryNameEntry("ColorSpace", "DeviceRGB");

            if(!string.IsNullOrEmpty(this.Filter))
                writer.WriteDictionaryNameEntry("Filter", this.Filter);

            writer.WriteDictionaryNumberEntry("BitsPerComponent", this.BitsPerColor);

            
        }

        protected virtual void RenderCustomColorSpace(PDFWriter writer)
        {
            throw new NotSupportedException("Implementors must use their own custom color space rendering");
        }

        protected virtual void RenderImageStreamData(PDFContextBase context, PDFWriter writer)
        {
            if (this.Data.LongLength > (long)int.MaxValue)
                throw new ArgumentOutOfRangeException("This image is too large to be included in a PDF file");

            writer.WriteRaw(this.Data, 0, this.Data.Length);
        }

        //
        // static methods
        //

        public static PDFImageData LoadImageFromURI(string uri)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            using (wc.OpenRead(uri))
            {
                PDFImageData img;
                byte[] data = wc.DownloadData(uri);
                img = InitImageData(uri, data);
                return img;
            }
            
        }

        public static PDFImageData LoadImageFromLocalFile(string path)
        {
                
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            if(fi.Exists == false)
                throw new ArgumentNullException("path", "The file at the path '" + path + "' does not exist.");
            
            using (System.Drawing.Image bmp = System.Drawing.Image.FromFile(path))
            {
                PDFImageData img;
                img = InitImageData(path, bmp);
                return img;
            }
        }

        /// <summary>
        /// Creates a new PDFImageData instance from the specified bitmap.
        /// The sourceKey is a unique reference to this image data in the document
        /// </summary>
        /// <param name="sourcekey"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static PDFImageData LoadImageFromBitmap(string sourcekey, System.Drawing.Bitmap bitmap)
        {
            if (null == bitmap)
                throw new ArgumentNullException("bitmap");
            PDFImageData data = InitImageData(sourcekey, bitmap);
            data.Data = Imaging.ImageFormatParser.GetRawBytesFromImage(bitmap);

            return data;
        }

        private static PDFImageData InitImageData(string uri, byte[] data)
        {
            PDFImageData img;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms, false);
                img = InitImageData(uri, bmp);
                bmp.Dispose();
            }
            return img;
        }

        private static PDFImageData InitImageData(string uri, System.Drawing.Image bmp)
        {
            PDFImageData imgdata;

            try
            {
                Imaging.ImageParser parser = Imaging.ImageFormatParser.GetParser(bmp);
                imgdata = parser(uri, bmp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format(Errors.CouldNotParseTheImageAtPath, uri, ex.Message), "bmp", ex);
            }

            return imgdata;
        }

        

        
        

        

        

       


        

    }
}
