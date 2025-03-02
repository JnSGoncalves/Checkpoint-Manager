using System.Windows.Input;
using System.Windows;
using System.IO;

namespace Checkpoint_Manager.Services
{
    public class NotifyIconService {
        private static NotifyIconService? _instance;
        public static NotifyIconService Instance => _instance ??= new NotifyIconService();

        private NotifyIcon _notifyIcon;
        private ICommand? _showWindowCommand;
        private ICommand? _exitCommand;

        private NotifyIconService() { }
        public void Initialize() {
            _notifyIcon = new NotifyIcon {
                Icon = LoadIcon(),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Checkpoint Manager"
            };

            _notifyIcon.DoubleClick += (s, e) => _showWindowCommand?.Execute(null);

            _notifyIcon.ContextMenuStrip.Items.Add("Abrir", null, (s, e) => _showWindowCommand?.Execute(null));
            _notifyIcon.ContextMenuStrip.Items.Add("Sair", null, (s, e) => _exitCommand?.Execute(null));
        }

        public void SetCommands(ICommand showCommand, ICommand exitCommand) {
            _showWindowCommand = showCommand;
            _exitCommand = exitCommand;
        }

        public void SetNotifyIconVisibility(bool isVisible) {
            _notifyIcon.Visible = isVisible;
        }

        private Icon LoadIcon() {
            using (var stream = App.GetResourceStream(new Uri("pack://application:,,,/app_icon.ico"))?.Stream) {
                return stream != null ? new Icon(stream) : SystemIcons.Application;
            }
        }
    }
}
