using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.Views;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        // Instacia das ViewModels de cada Page
        public SidePageViewModel SidePageVM { get; }
        public TopMenuViewModel TopMenuVM { get; }
        public SavesPageViewModel SavesPageVM {  get; }
        public AddGamePageViewModel AddGamePageVM { get; }
        
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
                    if (_selectedGame != null) {
                        _selectedGame.IsSelected = true;
                        OnPropertyChanged(nameof(SelectedGame.Saves));
                    }

                    OnPropertyChanged();
                }
            }
        }

        // Aba de Config está aberta?
        private bool _configIsOpen;
        public bool ConfigIsOpen {
            get => _configIsOpen;
            set {
                _configIsOpen = value;
                OnPropertyChanged();
            }
        }

        // Aba de Adição de Jogos está aberta?
        private bool _addPageIsOpen;
        public bool AddPageIsOpen {
            get => _addPageIsOpen;
            set {
                _addPageIsOpen = value;
                OnPropertyChanged();
            }
        }

        // Aba de configuração de Jogos está aberta?
        private bool _gameConfigIsOpen;
        public bool GameConfigIsOpen {
            get => _gameConfigIsOpen;
            set {
                _gameConfigIsOpen = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() {
            SidePageVM = new SidePageViewModel();
            TopMenuVM = new TopMenuViewModel();
            SavesPageVM = new SavesPageViewModel();
            AddGamePageVM = new AddGamePageViewModel();
        }

        public void StartApp() {
            ConfigInfo Config = FileManeger.StartConfigInfo();

            Games = FileManeger.FindGames();

            int contagem = 1;
            foreach (Game games in Games) {
                if (games.NewSave($"teste {contagem}", $"descricao {contagem}")) {
                    FileManeger.AttArquives(Games);
                }
                contagem++;
            }

            Debug.WriteLine($"Qtd de Jogos encontrados: {Games.Count}");
        }

        public void ResetOpenPages() {
            GameConfigIsOpen = false;
            AddPageIsOpen = false;
            ConfigIsOpen = false;
        }

        // Declaração do Evento de modificação de propriedade
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Converter do MainContent
    public class MultiPageConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

            // Ordem de prioridade (definida no parâmetro do XAML)
            var priority = (parameter as string)?.Split(',') ?? new[] { "Config", "GameConfig", "Add", "Game" };

            foreach (var pageType in priority) {
                switch (pageType.Trim()) {
                    case "Config" when values[1] is bool configOpen && configOpen:
                        Debug.WriteLine("Open Config ev");
                        return new ConfigPage();

                    case "GameConfig" when values[3] is bool gameConfigOpen && gameConfigOpen
                                          && values[0] is Game selectedGame && selectedGame != null:
                        Debug.WriteLine("Open Game Config ev");
                        return new GameConfigPage { DataContext = selectedGame }; // Exige jogo selecionado;

                    case "Add" when values[2] is bool addOpen && addOpen:
                        Debug.WriteLine("Open Add ev");
                        return new AddGamePage();

                    case "Game" when values[0] is Game game && game != null:
                        Debug.WriteLine("Open Save ev");
                        return new SavesPage { DataContext = game };
                }
            }

            return DefaultPage.GetDefaultPage();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
