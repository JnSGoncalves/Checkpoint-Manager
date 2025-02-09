using System.Windows;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager.Views
{
    /// <summary>
    /// Lógica interna para AddSaveWindow.xaml
    /// </summary>
    public partial class AddSaveWindow : Window
    {
        public AddSaveWindow()
        {
            InitializeComponent();

            var viewModel = new AddSaveViewModel();

            viewModel.CloseAction = () => this.Close();

            DataContext = viewModel;
        }
    }
}
