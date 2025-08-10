using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace nea
{

    public interface IRunner
    {
        void Run(IConfiguration config);
    }


    public class ClassifierTestRunner : IRunner
    {

        private const string DICTIONARYFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\EnglishDictionary.txt";

        public void Run(IConfiguration config)
        {
            Random random = new Random();

            TestResultsStore resultsStore = new TestResultsStore();

            IDataGenerator dataGenerator;
            ICipher cipher;
            IClassifier classifier;

            double[] results = new double[config.GetInt("iterations")];
            bool[] trueValues = new bool[config.GetInt("iterations")];

            switch (config.GetStr("dataGenerator"))
            {
                case "WordsFromDict":
                    dataGenerator = new WordsFromDict();
                    break;
                default:
                    throw new Exception("No valid data generator selected");
            }
            
            switch (config.GetStr("cipher"))
            {
                case "XOR":
                    cipher = new XOR();
                    break;
                case "ROT47":
                    cipher = new ROT47();
                    break;
                case "ROT13":
                    cipher = new ROT13();
                    break;
                case "Vigenere":
                    cipher = new Vigenere();
                    break;
                case "Substitution":
                    cipher = new Substitution();
                    break;
                default:
                    throw new Exception("No valid cipher selected");
            }

            switch (config.GetStr("classifier"))
            {
                case "RandomGuesser":
                    classifier = new RandomGuesser();
                    break;
                case "ProportionPrintable":
                    classifier = new ProportionPrintable();
                    break;
                case "DictionaryLookup":
                    classifier = new DictionaryLookup();
                    break;
                case "FrequencyAnalysis":
                    classifier = new FrequencyAnalysis();
                    break;
                case "Entropy":
                    classifier = new Entropy();
                    break;
                case "MajorityVoteEnsemble":
                    IClassifier[] classifiers = new IClassifier[] { new RandomGuesser(), new ProportionPrintable(), new DictionaryLookup(), new FrequencyAnalysis(), new Entropy() };
                    MajVoting trainer = new MajVoting();
                    classifier = new Ensemble(classifiers, trainer.GetWeights(classifiers, cipher));
                    break;
                default:
                    throw new Exception("No valid classifier selected");
            }

            for (int i = 0; i < config.GetInt("iterations"); i++)
            {
                string text = dataGenerator.GenerateData(DICTIONARYFILEPATH, random, config.GetInt("textLength"));
                if (random.Next(2) == 0)
                {
                    text = cipher.Encrypt(text, cipher.GetRandomKey(random));
                    Console.WriteLine("Not English");
                    trueValues[i] = false;
                }
                else
                {
                    Console.WriteLine("English");
                    trueValues[i] = true;
                }
                results[i] = classifier.Classify(text);
            }

            resultsStore.SaveResults(config, results, trueValues);
        }

    }

    public class DemoRunner : IRunner
    {
        private const string DICTIONARYFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\EnglishDictionary.txt";

        public void Run(IConfiguration config)
        {
            Random random = new Random();

            DemoResultsStore resultsStore = new DemoResultsStore();

            IDataGenerator dataGenerator;
            ICipher cipher;
            IClassifier classifier;

            double[] results = new double[config.GetInt("iterations")];
            bool[] trueValues = new bool[config.GetInt("iterations")];

            switch (config.GetStr("dataGenerator"))
            {
                case "WordsFromDict":
                    dataGenerator = new WordsFromDict();
                    break;
                default:
                    throw new Exception("No valid data generator selected");
            }

            switch (config.GetStr("cipher"))
            {
                case "XOR":
                    cipher = new XOR();
                    break;
                case "ROT47":
                    cipher = new ROT47();
                    break;
                case "ROT13":
                    cipher = new ROT13();
                    break;
                case "Vigenere":
                    cipher = new Vigenere();
                    break;
                case "Substitution":
                    cipher = new Substitution();
                    break;
                default:
                    throw new Exception("No valid cipher selected");
            }

            switch (config.GetStr("classifier"))
            {
                case "RandomGuesser":
                    classifier = new RandomGuesser();
                    break;
                case "ProportionPrintable":
                    classifier = new ProportionPrintable();
                    break;
                case "DictionaryLookup":
                    classifier = new DictionaryLookup();
                    break;
                case "FrequencyAnalysis":
                    classifier = new FrequencyAnalysis();
                    break;
                case "Entropy":
                    classifier = new Entropy();
                    break;
                case "MajorityVoteEnsemble":
                    IClassifier[] classifiers = new IClassifier[] { new RandomGuesser(), new ProportionPrintable(), new DictionaryLookup(), new FrequencyAnalysis(), new Entropy() };
                    MajVoting trainer = new MajVoting();
                    classifier = new Ensemble(classifiers, trainer.GetWeights(classifiers, cipher));
                    break;
                default:
                    throw new Exception("No valid classifier selected");
            }

            bool[] success = new bool[config.GetInt("iterations")];

            for (int i = 0; i < config.GetInt("iterations"); i++)
            {
                ICryptanalysis cryptanalysis = new VigenereCryptanalysis(); //JUST FOR NOW WHILE I ONLY HAVE ONE
                string plaintext = dataGenerator.GenerateData(DICTIONARYFILEPATH, random, config.GetInt("textLength"));
                string ciphertext = cipher.Encrypt(plaintext, cipher.GetRandomKey(random));
                string likelyPlaintext;
                success[i] = false;

                foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
                {
                    likelyPlaintext = cipher.Decrypt(ciphertext, key);
                    if (classifier.Classify(likelyPlaintext) >= config.GetDouble("threshold"))
                    {
                        if (likelyPlaintext == plaintext) success[i] = true;
                        break;
                    }
                }
            }

            resultsStore.SaveResults(config, success);
        }
    }

    public class ViewTestResultsRunner : IRunner
    {
        public void Run(IConfiguration config)
        {
            TestResultsStore resultsStore = new TestResultsStore();
            ThresholdSuccessGraph thresholdSuccessGraph = new ThresholdSuccessGraph();
            ROCCurve rocCurve = new ROCCurve();
            DETCurve detCurve = new DETCurve();

            (double[] results, bool[] trueValues) = resultsStore.GetResults(config.GetStr("filePath"));

            thresholdSuccessGraph.Display(new IConfiguration[] { config });
            rocCurve.Display(new IConfiguration[] { config });
            detCurve.Display(new IConfiguration[] { config });
        }

        public void Run(IConfiguration[] configs)
        {
            TestResultsStore resultsStore = new TestResultsStore();
            ThresholdSuccessGraph thresholdSuccessGraph = new ThresholdSuccessGraph();
            ROCCurve rocCurve = new ROCCurve();
            DETCurve detCurve = new DETCurve();

            thresholdSuccessGraph.Display(configs);
            rocCurve.Display(configs);
            detCurve.Display(configs);
        }
    }

    public class ViewDemoResultsRunner : IRunner
    {
        public void Run(IConfiguration config)
        {
            DemoResultsStore resultsStore = new DemoResultsStore();
            PrintSuccessRate printSuccess = new PrintSuccessRate();

            bool[] success = resultsStore.GetResults(config.GetStr("filePath"));

            printSuccess.Display(new IConfiguration[] { config });
        }

        public void Run(IConfiguration[] configs)
        {
            DemoResultsStore resultsStore = new DemoResultsStore();
            PrintSuccessRate printSuccess = new PrintSuccessRate();

            foreach (IConfiguration config in configs)
            {
                bool[] success = resultsStore.GetResults(config.GetStr("filePath"));

                printSuccess.Display(new IConfiguration[] { config });
            }
        }
    }


}
