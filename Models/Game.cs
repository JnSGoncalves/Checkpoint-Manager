using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Checkpoint_Manager.Models {
    public class Game : INotifyPropertyChanged {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
        public bool IsSingleFileSave { get; set; }
        public bool ConfigsIsDefault { get; set; }
        public int AutoBackupQtd { get; set; } = 0;

        [JsonIgnore]
        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected {
            get => _isSelected;
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        [JsonIgnore]
        private bool _isActualAutoBackup = false;
        [JsonIgnore]
        public bool IsActualAutoBackup {
            get => _isActualAutoBackup;
            set {
                if (_isActualAutoBackup != value) {
                    _isActualAutoBackup = value;
                    OnPropertyChanged();
                }
            }
        }
        public GameBackupConfigs? GameBackupConfigs { get; set; }

        private ObservableCollection<Save> _saves = [];
        public ObservableCollection<Save> Saves {
            get => _saves;
            set {
                _saves = value;
                Debug.WriteLine("Qtd de save " + _saves.Count);
                OnPropertyChanged();
            }
        }

        public Game() {
            Saves.CollectionChanged += Saves_CollectionChanged;
        }

        public Game(String id, String name, String path, bool isSingleFileSave) {
            this.IsSelected = false;
            this.IsActualAutoBackup = false;
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.IsSingleFileSave = isSingleFileSave;
            this.ConfigsIsDefault = true;

            Saves.CollectionChanged += Saves_CollectionChanged;
        }

        public Game(String id, String name, String path, bool isSingleFileSave, bool configIsDefault, int autoBackupQtd) { 
            this.IsSelected = false;
            this.IsActualAutoBackup = false;
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.IsSingleFileSave = isSingleFileSave;
            this.ConfigsIsDefault = configIsDefault;
            this.AutoBackupQtd = autoBackupQtd;

            Saves.CollectionChanged += Saves_CollectionChanged;
        }

        private void Saves_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (Save newSave in e.NewItems) {
                    newSave.PropertyChanged += Save_PropertyChanged;
                }
            }

            if (e.OldItems != null) {
                foreach (Save oldSave in e.OldItems) {
                    oldSave.PropertyChanged -= Save_PropertyChanged;
                }
            }
        }

        private void Save_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Save.Name)) {
                var save = sender as Save;
                Rename(save);
            }
        }

        public void Rename(Save? save) {
            if (save == null)
                return;

            if (save.IsAutoBackup) {
                save.IsAutoBackup = false;
                AutoBackupQtd--;
            }
            Debug.WriteLine($"Save '{save.Name}' renomeado. AutoBackupQtd: {AutoBackupQtd}");
        }

        // A verificação de nome de save deve ser feita na coleta dele antes da chamada dessa função
        public bool NewSave(string name, string description) {
            string saveId = IdGetter.CreateId(name);
            var save = new Save(saveId, name, description, DateGetter.GetActualDate());
            try {
                FileManager.CopyNewSave(this, name);

                Saves.Insert(0, save);
                Debug.WriteLine($"Save {name} de id {saveId} criado");

                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine($"Erro ao criar o Save {name} de id {saveId}");
                return false;
            }
        }

        public bool NewSave() {
            string date = DateGetter.GetDateToName();
            string name = $"{Name} - {date}";
            string id = IdGetter.CreateId(name);
            var save = new Save(id, name, "", DateGetter.GetActualDate());

            try {
                FileManager.CopyNewSave(this, name);

                Saves.Insert(0, save);
                Debug.WriteLine($"Save {name} de id {id} criado");

                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine($"Erro ao criar o Save {name} de id {id}");
                return false;
            }
        }

        public bool NewSave(bool isAutoBackup) {
            string date = DateGetter.GetDateToName();
            string name = $"{Name} - {date}";
            string id = IdGetter.CreateId(name);
            var save = new Save(id, name, "", DateGetter.GetActualDate(), isAutoBackup);

            try {
                FileManager.CopyNewSave(this, name);

                Saves.Insert(0, save);
                Debug.WriteLine($"Save {name} de id {id} criado");
                if (isAutoBackup)
                    AutoBackupQtd += 1;

                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine($"Erro ao criar o Save {name} de id {id}");
                return false;
            }
        }

        public void DeleteSave(Save save) {
            if (save.IsAutoBackup) {
                this.AutoBackupQtd -= 1;
            }

            Saves.Remove(save);
        }

        public Save? GetLastAutoSave() {
            for (int i = Saves.Count - 1; i >= 0; i--) {
                if (Saves[i].IsAutoBackup) {
                    return Saves[i];
                }
            }
            return null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GameBackupConfigs { 
        public double AutoSaveTime { get; set; }
        public int MaxSaves { get; set; }

        public GameBackupConfigs(double autoSaveTime, int maxSaves) {
            AutoSaveTime = autoSaveTime;
            MaxSaves = maxSaves;
        }

        public GameBackupConfigs() { }
    }
}
