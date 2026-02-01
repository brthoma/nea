using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace nea
{

    /* Interface definition for a Runner
     * Derived classes perform functions
     * Each derived class performs a different mode
     */
    public interface IRunner
    {
        void Run(IConfiguration config);
    }


    /* Classifier Test Runner
     * Generates plaintext
     * Randomly determines whether to encrypt the plaintext or not
     * Passes the new text through a classifier
     * Test has been successful if the classifier correctly classified the text as natural language or not
     * Tes has been unsuccessful if the classifier incorrectly classified the text
     * Results are stored in a text file
     */
    public class ClassifierTestRunner : IRunner
    {


        public void Run(IConfiguration config)
        {
            Random random = new Random();

            TestResultsHandler resultsStore = new TestResultsHandler();

            IDataGenerator dataGenerator = DataGeneratorFactory.GetDataGenerator(config.GetStr("dataGenerator"));
            ICipher cipher = CipherFactory.GetCipher(config.GetStr("cipher"));
            int iterations = config.GetInt("iterations");

            double[] results = new double[iterations];
            bool[] trueValues = new bool[iterations];

            for (int i = 0; i < iterations; i++)
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

            Console.WriteLine("Press any key to continue:");
            Console.ReadKey();
        }

    }

    /* Demonstration Runner
     * Generates plaintext
     * Encrypts text
     * Cryptanalysis is used to cycle through possible keys
     * For each key, the text is decrypted using that key and passed through a classifier
     * If the text is classified as not natural language, the next key is checked
     * If all keys returned by the cryptanalysis are tried and none are correct, then the test is unsuccessful
     * If text is classified as natural language it is compared with the plaintext
     * If the predicted text was correct, then the test was successful
     * If not, it was unsuccessful
     * Results are stored in a text file
     */
    public class DemoRunner : IRunner
    {

        public void Run(IConfiguration config)
        {
            Random random = new Random();

            DemoResultsHandler resultsStore = new DemoResultsHandler();

            IDataGenerator dataGenerator = DataGeneratorFactory.GetDataGenerator(config.GetStr("dataGenerator"));
            ICipher cipher = CipherFactory.GetCipher(config.GetStr("cipher"));
            int iterations = config.GetInt("iterations");

            double[] results = new double[iterations];
            bool[] trueValues = new bool[iterations];


            bool[] success = new bool[iterations];

            for (int i = 0; i < iterations; i++)
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
                    double classification = classifier.Classify(likelyPlaintext);
                    if (classification >= config.GetDouble("threshold"))
                    {
                        Console.WriteLine("GUESS:");
                        Console.WriteLine(likelyPlaintext);
                        Console.WriteLine("CIPHERTEXT:");
                        Console.WriteLine(ciphertext);

                        if (likelyPlaintext.ToLower() == plaintext.ToLower())
                        {
                            success[i] = true;
                        }
                        break;
                    }
                }
                Console.WriteLine($"Success: {success[i]}");
                Console.WriteLine();
            }

            resultsStore.SaveResults(config, success);

            Console.WriteLine("Press any key to continue:");
            Console.ReadKey();
        }
    }

    /* View Test Results Runner
     * Reads in the test results from a text file
     * Visually displays the results of the test to the user
     * Displays 2 graphs to the user:
     * - Threshold-success
     * - ROC curve
     * Multiple sets of test results can be viewed at once
     */
    public class ViewTestResultsRunner : IRunner
    {
        public void Run(IConfiguration config)
        {
            TestResultsHandler resultsStore = new TestResultsHandler();
            ThresholdSuccessGraph thresholdSuccessGraph = new ThresholdSuccessGraph();
            ROCCurve rocCurve = new ROCCurve();

            (double[] results, bool[] trueValues) = resultsStore.GetResults(config.GetStr("filePath"));

            thresholdSuccessGraph.Display(new IConfiguration[] { config });
            rocCurve.Display(new IConfiguration[] { config });
        }

        public void Run(IConfiguration[] configs)
        {
            ThresholdSuccessGraph thresholdSuccessGraph = new ThresholdSuccessGraph();
            ROCCurve rocCurve = new ROCCurve();

            thresholdSuccessGraph.Display(configs);
            rocCurve.Display(configs);
        }
    }

    /* View Demo Results Runner
     * Reads in the demo results from a text file
     * Visually displays the success rate as a bar graph
     * Multiple sets of demo results can be viewed at once
     */
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
