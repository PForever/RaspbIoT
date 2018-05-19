using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;
using ConnectionLibrary.Abstract.Server;

namespace ConnectionLibraryTests.Help
{
    public class MassageParserMoq : IMessageParser
    {
        public event Action<RemoteHostInfo, EventRequestArgs> RequestReceived;
        public void TelemetryReceivedInvoke(RemoteHostInfo hostInfo, EventRequestArgs args) => RequestReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventTelemetryArgs> TelemetryReceived;
        public void ConnectMessageReceivedInvoke(RemoteHostInfo hostInfo, EventMessageConnectArgs args) => ConnectMessageReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventMessageConnectArgs> ConnectMessageReceived;
        public void ErrorMessageReceivedInvoke(RemoteHostInfo hostInfo, EventErrArgs args) => ErrorMessageReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventErrArgs> ErrorMessageReceived;
        public void CommandMessageReceivedInvoke(RemoteHostInfo hostInfo, EventCommandMessageArgs args) => CommandMessageReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventCommandMessageArgs> CommandMessageReceived;
        public void CallReceivedInvoke(RemoteHostInfo hostInfo, EventCallArgs args) => CallReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventCallArgs> CallReceived;
        public void OrderReceivedInvoke(RemoteHostInfo hostInfo, EventOrderArgs args) => OrderReceived?.Invoke(hostInfo, args);
        public event Action<RemoteHostInfo, EventOrderArgs> OrderReceived;
        public void EventDataHandler(object sender, EventDataArg<string> e)
        {
        }
    }
}