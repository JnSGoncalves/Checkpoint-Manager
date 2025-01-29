using System.Windows.Controls;

namespace Checkpoint_Manager.Views;

/// <summary>
/// Interação lógica para DefaultPage.xam
/// </summary>
public partial class DefaultPage : Page {
    private static DefaultPage? defaultPage;

    private DefaultPage() {
        InitializeComponent();
    }

    public static DefaultPage GetDefaultPage() {
        defaultPage ??= new DefaultPage();
        return defaultPage;
    }
}