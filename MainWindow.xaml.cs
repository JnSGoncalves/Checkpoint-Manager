using System.Windows;
using Checkpoint_Manager.Views;

namespace Checkpoint_Manager;
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