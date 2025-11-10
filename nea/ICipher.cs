using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
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

    public interface ICipher
    {
        byte[] GetRandomKey(Random random);
        string Encrypt(string plaintext, byte[] key);
        string Decrypt(string ciphertext, byte[] key);
    }


    public class XOR : ICipher
    {

        private const int DEFAULTKEYLENGTH = 2;

        public byte[] GetRandomKey(Random random, int length)
        {
            string key = "";
            for (int i = 0; i < length; i++)
            {
                key += (char)(random.Next('A', 'Z' + 1));
            }
            return Encoding.UTF8.GetBytes(key);
        }
        public byte[] GetRandomKey(Random random)
        {
            return GetRandomKey(random, DEFAULTKEYLENGTH);
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

    public class ROT47 : ICipher
    {

        private const int MINRANGE = 33;
        private const int MAXRANGE = 126;
        private const int RANGE = MAXRANGE - MINRANGE;

        public byte[] GetRandomKey(Random random)
        {
            return BitConverter.GetBytes(random.Next(1, RANGE + 1));
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            int key = BitConverter.ToInt32(bKey, 0);
            string ciphertext = "";

            foreach (char c in plaintext)
            {
                if (c >= MINRANGE && c <= MAXRANGE)
                {
                    ciphertext += (char)(MINRANGE + ((c - MINRANGE + key) % (RANGE + 1)));
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
            int key = (RANGE + 1 - (BitConverter.ToInt32(bKey, 0) % (RANGE + 1))) % (RANGE + 1);
            return Encrypt(ciphertext, BitConverter.GetBytes(key));
        }

    }

    public class ROT13 : ICipher
    {

        private const int RANGE = 'z' - 'a';

        public byte[] GetRandomKey(Random random)
        {
            return BitConverter.GetBytes(random.Next(1, RANGE + 1));
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            int key = BitConverter.ToInt32(bKey, 0);
            string ciphertext = "";

            foreach (char c in plaintext)
            {
                if (c >= 'a' && c <= 'z')
                {
                    ciphertext += (char)('a' + ((c - 'a' + key) % ('z' - 'a' + 1)));
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    ciphertext += (char)('A' + ((c - 'A' + key) % ('Z' - 'A' + 1)));
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
            int key = (RANGE + 1 - (BitConverter.ToInt32(bKey, 0) % (RANGE + 1))) % (RANGE + 1);
            return Encrypt(ciphertext, BitConverter.GetBytes(key));
        }

    }

    public class Vigenere : ICipher
    {
        
        private const int RANGE = 'Z' - 'A';
        private const int DEFAULTKEYLENGTH = 2;
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public byte[] GetRandomKey(Random random, int length)
        {
            string key = "";
            for (int i = 0; i < length; i++)
            {
                key += (char)(random.Next('A', 'Z' + 1));
            }
            return Encoding.UTF8.GetBytes(key);
        }
        public byte[] GetRandomKey(Random random)
        {
            return GetRandomKey(random, DEFAULTKEYLENGTH);
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            ROT13 rot13 = new ROT13();

            string ciphertext = "";
            string key = Encoding.UTF8.GetString(bKey).ToUpper();
            int[] keyArr = new int[key.Length];

            for (int i = 0; i < key.Length; i++)
            {
                keyArr[i] = key[i] - 'A';
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
            string encryptKey = "";
            
            for (int i = 0; i < key.Length; i++)
            {
                encryptKey += (char) ('A' + ((RANGE + 1 - (key[i] - 'A')) % (RANGE + 1)));
            }

            return Encrypt(ciphertext, Encoding.UTF8.GetBytes(encryptKey));
        }
    }

    public class Substitution : ICipher
    {
        private const int RANGE = 'z' - 'a';
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public byte[] GetRandomKey(Random random)
        {
            string alphabet = ALPHABET;
            string key = "";
            
            for (int i = 0; i <= RANGE; i++)
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
                    transformations.Add((char)(i + 97), (int)key[i] - (i + 97));
                }
                else
                {
                    transformations.Add(key[i], (int)(i + 97) - key[i]);
                }
            }

            return transformations;
        }

        public string Encrypt(string plaintext, byte[] bKey)
        {
            Dictionary<char, int> transformations = GetTransformations(bKey);
            string ciphertext = "";

            foreach (char c in plaintext)
            {
                if (!ALPHABET.Contains(char.ToLower(c)))
                {
                    ciphertext += c;
                    continue;
                }
                ciphertext += (char) (c + transformations[char.ToLower(c)]);
            }

            return ciphertext;
        }

        public string Decrypt(string ciphertext, byte[] bKey)
        {
            Dictionary<char, int> transformations = GetTransformations(bKey, false);
            string plaintext = "";

            foreach (char c in ciphertext)
            {
                if (!ALPHABET.Contains(char.ToLower(c)))
                {
                    plaintext += c;
                    continue;
                }
                plaintext += (char)(c + transformations[char.ToLower(c)]);
            }

            return plaintext;
        }

    }

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
