using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    public class Save {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
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
