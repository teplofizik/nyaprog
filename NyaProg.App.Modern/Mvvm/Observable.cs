using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NyaProg.App.Modern.Mvvm
{
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Raise([CallerMemberName] string Name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));

        protected bool Set<T>(ref T Field, T Value, [CallerMemberName] string Name = null)
        {
            if (EqualityComparer<T>.Default.Equals(Field, Value)) return false;

            Field = Value;
            Raise(Name);
            return true;
        }
    }
}
