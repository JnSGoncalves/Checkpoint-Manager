using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    public class Game : INotifyPropertyChanged {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
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
        public ObservableCollection<Save> Saves { get; set; }

        public Game(String id, String name, String path) { 
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.ConfigsIsDefault = true;
            this.Saves = new ObservableCollection<Save>();
        }

        public Game(String id, String name, String path, GameBackupConfigs gameBackupConfigs) {
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.ConfigsIsDefault = false;
            this.Saves = new ObservableCollection<Save>();
        }

        public Game(String id, String name, String path, ObservableCollection<Save> saves) {
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.ConfigsIsDefault = true;
            this.Saves = saves;
        }

        public Game(String id, String name, String path, ObservableCollection<Save> saves, GameBackupConfigs gameBackupConfigs) {
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.Saves = saves;
            this.gameBackupConfigs = gameBackupConfigs;
            this.ConfigsIsDefault = false;
        }

        public void newSave() {
            Saves.Add(new Save());
        }

        public void newSave(Save save) {
            Saves.Add(save);
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
    }
}
