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
}
