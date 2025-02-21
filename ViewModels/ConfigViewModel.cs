using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels
{
    public class ConfigViewModel : INotifyPropertyChanged {
        public ObservableCollection<string> CountryList { get; } = new ObservableCollection<string> {
            "en-US", // Estados Unidos
            "en-GB", // Reino Unido
            "fr-FR", // França
            "de-DE", // Alemanha
            "es-ES", // Espanha
            "it-IT", // Itália
            "pt-BR", // Brasil
            "zh-CN", // China
            "ja-JP", // Japão
            "ru-RU", // Rússia
            "ko-KR", // Coreia do Sul
            "ar-SA", // Arábia Saudita
            "hi-IN"  // Índia
        };

        private string? _selectedCountry;
        public string? SelectedCountry {
            get => _selectedCountry;
            set {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
            }
        }

        private int? _maxSpace;
        public int? MaxSpace {
            get => _maxSpace;
            set {
                _maxSpace = value;
                OnPropertyChanged(nameof(MaxSpace));
            }
        }

        private string? _savesPath;
        public string? SavesPath {
            get => _savesPath;
            set {
                _savesPath = value;
                OnPropertyChanged(nameof(SavesPath));
            }
        }

        private bool? _isAutoSave;
        public bool? IsAutoSave {
            get => _isAutoSave;
            set {
                _isAutoSave = value;
                OnPropertyChanged(nameof(IsAutoSave));
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

        public ICommand SelectPathCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand ClosePageCommand { get; }

        public ConfigViewModel() {
            SelectPathCommand = new RelayCommand(SelectPath);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            ClosePageCommand = new RelayCommand(ClosePage);

            MaxSpace = FileManager.Config.MaxSpace;
            SelectedCountry = FileManager.Config.CultureCountry;
            SavesPath = FileManager.Config.SavesPath;
            IsAutoSave = FileManager.Config.IsAutoSave;

            GetHourMinute(FileManager.Config.AutoSaveTime, 
                out int hour, out int min);
            AutoSaveHour = hour;
            AutoSaveMinute = min;

            MaxSaves = FileManager.Config.MaxSaves;
        }

        private void GetHourMinute(int? minutes, out int hours, out int mins) {
            if(minutes == null) {
                hours = 0;
                mins = 0;
            } else {
                hours = (int)(minutes / 60);
                mins = (int)(minutes % 60);
            }
        }

        private int GetTimeInMinute() {
            return (int)((AutoSaveHour.GetValueOrDefault() * 60) + AutoSaveMinute.GetValueOrDefault());
        }

        private void SaveConfig() {
            MessageBoxResult result = System.Windows.MessageBox.Show(
                "Deseja salvar as alterações na configuração?","Confirmar alterações", 
                MessageBoxButton.YesNo, MessageBoxImage.None);
            
            if (result == MessageBoxResult.Yes) {
                FileManager.Config.CultureCountry = SelectedCountry;
                FileManager.Config.MaxSpace = MaxSpace;
                FileManager.Config.AutoSaveTime = GetTimeInMinute();
                FileManager.Config.MaxSaves = MaxSaves;
                FileManager.Config.SavesPath = SavesPath;
                FileManager.Config.IsAutoSave = IsAutoSave;

                FileManager.AttConfig();
            }
        }

        private void SelectPath() {
            string path = "";

            if (FileManager.Config.SavesPath != null) {
                var dialog = new FolderBrowserDialog {
                    SelectedPath = FileManager.Config.SavesPath
                };

                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    path = dialog.SelectedPath;
                } else {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(path) &&
                        !PathValidator.IsSystemOrProtectedPath(path)) {
                    SavesPath = path;
                    Debug.WriteLine($"Caminho selecionado {path}");
                } else {
                    System.Windows.Forms.MessageBox.Show("Caminho inválido!",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ClosePage() {
            if (App.MainViewModelInstance.ConfigIsOpen) {
                Debug.WriteLine("Fechando aba Config");
                App.MainViewModelInstance.ConfigIsOpen = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
