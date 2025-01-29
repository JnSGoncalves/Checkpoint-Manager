using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        
        private ObservableCollection<Game>? _games;
        public ObservableCollection<Game>? Games {
            get => _games;
            set {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        private Game? _selectedGame;
        public Game? SelectedGame { get => _selectedGame; 
            set {
                if (_selectedGame != value) {
                    // Desseleciona o jogo anterior
                    if (_selectedGame != null)
                        _selectedGame.IsSelected = false;

                    _selectedGame = value;

                    // Seleciona o novo jogo
                    if (_selectedGame != null)
                        _selectedGame.IsSelected = true;

                    OnPropertyChanged();
                }
            }
        }

        //public ICommand AddGameCommand { get; }
        public ICommand RemoveGameCommand { get; }
        public ICommand SelectGameCommand { get; }

        public MainViewModel() {
            Games = FileManeger.FindGames();

            //AddGameCommand = new RelayCommand(AddGame);
            RemoveGameCommand = new RelayCommand(RemoveGame);

            #pragma warning disable CS8622
            SelectGameCommand = new RelayCommand<Game>(SelectGame);
            #pragma warning restore CS8622
        }

        public void StartApp() {
            Games = FileManeger.FindGames();
            
            Debug.WriteLine($"Qtd de Jogos encontrados: {Games.Count}");
            FileManeger.StartConfigInfo();
        }

        private void SelectGame(Game game) {
            if (SelectedGame ==  game ) {
                SelectedGame = null;
                Debug.WriteLine("Nenhum game selecionado");
                return;
            } else {
                SelectedGame = game;
            }
            Debug.WriteLine($"Selected game: {SelectedGame.Name}");
        }

        private void RemoveGame() {
            if (SelectedGame == null) {
                MessageBox.Show("Nenhum jogo selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else if (Games != null) {
                MessageBoxResult result = MessageBox.Show("Deseja realmente remover " + SelectedGame.Name+
                " da lista?\nEssa ação não pode ser desfeita.", "Remover Jogo", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK) {
                    Debug.WriteLine($"Jogo {SelectedGame.Name} de id: {SelectedGame.Id} foi excluido");

                    Games.Remove(SelectedGame);
                    SelectedGame = null;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
