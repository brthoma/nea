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
        private const string TESTFILEPATH = "FilesForUse\\TestFiles.txt";
        private const string DEMOFILEPATH = "FilesForUse\\DemoFiles.txt";


        // Main menu: Choose mode and get configuration to pass into runners

        static void Main(string[] args)
        {

            Random random = new Random();

            //ICipher cipher = new XOR();

            ////string ptxt = "HEllo, nice to MEET YOU! What is Your Name?";
            ////Console.WriteLine(ptxt);
            ////string ctxt = cipher.Encrypt(ptxt, Encoding.UTF8.GetBytes("XA"));
            ////Console.WriteLine(ctxt);
            ////string dcrypt = cipher.Decrypt(ctxt, Encoding.UTF8.GetBytes("XA"));
            ////Console.WriteLine(dcrypt);
            ////Console.ReadKey();

            //string ciphertext = "604<<<<2r7 :=1!u☻0 <197&r6  !=71r►1:<:?0&';63u&'3;!2 0!&71r;;2:!?4 0!u49=:606u1=''1=%:?0<u! 01'06u4 (/+u&''06u&0 ';!='+u1:>93%!06u<:&6:u!4<1+u 05 >4 <&<7&r% :801!;#7u! \"0 %=&;;5u8<<2><<2r<?% 41!;639>,r8'&197&r<<6=8\"'7=7;!<097u!  #7,r&<4&6:0!u▼:7;r☼;:<&r0*!7'?<<4&<=;r6=' :60r <<# 7u☺=74r&39'!3!;:<u=7801!='r4!&;1':'&>,r73' 0>u44<;;;5u\"4&0<!7'!u1:';!0>971r\"7< 1r!;97u! \"%='&4097u6'=%\"<<2!u50>4&<<:'&r?'8\",r>;9=\"3!&u!!37006u64?450 &r% :\":!06u%'=;506u↨>!! :?u&=='<&r6=;$0<!;:<&r?7\"79r←='&= :\"u1:>:<<(0r♣";
            
            ////Console.WriteLine(cipher.Decrypt(ciphertext, Encoding.UTF8.GetBytes("RU")));
            ////Console.ReadKey();

            //ICryptanalysis cryptanalysis = new XORCryptanalysis();
            //foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            //{
            //    Console.WriteLine(Encoding.UTF8.GetString(key));
            //    Console.WriteLine(cipher.Decrypt(ciphertext, key));
            //    Console.ReadKey();
            //}
            

            bool cont = true;

            while (cont)
            {
                string[] choices = {
                    "Test Classifiers",
                    "Demonstrate success",
                    "View classifier test results",
                    "View demonstration results",
                    "Exit",
                };
                string mode = UI.GetChoice(choices, "Choose a mode: ");


                //Produce an IConfig object depending on the mode chosen and any parameters requiring entry
                //Use a runner to run the chosen function
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

                        string[] testsInfo = UI.GetChoices(
                            File.ReadAllLines(TESTFILEPATH).Reverse().ToArray(),
                            "Choose files to view (Click Done when all have been selected): "
                        );
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

                        string[] demoInfo = UI.GetChoices(
                            File.ReadAllLines(DEMOFILEPATH).Reverse().ToArray(),
                            "Choose files to view (Click Done when all have been selected): "
                        );
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

        }
    }
}
