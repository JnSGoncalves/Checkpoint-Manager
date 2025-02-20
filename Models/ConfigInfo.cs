using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Checkpoint_Manager.Models {
    internal class ConfigInfo {
        public bool IsAutoSave { get; set; }
        public double AutoSaveTime { get; set; } // Definido em minutos
        public int MaxSaves { get; set; }  // 0 = Ilimitado - Máximo de saves automáticos
        public int MaxSpace { get; set; } // 0 = Ilimitado - Definido em MB
        public string? SavesPath { get; set; }
        [JsonIgnore]
        public CultureInfo? Culture { get; set; }
        public double MaxUsedSpace { get; set; }
    }
}