using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.Views {
    public partial class SavesPage : Page {
        public SavesPage() {
            InitializeComponent();
        }

        public void DelSave(object sender, RoutedEventArgs e) {
            if(sender != null) {
                if(sender is System.Windows.Controls.Button button && button.DataContext is Save save) {
                    Debug.WriteLine($"Chamada do del Save para {save.Name}");
                    App.MainViewModelInstance.SavesPageVM.DelSave(save);
                }
            } else {
                Debug.WriteLine("Save não é DataContext");
            }
        }

        private void SwapSave(object sender, RoutedEventArgs e) {

        }
    }
}
