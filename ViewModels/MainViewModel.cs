using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        // Instacia das ViewModels de cada Page
        public SidePageViewModel SidePageVM { get; set; }
        
        // Lista dos Jogos cadastrados
        private ObservableCollection<Game>? _games;
        public ObservableCollection<Game>? Games {
            get => _games;
            set {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        // Game selecionado atualmente
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
        
        public MainViewModel() {
            //AddGameCommand = new RelayCommand(AddGame);
            SidePageVM = new SidePageViewModel();
        }

        public void StartApp() {
            Games = FileManeger.FindGames();
            
            Debug.WriteLine($"Qtd de Jogos encontrados: {Games.Count}");
            FileManeger.StartConfigInfo();
        }


        // Declaração do Evento de modificação de propriedade
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
