using System.Windows;
using Checkpoint_Manager.ViewModels;
using Checkpoint_Manager.Views;

namespace Checkpoint_Manager.Services
{
    public static class DialogService {
        public static void ShowDialog<Window2Open>() where Window2Open : Window, new () {
            var window = new Window2Open();
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static WaitWindow ShowWaitDialog(string msg) {
            WaitWindow window = new WaitWindow();
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.DataContext = new WaitWindowViewModel(msg);
            window.Show();

            return window;
        }
    }
}
