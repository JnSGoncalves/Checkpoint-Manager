using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    public class Save : INotifyPropertyChanged {
        public string? Id { get; set; }
        private string _name;
        public string Name { 
            get => _name; 
            set{
                if(_name != null) {
                    FileManager.RenameSave(this, value);
                }
                
                _name = value;

                OnPropertyChanged(nameof(Name));
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
        public Boolean? IsFavorite { get; set; }

        public Save(string id, string name, string description, string date) {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Date = date;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
