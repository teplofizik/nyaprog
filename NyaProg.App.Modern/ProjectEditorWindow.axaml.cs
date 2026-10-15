using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

using NyaProg.App.Modern.ViewModels;

namespace NyaProg.App.Modern
{
    public partial class ProjectEditorWindow : Window
    {
        public ProjectEditorWindow() { InitializeComponent(); }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private async void OnPickProjectPhoto(object Sender, RoutedEventArgs E)
        {
            var Path = await PickImage();
            if (Path != null) (DataContext as ProjectEditorViewModel)?.SetProjectPhoto(Path);
        }

        private async void OnPickTargetPhoto(object Sender, RoutedEventArgs E)
        {
            var Path = await PickImage();
            if (Path != null) ((Sender as Button)?.DataContext as TargetEditVM)?.SetPhoto(Path);
        }

        private async Task<string> PickImage()
        {
            var Files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Выбрать изображение",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Изображения") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } }
                }
            });

            if ((Files == null) || (Files.Count == 0)) return null;

            return Files[0].TryGetLocalPath();
        }

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

        private async void OnControllers(object Sender, RoutedEventArgs E)
        {
            var Vm = DataContext as ProjectEditorViewModel;
            if (Vm == null) return;

            var W = new ControllersWindow { DataContext = new ControllersViewModel(Vm.CatalogPath) };
            await W.ShowDialog(this);

            Vm.ReloadControllers();
        }
    }
}
