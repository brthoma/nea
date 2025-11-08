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

            //ICryptanalysis cryptanalysis = new ROT47Cryptanalysis();
            //IClassifier classifier = new FrequencyAnalysis();
            //ICipher cipher = new ROT47();
            //string ciphertext = "}j}}xx| vn||ru$ oj~u}nm U~}qn{jw lxvvxwyujln !xuun$kjuu ux~mu$ mn|rpwj}rwp lurlt }rn| ouxy {ny{n||n| Lxvjwlqn {no~nu| Krmmun |l{jv wj~pq}$ Ux\"n wxvnwluj} ~{ n lr} rn| sn|} nm v~lt sx${rmn mrjuxp| rvvx{ju Ur!n{vx{n ox~{| xvn vx{ yqr | v | m{ rwtrwp | ~yn{ kuxlt | qx~} rwp \"nj{| bj~wmn xvnw| mnmrlj}rwp oxxu| mn!ru| n#r}| Lqjvyujrw j||~vy}rxw| twnuu| |z~njtrwp |qx!nunm lxvyj{jku$ xyjz~nwn|| vrl{xs~vy rw|}{~vnw}jur|} rmnjur%j}rxw rw!ju~jkun ujvy knur}}un| n$n|rpq} sxl~wm {n|ynl}o~uu$ kr|v~}q M~pjw J|}j{}n Kjuuj{m";
            //    foreach (byte[] key in cryptanalysis.GetKeys(ciphertext))
            //    {
            //        string possiblePlaintext = cipher.Decrypt(ciphertext, key);
            //        Console.WriteLine(possiblePlaintext);
            //        Console.WriteLine();
            //        Console.ReadKey();
            //    }
            //    return;



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
