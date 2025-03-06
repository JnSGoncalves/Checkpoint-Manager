using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Checkpoint_Manager.Models {
    public class Save : INotifyPropertyChanged {
        public string? Id { get; set; }
        private string _name;
        public string Name { 
            get => _name; 
            set{
                if(_name != null && !_name.Equals(value)) {
                    Debug.WriteLine("Renomeado");
                    FileManager.RenameSave(this, value);
                }
                
                _name = value;

                OnPropertyChanged();
            } 
        }
        private string _description;
        public string? Description {
            get => _description;
            set {
                if (String.IsNullOrEmpty(value))
                    _description = "Description: Null";
                else
                    _description = "Description: " + value;
            }
        }
        public string? Date { get; set; }
        public bool? IsFavorite { get; set; }
        public bool IsAutoBackup { get; set; }

        public Save() { }

        public Save(string id, string name, string description, string date) {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Date = date;
            this.IsAutoBackup = false;
        }

        public Save(string id, string name, string description, string date, bool isAutoBackup) {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Date = date;
            this.IsAutoBackup = isAutoBackup;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
