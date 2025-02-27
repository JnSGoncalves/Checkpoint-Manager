using System.Diagnostics;
using System.IO;
using System.Windows;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.Services;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class TopMenuViewModel {
        public IRelayCommand OpenConfigCommand { get; }
        public IRelayCommand ExportCommand { get; }

        public TopMenuViewModel() { 
            OpenConfigCommand = new RelayCommand(OpenConfig);
            ExportCommand = new RelayCommand(async () => await ExportAsync());
        }

        private void OpenConfig (){
            if (App.MainViewModelInstance.ConfigIsOpen == true)
                return;
            App.MainViewModelInstance.ResetOpenPages();
            App.MainViewModelInstance.ConfigIsOpen = true;
            Debug.WriteLine("Open Config");
        }

        private async Task ExportAsync() {
            var dialog = new System.Windows.Forms.SaveFileDialog() {
                Title = "Export",
                InitialDirectory = "C:\\Users\\jnsil\\Documents\\Checkpoint Manager",
                FileName = $"Saves - {DateGetter.GetDateToName()}",
                Filter = "Export file (*.zip)|*.zip",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string path = dialog.FileName;
                string savesPath = FileManager.Config.SavesPath;

                var waitWindow = DialogService.ShowWaitDialog("Exportando Arquivo");

                try {
                    await Task.Run(() => {
                        #pragma warning disable CS8604
                        if (!FileManager.CompactFolder(path, savesPath)) {
                            throw new Exception("Erro ao tentar exportar os Checkpoints");
                        } else {
                            Debug.WriteLine("Exportado");
                        }
                        #pragma warning restore CS8604
                    });
                } catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.Message, "Erro",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                } finally {
                    waitWindow.Close();
                }
            }
        }
    }
}
