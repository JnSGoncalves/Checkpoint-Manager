using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkpoint_Manager.Models {
    internal class ConfigInfo {
        private static ConfigInfo configInfo;
        public int ActualId { get; set; }
        public bool IsAutoSave { get; set; }
        public double AutoSaveTime { get; set; }
        public int MaxSaves { get; set; }

        private ConfigInfo() { }

        public static ConfigInfo GetConfigInfo() { 
            if (configInfo == null) {
                configInfo = new ConfigInfo();
            }
            return configInfo;
        }
    }
}
