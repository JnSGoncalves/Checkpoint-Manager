using System.Windows;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.ViewModels;
using Checkpoint_Manager.Views;

namespace Checkpoint_Manager;

/// <summary>
/// Interação lógica para MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        ViewDefaultPage();
    }

    public void ViewDefaultPage() {
        mainContent.Content = DefaultPage.GetDefaultPage();
    }

    public void Teste() {
        mainContent.Content = new SavesPage();
    }
}