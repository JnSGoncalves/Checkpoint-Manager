using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class TopMenuViewModel {
        public IRelayCommand OpenConfigCommand { get; }
        
        public TopMenuViewModel() { 
            OpenConfigCommand = new RelayCommand(OpenConfig);
        }

        private void OpenConfig (){
            if (App.MainViewModelInstance.ConfigIsOpen == true)
                return;
            App.MainViewModelInstance.ResetOpenPages();
            App.MainViewModelInstance.ConfigIsOpen = true;
            Debug.WriteLine("Open Config");
        }
    }
}
