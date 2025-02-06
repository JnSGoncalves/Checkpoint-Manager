using System;

namespace Checkpoint_Manager.Models {
    internal class DateGetter {
        public static string GetActualDate() {
            DateTime actualDate = DateTime.Now;

            return actualDate.ToString("g", FileManeger.Config.Culture);
        }
    }
}
