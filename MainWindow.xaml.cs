using System.Windows;

namespace Checkpoint_Manager;
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    protected override void OnStateChanged(EventArgs e) {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Minimized) {
            Hide();
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
        e.Cancel = true;
        Hide();
    }
}