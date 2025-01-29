using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    public class Game {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Path { get; set; }
        public bool ConfigsIsDefault { get; set; }
        public GameBackupConfigs gameBackupConfigs { get; set; }
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
