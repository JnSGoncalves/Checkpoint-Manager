using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Checkpoint_Manager.Models
{
    public class NameValidator{
        public static bool IsValidName(string name) {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Remove espaços extras no início/fim
            string trimmedName = name.Trim();

            // Verifica se o nome está vazio após o trim
            if (trimmedName.Length == 0)
                return false;

            // Verifica apenas os caracteres proibidos
            return !ContainsInvalidChars(trimmedName);
        }

        public static int? ExtrairNumeroFinal(string texto) {
            Match match = Regex.Match(texto, @"\((\d+)\)\s*$"); // Captura o último número entre () no final da string

            if (match.Success) {
                return int.Parse(match.Groups[1].Value);
            }

            return null; // Retorna null se não encontrar número no final
        }

        private static bool ContainsInvalidChars(string name) {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return name.Any(c => invalidChars.Contains(c));
        }
    }
}
