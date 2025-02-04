using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Checkpoint_Manager.Models {
    internal class FileManeger {
        public static ConfigInfo Config { get; set; }

        private readonly static string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "CheckpointManeger");

        public static void AttArquives(ObservableCollection<Game> games) {
            string configArchivePath = Path.Combine(ConfigPath, "Config.json");
            string gamesArquivePath = Path.Combine(ConfigPath, "Games.json");

            string jsonConfig = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
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

                string documentosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Config.SavesPath = Path.Combine(documentosPath, "CheckpointManeger\\Saves");
                Debug.WriteLine(Config.SavesPath);

                var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                
                File.WriteAllText(configArchivePath, json);

                Debug.WriteLine("Novo arquivo de configuração criado em Roaming");
            }

            return Config;
        }
    }
}