using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Checkpoint_Manager.ViewModels {
    public class WaitWindowViewModel : INotifyPropertyChanged {
        private string _message;
        public string Message {
            get => _message;
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
        public WaitWindowViewModel(string msg) {
            _message = msg;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
