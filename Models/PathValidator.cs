using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Checkpoint_Manager.Models {
    public static class PathValidator {
        public static bool IsSystemOrProtectedPath(string path) {
            if (string.IsNullOrWhiteSpace(path))
                return true; // Caminho inválido

            try {
                if (!Directory.Exists(path) && !File.Exists(path))
                    return true; // Caminho não existe

                // Verificar se é um diretório do sistema
                string[] protectedPaths = {
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)
            };

                foreach (var protectedPath in protectedPaths) {
                    if (path.StartsWith(protectedPath, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                // Verificar atributo de sistema
                FileAttributes attributes = File.GetAttributes(path);
                if (attributes.HasFlag(FileAttributes.System) || attributes.HasFlag(FileAttributes.ReadOnly))
                    return true;

                // Testar permissão de escrita
                return !HasWritePermission(path);
            } catch {
                return true; // Qualquer erro indica um possível caminho protegido
            }
        }

        private static bool HasWritePermission(string path) {
            try {
                var directoryInfo = new DirectoryInfo(path);
                AuthorizationRuleCollection rules = directoryInfo.GetAccessControl()
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                foreach (FileSystemAccessRule rule in rules) {
                    if (rule.AccessControlType == AccessControlType.Deny)
                        return false; // Permissão negada

                    if (rule.AccessControlType == AccessControlType.Allow &&
                        principal.IsInRole((SecurityIdentifier)rule.IdentityReference) &&
                        rule.FileSystemRights.HasFlag(FileSystemRights.Write))
                        return true; // Tem permissão de escrita
                }
            } catch {
                return false;
            }

            return false;
        }
    }
}