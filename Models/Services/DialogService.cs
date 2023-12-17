using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BCSH2SemestralniPraceCermakPetr.ViewModels;
using BCSH2SemestralniPraceCermakPetr.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models.Services
{
    public class DialogService
    {
        public async Task<string> ShowOpenFileDialog(string title)
        {
            var path = string.Empty;
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var extensions = new List<string> { "jpg", "jpeg", "png", "gif", "bmp", "tiff" };
                var filters = new List<FileDialogFilter>() {
                    new FileDialogFilter { Name = "Obrázky", Extensions = extensions }
                };
                var dialog = new OpenFileDialog
                {
                    Title = title,
                    Filters = filters,
                    AllowMultiple = false,
                };

                var results = await dialog.ShowAsync(desktop.MainWindow);
                path = results.FirstOrDefault() ?? path;
            }
            return path;
        }
        public async Task ShowEditWindow<T>(T viewModel) where T : ViewModelBase
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var editWindow = new EditWindow
                {
                    DataContext = viewModel,
                };

                await editWindow.ShowDialog(desktop.MainWindow);
            }
        }

    }
}
