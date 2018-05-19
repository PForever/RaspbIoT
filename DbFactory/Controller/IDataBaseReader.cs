using System.Collections.Generic;

namespace DbSingleton.Controller
{
    public interface IDataBaseReader
    {
        int GetDevices(out IList<(string deviceCode, string deviceName, string macAddress)> result,
            IList<string> deviceCodes = null);
        int GetDevicesByPropNames(out IList<(string deviceCode, string deviceName, string macAddress)> result,
            IList<string> propNames,
            IList<string> deviceCodes = null);
        int GetProperties(string deviceCode, out IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> result);
        int GetProperty(string propCode, out (string deviceCode, string propCode, string propName, string propType, string isSetter, string description) result);
        int GetTelemetries(out IList<(string propName, string timeMarker, string propValue)> result,
            string deviceCode, IList<string> propNames, string timeMarker = null);
        int GetTelemetries(
            out IDictionary<string, IList<(string propName, string timeMarker, string propValue)>> dictionary,
            IList<string> deviceCodes = null, IList<string> propNames = null, string timeMarker = null);

        int GetDevicesPropertyNames(out IDictionary<string, IList<string>> properties, IList<string> deviceCodes = null,
            IList<string> propNames = null);
        int GetDevicesProperties(out IDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>> properties, IList<string> deviceCodes = null, IList<string> propNames = null);

    }
}