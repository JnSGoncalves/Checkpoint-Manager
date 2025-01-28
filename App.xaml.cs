using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application {
        public static MainViewModel mainViewModel { get; } = new MainViewModel();

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Inicializa os dados da ViewModel
            mainViewModel.StartApp();
        }
    }
}
