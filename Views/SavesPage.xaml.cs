using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Checkpoint_Manager.Models;

namespace Checkpoint_Manager.Views {
    public partial class SavesPage : Page {
        public SavesPage() {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void ActiveLoading() {
            if (LoadingIcon != null)
                LoadingIcon.Visibility = Visibility.Visible;
        }

        private void DisableLoading() {
            if (LoadingIcon != null)
                LoadingIcon.Visibility = Visibility.Hidden;
        }

        private void AutoBackupCommand(object sender, RoutedEventArgs e) {
            if (DataContext is Game game) {
                if (App.MainViewModelInstance.SelectedBackupGame != null &&
                    App.MainViewModelInstance.SelectedBackupGame.Equals(game)) {
                    DisableLoading();
                    App.MainViewModelInstance.SavesPageVM.AutoBackup(game, false);
                } else {
                    if (App.MainViewModelInstance.SavesPageVM.AutoBackup(game, true)) {
                        ActiveLoading();
                    } else {
                        DisableLoading();
                        PlayStopButton.IsChecked = false;
                    }
                }
            }
        }

        public void Rename(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Rename Function");

            if (sender != null) {
                if (sender is System.Windows.Controls.Button button && button.DataContext is Save save) {

                    var parent = VisualTreeHelper.GetParent(button);
                    while (parent != null && !(parent is StackPanel)) {
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    if (parent is StackPanel stackPanel) {
                        string txtName = "txt" + save.Id;
                        string lblName = "lbl" + save.Id;

                        var txt = FindChild<System.Windows.Controls.TextBox>(stackPanel, txtName);
                        var lbl = FindChild<System.Windows.Controls.Label>(stackPanel, lblName);

                        if (txt != null && lbl != null) {
                            if (lbl.Visibility == Visibility.Visible) {
                                lbl.Visibility = Visibility.Collapsed;
                                txt.Visibility = Visibility.Visible;
                            } else {
                                txt.Visibility = Visibility.Collapsed;
                                lbl.Visibility = Visibility.Visible;
                            }
                        } else {
                            Debug.WriteLine("lbl e txt null");
                        }
                    }
                }
            } else {
                Debug.WriteLine("Save não é DataContext");
            }
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject {
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null) {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null)
                        break;
                } else if (!string.IsNullOrEmpty(childName)) {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName) {
                        foundChild = (T)child;
                        break;
                    }
                } else {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e) {
            if (sender is System.Windows.Controls.TextBox txtBox && txtBox.Tag != null) {
                string id = txtBox.Tag.ToString();

                txtBox.Name = "txt" + id;
            }
        }

        private void Label_Loaded(object sender, RoutedEventArgs e) {
            if (sender is System.Windows.Controls.Label label && label.Tag != null) {
                string id = label.Tag.ToString();

                label.Name = "lbl" + id;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (DataContext is Game game) {
                Debug.WriteLine("1");
                if (App.MainViewModelInstance.SelectedBackupGame == null ||
                    !App.MainViewModelInstance.SelectedBackupGame.Equals(game)) {
                    Debug.WriteLine("2");
                    DisableLoading();
                }
            }
        }
    }
}
