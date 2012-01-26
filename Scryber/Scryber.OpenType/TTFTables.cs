using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public static class TTFTables
    {
        public const string HorizontalHeader = "hhea";
        public const string CharacterMapping = "cmap";
        public const string FontHeader = "head";
        public const string HorizontalMetrics = "hmtx";
        public const string MaximumProfile = "maxp";
        public const string NamingTable = "name";
        public const string WindowsMetrics = "OS/2";
        public const string PostscriptInformation = "post";
        
        //TrueType outlines
        public const string ControlValue = "cvt ";
        public const string FontProgram = "fpgm";
        public const string GlyphData = "glyf";
        public const string LocationIndex = "loca";
        public const string CVTProgram = "prep";
        
        //Postscript outlines
        public const string PostscriptProgram = "CFF ";
        public const string VerticalOrigin = "VORG";
        
        //Bitmap Glyphs
        public const string EmbeddedBitmapData = "EBDT";
        public const string EmbeddedBitmapLocationData = "EBLC";
        public const string EmbeddedBitmapScanningData = "EBSC";

        //Advanced Typogrpahic Tables
        public const string BaseLineData = "BASE";
        public const string GlyphDefinitionData = "GDEF";
        public const string GlyphPositionData = "GPOS";
        public const string GlyphSubstitutionData = "GSUB";
        public const string JustificationData = "JSTF";
        
        //Other open type tables
        public const string DigitalSignature = "DISG";
        public const string GridFittingAndScanConversion = "gasp";
        public const string HorizontalDeviceMetrics = "hdmx";
        public const string Kerning = "kern";
        public const string LinearThresholdData = "LTSH";
        public const string PCL5Data = "PCLT";
        public const string VerticalDeviceMetrics = "VMDX";
        public const string VerticalMetricsHeader = "vhea";
        public const string VerticalMetrics = "vmtx";

    }
}
