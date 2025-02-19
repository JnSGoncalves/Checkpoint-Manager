using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.ViewModels {
    public class DawnBarViewModel : INotifyPropertyChanged {
        // Espaço atualmente ocupado pelos saves
        private string? _actualSpace;
        public string? ActualSpace {
            get => _actualSpace;
            set {
                _actualSpace = value;
                OnPropertyChanged(nameof(ActualSpace));
            }
        }
        // Espaço máximo ocupado pelos saves
        private string? _maxSpace;
        public string? MaxSpace {
            get => _maxSpace;
            set {
                _maxSpace = value;
                OnPropertyChanged(nameof(MaxSpace));
            }
        }
        // Espaço ocupado pelos saves em porcentagem
        private int? _percentSpace;
        public int? PercentSpace {
            get => _percentSpace;
            set {
                _percentSpace = value;
                OnPropertyChanged(nameof(PercentSpace));
            }
        }

        public DawnBarViewModel() { }

        public void GetSpaces() {
            Debug.WriteLine("Pegando tamanho");
            int bytesActualSpace = FileManager.GetUsedSpace();

            ActualSpace = FileManager.BytesToString(bytesActualSpace);

            int bytesMaxSpace = FileManager.Config.MaxSpace; // Na config é definido em MB

            double percent = 0;

            if (bytesMaxSpace == 0) {
                MaxSpace = ActualSpace;

                percent = 0;
            } else {
                bytesMaxSpace *= 1024; // Converção para Kb
                bytesActualSpace /= 1024;

                percent = (bytesActualSpace * 100) / bytesMaxSpace;

                bytesMaxSpace *= 1024; // Conversão para Bytes
                MaxSpace = FileManager.BytesToString(bytesMaxSpace);
            }


            if ((int)Math.Ceiling(percent) >= 100) {
                PercentSpace = 100;
                FileManager.IsFull = true;
            } else {
                PercentSpace = (int)Math.Ceiling(percent);
                FileManager.IsFull = false;
            }

            OnPropertyChanged(nameof(PercentSpace));
            OnPropertyChanged(nameof(MaxSpace));
            OnPropertyChanged(nameof(ActualSpace));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
