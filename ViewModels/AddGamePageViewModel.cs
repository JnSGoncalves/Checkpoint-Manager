using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class AddGamePageViewModel : INotifyPropertyChanged {
        private string? _gameName;
        public string? GameName {
            get => _gameName;
            set {
                _gameName = value;
                OnPropertyChanged();
            }
        }

        private string? _gamePath;
        public string? GamePath {
            get => _gamePath;
            set {
                _gamePath = value;
                OnPropertyChanged();
            }
        }

        private bool _isSingleFileSave;
        public bool IsSingleFileSave {
            get => _isSingleFileSave;
            set {
                _isSingleFileSave = value;
                OnPropertyChanged();
            }
        }

        public ICommand ClosePageCommand { get; }
        public ICommand AddGameCommand { get; }
        public ICommand SelectPathCommand { get; }

        public AddGamePageViewModel() {
            ClosePageCommand = new RelayCommand(ClosePage);
            AddGameCommand = new RelayCommand(AddGame);
            SelectPathCommand = new RelayCommand(SelectPath);
        }

        private void SelectPath() {
            string path = "";

            if (IsSingleFileSave) {
                var dialog = new OpenFileDialog {
                    Multiselect = false,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                };
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    path = dialog.FileName;
                }
            } else {
                var dialog = new FolderBrowserDialog {
                    SelectedPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Roaming"
                    )
                };
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    path = dialog.SelectedPath;
                }
            }

            if (!string.IsNullOrWhiteSpace(path)) {
                GamePath = path; // A propriedade GamePath será atualizada corretamente
                Debug.WriteLine($"Caminho selecionado {path}");
            }
        }

        private void ClosePage() {
            if (App.MainViewModelInstance.AddPageIsOpen) {
                Debug.WriteLine("Fechando aba AddGame");
                App.MainViewModelInstance.AddPageIsOpen = false;
            }
        }

        private void AddGame() {
            if (!string.IsNullOrEmpty(GameName) && !string.IsNullOrEmpty(GamePath)) {
                if (!PathValidator.IsSystemOrProtectedPath(GamePath)) {
                    string newGameId = IdGetter.CreateId(GameName);

                    foreach (Game _ in App.MainViewModelInstance.Games) {
                        if (_.Id.Equals(newGameId)) {
                            System.Windows.Forms.MessageBox.Show("Jogo já adicionado.\n\n" +
                                "Caso queira duplicar modifique o nome apresentado", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            return;
                        }
                    }

                    MessageBoxResult result = System.Windows.MessageBox.Show($"O caminho de save '{GamePath}' está correto?\n\n" +
                        "O programa realizará alterações no caminho selecionado, tenha certeza que as modificações constantes " +
                        "no caminho não causarão problemas ao sistema ou outros aplicativos", 
                        "Adicionar Jogo", MessageBoxButton.YesNo, MessageBoxImage.None);

                    if (result == MessageBoxResult.Yes) {
                        Game game = new Game(newGameId, GameName, GamePath, IsSingleFileSave);
                        App.MainViewModelInstance.Games.Add(game);

                        System.Windows.Forms.MessageBox.Show($"Jogo {GameName} adicionado!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Atualiza o arquivo que salva os dados dos jogos
                        FileManeger.AttArquives(App.MainViewModelInstance.Games);

                        App.MainViewModelInstance.ResetOpenPages();

                        GameName = null;
                        GamePath = null;
                        IsSingleFileSave = false;
                    }
                } else {
                    System.Windows.Forms.MessageBox.Show("O caminho apresentado não pode ser adicionado!", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else {
                System.Windows.Forms.MessageBox.Show("Preencha todos os campos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
