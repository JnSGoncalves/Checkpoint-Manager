using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace Checkpoint_Manager.Models {
    internal class FileManager {
        public static ConfigInfo Config { get; set; }

        private readonly static string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Checkpoint Manager");


        // Ao salvar nos arquivos ele mantém o status de isSelected do jogo selecionado anteriormente
        // Fazer a modificação depois,

        // A Parte comentada resolve esse problema sem mudar a lista da aplicação,
        // mas não tenho certeza se funciona na conversão de volta na leitura do arquivo
        public static void AttArquives(ObservableCollection<Game> games) {
            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            string gamesArquivePath = Path.Combine(ConfigPath, "Games.json");

            //List<Game> copiaGames = games.ToList<Game>();

            foreach (Game game in games) {
                game.IsSelected = false;
            }

            string jsonConfig = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            //string jsonGames = JsonSerializer.Serialize(copiaGames, new JsonSerializerOptions { WriteIndented = true });
            string jsonGames = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(configArchivePath, jsonConfig);
            File.WriteAllText(gamesArquivePath, jsonGames);

            Debug.WriteLine("Atualização na Roaming Feita");
        }

        public static ObservableCollection<Game> FindGames() {
            // Cria a pasta onde ficam os saves
            if (!Directory.Exists(Config.SavesPath)) {
                Directory.CreateDirectory(Config.SavesPath);
            }

            string gamesArquive = Path.Combine(ConfigPath, "Games.json");
            if (!File.Exists(gamesArquive)) {
                File.Create(gamesArquive);
            } else {
                string jsonContent = File.ReadAllText(gamesArquive);

                if (string.IsNullOrWhiteSpace(jsonContent)) {
                    return new ObservableCollection<Game>();
                }

                try {
                    var games = JsonSerializer.Deserialize<ObservableCollection<Game>>(jsonContent);
                    return games ?? new ObservableCollection<Game>();
                } catch (JsonException) {
                    return new ObservableCollection<Game>();
                }
            }

            return new ObservableCollection<Game>();
        }

        public static ConfigInfo StartConfigInfo() {
            if (!Path.Exists(ConfigPath)) {
                Directory.CreateDirectory(ConfigPath);
            }

            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            if (File.Exists(configArchivePath)) {
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                };

                ConfigInfo config = JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(configArchivePath));

                if (config != null){
                    Config = (ConfigInfo)config;
                    Debug.WriteLine("Arquivo de configuração carregado");
                } else {
                    Debug.WriteLine("Erro ao carregar as configurações!");
                }

            } else {
                // Caso não exista um arquivo de config já criado ele gera um novo a
                // partir das configurações padrão abaixo

                Config = new ConfigInfo();
                Config.IsAutoSave = false;
                Config.AutoSaveTime = 60; // Definido em minutos
                Config.MaxSaves = 0; // 0 = Ilimitado
                Config.Culture = CultureInfo.GetCultureInfo("pt-BR");

                string documentosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Config.SavesPath = Path.Combine(documentosPath, "Checkpoint Manager\\Saves");
                Debug.WriteLine(Config.SavesPath);

                var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                
                File.WriteAllText(configArchivePath, json);

                Debug.WriteLine("Novo arquivo de configuração criado em Roaming");
            }

            return Config;
        }

        public static void SwapSave(Game selectedGame, string saveName) {
            // Save como pasta
            if (!selectedGame.IsSingleFileSave) { 
                string backupPath = Path.Combine(Config.SavesPath, Path.Combine(selectedGame.Name, saveName));

                DirectoryInfo swapSaveDirectoryInfo = new DirectoryInfo(backupPath);
                DirectoryInfo actualSaveDirectoryInfo = new DirectoryInfo(selectedGame.Path);

                if (swapSaveDirectoryInfo.Exists && actualSaveDirectoryInfo.Exists) {
                    FileManager.AttArquives(App.MainViewModelInstance.Games);

                    // Copia o save backup selecionado para a pasta de save do jogo
                    Copy(swapSaveDirectoryInfo, actualSaveDirectoryInfo);

                    return;
                } else {
                    selectedGame.Saves.RemoveAt(0);

                    Debug.WriteLine("Pasta de save de backup não encontrado nos arquivos");
                    throw new Exception("Pasta de save de backup não encontrado nos arquivos");
                }
            // Save de arquivo único
            } else {
                string backupSaveFolder = Path.Combine(Config.SavesPath, Path.Combine(selectedGame.Name, saveName));

                FileInfo actualSaveFile = new FileInfo(selectedGame.Path);
                FileInfo backupSaveFile = new FileInfo(Path.Combine(backupSaveFolder, actualSaveFile.Name));
                if (backupSaveFile.Exists && actualSaveFile.Exists) {
                    File.Copy(backupSaveFile.FullName, actualSaveFile.FullName, true);

                    return;
                } else {
                    selectedGame.Saves.RemoveAt(0);

                    Debug.WriteLine("Pasta de save de backup não encontrado nos arquivos");
                    throw new Exception("Pasta de save de backup não encontrado nos arquivos");
                }
            }
        }

        public static void CopyNewSave(Game game, string name) {
            string backupDirectory = Path.Combine(Config.SavesPath, game.Name);

            if (!Directory.Exists(backupDirectory)) {
                Directory.CreateDirectory(backupDirectory);
            }

            string saveBackupFolder = Path.Combine(backupDirectory, name);
            Directory.CreateDirectory(saveBackupFolder);
            DirectoryInfo finalDir = new DirectoryInfo(saveBackupFolder);

            if (game.IsSingleFileSave) {    
                FileInfo saveFile = new FileInfo(game.Path);
                if (saveFile.Exists) {
                    string newSaveFile = Path.Combine(finalDir.FullName, saveFile.Name);
                    
                    File.Copy(saveFile.FullName, newSaveFile);

                    Debug.WriteLine("Cópia de save criada");
                    return;
                } else {
                    finalDir.Delete();
                    Debug.WriteLine("Arquivo de Save do jogo não encontrado");
                    throw new DirectoryNotFoundException(
                        "Arquivo de origem não encontrado: " + saveFile);
                }
            } else {
                DirectoryInfo saveDirectory = new DirectoryInfo(game.Path);
                if (saveDirectory.Exists) {
                    Copy(saveDirectory, finalDir);

                    Debug.WriteLine("Cópia da pasta de save criada");
                    return;
                } else {
                    finalDir.Delete();
                    Debug.WriteLine("Pasta de Save do jogo não encontrado");
                    throw new DirectoryNotFoundException(
                        "Diretório de origem não encontrado: " + saveDirectory);
                }
            }
        }

        public static bool DeleteSave(string gameName, string saveName) {
            string pathFolder = Path.Combine(Config.SavesPath, Path.Combine(gameName, saveName));
            Debug.WriteLine(pathFolder);

            if (!Directory.Exists(pathFolder)) { 
                Debug.WriteLine($"Save {saveName} não existe");
                return false;
            }

            Directory.Delete(pathFolder, true);
            return true;
        }

        private static void Copy(DirectoryInfo sourceDir, DirectoryInfo destDir, bool copySubDirs = true) {
            if (!destDir.Exists) {
                Directory.CreateDirectory(sourceDir.FullName);
            }

            foreach (FileInfo file in sourceDir.GetFiles()) {
                string tempPath = Path.Combine(destDir.FullName, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubDirs) {
                foreach (DirectoryInfo subdir in sourceDir.GetDirectories()) {
                    DirectoryInfo tempPath = new DirectoryInfo(Path.Combine(destDir.FullName, subdir.Name));
                    if (!tempPath.Exists) {
                        Directory.CreateDirectory(tempPath.FullName);
                    }

                    // Chamada recursiva
                    Copy(subdir, tempPath, copySubDirs);
                }
            }
        }


        // adicionar essa verificação no textBox do nome de um novo save
        public static bool IsValidSaveName(string folderName) {
            if (string.IsNullOrWhiteSpace(folderName)) {
                return false;
            }

            if (folderName.EndsWith(".")) {
                return false;
            }

            string invalidChars = "<>:\"/\\|?*";
            foreach (char c in invalidChars) {
                if (folderName.Contains(c)) {
                    return false;
                }
            }

            string[] reservedNames = {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            foreach (string reservedName in reservedNames) {
                if (folderName.Equals(reservedName, StringComparison.OrdinalIgnoreCase)) {
                    return false;
                }
            }

            return true;
        }
    }
}