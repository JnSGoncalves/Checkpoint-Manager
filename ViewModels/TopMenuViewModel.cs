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

        private void OpenConfig() {
            if (App.MainViewModelInstance.ConfigIsOpen == true)
                return;
            App.MainViewModelInstance.ResetOpenPages();
            App.MainViewModelInstance.ConfigIsOpen = true;
            Debug.WriteLine("Open Config");
        }

        private async Task ExportAsync() {
            var dialog = new System.Windows.Forms.SaveFileDialog() {
                Title = "Export",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Import file (*.zip)|*.zip",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string path = dialog.FileName;
                string savesPath = FileManager.Config.SavesPath;

                var waitWindow = DialogService.ShowWaitDialog("Importando Checkpoints");

                string tempFolder = Path.Combine(Path.GetTempPath(), "checkpoint_manager_temp");

                if (FileManager.IsImportFile(path)) {
                    try {
                        await Task.Run(() => {

                            FileManager.DescompactZip(path, tempFolder);

                            List<Game> newGames = FileManager.FindGames(tempFolder) ?? throw new Exception("Erro ao Importar arquivo");

                            if (newGames.Count == 0) {
                                throw new Exception("Nenhum jogo encontrado no arquivo de importação");
                            }

                            if (App.MainViewModelInstance.Games == null)
                                throw new Exception("Erro");

                            // Caso do import ser totalmente novo
                            if (App.MainViewModelInstance.Games.Count == 0) {
                                FileManager.Copy(new DirectoryInfo(tempFolder), new DirectoryInfo(savesPath));
                                App.MainViewModelInstance.Games = new ObservableCollection<Game>(newGames);

                                Debug.WriteLine("Importado com sucesso");

                                FileManager.AttArquives(App.MainViewModelInstance.Games);
                                return;
                            }

                            // Adiciona todos os jogos, modificando renomeando os que já estavam cadastrados
                            foreach (var item in newGames) {
                                bool encontrouDuplicado;

                                do {
                                    encontrouDuplicado = false;

                                    foreach (var actGame in App.MainViewModelInstance.Games) {
                                        if (actGame.Id.Equals(item.Id)) {
                                            encontrouDuplicado = true;
                                            Debug.WriteLine("");

                                            int n = 1;
                                            int? oldN = null;
                                            if (item.Name.EndsWith(")")) {
                                                oldN = NameValidator.ExtrairNumeroFinal(item.Name);

                                                if (oldN != null) {
                                                    n = (int)(n + oldN);
                                                }
                                            }

                                            string newName;
                                            if (oldN != null)
                                                #pragma warning disable CS8602
                                                newName = string.Concat(item.Name.AsSpan(0, item.Name.Length - (oldN.ToString().Length + 2)), $"({n})");
                                                #pragma warning restore CS8602
                                            else if (item.Name.Length > (80 - (n.ToString().Length + 2)))
                                                newName = string.Concat(item.Name.AsSpan(0, item.Name.Length - (n.ToString().Length + 2)), $"({n})");
                                            else
                                                newName = item.Name + $"({n})";

                                            FileManager.RenameFolder(Path.Combine(tempFolder, item.Name), newName);

                                            item.Name = newName;
                                            item.Id = IdGetter.CreateId(newName);
                                            break;
                                        }
                                    }
                                } while (encontrouDuplicado);

                                string newGamePath = Path.Combine(tempFolder, item.Name);
                                if (!FileManager.Copy(
                                    new DirectoryInfo(newGamePath),
                                    new DirectoryInfo(Path.Combine(savesPath, item.Name))
                                    )
                                ) {
                                    throw new Exception("Erro ao importar os arquivos");
                                }
                                ;

                                App.Current.Dispatcher.Invoke(() => {
                                    App.MainViewModelInstance.Games.Add(item);
                                });
                            }

                            FileManager.AttArquives(App.MainViewModelInstance.Games);
                        });
                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show(ex.Message, "Erro",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                    } finally {
                        Directory.Delete(tempFolder, true);
                        waitWindow.Close();
                    }
                }
            }
        }
    }
}
