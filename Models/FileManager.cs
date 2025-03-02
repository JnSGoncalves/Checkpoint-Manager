using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkpoint_Manager.Models {
    internal class FileManager {
        public static ConfigInfo Config { get; set; }

        public readonly static string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Checkpoint Manager");

        public static bool IsFull { get; set; }

        public static void AttArquives(ObservableCollection<Game> games) {
            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            string gamesArquivePath = Path.Combine(Config.SavesPath, "Games.json");

            string jsonConfig = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            string jsonGames = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(configArchivePath, jsonConfig);
            File.WriteAllText(gamesArquivePath, jsonGames);

            App.MainViewModelInstance.DownBarVM.GetSpaces();

            Debug.WriteLine("Atualização na Roaming Feita");
        }

        public static void AttConfig() {
            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            string jsonConfig = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configArchivePath, jsonConfig);

            if (Config.IsStartupEnable.HasValue) {
                StartupManager.SetStartup(Config.IsStartupEnable.Value);
            }
            App.MainViewModelInstance.DownBarVM.GetSpaces();

            Debug.WriteLine("Atualização nas Configurações Feita");
        }

        public static void RenameSave(Save save, string newName) {
            if (App.MainViewModelInstance.SelectedGame.Name is string gameName && gameName != null) {
                string savePath = Path.Combine(Config.SavesPath, Path.Combine(gameName, save.Name));
                string renameSavePath = Path.Combine(Config.SavesPath, Path.Combine(gameName, newName));

                DirectoryInfo directory = new DirectoryInfo(savePath);
                DirectoryInfo renamedDirectory = new DirectoryInfo(renameSavePath);

                Copy(directory, renamedDirectory);

                DeleteSave(gameName, savePath);

                Debug.WriteLine("Save renomeado");
            }
        }

        public static void RenameFolder(string path, string newName) {
            DirectoryInfo directory = new DirectoryInfo(path);

            if (directory.Parent == null) {
                Debug.WriteLine("O diretório não possui um diretório pai.");
                return;
            }

            string renamePath = Path.Combine(directory.Parent.FullName, newName);

            DirectoryInfo renamedDirectory = new DirectoryInfo(renamePath);

            Copy(directory, renamedDirectory);

            directory.Delete(true);

            Debug.WriteLine("Pasta renomeada");
        }

        public static bool DeleteFolder(string path) {
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
                return true;
            } else {
                return false;
            }
        }

        public static ObservableCollection<Game> FindGames() {
            // Cria a pasta onde ficam os saves
            if (!Directory.Exists(Config.SavesPath)) {
                Directory.CreateDirectory(Config.SavesPath);
            }

            string gamesArquive = Path.Combine(Config.SavesPath, "Games.json");
            if (!File.Exists(gamesArquive)) {
                File.Create(gamesArquive);

                return new ObservableCollection<Game>();
            }

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

        public static List<Game>? FindGames(string folderPath) {
            // Cria a pasta onde ficam os saves
            if (!Directory.Exists(folderPath)) {
                return null;
            }

            string gamesArquive = Path.Combine(folderPath, "Games.json");
            if (!File.Exists(gamesArquive)) {
                return null;
            }

            string jsonContent = File.ReadAllText(gamesArquive);

            if (string.IsNullOrWhiteSpace(jsonContent)) {
                return new List<Game>();
            }
            
            try {
                var games = JsonSerializer.Deserialize<List<Game>>(jsonContent);
                return games ?? new List<Game>();
            } catch (JsonException) {
                return new List<Game>();
            }
        }

        public static void StartConfigInfo() {
            if (!Path.Exists(ConfigPath)) {
                Directory.CreateDirectory(ConfigPath);
            }

            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            if (File.Exists(configArchivePath)) {
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                };

                ConfigInfo? config = JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(configArchivePath));

                if (config != null){
                    Config = (ConfigInfo)config;
                    Config.SetCulture();
                    Config.IsStartupEnable = StartupManager.IsStartupEnabled();

                    Debug.WriteLine("Arquivo de configuração carregado");
                } else {
                    Debug.WriteLine("Erro ao carregar as configurações!");
                }

            } else {
                // Caso não exista um arquivo de config já criado ele gera um novo a
                // partir das configurações padrão abaixo

                Config = new ConfigInfo();
                Config.IsAutoSave = false;
                Config.AutoSaveTime = 60;
                Config.MaxSaves = 0;
                Config.MaxSpace = 0;
                Config.CultureCountry = "pt-BR";

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
        }

        // Retorna o espaço ocupado por todos os saves em Bytes
        public static int GetUsedSpace() {
            int usedSpace = 0;
            DirectoryInfo savesDirectory = new DirectoryInfo(Config.SavesPath);
            usedSpace += GetUsedSpace(savesDirectory);
            
            return usedSpace;
        }
        // Retorna o espaço ocupado por uma pasta em Bytes
        private static int GetUsedSpace(DirectoryInfo directory) {
            int usedSpace = 0;
            if (!directory.Exists) {
                return usedSpace;
            }
            usedSpace += (int)directory.EnumerateFiles().Sum(file => file.Length);
            foreach (DirectoryInfo subDir in directory.GetDirectories()) {
                usedSpace += GetUsedSpace(subDir);
            }
            
            return usedSpace;
        }

        public static string BytesToString(double value) {
            string[] convertions = { "B", "KB", "MB", "GB" };
            //double convertedValue = value;
            int count = 0;
            do {
                value /= 1024;
                count++;
            } while (value >= 1024 && count < 3);
            return $"{value.ToString("0.##")} {convertions[count]}";
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

        public static bool CompactFolder(string zipPath, string folderToZip) {
            try {
                using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create)) {
                    foreach (string filePath in Directory.GetFiles(folderToZip, "*", SearchOption.AllDirectories)) {
                        string entryName = Path.GetRelativePath(folderToZip, filePath);
                        zipArchive.CreateEntryFromFile(filePath, entryName, CompressionLevel.Fastest);
                    }
                }
                Debug.WriteLine($"Conteúdo da pasta compactado com sucesso em: {zipPath}");
                return true;
            } catch (Exception ex) {
                Debug.WriteLine($"Erro ao compactar o conteúdo da pasta: {folderToZip}\n{ex.Message}");
                File.Delete(zipPath);
                return false;
            }
        }
        public static bool IsImportFile(string path) {
            try {
                using (ZipArchive zipArchive = ZipFile.Open(path, ZipArchiveMode.Read)) {
                    return zipArchive.GetEntry("Games.json") != null;
                }
            }catch (Exception ex) {
                Debug.WriteLine("Erro ao tentar ler o arquivo .zip");
                return false;
            }
        }

        public static bool DescompactZip(string zipPath, string destinPath) {
            try {
                if (!Directory.Exists(destinPath)) {
                    Directory.CreateDirectory(destinPath);
                }

                ZipFile.ExtractToDirectory(zipPath, destinPath);
                Debug.WriteLine($"Arquivo descompactado com sucesso em: {destinPath}");
                return true;
            } catch (Exception ex) {
                Debug.WriteLine($"Erro ao descompactar o arquivo: {zipPath}");
                return false;
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

        public static bool Copy(DirectoryInfo sourceDir, DirectoryInfo destDir, bool copySubDirs = true) {
            try{
                if (!destDir.Exists) {
                    Directory.CreateDirectory(destDir.FullName);
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

                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return false;
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