using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public void Run(IConfiguration config)
        {
            Random random = new Random();

            TestResultsStore resultsStore = new TestResultsStore();

            IDataGenerator dataGenerator = DataGeneratorFactory.GetDataGenerator(config.GetStr("dataGenerator"));
            ICipher cipher = CipherFactory.GetCipher(config.GetStr("cipher"));

            double[] results = new double[config.GetInt("iterations")];
            bool[] trueValues = new bool[config.GetInt("iterations")];

            for (int i = 0; i < config.GetInt("iterations"); i++)
            {
                IClassifier classifier = ClassifierFactory.GetClassifier(config.GetStr("classifier"), cipher);

                string text = dataGenerator.GenerateData(random, config.GetInt("textLength"));
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

        public void Run(IConfiguration config)
        {
            Random random = new Random();

            DemoResultsStore resultsStore = new DemoResultsStore();

            IDataGenerator dataGenerator = DataGeneratorFactory.GetDataGenerator(config.GetStr("dataGenerator"));
            ICipher cipher = CipherFactory.GetCipher(config.GetStr("cipher"));

            double[] results = new double[config.GetInt("iterations")];
            bool[] trueValues = new bool[config.GetInt("iterations")];


            bool[] success = new bool[config.GetInt("iterations")];

            for (int i = 0; i < config.GetInt("iterations"); i++)
            {
                IClassifier classifier = ClassifierFactory.GetClassifier(config.GetStr("classifier"), cipher);
                ICryptanalysis cryptanalysis = CryptanalysisFactory.GetCryptanalysis(config.GetStr("cryptanalysis"));

                string plaintext = dataGenerator.GenerateData(random, config.GetInt("textLength"));
                string ciphertext = cipher.Encrypt(plaintext, cipher.GetRandomKey(random));
                string likelyPlaintext;
                success[i] = false;

                foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
                {
                    likelyPlaintext = cipher.Decrypt(ciphertext, key);
                    if (classifier.Classify(likelyPlaintext) >= config.GetDouble("threshold"))
                    {
                        Console.WriteLine(likelyPlaintext);
                        Console.WriteLine(ciphertext);
                        if (likelyPlaintext.ToLower() == plaintext.ToLower())
                        {
                            success[i] = true;
                        }
                        break;
                    }
                }
                if (success[i] == false)
                {
                    Console.WriteLine(plaintext);
                    Console.ReadKey();
                }
                Console.WriteLine($"Success: {success[i]}");
            }

            resultsStore.SaveResults(config, success);

            Console.WriteLine("Press any key to continue:");
            Console.ReadKey();
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
            ShowSuccessRate printSuccess = new ShowSuccessRate();

            printSuccess.Display(new IConfiguration[] { config });

            Console.WriteLine();
            Console.WriteLine("Press any key to continue: ");
            Console.ReadKey();
        }

        public void Run(IConfiguration[] configs)
        {
            ShowSuccessRate printSuccess = new ShowSuccessRate();

            printSuccess.Display(configs);

            Console.WriteLine();
            Console.WriteLine("Press any key to continue: ");
            Console.ReadKey();
        }
    }


}
