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

        public bool AutoBackup(Game game, bool enable) {
            if (enable) {
                MessageBoxResult result = System.Windows.MessageBox.Show($"Iniciar auto backup para o game: {game.Name}?" +
                    "\n\nLembre-se que essa função não funcionará em todos os jogos. E só é utilizada em um jogo por vez" +
                    "\nPara verificar essa funcionalidade:" +
                    "\n1 - Crie, por segurança, um Checkpoint antes de abrir o jogo (Coloque um nome que possa ser identificado facilmente)" +
                    "\n2 - Entre no save do jogo e faça alguma ação que possa ser identificada ao retornar aquele save" +
                    "\n3 - Criar um novo Checkpoint enquanto o jogo e save ainda estão abertos" +
                    "\n4 - Saia do save e volte ao menu do jogo" +
                    "\n5 - Troque o save para o Checkpoint que foi criado por segurança no passo 1" +
                    "\n6 - Troque o save para o Checkpoint que foi criado no passo 3" +
                    "\n7 - Tente carregar o save do jogo e veja se a ação feita no passo 2 foi realizada" +
                    "\nSe o save for corrompido ou a ação dentro do jogo ainda não foi realizada neste novo checkpoint, " +
                    "o jogo possivelmente não é compativel com essa função do Checkpoint Manager." +
                    "\nCaso não seja compativel, exclua o Checkpoint feito no passo 3 e retorne ao Checkpoint criado " +
                    "no passo 1 para recuperar seu save.",
                    "Ativar AutoBackup", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes) {
                    App.MainViewModelInstance.SelectedBackupGame = game;
                    return true;
                } else {
                    return false;
                }

            } else {
                App.MainViewModelInstance.SelectedBackupGame = null;
                return false;
            }
        }

        private void SwapSave(Save? save) {
            if (save != null && App.MainViewModelInstance.SelectedGame is Game selectedGame) {
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja criar um novo Checkpoint para o " +
                    "save atual antes da troca para o Checkpoint \"" + save.Name + 
                    "\"?\n\nLembre-se que essa função não funciona em algum jogos quando o save ainda está em andamento.", 
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
                    selectedGame.DeleteSave(save);

                    // Chamada das funçoes p/ atualização dos arquivos e visualização do espaço utilizado
                    FileManager.AttArquives(App.MainViewModelInstance.Games);
                    return;
                }

                if (result == MessageBoxResult.Yes) {
                    if (FileManager.DeleteSave(selectedGame.Name, save.Name)) {
                        selectedGame.DeleteSave(save);
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

        public bool DeleteLastAutoSave(Game game) {
            Save save = game.GetLastAutoSave();

            if (save != null) {
                if (FileManager.DeleteSave(game.Name, save.Name)) {
                    game.DeleteSave(save);
                    return true;
                }
            }

            return false;
        }

        public void Rename(Game game, Save save) {
            if (save.IsAutoBackup)
                game.AutoBackupQtd -= 1;
        }
    }
}
