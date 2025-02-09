using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Checkpoint_Manager.Models {
    internal class FileManager {
        public static ConfigInfo Config { get; set; }

        private readonly static string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Checkpoint Maneger");


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
                if (JsonSerializer.Deserialize<ObservableCollection<Game>>(File.ReadAllText(gamesArquive)) is
                    ObservableCollection<Game> games) {
                    return games;
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
                if(JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(configArchivePath)) is ConfigInfo config){
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
                Config.SavesPath = Path.Combine(documentosPath, "Checkpoint Maneger\\Saves");
                Debug.WriteLine(Config.SavesPath);

                var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                
                File.WriteAllText(configArchivePath, json);

                Debug.WriteLine("Novo arquivo de configuração criado em Roaming");
            }

            return Config;
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