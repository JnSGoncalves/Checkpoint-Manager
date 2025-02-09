using System.Windows;

namespace Checkpoint_Manager.Services
{
    public static class DialogService {
        public static void ShowDialog<Window2Open>() where Window2Open : Window, new () {
            var window = new Window2Open();
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.ShowDialog();
        }
    }
}
