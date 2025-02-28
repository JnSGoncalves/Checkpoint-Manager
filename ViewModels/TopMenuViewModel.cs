using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class TopMenuViewModel {
        public IRelayCommand OpenConfigCommand { get; }
        public IRelayCommand ExportCommand { get; }
        public IRelayCommand ImportCommand { get; }

        public TopMenuViewModel() { 
            OpenConfigCommand = new RelayCommand(OpenConfig);
            ExportCommand = new RelayCommand(async () => await ExportAsync());
            ImportCommand = new RelayCommand(async () => await ImportAsync());
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

                var waitWindow = DialogService.ShowWaitDialog("Exportando Checkpoints");

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

        private async Task ImportAsync() {
            var dialog = new System.Windows.Forms.OpenFileDialog() {
                Title = "Import",
                InitialDirectory = FileManager.Config.SavesPath,
                Filter = "Import file (*.zip)|*.zip",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string path = dialog.FileName;
                string savesPath = FileManager.Config.SavesPath;

                var waitWindow = DialogService.ShowWaitDialog("Importando Checkpoints");

                if (FileManager.IsImportFile(path)) {
                    try {
                        await Task.Run(() => {
                            string tempFolder = Path.Combine(Path.GetTempPath(), "checkpoint_manager_temp");

                            FileManager.DescompactZip(path, tempFolder);

                            List<Game> newGames = FileManager.FindGames(tempFolder) ?? throw new Exception("Erro ao Importar arquivo");

                            if (newGames.Count == 0) {
                                throw new Exception("Nenhum jogo encontrado no arquivo de importação");
                            }

                            if (App.MainViewModelInstance.Games == null)
                                throw new Exception("Erro");

                            if(App.MainViewModelInstance.Games.Count == 0) {
                                FileManager.Copy(new DirectoryInfo(tempFolder), new DirectoryInfo(savesPath));
                                App.MainViewModelInstance.Games = new ObservableCollection<Game>(newGames);

                                Debug.WriteLine("Importado com sucesso");

                                FileManager.AttArquives(App.MainViewModelInstance.Games);
                                return;
                            }

                            List<Game> duplicated = new List<Game>();
                            foreach (var item in newGames) {
                                if (App.MainViewModelInstance.Games.Contains(item)) {
                                    duplicated.Add(item);
                                }
                            }
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
}
