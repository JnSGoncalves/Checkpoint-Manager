using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class GameConfigViewModel : INotifyPropertyChanged {
        private Game _actualGame;
        public Game ActualGame {
            get => _actualGame;
            set {
                _actualGame = value;
                OnPropertyChanged(nameof(ActualGame));
            }
        }

        private bool? _configIsDefault;
        public bool? ConfigIsDefault {
            get => _configIsDefault;
            set {
                _configIsDefault = value;
                OnPropertyChanged(nameof(ConfigIsDefault));
            }
        }

        private int? _autoSaveMinute;
        public int? AutoSaveMinute {
            get => _autoSaveMinute;
            set {
                _autoSaveMinute = value;
                OnPropertyChanged(nameof(AutoSaveMinute));
            }
        }

        private int? _autoSaveHour;
        public int? AutoSaveHour {
            get => _autoSaveHour;
            set {
                _autoSaveHour = value;
                OnPropertyChanged(nameof(AutoSaveHour));
            }
        }

        private int? _maxSaves;
        public int? MaxSaves {
            get => _maxSaves;
            set {
                _maxSaves = value;
                OnPropertyChanged(nameof(MaxSaves));
            }
        }

        public RelayCommand ClosePageCommand { get; }
        public RelayCommand SaveConfigCommand { get; }

        public GameConfigViewModel(Game game) {
            ClosePageCommand = new RelayCommand(ClosePage);
            SaveConfigCommand = new RelayCommand(SaveConfig);

            ActualGame = game;

            ConfigIsDefault = ActualGame.ConfigsIsDefault;

            if (ActualGame.GameBackupConfigs != null) {
                MaxSaves = ActualGame.GameBackupConfigs.MaxSaves;

                HourGetter.GetHourMinute(ActualGame.GameBackupConfigs.AutoSaveTime,
                out int hour, out int min);
                AutoSaveHour = hour;
                AutoSaveMinute = min;
            } else {
                HourGetter.GetHourMinute(FileManager.Config.AutoSaveTime,
                out int hour, out int min);
                AutoSaveHour = hour;
                AutoSaveMinute = min;

                MaxSaves = FileManager.Config.MaxSaves;
            }
        }

        private void SaveConfig() {
            if (AutoSaveHour == 0 && AutoSaveMinute < 5) {
                System.Windows.MessageBox.Show(
                "O tempo minimo para os backup automáticos é de 5 minutos" +
                "\n\nCaso não utilize essa função, o tempo colocado não interferirá" +
                " nas outras funcionalidades da aplicação", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = System.Windows.MessageBox.Show(
                "Deseja salvar as alterações na configuração?", "Confirmar alterações",
                MessageBoxButton.YesNo, MessageBoxImage.None);

            if (result != MessageBoxResult.Yes) {
                return;
            }

            if(ActualGame.GameBackupConfigs == null) {
                ActualGame.GameBackupConfigs = new GameBackupConfigs();
            }

            ActualGame.ConfigsIsDefault = ConfigIsDefault.GetValueOrDefault();
            ActualGame.GameBackupConfigs.AutoSaveTime = HourGetter.GetTimeInMinute(AutoSaveHour, AutoSaveMinute);
            ActualGame.GameBackupConfigs.MaxSaves = MaxSaves.GetValueOrDefault();

            FileManager.AttArquives(App.MainViewModelInstance.Games);

            Debug.WriteLine($"Configuração do jogo {ActualGame.Name} salva");
        }

        private void ClosePage() {
            App.MainViewModelInstance.GameConfigIsOpen = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
