using System;
using System.Windows.Input;

namespace RaspbIoTViewModel
{
    public class PushCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            //TODO 
            return true;
        }
        protected PushCommand() { }
        public PushCommand(Action action)
        {
            if (action == null)
            {
                _action = () => { };
            }
            _action = action;
        }
        private readonly Action _action;
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _action.Invoke();
            }
        }
        public event EventHandler CanExecuteChanged;
    }
}