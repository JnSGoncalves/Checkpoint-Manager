using System.Diagnostics;
using System.IO;
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
        public RelayCommand<Save> SwapSaveCommand { get; }

        public SavesPageViewModel() {
            DeleteSaveCommand = new RelayCommand<Save>(DelSave);
            AddSaveCommand = new RelayCommand(AddSave);
            SwapSaveCommand = new RelayCommand<Save>(SwapSave);
        }

        private void SwapSave(Save? save) {
            if (save != null && App.MainViewModelInstance.SelectedGame is Game selectedGame) {
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja criar um novo Checkpoint para o " +
                    "save atual antes da troca para o Checkpoint \"" + save.Name + 
                    "\"?\n\nLembre-se que essa função só deve ser usada quando o save atual não está em andamento.", 
                    "Retornar Checkpoint", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    try {
                        AddSave();

                        FileManager.SwapSave(selectedGame, save.Name);

                        System.Windows.MessageBox.Show("Sucesso ao fazer a troca do save atual para o Checkpoint "
                            + save.Name + ".", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show(ex.Message, "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }else if(result == MessageBoxResult.No) {
                    try {
                        FileManager.SwapSave(selectedGame, save.Name);

                        System.Windows.MessageBox.Show("Sucesso ao fazer a troca do save atual para o Checkpoint "
                            + save.Name + ".", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show(ex.Message, "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } else if(result == MessageBoxResult.Cancel){
                    Debug.WriteLine("Operação de troca de save cancelada");
                } else {
                    Debug.WriteLine("Erro ao trocar de save");
                }
            } else{
                Debug.WriteLine("Erro ao trocar de save");
            }
        }

        private void AddSave() {
            if (FileManager.IsFull) {
                System.Windows.MessageBox.Show("Espaço de armazenamento máximo definido alcançado." +
                    "\n\nCaso queira criar um novo Checkpoint, exclua um antigo ou modifique o espaço de " +
                    "armazenamento máximo a ser usado.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            DialogResult result = System.Windows.Forms.MessageBox.Show("Deseja criar um Checkpoint personalizado?\n\n" +
                "Caso não, será criado um Checkpoint automático gerado pelo sistema.", "Save personalizado",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if(result == DialogResult.No) {
                if (App.MainViewModelInstance.SelectedGame != null) {
                    if (!App.MainViewModelInstance.SelectedGame.NewSave()) {
                        Debug.WriteLine("Erro ao criar save automático");
                        throw new Exception("Erro ao criar novo Save");
                    } else {
                        // Chamada das funçoes p/ atualização dos arquivos e visualização do espaço utilizado
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

                string saveFolder = Path.Combine(
                        FileManager.Config.SavesPath,
                        Path.Combine(selectedGame.Name, save.Name)
                    );

                if (!Directory.Exists(saveFolder)) {
                    System.Windows.MessageBox.Show("Este Checkpoint não foi encontrado na pasta de " +
                        "backup, ele será removido da lista", "Deletar Save", MessageBoxButton.OK, MessageBoxImage.Information);
                    selectedGame.Saves.Remove(save);

                    // Chamada das funçoes p/ atualização dos arquivos e visualização do espaço utilizado
                    FileManager.AttArquives(App.MainViewModelInstance.Games);
                    return;
                }

                if (result == MessageBoxResult.Yes) {
                    if (FileManager.DeleteSave(selectedGame.Name, save.Name)) {
                        selectedGame.Saves.Remove(save);
                        Debug.WriteLine($"Save {save.Name} do jogo {App.MainViewModelInstance.SelectedGame.Name} removido!");

                        // Chamada das funçoes p/ atualização dos arquivos e visualização do espaço utilizado
                        FileManager.AttArquives(App.MainViewModelInstance.Games);
                        return;
                    }

                    System.Windows.MessageBox.Show("Erro ao tentar excluir o save", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
