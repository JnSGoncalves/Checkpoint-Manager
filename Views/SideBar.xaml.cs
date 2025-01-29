using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager.Views;

/// <summary>
/// Interação lógica para SideBar.xam
/// </summary>
public partial class SideBar : Page {
    public SideBar() {
        InitializeComponent();
        UpdateToggleButtons();

        if (DataContext is MainViewModel viewModel) {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void GameClick(object sender, RoutedEventArgs e) {
        if (list == null)
            return;

        foreach (var child in list.Children) {
            if (child is ToggleButton toggleButton && !ReferenceEquals(child, sender)) {
                toggleButton.IsChecked = false;
            }
        }

        Debug.WriteLine($"Game id = {GetSenderId(sender)}");

        if (Application.Current.MainWindow is MainWindow mainWindow) {
            mainWindow.Teste();
        }
    }

    private void RemoveGame(object sender, RoutedEventArgs e) {
        var selectedGame = list?.Children.OfType<ToggleButton>()
            .FirstOrDefault(tb => tb.IsChecked == true);

        if (selectedGame == null) {
            MessageBox.Show("Nenhum jogo selecionado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        String id = GetSenderId(selectedGame);
        if (id.Equals("")) {
            MessageBox.Show("Erro ao buscar jogo", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var result = MessageBox.Show(
            $"Deseja realmente remover {selectedGame.Content} da lista?\nEssa ação não pode ser desfeita.",
            "Remover Jogo",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.OK) {
            if (DataContext is MainViewModel viewModel) {
                viewModel.DelGame(id);
                Debug.WriteLine($"Game {selectedGame.Content} Id: {id} Removido");
                ViewDefaultContent();
            }
        }
    }

    private String GetSenderId(object sender) {
        if (sender is not ToggleButton gameButton)
            return "";

        if (string.IsNullOrEmpty(gameButton.Name) || gameButton.Name == "_1")
            return "";

        int index = gameButton.Name.IndexOf('_');
        return gameButton.Name[(index + 1)..];
    }

    private void UpdateToggleButtons() {
        list.Children.Clear();

        if (App.mainViewModel.Games != null) {
            foreach (var game in App.mainViewModel.Games) {
                var toggleButton = new ToggleButton {
                    Name = $"GameId_{game.Id}",
                    Content = game.Name,
                    Style = (Style)FindResource("ButtonStyle")
                };

                list.Children.Add(toggleButton);
            }
        }
    }

    private void ViewDefaultContent() {
        if (Application.Current.MainWindow is MainWindow mainWindow) {
            mainWindow.ViewDefaultPage();
        }
    }

    private void UncheckedGame(object sender, RoutedEventArgs e) {
        ViewDefaultContent();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(MainViewModel.Games)) {
            UpdateToggleButtons();
        }
    }
}