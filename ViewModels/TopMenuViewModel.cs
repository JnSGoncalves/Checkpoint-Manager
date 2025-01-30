using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class TopMenuViewModel {
        public IRelayCommand OpenConfigCommand { get; }
        
        public TopMenuViewModel() { 
            OpenConfigCommand = new RelayCommand(OpenConfig);
        }

        private void OpenConfig (){
            App.MainViewModelInstance.ResetOpenPages();
            App.MainViewModelInstance.ConfigIsOpen = true;
            Debug.WriteLine("Open Config");
        }
    }
}
