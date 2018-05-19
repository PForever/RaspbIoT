using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbSingleton.Controller;
using LogSingleton;
using Microsoft.Data.Sqlite;
using static SqliteDb.SqlHelper;
namespace SqliteDb
{
    public sealed class DataBaseSqlite : ADataBaseController, ILoggable
    {
        public DataBaseSqlite()
        {
            Logger = Logging.Log;
        }

        #region DbOpen

        private SqliteDataReader ExecutCommand(string command, SqliteConnection db)
        {
            Logger.Debug($"ExecutCommand to {db.ConnectionString}:\r\n {command}");
            SqliteCommand createTable = new SqliteCommand(command, db);
            var query = createTable.ExecuteReader();
            Logger.Debug($"Result of executCommand: {query}");
            return query;
        }

        private int DbOpen<T>(string command, Func<SqliteDataReader, T> queryHandler, out T result)
        {
            result = default(T);
            SqliteConnection db = null;
            try
            {
                db = new SqliteConnection("Filename=Devices.db");
                db.Open();
                SqliteDataReader query = null;
                try
                {
                    query = ExecutCommand(command, db);
                    try
                    {
                        result = queryHandler(query);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Parsing has error: \r\n Message: {e.Message}");
                        return (int) DbErrors.QueryError;
                    }
                }
                catch (SqliteException e)
                {
                    Logger.Error($"Execute has error: \r\n Code: {e.SqliteErrorCode}\r\n Message: {e.Message}");
                    return (int) DbErrors.ExecuteError;
                }
                finally
                {
                    query?.Close();
                    query?.Dispose();
                }
            }
            catch (SqliteException e)
            {
                Logger.Error($"Connection has error: \r\n Code: {e.SqliteErrorCode}\r\n Message: {e.Message}");
                return (int) DbErrors.ConnectionError;
            }
            catch (Exception e)
            {
                Logger.Error($"Connection has unknow error: \r\n Message: {e.Message}");
                return (int)DbErrors.UnknowError;
            }
            finally
            {
                db?.Close();
                db?.Dispose();
            }
            return (int) DbErrors.Successful;
        }

        private static object QueryHandlerEmpty(SqliteDataReader query) => null;

        private int DbOpen(string command) => DbOpen(command, QueryHandlerEmpty, out object result);
        #endregion

        #region Add

        private int DropAll()
        {
            string command = $"select 'drop table ' || name || ';' from sqlite_master where type = 'table' and name not in ('{Tables.DEVICE}', '{Tables.PROPERTIES}', 'sqlite_sequence')";

            IList<string> QueryHandler(SqliteDataReader query)
            {
                var result = new List<string>();
                while (query.Read())
                {
                    result.Add(query.GetString(0));
                }
                return result;
            }
            int err = DbOpen(command, QueryHandler, out var columns);
            if (err != 0) return err;

            StringBuilder sb = new StringBuilder();
            foreach (var column in columns) sb.Append(column);

            sb.Append("DROP TABLE IF EXISTS ").Append(Tables.PROPERTIES).Append(";")
              .Append("DROP TABLE IF EXISTS ").Append(Tables.DEVICE).Append(";");
            return DbOpen(sb.ToString());
        }
        public override int Create(bool force = false)
        {
            string createDevice;
            string createProperties;
            if (force)
            {
                DropAll();
                createDevice = $"CREATE TABLE {Tables.DEVICE} ";
                createProperties = $"CREATE TABLE {Tables.PROPERTIES} ";
            }
            else
            {
                createDevice = $"CREATE TABLE IF NOT EXISTS {Tables.DEVICE} ";
                createProperties = $"CREATE TABLE IF NOT EXISTS {Tables.PROPERTIES} ";
            }
            string command = StringBuild(
                createDevice, "(\r\n",
                DeviceCode, " TEXT(20) not null constraint dev_code_pk primary key,\r\n",
                DeviceName, " TEXT(20) NOT NULL,\r\n", 
                MacAddress, " TEXT(20) NOT NULL);\r\n",
                createProperties, "(\r\n",
                DeviceCode, " text(20) not null,\r\n",
                PropCode, " INTEGER not null constraint pk_prop_code PRIMARY KEY AUTOINCREMENT,\r\n",
                PropName, " text(20) not null,\r\n",
                PropType, " text(10) not null,\r\n",
                IsSetter, " int(1) not null,\r\n",
                Description, " text(50) null,\r\n",
                "constraint uk_pro_code_name unique (", DeviceCode, ", ", PropName, "),\r\n",
                "constraint r_dev_code foreign key (", DeviceCode, ") references ", Tables.DEVICE, " (", DeviceCode, "));\r\n");
            return DbOpen(command);
        }

        public override int AddDevice(string deviceCode, string deviceName, string macAddress)
        {
            string command = StringBuild(
                "INSERT INTO ", Tables.DEVICE, "\r\n", 
                new []{DeviceCode, DeviceName, MacAddress}.ToSb(DuoQ), "\r\n",
                "VALUES \r\n", 
                new []{deviceCode, deviceName, macAddress}.ToSb(), ";\r\n",
                "CREATE TABLE IF NOT EXISTS ", deviceCode, " (\r\n",
                TimeMark, " INT NOT NULL,\r\n", 
                PropCode, " TEXT(20) NOT NULL,\r\n",
                PropValue, " TEXT(50) NOT NULL,\r\n",
                "PRIMARY KEY (", PropCode, ", ", TimeMark, "),\r\n",
                "FOREIGN KEY (", PropCode, ") REFERENCES ", Tables.PROPERTIES, " (", PropCode, "));\r\n");
            return DbOpen(command);
        }

        public override int AddProperties(string deviceCode, string propName, string propType, string isSetter, string description = null)
        {
            string command = StringBuild("INSERT INTO ", Tables.PROPERTIES, "\r\n",
                new[] {DeviceCode, PropName, PropType, IsSetter, Description}.ToSb(DuoQ), "\r\n",
                "VALUES \r\n", 
                new[] {deviceCode, propName, propType, isSetter, description ?? ""}.ToSb(), "\r\n");
            return DbOpen(command);
        }

        public override int AddTelemetry(string deviceCode, string propName, string timeMarker,
            string propValue)
        {
            //TODO использовать кэш
            int err = GetDevicesPropertyCode(out var properties, new[] {deviceCode}, new[] {propName});
            if (err != 0) return err;
            string propCode = properties[deviceCode][propName];
            string command = StringBuild("INSERT INTO ", deviceCode, "\r\n",
                new[] { TimeMark, PropCode, PropValue }.ToSb(DuoQ), "\r\n",
                "VALUES \r\n",
                new[] { timeMarker, propCode, propValue }.ToSb(), "\r\n");
            return DbOpen(command);
        }
        

        #endregion

        #region Get

        public override int GetDevices(out IList<(string deviceCode, string deviceName, string macAddress)> result, IList<string> deviceCodes = null)
        {
            string command;
            if(deviceCodes == null) command = StringBuild("SELECT * FROM ", Tables.DEVICE);
            else
                command = StringBuild(
                "SELECT * FROM ", Tables.DEVICE, "\r\n",
                " WHERE (", DeviceCode, " IN ", deviceCodes.ToSb(), "');\r\n");
            IList<(string deviceCode, string deviceName, string macAddress)> QueryHandler(SqliteDataReader query)
            {
                var parsResult = new List<(string deviceCode, string deviceName, string macAddress)>();

                while (query.Read())
                {
                    (string deviceCode, string deviceName, string macAddress) raw;
                    raw.deviceCode = query[DeviceCode].ToString();
                    raw.deviceName = query[DeviceName].ToString();
                    raw.macAddress = query[MacAddress].ToString();
                    parsResult.Add(raw);
                }
                return parsResult;
            }
            return DbOpen(command, QueryHandler, out result);
        }

        public override int GetDevicesByPropNames(out IList<(string deviceCode, string deviceName, string macAddress)> result, IList<string> propNames, IList<string> deviceCodes = null)
        {
            string command;
            if (deviceCodes == null) command = StringBuild("SELECT ", Tables.DEVICE, ".", DeviceCode, ", ", DeviceName, ", ", MacAddress, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES, " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, " AND ", PropName, " IN ", propNames.ToSb(), " GROUP BY ", Tables.DEVICE, ".", DeviceCode);
            else
                command = StringBuild(
                    "SELECT ", Tables.DEVICE, ".", DeviceCode, ", ", DeviceName, ", ", MacAddress, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES, " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, " AND ", PropName, " IN ", propNames.ToSb(),
                    " AND (", DeviceCode, " IN ", deviceCodes.ToSb(), "');\r\n", " GROUP BY ", Tables.DEVICE, ".", DeviceCode, "\r\n");
            IList<(string deviceCode, string deviceName, string macAddress)> QueryHandler(SqliteDataReader query)
            {
                var parsResult = new List<(string deviceCode, string deviceName, string macAddress)>();

                while (query.Read())
                {
                    (string deviceCode, string deviceName, string macAddress) raw;
                    raw.deviceCode = query[DeviceCode].ToString();
                    raw.deviceName = query[DeviceName].ToString();
                    raw.macAddress = query[MacAddress].ToString();
                    parsResult.Add(raw);
                }
                return parsResult;
            }
            return DbOpen(command, QueryHandler, out result);
        }

        public override int GetProperties(string deviceCode, out IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> result)
        {
            string command = StringBuild(
                "SELECT * FROM ", Tables.PROPERTIES, "\r\n",
                " WHERE (", DeviceCode, "='", deviceCode, "'); \r\n");
            IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> QueryHandler(SqliteDataReader query)
            {
                var parsResult = new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>();
                var propertyInfo = default((string deviceCode, string propCode, string propName, string propType, string isSetter, string description));
                while (query.Read())
                {
                    propertyInfo.deviceCode = query[DeviceCode].ToString();
                    propertyInfo.propCode = query[PropCode].ToString();
                    propertyInfo.propName = query[PropName].ToString();
                    propertyInfo.propType = query[PropType].ToString();
                    propertyInfo.isSetter = query[IsSetter].ToString();
                    propertyInfo.description = query[Description].ToString();
                    parsResult.Add(propertyInfo);
                }
                return parsResult;
            }
            return DbOpen(command, QueryHandler, out result);
        }

        private string GetDevicesCommand(IList<string> deviceCodes, IList<string> propNames)
        {
            if (deviceCodes != null && deviceCodes.Any())
            {
                if (propNames != null && propNames.Any())
                {
                    return StringBuild(
                        "SELECT ", Tables.DEVICE, ".", DeviceCode, " AS ", DeviceCode, ", ", PropCode, ", ", PropName, ", ", PropType, ", ", IsSetter, ", ", Description, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES,
                        " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, "\r\n",
                        " WHERE ",
                        PropName, " IN ", propNames.ToSb(),
                        " AND ",
                        Tables.DEVICE, ".", DeviceCode, " IN ", deviceCodes.ToSb(), "\r\n");
                }
                return StringBuild(
                    "SELECT ", Tables.DEVICE, ".", DeviceCode, " AS ", DeviceCode, ", ", PropCode, ", ", PropName, ", ", PropType, ", ", IsSetter, ", ", Description, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES,
                    " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, "\r\n",
                    " WHERE ",
                    Tables.DEVICE, ".", DeviceCode, " IN ", deviceCodes.ToSb(), "\r\n");
            }

            if (propNames != null && propNames.Any())
            {
                return StringBuild(
                    "SELECT ", Tables.DEVICE, ".", DeviceCode, " AS ", DeviceCode, ", ", PropCode, ", ", PropName, ", ", PropType, ", ", IsSetter, ", ", Description, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES,
                    " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, "\r\n",
                    " WHERE ",
                    PropName, " IN ", propNames.ToSb(), "\r\n");
            }
            return StringBuild(
                "SELECT ", Tables.DEVICE, ".", DeviceCode, " AS ", DeviceCode, ", ", PropCode, ", ", PropName, ", ", PropType, ", ", IsSetter, ", ", Description, " FROM ", Tables.DEVICE, " JOIN ", Tables.PROPERTIES,
                " ON ", Tables.DEVICE, ".", DeviceCode, " = ", Tables.PROPERTIES, ".", DeviceCode, "\r\n");
        }

        private int GetDevicesPropertyCode(out IDictionary<string, IDictionary<string, string>> properties, IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            string command = GetDevicesCommand(deviceCodes, propNames);

            IDictionary<string, IDictionary<string, string>>
            DevicesHandler(SqliteDataReader query)
            {
                var parsResult = new Dictionary<string, IDictionary<string, string>>();
                while (query.Read())
                {
                    string deviceCode = query[DeviceCode].ToString();
                    string propCode = query[PropCode].ToString();
                    string propName = query[PropName].ToString();
                    if(parsResult.ContainsKey(deviceCode)) parsResult[deviceCode].Add(propName, propCode);
                    else parsResult.Add(deviceCode, new Dictionary<string, string>{ { propName, propCode } });
                }
                return parsResult;
            }
            return DbOpen(command, DevicesHandler, out properties);
        }
        public override int GetDevicesPropertyNames(out IDictionary<string, IList<string>> properties, IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            string command = GetDevicesCommand(deviceCodes, propNames);

            IDictionary<string, IList<string>>
            DevicesHandler(SqliteDataReader query)
            {
                var parsResult = new Dictionary<string, IList<string>>();
                while (query.Read())
                {
                    string deviceCode = query[DeviceCode].ToString();
                    string propName = query[PropName].ToString();
                    if(parsResult.ContainsKey(deviceCode)) parsResult[deviceCode].Add(propName);
                    else parsResult.Add(deviceCode, new List<string> { propName });
                }
                return parsResult;
            }
            return DbOpen(command, DevicesHandler, out properties);
        }
        public override int GetDevicesProperties(out IDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>> properties, IList<string> deviceCodes = null, IList<string> propNames = null)
        {
            string command = GetDevicesCommand(deviceCodes, propNames);

            IDictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>
            DevicesHandler(SqliteDataReader query)
            {
                var parsResult = new Dictionary<string, IList<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)>>();
                while (query.Read())
                {
                    (string deviceCode, string propCode, string propName, string propType, string isSetter, string
                        description) row;
                    row.deviceCode = query[DeviceCode].ToString();
                    row.propCode = query[PropCode].ToString();
                    row.propName = query[PropName].ToString();
                    row.propType = query[PropType].ToString();
                    row.isSetter = query[IsSetter].ToString();
                    row.description = query[Description].ToString();
                    if (parsResult.ContainsKey(row.deviceCode)) parsResult[row.deviceCode].Add(row);
                    else parsResult.Add(row.deviceCode, new List<(string deviceCode, string propCode, string propName, string propType, string isSetter, string description)> { row });
                }
                return parsResult;
            }
            return DbOpen(command, DevicesHandler, out properties);
        }



        public override int GetProperty(string propCode, out (string deviceCode, string propCode, string propName, string propType, string isSetter, string description) result)
        {
            string command = StringBuild(
                "SELECT * FROM ", Tables.PROPERTIES, "\r\n",
                " WHERE (", PropCode, "='", propCode, "'); \r\n");
            (string deviceCode, string propCode, string propName, string propType, string isSetter, string description) QueryHandler(SqliteDataReader query)
            {
                query.Read();

                var parsResult = default((string deviceCode, string propCode, string propName, string propType, string isSetter, string description));
                parsResult.deviceCode = query[DeviceCode].ToString();
                parsResult.propCode = query[PropCode].ToString();
                parsResult.propName = query[PropName].ToString();
                parsResult.propType = query[PropType].ToString();
                parsResult.isSetter = query[IsSetter].ToString();
                parsResult.description = query[Description].ToString();
                return parsResult;
            }
            return DbOpen(command, QueryHandler, out result);
        }

        public override int GetTelemetries(out IList<(string propName, string timeMarker, string propValue)> result,
            string deviceCode, IList<string> propNames, string timeMarker = null)
        {
            string command;
            if (timeMarker == null)
            {
                command = StringBuild(
                    "SELECT ", PropName, ", ", PropValue, ", MAX(", TimeMark, ") AS ", TimeMark, " FROM ", deviceCode, " JOIN ", Tables.PROPERTIES,
                    " ON ", Tables.PROPERTIES, ".", PropCode, " = ", deviceCode, ".", PropCode,
                    " WHERE ", PropName, " IN ", propNames.ToSb()
                    , " GROUP BY ", PropName, " \r\n");
            }
            else
            {
                command = StringBuild(
                    "SELECT ", PropName, ", ", PropValue, ", ", TimeMark,  " FROM ", deviceCode, " WHERE ", PropName, " IN ", propNames.ToSb(),
                    " AND ", TimeMark, " > ", timeMarker, " \r\n");
            }

            IList<(string propName, string timeMarker, string propValue)> QueryHandler(SqliteDataReader query)
            {
                var parsResult = new List<(string propCode, string timeMarker, string propValue)>();
                var propertyInfo =
                    default((string propCode, string timeMarker, string propValue)
                    );
                while (query.Read())
                {
                    propertyInfo.propCode = query[PropName].ToString();
                    propertyInfo.timeMarker = query[TimeMark].ToString();
                    propertyInfo.propValue = query[PropValue].ToString();
                    parsResult.Add(propertyInfo);
                }
                return parsResult;
            }
            return DbOpen(command, QueryHandler, out result);
        }

        public override int GetTelemetries(out IDictionary<string, IList<(string propName, string timeMarker, string propValue)>> result,
            IList<string> deviceCodes = null, IList<string> propNames = null, string timeMarker = null)
        {
            int err = GetDevicesPropertyNames(out IDictionary<string, IList<string>> devices, deviceCodes, propNames);
            if (err != 0)
            {
                result = null;
                return err;
            }

            result = new Dictionary<string, IList<(string propCode, string timeMarker, string propValue)>>();
            if (timeMarker == null)
            {
                foreach (var device in devices)
                {
                    IList<(string propName, string timeMarker, string propValue)> devTelemeries;
                    if (err != 0) GetTelemetries(out devTelemeries, device.Key, propNames);
                    else err = GetTelemetries(out devTelemeries, device.Key, propNames);
                    result.Add(device.Key, devTelemeries);
                }
            }
            return err;
        }
        #endregion

        public ILogger Logger { get; }
    }
}
