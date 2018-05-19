using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Modules.MessageManager;
using ConnectionLibraryTests.Help;
using NUnit.Framework;
using static ConnectionLibraryTests.Help.TestHelper;

namespace ConnectionLibraryTests
{
    [TestFixture]
    public class ConnectionWorkerTest : ConnectionWorker
    {
        [Test]
        public void OpenDeviceConnectionTest()
        {
            AddressBook = new AddressBook();
        }

        [Test]
        public void GetIpTest()
        {

        }

        [Test]
        public void WakeUpTest()
        {
            
        }

        [Test]
        public void WaitReadyTest()
        {
            //1
            AddressBook = new AddressBook();
            TimeSpan timeOut = new TimeSpan(0, 0, 10);
            var serverMoq = new MassageParserMoq();

            string code = RndString();
            RemoteHostInfo hostInfo = RndRemoteHostInfo();
            Call call = RndCall(CallType.Ready, code);
            EventCallArgs arg = new EventCallArgs(call);

            Server = serverMoq;
            //2

            var task = Task.Run(() => WaitReady(code, timeOut));
            Task.Delay(new TimeSpan(0, 0, 3)).Wait();
            serverMoq.CallReceivedInvoke(hostInfo, arg);

            task.Wait();
            //3
            Assert.AreEqual(task.Result, ConnectionResult.Successful);
        }

        [Test]
        public void WaitRecallTest()
        {
            //1
            AddressBook = new AddressBook();
            TimeSpan timeOut = new TimeSpan(0, 0, 10);
            var serverMoq = new MassageParserMoq();

            string code = RndString();
            RemoteHostInfo hostInfo = RndRemoteHostInfo();
            Call call = RndCall(CallType.Recall, code);
            EventCallArgs arg = new EventCallArgs(call);

            Server = serverMoq;
            string ip = null;
            //2

            var task = Task.Run(() => WaitRecall(code, timeOut, out ip));
            Task.Delay(new TimeSpan(0, 0, 3)).Wait();
            serverMoq.CallReceivedInvoke(hostInfo, arg);

            task.Wait();
            //3
            Assert.AreEqual(task.Result, ConnectionResult.Successful);
            Assert.AreEqual(ip, hostInfo.Host);
        }
    }
}