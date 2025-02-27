using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels {
    public class SidePageViewModel {
        public ICommand SelectGameCommand { get; }
        public ICommand RemoveGameCommand { get; }
        public ICommand AddGameCommand { get; }
        public SidePageViewModel() {
            RemoveGameCommand = new RelayCommand(RemoveGame);
            AddGameCommand = new RelayCommand(AddGame);
            #pragma warning disable CS8622
            SelectGameCommand = new RelayCommand<Game>(SelectGame);
            #pragma warning restore CS8622
        }

        private void SelectGame(Game game) {
            if (App.MainViewModelInstance.SelectedGame == game) {
                App.MainViewModelInstance.SelectedGame = null;
                Debug.WriteLine("Nenhum game selecionado");
                return;
            } else {
                App.MainViewModelInstance.SelectedGame = game;
            }
            Debug.WriteLine($"Selected game: {App.MainViewModelInstance.SelectedGame.Name}");
        }

        // To do
        // Deletar a pasta dos arquivos do sistema, avisando que excluirá os saves ao fazer isso
        private void RemoveGame() { // Adicionar futuramente a remoção do jogo do arquivo de listagem dos jogos
            var SelectedGame = App.MainViewModelInstance.SelectedGame;
            var Games = App.MainViewModelInstance.Games;

            if (SelectedGame == null) {
                System.Windows.MessageBox.Show("Nenhum jogo selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else if (Games != null) {
                MessageBoxResult result = System.Windows.MessageBox.Show("Deseja realmente remover " + SelectedGame.Name +
                " da lista?\nEssa ação não pode ser desfeita.", "Remover Jogo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    Debug.WriteLine($"Jogo {SelectedGame.Name} de id: {SelectedGame.Id} foi excluido");

                    Games.Remove(SelectedGame);
                    App.MainViewModelInstance.SelectedGame = null;
                }
            }
        }

        private void AddGame() {
            Debug.WriteLine($"Open Add - Config is {App.MainViewModelInstance.ConfigIsOpen}");
            if (App.MainViewModelInstance.AddPageIsOpen == true)
                return;
            App.MainViewModelInstance.ResetOpenPages();
            App.MainViewModelInstance.AddPageIsOpen = true;
        }
    }
}
