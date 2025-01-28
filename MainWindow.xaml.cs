using System.Windows;
using Checkpoint_Manager.Models;
using Checkpoint_Manager.ViewModels;
using Checkpoint_Manager.Views;

namespace Checkpoint_Manager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            viewDefaultPage();
        }

        public void viewDefaultPage() {
            mainContent.Content = DefaultPage.getDefaultPage();
        }

        public void teste() {
            mainContent.Content = new SavesPage();
        }
    }
}
