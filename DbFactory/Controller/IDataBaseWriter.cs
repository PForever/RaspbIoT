namespace DbSingleton.Controller
{
    public interface IDataBaseWriter
    {
        int Create(bool force = false);
        int AddDevice(string deviceCode, string deviceName, string macAddress);
        int AddProperties(string deviceCode, string propName, string type, string isSetter, string description = null);
        int AddTelemetry(string deviceCode, string propName, string timeMarker,
            string propValue);
    }
}