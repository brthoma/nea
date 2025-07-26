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

    public interface IResultsStore
    {
        void SaveResults(IConfiguration config, double[] values, bool[] trueValues);
        (double[], bool[]) GetResults(string filePath);
    }


    public class TestResultsStore : IResultsStore
    {

        public void SaveResults(IConfiguration config, double[] values, bool[] trueValues)
        {
            using (StreamWriter sw = new StreamWriter(config.GetStr("filePath")))
            {
                sw.WriteLine($"{config.GetStr("cipher")}|{config.GetStr("classifier")}|{config.GetInt("textLength")}|{config.GetInt("iterations")}");
                foreach (double i in values) sw.Write(i + "|");
                sw.Write("\n");
                foreach (bool trueValue in trueValues) sw.Write(trueValue + "|");
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

    }
    
    public class DemoResultsStore
    {
        public void SaveResults(IConfiguration config, bool[] success)
        {
            using(StreamWriter sw = new StreamWriter(config.GetStr("filePath")))
            {
                sw.WriteLine($"{config.GetStr("cipher")}|{config.GetStr("classifier")}|{config.GetInt("textLength")}|{config.GetInt("iterations")}|{config.GetDouble("threshold")}");
                foreach (bool i in success) sw.Write(i + "|");
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
    }


}
