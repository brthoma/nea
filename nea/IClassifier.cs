using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nea
{

    /* Interface definition for a Classifier class.
     * Derived classes classify text samples as being English text or random characters.
     */
    public interface IClassifier
    {
        /* Return a value in the range 0-1 indicating whether a piece of text
         * is English or not. Random characters should cause a result close to
         * 0.0, while English text should cause a result close to 1.0.
         */
        double Classify(string text);
    }


    public class RandomGuesser : IClassifier
    {

        public double Classify(string text)
        {
            Random random = new Random();
            return random.NextDouble();
        }

    }

    public class ProportionPrintable : IClassifier
    {

        public double Classify(string text)
        {
            double printable = 0;
            foreach (char c in text)
            {
                if (!char.IsControl(c) && !char.IsSurrogate(c))
                {
                    printable++;
                }
            }

            double proportion = printable / text.Length;
            return proportion;
        }

    }

    public class DictionaryLookup : IClassifier
    {
        private const string DICTIONARYFILEPATH = "FilesForUse\\EnglishDictionary.txt";
        private string[] dictionary;

        public DictionaryLookup()
        {
            dictionary = File.ReadAllLines(DICTIONARYFILEPATH);

            for (int i = 0; i < dictionary.Length; i++)
            {
                dictionary[i] = dictionary[i].ToLower();
            }

            Array.Sort(dictionary);
        }

        private bool BinarySearch(string target)
        {
            int start = 0;
            int end = dictionary.Length - 1;

            while (start <= end)
            {
                int middle = (start + end) / 2;

                int comparison = String.Compare(dictionary[middle], target, comparisonType : StringComparison.OrdinalIgnoreCase);
                if (comparison == 0)
                {
                    return true;
                }
                else if (comparison < 0)
                {
                    start = middle + 1;
                }
                else
                {
                    end = middle - 1;
                }
            }
            return false;
        }

        public double Classify(string text)
        {
            int inDictionary = 0;
            int numWords = 0;


            foreach (Match match in Regex.Matches(text, "[a-zA-Z]+"))
            {
                string word = match.Value;
                numWords++;
                if (BinarySearch(word))
                {
                    inDictionary++;
                }
            }

            return (double) inDictionary / numWords;
        }
    }

    public class FrequencyAnalysis : IClassifier
    {
        private const string GAMMAFUNCTLOOKUP = "FilesForUse\\LookupGammaFunct.txt";
        private const string EXPECTEDFREQUENCIES = "FilesForUse\\EnglishLetterDistribution.txt";
        private const int OPTIMALN = 100;
        private const int LETTERSINALPHABET = 26;
        private const int NUMINTERVALS = 1000;
        private Dictionary<double, double> LookupGammaFunct = new Dictionary<double, double>();
        private double[] expectedEngDistribution = new double[LETTERSINALPHABET];

        public FrequencyAnalysis()
        {
            using(StreamReader sr = new StreamReader(GAMMAFUNCTLOOKUP))
            {
                while (!sr.EndOfStream)
                {
                    string[] kvp = sr.ReadLine().Trim().Split('|');
                    LookupGammaFunct.Add(double.Parse(kvp[0]), double.Parse(kvp[1]));
                }
                sr.Close();
            }

            using (StreamReader sr = new StreamReader(EXPECTEDFREQUENCIES))
            {
                for (int i = 0; i < LETTERSINALPHABET; i++)
                {
                    expectedEngDistribution[i] = double.Parse(sr.ReadLine().Trim().Split('|')[1]);
                }
                sr.Close();
            }
        }

        public double Classify(string text)
        {
            int numLetters = text.Count(c => "abcdefghijklmnopqrstuvwxyz".Contains(char.ToLower(c)));

            double[] expectedFreqs = new double[LETTERSINALPHABET];
            for (int i = 0; i < LETTERSINALPHABET; i++)
            {
                expectedFreqs[i] = expectedEngDistribution[i] * OPTIMALN;
            }

            double[] observedFreqs = new double[LETTERSINALPHABET];
            for (int i = 0; i < LETTERSINALPHABET; i++)
            {
                observedFreqs[i] = (double) text.Count(c => char.ToLower(c) == (char)('a' + i)) * ((double) OPTIMALN / numLetters);
            }

            (double[] combinedObsFreqs, double[] combinedExpFreqs) = Statistics.CombineChiSquaredClasses(observedFreqs, expectedFreqs, OPTIMALN);

            int degFreedom = combinedObsFreqs.Length - 1;
            if (degFreedom == 0)
            {
                throw new Exception("Text too short: insufficient information to classify.");
            }

            double pValue = Statistics.GetPValue(combinedObsFreqs, combinedExpFreqs, degFreedom, NUMINTERVALS, LookupGammaFunct);

            return pValue;
        }

    }

    public class WordLength : IClassifier
    {
        private const string GAMMAFUNCTLOOKUP = "FilesForUse\\LookupGammaFunct.txt";
        private const string EXPECTEDLENGTHS = "FilesForUse\\ExpectedWordLengths.txt";
        private const int MAXENGWORDLENGTH = 20;
        private const int OPTIMALN = 100;
        private const int NUMINTERVALS = 1000;
        private Dictionary<double, double> LookupGammaFunct = new Dictionary<double, double>();
        private double[] expectedEngDistribution = new double[MAXENGWORDLENGTH + 1];

        public WordLength()
        {
            using (StreamReader sr = new StreamReader(GAMMAFUNCTLOOKUP))
            {
                while (!sr.EndOfStream)
                {
                    string[] kvp = sr.ReadLine().Trim().Split('|');
                    LookupGammaFunct.Add(double.Parse(kvp[0]), double.Parse(kvp[1]));
                }
                sr.Close();
            }

            using (StreamReader sr = new StreamReader(EXPECTEDLENGTHS))
            {
                for (int i = 0; i < MAXENGWORDLENGTH + 1; i++)
                {
                    expectedEngDistribution[i] = double.Parse(sr.ReadLine().Trim().Split('|')[1]);
                }
                sr.Close();
            }
        }

        public double Classify(string text)
        {
            int numWords = 0;

            double[] expectedFreqs = new double[MAXENGWORDLENGTH + 1];
            for (int i = 0; i < MAXENGWORDLENGTH + 1; i++) expectedFreqs[i] = expectedEngDistribution[i] * OPTIMALN;

            double[] observedFreqs = new double[MAXENGWORDLENGTH + 1];
            int currentWordLength = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]))
                {
                    currentWordLength++;
                }
                else
                {
                    if (currentWordLength != 0)
                    {

                        if (currentWordLength <= MAXENGWORDLENGTH)
                        {
                            observedFreqs[currentWordLength - 1]++;
                        }
                        else
                        {
                            observedFreqs[MAXENGWORDLENGTH]++;
                        }
                        currentWordLength = 0;
                        numWords++;
                    }
                }
            }
            if (currentWordLength != 0 && currentWordLength <= MAXENGWORDLENGTH)
            {
                observedFreqs[currentWordLength - 1]++;
            }
            for (int i = 0; i < MAXENGWORDLENGTH + 1; i++)
            {
                observedFreqs[i] *= ((double)OPTIMALN / numWords);
            }

            (double[] combinedObsFreqs, double[] combinedExpFreqs) = Statistics.CombineChiSquaredClasses(observedFreqs, expectedFreqs, OPTIMALN);

            int degFreedom = combinedObsFreqs.Length - 1;
            if (degFreedom == 0)
            {
                throw new Exception("Text too short: insufficient information to classify.");
            }

            double pValue = Statistics.GetPValue(combinedObsFreqs, combinedExpFreqs, degFreedom, NUMINTERVALS, LookupGammaFunct);

            return pValue;
        }

    }

    public class Bigrams : IClassifier
    {
        private const string GAMMAFUNCTLOOKUP = "FilesForUse\\LookupGammaFunct.txt";
        private const string COMMONBIGRAMS = "FilesForUse\\CommonBigrams.txt";
        private const int NUMBIGRAMSINFILE = 50;
        private const int OPTIMALN = 100;
        private const int NUMINTERVALS = 1000;
        private Dictionary<double, double> LookupGammaFunct = new Dictionary<double, double>();
        private Dictionary<string, (double, int)> bigramsExpAndObs = new Dictionary<string, (double, int)>();


        public Bigrams()
        {
            using (StreamReader sr = new StreamReader(GAMMAFUNCTLOOKUP))
            {
                while (!sr.EndOfStream)
                {
                    string[] kvp = sr.ReadLine().Trim().Split('|');
                    LookupGammaFunct.Add(double.Parse(kvp[0]), double.Parse(kvp[1]));
                }
                sr.Close();
            }

            using (StreamReader sr = new StreamReader(COMMONBIGRAMS))
            {
                for (int i = 0; i < NUMBIGRAMSINFILE + 1; i++)
                {
                    string[] bigramFreq = sr.ReadLine().Trim().Split('|');
                    bigramsExpAndObs.Add(bigramFreq[0], (double.Parse(bigramFreq[1]), 0));
                }
                sr.Close();
            }
        }

        public double Classify(string text)
        {
            int numBigrams = 0;

            for (int i = 0; i < text.Length - 1; i++)
            {
                if (!(char.IsLetter(text[i]) && char.IsLetter(text[i + 1])))
                {
                    continue;
                }

                string bigram = text[i].ToString() + text[i + 1].ToString();
                numBigrams++;

                if (bigramsExpAndObs.ContainsKey(bigram.ToUpper()))
                {
                    bigramsExpAndObs[bigram.ToUpper()] = (bigramsExpAndObs[bigram.ToUpper()].Item1, bigramsExpAndObs[bigram.ToUpper()].Item2 + 1);
                }
                else
                {
                    bigramsExpAndObs["OTHER"] = (bigramsExpAndObs["OTHER"].Item1, bigramsExpAndObs["OTHER"].Item2 + 1);
                }
            }

            double[] expectedFreqs = new double[NUMBIGRAMSINFILE + 1];
            double[] observedFreqs = new double[NUMBIGRAMSINFILE + 1];

            int j = 0;
            foreach (KeyValuePair<string, (double, int)> kvp in bigramsExpAndObs)
            {
                expectedFreqs[j] = kvp.Value.Item1 * OPTIMALN;
                if (numBigrams == 0)
                {
                    observedFreqs[j] = 0;
                }
                else
                {
                    observedFreqs[j] = kvp.Value.Item2 * ((double)OPTIMALN / numBigrams);
                }
                j++;
            }

            (double[] combinedObsFreqs, double[] combinedExpFreqs) = Statistics.CombineChiSquaredClasses(observedFreqs, expectedFreqs, OPTIMALN);

            int degFreedom = combinedObsFreqs.Length - 1;
            if (degFreedom == 0)
            {
                throw new Exception("Text too short: insufficient information to classify.");
            }

            double pValue = Statistics.GetPValue(combinedObsFreqs, combinedExpFreqs, degFreedom, NUMINTERVALS, LookupGammaFunct);

            return pValue;
        }


    }

    public class Entropy : IClassifier
    {
        private const int MINRANGE = 32;
        private const int MAXRANGE = 126;
        private const double CLOSETOENGLISH = 4.25;
        private const double MAXENTROPY = 8.0;

        public double Classify(string text)
        {
            int[] occurrences = new int[MAXRANGE - MINRANGE + 1];
            double entropy = 0;
            double probability;

            foreach (char c in text)
            {
                if (MINRANGE <= c && c <= MAXRANGE)
                {
                    occurrences[c - MINRANGE]++;
                }
            }

            for (int i = 0; i < occurrences.Length; i++)
            {
                double p = (double) occurrences[i] / text.Length;
                if (p != 0)
                {
                    entropy += p * Math.Log(p, 2);
                }
            }

            entropy = - entropy;

            if (entropy <= CLOSETOENGLISH)
            {
                probability = 1 - (CLOSETOENGLISH - entropy) / CLOSETOENGLISH;
            }
            else
            {
                probability = 1 - (entropy - CLOSETOENGLISH) / (MAXENTROPY - CLOSETOENGLISH);
            }

            return probability;
        }

    }

    public class Ensemble : IClassifier
    {
        private IClassifier[] classifiers;
        private double[] weights;

        public Ensemble(IClassifier[] classifiers, double[] weights)
        {
            this.classifiers = classifiers;
            this.weights = weights;
        }

        public double Classify(string text)
        {
            double weightedAverage = 0;

            for (int i = 0; i < classifiers.Length; i++)
            {
                weightedAverage += weights[i] * classifiers[i].Classify(text);
            }

            return weightedAverage;
        }

    }


    class ClassifierFactory
    {
        public static IClassifier GetClassifier(string classifierType, ICipher cipher)
        {
            switch (classifierType)
            {
                case "RandomGuesser":
                    return new RandomGuesser();
                case "ProportionPrintable":
                    return new ProportionPrintable();
                case "DictionaryLookup":
                    return new DictionaryLookup();
                case "FrequencyAnalysis":
                    return new FrequencyAnalysis();
                case "Bigrams":
                    return new Bigrams();
                case "WordLength":
                    return new WordLength();
                case "Entropy":
                    return new Entropy();
                case "MajorityVoteEnsemble":
                    IClassifier[] classifiers = new IClassifier[] { new RandomGuesser(), new ProportionPrintable(), new DictionaryLookup(), new FrequencyAnalysis(), new Entropy() };
                    MajVoting trainer = new MajVoting();
                    return new Ensemble(classifiers, trainer.GetWeights(classifiers, cipher));
                default:
                    throw new Exception("No valid classifier selected");
            }
        }
    }

}
