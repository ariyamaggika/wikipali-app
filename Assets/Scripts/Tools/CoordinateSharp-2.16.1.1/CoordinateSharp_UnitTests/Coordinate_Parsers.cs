﻿using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using CoordinateSharp;
using System.Threading;
using System.Globalization;
namespace CoordinateSharp_UnitTests
{
    //[TestClass]
    public class Coordinate_Parsers
    {
        /// <summary>
        /// Ensures coordinate does not parse null or empty.
        /// </summary>
        //[TestMethod]
        public void Does_Not_Parse_Null_Or_Empty_Strings()
        {
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse(null));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse(""));
            Coordinate c;
            //Assert.IsFalse(Coordinate.TryParse(null, out c));
            //Assert.IsFalse(Coordinate.TryParse("", out c));
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Signed_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\Signed.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Signed_Parse_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\Signed.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Signed_DDM_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\SignedDDM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Signed_DDM_Parse_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\SignedDDM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }
        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Signed_DMS_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\SignedDMS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Signed_DMS_Parse_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\SignedDMS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Decimal_Degree_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\DD.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Decimal_Degree_Parse_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\DD.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Degree_Decimal_Minute_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\DDM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Degree_Decimal_Minute_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\DDM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Degree_Minute_Seconds_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\DMS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Degree_Minute_Seconds_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\DMS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void UTM_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\UTM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                UTM_Parse_Checks(cs);
                UTM_TryParse_Checks(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void UTM_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\UTM.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                UTM_Parse_Checks(cs);
                UTM_TryParse_Checks(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void MGRS_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\MGRS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                MGRS_Parse_Checks(cs);
                MGRS_TryParse_Checks(cs);
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void MGRS_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\MGRS.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                MGRS_Parse_Checks(cs);
                MGRS_TryParse_Checks(cs);
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void Cartesian_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\Cartesian.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                var vals = cs.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
                string coordString = vals[0];

                Cartesian cart;
                ECEF ecef;

                Cartesian.Parse(coordString);
                //Assert.IsTrue(Cartesian.TryParse(coordString, out cart));

                ECEF.Parse(coordString);
                //Assert.IsTrue(ECEF.TryParse(coordString, out ecef));
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void Cartesian_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\Cartesian.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                var vals = cs.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
                string coordString = vals[0];
               
                Cartesian cart;
                ECEF ecef;
                
                Cartesian.Parse(coordString);
                //Assert.IsTrue(Cartesian.TryParse(coordString, out cart));

                ECEF.Parse(coordString);
                //Assert.IsTrue(ECEF.TryParse(coordString, out ecef));
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void WebMercator_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\WebMercator.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                var vals = cs.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
                string coordString = vals[0];

                WebMercator webMercator;              
                //Assert.IsTrue(WebMercator.TryParse(coordString, out webMercator));              
            }
        }

        /// <summary>
        /// Ensures parses work in Dutch Culture formatting
        /// </summary>
        //[TestMethod]
        public void WebMercator_Dutch_Culture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl");
            var lines = File.ReadAllLines("CoordinateData\\WebMercator.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                var vals = cs.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
                string coordString = vals[0];

                WebMercator webMercator;
                //Assert.IsTrue(WebMercator.TryParse(coordString, out webMercator));
            }
        }

        /// <summary>
        /// Ensures parse
        /// </summary>
        //[TestMethod]
        public void GEOREF_Parse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var lines = File.ReadAllLines("CoordinateData\\GEOREF.txt");
            foreach (string cs in lines)
            {
                TryParse_Check(cs);
                Parse_Check(cs);

                var vals = cs.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
                string coordString = vals[0];

                GEOREF georef;
                //Assert.IsTrue(GEOREF.TryParse(coordString, out georef));
            }
        }

        /// <summary>
        /// Splits Coordinate test string to get value to parse and expected output.
        /// Checks proper parsing and value
        /// </summary>
        /// <param name="val">Test string value</param>
        private void Parse_Check(string val)
        {
            var vals = val.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
            string coordString = vals[0];
            string expected = vals[1];

            Coordinate c = Coordinate.Parse(coordString, new EagerLoad(false));//Will throw exception and fail if incorrect (intended behavior)

            //CHECK STRING COMPARISON, BUT REPLACE , and . with * to avoid cultural mismatch
            //Assert.AreEqual(expected.Replace(",", "*").Replace(".", "*"), c.ToString().Replace(",", "*").Replace(".", "*"), $"{vals} parsed as {c.ToString().Replace(",", "*").Replace(".", "*")} but {expected.Replace(",", "*").Replace(".", "*")} was expected.");

        }

        /// <summary>
        /// Splits Coordinate test string to get value to parse and expected output.
        /// Checks proper parsing and value
        /// </summary>
        /// <param name="val">Test string value</param>
        private void TryParse_Check(string val)
        {
            var vals = val.Split('#'); //First part of string is value to parse, seconds is expected string value after parse
            string coordString = vals[0];
            string expected = vals[1];
            Coordinate c;

            //Assert.IsTrue(Coordinate.TryParse(coordString, new EagerLoad(false), out c), $"{coordString} cannot be parsed.");

            //CHECK STRING COMPARISON, BUT REPLACE , and . with * to avoid cultural mismatch
            //Assert.AreEqual(expected.Replace(",", "*").Replace(".", "*"), c.ToString().Replace(",", "*").Replace(".", "*"), $"{vals} parsed as {c.ToString().Replace(",", "*").Replace(".", "*")} but {expected.Replace(",", "*").Replace(".", "*")} was expected.");
        }        

        private void MGRS_Parse_Checks(string val)
        {

            //Ensures parse success only. Values are compared in normal parser
            var vals = val.Split('#');
            string coordString = vals[0];
            MilitaryGridReferenceSystem.Parse(coordString);
            MilitaryGridReferenceSystem.Parse(coordString, Earth_Ellipsoid_Spec.WGS72_1972);
            MilitaryGridReferenceSystem.Parse(coordString, 6378137, 298.257222101);
        }

        private void MGRS_TryParse_Checks(string val)
        {
            //Ensures parse success only. Values are compared in normal parser
            var vals = val.Split('#'); 
            string coordString = vals[0];
            MilitaryGridReferenceSystem mgrs;
            //Assert.IsTrue(MilitaryGridReferenceSystem.TryParse(coordString, out mgrs), $"{coordString} MGRS cannot be parsed at TryParse 1.");
            //Assert.IsTrue(MilitaryGridReferenceSystem.TryParse(coordString, Earth_Ellipsoid_Spec.WGS72_1972, out mgrs), $"{coordString} MGRS cannot be parsed at TryParse 2.");
            //Assert.IsTrue(MilitaryGridReferenceSystem.TryParse(coordString, 6378137, 298.257222101, out mgrs), $"{coordString} MGRS cannot be parsed at TryParse 3.");
        }

        private void UTM_Parse_Checks(string val)
        {

            //Ensures parse success only. Values are compared in normal parser
            var vals = val.Split('#');
            string coordString = vals[0];
            UniversalTransverseMercator.Parse(coordString);
            UniversalTransverseMercator.Parse(coordString, Earth_Ellipsoid_Spec.WGS72_1972);
            UniversalTransverseMercator.Parse(coordString, 6378137, 298.257222101);
        }

        private void UTM_TryParse_Checks(string val)
        {
            //Ensures parse success only. Values are compared in normal parser
            var vals = val.Split('#');
            string coordString = vals[0];
            UniversalTransverseMercator utm;
            //Assert.IsTrue(UniversalTransverseMercator.TryParse(coordString, out utm), $"{coordString} UTM cannot be parsed at TryParse 1.");
            //Assert.IsTrue(UniversalTransverseMercator.TryParse(coordString, Earth_Ellipsoid_Spec.WGS72_1972, out utm), $"{coordString} UTM cannot be parsed at TryParse 2.");
            //Assert.IsTrue(UniversalTransverseMercator.TryParse(coordString, 6378137, 298.257222101, out utm), $"{coordString} UTM cannot be parsed at TryParse 3.");
        }

        /// <summary>
        /// Check that MGRS will parse if Easting and Northing values are not provided
        /// </summary>
        //[TestMethod]
        public void MGRS_Parses_With_No_Easting_Northing_Provided()
        {
            Coordinate c = null;

            //Assert.IsTrue(Coordinate.TryParse("38TNG", out c));
            //Assert.IsTrue(Coordinate.TryParse("38T NG", out c));
            //Assert.IsTrue(Coordinate.TryParse("38 T NG", out c));

            //Assert.IsFalse(Coordinate.TryParse("38TNG0", out c));
            //Assert.IsFalse(Coordinate.TryParse("38T NG 0", out c));
            //Assert.IsFalse(Coordinate.TryParse("38 T NG 0", out c));
        }

        /// <summary>
        /// Check that MGRS Polar will parse if Easting and Northing values are not provided
        /// </summary>
        //[TestMethod]
        public void MGRS_Polar_Parses_With_No_Easting_Northing_Provided()
        {
            Coordinate c = null;

            //Assert.IsTrue(Coordinate.TryParse("ZFG", out c));
            //Assert.IsTrue(Coordinate.TryParse("Z FG", out c));

            //Assert.IsFalse(Coordinate.TryParse("ZFG0", out c));
            //Assert.IsFalse(Coordinate.TryParse("Z FG 0", out c));
        }

        /// <summary>
        /// Ensures strings that are not MGRS do not parse in MGRS format
        /// </summary>
        //[TestMethod]
        public void MGRS_String_Fail()
        { 
            Coordinate c;
            //Assert.IsFalse(Coordinate.TryParse("smart", out c));
            //Assert.IsFalse(Coordinate.TryParse("warszawa", out c));
        }

        /// <summary>
        /// Check ECEF Parse Options Work
        /// </summary>
        //[TestMethod]
        public void ECEF_Options()
        {
            Coordinate coordinate;
           
            Coordinate.TryParse("5242.118   km 2444.44  km 2679.085   km", CartesianType.ECEF, out coordinate);
            //Assert.AreEqual(0, coordinate.Latitude.DecimalDegree - 25, .001, $"Latitude 1 exceed delta {coordinate.Latitude.DecimalDegree}.");
            //Assert.AreEqual(0, coordinate.Longitude.DecimalDegree - 25, .001, $"Longitude 1 exceed delta {coordinate.Longitude.DecimalDegree}.");
            //Assert.AreEqual(0, coordinate.ECEF.GeoDetic_Height.Meters - 25, 1, $"Height 1 exceed delta {coordinate.ECEF.GeoDetic_Height.Meters}.");

            Coordinate.TryParse("-96.867, -1107.196 , 6261.02   km", new DateTime(2019, 1, 1), CartesianType.ECEF, out coordinate);
            //Assert.AreEqual(0, coordinate.Latitude.DecimalDegree - 80, .001, $"Latitude 2 exceed delta {coordinate.Latitude.DecimalDegree}.");
            //Assert.AreEqual(0, coordinate.Longitude.DecimalDegree - -95, .001, $"Longitude 2 exceed delta {coordinate.Longitude.DecimalDegree}.");
            //Assert.AreEqual(0, coordinate.ECEF.GeoDetic_Height.Meters - 1500,1, $"Height 2 exceed delta {coordinate.ECEF.GeoDetic_Height.Meters}.");
        }

        /// <summary>
        /// Checks return parse types.
        /// </summary>
        //[TestMethod]
        public void Parse_Type()
        {
            Coordinate coordinate = new Coordinate(25, 25);
            //Assert.AreEqual(Parse_Format_Type.None, coordinate.Parse_Format, $"Parse_Format_Type.None expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("25,25", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Signed_Degree, coordinate.Parse_Format, $"Parse_Format_Type.Signed_Degree expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("N 25º E 25º", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Decimal_Degree, coordinate.Parse_Format, $"Parse_Format_Type.Decimal_Degree expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("N 25º 0' E 25º 0'", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Degree_Decimal_Minute, coordinate.Parse_Format, $"Parse_Format_Type.Degree_Decimal_Minute expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("N 25º 0' 0\" E 25º 0' 0\"", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Degree_Minute_Second, coordinate.Parse_Format, $"Parse_Format_Type.Degree_Minute_Seconde expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("35R 298154mE 2766437mN", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.UTM, coordinate.Parse_Format, $"Parse_Format_Type.UTM expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("35R KH 98154 66437", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.MGRS, coordinate.Parse_Format, $"Parse_Format_Type.MGRS expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("0.8213938 0.38302222 0.42261826", CartesianType.Cartesian, out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Cartesian_Spherical, coordinate.Parse_Format, $"Parse_Format_Type.Cartesian_Spherical expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("5242.097 km, 2444.43 km, 2679.074 km", CartesianType.ECEF, out coordinate);
            //Assert.AreEqual(Parse_Format_Type.Cartesian_ECEF, coordinate.Parse_Format, $"Parse_Format_Type.Cartesian_ECEF expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("8284118.2mE -6339892.6mN", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.WebMercator, coordinate.Parse_Format, $"Parse_Format_Type.WebMercator expected but {coordinate.Parse_Format} returned.");

            Coordinate.TryParse("SKHA45001256", out coordinate);
            //Assert.AreEqual(Parse_Format_Type.GEOREF, coordinate.Parse_Format, $"Parse_Format_Type.GEOREF expected but {coordinate.Parse_Format} returned.");
        }

        /// <summary>
        /// Checks return parse types.
        /// </summary>
        //[TestMethod]
        public void Parse_Simple_EagerLoad_Test()
        {
            Coordinate coordinate;
            //Parse Coordinate Formats

            EagerLoad el = new EagerLoad(false);
            //Assert.IsTrue(Coordinate.TryParse("12,12", el, out coordinate), "Parse Failed");
            //Assert.IsNull(coordinate.CelestialInfo, "Null CelestialInfo 2 expected");
            //Assert.IsNull(coordinate.ECEF, "Null ECEF2 expected");

            el = new EagerLoad(EagerLoadType.Celestial);
            //Assert.IsTrue(Coordinate.TryParse("12,12", el, out coordinate), "Parse Failed");
            //Assert.IsNotNull(coordinate.CelestialInfo, "CelestialInfo 2 expected");
            //Assert.IsNull(coordinate.ECEF, "Null ECEF 2 expected");

            el = new EagerLoad(EagerLoadType.ECEF);
            //Assert.IsTrue(Coordinate.TryParse("12,12", el, out coordinate), "Parse Failed");
            //Assert.IsNull(coordinate.CelestialInfo, "Null CelestialInfo 3 expected");
            //Assert.IsNotNull(coordinate.ECEF, "ECEF 3 expected");

            el = new EagerLoad(EagerLoadType.ECEF | EagerLoadType.Celestial);
            //Assert.IsTrue(Coordinate.TryParse("12,12", el, out coordinate), "Parse Failed");
            //Assert.IsNotNull(coordinate.CelestialInfo, "CelestialInfo 4 expected");
            //Assert.IsNotNull(coordinate.ECEF, "ECEF 4 expected");

            el = new EagerLoad(EagerLoadType.WebMercator | EagerLoadType.Celestial);
            //Assert.IsTrue(Coordinate.TryParse("12,12", el, out coordinate), "Parse Failed");
            //Assert.IsNotNull(coordinate.CelestialInfo, "CelestialInfo 5 expected");
            //Assert.IsNotNull(coordinate.WebMercator, "Web Mercator 5 expected");

            el = new EagerLoad(EagerLoadType.GEOREF| EagerLoadType.Celestial);
            //Assert.IsTrue(Coordinate.TryParse("SKAH0000", el, out coordinate), "Parse Failed");
            //Assert.IsNotNull(coordinate.CelestialInfo, "CelestialInfo 6 expected");
            //Assert.IsNotNull(coordinate.GEOREF, "GEOREF 6 expected");
        }

        /// <summary>
        /// Verifies that Parse is wrapping Try_Parse correctly
        /// </summary>
        //[TestMethod]
        public void Coordinate_Parse_Wrap_Tests()
        {
            string coord = "45.6, 22.4";
            EagerLoad el = new EagerLoad(EagerLoadType.Celestial | EagerLoadType.Cartesian | EagerLoadType.ECEF);
            CartesianType cType = CartesianType.ECEF;
            DateTime geoDate = new DateTime(2020, 3, 10, 10, 10, 12);

            Coordinate parseCoord;
            Coordinate tryParseCoord;

            parseCoord = Coordinate.Parse(coord);
            Coordinate.TryParse(coord, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, geoDate);
            Coordinate.TryParse(coord, geoDate, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, cType);
            Coordinate.TryParse(coord, cType, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, geoDate, cType);
            Coordinate.TryParse(coord, geoDate, cType, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, el);
            Coordinate.TryParse(coord, el, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, geoDate, el);
            Coordinate.TryParse(coord, geoDate, el, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, cType, el);
            Coordinate.TryParse(coord, cType, el, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

            parseCoord = Coordinate.Parse(coord, geoDate, cType, el);
            Coordinate.TryParse(coord, geoDate, cType, el, out tryParseCoord);
            //Assert.IsTrue(Parse_Wrap_Check(parseCoord, tryParseCoord, false));

          
        }

        /// <summary>
        /// Verifies restricted parse formats
        /// </summary>
        //[TestMethod]
        public void Disallowed_Parse_Check()
        {
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", new DateTime(2022,12,12), Allowed_Parse_Format.MGRS));           
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", new DateTime(2022, 12, 12), new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", CartesianType.ECEF, Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", new DateTime(2022, 12, 12), CartesianType.ECEF, Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("91 12", new DateTime(2022, 12, 12), CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS));


        }

        /// <summary>
        /// Verifies restricted parse formats
        /// </summary>
        //[TestMethod]
        public void Allowed_Parse_Check()
        {
            Coordinate.Parse("91 12", Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", new DateTime(2022, 12, 12), Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", new DateTime(2022, 12, 12), new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", CartesianType.ECEF, Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", new DateTime(2022, 12, 12), CartesianType.ECEF, Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS);
            Coordinate.Parse("35T LF 88428 80135", new DateTime(2022, 12, 12), CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS);
        }
        /// <summary>
        /// Verifies restricted try_parse formats
        /// </summary>
        //[TestMethod]
        public void Diallowed_TryParse_Check()
        {
            Coordinate c;
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", new DateTime(2022, 12, 12), Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", new DateTime(2022, 12, 12), new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", CartesianType.ECEF, Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", new DateTime(2022, 12, 12), CartesianType.ECEF, Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c));
            //Assert.AreEqual(false, Coordinate.TryParse("91 12", new DateTime(2022, 12, 12), CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c));

        }
        /// <summary>
        /// Verifies restricted try_parse formats
        /// </summary>
        //[TestMethod]
        public void Allowed_TryParse_Check()
        {
            Coordinate c;
            Coordinate.TryParse("91 12", Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", new DateTime(2022, 12, 12), Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", new DateTime(2022, 12, 12), new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", CartesianType.ECEF, Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", new DateTime(2022, 12, 12), CartesianType.ECEF, Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c);
            Coordinate.TryParse("35T LF 88428 80135", new DateTime(2022, 12, 12), CartesianType.ECEF, new EagerLoad(EagerLoadType.UTM_MGRS), Allowed_Parse_Format.MGRS, out c);
        }

        /// <summary>
        /// Verifies global parse format restrictions
        /// </summary>
        //[TestMethod]
        public void Z_Global_Settings_Parse_Works()
        {
            GlobalSettings.Default_Parsable_Formats = Allowed_Parse_Format.Lat_Long;

            Coordinate.Parse("24 56");
            Coordinate.Parse("24 56", new DateTime(2022, 2, 2));
            Coordinate.Parse("24 56", new EagerLoad(false));
            Coordinate.Parse("24 56", new DateTime(2022, 2, 2), new EagerLoad(false));

            Coordinate.Parse("24 56", CartesianType.ECEF);
            Coordinate.Parse("24 56", new DateTime(2022, 2, 2), CartesianType.ECEF);
            Coordinate.Parse("24 56", CartesianType.ECEF, new EagerLoad(false));
            Coordinate.Parse("24 56", new DateTime(2022, 2, 2), CartesianType.ECEF, new EagerLoad(false));

            GlobalSettings.Default_Parsable_Formats = Allowed_Parse_Format.Lat_Long | Allowed_Parse_Format.MGRS | Allowed_Parse_Format.UTM |
            Allowed_Parse_Format.Cartesian_ECEF | Allowed_Parse_Format.Cartesian_Spherical |
            Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.GEOREF;
        }

        /// <summary>
        /// Verifies global parse format fails
        /// </summary>
        //[TestMethod]
        public void Z_Global_Settings_Parse_Fails()
        {
            GlobalSettings.Default_Parsable_Formats = Allowed_Parse_Format.MGRS;

            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56"));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", new DateTime(2022, 2, 2)));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", new EagerLoad(false)));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", new DateTime(2022, 2, 2), new EagerLoad(false)));

            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", CartesianType.ECEF));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", new DateTime(2022, 2, 2), CartesianType.ECEF));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", CartesianType.ECEF, new EagerLoad(false)));
            //Assert.ThrowsException<FormatException>(() => Coordinate.Parse("24 56", new DateTime(2022, 2, 2), CartesianType.ECEF, new EagerLoad(false)));

            GlobalSettings.Default_Parsable_Formats = Allowed_Parse_Format.Lat_Long | Allowed_Parse_Format.MGRS | Allowed_Parse_Format.UTM |
            Allowed_Parse_Format.Cartesian_ECEF | Allowed_Parse_Format.Cartesian_Spherical |
            Allowed_Parse_Format.WebMercator | Allowed_Parse_Format.GEOREF;

        }

        private static bool Parse_Wrap_Check(Coordinate parseCoord, Coordinate tryParseCoord, bool eagerLoadCheck)
        {
            bool pass = true;

            if (parseCoord.Latitude.ToDouble() != tryParseCoord.Latitude.ToDouble()) { pass = false; }
            if (parseCoord.Longitude.ToDouble() != tryParseCoord.Longitude.ToDouble()) { pass = false; }
            if (parseCoord.GeoDate != tryParseCoord.GeoDate) { pass = false; }
            if (parseCoord.EagerLoadSettings != tryParseCoord.EagerLoadSettings) { pass = false; }
            if (parseCoord.Cartesian.X != tryParseCoord.Cartesian.X) { pass = false; }
            if (parseCoord.Cartesian.Y != tryParseCoord.Cartesian.Y) { pass = false; }
            if (parseCoord.Cartesian.Z != tryParseCoord.Cartesian.Z) { pass = false; }
            if (parseCoord.MGRS != null && eagerLoadCheck == true) { pass = false; }
            if(parseCoord.WebMercator !=null && eagerLoadCheck == true) { pass = false; }
            if (parseCoord.GEOREF!= null && eagerLoadCheck == true) { pass = false; }
            if (parseCoord.Parse_Format != tryParseCoord.Parse_Format) { pass = false; }

            return pass;
        }
    }
}
