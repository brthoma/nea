using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal interface ICryptanalysis
    {
        IEnumerable<byte[]> GetKeys(string text);
    }


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
                    continue;
                }
                textCounts.Add(c, 1);
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

    public class ROT47Cryptanalysis : ICryptanalysis
    {

        private const string ORDEROFCHECK = " etaoinsrhldcumfpgwybvkxjqzETAOINSRHLDCUMFPGWYBVKXJQZ,.";
        private const int ROT47RANGE = 126 - 33;
        private Dictionary<char, int> textCounts = new Dictionary<char, int>();
        
        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach (char c in text)
            {
                if (textCounts.ContainsKey(c))
                {
                    textCounts[c]++;
                    continue;
                }
                textCounts.Add(c, 1);
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

    public class VigenereCryptanalysis : ICryptanalysis
    {
        private const double IOTTHRESHOLD = 1.5;

        private string[] GetSlices(string text, int numSlices)
        {
            string[] slices = new string[numSlices];
            int keyIdx = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]))
                {
                    slices[keyIdx] += text[i];
                    keyIdx = (keyIdx + 1) % numSlices;
                }
            }

            return slices;
        }

        private Dictionary<char, int> GetCharCounts(string text)
        {
            Dictionary<char, int> textCounts = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (!textCounts.ContainsKey(c) && char.IsLetter(c))
                {
                    textCounts.Add(c, text.Count(chr => chr == c));
                }
            }

            return textCounts;
        }

        private double CalcIdxOfCoincidence(string textSlice)
            {
                double idxOfCoincidence = 0;
                Dictionary<char, int> charCounts = GetCharCounts(textSlice);

                foreach (KeyValuePair<char, int> kvp in charCounts)
                {
                    idxOfCoincidence += (double) kvp.Value * (kvp.Value - 1) / ( textSlice.Length * (textSlice.Length - 1) );
                }

                return idxOfCoincidence * 26;
            }

        private IEnumerable<(int, string[])> GetLikelyKeyLength(string text)
        {
            for (int i = 1; i < text.Length; i++)
            {
                double totalIoC = 0;

                string[] slices = GetSlices(text, i);

                foreach (string slice in slices)
                {
                    totalIoC += CalcIdxOfCoincidence(slice);
                }

                if (totalIoC / i > IOTTHRESHOLD) yield return (i, slices);
            }

        }

        private IEnumerable<int> GetSingleKey(string slice, int attempts = 26)
        {
            ROT13 cipher = new ROT13();
            FrequencyAnalysis freqAnalysis = new FrequencyAnalysis();

            List<int> keys = new List<int>();
            List<double> pvalues = new List<double>();

            for(int i = 0; i < 26; i++)
            {
                Console.WriteLine((char) ((int)'A' + i));
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

        private IEnumerable<string> Cycle(int keyLength, int idxInKey, string key, string[] slices)
        {
            Console.WriteLine(idxInKey);

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

        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach ((int keyLength, string[] slices) in GetLikelyKeyLength(text))
            {
                foreach (string possibleKey in Cycle(keyLength, 0, "", slices))
                {
                    Console.WriteLine(possibleKey);
                    yield return Encoding.UTF8.GetBytes(possibleKey);
                }
            }

        }
    }

    public class SubstitutionCryptanalysis : ICryptanalysis
    {

        private const string ORDEROFFREQUENCY = "etaoinsrhldcumfpgwybvkxjqz";

        public string GetCharFreqs(string text)
        {
            string orderOfFreqsInText = "";
            Dictionary<char, int> freqs = new Dictionary<char, int>();
            string lowerText = text.ToLower();

            foreach (char letter in "abcdefghijklmnopqrstuvwxyz")
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

        public IEnumerable<byte[]> GetKeys(string text)
        {
            string freqsInText = GetCharFreqs(text);
            string key = "";

            foreach (char letter in "abcdefghijklmnopqrstuvwxyz")
            {
                key += freqsInText[ORDEROFFREQUENCY.IndexOf(letter)];
            }

            yield return Encoding.UTF8.GetBytes(key);
        }
    }

    public class XORCryptanalysis : ICryptanalysis
    {
        private const double IOTTHRESHOLD = 0.06; //This is ~ 1.5 / 26 but will probably need to justify this in writeup

        private string[] GetSlices(string text, int numSlices)
        {
            string[] slices = new string[numSlices];

            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]))
                {
                    slices[i % numSlices] += text[i];
                }
            }

            return slices;
        }

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
        private IEnumerable<(int, string[])> GetLikelyKeyLength(string text)
        {
            for (int i = 1; i < text.Length; i++)
            {
                double totalIoC = 0;

                string[] slices = GetSlices(text, i);

                foreach (string slice in slices)
                {
                    totalIoC += CalcIdxOfCoincidence(slice);
                }

                if (totalIoC / i > IOTTHRESHOLD) yield return (i, slices);
            }
        }

        private IEnumerable<int> GetSingleKey(string slice, int attempts = 26)
        {
            XOR cipher = new XOR();
            FrequencyAnalysis classifier = new FrequencyAnalysis();

            List<int> keys = new List<int>();
            List<double> pvalues = new List<double>();

            for (int i = 0; i < 26; i++)
            {
                double pvalue = classifier.Classify(cipher.Decrypt(slice, BitConverter.GetBytes(i)));
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

        private IEnumerable<string> Cycle(int keyLength, int idxInKey, string key, string[] slices)
        {
            Console.WriteLine(idxInKey);

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

        public IEnumerable<byte[]> GetKeys(string text)
        {
            foreach ((int keyLength, string[] slices) in GetLikelyKeyLength(text))
            {
                foreach (string possibleKey in Cycle(keyLength, 0, "", slices))
                {
                    Console.WriteLine(possibleKey);
                    yield return Encoding.UTF8.GetBytes(possibleKey);
                }
            }

        }
    }


}
