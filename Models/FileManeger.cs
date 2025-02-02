using System.Collections.ObjectModel;

namespace Checkpoint_Manager.Models {
    internal class FileManeger {
        public static ObservableCollection<Game> FindGames() {
            ObservableCollection<Game> games = new ObservableCollection<Game>();



            return games;
        }

        public static void StartConfigInfo() {
            ConfigInfo configInfo = ConfigInfo.GetConfigInfo();
            configInfo.ActualId = 0;
            configInfo.IsAutoSave = false;
            configInfo.AutoSaveTime = 0;
            configInfo.MaxSaves = 0;
        }
    }
}