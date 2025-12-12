using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{

    /* Interface definition for a Configuration class
     * Derived classes return the test/demo parameters set at configuration
     */
    public interface IConfiguration
    {
        string GetStr(string property);
        int GetInt(string property);
        double GetDouble(string property);
    }
    

    public class TestConfiguration : IConfiguration
    {

        private string filePath, dataGenerator, cipher, classifier;
        private int textLength, iterations;

        public TestConfiguration()
        {
            filePath = "Data\\TestData\\" + UI.GetStringInput("Enter file name: ") + ".txt";

            textLength = UI.GetIntInput("Enter text length: ");

            iterations = UI.GetIntInput("Enter number of iterations: ");

            dataGenerator = UI.GetChoice(new string[] {"WordsFromDict", "TextFromCorpus" }, "Choose data generator: ");

            cipher = UI.GetChoice(new string[] { "XOR", "ROT47", "ROT13", "Vigenere", "Substitution" }, "Choose cipher: ");

            classifier = UI.GetChoice(new string[] { "RandomGuesser", "ProportionPrintable", "DictionaryLookup", "FrequencyAnalysis", "Bigrams", "WordLength", "Entropy", "MajorityVoteEnsemble" }, "Choose classifier: ");
        }

        public TestConfiguration(string filePath, int textLength, int iterations, string dataGenerator, string cipher, string classifier)
        {
            this.filePath = filePath;
            this.textLength = textLength;
            this.iterations = iterations;
            this.dataGenerator = dataGenerator;
            this.cipher = cipher;
            this.classifier = classifier;
        }

        public string GetStr(string property)
        {
            switch (property)
            {
                case "filePath":
                    return filePath;
                case "dataGenerator":
                    return dataGenerator;
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
        private string filePath, dataGenerator, cipher, classifier, cryptanalysis;
        private int textLength, iterations;
        private double threshold;

        public DemoConfiguration()
        {
            filePath = "Data\\DemoData\\" + UI.GetStringInput("Enter file name: ") + ".txt";

            textLength = UI.GetIntInput("Enter text length: ");

            iterations = UI.GetIntInput("Enter number of iterations: ");

            threshold = UI.GetDoubleInput("Enter classifier threshold to use: ");

            dataGenerator = UI.GetChoice(new string[] { "WordsFromDict", "TextFromCorpus" }, "Choose data generator: ");

            cipher = UI.GetChoice(new string[] { "XOR", "ROT47", "ROT13", "Vigenere", "Substitution" }, "Choose cipher: ");

            classifier = UI.GetChoice(new string[] { "RandomGuesser", "ProportionPrintable", "DictionaryLookup", "FrequencyAnalysis", "Bigrams", "WordLength", "Entropy", "MajorityVoteEnsemble" }, "Choose classifier: ");

            string cryptanalysisMessage = "Choose cryptanalysis method: ";
            switch (cipher)
            {
                case "XOR":
                    cryptanalysis = UI.GetChoice(new string[] { "XORCryptanalysis" }, cryptanalysisMessage);
                    break;
                case "ROT47":
                    cryptanalysis = UI.GetChoice(new string[] {"ROT47Cryptanalysis"}, cryptanalysisMessage);
                    break;
                case "ROT13":
                    cryptanalysis = UI.GetChoice(new string[] { "ROT13Cryptanalysis", "FasterROT13Cryptanalysis" }, cryptanalysisMessage);
                    break;
                case "Vigenere":
                    cryptanalysis = UI.GetChoice(new string[] { "VigenereCryptanalysis" }, cryptanalysisMessage);
                    break;
                case "Substitution":
                    cryptanalysis = UI.GetChoice(new string[] { "SubstitutionCryptanalysis", "FasterSubstitutionCryptanalysis" }, cryptanalysisMessage);
                    break;
            }
        }

        public DemoConfiguration(string filePath, int textLength, int iterations, double threshold, string dataGenerator, string cipher, string classifier, string cryptanalysis)
        {
            this.filePath = filePath;
            this.textLength = textLength;
            this.iterations = iterations;
            this.threshold = threshold;
            this.dataGenerator = dataGenerator;
            this.cipher = cipher;
            this.classifier = classifier;
            this.cryptanalysis = cryptanalysis;
        }

        public string GetStr(string property)
        {
            switch (property)
            {
                case "filePath":
                    return filePath;
                case "dataGenerator":
                    return dataGenerator;
                case "cipher":
                    return cipher;
                case "classifier":
                    return classifier;
                case "cryptanalysis":
                    return cryptanalysis;
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
