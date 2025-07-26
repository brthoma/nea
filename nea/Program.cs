using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();


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

            Substitution cipher = new Substitution();
            byte[] key = cipher.GetRandomKey(random);
            string plaintext = "abcdefghijklmnopqrstuvwxyz! The Quick Brown Fox Jumps Over The Lazy Dog.";
            string ciphertext = cipher.Encrypt(plaintext, key);
            Console.WriteLine(Encoding.UTF8.GetString(key));
            Console.WriteLine(plaintext);
            Console.WriteLine(ciphertext);
            Console.WriteLine(cipher.Decrypt(ciphertext, key));

            Console.ReadKey();


            DemoConfiguration demoConfig = new DemoConfiguration();
            DemoRunner runner = new DemoRunner();
            ViewDemoResultsRunner displayResults = new ViewDemoResultsRunner();
            WordsFromDict textGenerator = new WordsFromDict();

            runner.Run(demoConfig);
            displayResults.Run(demoConfig);




            Console.ReadKey();


            TestConfiguration testConfig = new TestConfiguration();
            ClassifierTestRunner testRunner = new ClassifierTestRunner();
            ViewTestResultsRunner viewResults = new ViewTestResultsRunner();

            testRunner.Run(testConfig);

            viewResults.Run(testConfig);

            Console.ReadKey();
        }
    }
}
