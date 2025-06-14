using System;
using System.Windows.Input;

namespace ProjectsNow.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public void Execute()
        {
            _execute?.Invoke();
        }
        void ICommand.Execute(object parameter)
        {
            Execute();
        }
        public bool CanExecute()
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            else
            {
                return true;
            }
        }
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute((T)parameter);
            }
            else
            {
                return false;
            }
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public void Execute(object parameter)
        {
            _execute?.Invoke((T)parameter);
        }
    }
}
