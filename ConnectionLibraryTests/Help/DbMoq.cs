using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.DbManager;

namespace ConnectionLibraryTests.Help
{
    public class DbMoq : IDb
    {
        public void AddDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        public void AddData(Telemetry telemetry)
        {
            throw new NotImplementedException();
        }

        public event DataAddHandler DataAdded;
        public void DataAddedInvok(object sender, Telemetry telemetry) => DataAdded?.Invoke(sender, telemetry);

        public IDictionary<string, IList<Telemetry>> SetData;
        public IDictionary<string, IList<Telemetry>> GetData(IList<string> deviceCodes = null, IList<string> properties = null, DateTime? dateTime = null)
        {
            return SetData;
        }
        public IList<string> SetDevicesResult;
        public IList<string> GetDevices()
        {
            return SetDevicesResult;
        }

        public Devices GetDevicesProperties(IList<string> deviceCodes, IList<string> propNames)
        {
            throw new NotImplementedException();
        }

        public Properties GetDeviceInfo(string code)
        {
            throw new NotImplementedException();
        }
    }
}