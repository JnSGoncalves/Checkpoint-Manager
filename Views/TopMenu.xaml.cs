using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkpoint_Manager.Views {
    /// <summary>
    /// Interação lógica para TopMenu.xam
    /// </summary>
    public partial class TopMenu : Page {
        public TopMenu() {
            InitializeComponent();
        }

        private void CloseSideBar(object sender, RoutedEventArgs e) {
            var sideBarFrame = (Application.Current.MainWindow as MainWindow).FindName("sideBarFrame") as Frame;
            if (sideBarFrame != null) {
                var translateTransform = sideBarFrame.RenderTransform as TranslateTransform;
                if (translateTransform != null) {
                    // Criando a animação para mover a SideBar para fora da tela
                    var animation = new DoubleAnimation {
                        To = -200, // Valor negativo para mover a Sidebar para fora
                        Duration = new Duration(TimeSpan.FromSeconds(0.3))
                    };
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                }
            }
        }

        private void OpenSideBar(object sender, RoutedEventArgs e) {
            var sideBarFrame = (Application.Current.MainWindow as MainWindow).FindName("sideBarFrame") as Frame;
            if (sideBarFrame != null) {
                var translateTransform = sideBarFrame.RenderTransform as TranslateTransform;
                if (translateTransform != null) {
                    // Criando a animação para mover a SideBar de volta para a posição inicial
                    var animation = new DoubleAnimation {
                        To = 0, // Valor 0 para colocar a Sidebar de volta à posição original
                        Duration = new Duration(TimeSpan.FromSeconds(0.3))
                    };
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                }
            }
        }
    }
}
