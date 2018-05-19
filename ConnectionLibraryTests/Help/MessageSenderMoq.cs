using System;
using ConnectionLibrary.Abstract.Modules.MessageManager;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Abstract.Modules.MessageManager.Parse;

namespace ConnectionLibraryTests.Help
{
    public class MessageSenderMoq : IFullMessageSender
    {
        public void OnRequest(object sender, EventRequestArgs args)
        {
        }

        public void OnConnectMessage(object sender, EventMessageConnectArgs args)
        {
        }

        public void OnCommandMessage(object sender, EventCommandMessageArgs args)
        {
        }

        public void OnTelemetry(object sender, EventTelemetryArgs args)
        {
        }

        public void OnEventErrorMessage(object sender, EventErrArgs args)
        {
        }

        public void OnOrder(object sender, EventOrderArgs args)
        {
        }
    }
}