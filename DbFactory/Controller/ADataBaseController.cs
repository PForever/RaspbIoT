using System.Collections.Generic;

namespace DbSingleton.Controller
{
    public abstract class ADataBaseController : IDataBaseWriter, IDataBaseReader
    {
        public abstract int Create(bool force = false);
        public abstract int AddDevice(string deviceCode, string deviceName, string macAddress);
        public abstract int AddProperties(string deviceCode, string propName, string type, string isSetter, string description = null);
        public abstract int AddTelemetry(string deviceCode, string propName, string timeMarker,
            string propValue);

        public abstract int GetDevices(out IList<(string deviceCode, string deviceName, string macAddress)> result,
            IList<string> deviceCodes = null);

        public abstract int GetDevicesByPropNames(
            out IList<(string deviceCode, string deviceName, string macAddress)> result, IList<string> propNames,
            IList<string> deviceCodes = null);
        public abstract int GetProperties(string deviceCode, out IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> result);
        public abstract int GetProperty(string propCode, out (string deviceCode, string propCode, string propName, string propType, string isSetter, string description) result);

        public abstract int GetTelemetries(
            out IList<(string propName, string timeMarker, string propValue)> result,
            string deviceCode, IList<string> propNames, string timeMarker = null);

        public abstract int GetTelemetries(
            out IDictionary<string, IList<(string propName, string timeMarker, string propValue)>> result,
            IList<string> deviceCodes = null, IList<string> propNames = null, string timeMarker = null);

        public abstract int GetDevicesPropertyNames(out IDictionary<string, IList<string>> properties,
            IList<string> deviceCodes = null, IList<string> propNames = null);

        public abstract int GetDevicesProperties(out IDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>> properties, IList<string> deviceCodes = null, IList<string> propNames = null);
    }
}