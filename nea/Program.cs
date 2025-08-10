using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XPlot.Plotly;

namespace nea
{
    internal class Program
    {
        private const string TESTFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\TestFiles.txt";
        private const string DEMOFILEPATH = "C:\\Users\\betha\\Code\\nea\\nea\\DemoFiles.txt";

        static void Main(string[] args)
        {
 

            Random random = new Random();

            bool cont = true;

            while (cont)
            {
                string mode = UI.GetChoice(new string[] { "Test Classifiers", "Demonstrate success", "View classifier test results", "View demonstration results", "Exit" }, "Choose a mode: ");

                switch (mode)
                {
                    case "Test Classifiers":
                        TestConfiguration testConfig = new TestConfiguration();
                        ClassifierTestRunner testRunner = new ClassifierTestRunner();

                        testRunner.Run(testConfig);

                        Console.Clear();

                        break;

                    case "Demonstrate success":
                        DemoConfiguration demoConfig = new DemoConfiguration();
                        DemoRunner demoRunner = new DemoRunner();

                        demoRunner.Run(demoConfig);

                        Console.Clear();

                        break;

                    case "View classifier test results":
                        TestResultsStore testResultsStore = new TestResultsStore();
                        ViewTestResultsRunner viewTestResultsRunner = new ViewTestResultsRunner();

                        string[] testsInfo = UI.GetChoices(File.ReadAllLines(TESTFILEPATH).Reverse().ToArray(), "Choose file to view: ").ToArray();
                        IConfiguration[] testConfigs = new IConfiguration[testsInfo.Length];

                        for (int i = 0; i < testsInfo.Length; i++)
                        {
                            string testFilePath = testsInfo[i].Split()[3];
                            testConfigs[i] = testResultsStore.GetConfiguration(testFilePath);
                        }

                        viewTestResultsRunner.Run(testConfigs);

                        break;

                    case "View demonstration results":
                        DemoResultsStore demoResultsStore = new DemoResultsStore();
                        ViewDemoResultsRunner viewDemoResultsRunner = new ViewDemoResultsRunner();

                        string[] demoInfo = UI.GetChoices(File.ReadAllLines(DEMOFILEPATH).Reverse().ToArray(), "Choose file to view: ").ToArray();
                        IConfiguration[] demoConfigs = new IConfiguration[demoInfo.Length];

                        for (int i = 0; i < demoInfo.Length; i++)
                        {
                            string demoFilePath = demoInfo[i].Split()[3];
                            demoConfigs[i] = demoResultsStore.GetConfiguration(demoFilePath);
                        }

                        viewDemoResultsRunner.Run(demoConfigs);

                        break;

                    case "Exit":
                        cont = false;

                        break;

                }

            }

            Console.ReadKey();






            ////////////////////////////////////////////////////////////////////////////////////////////////////////


            //Vigenere cipher = new Vigenere();
            //byte[] key = cipher.GetRandomKey(random, 4);
            //string plaintext = "abcABC xyzXYZ .,!~#!£$%^&*() aaaaaaaaaaaaaa";
            //string ciphertext = cipher.Encrypt(plaintext, key);
            //Console.WriteLine(Encoding.UTF8.GetString(key));
            //Console.WriteLine(plaintext);
            //Console.WriteLine(ciphertext);
            //Console.WriteLine(cipher.Decrypt(ciphertext, key));
            //Console.ReadKey();

            //Entropy entropy = new Entropy();
            //while (true)
            //{
            //    Console.WriteLine("Entropy : " + entropy.Classify(Console.ReadLine()));
            //}

            //Substitution cipher = new Substitution();
            //byte[] key = cipher.GetRandomKey(random);
            //string plaintext = "abcdefghijklmnopqrstuvwxyz! The Quick Brown Fox Jumps Over The Lazy Dog.";
            //string ciphertext = cipher.Encrypt(plaintext, key);
            //Console.WriteLine(Encoding.UTF8.GetString(key));
            //Console.WriteLine(plaintext);
            //Console.WriteLine(ciphertext);
            //Console.WriteLine(cipher.Decrypt(ciphertext, key));

            //Console.ReadKey();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //DemoConfiguration demoConfig = new DemoConfiguration();
            //DemoRunner runner = new DemoRunner();
            //ViewDemoResultsRunner displayResults = new ViewDemoResultsRunner();
            //WordsFromDict textGen = new WordsFromDict();

            //runner.Run(demoConfig);
            //displayResults.Run(demoConfig);

            //Console.ReadKey();



            //WordsFromDict textGenerator = new WordsFromDict();

            //TestConfiguration testConfig = new TestConfiguration();
            //TestConfiguration testConfig2 = new TestConfiguration();
            //ClassifierTestRunner testRunner = new ClassifierTestRunner();
            //ViewTestResultsRunner displayTestResults = new ViewTestResultsRunner();

            //testRunner.Run(testConfig);
            //testRunner.Run(testConfig2);
            //displayTestResults.Run(new TestConfiguration[] { testConfig, testConfig2 });

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //Console.ReadKey();


            //TestConfiguration testConfig = new TestConfiguration();
            //ClassifierTestRunner testRunner = new ClassifierTestRunner();
            //ViewTestResultsRunner viewResults = new ViewTestResultsRunner();

            //testRunner.Run(testConfig);

            //viewResults.Run(testConfig);

            //Console.ReadKey();
        }
    }
}
