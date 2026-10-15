using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class ProjectEditorWindow : Window
    {
        public ProjectEditorWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnAddTarget(object Sender, RoutedEventArgs E) =>
            (DataContext as ProjectEditorViewModel)?.AddTarget();

        private void OnRemoveTarget(object Sender, RoutedEventArgs E)
        {
            var Vm = (Sender as Button)?.DataContext as TargetEditVM;
            (DataContext as ProjectEditorViewModel)?.RemoveTarget(Vm);
        }

        private void OnAddStep(object Sender, RoutedEventArgs E) =>
            ((Sender as Button)?.DataContext as TargetEditVM)?.AddStep();

        private void OnRemoveStep(object Sender, RoutedEventArgs E)
        {
            var Vm = (Sender as Button)?.DataContext as StepEditVM;
            Vm?.Owner?.RemoveStep(Vm);
        }

        private void OnSave(object Sender, RoutedEventArgs E)
        {
            var Vm = DataContext as ProjectEditorViewModel;
            if (Vm == null) return;

            Vm.Save();
            if (Vm.Saved) Close();
        }

        private void OnCancel(object Sender, RoutedEventArgs E) => Close();
    }
}
