﻿using System.Collections.ObjectModel;

namespace Checkpoint_Manager.Models {
    internal class FileManeger {
        public static ObservableCollection<Game> FindGames() {
            ObservableCollection<Game> games = new ObservableCollection<Game>();
            ObservableCollection<Save> saves = new ObservableCollection<Save>();
            saves.Add(new Save(1 ,"Teste 1", "test desc", "01/01/1001"));
            saves.Add(new Save(2, "Teste 2", "test desc", "02/02/1002"));

            games.Add(new Game(IdGetter.CreateId("Game Test com Saves"), "Game Test com Saves", "C/Games/", saves, new GameBackupConfigs()));
            games.Add(new Game(IdGetter.CreateId("Game Test 2"), "Game Test 2", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 3"), "Game Test 3", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 4"), "Game Test 4", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 5"), "Game Test 5", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 6"), "Game Test 6", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 7"), "Game Test 7", "C/Games/"));
            games.Add(new Game(IdGetter.CreateId("Game Test 8"), "Game Test 8", "C/Games/"));

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