using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    internal class Game {
        public String Name { get; set; }
        public String Path { get; set; }
        public List<Save> Saves { get; set; }
        public Game(String name, String path) { 
            this.Name = name;
            this.Path = path;
        }
        public Game(String name, String path, List<Save> saves) {
            this.Name = name;
            this.Path = path;
            this.Saves = saves;
        }
    }
}
