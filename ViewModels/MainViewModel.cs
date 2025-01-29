using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        
        private ObservableCollection<Game> _games;
        public ObservableCollection<Game> Games {
            get => _games;
            set {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        public MainViewModel() { }

        public void StartApp() {
            Games = FileManeger.FindGames();
            Console.WriteLine("1");
            Console.WriteLine(Games.Count);
            FileManeger.StartConfigInfo();
        }

        public void AddGame(int id, String name, String path) {
            Games.Add(new Game(id, name, path));
            OnPropertyChanged(nameof(Games));
        }

        public void DelGame(int id) {
            for (int i = 0; i < Games.Count; i++) {
                if (Games[i].Id == id) {
                    Games.RemoveAt(i);
                    OnPropertyChanged(nameof(Games));
                    break;
                }
            }

            // Adicionar o resto do código de remoção dos arquivos
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
