﻿using System.Diagnostics;
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
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja realmente fazer a troca do " +
                    "save atual para o Checkpoint " + save.Name + "?\n\nLembre-se que essa função só deve ser usada " +
                    "quando o save atual não está em andamento.\n\n\n(Seu save atual será guardado em um Checkpont automático)", 
                    "Retornar Checkpoint", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    try {
                        FileManager.SwapSave(selectedGame, save.Name);

                        System.Windows.MessageBox.Show("Sucesso ao fazer a troca do save atual para o Checkpoint "
                            + save.Name + ".", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show(ex.Message, "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            } else {
                Debug.WriteLine("Erro ao trocar de save");
            }
        }

        private void AddSave() {
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

                string saveFolder = Path.Combine(
                        FileManager.Config.SavesPath,
                        Path.Combine(selectedGame.Name, save.Name)
                    );

                if (!Directory.Exists(saveFolder)) {
                    System.Windows.MessageBox.Show("Este Checkpoint não foi encontrado na pasta de " +
                        "backup, ele será removido da lista", "Deletar Save", MessageBoxButton.OK, MessageBoxImage.Information);
                    selectedGame.Saves.Remove(save);
                    FileManager.AttArquives(App.MainViewModelInstance.Games);
                    return;
                }

                if (result == MessageBoxResult.Yes) {
                    if (FileManager.DeleteSave(selectedGame.Name ,save.Name)) {
                        selectedGame.Saves.Remove(save);
                        Debug.WriteLine($"Save {save.Name} do jogo {App.MainViewModelInstance.SelectedGame.Name} removido!");
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
