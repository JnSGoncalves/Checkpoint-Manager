using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public MainViewModel() {
            Games = new ObservableCollection<Game>();
        }

        public void StartApp() {
            Games = FileManeger.FindGames();
            Console.WriteLine(Games.Count);
            FileManeger.StartConfigInfo();
        }

        public void AddGame(int id, String name, String path) {
            Games.Add(new Game(id, name, path));
            OnPropertyChanged(nameof(Games));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
