using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core;
using NUnit.Framework;
using SqliteDb;
using static ConnectionLibraryTests.Help.TestHelper;

namespace ConnectionLibraryTests
{
    [TestFixture]
    public class DataBaseSqliteTests
    {
        [Test]
        public void TestGetTelemetriesLastManyValues()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode = RndString(max: 20);
            string deviceName = RndString(max: 20);
            string macAddress = RndString(max: 20);
            string propCode = RndString(max: 20);
            string propName = RndString(max: 20);
            List<string> propNames = new List<string> {propName};
            string propType = RndPropertyType.ToString();
            string isSetter = RndNumber(max: 2).ToString();
            string timeMark1 = RndTime.TimeFormater();
            string value1 = RndString(3, 3);
            string timeMark2 = RndTime.TimeFormater();
            string value2 = RndString(3, 3);
            string timeMark3 = RndTime.TimeFormater();
            string value3 = RndString(3, 3);

            var listExpected = new List<(string propName, string timeMarker, string propValue)>(3)
            {
                (propName, timeMark1, value1),
                (propName, timeMark2, value2),
                (propName, timeMark3, value3)
            };
            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode, deviceName, macAddress);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode, propCode, propName, propType, isSetter);
            Assert.AreEqual(err, 0);

            err = db.AddTelemetry(deviceCode, propCode, timeMark1, value1);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode, propCode, timeMark2, value2);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode, propCode, timeMark3, value3);
            Assert.AreEqual(err, 0);

            //2
            err = db.GetTelemetries(out IList<(string propName, string timeMarker, string propValue)> list, deviceCode,
                propNames);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(list.Count, 1);
            var tMax = listExpected.Max(t => decimal.Parse(t.timeMarker, CultureInfo.InvariantCulture))
                .ToString(CultureInfo.InvariantCulture);
            var m = listExpected.Where(t => t.timeMarker == tMax).ToList();
            var d = list[0].timeMarker.TimeFormater();
            Assert.AreEqual(decimal.Parse(list[0].timeMarker, CultureInfo.GetCultureInfo("Ru-ru")),
                decimal.Parse(m[0].timeMarker, CultureInfo.InvariantCulture));
            Assert.AreEqual(list[0].propName, m[0].propName);
            Assert.AreEqual(list[0].propValue, m[0].propValue);
        }

        [Test]
        public void TestGetDevices()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString(max: 20);
            string deviceCode2 = RndString(max: 20);
            string deviceCode3 = RndString(max: 20);
            string deviceName1 = RndString(max: 20);
            string deviceName2 = RndString(max: 20);
            string deviceName3 = RndString(max: 20);
            string macAddress1 = RndString(max: 20);
            string macAddress2 = RndString(max: 20);
            string macAddress3 = RndString(max: 20);
            string propName1 = RndString(max: 20);
            string propName2 = RndString(max: 20);
            string propName3 = RndString(max: 20);
            string isSetter1 = RndNumber(max: 2).ToString();
            string isSetter2 = RndNumber(max: 2).ToString();
            string isSetter3 = RndNumber(max: 2).ToString();
            string propType1 = RndPropertyType.ToString();
            string propType2 = RndPropertyType.ToString();
            string propType3 = RndPropertyType.ToString();
            string propCode1 = RndString(max: 20);
            string propCode2 = RndString(max: 20);
            string propCode3 = RndString(max: 20);

            Dictionary<string, IList<string>> devices =
                new Dictionary<string, IList<string>>
                {
                    {deviceCode1, new List<string> {propName1}},
                    {deviceCode2, new List<string> {propName2}},
                    {deviceCode3, new List<string> {propName3}}
                };

            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode1, propCode1, propName1, isSetter1, propType1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propCode2, propName2, isSetter2, propType2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propCode3, propName3, isSetter3, propType3);
            Assert.AreEqual(err, 0);
            //2
            err = db.GetDevicesPropertyNames(out var devicesAct);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(devicesAct.Count, devices.Count);
            int difCount = 0;
            foreach (var keyValuePair in devicesAct)
            {
                difCount += devices[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }

        [Test]
        public void TestGetDevicesManyPropNames()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString();
            string deviceCode2 = RndString(max: 20);
            string deviceCode3 = RndString(max: 20);
            string deviceName1 = RndString(max: 20);
            string deviceName2 = RndString(max: 20);
            string deviceName3 = RndString(max: 20);
            string macAddress1 = RndString(max: 20);
            string macAddress2 = RndString(max: 20);
            string macAddress3 = RndString(max: 20);
            string propName1 = RndString(max: 20);
            string propName2 = RndString(max: 20);
            string propName3 = RndString(max: 20);
            string isSetter1 = RndNumber(max: 2).ToString();
            string isSetter2 = RndNumber(max: 2).ToString();
            string isSetter3 = RndNumber(max: 2).ToString();
            string propType1 = RndPropertyType.ToString();
            string propType2 = RndPropertyType.ToString();
            string propType3 = RndPropertyType.ToString();
            string propCode11 = RndString(max: 20);
            string propCode12 = RndString(max: 20);
            string propCode13 = RndString(max: 20);
            string propCode21 = RndString(max: 20);
            string propCode22 = RndString(max: 20);
            string propCode23 = RndString(max: 20);
            string propCode31 = RndString(max: 20);
            string propCode32 = RndString(max: 20);
            string propCode33 = RndString(max: 20);


            Dictionary<string, IList<string>> devices =
                new Dictionary<string, IList<string>>
                {
                    {deviceCode1, new List<string> {propName1, propName2, propName3}},
                    {deviceCode2, new List<string> {propName1, propName2, propName3}},
                    {deviceCode3, new List<string> {propName1, propName2, propName3}}
                };

            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode1, propCode11, propName1, isSetter1, propType1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode1, propCode12, propName2, isSetter2, propType2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode1, propCode13, propName3, isSetter3, propType3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode2, propCode21, propName1, isSetter1, propType1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propCode22, propName2, isSetter2, propType2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propCode23, propName3, isSetter3, propType3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode3, propCode31, propName1, isSetter1, propType1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propCode32, propName2, isSetter2, propType2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propCode33, propName3, isSetter3, propType3);
            Assert.AreEqual(err, 0);
            //2
            err = db.GetDevicesPropertyNames(out var devicesAct);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(devicesAct.Count, devices.Count);
            int difCount = 0;
            foreach (var keyValuePair in devicesAct)
            {
                difCount += devices[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }

        [Test]
        public void TestGetDevicesProperties()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString();
            string deviceCode2 = RndString();
            string deviceCode3 = RndString();
            string deviceName1 = RndString();
            string deviceName2 = RndString();
            string deviceName3 = RndString();
            string macAddress1 = RndString();
            string macAddress2 = RndString();
            string macAddress3 = RndString();
            string propName1 = RndString();
            string propName2 = RndString();
            string propName3 = RndString();
            string isSetter1 = RndNumber(max: 2).ToString();
            string isSetter2 = RndNumber(max: 2).ToString();
            string isSetter3 = RndNumber(max: 2).ToString();
            string propType1 = RndPropertyType.ToString();
            string propType2 = RndPropertyType.ToString();
            string propType3 = RndPropertyType.ToString();
            string description1 = RndString();
            string description2 = RndString();
            string description3 = RndString();
            string propCode1 = RndString();
            string propCode2 = RndString();
            string propCode3 = RndString();

            Dictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>> devices =
                new Dictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>
                {
                    {deviceCode1, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode1, propCode1, propName1, propType1, isSetter1, description1)}},
                    {deviceCode2, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode2, propCode2, propName2, propType2, isSetter2, description2)}},
                    {deviceCode3, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode3, propCode3, propName3, propType3, isSetter3, description3)}}
                };

            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode1, propName1, propType1, isSetter1, description1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propName2, propType2, isSetter2, description2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propName3, propType3, isSetter3, description3);
            Assert.AreEqual(err, 0);
            //2
            err = db.GetDevicesProperties(out var devicesAct);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(devicesAct.Count, devices.Count);
            int difCount = 0;
            
            foreach (var keyValuePair in devicesAct)
            {
                difCount += devices[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }
        [Test]
        public void TestGetDevicesPropertiesWithPropNames()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString();
            string deviceCode2 = RndString();
            string deviceCode3 = RndString();
            string deviceName1 = RndString();
            string deviceName2 = RndString();
            string deviceName3 = RndString();
            string macAddress1 = RndString();
            string macAddress2 = RndString();
            string macAddress3 = RndString();
            string propName1 = RndString();
            string propName2 = RndString();
            string propName3 = RndString();
            string isSetter1 = RndNumber(max: 2).ToString();
            string isSetter2 = RndNumber(max: 2).ToString();
            string isSetter3 = RndNumber(max: 2).ToString();
            string propType1 = RndPropertyType.ToString();
            string propType2 = RndPropertyType.ToString();
            string propType3 = RndPropertyType.ToString();
            string description1 = RndString();
            string description2 = RndString();
            string description3 = RndString();
            string propCode1 = RndString();
            string propCode2 = RndString();
            string propCode3 = RndString();

            var devices =
                new Dictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>
                {
                    {deviceCode1, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode1, propCode1, propName1, propType1, isSetter1, description1)}},
                    {deviceCode2, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode2, propCode2, propName2, propType2, isSetter2, description2)}},
                };
            var propNames = new[] {propName1, propName2};

            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode1, propName1, propType1, isSetter1, description1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propName2, propType2, isSetter2, description2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propName3, propType3, isSetter3, description3);
            Assert.AreEqual(err, 0);
            //2
            err = db.GetDevicesProperties(out var devicesAct, propNames: propNames);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(devicesAct.Count, devices.Count);
            int difCount = 0;

            foreach (var keyValuePair in devicesAct)
            {
                difCount += devices[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }
        [Test]
        public void TestGetDevicesPropertiesWithDevices()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString();
            string deviceCode2 = RndString();
            string deviceCode3 = RndString();
            string deviceName1 = RndString();
            string deviceName2 = RndString();
            string deviceName3 = RndString();
            string macAddress1 = RndString();
            string macAddress2 = RndString();
            string macAddress3 = RndString();
            string propName1 = RndString();
            string propName2 = RndString();
            string propName3 = RndString();
            string isSetter1 = RndNumber(max: 2).ToString();
            string isSetter2 = RndNumber(max: 2).ToString();
            string isSetter3 = RndNumber(max: 2).ToString();
            string propType1 = RndPropertyType.ToString();
            string propType2 = RndPropertyType.ToString();
            string propType3 = RndPropertyType.ToString();
            string description1 = RndString();
            string description2 = RndString();
            string description3 = RndString();
            string propCode1 = RndString();
            string propCode2 = RndString();
            string propCode3 = RndString();

            var devices =
                new Dictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>
                {
                    {deviceCode1, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode1, propCode1, propName1, propType1, isSetter1, description1)}},
                    {deviceCode2, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> {(deviceCode2, propCode2, propName2, propType2, isSetter2, description2)}},
                };
            var deviceCodes = new[] {deviceCode1, deviceCode2};

            //1.2
            int err = db.Create(true);
            Assert.AreEqual(err, 0);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);

            err = db.AddProperties(deviceCode1, propName1, propType1, isSetter1, description1);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propName2, propType2, isSetter2, description2);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propName3, propType3, isSetter3, description3);
            Assert.AreEqual(err, 0);
            //2
            err = db.GetDevicesProperties(out var devicesAct, deviceCodes: deviceCodes);
            //3
            Assert.AreEqual(err, 0);
            Assert.AreEqual(devicesAct.Count, devices.Count);
            int difCount = 0;
            foreach (var keyValuePair in devicesAct)
            {
                difCount += devices[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }
        [Test]
        public void TestGetTelemetriesManyProps()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode = RndString(max: 20);
            string deviceName = RndString(max: 20);
            string macAddress = RndString(max: 20);
            string propCode1 = RndString(max: 20);
            string propCode2 = RndString(max: 20);
            string propCode3 = RndString(max: 20);
            string propName1 = RndString(max: 20);
            string propName2 = RndString(max: 20);
            string propName3 = RndString(max: 20);
            List<string> propNames = new List<string> {propName1, propName2, propName3};
            string propType = RndPropertyType.ToString();
            string isSetter = RndNumber(max: 2).ToString();
            string timeMark1 = RndTime.TimeFormater();
            string value1 = RndString(3, 3);
            string timeMark2 = RndTime.TimeFormater();
            string value2 = RndString(3, 3);
            string timeMark3 = RndTime.TimeFormater();
            string value3 = RndString(3, 3);

            var listExpected = new List<(string propName, DateTime timeMarker, string propValue)>(3)
            {
                (propName1, timeMark1.TimeFormater(), value1),
                (propName2, timeMark2.TimeFormater(), value2),
                (propName3, timeMark3.TimeFormater(), value3)
            };
            //1.2
            int err = db.Create(true);

            err = db.AddDevice(deviceCode, deviceName, macAddress);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode, propCode1, propName1, propType, isSetter);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode, propCode2, propName2, propType, isSetter);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode, propCode3, propName3, propType, isSetter);
            Assert.AreEqual(err, 0);

            err = db.AddTelemetry(deviceCode, propCode1, timeMark1, value1);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode, propCode2, timeMark2, value2);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode, propCode3, timeMark3, value3);
            Assert.AreEqual(err, 0);

            //2
            err = db.GetTelemetries(out IList<(string propName, string timeMarker, string propValue)> list, deviceCode,
                propNames);
            //3
            var listAct = list.Select(t => (t.propName, t.timeMarker.TimeFormater(), t.propValue)).ToList();
            Assert.AreEqual(err, 0);
            Assert.AreEqual(list.Count, 3);
            Assert.IsEmpty(listAct.Except(listExpected));
        }

        [Test]
        public void TestGetTelemetriesManyDiveces()
        {
            //1.1
            var db = new DataBaseSqlite();
            string deviceCode1 = RndString(max: 20);
            string deviceCode2 = RndString(max: 20);
            string deviceCode3 = RndString(max: 20);
            string deviceName1 = RndString(max: 20);
            string deviceName2 = RndString(max: 20);
            string deviceName3 = RndString(max: 20);
            string macAddress1 = RndString(max: 20);
            string macAddress2 = RndString(max: 20);
            string macAddress3 = RndString(max: 20);
            string propCode1 = RndString(max: 20);
            string propCode2 = RndString(max: 20);
            string propCode3 = RndString(max: 20);
            string propName1 = RndString(max: 20);
            string propName2 = RndString(max: 20);
            string propName3 = RndString(max: 20);
            IList<string> propNames = new List<string> {propName1, propName2, propName3};
            Dictionary<string, IList<string>> devices =
                new Dictionary<string, IList<string>>
                {
                    {deviceCode1, new List<string> {propName1}},
                    {deviceCode2, new List<string> {propName2}},
                    {deviceCode3, new List<string> {propName3}}
                };
            string propType = RndPropertyType.ToString();
            string isSetter = RndNumber(max: 2).ToString();
            string timeMark1 = RndTime.TimeFormater();
            string value1 = RndString(3, 3);
            string timeMark2 = RndTime.TimeFormater();
            string value2 = RndString(3, 3);
            string timeMark3 = RndTime.TimeFormater();
            string value3 = RndString(3, 3);

            Dictionary<string, IList<(string propName, DateTime timeMarker, string propValue)>> dictionaryExpected =
                new Dictionary<string, IList<(string propName, DateTime timeMarker, string propValue)>>
                {
                    {
                        deviceCode1,
                        new List<(string propName, DateTime timeMarker, string propValue)>
                        {
                            (propName1, timeMark1.TimeFormater(), value1)
                        }
                    },
                    {
                        deviceCode2,
                        new List<(string propName, DateTime timeMarker, string propValue)>
                        {
                            (propName2, timeMark2.TimeFormater(), value2)
                        }
                    },
                    {
                        deviceCode3,
                        new List<(string propName, DateTime timeMarker, string propValue)>
                        {
                            (propName3, timeMark3.TimeFormater(), value3)
                        }
                    },
                };
            //1.2
            int err = db.Create(true);

            err = db.AddDevice(deviceCode1, deviceName1, macAddress1);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode2, deviceName2, macAddress2);
            Assert.AreEqual(err, 0);
            err = db.AddDevice(deviceCode3, deviceName3, macAddress3);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode1, propCode1, propName1, propType, isSetter);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode2, propCode2, propName2, propType, isSetter);
            Assert.AreEqual(err, 0);
            err = db.AddProperties(deviceCode3, propCode3, propName3, propType, isSetter);
            Assert.AreEqual(err, 0);

            err = db.AddTelemetry(deviceCode1, propCode1, timeMark1, value1);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode2, propCode2, timeMark2, value2);
            Assert.AreEqual(err, 0);
            err = db.AddTelemetry(deviceCode3, propCode3, timeMark3, value3);
            Assert.AreEqual(err, 0);

            //2
            err = db.GetTelemetries(out var dictionary, propNames: propNames);
            //3
            Assert.AreEqual(err, 0);

            var dictionaryAct = dictionary.Select(d =>
                new KeyValuePair<string, IList<(string propName, DateTime timeMarker, string propValue)>>
                (d.Key,
                    d.Value.ToList().ConvertAll(l =>
                        (l.propName, l.timeMarker.TimeFormater(), l.propValue))
                )).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Assert.AreEqual(dictionary.Count, 3);
            int difCount = 0;
            foreach (var keyValuePair in dictionaryAct)
            {
                difCount += dictionaryExpected[keyValuePair.Key].Except(keyValuePair.Value).Count();
            }

            Assert.AreEqual(difCount, 0);
        }
    }
}