using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Caliburn.Micro;

namespace Client.Main.Utilities
{
    public static class Statics
    {



        /// <summary>
        /// 'Global' variable wich indicates the server conection ClientStatus.
        /// </summary>
        private static string _clientStatus = "Trabajando localmente";
        public static string ClientStatus
        {
            get { return _clientStatus; }
            set
            {
                _clientStatus = value;

            }
        }

        /// <summary>
        /// Lista con los nombres de las unidades utilizadas para llenar diferentes formularios 
        /// </summary>
        private static BindableCollection<string> _unidades = new BindableCollection<string>()
        {           "Bandeja(s)",
                    "Bulto(s)",
                    "Docena(s)",
                    "Caja(s)",
                    "Canastilla(s)",
                    "Kilogramo(s)",
                    "Unidad(es)"
        };

        public static BindableCollection<string>  Unidades
        {
            get { return _unidades; }
        }



        #region Crypto
        /// Size of salt, must be 8 bytes or larger.
        private const int SaltSize = 16;

        /// Size of hash.
        private const int HashSize = 20;

        /// <summary>
        ///  Iteration count must be greater than zero, the minimum recommended number of iterations is 1000.
        /// </summary>
        private const int iterations = 10000;

        /// Creates a hash from a password.
        /// <param name="iterations">Number of iterations.</param>
        /// <returns>The hash.</returns>
        public static string Hash(string password)
        {
            /// Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            /// Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }


        /// <summary>
        /// Checks if hash is supported.
        /// </summary>
        /// <param name="hashString">The hash.</param>
        /// <returns>Is supported?</returns>
        public static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>Could be verified?</returns>
        public static bool Verify(string password, string hashedPassword)
        {
            // Check hash
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("Error en el hasshing de la contraseña.");
            }

            // Extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        } 
        #endregion



        public static string PrimeraAMayuscula(string palabra)
        {
            if (palabra.Length == 0)
                return palabra;
            else if (palabra.Length == 1)
                return palabra.ToUpper();
            else
                return palabra[0].ToString().ToUpper() + palabra.Substring(1);
        }

    }
}
