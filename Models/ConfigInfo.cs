﻿using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Checkpoint_Manager.Models {
    internal class ConfigInfo {
        public bool IsAutoSave { get; set; }
        public double AutoSaveTime { get; set; }
        public int MaxSaves { get; set; }
        public string? SavesPath { get; set; }
        [JsonIgnore]
        public CultureInfo? Culture { get; set; }
    }
}