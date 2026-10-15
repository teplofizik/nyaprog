using Avalonia.Media;

using NyaProg.App.Modern.Mvvm;
using NyaProg.Presentation;

namespace NyaProg.App.Modern.ViewModels
{
    public sealed class StepViewModel : Observable
    {
        public StepItem Item { get; }

        public StepViewModel(StepItem Item) { this.Item = Item; }

        public string Title => Item.Title;

        public bool Running => Item.Running;

        public string Result => Item.Result;

        public bool? Ok => Item.Ok;

        /// <summary>Цвет индикатора: серый — ожидает, оранжевый — выполняется, зелёный/красный — результат.</summary>
        public IBrush StatusBrush
        {
            get
            {
                if (Item.Running) return Brushes.Orange;
                if (Item.Ok == null) return Brushes.Gray;
                return Item.Ok.Value ? Brushes.Green : Brushes.Red;
            }
        }
    }
}
