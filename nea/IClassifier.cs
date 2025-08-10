using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nea
{

    public interface IClassifier
    {
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
                if ( !char.IsControl(c) && !char.IsSurrogate(c) ) printable++;
            }

            double proportion = printable / text.Length;
            return proportion;
        }

    }

    public class DictionaryLookup : IClassifier
    {
        private const string DICTIONARYFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\EnglishDictionary.txt";

        public double Classify(string text)
        {
            int inDictionary = 0;
            string[] dictionary = File.ReadAllLines(DICTIONARYFILEPATH);
            int numWords = 0;


            foreach (Match match in Regex.Matches(text, "[a-zA-Z]+"))
            {
                string word = match.Value;
                numWords++;
                if (dictionary.Contains(word, StringComparer.OrdinalIgnoreCase))
                {
                    inDictionary++;
                }
            }

            return (double) inDictionary / numWords;
        }
    }

    public class FrequencyAnalysis : IClassifier
    {

        private const int OPTIMALN = 100;
        private const int LETTERSINALPHABET = 26;
        private const int NUMINTERVALS = 1000;
        private Dictionary<double, double> LookupGammaFunct = new Dictionary<double, double>();
        private double[] expectedEngDistribution = new double[LETTERSINALPHABET];

        public FrequencyAnalysis()
        {
            using(StreamReader sr = new StreamReader("C:\\Users\\betha\\Code\\nea\\nea\\LookupGammaFunct.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string[] kvp = sr.ReadLine().Trim().Split('|');
                    LookupGammaFunct.Add(double.Parse(kvp[0]), double.Parse(kvp[1]));
                }
            }
            using (StreamReader sr = new StreamReader("C:\\Users\\betha\\Code\\nea\\nea\\EnglishLetterDistribution.txt"))
            {
                for (int i = 0; i < LETTERSINALPHABET; i++)
                {
                    expectedEngDistribution[i] = double.Parse(sr.ReadLine().Trim().Split('|')[1]);
                }
            }
        }

        private double ChiSquared(double[] observedFreqs, double[] expectedFreqs)
        {
            double chiSquared = 0;
            for (int i = 0; i < observedFreqs.Length; i++)
            {
                chiSquared += Math.Pow(observedFreqs[i] - expectedFreqs[i], 2) / expectedFreqs[i];
            }

            return chiSquared;
        }

        private double CDF(int degFreedom, double chiSquared)
        {
            //double cdf = ɣ((double)degFreedom / 2, chiSquared / 2) / LookupΓ[(double)degFreedom / 2];
            double cdf = SimpsonLowerIncompleteGammaFunct((double)degFreedom / 2, chiSquared / 2) / LookupGammaFunct[(double)degFreedom / 2];
            Console.WriteLine($"degFreedom = {degFreedom}, chiSquared = {chiSquared}; cdf = {cdf}");
            return cdf;
        }

        private double LowerIncompleteGammaFunct(double s, double x)
        {
            double intervalWidth = x / NUMINTERVALS;
            double result = 0;

            for (int i = 1; i < NUMINTERVALS; i++)
            {
                result += Math.Pow(i * intervalWidth, s - 1) * Math.Pow(Math.E, -i * intervalWidth);
            }
            result *= 2;
            result += NUMINTERVALS * intervalWidth * Math.Pow(Math.E, -intervalWidth);
            result *= intervalWidth / 2;

            return result;
        }

        private double f(double s, double x)
        {
            return Math.Pow(x, s - 1) * Math.Pow(Math.E, -x);
        }

        private double SimpsonLowerIncompleteGammaFunct(double s, double x)
        {
            double intervalWidth = x / NUMINTERVALS;
            double result = 0;

            for (int i = 0; i < NUMINTERVALS; i++)
            {
                double a = i * intervalWidth;
                double b = (i + 1) * intervalWidth;
                result += ((b - a) / 6) * (f(s, a) + 4 * f(s, ((a + b) / 2)) + f(s, b));
            }

            return result;
        }

        public (double[], double[]) CombineClasses(double[] observedFreqs, double[] expectedFreqs)
        {
            List<double> observedFreqList = observedFreqs.ToList();
            List<double> expectedFreqList = expectedFreqs.ToList();

            while ((double)expectedFreqList.Min() < 0.05 * OPTIMALN)
            {
                double minExpectedFreq = expectedFreqList.Min();
                int removeAtIdx = expectedFreqList.IndexOf(minExpectedFreq);
                double minObservedFreq = observedFreqList[removeAtIdx];

                observedFreqList.RemoveAt(removeAtIdx);
                expectedFreqList.RemoveAt(removeAtIdx);

                int addAtIdx = expectedFreqList.IndexOf(expectedFreqList.Min());
                expectedFreqList[addAtIdx] += minExpectedFreq;
                observedFreqList[addAtIdx] += minObservedFreq;
            }

            return (observedFreqList.ToArray(), expectedFreqList.ToArray());
        }

        public double Classify(string text)
        {
            int numLetters = text.Count(c => "abcdefghijklmnopqrstuvwxyz".Contains(char.ToLower(c)));

            double[] expectedFreqs = new double[LETTERSINALPHABET];
            for (int i = 0; i < LETTERSINALPHABET; i++) expectedFreqs[i] = expectedEngDistribution[i] * OPTIMALN;

            double[] observedFreqs = new double[LETTERSINALPHABET];
            for (int i = 0; i < LETTERSINALPHABET; i++)
            {
                observedFreqs[i] = (double) text.Count(c => char.ToLower(c) == (char)('a' + i)) * ((double) OPTIMALN / numLetters);
            }

            (double[] combinedObsFreqs, double[] combinedExpFreqs) = CombineClasses(observedFreqs, expectedFreqs);

            int degFreedom = combinedObsFreqs.Length - 1;
            if (degFreedom == 0) throw new Exception("Text too short: insufficient information to classify.");

            return 1 - CDF(degFreedom, ChiSquared(combinedObsFreqs, combinedExpFreqs));
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
                if (p != 0) entropy += p * Math.Log(p, 2);
            }

            entropy = - entropy;

            if (entropy <= CLOSETOENGLISH) probability = 1 - (CLOSETOENGLISH - entropy) / CLOSETOENGLISH;
            else probability = 1 - (entropy - CLOSETOENGLISH) / (MAXENTROPY - CLOSETOENGLISH);

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


}
