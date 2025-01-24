using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    internal class GameSave {
        public String name { get; set; }
        public String path { get; set; }

        public GameSave(String name, String path) { 
            this.name = name;
            this.path = path;
        }
    }
}
