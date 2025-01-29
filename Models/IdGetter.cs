using System.Security.Cryptography;
using System.Text;

namespace Checkpoint_Manager.Models {
    internal class IdGetter {
        public static String CreateId(String seed) {
            if (seed.Length > 35)
                seed = seed.Substring(0, 35); // Garante no máximo 35 caracteres

            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));

            StringBuilder result = new();
            foreach (byte b in hashBytes) {
                result.Append(b % 10); // Garante apenas dígitos de 0 a 9
                if (result.Length >= 20)
                    break;
            }

            return result.ToString();
        }
    }
}
