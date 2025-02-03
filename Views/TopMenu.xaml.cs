using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Checkpoint_Manager.Views {
    public partial class TopMenu : Page {
        public TopMenu() {
            InitializeComponent();
        }

        private void CloseSideBar(object sender, RoutedEventArgs e) {
            if (System.Windows.Application.Current.MainWindow is not MainWindow mainWindow)
                return;

            var sideBarFrame = mainWindow.FindName("sideBarFrame") as Frame;
            var mainContent = mainWindow.FindName("mainContent") as Frame;

            if (sideBarFrame?.RenderTransform is TranslateTransform translateTransform) {
                var animation = new DoubleAnimation {
                    To = -sideBarFrame.ActualWidth,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                animation.Completed += (s, e) => {
                    if (mainContent != null) {
                        Grid.SetColumn(mainContent, 0);
                        Grid.SetColumnSpan(mainContent, 3);
                    }
                };

                var gridSplitter = mainWindow.FindName("gridSplitter") as GridSplitter;
                gridSplitter.Visibility = Visibility.Collapsed;

                translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
            }
        }

        private void OpenSideBar(object sender, RoutedEventArgs e) {
            if (System.Windows.Application.Current.MainWindow is not MainWindow mainWindow)
                return;

            var sideBarFrame = mainWindow.FindName("sideBarFrame") as Frame;
            var mainContent = mainWindow.FindName("mainContent") as Frame;

            if (sideBarFrame?.RenderTransform is TranslateTransform translateTransform) {
                var animation = new DoubleAnimation {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                animation.Completed += (s, e) => {
                    var gridSplitter = mainWindow.FindName("gridSplitter") as GridSplitter;
                    gridSplitter.Visibility = Visibility.Visible;
                };

                translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

                if (mainContent != null) {
                    Grid.SetColumn(mainContent, 2);
                    Grid.SetColumnSpan(mainContent, 1);
                }
            }
        }
    }
}