//using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
namespace Atavism
{

    public class AtavismEncryption
    {

        // Declare CspParmeters and RsaCryptoServiceProvider
        // objects with global scope of your Form class.
        static CspParameters cspp = new CspParameters();
        public static RSACryptoServiceProvider rsa = null;

        /// <summary>
        /// Creates a Md5Sum of the string passed in. Can be useful if data needs to be encrypted 
        /// before being sent over the internet.
        /// </summary>
        /// <returns>
        /// The sum.
        /// </returns>
        /// <param name='strToEncrypt'>
        /// String to encrypt.
        /// </param>
        public static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static void ImportPublicKey(string pemkey)
        {
            rsa = PemKeyUtils.GetRSAProviderFromPemFile(pemkey.Trim());
        }

       public static string EncryptString(string text)
        {
            if (rsa == null)
            {
                return text;
            }
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] textBytes = encoding.GetBytes(text);
            byte[] encryptedOutput = rsa.Encrypt(textBytes, false);
            string outputB64 = Convert.ToBase64String(encryptedOutput);
            Console.WriteLine(outputB64);
            return outputB64;
        }
    }
}