using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibraryTests.Help
{
    public class ObjectParserMoq : IObjectParser
    {
        public void OnRequest(RemoteHostInfo remoteHost, EventRequestArgs args)
        {
        }

        public void OnConnectMessage(RemoteHostInfo remoteHost, EventMessageConnectArgs args)
        {
        }

        public void OnCommandMessage(RemoteHostInfo remoteHost, EventCommandMessageArgs args)
        {
        }

        public void OnTelemetry(RemoteHostInfo remoteHost, EventTelemetryArgs args)
        {
        }

        public void OnEventErrorMessage(RemoteHostInfo remoteHost, EventErrArgs args)
        {
        }

        public void OnOrder(RemoteHostInfo remoteHost, EventOrderArgs args)
        {
        }

        public void OnCall(RemoteHostInfo remoteHost, EventCallArgs args)
        {
        }

        public void OnSend(object sender, EventDataArg<string> e)
        {
        }
    }
}