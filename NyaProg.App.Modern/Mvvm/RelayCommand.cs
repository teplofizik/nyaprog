using System;
using System.Windows.Input;

namespace NyaProg.App.Modern.Mvvm
{
    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object> Exec;
        private readonly Func<object, bool> Can;

        public RelayCommand(Action<object> Exec, Func<object, bool> Can = null)
        {
            this.Exec = Exec;
            this.Can = Can;
        }

        public bool CanExecute(object Parameter) => (Can == null) || Can(Parameter);

        public void Execute(object Parameter) => Exec(Parameter);

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
