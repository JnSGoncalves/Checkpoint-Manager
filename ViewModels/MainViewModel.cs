using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.ViewModels {
     public class MainViewModel : INotifyPropertyChanged {
        
        private ObservableCollection<Game>? _games;
        public ObservableCollection<Game>? Games {
            get => _games;
            set {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        public MainViewModel() { }

        public void StartApp() {
            Games = FileManeger.FindGames();
            
            Debug.WriteLine($"Qtd de Jogos encontrados: {Games.Count}");
            FileManeger.StartConfigInfo();
        }

        public void AddGame(String name, String path) {
            if (Games != null) {
                Games.Add(new Game(IdGetter.CreateId(name), name, path));
                OnPropertyChanged(nameof(Games));
            }
        }

        public void DelGame(String id) {
            if(Games != null) {
                for (int i = 0; i < Games.Count; i++) {
                    if (Games[i].Id.Equals(id)) {
                        Games.RemoveAt(i);
                        OnPropertyChanged(nameof(Games));
                        break;
                    }
                }
            }

            // Adicionar o resto do código de remoção dos arquivos
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
