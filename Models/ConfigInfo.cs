using System.Globalization;
using System.Text.Json.Serialization;

namespace Checkpoint_Manager.Models {
    internal class ConfigInfo {
        public bool? IsAutoSave { get; set; }
        public int? AutoSaveTime { get; set; } // Definido em minutos
        public int? MaxSaves { get; set; }  // 0 = Ilimitado - Máximo de saves automáticos
        public int? MaxSpace { get => maxSpace; 
            set {
                maxSpace = value;
            } 
        } // 0 = Ilimitado - Definido em MB
        public string? SavesPath { get; set; }

        private string? _cultureCountry;
        public string? CultureCountry {
            get {
                return _cultureCountry;
            }
            set {
                _cultureCountry = value;
                SetCulture();
            }
        }
        [JsonIgnore]
        public bool? IsStartupEnable { get; set; } = false;
        [JsonIgnore]
        private CultureInfo? _culture;
        private int? maxSpace;

        [JsonIgnore]
        public CultureInfo? Culture {
            get {
                return _culture;
            }
        }

        public void SetCulture() {
            _culture = CultureInfo.GetCultureInfo(CultureCountry);
        }
    }
}