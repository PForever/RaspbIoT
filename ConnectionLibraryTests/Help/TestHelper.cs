using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;

namespace ConnectionLibraryTests.Help
{
    public static class TestHelper
    {
        #region Tools

        public static string MyCode;
        private const char FrCh = 'a';
        private const char LsCh = (char)('z' + 1);
        private static readonly Random Rnd;
        private static string[] _propNames;
        private static string[] _protocoles;

        static TestHelper()
        {
            Rnd = new Random();
            MyCode = RndString();
            _propNames = new[] {"Temperatur", "Time", "Wather", "TargetPosition"};
            _protocoles = new[] {"TCP", "UDP"};
        }

        public class EquaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;
            private readonly Func<T, int> _hasher;

            public EquaComparer(Func<T, T, bool> comparer)
            {
                _comparer = comparer;
                _hasher = obj => obj.GetHashCode();
            }
            public EquaComparer(Func<T, T, bool> comparer, Func<T, int> hasher)
            {
                _comparer = comparer;
                _hasher = hasher;
            }

            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _hasher(obj);
            }
        }

        public static string RndString(int min = 10, int max = 20)
        {
            int len = Rnd.Next(min, max);
            var sb = new StringBuilder(len);
            for (int i = 0; i < len; i++) sb.Append((char) Rnd.Next(FrCh, LsCh));
            return sb.ToString();
        }
        public static int RndNumber(int min = 0, int max = 10) => Rnd.Next(min, max);
        public static string RndName => _propNames[Rnd.Next(0, _propNames.Length)];
        public static bool RndBool => Rnd.Next(0, 2) == 1;
        public static T RndNull<T>(this T value) where T : class => Rnd.Next(0, 2) == 1 ? value : null;
        public static DateTime RndTime => DateTime.Now.AddSeconds(Rnd.Next(-20, 0));
        public static List<string> RndStringList(int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var list = new List<string>(count);
            for (int i = 0; i < count; i++) list.Add(RndString());
            return list;
        }

        public static string RndIp() => $"{Rnd.Next(0, 256)}.{Rnd.Next(0, 256)}.{Rnd.Next(0, 256)}.{Rnd.Next(0, 256)}";
        public static string RndProtocol => _protocoles[Rnd.Next(0, _protocoles.Length)];
        #endregion

        #region RndEnums

        public static ProperyType RndPropertyType => (ProperyType)Rnd.Next(0, (int)ProperyType.Time + 1);
        public static CallType RndCallType => (CallType)Rnd.Next(0, (int)CallType.Ready + 1);

        #endregion

        #region RndContainers
        public static IDevice RndDevice(string code = null)
        {
            if (code == null) code = RndString();
            string mac = RndString();
            string name = RndName;
            Properties properties = RndProperies();
            var decice = new Device(code, mac, name, properties);
            return decice;
        }

        public static Properties RndProperies(int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var properties = new Properties(count);
            for (int i = 0; i < count; i++)
            {
                string code = RndString();
                string description = RndString(50, 100).RndNull();
                ProperyType prType = RndPropertyType;
                bool isSetter = RndBool;
                PropertyInfo propertyInfo = new PropertyInfo(description, isSetter, prType);
                properties.Add(code, propertyInfo);
            }
            return properties;
        }
        public static PropertiesValues RndPropertiesValues(int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var propertiesValues = new PropertiesValues(count);
            for (int i = 0; i < count; i++)
            {
                string propCode = RndString();
                string value = RndString();
                propertiesValues.Add(propCode, value);
            }
            return propertiesValues;
        }

        public static RemoteHostInfo RndRemoteHostInfo() => new RemoteHostInfo(RndIp(), RndProtocol);
        #endregion

        #region RndMessages

        public static ConnectMessage RndConnection(string code = null)
        {
            if (code == null) code = RndString();
            IDevice device = RndDevice(code);
            return new ConnectMessage(device, device.Code, RndTime);
        }
        public static CommandMessage RndCommaMessage(string code = null)
        {
            if (code == null) code = RndString();
            string message = RndString();
            DateTime timeMark = RndTime;
            return new CommandMessage(message, MyCode, timeMark, code);
        }
        public static Telemetry RndTelemetry(string code = null)
        {
            if (code == null) code = RndString();
            PropertiesValues propertiesValues = RndPropertiesValues();
            DateTime timeMark = RndTime;
            return new Telemetry(MyCode, propertiesValues, timeMark, code);
        }

        public static Call RndCall(CallType? callType = null, string code = null)
        {
            var rndCallType = callType ?? RndCallType;
            if (code == null) code = RndString();
            string senderCode = code;
            string targetCode = RndString();
            DateTime time = RndTime;
            return new Call(time, senderCode, rndCallType, targetCode);
        }
        public static Request RndRequest(string code = null)
        {
            if (code == null) code = RndString();
            string message = RndString();
            DateTime dateTime = RndTime;
            //TODO rnd словарь
            return new Request(MyCode, message, dateTime, code);
        }

        public static Order RndOrder(string code = null)
        {
            if (code == null) code = RndString();
            PropertiesValues propertiesValues = RndPropertiesValues();
            List<string> properties = RndStringList();
            DateTime timeMark = RndTime;
            return new Order(MyCode, timeMark, code, propertiesValues, properties);
        }

        public static ErrorMessage RndErrorMessage(string code = null)
        {
            if (code == null) code = RndString();
            string message = RndString();
            return new ErrorMessage(MyCode, message, code);
        }

        #endregion

        #region RndLists

        public static List<Telemetry> RndTelemetries(string code = null, int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var telemetries = new List<Telemetry>(count);
            for (int i = 0; i < count; i++) telemetries.Add(RndTelemetry(code));
            return telemetries;
        }
        public static List<CommandMessage> RndCommandMessages(string code = null, int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var commandMessages = new List<CommandMessage>(count);
            for (int i = 0; i < count; i++) commandMessages.Add(RndCommaMessage(code));
            return commandMessages;
        }
        public static List<ConnectMessage> RndConnectMessages(string code = null, int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var connectMessages = new List<ConnectMessage>(count);
            for (int i = 0; i < count; i++) connectMessages.Add(RndConnection(code));
            return connectMessages;
        }
        public static List<Order> RndOrders(string code = null, int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var orders = new List<Order>(count);
            for (int i = 0; i < count; i++) orders.Add(RndOrder(code));
            return orders;
        }
        public static List<ErrorMessage> RndErrorMessages(string code = null, int min = 2, int max = 10)
        {
            int count = Rnd.Next(min, max);
            var errorMessages = new List<ErrorMessage>(count);
            for (int i = 0; i < count; i++) errorMessages.Add(RndErrorMessage(code));
            return errorMessages;
        }

        #endregion
    }
}