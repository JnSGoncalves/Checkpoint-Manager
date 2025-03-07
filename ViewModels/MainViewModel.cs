using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Timers;
using System.Windows.Input;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.Services;
using Checkpoint_Manager.Views;
using CommunityToolkit.Mvvm.Input;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        // Instacia das ViewModels de cada Page
        public SidePageViewModel SidePageVM { get; }
        public TopMenuViewModel TopMenuVM { get; }
        public SavesPageViewModel SavesPageVM {  get; }
        public AddGamePageViewModel AddGamePageVM { get; }
        public DawnBarViewModel DownBarVM { get; }

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

        // Gerenciamento dos backups automáticos
        private readonly Timer _timer;
        private Game? _selectedBackupGame;
        public Game? SelectedBackupGame {
            get => _selectedBackupGame;
            set {
                // Desseleciona o jogo anterior
                if (_selectedBackupGame != null)
                    _selectedBackupGame.IsActualAutoBackup = false;

                _selectedBackupGame = value;

                // Seleciona o novo jogo
                if (_selectedBackupGame != null) {
                    _selectedBackupGame.IsActualAutoBackup = true;
                    OnPropertyChanged(nameof(SelectedGame.Saves));
                }
                
                ConfigureTimer();
            }
        }


        // Commands
        public ICommand ShowWindowCommand { get; }
        public ICommand ExitCommand { get; }
        public MainViewModel() {
            SidePageVM = new SidePageViewModel();
            TopMenuVM = new TopMenuViewModel();
            SavesPageVM = new SavesPageViewModel();
            AddGamePageVM = new AddGamePageViewModel();
            DownBarVM = new DawnBarViewModel();

            ShowWindowCommand = new RelayCommand(ShowMainWindow);
            ExitCommand = new RelayCommand(ExitApplication);

            NotifyIconService.Instance.SetCommands(ShowWindowCommand, ExitCommand);

            _timer = new Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        public void ShowMainWindow() {
            App.Current.Dispatcher.Invoke(() => {
                var mainWindow = App.Current.MainWindow;
                if (mainWindow == null)
                    return;
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            });
        }

        public void ExitApplication() {
            NotifyIconService.Instance.SetNotifyIconVisibility(false);
            App.Current.Shutdown();
        }

        public void StartApp() {
            SelectedGame = null;

            FileManager.StartConfigInfo();

            Games = FileManager.FindGames();

            DownBarVM.GetSpaces();
        }

        public void ResetOpenPages() {
            GameConfigIsOpen = false;
            AddPageIsOpen = false;
            ConfigIsOpen = false;
        }

        //private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
        //    if (SelectedBackupGame != null) {
        //        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            try {
        //                if (FileManager.IsFull) {
        //                    if (SavesPageVM.DeleteLastAutoSave(SelectedBackupGame)) {
        //                        Debug.WriteLine("Último backup automático excluido. " +
        //                            "Limite de espaço ocupado atingido.");
        //                    } else {
        //                        throw new Exception("Erro ao tentar excluir o Checkpoint automático mais antigo. " +
        //                            "Case: Limite de espaço ocupado atingido.");
        //                    }
        //                }else if (SelectedBackupGame.ConfigsIsDefault && FileManager.Config.MaxSaves != 0) {
        //                    if (SelectedBackupGame.AutoBackupQtd >= FileManager.Config.MaxSaves) {
        //                        if (SavesPageVM.DeleteLastAutoSave(SelectedBackupGame)) {
        //                            Debug.WriteLine("Último backup automático excluido. " +
        //                                "Limite geral de Checkpoints automáticos atingido.");
        //                        } else {
        //                            throw new Exception("Erro ao tentar excluir o Checkpoint automático mais antigo. " +
        //                                "Case: Limite geral de Checkpoints automáticos atingido.");
        //                        }
        //                    }
        //                } else if (SelectedBackupGame.GameBackupConfigs != null && SelectedBackupGame.GameBackupConfigs.MaxSaves != 0) {
        //                    if (SelectedBackupGame.AutoBackupQtd >= SelectedBackupGame.GameBackupConfigs.MaxSaves) {
        //                        if (SavesPageVM.DeleteLastAutoSave(SelectedBackupGame)) {
        //                            Debug.WriteLine("Último backup automático excluido. " +
        //                                "Limite espcífico de Checkpoints automáticos atingido.");
        //                        } else {
        //                            throw new Exception("Erro ao tentar excluir o Checkpoint automático mais antigo. " +
        //                                "Case: Limite espcífico de Checkpoints automáticos atingido.");
        //                        }
        //                    }
        //                }
        //            }catch (Exception e) {
        //                Debug.WriteLine(e.Message);
        //            }

        //            if (!SelectedBackupGame.NewSave(true)) {
        //                Debug.WriteLine("Erro ao criar um backup automático");
        //            } else {
        //                FileManager.AttArquives(App.MainViewModelInstance.Games);
        //                Debug.WriteLine($"Backup automático feito.");
        //            }
        //        });
        //    }
        //}

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            if (SelectedBackupGame == null)
                return;

            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                try {
                    BackupDelete();
                    CreateNewBackup();
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
            });
        }

        private void BackupDelete() {
            if (FileManager.IsFull) {
                DeleteOldestAutoSave("Limite de espaço ocupado atingido.");
            } else if (SelectedBackupGame.ConfigsIsDefault && FileManager.Config.MaxSaves != 0) {
                if (SelectedBackupGame.AutoBackupQtd >= FileManager.Config.MaxSaves) {
                    DeleteOldestAutoSave("Limite geral de Checkpoints automáticos atingido.");
                }
            } else if (SelectedBackupGame.GameBackupConfigs != null && SelectedBackupGame.GameBackupConfigs.MaxSaves != 0) {
                if (SelectedBackupGame.AutoBackupQtd >= SelectedBackupGame.GameBackupConfigs.MaxSaves) {
                    DeleteOldestAutoSave("Limite específico de Checkpoints automáticos atingido.");
                }
            }
        }

        private void DeleteOldestAutoSave(string message) {
            if (SavesPageVM.DeleteLastAutoSave(SelectedBackupGame)) {
                Debug.WriteLine($"Último backup automático excluído. {message}");
            } else {
                throw new Exception($"Erro ao tentar excluir o Checkpoint automático mais antigo. Case: {message}");
            }
        }

        private void CreateNewBackup() {
            if (SelectedBackupGame.NewSave(true)) {
                FileManager.AttArquives(App.MainViewModelInstance.Games);
                Debug.WriteLine("Backup automático feito.");
            } else {
                Debug.WriteLine("Erro ao criar um backup automático");
            }
        }


        private void ConfigureTimer() {
            if (SelectedBackupGame == null) {
                _timer.Stop();
            } else {
                if (SelectedBackupGame.ConfigsIsDefault && FileManager.Config.AutoSaveTime != null &&
                    FileManager.Config.AutoSaveTime != 0) {
                    int time = (int)FileManager.Config.AutoSaveTime;
                    _timer.Interval = TimeSpan.FromMinutes(time).TotalMilliseconds;
                    _timer.Start();
                } else if (SelectedBackupGame.GameBackupConfigs != null) {
                    int time = (int)SelectedBackupGame.GameBackupConfigs.AutoSaveTime;
                    _timer.Interval = TimeSpan.FromMinutes(time).TotalMilliseconds;
                    _timer.Start();
                }
            }
        }

        // Atualização do tempo somente se o tempo das configurações for modificado e o jogo ainda é default
        public void ConfigureTimer(bool attByConfig) {
            if (SelectedBackupGame == null) {
                _timer.Stop();
            } else {
                if (attByConfig) {
                    if (SelectedBackupGame.ConfigsIsDefault && FileManager.Config.AutoSaveTime != null &&
                        FileManager.Config.AutoSaveTime != 0) {
                        int time = (int)FileManager.Config.AutoSaveTime;
                        _timer.Interval = TimeSpan.FromMinutes(time).TotalMilliseconds;
                        _timer.Start();
                    }
                }
            }
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
                        return new ConfigPage() { DataContext = new ConfigViewModel() };

                    case "GameConfig" when values[3] is bool gameConfigOpen && gameConfigOpen
                                          && values[0] is Game selectedGame && selectedGame != null:
                        return new GameConfigPage { DataContext = new GameConfigViewModel(selectedGame) }; // Exige jogo selecionado;

                    case "Add" when values[2] is bool addOpen && addOpen:
                        return new AddGamePage();

                    case "Game" when values[0] is Game game && game != null:
                        return new SavesPage() { DataContext = game };
                }
            }

            return DefaultPage.GetDefaultPage();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
