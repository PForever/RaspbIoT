using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ConnectionLibrary.Abstract.DataObjects.Containers;
using ConnectionLibrary.Abstract.DataObjects.Messages;
using ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles;
using ConnectionLibrary.Abstract.Modules.MessageManager.Handlers.Args;
using ConnectionLibrary.Modules;
using ConnectionLibrary.Modules.DbManager;
using ConnectionLibrary.Modules.MessageManager;
using ConnectionLibrary.Modules.RequestManager;
using DbSingleton;
using LogSingleton;
//using Log4NetProj;
using MetroLogForUwp;
using SqliteDb;

namespace RaspbIoTModel
{
    public class MainPageModel
    {
        #region Consts
        private const string MulticastHostint = "239.0.0.223";
        private const string Host = "192.168.1.33";
        private const int UdpPort = 8083;
        private const int TcpPort = 8082;
        private readonly ConcurrentBag<RemoteHostInfo> _subscribers;
        #endregion

        private string _protocol = "UDP";
        private string _myCode = "krakadile";

        private readonly ServerManager _server;
        private readonly SenderManager _sender;
        private readonly DbManager _dbManager;
        private readonly Actualizer _actualizer;
        private readonly MessageManager _messageManager;
        private readonly RequestManager _requestManager;

        #region Constructors

        public MainPageModel()
        {
            #region Preparing

            var log = new Logger();
            Logging.Initialize(log);

            var db = new DataBaseSqlite();
            DbControlling.Initialize(db);

            //TODO убрать тру и пересоздание таблиц вслучае ошибки 
            int res = db.Create(true);
            if (res != 0) db.Create(true);

            #endregion

            _subscribers = new ConcurrentBag<RemoteHostInfo>();
            _server = new ServerManager(MulticastHostint, UdpPort, TcpPort);
            _sender = new SenderManager(MulticastHostint, TcpPort, UdpPort);
            _dbManager = new DbManager(_myCode);

            _messageManager = new MessageManager(MulticastHostint, _myCode, _sender, _server, _dbManager);
            _actualizer = new Actualizer(_messageManager, _dbManager, _myCode);

            _requestManager = new RequestManager(_actualizer, _messageManager, _myCode);

            _messageManager.ConnectMessageReceived += (o, args) =>  PopHandler(args.ConnectMessage);
            _messageManager.TelemetryReceived += (o, args) =>  PopHandler(args.TelemetryInfo);
            _server.ServerStart();
        }

        ~MainPageModel()
        {
            _server?.Dispose();
            _sender?.Dispose();
        }

        public MainPageModel(string remoteMessage, string localMessage) : this()
        {
            RemoteMessage = remoteMessage;
            LocalMessage = localMessage;
        }

        #endregion

        #region Properties

        #region RemoteMessage

        private string _remoteMessage;

        public string RemoteMessage
        {
            get => _remoteMessage;
            set
            {
                _remoteMessage = value;
                _RemoteMessageChanged?.Invoke(value);
            }
        }

        private event Action<string> _RemoteMessageChanged;

        public event Action<string> RemoteMessageChanged
        {
            add => _RemoteMessageChanged += value;
            remove => _RemoteMessageChanged -= value;
        }

        #endregion

        public string LocalMessage { get; set; }
        public string RemoteIp { get; set; }
        public string Protocol { get; set; }

        #endregion

        #region Pop

        private void PopHandler(Telemetry telemetry)
        {
            RemoteMessage += $"\n(Tel) {telemetry.DeviceCode}: [{telemetry.TimeMarker}] ";
            foreach (var propertiesValue in telemetry.Values)
            {
                RemoteMessage += $"{propertiesValue.Key} = {propertiesValue.Value}; ";
            }
        }

        private void PopHandler(ConnectMessage connectMessage)
        {
            RemoteMessage +=
                $"\n(Con) {connectMessage.DeviceCode}: [{connectMessage.TimeMarker}] {connectMessage.Device.Name}";
        }
       
        #endregion

        #region Push

        public void PushHandler()
        {
            var devices = _dbManager.GetDevices();
            foreach (string device in devices)
            {
                Order order = new Order(_myCode, DateTime.Now, device);
                _messageManager.OnOrder(this, new EventOrderArgs(order));
            }
        }

        #endregion
    }
}
