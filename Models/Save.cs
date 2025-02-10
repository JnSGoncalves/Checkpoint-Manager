using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    public class Save {
        public string? Id { get; set; }
        public string Name { get; set; }
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
    }
}
