using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nea
{
    /* Interface definition for a Cryptanalysis class
     * Derived objects cycle through possible keys
     */
    internal interface ICryptanalysis
    {
        /* Returns an IEnumerable which cycles through possible keys
         */
        IEnumerable<byte[]> GetKeys(string text);
    }


    /* Cryptanalysis for the ROT13 cipher
     * Cycles through every possible key from 1 to 26
     */
    public class ROT13Cryptanalysis : ICryptanalysis
    {

        public IEnumerable<byte[]> GetKeys(string text)
        {
            for (int i = 1; i <= 26; i++)
            {
                yield return BitConverter.GetBytes(i);
            }
        }

    }

    /* Cryptanalysis for the ROT13 cipher
     * Takes character frequencies into account in order to prioritise likely keys
     */
    public class FasterROT13Cryptanalysis : ICryptanalysis
    {
        private const string ORDEROFCHECK = "etaoinsrhldcumfpgwybvkxjqz";
        private const int ROT13RANGE = 26 - 1;
        private Dictionary<char, int> textCounts = new Dictionary<char, int>();

        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach (char c in text.ToLower())
            {
                if (textCounts.ContainsKey(c))
                {
                    textCounts[c]++;
                }
                else
                {
                    textCounts.Add(c, 1);
                }
            }

            for (int i = 0; i < textCounts.Count; i++)
            {
                int maxCount = textCounts.Values.Max();
                char mostCommon = textCounts.First(kvp => kvp.Value == maxCount).Key;

                foreach (char nextLikelyPlain in ORDEROFCHECK)
                {
                    int possibleKey = ((mostCommon - nextLikelyPlain) % (ROT13RANGE + 1) + ROT13RANGE + 1) % (ROT13RANGE + 1);
                    yield return BitConverter.GetBytes(possibleKey);
                }
            }

        }
    }

    /* Cryptanalysis for the ROT47 cipher
     * Takes character frequencies into account in order to prioritise likely keys
     */
    public class ROT47Cryptanalysis : ICryptanalysis
    {

        private const string ORDEROFCHECK = "etaoinsrhldcumfpgwybvkxjqzETAOINSRHLDCUMFPGWYBVKXJQZ,.";
        private const int ROT47RANGE = 126 - 33;
        private Dictionary<char, int> textCounts = new Dictionary<char, int>();
        
        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach (char c in text)
            {
                if (c < '!' || c > '~')
                {
                    continue;
                }
                if (textCounts.ContainsKey(c))
                {
                    textCounts[c]++;
                }
                else
                {
                    textCounts.Add(c, 1);
                }
            }

            for (int i = 0; i < textCounts.Count; i++)
            {
                int maxCount = textCounts.Values.Max();
                char mostCommon = textCounts.First(kvp => kvp.Value == maxCount).Key;

                foreach (char nextLikelyPlain in ORDEROFCHECK)
                {
                    int possibleKey = ((mostCommon - nextLikelyPlain) % (ROT47RANGE + 1) + ROT47RANGE + 1) % (ROT47RANGE + 1);
                    yield return BitConverter.GetBytes(possibleKey);
                }
            }

        }

    }

    /* Cryptanalysis for the Vigenere cipher
     * Calculates Index of Coincidence in order to determine the likely key lengths
     * Splits the text up into slices - sections which have been encrypted using the same key
     * Uses frequency analysis in order to prioritise the most likely keys for each slice
     */
    public class VigenereCryptanalysis : ICryptanalysis
    {
        private const double IOCTHRESHOLD = 1.5;
        private const int MAXKEYLENGTH = 6;
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        /* Splits text into slices
         */
        private string[] GetSlices(string text, int numSlices)
        {
            string[] slices = new string[numSlices];
            int keyIdx = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (ALPHABET.Contains(char.ToLower(text[i])))
                {
                    slices[keyIdx] += text[i];
                    keyIdx = (keyIdx + 1) % numSlices;
                }
            }

            return slices;
        }

        /* Returns the character frequencies
         */
        private Dictionary<char, int> GetCharFreqs(string text)
        {
            Dictionary<char, int> textCounts = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (!textCounts.ContainsKey(c) && ALPHABET.Contains(char.ToLower(c)))
                {
                    textCounts.Add(c, text.Count(chr => chr == c));
                }
            }

            return textCounts;
        }

        /* Calculation of Index of Coincidence
         * Returns the probability that any two randomly selected characters in the text are the same
         */
        private double CalcIdxOfCoincidence(string textSlice)
        {
            double idxOfCoincidence = 0;
            Dictionary<char, int> charCounts = GetCharFreqs(textSlice);

            foreach (KeyValuePair<char, int> kvp in charCounts)
            {
                idxOfCoincidence += (double) kvp.Value * (kvp.Value - 1) / ( textSlice.Length * (textSlice.Length - 1) );
            }

            return idxOfCoincidence * 26;
        }

        /* Cycles through possible key lengths and their associated slices
         */
        private IEnumerable<(int, string[])> GetLikelyKeyLength(string text)
        {
            for (int i = 1; i < MAXKEYLENGTH; i++)
            {
                double totalIoC = 0;

                string[] slices = GetSlices(text, i);

                foreach (string slice in slices)
                {
                    totalIoC += CalcIdxOfCoincidence(slice);
                }

                if (totalIoC / i > IOCTHRESHOLD)
                {
                    yield return (i, slices);
                }
            }

        }

        /* Cycles through likely keys for a single slice
         */
        private IEnumerable<int> GetSingleKey(string slice, int attempts = 26)
        {
            ROT13 cipher = new ROT13();
            FrequencyAnalysis freqAnalysis = new FrequencyAnalysis();

            List<int> keys = new List<int>();
            List<double> pvalues = new List<double>();

            for(int i = 0; i < 26; i++)
            {
                double pvalue = freqAnalysis.Classify(cipher.Decrypt(slice, BitConverter.GetBytes(i)));
                keys.Add(i);
                pvalues.Add(pvalue);
            }
            for (int i = 0; i < attempts; i++)
            {
                int idx = pvalues.IndexOf(pvalues.Max());
                int key = keys[idx];
                pvalues.RemoveAt(idx);
                keys.RemoveAt(idx);
                yield return key;
            }
        }

        /* Recursive function which cycles through possible keys for a given key length
         * Increments keys in order of plausibility according to frequency analysis from rightmost to leftmost
         */
        private IEnumerable<string> Cycle(int keyLength, int idxInKey, string key, string[] slices)
        {

            foreach (int intKeyChar in GetSingleKey(slices[idxInKey], 10))
            {
                char keyChar = (char)((intKeyChar % 26 + 26) % 26 + 'A');

                if (idxInKey == keyLength - 1)
                {
                    yield return key + keyChar;
                }
                else
                {
                    foreach (string keyFound in Cycle(keyLength, idxInKey + 1, key + keyChar, slices))
                    {
                        yield return keyFound;
                    }
                }
            }

        }

        /* Cycles through keys and key lengths
         * Converts the output from Cycle into an appropriate format
         */
        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach ((int keyLength, string[] slices) in GetLikelyKeyLength(text))
            {
                foreach (string possibleKey in Cycle(keyLength, 0, "", slices))
                {
                    yield return Encoding.UTF8.GetBytes(possibleKey);
                }
            }

        }
    }

    /* Cryptanalysis for the Substitution cipher
     * Uses a dictionary to calculate the approximate Hamming edit distance of the text
     * Initial key is based on frequency analysis
     * Key characters are swapped at random and compared with the previous guess
     */
    public class SubstitutionCryptanalysis : ICryptanalysis
    {

        private const string ORDEROFFREQUENCY = "etaoinsrhldcumfpgwybvkxjqz";
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
        private const int NUMBEROFSWAPS = 1000;
        private string dictionaryFilePath;
        private Random random;
        private ICipher cipher;
        private IClassifier classifier;

        public SubstitutionCryptanalysis(string dictionaryFilePath)
        {
            this.dictionaryFilePath = dictionaryFilePath;
            random = new Random();
            cipher = new Substitution();
            classifier = new HammingEditDist(dictionaryFilePath);
        }

        /* Finds character frequencies used for frequency analysis
         */
        public string GetOrderOfCharFreqs(string text)
        {
            string orderOfFreqsInText = "";
            Dictionary<char, int> freqs = new Dictionary<char, int>();
            string lowerText = text.ToLower();

            foreach (char letter in ALPHABET)
            {
                freqs.Add(letter, lowerText.Count(c => c == letter));
            }

            while(orderOfFreqsInText.Length < 26)
            {
                int largestFreq = freqs.Values.Max();
                char nextMostCommon = freqs.First(kvp => kvp.Value == largestFreq).Key;
                orderOfFreqsInText += nextMostCommon;
                freqs.Remove(nextMostCommon);
            }

            return orderOfFreqsInText;
        }

        /* Makes the initial key guess based off of frequency analysis
         */
        private string FirstGuess(string text)
        {
            string freqsInText = GetOrderOfCharFreqs(text);
            string key = "";

            foreach (char letter in ALPHABET)
            {
                key += freqsInText[ORDEROFFREQUENCY.IndexOf(letter)];
            }

            return key;
        }

        /* Swaps key characters randomly
         */
        private string RandomSwap(string key, Random random)
        {
            int idx1 = random.Next(0, key.Length);
            int idx2;
            do
            {
                idx2 = random.Next(0, key.Length);
            } while (idx1 == idx2);
            if (idx1 > idx2)
            {
                (idx1, idx2) = (idx2, idx1);
            }
            string newKey = key.Substring(0, idx1) + key[idx2] + key.Substring(idx1 + 1, idx2 - idx1 - 1) + key[idx1] + key.Substring(idx2 + 1);
            return newKey;
        }

        /* Cycles through keys
         * After random swap, ClosenessToDictionary classifier is used to determine
         * whether the new key is an improvement or not
         * The better key is kept
         * Over iterations, the key will improve
         */
        public IEnumerable<byte[]> GetKeys(string text)
        {
            string key = FirstGuess(text);

            string decryptedText = cipher.Decrypt(text, Encoding.UTF8.GetBytes(key));
            double classification = classifier.Classify(decryptedText);

            for (int i = 0; i < NUMBEROFSWAPS; i++)
            {
                string newKey = RandomSwap(key, random);
                string decrypted = cipher.Decrypt(text, Encoding.UTF8.GetBytes(newKey));
                double newClassification = classifier.Classify(decrypted);

                if (newClassification > classification)
                {
                    key = newKey;
                    classification = newClassification;
                }

            }

            yield return Encoding.UTF8.GetBytes(key);

        }

    }

    /* Cryptanalysis for the XOR cipher
     * Calculates Index of Coincidence in order to determine the likely key lengths
     * Splits the text up into slices - sections which have been encrypted using the same key
     * Uses frequency analysis in order to prioritise the most likely keys for each slice
     */
    public class XORCryptanalysis : ICryptanalysis
    {
        private const double IOCTHRESHOLD = 0.06; //This is ~ 1.5 / 26 but will need to justify this in writeup
        private const int MAXKEYLENGTH = 6; //also try to maybe justify

        /* Splits the text up into slices
         */
        private string[] GetSlices(string text, int numSlices)
        {
            string[] slices = new string[numSlices];

            for (int i = 0; i < text.Length; i++)
            {
                slices[i % numSlices] += text[i];
            }

            return slices;
        }

        /* Returns character frequencies for use in frequency analysis
         */
        private Dictionary<char, int> GetCharCounts(string text)
        {
            Dictionary<char, int> textCounts = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (!textCounts.ContainsKey(c))
                {
                    textCounts.Add(c, text.Count(chr => chr == c));
                }
            }

            return textCounts;
        }

        /* Calculation of Index of Coincidence
         * Returns the probability that any two randomly selected characters in the text are the same
         */
        private double CalcIdxOfCoincidence(string textSlice)
        {
            double idxOfCoincidence = 0;
            Dictionary<char, int> charCounts = GetCharCounts(textSlice);

            foreach (KeyValuePair<char, int> kvp in charCounts)
            {
                idxOfCoincidence += (double)kvp.Value * (kvp.Value - 1) / (textSlice.Length * (textSlice.Length - 1));
            }

            return idxOfCoincidence;
        }

        /* Uses Index of Coincidence to estimate the key length
         */
        private IEnumerable<(int, string[])> GetLikelyKeyLength(string text)
        {
            List<int> keyLengths = new List<int>();
            List<double> IoCValues = new List<double>();

            for (int i = 1; i < MAXKEYLENGTH; i++)
            {
                double totalIoC = 0;

                string[] slices = GetSlices(text, i);

                foreach (string slice in slices)
                {
                    totalIoC += CalcIdxOfCoincidence(slice);
                }

                keyLengths.Add(i);
                IoCValues.Add(totalIoC / i);

                if (totalIoC / i > IOCTHRESHOLD)
                {
                    yield return (i, slices);
                }
            }

            while(keyLengths.Count() > 0)
            {
                int idx = IoCValues.IndexOf(IoCValues.Max());
                int keyLength = keyLengths[idx];
                IoCValues.RemoveAt(idx);
                keyLengths.RemoveAt(idx);
                yield return (keyLength, GetSlices(text, keyLength));
            }
        }

        /* Recursive function which cycles through likely keys for a single key character
         */
        private IEnumerable<char> GetSingleKey(string slice, int attempts = 26)
        {
            XOR cipher = new XOR();
            IClassifier classifier = new FrequencyAnalysis();

            List<char> keys = new List<char>();
            List<double> pvalues = new List<double>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                double pvalue = classifier.Classify(cipher.Decrypt(slice, BitConverter.GetBytes(c)));
                keys.Add(c);
                pvalues.Add(pvalue);
            }
            for (int i = 0; i < attempts; i++)
            {
                int idx = pvalues.IndexOf(pvalues.Max());
                char key = keys[idx];
                pvalues.RemoveAt(idx);
                keys.RemoveAt(idx);
                yield return key;
            }
        }

        /* Cycles through possible keys for a particular key length
         */
        private IEnumerable<string> Cycle(int keyLength, int idxInKey, string key, string[] slices)
        {
            foreach (char keyChar in GetSingleKey(slices[idxInKey], 10))
            {
                if (idxInKey == keyLength - 1)
                {
                    yield return key + keyChar;
                }
                else
                {
                    foreach (string keyFound in Cycle(keyLength, idxInKey + 1, key + keyChar, slices))
                    {
                        yield return keyFound;
                    }
                }
            }

        }

        /* Cycles through possible keys and key lengths
         * Converts keys into the appropriate format
         */
        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach ((int keyLength, string[] slices) in GetLikelyKeyLength(text))
            {
                foreach (string possibleKey in Cycle(keyLength, 0, "", slices))
                {
                    yield return Encoding.UTF8.GetBytes(possibleKey);
                }
            }

        }
    }

    /* Returns the cryptanalysis object associated with the cryptanalysis type passed in
     */
    class CryptanalysisFactory
    {
        public static ICryptanalysis GetCryptanalysis(string cryptanalysisType)
        {
            switch (cryptanalysisType)
            {
                case "XORCryptanalysis":
                    return new XORCryptanalysis();
                case "ROT47Cryptanalysis":
                    return new ROT47Cryptanalysis();
                case "ROT13Cryptanalysis":
                    return new ROT13Cryptanalysis();
                case "FasterROT13Cryptanalysis":
                    return new FasterROT13Cryptanalysis();
                case "VigenereCryptanalysis":
                    return new VigenereCryptanalysis();
                case "SubstitutionCryptanalysis":
                    return new SubstitutionCryptanalysis("C:\\Users\\betha\\Code\\nea\\FilesForUse\\EnglishDictionary.txt");
                case "FasterSubstitutionCryptanalysis":
                    return new SubstitutionCryptanalysis("C:\\Users\\betha\\Code\\nea\\FilesForUse\\CommonEnglishWords.txt");
                default:
                    throw new Exception("No valid cryptanalysis selected");
            }
        }
    }

}
