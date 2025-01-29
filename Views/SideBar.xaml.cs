using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager.Views {
    /// <summary>
    /// Interação lógica para SideBar.xam
    /// </summary>
    public partial class SideBar : Page {
        public SideBar() {
            InitializeComponent();
            UpdateToggleButtons();

            var viewModel = (MainViewModel)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void GameClick(object sender, RoutedEventArgs e) {
            if (list != null) {
                foreach (var child in list.Children) {
                    // Verifica se o item é um ToggleButton e não é o sender
                    if (child is ToggleButton toggleButton && !ReferenceEquals(child, sender)) {
                        toggleButton.IsChecked = false; // Desmarca o ToggleButton
                    }
                }
            }

            Console.WriteLine("Game id = " + GetSenderId(sender));

            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if(mainWindow != null) {
                mainWindow.teste();
            }
        }

        private void RemoveGame(object sender, RoutedEventArgs e) {
            ToggleButton gameButton = new ToggleButton();
            gameButton.Name = "_1";

            bool noGameSelected = true;

            if (list != null) {
                foreach (var child in list.Children) {
                    // Verifica se o item é um ToggleButton e não é o sender
                    if (child is ToggleButton toggleButton) {
                        if ((bool)toggleButton.IsChecked) {
                            noGameSelected = false;
                            gameButton = toggleButton;
                            break;
                        }
                    }
                }
            }

            if (noGameSelected) {
                MessageBox.Show("Nenhum jogo selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int id = GetSenderId(gameButton);
            if (id == -1) {
                MessageBox.Show("Erro ao buscar jogo", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Deseja realmente remover o jogo " + gameButton.Content + 
                "?\nEssa ação não pode ser desfeita.", "Remover Jogo", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK) { 
                ((MainViewModel)this.DataContext).DelGame(id);

                Console.WriteLine("Game " + id + " Removido");
            
                ViewDefaultContent();
            }
        }

        // Retorna o Id do jogo relacionado ao Toggle Button
        private int GetSenderId(object sender) {
            if (sender is ToggleButton gameButton) {
                String idString = gameButton.Name;
                if (idString != null && idString != "_1") {
                    int index = idString.IndexOf("_");
                    string result = idString.Substring(index + 1);
                    int id = int.Parse(result);

                    return id;
                } else
                    return -1;
            } else
                return -1;
        }

        private void UpdateToggleButtons() {
            list.Children.Clear();

            // Adiciona novos ToggleButtons para cada item na lista de jogos
            foreach (var game in App.mainViewModel.Games) {
                var toggleButton = new ToggleButton {
                    Name = "GameId_" + game.Id.ToString(),
                    Content = game.Name,
                    Style = (Style)FindResource("ButtonStyle")
                };

                // Adiciona o ToggleButton no StackPanel
                list.Children.Add(toggleButton);
            }
        }

        // Reseta a visualização do conteudo da página principal
        private void ViewDefaultContent() {
            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if (mainWindow != null) {
                mainWindow.viewDefaultPage();
            }
        }


        private void UncheckedGame(object sender, RoutedEventArgs e) {
            // Preferencialmente seria bom para a aplicação a adição de um timer
            // para impedir de clicar muito rápido no botão
            ViewDefaultContent();
        }

        // Evento criado para verificar se os itens da ViewModel foram modificados
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(MainViewModel.Games)) {
                // Chamar a função sempre que a propriedade modificada for a lista de Jogos
                UpdateToggleButtons();
            }
        }
    }
}
