using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.ViewModels {
    internal class MainViewModel {
        public List<Game> Games { get; set; }

        public void StartApp() {
            Games = FileManeger.FindGames();

                
        }
    }
}
