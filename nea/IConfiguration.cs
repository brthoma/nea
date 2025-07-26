using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{

    public interface IConfiguration
    {
        string GetStr(string property);
        int GetInt(string property);
        double GetDouble(string property);
    }
    

    public class TestConfiguration : IConfiguration
    {

        private string filePath, cipher, classifier;
        private int textLength, iterations;

        public TestConfiguration()
        {
            Console.Write("Enter file name: ");
            filePath = Console.ReadLine() + ".txt";

            Console.Write("Enter text length: ");
            textLength = int.Parse(Console.ReadLine());

            Console.Write("Enter number of iterations: ");
            iterations = int.Parse(Console.ReadLine());

            Console.Write("Enter cipher: ");
            cipher = Console.ReadLine();

            Console.Write("Enter classifier: ");
            classifier = Console.ReadLine();
        }

        public string GetStr(string property)
        {
            switch (property)
            {
                case "filePath":
                    return filePath;
                case "cipher":
                    return cipher;
                case "classifier":
                    return classifier;
                default:
                    throw new Exception($"No such string property as '{property}'");
            }
        }

        public int GetInt(string property)
        {
            switch (property)
            {
                case "textLength":
                    return textLength;
                case "iterations":
                    return iterations;
                default:
                    throw new Exception($"No such int property as '{property}'");
            }
        }

        public double GetDouble(string property)
        {
            switch (property)
            {
                default:
                    throw new Exception($"No such double property as '{property}'");
            }
        }
    }

    public class DemoConfiguration : IConfiguration
    {
        private string filePath, cipher, classifier;
        private int textLength, iterations;
        private double threshold;

        public DemoConfiguration()
        {
            Console.Write("Enter file name: ");
            filePath = Console.ReadLine() + ".txt";

            Console.Write("Enter text length: ");
            textLength = int.Parse(Console.ReadLine());

            Console.Write("Enter number of iterations: ");
            iterations = int.Parse(Console.ReadLine());

            Console.Write("Enter threshold: ");
            threshold = double.Parse(Console.ReadLine());

            Console.Write("Enter cipher: ");
            cipher = Console.ReadLine();

            Console.Write("Enter classifier: ");
            classifier = Console.ReadLine();
        }

        public string GetStr(string property)
        {
            switch (property)
            {
                case "filePath":
                    return filePath;
                case "cipher":
                    return cipher;
                case "classifier":
                    return classifier;
                default:
                    throw new Exception($"No such string property as '{property}'");
            }
        }

        public int GetInt(string property)
        {
            switch (property)
            {
                case "textLength":
                    return textLength;
                case "iterations":
                    return iterations;
                default:
                    throw new Exception($"No such int property as '{property}'");
            }
        }

        public double GetDouble(string property)
        {
            switch (property)
            {
                case "threshold":
                    return threshold;
                default:
                    throw new Exception($"No such double property as '{property}'");
            }
        }

    }


}
