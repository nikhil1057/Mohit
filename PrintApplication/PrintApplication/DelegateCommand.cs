using System;
using System.Windows;
using System.Windows.Input;

namespace PrintApplication
{
    public class DelegateCommand : ICommand
    {
        Action<object> _execute;
        Func<object, bool> _canExecute;
        private Action<Window> closeApplicationOK;
        private Func<object, bool> canSelectCabinets;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute )
        {
            _execute = execute;
            _canExecute = canExecute;
        }


        
        public DelegateCommand(ViewModel.PrintViewModel printViewModel, Action<Window> closeApplicationOK, Func<object, bool> canSelectCabinets)
        {
            this.closeApplicationOK = closeApplicationOK;
            this.canSelectCabinets = canSelectCabinets;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
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
            _execute(parameter);
        }
    }
}
