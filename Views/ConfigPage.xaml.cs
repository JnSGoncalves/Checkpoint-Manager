using System.Windows;
using System.Windows.Controls;

namespace Checkpoint_Manager.Views {
    /// <summary>
    /// Interação lógica para ConfigPage.xam
    /// </summary>
    public partial class ConfigPage : Page {
        public ConfigPage() {
            InitializeComponent();
            AutoBackupView();
        }

        private void AutoBackup_Click(object sender, RoutedEventArgs e) {
            AutoBackupView();
        }

        private void AutoBackupView() {
            if (AutoBackup.IsChecked == true) {
                BackupConfigs.Visibility = Visibility.Visible;
            } else {
                BackupConfigs.Visibility = Visibility.Hidden;
            }
        }
    }
}
