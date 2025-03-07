using System;

namespace Checkpoint_Manager.Models {
    internal class DateGetter {
        public static string GetActualDate() {
            DateTime actualDate = DateTime.Now;

            return actualDate.ToString("G", FileManager.Config.Culture);
        }

        public static string GetDateToName() {
            DateTime actualDate = DateTime.Now;

            string formattedDate = actualDate.ToString("G", FileManager.Config.Culture);

            // Substitui apenas os caracteres inválidos em pastas (mantendo outros como espaço, ponto, etc.)
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars) {
                formattedDate = formattedDate.Replace(c, '-');
            }

            return formattedDate;
        }
    }

    internal class HourGetter {
        public static void GetHourMinute(int? minutes, out int hours, out int mins) {
            if (minutes == null) {
                hours = 0;
                mins = 0;
            } else {
                hours = (int)(minutes / 60);
                mins = (int)(minutes % 60);
            }
        }

        public static int GetTimeInMinute(int? hour, int? minutes) {
            return (int)((hour.GetValueOrDefault() * 60) + minutes.GetValueOrDefault());
        }
    }
}
