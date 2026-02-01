using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom;
using Newtonsoft.Json.Linq;

namespace nea
{


    public class TestResultsHandler
    {

        public const string TESTFILES = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\TestFiles.txt";

        public void SaveResults(IConfiguration config, double[] values, bool[] trueValues)
        {
            using (StreamWriter sw = new StreamWriter(config.GetStr("filePath")))
            {
                sw.WriteLine($"{config.GetInt("textLength")}|{config.GetInt("iterations")}|{config.GetStr("dataGenerator")}|{config.GetStr("cipher")}|{config.GetStr("classifier")}");
                foreach (double i in values) sw.Write(i + "|");
                sw.Write("\n");
                foreach (bool trueValue in trueValues) sw.Write(trueValue + "|");
                sw.Close();
            }
            using (StreamWriter sw = new StreamWriter(TESTFILES, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " | " + config.GetStr("filePath"));
                sw.Close();
            }
        }

        public (double[], bool[]) GetResults(string filePath)
        {
            string[] strNums;
            string[] strTrues;
            using (StreamReader sr = new StreamReader(filePath))
            {
                sr.ReadLine();
                strNums = sr.ReadLine().Trim('|').Split('|');
                strTrues = sr.ReadLine().Trim('|').Split('|');
                sr.Close();
            }

            double[] results = new double[strNums.Length];
            bool[] trueValues = new bool[strTrues.Length];
            for (int i = 0; i < strNums.Length; i++)
            {
                results[i] = double.Parse(strNums[i]);
                trueValues[i] = bool.Parse(strTrues[i]);
            }

            return (results, trueValues);
        }

        public IConfiguration GetConfiguration(string filePath)
        {
            string[] configInfo;
            using (StreamReader sr = new StreamReader(filePath))
            {
                configInfo = sr.ReadLine().Trim('|').Split('|');
                sr.Close();
            }
            int textLength = int.Parse(configInfo[0]);
            int iterations = int.Parse(configInfo[1]);
            string dataGenerator = configInfo[2];
            string cipher = configInfo[3];
            string classifier = configInfo[4];

            TestConfiguration config = new TestConfiguration(filePath, textLength, iterations, dataGenerator, cipher, classifier);

            return config;
        }

    }
    
    public class DemoResultsHandler
    {
        public const string DEMOFILES = "C:\\Users\\betha\\Code\\nea\\FilesForUse\\DemoFiles.txt";

        public void SaveResults(IConfiguration config, bool[] success)
        {
            using(StreamWriter sw = new StreamWriter(config.GetStr("filePath")))
            {
                sw.WriteLine($"{config.GetInt("textLength")}|{config.GetInt("iterations")}|{config.GetDouble("threshold")}|{config.GetStr("dataGenerator")}|{config.GetStr("cipher")}|{config.GetStr("classifier")}|{config.GetStr("cryptanalysis")}");
                foreach (bool i in success) sw.Write(i + "|");
                sw.Close();
            }

            using(StreamWriter sw = new StreamWriter(DEMOFILES, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " | " + config.GetStr("filePath"));
                sw.Close();
            }
        }

        public bool[] GetResults(string filePath)
        {
            string[] strSuccess;
            using(StreamReader sr = new StreamReader(filePath))
            {
                sr.ReadLine();
                strSuccess = sr.ReadLine().Trim('|').Split('|');
            }

            bool[] success = new bool[strSuccess.Length];
            for (int i = 0; i < strSuccess.Length; i++)
            {
                success[i] = bool.Parse(strSuccess[i]);
            }

            return success;
        }

        public IConfiguration GetConfiguration(string filePath)
        {
            string[] configInfo;
            using (StreamReader sr = new StreamReader(filePath))
            {
                configInfo = sr.ReadLine().Trim('|').Split('|');
                sr.Close();
            }
            int textLength = int.Parse(configInfo[0]);
            int iterations = int.Parse(configInfo[1]);
            double threshold = double.Parse(configInfo[2]);
            string dataGenerator = configInfo[3];
            string cipher = configInfo[4];
            string classifier = configInfo[5];
            string cryptanalysis = configInfo[6];

            DemoConfiguration config = new DemoConfiguration(filePath, textLength, iterations, threshold, dataGenerator, cipher, classifier, cryptanalysis);

            return config;
        }

    }


}
