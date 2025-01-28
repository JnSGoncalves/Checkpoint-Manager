using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    internal class FileManeger {
        public static ObservableCollection<Game> FindGames() {
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            games.Add(new Game(1, "Game Test 1", "C/Games/"));
            games.Add(new Game(2, "Game Test 2", "C/Games/"));
            games.Add(new Game(3, "Game Test 3", "C/Games/"));
            games.Add(new Game(4, "Game Test 4", "C/Games/"));
            games.Add(new Game(5, "Game Test 5", "C/Games/"));
            games.Add(new Game(6, "Game Test 6", "C/Games/"));

            Console.WriteLine(games.ElementAt(1).Name);

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