using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XPlot.Plotly;

namespace nea
{
    internal class Program
    {

        static string GetChoice(string[] choices, string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.WriteLine();

            for (int i = 0; i < choices.Length; i++)
            {
                Console.WriteLine($"    {choices[i]}");
            }

            int line = 2;

            while (true)
            {
                Console.SetCursorPosition(2, line);
                Console.Write(">");

                ConsoleKey keyPressed = Console.ReadKey().Key;

                switch (keyPressed)
                {
                    case ConsoleKey.DownArrow:
                        if (line < 1 + choices.Length)
                        {
                            Console.SetCursorPosition(2, line);
                            Console.Write(" ");
                            line++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (line > 2)
                        {
                            Console.SetCursorPosition(2, line);
                            Console.Write(" ");
                            line--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return choices[line - 2];
                }
            }
        }

        static int GetIntInput(string message)
        {
            while (true)
            {
                Console.Clear();
                Console.Write(message);
                try
                {
                    int input = int.Parse(Console.ReadLine());
                    Console.Clear();
                    return input;
                }
                catch
                {
                    continue;
                }
            }
        }

        static double GetDoubleInput(string message)
        {
            while (true)
            {
                Console.Clear();
                Console.Write(message);
                try
                {
                    double input = double.Parse(Console.ReadLine());
                    Console.Clear();
                    return input;
                }
                catch
                {
                    continue;
                }
            }
        }

        static string GetStringInput(string message)
        {
            Console.Write(message);
            string input = Console.ReadLine();
            Console.Clear();
            return input;
        }


        static void Main(string[] args)
        {
 

            Random random = new Random();

            //bool cont = true;

            //while (cont)
            //{
            //    string mode = GetChoice(new string[] { "Test Classifiers", "Demonstrate success", "View results", "Exit" }, "Choose a mode: ");
            //    int numExperiments = GetIntInput("Enter number of experiments to run: ");

            //    switch (mode)
            //    {
            //        case "Test Classifiers":
            //            TestConfiguration[] configs = new TestConfiguration[numExperiments];

            //            for (int i = 0; i < numExperiments; i++)
            //            {
            //                string filePath = GetStringInput("Enter file name: ") + ".txt";
            //                int textLength = GetIntInput("Enter text length: ");
            //                int iterations = GetIntInput("Enter number of iterations: ");
            //                string dataGenerator = GetChoice(new string[] { "WordsFromDict" }, "Enter data generator: ");
            //                string cipher = GetChoice(new string[] { "XOR", "ROT47", "ROT13", "Vigenere", "Substitution" }, "Enter cipher: ");
            //                string classifier = GetChoice(new string[] { "RandomGuesser", "ProportionPrintable", "DictionaryLookup", "FrequencyAnalysis", "Entropy", "MajorityVoteEnsemble" }, "Enter classifier: ");

            //                configs[i] = new TestConfiguration(filePath, textLength, iterations, dataGenerator, cipher, classifier);

            //                ClassifierTestRunner runner = new ClassifierTestRunner();
            //                runner.Run(configs[i]);
            //            }
                            
            //                ViewTestResultsRunner results = new ViewTestResultsRunner();

            //                results.Run(configs);

            //            break;

            //        case "Demonstrate success":
            //            throw new NotImplementedException();
            //            break;
            //        case "View results":
            //            throw new NotImplementedException();
            //            break;
            //        case "Exit":
            //            cont = false;
            //            break;
            //    }

            //}

            //Console.ReadKey();






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


            DemoConfiguration demoConfig = new DemoConfiguration();
            DemoRunner runner = new DemoRunner();
            ViewDemoResultsRunner displayResults = new ViewDemoResultsRunner();
            WordsFromDict textGen = new WordsFromDict();

            runner.Run(demoConfig);
            displayResults.Run(demoConfig);

            Console.ReadKey();



            WordsFromDict textGenerator = new WordsFromDict();

            TestConfiguration testConfig = new TestConfiguration();
            TestConfiguration testConfig2 = new TestConfiguration();
            ClassifierTestRunner testRunner = new ClassifierTestRunner();
            ViewTestResultsRunner displayTestResults = new ViewTestResultsRunner();

            testRunner.Run(testConfig);
            testRunner.Run(testConfig2);
            displayTestResults.Run(new TestConfiguration[] { testConfig, testConfig2 });




            //Console.ReadKey();


            //TestConfiguration testConfig = new TestConfiguration();
            //ClassifierTestRunner testRunner = new ClassifierTestRunner();
            //ViewTestResultsRunner viewResults = new ViewTestResultsRunner();

            //testRunner.Run(testConfig);

            //viewResults.Run(testConfig);

            Console.ReadKey();
        }
    }
}
