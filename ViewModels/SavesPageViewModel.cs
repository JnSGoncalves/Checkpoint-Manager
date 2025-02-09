using System.Diagnostics;
using System.Windows;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.Services;
using Checkpoint_Manager.Views;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels
{
    public class SavesPageViewModel {
        public RelayCommand<Save> DeleteSaveCommand { get; }
        public RelayCommand AddSaveCommand { get; }

        public SavesPageViewModel() {
            DeleteSaveCommand = new RelayCommand<Save>(DelSave);
            AddSaveCommand = new RelayCommand(AddSave);
        }

        public void AddSave() {
            DialogResult result = System.Windows.Forms.MessageBox.Show("Deseja criar um Checkpoint personalizado?\n\n" +
                "Caso não, será criado um Checkpoint automático gerado pelo sistema.", "Save personalizado",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if(result == DialogResult.No) {
                if (App.MainViewModelInstance.SelectedGame != null) {
                    if (!App.MainViewModelInstance.SelectedGame.NewSave()) {
                        Debug.WriteLine("Erro ao criar save automático");
                    } else {
                        FileManager.AttArquives(App.MainViewModelInstance.Games);
                    }
                }
            }else if(result == DialogResult.Yes) {
                DialogService.ShowDialog<AddSaveWindow>();
            }else {
                return;
            }
        }

        public void DelSave(Save? save) {
            if (save != null && App.MainViewModelInstance.SelectedGame is Game selectedGame) {
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja realmente deletar o save " + save.Name +
                "?\nEssa ação não pode ser desfeita.", "Deletar Save", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    if (FileManager.DeleteSave(selectedGame.Name ,save.Name)) {
                        selectedGame.Saves.Remove(save);
                        Debug.WriteLine($"Save {save.Name} do jogo {App.MainViewModelInstance.SelectedGame.Name} removido!");
                        return;
                    }
                    System.Windows.MessageBox.Show("Erro ao tentar excluir o save", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
