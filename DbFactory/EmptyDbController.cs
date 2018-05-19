using System.Collections.Concurrent;
using System.Collections.Generic;
using DbSingleton.Controller;

namespace DbSingleton
{
    public class EmptyDbController : ADataBaseController
    {
        public override int Create(bool force = false)
        {
            return (int) DbErrors.Successful;
        }

        public override int AddDevice(string deviceCode, string deviceName, string macAddress)
        {
            return (int)DbErrors.Successful;
        }

        public override int AddProperties(string deviceCode, string propName, string type, string isSetter,
            string description = null)
        {
            return (int)DbErrors.Successful;
        }

        public override int AddTelemetry(string deviceCode, string propName, string timeMarker, string propValue)
        {
            return (int)DbErrors.Successful;
        }

        public override int GetDevices(out IList<(string deviceCode, string deviceName, string macAddress)> result,
            IList<string> deviceCode)
        {
            result = default(IList<(string deviceCode, string deviceName, string macAddress)>);
            return (int)DbErrors.Successful;
        }

        public override int GetDevicesByPropNames(out IList<(string deviceCode, string deviceName, string macAddress)> result, IList<string> propNames, IList<string> deviceCodes = null)
        {
            result = new List<(string deviceCode, string deviceName, string macAddress)>();
            return (int)DbErrors.Successful;
        }

        public override int GetProperties(string deviceCode, out IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> result)
        {
            result = new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>();
            return (int)DbErrors.Successful;
        }

        public override int GetProperty(string propCode,
            out (string deviceCode, string propCode, string propName, string propType, string isSetter, string description)
                result)
        {
            result = default((string deviceCode, string propCode, string propName, string propType, string isSetter, string description));
            return (int)DbErrors.Successful;
        }

        public override int GetTelemetries(out IList<(string propName, string timeMarker, string propValue)> result, string deviceCode, IList<string> propNames, string timeMarker = null)
        {
            result = new List<(string propCode, string timeMarker, string propValue)>();
            return (int) DbErrors.Successful;
        }

        public override int GetTelemetries(
            out IDictionary<string, IList<(string propName, string timeMarker, string propValue)>> result,
            IList<string> deviceCodes = null, IList<string> propNames = null, string timeMarker = null)
        {
            result = new Dictionary<string, IList<(string propCode, string timeMarker, string propValue)>>();
            return (int)DbErrors.Successful;
        }

        public override int GetDevicesPropertyNames(out IDictionary<string, IList<string>> properties, IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            properties = new Dictionary<string, IList<string>>();
            return (int) DbErrors.Successful;
        }

        public override int GetDevicesProperties(out IDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>> properties, IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            properties = new ConcurrentDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>();
            return (int) DbErrors.Successful;
        }
    }
}