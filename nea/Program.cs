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
        private const string TESTFILEPATH = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\TestFiles.txt";
        private const string DEMOFILEPATH = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\DemoFiles.txt";


        /* Main menu: Choose mode and get configuration to pass into runners
         */

        static void Main(string[] args)
        {

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


                /* Produces an IConfig object depending on the mode chosen and any parameters requiring entry
                 * Use a runner to run the chosen function
                 */
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


        }
    }
}
