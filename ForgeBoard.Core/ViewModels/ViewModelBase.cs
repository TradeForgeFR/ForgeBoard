using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ForgeBoard.Core.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class Command : ICommand
    {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
    }

    public class BasicCommand : Command
    {
        private readonly Action _action;

        public BasicCommand(Action action, IObservable<bool> canExecute = null)
        {
            _action = action ?? throw new ArgumentNullException();
        }

        public override void Execute(object _)
        {
            _action?.Invoke();
        }
    }
}
