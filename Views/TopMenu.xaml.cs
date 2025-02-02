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
                    To = -200,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

                // Futuramente fazer uma alteração na animação pois os elementos da MainContent acabam ficando
                // sobrepostos com os da side bar no incio da animação
                if (mainContent != null) {
                    Grid.SetColumn(mainContent, 0);
                    Grid.SetColumnSpan(mainContent, 2);
                }
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

                translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

                if (mainContent != null) {
                    Grid.SetColumn(mainContent, 1);
                    Grid.SetColumnSpan(mainContent, 1);
                }
            }
        }
    }
}