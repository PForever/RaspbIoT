using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using RaspbIoTModel;
using RaspbIoTViewModel.Annotations;
using Windows.UI.Xaml;

namespace RaspbIoTViewModel
{
    public class MainPageViewModel : DependencyObject, INotifyPropertyChanged
    {
        private readonly MainPageModel _model;
        public ICommand ButtonPush { get; }

        #region RemoteMessage

        public string RemoteMessage
        {
            get => _model.RemoteMessage;
            set => _model.RemoteMessage = value;
        }

        #endregion

        #region LocalMessage

        public string LocalMessage
        {
            get => _model.LocalMessage;
            set => _model.LocalMessage = value;
        }

        #endregion

        #region RemoteIp

        public string RemoteIp
        {
            get => _model.RemoteIp;
            set => _model.RemoteIp = value;
        }

        #endregion

        #region Protocol

        public string Protocol
        {
            get => _model.Protocol;
            set => _model.Protocol = value;
        }

        #endregion

        void PushHandler() => RemoteMessage = LocalMessage;

        public MainPageViewModel()
        {
            _model = new MainPageModel();
            _model.RemoteMessageChanged += (value) => DispatcherNoty(); //TODO Валидация по value

            RemoteMessage = "Нет сообщений";
            LocalMessage = "Введите текст";
            RemoteIp = "192.168.1.34";
            Protocol = "UDP";
            ButtonPush = new PushCommand(_model.PushHandler);
        }

        private void DispatcherNoty()
        {
            var t = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { OnPropertyChanged(nameof(RemoteMessage)); });
        }
        #region Noty
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion    
    }
}
