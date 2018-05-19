using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Modules.DbManager;
using DbSingleton;
using NUnit.Framework;
using SqliteDb;
using static ConnectionLibraryTests.Help.TestHelper;
namespace ConnectionLibraryTests
{
    [TestFixture]
    public class DbManagerTests : DbManager
    {
        [Test]
        public void TestCreateTelemetry()
        {
            //1
            string deviceCode = RndString(max: 20);

            string propName1 = RndString(max: 20);
            string timeMark1 = RndTime.TimeFormater();
            string propValue1 = RndString();

            string propName2 = RndString(max: 20);
            string timeMark2 = RndTime.TimeFormater();
            string propValue2 = RndString();

            string propName3 = RndString(max: 20);
            string timeMark3 = RndTime.TimeFormater();
            string propValue3 = RndString();

            var data = new List<(string propName, string timeMarker, string propValue)>
            {
                (propName1, timeMark1, propValue1),
                (propName2, timeMark2, propValue2),
                (propName3, timeMark3, propValue3)
            };


            var pv1 = new PropertiesValues(new Dictionary<string, string> {{propName1, propValue1}});
            Telemetry telemetry1 = new Telemetry(MyCode, pv1, timeMark1.TimeFormater(), deviceCode);

            var pv2 = new PropertiesValues(new Dictionary<string, string> {{propName2, propValue2}});
            Telemetry telemetry2 = new Telemetry(MyCode, pv2, timeMark2.TimeFormater(), deviceCode);

            var pv3 = new PropertiesValues(new Dictionary<string, string> {{propName3, propValue3}});
            Telemetry telemetry3 = new Telemetry(MyCode, pv3, timeMark3.TimeFormater(), deviceCode);

            IList<Telemetry> telemetriesExpected = new List<Telemetry>{telemetry1, telemetry2, telemetry3};

            var kvp = new KeyValuePair<string, IList<(string propName, string timeMarker, string propValue)>>(deviceCode, data);
            //2
            IList<Telemetry> telemetries = CreateTelemetry(kvp);
            //3
            bool Compare(Telemetry a, Telemetry b)
            {
                return a.DeviceCode == b.DeviceCode &&
                       a.MessageType == b.MessageType &&
                       a.TargetDeviceCode == b.TargetDeviceCode &&
                       a.TimeMarker == b.TimeMarker &&
                       !a.Values.Except(b.Values).Any();
            }
            var comparer = new EquaComparer<Telemetry>(Compare, obg => obg.DeviceCode.GetHashCode());
            Assert.IsEmpty(telemetries.Except(telemetriesExpected, comparer));
        }

        [Test]
        public void TestGetDevicesProperties()
        {
            //1.1
            var db = new DataBaseSqlite();
            DbControlling.Initialize(db);
            DbReader = DbControlling.DbReader;

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

            var propInfo1 = new PropertyInfo(description1, isSetter1.BoolFormater(), propType1.EnumFotmater<ProperyType>());
            var properties1 = new Properties(new Dictionary<string, PropertyInfo>{{propCode1, propInfo1}});
            var propInfo2 = new PropertyInfo(description2, isSetter2.BoolFormater(), propType2.EnumFotmater<ProperyType>());
            var properties2 = new Properties(new Dictionary<string, PropertyInfo>{{propCode2, propInfo2}});
            var propInfo3 = new PropertyInfo(description3, isSetter3.BoolFormater(), propType3.EnumFotmater<ProperyType>());
            var properties3 = new Properties(new Dictionary<string, PropertyInfo>{{propCode3, propInfo3}});
            var deviceExpected = new Devices
            {
                {deviceCode1, new Device(deviceCode1, macAddress1, deviceName1, properties1)},
                {deviceCode2, new Device(deviceCode2, macAddress2, deviceName2, properties2)},
                {deviceCode3, new Device(deviceCode3, macAddress3, deviceName3, properties3)}
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
            var devices = GetDevicesProperties();
            //3

            bool Compare(KeyValuePair<string, Device> a, KeyValuePair<string, Device> b)
            {
                return a.Key == b.Key &&
                       a.Value.MacAddress == b.Value.MacAddress &&
                       a.Value.Code == b.Value.Code &&
                       a.Value.Name == b.Value.Name &&
                       !a.Value.Info.Except(b.Value.Info).Any();
            }

            Assert.AreEqual(deviceExpected.Count, devices.Count);
            var comparer = new EquaComparer<KeyValuePair<string, Device>>(Compare, device => device.Key.GetHashCode());
            Assert.IsEmpty(deviceExpected.Except(devices, comparer));
        }

        [Test]
        public void TestGetDevicesPropertiesByPropNames()
        {
            //1.1
            var db = new DataBaseSqlite();
            DbControlling.Initialize(db);
            DbReader = DbControlling.DbReader;

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

            var propInfo1 = new PropertyInfo(description1, isSetter1.BoolFormater(), propType1.EnumFotmater<ProperyType>());
            var properties1 = new Properties(new Dictionary<string, PropertyInfo>{{propCode1, propInfo1}});
            var propInfo2 = new PropertyInfo(description2, isSetter2.BoolFormater(), propType2.EnumFotmater<ProperyType>());
            var properties2 = new Properties(new Dictionary<string, PropertyInfo>{{propCode2, propInfo2}});
            var propInfo3 = new PropertyInfo(description3, isSetter3.BoolFormater(), propType3.EnumFotmater<ProperyType>());
            var properties3 = new Properties(new Dictionary<string, PropertyInfo>{{propCode3, propInfo3}});
            var deviceExpected = new Devices
            {
                {deviceCode1, new Device(deviceCode1, macAddress1, deviceName1, properties1)},
                {deviceCode2, new Device(deviceCode2, macAddress2, deviceName2, properties2)},
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
            var devices = GetDevicesProperties(propNames: propNames);
            //3

            bool Compare(KeyValuePair<string, Device> a, KeyValuePair<string, Device> b)
            {
                return a.Key == b.Key &&
                       a.Value.MacAddress == b.Value.MacAddress &&
                       a.Value.Code == b.Value.Code &&
                       a.Value.Name == b.Value.Name &&
                       !a.Value.Info.Except(b.Value.Info).Any();
            }

            Assert.AreEqual(deviceExpected.Count, devices.Count);
            var comparer = new EquaComparer<KeyValuePair<string, Device>>(Compare, device => device.Key.GetHashCode());
            Assert.IsEmpty(deviceExpected.Except(devices, comparer));
        }
        public DbManagerTests() : base(RndString())
        {
        }
    }
}