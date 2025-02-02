using System.Windows;
using Checkpoint_Manager.ViewModels;

namespace Checkpoint_Manager;
public partial class App : System.Windows.Application {
    public static MainViewModel MainViewModelInstance { get; } = new MainViewModel();

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        // Inicializa os dados da ViewModel
        MainViewModelInstance.StartApp();
    }
}