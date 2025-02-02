using System.Diagnostics;
using System.Windows;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels
{
    public class SavesPageViewModel {
        public RelayCommand<Save> DeleteSaveCommand { get; }

        public SavesPageViewModel() {
            DeleteSaveCommand = new RelayCommand<Save>(DelSave);
        }

        public void DelSave(Save? save) {
            if (save != null && App.MainViewModelInstance.SelectedGame is Game selectedGame) {
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja realmente deletar o save " + save.Name +
                "?\nEssa ação não pode ser desfeita.", "Remover Save", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    selectedGame.Saves.Remove(save);
                    Debug.WriteLine($"Save {save.Name} do jogo {App.MainViewModelInstance.SelectedGame.Name} removido!");
                }
            }
        }
    }
}
