using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager.Views {
    /// <summary>
    /// Interação lógica para SideBar.xam
    /// </summary>
    public partial class SideBar : Page {
        public SideBar() {
            InitializeComponent();

            UpdateToggleButtons();
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

            if (sender is ToggleButton gameButton) {
                String idString = gameButton.Name;
                if (idString != null) {
                    int index = idString.IndexOf("_");
                    string result = idString.Substring(index + 1);
                    int id = int.Parse(result);

                    Console.WriteLine(id);
                }
            }

            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if(mainWindow != null) {
                mainWindow.teste();
            }
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

        private void ReturnToDefaultContent(object sender, RoutedEventArgs e) {
            // Preferencialmente seria bom para a aplicação a adição de um timer
            // para impedir de clicar muito rápido no botão
            var mainWindow = (Application.Current.MainWindow as MainWindow);
            if (mainWindow != null) {
                mainWindow.viewDefaultPage();
            } 
        }
    }
}
