using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Checkpoint_Manager.Models {
    public class Game : INotifyPropertyChanged {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
        public bool IsSingleFileSave { get; set; }
        public bool ConfigsIsDefault { get; set; }

        private bool _isSelected;
        public bool IsSelected {
            get => _isSelected;
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public GameBackupConfigs? gameBackupConfigs { get; set; }

        private ObservableCollection<Save> _saves = [];
        public ObservableCollection<Save> Saves {
            get => _saves;
            set {
                _saves = value;
                Debug.WriteLine("Qtd de save " + _saves.Count);
                OnPropertyChanged();
            }
        }

        public Game(String id, String name, String path, bool isSingleFileSave) { 
            this.IsSelected = false;
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.IsSingleFileSave = isSingleFileSave;
            this.ConfigsIsDefault = true;
        }

        public bool NewSave(string name, string description) {
            string saveId = IdGetter.CreateId(name);
            var save = new Save(saveId, name, description, DateGetter.GetActualDate());
            try {
                FileManeger.CopyNewSave(this, name);

                Saves.Add(save);
                Debug.WriteLine($"Save {name} de id {saveId} criado");

                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine($"Erro ao criar o Save {name} de id {saveId} criado");
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GameBackupConfigs { 
        public bool IsAutoSave { get; set; }
        public double AutoSaveTime { get; set; }
        public int MaxSaves { get; set; }

        public GameBackupConfigs(bool isAutoSave, double autoSaveTime, int maxSaves) {
            IsAutoSave = isAutoSave;
            AutoSaveTime = autoSaveTime;
            MaxSaves = maxSaves;
        }

        public GameBackupConfigs() {
        }
    }
}
