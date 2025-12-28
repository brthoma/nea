using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nea
{

    /* Interface definition for a Cipher class
     * Derived classes encrypt and decrypt text according to their algorithms
     */
    public interface ICipher
    {
        /* Returns a randomly generated key appropriate for the cipher
         */
        byte[] GetRandomKey(Random random);

        /* Encrypts the text
         */
        string Encrypt(string plaintext, byte[] key);

        /* Decrypts the text
         */
        string Decrypt(string ciphertext, byte[] key);
    }


    /* Bitwise XOR cipher
     */
    public class XOR : ICipher
    {
        private const int MIN = 'A';
        private const int MAX = 'Z';
        private const int MAXIMUMKEYLENGTH = 6;

        public byte[] GetRandomKey(Random random)
        {
            int length = random.Next(1, MAXIMUMKEYLENGTH);
            string key = "";
            for (int i = 0; i < length; i++)
            {
                key += (char)(random.Next(MIN, MAX + 1));
            }
            return Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            string key = Encoding.UTF8.GetString(bKey);
            string ciphertext = "";

            for (int i = 0; i < plaintext.Length; i++)
            {
                ciphertext += (char) (plaintext[i] ^ key[i % key.Length]);
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            return Encrypt(ciphertext, bKey);
        }

    }

    /* ROT47 rotational cipher
     */
    public class ROT47 : ICipher
    {

        private const int MIN = 33;
        private const int MAX = 126;
        private const int RANGE = MAX - MIN + 1;

        public byte[] GetRandomKey(Random random)
        {
            return BitConverter.GetBytes(random.Next(1, RANGE));
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            int key = BitConverter.ToInt32(bKey, 0);
            string ciphertext = "";

            foreach (char c in plaintext)
            {
                if (c >= MIN && c <= MAX)
                {
                    ciphertext += (char)(MIN + ((c - MIN + key) % RANGE));
                }
                else
                {
                    ciphertext += c;
                }
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            int key = (RANGE - (BitConverter.ToInt32(bKey, 0) % RANGE)) % RANGE;
            return Encrypt(ciphertext, BitConverter.GetBytes(key));
        }

    }

    /* ROT13 rotational cipher
     */
    public class ROT13 : ICipher
    {
        private const int LOWERMIN = 'a';
        private const int LOWERMAX = 'z';
        private const int UPPERMIN = 'A';
        private const int UPPERMAX = 'Z';
        private const int RANGE = ('z' - 'a') + 1;

        public byte[] GetRandomKey(Random random)
        {
            return BitConverter.GetBytes(random.Next(1, RANGE));
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            int key = BitConverter.ToInt32(bKey, 0);
            string ciphertext = "";

            foreach (char c in plaintext)
            {
                if (c >= LOWERMIN && c <= LOWERMAX)
                {
                    ciphertext += (char)(LOWERMIN + ((c - LOWERMIN + key) % RANGE));
                }
                else if (c >= UPPERMIN && c <= UPPERMAX)
                {
                    ciphertext += (char)(UPPERMIN + ((c - UPPERMIN + key) % RANGE));
                }
                else
                {
                    ciphertext += c;
                }
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            int key = (RANGE - (BitConverter.ToInt32(bKey, 0) % RANGE)) % RANGE;
            return Encrypt(ciphertext, BitConverter.GetBytes(key));
        }

    }

    /* Vigenere cipher
     */
    public class Vigenere : ICipher
    {
        private const int MIN = 'A';
        private const int MAX = 'Z';
        private const int RANGE = 'Z' - 'A' + 1;
        private const int MAXKEYLENGTH = 6;
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public byte[] GetRandomKey(Random random)
        {
            int length = random.Next(1, MAXKEYLENGTH);
            string key = "";
            for (int i = 0; i < length; i++)
            {
                key += (char)(random.Next(MIN, MAX + 1));
            }
            return Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            ROT13 rot13 = new ROT13();

            string ciphertext = "";
            string key = Encoding.UTF8.GetString(bKey).ToUpper();
            int[] keyArr = new int[key.Length];

            for (int i = 0; i < key.Length; i++)
            {
                keyArr[i] = key[i] - MIN;
            }

            int keyIdx = 0;

            foreach (char c in plaintext)
            {
                if (ALPHABET.Contains(char.ToLower(c)))
                {
                    ciphertext += rot13.Encrypt(c.ToString(), BitConverter.GetBytes(keyArr[keyIdx]));
                    keyIdx = (keyIdx + 1) % key.Length;
                }
                else
                {
                    ciphertext += c;
                }
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            string key = Encoding.UTF8.GetString(bKey).ToUpper();
            string inverseKey = "";
            
            for (int i = 0; i < key.Length; i++)
            {
                inverseKey += (char) (MIN + ((RANGE - (key[i] - MIN)) % RANGE));
            }

            return Encrypt(ciphertext, Encoding.UTF8.GetBytes(inverseKey));
        }
    }

    /* Monoalphabetic substitution cipher
     */
    public class Substitution : ICipher
    {
        private const int RANGE = 'z' - 'a' + 1;
        private const int MIN = 'a';
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public byte[] GetRandomKey(Random random)
        {
            string alphabet = ALPHABET;
            string key = "";

            for (int i = 0; i < RANGE; i++)
            {
                int idxOfLetter = random.Next(alphabet.Length);
                key += alphabet[idxOfLetter];
                alphabet = alphabet.Remove(idxOfLetter, 1);
            }

            return Encoding.UTF8.GetBytes(key);
        }

        private Dictionary<char, int> GetTransformations(byte[] bKey, bool encrypt = true)
        {
            string key = Encoding.UTF8.GetString(bKey).ToLower();
            Dictionary<char, int> transformations = new Dictionary<char, int>();

            for (int i = 0; i < key.Length; i++)
            {
                if (encrypt)
                {
                    transformations.Add((char)(i + MIN), (int)key[i] - (i + MIN));
                }
                else
                {
                    transformations.Add(key[i], (int)(i + MIN) - key[i]);
                }
            }

            return transformations;
        }

        private string Transform(string originalText, Dictionary<char, int> transformations)
        {
            string newText = "";

            foreach (char c in originalText)
            {
                if (ALPHABET.Contains(char.ToLower(c)))
                {
                    newText += (char)(c + transformations[char.ToLower(c)]);
                }
                else
                {
                    newText += c;
                }
            }

            return newText;
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            Dictionary<char, int> transformations = GetTransformations(bKey);

            string ciphertext = Transform(plaintext, transformations);

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            Dictionary<char, int> transformations = GetTransformations(bKey, false);

            string plaintext = Transform(ciphertext, transformations);

            return plaintext;
        }

    }

    /* Returns the cipher object associated with the cipher type passed in
     */
    class CipherFactory
    {
        public static ICipher GetCipher(string cipherType)
        {
            switch (cipherType)
            {
                case "XOR":
                    return new XOR();
                case "ROT47":
                    return new ROT47();
                case "ROT13":
                    return new ROT13();
                case "Vigenere":
                    return new Vigenere();
                case "Substitution":
                    return new Substitution();
                default:
                    throw new Exception("No valid cipher selected");
            }
        }
    }

}
