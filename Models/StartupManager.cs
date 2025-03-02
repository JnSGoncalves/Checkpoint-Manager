using Microsoft.Win32;

namespace Checkpoint_Manager.Models {
    public class StartupManager {
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "Checkpoint Manager";

        public static void SetStartup(bool enable) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true)) {
                if (key == null)
                    return;

                if (enable) {
                    key.SetValue(AppName, Application.ExecutablePath);
                } else {
                    key.DeleteValue(AppName, false);
                }
            }
        }

        public static bool IsStartupEnabled() {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false)) {
                return key?.GetValue(AppName) != null;
            }
        }
    }
}
