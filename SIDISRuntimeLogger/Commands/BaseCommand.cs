using System;
using System.Windows.Input;

namespace SIDISRuntimeLogger.Commands
{
    public class Command : ICommand
    {
        bool _execualbe;
        Action _action;

        public Command(Action action, bool execuable = true)
        {
            _action = action;
            _execualbe = execuable;
        }

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public bool CanExecute(object parameter)
        {
            return _execualbe;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
