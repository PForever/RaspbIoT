using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;
using ConnectionLibrary.Modules.DbManager;
using ConnectionLibraryTests.Help;
using Moq;
using SqliteDb;
using static ConnectionLibraryTests.Help.TestHelper;

namespace ConnectionLibraryTests
{
    [TestFixture]
    public class ActualizerTests : Actualizer
    {
        [Test]
        public void CreateOrderTests()
        {
            //1
            string code = RndString();

            string propName1 = RndString();
            string propValue1 = RndString();
            DateTime timeMark1 = RndTime;
            PropertiesValues properties1 = new PropertiesValues(new Dictionary<string, string> { {propName1, propValue1} });

            string propName2 = RndString();
            string propValue2 = RndString();
            DateTime timeMark2 = RndTime;
            PropertiesValues properties2 = new PropertiesValues(new Dictionary<string, string> { {propName2, propValue2} });

            string propName3 = RndString();
            string propValue3 = RndString();
            DateTime timeMark3 = RndTime;
            PropertiesValues properties3 = new PropertiesValues(new Dictionary<string, string> { { propName3, propValue3 } });

            IList<Telemetry> telemetries = new List<Telemetry>
            {
                new Telemetry(MyCode, properties1, timeMark1, code),
                new Telemetry(MyCode, properties2, timeMark2, code),
                new Telemetry(MyCode, properties3, timeMark3, code),
            };
            var kvp = new KeyValuePair<string, IList<Telemetry>>(code, telemetries);
            List<string> propNames = new List<string>{propName1, propName2, propName3};

            Order orderExpected = new Order(MyCode, DateTime.Now, code, getPropertiesValues: propNames);
            //2
            var orderAct = CreateOrder(kvp, out List<Telemetry> oldTelemetries);
            //3
            Assert.AreEqual(orderAct.DeviceCode, orderExpected.DeviceCode);
            Assert.IsEmpty(orderAct.GetPropertiesValues.Except(orderExpected.GetPropertiesValues));
            Assert.AreEqual(orderAct.GetPropertiesValues.Count, orderExpected.GetPropertiesValues.Count);
            Assert.IsNull(orderAct.SetPropertiesValues);
            Assert.AreNotEqual(orderAct.TimeMarker, timeMark1);
            Assert.AreNotEqual(orderAct.TimeMarker, timeMark2);
            Assert.AreNotEqual(orderAct.TimeMarker, timeMark3);
        }

        [Test]
        public void CreateOrdersTests()
        {
            //1
            var telemetriesDictionary = new Dictionary<string, IList<Telemetry>>(3);
            var devices = new List<string>(3);
            string devicesCode1 = RndString();

            string propName11 = RndString();
            string propValue11 = RndString();
            DateTime timeMark11 = RndTime;
            PropertiesValues properties11 = new PropertiesValues(new Dictionary<string, string> { { propName11, propValue11 } });

            string propName12 = RndString();
            string propValue12 = RndString();
            DateTime timeMark12 = RndTime;
            PropertiesValues properties12 = new PropertiesValues(new Dictionary<string, string> { { propName12, propValue12 } });

            string propName13 = RndString();
            string propValue13 = RndString();
            DateTime timeMark13 = RndTime;
            PropertiesValues properties13 = new PropertiesValues(new Dictionary<string, string> { { propName13, propValue13 } });

            IList<Telemetry> telemetries1 = new List<Telemetry>
            {
                new Telemetry(MyCode, properties11, timeMark11, devicesCode1),
                new Telemetry(MyCode, properties12, timeMark12, devicesCode1),
                new Telemetry(MyCode, properties13, timeMark13, devicesCode1),
            };
            telemetriesDictionary.Add(devicesCode1, telemetries1);
            List<string> propNames1 = new List<string> { propName11, propName12, propName13 };
            Order orderExpected1 = new Order(MyCode, DateTime.Now, devicesCode1, getPropertiesValues: propNames1);
            devices.Add(devicesCode1);


            string devicesCode2 = RndString();

            string propName21 = RndString();
            string propValue21 = RndString();
            DateTime timeMark21 = RndTime;
            PropertiesValues properties21 = new PropertiesValues(new Dictionary<string, string> { { propName21, propValue21 } });

            string propName22 = RndString();
            string propValue22 = RndString();
            DateTime timeMark22 = RndTime;
            PropertiesValues properties22 = new PropertiesValues(new Dictionary<string, string> { { propName22, propValue22 } });

            string propName23 = RndString();
            string propValue23 = RndString();
            DateTime timeMark23 = RndTime;
            PropertiesValues properties23 = new PropertiesValues(new Dictionary<string, string> { { propName23, propValue23 } });

            IList<Telemetry> telemetries2 = new List<Telemetry>
            {
                new Telemetry(MyCode, properties21, timeMark21, devicesCode2),
                new Telemetry(MyCode, properties22, timeMark22, devicesCode2),
                new Telemetry(MyCode, properties23, timeMark23, devicesCode2),
            };
            telemetriesDictionary.Add(devicesCode2, telemetries2);
            List<string> propNames2 = new List<string> { propName21, propName22, propName23 };
            Order orderExpected2 = new Order(MyCode, DateTime.Now, devicesCode2, getPropertiesValues: propNames2);
            devices.Add(devicesCode2);


            string devicesCode3 = RndString();

            string propName31 = RndString();
            string propValue31 = RndString();
            DateTime timeMark31 = RndTime;
            PropertiesValues properties31 = new PropertiesValues(new Dictionary<string, string> { { propName31, propValue31 } });

            string propName32 = RndString();
            string propValue32 = RndString();
            DateTime timeMark32 = RndTime;
            PropertiesValues properties32 = new PropertiesValues(new Dictionary<string, string> { { propName32, propValue32 } });

            string propName33 = RndString();
            string propValue33 = RndString();
            DateTime timeMark33 = RndTime;
            PropertiesValues properties33 = new PropertiesValues(new Dictionary<string, string> { { propName33, propValue33 } });

            IList<Telemetry> telemetries3 = new List<Telemetry>
            {
                new Telemetry(MyCode, properties31, timeMark31, devicesCode3),
                new Telemetry(MyCode, properties32, timeMark32, devicesCode3),
                new Telemetry(MyCode, properties33, timeMark33, devicesCode3),
            };
            telemetriesDictionary.Add(devicesCode3, telemetries3);
            List<string> propNames3 = new List<string> { propName31, propName32, propName33 };
            Order orderExpected3 = new Order(MyCode, DateTime.Now, devicesCode3, getPropertiesValues: propNames3);
            devices.Add(devicesCode2);


            List<Order> ordersExcpected = new List<Order>{orderExpected1, orderExpected2, orderExpected3};

            //2
            var orders = CreateOrders(telemetriesDictionary, out IList<string> devicesAct, out var oldTelemetries);
            //3

            Assert.AreEqual(orders.Count, ordersExcpected.Count);
            foreach (Order orderExpected in ordersExcpected)
            {
                var orderAct = orders.First(o => o.DeviceCode == orderExpected.DeviceCode);
                Assert.IsEmpty(orderExpected.GetPropertiesValues.Except(orderAct.GetPropertiesValues));
                Assert.AreNotEqual(orderAct.TimeMarker, orderExpected.TimeMarker);
                Assert.IsNull(orderAct.SetPropertiesValues);
            }
        }

        [Test]
        public void ActualCheckTest()
        {
            //1
            ActualSpan = new TimeSpan(0, 0, 10);
            Telemetry telemetry1 = new Telemetry(null, null, DateTime.Now.AddSeconds(-20), null);
            Telemetry telemetry2 = new Telemetry(null, null, DateTime.Now.AddSeconds(-5), null);
            //2
            bool act1 = ActualCheck(telemetry1);
            bool act2 = ActualCheck(telemetry2);
            //3
            Assert.IsFalse(act1);
            Assert.IsTrue(act2);
        }

        [Test]
        public void WaitRecallTest()
        {
            //1
            TimeSpan timeOut = new TimeSpan(0, 0, 10);
            DbMoq moq = new DbMoq();
            List<string> deviceCodes = RndStringList();
            List<Telemetry> telemetries = new List<Telemetry>(deviceCodes.Count);
            foreach (string deviceCode in deviceCodes)
                telemetries.Add(RndTelemetry(deviceCode));
            DbController = moq;
            IList<Telemetry> telemetriesAct = null;
            //2

            var task = Task.Run(() => WaitRecall(deviceCodes, timeOut, out telemetriesAct));
            Task.Delay(new TimeSpan(0, 0, 3)).Wait();
            foreach (Telemetry telemetry in telemetries)
            {
                moq.DataAddedInvok(this, telemetry);
            }
            task.Wait();
            //3
            Assert.AreEqual(task.Result, ConnectionResult.Successful);
            Assert.IsEmpty(telemetries.Except(telemetriesAct));
        }

        [Test]
        public void GetDataTest()
        {
            //1
            ActualSpan = new TimeSpan(0, 0, 10);
            MessageSendler = new MessageSenderMoq();

            var telemetriesDictionaryExpected = new Dictionary<string, IList<Telemetry>>(3);
            var telemetriesDictionaryUpdatedExpected = new Dictionary<string, Telemetry>(3);
            string deviceCode1 = RndString();
            List<Telemetry> telemetries1 = RndTelemetries(deviceCode1);
            telemetriesDictionaryExpected.Add(deviceCode1, telemetries1);

            PropertiesValues propertiesValues1 = new PropertiesValues();
            foreach (Telemetry telemetry in telemetries1)
            {
                if (ActualCheck(telemetry)) continue;
                foreach (var kvp in telemetry.Values)
                {
                    if (propertiesValues1.ContainsKey(kvp.Key)) continue;
                    propertiesValues1.Add(kvp.Key, kvp.Value);
                }
            }
            Telemetry newTelemetries1 = new Telemetry(MyCode, propertiesValues1, DateTime.Now, deviceCode1);
            telemetriesDictionaryExpected[deviceCode1].Add(newTelemetries1);
            telemetriesDictionaryUpdatedExpected.Add(deviceCode1, newTelemetries1);

            string deviceCode2 = RndString();
            List<Telemetry> telemetries2 = RndTelemetries(deviceCode2);
            telemetriesDictionaryExpected.Add(deviceCode2, telemetries2);

            PropertiesValues propertiesValues2 = new PropertiesValues();
            foreach (Telemetry telemetry in telemetries2)
            {
                if (ActualCheck(telemetry)) continue;
                foreach (var kvp in telemetry.Values)
                {
                    if (propertiesValues2.ContainsKey(kvp.Key)) continue;
                    propertiesValues2.Add(kvp.Key, kvp.Value);
                }
            }
            Telemetry newTelemetries2 = new Telemetry(MyCode, propertiesValues2, DateTime.Now, deviceCode2);
            telemetriesDictionaryExpected[deviceCode2].Add(newTelemetries2);
            telemetriesDictionaryUpdatedExpected.Add(deviceCode2, newTelemetries2);


            string deviceCode3 = RndString();
            List<Telemetry> telemetries3 = RndTelemetries(deviceCode3);
            telemetriesDictionaryExpected.Add(deviceCode3, telemetries3);

            PropertiesValues propertiesValues3 = new PropertiesValues();
            foreach (Telemetry telemetry in telemetries3)
            {
                if (ActualCheck(telemetry)) continue;
                foreach (var kvp in telemetry.Values)
                {
                    if (propertiesValues3.ContainsKey(kvp.Key)) continue;
                    propertiesValues3.Add(kvp.Key, kvp.Value);
                }
            }
            Telemetry newTelemetries3 = new Telemetry(MyCode, propertiesValues3, DateTime.Now, deviceCode3);
            telemetriesDictionaryExpected[deviceCode3].Add(newTelemetries3);
            telemetriesDictionaryUpdatedExpected.Add(deviceCode3, newTelemetries3);

            var moq = new DbMoq { SetData = telemetriesDictionaryExpected};
            DbController = moq;

            var telemetryName1 = telemetries1.SelectMany(t => t.Values.Keys);
            var telemetryName2 = telemetries1.SelectMany(t => t.Values.Keys);
            var telemetryName3 = telemetries1.SelectMany(t => t.Values.Keys);
            List<string> properties = telemetryName1.Concat(telemetryName2.Concat(telemetryName3)).Distinct().ToList();

            //2
            IDictionary<string, IList<Telemetry>> telemetriesDictionary = new Dictionary<string, IList<Telemetry>>();
            var task = Task.Run(() => GetData(out telemetriesDictionary, properties));
            Task.Delay(3000).Wait();

            foreach (var telemetriesList in telemetriesDictionaryExpected)
            {
                foreach (Telemetry telemetry in telemetriesList.Value)
                {
                    moq.DataAddedInvok(this, telemetry);
                }
            }
            task.Wait();
            //3

            Assert.AreEqual(task.Result, ConnectionResult.Successful);
            Assert.AreEqual(telemetriesDictionaryExpected.Count, telemetriesDictionary.Count);
            foreach (var kvp in telemetriesDictionaryExpected)
            {
                var maxDate = telemetriesDictionary[kvp.Key].Max(t => t.TimeMarker);
                var updatedTelemetries =
                    telemetriesDictionary[kvp.Key].Where(t => t.TimeMarker == maxDate);
                Assert.AreEqual(telemetriesDictionaryExpected[kvp.Key].Count, kvp.Value.Count);
                Assert.AreEqual(updatedTelemetries.Count(), 1);
                var act = updatedTelemetries.First();
                var arrage = telemetriesDictionaryUpdatedExpected[kvp.Key];
                Assert.AreEqual(act.DeviceCode, arrage.DeviceCode);
                Assert.AreEqual(act.TimeMarker, arrage.TimeMarker);
                Assert.IsEmpty(act.Values.Keys.Except(arrage.Values.Keys));
                //foreach (Telemetry telemetry in telemetriesList.Value)
                //{

                //}
            }
        }

        [Test]
        public void METHOD()
        {
            
        }
    }
}
